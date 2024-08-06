using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Utilities.OTP
{
    public interface IOtpService
    {
        void StoreOTP(string email, string otp, TimeSpan expiration);
        bool VerifyOTP(string email, string otp);
    }

}
