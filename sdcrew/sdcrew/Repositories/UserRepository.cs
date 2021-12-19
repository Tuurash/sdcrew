using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using sdcrew.Models;
using Xamarin.Essentials;
using sdcrew.Services;
using System.Net.Http.Headers;
using System.Net.Http;

namespace sdcrew.Repositories
{
    public class UserRepository
    {
        private readonly IRequestsService _requestService;

        User _user = new User();

        public UserRepository()
        {
            _requestService = new RequestsService();
        }

        public User getUser()
        {
            if (Settings.GeneralSettings != "")
            {
                string userFromStorage = Settings.GeneralSettings;
                _user = JsonConvert.DeserializeObject<User>(userFromStorage);

            }

            return _user;
        }

        public async Task<Document> GetDocumentAsync()
        {
            getUser();

            await Task.Delay(0);

            string url = "https://sd-profile-api.satcomdirect.com/preflight/api/Images/" + _user.CustomerId + "/9";

            return _requestService.GetAsync<Document>(url).Result;
        }

        public string GetImageAsync()
        {
            string url = "https://sd-profile-api.satcomdirect.com/preflight/api/FileStorage/" + GetDocumentAsync().Result.fileId;

            var _image = _requestService.GetAsync<Image>(url);

            return _image.Result.fileUri;
        }

    }
}
