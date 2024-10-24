using MessagePack;

namespace Bidirectional.Perf.SignalR.Contracts;

[Union(0, typeof(FileDto))]
[Union(1, typeof(DirectoryDto))]
public abstract record FileSystemBase
{
    [Key(0)]
    public required string Name { get; init; }
    
    [Key(1)]
    public DateTime CreationDateTime { get; init; }
    
    [Key(2)]
    public DateTime LastUpdateDateTime { get; init; }
}

[MessagePackObject]
public sealed record FileDto : FileSystemBase
{
    [Key(3)]
    public long Size { get; init; }
}

[MessagePackObject]
public sealed record DirectoryDto : FileSystemBase
{
    [Key(4)]
    public IList<FileSystemBase> Files { get; init; } = new List<FileSystemBase>();
    
    [Key(5)]
    public IList<FileSystemBase> Directories { get; init; } = new List<FileSystemBase>();
}
