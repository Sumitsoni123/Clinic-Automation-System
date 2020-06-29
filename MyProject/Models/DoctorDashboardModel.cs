using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyProject.Models
{
    public class DoctorDashboardModel
    {
        public int TotalAppointment { get; set; }
        public int Requested { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }

        public int TotalMessage { get; set; }
        public int Read { get; set; }
        public int Unread { get; set; }
    }
}