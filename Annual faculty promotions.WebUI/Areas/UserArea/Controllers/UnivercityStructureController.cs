using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Helpers.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Annual_faculty_promotions.Core.Enums;
using Microsoft.AspNet.Identity;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{
    public partial class UnivercityStructureController : Controller
    {
        private readonly IUnivercityStructureService _univercityStructureService;
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;

        public UnivercityStructureController(IUnitOfWork unitOfWork, ILogService logService, IUnivercityStructureService univercityStructureService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _univercityStructureService = univercityStructureService;

        }

        //[Expire]
        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult GetStructuresTreeNode()
        {
            try
            {
                List<JObject> jobjects = new List<JObject>();

                var items = _univercityStructureService.GetAllUnivercityStructures();

                if (items.Any())
                {
                    var rootNodes = items.Where(c => c.Level == 1);
                    foreach (var node in rootNodes)
                    {
                        JObject root = new JObject
                      {
                        {"id",node.Id},
                        {"text",node.Name},
                        {"hasChild",node.HasChild},
                        {"level",node.Level}
                      };
                        root.Add("children", this.GetChild(node, items));
                        jobjects.Add(root);
                    }
                }
                return Content(JsonConvert.SerializeObject(jobjects));
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود ساختار دانشگاه" });
            }
        }
        private JArray GetChild(UnivercityStructure parentNodes, IEnumerable<UnivercityStructure> nodes)
        {
            JArray childArray = new JArray();

            foreach (var node in nodes.Where(x => x.ParentId == parentNodes.Id))
            {
                JObject subObject = new JObject
                {
                    {"id",node.Id},
                    {"text",node.Name},
                    {"hasChild",node.HasChild},
                    {"level",node.Level}
                };

                if (nodes.Any(y => y.ParentId == node.Id))
                {
                    subObject.Add("children", this.GetChild(node, nodes));
                }
                childArray.Add(subObject);
            }
            return childArray;
        }

        [HttpPost]
        [AjaxOnly]
        [OutputCache(NoStore = true, Duration = 0)]
        public virtual ActionResult Create(UnivercityStructure univercityStructure)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Msg = "خطا - ثبت نشد" });
                }
                _univercityStructureService.AddNewUnivercityStructure(univercityStructure);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.ساختار_دانشگاه,
                    OperationDetail = OperationsDetail.ایجاد,
                    Description = " ایجاد ساختار دانشگاه-" + univercityStructure.Name
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { Msg = "ثبت شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ایجاد ساختار دانشگاه" });
            }
        }

        public virtual ActionResult GetCategoriesTreeNode()
        {
            try
            {
                List<JObject> jobjects = new List<JObject>();

                var items = _univercityStructureService.GetAllUnivercityStructures();

                if (items.Any())
                {
                    var rootNodes = items.Where(c => c.Level == 1);
                    foreach (var node in rootNodes)
                    {
                        JObject root = new JObject
                      {
                        {"id",node.Id},
                        {"text",node.Name},
                        {"level",node.Level}
                      };
                        root.Add("children", this.GetChild(node, items));
                        jobjects.Add(root);
                    }
                }
                return Content(JsonConvert.SerializeObject(jobjects));
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود ساختار دانشگاه" });
            }
        }

        private JArray GetChild(UnivercityStructure parentNodes, IList<UnivercityStructure> nodes)
        {
            JArray childArray = new JArray();

            foreach (var node in nodes.Where(x => x.ParentId == parentNodes.Id))
            {
                JObject subObject = new JObject
                {
                    {"id",node.Id},
                    {"text",node.Name},
                    {"level",node.Level}
                };

                if (nodes.Any(y => y.ParentId == node.Id))
                {
                    subObject.Add("children", this.GetChild(node, nodes));
                }
                childArray.Add(subObject);
            }
            return childArray;
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult Delete(int id)
        {
            try
            {
                var model = _univercityStructureService.Where(d => d.ParentId == id || (d.Id == id && d.ParentId == null)).FirstOrDefault();
                if (model != null)
                    return Json(new { isError = true, Msg = "این گروه آموزشی دارای زیر شاخه می باشد!" });
                model = _univercityStructureService.Where(s=>s.Id==id).Include(i=>i.Requests).FirstOrDefault();
                if (model == null)
                    return Json(new { isError = true, Msg = "رکورد مورد نظر یافت نشد!" });
                if (model.Requests.Count > 0)
                {
                    return Json(new { isError = true, Msg = "این گروه آموزشی دارای گردش می باشد!" });
                }
                _univercityStructureService.Delete(id);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.ساختار_دانشگاه,
                    OperationDetail = OperationsDetail.حذف,
                    Description = " حذف ساختار دانشگاه به شماره" + id
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

        public virtual ActionResult Edit(int? univercityId)
        {
            if (univercityId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var college = _univercityStructureService.Find(univercityId);
            if (college == null)
            {
                return HttpNotFound();
            }
            return PartialView("_edit", college);
        }

        [HttpPost]
        public virtual ActionResult Edit(UnivercityStructure univercity)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, Msg = "ورودی نامعتبر" });
                }
                _univercityStructureService.EditUnivercityStructure(univercity);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.ساختار_دانشگاه,
                    OperationDetail = OperationsDetail.ویرایش,
                    Description = " ویرایش ساختار دانشگاه به شماره" + univercity.Id
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { success = true, Msg = "تغییرات اعمال شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ویرایش ساختار دانشگاه" });
            }
        }
    }
}