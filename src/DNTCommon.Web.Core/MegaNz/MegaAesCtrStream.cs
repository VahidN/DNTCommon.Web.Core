namespace DNTCommon.Web.Core;

internal abstract class MegaAesCtrStream : Stream
{
    private readonly HashSet<long> _chunksPositionsCache;
    private readonly byte[] _counter = new byte[8];
    private readonly ICryptoTransform _encryptor;
    private readonly Mode _mode;

    private readonly Stream _stream;

    protected readonly byte[] FileKey;
    protected readonly byte[] Iv;

    protected readonly byte[] MetaMac = new byte[8];
    protected readonly long StreamLength;
    private byte[] _currentChunkMac = new byte[16];

    private long _currentCounter;
    private byte[] _fileMac = new byte[16];
    protected long StreamPosition;

    protected MegaAesCtrStream(Stream stream, long streamLength, Mode mode, byte[]? fileKey, byte[]? iv)
    {
        if (fileKey is null || fileKey.Length != 16)
        {
            throw new ArgumentException(message: "Invalid fileKey.", nameof(fileKey));
        }

        if (iv is null || iv.Length != 8)
        {
            throw new ArgumentException(message: "Invalid iv.", nameof(iv));
        }

        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        StreamLength = streamLength;
        _mode = mode;
        FileKey = fileKey;
        Iv = iv;

        ChunksPositions = [..GetChunksPositions(streamLength)];
        _chunksPositionsCache = [..ChunksPositions];
        _encryptor = FileKey.CreateAesEncryptor();
    }

    public long[] ChunksPositions { get; }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => StreamLength;

    public override long Position
    {
        get => StreamPosition;
        set
        {
            if (StreamPosition != value)
            {
                throw new NotSupportedException(message: "Seek is not supported.");
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _encryptor.Dispose();
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (StreamPosition == StreamLength)
        {
            return 0;
        }

        if (StreamPosition + count < StreamLength && count < 16)
        {
            throw new NotSupportedException(message: "Minimal read operation must be >= 16 bytes (except last read).");
        }

        count = StreamPosition + count < StreamLength ? count - (count % 16) : count;

        for (var pos = StreamPosition; pos < Math.Min(StreamPosition + count, StreamLength); pos += 16)
        {
            if (_chunksPositionsCache.Contains(pos))
            {
                if (pos != 0)
                {
                    ComputeChunk();
                }

                for (var i = 0; i < 8; i++)
                {
                    _currentChunkMac[i] = Iv[i];
                    _currentChunkMac[i + 8] = Iv[i];
                }
            }

            IncrementCounter();

            var block = new byte[16];
            var outBlock = new byte[16];

            var read = _stream.Read(block, offset: 0, block.Length);

            if (read != block.Length)
            {
                read += _stream.Read(block, read, block.Length - read);
            }

            var ivBlock = new byte[16];
            Array.Copy(Iv, ivBlock, length: 8);
            Array.Copy(_counter, sourceIndex: 0, ivBlock, destinationIndex: 8, length: 8);

            var keystream = ivBlock.EncryptAes(_encryptor);

            for (var i = 0; i < read; i++)
            {
                outBlock[i] = (byte)(keystream[i] ^ block[i]);
                _currentChunkMac[i] ^= _mode == Mode.Crypt ? block[i] : outBlock[i];
            }

            Array.Copy(outBlock, sourceIndex: 0, buffer, (int)(offset + pos - StreamPosition),
                (int)Math.Min(outBlock.Length, StreamLength - pos));

            _currentChunkMac = _currentChunkMac.EncryptAes(_encryptor);
        }

        var advanced = Math.Min(count, StreamLength - StreamPosition);
        StreamPosition += advanced;

        if (StreamPosition == StreamLength)
        {
            ComputeChunk();

            for (var k = 0; k < 4; k++)
            {
                MetaMac[k] = (byte)(_fileMac[k] ^ _fileMac[k + 4]);
                MetaMac[k + 4] = (byte)(_fileMac[k + 8] ^ _fileMac[k + 12]);
            }

            OnStreamRead();
        }

        return (int)advanced;
    }

    public override void Flush() => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    protected virtual void OnStreamRead()
    {
    }

    private void IncrementCounter()
    {
        var currentCounter = _currentCounter & 0xFF;

        if (currentCounter is not 255 and not 0)
        {
            _counter[7]++;
        }
        else
        {
            var bytes = BitConverter.GetBytes(_currentCounter);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            Array.Copy(bytes, _counter, length: 8);
        }

        _currentCounter++;
    }

    private void ComputeChunk()
    {
        for (var i = 0; i < 16; i++)
        {
            _fileMac[i] ^= _currentChunkMac[i];
        }

        _fileMac = _fileMac.EncryptAes(_encryptor);
    }

    private static IEnumerable<long> GetChunksPositions(long size)
    {
        yield return 0;

        long start = 0;

        for (var idx = 1; idx <= 8 && start < size - (idx * 131072); idx++)
        {
            start += idx * 131072;

            yield return start;
        }

        while (start + 1048576 < size)
        {
            start += 1048576;

            yield return start;
        }
    }

    protected enum Mode
    {
        Crypt,
        Decrypt
    }
}
