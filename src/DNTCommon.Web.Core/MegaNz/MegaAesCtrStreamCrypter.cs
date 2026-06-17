namespace DNTCommon.Web.Core;

internal sealed class MegaAesCtrStreamCrypter(Stream stream) : MegaAesCtrStream(stream, stream.Length, Mode.Crypt,
    MegaCrypto.CreateAesKey(), MegaCrypto.CreateAesKey().AsSpan(start: 0, length: 8).ToArray())
{
    public byte[] FileKeyBytes => FileKey;

    public byte[] IvBytes => Iv;

    public byte[] ComputedMetaMac => StreamPosition != StreamLength
        ? throw new NotSupportedException(message: "Stream must be fully read to obtain MetaMac.")
        : MetaMac;
}