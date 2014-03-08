// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="">
//   Copyright © 2014 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace App.MiniTrello.Web
{
    using System.Web;
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/content/css/app").Include("~/content/app.css").Include("~/content/Style.css"));
            

            //bundles.Add(new ScriptBundle("~/js/jquery").Include("~/scripts/vendor/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/js/vendors").Include(
                "~/scripts/vendor/jquery-2.0.3.js",
                "~/scripts/vendor/angular.js"));

            bundles.Add(new ScriptBundle("~/js/app").Include(
                "~/scripts/vendor/angular-ui-router.js",
                "~/scripts/Filters/filters.js",
                "~/scripts/Services/AccountServices.js",
                "~/scripts/Services/BoardServices.js",
                "~/scripts/Directives/directives.js",
                "~/scripts/Controllers/HomeController.js",
                "~/scripts/Controllers/AboutController.js",
                "~/scripts/Controllers/ErrorController.js",
                "~/scripts/Controllers/BoardController.js",
                "~/scripts/Controllers/AccountController.js",
                "~/scripts/app.js"));

        }
    }
}
