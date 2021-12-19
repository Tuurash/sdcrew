using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace sdcrew.Models
{
    
    public class PostFlightRoot
    {
        [PrimaryKey, AutoIncrement]
        [Column("PostFlightRootId")]
        public int? PostFlightRootId { get; set; }

        public int totalCount { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }

        [OneToMany]
        [JsonProperty(PropertyName = "ResultSet")]
        public List<PostFlight> resultSet { get; set; }
    }

    public class PostedFlightOooi
    {
        [PrimaryKey, AutoIncrement]
        [Column("PostFlightOooiId")]
        public int? PostFlightOooiId { get; set; }

        public int postedFlightId { get; set; }
        public int aircraftProfileId { get; set; }
        public int departureAirportId { get; set; }
        public int arrivalAirportId { get; set; }
        public DateTime blockStartDateTime { get; set; }
        public DateTime flightStartDateTime { get; set; }
        public DateTime? blockStopDateTime { get; set; }
        public DateTime? flightStopDateTime { get; set; }
    }

    public class PostedFlightFuel
    {
        [PrimaryKey, AutoIncrement]
        [Column("PostFlightFuelId")]
        public int? PostFlightFuelId { get; set; }

        public int postedFlightId { get; set; }
        public double fuelOut { get; set; }
        public double fuelOff { get; set; }
        public double? fuelOn { get; set; }
        public double? fuelIn { get; set; }
        public double? fuelBurn { get; set; }
    }


    //no Table
    public class PostedFlightDeIce
    {
        [PrimaryKey, AutoIncrement]
        [Column("PostedFlightDeIceId")]
        public int? PostedFlightDeIceId { get; set; }
    }

    //No Table
    public class PostedFlightAdditional
    {
        public int postedFlightId { get; set; }
    }

    //No table
    public class LogbookApproach
    {
        public int logbookApproachId { get; set; }
        public int airportId { get; set; }
        public int approachTypeId { get; set; }
        public int logbookId { get; set; }
        public bool isEVS { get; set; }
    }

    public class Logbook
    {
        [PrimaryKey, AutoIncrement]
        [Column("Postflight_LogbookId")]
        public int? Postflight_LogbookId { get; set; }

        public int logbookId { get; set; }
        public int flightCrewMemberId { get; set; }
        public int crewMemberId { get; set; }
        public int customerId { get; set; }
        public bool isHoldPerformed { get; set; }
        public bool isTrackPerformed { get; set; }
        public int actualInstrumentDurationMinutes { get; set; }
        public int nightDurationMinutes { get; set; }
        public int dayLandings { get; set; }
        public int nightLandings { get; set; }
        public int dayTakeoffs { get; set; }
        public int nightTakeoffs { get; set; }
        public bool pic { get; set; }
        public bool sic { get; set; }
        public bool isSimulatedFlight { get; set; }
        public bool isBulkPic { get; set; }
        public bool isBulkEntry { get; set; }

        [OneToMany]
        public List<LogbookApproach> logbookApproaches { get; set; }
        public DateTime createdDate { get; set; }
    }

    public class Postflight_CrewMember
    {
        [PrimaryKey, AutoIncrement]
        [Column("Postflight_CrewMemberId")]
        public int? Postflight_CrewMemberId { get; set; }

        public int crewMemberId { get; set; }
        public int personId { get; set; }
        public int businessPersonId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string initials { get; set; }
    }

    public class CrewMemberType
    {
        [PrimaryKey, AutoIncrement]
        [Column("PostFlightCrewMemberTypeId")]
        public int PostFlightCrewMemberTypeId { get; set; }

        public int crewMemberTypeId { get; set; }
        public int customerId { get; set; }
        public int controlId { get; set; }
    }

    public class PostFlightCrewMember
    {
        [PrimaryKey, AutoIncrement]
        [Column("PostFlightCrewMemberId")]
        public int? PostFlightCrewMemberId { get; set; }

        public int flightCrewMemberId { get; set; }
        public int customerId { get; set; }
        public int crewMemberId { get; set; }
        public int flightId { get; set; }
        public int crewMemberTypeId { get; set; }
        public bool isPilotFlying { get; set; }
        public bool isAssignedAutomatedData { get; set; }
        public int eApisAddressTypeId { get; set; }

        [ManyToOne]
        public Logbook logbook { get; set; }
        [ManyToOne]
        [JsonProperty(PropertyName = "CrewMember")]
        public Postflight_CrewMember crewMember { get; set; }
        [ManyToOne]
        public CrewMemberType crewMemberType { get; set; }
    }

    public class Postflight_Passenger
    {
        [PrimaryKey, AutoIncrement]
        [Column("Postflight_PassengerId")]
        public int? Postflight_Passengerid { get; set; }

        public int passengerId { get; set; }
        public int businessPersonId { get; set; }
        public int personId { get; set; }
        public bool reserved { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string initials { get; set; }
        public DateTime? birthdate { get; set; }
    }

    public class PostFlightPassenger
    {
        [PrimaryKey, AutoIncrement]
        [Column("PostFlightPassengerId")]
        public int? PostFlightPassengerId { get; set; }

        public int flightPassengerId { get; set; }
        public int flightId { get; set; }
        public int passengerId { get; set; }
        public int customerId { get; set; }
        public int passengerStatusId { get; set; }
        public bool isLeadPassenger { get; set; }
        public int eApisAddressTypeId { get; set; }

        [ManyToOne]
        [JsonProperty(PropertyName = "Passenger")]
        public Postflight_Passenger passenger { get; set; }
        public bool isLapChild { get; set; }
    }


    public class Fluid
    {
        [PrimaryKey, AutoIncrement]
        [Column("FluidId")]
        public int? FluidId { get; set; }

        public string type { get; set; }
        public int postedItemId { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int postedFlightId { get; set; }
        public int cycles { get; set; }
    }

    public class Oooi
    {
        [PrimaryKey, AutoIncrement]
        [Column("OooiId")]
        public int? OooiId { get; set; }

        public DateTime automatedOutTime { get; set; }
        public DateTime automatedOffTime { get; set; }
        public DateTime automatedOnTime { get; set; }
        public DateTime automatedInTime { get; set; }
    }

    public class Fuel
    {
        [PrimaryKey, AutoIncrement]
        [Column("FuelId")]
        public int? FuelId { get; set; }


        public double automatedFuelOut { get; set; }
        public double automatedFuelOff { get; set; }
        public double automatedFuelOn { get; set; }
        public double automatedFuelIn { get; set; }
        public double automatedFuelBurn { get; set; }
    }
    //No Table
    public class AutomatedFlightData
    {
        [PrimaryKey, AutoIncrement]
        public int AutomatedFlightDataId { get; set; }

        [ManyToOne]
        public Oooi oooi { get; set; }
        [ManyToOne]
        public Fuel fuel { get; set; }
    }

    //No Table
    public class PostFlight
    {
        public int flightId { get; set; }
        public int aircraftProfileId { get; set; }
        public int postedFlightId { get; set; }
        public string departureIcao { get; set; }
        public string airportIcao { get; set; }
        public int departureAirportId { get; set; }
        public int arrivalAirportId { get; set; }
        public int tripId { get; set; }
        public int postedAircraftTripId { get; set; }
        public string customTripId { get; set; }
        public string postFlightStatusName { get; set; }
        public int postedFlightStatusTypeId { get; set; }
        public int roundingThreshold { get; set; }
        public string aircraftColor { get; set; }
        public DateTime flightStartDateTime { get; set; }
        public int legCount { get; set; }
        public int legNumber { get; set; }
        public bool isUserSigned { get; set; }
        public int passengerLegCount { get; set; }

        [ManyToOne]

        public PostedFlightOooi PostedFlightOooi { get; set; }
        [ManyToOne]
        public PostedFlightFuel postedFlightFuel { get; set; }
        [ManyToOne]
        public PostedFlightDeIce postedFlightDeIce { get; set; }
        [ManyToOne]
        public PostedFlightAdditional postedFlightAdditional { get; set; }

        [OneToMany]
        [JsonProperty(PropertyName = "FlightCrewMember")]
        public List<PostFlightCrewMember> flightCrewMember { get; set; }
        [OneToMany]
        [JsonProperty(PropertyName = "FlightPassenger")]
        public List<PostFlightPassenger> flightPassengers { get; set; }
        [OneToMany]
        public List<Fluid> fluids { get; set; }

        [ManyToOne]
        public AutomatedFlightData automatedFlightData { get; set; }

        public string crewInitials { get; set; }
        public DateTime? flightStopDateTime { get; set; }
        public int? scheduledFlightId { get; set; }
        public int? departureFboId { get; set; }
        public int? arrivalFboId { get; set; }
        public string arrivalFboName { get; set; }
        public string departureFboName { get; set; }
    }

    public class PostFlightVM
    {

        [PrimaryKey, AutoIncrement]
        [Column("PostflightVmId")]
        public int? PostflightVmId { get; set; }

        public int flightId { get; set; }
        public int aircraftProfileId { get; set; }
        public int postedFlightId { get; set; }
        public string departureIcao { get; set; }
        public string airportIcao { get; set; }
        public int departureAirportId { get; set; }
        public int arrivalAirportId { get; set; }
        public int tripId { get; set; }
        public int postedAircraftTripId { get; set; }
        public string customTripId { get; set; }
        public string postFlightStatusName { get; set; }
        public int postedFlightStatusTypeId { get; set; }
        public int roundingThreshold { get; set; }
        public string aircraftColor { get; set; }
        public DateTime flightStartDateTime { get; set; }
        public int legCount { get; set; }
        public int legNumber { get; set; }
        public bool isUserSigned { get; set; }
        public int passengerLegCount { get; set; }

        [ManyToOne]
        public PostedFlightOooi postedFlightOooi { get; set; }
        [ManyToOne]
        public Oooi oooi { get; set; }
        [ManyToOne]
        public PostedFlightFuel postedFlightFuel { get; set; }
        [ManyToOne]
        public PostedFlightDeIce postedFlightDeIce { get; set; }
        [ManyToOne]
        public PostedFlightAdditional postedFlightAdditional { get; set; }

        [OneToMany]
        [JsonProperty(PropertyName = "FlightCrewMember")]
        public List<PostFlightCrewMember> flightCrewMember { get; set; }
        [OneToMany]
        public List<FlightPassenger> flightPassengers { get; set; }
        [OneToMany]
        public List<Fluid> fluids { get; set; }

        [ManyToOne]
        public AutomatedFlightData automatedFlightData { get; set; }
        public string crewInitials { get; set; }
        public DateTime? flightStopDateTime { get; set; }
        public int? scheduledFlightId { get; set; }
        public int? departureFboId { get; set; }
        public int? arrivalFboId { get; set; }
        public string arrivalFboName { get; set; }
        public string departureFboName { get; set; }



        //Custom
        public string Customized_TripId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string TailNumber { get; set; }

    }



}
