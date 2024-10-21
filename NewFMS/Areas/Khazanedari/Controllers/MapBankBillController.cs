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
using Newtonsoft.Json.Linq;
using Aspose.Cells.Utility;

namespace NewFMS.Areas.Khazanedari.Controllers
{
    public class MapBankBillController : Controller
    {
        // GET: Khazanedari/MapBankBill
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        public ActionResult Index(string containerId, int FiscalYearId, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new MapBankBill();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData=ViewData
            };
            var q=servic.GetCoding_DetailsWithFilter("fldItemId", FiscalYearId.ToString(), "19", "", 1, IP, out Err).FirstOrDefault();
            var q1=servic.GetCoding_DetailsWithFilter("fldItemId", FiscalYearId.ToString(), "14", "", 1, IP, out Err).FirstOrDefault();
            var q2 = servic.GetCoding_DetailsWithFilter("fldItemId", FiscalYearId.ToString(), "38", "", 1, IP, out Err).FirstOrDefault();
            var q3 = servic.GetCoding_DetailsWithFilter("fldItemId", FiscalYearId.ToString(), "26", "", 1, IP, out Err).FirstOrDefault();
            var q4 = servic.GetCoding_DetailsWithFilter("fldItemId", FiscalYearId.ToString(), "30", "", 1, IP, out Err).FirstOrDefault();

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.FiscalYearId = FiscalYearId.ToString();
            result.ViewBag.Year = Year.ToString();
            result.ViewBag.fldCodingBank = q.fldId;
            result.ViewBag.fldCodingHesab = q1.fldId;
            result.ViewBag.fldCodingCheck = q2.fldId;
            result.ViewBag.fldCodingEntezami_Vosoul = q3.fldId;
            result.ViewBag.fldCodingTarafEntezami_Vosoul = q4.fldId;
            return result;
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
            var data = servic_Com.GetShomareHesabeOmoomiWithFilter("fldOrganId", "", Session["OrganId"].ToString(), "", 0, IP, out Err_Com).ToList()
                .Where(l => l.fldBankId == BankId)
                .Select(c => new { fldId = c.fldId, fldName = c.fldShomareHesab });
            //قرار شد همه شماره حسابهای اون ارگان اینجا لیست بشه
            //var data = servic_Com.GetShomareHesabeOmoomiWithFilter("FishId", Session["OrganId"].ToString(), FiscalYearId.ToString(), BankId.ToString(), 0, IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldShomareHesab });
            return this.Store(data);
        }
        public ActionResult ReadBankBillNotMapped(int FiscalYearId, int ShomareHesabId, byte TransactionType)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Accounting.OBJ_BankBillDetails> data = null;
            if (TransactionType == 1)
                data =servic.GetBankBill_DetailsWithFilter("fldShomareHesabId", FiscalYearId.ToString(), ShomareHesabId.ToString(), 0, IP, out Err)
                    .Where(l => l.fldBedehkar != 0).OrderByDescending(l => l.fldTarikh).ThenByDescending(l => l.fldTime).ToList();
            else
                data = servic.GetBankBill_DetailsWithFilter("fldShomareHesabId", FiscalYearId.ToString(), ShomareHesabId.ToString(), 0, IP, out Err)
                    .Where(l => l.fldBestankar != 0).OrderByDescending(l => l.fldTarikh).ThenByDescending(l => l.fldTime).ToList();
            return this.Store(data);
        }
        public ActionResult ReadDocDetailsNotMapped(int FiscalYearId, int ShomareHesabId, byte TransactionType)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Accounting.OBJ_DocumentRecord_Details> data = null;
            var FieldName = "Fish";
            if (TransactionType==2)
                FieldName = "Check";
            data = servic.SelectHesabDaryaftani(FieldName,FiscalYearId, ShomareHesabId, IP, out Err).ToList();
            return this.Store(data);
        }
        //public ActionResult Save(int BankBillId,string DocDetailsIds,int FiscalYearId)
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    string Msg = "", MsgTitle = ""; var Er = 0;
        //    try
        //    {
        //            //ذخیره
        //        MsgTitle = "ذخیره موفق";
        //        Msg = servic.BankBillMap(BankBillId, FiscalYearId,DocDetailsIds,10,10,IP,"", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), out Err);
        //        if (Err.ErrorType)
        //        {
        //            MsgTitle = "خطا";
        //            Msg = Err.ErrorMsg;
        //            Er = 1;
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        if (x.InnerException != null)
        //            Msg = x.InnerException.Message;
        //        else
        //            Msg = x.Message;

        //        MsgTitle = "خطا";
        //        Er = 1;
        //    }
        //    return Json(new
        //    {
        //        Msg = Msg,
        //        MsgTitle = MsgTitle,
        //        Er = Er

        //    }, JsonRequestBehavior.AllowGet);
        //}
    }
}