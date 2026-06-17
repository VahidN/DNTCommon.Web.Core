namespace DNTCommon.Web.Core;

/// <summary>
///     MEGA node (file or directory) with decrypted metadata.
/// </summary>
public sealed record MegaNode
{
	/// <summary>
	///     Node id (handle).
	/// </summary>
	public string Id { get; init; } = string.Empty;

	/// <summary>
	///     Node type.
	/// </summary>
	public MegaNodeType Type { get; init; }

	/// <summary>
	///     Parent node id (handle).
	/// </summary>
	public string? ParentId { get; init; }

	/// <summary>
	///     Node name (from decrypted attributes).
	/// </summary>
	public string? Name { get; init; }

	/// <summary>
	///     Public handle for exported nodes (null when not exported).
	/// </summary>
	public string? PublicHandle { get; init; }

	/// <summary>
	///     File size in bytes (0 for folders).
	/// </summary>
	public long Size { get; init; }

	/// <summary>
	///     Creation date, if available.
	/// </summary>
	public DateTime? CreationDate { get; init; }

	/// <summary>
	///     Modification date, if available.
	/// </summary>
	public DateTime? ModificationDate { get; init; }

    // Present for file nodes.

    /// <summary>
    ///     File key (AES).
    /// </summary>
    public byte[]? FileKey { get; init; }

    /// <summary>
    ///     IV for AES-CTR.
    /// </summary>
    public byte[]? Iv { get; init; }

    /// <summary>
    ///     Expected file meta-MAC.
    /// </summary>
    public byte[]? MetaMac { get; init; }

    /// <summary>
    ///     Decrypted node key used for public links (16 bytes for folders, 32 bytes for files).
    /// </summary>
    public byte[]? NodeKey { get; init; }

    public override string ToString()
        => $"{nameof(Id)}: {Id}, {nameof(Type)}: {Type}, {nameof(ParentId)}: {ParentId}, {nameof(Name)}: {Name}, {nameof(PublicHandle)}: {PublicHandle}, {nameof(Size)}: {Size}, {nameof(CreationDate)}: {CreationDate}, {nameof(ModificationDate)}: {ModificationDate}, {nameof(FileKey)}: {FileKey.ToBase64Url()}, {nameof(Iv)}: {Iv.ToBase64Url()}, {nameof(MetaMac)}: {MetaMac.ToBase64Url()}, {nameof(NodeKey)}: {NodeKey.ToBase64Url()}";
}