using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Twilio;
using Annual_faculty_promotions.Service.Contracts;

namespace Annual_faculty_promotions.Service.Implemention
{
    public class SmsService : ISmsService
    {
        public Task SendAsync(IdentityMessage message)
        {
            string AccountSid = "YourTwilioAccountSID";
            string AuthToken = "YourTwilioAuthToken";
            string twilioPhoneNumber = "YourTwilioPhoneNumber";

            var twilio = new TwilioRestClient(AccountSid, AuthToken);

            twilio.SendMessage(twilioPhoneNumber, message.Destination, message.Body); 

            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
