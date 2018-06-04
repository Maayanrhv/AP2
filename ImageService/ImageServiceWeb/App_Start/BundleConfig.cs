using System.Web;
using System.Web.Optimization;

namespace ImageServiceWeb
{
    public class BundleConfig
    {
<<<<<<< HEAD
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
=======
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
>>>>>>> 2154b1ce7a911a68053cc028ff93f9917d74e464
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
<<<<<<< HEAD
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
=======
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
>>>>>>> 2154b1ce7a911a68053cc028ff93f9917d74e464
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
<<<<<<< HEAD
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
=======
                      "~/Scripts/bootstrap.js"));
>>>>>>> 2154b1ce7a911a68053cc028ff93f9917d74e464

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
