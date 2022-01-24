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
using sdcrew.Repositories.PreflightRepos;

using SQLite;

namespace sdcrew.Services.Data
{
    public class PostflightServices
    {
        KeyGenerator _keygen = new KeyGenerator();
        static SQLiteConnection db;

        PostflightRepository postflightRepo;
        PreflightRepository _preflightRepo;

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
                        await InsertBusinessCatagoriesAsync();
                        await InsertDepartmentsAsync();
                        await InsertDelayTypesAsync();
                        await InsertMixRatioTypes();
                        await InsertCrews();
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
                        await InsertCrews();
                        await InsertBusinessCatagoriesAsync();
                        await InsertDelayTypesAsync();
                        await InsertMixRatioTypes();
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
                db.Table<CrewDetailsVM>().Delete(x => x.CrewMemberId != 0);

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

                            FullName = crew.LastName + " " + crew.FirstName,

                            HomeBase = crew.HomeBase,
                            Ron = crew.Ron,
                            Initials = crew.Initials,
                            CrewMemberTypeName = crew.CrewMemberTypeRatings.Select(x => x.CrewMemberTypeName).ToString(),
                        };

                        try
                        {
                            db.Insert(crewDetails);
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
            try
            {
                var crews = db.Table<CrewDetailsVM>().ToList();
                return crews;
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
                return null;
            }
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

        #region Apu N Custom Components

        public async Task<ApuNCustomComponents> FetchApuNComponents(int PostedFlightId, int AircraftProfileId, DateTime FlightStartDateTime)
        {
            await Init();

            var Apu = await postflightRepo.FetchApuNCustomComponentsAsync(PostedFlightId, AircraftProfileId, FlightStartDateTime);

            var Exist = db.Table<ApuNCustomComponents>().FirstOrDefault(x => x.PostedFlightId == PostedFlightId);
            if (Exist != null)
            {
                return Exist;
            }
            else
            {
                db.Insert(Apu);
                return Apu;
            }
        }

        #endregion

        #region Checklists

        public IEnumerable<ChecklistVM> GetAll_ChecklistsByRoleName(int TripID, int CheckListTypeId, string ChecklistRoleTypeName)
        {
            _ = Init();

            var Checklist = db.Table<ChecklistVM>().Where(x => x.checklistRoleTypeName == ChecklistRoleTypeName & x.tripId == TripID & x.checkListTypeId == CheckListTypeId);
            return Checklist;
        }

        public List<ChecklistVM> Get_SelectedChecklists(List<string> cIDs, string ChecklistRoleTypeName, int TripId)
        {
            _ = Init();

            var ConfigIds = cIDs.Select(int.Parse).ToList();

            List<ChecklistVM> checklists = new List<ChecklistVM>();

            foreach (int i in ConfigIds)
            {
                var Checklist = db.Table<ChecklistVM>().Where(x => x.tripId == TripId & x.checklistRoleTypeName == ChecklistRoleTypeName & x.configTaskId.Equals(i)).First();

                checklists.Add(Checklist);
            }
            return checklists;
        }

        public IEnumerable<ChecklistVM> Get_UnSelectedChecklists(List<string> cIDs, string ChecklistRoleTypeName, int TripId)
        {
            _ = Init();

            var ConfigIds = cIDs.Select(int.Parse).ToList();
            List<ChecklistVM> checklists = new List<ChecklistVM>();

            foreach (int i in ConfigIds)
            {
                var Checklist = db.Table<ChecklistVM>().Where(x => x.tripId == TripId && x.checklistRoleTypeName == ChecklistRoleTypeName && x.configTaskId.Equals(i)).First();

                checklists.Add(Checklist);
            }
            return checklists;
        }

        public async Task<bool> AddPreflight_Checklists(int TripId, int CheckListTypeId)
        {
            PreflightChecklist preflightChecklist = new PreflightChecklist();
            SchedulingChecklist schedulingChecklist = new SchedulingChecklist();
            PilotChecklist pilotChecklist = new PilotChecklist();
            MaitenanceChecklist maitenanceChecklist = new MaitenanceChecklist();

            ChecklistVM checklistVM = new ChecklistVM();

            _preflightRepo = new PreflightRepository();

            await Init();

            ClearChecklistVmByAircraft(TripId);

            var checklists = await _preflightRepo.GetAircraftCheckLists(TripId, 2).ConfigureAwait(false);
            if (checklists != null)
            {
                var schedules = checklists.schedulingChecklists;
                if (schedules.Count != 0)
                {
                    foreach (var schedule in schedules)
                    {
                        checklistVM = new ChecklistVM
                        {
                            customerId = schedule.customerId,
                            tripId = schedule.tripId,
                            flightId = schedule.flightId,
                            legNumber = schedule.legNumber,
                            checkListTypeId = schedule.checkListTypeId,
                            checklistRoleTypeId = schedule.checklistRoleTypeId,
                            checklistRoleTypeName = schedule.checklistRoleTypeName,
                            configTaskId = schedule.configTaskId,
                            configTaskName = schedule.configTaskName,
                            configTaskOrder = schedule.configTaskOrder,
                            selectedByUser = schedule.selectedByUser,
                            selectedDateTime = schedule.selectedDateTime,
                            checklistSignId = schedule.checklistSignId
                        };

                        try
                        {
                            var exists = GetAll_ChecklistsByRoleName(TripId, CheckListTypeId, "Scheduling").FirstOrDefault(x => x.configTaskName == schedule.configTaskName);
                            if (exists != null)
                            {
                                UpdateChecklistVm(checklistVM);
                            }
                            else
                            {
                                db.Insert(checklistVM);
                            }
                        }
                        catch (Exception exc) { Crashes.TrackError(exc); }
                    }
                }

                var pilots = checklists.pilotChecklists;
                if (pilots.Count != 0)
                {
                    foreach (var pilot in pilots)
                    {

                        checklistVM = new ChecklistVM
                        {
                            customerId = pilot.customerId,
                            tripId = pilot.tripId,
                            flightId = pilot.flightId,
                            legNumber = pilot.legNumber,
                            checkListTypeId = pilot.checkListTypeId,
                            checklistRoleTypeName = pilot.checklistRoleTypeName,
                            checklistRoleTypeId = pilot.checklistRoleTypeId,
                            configTaskId = pilot.configTaskId,
                            configTaskName = pilot.configTaskName,
                            configTaskOrder = pilot.configTaskOrder,
                            selectedByUser = pilot.selectedByUser,
                            selectedDateTime = pilot.selectedDateTime,
                            checklistSignId = pilot.checklistSignId
                        };

                        try
                        {
                            var exists = GetAll_ChecklistsByRoleName(TripId, CheckListTypeId, "Pilot").FirstOrDefault(x => x.configTaskName == pilot.configTaskName);
                            if (exists != null)
                            {
                                UpdateChecklistVm(checklistVM);
                            }
                            else
                            {
                                db.Insert(checklistVM);
                            }
                        }
                        catch (Exception exc) { Crashes.TrackError(exc); }
                    }
                }

                var maintances = checklists.maitenanceChecklists;
                if (maintances.Count != 0)
                {
                    foreach (var maintance in maintances)
                    {
                        try
                        {
                            checklistVM = new ChecklistVM
                            {
                                customerId = maintance.customerId,
                                tripId = maintance.tripId,
                                flightId = maintance.flightId,
                                legNumber = maintance.legNumber,
                                checkListTypeId = maintance.checkListTypeId,
                                checklistRoleTypeName = maintance.checklistRoleTypeName,
                                checklistRoleTypeId = maintance.checklistRoleTypeId,
                                configTaskId = maintance.configTaskId,
                                configTaskName = maintance.configTaskName,
                                configTaskOrder = maintance.configTaskOrder,
                                selectedByUser = maintance.selectedByUser,
                                selectedDateTime = maintance.selectedDateTime,
                                checklistSignId = maintance.checklistSignId
                            };

                            var exists = GetAll_ChecklistsByRoleName(TripId, CheckListTypeId, "Maintenance").FirstOrDefault(x => x.configTaskName == maintance.configTaskName);
                            if (exists != null)
                            {
                                UpdateChecklistVm(checklistVM);
                            }
                            else
                            {
                                db.Insert(checklistVM);
                            }
                        }
                        catch (Exception exc) { Crashes.TrackError(exc); }
                    }
                }
            }

            return true;
        }

        internal void UpdateChecklistVm(ChecklistVM Checklist)
        {
            var getCheckItemj = db.Table<ChecklistVM>().Delete(x => x.legNumber == Checklist.legNumber & x.checklistRoleTypeName == Checklist.checklistRoleTypeName & x.tripId == Checklist.tripId & x.configTaskName == Checklist.configTaskName && x.configTaskId == Checklist.configTaskId);
            db.Insert(Checklist);
        }

        internal void ClearChecklistVmByRole(string RoleName)
        {
            var delete = db.Table<ChecklistVM>().Delete(x => x.checklistRoleTypeName == RoleName);
        }

        internal void ClearChecklistVmByAircraft(int SchAircraftID)
        {
            var delete = db.Table<ChecklistVM>().Delete(x => x.tripId == SchAircraftID);
        }


        public async Task<int> CheckStatusChecklist(PostFlightVM flight, string v)
        {
            var AllChecklist = db.Table<ChecklistVM>().Where(x => x.checklistRoleTypeName == v & x.tripId == flight.tripId).ToList(); //& x.checkListTypeId == CheckListTypeId

            var SavedChecklist = db.Table<ChecklistVM>().Where(x => x.checklistRoleTypeName == v & x.tripId == flight.tripId & x.selectedByUser != "").ToList(); //& x.checkListTypeId == CheckListTypeId

            await Task.Delay(1);

            if (AllChecklist.Count() > 0)
            {
                if (SavedChecklist.Count() == 0)
                {
                    return 0;  //red
                }
                else
                {
                    if (AllChecklist.Count() == SavedChecklist.Count())
                    {
                        return 1; //green
                    }
                    else
                    {
                        return 2; //yellow
                    }
                }
            }
            else
            {
                return 3; //NA
            }
        }

        #endregion

        #region PostedFlight Additional

        public async Task InsertBusinessCatagoriesAsync()
        {
            await Init();

            var businessCatagories = await postflightRepo.FetchBusinessCatagoriesAsync();
            if (businessCatagories != null)
            {
                db.Table<BusinessCatagory>().Delete(x => x.Id != 0);
                foreach (var item in businessCatagories)
                {
                    db.Insert(item);
                }
            }
        }

        public async Task<List<BusinessCatagory>> GetBusinessCatagoriesAsync()
        {
            await Init();
            return db.Table<BusinessCatagory>().ToList();
        }

        public async Task<BusinessCatagory> GetBusinessCatagoryAsync(int BusinesscatagoryId)
        {
            await Init();
            var businessCatagory = db.Table<BusinessCatagory>().FirstOrDefault(x => x.Id == BusinesscatagoryId);
            return businessCatagory;
        }

        public async Task InsertDelayTypesAsync()
        {
            await Init();

            var delayTypes = await postflightRepo.FetchDelayTypesAsync();
            if (delayTypes != null)
            {
                db.Table<DelayType>().Delete(x => x.Id != 0);
                foreach (var item in delayTypes)
                {
                    db.Insert(item);
                }
            }
        }

        public async Task<List<DelayType>> GetDelayTypesAsync()
        {
            await Init();
            return db.Table<DelayType>().ToList();
        }

        public async Task<DelayType> GetDelayTypeAsync(int DelayTypeId)
        {
            await Init();
            var businessCatagory = db.Table<DelayType>().FirstOrDefault(x => x.Id == DelayTypeId);
            return businessCatagory;
        }

        public async Task InsertDepartmentsAsync()
        {
            await Init();

            var departments = await postflightRepo.FetchDepartmentsAsync();
            foreach (var item in departments)
            {
                db.Insert(item);
            }
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            await Init();
            return db.Table<Department>().ToList();
        }

        public async Task<Department> GetDepartmentAsync(int DepartmentId)
        {
            await Init();
            var businessCatagory = db.Table<Department>().FirstOrDefault(x => x.Id == DepartmentId);
            return businessCatagory;
        }

        public async Task<PostedFlightAdditional> PostedFlightAdditionalAsync(int PostedFlightId)
        {
            await Init();
            var businessCatagory = db.Table<PostedFlightAdditional>().FirstOrDefault(x => x.postedFlightId == PostedFlightId);
            return businessCatagory;
        }

        #endregion

        #region OilsNFluids

        public async Task<List<Fluid>> GetFluidsAsync()
        {
            await Init();
            return db.Table<Fluid>().ToList();
        }

        #endregion

        #region DeAntiIce

        public async Task InsertMixRatioTypes()
        {
            await Init();
            var mixTypes = await postflightRepo.FetchMixTypesAsync();
            db.Table<MixType>().Delete(x => x.id != 0);
            foreach (var item in mixTypes)
            {
                db.Insert(item);
            }
        }

        public async Task<List<MixType>> MixTypesAsync()
        {
            await Init();
            var types = db.Table<MixType>().ToList();
            return types;
        }

        public async Task<PostedFlightDeIce> PostedFlightDeIceAsync(int PostedFlightId)
        {
            await Init();
            return db.Table<PostedFlightDeIce>().FirstOrDefault(x => x.postedFlightId == PostedFlightId);
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
            var flights = Typewiseflights.Where(x => !filteritemsList.Contains(x.TailNumber)).OrderByDescending(x => x.StartDate).ToList();

            return flights;
        }

        public List<PostFlightVM> GetPostFlightsByType(string type)
        {
            _ = Init();

            var flights = db.Table<PostFlightVM>().Where(x => x.postFlightStatusName == type).OrderByDescending(x => x.StartDate).ToList();
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

                            if (flight.postedFlightDeIce != null)
                            {
                                var deIce = new PostedFlightDeIce
                                {
                                    postedFlightId = flight.postedFlightId,
                                    deIceStartDateTime = flight.postedFlightDeIce.deIceStartDateTime,
                                    deIceEndDateTime = flight.postedFlightDeIce.deIceEndDateTime,
                                    deIceMixRatioTypeId = flight.postedFlightDeIce.deIceMixRatioTypeId,
                                    deIceTypeId = flight.postedFlightDeIce.deIceTypeId
                                };

                                db.Insert(deIce);
                            }

                            if (flight.fluids != null)
                            {
                                foreach (var fluid in flight.fluids)
                                {
                                    db.Insert(fluid);
                                }
                            }

                            if (flight.PostedFlightOooi != null)
                            {
                                var oooi = new Oooi
                                {
                                    postedFlightId = flight.postedFlightId,
                                    automatedOutTime = flight.automatedFlightData.oooi.automatedOutTime,
                                    automatedOffTime = flight.automatedFlightData.oooi.automatedOffTime,
                                    automatedOnTime = flight.automatedFlightData.oooi.automatedOnTime,
                                    automatedInTime = flight.automatedFlightData.oooi.automatedInTime,
                                };

                                db.Insert(oooi);
                            }

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
                            db.Insert(flight.postedFlightFuel);
                            db.Insert(flight.postedFlightAdditional);

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
                            var postflightVm = new PostFlightVM
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
                                //Console.WriteLine(flight.postFlightStatusName);

                                var Existing = await GetPostFlight(flight.postedFlightId);

                                if (Existing == null)
                                {
                                    if (flight.fluids != null)
                                    {
                                        foreach (var fluid in flight.fluids)
                                        {
                                            db.Insert(fluid);
                                        }
                                    }
                                    if (flight.PostedFlightOooi != null)
                                    {
                                        var oooi = new Oooi
                                        {
                                            postedFlightId = flight.postedFlightId,
                                            automatedOutTime = flight.automatedFlightData.oooi.automatedOutTime,
                                            automatedOffTime = flight.automatedFlightData.oooi.automatedOffTime,
                                            automatedOnTime = flight.automatedFlightData.oooi.automatedOnTime,
                                            automatedInTime = flight.automatedFlightData.oooi.automatedInTime,
                                        };

                                        db.Insert(oooi);
                                    }
                                    if (flight.flightPassengers != null)
                                    {
                                        foreach (var flightpassenger in flight.flightPassengers)
                                        {
                                            db.Insert(flightpassenger);
                                        }
                                    }

                                    db.Insert(postflightVm);
                                    db.Insert(flight.postedFlightFuel);
                                    db.Insert(flight.postedFlightAdditional);

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
