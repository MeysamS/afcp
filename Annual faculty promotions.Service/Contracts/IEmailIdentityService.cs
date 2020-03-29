using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annual_faculty_promotions.Service.Contracts
{
    public interface IEmailIdentityService : IIdentityMessageService
    {
        void SendEmailWithPostal(string view, string to, string cc, string bcc, string title, string subject,
            string body);
    }
}
