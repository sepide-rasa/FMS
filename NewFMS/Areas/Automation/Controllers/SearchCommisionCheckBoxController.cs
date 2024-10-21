using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFMS.Areas.Automation.Controllers
{
    public class SearchCommisionCheckBoxController : Controller
    {
        //
        // GET: /Automation/SearchCommisionCheckBox/

        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();
        public ActionResult Index(int State,string CommId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.CommId = CommId;
            return PartialView;
        }

        public ActionResult Read(string CommId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Automation.OBJ_Commision> data = null;
            data = servic.GetCommision_Post( Convert.ToInt32(CommId),Convert.ToInt32(Session["OrganId"]),IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }

    }
}
