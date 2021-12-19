using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;


namespace sdcrew.Models
{
    public class AircraftProfileDto
    {
        [PrimaryKey, AutoIncrement]
        public int aircraftProfileObjID { get; set; }

        public int aircraftProfileId { get; set; }
        public int customerId { get; set; }
        public string customerName { get; set; }
        public int displayOrder { get; set; }
        public string color { get; set; }
        public string tailNumber { get; set; }
        public string modelName { get; set; }
        public string homeBaseCountryName { get; set; }
        public string homeBase { get; set; }
        public int domesticPreFlightBuffer { get; set; }
        public int domesticPostFlightBuffer { get; set; }
        public int internationalPreFlightBuffer { get; set; }
        public int internationalPostFlightBuffer { get; set; }
        public string maxAircraftEndurance { get; set; }
        public int minimumRunwayWidth { get; set; }
        public int minimumRunwayLength { get; set; }
        public int aircraftTypeRatingId { get; set; }
        public int maximumDutyHours { get; set; }
        public int maximumFlightHours { get; set; }
        public int minimumRestHours { get; set; }
        public int sdoRecordId { get; set; }
        public bool active { get; set; }

        [OneToMany]
        public IEnumerable<DutyTimeAllowance> dutyTimeAllowances { get; set; }

        public int? maintenanceProviderId { get; set; }


        //From Postflight
        public int mostRecentScheduledFlightId { get; set; }
        public string vorBearingError1 { get; set; }
        public string vorBearingError2 { get; set; }
        public string cbpDecal { get; set; }

        public string eapisColor { get; set; }
        public string fmsRevision { get; set; }
        public string jeppRevision { get; set; }
        public string manufacturer { get; set; }

        public string serialNumber { get; set; }

        public string vorStation { get; set; }
        public string vorType { get; set; }
        public bool canSelectPartOperation { get; set; }
        public bool hasHobbsMeter { get; set; }
        public bool isCampMaint { get; set; }
        public bool isEVSEquipped { get; set; }

        public int crewSeatCount { get; set; }
        public DateTime fmsEffectiveDate { get; set; }
        public DateTime jeppEffectiveDate { get; set; }
        public DateTime vorUpdatedDate { get; set; }


    }

    public class DutyTimeAllowance
    {
        public int dutyTimeAllowanceId { get; set; }
        public int customerId { get; set; }
        public int aircraftProfileId { get; set; }
        public int dutyTypeId { get; set; }
        public int maximumDutyHours { get; set; }
        public int maximumFlightHours { get; set; }
        public int minimumRestHours { get; set; }
        public string createdByUser { get; set; }
        public DateTime createdDate { get; set; }
        public string modifiedByUser { get; set; }
        public DateTime modifiedDate { get; set; }
    }

    public class filteredTailNumber
    {
        public string TailNumber { get; set; }
        public bool isChecked { get; set; }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

}
