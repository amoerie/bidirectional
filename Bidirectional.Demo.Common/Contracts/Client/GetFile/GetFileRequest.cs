using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Common.Contracts.Client.GetFile
{
    [ProtoContract]
    public class GetFileRequest
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public int? RequestedSizeInBytes { get; set; }
    }
}
