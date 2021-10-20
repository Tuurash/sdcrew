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
using Xamarin.Essentials;
using Xamarin.Forms;

namespace sdcrew.ViewModels.Preflight
{
    public class EventPreflightsViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private PreflightServices _preflightServices;

        public EventPreflightsViewModel()
        {
            _preflightServices = new PreflightServices();
            //RefreshCommand = new Command(() => OnRefresh());
        }

        //public List<PreflightGroup> preflightStaffEvents { get => getAllStaffeventsList(); }

        public ObservableCollection<PreflightGroup> getAllStaffeventsList()
        {
            string selectedTimezone = Services.Settings.TimeZone;
            DateTime getStartDate = DateTime.MinValue;
            DateTime getEndDate = DateTime.MinValue;
            string getCurrentTimezone = "UTC";
            string getTimezoneIcon = "";
            string getTripID = "";
            string getCardheaderIcon = "";

            PreflightVM _preflightVM = new PreflightVM();

            List<PreflightGroup> _preflightList = new List<PreflightGroup>();

            IEnumerable<PreflightVM> PreflightSEs;
            List<PreflightVM> CustomList = new List<PreflightVM>();

            PreflightSEs = _preflightServices.GetAllStaffPreflights();

            foreach (var item in PreflightSEs)
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

                if (item.staffEventType.Contains("Maintenance"))
                {
                    getCardheaderIcon = "\uE80A";
                }
                else if (item.staffEventType.Contains("Hold"))
                {
                    getCardheaderIcon = "\uE80B";
                }
                else if (item.staffEventType.Contains("Custom"))
                {
                    getCardheaderIcon = "\uE809";
                }
                else
                {
                    getCardheaderIcon = item.abbreviation;
                }

                var customPreflight = new PreflightVM
                {
                    CardheaderIcon = getCardheaderIcon,
                    tailNumber = item.tailNumber,
                    color = item.color,

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

        #region Refresh


        //public ICommand RefreshCommand => new Command(async () => await RefreshDataAsync());

        //const int RefreshDuration = 2;

        //async Task RefreshDataAsync()
        //{
        //    IsBusy = true;
        //    await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
        //    OnPropertyChanged(nameof(preflightStaffEvents));
        //    IsBusy = false;
        //}


        #endregion

    }
}
