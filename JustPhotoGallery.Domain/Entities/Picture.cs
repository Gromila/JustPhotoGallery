using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustPhotoGallery.Domain.Entities
{
    public class Picture : Entity
    {
        public String FileName { get; set; }

        [Required]
        public String Title { get; set; }

        [Required]
        public String Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime AddictionDate { get; private set; }

        public String UserId { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
        
        public virtual ICollection<Tag> Tags { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public FilterType Filter { get; set; }

        public Picture()
        {
            Tags = new List<Tag>();
            Votes = new List<Vote>();
            AddictionDate = DateTime.Now;
        }

        public Picture(String fileName, String title, String description, String userId, FilterType filterType = FilterType.None)
        {
            Title = title;
            Tags = new List<Tag>();
            Votes = new List<Vote>();
            FileName = fileName;
            Description = description;
            UserId = userId;
            Filter = filterType;
            AddictionDate = DateTime.Now;
        }
    }
}
