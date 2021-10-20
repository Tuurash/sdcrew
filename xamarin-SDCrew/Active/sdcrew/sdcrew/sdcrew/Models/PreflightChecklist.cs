using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;


namespace sdcrew.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class SchedulingChecklist
    {
        [PrimaryKey, AutoIncrement]
        public int SchedulingChecklistID { get; set; }

        public int customerId { get; set; }
        public int tripId { get; set; }
        public int flightId { get; set; }
        public int legNumber { get; set; }
        public int checkListTypeId { get; set; }
        public int checklistRoleTypeId { get; set; }
        public string checklistRoleTypeName { get; set; }
        public int configTaskId { get; set; }
        public string configTaskName { get; set; }
        public int configTaskOrder { get; set; }
        public string selectedByUser { get; set; }
        public DateTime selectedDateTime { get; set; }
        public int checklistSignId { get; set; }
    }

    public class PilotChecklist
    {
        [PrimaryKey, AutoIncrement]
        public int PilotChecklistId { get; set; }
        public int customerId { get; set; }
        public int tripId { get; set; }
        public int flightId { get; set; }
        public int legNumber { get; set; }
        public int checkListTypeId { get; set; }
        public int checklistRoleTypeId { get; set; }
        public string checklistRoleTypeName { get; set; }
        public int configTaskId { get; set; }
        public string configTaskName { get; set; }
        public int configTaskOrder { get; set; }
        public string selectedByUser { get; set; }
        public DateTime selectedDateTime { get; set; }
        public int checklistSignId { get; set; }
    }

    public class MaitenanceChecklist
    {
        [PrimaryKey, AutoIncrement]
        public int MaitenanceChecklistId { get; set; }
        public int customerId { get; set; }
        public int tripId { get; set; }
        public int flightId { get; set; }
        public int legNumber { get; set; }
        public int checkListTypeId { get; set; }
        public int checklistRoleTypeId { get; set; }
        public string checklistRoleTypeName { get; set; }
        public int configTaskId { get; set; }
        public string configTaskName { get; set; }
        public int configTaskOrder { get; set; }
        public string selectedByUser { get; set; }
        public DateTime selectedDateTime { get; set; }
        public int checklistSignId { get; set; }
    }

    public class ChecklistVM
    {

        [PrimaryKey, AutoIncrement]
        [Column("ChecklistVMId")]
        public int? ChecklistVMId { get; set; }
        public int customerId { get; set; }
        public int tripId { get; set; }
        public int flightId { get; set; }
        public int legNumber { get; set; }
        public int checkListTypeId { get; set; }
        public int checklistRoleTypeId { get; set; }
        public string checklistRoleTypeName { get; set; }
        public int configTaskId { get; set; }
        public string configTaskName { get; set; }
        public int configTaskOrder { get; set; }
        public string selectedByUser { get; set; }
        public DateTime selectedDateTime { get; set; }
        public int checklistSignId { get; set; }

        public bool hasLocalModification { get; set; }
        public bool isSelected { get; set; }
        public bool isUpdated { get; set; }
    }

    public class PreflightChecklist
    {
        [PrimaryKey, AutoIncrement]
        public int PreflightChecklistId { get; set; }

        [OneToMany]
        public List<SchedulingChecklist> schedulingChecklists { get; set; }

        [OneToMany]
        public List<PilotChecklist> pilotChecklists { get; set; }

        [OneToMany]
        public List<MaitenanceChecklist> maitenanceChecklists { get; set; }

        [OneToMany]
        public List<object> checklistSignResponse { get; set; }
    }

}
