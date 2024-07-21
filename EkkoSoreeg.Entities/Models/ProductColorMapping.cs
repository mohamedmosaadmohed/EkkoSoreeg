using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Models
{
    public class ProductColorMapping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public int ProductColorId { get; set; }
        public ProductColor ProductColor { get; set; }
    }
}
