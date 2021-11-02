using sdcrew.Repositories;
using sdcrew.Services.Data;
using sdcrew.ViewModels;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace sdcrew.Views.Dashboard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        private readonly AppShellViewModel _viewModel;
        private UserServices _userServices;

        public AppShell()
        {
            InitializeComponent();
            //NavigationPage.SetHasNavigationBar(this, false);
            this.CurrentItem.CurrentItem = Tab_Preflight;
            BindingContext = _viewModel=new AppShellViewModel();
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            IsBusy = false;
            _userServices = new UserServices();
            try
            {
                await Task.Run(async () => { await _userServices.AddUser(); });
            }
            catch (Exception) { }
        }

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

    }
 
}