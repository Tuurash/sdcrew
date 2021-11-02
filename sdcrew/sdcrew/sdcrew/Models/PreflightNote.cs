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
}
