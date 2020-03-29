using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Annual_faculty_promotions.Core.Enums;


namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Models
{
    public class ProfileViewModel
    {
        public AppUser User { get; set; }
        public Messaging Messaging { get; set; }
        public IList<Messaging> Messagings { get; set; }
        public IEnumerable<CustomRole> Roles { get; set; }
        public IList<SelectListItem> EnumRoles { get; set; }
    }
}