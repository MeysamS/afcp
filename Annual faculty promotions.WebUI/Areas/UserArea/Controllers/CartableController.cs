using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Enums;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http.Headers;
using System.Transactions;
using Annual_faculty_promotions.WebUI.Helpers.Util;
using Postal;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.WebUI.Areas.UserArea.Models;
using Annual_faculty_promotions.WebUI.Helpers.Filters;
using Microsoft.Ajax.Utilities;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{
    [Authorize]
    public partial class CartableController : Controller
    {
        private readonly IStageService _stageService;
        private readonly IUnivercityStructureService _univercityStructure;
        private readonly IUserService _userService;
        private readonly IApplicationRoleManager _roleManager;
        private readonly ICartableService _cartableService;
        private readonly IArchiveService _archiveService;
        private readonly IRequestService _requestService;
        private readonly ILogService _logService;
        private readonly IApplicationRoleManager _roleService;
        private readonly IDefinitionService _definitionService;
        private readonly IMessagingService _messagingService;
        private readonly IUnitOfWork _unitOfWork;

        public CartableController(IUnitOfWork unitOfWork, IStageService stageService,
            IApplicationRoleManager roleManager, IUserService userService, ICartableService cartableService,
            IArchiveService archiveService, IRequestService requestService, ILogService logService,
            IApplicationRoleManager roleService, IUnivercityStructureService univercityStructure,
            IDefinitionService definitionService, IMessagingService messagingService)
        {
            _unitOfWork = unitOfWork;
            _stageService = stageService;
            _roleManager = roleManager;
            _userService = userService;
            _cartableService = cartableService;
            _univercityStructure = univercityStructure;
            _archiveService = archiveService;
            _requestService = requestService;
            _logService = logService;
            _roleManager = roleManager;
            _definitionService = definitionService;
            _messagingService = messagingService;
           
            
        }

        //[Expire]
        public virtual ActionResult Index()
        {
            //var lastStage =
            //        _stageService.GetAllStages().OrderByDescending(m => m.StageNumber).Select(s => s.StageNumber).FirstOrDefault();
            //var EndStage = _stageService.Where(s => s.StageNumber == lastStage).Select(s => s.Id).FirstOrDefault();
            //ViewBag.EndStage = EndStage;
            return View();
        }

        public virtual ActionResult GetCartables()
        {
            try
            {
                PersianCalendar pdate = new PersianCalendar();
                int uId = int.Parse(User.Identity.GetUserId());
                var lastStage =
                    _stageService.GetAllStagesAsQueryable().OrderByDescending(m => m.StageNumber).Select(s => s.StageNumber).FirstOrDefault();
                var endStageId = _stageService.Where(s => s.StageNumber == lastStage).Select(s => s.Id).FirstOrDefault();

                var items = _cartableService.Where(c => c.UserReciveId == uId && c.Active)
                    .Include(i => i.Request).Include(i => i.Request.User).Include(i => i.Request.User.Profile)
                    .Include(i => i.UserSender).Include(i => i.UserSender.Profile).Include(i => i.Stage)
                    .Include(i => i.Stage.Role).Include(i => i.Request.FurtherInformations)
                    .Include(i => i.Request.EducationalResearches).Include(i => i.Request.ScientificExecutives)
                    .Include(i => i.Request.Technologies).Include(i => i.Request.Dissertations);

                JArray ja = new JArray();

                foreach (var item in items)
                {
                    var itemObject = new JObject
                {
                    {"Id", item.Id},
                    {"OwnerRequest", item.Request.User.Profile.Name+" "+item.Request.User.Profile.Family},
                    {"text", "درخواست شماره " + item.Request.Id},
                    {"UserAvatar", item.UserSender.Profile.Avatar},
                    {"NameFamily", item.UserSender.Profile.Name + " " + item.UserSender.Profile.Family},
                    {"CurrentCartable",((int)item.CurrentCartable).ToString()},
                    {
                        "CreateDate",
                        pdate.GetYear(item.CreatedDate) + "/" + pdate.GetMonth(item.CreatedDate) + "/" +
                        pdate.GetDayOfMonth(item.CreatedDate)
                    },
                    {"RelativeTimeCreateDate", RelativeTimeCalculator.Calculate(item.CreatedDate)},
                    {"Description", item.Description},
                    {"RequestId", item.Request.Id},
                    {"StageId", item.StageId},
                     {"StageName", item.Stage.Name},
                    {"EndStage", endStageId},
                    {"RoleName",item.Stage.Role.Name},
                    {"FurtherInformations", item.Request.FurtherInformations.Any() ? "اطلاعات تکمیلی" : ""},
                    {
                        "Educational",
                        item.Request.EducationalResearches.Any(
                            x => x.EducationalResearchStatus == (int) EducationalResearchStatus.آموزشی)
                            ? "فعالیت های آموزشی"
                            : ""
                    },
                    {
                        "Researches",
                        item.Request.EducationalResearches.Any(
                            x => x.EducationalResearchStatus == (int) EducationalResearchStatus.پژوهشی)
                            ? "فعالیت های آموزشی"
                            : ""
                    },
                    {"ScientificExecutives", item.Request.ScientificExecutives.Any() ? "فعالیت های آموزشی اجرایی" : ""},
                    {
                        "Dissertations",
                        item.Request.Dissertations.Any() ? "راهنمایی پروژه،پایان نامه،و رساله دانشجویی خاتمه یافته" : ""
                    },
                    {"Technologies", item.Request.Technologies.Any() ? "فعالیت های پژوهشی فناوری" : ""}
                    //{"FurtherInformationId",item.Request.FurtherInformations.Any()?item.Request.FurtherInformations.First().Id:0},
                    //{"EducationalId",item.Request.EducationalResearches.Any(x=>x.EducationalResearchStatus==(int) EducationalResearchStatus.آموزشی)?item.Request.EducationalResearches.First(x=>x.EducationalResearchStatus==(int) EducationalResearchStatus.آموزشی).Id:0},
                    //{"ResearcheId",item.Request.EducationalResearches.Any(x=>x.EducationalResearchStatus==(int) EducationalResearchStatus.پژوهشی)?item.Request.EducationalResearches.First(x=>x.EducationalResearchStatus==(int) EducationalResearchStatus.پژوهشی).Id:0},
                    //{"ScientificExecutiveId",item.Request.ScientificExecutives.Any()?item.Request.ScientificExecutives.First().Id:0},
                    //{"DissertationId",item.Request.Dissertations.Any()?item.Request.Dissertations.First().Id:0},
                    //{"TechnologieId",item.Request.Technologies.Any()?item.Request.Technologies.First().Id:0},
                };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _cartableService.GetAllCartablesAsQueryable().Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات کارتابل" });
            }
        }

        public OutSend SendCartable(long cartableId, string desc)
        {
            int stageId = 0;
            try
            {
                var entity =
                    _cartableService.Where(x => x.Id == cartableId).Include(x => x.Request).Include(x => x.Stage)
                        .Include(r => r.Request.UnivercityStructure).Include(r => r.Request.UnivercityStructure.Parent)
                        .Include(r => r.Request.UnivercityStructure.Parent.Parent).FirstOrDefault();

                if (entity == null)
                {
                    return new OutSend(true, "خطا در بازیابی درخواست!", entity.UserReciveId, entity.UserSenderId);
                }

                entity.Active = false;
                //entity.CurrentCartable = CurrentCartable.عادی;
                _cartableService.EditCartable(entity);

                var newCartable = new Cartable
                {
                    RequestId = entity.RequestId,
                    UserSenderId = entity.UserReciveId,
                    Active = true,
                    CurrentCartable = CurrentCartable.عادی,
                    Description = desc
                };
                stageId =
                    _stageService.Where(x => x.StageNumber == entity.Stage.StageNumber + 1)
                        .Select(x => x.Id)
                        .FirstOrDefault();
                if (stageId == 0)
                {
                    return new OutSend(true, "مرحله بعد تعریف نشده است!", entity.UserReciveId, entity.UserSenderId);
                }
                newCartable.StageId = stageId;

                var userRole = _roleManager.GetAllCustomUserRole()
                    .Include(r => r.Department)
                    .Include(r => r.Department.Parent)
                    .Include(r => r.Department.Parent.Parent)
                    .Where(r => r.RoleId == stageId).ToList();
                if (userRole.Any())
                {
                    var tempUserRole =
                        userRole.Where(r => r.Department.Id == entity.Request.UnivercityStructureId).ToList();

                    if (tempUserRole.Any())
                    {
                        newCartable.UserReciveId = tempUserRole.First().UserId;
                    }
                    else
                    {
                        var result = userRole.FirstOrDefault(x => x.Department.Id == entity.Request.UnivercityStructure.Parent.Id);
                        if (result == null)
                        {
                            if (userRole.First().Department.Level == entity.Request.UnivercityStructure.Parent.Level)
                            {
                                return new OutSend(true, " امکان ارسال وجود ندارد! کاربری به این نقش نسبت داده نشده است!", entity.UserReciveId, entity.UserSenderId);
                            }

                            else
                            {
                                result = userRole.FirstOrDefault(x => x.Department.Id == entity.Request.UnivercityStructure.Parent.Parent.Id);
                                if (result == null)
                                {
                                    return new OutSend(true, " امکان ارسال وجود ندارد! کاربری به این نقش نسبت داده نشده است!", entity.UserReciveId, entity.UserSenderId);
                                }
                            }
                        }
                        newCartable.UserReciveId = result.UserId;
                    }
                }
                else
                {
                    return new OutSend(true, " امکان ارسال وجود ندارد! کاربری به این نقش نسبت داده نشده است!", entity.UserReciveId, entity.UserSenderId);
                }
                _cartableService.AddNewCartable(newCartable);

                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.کارتابل,
                    OperationDetail = OperationsDetail.ارسال,
                    Description = "تایید و ارسال درخواست شماره " + newCartable.RequestId
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();

                return new OutSend(false, "ارسال با موفقیت انجام شد!", entity.UserReciveId, entity.UserSenderId);
            }
            catch (Exception e)
            {
                return new OutSend(true, "خطا در تایید و ارسال!", 0, 0);
            }
        }

        [HttpPost]
        public virtual ActionResult Send(long cartableId, string desc)
        {
            var sended = SendCartable(cartableId, desc);
            return Json(new { isError = sended.IsError, Msg = sended.Msg, uidRecive = sended.UidRecive, uidSender = sended.UidSender });
        }

        [HttpPost]
        [Authorize]
        public virtual ActionResult SendRequest(long requestId, string desc)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    if (requestId > 0)
                    {
                        int req = _cartableService.Where(c => c.RequestId == requestId).Count();
                        if (req > 0) //&& (RequestIsReturn(requestId)))
                        {
                            return Json(new { isError = true, Msg = "درخواست مورد نظر قبلا ارسال شده است!" });
                        }
                    }
                    else
                    {
                        return Json(new { isError = true, Msg = "درخواست مورد نظر نامعتبر می باشد!" });
                    }
                    PersianCalendar d = new PersianCalendar();
                    var term = d.GetYear(DateTime.Now);
                    var request = _requestService.GetAllRequestsAsQueryable().Where(x => x.Id == requestId)
                        .Include(r => r.UnivercityStructure).Include(r => r.UnivercityStructure.Parent)
                        .Include(r => r.UnivercityStructure.Parent.Parent).FirstOrDefault(); // 

                    var newCartable = new Cartable
                    {
                        RequestId = request.Id,
                        UserSenderId = request.UserId,
                        Active = true,
                        CurrentCartable = CurrentCartable.عادی,
                        Description = desc
                    };
                    var stageFirst = _stageService.GetAllStagesAsQueryable().OrderBy(s => s.StageNumber).Select(s => s.StageNumber).FirstOrDefault();
                    int stageId = _stageService.Where(x => x.StageNumber == stageFirst).Select(x => x.Id).FirstOrDefault();
                    if (stageId == 0)
                    {
                        return Json(new { isError = true, Msg = "مرحله بعد تعریف نشده است!" });
                    }
                    newCartable.StageId = stageId;

                    newCartable.UserReciveId = newCartable.UserSenderId;
                    _cartableService.AddNewCartable(newCartable);
                    Log log = new Log()
                    {
                        UserId = int.Parse(User.Identity.GetUserId()),
                        Operation = Operations.درخواست,
                        OperationDetail = OperationsDetail.ارسال,
                        Description = "ارسال درخواست شماره " + newCartable.RequestId
                    };
                    _logService.AddNewLog(log);
                    _unitOfWork.SaveChanges();

                    var sended = SendCartable(newCartable.Id, desc);
                    if (!sended.IsError)
                        scope.Complete();
                    return Json(new { uidRecive = newCartable.UserReciveId, uidSender = newCartable.UserSenderId, isError = sended.IsError, Msg = sended.Msg });
                    //return Json(new { uidRecive = newCartable.UserReciveId, isError = false, Msg = "ارسال با موفقیت انجام شد!" });
                }
                catch (Exception)
                {
                    return Json(new { isError = true, Msg = "ارسال با خطا مواجه شد!" });
                }
            }
        }

        [Authorize(Roles = "RK")]
        public virtual ActionResult Archive(long cartableId)
        {
            var cartable =
                _cartableService.Where(c => c.Id == cartableId && c.CurrentCartable == CurrentCartable.عادی).Include(i => i.Request).Include(i => i.Request.User)
                    .Include(i => i.Request.User.Profile).Include(i => i.Request.UnivercityStructure)
                    .Include(i => i.Request.ScientificExecutives).FirstOrDefault();
            if (cartable == null)
                return HttpNotFound();
            var archive = new ArchiveViewModel();
            var entityArchive = _archiveService.Where(a => a.Request.Id == cartable.RequestId).FirstOrDefault();
            if (entityArchive != null)
            {
                archive.Archive = entityArchive;
                archive.Archive.TechnologyScore = _requestService.WhereTechnology(s => s.RequestId == cartable.RequestId &&
                                                                         s.TechnologyType !=
                                                                         TechnologyType.تالیف_ترجمه_یا_تصحیح_کتاب &&
                                                                         s.TechnologyType !=
                                                                         TechnologyType.مفالات_ارائه_شده_در_همایشها &&
                                                                         s.TechnologyType !=
                                                                         TechnologyType.مقالات_چاپ_شده_در_مجلات)
                    .Select(s => s.Score)
                    .DefaultIfEmpty(0)
                    .Sum();
            }
            else
            {
                var sumEducation =
                    _requestService.GetEducation_ByRequestId_ForCurrentYear(cartable.RequestId)
                        .Select(s => s.UnitEqual)
                        .DefaultIfEmpty(0)
                        .Sum();
                var sumResearch =
                    _requestService.GetReasearch_ByRequestId_ForCurrentYear(cartable.RequestId)
                        .Select(s => s.UnitEqual)
                        .DefaultIfEmpty(0)
                        .Sum();
                var sumDissertation =
                    _requestService.GetDissertation_ByRequestId_ForCurrentYear(cartable.RequestId)
                        .Select(s => s.UnitEqual)
                        .DefaultIfEmpty(0)
                        .Sum();

                var sumTecknology = _requestService.WhereTechnology(s => s.RequestId == cartable.RequestId &&
                                                                         s.TechnologyType !=
                                                                         TechnologyType.تالیف_ترجمه_یا_تصحیح_کتاب &&
                                                                         s.TechnologyType !=
                                                                         TechnologyType.مفالات_ارائه_شده_در_همایشها &&
                                                                         s.TechnologyType !=
                                                                         TechnologyType.مقالات_چاپ_شده_در_مجلات)
                    .Select(s => s.Score)
                    .DefaultIfEmpty(0)
                    .Sum();

                var sumScientificExecutive =
                    _requestService.GetScientificExecutive_ByRequestId_ForCurrentYear(cartable.RequestId)
                        .Select(s => s.Score)
                        .DefaultIfEmpty(0)
                        .Sum();

                archive.Archive.Request = cartable.Request;
                archive.Archive.DissertationScore = sumDissertation;
                archive.Archive.EducationScore = sumEducation;
                archive.Archive.ResearchScore = sumResearch;
                archive.Archive.TechnologyScore = sumTecknology;
                archive.Archive.ExecutiveScore = sumScientificExecutive;
                archive.Archive.SumScore = sumDissertation + sumEducation + sumResearch + sumTecknology +
                                           sumScientificExecutive;
            }
            ViewBag.carableId = cartableId;
            return View(archive);
        }
        public virtual ActionResult GetOldTechnology(long requestId)
        {
            PersianCalendar p = new PersianCalendar();
            string year = p.GetYear(DateTime.Now).ToString();
            string mounth = "0" + p.GetMonth(DateTime.Now).ToString();
            string day = "0" + p.GetDayOfMonth(DateTime.Now).ToString();
            string dateString = year + "/" + mounth.Substring(mounth.Length - 2, 2) + "/" + day.Substring(day.Length - 2, 2);
            string bookLicenseDate = Convert.ToString(int.Parse(year) - 3) + "/" + mounth.Substring(mounth.Length - 2, 2) + "/" + day.Substring(day.Length - 2, 2);
            string articleLicenseDate = Convert.ToString(int.Parse(year) - 2) + "/" + mounth.Substring(mounth.Length - 2, 2) + "/" + day.Substring(day.Length - 2, 2);
            string[] date;
            try
            {
                var req = _requestService.Find(requestId);

                var lstTech = _requestService.WhereTechnology(s => s.Request.UserId == req.UserId &&
                    s.RequestId != requestId && ((s.TechnologyType == TechnologyType.تالیف_ترجمه_یا_تصحیح_کتاب && string.Compare(s.PresentationDate, bookLicenseDate, StringComparison.CurrentCulture) >= 0) ||
                    (s.TechnologyType == TechnologyType.مفالات_ارائه_شده_در_همایشها && string.Compare(s.PresentationDate, articleLicenseDate, StringComparison.CurrentCulture) >= 0) ||
                    (s.TechnologyType == TechnologyType.مقالات_چاپ_شده_در_مجلات && string.Compare(s.PresentationDate, articleLicenseDate, StringComparison.CurrentCulture) >= 0))).AsQueryable();

                var lstTechnology = (from tech in lstTech
                                     select new
                                     {
                                         tech.Id,
                                         tech.RequestId,
                                         tech.TechnologyType,
                                         tech.Subject,
                                         tech.PresentationDate,
                                         tech.Score,
                                         tech.RemindScore,
                                         //RemindScore = tech.Score - tech.TechnologyDetails.Where(td => td.TechnologyId == tech.Id).Sum(s => s.Score),
                                         UsingScore = tech.TechnologyDetails.Where(td => td.RequestId == requestId).Select(s => s.Score).FirstOrDefault()
                                     }).ToList();

                //var technologyDetail = _requestService.WhereTechnologyDetail(t => t.RequestId == requestId).AsQueryable();

                //var lstTechnology = (from tech in lstTech
                //                     join techDetail in technologyDetail on tech.Id equals techDetail.TechnologyId
                //                         into t
                //                     from td in t.DefaultIfEmpty(new TechnologyDetail())
                //                     where tech.RemindScore > 0 || tech.Id == td.TechnologyId
                //                     select new
                //                     {
                //                         tech.Id,
                //                         tech.RequestId,
                //                         tech.TechnologyType,
                //                         tech.Subject,
                //                         tech.PresentationDate,
                //                         tech.Score,
                //                         tech.RemindScore,
                //                         UsingScore = td.Score
                //                     }).ToList();

                List<TechnologyViewModel> data = new List<TechnologyViewModel>();
                foreach (var item in lstTechnology.Where(t => t.RemindScore > 0 || t.UsingScore > 0))
                {
                    //var techDetail =
                    //     _requestService.WhereTechnologyDetail(t => t.TechnologyId == item.Id && t.RequestId == requestId)
                    //         .Select(c => c.Score)
                    //         .FirstOrDefault();
                    TechnologyViewModel tvm = new TechnologyViewModel()
                    {
                        Id = item.Id,
                        Used = item.UsingScore > 0,
                        RequestId = item.RequestId,
                        TechnologyType = (int)item.TechnologyType,
                        TechnologyTypeTitle = (TechnologyType)item.TechnologyType,
                        Subject = item.Subject,
                        PresentationDate = item.PresentationDate,
                        Score = item.Score,
                        RemindScore = item.RemindScore,
                        UsingScore = item.UsingScore
                    };

                    date = tvm.PresentationDate.Split('/');
                    if (tvm.TechnologyType == (int)TechnologyType.تالیف_ترجمه_یا_تصحیح_کتاب)
                    {
                        tvm.LicenseDate = Convert.ToString(int.Parse(date[0]) + 3) + "/" + date[1] + "/" + date[2];
                    }
                    else
                    {
                        tvm.LicenseDate = Convert.ToString(int.Parse(date[0]) + 2) + "/" + date[1] + "/" + date[2];
                    }
                    data.Add(tvm);
                }
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    if (string.Compare(dateString, item.LicenseDate, StringComparison.CurrentCulture) > 0)
                        continue;
                    var itemObject = new JObject
                {
                    {"Id", item.Id},
                     {"Used",item.Used},
                    {"RequestId", item.RequestId},
                    {"TechnologyType",(int)item.TechnologyType},
                    {"TechnologyTypeTitle", ((TechnologyType) item.TechnologyType).ToString().Replace("_", " ")},
                    {"Subject", item.Subject},
                    {"PresentationDate", item.PresentationDate},
                    {"LicenseDate", item.LicenseDate},
                    {"Score", item.Score},
                    {"RemindScore", item.RemindScore},
                    {"UsingScore", item.UsingScore}
                };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", 3);
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات پژوهشی" });
            }
        }
        public virtual ActionResult GetNewTechnology(long requestId)
        {
            PersianCalendar p = new PersianCalendar();
            string year = p.GetYear(DateTime.Now).ToString();
            string mounth = "0" + p.GetMonth(DateTime.Now).ToString();
            string day = "0" + p.GetDayOfMonth(DateTime.Now).ToString();
            string dateString = year + "/" + mounth.Substring(mounth.Length - 2, 2) + "/" + day.Substring(day.Length - 2, 2);
            string bookLicenseDate = Convert.ToString(int.Parse(year) - 3) + "/" + mounth.Substring(mounth.Length - 2, 2) + "/" + day.Substring(day.Length - 2, 2);
            string articleLicenseDate = Convert.ToString(int.Parse(year) - 2) + "/" + mounth.Substring(mounth.Length - 2, 2) + "/" + day.Substring(day.Length - 2, 2);
            string[] date;
            try
            {
                var lstTech = _requestService.WhereTechnology(s => s.RequestId == requestId &&
                    ((s.TechnologyType == TechnologyType.تالیف_ترجمه_یا_تصحیح_کتاب && string.Compare(s.PresentationDate, bookLicenseDate, StringComparison.CurrentCulture) >= 0) ||
                    (s.TechnologyType == TechnologyType.مفالات_ارائه_شده_در_همایشها && string.Compare(s.PresentationDate, articleLicenseDate, StringComparison.CurrentCulture) >= 0) ||
                    (s.TechnologyType == TechnologyType.مقالات_چاپ_شده_در_مجلات && string.Compare(s.PresentationDate, articleLicenseDate, StringComparison.CurrentCulture) >= 0))).AsQueryable();

                var lstTechnology = (from tech in lstTech
                                     select new
                                     {
                                         tech.Id,
                                         tech.RequestId,
                                         tech.TechnologyType,
                                         tech.Subject,
                                         tech.PresentationDate,
                                         tech.Score,
                                         tech.RemindScore,
                                         UsingScore = tech.TechnologyDetails.Where(td => td.RequestId == requestId).Select(s => s.Score).FirstOrDefault()
                                     }).ToList();

                //var technologyDetail = _requestService.WhereTechnologyDetail(t => t.RequestId == requestId).AsQueryable();
                //var lstTechnology = (from tech in lstTech
                //                     join techDetail in technologyDetail on tech.Id equals techDetail.TechnologyId
                //                     into t
                //                     from td in t.DefaultIfEmpty(new TechnologyDetail())
                //                     select new
                //                     {
                //                         tech.Id,
                //                         tech.RequestId,
                //                         tech.TechnologyType,
                //                         tech.Subject,
                //                         tech.PresentationDate,
                //                         tech.Score,
                //                         tech.RemindScore,
                //                         //UsingScore = td.Score
                //                     }).ToList();

                List<TechnologyViewModel> data = new List<TechnologyViewModel>();
                foreach (var item in lstTechnology.Where(t => t.RemindScore > 0 || t.UsingScore > 0))
                {
                    TechnologyViewModel tvm = new TechnologyViewModel()
                    {
                        Id = item.Id,
                        Used = (item.Score != item.RemindScore),
                        RequestId = item.RequestId,
                        TechnologyType = (int)item.TechnologyType,
                        TechnologyTypeTitle = (TechnologyType)item.TechnologyType,
                        Subject = item.Subject,
                        PresentationDate = item.PresentationDate,
                        Score = item.Score,
                        RemindScore = item.RemindScore,
                        UsingScore = item.UsingScore
                    };
                    date = tvm.PresentationDate.Split('/');
                    if (tvm.TechnologyType == (int)TechnologyType.تالیف_ترجمه_یا_تصحیح_کتاب)
                    {
                        tvm.LicenseDate = Convert.ToString(int.Parse(date[0]) + 3) + "/" + date[1] + "/" + date[2];
                    }
                    else
                    {
                        tvm.LicenseDate = Convert.ToString(int.Parse(date[0]) + 2) + "/" + date[1] + "/" + date[2];
                    }
                    data.Add(tvm);
                }
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    if (string.Compare(dateString, item.LicenseDate, StringComparison.CurrentCulture) > 0)
                        continue;
                    var itemObject = new JObject
                {
                    {"Id", item.Id},
                    {"Used",item.Used},
                    {"RequestId", item.RequestId},
                    {"TechnologyType",(int)item.TechnologyType},
                    {"TechnologyTypeTitle", ((TechnologyType) item.TechnologyType).ToString().Replace("_", " ")},
                    {"Subject", item.Subject},
                    {"PresentationDate", item.PresentationDate},
                    {"LicenseDate", item.LicenseDate},
                    {"Score", item.Score},
                    {"RemindScore", item.RemindScore},
                    {"UsingScore", item.UsingScore}
                };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", data.Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات پژوهشی" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "RK")]
        public virtual ActionResult SaveTemporary(string oldTech, string newTech, Archive archive, long cartableId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    string[] arrOldTech = oldTech.Split('&');
                    string[] arrNewTech = newTech.Split('&');
                    string[] fields;
                    var entity =
                        _cartableService.Where(x => x.Id == cartableId).Include(x => x.Request).FirstOrDefault();
                    if (entity == null)
                        return HttpNotFound();

                    Archive newArchive;
                    newArchive = _archiveService.Where(a => a.Request.Id == entity.RequestId).FirstOrDefault();
                    if (newArchive == null)
                    {
                        newArchive = new Archive
                        {
                            Request = entity.Request,
                            UserId = entity.Request.UserId,
                            ChiefCommiteId = entity.UserReciveId,
                            ChiefId = entity.UserSenderId,
                            Description = archive.Description,
                            EducationScore = archive.EducationScore,
                            ResearchScore = archive.ResearchScore,
                            ExecutiveScore = archive.ExecutiveScore,
                            TechnologyScore = archive.TechnologyScore,
                            DissertationScore = archive.DissertationScore,
                            SumScore = archive.SumScore,
                            IsOpinionCommite = archive.IsOpinionCommite,
                            Grade = archive.Grade
                        };
                    }
                    else
                    {
                        newArchive.Description = archive.Description;
                        newArchive.EducationScore = archive.EducationScore;
                        newArchive.ResearchScore = archive.ResearchScore;
                        newArchive.ExecutiveScore = archive.ExecutiveScore;
                        newArchive.TechnologyScore = archive.TechnologyScore;
                        newArchive.DissertationScore = archive.DissertationScore;
                        newArchive.SumScore = archive.SumScore;
                        newArchive.IsOpinionCommite = archive.IsOpinionCommite;
                        newArchive.Grade = archive.Grade;
                    }

                    //var listTechnologyDetailInRequest =
                    //    _requestService.WhereTechnologyDetail(r => r.RequestId == entity.RequestId).ToList();
                    //foreach (var item in listTechnologyDetailInRequest)
                    //{
                    //    var technology = _requestService.FindTechnology(item.TechnologyId);
                    //    if (technology != null)
                    //    {
                    //        technology.RemindScore += item.Score;
                    //        _requestService.EditTechnology(technology);
                    //    }
                    //    _requestService.DeleteTechnologyDetail(item.TechnologyId, item.RequestId);
                    //}
                    _requestService.DeleteListofTechnologyDetail(entity.RequestId);

                    _unitOfWork.SaveChanges();
                    long lngTechId;
                    if (arrOldTech.Length > 1)
                    {
                        for (int i = 0; i < arrOldTech.Length - 1; i++)
                        {
                            fields = arrOldTech[i].Split(',');
                            lngTechId = Convert.ToInt64(fields[0]);
                            //Technology tech = _requestService.FindTechnology(lngTechId);
                            if ((Convert.ToSingle(fields[9]) <= 0))// || (tech == null))
                            {
                                continue;
                            }
                            var td =
                                _requestService.WhereTechnologyDetail(
                                    t =>
                                        t.TechnologyId == lngTechId &&
                                        t.RequestId == entity.RequestId).FirstOrDefault();
                            if (td == null)
                            {
                                //tech.RemindScore = tech.RemindScore - Convert.ToSingle(fields[9]);
                                td = new TechnologyDetail()
                                {
                                    TechnologyId = lngTechId,
                                    RequestId = entity.RequestId,
                                    Score = Convert.ToSingle(fields[9])
                                };
                            }
                            else
                            {
                                //tech.RemindScore = tech.RemindScore + td.Score - Convert.ToSingle(fields[9]);
                                td.Score = Convert.ToSingle(fields[9]);
                            }
                            _requestService.AddOrUpdateTechnologyDetail(td);
                            //_requestService.EditTechnology(tech);
                        }
                    }
                    if (arrNewTech.Length > 1)
                    {
                        for (int i = 0; i < arrNewTech.Length - 1; i++)
                        {
                            fields = arrNewTech[i].Split(',');
                            lngTechId = Convert.ToInt64(fields[0]);
                            //Technology tech = _requestService.FindTechnology(lngTechId);
                            if ((Convert.ToSingle(fields[9]) <= 0))// || (tech == null))
                            {
                                continue;
                            }
                            var td =
                                _requestService.WhereTechnologyDetail(
                                    t =>
                                        t.TechnologyId == lngTechId &&
                                        t.RequestId == entity.RequestId).FirstOrDefault();
                            if (td == null)
                            {
                                //tech.RemindScore = tech.RemindScore - Convert.ToSingle(fields[9]);
                                td = new TechnologyDetail()
                                {
                                    TechnologyId = lngTechId,
                                    RequestId = entity.RequestId,
                                    Score = Convert.ToSingle(fields[9])
                                };
                            }
                            else
                            {
                                //tech.RemindScore = tech.RemindScore + td.Score - Convert.ToSingle(fields[9]);
                                td.Score = Convert.ToSingle(fields[9]);
                            }
                            _requestService.AddOrUpdateTechnologyDetail(td);
                            //_requestService.EditTechnology(tech);
                        }
                    }
                    Log log = new Log()
                    {
                        UserId = int.Parse(User.Identity.GetUserId()),
                        Operation = Operations.کارتابل,
                        OperationDetail = OperationsDetail.بایگانی,
                        Description = "ثبت بایگانی موقت درخواست شماره " + newArchive.Request.Id
                    };
                    _archiveService.EditArchive(newArchive);
                    _logService.AddNewLog(log);
                    _unitOfWork.SaveChanges();
                    scope.Complete();
                    return Json(new { success = true, msg = "بایگانی موقت با موفقیت انجام شد!" }); //View("Index"); 
                }
                catch (Exception)
                {
                    return Json(new { isError = true, Msg = "بایگانی موقت با خطا مواجه شد!" });
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "RK")]
        public virtual ActionResult Archive(string oldTech, string newTech, Archive archive, long cartableId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    string[] arrOldTech = oldTech.Split('&');
                    string[] arrNewTech = newTech.Split('&');
                    string[] fields;
                    var entity =
                        _cartableService.Where(x => x.Id == cartableId).Include(x => x.Request).FirstOrDefault();
                    if (entity == null)
                        return HttpNotFound();
                    entity.Active = false;
                    _cartableService.EditCartable(entity);

                    var newCartable = new Cartable
                    {
                        RequestId = entity.RequestId,
                        UserSenderId = entity.UserReciveId,
                        UserReciveId = entity.UserReciveId,
                        Active = false,
                        StageId = entity.StageId,
                        CurrentCartable = CurrentCartable.تمام_شده,
                        Description = "بایگانی "
                    };
                    Archive newArchive = _archiveService.Where(a => a.Request.Id == entity.RequestId).FirstOrDefault();
                    if (newArchive == null)
                    {
                        newArchive = new Archive
                        {
                            Request = entity.Request,
                            UserId = entity.Request.UserId,
                            ChiefCommiteId = entity.UserReciveId,
                            ChiefId = entity.UserSenderId,
                            Description = archive.Description,
                            EducationScore = archive.EducationScore,
                            ResearchScore = archive.ResearchScore,
                            ExecutiveScore = archive.ExecutiveScore,
                            TechnologyScore = archive.TechnologyScore,
                            DissertationScore = archive.DissertationScore,
                            SumScore = archive.SumScore,
                            IsOpinionCommite = archive.IsOpinionCommite,
                            Grade = archive.Grade
                        };
                    }
                    else
                    {
                        newArchive.Description = archive.Description;
                        newArchive.EducationScore = archive.EducationScore;
                        newArchive.ResearchScore = archive.ResearchScore;
                        newArchive.ExecutiveScore = archive.ExecutiveScore;
                        newArchive.TechnologyScore = archive.TechnologyScore;
                        newArchive.DissertationScore = archive.DissertationScore;
                        newArchive.SumScore = archive.SumScore;
                        newArchive.IsOpinionCommite = archive.IsOpinionCommite;
                        newArchive.Grade = archive.Grade;
                    }
                    //var listTechnologyDetailInRequest =
                    //    _requestService.WhereTechnologyDetail(r => r.RequestId == entity.RequestId).ToList();
                    //foreach (var item in listTechnologyDetailInRequest)
                    //{
                    //    var technology = _requestService.FindTechnology(item.TechnologyId);
                    //    if (technology != null)
                    //    {
                    //        technology.RemindScore += item.Score;
                    //        _requestService.EditTechnology(technology);
                    //    }
                    //    _requestService.DeleteTechnologyDetail(item.TechnologyId, item.RequestId);
                    //}

                    _requestService.DeleteListofTechnologyDetail(entity.RequestId);

                    _unitOfWork.SaveChanges();
                    long lngTechId;
                    if (arrOldTech.Length > 1)
                    {
                        for (int i = 0; i < arrOldTech.Length - 1; i++)
                        {
                            fields = arrOldTech[i].Split(',');
                            lngTechId = Convert.ToInt64(fields[0]);
                            Technology tech = _requestService.FindTechnology(lngTechId);
                            if ((Convert.ToSingle(fields[9]) <= 0) || (tech == null))
                            {
                                continue;
                            }
                            var td =
                                _requestService.WhereTechnologyDetail(
                                    t =>
                                        t.TechnologyId == lngTechId &&
                                        t.RequestId == entity.RequestId).FirstOrDefault();
                            if (td == null)
                            {
                                tech.RemindScore = tech.RemindScore - Convert.ToSingle(fields[9]);
                                td = new TechnologyDetail()
                                {
                                    TechnologyId = lngTechId,
                                    RequestId = entity.RequestId,
                                    Score = Convert.ToSingle(fields[9])
                                };
                            }
                            else
                            {
                                tech.RemindScore = tech.RemindScore + td.Score - Convert.ToSingle(fields[9]);
                                td.Score = Convert.ToSingle(fields[9]);
                            }
                            _requestService.AddOrUpdateTechnologyDetail(td);
                            _requestService.EditTechnology(tech);
                        }
                    }
                    if (arrNewTech.Length > 1)
                    {
                        for (int i = 0; i < arrNewTech.Length - 1; i++)
                        {
                            fields = arrNewTech[i].Split(',');
                            lngTechId = Convert.ToInt64(fields[0]);
                            Technology tech = _requestService.FindTechnology(lngTechId);
                            if ((Convert.ToSingle(fields[9]) <= 0) || (tech == null))
                            {
                                continue;
                            }
                            var td =
                                _requestService.WhereTechnologyDetail(
                                    t =>
                                        t.TechnologyId == lngTechId &&
                                        t.RequestId == entity.RequestId).FirstOrDefault();
                            if (td == null)
                            {
                                tech.RemindScore = tech.RemindScore - Convert.ToSingle(fields[9]);
                                td = new TechnologyDetail()
                                {
                                    TechnologyId = lngTechId,
                                    RequestId = entity.RequestId,
                                    Score = Convert.ToSingle(fields[9])
                                };
                            }
                            else
                            {
                                tech.RemindScore = tech.RemindScore + td.Score - Convert.ToSingle(fields[9]);
                                td.Score = Convert.ToSingle(fields[9]);
                            }
                            _requestService.AddOrUpdateTechnologyDetail(td);
                            _requestService.EditTechnology(tech);
                        }
                    }
                    Log log = new Log()
                    {
                        UserId = int.Parse(User.Identity.GetUserId()),
                        Operation = Operations.کارتابل,
                        OperationDetail = OperationsDetail.بایگانی,
                        Description = "بایگانی درخواست شماره " + newArchive.Request.Id
                    };
                    _cartableService.AddNewCartable(newCartable);
                    _archiveService.EditArchive(newArchive);
                    _logService.AddNewLog(log);
                    _unitOfWork.SaveChanges();
                    scope.Complete();
                    return Json(new { success = true, msg = "بایگانی با موفقیت انجام شد!" }); //View("Index"); 
                }
                catch (Exception)
                {
                    return Json(new { isError = true, Msg = "بایگانی با خطا مواجه شد!" });
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }

        [HttpPost]
        public virtual ActionResult Return(long cartableId, string desc)
        {
            try
            {
                int uId = int.Parse(User.Identity.GetUserId());
                var entity = _cartableService.Where(c => c.Id == cartableId).Include(i => i.Request).Include(i => i.Request.User).FirstOrDefault();
                if (entity == null)
                    return Json(new { isError = true, Msg = "خطا در انجام عملیات برگشت" });
                //if (entity.Request.UserId == uId)
                //    return Json(new { isError = true, Msg = "شما نمی توانید درخواست مربوط به خودتان را برگشت بزنید!" });
                entity.Active = false;
                _cartableService.EditCartable(entity);
                var firstStage =
                    _stageService.GetAllStagesAsQueryable().OrderBy(m => m.StageNumber).Select(s => s.StageNumber).FirstOrDefault();
                var stageId = _stageService.Where(s => s.StageNumber == firstStage).Select(s => s.Id).FirstOrDefault();
                var newCartable = new Cartable
                {
                    RequestId = entity.RequestId,
                    UserSenderId = entity.UserReciveId,
                    UserReciveId = entity.Request.UserId,
                    Active = true,
                    CurrentCartable = CurrentCartable.برگشت,
                    StageId = stageId,
                    Description = desc
                };
                _cartableService.AddNewCartable(newCartable);
                Log log = new Log()
                {
                    UserId = int.Parse(User.Identity.GetUserId()),
                    Operation = Operations.کارتابل,
                    OperationDetail = OperationsDetail.ارسال,
                    Description = "برگشت زدن درخواست شماره " + newCartable.RequestId
                };
                _logService.AddNewLog(log);

                var def = _definitionService.GetAllDefinitionsAsQueryable().FirstOrDefault();
                string University = ((def == null) || (def.UniversityName.Trim().Length == 0)) ? "دانشگاه آزاد" : def.UniversityName;
                _unitOfWork.SaveChanges();
                try
                {
                    SendMail(entity.Request.User.Email, University, "برگشت درخواست ترفیع", "درخواست شماره " + entity.RequestId + " شما برگشت خورده است. لطفا جهت اصلاح درخواست و ارسال مجدد اقدام فرمایید!");
                }
                catch (Exception ex)
                {
                    return Json(new { isError = true, Msg = "ارسال ایمیل با خطا مواجه شد" });
                }
                if (SendMessage(entity.Request.UserId, uId, "درخواست ترفیع شما برگشت خورده است لطفا جهت رفع اشکالات و ارسال مجدد اقدام نمایید!") == 0)
                {
                    return Json(new { isError = true, Msg = "ارسال پیام با خطا مواجه شد" });
                }
                return Json(new { uidRecive = newCartable.UserReciveId, uidSender = newCartable.UserSenderId, isError = false, Msg = "ارسال با موفقیت انجام شد!" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, Msg = "خطا در انجام عملیات برگشت" + ex.Message });
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
                    {"Id", item.Id},
                    {"EducationalResearchStatus", (item.EducationalResearchStatus)},
                    {"Term", item.Term},
                    {"TermName", ((Term) item.Term).ToString().Replace("_", " ")},
                    {"Subject", item.Subject},
                    {"CourseNo", item.CourseNo},
                    {"UnitCount", item.UnitCount},
                    {"GradeEducation", (int)item.GradeEducation},
                    {"GradeEducationTitle", ((GradeEducation)item.GradeEducation).ToString().Replace("_"," ")},
                    {"StudentCount", item.StudentCount},
                    {"EvaluationFormCount", item.EvaluationFormCount},
                    {"QualityTeaching", item.QualityTeaching},
                    {"UnitEqual", item.UnitEqual},
                };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _requestService.GetEducation_ByRequestId_ForCurrentYear(requestid).Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات آموزشی" });
            }
        }

        public virtual ActionResult Educational(long requestId, string roleName)
        {
            //var req = (from p in _requestService.GetAllRequests().Where(x=>x.Id==requestId))
            var request =
                _requestService.GetAllRequestsAsQueryable()
                    .Where(x => x.Id == requestId)
                    .Include(x => x.User)
                    .Include(x => x.User.Profile)
                    .Include(x => x.Cartables)
                    .FirstOrDefault();
            ViewBag.roleName = roleName;
            //_roleManager.GetAllCustomRolesAsQueryable()
            //    .Where(r => r.Id == stageId)
            //    .Select(s => s.Name)
            //    .FirstOrDefault();

            return View(request);
        }

        public virtual ActionResult FurtherInformation(long requestId)
        {
            var furth = _requestService.Where(x => x.RequestId == requestId)
                .Include(x => x.Request)
                .Include(x => x.AttachmentFurtherInformations)
                .Include(x => x.Request.User)
                .Include(x => x.Request.User.Profile).FirstOrDefault();
            return View(furth);
        }

        public virtual ActionResult OpenAttachsFromFurtherInformation(long furtherInformationId, int furtherInformationType)
        {
            var model =
                _requestService.GetAttachmentFurtherInformations(furtherInformationId, furtherInformationType)
                .Include(x => x.FurtherInformation)
                .Include(x => x.FurtherInformation.Request)
                .Include(x => x.FurtherInformation.Request.User).ToList();
            return PartialView("_AttachFurtherInformations", model);
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
                    {"Id", item.Id},
                    {"EducationalResearchStatus", (item.EducationalResearchStatus)},
                    {"Term", item.Term},
                    {"TermName", ((Term) item.Term).ToString().Replace("_", " ")},
                    {"Subject", item.Subject},
                    {"ResearchPost",(int)item.ResearchPost},
                    {"ResearchPostTitle",((PostPlan)item.ResearchPost).ToString()},
                    {"BeginDate", item.BeginDate},
                    {"EndDate", item.EndDate},
                    {"StudentCount", item.StudentCount},
                    {"EvaluationFormCount", item.EvaluationFormCount},
                    {"QualityTeaching", item.QualityTeaching},
                    {"UnitEqual", item.UnitEqual},
                };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _requestService.GetReasearch_ByRequestId_ForCurrentYear(requestid).Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات پژوهشی" });
            }
        }

        public virtual ActionResult Research(long requestId, string roleName)
        {
            //var req = (from p in _requestService.GetAllRequests().Where(x=>x.Id==requestId))
            var request =
                _requestService.GetAllRequestsAsQueryable()
                    .Where(x => x.Id == requestId)
                    .Include(x => x.User)
                    .Include(x => x.User.Profile)
                    .Include(x => x.Cartables)
                    .FirstOrDefault();
            ViewBag.roleName = roleName;
            //_roleManager.GetAllCustomRolesAsQueryable()
            //    .Where(r => r.Id == stageId)
            //    .Select(s => s.Name)
            //    .FirstOrDefault();

            return View(request);
        }

        public virtual ActionResult GetDissertation(long requestid)
        {
            try
            {
                var data = _requestService.GetDissertation_ByRequestId_ForCurrentYear(requestid).ToList();
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    var itemObject = new JObject
                {
                    {"Id", item.Id},
                    {"Subject", item.Subject},
                    {"StudentName", item.StudentName},
                    {"GradeEducation",(int)item.GradeEducation},
                    {"GradeEducationTitle", ((GradeEducation) item.GradeEducation).ToString().Replace("_", " ")},
                    {"BeginDate", item.BeginDate},
                    {"VindicationDate", item.VindicationDate},
                    {"UnitCount", item.UnitCount},
                    {"UnitEqual", item.UnitEqual},
                };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _requestService.GetDissertation_ByRequestId_ForCurrentYear(requestid).Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات پروژه ، پایان نامه" });
            }
        }
        public virtual ActionResult Dissertation(long requestId, string roleName)
        {
            //var req = (from p in _requestService.GetAllRequests().Where(x=>x.Id==requestId))
            var request =
                _requestService.GetAllRequestsAsQueryable()
                    .Where(x => x.Id == requestId)
                    .Include(x => x.User)
                    .Include(x => x.User.Profile)
                    .Include(x => x.Cartables)
                    .FirstOrDefault();
            ViewBag.roleName = roleName;
            //_roleManager.GetAllCustomRolesAsQueryable()
            //    .Where(r => r.Id == stageId)
            //    .Select(s => s.Name)
            //    .FirstOrDefault();

            return View(request);
        }
        public virtual ActionResult GetTechnology(long requestid)
        {
            try
            {
                var data = _requestService.GetTechnology_ByRequestId_ForCurrentYear(requestid).ToList();
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    var itemObject = new JObject
                {
                    {"Id", item.Id},
                    {"TechnologyType",(int)item.TechnologyType},
                    {"TechnologyTypeTitle", ((TechnologyType) item.TechnologyType).ToString().Replace("_", " ")},
                    {"Subject", item.Subject},
                    {"PresentationDate", item.PresentationDate},
                    {"PlacePresentation", item.PlacePresentation},
                    {"PartnersNames", item.PartnersNames},
                    {"Score", item.Score},
                };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _requestService.GetTechnology_ByRequestId_ForCurrentYear(requestid).Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات فناوری" });
            }
        }
        public virtual ActionResult Technology(long requestId, string roleName)
        {
            //var req = (from p in _requestService.GetAllRequests().Where(x=>x.Id==requestId))
            var request =
                _requestService.GetAllRequestsAsQueryable()
                    .Where(x => x.Id == requestId)
                    .Include(x => x.User)
                    .Include(x => x.User.Profile)
                    .Include(x => x.Cartables)
                    .FirstOrDefault();
            ViewBag.roleName = roleName;
            //_roleManager.GetAllCustomRolesAsQueryable()
            //    .Where(r => r.Id == stageId)
            //    .Select(s => s.Name)
            //    .FirstOrDefault();
            return View(request);
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
                    {"Id", item.Id},
                    {"Subject", item.Subject},
                    {"StartDate", item.StartDate},
                    {"EndDate", item.EndDate},
                    {"TimeofMounth", item.TimeofMounth},
                    {"Score", item.Score}
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
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات علمی اجرایی" });
            }
        }
        public virtual ActionResult ScientificExecutive(long requestId, string roleName)
        {
            //var req = (from p in _requestService.GetAllRequests().Where(x=>x.Id==requestId))
            var request =
                _requestService.GetAllRequestsAsQueryable()
                    .Where(x => x.Id == requestId)
                    .Include(x => x.User)
                    .Include(x => x.User.Profile)
                    .Include(x => x.Cartables)
                    .FirstOrDefault();
            ViewBag.roleName = roleName;
            //_roleManager.GetAllCustomRolesAsQueryable()
            //    .Where(r => r.Id == stageId)
            //    .Select(s => s.Name)
            //    .FirstOrDefault();

            return View(request);
        }
        public virtual ActionResult TrackingRequest()
        {
            TrackingRequestViewModel model = new TrackingRequestViewModel();
            PersianCalendar d = new PersianCalendar();
            int term = d.GetYear(DateTime.Now);
            int uid = int.Parse(User.Identity.GetUserId());

            var activitysCartable = _cartableService.Where(x => x.Request.UserId == uid)
                .Include(i => i.Request)
                .Include(i => i.Request.User)
                .Include(i => i.Request.User.Profile)
                .Include(i => i.UserRecive)
                .Include(i => i.UserRecive.Profile)
                .Include(i => i.UserSender)
                .Include(i => i.UserSender.Profile)
                .Include(i => i.Stage)
                .Include(i => i.Stage.Role)
                .OrderByDescending(o => o.CreatedDate).ToList();

            model.Trackings = activitysCartable;
            model.RequestsId = activitysCartable.Select(x => x.RequestId).Distinct();

            return View(model);
        }
        public virtual ActionResult GetTrakingsByRequestId(long reqId)
        {
            var requestsModel = _cartableService
                .Where(c => c.RequestId == reqId)
                .Include(i => i.Request)
                .Include(i => i.Request.User)
                .Include(i => i.Request.User.Profile)
                .Include(i => i.UserRecive)
                .Include(i => i.UserRecive.Profile)
                .Include(i => i.UserSender)
                .Include(i => i.UserSender.Profile)
                .Include(i => i.Stage)
                .Include(i => i.Stage.Role).
                OrderBy(o => o.CreatedDate).ToList();

            return PartialView("_GetTrakings", requestsModel);
        }
        public virtual ActionResult MyArchive()
        {
            return View();
        }
        public virtual ActionResult GetArchives()
        {
            try
            {
                int uId = int.Parse(User.Identity.GetUserId());
                var data = _archiveService.Where(a => a.UserId == uId).Include(i => i.Request).OrderByDescending(o => o.CreatedDate).ToList();
                JArray ja = new JArray();

                foreach (var item in data)
                {
                    var itemObject = new JObject
                {
                    {"Id", item.Id},
                    {"CreatedDate", item.CreatedDate.ToPeString()},
                    {"RequestId", item.Request.Id},
                    {"RequestDate", item.Request.CreatedDate.ToPeString()},
                    {"IsOpinionCommite", item.IsOpinionCommite},
                    {"EducationScore",item.EducationScore },
                    {"ResearchScore" ,item.ResearchScore },
                    {"ExecutiveScore" ,item.ExecutiveScore },
                    {"TechnologyScore",item.TechnologyScore},
                    {"DissertationScore",item.DissertationScore },
                     {"SumScore", item.SumScore},
                     {"Grade" ,item.Grade },
                     {"Description" ,item.Description ?? "" }
                };
                    ja.Add(itemObject);
                }

                JObject jo = new JObject();
                jo.Add("total", _archiveService.Where(a => a.UserId == uId).Include(i => i.Request).Count());
                jo.Add("rows", ja);
                return Content(JsonConvert.SerializeObject(jo), "application/json");
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در لود اطلاعات بایگانی" });
            }
        }
        public virtual ActionResult UpdateEducationReasearch(EducationalResearch educationalResearch)
        {
            try
            {
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
                //string er;
                //foreach (ModelState modelState in ViewData.ModelState.Values)
                //{
                //    foreach (ModelError error in modelState.Errors)
                //    {
                //        er = error.ErrorMessage;
                //    }
                //}
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
                technology.RemindScore = technology.Score;
                _requestService.EditTechnology(technology);
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "تغییرات اعمال شد" });
            }
            catch (Exception)
            {
                return Json(new { isError = true, Msg = "خطا در ویرایش فعالیت فناوری" });
            }
        }

        public void SendMail(string _To, string _Subject, string _Title, string _Body)
        {
            try
            {
                dynamic email = new Email("mail.Html");
                email.To = _To;
                email.Subject = _Subject;
                email.Title = _Title;
                email.Body = _Body;
                if (email.To != string.Empty)
                    email.Send();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        /// <summary>
        /// برگشتی یک یعنی ارسال و خروجی صفر یعنی عدم ارسال پیام 
        /// </summary>
        public int SendMessage(int userSender, int userReciever, string text)
        {
            try
            {
                int currentUser = int.Parse(User.Identity.GetUserId());
                if (userSender <= 0)
                {
                    new Exception("کاربر فرستنده نا معتبر می باشد!");
                }
                if (userReciever <= 0)
                {
                    new Exception("کاربر گیرنده نا معتبر می باشد!");
                }
                if (userSender == userReciever)
                {
                    new Exception("کاربر فرستنده و گیرنده برابر می باشند!");
                }
                Messaging msg = new Messaging()
                {
                    Text = text,
                    UserSenderId = userSender,
                    UserRecieverId = userReciever,
                    Readed = false
                };
                _messagingService.AddNewMessaging(msg);
                Log log = new Log()
                {
                    UserId = currentUser,
                    Operation = Operations.پیام,
                    OperationDetail = OperationsDetail.ارسال,
                    Description = text,
                    Messaging = msg
                };
                _logService.AddNewLog(log);
                _unitOfWork.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public string CartableCount()
        {
            var uid = int.Parse(User.Identity.GetUserId());
            var count = _cartableService.GetAllCartablesAsQueryable().Count(x => x.UserReciveId == uid && x.Active == true).ToString();
            return count;
        }
    }


    public class OutSend
    {
        public int UidRecive { get; set; }
        public int UidSender { get; set; }
        public bool IsError { get; set; }
        public string Msg { get; set; }

        public OutSend(bool _isError, string _msg, int _uidrecive, int _uidSender)
        {
            IsError = _isError;
            Msg = _msg;
            UidRecive = _uidrecive;
            UidSender = _uidSender;
        }
    }
}