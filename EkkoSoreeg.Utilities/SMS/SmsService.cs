using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EkkoSoreeg.Utilities.SMS
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _client;
        public SmsService(HttpClient client)
        {
            _client = client;
        }

        public async Task SendOTPAsync(string toPhone, string otp)
        {
            try
            {
                string url = $"{SD.Url}?environment=2&username={SD.UserName}&password={SD.Password}&sender={SD.SenderIDs}" +
                             $"&mobile={toPhone}&template={SD.Templete}&otp={otp}";

                HttpResponseMessage response = await _client.PostAsync(url, null);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Failed to send OTP. HTTP request error.", ex);
            }
        }
    }
}
