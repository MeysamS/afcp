using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Annual_faculty_promotions.Service.Contracts;
using Postal;

namespace Annual_faculty_promotions.Service.Implemention
{
    public class EfEmailService : IEmailIdentityService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Credentials:
            const string credentialUserName = "info@sotreza.ir";
            const string sentFrom = "info@sotreza.ir";
            const string pwd = "3dZxd98";

            // Configure the client:
            var client =
                new System.Net.Mail.SmtpClient("mail.sotreza.ir")
                {
                    Port = 25,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

            // Creatte the credentials:
            var credentials =
                new System.Net.NetworkCredential(credentialUserName, pwd);

            client.EnableSsl = false;
            client.Credentials = credentials;

            // Create the message:
            var mail =
                new System.Net.Mail.MailMessage(sentFrom, message.Destination)
                {
                    Subject = message.Subject,
                    Body = message.Body
                };

            // Send:
            return  client.SendMailAsync(mail);
        }

        public void SendAsync(string ToName,string ToEmail,string Subject,string Description)
        {
            //میشه به جای خوراک از تکست باکس ها متن های بهتری ارسال کنید
            //ایجاد یک بدنه با قالب برای ایمیل
            //میتونید به اچ تی ام ال درست کنید  
            //این بدنه داخل ایمیل ادمین میاد
            string strBody = string.Empty;

            strBody += string.Format("<b>Full Name</b>: {0}<br />", ToName);
            strBody += string.Format("<b>E-Mail</b>: <a href='mailto:{0}'>{0}</a><br />", ToEmail);
            strBody += string.Format("<b>Subject</b>: {0}<br />", Subject);
            strBody += string.Format("<b>Description</b>: {0}<br />", Description.Replace("\n", "<br />"));

            //در 5 جا که 3 تای اون پراپرتی میل مسیج هستند
            //فرام -سندر - ریپلای تو -فرام وب کانفیگ و اکانت وب کانفیگ
            //اگر یکسان باشد احتمال اینکه ایمیل شما
            //وارد اسپام نشود زیادتره

            //ایجاد یک شی از میل مسیج
            System.Net.Mail.MailMessage oMailMessage =
                new System.Net.Mail.MailMessage();

            //فرام - سندر - میل آدرس 
            //هر سه پراپرتی میل مسیج از جنس میل آدرس هستند 
            //پس یک شی از میل آدرس ایجاد میکنیم
            System.Net.Mail.MailAddress oMailAddress = null;

            // کاملترین شکل ایجاد میل آدرس میل آدرس سه قسمت دارد 
            //ایمیل
            //دیسپلی نیم
            //یونی کد
            oMailAddress =
                new System.Net.Mail.MailAddress
                    (
                        "ali.kalij@gmail.com",
                        "Auto Response Email Sender",
                        System.Text.Encoding.UTF8
                    );

            //برای فرام - میل آدرس و سندر 
            //معمولا یک آدرس را انتخاب میکنیم
            oMailMessage.From = oMailAddress;
            oMailMessage.Sender = oMailAddress;



            //آیتم های ذیل از جنس میل آدرس کالکشن هست
            //چون گیرنده یکی هست گیرندگان  بیشتر از یکی هستند
            //چهار کالکشن ذیل بهتر است ابتدا کلیر شوند
            //تو - سی سی - بی سی سی -ریپلای تو
            //باید به میل آدرس اد شوند
            oMailMessage.To.Clear();
            oMailMessage.CC.Clear();
            oMailMessage.Bcc.Clear();
            oMailMessage.ReplyToList.Clear();
            oMailMessage.Attachments.Clear();



            oMailMessage.ReplyToList.Add(oMailAddress);

            //میتونیم شخص یا اشخاص دیگری را به غیر از خودمون مشخص کنیم 
            //که ایمیل را دریافت کنند
            //مثل خود شخصی که متن را ارسال نموده است
            oMailAddress =
                new System.Net.Mail.MailAddress
                    (
                        ToEmail,
                        ToName,
                        System.Text.Encoding.UTF8
                    );

            //ما با تو این کار را انجام دادیم
            oMailMessage.To.Add(oMailAddress);

            //بادی را این کدینگ میکنیم
            oMailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            oMailMessage.Body = strBody;

            //سابجکت را یونی کد میکنیم
            oMailMessage.SubjectEncoding = System.Text.Encoding.UTF8;


            //یه پیشوند یا امضا قبل از سابجکت قرار میدهیم تا فیلترینگ
            //ایمیل ها ساده شوند
            oMailMessage.Subject =
                "[-<Company Name>-] - " + Subject;

            //اگر تورو باشد و متن حاوی کد اچ تی ام ال باشد 
            //کد ترجمه شده و عمل کرده میبینید
            oMailMessage.IsBodyHtml = true;

            //اولویت ایمیل را مشخص میکنیم
            oMailMessage.Priority =
                System.Net.Mail.MailPriority.Normal;

            //نحوه پیام دادن سیستم در مورد ایمیل ارسالی را مشخص میکنیم
            //پنج حالت دارد که اگر ترکیب حالات مد نظر باشد باید دستو ذیل به تعداد نوشته و با هم اور شوند 
            oMailMessage.DeliveryNotificationOptions =
                System.Net.Mail.DeliveryNotificationOptions.Never;


            //با استفاده از تبدیل مسیر نسبی به فیزیکی 
            //پیوستی برای ایمیل در نظر میگیریم

            //مسیر فیزیکی نسبت به روت
            //string strRootRelativePathName = "~/Attachments/Attachment.png";

            //تبدیل مسیر نسبی به فیزیکی
            //string strPathName =
            //    Server.MapPath(strRootRelativePathName);

            //if (System.IO.File.Exists(strPathName))
            //{
            //    System.Net.Mail.Attachment oAttachment =
            //        new System.Net.Mail.Attachment(strPathName);

            //    oMailMessage.Attachments.Add(oAttachment);
            //}

            //در محیط میل سرور های عمومی وقتی ایمیل ارسال میشود پروتکل اچ تی تی پی است
            //اما در محیط خارجی
            //اس ام تی پی برای ارسال و پاپ تری برای دریافت ایمیل است

            //یک شی از اس ام تی پی ایجاد میکنیم
            System.Net.Mail.SmtpClient oSmtpClient =
                new System.Net.Mail.SmtpClient();

            //تایم اوت پیش فرض یعنی صد ثانیه را برای ارسال ایمیل در نظر میگیریم
            //زمان برقراری ارتباط است اگر بیشتر شود پیغام خطا داده میشود
            oSmtpClient.Timeout = 100000;

            //امضا الکترونیکی ایمیل را مشخص میکنیم
            //سکیوت ساکت لایر -انتقال امن دیتا در جی میل که اولین ارائه کننده این سرویس است رایگان است
            //اگر جی میل در نظر میگیرید حتما این گزینه را تورو قرار دهید
            oSmtpClient.EnableSsl = false;

            //شی میل مسیج را به متد سند شی اس ام تی پی میدیم
            oSmtpClient.Send(oMailMessage);
        }

        public void SendEmailWithPostal(string view,string to,string cc,string bcc,string title,string subject,string body)
        {
            dynamic email=new Email(view);
            email.To = to;
            email.CC = cc;
            email.BCC = bcc;
            email.Title = title;
            email.Subject = subject;
            email.Body = body;
            if (!string.IsNullOrWhiteSpace(email.To.ToString()))
                email.Send();
        }
    }
}