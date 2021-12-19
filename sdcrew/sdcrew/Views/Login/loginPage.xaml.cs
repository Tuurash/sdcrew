using sdcrew.Services;
using sdcrew.ViewModels;
using sdcrew.Views.Dashboard;

using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Login
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class loginPage : ContentPage
    {
        credentialsViewModel viewModel;

        public loginPage()
        {
            InitializeComponent();
            viewModel = new credentialsViewModel();

            if (!string.IsNullOrEmpty(Services.Settings.UpdateNotification) & Services.Settings.UpdateNotification != "False")
            {
                Task.Run(async () => await CheckVersion());
            }
        }

        private async Task CheckVersion()
        {
            VersionTracker versionTracker = new VersionTracker();

            var Islatest = await versionTracker.CheckForUpdate();
            if (Islatest != 0)
            {
                var update = await DisplayAlert("New Version", "There is a new version of this app available. Would you like to update now?", "Yes", "No");

                if (update)
                {
                    await versionTracker.OpenAppInStore();
                }
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(1);
            Loader.IsVisible = false;

            if (Services.Settings.StoreAuthidentifierUrl == "" || Services.Settings.StoreAuthidentifierUrl == null)
            {
                Services.Settings.StoreAuthidentifierUrl = @"https://identity.satcomdirect.com";
            }
        }

        private async void ImageTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Switch to:", "Cancel", null, "Production Mode", "Dev Mode", "Test Mode");

            if (action == "Dev Mode")
            {
                Services.Settings.StoreAuthidentifierUrl = @"https://identity.satcomdev.com";
            }
            else if (action == "Test Mode")
            {
                Services.Settings.StoreAuthidentifierUrl = @"https://identity.satcomtest.com";
            }
            else
            {
                Services.Settings.StoreAuthidentifierUrl = @"https://identity.satcomdirect.com";
            }
        }

        private async void FaceButton_Clicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Loader.IsVisible = true;
            });
            credentialsViewModel.User? any = await viewModel.LoginAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (any != null)
                {
                    App.Current.MainPage = new AppShell();
                }
            });
        }
    }
}
