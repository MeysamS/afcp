using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;

namespace Annual_faculty_promotions.Core.Domain
{
    public class UnivercityStructure:Entity<int>
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "نام وارد نشده است!")]
        public string Name { get; set; }
        
        [Display(Name = "والد")]
        public bool HasChild { get; set; }
        public int? ParentId { get; set; }
        public UnivercityStructure Parent { get; set; }
        public byte Level { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}
