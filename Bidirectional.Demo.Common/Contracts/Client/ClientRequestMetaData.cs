using System;
using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Client
{
    [ProtoContract(SkipConstructor = true)]
    public record ClientRequestMetaData(
        [property: ProtoMember(1)] string RequestId,
        [property: ProtoMember(2)] DateTime RequestTimeStampUtc
    )
    {
        public static readonly ClientRequestMetaData Missing = new ClientRequestMetaData("", default);
    }
}