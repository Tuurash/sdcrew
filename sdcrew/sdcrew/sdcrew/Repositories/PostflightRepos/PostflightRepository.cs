using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using sdcrew.Models;
using sdcrew.Repositories.PreflightRepos;

namespace sdcrew.Repositories.PostflightRepos
{

    public class PostflightRepository
    {
        private readonly IRequestsService _requestService;

        PostflightEndPoints postflightEndPoints = new PostflightEndPoints();
        PreflightEndPoints preflightEndPoints;
        PostFlightVM postFlightVm = new PostFlightVM();

        int Pastdays = 0;

        public PostflightRepository()
        {
            _requestService = new RequestsService();
        }

        void GetDaysfromStorage()
        {
            string getPastDysfromStorage = Services.Settings.PostflightPastDays;

            if (getPastDysfromStorage != "")
            {
                switch (getPastDysfromStorage)
                {
                    case "Present":
                        Pastdays = 0;
                        break;
                    default:
                        string trimday = getPastDysfromStorage.Trim('-');
                        trimday = trimday.Replace(" days", "");
                        Pastdays = int.Parse(trimday.Trim());
                        break;
                }
            }
        }

        public async Task<List<CrewDetailsVM>> FetchAllCrews()
        {
            preflightEndPoints = new PreflightEndPoints();
            var url = preflightEndPoints.PRE_FLIGHT_REQUEST_URL + preflightEndPoints.FETCH_ALL_CREWS;

            var Jsonresult = await _requestService.GetAsyncJsonResult(url);
            string JString = Jsonresult.ToString();

            var crews = new List<CrewDetailsVM>();
            try
            {
                crews = JsonConvert.DeserializeObject<IEnumerable<CrewDetailsVM>>(JString).ToList();
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
            }

            return crews;
        }

        public async Task<string> FetchAllAirports()
        {
            var url = postflightEndPoints.POST_FLIGHT_REQUEST_URL + postflightEndPoints.AIRPORTS;
            await Task.Delay(2);

            var Jsonresult = _requestService.GetJsonResult(url);
            string JString = Jsonresult.ToString();

            return JString;
        }

        public async Task<ApuNCustomComponents> FetchApuNCustomComponentsAsync(int PostedFlightId, int AircraftProfileId, DateTime FlightStartDateTime)
        {
            string url = String.Empty;
            if (PostedFlightId != 0)
            {
                //sd-profile-api.satcomdirect.com/api/MobilePostFlightComponent/Filter?postedFlightId={postedFlightId}
                url = postflightEndPoints.POST_FLIGHT_REQUEST_URL + postflightEndPoints.FETCH_APUNCUSTOMCOMPONENETS + "Filter?postedFlightId=" + PostedFlightId;
            }
            else
            {
                //sd-profile-api.satcomdirect.com/api/MobilePostFlightComponent/AttachedComponents?aircraftProfileId={aircraftProfileId}&attachDate={flightStartDateTime}
                url = postflightEndPoints.POST_FLIGHT_REQUEST_URL + postflightEndPoints.FETCH_APUNCUSTOMCOMPONENETS + "AttachedComponents?aircraftProfileId=" + AircraftProfileId + "&attachDate=" + FlightStartDateTime;
            }

            await Task.Delay(2);

            var Jsonresult = await _requestService.GetAsyncJsonResult(url);
            string JString = Jsonresult.ToString().Trim('[', ']');

            var apuNCustomComponents = new ApuNCustomComponents();
            try
            {
                apuNCustomComponents = JsonConvert.DeserializeObject<ApuNCustomComponents>(JString);
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
            }

            return apuNCustomComponents;
        }

        #region Additional Details

