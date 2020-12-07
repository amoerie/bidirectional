using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Common.Contracts.Client.PingPong
{
    [ProtoContract(SkipConstructor = true)]
    public record PingPongResponse(
        [property: ProtoMember(1)] bool Result
    );
}
