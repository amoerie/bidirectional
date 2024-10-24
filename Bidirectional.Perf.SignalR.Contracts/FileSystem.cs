namespace Bidirectional.Perf.SignalR.Contracts;

public record FileSystemBase
{
    public string Name { get; set; } = default!;
    public DateTime CreationDateTime { get; set; }
    public DateTime LastUpdateDateTime { get; set; }
}

public sealed record FileDto : FileSystemBase
{
    public long Size { get; set; }
}

public sealed record DirectoryDto : FileSystemBase
{
    public IList<FileSystemBase> Files { get; set; } = new List<FileSystemBase>();
    public IList<FileSystemBase> Directories { get; set; } = new List<FileSystemBase>();
}
