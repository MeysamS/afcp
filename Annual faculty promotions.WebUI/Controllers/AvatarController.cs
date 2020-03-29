using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Annual_faculty_promotions.WebUI.Controllers
{
    [Authorize]
    public partial class AvatarController : Controller
    {
        private int _avatarWidth = 200; // ToDo - Change the size of the stored avatar image
        private int _avatarHeight = 200; // ToDo - Change the size of the stored avatar image

        [HttpGet]
        public virtual ActionResult _Upload()
        {
            return PartialView();
        }

        [ValidateAntiForgeryToken]
        public virtual ActionResult _Upload(IEnumerable<HttpPostedFileBase> files)
        {
            string errorMessage = "";

            if (files != null && files.Count() > 0)
            {
                // Get one only
                var file = files.FirstOrDefault();
                // Check if the file is an image
                if (file != null && IsImage(file))
                {
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {
                        var webPath = SaveTemporaryFile(file);
                        return Json(new { success = true, fileName = webPath.Replace("\\", "/") }); // success
                    }
                    errorMessage = "File cannot be zero length."; //failure
                }
                errorMessage = "فرمت فایل اشتباه است"; //failure
            }
            errorMessage = "آپلود عکس ناموفق!"; //failure

            return Json(new { success = false, errorMessage = errorMessage });
        }

        [HttpPost]
        public virtual ActionResult Save(string t, string l, string h, string w, string fileName)
        {

            try
            {

                // Get file from temporary folder
                var fn = Path.Combine(Server.MapPath("/Content/Images/Temp"), Path.GetFileName(fileName));

                // Calculate dimesnions
                int top = Convert.ToInt32(t.Replace("-", "").Replace("px", ""));
                int left = Convert.ToInt32(l.Replace("-", "").Replace("px", ""));
                int height = Convert.ToInt32(h.Replace("-", "").Replace("px", ""));
                int width = Convert.ToInt32(w.Replace("-", "").Replace("px", ""));

                // Get image and resize it, ...
                var img = new WebImage(fn);
                img.Resize(width, height);
                // ... crop the part the user selected, ...
                img.Crop(top, left, img.Height - top - _avatarHeight, img.Width - left - _avatarWidth);
                // ... delete the temporary file,...
                System.IO.File.Delete(fn);
                // ... and save the new one.
                var fname = User.Identity.GetUserName();
                string newFileName = "/Content/Images/Avatars/" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 16) + Path.GetExtension(fileName);
                string newFileLocation = HttpContext.Server.MapPath(newFileName);
                if (Directory.Exists(Path.GetDirectoryName(newFileLocation)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newFileLocation));
                }

                img.Save(newFileLocation);
                return Json(new { success = true, avatarFileLocation = newFileName, avatarId = Path.GetFileName(newFileLocation) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to upload file.\nERRORINFO: " + ex.Message });
            }
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            var extensions = new string[] { ".jpg", ".png", ".gif", ".jpeg" }; // ToDo - add more if you like...

            // linq from Henrik Stenbæk
            return extensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private string SaveTemporaryFile(HttpPostedFileBase file)
        {


            // Define destination
            var folderName = "/Content/Images/Temp";
            var serverPath = HttpContext.Server.MapPath(folderName);
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }

            // Generate unique file name
            var fileName = Path.GetFileName(file.FileName);
            string ext = fileName.Substring(fileName.IndexOf("."), 4);
            fileName = User.Identity.GetUserId() + ext;
            fileName = SaveTemporaryAvatarFileImage(file, serverPath, fileName);

            // Clean up old files after every save
            CleanUpTempFolder(1);

            return Path.Combine(folderName, fileName);
        }

        private string SaveTemporaryAvatarFileImage(HttpPostedFileBase file, string serverPath, string fileName)
        {
            var img = new WebImage(file.InputStream);
            double ratio = (double)img.Height / (double)img.Width;

            string fullFileName = Path.Combine(serverPath, fileName);

            img.Resize(400, (int)(400 * ratio)); // ToDo - Change the value of the width of the image on the screen

            if (System.IO.File.Exists(fullFileName))
                System.IO.File.Delete(fullFileName);

            img.Save(fullFileName);

            return Path.GetFileName(img.FileName);
        }

        private void CleanUpTempFolder(int hoursOld)
        {
            try
            {
                DateTime fileCreationTime;
                DateTime currentUtcNow = DateTime.UtcNow;

                var serverPath = HttpContext.Server.MapPath("~/Content/Images/Temp");
                if (Directory.Exists(serverPath))
                {
                    string[] fileEntries = Directory.GetFiles(serverPath);
                    foreach (var fileEntry in fileEntries)
                    {
                        fileCreationTime = System.IO.File.GetCreationTimeUtc(fileEntry);
                        var res = currentUtcNow - fileCreationTime;
                        if (res.TotalHours > hoursOld)
                        {
                            System.IO.File.Delete(fileEntry);
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}