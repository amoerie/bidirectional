using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Common.Contracts.Client.GetFile
{
    public class GetFileResponse
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
        public long? GenerationTimeInMs { get; set; }
    }
}
