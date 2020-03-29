using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Areas.UserArea.Models;
using Annual_faculty_promotions.WebUI.Helpers.Filters;
using Annual_faculty_promotions.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Annual_faculty_promotions.Data;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{

    public partial class DashboardController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IUserService _userService;
        private readonly IMessagingService _messagingService;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IUnitOfWork _uow;
        public DashboardController(IUnitOfWork uow,
                                 IAuthenticationManager authenticationManager, IUserService userService, IMessagingService messagingService, IApplicationRoleManager roleManager)
        {
            _authenticationManager = authenticationManager;
            _userService = userService;
            _messagingService = messagingService;
            _roleManager = roleManager;
            _uow = uow;
        }

        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LogOff()
        {
            var uId = int.Parse(User.Identity.GetUserId());
            var user = _userService.Find(uId);
            user.IsOnline = false;
            _userService.EditUser(user);
            _uow.SaveChanges();
            _authenticationManager.SignOut();
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        [Authorize(Roles = "Admin")]
        public virtual ActionResult Users()
        {
            var model = _userService.Where(x => x.Profile != null)
                .Include(x => x.Profile)
                .Include(x => x.Requests).ToList();
            return View(model);
        }

        public virtual ActionResult Profile(int uid)
        {
            ProfileViewModel model = new ProfileViewModel();

            model.User = _userService.Where(x => x.Id == uid)
                .Include(x => x.Profile).FirstOrDefault();
            model.Messagings = _messagingService.Where(x => x.UserSenderId == uid || x.UserRecieverId == uid)
                .Include(x => x.UserSender)
                .Include(x => x.UserReciever).ToList();
            model.Roles = _roleManager.FindUserRoles(uid).ToList();
            return View(model);
        }

        public virtual ActionResult GetMessages()
        {
            int uid = int.Parse(User.Identity.GetUserId());
            var model =
                _messagingService.Where(x => x.UserRecieverId == uid && x.Readed == false)
                    .OrderByDescending(o => o.UpdatedDate)
                    .Include(x => x.UserReciever)
                    .Include(x => x.UserReciever.Profile)
                    .Include(x => x.UserSender)
                    .Include(x => x.UserSender.Profile).ToList();
            return PartialView("_MessagesNotifications", model);
        }
        public virtual ActionResult Message(long messageId)
        {
            try
            {
                int uid = int.Parse(User.Identity.GetUserId());
                var message = _messagingService.Find(messageId);
                message.Readed = true;
                _messagingService.Edit(message);
                var model =
                    _messagingService.Where(x => x.Id == messageId)
                        .Include(x => x.UserReciever)
                        .Include(x => x.UserReciever.Profile)
                        .Include(x => x.UserSender)
                        .Include(x => x.UserSender.Profile).FirstOrDefault();
                _uow.SaveChanges();
                return View(model);

            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }
    }
}