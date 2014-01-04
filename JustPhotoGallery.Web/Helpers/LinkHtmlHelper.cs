using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace JustPhotoGallery.Web.Helpers
{
    public static class LinkHtmlHelper
    {
        public static MvcHtmlString NavBarLink(this HtmlHelper html, String textLink, String actionName,
            String controllerName, String iconType = "", object routeValues = null, object htmlAttributes = null)
        {
            var builder = new TagBuilder("span");
            builder.MergeAttribute("class", iconType);
            var link = html.ActionLink("[replaceme] " + textLink, actionName, controllerName, routeValues, htmlAttributes).ToHtmlString();
            return new MvcHtmlString(link.Replace("[replaceme]", builder.ToString()));
        }
    }
}