using sdcrew.ViewModels;
using sdcrew.Views;
using sdcrew.Views.Dashboard;
using sdcrew.Views.Login;
using sdcrew.Views.Postflight.SubViews;
using sdcrew.Views.Preflight;
using sdcrew.Views.ViewHelpers;

using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;



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

            //App.Current.MainPage = new TestPage();
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
