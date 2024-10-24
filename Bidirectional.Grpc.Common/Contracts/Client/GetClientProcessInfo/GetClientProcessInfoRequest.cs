using ProtoBuf;

namespace Bidirectional.Grpc.Common.Contracts.Client.GetClientProcessInfo
{
    [ProtoContract(SkipConstructor = true)]
    public record GetClientProcessInfoRequest() : ClientRequest(ClientRequestMetaData.Missing);
}