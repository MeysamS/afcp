using Annual_faculty_promotions.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Models
{
    public class ArchiveViewModel
    {
        public ArchiveViewModel()
        {
            Archive=new Archive();
            LstOldTechnology=new List<TechnologyViewModel>();
            LstNewTechnology = new List<TechnologyViewModel>();
        }
        public Archive Archive { get; set; }
        public IList<TechnologyViewModel> LstOldTechnology { get; set; }
        public IList<TechnologyViewModel> LstNewTechnology { get; set; }
    }
}