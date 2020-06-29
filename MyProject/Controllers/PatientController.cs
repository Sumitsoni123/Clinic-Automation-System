using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Controllers
{
    [MyCustomAuthorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        public ActionResult Decision()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            using (ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Patients.FirstOrDefault(a => a.MemberId == mid);
                if (getdata != null)
                {
                    return RedirectToAction("GetDashboard", "Patient");
                }
                else
                {
                    return View("EditProfile");
                }
            }
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult GetDashboard()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            
            using (ProjectEntities db = new ProjectEntities())
            {
                var totalapp = (from da in db.DoctorAppointments join p in db.Patients
                                on da.PatientId equals p.PatientId
                                where p.MemberId == mid
                                select new { da.PatientId}).Count();

                var approved = (from da in db.DoctorAppointments join p in db.Patients
                                on da.PatientId equals p.PatientId
                                where p.MemberId == mid && da.AppointmentStatus == "Approved"
                                select new { da.PatientId}).Count();

                var rejected = (from da in db.DoctorAppointments join p in db.Patients
                                on da.PatientId equals p.PatientId
                                where p.MemberId == mid && da.AppointmentStatus == "Rejected"
                                select new { da.PatientId}).Count();

                var requested =  totalapp - (approved + rejected);

                var totalorder = (from pod in db.PatientOrderDetails join p in db.Patients
                                  on pod.PatientId equals p.PatientId
                                  where p.MemberId == mid
                                  select new { pod.PatientId}).Count();

                var delivered = (from pod in db.PatientOrderDetails join p in db.Patients
                                 on pod.PatientId equals p.PatientId
                                 where p.MemberId == mid && pod.OrderStatus == "Delivered"
                                 select new { pod.PatientId}).Count();

                var remaining = totalorder - delivered;

                PatientDashboardModel model = new PatientDashboardModel();

                model.TotalAppointment = totalapp;
                model.AppointmentRequested = requested;
                model.AppointmentApproved = approved;
                model.AppointmentRejected = rejected;
                model.TotalOrders = totalorder;
                model.OrdersDelivered = delivered;
                model.OrdersRemaining = remaining;
                return View("Dashboard",model);
            }          
        }

        [HttpPost]
        public ActionResult PostPatientData(PatientViewModel model)
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Patients.FirstOrDefault(a => a.MemberId == mid);
                if(getdata == null)
                {
                    // insert
                    db.InsertPatient(model.FirstName, model.LastName, model.DateOfBirth,
                                     model.Gender, model.Address, mid);

                    ViewBag.Message = "Inserted";
                }
                else
                {
                    // update
                    db.UpdatePatient(mid,model.FirstName, model.LastName, model.DateOfBirth,
                                     model.Gender,model.Address);
                    
                    ViewBag.Message = "Updated";
                }
            }
            return View("EditProfile");
        }
        
        
        public ActionResult ChangePassword()
        {          
            return View();
        }

        public ActionResult DoctorAppointment()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Doctors.ToList();
                foreach (var item in getdata)
                {
                    lst.Add(new SelectListItem
                    {
                        Value = item.DoctorId.ToString(),
                        Text = item.FirstName
                    });
                }               
            }

            DoctorAppointmentModel model = new DoctorAppointmentModel();
            model.lstDoctor = lst;
            return View(model);
        }

        public ActionResult ViewAppointmentDetails()                            
        {
            int mid = Convert.ToInt32(Session["MemberId"]);           
            using (ProjectEntities db = new ProjectEntities())
            {
                var id = db.Patients.FirstOrDefault(a => a.MemberId == mid);
                int pid = id.PatientId;
                var getdata = db.DoctorAppointments.Where(a => a.PatientId == pid);

                List<DoctorAppointmentModel> lst = new List<DoctorAppointmentModel>();
                foreach (var item in getdata)
                {
                    lst.Add(new DoctorAppointmentModel
                    {
                        DoctorName = item.Doctor.FirstName,
                        Subject = item.Subject,
                        Description = item.Description,
                        AppointmentDate = Convert.ToDateTime(item.AppointmentDate),
                        AppointmentStatus = item.AppointmentStatus
                    });
                }

                DoctorAppointmentModel model = new DoctorAppointmentModel();
                model.lstAppointment = lst;
                return View(model);
            }
        }

        public ActionResult SendMessageToDoctor()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Doctors.ToList();
                foreach (var item in getdata)
                {
                    lst.Add(new SelectListItem
                    {
                       Value = item.DoctorId.ToString(),
                       Text = item.FirstName
                    });
                }
            }

            InboxMessageModel model = new InboxMessageModel();
            model.lstDoctor = lst;
            return View(model);
        }

        [HttpPost]
        public ActionResult PostMessageToDoctor(InboxMessageModel model)
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.MemberLogins.FirstOrDefault(a => a.MemberId == mid);
                string Fromemailid = getdata.EmailId;
                int docid = Convert.ToInt32(model.ToEmailId);
                var data = (from d in db.Doctors join m in db.MemberLogins
                            on d.MemberId equals m.MemberId
                            where d.DoctorId == docid
                            select new { m.EmailId }).FirstOrDefault();
                
                model.ToEmailId = data.EmailId;                
                DateTime msgdate = DateTime.Now;
                int rid = 0;
                bool isread = false;
                
                if(model.ToEmailId != null)
                {
                    db.InsertMessage(Fromemailid, model.ToEmailId, model.Subject, model.MessageDetail, msgdate, rid, isread);
                    ViewBag.Message = "Message Sent To Doctor";

                }
                else
                {
                    ViewBag.Message = "Select Your Doctor";
                }
            }

            List<SelectListItem> lst = new List<SelectListItem>();     // populate ddlDoctor after submit 
            using (ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Doctors.ToList();
                foreach (var item in getdata)
                {
                    lst.Add(new SelectListItem
                    {
                        Value = item.DoctorId.ToString(),
                        Text = item.FirstName
                    });
                }
            }

            DoctorAppointmentModel model1 = new DoctorAppointmentModel();
            model1.lstDoctor = lst;
            return View("DoctorAppointment", model1);
        }

        public ActionResult ViewDoctorMessage()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);

            List<InboxMessageModel> lst = new List<InboxMessageModel>();
            using (ProjectEntities db = new ProjectEntities())
            {
                var gta = db.MemberLogins.FirstOrDefault(a => a.MemberId == mid);
                string emailid = gta.EmailId;
                var getdata = db.Inboxes.Where(a => a.ToEmailId == emailid);

                foreach (var item in getdata)
                {
                    lst.Add(new InboxMessageModel
                    {
                        MessageId = item.MessageId,
                        FromEmailId = item.FromEmailId,
                        ToEmailId = item.ToEmailId,
                        Subject = item.Subject,
                        MessageDetail = item.MessageDetail,
                        MessageDate = Convert.ToDateTime(item.MessageDate)
                    });
                }
            }

            InboxMessageModel model = new InboxMessageModel();
            model.lstMessage = lst;
            return View(model);
        }

        [HttpPost]
        public ActionResult PostPatientReply(InboxMessageModel model)
        {
            bool isread = false;

            using (ProjectEntities db = new ProjectEntities())
            {
                if (model.MessageDetail != null)
                {
                    // Insert Reply
                    db.InsertMessage(model.ToEmailId, model.FromEmailId, model.Subject, model.MessageDetail,
                                     DateTime.Now, model.MessageId, isread);

                    // update IsRead
                    db.UpdateIsRead(model.MessageId, true);

                    return Json("Reply Sent");
                }
                else
                    return Json("Reply Message Body is Empty");
            }
        }

        public ActionResult RaiseDrugOrder()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Drugs.ToList();
                foreach (var item in getdata)
                {
                    lst.Add(new SelectListItem
                    {
                        Value = item.DrugId.ToString(),
                        Text = item.DrugName
                    });
                }
            }

            DrugOrderModel model = new DrugOrderModel();
            model.lstDrugName = lst;
            return View(model);
        }

        [HttpPost]
        public ActionResult PostDrugOrder(DrugOrderModel model)
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            int year = Convert.ToInt32(DateTime.Now.Year);
            int month = Convert.ToInt32(DateTime.Now.Month);
            int minute = Convert.ToInt32(DateTime.Now.Minute);
            int ordernumber = Convert.ToInt32(string.Format("{0}{1}{2}",year,month,minute));
            string orderstatus = "Requested";
            using (ProjectEntities db = new ProjectEntities())
            {
                var id = db.Patients.FirstOrDefault(a => a.MemberId == mid);
                int pid = id.PatientId;

                if(model.DrugName != "Select")
                {
                    // insert
                    db.InsertPatientDrugOrder(pid,model.DrugId,ordernumber,model.Quantity,DateTime.Now,orderstatus);
                    ViewBag.Message = "Order Successful";
                }
                else
                {
                    ViewBag.Message = "Please Select Drug to be Ordered";
                }
            }

            List<SelectListItem> lst = new List<SelectListItem>();
            using (ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Drugs.ToList();
                foreach (var item in getdata)
                {
                    lst.Add(new SelectListItem
                    {
                        Value = item.DrugId.ToString(),
                        Text = item.DrugName
                    });
                }
            }

            DrugOrderModel model1 = new DrugOrderModel();
            model1.lstDrugName = lst;
            return View("RaiseDrugOrder",model1);
        }

        public ActionResult ViewOrderDetails()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            List<DrugOrderModel> lst = new List<DrugOrderModel>();
            using(ProjectEntities db = new ProjectEntities())
            {
                var id = db.Patients.FirstOrDefault(a => a.MemberId == mid);
                int pid = id.PatientId;
                var getdata = db.PatientOrderDetails.Where(a => a.PatientId == pid);

                foreach (var item in getdata)
                {
                    lst.Add(new DrugOrderModel
                    {
                        DrugId = item.DrugId,
                        DrugName = item.Drug.DrugName,
                        OrderNumber = Convert.ToInt32(item.OrderNumber),
                        Quantity  = Convert.ToInt32(item.Quantity),
                        OrderDate = Convert.ToDateTime(item.OrderedDate),
                        OrderStatus = item.OrderStatus
                    });
                }
            }
            DrugOrderModel model = new DrugOrderModel();
            model.lstDrugOrder = lst;
            return View(model);
        }    
        

        public ActionResult GetPatientData()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Patients.FirstOrDefault(a => a.MemberId == mid);
                PatientViewModel model = new PatientViewModel();
                if(getdata !=null)
                {
                    model.FirstName = getdata.FirstName;
                    model.LastName = getdata.LastName;
                    model.DateOfBirth = getdata.DateOfBirth;
                    model.Gender = getdata.Gender;
                    model.Address = getdata.Address;
                    model.MemberId = getdata.MemberId.ToString();
                    return View("EditProfile", model);
                }                
                else
                {
                    return View("EditProfile");
                }
            }
        }

        [HttpPost]
        public ActionResult ValidatePassword(ChangePasswordModel model)
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            using (ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.MemberLogins.FirstOrDefault(a => a.MemberId == mid);
                if (getdata.Password == model.OldPassword)
                {
                    db.UpdatePassword(mid, model.NewPassword);

                    ViewBag.Message = "Password Updated";
                }
                else
                {
                    ViewBag.Message = "Old Password not correct";
                    //return View("ChangePassword");
                }
            }
            return View("ChangePassword");
        }

        [HttpPost]
        public ActionResult PostAppointment(DoctorAppointmentModel model)
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            string apStatus = "Requested";
            using (ProjectEntities db = new ProjectEntities())
            {
                var pid = db.Patients.FirstOrDefault(a => a.MemberId == mid);
                int id = pid.PatientId;
                if(model.AppointmentDate > DateTime.Now)
                {
                    db.InsertDoctorAppointment(id,model.DoctorId,model.Subject,
                                               model.Description,model.AppointmentDate,apStatus);

                    ViewBag.Message = "Appointment Fixed";
                }
                else
                {
                    ViewBag.Message = "Appointment date must be in future";
                    
                }
            }
            List<SelectListItem> lst = new List<SelectListItem>();     // populate ddlDoctor after submit 
            using (ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Doctors.ToList();
                foreach (var item in getdata)
                {
                    lst.Add(new SelectListItem
                    {
                        Value = item.DoctorId.ToString(),
                        Text = item.FirstName
                    });
                }
            }

            DoctorAppointmentModel model1 = new DoctorAppointmentModel();
            model1.lstDoctor = lst;
            return View("DoctorAppointment",model1);
        }
    }
}