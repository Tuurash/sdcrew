﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using sdcrew.Models;
using sdcrew.Repositories.PreflightRepos;
using SQLite;

namespace sdcrew.Services.Data
{
    public class PreflightServices
    {
        KeyGenerator _keygen = new KeyGenerator();
        static SQLiteConnection db;

        PreflightRepository _preflightRepo;

        string colorString = "";

        public PreflightServices()
        {
            _keygen.CheckKey();
            _preflightRepo = new PreflightRepository();

            //InsertDatas();
        }

        public async Task InsertDatas()
        {
            var t1 = AddAircraftProfileDtos();
            //var t2= AddStaffEventTypes();
            var t3 = AddPreflightACE();
            var t4 = AddPreflightSE();
            var t5 = AddNotes();

            await Task.WhenAll(t1, t3, t4, t5).ConfigureAwait(false);
            //MessagingCenter.Send<string, string>("MyApp", "DBUpdated", "From AllpreflightViewModel");
        }

        private async Task Init()
        {
            db = await DB_Init.Init();
        }

        public IEnumerable<PreflightVM> GetAllPreflights()
        {
            _ = Init();
            var Preflights = db.Table<PreflightVM>().OrderBy(x => x.date);
            return Preflights;
        }

        public async Task<IEnumerable<PreflightVM>> GetPreflights()
        {
            await Init();

            var Preflights = db.Table<PreflightVM>().ToList();
            return Preflights;
        }

        public IEnumerable<string> getTailnumbers()
        {
            var TailNumbers = db.Table<PreflightVM>().Where(x => x.eventName == "Aircraft").GroupBy(x => x.tailNumber).Select(x => x.First().tailNumber);
            return TailNumbers;
        }

        #region Notes

        public IEnumerable<PreflightNote> GetNotes()
        {
            _ = Init();

            var Notes = db.Table<PreflightNote>().OrderBy(x => x.createdDate);
            return Notes;
        }

        public PreflightNote GetNoteByDate(DateTime Date)
        {
            _ = Init();

            var Note = db.Table<PreflightNote>().Where(x => x.calendarDate == Date.Date).FirstOrDefault();
            return Note;
        }

        internal PreflightNote getNoteByID(int noteId)
        {
            _ = Init();

            var Note = db.Table<PreflightNote>().Where(x => x.calendarNoteId == noteId).FirstOrDefault();
            return Note;
        }

        public async Task AddNotes()
        {
            PreflightNote preflightNote = new PreflightNote();
            await Init();
            var Notes = await _preflightRepo.GetPreflightNotes().ConfigureAwait(false);
            if (Notes != null)
            {
                foreach (var Note in Notes)
                {
                    try
                    {
                        preflightNote = new PreflightNote
                        {
                            calendarNoteId = Note.calendarNoteId,
                            note = Note.note,
                            customerId = Note.customerId,
                            calendarDate = Note.calendarDate.Date,
                            createdByUser = Note.createdByUser,
                            createdDate = Note.createdDate,
                            modifiedByUser = Note.modifiedByUser,
                            modifiedDate = Note.modifiedDate,

                            IsVisible = "True"
                        };

                        try
                        {
                            var Exists = GetNotes().FirstOrDefault(x => x.calendarNoteId == Note.calendarNoteId);
                            if (Exists != null)
                            {

                            }
                            else
                            {
                                db.Insert(preflightNote);
                                Console.WriteLine("Added Note: " + Note.calendarNoteId);
                            }
                        }
                        catch (Exception exc) { Console.WriteLine(exc); }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                    }
                }
            }

        }

        #endregion

        #region preflightDetails

        public PreflightVM GetFlightInfo(int _aircraftId)
        {
            PreflightVM flight = db.Table<PreflightVM>().Where(x => x.aircraftId == _aircraftId).First();
            return flight;
        }

        public FboBasicInfo GetFboInformation(int AircraftId)
        {
            var flight = db.Table<FboBasicInfo>().Where(x => x.aircraftId == AircraftId).FirstOrDefault();
            return flight;
        }

