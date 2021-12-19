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
    public class SettingsViewModel : BaseViewModel
    {

        UserServices _userServices;

        credentialsViewModel credentials;

        User _user = new User();

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

        public async Task PerformScheduledRefresh()
        {
            int Refreshtimer = 30;

            if(!String.IsNullOrEmpty(Services.Settings.Refreshtimer))
            {
                try
                {
                    Refreshtimer = int.Parse(Services.Settings.Refreshtimer.Replace("Minuite","").Trim());
                }
                catch(Exception)
                {
                    Refreshtimer = 30;
                }
            }else
            {
                Refreshtimer = 30;
            }

            await Task.Delay(1);
            Device.StartTimer(TimeSpan.FromMinutes(Refreshtimer), () => //Will start after 20 min
            {
                Task.Run(async () =>
                {
                   await Refreshing();
                });

                return true; // To repeat timer,always return true.
            });
        }

        public async Task Refreshing()
        {
            credentials = new credentialsViewModel();

            int tokenFlag = await credentials.PerformRefreshToken();
            if (tokenFlag == 1)
            {
                MessagingCenter.Send("DummyVal", "PreflightDateUpdated");
                await Task.Delay(1);
                MessagingCenter.Send("DummyVal", "PostflightUpdated");
            }
        }
    }
}
