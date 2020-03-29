using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annual_faculty_promotions.Core.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum Roles
    {
        /// <summary>
        /// اعضاء
        /// </summary>
        [Description("اعضاء")]
        User,
        /// <summary>
        /// مدیر گروه
        /// </summary>
        [Description("مدیر گروه")]
        MG,
        /// <summary>
        /// رئیس دانشکده
        /// </summary>
        [Description("رئیس دانشکده")]
        RD,
        /// <summary>
        /// معاون آموزشی
        /// </summary>
        [Description("معاون آموزشی")]
        MA,
        /// <summary>
        /// رئیس کارگزینی
        /// </summary>
        [Description("رئیس کارگزینی")]
        RK
    }
}
