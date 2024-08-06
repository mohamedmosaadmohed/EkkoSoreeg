using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Utilities.OTP
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _memoryCache;
        private const string OtpCacheKeyPrefix = "OTP_";

        public OtpService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void StoreOTP(string email, string otp, TimeSpan expiration)
        {
            var cacheKey = OtpCacheKeyPrefix + email;
            _memoryCache.Set(cacheKey, otp, expiration);
        }

        public bool VerifyOTP(string email, string otp)
        {
            var cacheKey = OtpCacheKeyPrefix + email;
            if (_memoryCache.TryGetValue(cacheKey, out string storedOtp))
            {
                if (storedOtp == otp)
                {
                    // OTP matches, remove it from cache
                    _memoryCache.Remove(cacheKey);
                    return true;
                }
            }

            return false;
        }
    }
}
