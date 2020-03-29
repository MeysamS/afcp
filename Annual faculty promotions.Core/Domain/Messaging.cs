using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Domain.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annual_faculty_promotions.Core.Domain
{
    public class Messaging : AuditableEntity<long>
    {

        //[Required(ErrorMessage = "متن پیام وارد نشده است")]
        public string Text { get; set; }
        public bool Readed { get; set; }
        public int UserSenderId { get; set; }
        public virtual AppUser UserSender { get; set; }
        public int UserRecieverId { get; set; }
        public virtual AppUser UserReciever { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
    }
}
