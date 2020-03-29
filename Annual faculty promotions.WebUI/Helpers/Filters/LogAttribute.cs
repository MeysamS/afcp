using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Web.Mvc;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Core.Enums;
using Microsoft.AspNet.Identity;

namespace Annual_faculty_promotions.WebUI.Helpers.Filters
{
    public class LogAttribute : ActionFilterAttribute
    {
        public int UserId { get; set; }
        //public virtual AppUser User { get; set; }
        public Operations Operation { get; set; }
        public OperationsDetail OperationDetail { get; set; }
        public string Description { get; set; }
        
        
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    Log("OnActionExecuting", filterContext);
        //}

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    Log("OnActionExecuted", filterContext);
        //}

        //public override void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //    Log("OnResultExecuting", filterContext);
        //}

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Log("OnResultExecuted", filterContext);
        }

        public void Log(string stage, ControllerContext ctx)
        {
            ctx.HttpContext.Response.Write(
                string.Format("{0}:{1} - {2} < br/> ",
                ctx.RouteData.Values["controller"], ctx.RouteData.Values["action"], stage));
        }
    }
}