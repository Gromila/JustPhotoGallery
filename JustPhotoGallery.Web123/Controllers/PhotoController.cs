using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustPhotoGallery.Domain.Entities;
using JustPhotoGallery.Repositories;

namespace JustPhotoGallery.Web.Controllers
{
    public class PhotoController : Controller
    {
        private UnitOfWork unitOfWork;

        public PhotoController()
        {
            unitOfWork = new UnitOfWork();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View("Upload", new Picture());
        }

        [HttpPost]
        public ActionResult Upload(Picture picture, HttpPostedFileBase file, string tagsString)
        {
            if (ModelState.IsValid && file != null)
            {
                picture.FileName = file.FileName;
                SaveFile(file);
                SaveEntities(picture, ParseTags(tagsString));
            }
            return RedirectToAction("Index", "Home");
        }

        private void SaveEntities(Picture picture, IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                tag.Pictures.Add(picture);
                picture.Tags.Add(tag);
                SaveTag(tag);
            }
            unitOfWork.PictureRepository.Create(picture);
            unitOfWork.Save();
        }

        private void SaveTag(Tag tag)
        {
            var existedTag = unitOfWork.TagRepository.Read(a => a.Content == tag.Content).FirstOrDefault();
            if (existedTag == null)
            {
                unitOfWork.TagRepository.Create(tag);
            }
            else
            {
                existedTag.Pictures.Add(tag.Pictures.ToList()[0]);
                unitOfWork.TagRepository.Update(existedTag);
            }
        }

        private IEnumerable<Tag> ParseTags(string tags)
        {
            return tags.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(tag => new Tag(tag)).ToList();
        }

        private void SaveFile(HttpPostedFileBase file)
        {
            string path = Path.Combine(Server.MapPath("~/Upload/Images/"), Path.GetFileName(file.FileName));
            if (!Directory.Exists(Server.MapPath("~/Upload/Images")))
                Directory.CreateDirectory(Server.MapPath("~/Upload/Images"));
            file.SaveAs(path);
        }

    }
}