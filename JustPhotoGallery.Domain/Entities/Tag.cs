using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustPhotoGallery.Domain.Entities
{
    public class Tag : Entity
    {
        public String Content { get; set; }
        
        public virtual ICollection<Picture> Pictures { get; set; }

        public Tag()
        {
            Pictures = new List<Picture>();
        }
        
        public Tag(String content)
        {
            Pictures = new List<Picture>();
            Content = content;
        }
    }
}
