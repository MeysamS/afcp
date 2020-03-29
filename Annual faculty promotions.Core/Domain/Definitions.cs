using Annual_faculty_promotions.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.SqlServer.Server;

namespace Annual_faculty_promotions.Core.Domain
{
    public class Definitions : AuditableEntity<int>
    {
        public string UniversityName { get; set; }
        public string Logo { get; set; }
        public string Address { get; set; }
        public string SMSUserName { get; set; }
        public string SMSPass { get; set; }
        public string SMSNumber { get; set; }
        public string TextSMS { get; set; }
        public int? RepeatMail { get; set; }
        public int? StartMail { get; set; }

        // متن ایمیل مهلت ارسال درخواست
        public string TextMailForDourtionRequest { get; set; }
        public string SmtpHost { get; set; }

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "ایمیل را اشتباه وارد کردید!")]
        public string SmtpFrom { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPass { get; set; }
        public string SmtpPort { get; set; }
        public string CommitteeName1 { get; set; }
        public string UniversPosition1 { get; set; }
        public string CommitteePosition1 { get; set; }
        public string CommitteeName2 { get; set; }
        public string UniversPosition2 { get; set; }
        public string CommitteePosition2 { get; set; }
        public string CommitteeName3 { get; set; }
        public string UniversPosition3 { get; set; }
        public string CommitteePosition3 { get; set; }
        public string CommitteeName4 { get; set; }
        public string UniversPosition4 { get; set; }
        public string CommitteePosition4 { get; set; }
    }
}
