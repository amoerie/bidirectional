using System;
using System.Diagnostics;
using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Client.GetClientProcessInfo
{
    [ProtoContract]
    public class GetClientProcessInfoResponse : ClientResponse
    {
        [ProtoMember(1)]
        public TimeSpan TotalProcessorTime { get; set; }
        
        [ProtoMember(2)]
        public TimeSpan UserProcessorTime { get; set; }
        
        [ProtoMember(3)]
        public TimeSpan PrivilegedProcessorTime { get; set; }
        
        [ProtoMember(4)]
        public string CurrentMemoryUsage { get; set; }
        
        [ProtoMember(5)]
        public string PeakMemoryUsage { get; set; }
        
        [ProtoMember(6)]
        public int ActiveThreads { get; set; }

        public GetClientProcessInfoResponse() { }

        public GetClientProcessInfoResponse(Process process)
        {
            if (process == null) throw new ArgumentNullException(nameof(process));
            
            TotalProcessorTime = process.TotalProcessorTime;
            UserProcessorTime = process.UserProcessorTime;
            PrivilegedProcessorTime = process.PrivilegedProcessorTime;
            CurrentMemoryUsage = $"{process.WorkingSet64:N0} B";
            PeakMemoryUsage = $"{process.PeakWorkingSet64:N0} B";
            ActiveThreads = process.Threads.Count;
        }

    }
}