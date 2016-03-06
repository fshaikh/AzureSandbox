using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public enum TimeSpanType
    {
        Milliseconds,
        Seconds,
        Minutes,
        Hours
    }
    public class LeaseRequest
    {
        /// <summary>
        /// Lease Id. Clients can provide their own lease id. Blob Storage will generate system lease ids if not user provided.
        /// </summary>
        public string LeaseId { get; set; }
        /// <summary>
        /// Lease period. Specify the span type in TimeSpanType property .Client will have exclusive lease on the blob/container for this period if time.
        /// </summary>
        public double LeasePeriod { get; set; }

        /// <summary>
        /// Blob for which to lease
        /// </summary>
        public Blob Blob { get; set; }

        public TimeSpanType TimeSpanType { get; set; }
    }
}
