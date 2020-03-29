using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Areas.UserArea.Models;
using Annual_faculty_promotions.WebUI.Helpers.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using Action = Antlr.Runtime.Misc.Action;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{
    public partial class StageController : Controller
    {
        private readonly IStageService _stageService;
        private readonly IUnivercityStructureService _univercityStructure;
        private readonly IUserService _userService;
        private readonly IApplicationRoleManager _roleManager;
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;

        public StageController(IUnitOfWork unitOfWork,
            IStageService stageService,
            IApplicationRoleManager roleManager,
            IUserService userService,
            ILogService logService,
            IUnivercityStructureService univercityStructure)
        {
            _unitOfWork = unitOfWork;
            _stageService = stageService;
            _roleManager = roleManager;
            _userService = userService;
            _univercityStructure = univercityStructure;
            _logService = logService;
        }
        //
        // GET: /UserArea/Stage/
        public virtual ActionResult Index()
        {

            return View();
        }

        public virtual ActionResult GetStageNumberToJson()
        {
            try
            {
                var sizes = _stageService.GetAllStages();
                var serializer = new JavaScriptSerializer();
                JArray ja = new JArray();

                foreach (var item in sizes)
                {
                    var itemObject = new JObject
                {
                    {"StageId",item.Id},
                    {"StageNumber",item.StageNumber}
                };
                    ja.Add(itemObject);
                }
                return Content(JsonConvert.SerializeObject(ja), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود مراحل" });
            }
        }

        public virtual ActionResult GetRoleJason()
        {
            try
            {
                var sizes = _roleManager.GetAllCustomRolesAsQueryable();
                JArray ja = new JArray();

                foreach (var item in sizes)
                {
                    var itemObject = new JObject
                {
                    {"Id",item.Id},
                    {"PersianName",item.PersianName}
                };
                    ja.Add(itemObject);
                }
                return Content(JsonConvert.SerializeObject(ja), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود نقش" });
            }
        }

        public virtual ActionResult GetUserJson()
        {
            try
            {
                var user = _userService.Where(x => x.Profile != null).Include(x => x.Profile).ToList();

                JArray ja = new JArray();

                foreach (var item in user)
                {
                    var itemObject = new JObject
                {
                    {"UserId",item.Id},
                    {"UserName",item.Profile.Name+" "+item.Profile.Family}
                };
                    ja.Add(itemObject);
                }
                return Content(JsonConvert.SerializeObject(ja), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود کاربران" });
            }
        }


        public virtual ActionResult GetDepartmentJson()
        {
            try
            {
                var structures = _univercityStructure.GetAllUnivercityStructures();

                JArray ja = new JArray();

                foreach (var item in structures)
                {
                    var itemObject = new JObject
                {
                    {"StructId",item.Id},
                    {"StructName",item.Name}
                };
                    ja.Add(itemObject);
                }
                return Content(JsonConvert.SerializeObject(ja), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود دپارتمان" });
            }
        }

        public virtual ActionResult GetStages(int page = 1, int pageSize = 17)
        {
            try
            {
                var items = _stageService.GetAllStagesAsQueryable().Include(c => c.Role)
                    .OrderBy(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                JArray ja = new JArray();

                foreach (var item in items)
                {
                    var itemObject = new JObject
                {
                    {"Id",item.Id},
                    {"RoleName",item.Role.Name},
                    {"Name",item.Name},
                    {"StageNumber",item.StageNumber}
                };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _stageService.GetAllStagesAsQueryable().Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود مراحل" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult Create(Stage stage)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر!" });
                }
                _stageService.AddNewStage(stage);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.مراحل,
                    OperationDetail = OperationsDetail.ایجاد,
                    Description = " ایجاد مرحله-" + stage.Name
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "ثبت شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ایجاد مرحله" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult Delete(int id)
        {
            try
            {
                var model = _stageService.Find(id);
                if (model == null)
                    return HttpNotFound();
                _stageService.Delete(id);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.مراحل,
                    OperationDetail = OperationsDetail.حذف,
                    Description = " حذف مرحله به شماره" + id
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "حذف شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در عملیات حذف مرحله " });
            }
        }

        [HttpPost]
        public virtual ActionResult Edit(Stage stage)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر" });
                }
                _stageService.EditStage(stage);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.مراحل,
                    OperationDetail = OperationsDetail.ویرایش,
                    Description = " حذف اطلاعات پایه کاربران مجاز به سیستم به شماره" + stage.Id
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "تغییرات اعمال شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ویرایش مرحله" });
            }
        }


        public virtual ActionResult GetStageJason()
        {
            try
            {
                var sizes = _stageService.GetAllStages();
                JArray ja = new JArray();
                foreach (var item in sizes)
                {
                    var itemObject = new JObject
                {
                    {"StageId",item.Id},
                    {"StageName",item.Name}
                };
                    ja.Add(itemObject);
                }
                return Content(JsonConvert.SerializeObject(ja), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود مراحل" });
            }
        }

    }
}