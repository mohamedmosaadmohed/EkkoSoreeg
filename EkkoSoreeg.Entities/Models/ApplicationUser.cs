using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(450)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(450)]
        public string LastName { get; set; }
        [MaxLength(450)]
        public string Address { get; set; }
        [MaxLength(450)]
        public string Region { get; set; }
        public string City { get; set; }
        [Phone]
        public string? AdditionalPhoneNumber { get; set; }
    }
}
