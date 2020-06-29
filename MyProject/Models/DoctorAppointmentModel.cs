using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Models
{
    public class DoctorAppointmentModel
    {
        public int AppointmentId { get; set; }                         // for decision save button (for doctor)
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }        
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentStatus { get; set; }

        public List<SelectListItem> lstDoctor { get; set; }               // to populate ddl Doctor
        public List<DoctorAppointmentModel> lstAppointment { get; set; }  //to pass list of records from contrler to model

        public string DoctorDecision { get; set; }
    }
}