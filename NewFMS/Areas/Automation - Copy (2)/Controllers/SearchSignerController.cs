using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFMS.Areas.Automation.Controllers
{
    public class SearchSignerController : Controller
    {
        //
        // GET: /Automation/SearchSigner/

        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();
        public ActionResult Index(int State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            return PartialView;
        }

        public ActionResult Read()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Automation.OBJ_Commision> data = null;
            data = servic.GetCommisionWithFilter("HaveSign","",Convert.ToInt32(Session["OrganId"]),0, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }

    }
}
