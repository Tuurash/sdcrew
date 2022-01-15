using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                Console.WriteLine(exc);
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
                Console.WriteLine(exc);
            }

            return flights;
        }
    }

}
