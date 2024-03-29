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
    public partial class UserAdminController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected UserAdminController(Dummy d) { }

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
        public virtual System.Web.Mvc.ActionResult SetRole()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SetRole);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult GetUserRole()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GetUserRole);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult DestroyUserRole()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.DestroyUserRole);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult GetRoleByUserId()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GetRoleByUserId);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.ActionResult CreateUserRole()
        {
            return new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CreateUserRole);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public UserAdminController Actions { get { return MVC.UserArea.UserAdmin; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "UserArea";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "UserAdmin";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "UserAdmin";
        [GeneratedCode("T4MVC", "2.0")]
        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string SetRole = "SetRole";
            public readonly string GetUsers = "GetUsers";
            public readonly string GetUserRole = "GetUserRole";
            public readonly string DestroyUserRole = "DestroyUserRole";
            public readonly string GetRoleByUserId = "GetRoleByUserId";
            public readonly string GetRoles = "GetRoles";
            public readonly string CreateUserRole = "CreateUserRole";
            public readonly string BackupDB = "BackupDB";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string SetRole = "SetRole";
            public const string GetUsers = "GetUsers";
            public const string GetUserRole = "GetUserRole";
            public const string DestroyUserRole = "DestroyUserRole";
            public const string GetRoleByUserId = "GetRoleByUserId";
            public const string GetRoles = "GetRoles";
            public const string CreateUserRole = "CreateUserRole";
            public const string BackupDB = "BackupDB";
        }


        static readonly ActionParamsClass_SetRole s_params_SetRole = new ActionParamsClass_SetRole();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_SetRole SetRoleParams { get { return s_params_SetRole; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_SetRole
        {
            public readonly string uid = "uid";
        }
        static readonly ActionParamsClass_GetUserRole s_params_GetUserRole = new ActionParamsClass_GetUserRole();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetUserRole GetUserRoleParams { get { return s_params_GetUserRole; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetUserRole
        {
            public readonly string uid = "uid";
        }
        static readonly ActionParamsClass_DestroyUserRole s_params_DestroyUserRole = new ActionParamsClass_DestroyUserRole();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_DestroyUserRole DestroyUserRoleParams { get { return s_params_DestroyUserRole; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_DestroyUserRole
        {
            public readonly string userId = "userId";
            public readonly string roleId = "roleId";
            public readonly string departmentId = "departmentId";
            public readonly string userid = "userid";
            public readonly string rolename = "rolename";
        }
        static readonly ActionParamsClass_GetRoleByUserId s_params_GetRoleByUserId = new ActionParamsClass_GetRoleByUserId();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetRoleByUserId GetRoleByUserIdParams { get { return s_params_GetRoleByUserId; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetRoleByUserId
        {
            public readonly string userId = "userId";
        }
        static readonly ActionParamsClass_CreateUserRole s_params_CreateUserRole = new ActionParamsClass_CreateUserRole();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_CreateUserRole CreateUserRoleParams { get { return s_params_CreateUserRole; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_CreateUserRole
        {
            public readonly string userid = "userid";
            public readonly string roleId = "roleId";
            public readonly string structId = "structId";
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
                public readonly string _SetRole = "_SetRole";
                public readonly string BaseUserLogin = "BaseUserLogin";
                public readonly string Index = "Index";
            }
            public readonly string _SetRole = "~/Areas/UserArea/Views/UserAdmin/_SetRole.cshtml";
            public readonly string BaseUserLogin = "~/Areas/UserArea/Views/UserAdmin/BaseUserLogin.cshtml";
            public readonly string Index = "~/Areas/UserArea/Views/UserAdmin/Index.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_UserAdminController : Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers.UserAdminController
    {
        public T4MVC_UserAdminController() : base(Dummy.Instance) { }

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
        partial void SetRoleOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int uid);

        [NonAction]
        public override System.Web.Mvc.ActionResult SetRole(int uid)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.SetRole);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "uid", uid);
            SetRoleOverride(callInfo, uid);
            return callInfo;
        }

        [NonAction]
        partial void GetUsersOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult GetUsers()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GetUsers);
            GetUsersOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void GetUserRoleOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int uid);

        [NonAction]
        public override System.Web.Mvc.ActionResult GetUserRole(int uid)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GetUserRole);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "uid", uid);
            GetUserRoleOverride(callInfo, uid);
            return callInfo;
        }

        [NonAction]
        partial void DestroyUserRoleOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int userId, int roleId, int departmentId);

        [NonAction]
        public override System.Web.Mvc.ActionResult DestroyUserRole(int userId, int roleId, int departmentId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.DestroyUserRole);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "userId", userId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "roleId", roleId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "departmentId", departmentId);
            DestroyUserRoleOverride(callInfo, userId, roleId, departmentId);
            return callInfo;
        }

        [NonAction]
        partial void GetRoleByUserIdOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int userId);

        [NonAction]
        public override System.Web.Mvc.ActionResult GetRoleByUserId(int userId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GetRoleByUserId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "userId", userId);
            GetRoleByUserIdOverride(callInfo, userId);
            return callInfo;
        }

        [NonAction]
        partial void GetRolesOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult GetRoles()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.GetRoles);
            GetRolesOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void DestroyUserRoleOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int userid, string rolename);

        [NonAction]
        public override System.Threading.Tasks.Task<System.Web.Mvc.ActionResult> DestroyUserRole(int userid, string rolename)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.DestroyUserRole);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "userid", userid);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "rolename", rolename);
            DestroyUserRoleOverride(callInfo, userid, rolename);
            return System.Threading.Tasks.Task.FromResult(callInfo as System.Web.Mvc.ActionResult);
        }

        [NonAction]
        partial void CreateUserRoleOverride(T4MVC_System_Web_Mvc_ActionResult callInfo, int userid, int roleId, int structId);

        [NonAction]
        public override System.Web.Mvc.ActionResult CreateUserRole(int userid, int roleId, int structId)
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.CreateUserRole);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "userid", userid);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "roleId", roleId);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "structId", structId);
            CreateUserRoleOverride(callInfo, userid, roleId, structId);
            return callInfo;
        }

        [NonAction]
        partial void BackupDBOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult BackupDB()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.BackupDB);
            BackupDBOverride(callInfo);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114
