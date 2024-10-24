using Bidirectional.Grpc.Common.Contracts.Client.GetClientProcessInfo;
using ProtoBuf;

namespace Bidirectional.Grpc.Common.Contracts.Client
{
    [ProtoContract]
    [ProtoInclude(1, typeof(GetClientProcessInfoResponse))]
    public abstract record ClientResponse(
        [property: ProtoMember(2)] ClientResponseMetaData MetaData
    );
}