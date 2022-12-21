using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aaa3.basic.WebApi.Models
{
    public class PingResponse
    {
        public PingResponse()
        {
            CurrentDate = DateTimeOffset.Now;
        }

        public DateTimeOffset CurrentDate { get; private set; }
        public string Version { get; set; }
        public string Specification { get; set; }
    }
}
