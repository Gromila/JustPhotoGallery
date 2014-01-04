using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using JustPhotoGallery.Domain.Entities;
using JustPhotoGallery.Repositories;

namespace JustPhotoGallery.Web.Controllers
{
    public class SearchController : Controller
    {
        private UnitOfWork unitOfWork;

        public SearchController()
        {
            unitOfWork = new UnitOfWork();
        }

        public JsonResult SearchAutoCompleter(string term)
        {
            var tags =
                unitOfWork.TagRepository.Read(tag => tag.Content.Contains(term))
                    .Select(label => label.Content)
                    .Distinct()
                    .Take(10);
            return Json(tags, JsonRequestBehavior.AllowGet);
        }
	}
}