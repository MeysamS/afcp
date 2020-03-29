using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Models
{
    public class BaseUserLoginViewModel
    {
        public long CodeMeli { get; set; }
        public string CodeEstekhdam { get; set; }
    }

    public class BaseUserLoginComparer : IEqualityComparer<BaseUserLoginViewModel>
    {
        public bool Equals(BaseUserLoginViewModel x, BaseUserLoginViewModel y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.CodeMeli == y.CodeMeli || x.CodeEstekhdam == y.CodeEstekhdam;
        }

        public int GetHashCode(BaseUserLoginViewModel baseUserLogin)
        {
            if (Object.ReferenceEquals(baseUserLogin, null))
                return 0;

            int hashCodeMeli = baseUserLogin.CodeMeli.GetHashCode();

            int hashCodeEstekhdam = baseUserLogin.CodeEstekhdam.GetHashCode();

            return hashCodeMeli ^ hashCodeEstekhdam;
        }
    }
}