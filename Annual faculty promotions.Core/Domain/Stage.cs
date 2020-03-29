using Annual_faculty_promotions.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain.User;

namespace Annual_faculty_promotions.Core.Domain
{
    public class Stage
    {
        public int Id { get; set; }

        [Display(Name = "نام")]
        public string Name { get; set; }

        [Display(Name = "شماره مرحله")]
        public byte? StageNumber { get; set; }

        public virtual CustomRole Role { get; set; }

    }


}
