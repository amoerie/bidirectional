using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Client.Models.Diagnostics
{
    public class DiagnosticModel<T>
    {
        public T Inner { get; set; }
        public long ProcessingTimeInMs { get; set; }

        public DiagnosticModel(T inner, long processingTimeInMs)
        {
            this.Inner = inner;
            this.ProcessingTimeInMs = processingTimeInMs;
        }
    }
}
