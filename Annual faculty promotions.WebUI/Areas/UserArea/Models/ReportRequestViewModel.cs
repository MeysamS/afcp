using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Models
{
    public class ReportRequestViewModel
    {
        public Request request { get; set; }
        public Definitions definition { get; set; }
    }
}