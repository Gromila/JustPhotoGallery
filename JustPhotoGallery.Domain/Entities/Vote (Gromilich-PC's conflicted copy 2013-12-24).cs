using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustPhotoGallery.Domain.Entities
{
    public class Vote : Entity
    {
        public String UserId { get; set; }

        public int PictureId { get; set; }

        public int Value { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreationDate { get; private set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("PictureId")]
        public virtual Picture Picture { get; set; }

        public Vote()
        {
            CreationDate = DateTime.Now;
        }

        public Vote(String userId, int pictureId, int value)
        {
            UserId = userId;
            PictureId = pictureId;
            Value = value;
        }
    }
}
