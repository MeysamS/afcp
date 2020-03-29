using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Areas.UserArea.Models;
using Annual_faculty_promotions.WebUI.Helpers.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{
    [Authorize(Roles = "Admin")]
    public partial class BaseInfoUsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IApplicationRoleManager _roleManager;
        //private readonly IApplicationUserManager _userManager;
        //private readonly IUserService _userService;
        private readonly IBaseUserService _baseuserService;
        private readonly ILogService _logService;

        public BaseInfoUsersController(IUnitOfWork unitOfWork,
            //IApplicationUserManager userManager,
            //                        IApplicationRoleManager roleManager,
            //                        IUserService userService,
            IBaseUserService baseuserService,
            ILogService logService)
        {
            _unitOfWork = unitOfWork;
            //_userManager = userManager;
            //_roleManager = roleManager;
            //_userService = userService;
            _baseuserService = baseuserService;
            _logService = logService;
        }

        //[Expire]
        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult FileUpload(HttpPostedFileBase excelFile)
        {
            try
            {
                var data = new byte[256];
                excelFile.InputStream.Read(data, 0, 256);
                var detector = new MimeTypeDetector();
                var mimeType = detector.GetMimeType(data);
                //if (CheckWhiteList(mimeType) == false)
                //{
                //    ModelState.AddModelError("InvalidFileContent", "فایل بارگزاری شده مورد پذیرش نیست.");
                //}
                //return View();
                string savedFileName = "~/App_Data/";
                CreateFolderIfNeeded(savedFileName);
                string filePath = Path.Combine(savedFileName, Path.GetFileName(excelFile.FileName));
                var str = Server.MapPath(filePath);
                excelFile.SaveAs(Server.MapPath(filePath));
                SaveFileToDatabase(filePath);
                return Json(new { isError = false, Msg = "عمل انتقال به درستی انجام شد" });
                //return View("Index");
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در انتقال کاربران" });
            }
        }

        private void SaveFileToDatabase(string savedFileName)
        {
            try
            {
                String connectionString =
                    string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0;",
                        Server.MapPath(savedFileName));
                int index = (savedFileName).LastIndexOf('.');
                int indexs = (savedFileName).LastIndexOf('\\');
                string name = (savedFileName).Substring(indexs + 1);
                string sheet = (savedFileName).Substring(indexs + 1, index - (indexs + 1));

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    using (
                        OleDbCommand cmd = new OleDbCommand("SELECT CodeMeli,CodeEstekhdam FROM [Sheet1$]", connection))
                    {
                        connection.Open();
                        using (OleDbDataReader dReader = cmd.ExecuteReader())
                        {
                            var lstExcel = new List<BaseUserLoginViewModel>();
                            while (dReader != null && dReader.Read())
                            {
                                BaseUserLoginViewModel bulvm = new BaseUserLoginViewModel
                                {
                                    CodeMeli = Convert.ToInt64(dReader["CodeMeli"]),
                                    CodeEstekhdam = Convert.ToString(dReader["CodeEstekhdam"])
                                };
                                lstExcel.Add(bulvm);
                            }
                            var lstAllbaseuser =
                                _baseuserService.GetAllBaseUserLogin()
                                    .Select(
                                        s =>
                                            new BaseUserLoginViewModel
                                            {
                                                CodeMeli = s.CodeMeli,
                                                CodeEstekhdam = s.CodeEstekhdam
                                            })
                                    .ToList();
                           
                            var lstNewbaseuser = lstExcel.Except(lstAllbaseuser, new BaseUserLoginComparer()).ToList();
                           
                            var lstbul = lstNewbaseuser.Select(item => new BaseUserLogin()
                            {
                                CodeMeli = item.CodeMeli, CodeEstekhdam = item.CodeEstekhdam
                            }).ToList();
                            lstbul.ForEach(b => _baseuserService.AddNewBaseUserLogin(b));
                            Log log = new Log()
                            {
                                UserId = int.Parse(User.Identity.GetUserId()),
                                Operation = Operations.اطلاعات_پایه_کاربر,
                                OperationDetail = OperationsDetail.ایجاد,
                                Description = "اضافه کردن اطلاعات پایه کاربران مجاز به سیستم از طریق فایل "
                            };
                            _logService.AddNewLog(log);
                            _unitOfWork.SaveChanges();

                            //using (SqlBulkCopy sqlBulk = new SqlBulkCopy(System.Configuration.ConfigurationManager.ConnectionStrings["cnnString"].ConnectionString))
                            //{
                            //    sqlBulk.DestinationTableName = "BaseUserLogins";
                            //    sqlBulk.WriteToServer((DataTable)lstNewBaseUser);
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("خطا در ثبت اطلاعات فایل");
            }
        }

        private string GetLocalFilePath(string saveDirectory, FileUpload fileUploadControl)
        {
            //System.Web.UI.WebControls.WebControl
            string filePath = Path.Combine(saveDirectory, fileUploadControl.FileName);

            fileUploadControl.SaveAs(filePath);

            return filePath;
        }

        public bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        public virtual ActionResult GetBaseInfoUsers()
        {
            try
            {
                var data = _baseuserService.GetAllBaseUserLogin();
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    var itemObject = new JObject
                    {
                        {"BaseUserLoginId", item.BaseUserLoginId},
                        {"CodeMeli", item.CodeMeli},
                        {"CodeEstekhdam", item.CodeEstekhdam},
                        {"Active", item.Active}
                    };
                    ja.Add(itemObject);
                }
                JObject jo = new JObject();
                jo.Add("total", _baseuserService.GetAllBaseUserLogin().Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات کاربران" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult Create(BaseUserLogin bul)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نا معتبر!" });
                }
                var baseuser = _baseuserService.Where(b => b.CodeMeli == bul.CodeMeli).FirstOrDefault();
                if (baseuser != null)
                {
                    return Json(new { isError = true, Msg = "کد ملی در بانک اطلاعاتی وجود دارد!" });
                }
                baseuser = _baseuserService.Where(b => b.CodeEstekhdam == bul.CodeEstekhdam).FirstOrDefault();
                if (baseuser != null)
                {
                    return Json(new { isError = true, Msg = "کد استخدامی در بانک اطلاعاتی وجود دارد!" });
                }
                _baseuserService.AddNewBaseUserLogin(bul);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.اطلاعات_پایه_کاربر,
                    OperationDetail = OperationsDetail.ایجاد,
                    Description = "ایجاد اطلاعات پایه کاربران مجاز به سیستم "
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "ثبت شد" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در ثبت اطلاعات!" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult Delete(int id)
        {
            try
            {
                var model = _baseuserService.Find(id);
                if (model == null)
                    return HttpNotFound();
                if (model.Active)
                {
                    return Json(new { isError = true, Msg = "کاربر فعال قابل حذف نمی باشد!" });
                }
                _baseuserService.Delete(id);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.اطلاعات_پایه_کاربر,
                    OperationDetail = OperationsDetail.حذف,
                    Description = " حذف اطلاعات پایه کاربران مجاز به سیستم به شماره" + id
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "حذف شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در حذف" });
            }
        }

        [HttpPost]
        public virtual ActionResult Edit(BaseUserLogin bul)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر" });
                }
                var baseuser =
                    _baseuserService.Where(b => b.CodeMeli == bul.CodeMeli && b.BaseUserLoginId != bul.BaseUserLoginId)
                        .FirstOrDefault();
                if (baseuser != null)
                {
                    return Json(new { isError = true, Msg = "کد ملی در بانک اطلاعاتی وجود دارد!" });
                }
                baseuser =
                    _baseuserService.Where(
                        b => b.CodeEstekhdam == bul.CodeEstekhdam && b.BaseUserLoginId != bul.BaseUserLoginId)
                        .FirstOrDefault();
                if (baseuser != null)
                {
                    return Json(new { isError = true, Msg = "کد استخدامی در بانک اطلاعاتی وجود دارد!" });
                }
                _baseuserService.Edit(bul);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.اطلاعات_پایه_کاربر,
                    OperationDetail = OperationsDetail.ویرایش,
                    Description = "ویرایش اطلاعات پایه کاربران مجاز به سیستم به شماره " + bul.BaseUserLoginId
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "تغییرات اعمال شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ویرایش اطلاعات کاربر" });
            }
        }
    }
}