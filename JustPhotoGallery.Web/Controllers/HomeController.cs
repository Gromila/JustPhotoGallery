using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustPhotoGallery.Repositories;
using JustPhotoGallery.Web.App_LocalResources;
using JustPhotoGallery.Web.Models;

namespace JustPhotoGallery.Web.Controllers
{
    public class HomeController : Controller
    {
        private UnitOfWork unitOfWork;

        public HomeController()
        {
            unitOfWork = new UnitOfWork();
        }

        public ActionResult Index()
        {
            ViewBag.SlideShow = true;
            
            return View();
        }
        
        public ActionResult DisplayTagsCloud()
        {
            var tags = unitOfWork.TagRepository.Read().OrderByDescending(a => a.Pictures.Count).Distinct().Take(18).OrderBy(a => a.Pictures.Count).ToList();
            if (tags.Count == 0)
                return Content(GlobalRes.NoTags);
            return PartialView("_TagsCloudPartial", tags);
        }

        public ActionResult Error(String errorMessage)
        {
            ViewBag.Error = errorMessage;
            return View("Error");
        }
    }
}