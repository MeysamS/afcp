using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.Service.DNT_Sheduler;
using Annual_faculty_promotions.Service.Implemention;
using Annual_faculty_promotions.WebUI.Helpers.Util;
using Annual_faculty_promotions.WebUI.Ioc;
using Postal;

namespace Annual_faculty_promotions.WebUI.Shedule
{
    public class SendEmailTask : ScheduledTaskTemplate
    {

        public override string Name
        {
            get { return "ارسال ایمیل"; }
        }

        public override int Order
        {
            get { return 1; }
        }

        public override bool RunAt(DateTime utcNow)
        {
            if (this.IsShuttingDown || this.Pause)
                return false;
            var now = utcNow.AddHours(3.5);
            var definition = SmObjectFactory.Container.GetInstance<IDefinitionService>();
            var def = definition.GetAllDefinitionsAsQueryable().FirstOrDefault();
            //int RepeatSend = (def == null) || (def.RepeatMail == null) ? 1 : def.RepeatMail;
            //if ((def != null) && (!string.IsNullOrWhiteSpace(def.RepeatMail.ToString())))
            //{
            //    RepeatSend = int.Parse(def.RepeatMail);
            //}
            //return true;
            if (def == null || string.IsNullOrWhiteSpace(def.RepeatMail.ToString()))
                return false;
            return (now.Day%def.RepeatMail ==0) && (now.Hour == 9 && now.Minute == 0 && now.Second == 0);
        }

        public override void Run()
        {
            if (this.IsShuttingDown || this.Pause)
                return;

            var userService = SmObjectFactory.Container.GetInstance<IUserService>();
            var def = SmObjectFactory.Container.GetInstance<IDefinitionService>();
            var definitions = def.GetAllDefinitionsAsQueryable().FirstOrDefault();
            if (definitions == null)
                return;
            var nowDate = DateTime.Now;
            PersianCalendar calendar = new PersianCalendar();

            try
            {
                var query = (from p in userService.GetAllUsersAsQueryable()
                             select new
                             {
                                 User = p,
                                 User_Profile = p.Profile,
                                 Request = p.Requests,
                                 Archive = p.Requests.Select(c => c.Archive).ToList()
                             });
                dynamic email = new Email("Reg.Html");
                email.To = "";
                var userByArchives = query.AsEnumerable().Select(x => x.User).Where(x => x.Archives != null).ToList();
                for (int i = 0; i < userByArchives.Count; i++)
                {
                    var archiveDate = userByArchives[i].Archives.Last().CreatedDate;
                    DateTime dt1 = calendar.ToDateTime(nowDate.Year, nowDate.Month, nowDate.Day, 0, 0, 0, 0);
                    DateTime dt2 = calendar.ToDateTime(archiveDate.Year, archiveDate.Month, archiveDate.Day, 0, 0, 0, 0);
                    TimeSpan ts = dt1.Subtract(dt2);
                    int days = ts.Days;
                    if (days > definitions.StartMail)
                    {
                        if (string.IsNullOrWhiteSpace(email.To.ToString()))
                        {
                            email.To = userByArchives[i].Email;
                        }
                        else
                        {
                            email.BCC = userByArchives[i].Email;
                        }
                    }
                }

                var userByEmployees = query.AsEnumerable().Select(x => x.User).Where(x => x.Archives == null).ToList();

                for (int i = 0; i < userByEmployees.Count; i++)
                {
                    var employeeDate = userByEmployees[i].Profile.EmployeeDate;
                    DateTime dt1 = calendar.ToDateTime(nowDate.Year, nowDate.Month, nowDate.Day, 0, 0, 0, 0);
                    DateTime dt2 = calendar.ToDateTime(employeeDate.Year, employeeDate.Month, employeeDate.Day, 0, 0, 0, 0);
                    TimeSpan ts = dt1.Subtract(dt2);
                    int days = ts.Days;
                    if (days > definitions.StartMail)
                    {
                        if (string.IsNullOrWhiteSpace(email.To.ToString()))
                        {
                            email.To = userByEmployees[i].Email;
                        }
                        else
                        {
                            email.BCC = userByEmployees[i].Email;
                        }
                    }
                }
                // string body = "با عرض سلام خدمت شما استاد گرامی ، به استحضار می رساند که در صورت تمایل افزایش ترفیع میتوانید نسبت به ارسال درخواست در سامانه اقدام فرمائید  با تشکر - واحد آموزشی دانشگاه آزاد واحد آیت الله آملی ";
                email.Subject = "مهلت ارسال درخواست ترفیع اعضای هیئت علمی";
                email.Title = "اطلاعیه مهم";
                email.Body = definitions.TextMailForDourtionRequest;
                if (!string.IsNullOrWhiteSpace(email.To.ToString()))
                    email.SendAsync();
            }
            catch (Exception ex)

            {

                throw;
            }

        }
    }
}
