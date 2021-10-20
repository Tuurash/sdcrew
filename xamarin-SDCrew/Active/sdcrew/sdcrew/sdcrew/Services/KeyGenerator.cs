using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace sdcrew.Services
{
    public class KeyGenerator
    {
        //Xamarin.Essentials.SecureStorage.SetAsync(CurrentUserKey, strUser);

        private static Random random = new Random();

        public string generated { get; set; }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            string generated = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            char firstCharacter = generated.ToCharArray().ElementAt(0);

            if (!char.IsLetter(firstCharacter))
            {
                RandomString(32);
            }

            return generated;
        }

        public async void CheckKey()
        {
            var MyKey = await SecureStorage.GetAsync("Ckey");

            if (MyKey == null || string.IsNullOrEmpty(MyKey))
            {
                await SecureStorage.SetAsync("Ckey", RandomString(32));
            }
        }


    }
}
