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
    public class RptTafrighBudjeController : Controller
    {
        // GET: Budget/RptTafrighBudje
        WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
        WCF_Budget.BudgetService servicb = new WCF_Budget.BudgetService();
        WCF_Budget.ClsError Errb = new WCF_Budget.ClsError();


        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        public ActionResult Index(short Year, string containerId)
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

            if (Year == Convert.ToInt16(servic_Common.GetDate(IP, out ErrCommon).fldTarikh.Substring(0, 4)))
            {
                result.ViewBag.CurrentDay = servic_Common.GetDate(IP, out ErrCommon).fldTarikh;
            }
            else
            {
                result.ViewBag.CurrentDay = Year + "/12/" + MyLib.Shamsi.LastDayOfShamsiYear(Year);
            }

            //result.ViewBag.CurrentDay = servic_Common.GetDate(IP, out ErrCommon).fldTarikh;
            //result.ViewBag.CurrentYear = Convert.ToInt32(servic_Common.GetDate(IP, out ErrCommon).fldTarikh.Substring(0, 4)).ToString();

            return result;
        }
        public ActionResult GetAzSanad(short Year, string FilterType, string AzMah, string TaMah, string AzTarikh, string TaTarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var FieldName = "fldAzTarikh_TaTarikhDoc";
            string Az = AzTarikh;
            string Ta = TaTarikh;
            if (FilterType == "1")
            {
                FieldName = "fldAzMah_TaMahDoc";
                Az = AzMah;
                Ta = TaMah;
            }

            var q = servic.GetDocumentRecord_HeaderWithFilter(FieldName, "", Az, Ta, Convert.ToInt32(Session["OrganId"]), Year, 4, 0, IP, out Err).OrderBy(l => l.fldDocumentNum).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldDocumentNum });
            return this.Store(q);
        }
        public ActionResult GetTaSanad(short Year, string FilterType, string AzMah, string TaMah, string AzTarikh, string TaTarikh, string AzSanad)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var FieldName = "fldAzTarikh_TaTarikhDoc";
            string Az = AzTarikh;
            string Ta = TaTarikh;
            if (FilterType == "1")
            {
                FieldName = "fldAzMah_TaMahDoc";
                Az = AzMah;
                Ta = TaMah;
            }
            var q = servic.GetDocumentRecord_HeaderWithFilter(FieldName, AzSanad, Az, Ta, Convert.ToInt32(Session["OrganId"]), Year, 4, 0, IP, out Err).OrderBy(l => l.fldDocumentNum).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldDocumentNum });
            return this.Store(q);
        }
        public ActionResult GetAzMah()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Common.SelectMonth(IP, out ErrCommon).ToList().Select(c => new { fldId = c.fldCode, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetTaMah(string AzMah)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Common.SelectMonth(IP, out ErrCommon).Where(l => Convert.ToInt32(l.fldCode) >= Convert.ToInt32(AzMah)).ToList().Select(c => new { fldId = c.fldCode, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult PrintTafrighBudje(/*string containerId,*/ int Year, string AzTarikh, string TaTarikh, string AzSanad, string TaSanad, bool PrintZero, string sanadtype, int digits)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var az = servic.GetDocumentRecord_HeaderWithFilter("fldId", AzSanad, "", "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt16(Year), 4, 0, IP, out Err).FirstOrDefault();
            var ta = servic.GetDocumentRecord_HeaderWithFilter("fldId", TaSanad, "", "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt16(Year), 4, 0, IP, out Err).FirstOrDefault();
            var ss = servic.GetFiscalYearWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            PartialView.ViewBag.AzTarikh = AzTarikh;
            PartialView.ViewBag.TaTarikh = TaTarikh;
            PartialView.ViewBag.AzSanad = az.fldDocumentNum.ToString();
            PartialView.ViewBag.TaSanad = ta.fldDocumentNum.ToString();
            PartialView.ViewBag.PrintZero = PrintZero;
            PartialView.ViewBag.sanadtype = sanadtype;
            PartialView.ViewBag.salMali = ss.fldId;
            PartialView.ViewBag.sal = Year;
            PartialView.ViewBag.digits = digits;
            PartialView.ViewBag.OrganId = Convert.ToInt32(Session["OrganId"]);
            PartialView.ViewBag.UserId = Convert.ToInt32(Session["UserId"]);

            return PartialView;
        }

        public class Itemman
        {
            public string fldBudjecode;
            public long fldcol1;
            public long fldcol2;
            public long fldcol3;
            public long fldcol4;
            public long fldcol5;
            public long fldcol6;
            public long fldcol7;
            public long fldcol8;
            public long fldcol9;
            public long fldcol10;
            public long fldcol11;
            public long fldcol12;
            public int fldId;
            public int fldnecid;
            public int fldflag;
            public string fldcode;
            public string fldTitle;
            public int fldAcctype;
            public long fldLevel;

        }
        public ActionResult PdfTafrighBudje(short Year, int sal, string AzTarikh, string TaTarikh, string AzSanad, string TaSanad, bool PrintZero, string sanadtype, int digits)
        {
            try
            {
                var Module_OrganId = servic_Common.GetModule_OrganWithFilter("CheckOrganId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP,
                    out ErrCommon).Where(l=>l.fldModuleId==7).FirstOrDefault().fldId;
                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Budget\RptTafrighBudje.frx";
                rep.Load(path);
                rep.SetParameterValue("aztarikh", AzTarikh);
                rep.SetParameterValue("tatarikh", TaTarikh);
                rep.SetParameterValue("salmaliID", Year);
                rep.SetParameterValue("sal", sal);
                rep.SetParameterValue("OrganId", Convert.ToInt32(Session["OrganId"]));
                rep.SetParameterValue("userId", Convert.ToInt32(Session["UserId"]));
                rep.SetParameterValue("Module_OrganId", Module_OrganId);
                rep.SetParameterValue("Tarikh", servic_Common.GetDate(IP, out ErrCommon).fldTarikh);
                rep.SetParameterValue("azsanad", Convert.ToInt32(AzSanad));
                rep.SetParameterValue("tasanad", Convert.ToInt32(TaSanad));
                rep.SetParameterValue("ReportId", 53);
                rep.SetParameterValue("digits", digits);
                rep.SetParameterValue("Psefr", Convert.ToBoolean(PrintZero));
                rep.SetParameterValue("sanadtype", Convert.ToInt32(sanadtype));
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
        public ActionResult ReportPage(short Year, string AzTarikh, string TaTarikh, string AzSanad, string TaSanad, bool PrintZero, string sanadtype, int digits)
        {//باز شدن تب جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Budget\RptTafrighBudje.frx";
            ViewBag.Path = path;
            ViewBag.RId = "Tafrigh" + Session["Username"].ToString() + ";" + Session["Password"].ToString() + ";" + Session["OrganId"].ToString();

            var az = servic.GetDocumentRecord_HeaderWithFilter("fldId", AzSanad, "", "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt16(Year), 4, 0, IP, out Err).FirstOrDefault();
            var ta = servic.GetDocumentRecord_HeaderWithFilter("fldId", TaSanad, "", "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt16(Year), 4, 0, IP, out Err).FirstOrDefault();

            var ss = servic.GetFiscalYearWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            ViewBag.AzTarikh = AzTarikh;
            ViewBag.TaTarikh = TaTarikh;
            ViewBag.AzSanad = az.fldDocumentNum.ToString();
            ViewBag.TaSanad = ta.fldDocumentNum.ToString();
            ViewBag.PrintZero = PrintZero;
            ViewBag.sanadtype = sanadtype;
            ViewBag.salMali = ss.fldId;
            ViewBag.sal = Year;
            ViewBag.digits = digits;
            ViewBag.OrganId = Convert.ToInt32(Session["OrganId"]);
            ViewBag.UserId = Convert.ToInt32(Session["UserId"]);
            ViewBag.Tarikh = servic_Common.GetDate(IP, out ErrCommon).fldTarikh;
            ViewBag.connectionstr = System.Configuration.ConfigurationManager.ConnectionStrings["RasaFMSCommonDBConnectionString"].ConnectionString;
            return View();
        }
        public ActionResult CheckTarikh(string Tarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var sal = Tarikh.Substring(0, 4);
            var dateAz = MyLib.Shamsi.Shamsi2miladiDateTime(sal + "/01/01");
            var dateTa = MyLib.Shamsi.Shamsi2miladiDateTime(sal + "/12/29");
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(sal)))
                dateTa = MyLib.Shamsi.Shamsi2miladiDateTime(sal + "/12/30");


            return Json(new
            {
                dateAz = dateAz.ToString("yyyy-MM-dd"),
                dateTa = dateTa.ToString("yyyy-MM-dd")
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckAllMapping(short Year, string AzTarikh, string TaTarikh, string AzSanad, string TaSanad, string sanadtype)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string Msg = "";
            var ss = servic.GetFiscalYearWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            var porojeData = servicb.SelectCodingAccNotBudje("poroje", AzTarikh, TaTarikh, Convert.ToByte(ss.fldId), Convert.ToByte(Session["OrganId"]), Convert.ToInt32(AzSanad), Convert.ToInt32(TaSanad), Convert.ToByte(sanadtype), IP, out Errb).ToList();
            if (porojeData.Count > 0)
                Msg = "برای بعضی کدهای بودجه ای، پروژه تعریف نشده است.";
            var EghtesadiData = servicb.SelectCodingAccNotBudje("Eghtesadi", AzTarikh, TaTarikh, Convert.ToByte(ss.fldId), Convert.ToByte(Session["OrganId"]), Convert.ToInt32(AzSanad), Convert.ToInt32(TaSanad), Convert.ToByte(sanadtype), IP, out Errb).ToList();
            if (EghtesadiData.Count > 0)
                Msg = "برای بعضی کدهای حسابداری، خدمت تعریف نشده است.";
            return Json(new
            {
                Msg = Msg
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NotInKhedmat(short Year, string AzTarikh, string TaTarikh, string AzSanad, string TaSanad, string sanadtype)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result =new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Year = Year;
            result.ViewBag.AzTarikh = AzTarikh;
            result.ViewBag.TaTarikh = TaTarikh;
            result.ViewBag.AzSanad = AzSanad;
            result.ViewBag.TaSanad = TaSanad;
            result.ViewBag.sanadtype = sanadtype;
            return result;
        }
        public ActionResult ReadCodingAccNotBudje(string FieldName, short Year, string AzTarikh, string TaTarikh, string AzSanad, string TaSanad, string sanadtype)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Budget.OBJ_SelectCodingAccNotBudje> data = null;
            var ss = servic.GetFiscalYearWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            data = servicb.SelectCodingAccNotBudje(FieldName, AzTarikh, TaTarikh, Convert.ToByte(ss.fldId), Convert.ToByte(Session["OrganId"]), Convert.ToInt32(AzSanad), Convert.ToInt32(TaSanad), Convert.ToByte(sanadtype), IP, out Errb).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NotInProject(string containerId, short Year, string AzTarikh, string TaTarikh, string AzSanad, string TaSanad, string sanadtype)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            // this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            result.ViewBag.AzTarikh = AzTarikh;
            result.ViewBag.TaTarikh = TaTarikh;
            result.ViewBag.AzSanad = AzSanad;
            result.ViewBag.TaSanad = TaSanad;
            result.ViewBag.sanadtype = sanadtype;
            return result;
        }
    }
}