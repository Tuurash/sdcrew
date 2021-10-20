using sdcrew.Models;
using sdcrew.Services.Data;
using sdcrew.ViewModels.Preflight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class EventPreflights : ContentView
    {
        EventPreflightsViewModel viewModel;


        private ObservableCollection<PreflightGroup> _allGroups;
        private ObservableCollection<PreflightGroup> _expandedGroups;


        public EventPreflights()
        {
            InitializeComponent();
            BindingContext = viewModel = new EventPreflightsViewModel();

            PopulateDate();

            #region MessagingCenter
            try
            {
                Task.Run(() =>
                {
                    MessagingCenter.Subscribe<string, string>("MyApp", "DBUpdated", (sender, arg) =>
                    {
                        PopulateDate();
                    });

                    MessagingCenter.Subscribe<string, string>("MyApp", "TimeZoneChanged", (sender, arg) =>
                    {
                        PopulateDate();
                    });

                });
            }
            catch (Exception) { }
            #endregion
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            int selectedIndex = _expandedGroups.IndexOf(((PreflightGroup)((ImageButton)sender).CommandParameter));
            _allGroups[selectedIndex].Expanded = !_allGroups[selectedIndex].Expanded;
            try
            {
                UpdateListContent();
            }
            catch (Exception exc) { }

        }

        private void PopulateDate()
        {
            GroupedView.IsRefreshing = true;
            _allGroups = viewModel.getAllStaffeventsList();
            UpdateListContent();
            GroupedView.IsRefreshing = false;
        }

        private void UpdateListContent()
        {
            _expandedGroups = new ObservableCollection<PreflightGroup>();

            foreach (PreflightGroup group in _allGroups)
            {
                //Create new FoodGroups so we do not alter original list
                PreflightGroup newGroup = new PreflightGroup(group.Date, group.Expanded);

                if (group.Expanded)
                {
                    foreach (PreflightVM flight in group)
                    {
                        newGroup.Add(flight);
                    }
                }
                _expandedGroups.Add(newGroup);
            }

            GroupedView.ItemsSource = _expandedGroups;
        }

        #region Refresh
        const int RefreshDuration = 2;

        private async void GroupedView_Refreshing(object sender, EventArgs e)
        {
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));

            PopulateDate();

            GroupedView.EndRefresh();

        }

        #endregion
    }
}