        public IEnumerable<FlightCrewMember> GetCrewMembers(int aircraftId, int LegNumber, int FlightId, int ScheduledAircraftTripId) //
        {
            var crew = db.Table<FlightCrewMember>().ToList();

            var crews = db.Table<FlightCrewMember>().Where(x => x.ScheduledAircraftTripId == ScheduledAircraftTripId & x.CrewFlightLegNumber == LegNumber); // & x.aircraftLegNumber==LegNumber
            return crews;
        }

        public IEnumerable<FlightPassenger> GetPassengers(int aircraftId, int LegNumber, int FlightId, int ScheduledAircraftTripId)
        {
            //var allCrews = db.Table<FlightPassenger>().ToList();

            var passengers = db.Table<FlightPassenger>().Where(x => x.ScheduledAircraftTripId == ScheduledAircraftTripId & x.aircraftLegNumber == LegNumber); // & x.aircraftLegNumber==LegNumber
            return passengers;
        }

        public int GetPassengersOnboard(int ScheduledAircraftTripId, int LegNumber)
        {
            var OnboardCount = db.Table<FlightPassenger>().Count(x => x.ScheduledAircraftTripId == ScheduledAircraftTripId & x.aircraftLegNumber == LegNumber & x.passengerStatusId == 2);
            return OnboardCount;
        }

        #endregion

        #region Checklists


        public IEnumerable<ChecklistVM> GetAll_ChecklistsByRoleName(int SchAircraftID, int LegNumber, int CheckListTypeId, int FlightId, string ChecklistRoleTypeName)
        {
            _ = Init();

            var r = db.Table<ChecklistVM>().Where(x=>x.checklistRoleTypeName== ChecklistRoleTypeName).ToList();

            var Checklist = db.Table<ChecklistVM>().Where(x => x.checklistRoleTypeName == ChecklistRoleTypeName & x.tripId == SchAircraftID & x.checkListTypeId == CheckListTypeId & x.legNumber==LegNumber);
            return Checklist;
        }

        public List<ChecklistVM> Get_SelectedChecklists(List<string> cIDs, string ChecklistRoleTypeName, int AircraftID, int LegNumber)
        {
            _ = Init();

            var ConfigIds = cIDs.Select(int.Parse).ToList();

            List<ChecklistVM> checklists = new List<ChecklistVM>();

            foreach (int i in ConfigIds)
            {
                var Checklist = db.Table<ChecklistVM>().Where(x => x.tripId == AircraftID & x.checklistRoleTypeName == ChecklistRoleTypeName & x.legNumber==LegNumber & x.configTaskId.Equals(i)).First();

                checklists.Add(Checklist);
            }
            return checklists;
        }

        public IEnumerable<ChecklistVM> Get_UnSelectedChecklists(List<string> cIDs, string ChecklistRoleTypeName, int AircraftID, int LegNumber)
        {
            _ = Init();

            var ConfigIds = cIDs.Select(int.Parse).ToList();
            List<ChecklistVM> checklists = new List<ChecklistVM>();

            foreach (int i in ConfigIds)
            {
                var Checklist = db.Table<ChecklistVM>().Where(x => x.tripId == AircraftID && x.checklistRoleTypeName == ChecklistRoleTypeName && x.legNumber == LegNumber && x.configTaskId.Equals(i)).First();

                checklists.Add(Checklist);
            }
            return checklists;
        }

