using System.Web;
using System.Web.Optimization;

namespace RMS
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

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/assets/css/jquery-ui.custom.min.css").Include(
            //          "~/assets/css/jquery-ui.custom.min.css",
            //          "assets/css/jquery.gritter.min.css",
            //          "~/assets/css/jquery-ui.min.css",
            //          "~/assets/css/jquery-ui.min.css",
            //          "~/assets/css/datepicker.min.css",
            //          "~/assets/css/bootstrap-timepicker.min.css",
            //          "~/assets/css/daterangepicker.min.css",
            //          "~/assets/css/bootstrap-datetimepicker.min.css",
            //          "~/assets/fonts/fonts.googleapis.com.css",
            //          "../../assets/font-awesome/4.2.0/css/font-awesome.min.css",
            //          "~/assets/css/malaka.css"
            //          ));


        }
    }
}
