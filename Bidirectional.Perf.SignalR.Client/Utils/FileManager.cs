using Bidirectional.Perf.SignalR.Contracts;

namespace Bidirectional.Perf.SignalR.Client.Utils;

public sealed class FileManager
{
    Task<DirectoryDto> GetPathAsync(string path, int maxDepth = 3)
    {
        return GetDirectoryInfoRecursive(path, maxDepth);
    }

    private Task<DirectoryDto> GetDirectoryInfoRecursive(string path, int maxDepth)
    {
        throw new NotImplementedException();
    }
}
