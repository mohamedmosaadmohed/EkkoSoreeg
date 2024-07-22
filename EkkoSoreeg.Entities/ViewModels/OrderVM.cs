using EkkoSoreeg.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.ViewModels
{
    public class OrderVM
    {
        public OrderHeader orderHeader { get; set; }
        public List<OrderDetails> orderDetails { get; set; }
    }
}
