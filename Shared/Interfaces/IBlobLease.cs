using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    /// <summary>
    /// Thi interface defines methods for leasing of blobs/containers. It defines methods for acquiring, renewing, breaking, changing lease.
    /// </summary>
    public interface IBlobLease
    {
        /// <summary>
        /// Tries to acquire lease on blob.
        /// </summary>
        /// <param name="leaseRequest">Lease request</param>
        /// <returns>Lease response</returns>
        LeaseResponse AcquireLease(LeaseRequest leaseRequest);
    }
}
