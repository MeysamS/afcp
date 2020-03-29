using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Helpers.Filters;
using Annual_faculty_promotions.WebUI.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{
    public partial class RolesAdminController : Controller
    {

        private readonly IApplicationRoleManager _roleManager;
        private readonly IApplicationUserManager _userManager;
        private readonly IUserService _userService;
        private readonly ILogService _logService;
        public RolesAdminController(IApplicationUserManager userManager,
                                    IApplicationRoleManager roleManager,
                                    IUserService userService,
                                    ILogService logService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _logService = logService;
        }

        //[Expire]
        public virtual ActionResult Index()
        {
            return View();
        }


        public virtual ActionResult GetRoles(int page = 1, int pageSize = 10)
        {
            try
            {
                var roles = _roleManager.GetAllCustomRolesAsQueryable()
                    .OrderBy(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                JArray ja = new JArray();

                foreach (var item in roles)
                {
                    var itemObject = new JObject
                {
                    {"Id",item.Id},
                    {"Name",item.Name},
                    {"PersianName",item.PersianName}
                };
                    ja.Add(itemObject);
                }
                JObject jo = new JObject();
                jo.Add("total", _roleManager.GetAllCustomRolesAsQueryable().Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود نقش" });
            }
        }

        public virtual ActionResult DatalistRole(int uid)
        {
            try
            {
                var roles = _roleManager.GetAllCustomRolesAsQueryable().ToList();
                var userrole = _roleManager.GetAllCustomUserRole().Where(x => x.UserId == uid).ToList();
                JArray ja = new JArray();

                foreach (var item in roles)
                {
                    var itemObject = new JObject
                {
                    {"id",item.Id},
                    {"Name",item.Name},
                    {"text",item.PersianName},
                };

                    foreach (var itm in userrole)
                    {
                        if (item.Id == itm.RoleId)
                            itemObject.Add("checked", "True");
                    }

                    ja.Add(itemObject);
                }
                JObject jo = new JObject();
                jo.Add("total", _roleManager.GetAllCustomRolesAsQueryable().Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود نقش کاربران" });
            }
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public virtual async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            try
            {
                var existName = _roleManager.FindByNameAsync(roleViewModel.Name);
                if (existName.Result != null)
                    return Json(new { isError = true, Msg = "خطا در انجام عملیات : نام نقش(لاتین) وجود دارد " });

                var role = new CustomRole(roleViewModel.Name, roleViewModel.PersianName);
                var roleresult = await _roleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    return Json(new { isError = true, Msg = roleresult.Errors.First() });
                }
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.نقش,
                    OperationDetail = OperationsDetail.ایجاد,
                    Description = " ایجاد نقش به نام" + roleViewModel.Name
                };
                _logService.AddNewLog(log);
                return Json(new { isError = false, Msg = "ثبت شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ثبت نقش برای کاربر" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual async Task<ActionResult> Delete(int? id, string deleteUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر" });
                }
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await _roleManager.FindByIdAsync(id.Value);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await _roleManager.DeleteAsync(role);
                }
                else
                {
                    result = await _roleManager.DeleteAsync(role);
                }
                if (!result.Succeeded)
                {
                    return Json(new { isError = true, Msg = result.Errors.First() });
                }
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.نقش,
                    OperationDetail = OperationsDetail.حذف,
                    Description = " حذف نقش به شماره" + id
                };
                _logService.AddNewLog(log);
                return Json(new { isError = false, Msg = "حذف شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در حذف نقش" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual async Task<ActionResult> Edit(RoleViewModel roleModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "خطا در انجام عملیات : ورودی نامعتبر" });
                }
                var role = await _roleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;

                var existName = _roleManager.FindByNameAsync(roleModel.Name);
                if (existName != null)
                    return Json(new { isError = true, Msg = "خطا در انجام عملیات : نام نقش(لاتین) وجود دارد " });

                role.PersianName = roleModel.PersianName;
                await _roleManager.UpdateAsync(role);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.نقش,
                    OperationDetail = OperationsDetail.ویرایش,
                    Description = " ویرایش نقش " + roleModel.Name
                };
                _logService.AddNewLog(log);

                return Json(new { isError = false, Msg = "تغییرات اعمال شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ویرایش نقش" });
            }
        }
    }
}