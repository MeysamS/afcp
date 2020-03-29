using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Helpers.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Enums;
using Microsoft.AspNet.Identity;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{
    public partial class UserAdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IApplicationUserManager _userManager;
        private readonly IUserService _userService;
        private readonly ILogService _logService;
        public UserAdminController(IUnitOfWork unitOfWork, IApplicationUserManager userManager,
                                    IApplicationRoleManager roleManager,
                                    ILogService logService,
                                    IUserService userService)
        {
            _unitOfWork = unitOfWork;
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

        [HttpGet]
        [AjaxOnly]
        public virtual ActionResult SetRole(int uid)
        {
            var data = (from u in _userService.Where(x => x.Id == uid)
                        select new
                        {
                            User = u,
                            User_Roles = u.Roles,
                            User_Roles_Department = u.Roles.Select(c => new { c.Department })
                        });

            var model = data.AsEnumerable().Select(m => m.User).FirstOrDefault();

            if (model == null)
                return new HttpNotFoundResult();
            return PartialView("_SetRole", model);
        }
        public virtual ActionResult GetUsers()
        {
            try
            {
                //var users = _userService.Where(u => u.EmailConfirmed == true && u.Profile != null).Include(u => u.Profile).ToList();
                string rolesName = string.Empty;
                var data = (from u in _userService.Where(u => u.EmailConfirmed == true && u.Profile != null)
                            select new
                            {
                                User = u,
                                User_Roles = u.Roles,
                                User_Roles_Department = u.Roles.Select(c => new { c.Department }),
                                User_Profile = u.Profile
                            });

                var users = data.AsEnumerable().Select(c => c.User).ToList();
                var roles = _roleManager.GetAllCustomRolesAsQueryable().ToList();

                JArray ja = new JArray();
                foreach (var item in users)
                {
                    var itemObject = new JObject
                    {
                        {"id",item.Id},
                        {"text", item.Profile.Name + " " +item.Profile.Family},
                        {"email",item.Email},
                        {"avatar",item.Profile.Avatar},
                        {"departmentId",item.Roles.Any() ? item.Roles.FirstOrDefault().Department.Name : ""}
                    };
                    rolesName = item.Roles.Aggregate(rolesName, (current, itm) => current + (roles.Find(c => c.Id == itm.RoleId).PersianName + " ، "));
                    itemObject.Add("roleName", rolesName);
                    ja.Add(itemObject);
                    rolesName = string.Empty;
                }
                JObject jo = new JObject();
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در کاربران" });
            }
        }

        public virtual ActionResult GetUserRole(int uid)
        {
            try
            {
                var userRole = _roleManager.GetAllCustomUserRole()
                    .Where(x => x.UserId == uid)
                    .Include(x => x.Department);
                var roles = _roleManager.GetAllCustomRolesAsQueryable().ToList();
                JArray ja = new JArray();
                if (userRole != null)
                {
                    foreach (var item in userRole)
                    {
                        var itemObject = new JObject
                        {
                            {"RoleId", item.RoleId},
                            {"DepartmentId", item.Department.Id},
                            {"DepartmentName", item.Department.Name}
                        };
                        foreach (var itm in roles)
                        {
                            if (item.RoleId == itm.Id)
                            {
                                itemObject.Add("RoleName", itm.Name);
                                itemObject.Add("RolePersianName", itm.PersianName);
                            }
                        }
                        ja.Add(itemObject);
                    }
                }
                JObject jo = new JObject();
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود نقش ها" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult DestroyUserRole(int userId, int roleId, int departmentId)
        {
            try
            {
                _userService.DeleteUserRole(userId, roleId, departmentId);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.نقش,
                    OperationDetail = OperationsDetail.حذف,
                    Description = " حذف نقش شماره " + roleId + " مربوط به کاربر شماره " + userId
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "حذف شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "حذف نشد" });
            }
        }

        public virtual ActionResult GetRoleByUserId(int userId)
        {
            var roles = _roleManager.FindUserRoles(userId);

            JArray ja = new JArray();
            if (roles != null)
            {
                foreach (var item in roles)
                {
                    var itemObject = new JObject
                {
                    {"id",item.Id},
                    {"text", item.Name}
                };
                    ja.Add(itemObject);
                }
            }
            JObject jo = new JObject();
            jo.Add("rows", ja);
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }


        public virtual ActionResult GetRoles()
        {
            var roles = _roleManager.GetAllCustomRolesAsQueryable().ToList();

            JArray ja = new JArray();
            if (roles != null)
            {
                foreach (var item in roles)
                {
                    var itemObject = new JObject
                {
                    {"id",item.Id},
                    {"text", item.PersianName},
                    {"Name", item.Name}
                };
                    ja.Add(itemObject);
                }
            }
            JObject jo = new JObject();
            jo.Add("rows", ja);
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public virtual async Task<ActionResult> DestroyUserRole(int userid, string rolename)
        {
            try
            {
                await _userManager.RemoveFromRoleAsync(userid, rolename);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.نقش,
                    OperationDetail = OperationsDetail.حذف,
                    Description = " حذف نقش  " + rolename + " مربوط به کاربر شماره " + userid
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { success = true, Msg = "عملیات موفق آمیز بود" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در حذف نقش مربوط به کاربر" });
            }
        }

        [HttpPost]
        public virtual ActionResult CreateUserRole(int userid, int roleId, int structId)
        {
            try
            {
                var userInRole = _roleManager.GetAllCustomUserRole().FirstOrDefault(u => u.RoleId == roleId && u.Department.Id == structId);
                if (userInRole != null)
                {
                    return Json(new { isError = true, Msg = "این نقش قبلا نسبت داده شد! " });
                }
                var result = _roleManager.AddUserRole(userid, roleId, structId);
                if (!result)
                {
                    return Json(new { isError = true, Msg = "خطا در عملیات" });
                }
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.نقش,
                    OperationDetail = OperationsDetail.ایجاد,
                    Description = " ایجاد نقش شماره " + roleId + " مربوط به کاربر شماره " + userid
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "عملیات موفق آمیز بود" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ایجاد نقش مربوط به کاربر" });
            }
            // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public virtual ActionResult BackupDB()
        {
            try
            {
                if (!Directory.Exists(Server.MapPath("~/App_Data/BackUpDataBase/")))
                    Directory.CreateDirectory(Server.MapPath("~/App_Data/BackUpDataBase/"));
                var fileName = "afcp" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".Bak'";
                var path = Path.Combine(Server.MapPath("~/App_Data/BackUpDataBase/"), fileName);
                //_unitOfWork.GetRows<>("BACKUP DATABASE  afcp TO DISK = '" + path);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, message = "تهيه پشتيبان با موفقيت انجام شد!" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, message = ex.Message });
            }
        }

        //public ActionResult BaseUserLogin()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult FileUpload(HttpPostedFileBase excelFile)
        //{
        //    //Save the uploaded file to the disc.
        //    string savedFileName = "~/App_Data/";// +excelFile.FileName;
        //    CreateFolderIfNeeded(savedFileName);
        //    string filePath = Path.Combine(savedFileName, excelFile.FileName);
        //    var str = Server.MapPath(filePath);
        //    excelFile.SaveAs(Server.MapPath(filePath));

        //    // Call function to place temporary file into database
        //    SaveFileToDatabase(filePath);

        //    // Optional: Delete temporary Excel file from server
        //    return View("BaseUserLogin");
        //    //RedirectToAction("AuthenticatedUsers");
        //}

        //private void SaveFileToDatabase(string savedFileName)
        //{
        //    String connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0;", Server.MapPath(savedFileName));
        //    //int index = (savedFileName).LastIndexOf('.');
        //    //int indexs = (savedFileName).LastIndexOf('\\');
        //    //string name = (savedFileName).Substring(indexs + 1);
        //    //string sheet = (savedFileName).Substring(indexs + 1, index - (indexs + 1));
        //    {
        //        using (OleDbConnection connection = new OleDbConnection(connectionString))
        //        {
        //            using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", connection))
        //            {
        //                connection.Open();

        //                using (OleDbDataReader dReader = cmd.ExecuteReader())
        //                {
        //                    using (SqlBulkCopy sqlBulk = new SqlBulkCopy(System.Configuration.ConfigurationManager.ConnectionStrings["connectionStringName"].ConnectionString))
        //                    {
        //                        //Give your Destination table name 
        //                        sqlBulk.DestinationTableName = "BaseUserLogin";
        //                        sqlBulk.WriteToServer(dReader);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //private string GetLocalFilePath(string saveDirectory, FileUpload fileUploadControl)
        //{
        //    //System.Web.UI.WebControls.WebControl
        //    string filePath = Path.Combine(saveDirectory, fileUploadControl.FileName);

        //    fileUploadControl.SaveAs(filePath);

        //    return filePath;
        //}

        //private bool CreateFolderIfNeeded(string path)
        //{
        //    bool result = true;
        //    if (!Directory.Exists(path))
        //    {
        //        try
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        catch (Exception)
        //        {
        //            /*TODO: You must process this exception.*/
        //            result = false;
        //        }
        //    }
        //    return result;
        //}
    }
}