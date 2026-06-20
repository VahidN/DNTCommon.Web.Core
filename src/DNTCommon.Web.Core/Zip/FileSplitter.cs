namespace DNTCommon.Web.Core;

public static class FileSplitter
{
    private const int
        MaxBufferSize =
            0x10000; // 64K. The artificial constraint due to win32 api limitations. Increasing the buffer size beyond 64k will not help in any circumstance, as the underlying SMB protocol does not support buffer lengths beyond 64k.

    public static async Task<IList<string>> SplitFileAsync(this string? inputFilePath,
        string? outputDir,
        Func<(int PartNumber, int TotalParts), string> partFileName,
        long maxPartSizeInBytes,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(partFileName);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxPartSizeInBytes);

        var outputFiles = new List<string>();

        inputFilePath = inputFilePath.NormalizePath();

        if (!inputFilePath.FileExists())
        {
            throw new InvalidOperationException($"Input file: `{inputFilePath}` doesn't exist.");
        }

        outputDir = outputDir.NormalizePath();

        if (outputDir.IsEmpty())
        {
            throw new InvalidOperationException(message: "Output dir is not set");
        }

        outputDir.TryCreateDirectory();

        var fileInfo = new FileInfo(inputFilePath);

        if (fileInfo.Length == 0)
        {
            return [];
        }

        var totalParts = (int)Math.Ceiling((double)fileInfo.Length / maxPartSizeInBytes);

        var buffer = new byte[MaxBufferSize];

        await using var sourceStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read,
            MaxBufferSize, useAsync: true);

        for (var i = 0; i < totalParts; i++)
        {
            var partNumber = i + 1;
            var fileName = partFileName((partNumber, totalParts));
            var outputPartFilePath = outputDir.SafePathCombine(fileName);

            await using var outputStream = new FileStream(outputPartFilePath, FileMode.Create, FileAccess.Write,
                FileShare.None, MaxBufferSize, useAsync: true);

            outputFiles.Add(outputPartFilePath);

            var bytesRemaining = Math.Min(maxPartSizeInBytes, fileInfo.Length - sourceStream.Position);

            while (bytesRemaining > 0)
            {
                var bytesToRead = (int)Math.Min(buffer.Length, bytesRemaining);
                var bytesRead = await sourceStream.ReadAsync(buffer.AsMemory(start: 0, bytesToRead), cancellationToken);

                if (bytesRead == 0)
                {
                    break; // پایان فایل (نباید در شرایط عادی رخ دهد)
                }

                await outputStream.WriteAsync(buffer.AsMemory(start: 0, bytesRead), cancellationToken);
                bytesRemaining -= bytesRead;
            }
        }

        return outputFiles;
    }
}
