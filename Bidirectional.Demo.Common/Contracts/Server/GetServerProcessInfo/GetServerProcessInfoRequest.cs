using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Server.GetServerProcessInfo
{
    [ProtoContract(SkipConstructor = true)]
    public record GetServerProcessInfoRequest;
}