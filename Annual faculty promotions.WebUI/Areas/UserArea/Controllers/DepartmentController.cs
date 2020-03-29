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

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{

    public partial class DepartmentController : Controller
    {

        //private readonly IDepartmentService _departmentService;
        //private readonly IUnitOfWork _unitOfWork;

        //public DepartmentController(IUnitOfWork unitOfWork, IDepartmentService departmentService)
        //{
        //    _unitOfWork = unitOfWork;
        //    _departmentService = departmentService;

        //}

        //public ActionResult Index()
        //{
        //    return View();
        //}


        //public ActionResult GetDepartments(int page = 1, int pageSize = 17)
        //{
        //    var sizes = _departmentService.GetAllDepartmentsAsQueryable().Include(x => x.College)
        //        .OrderBy(x => x.Id)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize);

        //    JArray ja = new JArray();

        //    foreach (var item in sizes)
        //    {
        //        var itemObject = new JObject
        //        {
        //            {"Id",item.Id},
        //            {"Name",item.Name},
        //            {"College",item.College.Name}
        //        };
        //        ja.Add(itemObject);
        //    }

        //    JObject jo = new JObject();
        //    jo.Add("total", _departmentService.GetAllDepartmentsAsQueryable().Count());
        //    jo.Add("rows", ja);
        //    return Content(JsonConvert.SerializeObject(jo), "application/json");
        //}

        //[HttpPost]
        //[AjaxOnly]
        //public ActionResult Create([Bind(Include = "Name,CollegeId")] Department department)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _departmentService.AddNewDepartment(department);
        //        _unitOfWork.SaveChanges();
        //        return Json(new { Msg = "ثبت شد" });
        //    }
        //    return Json(new { Msg = "خطا در ثبت!" });
        //}


        //[HttpPost]
        //[AjaxOnly]
        //public ActionResult Delete(int id)
        //{
        //    try
        //    {
        //        var model = _departmentService.Find(id);
        //        if (model == null)
        //            return HttpNotFound();
        //        _departmentService.Delete(id);
        //        _unitOfWork.SaveChanges();
        //        return Json(new { success = true, Msg = "حذف شد" });
        //    }
        //    catch (Exception)
        //    {

        //        return Json(new { success = false, Msg = "حذف نشد" });
        //    }
        //}


        //public ActionResult Edit(long? departmentId)
        //{
        //    if (departmentId == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var college = _departmentService.Find(c => c.Id == departmentId);
        //    if (college == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return PartialView("_Edit", college);
        //}

        //[HttpPost]
        //public ActionResult Edit(Department department)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _departmentService.EditDepartment(department);
        //        _unitOfWork.SaveChanges();
        //        return Json(new { success = true, Msg = "تغییرات اعمال شد" });
        //    }
        //    return Json(new { success = false, Msg = "ورودی نامعتبر" });
        //}
    }
}