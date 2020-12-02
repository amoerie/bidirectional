using Bidirectional.Demo.Common.Contracts.Client.GetClientProcessInfo;
using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Client
{
    [ProtoContract]
    [ProtoInclude(1, typeof(GetClientProcessInfoRequest))]
    public abstract class ClientRequest
    {
        [ProtoMember(1)]
        public string RequestId { get; set; }
    }
}