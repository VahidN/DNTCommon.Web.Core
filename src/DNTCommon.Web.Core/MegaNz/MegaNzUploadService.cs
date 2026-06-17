namespace DNTCommon.Web.Core;

public static class MegaNzUploadService
{
    public static async Task<string> UploadFileToMegaNzAsync(this HttpClient httpClient,
        string email,
        string password,
        string localFilePath,
        string uploadFolderNameOnMegaNz,
        int keepLastNFilesOnMegaNz,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(localFilePath))
        {
            throw new FileNotFoundException($"`{localFilePath}` not found.");
        }

        var client = new MegaClient(httpClient);
        await client.LoginAsync(email, password, cancellationToken);

        var allNodes = await client.FillDefaultNodesAsync(cancellationToken);

        var uploadFolder =
            await GetOrCreateUploadFolderAsync(client, uploadFolderNameOnMegaNz, allNodes, cancellationToken);

        await DeleteOldFilesAsync(client, keepLastNFilesOnMegaNz, allNodes, uploadFolder, cancellationToken);

        var uploadUri = await UploadFileAsync(client, localFilePath, uploadFolder, cancellationToken);

        await client.LogoutAsync(cancellationToken);

        return uploadUri;
    }

    private static async Task<string> UploadFileAsync(MegaClient client,
        string localFilePath,
        MegaNode uploadFolder,
        CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(localFilePath);

        var uploadFile = await client.UploadAsync(uploadFolder.Id, Path.GetFileName(localFilePath), stream,
            modificationDate: null, progress: null, cancellationToken);

        var uploadUri = await client.PublishAsync(uploadFile, cancellationToken);

        return uploadUri;
    }

    private static async Task<MegaNode> GetOrCreateUploadFolderAsync(MegaClient client,
        string uploadFolderNameOnMegaNz,
        IList<MegaNode> allNodes,
        CancellationToken cancellationToken)
    {
        var rootNode = allNodes.Single(node => node.Type == MegaNodeType.Root);

        return allNodes.FirstOrDefault(node
                   => node.Type == MegaNodeType.Directory &&
                      string.Equals(node.ParentId, rootNode.Id, StringComparison.Ordinal) &&
                      string.Equals(node.Name, uploadFolderNameOnMegaNz, StringComparison.OrdinalIgnoreCase)) ??
               await client.CreateFolderAsync(rootNode.Id, uploadFolderNameOnMegaNz, cancellationToken);
    }

    private static async Task DeleteOldFilesAsync(MegaClient client,
        int keepLastNFilesOnMegaNz,
        IList<MegaNode> allNodes,
        MegaNode uploadFolder,
        CancellationToken cancellationToken)
    {
        var availableFiles = allNodes.Where(node => node.Type == MegaNodeType.File &&
                                                    string.Equals(node.ParentId, uploadFolder.Id,
                                                        StringComparison.Ordinal))
            .OrderByDescending(node => node.CreationDate)
            .ToList();

        if (availableFiles.Count > keepLastNFilesOnMegaNz)
        {
            foreach (var oldFile in availableFiles.Skip(keepLastNFilesOnMegaNz))
            {
                await client.DeleteAsync(oldFile.Id, cancellationToken);
            }
        }
    }
}
