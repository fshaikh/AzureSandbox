using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class LeaseResponse
    {
        public string LeaseId { get; set; }
        public bool  IsSucces { get; set; }
        public HttpStatusCode Status { get; set; }
    }
}
