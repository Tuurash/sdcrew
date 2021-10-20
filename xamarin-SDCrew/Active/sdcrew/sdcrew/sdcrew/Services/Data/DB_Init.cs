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

            //if (db != null)
            //{
            //    return db;
            //}
            //else
            //{
            //    try
            //    {
            //        db.CreateTable<User>();

            //        #region Preflight_Aircraft & related Tables
            //        db.CreateTable<PreFlight>();
            //        db.CreateTable<PreflightVM>();

            //        db.CreateTable<PassengerStatus>();
            //        db.CreateTable<Passenger>();
            //        db.CreateTable<FlightPassenger>();
            //        db.CreateTable<FlightCrewMember>();

            //        db.CreateTable<DepartureFbo>();
            //        db.CreateTable<ArrivalFbo>();
            //        //db.CreateTable<FboBasicInfo>();

            //        db.CreateTable<Leg>();
            //        db.CreateTable<WorldTimeZone>();
            //        db.CreateTable<DepartureAirport>();
            //        db.CreateTable<ArrivalAirport>();
            //        db.CreateTable<AircraftEvent>();
            //        #endregion

            //        db.CreateTable<AircraftProfileDto>();
            //        db.CreateTable<Airport>();
            //        db.CreateTable<MobileStaffEventBusinessPersonsDto>();
            //        db.CreateTable<StaffEventTypes>();

            //        //Checklists
            //        db.CreateTable<SchedulingChecklist>();
            //        db.CreateTable<PilotChecklist>();
            //        db.CreateTable<MaitenanceChecklist>();
            //        db.CreateTable<ChecklistVM>();

            //        //Services
            //        db.CreateTable<Crew>();
            //        db.CreateTable<Hotel>();
            //        db.CreateTable<Catering>();
            //        db.CreateTable<Rental>();
            //        db.CreateTable<AircraftSevices>();
            //    }
            //    catch (Exception exc)
            //    {
            //        Console.WriteLine(exc);
            //    }
            //}

            try
            {
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
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            return db;
        }

    }
}
