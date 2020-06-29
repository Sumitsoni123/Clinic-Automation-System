using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Models
{
    public class InboxMessageModel
    {
        public int MessageId { get; set; }
        public string FromEmailId { get; set; }
        public string PatientName { get; set; }                   // to be taken from Session data
        public string ToEmailId { get; set; }
        public string Subject { get; set; }
        public string MessageDetail { get; set; }
        public DateTime MessageDate { get; set; }
        public int ReplyId { get; set; }
        public bool IsRead { get; set; }

        public List<SelectListItem> lstDoctor { get; set; }      // populate ddl Doctor
        public List<InboxMessageModel> lstMessage { get; set; }  //to get list of msgs from controller to view
       
    }
}