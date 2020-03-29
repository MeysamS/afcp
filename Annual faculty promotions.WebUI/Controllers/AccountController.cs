using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Core.Enums;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Helpers;
using Annual_faculty_promotions.WebUI.Helpers.Filters;
using Annual_faculty_promotions.WebUI.Models;
using Elmah;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Postal;

// meysam 
namespace Annual_faculty_promotions.WebUI.Controllers
{
    [Authorize]
    //[Expire]
    public partial class AccountController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IApplicationUserManager _userManager;
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;
        private readonly IIdentityMessageService _identityMessageService;
        private readonly IEmailService _emailService;
        private readonly IEmailIdentityService _emailIdentityService;
        private readonly IDefinitionService _definitionService;
        private readonly IBaseUserService _baseuserService;
        private readonly IUnitOfWork _uow;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IUnivercityStructureService _univercity;
        public AccountController(IApplicationUserManager userManager,
                                 IApplicationSignInManager signInManager,
                                 IAuthenticationManager authenticationManager,
                                 IProfileService profileService, IUserService userService,
                                 IIdentityMessageService identityMessageService,
                                 IEmailService emailService,
                                 IEmailIdentityService emailIdentityService,
                                 IDefinitionService definitionService,
                                 IBaseUserService baseuserService,
                                IUnivercityStructureService univercity,
                                IApplicationRoleManager roleManager,
                                 IUnitOfWork uow)
        {
            _uow = uow;
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationManager = authenticationManager;
            _profileService = profileService;
            _identityMessageService = identityMessageService;
            _userService = userService;
            _emailService = emailService;
            _emailIdentityService = emailIdentityService;
            _definitionService = definitionService;
            _baseuserService = baseuserService;
            _roleManager = roleManager;
            _univercity = univercity;
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public virtual async Task<ActionResult> ConfirmEmail(int? userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(userId.Value, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }


        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await _authenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await _signInManager.ExternalSignInAsync(loginInfo, false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _authenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false, false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public virtual ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public virtual ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, code }, Request.Url.Scheme);
                await _userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public virtual ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }



        [AllowAnonymous]
        public virtual ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindAsync(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("InvalidUser", "نام کاربری یا رمز عبور اشتباه است");
                return View();
            }
            if (await SignInAsync(user, model.RememberMe))
            {
                user.IsOnline = true;
                _userService.EditUser(user);
                _uow.SaveChanges();
                return RedirectToAction("CheckProfile");
            }
            ModelState.AddModelError("", "ورودی نامعتبر میباشد!");
            return View(model);
        }

        public virtual ActionResult CheckProfile()
        {
            if (!_profileService.HasProfile(int.Parse(User.Identity.GetUserId())))
            {
                return RedirectToAction("AddProfile", "Account");
            }
            return RedirectToAction("Index", "Cartable", new { area = "UserArea" });
        }

        private async Task<bool> SignInAsync(AppUser user, bool isPersistent)
        {
            var myuser = await _userManager.FindCustomByIdAsync(user.Id);
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
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
            _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
            return true;
        }

        // POST: /Account/LogOff
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
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public virtual ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        BaseUserLogin baseuserlogin =_userService.Where(x => x.CodeMeli == model.CodeMeli && x.CodeEstekhdam == model.CodeEstekhdam).FirstOrDefault();
                        if (baseuserlogin == null)
                        {
                            ModelState.AddModelError("", "اطلاعات پایه کاربری وجود ندارد");
                            return View(model);
                        }
                        if (baseuserlogin.Active)
                        {
                            ModelState.AddModelError("", "شماره شناسایی قبلا استفاده شده است");
                            return View(model);
                        }
                        //throw new Exception("اطلاعات پایه کاربری وجود ندارد");
                        var user = new AppUser
                        {
                            UserName = model.Email,
                            Email = model.Email,
                            BaseUserLoginId = baseuserlogin.BaseUserLoginId
                        };
                        var result = await _userManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            baseuserlogin.Active = true;
                            _baseuserService.Edit(baseuserlogin);
                            var roleid = _roleManager.FindRoleByName(Roles.User.ToString()).Id;
                            var departman = _univercity.Where(x => x.Level == 1).SingleOrDefault();
                            if (departman != null)
                                _roleManager.AddUserRole(user.Id, roleid, departman.Id);
                            _uow.SaveChanges();
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action(MVC.Account.ActionNames.ConfirmEmail, MVC.Account.Name, new { userId = user.Id, code }, Request.Url.Scheme);
                            ViewBag.Link = callbackUrl;
                            //_emailIdentityService.SendEmailWithPostal("Reg.Html", user.Email, "", "", "تائید حساب کاربری ", "تائید حساب کاربری ", "جهت فعال سازی حساب کاربری خود بر روی لینک زیر کلیک کنید: <a href=\"" + callbackUrl + "\">link</a>");
                            var def = _definitionService.GetAllDefinitionsAsQueryable().FirstOrDefault();
                            dynamic email = new Email("Reg.Html");
                            email.To = user.Email;
                            email.Title = "تائید حساب کاربری ";
                            email.Subject = (def == null ? "تائید حساب کاربری " : "تائید حساب کاربری " + def.UniversityName);
                            email.Body = "جهت فعال سازی حساب کاربری خود بر روی لینک زیر کلیک کنید";
                            email.Link = callbackUrl;
                            //if (!string.IsNullOrWhiteSpace(email.To.ToString()))
                            //    email.Send();
                            scope.Complete();
                            return View("DisplayEmail");
                        }
                        addErrors(result);
                    }
                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                        ModelState.AddModelError("", "عملیات با مشکل مواجه شد! " + ex.Message);
                        return View(model);
                    }
                    finally
                    {
                        //scope.Dispose();
                    }
                }
            }
            
            return View(model);
        }

        private void addErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult redirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public virtual ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public virtual ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public virtual async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await _signInManager.GetVerifiedUserIdAsync();
            /*if (userId == null)
            {
                return View("Error");
            }*/
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await _signInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, model.ReturnUrl });
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public virtual async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await _signInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(await _signInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await _userManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, false, model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public virtual ActionResult AddProfile()
        {
            var Profile = _profileService.Find(int.Parse(User.Identity.GetUserId()));
            return View("AddProfile", Profile);
        }

        [HttpPost]
        [Authorize]
        public virtual ActionResult AddProfile(Profile profile)
        {
            if (ModelState.IsValid)
            {
                if ((profile.Avatar == null) || (profile.Avatar.Length == 0))
                    profile.Avatar = "profilepicture.png";
                _profileService.AddNewProfile(profile);
                _uow.SaveChanges();
                return RedirectToAction("Index", "Cartable", new { area = "UserArea" });
            }
            return View(profile);

        }

        //[Route("User/Profile")]
        [HttpGet]
        [Authorize]
        public virtual ActionResult UserProfile(int? opType)
        {
            ViewBag.opType = opType == null ? 3 : opType;
            return View();
        }
    }
}