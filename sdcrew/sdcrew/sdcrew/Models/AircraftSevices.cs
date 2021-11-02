using sdcrew.Models;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace sdcrew.Models
{
    public class Crew
    {
        [PrimaryKey, AutoIncrement]
        [Column("CrewID")]
        public int? CrewID { get; set; }

        public int vendorId { get; set; }
        public string vendorName { get; set; }
        public string phone { get; set; }
        public string url { get; set; }
        public bool isCurrent { get; set; }
        public bool isAirportCurrent { get; set; }
        public string notes { get; set; }
        public string generalNotes { get; set; }
        public int memberId { get; set; }
        public string urlHref { get; set; }
        public string phoneHref { get; set; }
        public string phoneFormattedText { get; set; }


        public string CrewName { get; set; }
        public int ScheduledFlightId { get; set; }
        public string ServiceType { get; set; }
    }

    public class Hotel
    {
        [PrimaryKey, AutoIncrement]
        [Column("HotelID")]
        public int? HotelID { get; set; }

        [OneToMany]
        public List<Crew> crew { get; set; }

        [OneToMany]
        public List<object> pax { get; set; }
    }

    public class Catering
    {
        [PrimaryKey, AutoIncrement]
        [Column("CateringID")]
        public int? CateringID { get; set; }

        [OneToMany]
        public List<Crew> crew { get; set; }

        [OneToMany]
        public List<object> pax { get; set; }
    }

    public class Rental
    {
        [PrimaryKey, AutoIncrement]
        [Column("RentalID")]
        public int? RentalID { get; set; }

        [OneToMany]
        public List<Crew> crew { get; set; }

        [OneToMany]
        public List<object> pax { get; set; }
    }


    public class VService
    {
        [PrimaryKey, AutoIncrement]
        [Column("ServiceID")]
        public int? ServiceID { get; set; }

        public string ServiceType { get; set; }

        [OneToMany]
        public List<Crew> crew { get; set; }

        [OneToMany]
        public List<object> pax { get; set; }

        public int ScheduledFlightId { get; set; }
    }


    public class AircraftSevices
    {
        [PrimaryKey, AutoIncrement]
        [Column("AircraftSevicesID")]
        public int? AircraftSevicesID { get; set; }

        public int legID { get; set; }

        [ManyToOne]
        public Hotel hotel { get; set; }

        [ManyToOne]
        public Catering catering { get; set; }

        [ManyToOne]
        public Rental rental { get; set; }
    }

}
