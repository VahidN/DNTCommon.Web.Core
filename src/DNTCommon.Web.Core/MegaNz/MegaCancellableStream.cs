namespace DNTCommon.Web.Core;

internal sealed class MegaCancellableStream(Stream inner, CancellationToken cancellationToken) : Stream
{    
    public override bool CanRead => inner.CanRead;

    public override bool CanSeek => inner.CanSeek;

    public override bool CanWrite => inner.CanWrite;

    public override long Length => inner.Length;

    public override long Position
    {
        get => inner.Position;
        set => inner.Position = value;
    }

    public override void Flush()
    {
        cancellationToken.ThrowIfCancellationRequested();
        inner.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return inner.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return inner.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        cancellationToken.ThrowIfCancellationRequested();
        inner.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        cancellationToken.ThrowIfCancellationRequested();
        inner.Write(buffer, offset, count);
    }
}
