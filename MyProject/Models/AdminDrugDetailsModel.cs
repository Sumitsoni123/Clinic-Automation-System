using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyProject.Models
{
    public class AdminDrugDetailsModel
    {
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string UsedFor { get; set; }
        public string SideEffects { get; set; }
        public int TotalQuantityAvailable { get; set; }
        public bool IsDeleted { get; set; }

        public List<AdminDrugDetailsModel> lstdrugdetail { get; set; }
    }
}