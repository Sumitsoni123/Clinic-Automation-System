using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Controllers
{
    [MyCustomAuthorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        public ActionResult Decision()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            using (ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Doctors.FirstOrDefault(a => a.MemberId == mid);
                if (getdata != null)
                {
                    return RedirectToAction("GetDashboard","Doctor");
                }
                else
                {
                    return RedirectToAction("GetDoctorData","Doctor");
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
            using(ProjectEntities db = new ProjectEntities())
            {
                var totalapp = (from da in db.DoctorAppointments join d in db.Doctors
                                on da.DoctorId equals d.DoctorId
                                where d.MemberId == mid
                                select new { da.DoctorId }).Count();
                
                var requested = (from da in db.DoctorAppointments join d in db.Doctors
                                on da.DoctorId equals d.DoctorId
                                where d.MemberId == mid && da.AppointmentStatus == "Requested"
                                select new { da.DoctorId }).Count();

                var approved = (from da in db.DoctorAppointments join d in db.Doctors
                                on da.DoctorId equals d.DoctorId
                                where d.MemberId == mid && da.AppointmentStatus == "Approved"
                                select new { da.DoctorId }).Count();

                var rejected = totalapp - (requested + approved);

                var totalmessage = (from ml in db.MemberLogins join i in db.Inboxes
                                    on ml.EmailId equals i.ToEmailId
                                    where ml.MemberId == mid
                                    select new { i.ToEmailId }).Count();

                 var read =        (from ml in db.MemberLogins join i in db.Inboxes
                                    on ml.EmailId equals i.ToEmailId
                                    where ml.MemberId == mid && i.IsRead == true
                                    select new { i.IsRead }).Count();
                
                var unread = totalmessage - read;

                DoctorDashboardModel model = new DoctorDashboardModel();

                model.TotalAppointment = totalapp;
                model.Requested = requested;
                model.Approved = approved;
                model.Rejected = rejected;
                model.TotalMessage = totalmessage;
                model.Read = read;
                model.Unread = unread;
                return View("Dashboard", model);
            }
        }

        public ActionResult GetDoctorData()
        {            
            int mid = Convert.ToInt32(Session["MemberId"]);
            List<SelectListItem> lst = new List<SelectListItem>();
            DoctorViewModel model = new DoctorViewModel();

            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.SpecializedDatas.ToList();                        
                foreach (var item in getdata)
                {
                    lst.Add(new SelectListItem
                    {
                        Value = item.SpecializedId.ToString(),
                        Text = item.SpecializedName
                    });
                }

                var getdta = db.Doctors.FirstOrDefault(a => a.MemberId == mid);   
                if(getdta != null)
                {
                    model.FirstName = getdta.FirstName;
                    model.LastName = getdta.LastName;
                    model.TotalExperience = Convert.ToInt32(getdta.TotalExperience);
                    model.SpecializedId = Convert.ToInt32(getdta.SpecializedId);
                    model.Gender = getdta.Gender;
                    model.MemberId = mid;
                    model.lstSpecialization = lst;
                    return View("EditProfile", model);

                }
                else
                {
                    model.lstSpecialization = lst;
                    return View("EditProfile",model);
                }
            }

        }

        [HttpPost]
        public ActionResult PostDoctorData(DoctorViewModel model)
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Doctors.FirstOrDefault(a => a.MemberId == mid);
                if(getdata == null)
                {
                    // insert
                    db.InsertDoctor(model.FirstName,model.LastName,model.TotalExperience,
                                    model.SpecializedId,model.Gender,mid);

                    ViewBag.Message = "Inserted";
                }
                else
                {
                    // update
                    db.UpdateDoctor(mid,model.FirstName,model.LastName,model.TotalExperience,
                                    model.SpecializedId,model.Gender);
                    ViewBag.Message = "Updated";
                }
            }

            List<SelectListItem> lst = new List<SelectListItem>();
            using (ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.SpecializedDatas.ToList();
                foreach (var item in getdata)
                {
                    lst.Add(new SelectListItem
                    {
                        Value = item.SpecializedId.ToString(),
                        Text = item.SpecializedName
                    });
                }
            }

            DoctorViewModel model1 = new DoctorViewModel();
            model1.lstSpecialization = lst;
            return View("EditProfile",model1);
        }

        public ActionResult ChangePassword()
        {
            return View();
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
                    
                }
            }
            return View("ChangePassword");
        }

        public ActionResult ViewAppointmentDetails()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            List<DoctorAppointmentModel> lst = new List<DoctorAppointmentModel>();
            using(ProjectEntities db = new ProjectEntities())
            {
                var data = db.Doctors.SingleOrDefault(a => a.MemberId == mid);
                int docid = data.DoctorId;

                var getdata = db.DoctorAppointments.Where(a => a.DoctorId == docid);
                foreach (var item in getdata)
                {
                    lst.Add(new DoctorAppointmentModel
                    {
                        AppointmentId = item.AppointmentId,
                        PatientId = item.PatientId,
                        PatientName = item.Patient.FirstName,
                        Subject = item.Subject,
                        Description = item.Description,
                        AppointmentDate = Convert.ToDateTime(item.AppointmentDate)
                    });
                }                
            }
            DoctorAppointmentModel model = new DoctorAppointmentModel();
            model.lstAppointment = lst;
            return View(model);
        }

        [HttpPost]
        public ActionResult PostAppointmentDecision(DoctorAppointmentModel model)
        {
            int aid = model.AppointmentId;
            using(ProjectEntities db = new ProjectEntities())
            {
                if (model.DoctorDecision != null)
                {
                    var getdata = db.DoctorAppointments.SingleOrDefault(a => a.AppointmentId == aid);
                    string status = getdata.AppointmentStatus;
                    if (status == "Requested")
                    {
                        db.UpdateAppointmentStatus(model.AppointmentId, model.DoctorDecision);

                        return Json("Appointment Status Updated");
                    }
                    else
                        return Json("Decision Already Taken");
                }
                else
                    return Json("Choose Your Decision Wisely");
            }
        }

        
        public ActionResult ViewPatientMessage()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            
            List<InboxMessageModel> lst = new List<InboxMessageModel>();
            using(ProjectEntities db = new ProjectEntities())
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
        public ActionResult PostDoctorReply(InboxMessageModel model)
        {           
            bool isread = false;
            
            using(ProjectEntities db = new ProjectEntities())
            {
                if (model.MessageDetail != null)
                {
                    // Insert Reply
                    db.InsertMessage(model.ToEmailId, model.FromEmailId, model.Subject, model.MessageDetail,
                                     DateTime.Now, model.MessageId, isread);

                    // update IsRead
                    db.UpdateIsRead(model.MessageId,true);

                    return Json("Reply Sent");
                }
                else
                    return Json("Reply Message Body is Empty");
            }
            
        }
    }
}