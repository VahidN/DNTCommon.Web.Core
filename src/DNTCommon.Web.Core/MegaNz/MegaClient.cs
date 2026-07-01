using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Org.BouncyCastle.Security;

namespace DNTCommon.Web.Core;

/// <summary>
///     Minimal MEGA client implementation
/// </summary>
public sealed class MegaClient(HttpClient httpClient)
{
    private const string ApplicationKey = "axhQiYyQ";
    private static readonly Uri ApiUri = new(uriString: "https://g.api.mega.co.nz/cs");

    private readonly List<MegaNode> _nodes = [];
    private byte[]? _masterKey;
    private uint _sequence = (uint)(uint.MaxValue * Random.Shared.NextDouble());
    private string? _sessionId;

    /// <summary>
    ///     Whether the client has an active session.
    /// </summary>
    public bool IsLoggedIn => _sessionId is not null;

    /// <summary>
    ///     Logs in using email and password.
    /// </summary>
    public async Task LoginAsync(string? email, string? password, CancellationToken cancellationToken = default)
    {
        if (email.IsEmpty())
        {
            throw new ArgumentNullException(nameof(email));
        }

        if (password.IsEmpty())
        {
            throw new ArgumentNullException(nameof(password));
        }

        if (IsLoggedIn)
        {
            return;
        }

        var preLoginResponse = await RequestAsync<MegaPreLoginResponse>(new MegaPreLoginRequest
                               {
                                   User = email
                               }, hashcash: null, cancellationToken) ??
                               throw new InvalidOperationException(message: "PreLoginResponse is null");

        byte[] passwordAesKey;
        string hash;

        switch (preLoginResponse.Version)
        {
            case 1:
                passwordAesKey = password.ToBytesPassword().PrepareKey();
                hash = email.ToLowerInvariant().GenerateHash(passwordAesKey);

                break;
            case 2 when !string.IsNullOrEmpty(preLoginResponse.Salt):

                var passwordBytes = password.ToBytesPassword();
                var saltBytes = preLoginResponse.Salt.FromBase64Url();
                var derivedKeyBytes = new byte[32];

                Rfc2898DeriveBytes.Pbkdf2(passwordBytes, saltBytes, derivedKeyBytes, iterations: 100000,
                    HashAlgorithmName.SHA512);

                hash = derivedKeyBytes.Skip(count: 16).ToArray().ToBase64Url();
                passwordAesKey = [..derivedKeyBytes.Take(count: 16)];

                break;

            default:
                throw new NotSupportedException(string.Create(CultureInfo.InvariantCulture,
                    $"Unsupported account version {preLoginResponse.Version}."));
        }

        var login = await RequestAsync<MegaLoginResponse>(new MegaLoginRequest
        {
            User = email,
            PasswordHash = hash
        }, hashcash: null, cancellationToken) ?? throw new InvalidOperationException(message: "login is null");

        var masterKeyEnc = login.MasterKey?.FromBase64Url() ??
                           throw new InvalidKeyException(message: "masterKeyEnc is null");

        var masterKey = MegaCrypto.DecryptKey(masterKeyEnc, passwordAesKey);

        var encodedRsaPrivateKey = login.PrivateKey?.FromBase64Url() ??
                                   throw new InvalidKeyException(message: "encodedRsaPrivateKey is null");

        var rsaPriv = MegaCrypto.GetRsaPrivateKeyComponents(encodedRsaPrivateKey, masterKey);
        var sessionMpi = (login.SessionId?.FromBase64Url()).FromMpiNumber();
        var sessionBytes = sessionMpi.RsaDecrypt(rsaPriv[0], rsaPriv[1], rsaPriv[2]);

        _sessionId = sessionBytes.Take(count: 43).ToArray().ToBase64Url();
        _masterKey = masterKey;
    }

    /// <summary>
    ///     Logs out and clears the current session.
    /// </summary>
    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        if (!IsLoggedIn)
        {
            return;
        }

