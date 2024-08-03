using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.ValidateDataAnotation
{
    public class EgyptianPhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var phoneNumber = value.ToString();
                var regex = new Regex(@"^(?:\+20|0)?1[0125]\d{8}$");
                if (!regex.IsMatch(phoneNumber))
                {
                    return new ValidationResult("Invalid Egyptian phone number format.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
