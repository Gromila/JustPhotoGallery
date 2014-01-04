using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustPhotoGallery.Domain.Entities;
using JustPhotoGallery.Repositories;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main (string[] args)
        {
            using (var uow = new UnitOfWork())
            {
                var user = new User("Marusya", "mashka@mail.ru", "PASS :D ", DateTime.Parse("27.04.1983"));
                uow.UserRepository.Create(user);
                uow.Save();
                user.Password = "new pass";
                uow.UserRepository.Update(user);
                uow.Save();
                Console.WriteLine((uow.UserRepository.Read(a => a.Login == "Marusya").ToList()[0]).ToString());
                uow.UserRepository.Delete(2);
                uow.Save();
            }
        }
    }
}
