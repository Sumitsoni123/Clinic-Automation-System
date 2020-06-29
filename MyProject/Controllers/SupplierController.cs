using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Controllers
{
    [MyCustomAuthorize(Roles = "Supplier")]
    public class SupplierController : Controller
    {
        public ActionResult Decision()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Suppliers.SingleOrDefault(a => a.MemberId == mid);
                if (getdata != null)
                {
                    return RedirectToAction("GetDashboard","Supplier");
                }
                else
                    return View("EditProfile");
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
                var totalorder = (from dd in db.DrugDeliveries join pod in db.PatientOrderDetails
                                  on dd.OrderId equals pod.OrderId join s in db.Suppliers
                                  on dd.SupplierId equals s.SupplierId
                                  where s.MemberId == mid
                                  select new { pod.OrderId}).Count();

                var dispatched = (from dd in db.DrugDeliveries join pod in db.PatientOrderDetails
                                  on dd.OrderId equals pod.OrderId join s in db.Suppliers
                                  on dd.SupplierId equals s.SupplierId
                                  where s.MemberId == mid && pod.OrderStatus == "Dispatched"
                                  select new { pod.OrderId}).Count();

                var delivered =  (from dd in db.DrugDeliveries join pod in db.PatientOrderDetails
                                  on dd.OrderId equals pod.OrderId join s in db.Suppliers
                                  on dd.SupplierId equals s.SupplierId
                                  where s.MemberId == mid && pod.OrderStatus == "Delivered"
                                  select new { pod.OrderId}).Count();

                var requested = totalorder - (dispatched + delivered);

                SupplierDashboardModel model = new SupplierDashboardModel();
                model.TotalOrders = totalorder;
                model.Requested = requested;
                model.Dispatched = dispatched;
                model.Delivered = delivered;

                return View("Dashboard", model);
            }
        }

        public ActionResult GetSupplierData()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            using (ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Suppliers.FirstOrDefault(a => a.MemberId == mid);
                SupplierViewModel model = new SupplierViewModel();
                if (getdata != null)
                {
                    model.FirstName = getdata.FirstName;
                    model.LastName = getdata.LastName;
                    model.CompanyName = getdata.CompanyName;
                    model.CompanyAddress = getdata.CompanyAddress;
                    model.MemberId = Convert.ToInt32(getdata.MemberId);
                    return View("EditProfile",model);
                }
                else
                    return View("EditProfile");
            }
        }
        

        public ActionResult EditProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PostSupplierData(SupplierViewModel model)
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            using(ProjectEntities db = new ProjectEntities())
            {
                var getdata = db.Suppliers.FirstOrDefault(a => a.MemberId == mid);

                if(getdata == null)
                {
                    // insert
                    db.InsertSupplier(model.FirstName,model.LastName,model.CompanyName,model.CompanyAddress,mid);

                    ViewBag.Message = "Inserted";
                }
                else
                {
                    // update
                    db.UpdateSupplier(model.FirstName,model.LastName,model.CompanyName,model.CompanyAddress,mid);

                    ViewBag.Message = "Updated";
                }
            }
            return View("Editprofile");
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

        public ActionResult ViewPatientOrder()
        {
            int mid = Convert.ToInt32(Session["MemberId"]);
            List<DrugOrderModel> lst = new List<DrugOrderModel>();
            using(ProjectEntities db = new ProjectEntities())
            {
                var data = db.Suppliers.SingleOrDefault(a => a.MemberId == mid);
                int sid = data.SupplierId;
                var getdata = (from dd in db.DrugDeliveries join pod in db.PatientOrderDetails
                               on dd.OrderId equals pod.OrderId join p in db.Patients 
                               on pod.PatientId equals p.PatientId join d in db.Drugs
                               on pod.DrugId equals d.DrugId
                               where dd.SupplierId == sid
                               select new {p.FirstName,p.Address,pod.OrderId,pod.OrderNumber,pod.Quantity,
                                           d.DrugName,pod.OrderedDate }).ToList();

                foreach (var item in getdata)
                {
                    lst.Add(new DrugOrderModel
                    {
                        OrderId = item.OrderId,
                        OrderNumber = Convert.ToInt32(item.OrderNumber),
                        PatientName = item.FirstName,
                        DrugName = item.DrugName,
                        Address = item.Address,                                                                       
                        Quantity = Convert.ToInt32(item.Quantity),
                        OrderDate = Convert.ToDateTime(item.OrderedDate)
                    });
                }
            }

            DrugOrderModel model = new DrugOrderModel();
            model.lstDrugOrder = lst;
            return View(model);
        }

        [HttpPost]
        public ActionResult PostOrderDetail(DrugOrderModel model)
        {
            using(ProjectEntities db = new ProjectEntities())
            {
                if (model.Decision != null)
                {
                    if(model.Decision == "Dispatched")
                    {
                        var getdata = db.PatientOrderDetails.SingleOrDefault(a => a.OrderId == model.OrderId);
                        string status = getdata.OrderStatus;
                        if(status != "Dispatched")
                        {
                            db.UpdatePatientOrderDetails(model.OrderId, model.Decision);

                            return Json("Item Dispatched");
                        }
                        else
                            return Json("Oops! Item has already been Dispatched");
                    }
                    else
                    {
                        var getdata = db.PatientOrderDetails.SingleOrDefault(a => a.OrderId == model.OrderId);
                        string status = getdata.OrderStatus;
                        if (status == "Dispatched")
                        {
                            db.UpdatePatientOrderDetails(model.OrderId, model.Decision);
                            db.UpdateDrugDelivery(model.OrderId, DateTime.Now);

                            return Json("Item Delivered");
                        }
                        else
                            return Json("Ooops ! Item has not been Dispatched yet");
                    }
                   
                }
                else
                    return Json("Please Choose Your Decision");
            }
        }
    }
}