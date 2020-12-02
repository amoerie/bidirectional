using System;
using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Client.Ping
{
    [ProtoContract]
    public class PingRequest : ClientRequest
    {
        [ProtoMember(1)]
        public DateTime TimeStamp { get; set; }
    }
}