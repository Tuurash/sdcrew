using sdcrew.Models;
using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace sdcrew.ViewModels.Preflight
{



    public class AllPreFlightsViewModel : BaseViewModel, INotifyPropertyChanged
    {
        PreflightServices _preflightServices;

        public AllPreFlightsViewModel()
        {
            _preflightServices = new PreflightServices();

        }

        public ObservableCollection<PreflightGroup> getAllPreflightsList()
        {
            PreflightNote note = new PreflightNote();

            string selectedTimezone = Services.Settings.TimeZone;
            DateTime getStartDate = DateTime.MinValue;
            DateTime getEndDate = DateTime.MinValue;
            string getCurrentTimezone = "UTC";
            string getTimezoneIcon = "";
            string getTripID = "";
            string getCardheaderIcon = "";

            int getCalendarNoteId = 0;
            DateTime getNoteDate = DateTime.MinValue;
            string getNoteBody = "";
            string getNoteIsVisible = "false";
            string getNoteIsEnable = "false";

            string getETE = "";
            string getEventType = "";
            string getEventTypeIsVisible = "false";
            string getFlightIconIsVisible = "false";
            string getHeaderTextBg = "";
            string getHeaderTextColor = "";
            string getEcrewPaxVisibility = "";

            Console.Write(getNoteDate.ToShortDateString());

            bool flag_Note = true;

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
                Preflights = _preflightServices.GetFilteredFlights(filteritemsList);
            }
            else
            {
                Preflights = _preflightServices.GetAllPreflights();
            }

            foreach (var item in Preflights)
            {
                if (getNoteDate.Date != item.date.Date)
                {
                    flag_Note = true;
                }


                var getNote = _preflightServices.GetNoteByDate(item.date);

                if (getNote != null & flag_Note == true)
                {
                    getCalendarNoteId = getNote.calendarNoteId;
                    getNoteBody = getNote.note;
                    getNoteDate = getNote.calendarDate;
                    getNoteIsVisible = "true";
                    getNoteIsEnable = "true";

                    flag_Note = false;

                }
                else
                {
                    getNoteIsVisible = "false";
                    getNoteIsEnable = "false";
                }

                if (item.tentativeEta == true || item.tentativeEtd == true)
                {
                    getTimezoneIcon = "scheduletentative";
                }
                else { getTimezoneIcon = "scheduleactual"; }

                if (selectedTimezone != null || selectedTimezone == "")
                {
                    if (selectedTimezone == "AIRPORT")
                    {


                        getStartDate = item.startDateTimeLocal;
                        getEndDate = item.endDateTimeLocal;
                        getCurrentTimezone = "AIRPORT";
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
                    getTripID = "TRIP ID: " + item.customTripId + " - " + item.legNumber.ToString() + "/" + item.legCount.ToString();
                }
                else { getTripID = ""; }

                if (item.ete != null & item.ete != "")
                {
                    getETE = "ETE - " + item.ete;
                }
                else { getETE = ""; }

                //Dinamic Font Resource => https://stackoverflow.com/questions/52274361/xamarin-fontawesome-not-working-from-code-behind
                // &#xf11a; => \uf11a
                if (item.eventName == "Aircraft")
                {
                    getEcrewPaxVisibility = "true";

                    getCardheaderIcon = "\uE805";
                    getFlightIconIsVisible = "true";
                    getEventTypeIsVisible = "false";
                    getEventType = "";

                    getHeaderTextBg = "#0000ffff";
                    getHeaderTextColor = item.color;
                }
                else
                {
                    getEcrewPaxVisibility = "false";

                    getFlightIconIsVisible = "false";
                    getEventTypeIsVisible = "true";

                    if (item.staffEventType.Contains("Maintenance"))
                    {
                        getCardheaderIcon = "\uE80A";
                        getEventType = "Maintenance";


                        getHeaderTextBg = Color.Transparent.ToString();
                        getHeaderTextColor = item.color;
                    }
                    else if (item.staffEventType.Contains("Hold"))
                    {
                        getCardheaderIcon = "\uE80B";
                        getEventType = "Hold";

                        getHeaderTextBg = Color.Transparent.ToString();
                        getHeaderTextColor = item.color;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(item.abbreviation))
                        {

                            //else if (item.staffEventType.Contains("Custom") || item.staffEventType.Contains("AOG"))

                            getCardheaderIcon = "\uE809";
                            getEventType = item.staffEventType;

                            getHeaderTextBg = Color.Transparent.ToString();
                            getHeaderTextColor = item.color;
                        }
                        else
                        {
                            getCardheaderIcon = item.abbreviation;
                            getEventType = item.staffEventType;
                            getHeaderTextBg = item.color;
                            getHeaderTextColor = "#FFFFFF";
                        }


                    }
                    
                    
                }



                var customPreflight = new PreflightVM
                {
                    EcrewPaxVisibility = getEcrewPaxVisibility,

                    aircraftId = item.aircraftId,
                    CardheaderIcon = getCardheaderIcon,
                    tailNumber = item.tailNumber,
                    color = item.color,
                    TripID = getTripID,
                    legNumber = item.legNumber,
                    flightId = item.scheduledFlightId,
                    scheduledFlightId = item.scheduledFlightId,
                    FID = item.FID,
                    scheduledAircraftTripId = item.scheduledAircraftTripId,

                    //fboInfo
                    DeparturefboHandlerName = item.DeparturefboHandlerName,
                    ArrvailfboHandlerName = item.ArrvailfboHandlerName,
                    DeparturelocalPhone = item.DeparturelocalPhone,
                    ArrivallocalPhone = item.ArrivallocalPhone,
                    DepartureserviceEmailAddress = item.DepartureserviceEmailAddress,
                    ArrivalserviceEmailAddress = item.ArrivalserviceEmailAddress,

                    departureAirportIcao = item.departureAirportIcao,
                    arrivalAirportIcao = item.arrivalAirportIcao,

                    TimezoneIcon = getTimezoneIcon,
                    StartTime = getStartDate,
                    EndTime = getEndDate,

                    CurrentTimezone = getCurrentTimezone,

                    crewInitials = item.crewInitials,
                    ete = getETE,
                    passengerLegCount = item.passengerLegCount,

                    date = item.date,

                    eventName = item.eventName,
                    EventType = getEventType,
                    EventTypeIsVisible = getEventTypeIsVisible,
                    FlightIconIsVisible = getFlightIconIsVisible,

                    HeadeTextBGColor = getHeaderTextBg,
                    HeadeTextColor = getHeaderTextColor,

                    //Note
                    CalendarNoteId = getCalendarNoteId,
                    NoteBody = getNoteBody,
                    NoteDate = getNoteDate,
                    NoteIsVisible = getNoteIsVisible,
                    NoteIsEnable = getNoteIsEnable,


                    //event notes
                    notes = item.notes
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

        public ICommand RefreshCommand => new Command(async () => await RefreshDataAsync());

        const int RefreshDuration = 2;
        async Task RefreshDataAsync()
        {
            IsBusy = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));

            IsBusy = false;
        }

        #endregion

    }
}
