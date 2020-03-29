using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Annual_faculty_promotions.Core.Domain.User
{
    public class BaseUserLogin
    {
        public int BaseUserLoginId { get; set; }

        [Index("IX_CodeMeli_BaseInfo", IsUnique = true)]
        public long CodeMeli { get; set; }

        //[Index("IX_CodeEstekhdam_BaseInfo", IsUnique = true)]
        public string CodeEstekhdam { get; set; }
        public bool Active { get; set; }

        //public virtual AppUser User { get; set; }

    }
}
