using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Annual_faculty_promotions.WebUI.Models
{
    public class Captcha
    {
        public class CaptchImageAction : ActionResult
        {
            public Color BackgroundColor { get; set; }
            public Color RandomTextColor { get; set; }
            public string RandomText { get; set; }
            public override void ExecuteResult(ControllerContext context)
            {
                RenderCaptchaImage(context);
            }
            private void RenderCaptchaImage(ControllerContext context)
            {
                Bitmap objBmp = new Bitmap(150, 60);
                Graphics objGraphic = Graphics.FromImage(objBmp);
                objGraphic.Clear(BackgroundColor);
                SolidBrush objBrush = new SolidBrush(RandomTextColor);
                Font objFont = null;
                int a;
                string myFont, str;
                string[] crypticsFont = new string[11];
                crypticsFont[0] = "Times New roman";
                crypticsFont[1] = "Verdana";
                crypticsFont[2] = "Sylfaen";
                crypticsFont[3] = "Microsoft Sans Serif";
                crypticsFont[4] = "Algerian";
                crypticsFont[5] = "Agency FB";
                crypticsFont[6] = "Andalus";
                crypticsFont[7] = "Cambria";
                crypticsFont[8] = "Calibri";
                crypticsFont[9] = "Courier";
                crypticsFont[10] = "Tahoma";
                for (a = 0; a < RandomText.Length; a++)
                {
                    myFont = crypticsFont[a];
                    objFont = new Font(myFont, 18, FontStyle.Bold | FontStyle.Italic |
                                                                      FontStyle.Strikeout);
                    str = RandomText.Substring(a, 1).ToLower();
                    objGraphic.DrawString(str, objFont, objBrush, a * 20, 20);
                    objGraphic.Flush();
                }
                context.HttpContext.Response.ContentType = "image/GF";
                objBmp.Save(context.HttpContext.Response.OutputStream, ImageFormat.Gif);
                objFont.Dispose();
                objGraphic.Dispose();
                objBmp.Dispose();
            }
        }
    }
}