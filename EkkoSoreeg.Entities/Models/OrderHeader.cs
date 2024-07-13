using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Models
{
	public class OrderHeader
	{
		[Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ValidateNever]
		public ApplicationUser applicationUser { get; set; }
        public DateTime orderDate { get; set; }
        public DateTime shippingDate { get; set; }
        public decimal totalPrice { get; set; }
        public string? orderStatus { get; set; }
        public string? paymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public bool Downloader { get; set; } = false;
        public DateTime paymentDate { get; set; }

        // Stripe
		public string? sessionId { get; set; }
		public string? paymentIntentId { get; set; }

        // Data For User
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        public string? AdditionalInformation { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Phone]
        public string? AdditionalPhoneNumber { get; set; }

    }
}
