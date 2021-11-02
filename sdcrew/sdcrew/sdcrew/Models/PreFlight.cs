using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace sdcrew.Models
{

    public class PreFlight
    {
        //From AircraftEvents
        [PrimaryKey, AutoIncrement]
        public int PreflightID { get; set; }

        public int customerId { get; set; }
        public int aircraftId { get; set; }

        [OneToMany]
        public IEnumerable<Leg> legs { get; set; }

        [OneToMany]
        public IEnumerable<object> trips { get; set; }

        [OneToMany]
        public IEnumerable<AircraftEvent> aircraftEvents { get; set; }
        public DateTime date { get; set; }
        public int day { get; set; }
        public string dayOfTheWeek { get; set; }
        public bool isToday { get; set; }
        public bool isWeekend { get; set; }
        public bool postFlightOverridden { get; set; }
        public int lastKnownAirportId { get; set; }
        public string lastKnownAirport { get; set; }


        //From StaffEvents
        [ManyToOne]
        public Airport airport { get; set; }

        [OneToMany]
        public IEnumerable<MobileStaffEventBusinessPersonsDto> mobileStaffEventBusinessPersonsDto { get; set; }
        public DateTime startDateTimeUtc { get; set; }
        public DateTime endDateTimeUtc { get; set; }
        public int staffEventId { get; set; }
        public int staffEventTypeId { get; set; }
        public string staffEventType { get; set; }

    }

    #region FromStaffevents

    public class Airport
    {
        public string airportName { get; set; }
        public int airportId { get; set; }
        public bool isCurrent { get; set; }
        public string airportCode { get; set; }
        public int customerId { get; set; }
        public int uvwAirportId { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class MobileStaffEventBusinessPersonsDto
    {
        public int staffEventBusinessPersonId { get; set; }
        public int businessPersonId { get; set; }
        public int customerId { get; set; }
        public int personId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int maximumDutyHours { get; set; }
        public string phoneNumber { get; set; }
        public string phoneCode { get; set; }
    }

    #endregion

    #region FromAircraftEvents

    public class ArrivalFbo
    {

        [PrimaryKey, AutoIncrement]
        public int ArrivalFboID { get; set; }

        public int fboId { get; set; }
        public string landingFacilityIdentifier { get; set; }
        public string fboHandlerName { get; set; }
        public string addressLine1 { get; set; }
        public string localPhone { get; set; }
        public string localPhoneCode { get; set; }
        public bool isCurrent { get; set; }
        public bool isCustomerFbo { get; set; }
        public bool isPreferredFbo { get; set; }
        public string serviceEmailAddress { get; set; }
        public string notes { get; set; }

       
    }

    public class FboBasicInfo
    {
        [PrimaryKey, AutoIncrement]
        [Column("FboID")]
        public int? FboInfoID { get; set; }
        public string DeparturefboHandlerName { get; set; }
        public string ArrvailfboHandlerName { get; set; }
        public string DepartureICAO { get; set; }
        public string ArrivalICAO { get; set; }
        public string DeparturelocalPhone { get; set; }
        public string ArrivallocalPhone { get; set; }
        public string DepartureserviceEmailAddress { get; set; }
        public string ArrivalserviceEmailAddress { get; set; }

        public int aircraftId { get; set; }
    }

    public class DepartureFbo
    {
        [PrimaryKey, AutoIncrement]
        public int DepartureFboID { get; set; }

        public int fboId { get; set; }
        public string landingFacilityIdentifier { get; set; }
        public string fboHandlerName { get; set; }
        public string addressLine1 { get; set; }
        public string localPhone { get; set; }
        public string localPhoneCode { get; set; }
        public bool isCurrent { get; set; }
        public bool isCustomerFbo { get; set; }
        public bool isPreferredFbo { get; set; }
        public string serviceEmailAddress { get; set; }
        public string notes { get; set; }
    }

    public class PassengerStatus
    {
        [PrimaryKey, AutoIncrement]
        public int PassengerStatusobjID { get; set; }

        public int passengerStatusId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class Passenger
    {
        [PrimaryKey, AutoIncrement]
        public int PassengerObjID { get; set; }

        public int passengerId { get; set; }
        public int businessPersonId { get; set; }
        public int personId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string initials { get; set; }
        public string phoneNumber { get; set; }
        public string phoneCode { get; set; }
        public string emailAddress { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }

    public class FlightPassenger
    {
        [PrimaryKey, AutoIncrement]
        public int FlightPassengerObjID { get; set; }

        public int flightPassengerId { get; set; }

        public int ScheduledAircraftTripId { get; set; }
        public int aircraftLegNumber { get; set; }
        public string PassengerName { get; set; }

        public int flightId { get; set; }
        public int passengerId { get; set; }
        public int customerId { get; set; }
        public int businessCategoryId { get; set; }
        public int passengerStatusId { get; set; }
        public bool isLeadPassenger { get; set; }
        public int eApisAddressTypeId { get; set; }

        public int businessPersonId { get; set; }

        [ManyToOne]
        public PassengerStatus passengerStatus { get; set; }
        [ManyToOne]
        public Passenger passenger { get; set; }
    }

    public class FlightCrewMember
    {
        [PrimaryKey, AutoIncrement]
        public int FlightCrewMemberID { get; set; }

        public int id { get; set; }
        public string crewMemberType { get; set; }
        public string initials { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }

        public string fullName { get; set; }
        public string fullPhoneNumber { get; set; }
        public string CallVisible { get; set; }

        public int crewMemberTypeControlId { get; set; }
        public string phoneNumber { get; set; }
        public string phoneCode { get; set; }

        public int ScheduledAircraftTripId { get; set; }
        public int CrewFlightLegNumber { get; set; }

        public int flightId { get; set; }

    }

    

    public class Leg
    {
        [PrimaryKey, AutoIncrement]
        public int LegID { get; set; }

        [ManyToOne]
        public ArrivalFbo arrivalFbo { get; set; }

        [OneToMany]
        public IEnumerable<FlightPassenger> flightPassenger { get; set; }
        [OneToMany]
        public IEnumerable<FlightCrewMember> flightCrewMembers { get; set; }

        public int scheduledFlightId { get; set; }
        public int flightId { get; set; }
        public int arrivalAirportId { get; set; }
        public int departureAirportId { get; set; }
        public int aircraftId { get; set; }
        public string tailNumber { get; set; }
        public int aircraftDisplayOrder { get; set; }
        public string customTripId { get; set; }
        public string color { get; set; }
        public string arrival { get; set; }
        public string departure { get; set; }
        public string ete { get; set; }
        public string endDateTimeUtc { get; set; }
        public string startDateTimeUtc { get; set; }
        public string endDateTimeLocal { get; set; }
        public string startDateTimeLocal { get; set; }
        public double departureLatitude { get; set; }
        public double departureLongitude { get; set; }
        public double arrivalLatitude { get; set; }
        public double arrivalLongitude { get; set; }
        public int scheduledAircraftTripId { get; set; }
        public int legCount { get; set; }
        public int legNumber { get; set; }
        public bool isPrivate { get; set; }
        public bool showPostFlightLink { get; set; }
        public bool isVerifiedInPostFlight { get; set; }
        public bool isCanceled { get; set; }
        public int passengerLegCount { get; set; }
        public bool isCrewSwap { get; set; }
        public bool tentativeEta { get; set; }
        public bool tentativeEtd { get; set; }
        public bool hasAcknowledged { get; set; }
        public string description { get; set; }
        public string crewInitials { get; set; }

        [ManyToOne]
        public DepartureFbo departureFbo { get; set; }
    }

    public class WorldTimeZone
    {
        [PrimaryKey, AutoIncrement]
        public int WorldTimeZoneID { get; set; }

        public string status { get; set; }
        public int airportId { get; set; }
        public DateTime localTime { get; set; }
        public DateTime utcTime { get; set; }
        public double offset { get; set; }
        public string abbreviation { get; set; }
        public string name { get; set; }
        public bool isDst { get; set; }
        public DateTime dstEndDate { get; set; }
        public double dstOffset { get; set; }
        public DateTime dstStartDate { get; set; }
        public double standardOffset { get; set; }
        public int territoryId { get; set; }
    }

    public class DepartureAirport
    {
        [PrimaryKey, AutoIncrement]
        public int DepartureAirportID { get; set; }

        public int airportId { get; set; }
        public string icao { get; set; }
        public string iata { get; set; }
        public string faaCode { get; set; }
        public string airportName { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string associatedCityName { get; set; }
        public string stateCodeUsAndTer { get; set; }
        public string associatedCountryName { get; set; }
        public string landingFacilityIdentifier { get; set; }
        public bool jetA { get; set; }
        public bool avgas100 { get; set; }
        public bool avgas80 { get; set; }
        public string airportOperatingHours { get; set; }
        public double airportElevation { get; set; }
        public int customerId { get; set; }
        public int uvwAirportId { get; set; }

        [ManyToOne]
        public WorldTimeZone worldTimeZone { get; set; }

        public bool isCurrent { get; set; }
        public bool isCustomerAirport { get; set; }
        public DateTime daySaveTimeStartDateUtc { get; set; }
        public DateTime daySaveTimeEndDateUtc { get; set; }
        public string timeConversion { get; set; }
        public string timeConversionDaySave { get; set; }
        public int territoryId { get; set; }
    }

    public class ArrivalAirport
    {
        [PrimaryKey, AutoIncrement]
        public int ArrivalAirportID { get; set; }

        public int airportId { get; set; }
        public string icao { get; set; }
        public string iata { get; set; }
        public string faaCode { get; set; }
        public string airportName { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string associatedCityName { get; set; }
        public string stateCodeUsAndTer { get; set; }
        public string associatedCountryName { get; set; }
        public string landingFacilityIdentifier { get; set; }
        public bool jetA { get; set; }
        public bool avgas100 { get; set; }
        public bool avgas80 { get; set; }
        public string airportOperatingHours { get; set; }
        public double airportElevation { get; set; }
        public int customerId { get; set; }
        public int uvwAirportId { get; set; }

        [ManyToOne]
        public WorldTimeZone worldTimeZone { get; set; }

        public bool isCurrent { get; set; }
        public bool isCustomerAirport { get; set; }
        public DateTime daySaveTimeStartDateUtc { get; set; }
        public DateTime daySaveTimeEndDateUtc { get; set; }
        public string timeConversion { get; set; }
        public string timeConversionDaySave { get; set; }
        public int territoryId { get; set; }
    }

    public class AirportInfo
    {
        public int airportId { get; set; }
        public string icao { get; set; }
        public string airportName { get; set; }
    }

    public class AircraftEvent
    {
        [PrimaryKey, AutoIncrement]
        public int AircraftEventObjID { get; set; }

        public int aircraftEventId { get; set; }
        public int aircraftProfileId { get; set; }
        public int aircraftEventTypeId { get; set; }
        public int aircraftEventTypeControlId { get; set; }
        public string aircraftEventType { get; set; }
        public int scheduledEventId { get; set; }
        public string notes { get; set; }
        public int arrivalAirportId { get; set; }
        public int departureAirportId { get; set; }
        public DateTime departureDateTimeUtc { get; set; }
        public DateTime arrivalDateTimeUtc { get; set; }
        public DateTime departureDateTimeLocal { get; set; }
        public DateTime arrivalDateTimeLocal { get; set; }
        public double departureLatitude { get; set; }
        public double departureLongitude { get; set; }
        public double arrivalLatitude { get; set; }
        public double arrivalLongitude { get; set; }
        public string departureAirportIcao { get; set; }
        public string departureAirportIata { get; set; }
        public string departureAirportFaaCode { get; set; }
        public string arrivalAirportIcao { get; set; }
        public string arrivalAirportIata { get; set; }
        public string arrivalAirportFaaCode { get; set; }

        [OneToMany]
        public IEnumerable<object> aircraftBusinessPersons { get; set; }
        [OneToMany]
        public IEnumerable<object> aircraftCrewMembers { get; set; }

        public bool isStartDay { get; set; }
        public bool isEndDay { get; set; }
        public int calendarPosition { get; set; }
        public string color { get; set; }
        public string tailNumber { get; set; }

        [ManyToOne]
        public DepartureAirport departureAirport { get; set; }
        [ManyToOne]
        public ArrivalAirport arrivalAirport { get; set; }

        public string pilotInitials { get; set; }
        public bool isNotifyCrew { get; set; }
        public bool isAcknowledgement { get; set; }
    }

    #endregion

    public class PreflightVM
    {
        [PrimaryKey, AutoIncrement]
        public int PreflightvmID { get; set; }

        //from Preflight
        public int customerId { get; set; }
        public int aircraftId { get; set; }
        public int staffEventId { get; set; }

        [OneToMany]
        public IEnumerable<Leg> legs { get; set; }
        [OneToMany]
        public IEnumerable<object> trips { get; set; }
        [OneToMany]
        public IEnumerable<AircraftEvent> aircraftEvents { get; set; }

        [OneToMany]
        public IEnumerable<StaffEventTypes> staffEventTypes { get; set; }

        public DateTime date { get; set; }

        public int day { get; set; }
        public string dayOfTheWeek { get; set; }
        public bool isToday { get; set; }
        public bool isWeekend { get; set; }
        public bool postFlightOverridden { get; set; }
        public int lastKnownAirportId { get; set; }
        public string lastKnownAirport { get; set; }

        [OneToMany]
        public IEnumerable<FlightCrewMember> crews { get; set; }
        [OneToMany]
        public IEnumerable<FlightPassenger> flightpassengers { get; set; }
        [OneToMany]
        public IEnumerable<ArrivalFbo> fbos { get; set; }

        //from staffevents
        [OneToMany]
        public IEnumerable<MobileStaffEventBusinessPersonsDto> mobileStaffEventBusinessPersonsDto { get; set; }
        [ManyToOne]
        public Airport airport { get; set; }


        //from leg
        public DateTime endDateTimeUtc { get; set; }
        public DateTime startDateTimeUtc { get; set; }
        public DateTime endDateTimeLocal { get; set; }
        public DateTime startDateTimeLocal { get; set; }

        //scheduleflightid
        public int flightId { get; set; }
        public int scheduledAircraftTripId { get; set; }
        public bool tentativeEta { get; set; }
        public bool tentativeEtd { get; set; }
        public string ete { get; set; }
        public string crewInitials { get; set; }
        public int legCount { get; set; }
        public int legNumber { get; set; }
        public int scheduledFlightId { get; set; }


        public int passengerLegCount { get; set; }
        public string customTripId { get; set; }

        //from Aircraftevent
        public int aircraftEventTypeId { get; set; }
        public string aircraftEventType { get; set; }

        public string departureAirportIcao { get; set; }
        public string arrivalAirportIcao { get; set; }
        public string notes { get; set; }

        //From Staff event types
        public string staffEventType { get; set; }
        public int staffEventTypeId { get; set; }


        //from DepartureAirport or ArrivalAirport
        public string airportName { get; set; }
        public string airportIcao { get; set; }

        //based on event 
        public string eventName { get; set; }
        public bool IsUpdated { get; set; }


        public string color { get; set; } //legs,Staffevent types

        //Added Extra
        //fromAircraftDtos
        public string tailNumber { get; set; }

        //Custom
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string StartLocalTimeString { get; set; }
        public string EndLocalTimeString { get; set; }


        public string CurrentTimezone { get; set; }
        public string TimezoneIcon { get; set; }

        public string TripID { get; set; }

        public string abbreviation { get; set; }
        public string CardheaderIcon { get; set; }

        public string DeparturefboHandlerName { get; set; }
        public string ArrvailfboHandlerName { get; set; }
        public string DeparturelocalPhone { get; set; }
        public string ArrivallocalPhone { get; set; }
        public string DepartureserviceEmailAddress { get; set; }
        public string ArrivalserviceEmailAddress { get; set; }

        //flightid
        public int FID { get; set; }


        //Note
        public int CalendarNoteId { get; set; }
        public DateTime NoteDate { get; set; }
        public string NoteBody { get; set; }
        public string NoteIsVisible { get; set; }
        public string NoteIsEnable { get; set; }

        //Event Type
        public string EventType { get; set; }
        public string EventTypeIsVisible { get; set; }

        public string FlightIconIsVisible { get; set; }

        public string HeadeTextColor { get; set; }
        public string HeadeTextBGColor { get; set; }


        public string EcrewPaxVisibility { get; set; }


    }

    public class PreflightGroup : ObservableCollection<PreflightVM>, INotifyPropertyChanged
    {
        public DateTime Date { get; private set; }

        private bool _expanded=true;

        public PreflightGroup(DateTime date, List<PreflightVM> preflights) : base(preflights)
        {
            Date = date;
        }

        public ObservableCollection<PreflightVM> flights { get; set; }

        public PreflightGroup(DateTime date, bool expanded = true)
        {
            Date = date;
            Expanded = expanded;
        }

        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    OnPropertyChanged("Expanded");
                    OnPropertyChanged("StateIcon");
                }
            }
        }

        public string StateIcon
        {
            get { return Expanded ? "arrow_a.png" : "arrow_b.png"; }
        }

        public new event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
    


