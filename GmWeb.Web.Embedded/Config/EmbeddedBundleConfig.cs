using System.Web;
using System.Web.Optimization;
using GmWeb.Logic.Utility.Extensions;

namespace GmWeb.Web.Embedded.Config
{
    public class EmbeddedBundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Scripts

            // Vendor scripts
            bundles.Add(new ScriptBundle("~/Scripts/Bundles/jquery").Include(
                "~/Scripts/plugins/jquery/jquery-{version}.js",
                "~/Scripts/plugins/jquery.ui/jquery-ui.js"
            ));

            // jQuery Validation
            bundles.Add(new ScriptBundle("~/Scripts/Bundles/jquery.validate").Include(
                "~/Scripts/plugins/jquery.validate/jquery.validate.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/Bundles/bootstrap").Include(
                "~/Scripts/plugins/bootstrap/bootstrap.js",
                "~/Scripts/plugins/popper-umd/popper.js"
            ));

            // Inspinia script
            bundles.Add(new ScriptBundle("~/Scripts/Bundles/inspinia").Include(
                "~/Scripts/app/inspinia.js"
            ));

            // SlimScroll
            bundles.Add(new ScriptBundle("~/Scripts/Bundles/slimScroll").Include(
                "~/Scripts/plugins/jquery.slimscroll/jquery.slimscroll.js"
            ));

            // jQuery plugins
            bundles.Add(new ScriptBundle("~/Scripts/Bundles/metisMenu").Include(
                "~/Scripts/plugins/metisMenu/metisMenu.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/Bundles/pace").Include(
                "~/Scripts/plugins/pace/pace-1.0.2.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/Bundles/modernizr").Include(
                "~/Scripts/plugins/modernizr/modernizr.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/Bundles/jSignature").Include(
                "~/Scripts/plugins/jSignature/jSignature.noconflict.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/Bundles/sparkline").Include(
                "~/Scripts/plugins/jquery.sparkline/jquery.sparkline.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/Bundles/moment").Include(
                "~/Scripts/plugins/moment/moment.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/Bundles/pwstrength").Include(
                "~/Scripts/plugins/jquery.pwstrength.bootstrap/pwstrength-bootstrap.js",
                "~/Scripts/plugins/jquery.pwstrength.bootstrap/zxcvbn.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/Bundles/qrcode").Include(
                "~/Scripts/plugins/qrcode/qrcode.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/Bundles/gm-preload").Include(
                "~/Scripts/gm/preload.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/Bundles/gm-postload").Include(
                "~/Scripts/gm/load.handlers.js",
                "~/Scripts/gm/observable.view.model.js",
                "~/Scripts/gm/ordered.set.js",
                "~/Scripts/gm/textarea.autosize.js",
                "~/Scripts/gm/password.meter.js",
                "~/Scripts/gm/utility.js",
                "~/Scripts/plugins/ajax-dates/ajax-dates.js",
                "~/Scripts/gm/collection.editors.js"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/Bundles/kendo").Include(
                "~/Scripts/kendo/jszip.min.js",
                "~/Scripts/kendo/kendo.all.min.js",
                "~/Scripts/kendo/kendo.aspnetmvc.min.js"
            ));

            #endregion

            #region Contents

            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/Bundles/css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/animate.css",
                "~/Content/style.css",
                "~/Content/fonts/font-awesome/css/all.css",
                "~/Content/gm/inspinia.overrides.css",
                "~/Content/gm/textarea.autosize.css",
                "~/Content/gm/collection.editors.css",
                "~/Content/gm/pwstrength.overrides.css",
                "~/Content/jquery.ui/jquery-ui.css",
                "~/Content/jquery.ui/jquery-ui.structure.css",
                "~/Content/jquery.ui/jquery-ui.theme.css"
            ));

            bundles.Add(new StyleBundle("~/Content/Bundles/kendo").Include(
                "~/Content/kendo/kendo.common.min.css",
                "~/Content/kendo/kendo.uniform.min.css",
                "~/Content/kendo-gm-overrides.css"
            ));

            #endregion
        }
    }
}
