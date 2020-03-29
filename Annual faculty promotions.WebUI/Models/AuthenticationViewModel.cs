using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Annual_faculty_promotions.WebUI.Models
{
    public class AuthenticationViewModel
    {
        public LoginViewModel LoginViewModel { get; set; }
        public RegisterViewModel RegisterViewModel { get; set; }
    }
}