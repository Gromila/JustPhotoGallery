using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustPhotoGallery.Domain.Entities;
using JustPhotoGallery.Repositories;
using JustPhotoGallery.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JustPhotoGallery.Web.Controllers
{
    public class PhotoController : Controller
    {
        private UnitOfWork unitOfWork;

        public PhotoController()
        {
            unitOfWork = new UnitOfWork();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Upload()
        {
            return View("Upload", new Picture());
        }

        [Authorize]
        [HttpPost]
        public ActionResult Upload(Picture picture, HttpPostedFileBase file, string tagsString)
        {
            if (ModelState.IsValid && file != null)
            {
                //TO DO: timestamp or hashcode
                picture.FileName = file.FileName;
                picture.UserId = User.Identity.GetUserId();
                SaveFile(file);
                SaveEntities(picture, ParseTags(tagsString));
            }
            return RedirectToAction("Index", "Home");
        }

        private void SaveEntities(Picture picture, IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                picture.Tags.Add(SearchTagInDb(tag));
            }
            unitOfWork.PictureRepository.Create(picture);
            unitOfWork.Save();
        }

        private Tag SearchTagInDb(Tag tag)
        {
            var existedTag = unitOfWork.TagRepository.Read(a => a.Content == tag.Content).FirstOrDefault();
            if (existedTag == null)
                return tag;
            return existedTag;
        }

        private IEnumerable<Tag> ParseTags(string tags)
        {
            return tags.Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries).Select(tag => new Tag(tag)).ToList();
        }

        private void SaveFile(HttpPostedFileBase file)
        {
            string path = Path.Combine(Server.MapPath("~/Upload/Images/"), Path.GetFileName(file.FileName));
            if (!Directory.Exists(Server.MapPath("~/Upload/Images")))
                Directory.CreateDirectory(Server.MapPath("~/Upload/Images"));
            file.SaveAs(path);
        }

        

        public ActionResult DisplayTop(int number)
        {
            var photos = unitOfWork.PictureRepository.Read().Select(photo => new PhotoViewModel { Picture = photo }).OrderByDescending(m => m.VotesValue).Take(number).ToList();
            /*for (int i = 0; i < photos.Count; i++)
            {
                photos[i].FilePath = Path.Combine(Server.MapPath("~/Upload/Images/"), photos[i].FileName);
            }*/
            return PartialView("Shared/_TopPhotosPartial", photos);
        }
    }
}