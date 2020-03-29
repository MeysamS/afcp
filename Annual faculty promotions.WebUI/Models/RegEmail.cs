
using Postal;

namespace Annual_faculty_promotions.WebUI.Models
{
    public class RegEmail:Email
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

  
}