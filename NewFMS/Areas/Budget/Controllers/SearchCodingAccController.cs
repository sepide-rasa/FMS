using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFMS.Areas.Budget.Controllers
{
    public class SearchCodingAccController : Controller
    {
        // GET: Budget/SearchCodingAcc
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        public ActionResult Index(string FieldName, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();

            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.FieldName = FieldName;
            return PartialView;
        }

        public ActionResult Read(string FieldName, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            /*var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);*/
           
            List<WCF_Accounting.OBJ_Coding_Details> data = null;
            data = servic.GetCoding_DetailsWithFilter(FieldName, Year.ToString(), (Session["OrganId"]).ToString(), "", 0, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}