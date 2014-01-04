using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JustPhotoGallery.Domain;
using JustPhotoGallery.Domain.Entities;
using JustPhotoGallery.Repositories.Interfaces;

    namespace JustPhotoGallery.Repositories
    {
        public class UserRepository
        {

            private PhotoGalleryContext context;

            private DbSet<User> dbSet;

            public UserRepository(PhotoGalleryContext context)
            {
                this.context = context;
                dbSet = context.Set<User>();
            }

            public void Create(User entity)
            {
                dbSet.Add(entity);
            }

            public IEnumerable<User> Read(Expression<Func<User, bool>> filter = null, Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null, string includeProperties ="")
            {
                IQueryable<User> query = dbSet;

                if (filter != null)
                    query = query.Where(filter);

                query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

                if (orderBy != null)
                    return orderBy(query).ToList();

                return query;
            }

            public User ReadById(object id)
            {
                return dbSet.Find(id);
            }

            public void Update(User entity)
            {
                dbSet.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
            }

            public void Delete(object id)
            {
                User entity = dbSet.Find(id);
                Delete(entity);
            }

            public void Delete(User entity)
            {
                if (context.Entry(entity).State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }
                dbSet.Remove(entity);
            }
        
        }
    }
