using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Newtonsoft.Json;

using sdcrew.Models;
using sdcrew.Repositories.PostflightRepos;

using SQLite;

namespace sdcrew.Services.Data
{
    public class PostflightServices
    {
        KeyGenerator _keygen = new KeyGenerator();
        static SQLiteConnection db;

        PostflightRepository postflightRepo;

        string colorString = "";

        string AllAirportsJsonfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"MobilePostFlightAirport_Prod.json");

        public PostflightServices()
        {
            _keygen.CheckKey();
            postflightRepo = new PostflightRepository();
        }

        bool IsBusy = false;

        public async Task InsertDatas()
        {
            var t1 = AddAllPostflights();
            var t2 = AddPostflight_AircraftProfileDTos();


            if (IsBusy == false)
            {
                IsBusy = true;

                await Task.WhenAll(t2, t1)
                    .ContinueWith(x => IsBusy = false).ConfigureAwait(false);
            }
        }


        #region Helpers

        private async Task Init()
        {
            db = await DB_Init.Init();
        }

        private string ConvertHex(string colorString)
        {
            string[] Rgb = colorString.Split(',');

            string hex = "";
            if (colorString != "")
            {
                try
                {
                    System.Drawing.Color myColor = System.Drawing.Color.FromArgb(int.Parse(Rgb[0]), int.Parse(Rgb[1]), int.Parse(Rgb[2]));
                    hex = "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2");
                }
                catch (Exception) { }
            }

