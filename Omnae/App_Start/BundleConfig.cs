using System.Web.Optimization;

namespace Omnae
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-extras").Include(
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/jquery-ui-1.12.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap-{version}.js",
            //          "~/Scripts/alertify/alertify.min.js",
            //          //"~/Scripts/umd/popper.min.js",
            //          "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/mdbootstrap").Include(
                        "~/Content/mdb-pro/js/tether.js",
                        "~/Content/mdb-pro/js/bootstrap.js",
                        "~/Content/mdb-pro/js/mdb.js",
                        "~/Content/mdb-pro/js/prism.js",
                        "~/Content/mdb-pro/js/fontawesome-all.js"));

            bundles.Add(new ScriptBundle("~/bundles/extras").Include(
                        "~/Scripts/gridmvc.js",
                        "~/Scripts/gridmvc-ext.js",
                        "~/Scripts/ladda-bootstrap/ladda.min.js",
                        "~/Scripts/ladda-bootstrap/spin.min.js",
                        "~/Scripts/bootstrap-submenu-2.0.4/dist/js/bootstrap-submenu.js",
                        "~/Scripts/bootstrap-editable.min.js",
                        "~/Scripts/app/common.js",
                        "~/Scripts/fileinput.min.js",
                        "~/Scripts/alertify/alertify.min.js",
                        "~/Scripts/tablesorter/jquery.tablesorter.combined.min.js"));



            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/alertify/alertify.core.css",
            //          "~/Content/alertify/alertify.default.css",
            //          "~/Content/media-screens.css",
            //          "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/alertify/alertify.core.css",
                      "~/Content/alertify/alertify.default.css",
                      "~/Content/media-screens.css"));

            bundles.Add(new StyleBundle("~/Content/mdb-pro/css/mdbootstrap-css").Include(
                      "~/Content/mdb-pro/css/bootstrap.min.css",
                      "~/Content/mdb-pro/css/compiled.min.css",
                      "~/Content/mdb-pro/css/woocommerce.css",
                      "~/Content/mdb-pro/css/woocommerce-layout.css",
                      "~/Content/mdb-pro/css/woocommerce-smallscreen.css",
                      //"~/Content/mdb-pro/css/font.awesome.min.css",
                      "~/Content/mdb-pro/css/mdb.min.css",
                      "~/Content/mdb-pro/css/fa-svg-with-js.css"));

            bundles.Add(new StyleBundle("~/Content/css2").Include(
                "~/Content/Site.css",
                "~/Content/Gridmvc.css",
                "~/Content/ladda-bootstrap/ladda-themeless.min.css",
                "~/Scripts/bootstrap-submenu-2.0.4/dist/css/bootstrap-submenu.min.css",
                "~/Content/themes/base/jquery-ui.min.css",
                "~/Content/themes/base/jquery.ui.tabs.css",
                "~/Content/bootstrap-fileinput/css/fileinput.min.css",
                "~/Content/tablesorter/style.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/jqueryui").Include(
                "~/Content/themes/base/jquery-ui.css"));
        }
    }
}
