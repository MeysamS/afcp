using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Service.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;


namespace Annual_faculty_promotions.Service.Implemention
{
    public class ApplicationUserManager
        : UserManager<AppUser, int>, IApplicationUserManager
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IIdentityMessageService _emailService;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IIdentityMessageService _smsService;
        private readonly IUserService _userService;
        private readonly IUserStore<AppUser, int> _store;

        public ApplicationUserManager(IUserStore<AppUser, int> store,
            IApplicationRoleManager roleManager,
            IDataProtectionProvider dataProtectionProvider,
            IIdentityMessageService smsService,
            IIdentityMessageService emailService,IUserService userService)
            : base(store)
        {
            _store = store;
            _roleManager = roleManager;
            _dataProtectionProvider = dataProtectionProvider;
            _smsService = smsService;
            _emailService = emailService;
            _userService = userService;
            CreateApplicationUserManager();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(AppUser applicationUser)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await CreateIdentityAsync(applicationUser, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("Email", applicationUser.Email));
            if (applicationUser.Profile != null)
                userIdentity.AddClaim(new Claim("FullName", applicationUser.Profile.Name + " " + applicationUser.Profile.Family));
            return userIdentity;
        }

        public async Task<bool> HasPassword(int userId)
        {
            var user = await FindByIdAsync(userId);
            return user != null && user.PasswordHash != null;
        }

        public async Task<bool> HasPhoneNumber(int userId)
        {
            var user = await FindByIdAsync(userId);
            return user != null && user.PhoneNumber != null;
        }


        public async Task<AppUser> FindCustomByIdAsync(int userId)
        {
            AppUser user= await  _userService.GetAllUsersAsQueryable().Where(x=>x.Id==userId).Include(x => x.Profile).FirstOrDefaultAsync();
            return user;
        }

        public AppUser FindById(int userId)
        {
            return FindById(userId);
        }


   
        public Func<CookieValidateIdentityContext, Task> OnValidateIdentity()
        {
            return SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, AppUser, int>(
                         validateInterval: TimeSpan.FromMinutes(30),
                         regenerateIdentityCallback: (manager, user) => generateUserIdentityAsync(manager, user),
                         getUserIdCallback: (id) => (Int32.Parse(id.GetUserId())));
        }

        public void SeedDatabase()
        {
            //const string name = "admin@example.com";
            //const string password = "Admin@123456";
            //const string roleName = "Admin";

            ////Create Role Admin if it does not exist
            //var role = _roleManager.FindRoleByName(roleName);
            //if (role == null)
            //{
            //    role = new CustomRole(roleName,"مدیر سایت");
            //    var roleresult = _roleManager.CreateRole(role);
            //}

            //var user = this.FindByName(name);
            //if (user == null)
            //{
            //    user = new AppUser { UserName = name, Email = name };
            //    var result = this.Create(user, password);
            //    result = this.SetLockoutEnabled(user.Id, false);
            //}

            //// Add user admin to Role Admin if not already added
            //var rolesForUser = this.GetRoles(user.Id);
            //if (!rolesForUser.Contains(role.Name))
            //{
            //    var result = this.AddToRole(user.Id, role.Name);
            //}
        }

        private void CreateApplicationUserManager()
        {
            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<AppUser, int>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            this.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            this.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<AppUser, int>
            {
                MessageFormat = "Your security code is: {0}"
            });
            this.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<AppUser, int>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            this.EmailService = _emailService;
            this.SmsService = _smsService;

            if (_dataProtectionProvider != null)
            {
                var dataProtector = _dataProtectionProvider.Create("ASP.NET Identity");
                this.UserTokenProvider = new DataProtectorTokenProvider<AppUser, int>(dataProtector);
            }
        }

        private async Task<ClaimsIdentity> generateUserIdentityAsync(ApplicationUserManager manager, AppUser applicationUser)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(applicationUser, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}