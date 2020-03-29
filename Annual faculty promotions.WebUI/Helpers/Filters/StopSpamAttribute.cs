using System;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web.Caching;

namespace Annual_faculty_promotions.WebUI.Helpers.Filters
{
    public class StopSpamAttribute: ActionFilterAttribute
    {
        // حداقل زمان مجاز بین درخواست‌ها برحسب ثانیه
        public int DelayRequest = 7;

        // پیام خطایی که در صورت رسیدن درخواست غیرمجاز باید صادر کنیم
        public string ErrorMessage = "درخواست‌های شما در مدت زمان معقولی صورت نگرفته است.";

        // خصوصیتی برای تعیین اینکه آدرس درخواست هم به شناسه یکتا افزوده شود یا خیر
        public bool AddAddress = true;

        // باید نتیجه اجرای اکشن را بررسی کند یا خیر
        public bool CheckResult = true;

        string hashValue = "";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var cache = System.Web.HttpContext.Current.Cache;

            hashValue = this.MakeKey(context.HttpContext);

            // ابتدا چک می‌کنیم که آیا شناسه‌ی یکتای درخواست در کش موجود نباشد
            if (cache[hashValue] != null)
            {

                // یک خطا اضافه می‌کنیم ModelState اگر موجود بود یعنی کمتر از زمان موردنظر درخواست مجددی صورت گرفته و به
                context.Controller.ViewData.ModelState.AddModelError("ExcessiveRequests", ErrorMessage);
            }
            else
            {
                // اگر موجود نبود یعنی درخواست با زمانی بیشتر از مقداری که تعیین کرده‌ایم انجام شده
                // پس شناسه درخواست جدید را با پارامتر زمانی که تعیین کرده بودیم به شیئ کش اضافه می‌کنیم
                cache.Add(hashValue, true, null, DateTime.Now.AddSeconds(DelayRequest), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (this.CheckResult && context.Controller.ViewBag.ExecuteResult != null && context.Controller.ViewBag.ExecuteResult != true)
            {
                var cache = System.Web.HttpContext.Current.Cache;
                if (cache[hashValue] != null)
                    cache.Remove(hashValue);
            }

            base.OnActionExecuted(context);
        }


        private string MakeKey(System.Web.HttpContextBase context)
        {

            // درسترسی به شئی درخواست
            var request = context.Request;

            // دسترسی به شیئ کش
            var cache = context.Cache;

            // کاربر IP بدست آوردن
            var IP = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;

            // در اینجا آدرس درخواست جاری را تعیین می‌کنیم
            var targetInfo = (this.AddAddress) ? (request.RawUrl + request.QueryString) : "";

            // شناسه یکتای درخواست
            var Uniquely = String.Concat(IP, targetInfo);


            // در اینجا با کمک هش یک امضا از شناسه‌ی درخواست ایجاد می‌کنیم
            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(Uniquely)).Select(s => s.ToString("x2")));

            return hashValue;
        }
    }
}