using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustPhotoGallery.Domain;
using JustPhotoGallery.Domain.Migrations;
using JustPhotoGallery.Domain.Entities;
using JustPhotoGallery.Repositories.Interfaces;

namespace JustPhotoGallery.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private bool disposed = false;

        private readonly PhotoGalleryContext context = new PhotoGalleryContext();

        private IRepository<Picture> pictureRepository;

        private IRepository<Tag> tagRepository;

     //   private IRepository<User> userRepository;

        private IRepository<Vote> voteRepository;

        public IRepository<Picture> PictureRepository
        {
            get { return pictureRepository ?? (pictureRepository = new BaseRepository<Picture>(context)); }
        }

        public IRepository<Tag> TagRepository
        {
            get { return tagRepository ?? (tagRepository = new BaseRepository<Tag>(context)); }
        }

/*        public IRepository<User> UserRepository
        {
            get { return userRepository ?? (userRepository = new BaseRepository<User>(context)); }
        }
        */
        public IRepository<Vote> VoteRepository
        {
            get { return voteRepository ?? (voteRepository = new BaseRepository<Vote>(context)); }
        }

        public UnitOfWork()
        {
           Database.SetInitializer(new MigrateDatabaseToLatestVersion<PhotoGalleryContext, Configuration>());
        }

        public void Save()
        {
            context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
