using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Aspose.Cells;
using System.Data;
using System.Web.Configuration;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Daramad/Report/

        WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
        
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();

        public ActionResult Index(string containerId, string State, string FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            var q = servic_Common.GetDate(IP, out ErrCommon);
            string year = q.fldTarikh.Substring(0, 4);
            result.ViewBag.State = State;
            result.ViewBag.Year = year;
            result.ViewBag.FiscalYearId = FiscalYearId;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult GetSal()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            var q = servic_Common.GetDate(IP, out ErrCommon);
            int fldsal = Convert.ToInt32(q.fldTarikh.Substring(0, 4)) - 7;
            for (int i = fldsal; i < fldsal + 16; i++)
            {
                Models.CurrentDate CboSal = new Models.CurrentDate();

                CboSal.fldYear = i;
                sh.Add(CboSal);
            }
            return Json(sh, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IndexAsli(string containerId, string State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.State = State;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult CheckListVosuli(string StartDate, string EndDate,byte TypeDate, string nodeId, string ShomareHesabId, bool chkDetail)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            dt.EnforceConstraints = false;
            DataSet.DataSet1TableAdapters.spr_RptListCodeDaramadTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptListCodeDaramadTableAdapter();
            int IdShomareHesab = 0;
            if (ShomareHesabId != "")
                IdShomareHesab = Convert.ToInt32(ShomareHesabId);
            if (chkDetail == true)
            {
                ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "Detail", TypeDate);
              
            }
            else
            {
                ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "All", TypeDate);
        
            }
            var q= dt.spr_RptListCodeDaramad.Count();
            bool AllowPrint = true;
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintListVosuli(string StartDate, string EndDate,byte TypeDate, string nodeId, string ShomareHesabId, bool chkDetail)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.StartDate = StartDate;
            PartialView.ViewBag.EndDate = EndDate; 
            PartialView.ViewBag.nodeId = nodeId;
            PartialView.ViewBag.ShomareHesabId = ShomareHesabId;
            PartialView.ViewBag.chkDetail = chkDetail;
            PartialView.ViewBag.TypeDate = TypeDate;
            return PartialView;
        }
        public ActionResult GeneratePDFListVosuli(string StartDate, string EndDate,byte TypeDate, string nodeId, string ShomareHesabId, bool chkDetail)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptListCodeDaramadTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptListCodeDaramadTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetListVosuli?StartDate=" + StartDate + "&EndDate=" + EndDate + "&ShomareHesabId=" + ShomareHesabId + "&nodeId=" + nodeId + "&chkDetail=" + chkDetail + "&TypeDate=" + TypeDate + "&flag=" + Convert.ToInt32(Session["OrganId"]);

                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                int IdShomareHesab = 0;
                if (ShomareHesabId != "")
                    IdShomareHesab =Convert.ToInt32(ShomareHesabId);
                
                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDaramad");
                Report.RegisterData(dt, "rasaFMSDaramad");
                if (chkDetail == true)
                {
                    ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "Detail", TypeDate);
                    if (nodeId == "0")
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoli.frx");
                    else
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoli1.frx");
                }
                else
                {
                    ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "All", TypeDate);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoli2.frx");
                }
                
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("AzTarikh", StartDate);
                Report.SetParameterValue("TaTarikh", EndDate);
                Report.SetParameterValue("barcode", barcode);
                Report.SetParameterValue("TypeDate", TypeDate);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult GenerateXlsListVosuli(string StartDate, string EndDate, byte TypeDate, string nodeId, string ShomareHesabId, bool chkDetail)
        {
            if (Session["Username"] == null)
                return null;
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptListCodeDaramadTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptListCodeDaramadTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetListVosuli?StartDate=" + StartDate + "&EndDate=" + EndDate + "&ShomareHesabId=" + ShomareHesabId + "&nodeId=" + nodeId + "&chkDetail=" + chkDetail + "&TypeDate=" + TypeDate + "&flag=" + Convert.ToInt32(Session["OrganId"]);

                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                int IdShomareHesab = 0;
                if (ShomareHesabId != "")
                    IdShomareHesab = Convert.ToInt32(ShomareHesabId);

                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDaramad");
                Report.RegisterData(dt, "rasaFMSDaramad");
                if (chkDetail == true)
                {
                    ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "Detail", TypeDate);
                    if (nodeId == "0")
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoli.frx");
                    else
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoli1.frx");
                }
                else
                {
                    ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "All", TypeDate);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoli2.frx");
                }

                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("AzTarikh", StartDate);
                Report.SetParameterValue("TaTarikh", EndDate);
                Report.SetParameterValue("barcode", barcode);
                Report.SetParameterValue("TypeDate", TypeDate);


                FastReport.Export.OoXML.Excel2007Export xlsExport = new FastReport.Export.OoXML.Excel2007Export();


                MemoryStream stream = new MemoryStream();

                Report.Prepare();

                Report.Export(xlsExport, stream);
                return File(stream.ToArray(), "xls", "ListVosuli.xls");
            }
            catch (Exception x)
            {
                return null;
            }
        }
        public ActionResult GetListVosuli(string StartDate, string EndDate,byte TypeDate, string nodeId, string ShomareHesabId, bool chkDetail, int flag)
        {
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            ViewBag.ShomareHesabId = ShomareHesabId;
            ViewBag.nodeId = nodeId;
            ViewBag.chkDetail = chkDetail;
            ViewBag.TypeDate = TypeDate;
            ViewBag.flag = flag;
            return View();
        }
        public FileResult DownloadListVosuli(string StartDate, string EndDate,byte TypeDate, string nodeId, string ShomareHesabId, bool chkDetail, int flag)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

            dt.EnforceConstraints = false;
            dt_Com.EnforceConstraints = false;
            DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
            DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
            DataSet.DataSet1TableAdapters.spr_RptListCodeDaramadTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptListCodeDaramadTableAdapter();

            WCF_Common.OBJ_User User = null;
            if (Session["Username"] != null)
                User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
            string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetListVosuli?StartDate=" + StartDate + "&EndDate=" + EndDate + "&ShomareHesabId=" + ShomareHesabId + "&nodeId=" + nodeId + "&chkDetail=" + chkDetail + "&TypeDate=" + TypeDate + "&flag=" + flag;

            LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, flag);
            GetDate.Fill(dt_Com.spr_GetDate);
            int IdShomareHesab = 0;
            if (ShomareHesabId != "")
                IdShomareHesab = Convert.ToInt32(ShomareHesabId);

            FastReport.Report Report = new FastReport.Report();
            Report.RegisterData(dt_Com, "rasaFMSDaramad");
            Report.RegisterData(dt, "rasaFMSDaramad");
            if (chkDetail == true)
            {
                ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), flag, "Detail", TypeDate);
                if (nodeId == "0")
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoli.frx");
                else
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoli1.frx");
            }
            else
            {
                ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), flag, "All", TypeDate);
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoli2.frx");
            }
            if (Session["Username"] != null)
                Report.SetParameterValue("UserName", User.fldNameEmployee);
            else
                Report.SetParameterValue("UserName", "-");
            Report.SetParameterValue("AzTarikh", StartDate);
            Report.SetParameterValue("TaTarikh", EndDate);
            Report.SetParameterValue("barcode", barcode);
            Report.SetParameterValue("TypeDate", TypeDate);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);
            //  return File(stream.ToArray(), "application/pdf");

            return File(stream.ToArray(), "application/pdf", "PrintForm.pdf");
            //else
            //    return null;

        }
        public ActionResult CheckListVosuliAsli(string StartDate, string EndDate, string nodeId, string ShomareHesabId, bool chkDetail, byte TypeDate)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_RptListCodeDaramad_AsliTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptListCodeDaramad_AsliTableAdapter();
            dt.EnforceConstraints = false;
            int IdShomareHesab = 0;
            if (ShomareHesabId != "")
                IdShomareHesab = Convert.ToInt32(ShomareHesabId);

            if (chkDetail == true)
            {
                ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad_Asli, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "Detail", TypeDate);
            }
            else
            {
                ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad_Asli, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "All", TypeDate);
            }
            var AllowPrint = true;
            var q = dt.spr_RptListCodeDaramad_Asli.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintListVosuliAsli(string StartDate, string EndDate,byte TypeDate, string nodeId, string ShomareHesabId, bool chkDetail)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.StartDate = StartDate;
            PartialView.ViewBag.EndDate = EndDate;
            PartialView.ViewBag.nodeId = nodeId;
            PartialView.ViewBag.ShomareHesabId = ShomareHesabId;
            PartialView.ViewBag.chkDetail = chkDetail;
            PartialView.ViewBag.TypeDate = TypeDate;
            return PartialView;
        }
        public ActionResult GeneratePDFListVosuliAsli(string StartDate, string EndDate,byte TypeDate, string nodeId, string ShomareHesabId, bool chkDetail)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptListCodeDaramad_AsliTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptListCodeDaramad_AsliTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();

                string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetListVosuliAsli?StartDate=" + StartDate + "&EndDate=" + EndDate + "&ShomareHesabId=" + ShomareHesabId + "&nodeId=" + nodeId + "&chkDetail=" + chkDetail + "&TypeDate=" + TypeDate + "&flag=" + Convert.ToInt32(Session["OrganId"]);

                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                int IdShomareHesab = 0;
                if (ShomareHesabId != "")
                    IdShomareHesab = Convert.ToInt32(ShomareHesabId);

                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDaramad");
                Report.RegisterData(dt, "rasaFMSDaramad");
                if (chkDetail == true)
                {
                    ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad_Asli, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "Detail", TypeDate);
                    if (nodeId == "0")
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoliAsli.frx");
                    else
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoliAsli1.frx");
                }
                else
                {
                    ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad_Asli, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "All", TypeDate);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoliAsli2.frx");
                }

                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("AzTarikh", StartDate);
                Report.SetParameterValue("TaTarikh", EndDate);
                Report.SetParameterValue("barcode", barcode);
                Report.SetParameterValue("TypeDate", TypeDate);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult GenerateXlsListVosuliAsli(string StartDate, string EndDate, byte TypeDate, string nodeId, string ShomareHesabId, bool chkDetail)
        {
            if (Session["Username"] == null)
                return null;
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptListCodeDaramad_AsliTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptListCodeDaramad_AsliTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();

                string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetListVosuliAsli?StartDate=" + StartDate + "&EndDate=" + EndDate + "&ShomareHesabId=" + ShomareHesabId + "&nodeId=" + nodeId + "&chkDetail=" + chkDetail + "&TypeDate=" + TypeDate + "&flag=" + Convert.ToInt32(Session["OrganId"]);

                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                int IdShomareHesab = 0;
                if (ShomareHesabId != "")
                    IdShomareHesab = Convert.ToInt32(ShomareHesabId);

                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDaramad");
                Report.RegisterData(dt, "rasaFMSDaramad");
                if (chkDetail == true)
                {
                    ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad_Asli, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "Detail", TypeDate);
                    if (nodeId == "0")
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoliAsli.frx");
                    else
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoliAsli1.frx");
                }
                else
                {
                    ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad_Asli, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), Convert.ToInt32(Session["OrganId"]), "All", TypeDate);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoliAsli2.frx");
                }

                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("AzTarikh", StartDate);
                Report.SetParameterValue("TaTarikh", EndDate);
                Report.SetParameterValue("barcode", barcode);
                Report.SetParameterValue("TypeDate", TypeDate);
              

                FastReport.Export.OoXML.Excel2007Export xlsExport = new FastReport.Export.OoXML.Excel2007Export();


                MemoryStream stream = new MemoryStream();

                Report.Prepare();

                Report.Export(xlsExport, stream);
                return File(stream.ToArray(), "xls", "VosoliAsli.xls");
            }
            catch (Exception x)
            {
                return null;
            }
        }
        public ActionResult GetListVosuliAsli(string StartDate, string EndDate,byte TypeDate, string nodeId, string ShomareHesabId, bool chkDetail,int flag)
        {
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            ViewBag.ShomareHesabId = ShomareHesabId;
            ViewBag.nodeId = nodeId;
            ViewBag.chkDetail = chkDetail;
            ViewBag.TypeDate = TypeDate;
            ViewBag.flag = flag;
            return View();
        }
        public FileResult DownloadListVosuliAsli(string StartDate, string EndDate,byte TypeDate, string nodeId, string ShomareHesabId, bool chkDetail, int flag)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
            dt.EnforceConstraints = false;
            dt_Com.EnforceConstraints = false;

            DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
            DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
            DataSet.DataSet1TableAdapters.spr_RptListCodeDaramad_AsliTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptListCodeDaramad_AsliTableAdapter();
            
            WCF_Common.OBJ_User User = null;
            if (Session["Username"] != null)
                User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();

            string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetListVosuliAsli?StartDate=" + StartDate + "&EndDate=" + EndDate + "&ShomareHesabId=" + ShomareHesabId + "&nodeId=" + nodeId + "&chkDetail=" + chkDetail + "&TypeDate=" + TypeDate + "&flag=" + flag;

            LogoReport.Fill(dt_Com.spr_LogoReportWithUserId,flag);
            GetDate.Fill(dt_Com.spr_GetDate);
            int IdShomareHesab = 0;
            if (ShomareHesabId != "")
                IdShomareHesab = Convert.ToInt32(ShomareHesabId);

            FastReport.Report Report = new FastReport.Report();
            Report.RegisterData(dt_Com, "rasaFMSDaramad");
            Report.RegisterData(dt, "rasaFMSDaramad");
            if (chkDetail == true)
            {
                ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad_Asli, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), flag, "Detail", TypeDate);
                if (nodeId == "0")
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoliAsli.frx");
                else
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoliAsli1.frx");
            }
            else
            {
                ListCodeDaramad.Fill(dt.spr_RptListCodeDaramad_Asli, StartDate, EndDate, IdShomareHesab, Convert.ToInt32(nodeId), flag, "All", TypeDate);
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptVosoliAsli2.frx");
            }
            if (Session["Username"] != null)
                Report.SetParameterValue("UserName", User.fldNameEmployee);
            else
                Report.SetParameterValue("UserName", "-");
            Report.SetParameterValue("AzTarikh", StartDate);
            Report.SetParameterValue("TaTarikh", EndDate);
            Report.SetParameterValue("barcode", barcode);
            Report.SetParameterValue("TypeDate", TypeDate);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);
            //  return File(stream.ToArray(), "application/pdf");

            return File(stream.ToArray(), "application/pdf", "PrintForm.pdf");
            //else
            //    return null;

        }
        public ActionResult CheckFishPardakhti_codeD(string StartDate, string EndDate,byte TypeDate, string ShomareHesabId, string nodeId)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_RptFishPardakhtShode_CodeDaramdTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptFishPardakhtShode_CodeDaramdTableAdapter();
            dt.EnforceConstraints = false;
            if (ShomareHesabId == "")
                ShomareHesabId = "0";
            ListCodeDaramad.Fill(dt.spr_RptFishPardakhtShode_CodeDaramd, StartDate, EndDate, Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(ShomareHesabId), nodeId, TypeDate);
            var AllowPrint = true;
            var q = dt.spr_RptFishPardakhtShode_CodeDaramd.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintFishPardakhti_codeD(string StartDate, string EndDate, byte TypeDate, string ShomareHesabId, string nodeId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.StartDate = StartDate;
            PartialView.ViewBag.EndDate = EndDate;
            PartialView.ViewBag.nodeId = nodeId;
            PartialView.ViewBag.TypeDate = TypeDate;
            PartialView.ViewBag.ShomareHesabId = ShomareHesabId;
            return PartialView;
        }
        public ActionResult GeneratePDFFishPardakhti_codeD(string StartDate, string EndDate, byte TypeDate,string ShomareHesabId, string nodeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;

                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptFishPardakhtShode_CodeDaramdTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptFishPardakhtShode_CodeDaramdTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetFishPardakhti_codeD?StartDate=" + StartDate + "&EndDate=" + EndDate + "&ShomareHesabId=" + ShomareHesabId + "&nodeId=" + nodeId + "&TypeDate=" + TypeDate + "&flag=" + Convert.ToInt32(Session["OrganId"]);
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);

                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt, "rasaFMSDaramad");
                if (ShomareHesabId == "")
                    ShomareHesabId = "0";
                ListCodeDaramad.Fill(dt.spr_RptFishPardakhtShode_CodeDaramd, StartDate, EndDate, Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(ShomareHesabId), nodeId, TypeDate);
                if (nodeId == "0")
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptFishPardakhti_Daramadcode.frx");
                else
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptFishPardakhti_Daramadcode1.frx");

                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("AzTarikh", StartDate);
                Report.SetParameterValue("TaTarikh", EndDate);
                Report.SetParameterValue("barcode", barcode);
                Report.SetParameterValue("TypeDate", TypeDate);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult GenerateXlsFishPardakhti_codeD(string StartDate, string EndDate, byte TypeDate, string ShomareHesabId, string nodeId)
        {
            if (Session["Username"] == null)
                return null;
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;

                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptFishPardakhtShode_CodeDaramdTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptFishPardakhtShode_CodeDaramdTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetFishPardakhti_codeD?StartDate=" + StartDate + "&EndDate=" + EndDate + "&ShomareHesabId=" + ShomareHesabId + "&nodeId=" + nodeId + "&TypeDate=" + TypeDate + "&flag=" + Convert.ToInt32(Session["OrganId"]);
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);

                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt, "rasaFMSDaramad");
                if (ShomareHesabId == "")
                    ShomareHesabId = "0";
                ListCodeDaramad.Fill(dt.spr_RptFishPardakhtShode_CodeDaramd, StartDate, EndDate, Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(ShomareHesabId), nodeId, TypeDate);
                if (nodeId == "0")
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptFishPardakhti_Daramadcode.frx");
                else
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptFishPardakhti_Daramadcode1.frx");

                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("AzTarikh", StartDate);
                Report.SetParameterValue("TaTarikh", EndDate);
                Report.SetParameterValue("barcode", barcode);
                Report.SetParameterValue("TypeDate", TypeDate);

                FastReport.Export.OoXML.Excel2007Export xlsExport = new FastReport.Export.OoXML.Excel2007Export();


                MemoryStream stream = new MemoryStream();

                Report.Prepare();

                Report.Export(xlsExport, stream);
                return File(stream.ToArray(), "xls", "FishPardakhti_Daramadcode.xls");
            }
            catch (Exception x)
            {
                return null;
            }
        }
        public ActionResult GetFishPardakhti_codeD(string StartDate, string EndDate,byte TypeDate, string ShomareHesabId, string nodeId, int flag)
        {
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
            ViewBag.ShomareHesabId = ShomareHesabId;
            ViewBag.nodeId = nodeId;
            ViewBag.TypeDate = TypeDate;
            ViewBag.flag = flag;
            return View();
        }
        public FileResult DownloadRptFishPardakhti_codeD(string StartDate, string EndDate,byte TypeDate, string ShomareHesabId, string nodeId, int flag)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
            dt.EnforceConstraints = false;
            dt_Com.EnforceConstraints = false;

            DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
            DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
            DataSet.DataSet1TableAdapters.spr_RptFishPardakhtShode_CodeDaramdTableAdapter ListCodeDaramad = new DataSet.DataSet1TableAdapters.spr_RptFishPardakhtShode_CodeDaramdTableAdapter();
            
            WCF_Common.OBJ_User User = null;
             if (Session["Username"] != null)
             User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();

             string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetFishPardakhti_codeD?StartDate=" + StartDate + "&EndDate=" + EndDate + "&ShomareHesabId=" + ShomareHesabId + "&nodeId=" + nodeId + "&TypeDate=" + TypeDate + "&flag=" + flag;
            LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, flag);
            GetDate.Fill(dt_Com.spr_GetDate);

            FastReport.Report Report = new FastReport.Report();
            Report.RegisterData(dt_Com, "rasaFMSCommon");
            Report.RegisterData(dt, "rasaFMSDaramad");
            if (ShomareHesabId == "")
                ShomareHesabId = "0";
            ListCodeDaramad.Fill(dt.spr_RptFishPardakhtShode_CodeDaramd, StartDate, EndDate, flag, Convert.ToInt32(ShomareHesabId), nodeId, TypeDate);
            if (nodeId == "0")
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptFishPardakhti_Daramadcode.frx");
            else
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptFishPardakhti_Daramadcode1.frx");

            if (Session["Username"] != null)
                Report.SetParameterValue("UserName", User.fldNameEmployee);
            else
                Report.SetParameterValue("UserName", "-");
            Report.SetParameterValue("AzTarikh", StartDate);
            Report.SetParameterValue("TaTarikh", EndDate);
            Report.SetParameterValue("barcode", barcode);
            Report.SetParameterValue("TypeDate", TypeDate);
            FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
            pdf.EmbeddingFonts = true;
            MemoryStream stream = new MemoryStream();
            Report.Prepare();
            Report.Export(pdf, stream);
            //  return File(stream.ToArray(), "application/pdf");

            return File(stream.ToArray(), "application/pdf", "PrintForm.pdf");
            //else
            //    return null;

        }
        public ActionResult CheckRptMeasureUnit()
         {
             DataSet.DataSet1 dt = new DataSet.DataSet1();
             DataSet.DataSet1TableAdapters.spr_tblComboBoxSelectTableAdapter ComboBox = new DataSet.DataSet1TableAdapters.spr_tblComboBoxSelectTableAdapter();
             dt.EnforceConstraints = false;
            ComboBox.Fill(dt.spr_tblComboBoxSelect, "", "", 0);
             var AllowPrint = true;
             var q = dt.spr_tblComboBoxSelect.Count();
             if (q == 0)
                 AllowPrint = false;
             return Json(new {
                 AllowPrint = AllowPrint
             },JsonRequestBehavior.AllowGet);
         }
         public ActionResult PrintRptMeasureUnit(string containerId)
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
             return result;
         }
         public ActionResult GeneratePDFRptMeasureUnit()
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             try
             {
                 DataSet.DataSet1 dt = new DataSet.DataSet1();
                 DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                 dt.EnforceConstraints = false;
                 dt_Com.EnforceConstraints = false;
                 DataSet.DataSet1TableAdapters.spr_tblMeasureUnitSelectTableAdapter MeasureUnit = new DataSet.DataSet1TableAdapters.spr_tblMeasureUnitSelectTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                 
                 var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1,IP, out ErrCommon).FirstOrDefault();
                 MeasureUnit.Fill(dt.spr_tblMeasureUnitSelect,"", "", 0);
                 LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                 GetDate.Fill(dt_Com.spr_GetDate);
                 FastReport.Report Report = new FastReport.Report();
                 Report.RegisterData(dt_Com, "rasaDaramad");
                 Report.RegisterData(dt, "rasaDaramad");
                 var q=dt.spr_tblMeasureUnitSelect.Count();
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\MeasureUnit.frx");
                 FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                 pdf.EmbeddingFonts = true;
                 MemoryStream stream = new MemoryStream();
                 Report.SetParameterValue("UserName", User.fldNameEmployee);
                 Report.Prepare();
                 Report.Export(pdf, stream);
                 return File(stream.ToArray(), "application/pdf");
             }
             catch (Exception x)
             {
                 return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
             }
         }
         public ActionResult CheckRptComboBox()
         {
             DataSet.DataSet1 dt = new DataSet.DataSet1();
             DataSet.DataSet1TableAdapters.spr_tblComboBoxSelectTableAdapter ComboBox = new DataSet.DataSet1TableAdapters.spr_tblComboBoxSelectTableAdapter();
             dt.EnforceConstraints = false;
             ComboBox.Fill(dt.spr_tblComboBoxSelect, "", "", 0);
             var AllowPrint = true;
             var q = dt.spr_tblComboBoxSelect.Count();
             if (q == 0)
                 AllowPrint = false;
             return Json(new
             {
                 AllowPrint = AllowPrint
             }, JsonRequestBehavior.AllowGet);
         }
         public ActionResult PrintComboBox2(string containerId)
         {
             string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\ComboBox.frx";
             ViewBag.Path = path;
             ViewBag.RId ="111";
             return View();
         }
         public ActionResult PrintComboBox(string containerId)
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
             return result;
         }
         public ActionResult GeneratePDFRptComboBox()
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             try
             {
                 DataSet.DataSet1 dt = new DataSet.DataSet1();
                 DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                 dt.EnforceConstraints = false;
                 dt_Com.EnforceConstraints = false;
                 DataSet.DataSet1TableAdapters.spr_tblComboBoxSelectTableAdapter ComboBox = new DataSet.DataSet1TableAdapters.spr_tblComboBoxSelectTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                 var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                 ComboBox.Fill(dt.spr_tblComboBoxSelect, "", "", 0);
                 LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                 GetDate.Fill(dt_Com.spr_GetDate);
                 FastReport.Report Report = new FastReport.Report();
                 Report.RegisterData(dt_Com, "rasaDaramad");
                 Report.RegisterData(dt, "rasaDaramad");
                 
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\ComboBox.frx");
                 FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                 pdf.EmbeddingFonts = true;
                 MemoryStream stream = new MemoryStream();
                 Report.SetParameterValue("Name", User.fldNameEmployee);
                 Report.Prepare();
                 Report.Export(pdf, stream);
                 return File(stream.ToArray(), "application/pdf");
             }
             catch (Exception x)
             {
                 return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
             }
         }

         public ActionResult PrintComboBoxValue(string containerId)
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
             return result;
         }
         public ActionResult GeneratePDFRptComboBoxValue()
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             try
             {
                 var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();

                 DataSet.DataSet1 dt = new DataSet.DataSet1();
                 DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                 DataSet.DataSet1TableAdapters.spr_tblComboBoxValueSelectTableAdapter ComboBoxValue = new DataSet.DataSet1TableAdapters.spr_tblComboBoxValueSelectTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                 dt.EnforceConstraints = false;
                 dt_Com.EnforceConstraints = false;
                 ComboBoxValue.Fill(dt.spr_tblComboBoxValueSelect, "", "","", 0);
                 LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                 GetDate.Fill(dt_Com.spr_GetDate);
                 FastReport.Report Report = new FastReport.Report();
                 Report.RegisterData(dt_Com, "rasaDaramad");
                 Report.RegisterData(dt, "rasaDaramad");
                
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\ComboBoxValue.frx");
                 FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                 pdf.EmbeddingFonts = true;
                 MemoryStream stream = new MemoryStream();
                 Report.SetParameterValue("Parameter", User.fldCodemeli);
                 Report.Prepare();
                 Report.Export(pdf, stream);
                 return File(stream.ToArray(), "application/pdf");
             }
             catch (Exception x)
             {
                 return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
             }
         }
         public ActionResult ReadDaramadParamFormul(StoreRequestParameters parameters, int FiscalYearId, byte State/*,int? OrganId*/)
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

             List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data = null;
             if (filterHeaders.Conditions.Count > 0)
             {
                 string field = "";
                 string searchtext = "";
                 List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data1 = null;
                 foreach (var item in filterHeaders.Conditions)
                 {
                     var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                     if (State == 1)
                     {
                         switch (item.FilterProperty.Name)
                         {
                             case "fldId":
                                 searchtext = ConditionValue.Value.ToString();
                                 field = "fldId";
                                 break;
                             case "fldDaramadTitle":
                                 searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                 field = "LastNode_Organ_Tanzimat_fldDaramadTitle";
                                 break;
                             case "fldDaramadCode":
                                 searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                 field = "LastNode_Organ_Tanzimat_fldDaramadCode";
                                 break;
                             case "fldNameVahed":
                                 searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                 field = "LastNode_fldNameVahed";
                                 break;
                         }
                         if (data != null)
                             data1 = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, (Session["OrganId"]).ToString(), FiscalYearId, 100, IP, out Err).ToList();
                         else
                             data = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, (Session["OrganId"]).ToString(), FiscalYearId, 100, IP, out Err).ToList();
                     }
                     else
                     {
                         switch (item.FilterProperty.Name)
                         {
                             case "fldId":
                                 searchtext = ConditionValue.Value.ToString();
                                 field = "fldId";
                                 break;
                             case "fldDaramadTitle":
                                 searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                 field = "LastNode_fldDaramadTitle";
                                 break;
                             case "fldDaramadCode":
                                 searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                 field = "LastNode_fldDaramadCode";
                                 break;
                             case "fldNameVahed":
                                 searchtext = "%" + ConditionValue.Value.ToString() + "%";
                                 field = "LastNode_fldNameVahed";
                                 break;
                         }
                         if (data != null)
                             data1 = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, (Session["OrganId"]).ToString(), FiscalYearId, 100, IP, out Err).ToList();
                         else
                             data = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, (Session["OrganId"]).ToString(), FiscalYearId, 100, IP, out Err).ToList();
                     }
                 }
                 if (data != null && data1 != null)
                     data.Intersect(data1);
             }
             else
             {
                 if (State == 1)
                 {
                     data = servic.GetShomareHesabCodeDaramadWithFilter("LastNode_Organ_Tanzimat","", (Session["OrganId"]).ToString(), FiscalYearId, 100, IP, out Err).ToList();
                 }
                 else
                 {
                     data = servic.GetShomareHesabCodeDaramadWithFilter("LastNode_Organ", (Session["OrganId"]).ToString(), "", FiscalYearId, 100, IP, out Err).ToList();
                 }
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
                             return !oValue.ToString().Contains(value.ToString());
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

             List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
             //-- end paging ------------------------------------------------------------

             return this.Store(rangeData, data.Count);
         }
         public ActionResult ReadDaramadParamFormulForAsli(StoreRequestParameters parameters/*,int? OrganId*/)
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

             List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data = null;
             if (filterHeaders.Conditions.Count > 0)
             {
                 string field = "";
                 string searchtext = "";
                 List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data1 = null;
                 foreach (var item in filterHeaders.Conditions)
                 {
                     var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                     switch (item.FilterProperty.Name)
                     {
                         case "fldId":
                             searchtext = ConditionValue.Value.ToString();
                             field = "fldId";
                             break;
                     }
                     if (data != null)
                         data1 = servic.GetCodeDarAmadAsliWithFilter( IP, out Err).ToList();
                     else
                         data = servic.GetCodeDarAmadAsliWithFilter(IP, out Err).ToList();
                 }
                 if (data != null && data1 != null)
                     data.Intersect(data1);
             }
             else
             {
                 data = servic.GetCodeDarAmadAsliWithFilter(IP, out Err).ToList();
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
                             return !oValue.ToString().Contains(value.ToString());
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

             List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
             //-- end paging ------------------------------------------------------------

             return this.Store(rangeData, data.Count);
         }
         public ActionResult NodeLoadGroup(string nod, int FiscalYearId)
         {
             if (Session["Username"] == null)
                 return RedirectToAction("Login", "Account", new { area = "" });
             NodeCollection nodes = new Ext.Net.NodeCollection();
             if (nod == "root")
             {
                 nod = "0";
             }
             var q = servic.GetCodhayeDaramdWithFilter("PId", nod,FiscalYearId, 0, 0, IP, out Err).ToList();
             foreach (var item in q)
             {
                 Node asyncNode = new Node();
                 asyncNode.NodeID = item.fldId.ToString();
                 asyncNode.Text = item.fldDaramadTitle;
                 /*asyncNode.Expanded = true;*/
                 asyncNode.AttributesObject = new
                 {
                     fldDaramadCode = item.fldDaramadCode,
                     fldDaramadTitle = item.fldDaramadTitle,
                     fldMashmooleArzesheAfzoode = item.fldMashmooleArzesheAfzoode,
                     fldMashmooleKarmozd = item.fldMashmooleKarmozd,
                     fldNameVahed = item.fldNameVahed,
                     fldUnitId = item.fldUnitId,
                     fldDesc = item.fldDesc
                     //,fldFormolsaz=item.fldFormolsaz,fldFormulKoliId=item.fldFormulKoliId,fldFormulMohasebatId=item.fldFormulMohasebatId
                 };
                 nodes.Add(asyncNode);
             }
             return this.Store(nodes);
         }
         public ActionResult CheckNerkhSalane(string Year)
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             DataSet.DataSet1 dt = new DataSet.DataSet1();
             DataSet.DataSet1TableAdapters.spr_NerkhSalTableAdapter NerkhSal = new DataSet.DataSet1TableAdapters.spr_NerkhSalTableAdapter();
             NerkhSal.Fill(dt.spr_NerkhSal, Year);
             var AllowPrint = true;
             var q = dt.spr_NerkhSal.Count();
             if (q == 0)
                 AllowPrint = false;
             return Json(new
             {
                 AllowPrint = AllowPrint
             }, JsonRequestBehavior.AllowGet);
         }
         public ActionResult GeneratePDFNerkhSalane(string Year)
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             try
             {
                 DataSet.DataSet1 dt = new DataSet.DataSet1();
                 DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                 DataSet.DataSet1TableAdapters.spr_NerkhSalTableAdapter NerkhSal = new DataSet.DataSet1TableAdapters.spr_NerkhSalTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                 dt.EnforceConstraints = false;
                 dt_Com.EnforceConstraints = false;
                 var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();

                 LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                 GetDate.Fill(dt_Com.spr_GetDate);
                 FastReport.Report Report = new FastReport.Report();

                 NerkhSal.Fill(dt.spr_NerkhSal, Year);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptNerkhSalane.frx");

                 Report.RegisterData(dt_Com, "rasaFMSCommon");
                 Report.RegisterData(dt, "rasaFMSDaramad");
                 Report.SetParameterValue("UserName", User.fldNameEmployee);
                 Report.SetParameterValue("Sal", Year);
                 FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                 pdf.EmbeddingFonts = true;
                 MemoryStream stream = new MemoryStream();
                 Report.Prepare();
                 Report.Export(pdf, stream);
                 return File(stream.ToArray(), "application/pdf");
             }
             catch (Exception x)
             {
                 return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
             }
         }
         public FileResult GenerateXlsNerkhSalane(string Year)
         {
             if (Session["Username"] == null)
                 return null;
             try
             {
                 DataSet.DataSet1 dt = new DataSet.DataSet1();
                 DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                 DataSet.DataSet1TableAdapters.spr_NerkhSalTableAdapter NerkhSal = new DataSet.DataSet1TableAdapters.spr_NerkhSalTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                 dt.EnforceConstraints = false;
                 dt_Com.EnforceConstraints = false;
                 var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();

                 LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                 GetDate.Fill(dt_Com.spr_GetDate);
                 FastReport.Report Report = new FastReport.Report();

                 NerkhSal.Fill(dt.spr_NerkhSal, Year);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptNerkhSalane.frx");

                 Report.RegisterData(dt_Com, "rasaFMSCommon");
                 Report.RegisterData(dt, "rasaFMSDaramad");
                 Report.SetParameterValue("UserName", User.fldNameEmployee);
                 Report.SetParameterValue("Sal", Year);

                 FastReport.Export.OoXML.Excel2007Export xlsExport = new FastReport.Export.OoXML.Excel2007Export();


                 MemoryStream stream = new MemoryStream();

                 Report.Prepare();

                 Report.Export(xlsExport, stream);
                 return File(stream.ToArray(), "xls", "NerkhSalane.xls");
             }
             catch (Exception x)
             {
                 return null;
             }
         }
         public ActionResult CheckFishSaderShode(string StartDate, string EndDate,byte TypeDate, string TypeFish, string ShHesabId)
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             DataSet.DataSet1 dt = new DataSet.DataSet1();
             DataSet.DataSet1TableAdapters.spr_RptStatusFishTableAdapter StatusFish = new DataSet.DataSet1TableAdapters.spr_RptStatusFishTableAdapter();
             dt.EnforceConstraints = false;
             var FieldName = "";
             int idShomareHesab = 0;
             if (ShHesabId == "")
                 idShomareHesab = 0;
             else
                 idShomareHesab = Convert.ToInt32(ShHesabId);
             if (TypeFish == "1")
             {
                 if (TypeDate == 1)//تاریخ پرداخت
                 {
                     FieldName = "PardakhtShode";
                 }
                 else//تاریخ واریز
                 {
                     FieldName = "fldDateVarizPardakhtShode";
                 }
                 StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeFish == "11")
             {
                 if (TypeDate == 1)//تاریخ پرداخت
                 {
                     FieldName = "PardakhtShode_Naghdi";
                 }
                 else//تاریخ واریز
                 {
                     FieldName = "fldDateVarizPardakhtShode_Naghdi";
                 }
                 StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeFish == "0")
             {
                 FieldName = "PardakhtNaShode";
                 StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeFish == "01")
             {
                 FieldName = "PardakhtNaShode_Naghdi";
                 StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeFish == "2")
             {
                 FieldName = "EbtalShode";
                 StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeFish == "21")
             {
                 FieldName = "EbtalShode_Naghdi";
                 StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
             }
             var AllowPrint = true;
             var q = dt.spr_RptStatusFish.Count();
             if (q == 0)
                 AllowPrint = false;
             return Json(new
             {
                 AllowPrint = AllowPrint
             }, JsonRequestBehavior.AllowGet);
         }
         public ActionResult GeneratePDFFishSaderShode(string StartDate, string EndDate,byte TypeDate, string TypeFish, string ShHesabId)
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             try
             {
                 DataSet.DataSet1 dt = new DataSet.DataSet1();
                 DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                 DataSet.DataSet1TableAdapters.spr_RptStatusFishTableAdapter StatusFish = new DataSet.DataSet1TableAdapters.spr_RptStatusFishTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                 dt.EnforceConstraints = false;
                 dt_Com.EnforceConstraints = false;
                 string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetFishSaderShode?StartDate=" + StartDate + "&EndDate=" + EndDate + "&TypeFish=" + TypeFish + "&ShHesabId=" + ShHesabId + "&TypeDate=" + TypeDate + "&flag=" + Convert.ToInt32(Session["OrganId"]);

                 var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                 var FieldName = "";
                 int idShomareHesab = 0;
                 if (ShHesabId == "")
                     idShomareHesab = 0;
                 else
                     idShomareHesab = Convert.ToInt32(ShHesabId);
                 LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                 GetDate.Fill(dt_Com.spr_GetDate);
                 FastReport.Report Report = new FastReport.Report();
                 if (TypeFish == "1")
                 {
                    if (TypeDate == 1)//تاریخ پرداخت
                    {
                        FieldName = "PardakhtShode";
                    }
                    else//تاریخ واریز
                    {
                        FieldName = "fldDateVarizPardakhtShode";
                    }
                    StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishPardakhtShode.frx");
                 }
                 if (TypeFish == "11")
                 {
                     if (TypeDate == 1)//تاریخ پرداخت
                     {
                         FieldName = "PardakhtShode_Naghdi";
                     }
                     else//تاریخ واریز
                     {
                         FieldName = "fldDateVarizPardakhtShode_Naghdi";
                     }
                     StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishPardakhtShode.frx");
                 }
                 else if (TypeFish == "0")
                 {
                     FieldName = "PardakhtNaShode";
                     StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishPardakhtNaShode.frx");
                     Report.SetParameterValue("NameReport", "گزارش فیش های صادر شده پرداخت نشده");
                 }
                 else if (TypeFish == "01")
                 {
                     FieldName = "PardakhtNaShode_Naghdi";
                     StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishPardakhtNaShode.frx");
                     Report.SetParameterValue("NameReport", "گزارش فیش های صادر شده پرداخت نشده");
                 }
                 else if (TypeFish == "2")
                 {
                     FieldName = "EbtalShode";
                     StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishEbtalShode.frx");
                     Report.SetParameterValue("NameReport", "گزارش فیش های صادر شده ابطال شده");
                 }
                 else if (TypeFish == "21")
                 {
                     FieldName = "EbtalShode_Naghdi";
                     StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishEbtalShode.frx");
                     Report.SetParameterValue("NameReport", "گزارش فیش های صادر شده ابطال شده");
                 }
                 Report.RegisterData(dt_Com, "rasaFMSCommon");
                 Report.RegisterData(dt, "rasaFMSDaramad");
                 Report.SetParameterValue("UserName", User.fldNameEmployee);
                 Report.SetParameterValue("AzTarikh", StartDate);
                 Report.SetParameterValue("TaTarikh", EndDate);
                 Report.SetParameterValue("barcode", barcode);
                 Report.SetParameterValue("TypeDate", TypeDate);
                 FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                 pdf.EmbeddingFonts = true;
                 MemoryStream stream = new MemoryStream();
                 Report.Prepare();
                 Report.Export(pdf, stream);
                 return File(stream.ToArray(), "application/pdf");
             }
             catch (Exception x)
             {
                 return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
             }
         }
         public FileResult GenerateXlsFishSaderShode(string StartDate, string EndDate, byte TypeDate, string TypeFish, string ShHesabId)
         {
             if (Session["Username"] == null)
                 return null;
             try
             {
                 DataSet.DataSet1 dt = new DataSet.DataSet1();
                 DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                 DataSet.DataSet1TableAdapters.spr_RptStatusFishTableAdapter StatusFish = new DataSet.DataSet1TableAdapters.spr_RptStatusFishTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                 dt.EnforceConstraints = false;
                 dt_Com.EnforceConstraints = false;
                 string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetFishSaderShode?StartDate=" + StartDate + "&EndDate=" + EndDate + "&TypeFish=" + TypeFish + "&ShHesabId=" + ShHesabId + "&TypeDate=" + TypeDate + "&flag=" + Convert.ToInt32(Session["OrganId"]);

                 var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                 var FieldName = "";
                 int idShomareHesab = 0;
                 if (ShHesabId == "")
                     idShomareHesab = 0;
                 else
                     idShomareHesab = Convert.ToInt32(ShHesabId);
                 LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                 GetDate.Fill(dt_Com.spr_GetDate);
                 FastReport.Report Report = new FastReport.Report();
                 if (TypeFish == "1")
                 {
                     if (TypeDate == 1)//تاریخ پرداخت
                     {
                         FieldName = "PardakhtShode";
                     }
                     else//تاریخ واریز
                     {
                         FieldName = "fldDateVarizPardakhtShode";
                     }
                     StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishPardakhtShode.frx");
                 }
                 if (TypeFish == "11")
                 {
                     if (TypeDate == 1)//تاریخ پرداخت
                     {
                         FieldName = "PardakhtShode_Naghdi";
                     }
                     else//تاریخ واریز
                     {
                         FieldName = "fldDateVarizPardakhtShode_Naghdi";
                     }
                     StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishPardakhtShode.frx");
                 }
                 else if (TypeFish == "0")
                 {
                     FieldName = "PardakhtNaShode";
                     StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishPardakhtNaShode.frx");
                     Report.SetParameterValue("NameReport", "گزارش فیش های صادر شده پرداخت نشده");
                 }
                 else if (TypeFish == "01")
                 {
                     FieldName = "PardakhtNaShode_Naghdi";
                     StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishPardakhtNaShode.frx");
                     Report.SetParameterValue("NameReport", "گزارش فیش های صادر شده پرداخت نشده");
                 }
                 else if (TypeFish == "2")
                 {
                     FieldName = "EbtalShode";
                     StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishEbtalShode.frx");
                     Report.SetParameterValue("NameReport", "گزارش فیش های صادر شده ابطال شده");
                 }
                 else if (TypeFish == "21")
                 {
                     FieldName = "EbtalShode_Naghdi";
                     StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishEbtalShode.frx");
                     Report.SetParameterValue("NameReport", "گزارش فیش های صادر شده ابطال شده");
                 }
                 Report.RegisterData(dt_Com, "rasaFMSCommon");
                 Report.RegisterData(dt, "rasaFMSDaramad");
                 Report.SetParameterValue("UserName", User.fldNameEmployee);
                 Report.SetParameterValue("AzTarikh", StartDate);
                 Report.SetParameterValue("TaTarikh", EndDate);
                 Report.SetParameterValue("barcode", barcode);
                 Report.SetParameterValue("TypeDate", TypeDate);

                 FastReport.Export.OoXML.Excel2007Export xlsExport = new FastReport.Export.OoXML.Excel2007Export();


                 MemoryStream stream = new MemoryStream();

                 Report.Prepare();

                 Report.Export(xlsExport, stream);
                 return File(stream.ToArray(), "xls", "FishSaderShode.xls");
             }
             catch (Exception x)
             {
                 return null;
             }
         }
         public ActionResult GetFishSaderShode(string StartDate, string EndDate,byte TypeDate, string TypeFish, string ShHesabId, int flag)
         {
             ViewBag.StartDate = StartDate;
             ViewBag.EndDate = EndDate;
             ViewBag.TypeFish = TypeFish;
             ViewBag.ShHesabId = ShHesabId;
             ViewBag.TypeDate = TypeDate;
             ViewBag.flag = flag;
             return View();
         }
         
         public FileResult DownloadRptFishSaderShode(string StartDate, string EndDate,byte TypeDate, string TypeFish, string ShHesabId, int flag)
         {
             DataSet.DataSet1 dt = new DataSet.DataSet1();
             DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

             DataSet.DataSet1TableAdapters.spr_RptStatusFishTableAdapter StatusFish = new DataSet.DataSet1TableAdapters.spr_RptStatusFishTableAdapter();
             DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
             DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
             dt.EnforceConstraints = false;
             dt_Com.EnforceConstraints = false;
             string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetFishSaderShode?StartDate=" + StartDate + "&EndDate=" + EndDate + "&TypeFish=" + TypeFish + "&ShHesabId=" + ShHesabId + "&TypeDate=" + TypeDate + "&flag=" + flag;
             WCF_Common.OBJ_User User = null;
             if (Session["Username"] != null)
                 User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
             var FieldName = "";
             int idShomareHesab = 0;
             if (ShHesabId == "")
                 idShomareHesab = 0;
             else
                 idShomareHesab = Convert.ToInt32(ShHesabId);
             LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, flag);
             GetDate.Fill(dt_Com.spr_GetDate);
             FastReport.Report Report = new FastReport.Report();
             if (TypeFish == "1")
             {
                 if (TypeDate == 1)//تاریخ پرداخت
                 {
                     FieldName = "PardakhtShode";
                 }
                 else//تاریخ واریز
                 {
                     FieldName = "fldDateVarizPardakhtShode";
                 }
                 StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, flag);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishPardakhtShode.frx");
             }
             else if (TypeFish == "0")
             {
                 FieldName = "PardakhtNaShode";
                 StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, flag);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishPardakhtNaShode.frx");
                 Report.SetParameterValue("NameReport", "گزارش فیش های صادر شده پرداخت نشده");
             }
             else if (TypeFish == "2")
             {
                 FieldName = "EbtalShode";
                 StatusFish.Fill(dt.spr_RptStatusFish, FieldName, StartDate, EndDate, idShomareHesab, flag);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptStatusFishEbtalShode.frx");
                 Report.SetParameterValue("NameReport", "گزارش فیش های صادر شده ابطال شده");
             }
             Report.RegisterData(dt_Com, "rasaFMSCommon");
             Report.RegisterData(dt, "rasaFMSDaramad");
             if (Session["Username"] != null)
                 Report.SetParameterValue("UserName", User.fldNameEmployee);
             else
                 Report.SetParameterValue("UserName", "-");
             Report.SetParameterValue("AzTarikh", StartDate);
             Report.SetParameterValue("TaTarikh", EndDate);
             Report.SetParameterValue("barcode", barcode);
             Report.SetParameterValue("TypeDate", TypeDate);
             FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
             pdf.EmbeddingFonts = true;
             MemoryStream stream = new MemoryStream();
             Report.Prepare();
             Report.Export(pdf, stream);
             //  return File(stream.ToArray(), "application/pdf");

             return File(stream.ToArray(), "application/pdf", "PrintForm.pdf");
             //else
             //    return null;

         }
         public ActionResult CheckAsnadDaryafti(string StartDate, string EndDate, string TypeSand, string Status, string StatusName)
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             DataSet.DataSet1 dt = new DataSet.DataSet1();
             dt.EnforceConstraints = false;
             DataSet.DataSet1TableAdapters.spr_RptAsnadTableAdapter Asnad = new DataSet.DataSet1TableAdapters.spr_RptAsnadTableAdapter();
             DataSet.DataSet1TableAdapters.spr_RptStatusCeckTableAdapter allAsnad = new DataSet.DataSet1TableAdapters.spr_RptStatusCeckTableAdapter();
             dt.EnforceConstraints = false;
             if (TypeSand == "2" && Status == "1")
             {
                 Asnad.Fill(dt.spr_RptAsnad, "Check", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeSand == "2" && Status == "6")
             {
                 Asnad.Fill(dt.spr_RptAsnad, "Check_Date", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeSand == "2" && Status == "7")
             {
                 allAsnad.Fill(dt.spr_RptStatusCeck, StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeSand == "2")
             {
                 Asnad.Fill(dt.spr_RptAsnad, "Check", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeSand == "3" && Status == "6")
             {
                 Asnad.Fill(dt.spr_RptAsnad, "Safte_Date", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeSand == "3")
             {
                 Asnad.Fill(dt.spr_RptAsnad, "Safte", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeSand == "4" && Status == "6")
             {
                 Asnad.Fill(dt.spr_RptAsnad, "Barat_Date", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
             }
             else if (TypeSand == "4")
             {
                 Asnad.Fill(dt.spr_RptAsnad, "Barat", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
             }
             var q = dt.spr_RptAsnad.Count();
             if (Status == "7")
                 q = dt.spr_RptStatusCeck.Count();
             var AllowPrint = true;
             if (q == 0)
                 AllowPrint = false;
             return Json(new
             {
                 AllowPrint = AllowPrint
             }, JsonRequestBehavior.AllowGet);

         }
         public ActionResult GeneratePDFAsnadDaryafti(string StartDate, string EndDate, string TypeSand, string Status,string StatusName)
         {
             if (Session["Username"] == null)
                 return RedirectToAction("logon", "Account", new { area = "" });
             try
             {
                 string NameReport = "";
                 DataSet.DataSet1 dt = new DataSet.DataSet1();
                 DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                 DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                 DataSet.DataSet1TableAdapters.spr_RptAsnadTableAdapter Asnad = new DataSet.DataSet1TableAdapters.spr_RptAsnadTableAdapter();
                 DataSet.DataSet1TableAdapters.spr_RptStatusCeckTableAdapter allCheck = new DataSet.DataSet1TableAdapters.spr_RptStatusCeckTableAdapter();

                 dt.EnforceConstraints = false;
                 dt_Com.EnforceConstraints = false;
                 var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                 dt.EnforceConstraints = false;
                 LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                 GetDate.Fill(dt_Com.spr_GetDate);
                 FastReport.Report Report = new FastReport.Report();
                 string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetAsnadDaryafti?StartDate=" + StartDate + "&EndDate=" + EndDate + "&TypeSand=" + TypeSand + "&Status=" + Status + "&StatusName=" + StatusName + "&flag=" + Convert.ToInt32(Session["OrganId"]);
                 if (TypeSand == "2" && Status == "1")
                 {
                     NameReport = "گزارش چک های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Check", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(check_DarentezarVosol).frx");

                 }
                 else if (TypeSand == "2" && Status == "6")
                 {
                     NameReport = "گزارش چک های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Check_Date", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(check).frx");

                 }                 
                 else if (TypeSand == "2" && Status == "7")
                 {
                     NameReport = "گزارش " + StatusName;
                     allCheck.Fill(dt.spr_RptStatusCeck, StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAllAsnad.frx");

                 }
                 else if (TypeSand == "2")
                 {
                     NameReport = "گزارش چک های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Check", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(check).frx");

                 }
                 else if (TypeSand == "3" && Status == "6")
                 {
                     NameReport = "گزارش سفته های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Safte_Date", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Safte).frx");
                 }
                 else if (TypeSand == "3")
                 {
                     NameReport = "گزارش سفته های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Safte", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Safte).frx");
                 }
                 else if (TypeSand == "4" && Status == "6")
                 {
                     NameReport = "گزارش برات های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Barat_Date", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Barat).frx");
                 }
                 else if (TypeSand == "4")
                 {
                     NameReport = "گزارش برات های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Barat", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Barat).frx");
                 }
                 
                 Report.RegisterData(dt_Com, "rasaFMSCommon");
                 Report.RegisterData(dt, "rasaFMSDaramad");
                 Report.SetParameterValue("UserName", User.fldNameEmployee);
                 Report.SetParameterValue("AzTarikh", StartDate);
                 Report.SetParameterValue("TaTarikh", EndDate);
                 Report.SetParameterValue("NameReport", NameReport);
                 Report.SetParameterValue("barcode", barcode);
                 

                 FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                 pdf.EmbeddingFonts = true;
                 MemoryStream stream = new MemoryStream();
                 Report.Prepare();
                 Report.Export(pdf, stream);
                 return File(stream.ToArray(), "application/pdf");
             }
             catch (Exception x)
             {
                 return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
             }
         }
         public FileResult GenerateXlsAsnadDaryafti(string StartDate, string EndDate, string TypeSand, string Status, string StatusName)
         {
             if (Session["Username"] == null)
                 return null;
             try
             {
                 string NameReport = "";
                 DataSet.DataSet1 dt = new DataSet.DataSet1();
                 DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                 DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                 DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                 DataSet.DataSet1TableAdapters.spr_RptAsnadTableAdapter Asnad = new DataSet.DataSet1TableAdapters.spr_RptAsnadTableAdapter();
                 DataSet.DataSet1TableAdapters.spr_RptStatusCeckTableAdapter allCheck = new DataSet.DataSet1TableAdapters.spr_RptStatusCeckTableAdapter();

                 dt.EnforceConstraints = false;
                 dt_Com.EnforceConstraints = false;
                 var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                 dt.EnforceConstraints = false;
                 LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                 GetDate.Fill(dt_Com.spr_GetDate);
                 FastReport.Report Report = new FastReport.Report();
                 string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetAsnadDaryafti?StartDate=" + StartDate + "&EndDate=" + EndDate + "&TypeSand=" + TypeSand + "&Status=" + Status + "&StatusName=" + StatusName + "&flag=" + Convert.ToInt32(Session["OrganId"]);
                 if (TypeSand == "2" && Status == "1")
                 {
                     NameReport = "گزارش چک های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Check", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(check_DarentezarVosol)2.frx");

                 }
                 else if (TypeSand == "2" && Status == "6")
                 {
                     NameReport = "گزارش چک های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Check_Date", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(check)2.frx");

                 }
                 else if (TypeSand == "2" && Status == "7")
                 {
                     NameReport = "گزارش " + StatusName;
                     allCheck.Fill(dt.spr_RptStatusCeck, StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAllAsnad.frx");

                 }
                 else if (TypeSand == "2")
                 {
                     NameReport = "گزارش چک های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Check", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(check)2.frx");

                 }
                 else if (TypeSand == "3" && Status == "6")
                 {
                     NameReport = "گزارش سفته های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Safte_Date", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Safte)2.frx");
                 }
                 else if (TypeSand == "3")
                 {
                     NameReport = "گزارش سفته های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Safte", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Safte)2.frx");
                 }
                 else if (TypeSand == "4" && Status == "6")
                 {
                     NameReport = "گزارش برات های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Barat_Date", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Barat)2.frx");
                 }
                 else if (TypeSand == "4")
                 {
                     NameReport = "گزارش برات های " + StatusName;
                     Asnad.Fill(dt.spr_RptAsnad, "Barat", Convert.ToByte(Status), StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Barat)2.frx");
                 }

                 Report.RegisterData(dt_Com, "rasaFMSCommon");
                 Report.RegisterData(dt, "rasaFMSDaramad");
                 Report.SetParameterValue("UserName", User.fldNameEmployee);
                 Report.SetParameterValue("AzTarikh", StartDate);
                 Report.SetParameterValue("TaTarikh", EndDate);
                 Report.SetParameterValue("NameReport", NameReport);
                 Report.SetParameterValue("barcode", barcode);

                 FastReport.Export.OoXML.Excel2007Export xlsExport = new FastReport.Export.OoXML.Excel2007Export();


                 MemoryStream stream = new MemoryStream();
                
                 Report.Prepare();

                 Report.Export(xlsExport, stream);
                 return File(stream.ToArray(), "xls", "AsnadDaryaftani.xls");
             }
             catch (Exception x)
             {
                 return null;
             }
         }
         public ActionResult GetAsnadDaryafti(string StartDate, string EndDate, string TypeSand, string Status, string StatusName,int flag)
         {
             ViewBag.StartDate = StartDate;
             ViewBag.EndDate = EndDate;
             ViewBag.TypeSand = TypeSand;
             ViewBag.Status = Status;
             ViewBag.StatusName = StatusName;
             ViewBag.flag = flag;
             return View();
         }
         public FileResult DownloadRptAsnadDaryafti(string StartDate, string EndDate, string TypeSand, string Status, string StatusName, int flag)
         {
             string NameReport = "";
             DataSet.DataSet1 dt = new DataSet.DataSet1();
             DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
             DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
             DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
             DataSet.DataSet1TableAdapters.spr_RptAsnadTableAdapter Asnad = new DataSet.DataSet1TableAdapters.spr_RptAsnadTableAdapter();
             dt.EnforceConstraints = false;
             dt_Com.EnforceConstraints = false;
             WCF_Common.OBJ_User User = null;
             if (Session["Username"] != null)
                 User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
             
            
             LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, flag);
             GetDate.Fill(dt_Com.spr_GetDate);
             FastReport.Report Report = new FastReport.Report();
             string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/Report/GetAsnadDaryafti?StartDate=" + StartDate + "&EndDate=" + EndDate + "&TypeSand=" + TypeSand + "&Status=" + Status + "&StatusName=" + StatusName + "&flag=" + flag;

             if (TypeSand == "2" && Status == "1")
             {
                 NameReport = "گزارش چک های " + StatusName;
                 Asnad.Fill(dt.spr_RptAsnad, "Check", Convert.ToByte(Status), StartDate, EndDate, flag);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(check_DarentezarVosol).frx");

             }
             else if (TypeSand == "2" && Status == "6")
             {
                 NameReport = "گزارش چک های " + StatusName;
                 Asnad.Fill(dt.spr_RptAsnad, "Check_Date", Convert.ToByte(Status), StartDate, EndDate, flag);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(check).frx");

             }
             else if (TypeSand == "2")
             {
                 NameReport = "گزارش چک های " + StatusName;
                 Asnad.Fill(dt.spr_RptAsnad, "Check", Convert.ToByte(Status), StartDate, EndDate, flag);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(check).frx");

             }
             else if (TypeSand == "3" && Status == "6")
             {
                 NameReport = "گزارش سفته های " + StatusName;
                 Asnad.Fill(dt.spr_RptAsnad, "Safte_Date", Convert.ToByte(Status), StartDate, EndDate, flag);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Safte).frx");
             }
             else if (TypeSand == "3")
             {
                 NameReport = "گزارش سفته های " + StatusName;
                 Asnad.Fill(dt.spr_RptAsnad, "Safte", Convert.ToByte(Status), StartDate, EndDate, flag);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Safte).frx");
             }
             else if (TypeSand == "4" && Status == "6")
             {
                 NameReport = "گزارش برات های " + StatusName;
                 Asnad.Fill(dt.spr_RptAsnad, "Barat_Date", Convert.ToByte(Status), StartDate, EndDate, flag);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Barat).frx");
             }
             else if (TypeSand == "4")
             {
                 NameReport = "گزارش برات های " + StatusName;
                 Asnad.Fill(dt.spr_RptAsnad, "Barat", Convert.ToByte(Status), StartDate, EndDate, flag);
                 Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAsnad(Barat).frx");
             }

             Report.RegisterData(dt_Com, "rasaFMSCommon");
             Report.RegisterData(dt, "rasaFMSDaramad");
             if (Session["Username"] != null)
                 Report.SetParameterValue("UserName", User.fldNameEmployee);
             else
                 Report.SetParameterValue("UserName", "-");
             Report.SetParameterValue("AzTarikh", StartDate);
             Report.SetParameterValue("TaTarikh", EndDate);
             Report.SetParameterValue("NameReport", NameReport);
             Report.SetParameterValue("barcode", barcode);

             FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
             pdf.EmbeddingFonts = true;
             MemoryStream stream = new MemoryStream();
             Report.Prepare();
             Report.Export(pdf, stream);
             //  return File(stream.ToArray(), "application/pdf");

             return File(stream.ToArray(), "application/pdf", "PrintForm.pdf");
             //else
             //    return null;

         }
    }
}
