﻿using System.Web.Optimization;

namespace JustPhotoGallery.Web
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. по адресу: http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-fileinput.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/jquery.ui.js",
                      "~/Scripts/holder.js"));

            bundles.Add(new ScriptBundle("~/bundles/tagcloud").Include(
                "~/Scripts/jquery.xdcloudtags.min.js"));

            bundles.Add(new ScriptBundle("~/bundle/slideshow").Include(
                "~/Scripts/jquery-2.0.3.min.js",
                "~/Scripts/jquery.easing.1.3.js",
                "~/Scripts/photos.gallery.js"));

            bundles.Add(new ScriptBundle("~/bundle/imagepreview").Include(
                "~/Scripts/jquery.image.preview.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/blueimp-gallery.css",
                      "~/Content/bootstrap-fileinput.css",
                      "~/Content/jquery*",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/search").Include("~/Scripts/search.*"));
        }
    }
}
