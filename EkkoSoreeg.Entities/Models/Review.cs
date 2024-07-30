using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Models
{
	public class Review
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		[Required]
		public string Description { get; set; }
		public int ProductId { get; set; }
		[ValidateNever]
		public Product Product { get; set; }
		[Range(1, 5)]
		public int Rating { get; set; }
	}

}
