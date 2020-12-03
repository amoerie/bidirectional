using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Client.Models.Diagnostics
{
    public class DiagnosticModel<T>
    {
        public T Inner { get; set; }

        public long TotalCallTimeInMs { get; set; }

        public long? ProcessingTimeByRemoteInMs { get; set; }

        public long? TransferedBytes { get; set; }

        public int? NetworkTimeInMs => ProcessingTimeByRemoteInMs == null ? null : (int)(TotalCallTimeInMs - ProcessingTimeByRemoteInMs);

        public double? TransferSpeedInMbPerS => TransferedBytes == null || NetworkTimeInMs == null ? null : (double)TransferedBytes / 1024.0 / 1024.0 / ((double)NetworkTimeInMs / 1000.0);

        public DiagnosticModel(T inner, long totalCallTime)
        {
            this.Inner = inner;
            this.TotalCallTimeInMs = totalCallTime;
        }

        public DiagnosticModel(T inner, long totalCallTime, long? processingTimeByRemote)
        {
            this.Inner = inner;
            this.TotalCallTimeInMs = totalCallTime;
            this.ProcessingTimeByRemoteInMs = processingTimeByRemote;
        }

        public DiagnosticModel(T inner, long totalCallTime, long? processingTimeByRemote, long? transferedBytes)
        {
            this.Inner = inner;
            this.TotalCallTimeInMs = totalCallTime;
            this.ProcessingTimeByRemoteInMs = processingTimeByRemote;
            this.TransferedBytes = transferedBytes;
        }
    }
}
