using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using ChoETL;

using Microsoft.AppCenter.Crashes;

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
            if (IsBusy == false)
            {
                IsBusy = true;

                await AddPostflight_AircraftProfileDTos()
                    .ContinueWith(async x =>
                    {
                        await AddAllPostflights();
                    });

                IsBusy = false;
            }
        }

        public async Task RefreshDatas()
        {
            if (IsBusy == false)
            {
                IsBusy = true;

                await AddPostflight_AircraftProfileDTos()
                    .ContinueWith(async x =>
                    {
                        await RefreshPostflights();
                    });


                IsBusy = false;
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
                catch (Exception exc) { Crashes.TrackError(exc); }
            }

            return hex;
        }

        #endregion

        #region Crews Logbook

        public async Task InsertCrews()
        {

            CrewDetailsVM crewDetails;

            await Init();

            var AllCrews = await postflightRepo.FetchAllCrews();

            if (AllCrews != null)
            {
                //Clear Table before Insertion
                var dltStatusCrews = db.Table<CrewDetailsVM>().Delete(x => x.CrewMemberId != 0);


                foreach (var crew in AllCrews)
                {
                    if (crew.CrewMemberId != 0)
                    {
                        crewDetails = new CrewDetailsVM
                        {
                            CrewMemberId = crew.CrewMemberId,
                            Active = crew.Active,
                            PersonId = crew.PersonId,
                            BusinessPersonId = crew.BusinessPersonId,
                            FirstName = crew.FirstName,
                            LastName = crew.LastName,
                            HomeBase = crew.HomeBase,
                            Ron = crew.Ron,
                            Initials = crew.Initials,
                            CrewMemberTypeName = crew.CrewMemberTypeRatings.Select(x => x.CrewMemberTypeName).ToString(),
                        };

                        try
                        {
                            db.Insert(crew);
                        }
                        catch (Exception exc)
                        { Crashes.TrackError(exc); }
                    }
                }

            }
        }

        public async Task<List<CrewDetailsVM>> GetCrewsAsync()
        {
            await Init();
            var crews = db.Table<CrewDetailsVM>().ToList();
            return crews;
        }

        public async Task<List<FlightCrewMember>> GetFlightCrewMembersAsync(int FlightId)
        {
            await Init();
            var crews = db.Table<FlightCrewMember>().Where(x => x.flightId == FlightId).ToList();
            return crews;
        }

        public async Task<Logbook> GetLogbookAsync(int FlightCrewMemberId)
        {
            await Init();
            var logbook = db.Table<Logbook>().FirstOrDefault(x => x.flightCrewMemberId == FlightCrewMemberId);
            return logbook;
        }

        #endregion



        #region Oooi

        public Oooi GetOooiDetails(int PostedFlightId)
        {
            _ = Init();

            var item = db.Table<Oooi>().FirstOrDefault(x => x.postedFlightId == PostedFlightId);
            return item;
        }

        public async Task InsertUpdateOooiLocal(Oooi oooi)
        {
            await Init();

            var Existing = GetOooiDetails(oooi.postedFlightId);
            if (Existing != null)
            {
                Existing.postedFlightId = oooi.postedFlightId;
                Existing.IsUpdated = oooi.IsUpdated;
                Existing.HasLocalModification = oooi.HasLocalModification;
                Existing.automatedInTime = oooi.automatedInTime;
                Existing.automatedOffTime = oooi.automatedOffTime;
                Existing.automatedOnTime = oooi.automatedOnTime;
                Existing.automatedOutTime = oooi.automatedOutTime;

                db.Update(Existing);
            }
            else
            {
                db.Insert(oooi);
            }
        }

        #endregion

        #region Fuel

        public async Task<PostedFlightFuel> GetFuelDetails(int PostedFlightId)
        {
            await Init();

            var item = db.Table<PostedFlightFuel>().FirstOrDefault(x => x.postedFlightId == PostedFlightId);
            return item;
        }

        public async Task InsertUpdateFuelLocal(PostedFlightFuel fuel)
        {
            await Init();

            var Existing = await GetFuelDetails(fuel.postedFlightId);
            if (Existing != null)
            {
                Existing.postedFlightId = fuel.postedFlightId;
                Existing.IsUpdated = fuel.IsUpdated;
                Existing.HasLocalModification = fuel.HasLocalModification;

                Existing.fuelOut = fuel.fuelOut;
                Existing.fuelIn = fuel.fuelIn;
                Existing.fuelOff = fuel.fuelOff;
                Existing.fuelOn = fuel.fuelOn;

                db.Update(Existing);
            }
            else
            {
                db.Insert(fuel);
            }
        }


        #endregion

        #region Passengers

        public async Task<List<PostFlightPassenger>> FlightPassengersAsync(int FlightID)
        {
            await Init();

            var passengers = db.Table<PostFlightPassenger>().Where(x => x.flightId == FlightID).ToList();
            return passengers;
        }




        #endregion

        #region Airports

        public List<AllAirports> GetAllAirports()
        {
            return GetAirportList(AllAirportsJsonfile);
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
            catch (Exception exc)
            { Crashes.TrackError(exc); }
        }

        #endregion

        #region AllPostFlights

        public async Task AddPostflight_AircraftProfileDTos()
        {
            AircraftProfileDto aircraftProfileDto = new AircraftProfileDto();

            await Init();

            var getAircraftProfileDtos = await postflightRepo.FetchPostflightAircraftDTOs();
            try
            {
                if (getAircraftProfileDtos.Count != 0)
                {
                    var dltStatusDTos = db.Table<AircraftProfileDto>().Delete(x => x.aircraftProfileId != 0);
                    foreach (var profile in getAircraftProfileDtos)
                    {
                        var dto = new AircraftProfileDto
                        {
                            aircraftProfileId = profile.aircraftProfileId,
                            tailNumber = profile.tailNumber,
                            serialNumber = profile.serialNumber,
                        };
                        db.Insert(dto);

                        //Console.WriteLine("added Profile profileID " + profile.aircraftProfileId);
                    }
                }
            }
            catch (Exception exc)
            { Crashes.TrackError(exc); }
        }

        public async Task<string> FetchTailNumber(int aircraftProfileId)
        {
            await Init();
            string tail = db.Table<AircraftProfileDto>().Where(x => x.aircraftProfileId == aircraftProfileId).Select(x => x.tailNumber).FirstOrDefault();
            return tail;
        }

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

        public async Task<PostFlightVM> GetPostFlight(int PostedFlightID)
        {
            await Init();

            var item = db.Table<PostFlightVM>().FirstOrDefault(x => x.postedFlightId == PostedFlightID);
            return item;
        }

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
                //OOOI
                var dltStatusPostFlightoooi = db.Table<PostedFlightOooi>().Delete(x => x.postedFlightId != 0);
                var dltStatusOOOI = db.Table<Oooi>().Delete(x => x.postedFlightId != 0);
                var dltFlightCrews = db.Table<CrewDetailsVM>().Delete(x => x.CrewMemberId != 0);
                var dltLogs = db.Table<Logbook>().Delete(x => x.logbookId != 0);
                var dltApproch = db.Table<LogbookApproach>().Delete(x => x.logbookApproachId != 0);
                var dltCrews = db.Table<Postflight_CrewMember>().Delete(x => x.crewMemberId != 0);

                foreach (var flight in AllPostflights)
                {
                    if (flight.flightId != 0)
                    {
                        getTailnumber = await FetchTailNumber(flight.aircraftProfileId);

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

                                oooi = new Oooi
                                {
                                    postedFlightId = flight.postedFlightId,
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

                            if (flight.flightPassengers != null)
                            {
                                foreach (var passenger in flight.flightPassengers)
                                {
                                    var passengerDetails = new PostFlightPassenger
                                    {
                                        flightPassengerId = passenger.flightPassengerId,
                                        flightId = passenger.flightId,
                                        passengerId = passenger.passengerId,
                                        passengerStatusId = passenger.passengerStatusId,
                                        isLeadPassenger = passenger.isLeadPassenger,

                                        businessPersonId = passenger.passenger.businessPersonId,
                                        reserved = passenger.passenger.reserved,
                                        firstName = passenger.passenger.firstName,
                                        lastName = passenger.passenger.lastName,
                                    };

                                    db.Insert(passengerDetails);
                                }
                            }
                            db.Insert(postflightVm);
                            db.Insert(flight.PostedFlightOooi);
                            db.Insert(postflightVm.oooi);
                            db.Insert(flight.postedFlightFuel);
                            if (flight.flightCrewMember.Count != 0)
                            {
                                foreach (var flightcrew in flight.flightCrewMember)
                                {
                                    db.Insert(flightcrew);
                                    db.Insert(flightcrew.logbook);
                                    db.Insert(flightcrew.crewMember);
                                    db.Insert(flightcrew.logbook.logbookApproaches);
                                }
                            }

                            Console.WriteLine("Inserted Postflight flightID: " + flight.flightId);
                        }
                        catch (Exception exc)
                        {
                            Crashes.TrackError(exc);
                        }

                    }
                }

            }
        }

        //Delete Avoided While Refreshing
        public async Task RefreshPostflights()
        {
            string getTailnumber = "";
            string getEndDate = "N/A";

            PostFlightVM postflightVm = new PostFlightVM();

            await Init();

            var AllPostflights = await postflightRepo.GetAllPostFlightEvents();
            if (AllPostflights != null)
            {
                foreach (var flight in AllPostflights)
                {
                    if (flight.flightId != 0)
                    {
                        getTailnumber = await FetchTailNumber(flight.aircraftProfileId);

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

                                oooi = new Oooi
                                {
                                    postedFlightId = flight.postedFlightId,
                                    automatedOutTime = flight.automatedFlightData.oooi.automatedOutTime,
                                    automatedOffTime = flight.automatedFlightData.oooi.automatedOffTime,
                                    automatedOnTime = flight.automatedFlightData.oooi.automatedOnTime,
                                    automatedInTime = flight.automatedFlightData.oooi.automatedInTime,
                                },

                                postedFlightAdditional = flight.postedFlightAdditional, //model

                                //crewInitials = flight.crewInitials, //remove if not necessary

                                flightStopDateTime = flight.flightStopDateTime.GetValueOrDefault().ToUniversalTime(),

                                scheduledFlightId = flight.scheduledFlightId ?? 0,
                                departureFboId = flight.departureFboId ?? 0,

                                arrivalFboId = flight.arrivalFboId ?? 0,
                                arrivalFboName = flight.arrivalFboName,
                                departureFboName = flight.departureFboName,

                                EndDate = getEndDate,

                                TailNumber = getTailnumber,
                            };

                            try
                            {
                                Console.WriteLine(flight.postFlightStatusName);

                                var Existing = await GetPostFlight(flight.postedFlightId);

                                if (Existing == null)
                                {
                                    if (flight.flightPassengers != null)
                                    {
                                        foreach (var flightpassenger in flight.flightPassengers)
                                        {
                                            db.Insert(flightpassenger);
                                        }
                                    }
                                    db.Insert(postflightVm);
                                    db.Insert(flight.PostedFlightOooi);
                                    db.Insert(postflightVm.oooi);
                                    db.Insert(flight.postedFlightFuel);
                                    if (flight.flightCrewMember.Count != 0)
                                    {
                                        foreach (var flightcrew in flight.flightCrewMember)
                                        {
                                            db.Insert(flightcrew);
                                            db.Insert(flightcrew.logbook);
                                            db.Insert(flightcrew.crewMember);


                                            if (flightcrew.logbook.logbookApproaches.Count != 0)
                                            {
                                                foreach (var approch in flightcrew.logbook.logbookApproaches)
                                                {
                                                    db.Insert(approch);
                                                }
                                            }

                                        }
                                    }


                                }

                                Console.WriteLine("Refreshed Postflight flightID: " + flight.flightId);
                            }
                            catch (Exception exc)
                            {
                                Crashes.TrackError(exc);
                            }


                        }
                        catch (Exception exc)
                        {
                            Crashes.TrackError(exc);
                        }

                    }
                }

            }
        }

        #endregion

    }
}
