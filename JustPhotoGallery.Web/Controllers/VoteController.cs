using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using JustPhotoGallery.Domain.Entities;
using JustPhotoGallery.Repositories;
using Microsoft.AspNet.Identity;

namespace JustPhotoGallery.Web.Controllers
{
    public class VoteController : Controller
    {
        private UnitOfWork unitOfWork;

        public VoteController()
        {
            unitOfWork = new UnitOfWork();
        }

        public ActionResult VoteDown(int id)
        {
            var photo = unitOfWork.PictureRepository.ReadById(id);
            if (Request.IsAuthenticated)
            {
                AddVote(photo, -1);
            }
            return Json(CountVotes(photo), JsonRequestBehavior.AllowGet);
        }

        public ActionResult VoteUp(int id)
        {
            var photo = unitOfWork.PictureRepository.ReadById(id);
            if (Request.IsAuthenticated)
            {
                AddVote(photo, 1);
            }
            return Json(CountVotes(photo), JsonRequestBehavior.AllowGet);
        }

        private void AddVote(Picture photo, int value)
        {
            if (CheckVote(photo))
            {
                photo.Votes.Add(new Vote(User.Identity.GetUserId(), photo.Id, value));
                unitOfWork.Save();
            }
        }

        private bool CheckVote(Picture photo)
        {
            return photo.UserId != User.Identity.GetUserId() && photo.Votes.All(vote => vote.UserId != User.Identity.GetUserId());
        }

        private int CountVotes(Picture photo)
        {
            return photo.Votes.Sum(vote => vote.Value);
        }

        public ActionResult GetVotesCount(String userId)
        {
            return Content(unitOfWork.UserRepository.ReadById(userId).Votes.Count.ToString());
        }

        public ActionResult ShowLastVotes(String userId)
        {
            var votes = unitOfWork.UserRepository.ReadById(userId).Votes.Distinct().OrderByDescending(a => a.CreationDate).Take(10);
            return PartialView("Shared/_LastVotesPartial", votes);
        }
	}
}