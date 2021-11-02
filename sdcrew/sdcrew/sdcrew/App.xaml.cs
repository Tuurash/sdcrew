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

            if(Services.Settings.AccessTokenExpiry!=null & Services.Settings.AccessTokenExpiry!="")
            {
                string arg = Services.Settings.AccessTokenExpiry;

                var goodUntil = DateTime.Parse(Services.Settings.AccessTokenExpiry);

                var remainingMinuites = (int)Math.Round((goodUntil - DateTime.Now.ToUniversalTime()).TotalMinutes);

                if(remainingMinuites>1)
                {
                    App.Current.MainPage = new AppShell();
                }
                else
                {
                    App.Current.MainPage = new loginPage();
                }
            }
            else
            {
                App.Current.MainPage = new loginPage();
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
