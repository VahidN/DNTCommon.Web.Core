using System.Buffers.Binary;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace DNTCommon.Web.Core;

public static class MegaCrypto
{
    private static readonly byte[] AesZeroIv = new byte[16];

    public static byte[] CreateAesKey()
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.KeySize = 128;
        aes.Padding = PaddingMode.None;
        aes.GenerateKey();

        return aes.Key;
    }

    public static ICryptoTransform CreateAesEncryptor(this byte[] key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;

        return aes.CreateEncryptor(key, AesZeroIv);
    }

    public static byte[] EncryptAes(this byte[] data, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(key);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;
        using var enc = aes.CreateEncryptor(key, AesZeroIv);

        return enc.TransformFinalBlock(data, inputOffset: 0, data.Length);
    }

    public static byte[] EncryptAes(this byte[] data, ICryptoTransform encryptor)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(encryptor);

        return encryptor.TransformFinalBlock(data, inputOffset: 0, data.Length);
    }

    public static byte[] DecryptAes(this byte[] data, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(key);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;
        using var dec = aes.CreateDecryptor(key, AesZeroIv);

        return dec.TransformFinalBlock(data, inputOffset: 0, data.Length);
    }

    internal static byte[] EncryptKey(byte[] data, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(key);

        var output = new byte[data.Length];
        using var enc = key.CreateAesEncryptor();

        for (var i = 0; i < data.Length; i += 16)
        {
            var block = new byte[16];
            Array.Copy(data, i, block, destinationIndex: 0, length: 16);
            var encrypted = block.EncryptAes(enc);
            Array.Copy(encrypted, sourceIndex: 0, output, i, length: 16);
        }

        return output;
    }

    internal static byte[] DecryptKey(byte[] data, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(key);

        var output = new byte[data.Length];

        for (var i = 0; i < data.Length; i += 16)
        {
            var block = new byte[16];
            Array.Copy(data, i, block, destinationIndex: 0, length: 16);
            var decrypted = block.DecryptAes(key);
            Array.Copy(decrypted, sourceIndex: 0, output, i, length: 16);
        }

        return output;
    }

    internal static void GetPartsFromDecryptedKey(byte[] decryptedKey,
        out byte[] iv,
        out byte[] metaMac,
        out byte[] fileKey)
    {
        ArgumentNullException.ThrowIfNull(decryptedKey);
        iv = new byte[8];
        metaMac = new byte[8];
        Array.Copy(decryptedKey, sourceIndex: 16, iv, destinationIndex: 0, length: 8);
        Array.Copy(decryptedKey, sourceIndex: 24, metaMac, destinationIndex: 0, length: 8);

        fileKey = new byte[16];

        for (var i = 0; i < 16; i++)
        {
            fileKey[i] = (byte)(decryptedKey[i] ^ decryptedKey[i + 16]);
        }
    }

    [return: NotNullIfNotNull(nameof(data))]
    public static string? ToBase64Url(this byte[]? data)
    {
        if (data is null)
        {
            return null;
        }

        var s = Convert.ToBase64String(data);

        return s.Replace(oldChar: '+', newChar: '-').Replace(oldChar: '/', newChar: '_').TrimEnd(trimChar: '=');
    }

    [return: NotNullIfNotNull(nameof(data))]
    public static byte[]? FromBase64Url(this string? data)
    {
        if (data.IsEmpty())
        {
            return null;
        }

        var s = data.Replace(oldChar: '-', newChar: '+')
            .Replace(oldChar: '_', newChar: '/')
            .Replace(oldValue: ",", string.Empty, StringComparison.Ordinal);

        s = s.PadRight((s.Length + 3) / 4 * 4, paddingChar: '=');

        return Convert.FromBase64String(s);
    }

    public static byte[] ToBytesUtf8(this string s) => Encoding.UTF8.GetBytes(s);

    internal static byte[] ToBytesPassword(this string s)
    {
        var words = new uint[(s.Length + 3) >> 2];

        for (var i = 0; i < s.Length; i++)
        {
            words[i >> 2] |= (uint)s[i] << (24 - ((i & 3) * 8));
        }

        var ret = new byte[words.Length * 4];

        for (var i = 0; i < words.Length; i++)
        {
            BinaryPrimitives.WriteUInt32BigEndian(ret.AsSpan(i * 4, length: 4), words[i]);
        }

        return ret;
    }

    internal static byte[] PrepareKey(this byte[] data)
    {
        var key = new byte[]
        {
            147, 196, 103, 227, 125, 176, 199, 164, 209, 190, 63, 129, 1, 82, 203, 86
        };

        for (var i = 0; i < 65536; i++)
        {
            for (var j = 0; j < data.Length; j += 16)
            {
                var k = new byte[16];
                Array.Copy(data, j, k, destinationIndex: 0, Math.Min(val1: 16, data.Length - j));
                key = key.EncryptAes(k);
            }
        }

        return key;
    }

    internal static string GenerateHash(this string emailLower, byte[] passwordAesKey)
    {
        var emailBytes = emailLower.ToBytesUtf8();
        var hashInput = new byte[16];

        for (var i = 0; i < emailBytes.Length; i++)
        {
            hashInput[i % 16] ^= emailBytes[i];
        }

        using var enc = passwordAesKey.CreateAesEncryptor();

        for (var j = 0; j < 16384; j++)
        {
            hashInput = hashInput.EncryptAes(enc);
        }

        var output = new byte[8];
        Array.Copy(hashInput, sourceIndex: 0, output, destinationIndex: 0, length: 4);
        Array.Copy(hashInput, sourceIndex: 8, output, destinationIndex: 4, length: 4);

        return output.ToBase64Url();
    }

    internal static string GenerateHashcashToken(string? challenge)
    {
        var parts = challenge?.Split(separator: ':') ?? [];

        if (parts.Length < 4)
        {
            throw new ArgumentException(message: "Invalid challenge format.", nameof(challenge));
        }

        if (parts[0].ToInt() != 1)
        {
            throw new ArgumentException(message: "Hashcash challenge is not version 1.", nameof(challenge));
        }

        var num = parts[1].ToInt();
        var value = parts[3];

        var num2 = ((num & 0x3F) << 1) + 1;
        var num3 = ((num >> 6) * 7) + 3;
        var num4 = num2 << num3;

        var bytes = value.FromBase64Url();

        const int Blocks = 262144;
        const int Width = 48;
        const int Prefix = 4;

        var data = new byte[Prefix + (Blocks * Width)];

        for (var i = 0; i < Blocks; i++)
        {
            Buffer.BlockCopy(bytes, srcOffset: 0, data, Prefix + (i * Width), Width);
        }

        while (true)
        {
            var hash = SHA256.HashData(data);
            var first = BinaryPrimitives.ReadUInt32BigEndian(hash);

            if (first <= num4)
            {
                break;
            }

            var idx = 0;

            do
            {
                data[idx]++;
            }
            while (data[idx++] == 0);
        }

        var proof = new byte[4];
        Array.Copy(data, sourceIndex: 0, proof, destinationIndex: 0, length: 4);

        return "1:" + value + ":" + proof.ToBase64Url();
    }

    internal static BigInteger[] GetRsaPrivateKeyComponents(byte[] encodedRsaPrivateKey, byte[] masterKey)
    {
        ArgumentNullException.ThrowIfNull(encodedRsaPrivateKey);
        ArgumentNullException.ThrowIfNull(masterKey);

        encodedRsaPrivateKey = encodedRsaPrivateKey.Pad16();

        var decrypted = DecryptKey(encodedRsaPrivateKey, masterKey);
        var span = new ReadOnlySpan<byte>(decrypted);

        var parts = new BigInteger[4];

        for (var i = 0; i < 4; i++)
        {
            parts[i] = ReadMpi(ref span);
        }

        return parts;
    }

    public static byte[] RsaDecrypt(this BigInteger data, BigInteger p, BigInteger q, BigInteger d)
    {
        var n = p * q;
        var x = BigInteger.ModPow(data, d, n);

        return x.ToBigEndianUnsigned();
    }

    public static BigInteger FromMpiNumber(this byte[]? data)
    {
        var span = new ReadOnlySpan<byte>(data);

        return ReadMpi(ref span);
    }

    private static BigInteger ReadMpi(ref ReadOnlySpan<byte> data)
    {
        if (data.Length < 2)
        {
            throw new ArgumentException(message: "Invalid MPI.");
        }

        var bitLen = (data[index: 0] << 8) | data[index: 1];
        var byteLen = (bitLen + 7) / 8;

        if (data.Length < 2 + byteLen)
        {
            throw new ArgumentException(message: "Invalid MPI.");
        }

        var mpi = data.Slice(start: 2, byteLen);
        data = data.Slice(2 + byteLen);

        return mpi.FromBigEndianUnsigned();
    }

    public static BigInteger FromBigEndianUnsigned(this ReadOnlySpan<byte> bytes)
    {
        // BigInteger expects little-endian two's complement.
        var tmp = new byte[bytes.Length + 1];

        for (var i = 0; i < bytes.Length; i++)
        {
            tmp[i] = bytes[bytes.Length - 1 - i];
        }

        tmp[^1] = 0;

        return new BigInteger(tmp);
    }

    public static byte[] ToBigEndianUnsigned(this BigInteger value)
    {
        if (value.Sign < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        var le = value.ToByteArray(); // little-endian two's complement

        // Remove sign byte if present.
        if (le.Length > 1 && le[^1] == 0)
        {
            Array.Resize(ref le, le.Length - 1);
        }

        Array.Reverse(le);

        return le;
    }

    public static byte[] Pad16(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        var pad = (16 - (data.Length % 16)) % 16;

        if (pad == 0)
        {
            return data;
        }

        var ret = new byte[data.Length + pad];
        Array.Copy(data, ret, data.Length);

        return ret;
    }

    internal static byte[] EncryptAttributes(MegaAttributes attributes, byte[] nodeKey)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        ArgumentNullException.ThrowIfNull(nodeKey);
        var json = JsonSerializer.Serialize(attributes);
        var payload = ("MEGA" + json).ToBytesUtf8();
        payload = payload.Pad16();

        return payload.EncryptAes(nodeKey);
    }

    internal static MegaAttributes? DecryptAttributes(byte[] encryptedAttributes, byte[] nodeKey)
    {
        ArgumentNullException.ThrowIfNull(encryptedAttributes);
        ArgumentNullException.ThrowIfNull(nodeKey);

        var data = encryptedAttributes.DecryptAes(nodeKey);

        try
        {
            var text = Encoding.UTF8.GetString(data);

            if (text.StartsWith(value: "MEGA", StringComparison.Ordinal))
            {
                text = text.Substring(startIndex: 4);
            }

            var nullPos = text.IndexOf(value: '\0', StringComparison.Ordinal);

            if (nullPos >= 0)
            {
                text = text.Substring(startIndex: 0, nullPos);
            }

            return JsonSerializer.Deserialize<MegaAttributes?>(text);
        }
        catch (Exception ex)
        {
            return new MegaAttributes
            {
                Name = $"Attribute deserialization failed: {ex.Message}"
            };
        }
    }

    public static long ToEpochSeconds(this DateTime dt)
    {
        var epoch = DateTime.UnixEpoch;

        return (long)(dt.ToUniversalTime() - epoch).TotalSeconds;
    }

    public static DateTime FromEpochSeconds(this long seconds)
    {
        var epoch = DateTime.UnixEpoch;

        return epoch.AddSeconds(seconds).ToLocalTime();
    }

    public static byte[] SerializeToBytes(this long value)
    {
        var buf = new byte[9];
        byte len = 0;
        var x = value;

        while (x != 0)
        {
            buf[++len] = (byte)x;
            x >>= 8;
        }

        buf[0] = len;
        Array.Resize(ref buf, len + 1);

        return buf;
    }

    public static long DeserializeToLong(this byte[] data, int index, int length)
    {
        ArgumentNullException.ThrowIfNull(data);

        var len = data[index];
        long ret = 0;

        if (len > 8 || len >= length)
        {
            throw new ArgumentException(message: "Invalid value.");
        }

        while (len > 0)
        {
            ret = (ret << 8) + data[index + len--];
        }

        return ret;
    }
}
