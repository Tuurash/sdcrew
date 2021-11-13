using Newtonsoft.Json;
using sdcrew.Models;
using sdcrew.Repositories;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using sdcrew.Services;

namespace sdcrew.Services.Data
{
    public class UserServices
    {
        KeyGenerator _keygen = new KeyGenerator();
       
        UserRepository _userRepo;
        static SQLiteConnection db;

        public static string GetKey { get; private set; }


        public UserServices()
        {
            _userRepo = new UserRepository();
            _keygen.CheckKey();
        }

        private async Task Init()
        {
           db= await DB_Init.Init();
        }

        public async Task AddUser(User _user)
        {
            await Init();

            if (Settings.GeneralSettings != "")
            {
                string userFromStorage = Settings.GeneralSettings;
                _user = JsonConvert.DeserializeObject<User>(userFromStorage);

                string imgUri= _userRepo.GetImageAsync();

                _user = new User
                {
                    IdentityToken = _user.IdentityToken,
                    AccessToken = _user.AccessToken,
                    AccessTokenGoodUntilUTC = _user.AccessTokenGoodUntilUTC,
                    RefreshToken = _user.RefreshToken,
                    Name = _user.Name,
                    Email = _user.Email,
                    AccountId = _user.AccountId,
                    CustomerId = _user.CustomerId,
                    LoginTime = DateTime.Now,
                    ImageUri = imgUri
                };

                db.Insert(_user);
            }

        }

        public async Task<User> GetUser()
        {
            await Init();

            var user = db.Table<User>().LastOrDefault();
            return user;
        }

        public async Task RemoveUser(int id)
        {
            await Init();
            db.Delete<User>(id);

        }

        public async Task AddUser()
        {
            User _user = new User();

            await Init();

            if (Settings.GeneralSettings != "")
            {
                string userFromStorage = Settings.GeneralSettings;
                _user = JsonConvert.DeserializeObject<User>(userFromStorage);

                string imgUri = _userRepo.GetImageAsync();

                var user = new User
                {
                    IdentityToken = _user.IdentityToken,
                    AccessToken = _user.AccessToken,
                    AccessTokenGoodUntilUTC = _user.AccessTokenGoodUntilUTC,
                    RefreshToken = _user.RefreshToken,
                    Name = _user.Name,
                    Email = _user.Email,
                    AccountId = _user.AccountId,
                    CustomerId = _user.CustomerId,
                    LoginTime = DateTime.Now,
                    ImageUri = imgUri
                };

                try
                {
                    db.Insert(user);
                }
                catch(Exception exc) { Console.WriteLine(exc); }

                Console.WriteLine("Added User: " + user.UserID);

                Settings.GetUserMail = user.Email;

            }
        }

    }
}