        public async Task<List<AircraftProfileDto>> FetchPostflightAircraftDTOs()
        {
            var url = postflightEndPoints.POST_FLIGHT_REQUEST_URL + postflightEndPoints.POST_FLIGHT_AIRCRAFTDTOS;


            var Jsonresult = await _requestService.GetAsyncJsonResult(url);
            string JString = Jsonresult.ToString();

            var DTOs = new List<AircraftProfileDto>();
            try
            {
                DTOs = JsonConvert.DeserializeObject<IEnumerable<AircraftProfileDto>>(JString).ToList();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            return DTOs;
        }

        public async Task<List<BusinessCatagory>> FetchBusinessCatagoriesAsync()
        {
            string url = postflightEndPoints.PROFILE_REQUEST_URL + postflightEndPoints.FETCH_BUSINESSCATAGORIES;


            var Jsonresult = await _requestService.GetAsyncJsonResult(url);
            await Task.Delay(2);
            string JString = Jsonresult.ToString();

            var businessCatagories = new List<BusinessCatagory>();
            try
            {
                businessCatagories = JsonConvert.DeserializeObject<List<BusinessCatagory>>(JString);
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
            }
            return businessCatagories;
        }

        public async Task<List<DelayType>> FetchDelayTypesAsync()
        {
            string url = postflightEndPoints.PROFILE_REQUEST_URL + postflightEndPoints.FETCH_DELAYTYPES;


            var Jsonresult = await _requestService.GetAsyncJsonResult(url);
            await Task.Delay(2);
            string JString = Jsonresult.ToString();

            var delayTypes = new List<DelayType>();
            try
            {
                delayTypes = JsonConvert.DeserializeObject<List<DelayType>>(JString);
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
            }
            return delayTypes;
        }

        public async Task<List<Department>> FetchDepartmentsAsync()
        {
            string url = postflightEndPoints.PROFILE_REQUEST_URL + postflightEndPoints.FETCH_DEPARTMENTS;


            var Jsonresult = await _requestService.GetAsyncJsonResult(url);
            await Task.Delay(2);
            string JString = Jsonresult.ToString();

            var departments = new List<Department>();
            try
            {
                departments = JsonConvert.DeserializeObject<List<Department>>(JString);
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
            }
            return departments;
        }

        #endregion

        #region DeIce

        public async Task<PostedFlightDeIce> FetchDeIce()
        {
            string url = postflightEndPoints.PROFILE_REQUEST_URL + postflightEndPoints.FETCH_DEICE;

            await Task.Delay(2);

            var Jsonresult = await _requestService.GetAsyncJsonResult(url);
            string JString = Jsonresult.ToString().Trim('[', ']');

            var deIce = new PostedFlightDeIce();
            try
            {
                deIce = JsonConvert.DeserializeObject<PostedFlightDeIce>(JString);
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
            }
            return deIce;
        }

        public async Task<List<MixType>> FetchMixTypesAsync()
        {
            string url = postflightEndPoints.PROFILE_REQUEST_URL + postflightEndPoints.FETCH_MIXRATIOTYPES;


            var Jsonresult = await _requestService.GetAsyncJsonResult(url);
            await Task.Delay(2);
            string JString = Jsonresult.ToString();

            var mixTypes = new List<MixType>();
            try
            {
                mixTypes = JsonConvert.DeserializeObject<List<MixType>>(JString);
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
            }
            return mixTypes;
        }


        #endregion

        #region checklist

        public async Task<bool> Save_Checklist(ChecklistVM Checklist)
        {
            var url = postflightEndPoints.POST_FLIGHT_REQUEST_URL + postflightEndPoints.SAVE_POSTFLIGHT_CHECKLIST;

            try
            {
                var r = await _requestService.Post_Custom(Checklist, url);
                return r;
            }
            catch (Exception exc)
            {
                Console.Write(exc);
                return false;
            }
        }

        public async Task<bool> Delete_Checklist(ChecklistVM maitenanceChecklist)
        {
            var url = postflightEndPoints.POST_FLIGHT_REQUEST_URL + postflightEndPoints.DELETE_POSTFLIGHT_CHECKLIST;

            try
            {
                await _requestService.Post_Custom(maitenanceChecklist, url);
                return true;
            }
            catch (Exception exc)
            {
                Console.Write(exc);
                return false;
            }
        }

        public async Task<bool> postChecklistESign(int getEsignPin, int flightId, int v, int checkListTypeId, int checklistRoleTypeId, List<ChecklistVM> getSavedList, string email, int legNumber)
        {
            List<ChecklistTemp> CheckListTasks = new List<ChecklistTemp>();
            await Task.Delay(0);
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
                pinNumber = getEsignPin,
                flightId = flightId,
                checkListTypeId = checkListTypeId,
                checklistRoleTypeId = checklistRoleTypeId,
                checklistTasks = CheckListTasks
            };

            var url = postflightEndPoints.POST_FLIGHT_REQUEST_URL + postflightEndPoints.ESIGN_POSTFLIGHT_CHECKLIST;

            try
            {
                var getESignStatus = _requestService.Post_Custom(eSingBody, url);
                return true;
            }
            catch (Exception exc) { Crashes.TrackError(exc); return false; }

        }

        #endregion

        public async Task<List<PostFlight>> GetAllPostFlightEvents()
        {
            GetDaysfromStorage();

            string getEndDate = DateTime.Today.AddDays(+1).ToString("yyyy-MM-dd");
            string getStartDate = DateTime.Today.AddDays(-(Pastdays + 1)).ToString("yyyy-MM-dd");


            string getExcludeHobbsEnableTails = "true";
            string getPage = "1";
            string getPageSize = "100";
            string getSortDirection = "2";

            //?StartDate=2021-11-13&EndDate=2021-11-22&ExcludeHobbsEnableTails=true&Page=1&PageSize=100&SortDirection=2
            //https://sd-profile-api.satcomdirect.com/api/MobileFlightList/List?StartDate=2021-11-13&EndDate=2021-11-22&ExcludeHobbsEnableTails=true&Page=1&PageSize=100&SortDirection=2

            var url = postflightEndPoints.POST_FLIGHT_REQUEST_URL + postflightEndPoints.POST_FLIGHT_EVENTS + "?StartDate=" + getStartDate + "&EndDate=" + getEndDate + "&ExcludeHobbsEnableTails=" + getExcludeHobbsEnableTails + "&Page=" + getPage + "&PageSize=" + getPageSize + "&SortDirection=" + getSortDirection;

            var Jsonresult = await _requestService.GetAsyncJsonResult(url);

            string JString = Jsonresult.ToString();

            var flights = new List<PostFlight>();
            try
            {
                var RootObj = JsonConvert.DeserializeObject<PostFlightRoot>(JString);

                if (RootObj.resultSet.Count > 0)
                {
                    foreach (var postflight in RootObj.resultSet)
                    {
                        flights.Add(postflight);
                    }
                }
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
            }

            return flights;
        }

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

}
