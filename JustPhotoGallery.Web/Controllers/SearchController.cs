using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using JustPhotoGallery.Domain.Entities;
using JustPhotoGallery.Repositories;
using JustPhotoGallery.Web.App_LocalResources;
using JustPhotoGallery.Web.Models;
using Microsoft.Ajax.Utilities;

namespace JustPhotoGallery.Web.Controllers
{
    public class SearchController : Controller
    {
        private UnitOfWork unitOfWork;

        public SearchController()
        {
            unitOfWork = new UnitOfWork();
        }

        public ActionResult Search(String searchString)
        {
            ViewBag.SlideShow = true;
            ViewBag.SearchString = searchString;
            return View("Result");
        }

        public ActionResult SearchTags(String searchString)
        {
            var tags =
                unitOfWork.TagRepository.Read(a => a.Content.Contains(searchString))
                    .OrderByDescending(a => a.Pictures.Count)
                    .Take(15);
            if (tags.Any())
                return PartialView("Shared/_TagsPartial", tags);
            return Content(GlobalRes.NoTags);
        }

        public ActionResult SearchUsers(String searchString)
        {
            var users = unitOfWork.UserRepository.Read(a => a.UserName.Contains(searchString)).Take(15);
            if (users.Any())
                return PartialView("Shared/_UsersPartial", users);
            return Content(GlobalRes.NoUsers);
        }

        public ActionResult SearchPhotos(String searchString, int page)
        {
            const int pageSize = 2;

            var photos = unitOfWork.PictureRepository.Read(filter: a => a.Title.Contains(searchString)).Select(photo => new PhotoViewModel { Picture = photo }).OrderByDescending(m => m.VotesValue).ToList();

            var photosViewModel = new PagedViewModel<PhotoViewModel>
            {
                Data = photos.Skip(page == 1 ? 0 : 6 + pageSize * (page - 2)).Take(page == 1 ? 6 : pageSize).ToList(),
                PageNumber = photos.Count % pageSize == 0 ? (photos.Count / pageSize - 2) : (photos.Count / pageSize - 1),
                CurrentPage = page
            };

            if (photosViewModel.PageNumber == photosViewModel.CurrentPage)
                photosViewModel.CurrentPage = -10;

            if (photos.Any())
                return PartialView("Shared/_PhotosPartial", photosViewModel);
            return Content(GlobalRes.NoPhotos);
        }

        public ActionResult SearchByTag(int tagId)
        {
            var photos = unitOfWork.TagRepository.ReadById(tagId).Pictures.Select(photo => new PhotoViewModel { Picture = photo }).OrderByDescending(m => m.VotesValue).ToList();
            return View("Results", photos);
        }

        public JsonResult SearchAutoCompleter(string term)
        {
            term = term.Split(new char[] {',', ' '}).Last();
            if (term.IsNullOrWhiteSpace())
                return Json("", JsonRequestBehavior.AllowGet);
            var tags =
                unitOfWork.TagRepository.Read(a => a.Content.Contains(term))
                    .Select(label => label.Content)
                    .Distinct()
                    .Take(10);
            return Json(tags, JsonRequestBehavior.AllowGet);
        }
	}
}