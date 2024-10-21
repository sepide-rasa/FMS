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
    public class RptCodingStatusController : Controller
    {
        //
        // GET: /Accounting/RptCodingStatus/

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService(); 
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();

        public ActionResult Index(string containerId,short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.CodingStatus(); ;
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            return result;
        }
        public ActionResult MoghayeratRemain(string containerId, short Year, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            var LastDay = MyLib.Shamsi.LastDayOfShamsiYear(Year);
            var MinDate = MyLib.Shamsi.Shamsi2miladiDateTime(Year.ToString() + "/01/01").ToString();
            var MaxDate = MyLib.Shamsi.Shamsi2miladiDateTime(Year.ToString() + "/12/" + LastDay.ToString().PadLeft(2, '0')).ToString();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            result.ViewBag.FiscalYearId = FiscalYearId;
            result.ViewBag.MinDate = MinDate;
            result.ViewBag.MaxDate = MaxDate;
            return result;
        }
        public ActionResult PdfRptMoghayerat(short Year, int FiscalYearId, string AzTarikh, string TaTarikh)
        {
            try
            {
                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\Rptmoghayerat.frx";
                rep.Load(path);
                rep.SetParameterValue("organId", Convert.ToInt32(Session["OrganId"]));
                rep.SetParameterValue("UserName", Session["Username"]);
                rep.SetParameterValue("FiscalYearId", FiscalYearId);
                rep.SetParameterValue("tatarikh", TaTarikh);
                rep.SetParameterValue("aztarikh", AzTarikh);
                rep.SetParameterValue("sal", Year);
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
        public ActionResult ShowRptTaraz(string containerId, short Year, int SourceId, int CaseTypeId, byte flag, int CodingDetail, byte TypeSanad)
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
            result.ViewBag.Year = Year;
            result.ViewBag.SourceId = SourceId;
            result.ViewBag.CaseTypeId = CaseTypeId;
            result.ViewBag.flag = flag;
            result.ViewBag.CodingDetail = CodingDetail;
            result.ViewBag.TypeSanad = TypeSanad;
            return result;
        }
        public ActionResult GetCaseType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCaseTypeWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetCoding_Header()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCoding_HeaderWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldYear });
            return this.Store(q);
        }
        public ActionResult LoadTreeCoding(string nod, short Year,string FilterValue)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var CodingHeader = servic.GetCoding_HeaderWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
            var q = servic.GetCoding_DetailsWithFilter("fldPID", nod, CodingHeader.fldId.ToString(), "%" + FilterValue + "%", 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                var LevelName = servic.GetAccountingLevelDetail(item.fldAccountLevelId, Convert.ToInt32(Session["OrganId"]), IP, out Err).fldName;
                switch (LevelName)
                {
                    case "گروه":
                        asyncNode.IconCls = "GroupIco";
                        break;
                    case "کل":
                        asyncNode.IconCls = "KolIco";
                        break;
                    case "معین":
                        asyncNode.IconCls = "MoeinIco";
                        break;
                    default:
                        asyncNode.IconCls = "TafsiliIco";
                        break;
                }
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text =item.fldCode+"-"+ item.fldTitle;
                asyncNode.Expanded= false;
                asyncNode.AttributesObject = new
                {
                    fldCode = item.fldCode,
                    fldTempCodingId = item.fldTempCodingId.ToString(),
                    fldTitle = item.fldCode + "-" + item.fldTitle,
                    fldMahiyatId = item.fldMahiyatId,
                    fldTempNameId = item.fldTempNameId.ToString(),
                    fldAccountingTypeId = item.fldAccountingTypeId.ToString(),
                    fldAccountLevelId = item.fldAccountLevelId,
                    fldName_AccountingLevel = item.fldName_AccountingLevel,
                    fldPCod = item.fldPCod,
                    fldDesc = item.fldDesc
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }
        public ActionResult ReadRemain(int CodingDetail, byte flag, int CaseTypeId, int SourceId, short Year,byte TypeSanad,byte Type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Accounting.OBJ_RptCodingStatus> data = null;
            if (Type == 1)
            {
                data = servic.RptCodingStatus(CodingDetail, flag, CaseTypeId, SourceId, Year, Convert.ToInt32(Session["OrganId"]), 4, TypeSanad, IP, out Err).ToList();
            }
            else
            {
                data = servic.RptCodingStatus_Parvande(CodingDetail, flag, CaseTypeId, SourceId, Year, Convert.ToInt32(Session["OrganId"]), 4, TypeSanad, IP, out Err).ToList();
            }
            return this.Store(data);
        }
        public ActionResult WinCodingTurnover(int CodingDetail,int CaseTypeId, int SourceId,short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.CodingDetail = CodingDetail;
            PartialView.ViewBag.CaseTypeId = CaseTypeId;
            PartialView.ViewBag.SourceId = SourceId;
            PartialView.ViewBag.Year = Year;
            return PartialView;
        }

        public ActionResult ReadCodingTurnover(StoreRequestParameters parameters, int CodingDetail, int CaseTypeId, int SourceId,string CodingTitle,short Year, string AzTarikh, string TaTArikh)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Accounting.OBJ_RptCodingTurnover> data = null;

            if (AzTarikh != null && AzTarikh != "" && TaTArikh != null && TaTArikh != "")
            {
                data = servic.RptCodingTurnover_Tarikh(CodingDetail, CaseTypeId, SourceId, Year, Convert.ToInt32(Session["OrganId"]), 4, IP,AzTarikh,TaTArikh, out Err).ToList();
            }
            else
            {
                data = servic.RptCodingTurnover(CodingDetail, CaseTypeId, SourceId, Year, Convert.ToInt32(Session["OrganId"]), 4, IP, out Err).ToList();
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

            List<WCF_Accounting.OBJ_RptCodingTurnover> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult PrintRptCodingStatus(int CodingDetail, byte flag, int CaseTypeId, int SourceId, byte TypeSanad, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.CodingDetail = CodingDetail;
            PartialView.ViewBag.flag = flag;
            PartialView.ViewBag.CaseTypeId = CaseTypeId;
            PartialView.ViewBag.SourceId = SourceId;
            PartialView.ViewBag.TypeSanad = TypeSanad;
            PartialView.ViewBag.Year = Year;
            return PartialView;
        }
        public ActionResult PrintRptCoding(int CodingDetail, int CaseTypeId, int SourceId, short Year, string AzTarikh, string TaTArikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.CodingDetail = CodingDetail;
            PartialView.ViewBag.CaseTypeId = CaseTypeId;
            PartialView.ViewBag.SourceId = SourceId;
            PartialView.ViewBag.AzTarikh = AzTarikh;
            PartialView.ViewBag.TaTArikh = TaTArikh;
            PartialView.ViewBag.Year = Year;
            return PartialView;
        }
        public ActionResult GeneratePdfRptCoding(int CodingDetail, int CaseTypeId, int SourceId, short Year,string AzTarikh, string TaTArikh)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
                WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                DataSet.DataSet1TableAdapters.spr_RptCodingTurnover_TarikhTableAdapter RptCoding = new DataSet.DataSet1TableAdapters.spr_RptCodingTurnover_TarikhTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptCodingTurnoverTableAdapter RptCoding2 = new DataSet.DataSet1TableAdapters.spr_RptCodingTurnoverTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "dataSetAccounting");
                Report.RegisterData(dt, "dataSetAccounting");
                var CodingTitle = servic.GetCoding_DetailsDetail(CodingDetail, IP, out Err).fldTitle;
                if (AzTarikh != null && AzTarikh != "" && TaTArikh != null && TaTArikh != "")
                {
                    RptCoding.Fill(dt.spr_RptCodingTurnover_Tarikh, CodingDetail, CaseTypeId, SourceId, Year, Convert.ToInt32(Session["OrganId"]), 4, AzTarikh, TaTArikh);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptCodingTurnover_Tarikh.frx");
                }
                else
                {
                    RptCoding2.Fill(dt.spr_RptCodingTurnover, CodingDetail, CaseTypeId, SourceId, Year, Convert.ToInt32(Session["OrganId"]), 4);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptCodingTurnover.frx");
                } 

                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.SetParameterValue("CodingTitle", CodingTitle);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("AzTarikh", AzTarikh);
                Report.SetParameterValue("TaTarikh", TaTArikh);
                Report.SetParameterValue("Sal", Year);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult CreateExcelRptCoding(int CodingDetail, int CaseTypeId, int SourceId, short Year, string AzTarikh, string TaTArikh)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
            int index = 0; var fldDocumentNum = ""; var fldTarikhDocument = ""; var fldDescriptionDocu = ""; var fldParvande = ""; var fldBedehkar_t = "0"; var fldBestankar_t = "0"; var fldMande_t = "0"; var Check = "";
            var StatusCheck="fldDocumentNum;fldTarikhDocument;fldDescriptionDocu;fldParvande;fldBedehkar_t;fldBestankar_t;fldMande_t".Split(';');
            List<WCF_Accounting.OBJ_RptCodingTurnover> data = null;
            if (AzTarikh != null && AzTarikh != "" && TaTArikh != null && TaTArikh != "")
            {
                data = servic.RptCodingTurnover_Tarikh(CodingDetail, CaseTypeId, SourceId, Year, Convert.ToInt32(Session["OrganId"]), 4, IP, AzTarikh, TaTArikh, out Err).ToList();
            }
            else
            {
                data = servic.RptCodingTurnover(CodingDetail, CaseTypeId, SourceId, Year, Convert.ToInt32(Session["OrganId"]), 4, IP, out Err).ToList();
            }  
            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];

            foreach (var item in StatusCheck)
            {
                switch (item)
                {

                    case "fldDocumentNum":
                        Check = "شماره سند";
                        Cell cell = sheet.Cells[alpha[index] + "1"];
                        cell.PutValue(Check);
                        int i = 0;
                        foreach (var _item in data)
                        {
                            fldDocumentNum = _item.fldDocumentNum.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                            Cell.PutValue(fldDocumentNum);
                            i++;
                        }
                        index++;
                        break;
                    case "fldDescriptionDocu":
                        Check = "شرح سند";
                        Cell cell1 = sheet.Cells[alpha[index] + "1"];
                        cell1.PutValue(Check);
                        int i1 = 0;
                        foreach (var _item in data)
                        {
                            fldDescriptionDocu = _item.fldDescriptionDocu;
                            Cell Cell = sheet.Cells[alpha[index] + (i1 + 2)];
                            Cell.PutValue(fldDescriptionDocu);
                            i1++;
                        }
                        index++;
                        break;
                    case "fldParvande":
                        Check = "پرونده";
                        Cell cell2 = sheet.Cells[alpha[index] + "1"];
                        cell2.PutValue(Check);
                        int i2 = 0;
                        foreach (var _item in data)
                        {
                            fldParvande = _item.fldParvande;
                            Cell Cell = sheet.Cells[alpha[index] + (i2 + 2)];
                            Cell.PutValue(fldParvande);
                            i2++;
                        }
                        index++;
                        break;
                    case "fldBedehkar_t":
                        Check = "بدهکار";
                        Cell cell3 = sheet.Cells[alpha[index] + "1"];
                        cell3.PutValue(Check);
                        int i3 = 0;
                        foreach (var _item in data)
                        {
                            fldBedehkar_t = _item.fldBedehkar_t.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i3 + 2)];
                            Cell.PutValue(fldBedehkar_t);
                            i3++;
                        }
                        index++;
                        break;
                    case "fldBestankar_t":
                        Check = "بستانکار";
                        Cell cell4 = sheet.Cells[alpha[index] + "1"];
                        cell4.PutValue(Check);
                        int i4 = 0;
                        foreach (var _item in data)
                        {
                            fldBestankar_t = _item.fldBestankar_t.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i4 + 2)];
                            Cell.PutValue(fldBestankar_t);
                            i4++;
                        }
                        index++;
                        break;
                    case "fldMande_t":
                        Check = "مانده";
                        Cell cell5 = sheet.Cells[alpha[index] + "1"];
                        cell5.PutValue(Check);
                        int i5 = 0;
                        foreach (var _item in data)
                        {
                            fldMande_t = _item.fldMande_t.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i5 + 2)];
                            Cell.PutValue(fldMande_t);
                            i5++;
                        }
                        index++;
                        break;
                    case "fldTarikhDocument":
                        Check = "تاریخ";
                        Cell cell6 = sheet.Cells[alpha[index] + "1"];
                        cell6.PutValue(Check);
                        int i6 = 0;
                        foreach (var _item in data)
                        {
                            fldTarikhDocument = _item.fldTarikhDocument.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i6 + 2)];
                            Cell.PutValue(fldTarikhDocument);
                            i6++;
                        }
                        index++;
                        break;
                }
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            return File(stream.ToArray(), "xls", "File.xls");
        }
        public ActionResult GeneratePdfRptCodingStatus(int CodingDetail, byte flag, int CaseTypeId, int SourceId, byte TypeSanad, short Year)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {

                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
                WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                DataSet.DataSet1TableAdapters.spr_RptCodingStatusTableAdapter CodingStatus = new DataSet.DataSet1TableAdapters.spr_RptCodingStatusTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                CodingStatus.Fill(dt.spr_RptCodingStatus, CodingDetail, flag, CaseTypeId, SourceId,Year,Convert.ToInt32(Session["OrganId"]),4, TypeSanad);
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "dataSetAccounting");
                Report.RegisterData(dt, "dataSetAccounting");

                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptCodingStatus.frx");
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
    }
}
