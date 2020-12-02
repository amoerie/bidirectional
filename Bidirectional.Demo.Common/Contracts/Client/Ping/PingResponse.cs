using System;
using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Client.Ping
{
    [ProtoContract]
    public class PingResponse : ClientResponse
    {
        [ProtoMember(1, DataFormat = DataFormat.WellKnown)]
        public DateTime TimeStamp { get; set; }
    }
}