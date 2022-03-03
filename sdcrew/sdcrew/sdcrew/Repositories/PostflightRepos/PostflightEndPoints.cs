using System;
namespace sdcrew.Repositories.PostflightRepos
{
    public class PostflightEndPoints
    {
        //api/MobileFlightList/List?StartDate=2021-11-13&EndDate=2021-11-22&ExcludeHobbsEnableTails=true&Page=1&PageSize=100&SortDirection=2
        //DefaultEnd Points
        public readonly string POST_FLIGHT_REQUEST_URL = @"https://sd-profile-api.satcomdirect.com/";

        public readonly string PROFILE_REQUEST_URL = @"https://sd-profile-api.satcomdirect.com/profile/";

        //All_Airports->Icaos
        public readonly string AIRPORTS = @"api/MobilePostFlightAirport";

        //ALL post flight events Logged & Not Logged
        public readonly string POST_FLIGHT_EVENTS = @"api/MobileFlightList/List";

        //postflight/api/AircraftProfile/AircraftProfileDtos
        public readonly string POST_FLIGHT_AIRCRAFTDTOS = @"postflight/api/AircraftProfile/AircraftProfileDtos";

        public readonly string FETCH_APUNCUSTOMCOMPONENETS = @"api/MobilePostFlightComponent/";

        public readonly string ESIGN_POSTFLIGHT_CHECKLIST = @"postflight/api/postedFlight/PostFlightChecklistSign";

        public readonly string SAVE_POSTFLIGHT_CHECKLIST = @"postflight/api/PostedFlight/SaveCheckedData";

        public readonly string DELETE_POSTFLIGHT_CHECKLIST = @"postflight/api/PostedFlight/DeleteCheckedData";

        public readonly string FETCH_BUSINESSCATAGORIES = @"api/Reference/businessCategory?activeOnly=false&includeSystem=false";

        public readonly string FETCH_DELAYTYPES = @"api/Reference/delayType?activeOnly=false&includeSystem=false";

        public readonly string FETCH_DEPARTMENTS = @"api/Reference/department?activeOnly=false&includeSystem=false";

        public readonly string FETCH_DEICE = @"api/Reference/deiceType?activeOnly=false&includeSystem=false";

        public readonly string FETCH_MIXRATIOTYPES = @"api/Reference/mixRatioType?activeOnly=false&includeSystem=false";

        public readonly string FETCH_DOCUMENTTYPES = @"api/Reference/DocumentType?activeOnly=false&includeSystem=true";

        public readonly string FETCH_ATA_CODES = @"api/Reference/DocumentType?activeOnly=false&includeSystem=true";

        public readonly string FETCH_SQUAWK_CATAGORIES = @"api/Reference/squawkCategory?activeOnly=true&includeSystem=false";

        public readonly string FETCH_SQUAWK_TYPES = @"api/Reference/squawkType?activeOnly=true&includeSystem=false";

        public readonly string FETCH_SQUAWK_STATUS = @"api/Reference/squawkStatus?activeOnly=true&includeSystem=false";

        public readonly string FETCH_CREWMEMBERtYPE = @"api/Reference/crewMemberType";

        public readonly string FETCH_APPROACHTYPE = @"api/Reference/approachType";

        public readonly string FETCH_EXPENSECATAGORY = @"api/Reference/expenseCategory";

        public readonly string FETCH_PAYMENTYPE = @"api/Reference/approachType";

        public readonly string FETCH_QUANTITYTYPE = @"api/Reference/approachType";

        public readonly string FETCH_FUELQUANTITYTYPE = @"api/Reference/approachType";


        //Post
        public readonly string POST_OOOI = @"api/Reference/approachType";

        public readonly string PUT_ADDITIONALDETAILS = @"api/Reference/approachType";

        public readonly string PUT_DEICE = @"api/Reference/approachType";

        public readonly string PUT_FUEL = @"api/MobilePostFlightFuel";




    }
}