        public async Task<bool> AddPreflight_Checklists(int aircraftID, int LegNumber, int CheckListTypeId, int FlightId)
        {
            PreflightChecklist preflightChecklist = new PreflightChecklist();
            SchedulingChecklist schedulingChecklist = new SchedulingChecklist();
            PilotChecklist pilotChecklist = new PilotChecklist();
            MaitenanceChecklist maitenanceChecklist = new MaitenanceChecklist();

            ChecklistVM checklistVM = new ChecklistVM();

            await Init();

            ClearChecklistVmByAircraft(aircraftID);

            var checklists = await _preflightRepo.GetPreflightAircraftCheckLists(aircraftID).ConfigureAwait(false);
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
                            var exists = GetAll_ChecklistsByRoleName(aircraftID, LegNumber, CheckListTypeId, FlightId, "Scheduling").FirstOrDefault(x => x.configTaskName == schedule.configTaskName);
                            if (exists != null)
                            {
                                UpdateChecklistVm(checklistVM);
                            }
                            else
                            {
                                db.Insert(checklistVM);
                            }
                        }
                        catch (Exception exc) { Console.WriteLine(exc); }
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
                            var exists = GetAll_ChecklistsByRoleName(aircraftID, LegNumber, CheckListTypeId, FlightId, "Pilot").FirstOrDefault(x => x.configTaskName == pilot.configTaskName);
                            if (exists != null)
                            {
                                UpdateChecklistVm(checklistVM);
                            }
                            else
                            {
                                db.Insert(checklistVM);
                            }
                        }
                        catch (Exception exc) { Console.WriteLine(exc); }
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

