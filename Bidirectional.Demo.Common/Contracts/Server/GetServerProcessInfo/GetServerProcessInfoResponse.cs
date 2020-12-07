using System;
using System.Diagnostics;
using ProtoBuf;

namespace Bidirectional.Demo.Common.Contracts.Server.GetServerProcessInfo
{
    [ProtoContract(SkipConstructor = true)]
    public record GetServerProcessInfoResponse(
        [property: ProtoMember(1)] TimeSpan TotalProcessorTime,
        [property: ProtoMember(2)] TimeSpan UserProcessorTime,
        [property: ProtoMember(3)] TimeSpan PrivilegedProcessorTime,
        [property: ProtoMember(4)] string CurrentMemoryUsage,
        [property: ProtoMember(5)] string PeakMemoryUsage,
        [property: ProtoMember(6)] int ActiveThreads
    );

    public static class ExtensionsForProcess
    {
        public static GetServerProcessInfoResponse ToGetServerProcessInfoResponse(this Process process)
        {
            return new GetServerProcessInfoResponse(
                TotalProcessorTime: process.TotalProcessorTime,
                UserProcessorTime: process.UserProcessorTime,
                PrivilegedProcessorTime: process.PrivilegedProcessorTime,
                CurrentMemoryUsage: $"{process.WorkingSet64:N0} B",
                PeakMemoryUsage: $"{process.PeakWorkingSet64:N0} B",
                ActiveThreads: process.Threads.Count
            );
        }
    }
}