using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustPhotoGallery.Repositories;
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
            return View();
        }
        
        public ActionResult DisplayTagsCloud()
        {
            // TODO: TAGs parsing!
            // TODO: read javascriptkit.com
            var tags = unitOfWork.TagRepository.Read();
            return PartialView("Shared/_TagsCloudPartial", tags);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}