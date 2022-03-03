using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace sdcrew.Models
{
    public class OooiPostModel
    {
        [JsonProperty("postedFlightId")]
        public int PostedFlightId { get; set; }

        [JsonProperty("aircraftProfileId")]
        public int AircraftProfileId { get; set; }

        [JsonProperty("scheduledFlightId")]
        public int ScheduledFlightId { get; set; }

        [JsonProperty("departureAirportId")]
        public int DepartureAirportId { get; set; }

        [JsonProperty("arrivalAirportId")]
        public int ArrivalAirportId { get; set; }

        [JsonProperty("blockStartDateTime")]
        public DateTime BlockStartDateTime { get; set; }

        [JsonProperty("blockStopDateTime")]
        public DateTime BlockStopDateTime { get; set; }

        [JsonProperty("flightStartDateTime")]
        public DateTime FlightStartDateTime { get; set; }

        [JsonProperty("flightStopDateTime")]
        public DateTime FlightStopDateTime { get; set; }
    }

    public class AdditionalPostingModel
    {
        [JsonProperty("postedFlightId")]
        public int PostedFlightId { get; set; }

        [JsonProperty("delayTypeId")]
        public int DelayTypeId { get; set; }

        [JsonProperty("delayDuration")]
        public int DelayDuration { get; set; }

        [JsonProperty("goArounds")]
        public int GoArounds { get; set; }

        [JsonProperty("rejectedTakeoffs")]
        public int RejectedTakeoffs { get; set; }

        [JsonProperty("legNotes")]
        public string LegNotes { get; set; }

        [JsonProperty("departmentId")]
        public int DepartmentId { get; set; }

        [JsonProperty("businessCategoryId")]
        public int BusinessCategoryId { get; set; }
    }

    public class DeIcePostingModel
    {
        [JsonProperty("deIceStartDateTime")]
        public DateTime DeIceStartDateTime { get; set; }

        [JsonProperty("deIceEndDateTime")]
        public DateTime DeIceEndDateTime { get; set; }

        [JsonProperty("deIceMixRatioTypeId")]
        public int DeIceMixRatioTypeId { get; set; }

        [JsonProperty("deIceTypeId")]
        public int DeIceTypeId { get; set; }
    }

    public class FuelPostingModel
    {
        [JsonProperty("postedFlightId")]
        public int PostedFlightId { get; set; }

        [JsonProperty("fuelOut")]
        public int FuelOut { get; set; }

        [JsonProperty("fuelOff")]
        public int FuelOff { get; set; }

        [JsonProperty("fuelOn")]
        public int FuelOn { get; set; }

        [JsonProperty("fuelIn")]
        public int FuelIn { get; set; }

        [JsonProperty("fuelBurn")]
        public int FuelBurn { get; set; }

        [JsonProperty("quantityTypeId")]
        public int QuantityTypeId { get; set; }

        [JsonProperty("plannedUp")]
        public int PlannedUp { get; set; }

        [JsonProperty("actualUp")]
        public int ActualUp { get; set; }

        [JsonProperty("upliftQuantityTypeId")]
        public int UpliftQuantityTypeId { get; set; }
    }


    #region DutyTime

    public class CrewMemberEvent
    {
        [JsonProperty("crewMemberEventId")]
        public int CrewMemberEventId { get; set; }

        [JsonProperty("crewMemberId")]
        public int CrewMemberId { get; set; }

        [JsonProperty("departureAirportCode")]
        public string DepartureAirportCode { get; set; }

        [JsonProperty("arrivalAirportCode")]
        public string ArrivalAirportCode { get; set; }

        [JsonProperty("crewMemberEventType")]
        public int CrewMemberEventType { get; set; }

        [JsonProperty("crewMemberEventStartDateTimeUtc")]
        public DateTime CrewMemberEventStartDateTimeUtc { get; set; }

        [JsonProperty("crewMemberEventStopDateTimeUtc")]
        public DateTime CrewMemberEventStopDateTimeUtc { get; set; }

        [JsonProperty("domesticPreflightBuffer")]
        public int DomesticPreflightBuffer { get; set; }

        [JsonProperty("domesticPostflightBuffer")]
        public int DomesticPostflightBuffer { get; set; }

        [JsonProperty("internationalPreflightBuffer")]
        public int InternationalPreflightBuffer { get; set; }

        [JsonProperty("internationalPostflightBuffer")]
        public int InternationalPostflightBuffer { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("abbreviation")]
        public string Abbreviation { get; set; }

        [JsonProperty("isInternational")]
        public bool IsInternational { get; set; }

        [JsonProperty("aircraftType")]
        public string AircraftType { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("crewMemberType")]
        public string CrewMemberType { get; set; }
    }

    public class DutyTimePostingModel
    {
        [JsonProperty("crewMemberMetricDateId")]
        public int CrewMemberMetricDateId { get; set; }

        [JsonProperty("crewMemberId")]
        public int CrewMemberId { get; set; }

        [JsonProperty("businessPersonId")]
        public int BusinessPersonId { get; set; }

        [JsonProperty("metricDate")]
        public DateTime MetricDate { get; set; }

        [JsonProperty("dutyTimeStartUtc")]
        public DateTime DutyTimeStartUtc { get; set; }

        [JsonProperty("dutyTimeEndUtc")]
        public DateTime DutyTimeEndUtc { get; set; }

        [JsonProperty("isDutyDay")]
        public bool IsDutyDay { get; set; }

        [JsonProperty("isDutyDayOverriden")]
        public bool IsDutyDayOverriden { get; set; }

        [JsonProperty("isRon")]
        public bool IsRon { get; set; }

        [JsonProperty("isRonOverriden")]
        public bool IsRonOverriden { get; set; }

        [JsonProperty("lastKnownAirportId")]
        public int LastKnownAirportId { get; set; }

        [JsonProperty("metricDateSourceId")]
        public int MetricDateSourceId { get; set; }

        [JsonProperty("crewMemberEvents")]
        public List<CrewMemberEvent> CrewMemberEvents { get; set; }
    }


    #endregion


}
