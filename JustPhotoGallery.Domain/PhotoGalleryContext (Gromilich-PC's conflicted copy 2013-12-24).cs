using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustPhotoGallery.Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JustPhotoGallery.Domain
{
    public class PhotoGalleryContext : IdentityDbContext<User>
    {
        public PhotoGalleryContext () : base ("DefaultConnection")
        { }

        public DbSet<Picture> Pictures { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Vote> Votes { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
