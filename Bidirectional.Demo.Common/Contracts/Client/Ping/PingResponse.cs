using System;
using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Client.Ping
{
    [ProtoContract]
    public class PingResponse : ClientResponse
    {
        [ProtoMember(1)]
        public DateTime TimeStamp { get; set; }
    }
}