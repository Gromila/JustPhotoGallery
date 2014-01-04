using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace JustPhotoGallery.Web.Helpers
{
    public static class FormHtmlHelper
    {
        public static MvcHtmlString DatePickerFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var text = html.TextBoxFor(expression, new { type = "text", id = "datepicker", @class = "date form-control" }).ToString();
            return MvcHtmlString.Create(text);
        }

        public static MvcHtmlString ThumbnailFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            var value = html.ValueFor(expression).ToString();
            return MvcHtmlString.Create(RenderThumbnail(value));
        }

        private static string RenderThumbnail(string value)
        {
            return String.Format("<a href=\"{0}\" class=\"thumbnail\" data-gallery><img src=\"{0}\" /\"></a>", value);
        }
    }
}