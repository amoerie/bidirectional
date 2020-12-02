using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Common.Contracts.Client.GetFile
{
    public class GetFileRequest
    {
        public string Path { get; set; }
        public int? RequestedSizeInBytes { get; set; }
    }
}
