using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustPhotoGallery.Domain.Entities;

namespace JustPhotoGallery.Web.Models
{
    public class PhotoViewModel
    {
        //TODO: Filters!!!
        public Picture Picture { get; set; }

        public String FilePath
        {
            get
            {
                return "/Upload/Images/" + FileName;
            }
        }

        public String FileName
        {
            get { return Picture.FileName; }
        }

        public int VotesValue
        {
            get
            {
                return Picture.Votes.Sum(vote => vote.Value);
            }
        }

        public String Username
        {
            get { return Picture.User.UserName; }
        }
    }
}