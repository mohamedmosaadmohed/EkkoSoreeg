using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Models
{
    public class ShoppingCart
    {
        [Key]
        public int shoppingId { get; set; }
        [ValidateNever]
        public Guid? shoppingIdGuid { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }
        [Range(1, 20, ErrorMessage = "Count Must be between 1 to 20")]
        public int Count { get; set; }
        [Required]
        public string Color { get; set; }
		[Required]
		public string Size { get; set; }
        public string ApplicationUserId  { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<Product> RelatedProducts { get; set; }
    }
}
