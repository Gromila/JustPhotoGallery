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

        public String ThumbFilePath
        {
            get
            {
                return Picture.Filter == FilterType.None ? String.Format("/Upload/Images/thumb_{0}", FileName) : String.Format("/Upload/Images/thumb_filtered_{0}.png", FileName);
            }
        }
        
        public String FilePath
        {
            get
            {
                return Picture.Filter == FilterType.None ? String.Format("/Upload/Images/{0}", FileName) : String.Format("/Upload/Images/filtered_{0}", FileName);
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