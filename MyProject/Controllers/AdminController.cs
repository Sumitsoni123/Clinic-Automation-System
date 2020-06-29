using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MyProject.Controllers
{
    //[MyCustomAuthorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult AuthenticateLogin(LoginViewModel model)
        {
            using(ProjectEntities db = new ProjectEntities())
            {
                var checkuser = (from a in db.Admins
                                 where a.EmailId == model.EmailId && a.Password == model.Password
                                 select new { a.AdminId,a.EmailId,a.Password }).FirstOrDefault();

                string RoleName = "Admin";
                if(checkuser!= null)
                {                   
                    FormsAuthentication.SetAuthCookie(checkuser.EmailId, false);
                    var authTicket = new FormsAuthenticationTicket(1, checkuser.EmailId, DateTime.Now,
                    DateTime.Now.AddMinutes(1), false, RoleName);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    HttpContext.Response.Cookies.Add(authCookie);
                   
                    Session["MemberId"] = checkuser.AdminId;

                    return RedirectToAction("GetDashboard", "Admin", new { id = checkuser.AdminId });                   
                }
                else
                {
                    ViewBag.Message = "Invalid Email Id or Password";
                }

                return View("Login");
            }
        }       

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();                          // it will clear the session[data] after browser closed
            return RedirectToAction("Login", "Admin");
        }

        [MyCustomAuthorize(Roles = "Admin")]
        public ActionResult Dashboard()
        {
            return View();
        }
        [MyCustomAuthorize(Roles = "Admin")]
        public ActionResult GetDashboard()
        {
            using(ProjectEntities db = new ProjectEntities())
            {
                var TotalPatient = db.Patients.Count();
                var TotalDoctor = db.Doctors.Count();
                var TotalSupplier = db.Suppliers.Count();
                var TotalOrders = db.PatientOrderDetails.Count();
                var Assigned = db.DrugDeliveries.Count();
                var NotAssigned = TotalOrders - Assigned;
                var Requested = db.PatientOrderDetails.Where(a => a.OrderStatus == "Requested").Count();
                var Dispatched = db.PatientOrderDetails.Where(a => a.OrderStatus == "Dispatched").Count();
                var Delivered = db.PatientOrderDetails.Where(a => a.OrderStatus == "Delivered").Count();

                AdminDashboardModel model = new AdminDashboardModel();
                model.TotalPatient = TotalPatient;
                model.TotalDoctor = TotalDoctor;
                model.TotalSupplier = TotalSupplier;
                model.TotalOrders = TotalOrders;
                model.Assigned = Assigned;
                model.NotAssigned = NotAssigned;
                model.Requested = Requested;
                model.Dispatched = Dispatched;
                model.Delivered = Delivered;

                return View("Dashboard",model);
            }
            
        }
        [MyCustomAuthorize(Roles = "Admin")]
        public ActionResult GetAdminData()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            AdminViewModel model = new AdminViewModel();

            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Admins.SingleOrDefault(a => a.AdminId == mid);

                if (getdata != null)
                {
                    model.AdminId = getdata.AdminId;
                    model.FirstName = getdata.FirstName;
                    model.LastName = getdata.LastName;
                    Session["EmailId"] = getdata.EmailId;
                    Session["Password"] = getdata.Password;
                    model.Gender = getdata.Gender;
                    model.Address = getdata.Address;

                    return View("Editprofile", model);
                }
                else
                    return View("Editprofile");
            }
        }
        [MyCustomAuthorize(Roles = "Admin")]
        public ActionResult EditProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PostAdminData(AdminViewModel model)
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            using(ProjectEntities db = new ProjectEntities())
            {
                // update admin profile
                db.UpdateAdminProfile(mid,model.FirstName,model.LastName,Session["EmailId"].ToString(),
                                      Session["Password"].ToString(), model.Gender, model.Address);


                ViewBag.Message = "Updated";
            }
            return View("EditProfile");
        }
        [MyCustomAuthorize(Roles = "Admin")]
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
                var getdata = db.Admins.SingleOrDefault(a => a.AdminId == mid);
                if (model.OldPassword == getdata.Password)
                {
                    db.UpdateAdminPassword(mid, model.NewPassword);

                    ViewBag.Message = "Password Updated";
                }
                else
                    ViewBag.Message = "Old Password doesn't matched !";
            }
            return View("ChangePassword");
        }
        [MyCustomAuthorize(Roles = "Admin")]
        public ActionResult AddDrugDetails()
        {
            return View();
        }
        [MyCustomAuthorize(Roles = "Admin")]
        public ActionResult EditDrugDetails(int id)
        {
            AdminDrugDetailsModel model = new AdminDrugDetailsModel();
            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Drugs.SingleOrDefault(a => a.DrugId == id);


                model.DrugId = getdata.DrugId;
                model.DrugName = getdata.DrugName;
                model.ManufactureDate = Convert.ToDateTime(getdata.ManufactureDate);
                model.ExpiredDate = Convert.ToDateTime(getdata.ExpiredDate);
                model.UsedFor = getdata.UsedFor;
                model.SideEffects = getdata.SideEffects;
                model.TotalQuantityAvailable = Convert.ToInt32(getdata.TotalQuantityAvailable);
                model.IsDeleted = Convert.ToBoolean(getdata.IsDeleted);

                return View("AddDrugDetails", model);
            }
            
        }

        [HttpPost]
        public ActionResult PostDrugDetails(AdminDrugDetailsModel model)
        {
            using(ProjectEntities db = new ProjectEntities())
            {
                if(model.DrugId == 0)
                {
                    // insert
                    db.InsertDrugDetails(model.DrugName, model.ManufactureDate, model.ExpiredDate, model.UsedFor,
                                         model.SideEffects,model.TotalQuantityAvailable, model.IsDeleted);

                    ViewBag.Message = "Inserted";
                }
                else
                {
                    // update
                    db.UpdateDrugDetails(model.DrugId, model.DrugName, model.ManufactureDate, model.ExpiredDate,
                                 model.UsedFor, model.SideEffects, model.TotalQuantityAvailable, model.IsDeleted);

                    ViewBag.Message = "Updated";
                }
            }
            return View("AddDrugDetails");
        }
        [MyCustomAuthorize(Roles = "Admin")]
        public ActionResult ViewDrugsDetails()
        {
            List<AdminDrugDetailsModel> lst = new List<AdminDrugDetailsModel>();
            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Drugs.Where(a => a.IsDeleted == false);

                foreach (var item in getdata)
                {
                    lst.Add(new AdminDrugDetailsModel
                    {
                        DrugId = item.DrugId,
                        DrugName = item.DrugName,
                        ManufactureDate = Convert.ToDateTime(item.ManufactureDate),
                        ExpiredDate = Convert.ToDateTime(item.ExpiredDate),
                        UsedFor = item.UsedFor,
                        SideEffects = item.SideEffects,
                        TotalQuantityAvailable = Convert.ToInt32(item.TotalQuantityAvailable),
                    });
                }
            }
            AdminDrugDetailsModel model = new AdminDrugDetailsModel();
            model.lstdrugdetail = lst;
            return View(model);
        }
        [MyCustomAuthorize(Roles = "Admin")]
        public ActionResult DeleteDrugsDetails(int id)
        {
            using(ProjectEntities db = new ProjectEntities())
            {
                db.UpdateDrug(id,true);
                return RedirectToAction("ViewDrugsDetails");
            }
        }
        [MyCustomAuthorize(Roles = "Admin")]
        public ActionResult ViewOrderDetail()
        {
            List<SelectListItem> lstsup = new List<SelectListItem>();
            List<DrugOrderModel> lst = new List<DrugOrderModel>();
            using(ProjectEntities db = new ProjectEntities())
            {
                var data = db.Suppliers.ToList();                 // ddl supplier populated here
                foreach (var item in data)
                {
                    lstsup.Add(new SelectListItem
                    {
                        Value = item.SupplierId.ToString(),
                        Text = item.FirstName
                    });
                }

                var getdata = db.PatientOrderDetails.ToList();    // table record updated here
                foreach (var item in getdata)
                {
                    lst.Add(new DrugOrderModel
                    {
                       OrderId = item.OrderId,
                       PatientId = item.PatientId,
                       PatientName = item.Patient.FirstName,
                       DrugId = item.DrugId,
                       DrugName = item.Drug.DrugName,
                       OrderNumber = Convert.ToInt32(item.OrderNumber),
                       Quantity = Convert.ToInt32(item.Quantity),
                       OrderDate = Convert.ToDateTime(item.OrderedDate)
                    });
                }
            }

            DrugOrderModel model = new DrugOrderModel();       // passing all records collected above to model
            model.lstDrugOrder = lst;
            model.lstSupplier = lstsup;
            return View(model);
        }

        [HttpPost]
        public ActionResult PostOrderDetail(DrugOrderModel model)
        {
            
            using(ProjectEntities db = new ProjectEntities())
            {
                if (model.SupplierId != 0)
                {
                    // insert
                    db.InsertDrugDelivery(model.OrderId, model.SupplierId,null);
                    return Json("assigned");
                }
                else
                    return Json("Choose Supplier");
            }

        }
    }
}