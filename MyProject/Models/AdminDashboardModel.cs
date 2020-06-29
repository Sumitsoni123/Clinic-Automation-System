using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyProject.Models
{
    public class AdminDashboardModel
    {
        public int TotalPatient { get; set; }
        public int TotalDoctor { get; set; }
        public int TotalSupplier { get; set; }

        public int TotalOrders { get; set; }
        public int Assigned { get; set; }
        public int NotAssigned { get; set; }
        public int Requested { get; set; }
        public int Dispatched { get; set; }
        public int Delivered { get; set; }
    }
}