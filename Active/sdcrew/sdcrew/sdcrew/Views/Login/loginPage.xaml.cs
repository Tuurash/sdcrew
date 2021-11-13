using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Rg.Plugins.Popup.Services;
using sdcrew.ViewModels;
using sdcrew.Views.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace sdcrew.Views.Login
{
    public partial class loginPage : ContentPage
    {
        credentialsViewModel viewModel;

        public loginPage()
        {
            InitializeComponent();
            viewModel = new credentialsViewModel();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(1);
            Loader.IsVisible = false;

            if (Services.Settings.StoreAuthidentifierUrl==""  || Services.Settings.StoreAuthidentifierUrl==null)
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
            MainThread.BeginInvokeOnMainThread(() => {
                Loader.IsVisible = true;
            });
            var any = await viewModel.LoginAsync();

            MainThread.BeginInvokeOnMainThread(()=>
            {
                if(any!=null)
                {
                    App.Current.MainPage = new AppShell();
                }
            });
        }
    }
}
