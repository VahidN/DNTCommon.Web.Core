namespace DNTCommon.Web.Core;

/// <summary>
///     MEGA API error codes (negative integers) returned as JSON numbers.
/// </summary>
public enum MegaErrorCode
{
    /// <summary>
    ///     No error.
    /// </summary>
    Ok = 0,

    /// <summary>
    ///     Unknown or unsupported error code.
    /// </summary>
    Unknown = int.MinValue,

    /// <summary>Internal error.</summary>
    Internal = -1,

    /// <summary>Bad arguments.</summary>
    BadArguments = -2,

    /// <summary>Request failed, retry with exponential backoff.</summary>
    Again = -3,

    /// <summary>Too many requests, slow down.</summary>
    RateLimit = -4,

    /// <summary>Request failed permanently.</summary>
    Failed = -5,

    /// <summary>Too many requests for this resource.</summary>
    TooMany = -6,

    /// <summary>Resource access out of range.</summary>
    Range = -7,

    /// <summary>Resource expired.</summary>
    Expired = -8,

    /// <summary>Resource does not exist.</summary>
    NotFound = -9,

    /// <summary>Circular linkage.</summary>
    Circular = -10,

    /// <summary>Access denied.</summary>
    Access = -11,

    /// <summary>Resource already exists.</summary>
    Exists = -12,

    /// <summary>Request incomplete.</summary>
    Incomplete = -13,

    /// <summary>Cryptographic error.</summary>
    Key = -14,

    /// <summary>Bad session ID.</summary>
    BadSession = -15,

    /// <summary>Resource administratively blocked.</summary>
    Blocked = -16,

    /// <summary>Quota exceeded.</summary>
    OverQuota = -17,

    /// <summary>Resource temporarily not available.</summary>
    TempUnavailable = -18,

    /// <summary>Too many connections on this resource.</summary>
    TooManyConnections = -19,

    /// <summary>File could not be written (or failed post-write integrity check).</summary>
    Write = -20,

    /// <summary>File could not be read (or changed unexpectedly during reading).</summary>
    Read = -21,

    /// <summary>Invalid or missing application key.</summary>
    AppKey = -22,

    /// <summary>SSL verification failed.</summary>
    Ssl = -23,

    /// <summary>Not enough quota.</summary>
    GoingOverQuota = -24,

    /// <summary>A strongly-grouped request was rolled back.</summary>
    RolledBack = -25,

    /// <summary>Multi-factor authentication required.</summary>
    MfaRequired = -26,

    /// <summary>MEGA API also uses -27 in a special array response to request hashcash; see client parser.</summary>
    HashcashRequired = -27,

    /// <summary>Business account expired.</summary>
    BusinessPastDue = -28,

    /// <summary>Over Disk Quota Paywall.</summary>
    Paywall = -29,

    /// <summary>Subuser has not yet encrypted their master key for the admin user.</summary>
    SubUserKeyMissing = -30,

    /// <summary>Local error: not enough disk space.</summary>
    LocalNoSpace = -1000,

    /// <summary>Local error: operation timed out.</summary>
    LocalTimeout = -1001,

    /// <summary>Local error: operation was abandoned.</summary>
    LocalAbandoned = -1002,

    /// <summary>Local error: network error.</summary>
    LocalNetwork = -1003,

    /// <summary>Local error: client is not logged in.</summary>
    LocalLoggedOut = -1004,

    /// <summary>FUSE: bad file descriptor.</summary>
    FuseBadFileDescriptor = -2000,

    /// <summary>FUSE: is a directory.</summary>
    FuseIsDirectory = -2001,

    /// <summary>FUSE: name too long.</summary>
    FuseNameTooLong = -2002,

    /// <summary>FUSE: not a directory.</summary>
    FuseNotDirectory = -2003,

    /// <summary>FUSE: directory not empty.</summary>
    FuseNotEmpty = -2004,

    /// <summary>FUSE: not found.</summary>
    FuseNotFound = -2005,

    /// <summary>FUSE: permission denied.</summary>
    FusePermission = -2006,

    /// <summary>FUSE: read-only file system.</summary>
    FuseReadOnlyFileSystem = -2007,

    /// <summary>FUSE: already exists.</summary>
    FuseAlready = -2008,

    /// <summary>FUSE: operation cancelled.</summary>
    FuseCancelled = -2009,

    /// <summary>FUSE: duplicate.</summary>
    FuseDuplicate = -2010
}
