using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace sdcrew.Models
{
    public class PreflightNote
    {
        [PrimaryKey, AutoIncrement]
        [Column("NoteId")]
        public int? NoteId { get; set; }

        public int calendarNoteId { get; set; }
        public string note { get; set; }
        public int customerId { get; set; }
        public DateTime calendarDate { get; set; }
        public string createdByUser { get; set; }
        public DateTime createdDate { get; set; }
        public string modifiedByUser { get; set; }
        public string modifiedDate { get; set; }

        public string IsVisible { get; set; }

    }

    public class StaffNotes
    {
        [PrimaryKey, AutoIncrement]
        [Column("NoteId")]
        public int? StaffNoteId { get; set; }

        public int staffEventId { get; set; }
        public int staffEventTypeId { get; set; }
        public string staffEventType { get; set; }
        public int scheduledEventId { get; set; }
        public int airportId { get; set; }
        public string notes { get; set; }
        public bool isDuty { get; set; }
        public bool daily { get; set; }
        public bool isNotifyParticipants { get; set; }
        public bool isAcknowledgement { get; set; }
        public bool isCompleted { get; set; }
    }
}
