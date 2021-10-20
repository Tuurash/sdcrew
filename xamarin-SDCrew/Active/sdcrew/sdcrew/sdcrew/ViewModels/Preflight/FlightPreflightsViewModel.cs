using sdcrew.Models;
using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace sdcrew.ViewModels.Preflight
{

    public class FlightPreflightsViewModel : BaseViewModel, INotifyPropertyChanged
    {
        PreflightServices _preflightServices;

        public FlightPreflightsViewModel()
        {
            _preflightServices = new PreflightServices();

            //try
            //{
            //    Task.Run(() =>
            //    {
            //        MessagingCenter.Subscribe<App>((App)Application.Current, "OnFilterSelected", (sender) =>
            //        {
            //            OnPropertyChanged(nameof(preflightAircraftEvents));
            //        });
            //    });
            //}
            //catch (Exception) { }
        }

        //public List<PreflightGroup> preflightAircraftEvents { get => getAllAircraftEventsList(); }

        public ObservableCollection<PreflightGroup> getAllAircraftEventsList()
        {
            string selectedTimezone = Services.Settings.TimeZone;
            DateTime getStartDate = DateTime.MinValue;
            DateTime getEndDate = DateTime.MinValue;
            string getCurrentTimezone = "UTC";
            string getTimezoneIcon = "";
            string getTripID = "";

            PreflightVM _preflightVM = new PreflightVM();

            List<PreflightGroup> _preflightList = new List<PreflightGroup>();

            IEnumerable<PreflightVM> Preflights;
            List<PreflightVM> CustomList = new List<PreflightVM>();

            if (Services.Settings.FilterItems != null && Services.Settings.FilterItems != "")
            {
                string fi = Services.Settings.FilterItems.Remove(0, 1);
                string[] filterItemsArray = fi.Split(',');

                List<string> filteritemsList = new List<string>();

                foreach (var i in filterItemsArray)
                {
                    filteritemsList.Add(i);
                }
                Preflights = _preflightServices.GetFilteredAircraftFlights(filteritemsList);
            }
            else
            {
                Preflights = _preflightServices.GetAllAircraftEventsPreflights();
            }

            foreach (var item in Preflights)
            {
                if (item.tentativeEta == true || item.tentativeEtd == true)
                {
                    getTimezoneIcon = "scheduletentative";
                }
                else { getTimezoneIcon = "scheduleactual"; }

                if (selectedTimezone != null || selectedTimezone == "")
                {
                    if (selectedTimezone == "Local")
                    {
                        getStartDate = item.startDateTimeUtc;
                        getEndDate = item.endDateTimeUtc;
                        getCurrentTimezone = "LOCAL";
                    }
                    else
                    {
                        getStartDate = item.startDateTimeUtc.ToUniversalTime();
                        getEndDate = item.endDateTimeUtc.ToUniversalTime();
                        getCurrentTimezone = "UTC";
                    }
                }
                else
                {
                    getStartDate = item.startDateTimeUtc.ToUniversalTime();
                    getEndDate = item.endDateTimeUtc.ToUniversalTime();
                    getCurrentTimezone = "UTC";
                }

                if (item.legCount != 0)
                {
                    getTripID = "TRIP ID: " + item.scheduledAircraftTripId + "-" + item.legNumber.ToString() + "/" + item.legCount.ToString();
                }
                else { getTripID = ""; }

                var customPreflight = new PreflightVM
                {
                    tailNumber = item.tailNumber,
                    color = item.color,
                    TripID = getTripID,

                    departureAirportIcao = item.departureAirportIcao,
                    arrivalAirportIcao = item.arrivalAirportIcao,

                    TimezoneIcon = getTimezoneIcon,
                    StartTime = getStartDate,
                    EndTime = getEndDate,

                    CurrentTimezone = getCurrentTimezone,

                    crewInitials = item.crewInitials,
                    ete = item.ete,
                    passengerLegCount = item.passengerLegCount,

                    date = item.date
                };

                CustomList.Add(customPreflight);
            }



            var dictionary = CustomList.GroupBy(x => x.date).ToList();

            foreach (var item in dictionary)
            {
                _preflightList.Add(new PreflightGroup(item.Key, item.ToList()));
            }

            var FlightObservableCollection = new ObservableCollection<PreflightGroup>(_preflightList);

            return FlightObservableCollection;
        }

        #region refresh

        //public ICommand RefreshCommand => new Command(async () => await RefreshDataAsync());

        //const int RefreshDuration = 2;

        //async Task RefreshDataAsync()
        //{
        //    IsBusy = true;
        //    await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
        //    OnPropertyChanged(nameof(preflightAircraftEvents));
        //    IsBusy = false;
        //}


        #endregion
    }
}
