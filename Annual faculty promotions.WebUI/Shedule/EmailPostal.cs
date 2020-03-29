using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Postal;

namespace Annual_faculty_promotions.WebUI.Shedule
{
    public class EmailPostal : Email
    {
        public EmailPostal(string view, string to, string cc, string bcc, string title, string subject,string body)
        {
            base.ViewName = view;
            To = to;
            CC = cc;
            Bcc = bcc;
            Subject = subject;
            Title = title;
            Body = body;
            }
        public string To { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}