using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyProject.Models
{
    public class PatientDashboardModel
    {
        public int TotalAppointment { get; set; }
        public int AppointmentRequested { get; set; }
        public int AppointmentApproved { get; set; }
        public int AppointmentRejected { get; set; }

        public int TotalOrders { get; set; }
        public int OrdersDelivered { get; set; }
        public int OrdersRemaining { get; set; }
    }
}