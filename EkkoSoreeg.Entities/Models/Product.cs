using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(450)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [ValidateNever]
        public string Image { get; set; }
        [Required]
        public decimal Price { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Required]
        public int CatagoryId { get; set; }
        [ValidateNever]
        public Catagory TbCatagory { get; set; }

    }
}
