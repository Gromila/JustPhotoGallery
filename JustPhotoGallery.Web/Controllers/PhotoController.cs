using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using JustPhotoGallery.Domain.Entities;
using JustPhotoGallery.Repositories;
using JustPhotoGallery.Web.App_LocalResources;
using JustPhotoGallery.Web.Helpers;
using JustPhotoGallery.Web.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;

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
            try
            {
                if (ModelState.IsValid && file != null && CheckExtension(Path.GetExtension(file.FileName).ToLower()))
                {
                    picture.FileName = GenerateFileName(file);
                    picture.UserId = User.Identity.GetUserId();
                    SaveEntities(picture, ParseTags(tagsString), file);
                }
            }
            catch (Exception exception)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = exception.Message });
            }
            return RedirectToAction("Show", "Photo", new { id = picture.Id });
        }

        private void SaveEntities(Picture picture, IEnumerable<Tag> tags, HttpPostedFileBase file)
        {
            foreach (var tag in tags)
            {
                picture.Tags.Add(SearchTagInDb(tag));
            }
            unitOfWork.PictureRepository.Create(picture);
            unitOfWork.Save();
            SaveFile(file, picture.FileName);
            if (picture.Filter != FilterType.None)
                SaveFilteredImage(picture.FileName, picture.Filter);
        }

        private Tag SearchTagInDb(Tag tag)
        {
            var existedTag = unitOfWork.TagRepository.Read(a => a.Content == tag.Content).FirstOrDefault();
            return existedTag ?? tag;
        }

        private IEnumerable<Tag> ParseTags(string tagsString)
        {
            var tags = tagsString.Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tags.Count(); i++)
            {
                tags[i] = tags[i].ToLower();
            }
            return tags.Select(tag => new Tag(tag)).ToList();
        }

        private void SaveFile(HttpPostedFileBase file, String filename)
        {
            string path = Path.Combine(Server.MapPath(GlobalRes.ImagesFolder), filename);
            if (!Directory.Exists(Server.MapPath("~/Upload/Images")))
                Directory.CreateDirectory(Server.MapPath("~/Upload/Images"));
            file.SaveAs(path);
            SaveThumbnail(filename);
        }

        private void SaveFilteredImage(String filename, FilterType filterType)
        {
            (new ImageProcessing()).FilterImage(filename, Server.MapPath(GlobalRes.ImagesFolder), filterType);
            SaveThumbnail(String.Format("filtered_{0}", filename));
        }

        private void SaveThumbnail(String filename)
        {
            (new ImageProcessing()).CreateThumbnail(filename, Server.MapPath(GlobalRes.ImagesFolder), 200, 200);
        }

        private String GenerateFileName(HttpPostedFileBase file)
        {
            String result = "";
            using (var crypto = new SHA1CryptoServiceProvider())
            {
                result = Convert.ToBase64String(crypto.ComputeHash(file.InputStream))
                    .Replace(" ", "0")
                    .Replace("\\", "2")
                    .Replace("/", "3")
                    .Replace("+", "4")
                    .Replace("-", "5")
                    .Replace("=", "6")
                    .Replace("%", "7")
                    .Replace("$", "9")
                    .Replace("^", "8");
            }
            return String.Format("{0}{1}", result, file.FileName);
        }

        public ActionResult DisplayTop(int page)
        {
            try
            {
                var pageSize = 3;

                var photos = unitOfWork.PictureRepository.Read().Select(photo => new PhotoViewModel { Picture = photo }).OrderByDescending(m => m.VotesValue).ToList();
                var photosViewModel = new PagedViewModel<PhotoViewModel>
                {
                    Data = photos.Skip(page == 1 ? 0 : 9 + pageSize * (page - 2)).Take(page == 1 ? 9 : pageSize).ToList(),
                    PageNumber = photos.Count % pageSize == 0 ? (photos.Count / pageSize - 2) : (photos.Count / pageSize - 1),
                    CurrentPage = page
                };
                if (photosViewModel.PageNumber == photosViewModel.CurrentPage)
                    photosViewModel.CurrentPage = -10;
                if (photos.Count == 0)
                    return Content(GlobalRes.NoPhotos);
                return PartialView("_TopPhotosPartial", photosViewModel);
            }
            catch (Exception exception)
            {
                return RedirectToAction("Error", "Home", new {errorMessage = exception.Message});
            }
        }

        public ActionResult Show(int id)
        {
            var photo = new PhotoViewModel() {Picture = unitOfWork.PictureRepository.ReadById(id)};
            ViewBag.Disabled = !Request.IsAuthenticated ? "disabled" : "";
            return View("SinglePhoto", photo);
        }

        [Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                var filename = unitOfWork.PictureRepository.ReadById(id).FileName;
                System.IO.File.Delete(Path.Combine(Server.MapPath(GlobalRes.ImagesFolder), filename));
                System.IO.File.Delete(Path.Combine(Server.MapPath(GlobalRes.ImagesFolder), String.Format("thumb_{0}", filename)));
                if (System.IO.File.Exists(Path.Combine(Server.MapPath(GlobalRes.ImagesFolder), String.Format("filtered_{0}", filename))))
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath(GlobalRes.ImagesFolder), String.Format("filtered_{0}", filename)));
                    System.IO.File.Delete(Path.Combine(Server.MapPath(GlobalRes.ImagesFolder), String.Format("thumb_filtered_{0}.png", filename)));
                }
                foreach (var vote in unitOfWork.PictureRepository.ReadById(id).Votes)
                {
                    unitOfWork.VoteRepository.Delete(vote);
                }
                unitOfWork.PictureRepository.Delete(id);
                unitOfWork.Save();
                return RedirectToAction("Profile", "Photo", new { username = User.Identity.GetUserName() });
            }
            catch (Exception exception)
            {
                return RedirectToAction("Error", "Home", new {errorMessage = exception.Message});
            }
        }

        public ActionResult CheckExtensionAction(String filepath)
        {
            var extension = Path.GetExtension(filepath).ToLower();
            return Content(CheckExtension(extension).ToString().ToLower());
        }

        private bool CheckExtension(String extension)
        {
            return (extension == ".png" || extension == ".jpeg" || extension == ".jpg" || extension == ".gif");
        }

        public ActionResult GetAuthorName(int pictureId)
        {
            var user =
                unitOfWork.UserRepository.ReadById(unitOfWork.PictureRepository.ReadById(pictureId).UserId);
            String username = user == null ? "Deleted" : user.UserName;
            return Content(String.Format("<a href=\"{0}\">{1}</a>", Url.Action("Profile", "Photo", new { username = username }), username));
        }

        [Authorize]
        public ActionResult Profile(String username)
        {
            ViewBag.SlideShow = true;

            ViewBag.PageHeader = String.Format(GlobalRes.ProfileOwner, username);
            var user = unitOfWork.UserRepository.Read(filter: a => a.UserName == username).SingleOrDefault();
            return View("Profile", user);
        }

        public ActionResult ShowUserPhotos(String userId, int page)
        {
            try
            {
                const int pageSize = 3;

                var photos = unitOfWork.PictureRepository.Read(filter: a => a.UserId == userId).Select(photo => new PhotoViewModel { Picture = photo }).OrderByDescending(m => m.VotesValue).ToList();

                var photosViewModel = new PagedViewModel<PhotoViewModel>
                {
                    Data = photos.Skip(page == 1 ? 0 : 9 + pageSize * (page - 2)).Take(page == 1 ? 9 : pageSize).ToList(),
                    PageNumber = photos.Count % pageSize == 0 ? (photos.Count / pageSize - 2) : (photos.Count / pageSize - 1),
                    CurrentPage = page
                };

                if (photosViewModel.PageNumber == photosViewModel.CurrentPage)
                    photosViewModel.CurrentPage = -10;

                if (photos.Count == 0)
                    return Content(GlobalRes.NoPhotos);

                return PartialView("_TopPhotosPartial", photosViewModel);

            }
            catch (Exception exception)
            {
                return RedirectToAction("Error", "Home", new {errorMessage = exception.Message});
            }
        }

        public ActionResult GetPhotosCount(String userId)
        {
            return Content(unitOfWork.UserRepository.ReadById(userId).Pictures.Count.ToString());
        }

        public ActionResult GetPhotoTitle(int photoId)
        {
            return Content(unitOfWork.PictureRepository.ReadById(photoId).Title);
        }

        public ActionResult ShowUserTags(String userId)
        {
            var tags = new List<Tag>();
            foreach (var photo in unitOfWork.UserRepository.ReadById(userId).Pictures)
                tags.AddRange(photo.Tags);
            return PartialView("Shared/_TagsPartial", tags.Distinct().OrderByDescending(a => a.Pictures.Count).Take(10));
        }

        public ActionResult Slideshow(String url, String query)
        {
            try
            {
                ViewBag.Url = url;
                var photos = new List<PhotoViewModel>();
                if (query == null)
                {
                    photos = unitOfWork.PictureRepository.Read().Select(photo => new PhotoViewModel { Picture = photo }).OrderByDescending(m => m.VotesValue).Take(20).ToList();
                }
                else if (query.Contains("tagId"))
                {
                    int tagId = Int32.Parse(query.Split(new char[] { '=' })[1]);
                    photos = unitOfWork.TagRepository.ReadById(tagId).Pictures.Select(photo => new PhotoViewModel { Picture = photo }).OrderByDescending(m => m.VotesValue).ToList();
                }
                else if (query.Contains("username"))
                {
                    var username = query.Split(new char[] { '=' })[1];
                    String userId = unitOfWork.UserRepository.Read(a => a.UserName == username).FirstOrDefault().Id;
                    photos = unitOfWork.UserRepository.ReadById(userId).Pictures.Select(photo => new PhotoViewModel() { Picture = photo }).ToList();
                }
                if (photos.Count == 0)
                    return Content(GlobalRes.NoPhotos);

                return View("Slideshow", photos);
            }
            catch (Exception exception)
            {
                return RedirectToAction("Error", "Home", new {errorMessage = exception.Message});
            }
        }

        public ActionResult CloseSlideshow(String url)
        {
            if (url.Contains("Profile"))
            {
                var username = url.Split(new char[] {'='})[1];
                return RedirectToAction("Profile", "Photo", new {username = username});
            }
            else
                return Redirect(url);

        }

        //TODO: add checks on null!!!

        public ActionResult ApplyFilter(String filter, int pictureId)
        {
            var type = (FilterType)Enum.Parse(typeof (FilterType), filter);
            var photo = unitOfWork.PictureRepository.ReadById(pictureId);
            photo.Filter = type;
            SaveFilteredImage(photo.FileName, type);
            var viewModel = new PhotoViewModel() {Picture = photo};
            unitOfWork.Save();
            return Content(viewModel.FilePath);
        }

        public ActionResult EditTitle(String title, int pictureId)
        {
            var photo = unitOfWork.PictureRepository.ReadById(pictureId);
            photo.Title = title;
            unitOfWork.Save();
            ViewBag.PageHeader = photo.Title;
            return Content(photo.Title);
        }

        public ActionResult EditDescription(String description, int pictureId)
        {
            var photo = unitOfWork.PictureRepository.ReadById(pictureId);
            photo.Description = description;
            unitOfWork.Save();
            return Content(photo.Description);
        }
    }
}