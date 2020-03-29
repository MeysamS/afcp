using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Core.Enums;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.WebUI.Helpers;
using Annual_faculty_promotions.WebUI.Helpers.Filters;

namespace Annual_faculty_promotions.WebUI.Areas.UserArea.Controllers
{
    public partial class DefinitionsController : Controller
    {
        private readonly IDefinitionService _definitionService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService _logService;
        private readonly IMessagingService _messagingService;
        private readonly IRequestService _requestService;

        public DefinitionsController(IUnitOfWork unitOfWork,
            IDefinitionService definitionService, ILogService logService, IMessagingService messagingService,
            IRequestService requestService)
        {
            _unitOfWork = unitOfWork;
            _definitionService = definitionService;
            _logService = logService;
            _messagingService = messagingService;
            _requestService = requestService;
        }

        //[Expire]
        public virtual ActionResult Index()
        {
            var data = _definitionService.GetAllDefinitionsAsQueryable().FirstOrDefault();
            if (data == null)
            {
                data = new Definitions();
            }
            return View(data);
        }

        [HttpPost]
        public virtual ActionResult Edit(Definitions definition, HttpPostedFileBase fileLogo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { isError = false, Msg = "ورودی نامعتبر" });
                }
                if (Utility.CreateFolderIfNeeded(Server.MapPath("~/Content/Images/App")))
                {
                    if (fileLogo != null)
                    {
                        //string path = Path.Combine(Server.MapPath("~/Content/Images/App"), Path.GetFileName(fileLogo.FileName));
                        //fileLogo.SaveAs(path);
                        //definition.Logo = Path.GetFileName(fileLogo.FileName);

                        if (System.IO.File.Exists(Server.MapPath("~") + "Content\\Images\\App\\" + definition.Logo))
                            System.IO.File.Delete(Server.MapPath("~") + "Content\\Images\\App\\" + definition.Logo);

                        string temppath = Guid.NewGuid().ToString() + Path.GetExtension(fileLogo.FileName);
                        string physicalPath = Server.MapPath("~") + "Content\\Images\\App\\" + temppath;
                        fileLogo.InputStream.ResizeImageByWidth(50, physicalPath, Utility.ImageComperssion.Normal);
                        definition.Logo = temppath;
                    }
                }
                else
                {
                    new Exception("خطا در آدرس دهی مسیر ذخیره عکس");
                }
                UpgradeWebconfig(definition.SmtpFrom, definition.SmtpHost, definition.SmtpUserName, definition.SmtpPass, Convert.ToInt32(definition.SmtpPort));
                _definitionService.AddorUpdateDefinitions(definition);
                
                _unitOfWork.SaveChanges();
                return Json(new { isError = false, Msg = "عمل ثبت به درستی انجام شد" });
            }
            catch (Exception e)
            {
                return Json(new { isError = true, Msg = e.Message + " خطا در ثبت تنظیمات عمومی" });
            }
        }

        public void UpgradeWebconfig(string from, string host, string username, string pass, int port)
        {
            var myConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");

            System.Net.Configuration.MailSettingsSectionGroup mailSection = myConfig.GetSectionGroup("system.net/mailSettings") as System.Net.Configuration.MailSettingsSectionGroup;
            if (mailSection != null)
            {
                mailSection.Smtp.From = @from;
                mailSection.Smtp.Network.Host = host;
                mailSection.Smtp.Network.UserName = username;
                mailSection.Smtp.Network.Password = pass;
                mailSection.Smtp.Network.Port = port == 0 ? 1 : port;
            }
            myConfig.Save(ConfigurationSaveMode.Modified);

        }
    }
}