        try
        {
            try
            {
                await RequestAsync<string>(new MegaLogoutRequest(), hashcash: null, cancellationToken);
            }
            catch
            {
                // The local session is still cleared below.
            }
        }
        finally
        {
            _sessionId = null;
            _masterKey = null;
        }
    }

    /// <summary>
    ///     Retrieves the node tree (files/folders) for the current account.
    /// </summary>
    public async Task<IReadOnlyList<MegaNode>> GetNodesAsync(CancellationToken cancellationToken = default)
    {
        EnsureLoggedIn();
        var root = await RequestElementAsync(new MegaGetNodesRequest(), hashcash: null, cancellationToken);

        return ParseNodes(root);
    }

    /// <summary>
    ///     Creates a folder under the given parent node id.
    /// </summary>
    public async Task<MegaNode> CreateFolderAsync(string? parentId,
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(name);

        EnsureLoggedIn();

        var folderKey = MegaCrypto.CreateAesKey();

        var attrs = MegaCrypto.EncryptAttributes(new MegaAttributes
            {
                Name = name
            }, folderKey)
            .ToBase64Url();

        if (_masterKey is null)
        {
            throw new InvalidKeyException(message: "masterKeyEnc is null");
        }

        var encKey = folderKey.EncryptAes(_masterKey).ToBase64Url();

        var req = new MegaCreateNodeRequest
        {
            ParentId = parentId,
            Nodes =
            [
                new MegaCreateNodeData
                {
                    CompletionHandle = "xxxxxxxx",
                    Type = (int)MegaNodeType.Directory,
                    Attributes = attrs,
                    Key = encKey
                }
            ]
        };

        var resp = await RequestElementAsync(req, hashcash: null, cancellationToken);

        return ParseNodes(resp).Single();
    }

    /// <summary>
    ///     Uploads a stream as a file under the given parent node id.
    /// </summary>
    public async Task<MegaNode> UploadAsync(string? parentId,
        string name,
        Stream stream,
        DateTime? modificationDate,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(stream);

        EnsureLoggedIn();

        var uploadUrl = await RequestAsync<MegaUploadUrlResponse>(new MegaUploadUrlRequest
                        {
                            Size = stream.Length
                        }, hashcash: null, cancellationToken) ??
                        throw new InvalidOperationException(message: "UploadUrlResponse is null");

        string? completionHandle = null;

        await using var crypter = new MegaAesCtrStreamCrypter(stream);

        var total = stream.Length;
        var sent = 0L;

        foreach (var chunkSize in ComputeChunkSizes(crypter.ChunksPositions, total))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var buffer = new byte[chunkSize];
            var read = 0;

            while (read < chunkSize)
            {
                var r = await crypter.ReadAsync(buffer.AsMemory(read, chunkSize - read), cancellationToken);

                if (r == 0)
                {
                    break;
                }

                read += r;
            }

            await using var ms = new MemoryStream(buffer, index: 0, read, writable: false);
            var url = new Uri(string.Create(CultureInfo.InvariantCulture, $"{uploadUrl.Url}/{sent}"));
            sent += read;

            completionHandle = await PostRawAsync(url, ms, cancellationToken);
            progress?.Report((double)sent / total * 100d);
        }

        if (completionHandle.IsEmpty())
        {
            throw new InvalidOperationException(message: "Upload did not return completion handle.");
        }

        var attrs = MegaAttributes.Create(name, stream, modificationDate);
        var encAttrs = MegaCrypto.EncryptAttributes(attrs, crypter.FileKeyBytes).ToBase64Url();

        var fileKey = new byte[32];

        for (var i = 0; i < 8; i++)
        {
            fileKey[i] = (byte)(crypter.FileKeyBytes[i] ^ crypter.IvBytes[i]);
            fileKey[i + 16] = crypter.IvBytes[i];
        }

        for (var i = 8; i < 16; i++)
        {
            fileKey[i] = (byte)(crypter.FileKeyBytes[i] ^ crypter.ComputedMetaMac[i - 8]);
            fileKey[i + 16] = crypter.ComputedMetaMac[i - 8];
        }

        if (_masterKey is null)
        {
            throw new InvalidKeyException(message: "masterKeyEnc is null");
        }

        var encKey = MegaCrypto.EncryptKey(fileKey, _masterKey).ToBase64Url();

        var req = new MegaCreateNodeRequest
        {
            ParentId = parentId,
            Nodes =
            [
                new MegaCreateNodeData
                {
                    CompletionHandle = completionHandle,
                    Type = (int)MegaNodeType.File,
                    Attributes = encAttrs,
                    Key = encKey
                }
            ]
        };

        var resp = await RequestElementAsync(req, hashcash: null, cancellationToken);

        return ParseNodes(resp).Single();
    }

    /// <summary>
    ///     Creates a public link (export) for a node and returns the URL.
    /// </summary>
    public async Task<string> PublishAsync(MegaNode megaNode, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(megaNode);
        EnsureLoggedIn();

        if (megaNode.Type is not MegaNodeType.File and not MegaNodeType.Directory)
        {
            throw new ArgumentException(message: "Only file and directory nodes can be published.", nameof(megaNode));
        }

        if (megaNode.NodeKey is null || megaNode.NodeKey.Length == 0)
        {
            throw new InvalidOperationException(message: "Node key is missing.");
        }

        var el = await RequestElementAsync(new MegaExportLinkRequest
        {
            NodeId = megaNode.Id
        }, hashcash: null, cancellationToken);

        string? publicHandle;

        if (el.ValueKind == JsonValueKind.String)
        {
            publicHandle = el.GetString();
        }
        else if (el.ValueKind == JsonValueKind.Object && el.TryGetProperty(propertyName: "ph", out var ph))
        {
            publicHandle = ph.GetString();
        }
        else
        {
            publicHandle = null;
        }

        if (publicHandle.IsEmpty())
        {
            throw new InvalidOperationException(
                $"Export link response is missing public handle: {TrimForError(el.GetRawText())}");
        }

        var key = megaNode.NodeKey.ToBase64Url();
        var kind = megaNode.Type == MegaNodeType.Directory ? "folder" : "file";

        return $"https://mega.nz/{kind}/{publicHandle}#{key}";
    }

    /// <summary>
    ///     Removes a public link (export) for a node.
    /// </summary>
    public async Task UnpublishAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        if (nodeId.IsEmpty())
        {
            throw new ArgumentNullException(nameof(nodeId));
        }

        EnsureLoggedIn();

        await RequestAsync<string>(new MegaExportLinkRequest
        {
            NodeId = nodeId,
            Disable = 1
        }, hashcash: null, cancellationToken);
    }

    /// <summary>
    ///     Downloads a file node into the destination stream.
    /// </summary>
    public async Task DownloadAsync(MegaNode megaNode,
        Stream destination,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(megaNode);
        ArgumentNullException.ThrowIfNull(destination);
        EnsureLoggedIn();

        if (megaNode.Type != MegaNodeType.File)
        {
            throw new ArgumentException(message: "Only file nodes can be downloaded.", nameof(megaNode));
        }

        var dl = await RequestAsync<MegaDownloadUrlResponse>(new MegaDownloadUrlRequest
        {
            Id = megaNode.Id
        }, hashcash: null, cancellationToken);

        if (dl is null || dl.Url is null)
        {
            throw new InvalidOperationException(message: "DownloadUrlResponse is null");
        }

        await using var raw = await httpClient.GetStreamAsync(new Uri(dl.Url), cancellationToken);

        await using var decrypt =
            new MegaAesCtrStreamDecrypter(raw, dl.Size, megaNode.FileKey, megaNode.Iv, megaNode.MetaMac);

        var buffer = new byte[64 * 1024];
        long readTotal = 0;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var r = await decrypt.ReadAsync(buffer.AsMemory(start: 0, buffer.Length), cancellationToken);

            if (r == 0)
            {
                break;
            }

            await destination.WriteAsync(buffer.AsMemory(start: 0, r), cancellationToken);
            readTotal += r;
            progress?.Report((double)readTotal / dl.Size * 100d);
        }
    }

    /// <summary>
    ///     Resolves a public file handle (export link) to a direct download URL.
    /// </summary>
    public Task<MegaDownloadUrlResponse?> GetPublicDownloadUrlAsync(string publicHandle,
        CancellationToken cancellationToken = default)
    {
        if (publicHandle.IsEmpty())
        {
            throw new ArgumentNullException(nameof(publicHandle));
        }

        return RequestAsync<MegaDownloadUrlResponse?>(new MegaDownloadUrlRequest
        {
            PublicHandle = publicHandle
        }, hashcash: null, cancellationToken);
    }

    /// <summary>
    ///     Deletes a node by its id.
    /// </summary>
    public async Task DeleteAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        EnsureLoggedIn();

        await RequestAsync<string>(new MegaDeleteRequest
        {
            NodeId = nodeId
        }, hashcash: null, cancellationToken);
    }

    private void EnsureLoggedIn()
    {
        if (!IsLoggedIn)
        {
            throw new InvalidOperationException(message: "Not logged in.");
        }
    }

    private async Task<string> PostRawAsync(Uri url, Stream data, CancellationToken cancellationToken = default)
    {
        using var content = new StreamContent(data);
        content.Headers.ContentType = new MediaTypeHeaderValue(mediaType: "application/octet-stream");

        using var resp = await httpClient.PostAsync(url, content, cancellationToken);
        await resp.EnsureSuccessStatusCodeAsync(cancellationToken);

        return (await resp.Content.ReadAsStringAsync(cancellationToken)).Trim();
    }

    private Uri BuildApiUrl(string requestId, Dictionary<string, string>? extraQuery)
    {
        var q = new Dictionary<string, string>(extraQuery ?? new Dictionary<string, string>())
        {
            [key: "id"] = requestId,
            [key: "ak"] = ApplicationKey
        };

        if (_sessionId is not null)
        {
            q[key: "sid"] = _sessionId;
        }

        var sb = new StringBuilder();

        foreach (var kv in q)
        {
            sb.Append(kv.Key).Append(value: '=').Append(kv.Value).Append(value: '&');
        }

        sb.Length--;

        return new UriBuilder(ApiUri)
        {
            Query = sb.ToString()
        }.Uri;
    }

    private async Task<JsonElement> RequestElementAsync(MegaRequest request,
        string? hashcash,
        CancellationToken cancellationToken = default)
    {
        var requestId = (_sequence++ % uint.MaxValue).ToString(CultureInfo.InvariantCulture);
        request.RequestId = requestId;

        var payload = JsonSerializer.Serialize(new object[]
        {
            request
        });

        var attempt = 0;
        const int MaxAttempts = 10;

        while (true)
        {
            var url = BuildApiUrl(requestId, hashcash is null
                ? null
                : new Dictionary<string, string>
                {
                    [key: "hashcash"] = hashcash
                });

            try
            {
                using var content = new StringContent(payload, Encoding.UTF8, mediaType: "application/json");

                using var resp = await httpClient.PostAsync(url, content, cancellationToken);
                await resp.EnsureSuccessStatusCodeAsync(cancellationToken);

                var text = await resp.Content.ReadAsStringAsync(cancellationToken);

                if (text.IsEmpty())
                {
                    attempt++;

                    if (attempt >= MaxAttempts)
                    {
                        throw new InvalidOperationException(message: "Empty API response.");
                    }

                    await Task.Delay(GetRetryDelay(errorCode: null, attempt), cancellationToken);

                    continue;
                }

                using var doc = JsonDocument.Parse(text);
                var root = doc.RootElement;

                switch (root.ValueKind)
                {
                    case JsonValueKind.Number:
                        var rawCode = root.GetInt32();
                        var errorCode = (MegaErrorCode)rawCode;

                        if (errorCode == MegaErrorCode.Ok)
                        {
                            return root.Clone();
                        }

                        if (IsRetryableErrorCode(errorCode))
                        {
                            attempt++;

                            if (attempt >= MaxAttempts)
                            {
                                throw new MegaApiException(rawCode);
                            }

                            await Task.Delay(GetRetryDelay(errorCode, attempt), cancellationToken);

                            continue;
                        }

                        throw new MegaApiException(rawCode);
                    case JsonValueKind.Object:
                        return root.Clone();
                }

                if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                {
                    throw new InvalidOperationException($"Unexpected API response shape: {TrimForError(text)}");
                }

                var first = root[index: 0];

                if (first.ValueKind == JsonValueKind.Number)
                {
                    var rawCode = first.GetInt32();
                    var errorCode = (MegaErrorCode)rawCode;

                    if (errorCode == MegaErrorCode.Ok)
                    {
                        return first.Clone();
                    }

                    // Hashcash required: [-27, "<challenge>"]
                    if (errorCode == MegaErrorCode.HashcashRequired && root.GetArrayLength() >= 2)
                    {
                        var challenge = root[index: 1].GetString();
                        hashcash = MegaCrypto.GenerateHashcashToken(challenge);
                        attempt = 0;

                        continue;
                    }

                    if (IsRetryableErrorCode(errorCode))
                    {
                        attempt++;

                        if (attempt >= MaxAttempts)
                        {
                            throw new MegaApiException(rawCode);
                        }

                        await Task.Delay(GetRetryDelay(errorCode, attempt), cancellationToken);

                        continue;
                    }

                    throw new MegaApiException(rawCode);
                }

                return first.Clone();
            }
            catch (Exception ex) when (ex is HttpRequestException or IOException or JsonException)
            {
                attempt++;

                if (attempt >= MaxAttempts)
                {
                    throw;
                }

                await Task.Delay(GetRetryDelay(errorCode: null, attempt), cancellationToken);
            }
        }
    }

    private static bool IsRetryableErrorCode(MegaErrorCode code)
        => code is MegaErrorCode.Again or MegaErrorCode.RateLimit or MegaErrorCode.TempUnavailable;

    private static TimeSpan GetRetryDelay(MegaErrorCode? errorCode, int attempt)
    {
        var baseMs = errorCode == MegaErrorCode.RateLimit ? 1000 : 200;
        var maxMs = errorCode == MegaErrorCode.RateLimit ? 15000 : 3000;

        var ms = baseMs * Math.Pow(x: 2, Math.Min(val1: 6, Math.Max(val1: 0, attempt - 1)));
        ms = Math.Min(ms, maxMs);

        return TimeSpan.FromMilliseconds(ms);
    }

    private static string TrimForError(string? value)
    {
        if (value is null)
        {
            return "<null>";
        }

        value = value.Replace(oldValue: "\r", string.Empty, StringComparison.Ordinal)
            .Replace(oldValue: "\n", string.Empty, StringComparison.Ordinal);

        return value.Length <= 512 ? value : $"{value.AsSpan(start: 0, length: 512)}...";
    }

    private async Task<T?> RequestAsync<T>(MegaRequest request,
        string? hashcash,
        CancellationToken cancellationToken = default)
    {
        var el = await RequestElementAsync(request, hashcash, cancellationToken);

        if (typeof(T) == typeof(string))
        {
            return (T)(object)el.GetRawText().Trim(trimChar: '"');
        }

        return JsonSerializer.Deserialize<T?>(el.GetRawText());
    }

    private List<MegaNode> ParseNodes(JsonElement response)
    {
        // Response contains { "f": [...], "ok": [...] }
        if (!response.TryGetProperty(propertyName: "f", out var fJsonElement) ||
            fJsonElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException(message: "Missing nodes array.");
        }

        var sharedKeys = GetSharedKeys(response);

        var nodes = new List<MegaNode>();

        using var fJsonElementArrayEnumerator = fJsonElement.EnumerateArray();

        foreach (var n in fJsonElementArrayEnumerator)
        {
            var type = (MegaNodeType)n.GetProperty(propertyName: "t").GetInt32();

            if (type is not MegaNodeType.File and not MegaNodeType.Directory and not MegaNodeType.Root and
                not MegaNodeType.Trash and not MegaNodeType.Inbox)
            {
                continue;
            }

            var id = n.GetProperty(propertyName: "h").GetString();

            if (id.IsEmpty())
            {
                continue;
            }

            var parentId = n.TryGetProperty(propertyName: "p", out var p) ? p.GetString() : null;
            var size = n.TryGetProperty(propertyName: "s", out var s) ? s.GetInt64() : 0L;
            var ts = n.TryGetProperty(propertyName: "ts", out var tsv) ? tsv.GetInt64() : (long?)null;
            var publicHandle = n.TryGetProperty(propertyName: "ph", out var ph) ? ph.GetString() : null;

            string? name = null;
            DateTime? mod = null;
            byte[]? fileKey = null;
            byte[]? iv = null;
            byte[]? metaMac = null;
            byte[]? nodeKey = null;

            if (type is MegaNodeType.File or MegaNodeType.Directory)
            {
                if (!n.TryGetProperty(propertyName: "k", out var kprop))
                {
                    continue;
                }

                var keyText = kprop.GetString();

                if (keyText.IsEmpty())
                {
                    continue;
                }

                var keyPart = keyText.Split(separator: '/')[0];
                var colon = keyPart.IndexOf(value: ':', StringComparison.Ordinal);

                if (colon <= 0)
                {
                    continue;
                }

                var handle = keyPart.Substring(startIndex: 0, colon);
                var encKey = keyPart.Substring(colon + 1).FromBase64Url();

                if (_masterKey is null)
                {
                    throw new InvalidKeyException(message: "masterKeyEnc is null");
                }

                var nodeMasterKey = _masterKey;

                if (sharedKeys.TryGetValue(handle, out var shared))
                {
                    nodeMasterKey = MegaCrypto.DecryptKey(shared.FromBase64Url(), _masterKey);
                }

                var fullKey = MegaCrypto.DecryptKey(encKey, nodeMasterKey);
                nodeKey = fullKey;

                if (type == MegaNodeType.File)
                {
                    MegaCrypto.GetPartsFromDecryptedKey(fullKey, out iv, out metaMac, out fileKey);
                }

                if (n.TryGetProperty(propertyName: "a", out var aprop))
                {
                    var attrsEnc = aprop.GetString();

                    if (!attrsEnc.IsEmpty())
                    {
                        var key = (type == MegaNodeType.File ? fileKey : fullKey) ??
                                  throw new InvalidKeyException(message: "nodeKey is null");

                        var attrs = MegaCrypto.DecryptAttributes(attrsEnc.FromBase64Url(), key);

                        attrs?.HydrateAfterDeserialize();
                        name = attrs?.Name;
                        mod = attrs?.ModificationDate;
                    }
                }
            }

            nodes.Add(new MegaNode
            {
                Id = id,
                Type = type,
                ParentId = parentId,
                Name = name,
                PublicHandle = publicHandle,
                Size = size,
                CreationDate = ts is { } l ? l.FromEpochSeconds() : null,
                ModificationDate = mod,
                FileKey = fileKey,
                Iv = iv,
                MetaMac = metaMac,
                NodeKey = nodeKey
            });
        }

        return nodes;
    }

    private static Dictionary<string, string> GetSharedKeys(JsonElement response)
    {
        var sharedKeys = new Dictionary<string, string>(StringComparer.Ordinal);

        if (!response.TryGetProperty(propertyName: "ok", out var okJsonElement) ||
            okJsonElement.ValueKind != JsonValueKind.Array)
        {
            return sharedKeys;
        }

        using var okJsonElementArrayEnumerator = okJsonElement.EnumerateArray();

        foreach (var item in okJsonElementArrayEnumerator)
        {
            var id = item.GetProperty(propertyName: "h").GetString();
            var key = item.GetProperty(propertyName: "k").GetString();

            if (!id.IsEmpty() && !key.IsEmpty())
            {
                sharedKeys[id] = key;
            }
        }

        return sharedKeys;
    }

    private static IEnumerable<int> ComputeChunkSizes(long[] chunkPositions, long streamLength)
    {
        for (var i = 0; i < chunkPositions.Length; i++)
        {
            var start = chunkPositions[i];
            var end = i == chunkPositions.Length - 1 ? streamLength : chunkPositions[i + 1];

            yield return checked((int)(end - start));
        }
    }

    public async Task CreateFolderAsync(MegaItem? item, CancellationToken cancellationToken = default)
    {
        await FillDefaultNodesAsync(cancellationToken);

        var folders = new List<MegaItem>();

        do
        {
            if (item is not null)
            {
                folders.Add(item);
            }

            item = item?.Parent;
        }
        while (item is not null);

        folders.Reverse();

        var needCheck = true;
        var curr = GetRoot();

        foreach (var folder in folders)
        {
            var name = folder.Name;

            if (name.IsEmpty())
            {
                continue;
            }

            if (needCheck)
            {
                var found = GetChild(curr)
                    .FirstOrDefault(n => n.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);

                if (found is null)
                {
                    needCheck = false;
                    curr = await CreateFolderAsync(curr.Id, name, cancellationToken);
                    _nodes.Add(curr);
                }
                else
                {
                    curr = found;
                }
            }
            else
            {
                curr = await CreateFolderAsync(curr.Id, name, cancellationToken);
                _nodes.Add(curr);
            }
        }
    }

    public async Task DeleteAsync(MegaItem item, CancellationToken cancellationToken = default)
    {
        await FillDefaultNodesAsync(cancellationToken);

        var node = FindItem(item);

        if (node is null)
        {
            return;
        }

        await DeleteAsync(node.Id, cancellationToken);
        _nodes.Remove(node);
    }

    public async Task DownloadAsync(MegaItem item,
        Stream stream,
        long? offset,
        long? length,
        Action<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(stream);

        if (offset is not null || length is not null)
        {
            throw new NotSupportedException(message: "Partial download is not supported.");
        }

        await FillDefaultNodesAsync(cancellationToken);

        var node = FindItem(item) ?? throw new ArgumentOutOfRangeException(nameof(item), message: "Entry not found.");

        await DownloadAsync(node, stream, new ProgressHandler(progress), cancellationToken);
    }

    public async Task UploadAsync(MegaItem item,
        Stream stream,
        Action<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(item.Name);
        ArgumentNullException.ThrowIfNull(stream);

        await FillDefaultNodesAsync(cancellationToken);

        var parent = (item.Parent is null ? GetRoot() : FindItem(item.Parent)) ??
                     throw new ArgumentOutOfRangeException(nameof(item), message: "Parent entry not found.");

        var node = await UploadAsync(parent.Id, item.Name, stream, modificationDate: null,
            new ProgressHandler(progress), cancellationToken);

        _nodes.Add(node);
    }

    public async Task FillInfoAsync(MegaItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        await FillDefaultNodesAsync(cancellationToken);

        var node = FindItem(item) ?? throw new ArgumentOutOfRangeException(nameof(item), message: "Entry not found.");

        item.LastModified = node.ModificationDate ?? node.CreationDate ?? default;
        item.Size = node.Size;
    }

    public IAsyncEnumerable<MegaItem> FindAsync(MegaItem parent, string criteria) => FindAsyncImpl(parent, criteria);

    public async Task<string> PublishAsync(MegaItem item,
        TimeSpan? expiresIn,
        CancellationToken cancellationToken = default)
    {
        if (expiresIn is not null)
        {
            throw new NotSupportedException(message: "Expiring links are not supported by MEGA export links.");
        }

        await FillDefaultNodesAsync(cancellationToken);

        var node = FindItem(item) ?? throw new ArgumentOutOfRangeException(nameof(item), message: "Entry not found.");

        var url = await PublishAsync(node, cancellationToken);

        _nodes.Clear();
        _nodes.AddRange(await GetNodesAsync(cancellationToken));

        return url;
    }

    public async Task UnPublishAsync(MegaItem item, CancellationToken cancellationToken = default)
    {
        await FillDefaultNodesAsync(cancellationToken);

        var node = FindItem(item);

        if (node is null)
        {
            return;
        }

        await UnpublishAsync(node.Id, cancellationToken);

        _nodes.Clear();
        _nodes.AddRange(await GetNodesAsync(cancellationToken));
    }

    public async Task<IList<MegaNode>> FillDefaultNodesAsync(CancellationToken cancellationToken = default)
    {
        EnsureLoggedIn();

        _nodes.AddRange(await GetNodesAsync(cancellationToken));

        return _nodes;
    }

    private MegaNode? FindItem(MegaItem? item)
    {
        if (item is null)
        {
            return null;
        }

        var folders = new List<MegaItem>();

        do
        {
            folders.Add(item);
            item = item.Parent;
        }
        while (item is not null);

        folders.Reverse();

        var curr = GetRoot();

        foreach (var folder in folders)
        {
            curr = GetChild(curr)
                .FirstOrDefault(n => n.Name?.Equals(folder.Name, StringComparison.OrdinalIgnoreCase) == true);

            if (curr is null)
            {
                break;
            }
        }

        return curr;
    }

    private MegaNode GetRoot() => _nodes.First(n => n.Type == MegaNodeType.Root);

    private IEnumerable<MegaNode> GetChild(MegaNode parent)
    {
        ArgumentNullException.ThrowIfNull(parent);

        return _nodes.Where(n => n.ParentId == parent.Id);
    }

    private async IAsyncEnumerable<MegaItem> FindAsyncImpl(MegaItem? parent,
        string criteria,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await FillDefaultNodesAsync(cancellationToken);

        var pe = parent is null ? GetRoot() : FindItem(parent);

        if (pe is null)
        {
            yield break;
        }

        var child = GetChild(pe);

        if (!criteria.IsEmpty())
        {
            child = child.Where(i => i.Name?.Contains(criteria, StringComparison.OrdinalIgnoreCase) == true);
        }

        foreach (var item in child)
        {
            yield return new MegaItem
            {
                Name = item.Name,
                Parent = parent,
                LastModified = item.ModificationDate ?? item.CreationDate ?? default,
                Size = item.Size
            };
        }
    }

    private sealed class ProgressHandler(Action<int>? progress) : IProgress<double>
    {
        void IProgress<double>.Report(double value) => progress?.Invoke((int)value);
    }
}
