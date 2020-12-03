using System;
using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Client
{
    [ProtoContract(SkipConstructor = true)]
    public record ClientResponseMetaData(
        [property: ProtoMember(1)] string RequestId,
        [property: ProtoMember(2)] DateTime RequestTimeStampUtc,
        [property: ProtoMember(3)] DateTime ResponseTimeStampUtc
    )
    {
        public static readonly ClientResponseMetaData Missing = new ClientResponseMetaData("", default, default);
    }
}