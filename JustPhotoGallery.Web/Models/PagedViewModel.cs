using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JustPhotoGallery.Web.Models
{
    public class PagedViewModel<TModel> where TModel : class
    {
        public IEnumerable<TModel> Data { get; set; }

        public int PageNumber { get; set; }

        public int CurrentPage { get; set; }
    }
}