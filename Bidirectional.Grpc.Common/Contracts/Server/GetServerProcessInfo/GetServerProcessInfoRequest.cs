using ProtoBuf;

namespace Bidirectional.Grpc.Common.Contracts.Server.GetServerProcessInfo
{
    [ProtoContract(SkipConstructor = true)]
    public record GetServerProcessInfoRequest;
}