using EkkoSoreeg.Entities.ValidateDataAnotation;
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
        [ValidateNever]
        public string ApplicationUserId { get; set; }
        [ValidateNever]
		public ApplicationUser applicationUser { get; set; }
        public DateTime orderDate { get; set; } = DateTime.UtcNow;
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
        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }

        [ValidateNever]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; }

        public string? AdditionalInformation { get; set; }

        [Required(ErrorMessage = "Region is required.")]
        public string? Region { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [EgyptianPhone(ErrorMessage = "Invalid Egyptian phone number format.")]
        public string? PhoneNumber { get; set; }

        [EgyptianPhone(ErrorMessage = "Invalid additional phone number format.")]
        public string? AdditionalPhoneNumber { get; set; }

    }
}
