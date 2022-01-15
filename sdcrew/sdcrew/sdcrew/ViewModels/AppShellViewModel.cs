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
    class AppShellViewModel : BaseViewModel
    {


        public AppShellViewModel()
        {
            RegisterRoutes();
        }

        Dictionary<string, Type> routes = new Dictionary<string, Type>();

        public ICommand NavigateCommand => new Command(Navigate);
        public ICommand NavigateNotification => new Command(async () => await PushPage(new notificationPage()));
        public ICommand NavigateSupport => new Command(async () => await PushPage(new supportPage()));


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

        //TabItems preflight,postflight
        private async void Navigate(object route)
        {

            var ShellTab = route.ToString().Split('/')[0];
            var ActiveTab = route.ToString().Split('/')[1];

            if (ShellTab == "Preflight")
            {
                MessagingCenter.Send(ActiveTab, "SelectedTabFromFlyout");
            }
            else if (ShellTab == "PostFlight")
            {
                MessagingCenter.Send(ActiveTab, "SelectedPostTab");
            }

            await (App.Current.MainPage as Xamarin.Forms.Shell).GoToAsync($"//flyout/tab/{ShellTab}", true);
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
