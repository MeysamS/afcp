using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Annual_faculty_promotions.WebUI.Helpers.Filters
{
    public class ExpireAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (DateTime.Now.Year > 2016)
            //{
            //    filterContext.Result = new ViewResult
            //    {
            //        ViewName = MVC.Shared.Views.Expire
            //    };
            //}
            base.OnActionExecuting(filterContext);
        }
    }
}