using sdcrew.ViewModels;
using sdcrew.Views;
using sdcrew.Views.Dashboard;
using sdcrew.Views.Login;
using sdcrew.Views.Preflight;
using sdcrew.Views.ViewHelpers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew
{
    public partial class App : Application
    {
        credentialsViewModel creds;

        public App()
        {
            InitializeComponent();
            SQLitePCL.Batteries_V2.Init();

            BindingContext = creds = new credentialsViewModel();

            string ExistingAccToken =Services.Settings.StoreAccessToken;

            if(String.IsNullOrEmpty(ExistingAccToken))
            {
                App.Current.MainPage = new loginPage();
            }
            else
            {
                App.Current.MainPage = new AppShell();
            }
            //MainPage = new NavigationPage(new loginPage());
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
            Services.Settings.FilterItems = null;
        }

        protected override void OnResume()
        {
            Services.Settings.FilterItems = null;
        }
    }
}
