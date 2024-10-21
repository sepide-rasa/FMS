using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Budget.Controllers
{ 
    public class HeadingLevelController : Controller
    {
        // GET: Budget/HeadingLevel
        WCF_Budget.BudgetService servic = new WCF_Budget.BudgetService();
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Accounting.AccountingService servic_acc = new WCF_Accounting.AccountingService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Budget.ClsError Err = new WCF_Budget.ClsError();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Accounting.ClsError Err_acc = new WCF_Accounting.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }

        public ActionResult GetSal()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_acc.GetFiscalYearWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err_acc).ToList().Select(c => new { fldId=c.fldId, fldYear = c.fldYear});
            return this.Store(q);
        }
        public ActionResult Save(List<WCF_Budget.OBJ_CodingLevel> CodingLevelArray)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                foreach (var item in CodingLevelArray)
                {
                    if (item.fldId == 0)
                    {//ذخیره

                        MsgTitle = "ذخیره موفق";
                        Msg = servic.InsertCodingLevel(item.fldName,item.fldFiscalBudjeId, item.fldArghamNum, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    else
                    {//ویرایش

                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdateCodingLevel(item.fldId, item.fldName, item.fldFiscalBudjeId, item.fldArghamNum, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }        
        public ActionResult Read(StoreRequestParameters parameters, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Budget.OBJ_BudjeLevel> data = null;
            data = servic.GetBudjeLevel(Year, Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Budget.OBJ_BudjeLevel> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}