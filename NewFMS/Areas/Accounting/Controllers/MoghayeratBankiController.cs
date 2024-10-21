using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using System.IO;
using NewFMS.Models;
using Aspose.Cells;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;

namespace NewFMS.Areas.Accounting.Controllers
{
    public class MoghayeratBankiController : Controller
    {
        // GET: Accounting/MoghayeratBanki
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        public ActionResult Index(string containerId, int FiscalYearId)
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
            result.ViewBag.FiscalYearId = FiscalYearId.ToString();
            return result;
        }
        public ActionResult GetMaxMinDate(int ShomareHesabId, int FiscalYearId)
        {
            var AzTarikh = "";
            var TaTarikh = "";
            var q = servic.SelectBankBill_Tarikh(FiscalYearId, ShomareHesabId,IP, out Err);
            if (q != null)
            {
                AzTarikh = q.fldMinTarikh;
                TaTarikh = q.fldMaxTarikh;
            }
            return Json(new
            {
                AzTarikh = AzTarikh,
                TaTarikh = TaTarikh

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBank()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var data = servic_Com.GetBankWithFilter("", "", 0, IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldBankName });
            return this.Store(data);
        }
        public ActionResult GetAccountNum(int BankId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var data = servic_Com.GetShomareHesabeOmoomiWithFilter("OrganId", Session["OrganId"].ToString(), FiscalYearId.ToString(), BankId.ToString(), 0, IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldShomareHesab });
            return this.Store(data);
        }
        public ActionResult GetBankBill(int FiscalYearId, int ShomareHesabId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var data = servic.GetBankBill_HeaderWithFilter("fldShomareHesabId", ShomareHesabId.ToString(), 0, IP, out Err).Where(l => l.fldFiscalYearId == FiscalYearId).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(data);
        }
        public ActionResult Read(StoreRequestParameters parameters,int ShomareHesabId, byte State, int FiscalYearId, string AzTarikh, string TaTarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Accounting.OBJ_MoghayeratBanki> data = null;
            string FieldName = "DocumentRecord";
            if(State==2)
                FieldName = "BankBill";
            data = servic.SelectMoghayeratBanki(FieldName, FiscalYearId,AzTarikh,TaTarikh,ShomareHesabId, IP, out Err).OrderBy(l => l.fldTarikh).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }
            List<WCF_Accounting.OBJ_MoghayeratBanki> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}