using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Annual_faculty_promotions.WebUI.Helpers.Filters
{
    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                var urlHelper = new UrlHelper(context.RequestContext);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                //context.HttpContext.Response.StatusDescription = "کاربر گرامی جهت استفاده از امکانات کامل سایت باید ابتدا وارد شوید.";
                //context.Result = new JsonResult
                //{
                //    Data = new
                //    {
                //        Error = "کاربر گرامی جهت استفاده از امکانات کامل سایت باید ابتدا وارد شوید.",
                //        LogInUrl = urlHelper.Action("Authentication", "Account", new { returnUrl = context.HttpContext.Request.Params["returnUrl"] })
                //    },
                //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                //};
            }
            else
            {
                base.HandleUnauthorizedRequest(context);
            }
        }
    }
}