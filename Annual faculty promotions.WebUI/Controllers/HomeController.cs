using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Helpers.Filters;
using Annual_faculty_promotions.WebUI.Models;

namespace Annual_faculty_promotions.WebUI.Controllers
{
    public partial class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IApplicationUserManager _userManager;
        private readonly IUnitOfWork _uow;
        public HomeController(IUnitOfWork uow, IApplicationUserManager userManager, IUserService userService)
        {
            _uow = uow;
            _userService = userService;
            _userManager = userManager;
        }

        //[Expire]
        //public virtual ActionResult Index()
        //{
        //    _userService.GetAllUsers();
        //    return View();
        //}

      
        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public virtual async Task<ActionResult> IsEmailExist(string email)
        {
            var u = await _userManager.FindByEmailAsync(email);
            if (u != null) return Json(false);
            return Json(true);
        }

        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public virtual ActionResult CompareCaptcha(string captcha)
        {
            if (captcha != (string)Session["captcha"]) return Json(false);
            return Json(true);
        }

        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public virtual async Task<ActionResult> IsEmailConfirmed(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
                if (!await _userManager.IsEmailConfirmedAsync(user.Id))
                    return Json(false);
            return Json(true);
        }


        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public virtual ActionResult HasValidCodeMeli(long codeMeli)
        {
            var data = _userService.Where(c => c.CodeMeli == codeMeli && c.Active == false).ToList();
            if (!data.Any())
                return Json(false);
            return Json(true);
        }

        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public virtual ActionResult HasValidCodeEstekhdami(string codeEstekhdam)
        {
            var data = _userService.Where(c => c.CodeEstekhdam == codeEstekhdam && c.Active == false).ToList();
            if (!data.Any())
                return Json(false);
            return Json(true);
        }

        public virtual Captcha.CaptchImageAction Image()
        {
            string randomText = SelectRandomWord(4);
            Session["captcha"] = randomText;
            if (HttpContext.Session != null) HttpContext.Session["RandomText"] = randomText;
            return new Captcha.CaptchImageAction()
            {
                BackgroundColor = System.Drawing.Color.LightGray,
                RandomTextColor = System.Drawing.Color.Black,
                RandomText = randomText
            };
        }

        public virtual JsonResult ImageJson()
        {
            try
            {
                string randomText = SelectRandomWord(4);
                Session["captcha"] = randomText;
                if (HttpContext.Session != null) HttpContext.Session["RandomText"] = randomText;
                var data = new Captcha.CaptchImageAction()
                {
                    BackgroundColor = System.Drawing.Color.LightGray,
                    RandomTextColor = System.Drawing.Color.Black,
                    RandomText = randomText
                };
                return Json(new { isError = false, data = data, Msg = "" });
            }
            catch
            {
                return Json(new { isError = true, Msg = "خطا در بازیابی یکبار رمز!" });
            }
        }

        private string SelectRandomWord(int numberOfChars)
        {
            if (numberOfChars > 36)
            {
                throw new InvalidOperationException("Random Word Characters cannot be greater than 36");
            }
            char[] columns = new char[36];
            for (int charPos = 65; charPos < 65 + 26; charPos++)
                columns[charPos - 65] = (char)charPos;
            for (int intPos = 48; intPos <= 57; intPos++)
                columns[26 + (intPos - 48)] = (char)intPos;
            StringBuilder randomBuilder = new StringBuilder();
            Random randomSeed = new Random();
            for (int incr = 0; incr < numberOfChars; incr++)
            {
                randomBuilder.Append(columns[randomSeed.Next(36)].ToString().ToLower());
            }
            return randomBuilder.ToString();
        }
    }
}