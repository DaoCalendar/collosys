using System.Web.Optimization;

namespace ColloSys.UserInterface.App_Start
{
    public static class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/moment.js",
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/select2.js",
                        "~/Scripts/bootstrap-datepicker.js",
                        "~/Scripts/alertify/alertify.js",
                        "~/Scripts/lodash.js",
                        "~/Scripts/underscore.string.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/angular.js",
                        "~/Scripts/restangular.js",
                        "~/Scripts/ng-upload.js",
                        "~/Scripts/ng-grid-{version}.js",
                        "~/Scripts/angular-ui.js",
                        //"~/Scripts/angular-ui-ieshiv.js",
                        "~/Scripts/ui-bootstrap-tpls-{version}.js",
                        "~/Scripts/angular-ui-select2.js",
                        "~/Shared/ng-collosys.js",
                        "~/Shared/cs-ng-grid.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-{version}.js"));

            bundles.Add(new StyleBundle("~/bundles/styles").Include(
                            "~/Content/bootstrap.css",
                            "~/Content/bootstrap-responsive.css",
                            "~/Content/font-awesome.css",
                            "~/Content/bootstrap-datepicker.css",
                            "~/Content/select2.css",
                            "~/Content/animateCss/animate.css",
                            "~/Content/alertify/alertify.core.css",
                            "~/Content/alertify/alertify.default.css",
                            "~/Content/angular-ui.css",
                            "~/Content/ng-grid.css",
                            "~/Content/menustyle.css",
                            "~/Content/Site.css"
                            ));
        }
    }
}


//    <!-- polyfill -->
//    @*<script src="~/Scripts/modernizr-2.6.2.js"></script>*@
//@*    <!-- bootstrap -->
//    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
//    <link href="~/Content/bootstrap-responsive.min.css" rel="stylesheet" />
//    <link href="~/Content/font-awesome.min.css" rel="stylesheet" />
//    <link href="~/Content/bootstrap-datepicker.css" rel="stylesheet" />
//    <link href="~/Content/select2.css" rel="stylesheet" />
//    <link href="~/Content/animateCss/animate.min.css" rel="stylesheet" />
//    <!-- alertify -->
//    <link href="~/Content/alertify/alertify.core.css" rel="stylesheet" />
//    <link href="~/Content/alertify/alertify.default.css" rel="stylesheet" />
//    <!-- angular -->
//    <link href="~/Content/angular-ui.css" rel="stylesheet" />
//    <link href="~/Content/ng-grid.css" rel="stylesheet" />
//    <!-- collosys -->
//    <link href="~/Content/Site.css" rel="stylesheet" />
//    <link href="~/Content/menustyle.css" rel="stylesheet" />*@


//    <!-- jquery -->
//@*  <script src="~/Scripts/moment.min.js"></script>
//    <script src="~/Scripts/jquery-1.10.2.min.js"></script>
//    <script src="~/Scripts/select2.min.js"></script>
//    <script src="~/Scripts/bootstrap-datepicker.min.js"></script>
//    <script src="~/Scripts/alertify/alertify.min.js"></script>
//    <script src="~/Scripts/lodash.min.js"></script>*@
//    <!-- angular -->
//@*    <script src="~/Scripts/angular.js"></script>
//    <!--script src="~/Scripts/angular-resource.min.js"></!--script--> 
//    <script src="~/Scripts/restangular.min.js"></script>
//    <script src="~/Scripts/i18n/angular-locale_en-in.js"></script>
//    <!--script src="~/Scripts/angular-mocks.js"></!--script-->
//    <script src="~/Scripts/ng-upload.min.js"></script>
//    <script src="~/Scripts/ng-grid-2.0.7.js"></script>
//    <script src="~/Scripts/angular-ui.min.js"></script>
//    <script src="~/Scripts/angular-ui-ieshiv.min.js"></script>
//    <script src="~/Scripts/ui-bootstrap-tpls-0.4.0.min.js"></script>
//    <script src="~/Scripts/angular-ui-select2.js"></script>
//    <!--script src="~/Scripts/angular-dragdrop.min.js"></!--script-->
//    <!-- collosys -->
//    <script src="~/Shared/ng-collosys.js"></script>*@


//bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
//            "~/Scripts/jquery-ui-{version}.js"));

//bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
//            "~/Scripts/jquery.unobtrusive*",
//            "~/Scripts/jquery.validate*"));

//// Use the development version of Modernizr to develop with and learn from. Then, when you're
//// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
