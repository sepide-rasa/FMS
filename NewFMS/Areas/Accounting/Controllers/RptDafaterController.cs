using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using System.IO;
using Aspose.Cells;
using System.Data;
using FastReport;

namespace NewFMS.Areas.Accounting.Controllers 
{
    public class RptDafaterController : Controller
    {
        // GET: Accounting/RptDafater
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
        public ActionResult Index(string containerId,int FiscalYearId,short Year)
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
            result.ViewBag.Year = Year.ToString();
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult GetAccountingLevel(short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAccountingLevelWithFilter("fldYear", Year.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).OrderBy(l => l.fldId).ToList()
                .Select(c => new { fldId = c.fldId, fldName = c.fldName, fldLevelId=c.fldLevelId });
            return this.Store(q);
        }
        public ActionResult GetCaseType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCaseTypeWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetNextAccountingLevel(short Year, int fldLevelId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAccountingLevelWithFilter("fldYear_AccountLevel", Year.ToString(), fldLevelId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).OrderBy(l => l.fldId).ToList()
                .Select(c => new { fldId = c.fldId, fldName = c.fldName, fldLevelId = c.fldLevelId });
            return this.Store(q);
        }
        public ActionResult GetCodingDetails(int AccountLevelId, byte state,int CodingId, string Code)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            if (state == 1)
            {
                var q = servic.GetCoding_DetailsWithFilter("fldAccountLevelId", AccountLevelId.ToString(), "", "", 0, IP, out Err).ToList()
                .Select(c => new { fldId = c.fldId, fldName = c.fldCode });
                return this.Store(q);
            }
            else
            {
                var q = servic.GetCoding_DetailsWithFilter("fldAccountLevelId_PID", AccountLevelId.ToString(), CodingId.ToString(), Code, 0, IP, out Err).ToList()
                .Select(c => new { fldId = c.fldId, fldName = c.fldCode });
                return this.Store(q);
            }
        }

        public ActionResult GetDocNum(short Year, int DocNum)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDocumentRecord_HeaderWithFilter("", "", "", "", Convert.ToInt32(Session["OrganId"]), Year, 4, 0, IP, out Err).Where(l=>l.fldDocumentNum>=DocNum)
                .OrderBy(l => l.fldDocumentNum).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldDocumentNum });
            return this.Store(q);
        }
        public ActionResult ReadDafater(StoreRequestParameters parameters,int CaseTypeId,int SourceId, string AzCode,string TaCode,int AzSanad,int TaSanad,byte Group,int FiscalYearId)
        {
            List<WCF_Accounting.OBJ_Dafater> data = null;
            data = servic.RptDafater(AzCode, TaCode, AzSanad, TaSanad, Group, FiscalYearId, CaseTypeId,SourceId, IP, out Err).ToList();
            return this.Store(data);
        }
        public ActionResult PrintRptDafater(int CaseTypeId, int SourceId, short Year, string AzCode, string TaCode, int AzSanad, int TaSanad, byte Group, string LevelName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.AzCode = AzCode;
            PartialView.ViewBag.TaCode = TaCode;
            PartialView.ViewBag.AzSanad = AzSanad;
            PartialView.ViewBag.TaSanad = TaSanad;
            PartialView.ViewBag.Group = Group;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.LevelName = LevelName;
            PartialView.ViewBag.CaseTypeId = CaseTypeId;
            PartialView.ViewBag.SourceId = SourceId;
            return PartialView;
        }
        public ActionResult PdfRptDafater(int CaseTypeId, int SourceId, short Year, string AzCode, string TaCode, int AzSanad, int TaSanad, byte Group, string LevelName)
        {
            try
            {
                var salmali = servic.GetFiscalYearWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault().fldId;
                var Module_OrganId = servic_com.GetModule_OrganWithFilter("CheckOrganId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP,
                    out Err_com).Where(l => l.fldModuleId == 4).FirstOrDefault().fldId;
                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptDafater.frx";
                rep.Load(path);
                string Name = "گزارش دفتر " +LevelName+ " ازکد " + AzCode + " تا کد " + TaCode + "\nاز سند " + AzSanad + " تا سند " + TaSanad + "\nسال مالی " + Year;
                string Name2 = "از سند " + AzSanad + " تا سند " + TaSanad + "\nسال مالی " + Year;
                rep.SetParameterValue("organId", Convert.ToInt32(Session["OrganId"]));
                rep.SetParameterValue("fldModule_OrganId", Module_OrganId);
                rep.SetParameterValue("AzCodde", AzCode);
                rep.SetParameterValue("TaCode", TaCode);
                rep.SetParameterValue("AzSanad", AzSanad);
                rep.SetParameterValue("TaSanad", TaSanad);
                rep.SetParameterValue("ReportId", 57);
                rep.SetParameterValue("Group", Group);
                rep.SetParameterValue("UserName", Session["Username"].ToString());
                rep.SetParameterValue("FiscalYearId", salmali);
                rep.SetParameterValue("Name", Name);
                rep.SetParameterValue("Name2", Name2);
                rep.SetParameterValue("CaseTypeId", CaseTypeId);
                rep.SetParameterValue("SourceId", SourceId);
                rep.SetParameterValue("connectionstr", System.Configuration.ConfigurationManager.ConnectionStrings["RasaFMSCommonDBConnectionString"].ConnectionString);

                if (rep.Report.Prepare())
                {
                    // Set PDF export props
                    FastReport.Export.Pdf.PDFExport pdfExport = new FastReport.Export.Pdf.PDFExport();
                    pdfExport.ShowProgress = false;
                    pdfExport.Compressed = true;
                    pdfExport.AllowPrint = true;
                    pdfExport.EmbeddingFonts = true;
                    MemoryStream strm = new MemoryStream();
                    rep.Report.Export(pdfExport, strm);
                    rep.Dispose();
                    pdfExport.Dispose();
                    strm.Position = 0;
                    // return stream in browser
                    return File(strm.ToArray(), "application/pdf");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}