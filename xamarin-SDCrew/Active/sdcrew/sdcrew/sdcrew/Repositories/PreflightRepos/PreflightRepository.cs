using Newtonsoft.Json;
using RestSharp;
using sdcrew.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace sdcrew.Repositories.PreflightRepos
{
    public class PreflightRepository
    {
        private readonly IRequestsService _requestService;

        PreflightEndPoints _preflightEndpoints = new PreflightEndPoints();
        PreflightVM _preflightVm = new PreflightVM();

        public PreflightRepository()
        {
            _requestService = new RequestsService();
        }

        //FetchTailnumber
        public async Task<IEnumerable<AircraftProfileDto>> GetAircraftProfileDtos()
        {
            var url = _preflightEndpoints.PRE_FLIGHT_PROFILE_REQUEST_URL + _preflightEndpoints.FETCH_AIRCRAFT_PROFILE_DTOS;

            var Jsonresult =await _requestService.GetAsyncJsonResult(url);
            string JString = Jsonresult.ToString();

            var APDTOs = new List<AircraftProfileDto>();
            try
            {
                APDTOs = JsonConvert.DeserializeObject<IEnumerable<AircraftProfileDto>>(JString).ToList();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            return APDTOs;
        }

        public async Task<IEnumerable<StaffEventTypes>> GetStaffEventTypes()
        {
            var url = _preflightEndpoints.PRE_FLIGHT_REQUEST_URL + _preflightEndpoints.FETCH_STAFF_EVENT_TYPES;

            var Jsonresult =await _requestService.GetAsyncJsonResult(url);
            string JString = Jsonresult.ToString();

            var _staffEventTypes = new List<StaffEventTypes>();
            try
            {
                _staffEventTypes = JsonConvert.DeserializeObject<IEnumerable<StaffEventTypes>>(JString).ToList();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            return _staffEventTypes;
        }

        #region AircraftEvents
        public async Task<IEnumerable<PreFlight>> GetPreflightAircraftEvents()
        {

            string getStartDate = DateTime.Today.AddDays(-3).ToShortDateString();
            string getEndDate = DateTime.Today.AddDays(+3).ToShortDateString();
         

            string getInAirportLocalTime = "true";
            string getshowCanceled = "false";

            //sd-profile-api.satcomdirect.com/preflight/api/MobileCalendar/GetCalendarDaysByDateRange?startDateTime=2020-08-03&endDateTime=2021-08-03&inAirportLocalTime=false&showCanceled=false
            var url = _preflightEndpoints.PRE_FLIGHT_REQUEST_URL + _preflightEndpoints.PRE_FLIGHT_AIRCRAFT_EVENT + "?startDateTime=" + getStartDate + "&endDateTime=" + getEndDate + "&inAirportLocalTime=" + getInAirportLocalTime + "&showCanceled=" + getshowCanceled;


            var Jsonresult =await _requestService.GetAsyncJsonResult(url);
            string JString = Jsonresult.ToString();

            var PreFlights = new List<PreFlight>();
            try
            {
                PreFlights = JsonConvert.DeserializeObject<IEnumerable<PreFlight>>(JString).ToList();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            return PreFlights;
        }

        #endregion

        #region StaffEvents

        public async Task<IEnumerable<PreFlight>> GetPreflightStaffEvents()
        {

            string getStartDate = DateTime.Today.AddDays(-3).ToShortDateString();
            string getEndDate = DateTime.Today.AddDays(+3).ToShortDateString();

            //string getStartDate = "2020-08-03";
            //string getEndDate = DateTime.Today.AddDays(+15).ToShortDateString();
            string getInAirportLocalTime = "false";

            //sd-profile-api.satcomdirect.com/preflight/api/MobileCalendar/GetCalendarDaysByDateRange?startDateTime=2020-08-03&endDateTime=2021-08-03&inAirportLocalTime=false&showCanceled=false
            var url = _preflightEndpoints.PRE_FLIGHT_REQUEST_URL + _preflightEndpoints.PRE_FLIGHT_STUFF_EVENT + "?startDateTime=" + getStartDate + "&endDateTime=" + getEndDate + "&inAirportLocalTime=" + getInAirportLocalTime+ "&showCanceled=false";


            var Jsonresult =await _requestService.GetAsyncJsonResult(url);
            string JString = Jsonresult.ToString();

            var PreFlights = new List<PreFlight>();
            try
            {
                PreFlights = JsonConvert.DeserializeObject<IEnumerable<PreFlight>>(JString).ToList();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            return PreFlights;
        }

        #endregion

        #region Services&Notes

        public async Task<AircraftSevices> GetAircraftServices(int scheduledFlightId)
        {
            int getScheduledFlightID = scheduledFlightId;

            int getServiceType = 0;
            int getMemberType = 0;

            //https://sd-profile-api.satcomdirect.com/preflight/api/MobileScheduling/GetServices?scheduledFlightId=27638&serviceType=0&memberType=0

            var url = _preflightEndpoints.PRE_FLIGHT_REQUEST_URL + _preflightEndpoints.FETCH_SERVICES + "?scheduledFlightId=" + getScheduledFlightID + "&serviceType=" + getServiceType + "&memberType=" + getMemberType;

            var Jsonresult = await _requestService.GetAsyncJsonResult(url);
            string JString = Jsonresult.ToString();

            var aircraftSevices = new AircraftSevices();
            try
            {
                aircraftSevices = JsonConvert.DeserializeObject<AircraftSevices>(JString);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            return aircraftSevices;
        }

        public async Task<IEnumerable<PreflightNote>> GetPreflightNotes()
        {

            string getStartDate = DateTime.Today.AddDays(-3).ToShortDateString();
            string getEndDate = DateTime.Today.AddDays(+3).ToShortDateString();


            string getInAirportLocalTime = "true";
            string getshowCanceled = "false";

            //RequestAPIS.PRE_FLIGHT_REQUEST_URL + AppConfig.RequestAPIS.FETCH_PREFLIGHT_NOTES + "?startDateTime=\(escapedStringStartdate)&endDateTime=\(escapedStringtodate)&inAirportLocalTime=false&showCanceled=false"
            var url = _preflightEndpoints.PRE_FLIGHT_REQUEST_URL + _preflightEndpoints.FETCH_PREFLIGHT_NOTES + "?startDateTime=" + getStartDate + "&endDateTime=" + getEndDate + "&inAirportLocalTime=" + getInAirportLocalTime + "&showCanceled=" + getshowCanceled;


            var Jsonresult = await _requestService.GetAsyncJsonResult(url);
            string JString = Jsonresult.ToString();

            var preflightNotes = new List<PreflightNote>();
            try
            {
                preflightNotes = JsonConvert.DeserializeObject<IEnumerable<PreflightNote>>(JString).ToList();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            return preflightNotes;
        }


        #endregion

        #region Iteniery


        public async Task Fetch_Itineray(int type, int legNumber, ArrayList crewMemberIds, ArrayList passengerBusinessPersonIds, int scheduledAircraftTripId)
        {
            var url = _preflightEndpoints.PRE_FLIGHT_REQUEST_URL + _preflightEndpoints.FETCH_ITINERARY;

            Dictionary<string, object> Legs = new Dictionary<string, object>
            {
                { "legNumber", legNumber },
                { "crewMemberIds", crewMemberIds },
                { "passengerBusinessPersonIds", passengerBusinessPersonIds }
            };

            IteneryRequestBody requestBody = new IteneryRequestBody();

            requestBody = new IteneryRequestBody
            {
                itineraryTripId = scheduledAircraftTripId,
                itineraryName = "",
                itineraryRequestType = type,
                showCrewCateringNotes = false,
                showGroundCrewNotes = false,
                legs = new object[] { Legs },
                outputType = "application/pdf"
            };

            try
            {
               await requestsService.PostAsyncPDF(url,requestBody);

            }
            catch (Exception exc)
            {
                Console.Write(exc);

            }
        }

        public class IteneryRequestBody
        {
            public int itineraryTripId { get; set; }
            public string itineraryName { get; set; }
            public int itineraryRequestType { get; set; }
            public bool showCrewCateringNotes { get; set; }
            public bool showGroundCrewNotes { get; set; }
            public object legs { get; set; }
            public string outputType { get; set; }
        }


        #endregion

        #region CheckList

        public async Task<PreflightChecklist> GetPreflightAircraftCheckLists(int _aircraftId)
        {
            string checklistTypeId = "1";
            //https://sd-profile-api.satcomdirect.com/preflight/api/ScheduledAircraftTrip/GetAppliedChecklists/1/15117 

            var url = _preflightEndpoints.PRE_FLIGHT_REQUEST_URL + _preflightEndpoints.FETCH_CHECKLIST +"/"+ checklistTypeId+"/"+_aircraftId.ToString();

            var Jsonresult = await _requestService.GetAsyncJsonResult(url);
            string JString = Jsonresult.ToString();

            var CheckLists = new PreflightChecklist();
            try
            {
                CheckLists = JsonConvert.DeserializeObject<PreflightChecklist>(JString);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            return CheckLists;
        }

        public async Task<bool> SaveChecklist(ChecklistVM Checklist)
        {
            var url = _preflightEndpoints.PRE_FLIGHT_REQUEST_URL + _preflightEndpoints.POST_PREFLIGHT_CHECKED_CHECKLIST;

            try
            {
                var r = await requestsService.Post_Custom(Checklist, url);
                return true;
            }catch(Exception exc)
            {
                Console.Write(exc);
                return false;
            }
        }

        RequestsService requestsService = new RequestsService();

        public async Task<bool> Save_Checklist(ChecklistVM maitenanceChecklist)
        {
            var url = _preflightEndpoints.PRE_FLIGHT_REQUEST_URL + _preflightEndpoints.POST_PREFLIGHT_CHECKED_CHECKLIST;

            

            try
            {
                var r= await requestsService.Post_Custom(maitenanceChecklist, url);
                return r;
            }
            catch (Exception exc)
            {
                Console.Write(exc);
                return false;
            }
        }

        public async Task<bool> DeleteChecklist(ChecklistVM maitenanceChecklist)
        {
            var url = _preflightEndpoints.PRE_FLIGHT_REQUEST_URL + _preflightEndpoints.POST_PREFLIGHT_DELETE_CHECKLIST;

            try
            {
                await requestsService.Post_Custom(maitenanceChecklist, url);
                return true;
            }
            catch (Exception exc)
            {
                Console.Write(exc);
                return false;
            }
        }

        public async Task<bool> postESign(int getEsignPin, int flightId, int v, int checkListTypeId, int checklistRoleTypeId, List<ChecklistVM> getSavedList, string email, int legNumber)
        {
            List<ChecklistTemp> CheckListTasks = new List<ChecklistTemp>();

            foreach (var selected in getSavedList)
            {
                var Temp = new ChecklistTemp
                {
                    customerId = selected.customerId,
                    tripId = selected.tripId,
                    flightId = selected.flightId,
                    legNumber = legNumber,                       //check Errors Here
                    checkListTypeId = selected.checkListTypeId,
                    checklistRoleTypeId = selected.checklistRoleTypeId,
                    checklistRoleTypeName = selected.checklistRoleTypeName,
                    configTaskId = selected.configTaskId,
                    configTaskName = selected.configTaskName,
                    configTaskOrder = selected.configTaskOrder,
                    selectedByUser = email,  //Change
                    selectedDateTime = DateTime.Now,
                    checklistSignId = selected.checklistSignId
                };

                CheckListTasks.Add(Temp);
            }

            ESingBody eSingBody = new ESingBody();

            eSingBody = new ESingBody
            {
                pinNumber=getEsignPin,
                flightId= flightId,
                checkListTypeId= checkListTypeId,
                checklistRoleTypeId= checklistRoleTypeId,
                checklistTasks= CheckListTasks
            };

            var url = _preflightEndpoints.PRE_FLIGHT_REQUEST_URL + _preflightEndpoints.ESIGN_CHECKLIST_PREFLIGHT;

            try
            {
               var getESignStatus = requestsService.Post_Custom(eSingBody,url);
                return true;
            }
            catch(Exception exc) { Debug.WriteLine(exc); return false; }

        }

        public class ChecklistTemp
        {
            public int flightId { get; set; }
            public int configTaskId { get; set; }
            public int tripId { get; set; }
            public int legNumber { get; set; }
            public int checkListTypeId { get; set; }
            public int checklistRoleTypeId { get; set; }
            public string checklistRoleTypeName { get; set; }

            public string configTaskName { get; set; }
            public int configTaskOrder { get; set; }
            public string selectedByUser { get; set; }
            public int checklistSignId { get; set; }
            public DateTime selectedDateTime { get; set; }

            public int customerId { get; set; }
        }

        public class ESingBody
        {
            public int pinNumber { get; set; }
            public int flightId { get; set; }
            public int checkListTypeId { get; set; }
            public int checklistRoleTypeId { get; set; }
            public List<ChecklistTemp> checklistTasks { get; set; }
        }

        #endregion







    }

}
