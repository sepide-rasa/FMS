using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFMS.Areas.Budget.Controllers
{
    public class SearchCodingBudgetController : Controller
    {
        // GET: Budget/SearchCodingBudget
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Budget.BudgetService servic = new WCF_Budget.BudgetService();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Budget.ClsError Err = new WCF_Budget.ClsError();
        public ActionResult Index(int State, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();

            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.State = State;
            return PartialView;
        }

        public ActionResult Read(int State, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            /*var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);*/
            var fieldName = "Khedmat";
            if (State == 2)
                fieldName = "Proje";
            List<WCF_Budget.OBJ_CodingBudje_Detail> data = null;
            data = servic.GetCodingBudje_DetailWithFilter(fieldName, Year.ToString(), (Session["OrganId"]).ToString(),"", 0, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}