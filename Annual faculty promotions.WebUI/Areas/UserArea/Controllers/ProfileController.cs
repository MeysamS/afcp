using Annual_faculty_promotions.WebUI.Areas.UserArea.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Core.Enums;
using System.Data.Entity;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.WebUI.Helpers;
using Annual_faculty_promotions.WebUI.Helpers.Filters;
using Microsoft.Owin.Security;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{
    public partial class ProfileController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IMessagingService _messagingService;
        private readonly IApplicationUserManager _userManager;
        private readonly IUserService _userService;
        private readonly IApplicationRoleManager _roleManager;
        private readonly ICartableService _cartableService;
        private readonly IArchiveService _archiveService;
        private readonly IRequestService _requestService;
        private readonly IProfileService _profileService;
        private readonly ILogService _logService;
        private readonly IDefinitionService _definitionService;
        private readonly IUnivercityStructureService _univercityStructureService;
        private readonly IUnitOfWork _unitOfWork;

        public ProfileController(IUnitOfWork unitOfWork,
            IAuthenticationManager authenticationManager,
            IApplicationUserManager userManager,
            IMessagingService messagingService,
            IApplicationRoleManager roleManager,
            IUserService userService,
            ICartableService cartableService,
            IArchiveService archiveService,
            IRequestService requestService,
            IDefinitionService definitionService,
            IUnivercityStructureService univercityStructureService,
        ILogService logService, IProfileService profileService)
        {
            _unitOfWork = unitOfWork;
            _authenticationManager = authenticationManager;
            _userManager = userManager;
            _messagingService = messagingService;
            _roleManager = roleManager;
            _userService = userService;
            _cartableService = cartableService;
            _archiveService = archiveService;
            _requestService = requestService;
            _logService = logService;
            _profileService = profileService;
            _definitionService = definitionService;
            _univercityStructureService = univercityStructureService;
        }

        //[Expire]
        public virtual ActionResult Index()
        {
            int uid = int.Parse(User.Identity.GetUserId());
            ProfileViewModel model = new ProfileViewModel();
            model.User = _userService.Where(x => x.Id == uid)
               .Include(x => x.Profile).FirstOrDefault();

            model.Messagings = _messagingService.Where(m => m.UserSenderId == uid || m.UserRecieverId == uid)
                .OrderByDescending(o => o.UpdatedDate)
                .Include(i => i.UserSender)
                .Include(i => i.UserSender.Profile)
                .Include(i => i.UserReciever)
                .Include(i => i.UserReciever.Profile).ToList();
            model.Messaging = new Messaging();
            model.Roles = _roleManager.FindUserRoles(uid).ToList();
            return View("Index", model);
        }
        public virtual ActionResult Activity()
        {
            int uId = int.Parse(User.Identity.GetUserId());
            ProfileViewModel p = new ProfileViewModel();
            p.Messagings = _messagingService.Where(m => m.UserSenderId == uId || m.UserRecieverId == uId).OrderByDescending(o => o.UpdatedDate)
                .Include(i => i.UserSender).Include(i => i.UserSender.Profile).Include(i => i.UserReciever).Include(i => i.UserReciever.Profile).ToList();
            return PartialView("_Activity", p);
        }

        [HttpPost]
        [StopSpam]
        public virtual ActionResult SendMessage(string text, int enumRoles, int recievers)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = string.Join("|", ModelState.Values.SelectMany(x => x.Errors).Select(s=>s.ErrorMessage)) });
                }
                int uId = int.Parse(User.Identity.GetUserId());
                var roleName = ((Roles)enumRoles).ToString();
                int recieverUserId = 0;
                var senderUser = _requestService.Where(x => x.UserId == uId).Include(i => i.UnivercityStructure).Include(i => i.UnivercityStructure.Parent)
                    .Include(i => i.UnivercityStructure.Parent.Parent).OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
                if (senderUser == null)
                {
                    return Json(new { isError = true, Msg = "کاربر فرستنده نا معتبر می باشد!" });
                }
                var recieverRoleId = _roleManager.FindRoleByName(roleName).Id;
                switch (roleName)
                {
                    case "MG":
                        recieverUserId = _roleManager.GetAllCustomUserRole().Where(w => w.RoleId == recieverRoleId && w.Department.Id == recievers).Select(s => s.UserId).FirstOrDefault();
                        //_roleManager.GetAllCustomUserRole().Where(w => w.RoleId == recieverRoleId && w.Department.Id == senderUser.UnivercityStructureId).Select(s => s.UserId).FirstOrDefault();
                        break;
                    case "RD":
                        recieverUserId = _roleManager.GetAllCustomUserRole().Where(w => w.RoleId == recieverRoleId && w.Department.Id == recievers).Select(s => s.UserId).FirstOrDefault();
                        //_roleManager.GetAllCustomUserRole().Where(w => w.RoleId == recieverRoleId && w.Department.Id == senderUser.UnivercityStructure.ParentId).Select(s => s.UserId).FirstOrDefault();

                        break;
                    case "RK":
                        recieverUserId = recievers;
                        //_roleManager.GetAllCustomUserRole().Where(w => w.RoleId == recieverRoleId && w.Department.Id == senderUser.UnivercityStructure.Parent.ParentId).Select(s => s.UserId).FirstOrDefault();
                        break;
                    case "MA":
                        recieverUserId = recievers;
                        //_roleManager.GetAllCustomUserRole().Where(w => w.RoleId == recieverRoleId && w.Department.Id == senderUser.UnivercityStructure.Parent.ParentId).Select(s => s.UserId).FirstOrDefault();
                        break;
                    case "User":
                        recieverUserId = recievers;
                        break;
                    default:
                        recieverUserId = 0;
                        break;
                }

                if (recieverUserId == 0)
                {
                    return Json(new { isError = true, Msg = "کاربر مقصد یافت نشد!" });
                }
                if (uId == recieverUserId)
                {
                    return Json(new { isError = true, Msg = "کاربر فرستنده و گیرنده برابر می باشند!" });
                }
                Messaging msg = new Messaging()
                {
                    Text = text,
                    UserSenderId = uId,
                    UserRecieverId = recieverUserId,
                    Readed = false
                };
                _messagingService.AddNewMessaging(msg);
                Log log = new Log()
                {
                    UserId = uId,
                    Operation = Operations.پیام,
                    OperationDetail = OperationsDetail.ارسال,
                    Description = text,
                    Messaging = msg
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { messageid = msg.Id, isError = false, Msg = "ارسال پیامک با موفقیت انجام شد!" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در ارسال پیامک!" });
            }
        }
        public virtual ActionResult RenderParialMessage()
        {
            int uid = int.Parse(User.Identity.GetUserId());
            ProfileViewModel model = new ProfileViewModel();
            model.User = _userService.Where(x => x.Id == uid)
               .Include(x => x.Profile).FirstOrDefault();

            model.Messagings = _messagingService.Where(m => m.UserSenderId == uid || m.UserRecieverId == uid)
                .OrderByDescending(o => o.UpdatedDate)
                .Include(i => i.UserSender)
                .Include(i => i.UserSender.Profile)
                .Include(i => i.UserReciever)
                .Include(i => i.UserReciever.Profile).ToList();
            model.Messaging = new Messaging();
            ViewBag.EnumRoles = new SelectList(from Roles n in Enum.GetValues(typeof(Roles))
                                               select new { Value = (int)n, Text = EnumHelper.GetDescription(n) }, "Value", "Text", -1);
            return PartialView("_Messages", model);
        }
        public virtual ActionResult RenderMessagesList(int page = 1, int pageSize = 10)
        {
            int uid = int.Parse(User.Identity.GetUserId());
            ProfileViewModel model = new ProfileViewModel();
            model.User = _userService.Where(x => x.Id == uid)
               .Include(x => x.Profile).FirstOrDefault();

            model.Messagings = _messagingService.Where(m => m.UserSenderId == uid || m.UserRecieverId == uid)
                .OrderByDescending(o => o.UpdatedDate)
                .Include(i => i.UserSender)
                .Include(i => i.UserSender.Profile)
                .Include(i => i.UserReciever)
                .Include(i => i.UserReciever.Profile).ToList();
            model.Messaging = new Messaging();
            return PartialView("_MessagesList", model);
        }
        public virtual ActionResult GetMessages(int page = 1, int pageSize = 4) //bool friend = true, bool favorite = true,
        {
            try
            {
                var uId = int.Parse(User.Identity.GetUserId());
                ProfileViewModel model = new ProfileViewModel();
                model.User = _userService.Where(x => x.Id == uId)
                   .Include(x => x.Profile).FirstOrDefault();

                model.Messagings = _messagingService.Where(m => m.UserSenderId == uId || m.UserRecieverId == uId)
                    .OrderByDescending(o => o.UpdatedDate)
                    .Include(i => i.UserSender)
                    .Include(i => i.UserSender.Profile)
                    .Include(i => i.UserReciever)
                    .Include(i => i.UserReciever.Profile).ToList();
                model.Messaging = new Messaging();
                return PartialView("_MessagesList", model);

                //PagingActivitiesViewModel pagingActivity = new PagingActivitiesViewModel()
                //{
                //    CurrentPage = page,
                //    PageSize = pageSize,
                //    TotalItemCount = lstActivity.Count(),
                //    PagedListActivity = (PagedList<ProfessorActivityViewModel>)lstActivity.ToPagedList(page, pageSize)
                //};
                //ViewData["professorRoleId"] = professorRoleId;
                //ViewData["pageCount"] = page;
                //return PartialView(MVC.Student.Default.Views._ProfessorActivities, pagingActivity);
            }
            catch (Exception ex)
            {
                return PartialView();
            }
        }
        public virtual ActionResult RenderComboRecievers(int enumvalue)
        {
            try
            {
                var roleName = ((Roles)enumvalue).ToString();
                int uid = int.Parse(User.Identity.GetUserId());
                var role = _roleManager.FindRoleByName(((Roles)enumvalue).ToString());
                if (role == null)
                {
                    new Exception();
                }
                var userRole = _roleManager.GetAllCustomUserRole().Include(u => u.Department).FirstOrDefault(r => r.RoleId == role.Id);
                if (userRole == null)
                {
                    new Exception();
                }
                switch (roleName)
                {
                    case "MG":
                        var ListMG = _univercityStructureService.Where(u => u.Level == userRole.Department.Level).ToList();
                        ViewBag.Recievers = new SelectList(ListMG, "Id", "Name", 0);
                        break;
                    case "RD":
                        var ListRD = _univercityStructureService.Where(u => u.Level == userRole.Department.Level);
                        ViewBag.Recievers =
                            new SelectList(ListRD, "Id", "Name", 0);
                        break;
                    case "RK":
                        var userRK = from a in _userService.Where(u => u.Id == userRole.UserId).Include(p => p.Profile)
                                     select new { Name = a.Profile.Name + " " + a.Profile.Family, Id = a.Id };
                        ViewBag.Recievers = new SelectList(userRK, "Id", "Name", 0);
                        break;
                    case "MA":
                        var userMA = from a in _userService.Where(u => u.Id == userRole.UserId).Include(p => p.Profile)
                                     select new { Name = a.Profile.Name + " " + a.Profile.Family, Id = a.Id };
                        ViewBag.Recievers = new SelectList(userMA, "Id", "Name", 0);
                        break;
                    case "User":
                        var user = from a in _userService.GetAllUsersAsQueryable().Include(p => p.Profile)
                                   select new { Name = a.Profile.Name + " " + a.Profile.Family, Id = a.Id };
                        ViewBag.Recievers = new SelectList(user, "Id", "Name", 0);
                        break;
                    default:
                        var userDefault = from a in _userService.GetAllUsersAsQueryable().Include(p => p.Profile)
                                          select new { Name = a.Profile.Name + " " + a.Profile.Family, Id = a.Id };
                        ViewBag.Recievers = new SelectList(userDefault, "Id", "Name", 0);
                        break;
                }

                return PartialView("_ComboRecievers");
            }
            catch (Exception ex)
            {
                return PartialView("_NotFound");
            }
        }

        public virtual ActionResult RenderPartialEditProfile()
        {
            int uid = int.Parse(User.Identity.GetUserId());
            var user = _userService.Where(x => x.Id == uid).Include(x => x.Profile).FirstOrDefault();
            if (user == null) return View("Index");
            return PartialView("_EditProfile", user.Profile);
        }

        [HttpPost]
        public virtual ActionResult UpdateProfile(Profile profile)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { isError = true, Msg = "اطلاعات کامل وارد نشده اند" });
            }
            try
            {
                if ((profile.Avatar == null) || (profile.Avatar.Length == 0))
                    profile.Avatar = "profilepicture.png";
                _profileService.EditProfile(profile);
                _unitOfWork.SaveChanges();
                //ImpersonateUserAsync();
                //AddClaim();
                return Json(new { isError = false, Msg = "پروفایل کاربری شما ویرایش شد" });
            }
            catch (Exception e)
            {
                return Json(new { isError = true, Msg = e.Message + "خطا در انجام ویرایش پروفایل" });
            }
        }

        public async Task ImpersonateUserAsync()
        {
            var context = System.Web.HttpContext.Current;
            var originalUsername = context.User.Identity.Name;
            var myuser = _userService.Find(int.Parse(User.Identity.GetUserId()));

            var impersonatedUser = await _userManager.FindByIdAsync(int.Parse(User.Identity.GetUserId()));

            var impersonatedIdentity = await _userManager.CreateIdentityAsync(impersonatedUser, DefaultAuthenticationTypes.ApplicationCookie);
            impersonatedIdentity.AddClaim(new Claim("FullName", myuser.Profile.Name + " " + myuser.Profile.Family));
            impersonatedIdentity.AddClaim(new Claim("FirstName", myuser.Profile.Name));
            impersonatedIdentity.AddClaim(new Claim("LastName", myuser.Profile.Family));
            if (myuser.Profile.Avatar != null)
                impersonatedIdentity.AddClaim(new Claim("Avatar", myuser.Profile.Avatar));
            var def = _definitionService.GetAllDefinitionsAsQueryable().FirstOrDefault();
            if (def != null)
            {
                if (def.UniversityName != null)
                    impersonatedIdentity.AddClaim(new Claim("UniversName", def.UniversityName));
                if (def.Logo != null)
                    impersonatedIdentity.AddClaim(new Claim("Logo", def.Logo));
            }
            var authenticationManager = context.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, impersonatedIdentity);
        }
        public bool RefreshClaim()
        {
            var myuser = _userService.Find(int.Parse(User.Identity.GetUserId()));
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = new ClaimsIdentity();
            if (identity == null)
                return false;
            if (myuser.Profile != null)
            {
                identity.AddClaim(new Claim("FullName", myuser.Profile.Name + " " + myuser.Profile.Family));
                identity.AddClaim(new Claim("FirstName", myuser.Profile.Name));
                identity.AddClaim(new Claim("LastName", myuser.Profile.Family));
                if (myuser.Profile.Avatar != null)
                    identity.AddClaim(new Claim("Avatar", myuser.Profile.Avatar));
            }
            var def = _definitionService.GetAllDefinitionsAsQueryable().FirstOrDefault();
            if (def != null)
            {
                if (def.UniversityName != null)
                    identity.AddClaim(new Claim("UniversName", def.UniversityName));
                if (def.Logo != null)
                    identity.AddClaim(new Claim("Logo", def.Logo));
            }
            _authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);
            return true;
        }

        public bool AddClaim()
        {
            var myuser = _userService.Find(int.Parse(User.Identity.GetUserId()));
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var claims = new List<Claim>();
            if (myuser.Profile != null)
            {
                claims.Add(new Claim("FullName", myuser.Profile.Name + " " + myuser.Profile.Family));
                claims.Add(new Claim("FirstName", myuser.Profile.Name));
                claims.Add(new Claim("LastName", myuser.Profile.Family));
                if (myuser.Profile.Avatar != null)
                    claims.Add(new Claim("Avatar", myuser.Profile.Avatar));
            }
            var def = _definitionService.GetAllDefinitionsAsQueryable().FirstOrDefault();
            if (def != null)
            {
                if (def.UniversityName != null)
                    claims.Add(new Claim("UniversName", def.UniversityName));
                if (def.Logo != null)
                    claims.Add(new Claim("Logo", def.Logo));
            }
            var id = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignIn(id);
            return true;
        }
    }
}