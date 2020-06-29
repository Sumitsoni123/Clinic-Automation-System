using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Models
{
    public class DrugOrderModel
    {
        public int OrderId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string Address { get; set; }
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public int OrderNumber { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }

        public List<SelectListItem> lstDrugName { get; set; } 
        public List<DrugOrderModel> lstDrugOrder { get; set; }

        public int SupplierId { get; set; }
        public List<SelectListItem> lstSupplier { get; set; }

        public string Decision { get; set; }
    }
}