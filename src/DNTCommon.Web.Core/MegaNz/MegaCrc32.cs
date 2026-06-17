namespace DNTCommon.Web.Core;

internal static class MegaCrc32
{
    private const uint Poly = 3988292384u; // 0xEDB88320 (CryptoPP CRC32 polynomial)

    private static uint Compute(ReadOnlySpan<byte> buffer, uint seed)
    {
        var crc = ~seed;

        for (var i = 0; i < buffer.Length; i++)
        {
            crc ^= buffer[i];

            for (var j = 0; j < 8; j++)
            {
                var mask = (uint)-(int)(crc & 1);
                crc = (crc >> 1) ^ (Poly & mask);
            }
        }

        return ~crc;
    }

    public static byte[] ComputeMegaCrc(Stream stream)
    {
        stream.Seek(offset: 0, SeekOrigin.Begin);

        var crc = new uint[4];
        var small = new byte[16];

        if (stream.Length <= 16)
        {
            var read = stream.Read(small, offset: 0, (int)stream.Length);

            if (read > 0)
            {
                Buffer.BlockCopy(small, srcOffset: 0, crc, dstOffset: 0, small.Length);
            }
        }
        else if (stream.Length <= 8192)
        {
            var buffer = new byte[stream.Length];
            var read = 0;

            while (read < buffer.Length)
            {
                var r = stream.Read(buffer, read, buffer.Length - read);

                if (r == 0)
                {
                    break;
                }

                read += r;
            }

            for (var i = 0; i < 4; i++)
            {
                var start = (int)(i * stream.Length / 4);
                var end = (int)((i + 1) * stream.Length / 4);
                var value = Compute(buffer.AsSpan(start, end - start), uint.MaxValue);
                crc[i] = value;
            }
        }
        else
        {
            var buf = new byte[64];
            var count = (uint)(8192 / (buf.Length * 4));
            long pos = 0;

            for (uint part = 0; part < 4; part++)
            {
                var seed = uint.MaxValue;
                byte[]? lastHash = null;

                for (uint n = 0; n < count; n++)
                {
                    var target = (stream.Length - buf.Length) * (part * count + n) / (4 * count - 1);
                    stream.Seek(target - pos, SeekOrigin.Current);
                    pos += target - pos;

                    var r = stream.Read(buf, offset: 0, buf.Length);
                    pos += r;

                    seed = ~Compute(buf.AsSpan(start: 0, r), seed);

                    lastHash = BitConverter.GetBytes(~seed);
                }

                if (lastHash is null)
                {
                    throw new InvalidOperationException(message: "lastHash is null");
                }

                crc[part] = BitConverter.ToUInt32(lastHash, startIndex: 0);
            }
        }

        var outBytes = new byte[16];
        Buffer.BlockCopy(crc, srcOffset: 0, outBytes, dstOffset: 0, count: 16);

        return outBytes;
    }
}