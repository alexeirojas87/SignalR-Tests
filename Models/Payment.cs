using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR_Tests
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string Status { get; set; }
        public double Paid { get; set; }
    }
}
