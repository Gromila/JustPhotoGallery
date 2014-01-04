using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JustPhotoGallery.Domain.Entities
{
    public class User : IdentityUser 
    {
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; private set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        
        // Add avatar

        public String ConfirmationToken { get; set; }

        public bool IsConfirmed { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }

        public virtual ICollection<Picture> Pictures { get; set; }
        
        public User()
        {
            Votes = new List<Vote>();
            Pictures = new List<Picture>();
            RegistrationDate = DateTime.Now;
        }

        public User(String email, DateTime dateOfBirth)
        {
            Votes = new List<Vote>();
            Pictures = new List<Picture>();
            Email = email;
            DateOfBirth = dateOfBirth;
            RegistrationDate = DateTime.Now;
        }

    }
}
