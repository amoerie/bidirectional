using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Common.Contracts.Client.GetFile
{
    [ProtoContract]
    public class GetFileResponse
    {
        [ProtoMember(1)]
        public byte[] Bytes { get; set; }

        [ProtoMember(2)]
        public string FileName { get; set; }

        [ProtoMember(3)]
        public long? GenerationTimeInMs { get; set; }
    }
}
