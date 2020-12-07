using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidirectional.Demo.Client.Models
{
    public record PingPongResult(
        int NumberOfRequests,
        int TotalTimeImMs
    )
    {
        public double RequestsPerSecond => NumberOfRequests / (TotalTimeImMs / 1000.0);
    }
}