            return hex;
        }

        #endregion

        public List<PostFlightVM> GetFilteredPostFlights(List<string> filteritemsList, string type)
        {
            _ = Init();

            var Typewiseflights = db.Table<PostFlightVM>().Where(x => x.postFlightStatusName == type);
            var flights = Typewiseflights.Where(x => !filteritemsList.Contains(x.TailNumber)).OrderBy(x => x.StartDate).ToList();

            return flights;
        }

        public List<PostFlightVM> GetPostFlightsByType(string type)
        {
            _ = Init();

            var flights = db.Table<PostFlightVM>().Where(x => x.postFlightStatusName == type).OrderBy(x => x.StartDate).ToList();
            return flights;
        }

        public List<AllAirports> GetAllAirports()
        {
            try
            {
                return GetAirportList(AllAirportsJsonfile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<AllAirports> GetAirportList(string allAirportsJsonfile)
        {
            List<AllAirports> airports = new List<AllAirports>();
            string json = "";

            string text = "";

            if (File.Exists(AllAirportsJsonfile))
            {
                json = File.ReadAllText(AllAirportsJsonfile);
                airports = JsonConvert.DeserializeObject<List<AllAirports>>(json);
                return airports;
            }
            else
            {
                Stream stream = this.GetType().Assembly.GetManifestResourceStream("sdcrew.Services.Data.AllAirportsFile.json");

                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }

                airports = JsonConvert.DeserializeObject<List<AllAirports>>(text);
                return airports;
            }
        }

        public async Task AddPostflight_AircraftProfileDTos()
        {
            AircraftProfileDto aircraftProfileDto = new AircraftProfileDto();

            await Init();

            var getAircraftProfileDtos = await postflightRepo.FetchPostflightAircraftDTOs().ConfigureAwait(false);
            try
            {

                if (getAircraftProfileDtos != null)
                {
                    foreach (var profile in getAircraftProfileDtos)
                    {
                        var dto = new AircraftProfileDto
                        {
                            aircraftProfileId = profile.aircraftProfileId,
                            tailNumber = profile.tailNumber,
                            serialNumber = profile.serialNumber,
                        };

                        db.Insert(dto);
                    }
                }
            }
            catch (Exception)
            { }
        }

        public async Task InsertAllAirports()
        {
            AllAirports airports = new AllAirports();

            await Init();

            var getAllAirports = await postflightRepo.FetchAllAirports();
            try
            {

                if (getAllAirports != null)
                {
                    File.WriteAllText(AllAirportsJsonfile, getAllAirports);
                }
            }
            catch (Exception)
            { }
        }

        #region AllPostFlights

        public async Task AddAllPostflights()
        {
            string getTailnumber = "";
            string getEndDate = "N/A";



            PostFlightVM postflightVm = new PostFlightVM();

            await Init();

            var AllPostflights = await postflightRepo.GetAllPostFlightEvents();
            if (AllPostflights != null)
            {
                //Clear Table before Insertion
                var dltStatusPostFlights = db.Table<PostFlightVM>().Delete(x => x.flightId != 0);

                foreach (var flight in AllPostflights)
                {
                    if (flight.flightId != 0)
                    {
                        getTailnumber = db.Table<AircraftProfileDto>().Where(x => x.aircraftProfileId == flight.aircraftProfileId).Select(x => x.tailNumber).FirstOrDefault();

                        if (flight.flightStopDateTime != null)
                        {
                            getEndDate = flight.flightStopDateTime.ToString();
                        }
                        try
                        {
                            postflightVm = new PostFlightVM
                            {
                                flightId = flight.flightId,
                                aircraftProfileId = flight.aircraftProfileId,
                                postedFlightId = flight.postedFlightId,
                                departureIcao = flight.departureIcao,
                                airportIcao = flight.airportIcao,
                                departureAirportId = flight.departureAirportId,
                                arrivalAirportId = flight.arrivalAirportId,
                                tripId = flight.tripId,
                                postedAircraftTripId = flight.postedAircraftTripId,
                                customTripId = flight.customTripId,

                                //Differentiate between Logged and not logged
                                postFlightStatusName = flight.postFlightStatusName,

                                postedFlightStatusTypeId = flight.postedFlightStatusTypeId,
                                roundingThreshold = flight.roundingThreshold,

                                aircraftColor = ConvertHex(flight.aircraftColor),

                                flightStartDateTime = flight.flightStartDateTime.ToUniversalTime(),

                                legCount = flight.legCount,
                                legNumber = flight.legNumber,
                                isUserSigned = flight.isUserSigned,             //remove if not necessary
                                passengerLegCount = flight.passengerLegCount,   //remove if not necessary

                                postedFlightOooi = new PostedFlightOooi
                                {
                                    aircraftProfileId = flight.PostedFlightOooi.aircraftProfileId,
                                    departureAirportId = flight.PostedFlightOooi.departureAirportId,
                                    arrivalAirportId = flight.PostedFlightOooi.arrivalAirportId,
                                    blockStartDateTime = flight.PostedFlightOooi.blockStartDateTime,
                                    flightStartDateTime = flight.PostedFlightOooi.flightStartDateTime,
                                    blockStopDateTime = flight.PostedFlightOooi.blockStopDateTime,
                                    flightStopDateTime = flight.PostedFlightOooi.flightStopDateTime,
                                },
                                oooi = new Oooi
                                {
                                    automatedOutTime = flight.automatedFlightData.oooi.automatedOutTime,
                                    automatedOffTime = flight.automatedFlightData.oooi.automatedOffTime,
                                    automatedOnTime = flight.automatedFlightData.oooi.automatedOnTime,
                                    automatedInTime = flight.automatedFlightData.oooi.automatedInTime,
                                },

                                postedFlightAdditional = flight.postedFlightAdditional, //model

                                crewInitials = flight.crewInitials, //remove if not necessary

                                flightStopDateTime = flight.flightStopDateTime.GetValueOrDefault().ToUniversalTime(),

                                scheduledFlightId = flight.scheduledFlightId ?? 0,
                                departureFboId = flight.departureFboId ?? 0,

                                arrivalFboId = flight.arrivalFboId ?? 0,
                                arrivalFboName = flight.arrivalFboName,
                                departureFboName = flight.departureFboName,

                                EndDate = getEndDate,

                                TailNumber = getTailnumber,
                            };

                            db.Insert(postflightVm);
                            db.Insert(flight.PostedFlightOooi);
                            db.Insert(flight.automatedFlightData.oooi);
                            Console.WriteLine("Inserted Postflight flightID: " + flight.flightId);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc);
                        }

                    }
                }

            }
        }

        #endregion

    }
}
