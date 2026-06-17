namespace DNTCommon.Web.Core;

internal sealed class MegaAesCtrStreamDecrypter : MegaAesCtrStream
{
    private readonly byte[] _expectedMetaMac;

    public MegaAesCtrStreamDecrypter(Stream stream,
        long streamLength,
        byte[]? fileKey,
        byte[]? iv,
        byte[]? expectedMetaMac) : base(stream, streamLength, Mode.Decrypt, fileKey, iv)
    {
        if (expectedMetaMac is null || expectedMetaMac.Length != 8)
        {
            throw new ArgumentException(message: "Invalid expectedMetaMac.", nameof(expectedMetaMac));
        }

        _expectedMetaMac = expectedMetaMac;
    }

    protected override void OnStreamRead()
    {
        if (!_expectedMetaMac.SequenceEqual(MetaMac))
        {
            throw new InvalidOperationException(message: "Checksum is invalid. Downloaded data are corrupted.");
        }
    }
}