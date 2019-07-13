using ExpensePortal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExpensePortal.Controllers
{
    public enum ApprovalStatus
    {
        Submitted, Approved, Rejected
    }
    public enum Region
    {
        Asia, Europe
    }

    [Authorize(Roles = "Team Lead, Employee")]
    public class ExpenseController : Controller
    {
        ActionResult GetModel(int id, out ExpenseRecord model)
        {
            using (var db = new ExpenseDBEntities())
            {
                model = db.ExpenseRecords.Where(x => x.ExpenseRecordId == id).FirstOrDefault();
                if (model == null)
                    return View("CustomError", null, "This record does not exist.");
                //if (HttpContext.User.IsInRole("Employee") && model.RequesterUserId != HttpContext.User.Identity.Name)
                //    return View("CustomError", null, "This user does not have permission to perform this action.");
            }
            return null;
        }
        public ActionResult Preview(int id)
        {
            ExpenseRecord model;
            var result = GetModel(id, out model);
            if (result != null) return result;
            return PartialView("_PreviewReceipt", model);
        }

        public ActionResult Delete(int id)
        {
            ExpenseRecord model;
            var result = GetModel(id, out model);
            if (result != null) return result;
            using (var db = new ExpenseDBEntities())
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            ExpenseRecord model;
            var result = GetModel(id, out model);
            if (result != null) return result;
            return View(model);
        }

        private int GetRegion(string userId)
        {
            using (var db = new ExpenseDBEntities())
            {
                return db.UserRegions.Where(x => x.Email == userId).FirstOrDefault().RegionId;
            }
        }

        public ActionResult Index()
        {
            var model = new List<ExpenseRecord>();
            if (HttpContext.User.IsInRole("Team Lead"))
            {
                int tlregionId = GetRegion(HttpContext.User.Identity.Name);
                using (var db = new ExpenseDBEntities())
                {
                    foreach (var item in db.ExpenseRecords)
                    {
                        if (GetRegion(item.RequesterUserId) == tlregionId)
                            model.Add(item);
                    }
                    model = model.OrderByDescending(x => x.ExpenseRecordId).ToList();
                }
            }
            else
            {
                using (var db = new ExpenseDBEntities())
                {
                    model = db.ExpenseRecords.Where(x=>x.RequesterUserId == HttpContext.User.Identity.Name).OrderByDescending(x => x.ExpenseRecordId).ToList();
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Approve(ExpenseRecord model, string submitButton)
        {
            var expense = new ExpenseRecord();
            using (var db = new ExpenseDBEntities())
            {
                expense = db.ExpenseRecords.Where(x => x.ExpenseRecordId == model.ExpenseRecordId).FirstOrDefault();
            }
            expense.ApprovalStatusId = submitButton == "Approve" ? (byte)1 : (byte)2;
            expense.ApproverComment = model.ApproverComment;
            expense.ApproverId = HttpContext.User.Identity.Name;
            expense.ApprovedOrRejectedDate = DateTime.Now;
            using (var db = new ExpenseDBEntities())
            {
                db.Entry(expense).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            ModelState.Clear();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Reject(ExpenseRecord model)
        {
            var expense = new ExpenseRecord();
            using (var db = new ExpenseDBEntities())
            {
                expense = db.ExpenseRecords.Where(x => x.ExpenseRecordId == model.ExpenseRecordId).FirstOrDefault();
            }
            expense.ApprovalStatusId = 2;
            expense.ApproverComment = "Looks good";
            expense.ApproverId = HttpContext.User.Identity.Name;
            expense.ApprovedOrRejectedDate = DateTime.Now;
            using (var db = new ExpenseDBEntities())
            {
                db.Entry(expense).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            ModelState.Clear();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Add(ExpenseRecord model)
        {
            var fileName = Path.GetFileNameWithoutExtension(model.ReceiptImageFile.FileName);
            var extn = Path.GetExtension(model.ReceiptImageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extn;

            model.ReceiptImagePath = "~/Image/" + fileName;
            model.ReceiptImageFile.SaveAs(Path.Combine(Server.MapPath("~/Image"), fileName));
            model.ApprovalStatusId = (byte)ApprovalStatus.Submitted;
            model.RequesterUserId = HttpContext.User.Identity.Name;
            model.SubmittedDate = DateTime.Now;

            using (var db = new ExpenseDBEntities())
            {
                db.ExpenseRecords.Add(model);
                db.SaveChanges();
            }
            ModelState.Clear();
            return RedirectToAction("Index");
        }
    }
}