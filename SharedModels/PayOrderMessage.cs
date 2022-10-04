using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels
{
    public class PayOrderMessage
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
    }
}