                            var exists = GetAll_ChecklistsByRoleName(aircraftID, LegNumber, CheckListTypeId, FlightId, "Maintenance").FirstOrDefault(x => x.configTaskName == maintance.configTaskName);
                            if (exists != null)
                            {
                                UpdateChecklistVm(checklistVM);
                            }
                            else
                            {
                                db.Insert(checklistVM);
                            }
                        }
                        catch (Exception exc) { Console.WriteLine(exc); }
                    }
                }
            }

            return true;
        }

        internal void UpdateChecklistVm(ChecklistVM Checklist)
        {

            var getCheckItemj = db.Table<ChecklistVM>().Delete(x => x.legNumber == Checklist.legNumber & x.checklistRoleTypeName == Checklist.checklistRoleTypeName & x.tripId == Checklist.tripId & x.configTaskName == Checklist.configTaskName && x.configTaskId==Checklist.configTaskId);
            db.Insert(Checklist);
        }

        internal void ClearChecklistVmByRole(string RoleName)
        {
            var delete = db.Table<ChecklistVM>().Delete(x =>x.checklistRoleTypeName==RoleName);
        }

        internal void ClearChecklistVmByAircraft(int SchAircraftID)
        {
            var delete = db.Table<ChecklistVM>().Delete(x => x.tripId== SchAircraftID);
        }


        //public int GetChecklistStatus(string ChecklistRoleTypeName, int AircraftID)
        //{
        //    var Checklist = db.Table<ChecklistVM>().Where(x => x.tripId == AircraftID & x.checklistRoleTypeName == ChecklistRoleTypeName);
        //}

        #endregion

        #region Filtering_Preflight

        public IEnumerable<PreflightVM> GetFilteredFlights(List<string> filteritemsList)
        {
            _ = Init();

            var Preflights = db.Table<PreflightVM>().Where(x => !filteritemsList.Contains(x.tailNumber));
            //var Preflights = db.Table<PreflightVM>().Except(FilterPreflights);

            return Preflights;
        }

        public IEnumerable<PreflightVM> GetFilteredAircraftFlights(List<string> filteritemsList)
        {
            _ = Init();

            var Preflights = GetAllAircraftEventsPreflights().Where(x => !filteritemsList.Contains(x.tailNumber));
            return Preflights;
        }

        public IEnumerable<PreflightVM> GetFilteredStaffFlights(List<string> filteritemsList)
        {
            _ = Init();

            var Preflights = GetAllStaffPreflights().Where(x => !filteritemsList.Contains(x.tailNumber));
            return Preflights;
        }

        #endregion

        #region PreflightServices
        
        public async Task AddServices_Preflight(int ScheduledFlightId)
        {
            //AircraftSevices aircraftSevices = new AircraftSevices();
            Crew _crew = new Crew();
            Rental rental = new Rental();
            Hotel hotel = new Hotel();
            VService Service = new VService();

            await Init();

            var aircraftServices = await _preflightRepo.GetAircraftServices(ScheduledFlightId).ConfigureAwait(false);  

            if (aircraftServices != null)
            {
                if (aircraftServices.rental != null)
                {
                    foreach(var crew in aircraftServices.rental.crew)
                    {

                        _crew = new Crew
                        {
                            vendorName = crew.vendorName,
                            phone = crew.phone,
                            url = crew.url,
                            urlHref = crew.urlHref,
                            phoneFormattedText = crew.phoneFormattedText,

                            notes=crew.notes,
                            generalNotes=crew.generalNotes,

                            ScheduledFlightId = ScheduledFlightId,
                            ServiceType = "Rental",
                        };

                        try
                        {
                            var exists = db.Table<Crew>().Select(x => x.ScheduledFlightId == ScheduledFlightId & x.ServiceType == "Rental").FirstOrDefault();
                            if (exists== true)
                            {

                            }
                            else
                            {
                                db.Insert(_crew);
                            }
                        }
                        catch (Exception) { }
                    }

                   
                }

                if (aircraftServices.hotel != null)
                {
                    foreach (var crew in aircraftServices.hotel.crew)
                    {
                        _crew = new Crew
                        {
                            vendorName = crew.vendorName,
                            phone = crew.phone,
                            url = crew.url,
                            urlHref = crew.urlHref,
                            phoneFormattedText = crew.phoneFormattedText,

                            notes = crew.notes,
                            generalNotes = crew.generalNotes,

                            ScheduledFlightId = ScheduledFlightId,
                            ServiceType = "Hotel",
                        };

                        try
                        {
                            var exists = db.Table<Crew>().Select(x => x.ScheduledFlightId == ScheduledFlightId & x.ServiceType == "Hotel").FirstOrDefault();
                            if (exists == true)
                            {

                            }
                            else
                            {
                                db.Insert(_crew);
                            }
                        }
                        catch (Exception) { }
                    }

                    
                }
                    
                if (aircraftServices.catering != null)
                {
                    foreach (var crew in aircraftServices.hotel.crew)
                    {

                        _crew = new Crew
                        {
                            vendorName = crew.vendorName,
                            phone = crew.phone,
                            url = crew.url,
                            urlHref = crew.urlHref,
                            phoneFormattedText = crew.phoneFormattedText,

                            notes = crew.notes,
                            generalNotes = crew.generalNotes,

                            ScheduledFlightId = ScheduledFlightId,
                            ServiceType = "Catering",
                        };

                        try
                        {
                            var exists = db.Table<Crew>().Select(x => x.ScheduledFlightId == ScheduledFlightId & x.ServiceType == "Catering").FirstOrDefault();
                            if (exists == true)
                            {

                            }
                            else
                            {
                                db.Insert(_crew);

                            }
                        }
                        catch (Exception) { }
                    }

                    
                }
            }
            else
            {
       
            }
        }

        public IEnumerable<Crew> getServiceCrews(int ScheduledFlightId, string serviceType)
        {
            Init();

            var ServiceCrews = db.Table<Crew>().Where(x=>x.ServiceType==serviceType & x.ScheduledFlightId==ScheduledFlightId).ToList();
            return ServiceCrews;
        }


        #endregion

        #region AircraftEvents

        public IEnumerable<PreflightVM> GetAllAircraftEventsPreflights()
        {
            _ = Init();

            var AircraftEvents = db.Table<PreflightVM>().Where(x => x.eventName == "Aircraft").OrderBy(x => x.date);
            return AircraftEvents;
        }

        public async Task<IEnumerable<AircraftProfileDto>> GetAircraftProfiles()
        {
            await Init();

            var aircraftProfiles = db.Table<AircraftProfileDto>().ToList();
            return aircraftProfiles;
        }

        public string GetTailNumberByAircraftProfile(int AircraftId)
        {
            string tailNumber = "";

            try
            {
                tailNumber = db.Table<AircraftProfileDto>().Where(x => x.aircraftProfileId == AircraftId).Select(x => x.tailNumber).FirstOrDefault();
            }
            catch (Exception) { }

            return tailNumber;
        }

        public async Task AddAircraftProfileDtos()
        {
            AircraftProfileDto _aircraftProfileDto = new AircraftProfileDto();

            
            if(GetAircraftProfiles()==null)
            {
                await Init();
                var aircraftProfiles = await _preflightRepo.GetAircraftProfileDtos().ConfigureAwait(false);
                if (aircraftProfiles != null)
                {
                    foreach (var aircraft in aircraftProfiles)
                    {
                        try
                        {
                            _aircraftProfileDto = new AircraftProfileDto
                            {
                                aircraftProfileId = aircraft.aircraftProfileId,
                                customerId = aircraft.customerId,
                                customerName = aircraft.customerName,
                                displayOrder = aircraft.displayOrder,
                                color = aircraft.color,
                                tailNumber = aircraft.tailNumber,
                                modelName = aircraft.modelName,

                                active = aircraft.active,
                                sdoRecordId = aircraft.sdoRecordId,
                                aircraftTypeRatingId = aircraft.aircraftTypeRatingId

                                //Others are being avoided for now
                            };

                            try
                            {
                                var Exists = GetAircraftProfiles().Result.FirstOrDefault(x => x.aircraftProfileId == _aircraftProfileDto.aircraftProfileId);
                                if (Exists != null)
                                {

                                }
                                else
                                {
                                    db.Insert(_aircraftProfileDto);
                                    Console.WriteLine("Added Aircraft Event Profie: " + _aircraftProfileDto.aircraftProfileId);
                                }
                            }
                            catch (Exception exc) { Console.WriteLine(exc); }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc);
                        }
                    }
                }
            }
            

        }

        public async Task AddPreflightACE()
        {
            PreflightVM _preflightVM = new PreflightVM();
            FboBasicInfo fboInfo = new FboBasicInfo();
            FlightPassenger flightPassenger = new FlightPassenger();
            FlightCrewMember flightCrewMember = new FlightCrewMember();


            var dltStatusCrew = db.Table<FlightCrewMember>().Delete(x => x.flightId != 0);
            var dltPassengers = db.Table<FlightPassenger>().Delete(x => x.flightId != 0);

            string getTempArrivalICao="";

            await Init();

            var preflightACEvents = await _preflightRepo.GetPreflightAircraftEvents();

            if (preflightACEvents != null)
            {
                foreach (var flight in preflightACEvents)
                {
                    if(flight.legs.Count()!=0)
                    {
                        foreach (var leg in flight.legs)
                        {
                            try
                            {
                                if (leg.color!= null)
                                {
                                    colorString =leg.color;
                                }

                                string getDeparturefboHandlerName = "NA";
                                string getArrvailfboHandlerName ="NA";
                                string getDeparturelocalPhone ="NA";
                                string getArrivallocalPhone = "NA";
                                string getDepartureserviceEmailAddress ="NA";
                                string getArrivalserviceEmailAddress = "NA";

                                if (leg.departureFbo!=null)
                                {
                                     getDeparturefboHandlerName = leg.departureFbo.fboHandlerName ?? "NA";                                   
                                     getDeparturelocalPhone = leg.departureFbo.localPhone ?? "NA";                                    
                                     getDepartureserviceEmailAddress = leg.departureFbo.serviceEmailAddress ?? "NA";                                   
                                }
                                if(leg.arrivalFbo!=null)
                                {
                                    getArrvailfboHandlerName = leg.arrivalFbo.fboHandlerName ?? "NA";
                                    getArrivallocalPhone = leg.arrivalFbo.localPhone ?? "NA";
                                    getArrivalserviceEmailAddress = leg.arrivalFbo.serviceEmailAddress ?? "NA";
                                }

                                var sdto = DateTimeOffset.ParseExact(leg.startDateTimeLocal,
                                    "yyyy-MM-ddTHH:mm:sszzz",
                                    CultureInfo.InvariantCulture);

                                var edto = DateTimeOffset.ParseExact(leg.endDateTimeLocal,
                                    "yyyy-MM-ddTHH:mm:sszzz",
                                    CultureInfo.InvariantCulture);


                                _preflightVM = new PreflightVM
                                {
                                    customerId = flight.customerId,
                                    aircraftId = flight.aircraftId,

                                    date = flight.date,

                                    isToday = flight.isToday,
                                    isWeekend = flight.isWeekend,
                                    postFlightOverridden = flight.postFlightOverridden,

                                    //from leg
                                    startDateTimeUtc = DateTime.Parse(leg.startDateTimeUtc),
                                    endDateTimeUtc = DateTime.Parse(leg.endDateTimeUtc),
                                    startDateTimeLocal = sdto.DateTime,
                                    endDateTimeLocal = edto.DateTime,

                                    StartLocalTimeString =leg.startDateTimeLocal,
                                    EndLocalTimeString=leg.endDateTimeLocal,

                                    scheduledFlightId = leg.scheduledFlightId,

                                    flightId = leg.flightId,
                                    scheduledAircraftTripId = leg.scheduledAircraftTripId,

                                    tentativeEta = leg.tentativeEta,
                                    tentativeEtd = leg.tentativeEtd,

                                    ete = leg.ete,
                                    crewInitials = leg.crewInitials,

                                    legCount = leg.legCount,
                                    legNumber = leg.legNumber,
                                    passengerLegCount = leg.passengerLegCount,
                                    customTripId = leg.customTripId,

                                    departureAirportIcao = leg.departure,
                                    arrivalAirportIcao = leg.arrival,
                                    tailNumber = leg.tailNumber,

                                    color = ConvertHex(colorString),  //Color

                                    eventName = "Aircraft",
                                    IsUpdated = true,

                                    //leg.departureFbo.fboHandlerName ?? new MyValue();
                                    DeparturefboHandlerName = getDeparturefboHandlerName,
                                    ArrvailfboHandlerName = getArrvailfboHandlerName,
                                    

                                    DeparturelocalPhone = getDeparturelocalPhone,  
                                    ArrivallocalPhone = getArrivallocalPhone,
                                    DepartureserviceEmailAddress = getDepartureserviceEmailAddress,
                                    ArrivalserviceEmailAddress = getArrivalserviceEmailAddress,

                                    FID = leg.flightId,
                                };

                                try
                                {
                                    var Exists = db.Table<PreflightVM>().Where(x => x.flightId == _preflightVM.flightId && x.customTripId == _preflightVM.customTripId).FirstOrDefault();
                                    if (Exists != null)
                                    {
                                        Console.WriteLine("Skipped Aircraft Event PreflightID: " + Exists.flightId);
                                    }
                                    else
                                    {
                                        db.Insert(_preflightVM);
                                        Console.WriteLine("Added Aircraft Event PreflightID: " + _preflightVM.PreflightvmID + " ; Flight ID: " + _preflightVM.flightId);
                                    }
                                }
                                catch (Exception exc) { Console.WriteLine(exc); }

                                //passenger Crews
                                try
                                {
      
                                    //Crews
                                    

                                    foreach (var crew in leg.flightCrewMembers)
                                    {
                                        string CV = "false";
                                        if (crew.phoneNumber != null && crew.phoneNumber != "")
                                        {
                                            CV = "true";
                                        }

                                        flightCrewMember = new FlightCrewMember
                                        {
                                            ScheduledAircraftTripId = leg.scheduledAircraftTripId,
                                            id = crew.id,
                                            initials = crew.initials,
                                            fullName = crew.lastName + " " + crew.firstName,
                                            CrewFlightLegNumber = leg.legNumber,
                                            fullPhoneNumber = crew.phoneNumber,
                                            CallVisible = CV,
                                            flightId = leg.flightId,
                                            crewMemberType=crew.crewMemberType
                                            
                                        };

                                        try
                                        {
                                            db.Insert(flightCrewMember);

                                            Console.WriteLine("Crew Name:" + flightCrewMember.fullName );
                                        }
                                        catch (Exception exc)
                                        {
                                            Console.WriteLine(exc);
                                        }
                                    }

                                    //passenger
                                    foreach (var passenger in leg.flightPassenger)
                                    {

                                        flightPassenger = new FlightPassenger
                                        {
                                            flightPassengerId=passenger.flightPassengerId,
                                            ScheduledAircraftTripId = leg.scheduledAircraftTripId,
                                            aircraftLegNumber = leg.legNumber,
                                            passengerId = passenger.passengerId,
                                            PassengerName = passenger.passenger.lastName+ " "+passenger.passenger.firstName,

                                            passengerStatusId = passenger.passengerStatusId,

                                            businessPersonId = passenger.passenger.businessPersonId,
                                            flightId = leg.flightId,
                                            
                                            
                                        };

                                        try
                                        {
                                            db.Insert(flightPassenger);
                                        }
                                        catch (Exception exc) { Console.WriteLine(exc); }

                                    }

                                    Console.WriteLine("LegNumber----------" + leg.legNumber);

                                    
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }

                            catch (Exception exc)
                            {
                                Console.WriteLine(exc);
                            }
                        }
                    }

                    if (flight.aircraftEvents.Count() != 0)
                    {
                        foreach (var aircraft in flight.aircraftEvents)
                        {
                            try
                            {
                                if (aircraft.color != null)
                                {
                                    colorString = aircraft.color;
                                }



                                _preflightVM = new PreflightVM
                                {
                                    customerId = flight.customerId,
                                    aircraftId = flight.aircraftId,

                                    date = flight.date,

                                    staffEventType = aircraft.aircraftEventType,
                                    isToday = flight.isToday,
                                    isWeekend = flight.isWeekend,
                                    postFlightOverridden = flight.postFlightOverridden,


                                    startDateTimeUtc = aircraft.departureDateTimeUtc,
                                    endDateTimeUtc = aircraft.arrivalDateTimeUtc,
                                    startDateTimeLocal = aircraft.departureDateTimeLocal,
                                    endDateTimeLocal = aircraft.departureDateTimeLocal,


                                    scheduledFlightId = aircraft.scheduledEventId,

                                    flightId = aircraft.scheduledEventId,



                                    crewInitials = aircraft.pilotInitials ?? "",

                                    departureAirportIcao = aircraft.departureAirportIcao ?? "",
                                    arrivalAirportIcao = aircraft.departureAirportIcao ?? "",
                                    tailNumber = aircraft.tailNumber,

                                    color = ConvertHex(colorString),  //Color

                                    eventName = "AircraftEvents",
                                    IsUpdated = true,
                                    notes = aircraft.notes
                                };

                                try
                                {
                                    var Exists = db.Table<PreflightVM>().Where(x => x.flightId == _preflightVM.flightId && x.customTripId == _preflightVM.customTripId).FirstOrDefault();
                                    if (Exists != null)
                                    {
                                        Console.WriteLine("Skipped Aircraft Event PreflightID: " + Exists.flightId);
                                    }
                                    else
                                    {
                                        db.Insert(_preflightVM);
                                        Console.WriteLine("Added Aircraft Event PreflightID: " + _preflightVM.PreflightvmID + " ; Flight ID: " + _preflightVM.flightId);
                                    }
                                }
                                catch (Exception exc) { Console.WriteLine(exc); }
                            }

                            catch (Exception exc)
                            {
                                Console.WriteLine(exc);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region StaffEvents

        public IEnumerable<PreflightVM> GetAllStaffPreflights()
        {
            _ = Init();

            var AircraftEvents = db.Table<PreflightVM>().Where(x => x.eventName == "Staff").OrderBy(x => x.date);
            return AircraftEvents;
        }

        public async Task<IEnumerable<StaffEventTypes>> GetStaffEventTypes()
        {
            await Init();

            var staffEventTypes = db.Table<StaffEventTypes>().ToList();
            return staffEventTypes;
        }

        public async Task AddStaffEventTypes()
        {
            StaffEventTypes _staffEventTypes = new StaffEventTypes();

                await Init();

                var _staffEvent = await _preflightRepo.GetStaffEventTypes();

                if (_staffEvent != null)
                {
                    foreach (var staff in _staffEvent)
                    {
                        try
                        {
                            _staffEventTypes = new StaffEventTypes
                            {
                                staffEventTypeId = staff.staffEventTypeId,
                                abbreviation = staff.abbreviation,
                                color = staff.color,
                                isDuty = staff.isDuty,
                                maximumDutyHours = staff.maximumDutyHours,
                                name = staff.name,
                                active = staff.active,

                                deleted = staff.deleted,
                                id = staff.id,
                                customerId = staff.customerId,

                                controlId = staff.controlId,
                                isOverride = staff.isOverride,

                                //Others are being avoided for now
                            };

                            try
                            {
                                var Exists = GetStaffEventTypes().Result.FirstOrDefault(x => x.staffEventTypeId == _staffEventTypes.staffEventTypeId);
                                if (Exists != null)
                                {
                                    
                                }
                                else
                                {
                                    db.Insert(_staffEventTypes);
                                    Console.WriteLine("Added Aircraft Event Profie: " + _staffEventTypes.staffEventTypeId);
                                }
                            }
                            catch (Exception exc) { Console.WriteLine(exc); }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc);
                        }
                    
                }
            }
        }

        public async Task AddPreflightSE()
        {
            PreflightVM _preflightVM = new PreflightVM();

            await AddStaffEventTypes();

            await Init();

            DateTime SetDate = DateTime.MinValue;
            //int SetAircraftId = 0;

            var preflightStaffEvents = await _preflightRepo.GetPreflightStaffEvents();

            if (preflightStaffEvents != null)
            {
                foreach (var flight in preflightStaffEvents)
                {
                    if (flight.startDateTimeUtc != null)
                    {
                        SetDate = flight.startDateTimeUtc;
                    }

                    string getColor = ConvertHex(db.Table<StaffEventTypes>().Where(x => x.staffEventTypeId == flight.staffEventTypeId).Select(x => x.color).FirstOrDefault() ?? "");
                    string getTailNumber = db.Table<StaffEventTypes>().Where(x => x.staffEventTypeId == flight.staffEventTypeId).Select(x => x.name).FirstOrDefault() ?? "";


                    try
                    {
                        _preflightVM = new PreflightVM
                        {
                            customerId = flight.customerId,
                            staffEventId = flight.staffEventId,

                            date = SetDate,
                            //day = flight.day,
                            //dayOfTheWeek = flight.dayOfTheWeek,


                            isToday = flight.isToday,
                            isWeekend = flight.isWeekend,
                            postFlightOverridden = flight.postFlightOverridden,

                            //lastKnownAirportId = flight.lastKnownAirportId,
                            //lastKnownAirport = flight.lastKnownAirport,

                            //from Staffevents Exclusive
                            endDateTimeUtc = flight.endDateTimeUtc,
                            startDateTimeUtc = flight.startDateTimeUtc,
                            staffEventTypeId = flight.staffEventTypeId,
                            staffEventType = flight.staffEventType,
                            mobileStaffEventBusinessPersonsDto = flight.mobileStaffEventBusinessPersonsDto.ToList(),

                            arrivalAirportIcao = flight.airport.airportCode,
                            departureAirportIcao = flight.airport.airportCode,

                            abbreviation = GetStaffEventTypes().Result.Where(x => x.staffEventTypeId == flight.staffEventTypeId).Select(x => x.abbreviation).FirstOrDefault(),

                            //To Do: color from staffEventType mayb ask fist
                            color = getColor,
                            tailNumber = getTailNumber,

                            eventName = "Staff",
                            IsUpdated = true,
                        };

                        try
                        {
                            var Exists = GetPreflights().Result.FirstOrDefault(x => x.staffEventId == _preflightVM.staffEventId);
                            if (Exists != null)
                            {
                                var delete = db.Table<PreflightVM>().Delete(x => x.staffEventId == Exists.staffEventId);
                                db.Insert(_preflightVM);
                                //Console.WriteLine("Updated Staff Event PreflightID: " + Exists.PreflightvmID);
                            }
                            else
                            {
                                db.Insert(_preflightVM);
                                Console.WriteLine("Added Staff Event PreflightID: " + _preflightVM.PreflightvmID);
                            }
                        }
                        catch (Exception exc) { Console.WriteLine(exc); }

                    }
                    catch (Exception exc)
                    {

                        Console.WriteLine(exc);
                    }
                }
            }

        }

        #endregion

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
    }
}
