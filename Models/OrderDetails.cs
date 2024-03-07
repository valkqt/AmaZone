using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmaZone.Models
{
    public class OrderDetails
    {
        public List<OrderState> history = new List<OrderState>();
        public Order order { get; set; }
        public OrderState state = new OrderState();
    }
}