using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels
{
    public class CreateOrderMessage
    {
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public IList<OrderLine> Lines { get; set; }
    }
}
