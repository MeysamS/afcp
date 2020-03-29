using System;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Annual_faculty_promotions.Core.Enums;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Reporting.WebForms;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Mvc;
using System.Web.Routing;
using Annual_faculty_promotions.WebUI.Helpers.Util;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{
    public partial class ReportController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly ICartableService _carableService;
        private readonly IDefinitionService _definitionService;
        private readonly IUserService _userService;
        private readonly IApplicationRoleManager _roleService;
        private readonly IUnitOfWork _unitOfWork;

        public ReportController(IUnitOfWork unitOfWork,
            IRequestService requestService,
            ICartableService cartableService,
            IUserService userService,
            IApplicationRoleManager roleService,
            IDefinitionService definitionService)
        {
            _unitOfWork = unitOfWork;
            _requestService = requestService;
            _carableService = cartableService;
            _definitionService = definitionService;
            _userService = userService;
            _roleService = roleService;
        }

        public virtual ActionResult Index(long cartableId)
        {
            TempData["cartableId"] = cartableId;
            return View("Rep");
        }

        public virtual ActionResult GetReportSnapShot()
        {

            long cartableId = 0;
            if (TempData["cartableId"] != null)
            {
                cartableId = long.Parse(TempData["cartableId"].ToString());
            }
            try
            {

                int userId = int.Parse(User.Identity.GetUserId());
                PersianCalendar d = new PersianCalendar();
                StiReport report = new StiReport();

                var cartable =
                    _carableService.Where(r => r.Id == cartableId &&
                                               r.Active == true && r.CurrentCartable != CurrentCartable.برگشت)
                        .Include(i => i.Request)
                        .Include(i => i.Request.User)
                        .Include(i => i.Request.User.Profile)
                        .Include(i => i.Request.UnivercityStructure)
                        .Include(i => i.Request.UnivercityStructure.Parent)
                        .FirstOrDefault();

                if ((cartable == null) || (cartable.RequestId == 0))
                {
                    return View("Error");
                }
                var lstEducationalResearch1 =
                    _requestService.WhereEducationalResearch(w => w.RequestId == cartable.RequestId).Take(15);
                var lstEducationalResearch2 =
                    _requestService.WhereEducationalResearch(w => w.RequestId == cartable.RequestId)
                        .Except(lstEducationalResearch1);
                var lstDissertation1 =
                    _requestService.GetDissertation_ByRequestId_ForCurrentYear(cartable.RequestId).Take(17);
                var lstDissertation2 =
                    _requestService.GetDissertation_ByRequestId_ForCurrentYear(cartable.RequestId)
                        .Except(lstDissertation1);
                var lstTechnology1 =
                    _requestService.GetTechnology_ByRequestId_ForCurrentYear(cartable.RequestId).Take(15);
                var lstTechnology2 =
                    _requestService.GetTechnology_ByRequestId_ForCurrentYear(cartable.RequestId).Except(lstTechnology1);
                var lstScientificExecutive1 =
                    _requestService.GetScientificExecutive_ByRequestId_ForCurrentYear(cartable.RequestId).Take(5);
                var lstScientificExecutive2 =
                    _requestService.GetScientificExecutive_ByRequestId_ForCurrentYear(cartable.RequestId)
                        .Except(lstScientificExecutive1);

                var definition = _definitionService.GetAllDefinitionsAsQueryable().SingleOrDefault();

                string Path = Server.MapPath("~/Reports/test.mrt");
                report.Load(Path);

                if (definition != null)
                {
                    var strUniversity = report.GetComponentByName("strUnivercity") as StiText;
                    if (strUniversity != null)
                        strUniversity.Text.Value = "کمیته ترفیعات " + definition.UniversityName;
                    var strUniversity2 = report.GetComponentByName("strUnivercity2") as StiText;
                    if (strUniversity2 != null)
                        strUniversity2.Text.Value = "کمیته ترفیعات " + definition.UniversityName;
                    var strLogoPath = report.GetComponentByName("strLogoPath") as StiImage;
                    if (strLogoPath != null)
                        strLogoPath.ImageURLValue = definition.Logo;
                    var strCommitteeName1 = report.GetComponentByName("strCommitteeName1") as StiText;
                    if (strCommitteeName1 != null)
                        strCommitteeName1.Text.Value = definition.CommitteeName1;
                    var strCommitteeName2 = report.GetComponentByName("strCommitteeName2") as StiText;
                    if (strCommitteeName2 != null)
                        strCommitteeName2.Text.Value = definition.CommitteeName2;
                    var strCommitteeName3 = report.GetComponentByName("strCommitteeName3") as StiText;
                    if (strCommitteeName3 != null)
                        strCommitteeName3.Text.Value = definition.CommitteeName3;
                    var strCommitteeName4 = report.GetComponentByName("strCommitteeName4") as StiText;
                    if (strCommitteeName4 != null)
                        strCommitteeName4.Text.Value = definition.CommitteeName4;
                    var strUniversPosition1 = report.GetComponentByName("strUniversPosition1") as StiText;
                    if (strUniversPosition1 != null)
                        strUniversPosition1.Text.Value = definition.UniversPosition1;
                    var strUniversPosition2 = report.GetComponentByName("strUniversPosition2") as StiText;
                    if (strUniversPosition2 != null)
                        strUniversPosition2.Text.Value = definition.UniversPosition2;
                    var strUniversPosition3 = report.GetComponentByName("strUniversPosition3") as StiText;
                    if (strUniversPosition3 != null)
                        strUniversPosition3.Text.Value = definition.UniversPosition3;
                    var strUniversPosition4 = report.GetComponentByName("strUniversPosition4") as StiText;
                    if (strUniversPosition4 != null)
                        strUniversPosition4.Text.Value = definition.UniversPosition4;
                    var strCommitteePosition1 = report.GetComponentByName("strCommitteePosition1") as StiText;
                    if (strCommitteePosition1 != null)
                        strCommitteePosition1.Text.Value = User.FirstName();
                    var strCommitteePosition2 = report.GetComponentByName("strCommitteePosition2") as StiText;
                    if (strCommitteePosition2 != null)
                        strCommitteePosition2.Text.Value = User.FirstName();
                    var strCommitteePosition3 = report.GetComponentByName("strCommitteePosition3") as StiText;
                    if (strCommitteePosition3 != null)
                        strCommitteePosition3.Text.Value = User.FirstName();
                    var strCommitteePosition4 = report.GetComponentByName("strCommitteePosition4") as StiText;
                    if (strCommitteePosition4 != null)
                        strCommitteePosition4.Text.Value = User.FirstName();
                }
                if ((cartable != null) && (cartable.Request != null))
                {
                    var strNameFamily = report.GetComponentByName("strNameFamily") as StiText;
                    if (strNameFamily != null)
                        strNameFamily.Text.Value = cartable.Request.User.Profile.Name + " " +
                                                   cartable.Request.User.Profile.Family;
                    var strNameFamily2 = report.GetComponentByName("strNameFamily2") as StiText;
                    if (strNameFamily2 != null)
                        strNameFamily2.Text.Value = cartable.Request.User.Profile.Name + " " +
                                                    cartable.Request.User.Profile.Family;
                    var strUnivercityStructure = report.GetComponentByName("strUnivercityStructure") as StiText;
                    if (strUnivercityStructure != null)
                        strUnivercityStructure.Text.Value = cartable.Request.UnivercityStructure.Parent.Name + "/" +
                                                            cartable.Request.UnivercityStructure.Name;
                    var strAcademicDegree = report.GetComponentByName("strAcademicDegree") as StiText;
                    if (strAcademicDegree != null)
                        strAcademicDegree.Text.Value =
                            ((AcademicDegree)cartable.Request.AcademicDegree).ToString().Replace("_", " ");
                    var strEmploymentStatus = report.GetComponentByName("strEmploymentStatus") as StiText;
                    if (strEmploymentStatus != null)
                        strEmploymentStatus.Text.Value =
                            ((EmploymentStatus)cartable.Request.EmploymentStatus).ToString().Replace("_", " ");
                    var strGrade = report.GetComponentByName("strGrade") as StiText;
                    if (strGrade != null)
                        strGrade.Text.Value = cartable.Request.Grade.ToString();
                    var strLastDateGrade = report.GetComponentByName("strLastDateGrade") as StiText;
                    if (strLastDateGrade != null)
                        strLastDateGrade.Text.Value = ((DateTime)cartable.Request.LastDateGrade).ToPeString();
                    var strPresenceInUnivercity = report.GetComponentByName("strPresenceInUnivercity") as StiText;
                    if (strPresenceInUnivercity != null)
                        strPresenceInUnivercity.Text.Value = cartable.Request.PresenceInUnivercity.ToString();
                    var strDate = report.GetComponentByName("strDate") as StiText;
                    if (strDate != null)
                        strDate.Text.Value = DateTime.Now.ToPeString();
                    var strDate2 = report.GetComponentByName("strDate2") as StiText;
                    if (strDate2 != null)
                        strDate2.Text.Value = DateTime.Now.ToPeString();
                    //d.ToDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0, 0)
                    //    .ToString();
                    var strDate3 = report.GetComponentByName("strDate3") as StiText;
                    if (strDate3 != null)
                        strDate3.Text.Value = DateTime.Now.ToPeString();
                    //d.ToDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0, 0)
                    //    .ToString();
                    var strDate4 = report.GetComponentByName("strDate4") as StiText;
                    if (strDate4 != null)
                        strDate4.Text.Value = DateTime.Now.ToPeString();
                    //d.ToDateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0, 0)
                    //    .ToString();
                    var strIsOpinionCommite = report.GetComponentByName("strIsOpinionCommite") as StiText;
                    var strIsNotOpinionCommite = report.GetComponentByName("strIsNotOpinionCommite") as StiText;
                    if ((strIsOpinionCommite != null) && (strIsNotOpinionCommite != null))
                    {
                        //if (request.IsOpinionCommite)
                        strIsOpinionCommite.Text.Value = "";
                    }
                    var UserRKId = _roleService.GetCustomUsersInRole("RK").Select(s => s.UserId).FirstOrDefault();
                    var UserRK = _userService.Where(w => w.Id == UserRKId).Include(i => i.Profile).FirstOrDefault();

                    var RoleMGId = _roleService.FindRoleByName("MG").Id;
                    var UserMGId =
                        _roleService.GetAllCustomUserRole()
                            .Where(
                                w => w.Department.Id == cartable.Request.UnivercityStructureId && w.RoleId == RoleMGId)
                            .Select(s => s.UserId).FirstOrDefault();
                    var UserMG = _userService.Where(w => w.Id == UserMGId).Include(i => i.Profile).FirstOrDefault();

                    var strModirGoroh = report.GetComponentByName("strModirGoroh") as StiText;
                    if ((strModirGoroh != null) && (UserMG != null))
                        strModirGoroh.Text.Value = UserMG.Profile.Name + " " + UserMG.Profile.Family;
                    var strBoosCmmittee = report.GetComponentByName("strBoosCmmittee") as StiText;
                    if ((strBoosCmmittee != null) && (UserRK != null))
                        strBoosCmmittee.Text.Value = UserRK.Profile.Name + " " + UserRK.Profile.Family;
                }
                var strYear = report.GetComponentByName("strYear") as StiText;
                if (strYear != null)
                {
                    int year = d.GetYear(DateTime.Now);
                    if (d.GetMonth(DateTime.Now) < 7)
                        strYear.Text.Value = (year - 1).ToString() + "-" + year.ToString();
                    else
                        strYear.Text.Value = year.ToString() + "-" + (year - 1).ToString();
                }
                report.Dictionary.Report.BusinessObjectsStore.Clear();

                report.RegBusinessObject("EducationalResearch1", lstEducationalResearch1.ToList());
                report.RegBusinessObject("EducationalResearch2", lstEducationalResearch2.ToList());
                report.RegBusinessObject("Dissertation1", lstDissertation1.ToList());
                report.RegBusinessObject("Dissertation2", lstDissertation2.ToList());
                report.RegBusinessObject("Technology1", lstTechnology1.ToList());
                report.RegBusinessObject("Technology2", lstTechnology2.ToList());
                report.RegBusinessObject("ScientificExecutive1", lstScientificExecutive1.ToList());
                report.RegBusinessObject("ScientificExecutive2", lstScientificExecutive2.ToList());
                report.Render(true);
                report.Dictionary.SynchronizeBusinessObjects(2);
                Stimulsoft.Report.StiConfig.LoadLocalization(Server.MapPath("~/Reports/fa.xml"));
                report.Show();
              
                return StiMvcViewerFx.GetReportSnapshotResult(report);
            }
            catch (Exception e)
            {
                return View("Error");
            }
        }

        public virtual ActionResult ViewExportReport()
        {
           return StiMvcViewerFx.ExportReportResult(HttpContext.Request);
          //  return StiMvcViewerFxHelper.ExportReportResult(HttpContext.Request);
        }


        public virtual ActionResult ViewReport()
        {
            return View("Rep");
        }
    }
}