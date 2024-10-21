using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using FastMember;
using System.IO;
using NewFMS.Models;
using System.Security.Cryptography;
using FastReport;

namespace NewFMS.Areas.Accounting.Controllers
{
    public class DocRegistrationController : Controller
    {
        //
        // GET: /Accounting/DocRegistration/
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Budget.BudgetService Bud_Service = new WCF_Budget.BudgetService();
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Chek.ChekService servic_Chk = new WCF_Chek.ChekService();

        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Chek.ClsError Err_Chk = new WCF_Chek.ClsError();

        WCF_Archive.ArchiveService ArchiveService = new WCF_Archive.ArchiveService();
        WCF_Archive.ClsError ErrArchive = new WCF_Archive.ClsError();
        WCF_Budget.ClsError ErrBudget = new WCF_Budget.ClsError();
        public ActionResult Index(string containerId, short Year, int FiscalYearId)
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
            var HaveEkhtetamiye = 0;
            var FiscalYear = servic.GetFiscalYearDetail(Convert.ToInt32(FiscalYearId), Convert.ToInt32(Session["OrganId"]), IP, out Err);
            var FiscalYear_Old = servic.GetFiscalYearWithFilter("fldYear", (FiscalYear.fldYear - 1).ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
            if (FiscalYear_Old != null)
            {
                var q1 = servic.GetDocumentRecord_HeaderWithFilter("fldTypeSanadId", "4", "", "", Convert.ToInt32(Session["OrganId"]), FiscalYear_Old.fldYear, 4, 1, IP, out Err).FirstOrDefault();
                if (q1 != null && servic_com.GetDate(IP, out Err_com).fldTarikh.Substring(0, 4) != "1403")
                {
                    HaveEkhtetamiye = 1;
                }
            }
            if (servic_com.GetDate(IP, out Err_com).fldTarikh.Substring(0, 4) == Year.ToString())
            {
                result.ViewBag.Flag = 1;
            }
            else
            {
                result.ViewBag.Flag = 0;
            }
            result.ViewBag.Year = Year;
            result.ViewBag.FiscalYearId = FiscalYearId;
            result.ViewBag.HaveEkhtetamiye = HaveEkhtetamiye;
            return result;
        }
        public ActionResult BackToKhazanedari(int DocId, byte fldModuleErsalId, byte fldModuleSaveId,string DocDate)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (DocDate == "")
                    DocDate = servic_com.GetDate(IP, out Err_com).fldTarikh;
                MsgTitle = "عملیات موفق";
                Msg = servic.Document_CopyInsert(DocId, Convert.ToInt32(Session["OrganId"]), fldModuleErsalId, fldModuleSaveId,2,DocDate, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
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
                Er = Er,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Document(/*string containerId,*/int DocHeaderId,short Year,byte ModuleId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.DocHeaderId=DocHeaderId.ToString();
            result.ViewBag.Year = Year.ToString();
            result.ViewBag.ModuleId = ModuleId.ToString();
            //this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult DocDate(int DocId, byte fldModuleErsalId, byte fldModuleSaveId,byte State,short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();

            var LastDay = MyLib.Shamsi.LastDayOfShamsiYear(Year);
            var MinDate = MyLib.Shamsi.Shamsi2miladiDateTime(Year.ToString() + "/01/01").ToString();
            var MaxDate = MyLib.Shamsi.Shamsi2miladiDateTime(Year.ToString() + "/12/" + LastDay.ToString().PadLeft(2, '0')).ToString();

            result.ViewBag.MinDate = MinDate;
            result.ViewBag.MaxDate = MaxDate;
            result.ViewBag.DocId = DocId;
            result.ViewBag.fldModuleErsalId = fldModuleErsalId;
            result.ViewBag.fldModuleSaveId = fldModuleSaveId;
            result.ViewBag.State = State;
            return result;
        }

        public ActionResult GetAccountingLevel(short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAccountingLevelWithFilter("fldYear", Year.ToString(), "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).OrderBy(l => l.fldId).ToList()
                .Select(c => new { fldId = c.fldLevelId, fldName = c.fldName});
            return this.Store(q);
        }
        public ActionResult RptDaftarRoozname(short Year, string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            return result;
        }
        public ActionResult GeneratePdfRptDaftarRoozname(short Year, string AzTarikh, string TaTarikh, byte TypeSanad,byte Level)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptDaftarRozname.frx";
                rep.Load(path);
                rep.SetParameterValue("organId", Convert.ToInt32(Session["OrganId"]));
                rep.SetParameterValue("ModuleSaveId", 4);
                rep.SetParameterValue("TaTarikh", TaTarikh);
                rep.SetParameterValue("AzTarikh", AzTarikh);
                rep.SetParameterValue("Year", Year);
                rep.SetParameterValue("LevelId", Level);
                rep.SetParameterValue("Type", TypeSanad);
                rep.SetParameterValue("UserName", Session["Username"].ToString());
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
                //DataSet.DataSet1 dt = new DataSet.DataSet1();
                //DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                //dt.EnforceConstraints = false;
                //dt_Com.EnforceConstraints = false;
                //WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
                //WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
                //var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                //DataSet.DataSet1TableAdapters.spr_RptDaftarRoozNameTableAdapter DaftarRoozName = new DataSet.DataSet1TableAdapters.spr_RptDaftarRoozNameTableAdapter();
                //DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                //DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                //DaftarRoozName.Fill(dt.spr_RptDaftarRoozName, Year, Convert.ToInt32(Session["OrganId"]), 4, AzTarikh, TaTarikh, TypeSanad, Level);
                //LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                //GetDate.Fill(dt_Com.spr_GetDate);
                //FastReport.Report Report = new FastReport.Report();
                //Report.RegisterData(dt_Com, "dataSetAccounting");
                //Report.RegisterData(dt, "dataSetAccounting");

                //Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptDaftarRozname.frx");
                //FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                //pdf.EmbeddingFonts = true;
                //MemoryStream stream = new MemoryStream();
                //Report.SetParameterValue("UserName", User.fldNameEmployee);
                //Report.SetParameterValue("AzTarikh", AzTarikh);                
                //Report.SetParameterValue("TaTarikh", TaTarikh);
                //Report.Prepare();
                //Report.Export(pdf, stream);
                //return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintDocument(int DocHeaderId, byte ModuleId, short Year, byte Type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.DocHeaderId = DocHeaderId;
            PartialView.ViewBag.ModuleId = ModuleId;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Type = Type;
            return PartialView;
        }
        public ActionResult GeneratePdfRptDocument(int DocHeaderId,byte ModuleId,short Year,byte Type)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
                WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                var Module = servic_Common.GetModuleDetail(ModuleId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_com);
                var q=servic.GetDocumentRecord_HeaderDetail(DocHeaderId, Convert.ToInt32(Session["OrganId"]),Year,ModuleId,IP,out Err);
                var Tarikh = servic_Common.GetDate(IP,out Err_com).fldTarikh;
                var Module_Organ = servic_com.GetModule_OrganWithFilter("fldOrganId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_com).Where(l => l.fldModuleId == ModuleId).FirstOrDefault();
                var signers = servic_com.GetSignersWithFilter(Module_Organ.fldId,Tarikh, 51, IP, out Err_com).ToList();
                if (signers.Count() == 1)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                }
                else if (signers.Count() == 2)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s4 = signers[1].Post;
                    t4 = signers[1].SignerName;
                }
                else if (signers.Count() == 3)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s3 = signers[1].Post;
                    t3 = signers[1].SignerName;
                    s5 = signers[2].Post;
                    t5 = signers[2].SignerName;
                }
                else if (signers.Count() == 4)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s2 = signers[1].Post;
                    t2 = signers[1].SignerName;
                    s3 = signers[2].Post;
                    t3 = signers[2].SignerName;
                    s4 = signers[3].Post;
                    t4 = signers[3].SignerName;
                }
                else if (signers.Count() == 5)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s2 = signers[1].Post;
                    t2 = signers[1].SignerName;
                    s3 = signers[2].Post;
                    t3 = signers[2].SignerName;
                    s4 = signers[3].Post;
                    t4 = signers[3].SignerName;
                    s5 = signers[4].Post;
                    t5 = signers[4].SignerName;
                }
                DataSet.DataSet1TableAdapters.spr_RptPrintDocumentTableAdapter Document = new DataSet.DataSet1TableAdapters.spr_RptPrintDocumentTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                Document.Fill(dt.spr_RptPrintDocument, DocHeaderId);
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "dataSetAccounting");
                Report.RegisterData(dt, "dataSetAccounting");
                if(Type==1)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptSanadHesabdari.frx");
                else
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptSanadHesabdari-portrait.frx");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("ModuleName", Module.fldTitle);
                Report.SetParameterValue("DocStatus", q.fldTypeSanadName_Pid);
                Report.SetParameterValue("ModuleId", ModuleId);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("s5", s5);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintDocument_Group(string containerId, short Year,int ModuleId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            result.ViewBag.ModuleId = ModuleId;
            return result;
        }
        public ActionResult GeneratePDFPrintDocument_Group(string AzShomareSanad, string TaShomareSanad, short Year, byte TypeSanad, int ModuleId,byte Type)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
                WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                var Module = servic_Common.GetModuleDetail(ModuleId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_com);
                var Tarikh = servic_Common.GetDate(IP, out Err_com).fldTarikh;
                var Module_Organ = servic_com.GetModule_OrganWithFilter("fldOrganId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_com).Where(l => l.fldModuleId == ModuleId).FirstOrDefault();
                var signers = servic_com.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 51, IP, out Err_com).ToList();
                if (signers.Count() == 1)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                }
                else if (signers.Count() == 2)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s4 = signers[1].Post;
                    t4 = signers[1].SignerName;
                }
                else if (signers.Count() == 3)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s3 = signers[1].Post;
                    t3 = signers[1].SignerName;
                    s5 = signers[2].Post;
                    t5 = signers[2].SignerName;
                }
                else if (signers.Count() == 4)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s2 = signers[1].Post;
                    t2 = signers[1].SignerName;
                    s3 = signers[2].Post;
                    t3 = signers[2].SignerName;
                    s4 = signers[3].Post;
                    t4 = signers[3].SignerName;
                }
                else if (signers.Count() == 5)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s2 = signers[1].Post;
                    t2 = signers[1].SignerName;
                    s3 = signers[2].Post;
                    t3 = signers[2].SignerName;
                    s4 = signers[3].Post;
                    t4 = signers[3].SignerName;
                    s5 = signers[4].Post;
                    t5 = signers[4].SignerName;
                }
                DataSet.DataSet1TableAdapters.spr_RptPrintDocument_GroupTableAdapter Document = new DataSet.DataSet1TableAdapters.spr_RptPrintDocument_GroupTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                Document.Fill(dt.spr_RptPrintDocument_Group, Convert.ToInt32(AzShomareSanad), Convert.ToInt32(TaShomareSanad), Year, Convert.ToInt32(Session["OrganId"]), ModuleId, TypeSanad);
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "dataSetAccounting");
                Report.RegisterData(dt, "dataSetAccounting");
                if(Type==1)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptSanadHesabdari_Group.frx");
                else
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptSanadHesabdari_GroupPortrait.frx");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("ModuleName", Module.fldTitle);
                Report.SetParameterValue("ModuleId", ModuleId);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("s5", s5);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SpecificEdit(int HeaderId,short Year,int ModuleId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Setting = servic_com.GetGeneralSettingWithFilter("fldModuleId", "4", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err_com).ToList();
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.HeaderId = HeaderId.ToString();
            result.ViewBag.Year = Year;
            result.ViewBag.ModuleId = ModuleId;
            result.ViewBag.Required = Setting.Where(l => l.fldId == 5).FirstOrDefault().fldValue;
            return result;
        }
        public ActionResult New(int HeaderId, int FiscalYearId,short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //چک کردن اختتامیه
            var q=servic.GetDocumentRecord_HeaderWithFilter("fldTypeSanadId", "4","","", Convert.ToInt32(Session["OrganId"]), Year, 4, 1, IP, out Err).FirstOrDefault();
            if (q != null)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = "پس از ثبت سند اختتامیه، امکان ثبت سند دیگری وجود ندارد."
                });
                DirectResult dr = new DirectResult();
                return dr;
            }
            var HaveEkhtetamiye = 0;
            var FiscalYear = servic.GetFiscalYearDetail(Convert.ToInt32(FiscalYearId), Convert.ToInt32(Session["OrganId"]), IP, out Err);
            var FiscalYear_Old = servic.GetFiscalYearWithFilter("fldYear", (FiscalYear.fldYear - 1).ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
            if (FiscalYear_Old != null)
            {
                var q1 = servic.GetDocumentRecord_HeaderWithFilter("fldTypeSanadId", "4", "", "", Convert.ToInt32(Session["OrganId"]), FiscalYear_Old.fldYear, 4, 1, IP, out Err).FirstOrDefault();
                if (q1 != null && servic_com.GetDate(IP,out Err_com).fldTarikh.Substring(0,4)!="1403")
                {
                    HaveEkhtetamiye = 1;
                }
            }
            var Setting = servic_com.GetGeneralSettingWithFilter("fldModuleId", "4", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err_com).ToList();
            var result = new Ext.Net.MVC.PartialViewResult();
            var CurrentDate = servic_com.GetDate(IP, out Err_com).fldTarikh;
            result.ViewBag.HeaderId = HeaderId.ToString();
            result.ViewBag.CurrentDate = CurrentDate;
            result.ViewBag.FiscalYearId = FiscalYearId;
            result.ViewBag.Year = Year;
            result.ViewBag.HaveEkhtetamiye = HaveEkhtetamiye;
            result.ViewBag.CheckRemain = Setting.Where(l=>l.fldId==4).FirstOrDefault().fldValue;
            result.ViewBag.Required = Setting.Where(l => l.fldId == 5).FirstOrDefault().fldValue;
            return result;
        }
        public ActionResult GeneralView(short Year,int ModuleId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new Document();
            var result = new Ext.Net.MVC.PartialViewResult { 
                ViewData=ViewData
            };
            result.ViewBag.Year = Year;
            result.ViewBag.ModuleId = ModuleId;
            return result;
        }
        //public ActionResult SelFiscalYear(byte Status)
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    var result = new Ext.Net.MVC.PartialViewResult();
        //    var CurrentDate = servic_com.GetDate(IP, out Err_com).fldTarikh;
        //    result.ViewBag.CurrentYear = CurrentDate.Substring(0, 4);
        //    result.ViewBag.Status = Status;
        //    return result;
        //}
        public ActionResult SortDocs(short Year,int ModuleId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            servic.UpdateMoratabSaziTarikhSanad(Year, ModuleId, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
            return Json(new
            {
                Er=Err.ErrorType?1:0,
                Msg=Err.ErrorType?Err.ErrorMsg:"مرتب سازی اسناد با موفقیت انجام شد.",
                MsgTitle = Err.ErrorType ? "خطا" : "عملیات موفق"
            }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetCoding_Project(int ProjectId)
        //{
        //    if (Session["UserId"] == null)
        //        return RedirectToAction("Logon", "Account");
        //    var q=servic.SelectCoding_Project("fldId", ProjectId.ToString(),0, IP, out Err).ToList();
        //    return Json(new
        //    {
        //        fldCodingDetailId = q[0].fldCodingDetailId,
        //        fldCodingTitle=q[0].fldCodingTitle,
        //        data=q.OrderBy(l=>l.fldCodingTitle).ToList().Select(c => new { fldId = c.fldCodingDetailId, 
        //            fldName = "(" + c.fldCodeAcc + ")" + c.fldCodingTitle, fldMahiyatId=c.fldMahiyatId })
        //    }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Archive(int HeaderId, int? FiscalYearId, short? Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model=new ArchiveFilesInf();
            var result = new Ext.Net.MVC.PartialViewResult { ViewData=ViewData};
            result.ViewBag.HeaderId = HeaderId.ToString();
            result.ViewBag.FiscalYearId = FiscalYearId.ToString();
            result.ViewBag.Year = Year.ToString();
            return result;
        }
        public ActionResult LoadArchiveTree(string nod)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var child = ArchiveService.GetArchiveTreeWithFilter("fldPID_Module", nod, "4", Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrArchive).ToList();
            foreach (var ch in child)
            {
                Node childNode = new Node();
                childNode.Text = ch.fldTitle;
                childNode.NodeID = ch.fldId.ToString();
                childNode.Expanded = true;
                childNode.AttributesObject = new
                {
                    fldTitle = ch.fldTitle
                };
                nodes.Add(childNode);
            }
            return this.Store(nodes);
        }
        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            FilesInfo_DataBaseGroup Pictures = new FilesInfo_DataBaseGroup();
           List<FilesInfo_DataBase> Items=new List<FilesInfo_DataBase>();
            try
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Session["ImageUrl" + i] != null)
                    {
                        System.IO.File.Delete(Session["ImageUrl" + i].ToString());
                        Session.Remove("ImageUrl" + i);
                    }
                    var IsImage = FileInfoExtensions.IsImage(Request.Files[i]);
                    var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                    if (IsImage == true)
                    {
                        //if (Request.Files[i].ContentLength <= 10485760)
                        //{
                            HttpPostedFileBase file = Request.Files[i];
                            var Name = Guid.NewGuid();
                            string path = Server.MapPath(@"~\Uploaded\" + Name + extension);
                            file.SaveAs(path);
                            Session["ImageUrl"+i] = path;
                            FilesInfo_DataBase r = new FilesInfo_DataBase()
                            {
                                fldId=0,
                                fldPasvand = extension,
                                Url = @"\Uploaded\" + Name + extension,
                                fldIsBookMark=false
                            };
                            Items.Add(r);
                                                       
                        //}
                        //else
                        //{
                        //    object r = new
                        //    {
                        //        success = true,
                        //        name = Request.Files[0].FileName,
                        //        Message = "حجم فایل انتخابی می بایست کمتر از 10 مگابایت باشد.",
                        //        IsValid = false
                        //    };
                        //    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        //}
                    }
                    else
                    {
                        object r = new
                        {
                            FileName = Request.Files[i].FileName,
                            Message = "فایل انتخاب شده غیر مجاز است.",
                            IsValid = false
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }                    
                }
                Pictures.fldTitle = "";
                Pictures.Items = Items;
                return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(Items)));
            }
            catch (Exception x)
            {
                var Msg=x.Message;
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;

                object r = new
                {
                    FileName = "",
                    Message = Msg,
                    IsValid = false
                };
                return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
            }
        }
        public ActionResult ListFiles(int rowIdx, byte State, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.rowIdx = rowIdx;
            result.ViewBag.State = State;
            result.ViewBag.Year = Year;
            return result;
        }

        public ActionResult ReadFiles(StoreRequestParameters parameters, string Type, short Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<NewFMS.WCF_Accounting.OBJ_ParvandeInfo> data = null;
            if (Type == "" || Type == null)
                Type = "0";

            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<NewFMS.WCF_Accounting.OBJ_ParvandeInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;
                        case "fldStartContract":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStartContract";
                            break;
                        case "fldEndContract":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEndContract";
                            break;
                        case "fldMablagh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablagh";
                            break;
                        case "fldShenasePardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenasePardakht";
                            break;
                        case "fldShenaseGhabz":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenaseGhabz";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHesab";
                            break;
                        case "fldSharh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSharh";
                            break;
                        case "fldType":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldType";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetParvandeInfo(field, searchtext, Convert.ToByte(Type), Convert.ToInt32(Session["OrganId"]),Year, 100, IP, out Err).ToList();
                    else
                        data = servic.GetParvandeInfo(field, searchtext, Convert.ToByte(Type), Convert.ToInt32(Session["OrganId"]), Year, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetParvandeInfo("", "", Convert.ToByte(Type), Convert.ToInt32(Session["OrganId"]), Year, 100, IP, out Err).ToList();
            }

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<NewFMS.WCF_Accounting.OBJ_ParvandeInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult GetDefaultNum_Atf(short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });            
            var q = servic.GetLastNum_AtfDocumentRecord(Year,Convert.ToInt32(Session["OrganId"]),4, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldDocumentNum = q.fldDocumentNum,
                fldAtfNum = q.fldAtfNum,
                ShomareRoozane=q.ShomareRoozane
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFirst_LastOfYear(short Year,int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var LastDate = "";
            var LastDateShamsi = "";

            var q = servic.GetDocumentRecord_HeaderWithFilter("lastDateDoc", Id.ToString(),"","", Convert.ToInt32(Session["OrganId"]), Year, 4, 0, IP, out Err).FirstOrDefault();
            if (q != null)
            {
                LastDate = MyLib.Shamsi.Shamsi2miladiDateTime(q.fldTarikhDocument).ToString();
                LastDateShamsi = q.fldTarikhDocument;
            }
            var LastDay=MyLib.Shamsi.LastDayOfShamsiYear(Year);
            var MinDate = MyLib.Shamsi.Shamsi2miladiDateTime(Year.ToString() + "/01/01").ToString();
            var MaxDate = MyLib.Shamsi.Shamsi2miladiDateTime(Year.ToString() + "/12/" + LastDay.ToString().PadLeft(2,'0')).ToString();
            return Json(new
            {
                MinDate = MinDate,
                MaxDate = MaxDate,
                LastDate = LastDate,
                LastDateShamsi = LastDateShamsi,
                MinDateShamsi = Year.ToString() + "/01/01",
                MaxDateShamsi = Year.ToString() + "/12/" + LastDay.ToString().PadLeft(2, '0')
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHeadLines(short Year,int? ProjectId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (ProjectId != 0 && ProjectId != null)
            {
                var q = servic.SelectCoding_Project("fldId", ProjectId.ToString(), 0, IP, out Err).ToList().Select(c => new
                {
                    fldId = c.fldCodingDetailId,
                    fldName = "(" + c.fldCodeAcc + ")" + c.fldCodingTitle,
                    fldMahiyatId = c.fldMahiyatId,
                    fldItemId=0
                });
                return this.Store(q);
            }
            else
            {
                var q = servic.GetCoding_DetailsWithFilter("Child", Year.ToString(), Session["OrganId"].ToString(), "", 0, IP, out Err).OrderBy(l => l.fldTitle).ToList().Select(c => new { fldId = c.fldId, fldName = "(" + c.fldCode + ")" + c.fldTitle, fldMahiyatId = c.fldMahiyatId, fldItemId=c.fldItemId });
                return this.Store(q);
            }
        }

        public ActionResult GetDocType(short Year,int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q1 = servic.GetDocumentRecord_HeaderWithFilter("fldTypeSanadId", "1", "", "", Convert.ToInt32(Session["OrganId"]), Year, 4, 1, IP, out Err).FirstOrDefault();
            if (q1 != null)
            {
                var q2 = servic.GetDocumentRecord_HeaderWithFilter("fldTypeSanadId", "5", "", "", Convert.ToInt32(Session["OrganId"]), Year, 4, 1, IP, out Err).FirstOrDefault();
                if (q2 != null)//سند بستن حسابها داریم
                {
                    var q = servic.GetDocumentTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Where(l => l.fldId != 1 && l.fldId!=5).Select(c => new { fldId = c.fldId, fldName = c.fldName });
                    return this.Store(q);
                }
                else
                {
                    var q = servic.GetDocumentTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Where(l => l.fldId != 1 && l.fldId != 4).Select(c => new { fldId = c.fldId, fldName = c.fldName });
                    return this.Store(q);
                }
            }
            else
            {
                var q = servic.GetDocumentTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l => l.fldId != 4).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName }).ToList();
                return this.Store(q);
            }            
        }

        public ActionResult GetCostCenter()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCenterCostWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldNameCenter});
            return this.Store(q);
        }

        public ActionResult GetFileType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCaseTypeWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName});
            return this.Store(q);
        }

        public ActionResult GetSal(int ModuleId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var LastYear = 0;
            var q = servic.GetFiscalYearWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldYear = c.fldYear });
            var q1 = servic.GetDocumentRecord_HeaderWithFilter("lastYear", "", "", "", Convert.ToInt32(Session["OrganId"]), 0, ModuleId, 0, IP, out Err).FirstOrDefault();
            if (q1 != null)
            {
                LastYear=q1.fldYear;
            }
            return Json(new { data = q, LastYear = LastYear }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFiscalYear()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetFiscalYearWithFilter("fldOrganId", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).OrderByDescending(l=>l.fldYear).ToList().Select(c => new { fldId = c.fldId, fldYear = c.fldYear });
            return this.Store(q);
        }
        //public ActionResult GetFiscalYear()
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    var q = servic.getdocumentt("fldOrganId", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).OrderByDescending(l => l.fldYear).ToList().Select(c => new { fldId = c.fldId, fldYear = c.fldYear });
        //    return this.Store(q);
        //}

        public ActionResult IsCostCenter(int CodingId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Flag = servic.IsCostCenter(CodingId,IP, out Err);
            return Json(new { Flag = Flag}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadDocRegister_Details(StoreRequestParameters parameters, int HeaderId)
        {
            List<WCF_Accounting.OBJ_DocumentRecord_Details> data = null;
            data = servic.GetDocumentRecord_DetailsWithFilter("fldDocument_HedearId", HeaderId.ToString(), 0, IP, out Err).ToList();
            return this.Store(data);
        }
        public ActionResult ReadDocEkhtetamiye(StoreRequestParameters parameters, byte? DocType, int? FiscalYearId)
        {
            List<WCF_Accounting.OBJ_DocumentRecord_Details> data = null;
            if (DocType==1)
            {
                var FiscalYear=servic.GetFiscalYearDetail(Convert.ToInt32(FiscalYearId),Convert.ToInt32(Session["OrganId"]),IP,out Err);
                var FiscalYear_Old=servic.GetFiscalYearWithFilter("fldYear",(FiscalYear.fldYear-1).ToString(), Convert.ToInt32(Session["OrganId"]),1,IP,out Err).FirstOrDefault();
                data = servic.SelectEftetahiye(FiscalYear_Old.fldId, IP, out Err).OrderBy(l => l.fldBestankar).ThenBy(l => l.fldBedehkar).ToList();
            }
            else if (DocType == 4)
            {
                data = servic.SelectEkhtetamiye(Convert.ToInt32(FiscalYearId), IP, out Err).OrderBy(l => l.fldBestankar).ThenBy(l => l.fldBedehkar).ToList();
            }
            else
            {
                data = servic.SelectBastanHesabha(Convert.ToInt32(FiscalYearId), IP, out Err).OrderBy(l => l.fldBestankar).ThenBy(l => l.fldBedehkar).ToList();
            }
            return this.Store(data);
        }
        public ActionResult GetDocDetails(int HeaderId,short Year,int ModuleId) 
        {
            List<WCF_Accounting.OBJ_DocumentRecord_Details> data = null;
            data = servic.GetDocumentRecord_DetailsWithFilter("fldDocument_HedearId", HeaderId.ToString(), 0, IP, out Err).ToList();
            var Header = servic.GetDocumentRecord_HeaderDetail(HeaderId, Convert.ToInt32(Session["OrganId"]), Year, ModuleId, IP, out Err);
            return Json(new { data = data, 
                fldAtfNum=Header.fldAtfNum, 
                fldDocumentNum=Header.fldDocumentNum,
                fldTarikhDocument=Header.fldTarikhDocument,
                fldDescriptionDocu = Header.fldDescriptionDocu, 
                fldShomareFaree = Header.fldShomareFaree==null?"":Header.fldShomareFaree.ToString(), 
                fldTypeSanadName = Header.fldTypeSanadName,
                ShomareRoozane=Header.ShomareRoozane,
                fldArchiveNum = Header.fldArchiveNum == null ? "" : Header.fldArchiveNum.ToString(),
                fldNameModule = Header.fldNameModule == null ? "" : Header.fldNameModule.ToString(),
                fldTypeName=Header.fldTypeName
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadAllDocs(StoreRequestParameters parameters, short Year, int ModuleId)
        {
            List<WCF_Accounting.OBJ_DocumentRecord_Header> data = null;
            data = servic.GetDocumentRecord_HeaderWithFilter("", "","", "", Convert.ToInt32(Session["OrganId"]),  Year, ModuleId, 0, IP, out Err).Where(l => l.fldDocumentNum > 0).ToList();
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Accounting.OBJ_DocumentRecord_Header> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
            //return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadArchiveFiles(short HeaderId,byte Filter,int? ArchiveTreeId)
        {
            List<FilesInfo_DataBaseGroup> data = null;
            string FieldName="";
            if (ArchiveTreeId == null)
                ArchiveTreeId = 0;
            if (Filter == 1)
            {
                FieldName = "IsNotBookMark";
            }
            else if (Filter == 2)
            {
                FieldName = "fldArchiveTreeId";
            }
            data = servic.SelectDocumentRecorde_File_BookMark(FieldName, HeaderId,Convert.ToInt32(ArchiveTreeId), IP, out Err).OrderBy(s=>s.fldTitle).GroupBy(g => g.fldTitle)
                .Select(l => new FilesInfo_DataBaseGroup()
                {
                    fldTitle = l.First().fldTitle,
                    Items = l.Select(m => new FilesInfo_DataBase()
                    {
                        fldId = m.fldId,
                        fldImage = m.fldImage,
                        fldPasvand = m.fldPasvand,
                        fldIsBookMark = Convert.ToBoolean(m.fldIsBookMark),
                        Url = "data:image/png;base64, " + Convert.ToBase64String(m.fldImage),
                        fldArchiveTreeId=m.fldArchiveTreeId
                    }).ToList()
                }).ToList();
            return new JsonResult()
            {
                Data =data,
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public ActionResult Read(StoreRequestParameters parameters,short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Accounting.OBJ_DocumentRecord_Header> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Accounting.OBJ_DocumentRecord_Header> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldDocumentNum":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDocumentNum";
                            break;
                        case "fldAtfNum":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAtfNum";
                            break;
                        case "ShomareRoozane":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "ShomareRoozane";
                            break;
                        case "fldArchiveNum":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldArchiveNum";
                            break;
                        case "fldShomareFaree":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareFaree";
                            break;
                        case "fldTarikhDocument":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhDocument";
                            break;
                        case "fldDescriptionDocu":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDescriptionDocu";
                            break;
                        case "fldTypeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeName";
                            break;
                        case "fldNameModule":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameModule";
                            break;
                        case "fldMainDocNum":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMainDocNum";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetDocumentRecord_HeaderWithFilter(field, searchtext, "", "", Convert.ToInt32(Session["OrganId"]), Year, 4, 500, IP, out Err).ToList();
                    else
                        data = servic.GetDocumentRecord_HeaderWithFilter(field, searchtext, "", "", Convert.ToInt32(Session["OrganId"]), Year, 4, 500, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetDocumentRecord_HeaderWithFilter("", "", "", "", Convert.ToInt32(Session["OrganId"]), Year, 4, 500, IP, out Err).ToList();
            }

            var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            if (fc != null)
            {
                foreach (var condition in fc.Conditions)
                {
                    string field = condition.FilterProperty.Name;
                    var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
                        }
                    );
                }
            }
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Accounting.OBJ_DocumentRecord_Header> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveFiles(int HeaderId,string DeletedFiles, List<FilesInfo> ArchiveArray)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable { TableName = "acc.tblRecorde_File" };        
                using (var reader = FastMember.ObjectReader.Create(ArchiveArray))
                {
                    dt.Load(reader);
                }
                dt.Columns.Remove("Url");
                MsgTitle = "عملیات موفق";
                servic.InsertDocumentRecorde_File_BookMark(HeaderId,"",dt,DeletedFiles, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                Msg = "عملیات با موفقیت انجام شد.";                    
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var item in ArchiveArray)
                    {
                        if (item.Url.Contains("Uploaded") && System.IO.File.Exists(Server.MapPath(item.Url)))
                            System.IO.File.Delete(Server.MapPath(item.Url));
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
                Err = Er

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveGhati(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MsgTitle = "عملیات موفق";
                servic.UpdateGhati(HeaderId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                Msg = "عملیات با موفقیت انجام شد.";
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
        public ActionResult DisableGhati(int HeaderId, byte TypeSanadId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MsgTitle = "عملیات موفق";
                Msg = "عملیات با موفقیت انجام شد.";
                if (TypeSanadId == 4)
                {
                    var FiscalYear = servic.GetFiscalYearDetail(Convert.ToInt32(FiscalYearId), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    var FiscalYear_Next = servic.GetFiscalYearWithFilter("fldYear", (FiscalYear.fldYear + 1).ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
                    if (FiscalYear_Next != null)
                    {
                        var q1 = servic.GetDocumentRecord_HeaderWithFilter("fldTypeSanadId", "1", "", "", Convert.ToInt32(Session["OrganId"]), FiscalYear_Next.fldYear, 4, 1, IP, out Err).FirstOrDefault();
                        if (q1 != null)
                        {
                            return Json(new
                            {
                                Msg = "به علت ثبت سند افتتاحیه در سال جاری، امکان هیچگونه تغییری در سند اختتامیه وجود ندارد",
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                servic.DisableGhati(HeaderId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);                
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
        public ActionResult UpdateHeader1(int HeaderId, string ArchiveNum, int FareiNum)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MsgTitle = "ویرایش موفق";
                Msg=servic.UpdateArchive_FareiNum(HeaderId,ArchiveNum,FareiNum, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Save(WCF_Accounting.OBJ_DocumentRecord_Header Header, List<NewFMS.Models.DocDetails> DocumentRecord_DetailsArray)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; int Msg2 = 0; var IdHeader = 0; var DocumentNumber = 0;
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable { TableName = "acc.tblDocumentRecord_Details" };
                using (var reader = FastMember.ObjectReader.Create(DocumentRecord_DetailsArray))
                {
                    dt.Load(reader);
                }
                if((Header.fldTypeSanad==1 || Header.fldTypeSanad==4) && Header.fldType==2){
                    Header.fldTypeSanad=2;
                }
                if (Header.fldId == 0)
                {
                    //ذخیره
                    dt.Columns.Remove("fldId");
                    dt.Columns.Remove("fldDocument_HedearId");
                    dt.Columns.Remove("fldCaseId");
                    var NumDoc = servic.InsertDocument(Header.fldArchiveNum, Header.fldDescriptionDocu, Header.fldTarikhDocument, 0, Header.fldType,
                        dt, 4, 4,Header.fldShomareFaree,Convert.ToInt32(Header.fldFiscalYearId),Convert.ToByte(Header.fldTypeSanad),null,Header.fldEdit, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ذخیره موفق";
                    Msg=NumDoc == 0 ? "یادداشت با موفقیت ثبت شد." : "سند شماره " + NumDoc + "  با موفقیت ثبت شد.";
                    //int HeaderId = servic.InsertDocumentRecord_Header(Header.fldDocumentNum,Header.fldAtfNum,Header.fldArchiveNum,Header.fldDescriptionDocu,
                    //    Header.fldTarikhDocument,0,Header.fldType ,"",Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }

                    if (NumDoc != 0)
                    {
                        var Sanad = servic.GetDocumentRecord_HeaderWithFilter("fldDocumentNum", NumDoc.ToString(), "", "", Convert.ToInt32(Session["OrganId"]), Header.fldYear, 4, 1, IP, out Err).FirstOrDefault();
                        IdHeader = Sanad.fldId;
                        DocumentNumber = NumDoc;
                    }

                    
                    //foreach (var item in DocumentRecord_DetailsArray)
                    //{
                    //    Msg=servic.InsertDocumentRecord_Details(HeaderId, item.fldCodingId, item.fldDescription, item.fldBedehkar, item.fldBestankar, item.fldCenterCoId,
                    //        item.fldCaseTypeId, item.fldSourceId, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    //    if (Err.ErrorType)
                    //    {
                    //        var MsgError = Err.ErrorMsg;
                    //        servic.DeleteDocumentRecord_Header(HeaderId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    //        return Json(new
                    //        {
                    //            Msg = MsgError,
                    //            MsgTitle = "خطا",
                    //            Er = 1
                    //        }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = "ویرایش با موفقیت انجام شد.";
                    var q=servic.GetDocumentRecord_HeaderDetail(Header.fldId, Convert.ToInt32(Session["OrganId"]),Header.fldYear,4, IP, out Err);
                    if (q.fldAccept == 1)
                    {
                        return Json(new
                        {
                            Msg = "سند انتخاب شده قطعی شده و امکان ویرایش آن وجود ندارد.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    servic.UpdateDocument(Header.fldId, Header.fldDocumentNum, Header.fldArchiveNum, Header.fldDescriptionDocu, Header.fldTarikhDocument, 0, Header.fldType,
                        dt, 4, 4,Header.fldShomareFaree,Convert.ToByte(Header.fldTypeSanad), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    //servic.UpdateDocumentRecord_Header(Header.fldId, Header.fldArchiveNum, Header.fldDescriptionDocu,
                    //    Header.fldTarikhDocument, 0,Header.fldType, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    IdHeader = Header.fldId;
                    DocumentNumber = Header.fldDocumentNum;
                    ////delete
                    //servic.DeleteDocumentRecord_Details("fldDocument_HedearId", Header.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    //if (Err.ErrorType)
                    //{
                    //    return Json(new
                    //    {
                    //        Msg = Err.ErrorMsg,
                    //        MsgTitle = "خطا",
                    //        Er = 1
                    //    }, JsonRequestBehavior.AllowGet);
                    //}
                    //foreach (var item in DocumentRecord_DetailsArray)
                    //{
                    //    servic.InsertDocumentRecord_Details(Header.fldId, item.fldCodingId, item.fldDescription, item.fldBedehkar, item.fldBestankar, item.fldCenterCoId,
                    //        item.fldCaseTypeId, item.fldSourceId, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    //    if (Err.ErrorType)
                    //    {
                    //        return Json(new
                    //        {
                    //            Msg = Err.ErrorMsg,
                    //            MsgTitle = "خطا",
                    //            Er = 1
                    //        }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
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
                Er = Er,
                IdHeader = IdHeader,
                DocumentNumber = DocumentNumber
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int HeaderId, short Year,byte ModuleId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDocumentRecord_HeaderDetail(HeaderId, Convert.ToInt32(Session["OrganId"]), Year, ModuleId, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldTarikhDocument = q.fldTarikhDocument,
                fldYear = q.fldTarikhDocument.Substring(0, 4),
                fldDocumentNum = q.fldDocumentNum,
                fldAtfNum = q.fldAtfNum,
                fldArchiveNum = q.fldArchiveNum,
                fldDescriptionDocu = q.fldDescriptionDocu,
                ShomareRoozane = q.ShomareRoozane,
                fldShomareFaree = q.fldShomareFaree,
                fldTypeSanad=q.fldTypeSanad.ToString(),
                fldTypeName=q.fldTypeName,
                fldTypeSanadName=q.fldTypeSanadName,
                fldType=q.fldType
            }, JsonRequestBehavior.AllowGet);
        }        
        public ActionResult GetCodingDetails_Tooltip(int fldCodingId, short fldYear)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetRptByCoding(fldCodingId, Convert.ToInt32(Session["OrganId"]),fldYear,4, IP, out Err);
            var Tashkhis = "";
            if (q.fldType == 1)
            {
                Tashkhis = "بدهکار";
            }
            else if (q.fldType == 2)
            {
                Tashkhis = "بستانکار";
            }
            return Json(new
            {
                fldBedehkar = q.fldBedehkar,
                fldBestankar = q.fldBestankar,
                fldTitle=q.fldTitle,
                Tashkhis = Tashkhis,
                fldType=q.fldType,
                MandehHeasb = q.MandehHeasb
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRemain(int fldCodingId, short fldYear,long Bes_Bed,string Type,int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            long Bedehkar = 0,Bestankar=0;
            if (Type == "ColBed")
            {
                Bedehkar = Bes_Bed;
            }
            else
            {
                Bestankar = Bes_Bed;
            }
            var q = servic.CheckRemain(fldCodingId,Id,Convert.ToInt64(Bedehkar),Convert.ToInt64(Bestankar),Convert.ToInt32(Session["OrganId"]), fldYear, 4, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldCheck = q.fldCheck
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteDetail(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteDocumentRecord_Details("fldId",Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
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

        public ActionResult DeleteHeader(int Id, short Year, int ModuleId, byte? fldModuleErsalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذفq
                MsgTitle = "حذف موفق";
                var q = servic.GetDocumentRecord_HeaderDetail(Id, Convert.ToInt32(Session["OrganId"]), Year, ModuleId, IP, out Err);
                if (q.fldAccept == 1)
                {
                    return Json(new
                    {
                        Msg =ModuleId==4?"سند انتخاب شده قطعی شده و امکان حذف آن وجود ندارد.":" سند انتخاب شده به حسابداری ارسال شده و امکان حذف آن وجود ندارد.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (fldModuleErsalId == 12)
                {
                    var q1=servic_Chk.GetCheckHayeVaredeDetail(Convert.ToInt32(q.fldMainDocNum), Convert.ToInt32(Session["OrganId"]), IP, out Err_Chk);
                    if (q1 != null && q1.fldReceive)
                    {
                        return Json(new
                        {
                            Msg = " پرونده چک برای سند مورد نظر ثبت دریافت شده و امکان حذف این سند وجود ندارد.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                var q2 = servic.GetDocumentRecord_DetailsWithFilter("fldDocument_HedearId", Id.ToString(), 0, IP, out Err).Where(l => l.fldCaseTypeId == 3 && l.fldId!=Id).FirstOrDefault();
                if (q2!=null)
                {
                    var q3=servic.GetDocumentRecord_HeaderWithFilter("fldCaseId",q.fldFiscalYearId.ToString(),q2.fldCaseId.ToString(),"26",Convert.ToInt32(Session["OrganId"]),
                        q.fldYear,10,1,IP,out Err).FirstOrDefault();
                    if (q3 != null)
                    {
                        return Json(new
                        {
                            Msg = "امکان حذف سند مورد نظر وجود ندارد.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                Msg = servic.DeleteDocumentRecord_Header(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
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
        public ActionResult CopyDocument(int DocHeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MsgTitle = "عملیات موفق";
                Msg = servic.CopyDocumentWithHeaderId(DocHeaderId, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
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
                Er = Er,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckMahiyatGardesh_Mande(int ID,short Year, int ModuleId,long Bed,long Bes,int CodeDetailId){
            var q = servic.CheckMahiyatGardesh_Mande(ID,Convert.ToInt32(Session["OrganId"]), Year, Bed, Bes, CodeDetailId, ModuleId, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldMahiyat_GardeshId=q.fldMahiyat_GardeshId,
                fldMahiyatGardesh=q.fldMahiyatGardesh,
                fldMahiyatId=q.fldMahiyatId,
                fldMahiyatMonde=q.fldMahiyatMonde
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
