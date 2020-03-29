using System;
using System.Data.Entity;
using System.Threading;
using System.Web;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.Service.Implemention;
using Annual_faculty_promotions.Service.Schedule;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Security;
using Postal;
using StructureMap;
using StructureMap.Web;

namespace Annual_faculty_promotions.WebUI.Ioc
{
    public static class SmObjectFactory
    {
        private static readonly Lazy<Container> ContainerBuilder =
            new Lazy<Container>(DefaultContainer, LazyThreadSafetyMode.ExecutionAndPublication);

        public static IContainer Container
        {
            get { return ContainerBuilder.Value; }
        }

        private static Container DefaultContainer()
        {
            return new Container(ioc =>
            {
                ioc.For<IUnitOfWork>()
                      .HybridHttpOrThreadLocalScoped()
                      .Use<AfpContext>();

                ioc.For<AfpContext>().HybridHttpOrThreadLocalScoped().Use<AfpContext>();
                ioc.For<DbContext>().HybridHttpOrThreadLocalScoped().Use<AfpContext>();

                ioc.For<IUserStore<AppUser, int>>()
                    .HybridHttpOrThreadLocalScoped()
                    .Use<UserStore<AppUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>>();

                ioc.For<IRoleStore<CustomRole, int>>()
                    .HybridHttpOrThreadLocalScoped()
                    .Use<RoleStore<CustomRole, int, CustomUserRole>>();

                ioc.For<IAuthenticationManager>()
                      .Use(() => HttpContext.Current.GetOwinContext().Authentication);

                ioc.For<IApplicationSignInManager>()
                      .HybridHttpOrThreadLocalScoped()
                      .Use<ApplicationSignInManager>();

                ioc.For<IApplicationUserManager>()
                      .HybridHttpOrThreadLocalScoped()
                      .Use<ApplicationUserManager>();

                ioc.For<IApplicationRoleManager>()
                      .HybridHttpOrThreadLocalScoped()
                      .Use<ApplicationRoleManager>();

                ioc.For<IIdentityMessageService>().Use<SmsService>();
                ioc.For<IIdentityMessageService>().Use<EfEmailService>();
                ioc.For<ICustomRoleStore>()
                      .HybridHttpOrThreadLocalScoped()
                      .Use<CustomRoleStore>();

                ioc.For<ICustomUserStore>()
                      .HybridHttpOrThreadLocalScoped()
                      .Use<CustomUserStore>();


                ioc.For<IUserService>().Use<EfUserService>();
                ioc.For<IProfileService>().Use<EfProfileService>();
                ioc.For<IUnivercityStructureService>().Use<EfUnivercityStructureService>();
                ioc.For<IRequestService>().Use<EfRequestService>();
                ioc.For<IArchiveService>().Use<EfArchiveService>();
                ioc.For<IStageService>().Use<EfStageService>();
                ioc.For<ICartableService>().Use<EfCartableService>();
                ioc.For<IBaseUserService>().Use<EfBaseUserService>();
                ioc.For<IMessagingService>().Use<EfMessagingService>();
                ioc.For<ILogService>().Use<EfLogService>();
                ioc.For<IEmailService>().Use<EmailService>();
                ioc.For<IEmailIdentityService>().Use<EfEmailService>();
                ioc.For<IEmailViewRenderer>().Use<EmailViewRenderer>();
                ioc.For<IEmailParser>().Use<EmailParser>();
                ioc.For<IDefinitionService>().Use<EfDefinitionService>();

            });
        }
    }
}