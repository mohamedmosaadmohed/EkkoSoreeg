using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Models
{
	public class OrderDetails
	{
        [Key]
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        [ValidateNever]
        public OrderHeader orderHeader { get; set; }
        public int productId { get; set; }
		[ValidateNever]
		public Product product { get; set; }
        public int Count { get; set; }
        public decimal price { get; set; }

    }
}
