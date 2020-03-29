using Annual_faculty_promotions.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Models
{
    public class TechnologyViewModel
    {
        public long Id { get; set; }
        
        //استفاده شده
        public bool Used { get; set; }
        
        // شماره درخواست
        public long RequestId { get; set; }

        // نوع پژوهشی فناوری
        public int TechnologyType { get; set; }
        public TechnologyType TechnologyTypeTitle { get; set; }
        public string Subject { get; set; }

        // تاریخ انتشار یا ارائه
        public string PresentationDate { get; set; }

        // تاریخ انتشار یا ارائه
        public string LicenseDate { get; set; }

        // امتیاز
        public float Score { get; set; }
        public float RemindScore { get; set; }
        public float UsingScore { get; set; }
    }

    public class TechnologySet
    {
        public long total { get; set; }
        public List<TechnologyViewModel> rows { get; set; }
    }
}