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

namespace NewFMS.Areas.Personeli.Controllers
{
    public class ReportsController : Controller
    {
        //
        // GET: /Personeli/Reports/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();

        WCF_Personeli.PersoneliService PersoneliService = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError ErrPersoneli = new WCF_Personeli.ClsError();

        WCF_Common_Pay.Comon_PayService Comon_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError ErrC_P = new WCF_Common_Pay.ClsError();
        public ActionResult Index(string containerId)
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

        public ActionResult AmarKarkonan(string containerId)
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

        public ActionResult Gheybat(string containerId, byte State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.State = State;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult RptListKhamKarkard(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.CostCenter = NewFMS.Models.CurrentDate.CostCenter;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult GetCostCenter()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetCostCenterWithFilter("", "", Session["OrganId"].ToString(), 0, IP, out ErrPay).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GeneratePDFListKhamKarkard(short Year, byte Month, int costCenter_ChartId, string costCenterName, string type, string typeList, byte state)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var FieldName = "";
                if (type == "1" && typeList=="1")
                    FieldName = "CostCenter";
                else if (type == "2" && typeList == "1")
                    FieldName = "ChartOrgan";
                else if (type == "1" && typeList == "2")
                    FieldName = "CostCenter_Info";
                else if (type == "2" && typeList == "2")
                    FieldName = "ChartOrgan_Info";
                else if (type == "0" && typeList == "2")
                    FieldName = "Info";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_pay_RptListKhamKarkardTableAdapter ListKhamKarkard = new DataSet.DataSet1TableAdapters.spr_pay_RptListKhamKarkardTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                ListKhamKarkard.Fill(dt.spr_pay_RptListKhamKarkard,FieldName, Year, Month, costCenter_ChartId, Convert.ToInt32(Session["OrganId"]));

                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                FastReport.Report Report = new FastReport.Report();
                if (typeList == "1")
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListKham1.frx");
                else
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListKham2.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Markaz", costCenterName);

                if (state == 1)
                {
                    FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();                    
                    pdf.EmbeddingFonts = true;
                    MemoryStream stream = new MemoryStream();
                    Report.Prepare();
                    Report.Export(pdf, stream);
                    return File(stream.ToArray(), "application/pdf");
                }
                else
                {
                    FastReport.Export.Xml.XMLExport excel = new FastReport.Export.Xml.XMLExport();
                    MemoryStream stream = new MemoryStream();
                    Report.Prepare();
                    Report.Export(excel, stream);
                    return File(stream.ToArray(), "application/vnd.ms-excel", "RptListKham.xls");
                }
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GeneratePDFStaffStatistics(string Modir, string Girande, string Sh_Nameh, string Tarikh, string Sal)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptAmarKarkonan.frx");
                Report.SetParameterValue("modir", Modir);
                Report.SetParameterValue("onvan", Girande);
                Report.SetParameterValue("sh_name", Sh_Nameh);
                Report.SetParameterValue("tarikh", Tarikh);
                Report.SetParameterValue("Sal", Sal);
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

        public ActionResult GeneratePDFCoveredPeople()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_RptAfradTahtePoosheshTableAdapter AfradTahtePooshesh = new DataSet.DataSet1TableAdapters.spr_RptAfradTahtePoosheshTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1,IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                AfradTahtePooshesh.Fill(dt.spr_RptAfradTahtePooshesh, Convert.ToInt32(Session["OrganId"]));
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptAfradTahtPoshesh.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("UserName", User.fldNameEmployee);
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

        public ActionResult GeneratePDFAbsenteeismList(short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptListGheybat_KarkardTableAdapter ListGheybat = new DataSet.DataSet1TableAdapters.spr_Pay_RptListGheybat_KarkardTableAdapter();
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                ListGheybat.Fill(dt.spr_Pay_RptListGheybat_Karkard, "GHeybat", Year, Convert.ToInt32(Session["OrganId"]));
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListGheibatHaySal.frx");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Sal", Year.ToString());
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

        public ActionResult GeneratePDFFunctionList(short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptListGheybat_KarkardTableAdapter ListGheybat = new DataSet.DataSet1TableAdapters.spr_Pay_RptListGheybat_KarkardTableAdapter();
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                ListGheybat.Fill(dt.spr_Pay_RptListGheybat_Karkard, "Karkard", Year, Convert.ToInt32(Session["OrganId"]));
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListKarkardSal.frx");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Sal", Year.ToString());
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

        public ActionResult GeneratePDFCompeletedFunctionList(short Year,byte Month,byte Group)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var fieldName = "CostCenter";
                if (Group == 2)
                    fieldName = "ChartOrgan";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptListTakmilShodeKarkardTableAdapter ListTakmilShode = new DataSet.DataSet1TableAdapters.spr_Pay_RptListTakmilShodeKarkardTableAdapter();
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1,IP, out Err).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                ListTakmilShode.Fill(dt.spr_Pay_RptListTakmilShodeKarkard, fieldName, Year, Month, Convert.ToInt32(Session["OrganId"]));
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListTakmilShodeKarkard.frx");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Sal", Year.ToString());
                Report.SetParameterValue("Mah", MonthName);
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

        public ActionResult GetNoeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Comon_Pay.GetAnvaEstekhdamWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }

        public ActionResult GeneratePDFPersonalList(int Value)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_PersonalInfoTableAdapter PersonalInfo = new DataSet.DataSet1TableAdapters.spr_PersonalInfoTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                PersonalInfo.Fill(dt.spr_PersonalInfo, Value.ToString(), Convert.ToInt32(Session["OrganId"]));
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptPersonal.frx");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                //Report.SetParameterValue("Sal", Year.ToString());
                //Report.SetParameterValue("Mah", MonthName);
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
    }
}
