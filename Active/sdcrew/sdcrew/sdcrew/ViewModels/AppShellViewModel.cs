using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

using sdcrew.ViewModels;
using sdcrew.Views.Login;
using sdcrew.Views.Postflight;
using sdcrew.Views.Preflight;
using sdcrew.Views.Settings;

using System.Threading.Tasks;
using Xamarin.Forms;

using sdcrew.Views;
using sdcrew.Views.Notification;
using sdcrew.Repositories;
using sdcrew.Models;
using sdcrew.Services.Data;
using Xamarin.CommunityToolkit.ObjectModel;

namespace sdcrew.ViewModels
{
    class AppShellViewModel: BaseViewModel
    {
        UserServices _userServices;
        User _user =new User();

        Dictionary<string, Type> routes = new Dictionary<string, Type>();

        public ICommand NavigateCommand => new Command(Navigate);
        public ICommand NavigateNotification => new Command(async () => await PushPage(new notificationPage()));
        public ICommand NavigateSupport => new Command(async () => await PushPage(new supportPage()));



        public AsyncCommand RefreshUserCommand { get; }
        public AsyncCommand GetUserCommand { get; }

        public string CurrentUserName { get; set; }
        public string CurrentUserEmail { get; set; }

        private Task LoadUserData { get; set; }

        public AppShellViewModel()
        {
            _userServices = new UserServices();

            RegisterRoutes();

            LoadUserData = Task.Run(async () =>
            {
                var user=await GetUser();
                CurrentUserName = user.Name;
                CurrentUserEmail = user.Email;
                if(user==null)
                {
                    RedirectToLogin();
                }
            });

            RefreshUserCommand = new AsyncCommand(RefreshUser);
            GetUserCommand = new AsyncCommand(GetUser);
        }

        private void RedirectToLogin()
        {
            App.Current.MainPage = new loginPage();
        }

        async Task<User> GetUser()
        {
            _user = await _userServices.GetUser();
            return _user;
        }

        async Task RefreshUser()
        {
            IsBusy = true;
            var _userData = await _userServices.GetUser();

            _user = _userData;
            IsBusy = false;
        }


        private async Task PushPage(Page page)
        {
            await Shell.Current.Navigation.PushAsync(page);

            string pageName = page.GetType().Name;
            Shell.Current.FlyoutIsPresented = false;
        }

        private Type ShowFlyout()
        {
            Shell.Current.FlyoutIsPresented = false;
            return typeof(CurrentItemChangedEventArgs);
        }

        private async void Navigate(object route)
        {

            await (App.Current.MainPage as Xamarin.Forms.Shell).GoToAsync($"//flyout/tab/{route}", true);

            //ShellNavigationState state = Shell.Current.CurrentState;
            //await Shell.Current.GoToAsync($"{state.Location}");
            Shell.Current.FlyoutIsPresented = false;
        }

        void RegisterRoutes()
        {
            routes.Add("login", typeof(loginPage));
            routes.Add("postflight", typeof(postFlightPage));
            routes.Add("preflight", typeof(preFlightPage));
            routes.Add("settings", typeof(settingsPage));
            routes.Add("support", typeof(supportPage));
            routes.Add("notification", typeof(notificationPage));
            routes.Add("preflightDetail", typeof(PreflightDetails));

            foreach (var item in routes)
            {
                Routing.RegisterRoute(item.Key, item.Value);
            }
        }

        //Logout
        credentialsViewModel _CredviewModel = new credentialsViewModel();
        public ICommand LogoutCommand => new Command(async () => await _CredviewModel.LogoutAsync());
    }

    
}
