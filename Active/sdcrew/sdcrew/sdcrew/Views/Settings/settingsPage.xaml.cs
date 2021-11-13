using sdcrew.Models;
using sdcrew.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using sdcrew.Services;

namespace sdcrew.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class settingsPage : ContentPage
    {

        UserRepository _userRepo = new UserRepository();
        public settingsPage()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            PopulateRefreshTimePicker();
            PopulateThemePicker();
            PopulatePastEventPicker();
            PopulateFutureEventPicker();
            PopulatePastFlightPicker();

            base.OnAppearing();
        }

        private User GetUserInfo()
        {
            return _userRepo.getUser();
        }

        public void PopulateRefreshTimePicker()
        {
            var pickerList = new List<string>();
            pickerList.Add("1 Minuite");
            pickerList.Add("5 Minuite");
            pickerList.Add("10 Minuite");
            pickerList.Add("15 Minuite");

            RefreshTimePicker.ItemsSource = pickerList;
            RefreshTimePicker.SelectedIndex = 0;
        }

        public void PopulateThemePicker()
        {
            var pickerList = new List<string>();
            pickerList.Add("SD Pro");
            pickerList.Add("Light");
            pickerList.Add("Dark");

            ThemePicker.ItemsSource = pickerList;
            ThemePicker.SelectedIndex = 0;
        }

        public void PopulatePastEventPicker()
        {
            var pickerList = new List<string>();
            pickerList.Add("Present");
            pickerList.Add("Something");


            PastEventPicker.ItemsSource = pickerList;
            PastEventPicker.SelectedIndex = 0;
        }

        public void PopulateFutureEventPicker()
        {
            var pickerList = new List<string>();

            pickerList.Add("-15 Days");
            pickerList.Add("-10 Days");
            pickerList.Add("-5  Days");
            pickerList.Add("-1  Days");

            pickerList.Add("1  Days");
            pickerList.Add("5  Days");
            pickerList.Add("10 Days");
            pickerList.Add("15 Days");


            FutureEventPicker.ItemsSource = pickerList;
            FutureEventPicker.SelectedIndex = 0;
        }

        public void PopulatePastFlightPicker()
        {
            var pickerList = new List<string>();

            pickerList.Add("-15 Days");
            pickerList.Add("-10 Days");
            pickerList.Add("-5  Days");
            pickerList.Add("-1  Days");

            pickerList.Add("1  Days");
            pickerList.Add("5  Days");
            pickerList.Add("10 Days");
            pickerList.Add("15 Days");


            PastFlightPicker.ItemsSource = pickerList;
            PastFlightPicker.SelectedIndex = 0;
        }

    }
}