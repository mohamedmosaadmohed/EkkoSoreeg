using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Models
{
    public class Contact
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
		[Required]
		public string Phone { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; }
    }
}
