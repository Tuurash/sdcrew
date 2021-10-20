using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace sdcrew.Models
{
    public class StaffEventTypes
    {
        [PrimaryKey, AutoIncrement]
        public int staffEventTypeobjID { get; set; }

        public int staffEventTypeId { get; set; }
        public string abbreviation { get; set; }
        public string color { get; set; }
        public bool isDuty { get; set; }
        public int maximumDutyHours { get; set; }
        public string name { get; set; }
        public bool active { get; set; }
        public bool deleted { get; set; }
        public int id { get; set; }
        public int customerId { get; set; }
        public int controlId { get; set; }
        public bool isOverride { get; set; }
    }
}
