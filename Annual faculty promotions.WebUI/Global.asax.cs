using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.WebUI.Ioc;
using Microsoft.AspNet.SignalR;
using StructureMap.Web.Pipeline;
using Annual_faculty_promotions.Service.Schedule;
using Annual_faculty_promotions.WebUI.Helpers;
using Annual_faculty_promotions.WebUI.Shedule;

namespace Annual_faculty_promotions.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeBinder.NullableDateTimeBinder());

            SetDbInitializer();
            //Set current Controller factory as StructureMapControllerFactory
            ControllerBuilder.Current.SetControllerFactory(new StructureMapControllerFactory());
            ScheduledTasksRegistry.Init();
            //ISchedule myTask = new EmailSmsSchedule();
            //myTask.Run();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            var httpException = exception as HttpException;

            if (httpException != null)
            {
                string action;

                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // page not found
                        action = "NotFound";
                        break;
                    case 403:
                        // forbidden
                        action = "Forbidden";
                        break;
                    case 500:
                        // server error
                        action = "HttpError500";
                        break;
                    default:
                        action = "Unknown";
                        break;
                }

                // clear error on server
                Server.ClearError();

                Response.Redirect(String.Format("~/Errors/{0}", action));
            }
            else
            {
                // this is my modification, which handles any type of an exception.
                Response.Redirect(String.Format("~/Errors/Unknown"));
            }
        }
        public class StructureMapControllerFactory : DefaultControllerFactory
        {
            protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
            {
                if (controllerType == null)
                    throw new InvalidOperationException(string.Format("Page not found: {0}", requestContext.HttpContext.Request.RawUrl));
                return SmObjectFactory.Container.GetInstance(controllerType) as Controller;
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            HttpContextLifecycle.DisposeAndClearAll();
        }
        protected void Application_End()
        {
            ScheduledTasksRegistry.End();
            ////نکته مهم این روش نیاز به سرویس پینگ سایت برای زنده نگه داشتن آن است
            ScheduledTasksRegistry.WakeUp(ConfigurationManager.AppSettings["SiteRootUrl"]);
        }
        private static void SetDbInitializer()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AfpContext, Annual_faculty_promotions.Data.Migrations.Configuration>());

            SmObjectFactory.Container.GetInstance<IUnitOfWork>().ForceDatabaseInitialize();
            SmObjectFactory.Container.Configure(x =>
            {
                x.For<Microsoft.AspNet.SignalR.IDependencyResolver>().Singleton().Add<StructureMapDependencyResolver>();
            });

            GlobalHost.DependencyResolver =
            SmObjectFactory.Container.GetInstance<Microsoft.AspNet.SignalR.IDependencyResolver>();
        }
    }
}
