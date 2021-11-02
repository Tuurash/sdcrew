using sdcrew.Models;
using sdcrew.Repositories.PreflightRepos;
using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace sdcrew.ViewModels
{
    public class SettingsViewModel:BaseViewModel
    {

        UserServices _userServices;
        User _user=new User();

        private Task LoadUserData { get; set; }

        public AsyncCommand GetUserCommand { get; }

        public User CurrentUser { get; set; }

        public SettingsViewModel()
        {
            _userServices = new UserServices();

            Title = "Settings";

            LoadUserData = Task.Run(async () =>
            {
                CurrentUser = await GetUser();
            });
        }

        async Task<User> GetUser()
        {       
            var userData = await _userServices.GetUser();
            return userData;
        }
    }
}
