using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Models
{
    public class DoctorViewModel
    {
        public int DoctorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TotalExperience { get; set; }
        public int SpecializedId { get; set; }
        public string Gender { get; set; }
        public int MemberId { get; set; }

        public List<SelectListItem> lstSpecialization { get; set; }   // for ddl Specialization
    }
}