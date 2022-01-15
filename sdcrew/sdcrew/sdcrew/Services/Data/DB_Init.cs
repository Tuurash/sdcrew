using sdcrew.Models;

using SQLite;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;

namespace sdcrew.Services.Data
{
    public class DB_Init
    {
        static SQLiteConnection db;

        public static async Task<SQLiteConnection> Init()
        {
            var MyKey = await SecureStorage.GetAsync("Ckey");
            string Ckey = MyKey;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "CrewData.db");

            db = new SQLiteConnection(databasePath, openFlags: SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            try
            {
                db.Query<int>("PRAGMA key=" + Ckey);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            //If Services.Settings.IsAppFirstLoaded==empty, will go through  table creation
            //if (String.IsNullOrEmpty(Services.Settings.IsAppFirstLoaded))
            //{
            try
            {
                #region PreflightTables

                db.CreateTable<User>();

                #region Preflight_Aircraft & related Tables
                db.CreateTable<PreFlight>();
                db.CreateTable<PreflightVM>();

                db.CreateTable<PassengerStatus>();
                db.CreateTable<Passenger>();
                db.CreateTable<FlightPassenger>();
                db.CreateTable<FlightCrewMember>();

                db.CreateTable<DepartureFbo>();
                db.CreateTable<ArrivalFbo>();
                //db.CreateTable<FboBasicInfo>();

                db.CreateTable<Leg>();
                db.CreateTable<WorldTimeZone>();
                db.CreateTable<DepartureAirport>();
                db.CreateTable<ArrivalAirport>();
                db.CreateTable<AircraftEvent>();
                #endregion

                db.CreateTable<AircraftProfileDto>();
                db.CreateTable<AircraftTailWithSn>();
                db.CreateTable<Airport>();
                db.CreateTable<MobileStaffEventBusinessPersonsDto>();
                db.CreateTable<StaffEventTypes>();

                //Checklists
                db.CreateTable<SchedulingChecklist>();
                db.CreateTable<PilotChecklist>();
                db.CreateTable<MaitenanceChecklist>();
                db.CreateTable<ChecklistVM>();

                //Services
                db.CreateTable<Crew>();
                db.CreateTable<Hotel>();
                db.CreateTable<Catering>();
                db.CreateTable<Rental>();
                db.CreateTable<VService>();
                db.CreateTable<AircraftSevices>();

                //notes
                db.CreateTable<PreflightNote>();
                db.CreateTable<StaffNotes>();


                #endregion

                #region PostflightTables


                db.CreateTable<PostedFlightOooi>();
                db.CreateTable<PostedFlightFuel>();
                db.CreateTable<PostedFlightDeIce>();
                db.CreateTable<PostedFlightAdditional>();
                db.CreateTable<LogbookApproach>();
                db.CreateTable<Logbook>();
                db.CreateTable<Postflight_CrewMember>();
                db.CreateTable<CrewMemberType>();
                db.CreateTable<PostFlightCrewMember>();
                db.CreateTable<Postflight_Passenger>();
                db.CreateTable<PostFlightPassenger>();
                db.CreateTable<Fluid>();
                db.CreateTable<Oooi>();

                db.CreateTable<Fuel>();
                db.CreateTable<AutomatedFlightData>();
                db.CreateTable<PostFlight>();
                db.CreateTable<PostFlightVM>();

                db.CreateTable<PostFlightRoot>();

                #endregion

                #region Others

                db.CreateTable<CrewDetailsVM>();

                #endregion

                Services.Settings.IsAppFirstLoaded = "False";
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            //}



            return db;

        }

    }
}
