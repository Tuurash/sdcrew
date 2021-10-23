using System;
using System.Collections.Generic;
using System.Text;

namespace sdcrew.Repositories.PreflightRepos
{
    public class PreflightEndPoints
    {
        //DefaultEnd Points
        public readonly string PRE_FLIGHT_REQUEST_URL = @"https://sd-profile-api.satcomdirect.com/preflight/";
        public readonly string PRE_FLIGHT_PROFILE_REQUEST_URL = @"https://sd-profile-api.satcomdirect.com/profile/";

        // data api's

        //Staff event types for preflight,[events in ui]
        public readonly string FETCH_STAFF_EVENT_TYPES = "api/StaffEvent/StaffEventTypes";

        //StaffEvents 
        //sd-profile-api.satcomdirect.com/preflight/api/MobileCalendar/StaffEvents?startDateTime=2020-06-01&endDateTime=2021-08-03&inAirportLocalTime=false
        public readonly string PRE_FLIGHT_STUFF_EVENT = "api/MobileCalendar/StaffEvents";


        //aircraftEvents 
        //sd-profile-api.satcomdirect.com/preflight/api/MobileCalendar/GetCalendarDaysByDateRange?startDateTime=2020-08-03&endDateTime=2021-08-03&inAirportLocalTime=false&showCanceled=false&inAirportLocalTime=false
        public readonly string PRE_FLIGHT_AIRCRAFT_EVENT = "api/MobileCalendar/GetCalendarDaysByDateRange";

        //PreflightNotes
        //RequestAPIS.PRE_FLIGHT_REQUEST_URL + AppConfig.RequestAPIS.FETCH_PREFLIGHT_NOTES + "?startDateTime=\(escapedStringStartdate)&endDateTime=\(escapedStringtodate)&inAirportLocalTime=false&showCanceled=false"
        public readonly string FETCH_PREFLIGHT_NOTES = "api/CalendarNote/GetCalendarNotes";


        //let notificationurlString = AppConfig.RequestAPIS.PRE_FLIGHT_REQUEST_URL + AppConfig.RequestAPIS.FETCH_NOTIFICATION + "?
        //notificationOption=\(notificationOption)&pageNumber=1&pageSize=25"
        public readonly string FETCH_NOTIFICATION = "Notfound in appconfig swift";

        //Tail Numbers
        //sd-profile-api.satcomdirect.com/preflight/api/AircraftProfiles/AircraftProfileDtos
        public readonly string FETCH_AIRCRAFT_PROFILE_DTOS = "api/AircraftProfiles/AircraftProfileDtos";
        public readonly string FETCH_AIRCRAFT_PROFILES = "api/AircraftProfile";


        //fETCH Aircraft checklists by AircraftID & typeID
        //https://sd-profile-api.satcomdirect.com/preflight/api/ScheduledAircraftTrip/GetAppliedChecklists/1/15117 

        public readonly string FETCH_CHECKLIST = "api/ScheduledAircraftTrip/GetAppliedChecklists";
        public readonly string POST_PREFLIGHT_CHECKED_CHECKLIST = "api/ScheduledAircraftTrip/SaveCheckedData";
        public readonly string POST_PREFLIGHT_DELETE_CHECKLIST = "api/ScheduledAircraftTrip/DeleteCheckedData";
        public readonly string FETCH_ITINERARY = "api/Scheduling/Itinerary/GetItinerary";

        //FETCH SERVICES
        //https://sd-profile-api.satcomdirect.com/preflight/api/MobileScheduling/GetServices?scheduledFlightId=27638&serviceType=0&memberType=0

        public readonly string FETCH_SERVICES = "api/MobileScheduling/GetServices";

        //ESign
        public readonly string ESIGN_CHECKLIST_PREFLIGHT = "api/ScheduledAircraftTrip/PreFlightChecklistSign";


    }
}
