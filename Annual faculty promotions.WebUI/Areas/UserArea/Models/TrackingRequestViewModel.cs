using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Models
{
    public class TrackingRequestViewModel
    {
        
        public IEnumerable<Cartable> Trackings { get; set; }
        public IEnumerable<long> RequestsId { get;set; } 
    }
}