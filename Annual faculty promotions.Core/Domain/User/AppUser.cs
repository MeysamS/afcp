using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Annual_faculty_promotions.Core.Domain.User
{
    public class AppUser : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public string Signture { get; set; }
        public bool IsOnline { get; set; }
        public virtual Profile Profile { get; set; }
        public int BaseUserLoginId { get; set; }
        //public virtual BaseUserLogin BaseUserLogin { get; set; }
        public virtual ICollection<Cartable> UserSenderCartables { get; set; }
        public virtual ICollection<Cartable> UserReciveCartables { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<Archive> ChiefRequests { get; set; }
        public virtual ICollection<Archive> ChiefCommiteRequests { get; set; }
        public virtual ICollection<Access> Accesses { get; set; }
     public virtual ICollection<Archive> Archives { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
        public virtual ICollection<Messaging> UserSenderMessagings { get; set; }
        public virtual ICollection<Messaging> UserRecieverMessagings { get; set; }
    }
}
