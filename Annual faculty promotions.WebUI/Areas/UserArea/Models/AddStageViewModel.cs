using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Models
{
    public class AddStageViewModel
    {
        public Stage Stage { get; set; }

        public string JsonStageNumbers { get; set; }

        public string JsonStages { get; set; }
    }
}