using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Client.GetClientProcessInfo
{
    [ProtoContract(SkipConstructor = true)]
    public record GetClientProcessInfoRequest() : ClientRequest(ClientRequestMetaData.Missing);
}