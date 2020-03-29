using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Enums;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Areas.UserArea.Models;
using Annual_faculty_promotions.WebUI.Helpers;
using Annual_faculty_promotions.WebUI.Helpers.Filters;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{
    [Authorize]
    public partial class RequestController : Controller
    {
        private readonly IUnivercityStructureService _univercityStructureService;
        private readonly IRequestService _requestService;
        private readonly IArchiveService _archiveService;
        private readonly IUserService _userService;
        private readonly ICartableService _cartableService;
        private readonly ILogService _logService;

        private readonly IUnitOfWork _unitOfWork;

        public RequestController(IUnivercityStructureService univercityStructure,
            IUnitOfWork unitOfWork,
            IRequestService requestService,
            IUserService userService,
            ICartableService cartableService,
            ILogService logService,
            IArchiveService archiveService)
        {
            _unitOfWork = unitOfWork;
            _univercityStructureService = univercityStructure;
            _requestService = requestService;
            _archiveService = archiveService;
            _userService = userService;
            _logService = logService;
            _cartableService = cartableService;
        }

        //[Expire]
        public virtual ActionResult Index()
        {
            try
            {
                var d = new PersianCalendar();
                int uId = int.Parse(User.Identity.GetUserId());

                //ViewBag.LastDateGrade = archive == null ? "بدون مقدار" : archive.CreatedDate.ToPeString();
                //  var req = System.Web.Helpers.Json.Encode(_requestService.Where(r => r.Term == DateTime.Now.Year).Include(r => r.EducationalResearches));
                var model = new AddRequestViewModel();
                //int _term = d.GetYear(DateTime.Now);

                model.Request = (from p in _requestService.Where(x => x.UserId == uId && (x.Cartables.Count == 0 || x.Cartables.Any(c => c.Active && c.CurrentCartable != CurrentCartable.تمام_شده)))
                                 select new
                                 {
                                     Request = p,
                                     Request_UnivercityStructure = p.UnivercityStructure,
                                     Request_Cartables = p.Cartables.ToList(),
                                     Request_Cartables_UserSender = p.Cartables.Select(x => x.UserSender),
                                     Request_Cartables_UserSender_Profile = p.Cartables.Select(x => x.UserSender.Profile)
                                 }).AsEnumerable().Select(x => x.Request).FirstOrDefault();

                model.UnivercityStructures = _univercityStructureService.Where(x => x.HasChild == false && x.Level == 3).ToList();

                if (model.Request != null)
                {
                    var md = _requestService.Where(x => x.RequestId == model.Request.Id).FirstOrDefault();
                    if (md == null)
                    {
                        model.FurtherInformation = new FurtherInformation();
                        model.AttachmentFurtherInformations = new List<AttachmentFurtherInformation>();
                    }
                    else
                    {
                        model.FurtherInformation = md;
                        model.AttachmentFurtherInformations =
                            _requestService.GetAttachmentFurtherInformations(md.Id).ToList();
                    }
                }
                else
                {
                    // اطلاعات پایه آخرین درخواست
                    model.Request = new Request();
                    var req = _requestService.Where(r => r.UserId == uId).Include(i => i.FurtherInformations).ToList().LastOrDefault();
                    if (req == null)
                    {
                        model.FurtherInformation = new FurtherInformation();
                        model.AttachmentFurtherInformations = new List<AttachmentFurtherInformation>();
                    }
                    else
                    {
                        model.Request = GetLastRequest(req);
                        var md = _requestService.Where(x => x.RequestId == req.Id).FirstOrDefault();
                        if (md == null)
                        {
                            model.FurtherInformation = new FurtherInformation();
                            model.AttachmentFurtherInformations = new List<AttachmentFurtherInformation>();
                        }
                        else
                        {
                            model.FurtherInformation = GetLastFurtherInformation(md);
                            model.AttachmentFurtherInformations =
                                _requestService.GetAttachmentFurtherInformations(md.Id).ToList();
                        }
                    }
                    var archive = _archiveService.Where(x => x.User.Id == uId).ToList();
                    var user = _userService.Find(uId);
                    if (archive.Count > 0)
                    {
                        model.Request.Grade = archive.Sum(s => s.Grade);
                        model.Request.LastDateGrade = archive.LastOrDefault()?.CreatedDate;
                    }
                    else if (req != null)
                    {
                        model.Request.Grade = req.Grade;
                        model.Request.LastDateGrade = req.LastDateGrade;
                    }
                    else
                    {
                        model.Request.Grade = 0;
                        //if(user.Profile!=null)
                        model.Request.LastDateGrade = user.Profile?.EmployeeDate;
                    }
                }

                ViewBag.Department =
                    new SelectList(_univercityStructureService.Where(x => x.HasChild == false && x.Level == 3), "Id", "Name", model.Request != null ? model.Request.UnivercityStructureId : 0);

                if (model.Request != null)
                {
                    if (!RequestIsReturn(model.Request.Id))
                    {
                        model.IsSent = false;
                    }
                    else
                    {
                        model.IsSent = true;
                    }
                }
                else
                {
                    model.IsSent = true;
                }
                //به دلیل مقدار نگرفتن آخرین تاریخ ترفیع زمان ثبت درخواست ، ما مجبور شدیم فیلد تاریخ را در ویو مدل بگذاریم
                model.LastDateGrade = model.Request?.LastDateGrade;
                return View(model);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult Create(AddRequestViewModel model,
            IEnumerable<HttpPostedFileBase> officersStudyPhDUpload,
            IEnumerable<HttpPostedFileBase> typeOfPartTimeWorkNoconnectionOtherUniversityUpload,
            IEnumerable<HttpPostedFileBase> typeOfPartTimeButEmployeesOtherUniversityUpload,
            IEnumerable<HttpPostedFileBase> typeOfFullTimeButRetiredUpload,
            IEnumerable<HttpPostedFileBase> typeOfFullTimeButNoRetiredUpload,
            IEnumerable<HttpPostedFileBase> passPhDPassaUpload,
            IEnumerable<HttpPostedFileBase> takingSabbaticalUpload,
            IEnumerable<HttpPostedFileBase> freedmanOrCaptiveTypeUpload,
            IEnumerable<HttpPostedFileBase> fighterTypeUpload,
            IEnumerable<HttpPostedFileBase> veteranTypeUpload)
        {
            try
            {
                if (model.Request.Id > 0)
                {
                    if (RequestIsReturn(model.Request.Id))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر!" });
                }
                var pc = new PersianCalendar();
                var req = model.Request;
                req.UserId = int.Parse(User.Identity.GetUserId());

                req.Term = pc.GetYear(DateTime.Now);
                req.Status = RequestStatus.درجریان;
                req.LastDateGrade = model.LastDateGrade;

                var log1 = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.درخواست
                };
                if (model.Request.Id == 0)
                {
                    log1.OperationDetail = OperationsDetail.ایجاد;
                    log1.Description = "ایجاد درخواست مربوط به کاربر " + req.UserId;
                    _requestService.AddNewRequest(req);
                }
                else
                {
                    log1.OperationDetail = OperationsDetail.ویرایش;
                    log1.Description = "ویرایش درخواست شماره " + req.Id;
                    _requestService.EditRequest(req);
                }
                _logService.AddNewLog(log1);
                //------------------------------ add furtherInformation

                var entity = _requestService.FindFurtherInformation(model.FurtherInformation.Id);
                if (entity == null)
                {
                    Log log2 = new Log()
                    {
                        UserId = int.Parse(User.Identity.GetUserId()),
                        Operation = Operations.اطلاعات_تکمیلی_درخواست,
                        OperationDetail = OperationsDetail.ویرایش,
                        Description = " ایجاد اطلاعات تکمیلی مربوط به درخواست شماره " + req.Id
                    };
                    _logService.AddNewLog(log2);
                    _requestService.AddFurtherInformation(model.FurtherInformation);
                }
                else
                {
                    Log log2 = new Log()
                    {
                        UserId = int.Parse(User.Identity.GetUserId()),
                        Operation = Operations.اطلاعات_تکمیلی_درخواست,
                        OperationDetail = OperationsDetail.ویرایش,
                        Description = " ویرایش اطلاعات تکمیلی مربوط به درخواست شماره " + req.Id
                    };
                    _logService.AddNewLog(log2);
                    _requestService.EditFurtherInformation(model.FurtherInformation);
                }
                var path = Server.MapPath("~/Content/FurtherInformation/" + User.Identity.Name);

                bool exist = System.IO.File.Exists(path);
                if (!exist)
                    System.IO.Directory.CreateDirectory(path);
                path += "\\";
                // ثبت پیوست های مربوطه
                if (officersStudyPhDUpload != null)
                {
                    foreach (var item in officersStudyPhDUpload)
                    {
                        if (item != null)
                        {
                            string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            //item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);
                            item.SaveAs(temppath);
                            _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                            {
                                FurtherInformationId = model.FurtherInformation.Id,
                                FurtherInformationType =
                                    FurtherInformationType.مامور_به_تحصیل_در_مقطع_دکتری_داخل_یا_خارج,
                                ImageName = Path.GetFileName(temppath)
                            });
                        }
                    }
                }
                if (typeOfPartTimeWorkNoconnectionOtherUniversityUpload != null)
                {
                    foreach (var item in typeOfPartTimeWorkNoconnectionOtherUniversityUpload)
                    {
                        if (item != null)
                        {
                            string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            //item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);
                            item.SaveAs(temppath);
                            _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                            {
                                FurtherInformationId = model.FurtherInformation.Id,
                                FurtherInformationType =
                                    FurtherInformationType.نوع_همکاری_نیمه_وقت_بدون_هیچگونه_ارتباط_با_سایر_دانشگاهها,
                                ImageName = Path.GetFileName(temppath)
                            });
                        }
                    }
                }
                if (typeOfPartTimeButEmployeesOtherUniversityUpload != null)
                {
                    foreach (var item in typeOfPartTimeButEmployeesOtherUniversityUpload)
                    {
                        if (item != null)
                        {
                            string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            //item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);
                            item.SaveAs(temppath);
                            _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                            {
                                FurtherInformationId = model.FurtherInformation.Id,
                                FurtherInformationType =
                                    FurtherInformationType
                                        .نوع_همکاری_نیمه_وقت_اما_به_صورت_تمام_وقت_شاغل_در_سایر_دانشگاهها_با_ارائه_حکم_کارگزینی_مبنی_بر_اعطای_پایه_سالیانه_دردانشگاه_دولتی,
                                ImageName = Path.GetFileName(temppath)
                            });
                        }
                    }
                }
                if (typeOfFullTimeButRetiredUpload != null)
                {
                    foreach (var item in typeOfFullTimeButRetiredUpload)
                    {
                        if (item != null)
                        {
                            string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            //item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);
                            item.SaveAs(temppath);
                            _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                            {
                                FurtherInformationId = model.FurtherInformation.Id,
                                FurtherInformationType =
                                    FurtherInformationType.نوع_همکاری_تمام_وقت_اما_بازنشسته_از_سایردانشگاههای_وابسته,
                                ImageName = Path.GetFileName(temppath)
                            });
                        }
                    }
                }
                if (typeOfFullTimeButNoRetiredUpload != null)
                {
                    foreach (var item in typeOfFullTimeButNoRetiredUpload)
                    {
                        if (item != null)
                        {
                            string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            //item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);
                            item.SaveAs(temppath);
                            _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                            {
                                FurtherInformationId = model.FurtherInformation.Id,
                                FurtherInformationType =
                                    FurtherInformationType.نوع_همکاری_تمام_وقت_اما_غیر_بازنشسته_در_صورت_استفاء,
                                ImageName = Path.GetFileName(temppath)
                            });
                        }
                    }
                }
                if (passPhDPassaUpload != null)
                {
                    foreach (var item in passPhDPassaUpload)
                    {
                        if (item != null)
                        {
                            string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            //item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);
                            item.SaveAs(temppath);
                            _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                            {
                                FurtherInformationId = model.FurtherInformation.Id,
                                FurtherInformationType = FurtherInformationType.گواهی_دوره_پسا_دکترا,
                                ImageName = Path.GetFileName(temppath)
                            });
                        }
                    }
                }
                if (takingSabbaticalUpload != null)
                {
                    foreach (var item in takingSabbaticalUpload)
                    {
                        if (item != null)
                        {
                            string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            //item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);
                            item.SaveAs(temppath);
                            _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                            {
                                FurtherInformationId = model.FurtherInformation.Id,
                                FurtherInformationType = FurtherInformationType.مجوز_سازمان_مرکزی_برای_فرصت_مطالعاتی,
                                ImageName = Path.GetFileName(temppath)
                            });
                        }
                    }
                }
                if (freedmanOrCaptiveTypeUpload != null)
                {
                    foreach (var item in freedmanOrCaptiveTypeUpload)
                    {
                        if (item != null)
                        {
                            string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            //item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);
                            item.SaveAs(temppath);
                            _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                            {
                                FurtherInformationId = model.FurtherInformation.Id,
                                FurtherInformationType = FurtherInformationType.مستندات_آزادگان_اسرا_مفقودین,
                                ImageName = Path.GetFileName(temppath)
                            });
                        }
                    }
                }
                if (fighterTypeUpload != null)
                {
                    foreach (var item in fighterTypeUpload)
                    {
                        if (item != null)
                        {
                            string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            //item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);
                            item.SaveAs(temppath);
                            _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                            {
                                FurtherInformationId = model.FurtherInformation.Id,
                                FurtherInformationType = FurtherInformationType.مستندات_رزمنده,
                                ImageName = Path.GetFileName(temppath)
                            });
                        }
                    }
                }
                if (veteranTypeUpload != null)
                {
                    foreach (var item in veteranTypeUpload)
                    {
                        if (item != null)
                        {
                            string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                            //item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);
                            item.SaveAs(temppath);
                            _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                            {
                                FurtherInformationId = model.FurtherInformation.Id,
                                FurtherInformationType = FurtherInformationType.مستندات_جانبازی,
                                ImageName = Path.GetFileName(temppath)
                            });
                        }
                    }
                }

                _unitOfWork.SaveChanges();

                AddRequestViewModel mymodel = new AddRequestViewModel();
                PersianCalendar d = new PersianCalendar();
                int uId = int.Parse(User.Identity.GetUserId());
                var archive = _archiveService
                    .Where(x => x.User.Id == uId).ToList().LastOrDefault();

                //ViewBag.LastDateGrade = archive == null ? null : d.ToDateTime(archive.CreatedDate.Year, archive.CreatedDate.Month, archive.CreatedDate.Day, 0, 0, 0, 0, 0).ToString();
                //  var req = System.Web.Helpers.Json.Encode(_requestService.Where(r => r.Term == DateTime.Now.Year).Include(r => r.EducationalResearches));

                var _term = d.GetYear(DateTime.Now);
                mymodel.Request = _requestService.Where(x => x.UserId == uId).Include(x => x.UnivercityStructure).AsEnumerable().OrderByDescending(o => o.Id).FirstOrDefault();
                mymodel.UnivercityStructures = _univercityStructureService.Where(x => x.HasChild == false && x.Level == 3).ToList();

                if (mymodel.Request != null)
                {
                    var md = _requestService.Where(x => x.RequestId == model.Request.Id).ToList().FirstOrDefault();
                    if (md == null)
                    {
                        mymodel.FurtherInformation = new FurtherInformation();
                        mymodel.AttachmentFurtherInformations = new List<AttachmentFurtherInformation>();
                    }
                    else
                    {
                        mymodel.FurtherInformation = md;
                        mymodel.AttachmentFurtherInformations =
                            _requestService.GetAttachmentFurtherInformations(md.Id).ToList();
                    }
                }
                else
                {
                    mymodel.Request = new Request();
                    mymodel.FurtherInformation = new FurtherInformation();
                    mymodel.AttachmentFurtherInformations = new List<AttachmentFurtherInformation>();
                }

                ViewBag.Department =
                    new SelectList(_univercityStructureService.Where(x => x.HasChild == false && x.Level == 3), "Id", "Name", mymodel.Request != null ? mymodel.Request.UnivercityStructureId : 0);
                ModelState.Clear();
                mymodel.LastDateGrade = mymodel.Request.LastDateGrade;
                //return PartialView("_CreateFurtherInformation", mymodel);
                return Json(new { isError = false, Msg = RenderRazorViewToString("_CreateFurtherInformation", mymodel) });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در ثبت درخواست" });
            }
        }
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        protected string RenderViewToString<T>(string viewPath, T model)
        {
            ViewData.Model = model;
            using (var writer = new StringWriter())
            {
                var view = new WebFormView(ControllerContext, viewPath);
                var vdd = new ViewDataDictionary<T>(model);
                var viewCxt = new ViewContext(ControllerContext, view, vdd,
                                            new TempDataDictionary(), writer);
                viewCxt.View.Render(viewCxt, writer);
                return writer.ToString();
            }
        }
        public virtual ActionResult GetEducation(long requestid)
        {
            try
            {
                var data = _requestService.GetEducation_ByRequestId_ForCurrentYear(requestid).ToList();
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    var itemObject = new JObject
                    {
                        {"Id",item.Id},
                        {"RequestId",item.RequestId},
                        {"EducationalResearchStatus",((EducationalResearchStatus)item.EducationalResearchStatus).ToString()},
                        {"Term",(int)item.Term},
                        {"TermTitle",((Term)item.Term).ToString().Replace("_"," ")},
                        {"Subject",item.Subject},
                        {"CourseNo",item.CourseNo},
                        {"UnitCount",item.UnitCount },
                        {"GradeEducation", (int)item.GradeEducation},
                        {"GradeEducationTitle", ((GradeEducation)item.GradeEducation).ToString().Replace("_"," ")},
                        {"StudentCount",item.StudentCount}
                    };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _requestService.GetEducation_ByRequestId_ForCurrentYear(requestid).Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات آموزشی" });
            }
        }
        public virtual ActionResult GetResearch(long requestid)
        {
            try
            {
                var data = _requestService.GetReasearch_ByRequestId_ForCurrentYear(requestid).ToList();
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    var itemObject = new JObject
                    {
                        {"Id",item.Id},
                        {"RequestId",item.RequestId},
                        {"EducationalResearchStatus",((EducationalResearchStatus)item.EducationalResearchStatus).ToString()},
                        {"Subject",item.Subject},
                        {"Term",(int)item.Term},
                        {"TermTitle",((Term)item.Term).ToString().Replace("_"," ")},
                        {"ResearchPost",(int)item.ResearchPost},
                        {"ResearchPostTitle",((PostPlan)item.ResearchPost).ToString()},
                        {"BeginDate",item.BeginDate},
                        {"EndDate",item.EndDate },
                        {"StudentCount",item.StudentCount}
                    };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _requestService.GetReasearch_ByRequestId_ForCurrentYear(requestid).Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات پژوهشی" });
            }
        }
        public virtual ActionResult GetEducationalResearchStatus()
        {
            var items = Enum.GetValues(typeof(EducationalResearchStatus));
            JArray ja = new JArray();
            foreach (var item in items)
            {
                var itemObject = new JObject
                {
                    {"EducationalResearchStatusId", ((int)((EducationalResearchStatus)Enum.Parse(typeof(EducationalResearchStatus), item.ToString()))).ToString()},
                    {"EducationalResearchStatus",item.ToString()}
                };
                ja.Add(itemObject);
            }
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }
        public virtual ActionResult GetPostPlan()
        {
            var items = Enum.GetValues(typeof(PostPlan));
            JArray ja = new JArray();
            foreach (var item in items)
            {
                var itemObject = new JObject
                {
                    {"PostPlanId", ((int)((PostPlan)Enum.Parse(typeof(PostPlan), item.ToString()))).ToString()},
                    {"PostPlan",item.ToString()}
                };
                ja.Add(itemObject);
            }
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }
        public virtual ActionResult GetTerm()
        {
            var items = Enum.GetValues(typeof(Term));
            JArray ja = new JArray();
            foreach (var item in items)
            {
                var itemObject = new JObject
                {
                     {"TermId", ((int)((Term)Enum.Parse(typeof(Term), item.ToString()))).ToString()},
                    {"Term",item.ToString().Replace("_"," ")}
                };
                ja.Add(itemObject);
            }
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }
        public virtual ActionResult GetGradeEducation()
        {
            var items = Enum.GetValues(typeof(GradeEducation));
            JArray ja = new JArray();
            foreach (var item in items)
            {
                var itemObject = new JObject
                {
                     {"GradeEducationId", ((int)((GradeEducation)Enum.Parse(typeof(GradeEducation), item.ToString()))).ToString()},
                    {"GradeEducation",item.ToString().Replace("_"," ")}
                };
                ja.Add(itemObject);
            }
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }

        public virtual ActionResult GetTechnologyType()
        {
            var items = Enum.GetValues(typeof(TechnologyType));
            JArray ja = new JArray();
            foreach (var item in items)
            {
                var itemObject = new JObject
                {
                     {"TechnologyTypeId", ((int)((TechnologyType)Enum.Parse(typeof(TechnologyType), item.ToString()))).ToString()},
                    {"TechnologyType",item.ToString().Replace("_"," ")}
                };
                ja.Add(itemObject);
            }
            return Content(JsonConvert.SerializeObject(ja), "application/json");
        }
        public virtual ActionResult CreateEducationReasearch(EducationalResearch educationalResearch)
        {
            try
            {
                if (educationalResearch.RequestId > 0)
                {
                    if (RequestIsReturn(educationalResearch.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                ModelState.Remove("Id");
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر!" });
                }
                _requestService.AddEducationReasearch(educationalResearch);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_آموزشی_پژوهشی,
                    OperationDetail = OperationsDetail.ایجاد,
                    Description = " ایجاد فعالیت آموزشی پژوهشی- " + educationalResearch.Subject + " مربوط به درخواست شماره " + educationalResearch.RequestId
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { outId = educationalResearch.Id, isError = false, Msg = "ثبت شد" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در ثبت اطلاعات آموزشی ، پژوهشی!" });
            }
        }
        public virtual ActionResult CreateDissertation(Dissertation dissertation)
        {
            try
            {
                if (dissertation.RequestId > 0)
                {
                    if (RequestIsReturn(dissertation.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر!" });
                }
                _requestService.AddDissertation(dissertation);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_پروژه_پایاننامه_مقاله,
                    OperationDetail = OperationsDetail.ایجاد,
                    Description = " ایجاد فعالیت پروژه،پایان نامه،رساله - " + dissertation.Subject + " مربوط به درخواست شماره " + dissertation.RequestId
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "ثبت شد" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در ثبت اطلاعات پروژه ، پایان نامه" });
            }
        }
        public virtual ActionResult CreateScientificExecutive(ScientificExecutive scientificExecutive)
        {
            try
            {
                if (scientificExecutive.RequestId > 0)
                {
                    if (RequestIsReturn(scientificExecutive.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر!" });
                }
                _requestService.AddScientificExecutive(scientificExecutive);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_علمی_اجرایی,
                    OperationDetail = OperationsDetail.ایجاد,
                    Description = " ایجاد فعالیت علمی اجرایی- " + scientificExecutive.Subject + " مربوط به درخواست شماره " + scientificExecutive.RequestId
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "ثبت شد" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در ثبت اطلاعات پروژه ، پایان نامه" });
            }
        }
        public virtual ActionResult CreateTechnology(Technology technology)
        {
            try
            {
                if (technology.RequestId > 0)
                {
                    if (RequestIsReturn(technology.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر!" });
                }
                _requestService.AddTechnology(technology);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_پژوهشی_فناوری,
                    OperationDetail = OperationsDetail.ایجاد,
                    Description = " ایجاد فعالیت پژوهشی فن آوری- " + technology.Subject + " مربوط به درخواست شماره " + technology.RequestId
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "ثبت شد" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در ثبت اطلاعات فناوری" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult DeleteEducationReasearch(int id)
        {
            try
            {
                var model = _requestService.FindEducationResearch(id);
                if (model == null)
                    return HttpNotFound();
                if (model.RequestId > 0)
                {
                    if (RequestIsReturn(model.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                _requestService.DeleteEducationReasearch(id);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_آموزشی_پژوهشی,
                    OperationDetail = OperationsDetail.حذف,
                    Description = " حذف فعالیت آموزشی پژوهشی مربوط به درخواست شماره " + id
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "حذف شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در حذف فعالیت آموزشی پژوهشی!" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult DeleteDissertation(int id)
        {
            try
            {
                var model = _requestService.FindDissertation(id);
                if (model == null)
                    return HttpNotFound();
                if (model.RequestId > 0)
                {
                    if (RequestIsReturn(model.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                _requestService.DeleteDissertation(id);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_پروژه_پایاننامه_مقاله,
                    OperationDetail = OperationsDetail.حذف,
                    Description = " حذف فعالیت پروژه،پایان نامه،مقاله مربوط به درخواست شماره " + id
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "حذف شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در حذف فعالیت پروژه ، پایان نامه" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult DeleteScientificExecutive(int id)
        {
            try
            {
                var model = _requestService.FindScientificExecutive(id);
                if (model == null)
                    return HttpNotFound();
                if (model.RequestId > 0)
                {
                    if (RequestIsReturn(model.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                _requestService.DeleteScientificExecutive(id);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_علمی_اجرایی,
                    OperationDetail = OperationsDetail.حذف,
                    Description = " حذف فعالیت علمی اجرایی مربوط به درخواست شماره " + id
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "حذف شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در حذف فعالیت علمی اجرایی" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult DeleteTechnology(int id)
        {
            try
            {
                var model = _requestService.FindTechnology(id);
                if (model == null)
                    return HttpNotFound();
                if (model.RequestId > 0)
                {
                    if (RequestIsReturn(model.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                _requestService.DeleteTechnology(id);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_پژوهشی_فناوری,
                    OperationDetail = OperationsDetail.حذف,
                    Description = " حذف فعالیت پژوهشی فناوری مربوط به درخواست شماره " + id
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "حذف شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در حذف فعالیت فناوری" });
            }
        }

        public virtual ActionResult UpdateEducationReasearch(EducationalResearch educationalResearch)
        {
            try
            {
                if (educationalResearch.RequestId > 0)
                {
                    if (RequestIsReturn(educationalResearch.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                //string er;
                //foreach (ModelState modelState in ViewData.ModelState.Values)
                //{
                //    foreach (ModelError error in modelState.Errors)
                //    {
                //        er=error.ErrorMessage;
                //    }
                //}
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر" });
                }
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_آموزشی_پژوهشی,
                    OperationDetail = OperationsDetail.ویرایش,
                    Description = " ویرایش فعالیت آموزشی پژوهشی مربوط به درخواست شماره " + educationalResearch.RequestId
                };
                _logService.AddNewLog(log);
                _requestService.EditEducationalResearch(educationalResearch);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "تغییرات اعمال شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ویرایش فعالیت آموزشی پژوهشی" });
            }
        }

        public virtual ActionResult UpdateDissertation(Dissertation dissertation)
        {
            try
            {
                if (dissertation.RequestId > 0)
                {
                    if (RequestIsReturn(dissertation.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر" });
                }
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_پروژه_پایاننامه_مقاله,
                    OperationDetail = OperationsDetail.ویرایش,
                    Description = " ویرایش فعالیت پروژه،پایان نامه،مقاله مربوط به درخواست شماره " + dissertation.RequestId
                };
                _logService.AddNewLog(log);
                _requestService.EditDissertation(dissertation);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "تغییرات اعمال شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ویرایش فعالیت پروژه پایان نامه" });
            }
        }
        public virtual ActionResult UpdateScientificExecutive(ScientificExecutive scientificExecutive)
        {
            try
            {
                if (scientificExecutive.RequestId > 0)
                {
                    if (RequestIsReturn(scientificExecutive.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر" });
                }
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_علمی_اجرایی,
                    OperationDetail = OperationsDetail.ویرایش,
                    Description = " ویرایش فعالیت علمی اجرایی مربوط به درخواست شماره " + scientificExecutive.RequestId
                };
                _logService.AddNewLog(log);
                _requestService.EditScientificExecutive(scientificExecutive);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "تغییرات اعمال شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ویرایش فعالیت علمی اجرایی" });
            }
        }
        public virtual ActionResult UpdateTechnology(Technology technology)
        {
            try
            {
                if (technology.RequestId > 0)
                {
                    if (RequestIsReturn(technology.RequestId))
                    {
                        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
                    }
                }
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = true, Msg = "ورودی نامعتبر" });
                }
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.فعالیت_پژوهشی_فناوری,
                    OperationDetail = OperationsDetail.ویرایش,
                    Description = " ویرایش فعالیت پژوهشی فن آوری مربوط به درخواست شماره " + technology.RequestId
                };
                _logService.AddNewLog(log);
                _requestService.EditTechnology(technology);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "تغییرات اعمال شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ویرایش فعالیت فناوری" });
            }
        }

        [HttpGet]
        [AjaxOnly]
        public virtual ActionResult UploadAttachReasearch(int reaserchId)
        {
            var model = _requestService.FindEducationResearch(reaserchId);
            return PartialView("_AttachReaserch", model);
        }

        [HttpPost]
        public virtual ActionResult UploadAttachReasearch(int? chunk, string name, int reaserchId)
        {
            var fileUpload = Request.Files[0];
            var uploadPath = Server.MapPath("~/Content/Attachments/Reaserch");
            bool exist = System.IO.File.Exists(uploadPath);
            if (!exist)
                System.IO.Directory.CreateDirectory(uploadPath);
            uploadPath += "/";
            chunk = chunk ?? 0;
            using (var fs = new FileStream(Path.Combine(uploadPath, name), chunk == 0 ? FileMode.Create : FileMode.Append))
            {
                if (fileUpload != null)
                {
                    var buffer = new byte[fileUpload.InputStream.Length];
                    fileUpload.InputStream.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, buffer.Length);
                    var model = new AttachmentResearch
                    {
                        EducationaResearchId = reaserchId,
                        FileName = name
                    };

                    _requestService.AddAttachmentResearch(model);
                }
                _unitOfWork.SaveChanges();
            }
            return Content("chunk uploaded", "text/plain");
        }
        public virtual ActionResult ListReaserchAttachForm(int researchId)
        {
            var model = _requestService.FindEducationResearch(researchId);
            return PartialView("_listReaserchAttach", model);
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult destroyAttachResearch(long researchAttachId)
        {
            try
            {
                var researchAttach = _requestService.FindAttachforResearch(researchAttachId);
                string path =
                        Server.MapPath("~/Content/Attachments/Reaserch/" + researchAttach.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                _requestService.DeleteReasearchAttach(researchAttach.Id);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.نقش,
                    OperationDetail = OperationsDetail.حذف,
                    Description = " حذف فایل پیوستیبه نام " + researchAttach.FileName
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
        public virtual ActionResult GetReaserchAttach(int researchId)
        {
            try
            {
                var data = _requestService.GetAttachmentResearches(researchId).ToList();
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    var itemObject = new JObject
                    {
                        {"Id",item.Id},
                        {"EducationaResearchId",item.EducationaResearchId},
                        {"FileName",item.FileName}
                    };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _requestService.GetEducation_ByRequestId_ForCurrentYear(researchId).Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود فعالیت پژوهشی" });
            }
        }
        public virtual ActionResult GetDissertation(int requestId)
        {
            try
            {
                var data = _requestService.GetDissertation_ByRequestId_ForCurrentYear(requestId).ToList();
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    var itemObject = new JObject
                    {
                        {"Id",item.Id},
                        {"RequestId",item.RequestId},
                        {"Subject",item.Subject},
                        {"StudentName",item.StudentName},
                        {"GradeEducation",(int)item.GradeEducation},
                        {"GradeEducationTitle", ((GradeEducation) item.GradeEducation).ToString().Replace("_", " ")},
                        {"BeginDate",item.BeginDate},
                        {"VindicationDate",item.VindicationDate },
                        {"UnitCount",item.UnitCount },
                    };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _requestService.GetDissertation_ByRequestId_ForCurrentYear(requestId).Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود فعالیت پروژه پایان نامه" });
            }
        }
        public virtual ActionResult GetScientificExecutive(long requestid)
        {
            try
            {
                var data = _requestService.GetScientificExecutive_ByRequestId_ForCurrentYear(requestid).ToList();
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    var itemObject = new JObject
                    {
                        {"Id",item.Id},
                        {"RequestId",item.RequestId},
                        {"Subject",item.Subject},
                        {"StartDate",item.StartDate},
                        {"EndDate",item.EndDate},
                        {"TimeofMounth",item.TimeofMounth},
                        {"Score",item.Score}
                    };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _requestService.GetScientificExecutive_ByRequestId_ForCurrentYear(requestid).Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود فعالیت علمی اجرایی" });
            }
        }
        public virtual ActionResult GetTechnologies(int requestId)
        {
            try
            {
                var data = _requestService.GetTechnology_ByRequestId_ForCurrentYear(requestId).ToList();
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    var itemObject = new JObject
                    {
                        {"Id",item.Id},
                        {"RequestId",item.RequestId},
                        {"TechnologyType",(int)item.TechnologyType},
                        {"TechnologyTypeTitle",item.TechnologyType.ToString().Replace("_"," ")},
                        {"Subject",item.Subject},
                        {"PresentationDate",item.PresentationDate},
                        {"PlacePresentation",item.PlacePresentation},
                        {"PartnersNames",item.PartnersNames}
                    };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _requestService.GetTechnology_ByRequestId_ForCurrentYear(requestId).Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود فعالیت فناوری" });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult CreateFurtherInformation(FurtherInformation furtherInformation,
            IEnumerable<HttpPostedFileBase> officersStudyPhDUpload,
            IEnumerable<HttpPostedFileBase> typeOfPartTimeWorkNoconnectionOtherUniversityUpload,
            IEnumerable<HttpPostedFileBase> typeOfPartTimeButEmployeesOtherUniversityUpload,
            IEnumerable<HttpPostedFileBase> typeOfFullTimeButRetiredUpload,
            IEnumerable<HttpPostedFileBase> typeOfFullTimeButNoRetired,
            IEnumerable<HttpPostedFileBase> passPhDPassaUpload,
            IEnumerable<HttpPostedFileBase> takingSabbaticalUpload,
            IEnumerable<HttpPostedFileBase> freedmanOrCaptiveTypeUpload,
            IEnumerable<HttpPostedFileBase> fighterTypeUpload,
            IEnumerable<HttpPostedFileBase> veteranTypeUpload
            )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Success = false, Msg = "ورودی نامعتبر!" });
                }
                var entity = _requestService.FindFurtherInformation(furtherInformation.Id);
                if (entity == null)
                {
                    Log log = new Log()
                    {
                        UserId = int.Parse(User.Identity.GetUserId()),
                        Operation = Operations.اطلاعات_تکمیلی_درخواست,
                        OperationDetail = OperationsDetail.ایجاد,
                        Description = " ایجاد فعالیت تکمیلی مربوط به درخواست شماره "
                    };
                    _logService.AddNewLog(log);
                    _requestService.AddFurtherInformation(furtherInformation);
                }
                else
                {
                    Log log = new Log()
                    {
                        UserId = int.Parse(User.Identity.GetUserId()),
                        Operation = Operations.اطلاعات_تکمیلی_درخواست,
                        OperationDetail = OperationsDetail.ویرایش,
                        Description = " ویرایش فعالیت تکمیلی مربوط به درخواست شماره " + furtherInformation.RequestId
                    };
                    _logService.AddNewLog(log);
                    _requestService.EditFurtherInformation(furtherInformation);
                }
                var path = Server.MapPath("~/Content/FurtherInformation/" + User.Identity.Name);

                bool exist = System.IO.File.Exists(path);
                if (!exist)
                    System.IO.Directory.CreateDirectory(path);
                path += "/";
                // ثبت پیوست های مربوطه
                if (officersStudyPhDUpload != null)
                {
                    foreach (var item in officersStudyPhDUpload)
                    {
                        string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);

                        _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                        {
                            FurtherInformationId = furtherInformation.Id,
                            FurtherInformationType = FurtherInformationType.مامور_به_تحصیل_در_مقطع_دکتری_داخل_یا_خارج,
                            ImageName = Path.GetFileName(temppath)
                        });
                    }
                }
                if (typeOfPartTimeWorkNoconnectionOtherUniversityUpload != null)
                {
                    foreach (var item in typeOfPartTimeWorkNoconnectionOtherUniversityUpload)
                    {
                        string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);

                        _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                        {
                            FurtherInformationId = furtherInformation.Id,
                            FurtherInformationType = FurtherInformationType.نوع_همکاری_نیمه_وقت_بدون_هیچگونه_ارتباط_با_سایر_دانشگاهها,
                            ImageName = Path.GetFileName(temppath)
                        });
                    }
                }
                if (typeOfPartTimeButEmployeesOtherUniversityUpload != null)
                {
                    foreach (var item in typeOfPartTimeButEmployeesOtherUniversityUpload)
                    {
                        string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);

                        _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                        {
                            FurtherInformationId = furtherInformation.Id,
                            FurtherInformationType = FurtherInformationType.نوع_همکاری_نیمه_وقت_اما_به_صورت_تمام_وقت_شاغل_در_سایر_دانشگاهها_با_ارائه_حکم_کارگزینی_مبنی_بر_اعطای_پایه_سالیانه_دردانشگاه_دولتی,
                            ImageName = Path.GetFileName(temppath)
                        });
                    }
                }
                if (typeOfFullTimeButRetiredUpload != null)
                {
                    foreach (var item in typeOfFullTimeButRetiredUpload)
                    {
                        string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);

                        _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                        {
                            FurtherInformationId = furtherInformation.Id,
                            FurtherInformationType = FurtherInformationType.نوع_همکاری_تمام_وقت_اما_بازنشسته_از_سایردانشگاههای_وابسته,
                            ImageName = Path.GetFileName(temppath)
                        });
                    }
                }
                if (typeOfFullTimeButNoRetired != null)
                {
                    foreach (var item in typeOfFullTimeButNoRetired)
                    {
                        string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);

                        _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                        {
                            FurtherInformationId = furtherInformation.Id,
                            FurtherInformationType = FurtherInformationType.نوع_همکاری_تمام_وقت_اما_غیر_بازنشسته_در_صورت_استفاء,
                            ImageName = Path.GetFileName(temppath)
                        });
                    }
                }
                if (passPhDPassaUpload != null)
                {
                    foreach (var item in passPhDPassaUpload)
                    {
                        string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);

                        _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                        {
                            FurtherInformationId = furtherInformation.Id,
                            FurtherInformationType = FurtherInformationType.گواهی_دوره_پسا_دکترا,
                            ImageName = Path.GetFileName(temppath)
                        });
                    }
                }
                if (takingSabbaticalUpload != null)
                {
                    foreach (var item in takingSabbaticalUpload)
                    {
                        string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);

                        _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                        {
                            FurtherInformationId = furtherInformation.Id,
                            FurtherInformationType = FurtherInformationType.مجوز_سازمان_مرکزی_برای_فرصت_مطالعاتی,
                            ImageName = Path.GetFileName(temppath)
                        });
                    }
                }
                if (freedmanOrCaptiveTypeUpload != null)
                {
                    foreach (var item in freedmanOrCaptiveTypeUpload)
                    {
                        string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);

                        _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                        {
                            FurtherInformationId = furtherInformation.Id,
                            FurtherInformationType = FurtherInformationType.مستندات_آزادگان_اسرا_مفقودین,
                            ImageName = Path.GetFileName(temppath)
                        });
                    }
                }
                if (fighterTypeUpload != null)
                {
                    foreach (var item in fighterTypeUpload)
                    {
                        string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);

                        _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                        {
                            FurtherInformationId = furtherInformation.Id,
                            FurtherInformationType = FurtherInformationType.مستندات_رزمنده,
                            ImageName = Path.GetFileName(temppath)
                        });
                    }
                }
                if (veteranTypeUpload != null)
                {
                    foreach (var item in veteranTypeUpload)
                    {
                        string temppath = path + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                        item.InputStream.ResizeImageByWidth(500, temppath, Utility.ImageComperssion.Normal);

                        _requestService.AddAttachmentFurtherInformation(new AttachmentFurtherInformation
                        {
                            FurtherInformationId = furtherInformation.Id,
                            FurtherInformationType = FurtherInformationType.مستندات_جانبازی,
                            ImageName = Path.GetFileName(temppath)
                        });
                    }
                }
                //--------------------------
                _unitOfWork.SaveChanges();

                PersianCalendar d = new PersianCalendar();
                int uId = int.Parse(User.Identity.GetUserId());

                var model = new AddRequestViewModel();
                var _term = d.GetYear(DateTime.Now);
                model.Request = _requestService.Where(x => x.Term == _term && x.UserId == uId).Include(x => x.UnivercityStructure).FirstOrDefault();
                model.UnivercityStructures = _univercityStructureService.Where(x => x.HasChild == false && x.Level == 3).ToList();


                if (model.Request != null)
                {
                    var md = _requestService.Where(x => x.RequestId == model.Request.Id).ToList().FirstOrDefault();
                    if (md == null)
                    {
                        model.FurtherInformation = new FurtherInformation();
                        model.AttachmentFurtherInformations = new List<AttachmentFurtherInformation>();
                    }
                    else
                    {
                        model.FurtherInformation = md;
                        model.AttachmentFurtherInformations =
                            _requestService.GetAttachmentFurtherInformations(md.Id).ToList();
                    }
                }
                else
                {
                    model.Request = new Request();
                    model.FurtherInformation = new FurtherInformation();
                    model.AttachmentFurtherInformations = new List<AttachmentFurtherInformation>();
                }
                return PartialView("_CreateFurtherInformation", model);
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ثبت اطلاعات تکمیلی درخواست" });
            }
        }

        public Request GetLastRequest(Request req)
        {
            return new Request()
            {
                UserId = req.UserId,
                Term = req.Term,
                EmploymentStatus = req.EmploymentStatus,
                AcademicDegree = req.AcademicDegree,
                UnivercityStructureId = req.UnivercityStructureId,
                LastDateGrade = req.LastDateGrade,
                Status = req.Status,
                PresenceInUnivercity = req.PresenceInUnivercity,
                FurtherInformations = req.FurtherInformations,
            };
        }
        public FurtherInformation GetLastFurtherInformation(FurtherInformation fi)
        {
            return new FurtherInformation()
            {
                AttachmentBasicDelayedPreviousYearses = fi.AttachmentBasicDelayedPreviousYearses,
                AttachmentFurtherInformations = fi.AttachmentFurtherInformations,
                ExecutivePostName = fi.ExecutivePostName,
                Fighter = fi.Fighter,
                FighterType = fi.FighterType,
                FreedmanOrCaptive = fi.FreedmanOrCaptive,
                FreedmanOrCaptiveType = fi.FreedmanOrCaptiveType,
                GraduationDaneshvari = fi.GraduationDaneshvari,
                HasBasicDelayedPreviousYears = fi.HasBasicDelayedPreviousYears,
                HasExecutivePosts = fi.HasExecutivePosts,
                HasMaternityLeave = fi.HasMaternityLeave,
                HasPhDIncludingMilitary = fi.HasPhDIncludingMilitary,
                HasPhDNoOfficersStudy = fi.HasPhDNoOfficersStudy,
                IsVeteran = fi.IsVeteran,
                OfficersStudyPhD = fi.OfficersStudyPhD,
                PassPhDPassa = fi.PassPhDPassa,
                TakingSabbatical = fi.TakingSabbatical,
                TypeOfFullTimeButNoRetired = fi.TypeOfFullTimeButNoRetired,
                TypeOfFullTimeButRetired = fi.TypeOfFullTimeButRetired,
                TypeOfPartTimeButEmployeesOtherUniversity = fi.TypeOfPartTimeButEmployeesOtherUniversity,
                TypeOfPartTimeWorkNoconnectionOtherUniversity = fi.TypeOfPartTimeWorkNoconnectionOtherUniversity,
                VeteranType = fi.VeteranType
            };

        }

        public bool RequestIsReturn(long reqId)
        {
            var data = _cartableService.Where(c => c.RequestId == reqId && c.Active == true && c.CurrentCartable != CurrentCartable.برگشت).FirstOrDefault();
            return data != null;
        }

        //public void CheckStatusRequestWithJsonMessage(long reqId)
        //{
        //    var data = _cartableService.Where(c => c.RequestId == reqId && c.Active == true && c.CurrentCartable == CurrentCartable.برگشت).FirstOrDefault();
        //    if (data != null)
        //    {
        //        return Json(new { isError = true, Msg = "درخواست ارسال شده قابل ویرایش نمی باشد!" });
        //    }
        //}
    }
}