using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using SQLiteNetExtensions.Attributes;

namespace sdcrew.Models
{


    public class CrewMemberTypeRatingCurrencyType
    {
        [JsonProperty("crewMemberTypeRatingCurrencyTypeId")]
        public int CrewMemberTypeRatingCurrencyTypeId { get; set; }

        [JsonProperty("crewMemberTypeRatingId")]
        public int CrewMemberTypeRatingId { get; set; }

        [JsonProperty("currencyTypeId")]
        public int CurrencyTypeId { get; set; }

        [JsonProperty("isTracked")]
        public bool IsTracked { get; set; }
    }

    public class CrewMemberTypeRating
    {
        [JsonProperty("crewMemberTypeRatingId")]
        public int CrewMemberTypeRatingId { get; set; }

        [JsonProperty("aircraftTypeRatingId")]
        public int AircraftTypeRatingId { get; set; }

        [JsonProperty("controlId")]
        public int ControlId { get; set; }

        [JsonProperty("crewMemberTypeId")]
        public int CrewMemberTypeId { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("aircraftTypeRatingName")]
        public string AircraftTypeRatingName { get; set; }

        [JsonProperty("crewMemberTypeName")]
        public string CrewMemberTypeName { get; set; }

        [OneToMany]
        [JsonProperty("crewMemberTypeRatingCurrencyTypes")]
        public List<CrewMemberTypeRatingCurrencyType> CrewMemberTypeRatingCurrencyTypes { get; set; }
    }

    //NoTable
    public class CrewCurrencyWarning
    {
        [JsonProperty("warningTypeId")]
        public int WarningTypeId { get; set; }

        [JsonProperty("eventTitle")]
        public string EventTitle { get; set; }

        [JsonProperty("dueDate")]
        public DateTime DueDate { get; set; }
    }

    public class CrewDetails
    {
        [JsonProperty("crewMemberId")]
        public int CrewMemberId { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("crewMemberTypeRatings")]
        public List<CrewMemberTypeRating> CrewMemberTypeRatings { get; set; }

        [JsonProperty("crewCurrencyWarnings")]
        public List<CrewCurrencyWarning> CrewCurrencyWarnings { get; set; }

        [JsonProperty("typeRating")]
        public string TypeRating { get; set; }

        [JsonProperty("warningTypeId")]
        public int WarningTypeId { get; set; }

        [JsonProperty("personId")]
        public int PersonId { get; set; }

        [JsonProperty("businessPersonId")]
        public int BusinessPersonId { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("initials")]
        public string Initials { get; set; }

        [JsonProperty("yearToDateHours")]
        public string YearToDateHours { get; set; }

        [JsonProperty("monthToDateHours")]
        public string MonthToDateHours { get; set; }

        [JsonProperty("homeBase")]
        public string HomeBase { get; set; }

        [JsonProperty("ron")]
        public int Ron { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("phoneCode")]
        public string PhoneCode { get; set; }
    }

    public class CrewDetailsVM
    {
        [JsonProperty("crewMemberId")]
        public int CrewMemberId { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [OneToMany]
        [JsonProperty("crewMemberTypeRatings")]
        public List<CrewMemberTypeRating> CrewMemberTypeRatings { get; set; }

        [OneToMany]
        [JsonProperty("crewCurrencyWarnings")]
        public List<CrewCurrencyWarning> CrewCurrencyWarnings { get; set; }

        [JsonProperty("typeRating")]
        public string TypeRating { get; set; }

        [JsonProperty("warningTypeId")]
        public int WarningTypeId { get; set; }

        [JsonProperty("personId")]
        public int PersonId { get; set; }

        [JsonProperty("businessPersonId")]
        public int BusinessPersonId { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("initials")]
        public string Initials { get; set; }

        [JsonProperty("yearToDateHours")]
        public string YearToDateHours { get; set; }

        [JsonProperty("monthToDateHours")]
        public string MonthToDateHours { get; set; }

        [JsonProperty("homeBase")]
        public string HomeBase { get; set; }

        [JsonProperty("ron")]
        public int Ron { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("phoneCode")]
        public string PhoneCode { get; set; }

        public string CrewMemberTypeName { get; set; }

        public bool IsTracked { get; set; }
    }
}
