using System;
using System.Diagnostics;
using ProtoBuf;

namespace Bidirectional.Demo.Server.GrpcServices.ServerProcessInformation
{
    [ProtoContract]
    public class GetServerProcessInfoResponse
    {
        [ProtoMember(1, DataFormat = DataFormat.WellKnown)]
        public TimeSpan TotalProcessorTime { get; set; }
        
        [ProtoMember(2, DataFormat = DataFormat.WellKnown)]
        public TimeSpan UserProcessorTime { get; set; }
        
        [ProtoMember(3, DataFormat = DataFormat.WellKnown)]
        public TimeSpan PrivilegedProcessorTime { get; set; }
        
        [ProtoMember(4)]
        public string CurrentMemoryUsage { get; set; }
        
        [ProtoMember(5)]
        public string PeakMemoryUsage { get; set; }
        
        [ProtoMember(6)]
        public int ActiveThreads { get; set; }

        public GetServerProcessInfoResponse() { }

        public GetServerProcessInfoResponse(Process process)
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