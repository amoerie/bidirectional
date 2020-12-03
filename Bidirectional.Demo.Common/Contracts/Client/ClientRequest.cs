using Bidirectional.Demo.Common.Contracts.Client.GetClientProcessInfo;
using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Client
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(1, typeof(GetClientProcessInfoRequest))]
    public abstract record ClientRequest(
        [property: ProtoMember(2)] ClientRequestMetaData MetaData
    );
}