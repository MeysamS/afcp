using System.Web;
using System.Web.Optimization;

namespace Annual_faculty_promotions.WebUI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryLinq").Include(
                      "~/Scripts/jquery.linq.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryCrop").Include(
                 "~/Scripts/jquery.Jcrop.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryAjaxForm").Include(
                        "~/Scripts/AjaxForm/jquery.form.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/avatar").Include(
                        "~/Scripts/site.avatar.js"));

            bundles.Add(new ScriptBundle("~/bundles/PersianDate").Include(
                    "~/Scripts/PersianDatePicker.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/avatar/css").Include(
                "~/Content/site.avatar.css"));

            bundles.Add(new StyleBundle("~/Content/Jcrop/css").Include(
                   "~/Content/jquery.Jcrop.min.css"));

            bundles.Add(new StyleBundle("~/Content/persianDate/css").Include(
                  "~/Content/PersianDatePicker.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            "~/Scripts/bootstrap.rtl.js",
            "~/Scripts/respond.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                 "~/Content/bootstrap.rtl.css", "~/Content/Site.css","~/Content/buttons.css"));

        }
    }
}
