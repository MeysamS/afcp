// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress "Foo hides inherited member Foo. Use the new keyword if hiding was intended." when a controller and its abstract parent are both processed
// 0114: suppress "Foo.BarController.Baz()' hides inherited member 'Qux.BarController.Baz()'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword." when an action (with an argument) overrides an action in a parent controller
#pragma warning disable 1591, 3008, 3009, 0108, 0114
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{
    public partial class DashboardController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected DashboardController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(Task<ActionResult> taskResult)
        {
            return RedirectToAction(taskResult.Result);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(Task<ActionResult> taskResult)
        {
            return RedirectToActionPermanent(taskResult.Result);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Profile()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Profile);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult Message()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Message);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public DashboardController Actions { get { return MVC.UserArea.Dashboard; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "UserArea";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Dashboard";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Dashboard";
        [GeneratedCode("T4MVC", "2.0")]
        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string LogOff = "LogOff";
            public readonly string Users = "Users";
            public readonly string Profile = "Profile";
            public readonly string GetMessages = "GetMessages";
            public readonly string Message = "Message";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string LogOff = "LogOff";
            public const string Users = "Users";
            public const string Profile = "Profile";
            public const string GetMessages = "GetMessages";
            public const string Message = "Message";
        }


        static readonly ActionParamsClass_Profile s_params_Profile = new ActionParamsClass_Profile();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Profile ProfileParams { get { return s_params_Profile; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Profile
        {
            public readonly string uid = "uid";
        }
        static readonly ActionParamsClass_Message s_params_Message = new ActionParamsClass_Message();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_Message MessageParams { get { return s_params_Message; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_Message
        {
            public readonly string messageId = "messageId";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string _Board = "_Board";
                public readonly string _Message = "_Message";
                public readonly string _Messages = "_Messages";
                public readonly string Cartable = "Cartable";
                public readonly string Index = "Index";
                public readonly string Message = "Message";
                public readonly string Profile = "Profile";
                public readonly string Users = "Users";
            }
            public readonly string _Board = "~/Areas/UserArea/Views/Dashboard/_Board.cshtml";
            public readonly string _Message = "~/Areas/UserArea/Views/Dashboard/_Message.cshtml";
            public readonly string _Messages = "~/Areas/UserArea/Views/Dashboard/_Messages.cshtml";
            public readonly string Cartable = "~/Areas/UserArea/Views/Dashboard/Cartable.cshtml";
            public readonly string Index = "~/Areas/UserArea/Views/Dashboard/Index.cshtml";
            public readonly string Message = "~/Areas/UserArea/Views/Dashboard/Message.cshtml";
            public readonly string Profile = "~/Areas/UserArea/Views/Dashboard/Profile.cshtml";
            public readonly string Users = "~/Areas/UserArea/Views/Dashboard/Users.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_DashboardController : Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers.DashboardController
    {
        public T4MVC_DashboardController() : base(Dummy.Instance) { }

        [NonAction]
        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            IndexOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void LogOffOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult LogOff()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.LogOff);
            LogOffOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void UsersOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult Users()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Users);
            UsersOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void ProfileOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int uid);

        [NonAction]
        public override System.Web.Mvc.ActionResult Profile(int uid)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Profile);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "uid", uid);
            ProfileOverride(callInfo, uid);
            return callInfo;
        }

        [NonAction]
        partial void GetMessagesOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult GetMessages()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GetMessages);
            GetMessagesOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void MessageOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, long messageId);

        [NonAction]
        public override System.Web.Mvc.ActionResult Message(long messageId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Message);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "messageId", messageId);
            MessageOverride(callInfo, messageId);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114
