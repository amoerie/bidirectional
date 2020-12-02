using Bidirectional.Demo.Common.Contracts.Client.GetClientProcessInfo;
using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Client
{
    [ProtoContract]
    [ProtoInclude(1, typeof(GetClientProcessInfoResponse))]
    public abstract class ClientResponse
    {
        [ProtoMember(1)]
        public string RequestId { get; set; }
    }
}