using System;
namespace sdcrew.Repositories.PostflightRepos
{
    public class PostflightEndPoints
    {
        //api/MobileFlightList/List?StartDate=2021-11-13&EndDate=2021-11-22&ExcludeHobbsEnableTails=true&Page=1&PageSize=100&SortDirection=2
        //DefaultEnd Points
        public readonly string POST_FLIGHT_REQUEST_URL = @"https://sd-profile-api.satcomdirect.com/";

        //All_Airports->Icaos
        public readonly string AIRPORTS = @"api/MobilePostFlightAirport";

        //ALL post flight events Logged & Not Logged
        public readonly string POST_FLIGHT_EVENTS = @"api/MobileFlightList/List";

        //postflight/api/AircraftProfile/AircraftProfileDtos
        public readonly string POST_FLIGHT_AIRCRAFTDTOS = @"postflight/api/AircraftProfile/AircraftProfileDtos";
    }
}
