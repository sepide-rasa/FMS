using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Aspose.Cells;
using FastReport;

namespace NewFMS.Areas.Budget.Controllers
{
    public class RptBujePishnahadiController : Controller
    {
        // GET: Budget/RptBujePishnahadi
        WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
        WCF_Budget.BudgetService servicBud = new WCF_Budget.BudgetService();
        WCF_Budget.ClsError ErrBud = new WCF_Budget.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        public ActionResult Index(short Year, string containerId, int state)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.CodingStatus(); ;
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            var CurrentMonth = Convert.ToInt32(servic_Common.GetDate(IP, out ErrCommon).fldTarikh.Substring(5, 2)).ToString();
            result.ViewBag.Year = Year;
            var isKabise = "0";
            if (MyLib.Shamsi.Iskabise(Year)) isKabise = "1";
            result.ViewBag.isKabise = isKabise;
            result.ViewBag.CurrentMonth = CurrentMonth.PadLeft('0', 2);
            result.ViewBag.CurrentDay = servic_Common.GetDate(IP, out ErrCommon).fldTarikh;
            result.ViewBag.CurrentYear = Convert.ToInt32(servic_Common.GetDate(IP, out ErrCommon).fldTarikh.Substring(0, 4)).ToString();
            result.ViewBag.state = state.ToString();
            return result;
        }
        public ActionResult PrintBujePishnahadi(short Year, bool PrintZero, int digits)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q=servicBud.CheckPishbiniBudje(Year, IP, out ErrBud).ToList();
            if (q.Count > 0)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "خطا",
                    Message = ".لطفا اطلاعات مرتبط با پیش بینی سال مربوطه را به صورت کامل وارد کرده و سپس اقدام به گزارش گیری نمایید"
                });
                DirectResult result = new DirectResult();
                result.IsUpload = true;
                return result;
            }
            PartialView.ViewBag.PrintZero = PrintZero;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.digits = digits;
            PartialView.ViewBag.OrganId = Convert.ToInt32(Session["OrganId"]);
            PartialView.ViewBag.UserId = Convert.ToInt32(Session["UserId"]);

            return PartialView;
        }
        public ActionResult PdfBujePishnahadi(int Year, bool PrintZero, int digits)
        {
            try
            {

                var salmali=servic.GetFiscalYearWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault().fldId;
                var Module_OrganId = servic_Common.GetModule_OrganWithFilter("fldOrganId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP,
                    out ErrCommon).Where(l => l.fldModuleId == 7).FirstOrDefault().fldId;

                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Budget\mosavab.frx";
                var curdate=servic_Common.GetDate(IP, out ErrCommon).fldTarikh;
                rep.Load(path);
                rep.SetParameterValue("OrganId", Convert.ToInt32(Session["OrganId"]));
                rep.SetParameterValue("UserId", Convert.ToInt32(Session["UserId"]));
                rep.SetParameterValue("Module_OrganId", Module_OrganId);
                rep.SetParameterValue("aztarikh", "");
                rep.SetParameterValue("tatarikh", "");
                rep.SetParameterValue("azsanad",0);
                rep.SetParameterValue("tasanad", 0);
                rep.SetParameterValue("sanadtype", 2);
                rep.SetParameterValue("salmaliID", salmali);
                rep.SetParameterValue("ReportId", 54);
                rep.SetParameterValue("sal", Year);
                rep.SetParameterValue("Psefr", Convert.ToBoolean(PrintZero));
                rep.SetParameterValue("digits", digits);
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
                if (x.InnerException != null)
                    return Json(x.InnerException.Message + "::::" + x.Message, JsonRequestBehavior.AllowGet);
                else
                    return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintBujeMotamam(short Year, bool PrintZero, int digits)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();

            PartialView.ViewBag.PrintZero = PrintZero;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.digits = digits;
            PartialView.ViewBag.OrganId = Convert.ToInt32(Session["OrganId"]);
            PartialView.ViewBag.UserId = Convert.ToInt32(Session["UserId"]);

            return PartialView;
        }
        public ActionResult PdfBujeMotamam(int Year, bool PrintZero, int digits)
        {
            try
            {
                var salmali = servic.GetFiscalYearWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault().fldId;
                var Module_OrganId = servic_Common.GetModule_OrganWithFilter("CheckOrganId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP,
                    out ErrCommon).Where(l => l.fldModuleId == 7).FirstOrDefault().fldId;
                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Budget\RptBujeMotamam.frx";
                rep.Load(path);
                rep.SetParameterValue("OrganId", Convert.ToInt32(Session["OrganId"]));
                rep.SetParameterValue("UserId", Convert.ToInt32(Session["UserId"]));
                rep.SetParameterValue("Module_OrganId", Module_OrganId);
                rep.SetParameterValue("azTarikh", "");
                rep.SetParameterValue("taTarikh", "");
                rep.SetParameterValue("azsanad", 0);
                rep.SetParameterValue("tasanad", 0);
                rep.SetParameterValue("sanadtype", 2);
                rep.SetParameterValue("salmaliID", salmali);
                rep.SetParameterValue("ReportId", 55);
                rep.SetParameterValue("digits", digits);
                rep.SetParameterValue("sal", Year);
                rep.SetParameterValue("Psefr", Convert.ToBoolean(PrintZero));
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