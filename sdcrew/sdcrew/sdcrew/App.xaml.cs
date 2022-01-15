using sdcrew.ViewModels;
using sdcrew.Views;
using sdcrew.Views.Dashboard;
using sdcrew.Views.Login;
using sdcrew.Views.Postflight;

using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;



namespace sdcrew
{
    public partial class App : Application
    {
        credentialsViewModel creds;
        int LoggedinFlag = 0;

        SettingsViewModel svm;

        public App()
        {
            InitializeComponent();
            SQLitePCL.Batteries_V2.Init();

            BindingContext = creds = new credentialsViewModel();
            string ExistingAccToken = Services.Settings.StoreAccessToken;

            //App.Current.MainPage = new Test();
            if (String.IsNullOrEmpty(ExistingAccToken))
            {
                MainPage = new NavigationPage(new loginPage());
            }
            else
            {
                LoggedinFlag = 1;
                App.Current.MainPage = new AppShell();
                Task.Delay(20000);

                svm = new SettingsViewModel();
                Task.Run(async () => await svm.PerformScheduledRefresh()).ConfigureAwait(false);
            }

        }

        protected override void OnStart()
        {
            AppCenter.Start("android=d65367a5-b409-4d87-99c1-14983311d1f6;" +
                  "ios=24b9c410-a61c-4fad-b9da-e51e1079cc85;",
                  typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            Services.Settings.FilterItems = null;
        }

        protected override void OnResume()
        {

        }

    }
}
