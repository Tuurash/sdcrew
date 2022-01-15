using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using sdcrew.Views.Login;
using sdcrew.Views.Postflight;
using sdcrew.Views.Preflight;
using sdcrew.Views.Settings;
using sdcrew.Views.Notification;
using sdcrew.Services.Data;
using sdcrew.ViewModels;
using Xamarin.Essentials;
using System.Runtime.CompilerServices;

namespace sdcrew.Views.Dashboard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class AppShell : Shell
    {
        #region Dependencies

        private readonly AppShellViewModel _viewModel;
        UserServices userServices;

        Dictionary<string, Type> routes = new Dictionary<string, Type>();

        public ICommand NavigateCommand => new Command(Navigate);


        public ICommand NavigateNotification => new Command(async () => await PushPage(new notificationPage()));
        public ICommand NavigateSupport => new Command(async () => await PushPage(new supportPage()));

        #endregion

        public AppShell()
        {

            RegisterRoutes();
            InitializeComponent();
            BindingContext = this;
            this.CurrentItem = Tab_Preflight;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await FillUserinfo();
                IsBusy = false;
            });
        }

        public async Task FillUserinfo()
        {
            userServices = new UserServices();

            var user = await userServices.GetUser();

            lblUserName.Text = user.Name;
            lblUserEmail.Text = user.Email;

            ImgUserSrc.Uri = user.ImageUri;

        }

        #region Route_Registration_Navigation

        private void Shell_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e.Current != null)
            {
                var deferral = e.GetDeferral(); // hey shell, wait a moment

                // intercept navigation here and do your custom logic. 
                // continue on to the destination route, cancel it, or reroute as needed

                // e.Cancel(); to stop routing
                // deferral.Complete(); to resume
                if (e.Target.Location.OriginalString.Contains("ShowFlyout"))
                {
                    Shell.Current.FlyoutIsPresented = true;
                    e.Cancel();//don't actually go to a route called back
                    Shell.Current.GoToAsync(".."); // this is the universal "back" in Shell                   
                }
                else if (e.Target.Location.OriginalString.Contains("Header"))
                {
                    e.Cancel();//don't actually go to a route called back
                    Shell.Current.GoToAsync("..");
                }

                deferral.Complete();
            }
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
            routes.Add("postflightDetail", typeof(PostflightDetails));
            routes.Add("RssNotifcation", typeof(RssNotifcationPage));

            foreach (var item in routes)
            {
                Routing.RegisterRoute(item.Key, item.Value);
            }
        }


        #endregion

        #region Footer_Functions
        //Logout
        credentialsViewModel _CredviewModel = new credentialsViewModel();
        public ICommand LogoutCommand => new Command(async () => await _CredviewModel.LogoutAsync());


        void footerPhone_Tapped(System.Object sender, System.EventArgs e)
        {
            PhoneDialer.Open("+1.321.777.3236");
        }

        async void footerMail_Tapped(System.Object sender, System.EventArgs e)
        {
            List<string> rcpnts = new List<string>();
            rcpnts.Add("support@satcomdirect.com");
            try
            {
                var message = new EmailMessage
                {
                    To = rcpnts,
                };
                await Email.ComposeAsync(message);
            }
            catch (Exception) { }
        }
        #endregion

        protected async override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName.Equals("CurrentItem") && Device.RuntimePlatform == Device.Android)
            {
                FlyoutIsPresented = false;
                await Task.Delay(300);
            }
            base.OnPropertyChanged(propertyName);
        }
    }

}

