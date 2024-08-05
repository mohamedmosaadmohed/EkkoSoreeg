using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Utilities.SMS
{
    public interface ISmsService
    {
       Task SendOTPAsync(string phoneNumber, string otp);
    }
}
