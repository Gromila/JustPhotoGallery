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
            return String.Format("<a href=\"{0}\" data-gallery><img src=\"{0}\" data-src=\"holder.js/300x200\" class=\"thumbnail\"  /\"></a>", value);
        }

        public static MvcHtmlString NewDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, Type type, object selected)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Type is not an enum.");

            if (selected != null && selected.GetType() != type)
                throw new ArgumentException("Selected object is not " + type.ToString());

            var enums = new List<SelectListItem>();
            foreach (int value in Enum.GetValues(type))
            {
                var item = new SelectListItem {Value = Enum.GetName(type, value), Text = Enum.GetName(type, value)};
                if (selected != null)
                    item.Selected = (int)selected == value;

                enums.Add(item);
            }
            return helper.DropDownListFor(expression, enums, "--= Select =--", new { @class = "form-control" });
        }
    }
}