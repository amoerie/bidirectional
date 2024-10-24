using Bidirectional.Perf.SignalR.Contracts;

namespace Bidirectional.Perf.SignalR.Client.Utils;

public sealed class FileManager
{
    public static DirectoryDto GetPath(string path, int maxDepth = 3)
    {
        return GetDirectoryInfoRecursive(path, maxDepth);
    }

    private static DirectoryDto GetDirectoryInfoRecursive(string path, int maxDepth)
    {
        var directory = new DirectoryInfo(path);
        if (!directory.Exists) throw new InvalidOperationException("Directory does not exist: " + path);

        var directoryDto = new DirectoryDto
        {
            Name = directory.Name,
            CreationDateTime = directory.CreationTime,
            LastUpdateDateTime = directory.LastWriteTime
        };

        if (maxDepth <= 0) return directoryDto;
        
        foreach (var subDirectory in directory.GetDirectories())
        {
            directoryDto.Directories.Add(GetDirectoryInfoRecursive(subDirectory.FullName, maxDepth - 1));
        }

        foreach (var file in directory.GetFiles())
        {
            directoryDto.Files.Add(new FileDto
            {
                Name = file.Name,
                CreationDateTime = file.CreationTime,
                LastUpdateDateTime = file.LastWriteTime,
                Size = file.Length
            });
        }

        return directoryDto;
    }
}
