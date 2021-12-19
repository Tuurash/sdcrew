using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using sdcrew.Models;
using sdcrew.Services.Data;

namespace sdcrew.ViewModels.Postflight
{
    public class NotLoggedFlightsViewModel: BaseViewModel, INotifyPropertyChanged
    {

        PostflightServices postflightServices; // = new PostflightServices();

        public NotLoggedFlightsViewModel()
        {
            
        }

        //public ObservableCollection<PostFlightVM> NotLogged_Postflights { get => getNotLogged_Postflights(); }

        public ObservableCollection<PostFlightVM> getNotLogged_Postflights()
        {
            postflightServices = new PostflightServices();

            string getEndDate = "";
            string getEndTime = "";

            List<PostFlightVM> getNotLoggedflightList;
            List<PostFlightVM> flightList = new List<PostFlightVM>();

            //filtering Conditions Will go Here
            if (Services.Settings.FilterItems != null && Services.Settings.FilterItems != "")
            {
                string fi = Services.Settings.FilterItems.Remove(0, 1);
                string[] filterItemsArray = fi.Split(',');

                List<string> filteritemsList = new List<string>();

                foreach (var i in filterItemsArray)
                {
                    filteritemsList.Add(i);
                }
                getNotLoggedflightList = postflightServices.GetFilteredPostFlights(filteritemsList, "Not Logged");
            }
            else
            {
                getNotLoggedflightList = postflightServices.GetPostFlightsByType("Not Logged");
            }

            if(getNotLoggedflightList!=null)
            {
                foreach(var flight in getNotLoggedflightList)
                {

                    if (flight.flightStopDateTime.GetValueOrDefault() == DateTime.MinValue.ToUniversalTime())
                    {
                        getEndDate = "N/A";
                        getEndTime = "N/A";
                    }
                    else { getEndDate = flight.flightStopDateTime.GetValueOrDefault().ToString("dd MMM yyyy"); getEndTime=flight.flightStopDateTime.GetValueOrDefault().ToString("hh:mm tt"); }

                    var flightSingle = new PostFlightVM
                    {
                        flightId = flight.flightId,
                        aircraftProfileId = flight.aircraftProfileId,
                        postedFlightId = flight.postedFlightId,
                        departureIcao = flight.departureIcao,

                        airportIcao = flight.airportIcao,                  //Arrival ICAO

                        departureAirportId = flight.departureAirportId,
                        arrivalAirportId = flight.arrivalAirportId,

                        postedAircraftTripId = flight.postedAircraftTripId,


                        //Differentiate between Logged and not logged
                        postFlightStatusName = flight.postFlightStatusName,

                        postedFlightStatusTypeId = flight.postedFlightStatusTypeId,
                        roundingThreshold = flight.roundingThreshold,


                        //flightStartDateTime = flight.flightStartDateTime.ToUniversalTime(), //Start Time

                        isUserSigned = flight.isUserSigned,
                        passengerLegCount = flight.passengerLegCount,

                        postedFlightAdditional = flight.postedFlightAdditional,

                        crewInitials = flight.crewInitials,             //remove if not necessary

                        //flightStopDateTime = flight.flightStopDateTime.GetValueOrDefault().ToUniversalTime(), //End Tijme
                        scheduledFlightId = flight.scheduledFlightId ?? 0,
                        departureFboId = flight.departureFboId ?? 0,

                        arrivalFboId = flight.arrivalFboId ?? 0,
                        arrivalFboName = flight.arrivalFboName,
                        departureFboName = flight.departureFboName,


                        aircraftColor = flight.aircraftColor,

                        StartTime = flight.flightStartDateTime.ToString("hh:mm tt"),
                        EndTime= getEndTime,

                        StartDate= flight.flightStartDateTime.ToString("dd MMM yyyy"),
                        EndDate = getEndDate,

                        TailNumber=flight.TailNumber,

                        Customized_TripId = flight.customTripId + "-" + flight.legNumber + "/" + flight.legCount,
                    };

                    flightList.Add(flightSingle);
                }
            }

            var FlightObservableCollection = new ObservableCollection<PostFlightVM>(flightList);
            return FlightObservableCollection;
        }

    }



}
