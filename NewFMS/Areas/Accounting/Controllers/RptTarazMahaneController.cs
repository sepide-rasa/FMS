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
namespace NewFMS.Areas.Accounting.Controllers
{
    public class RptTarazMahaneController : Controller
    {
        //
        // GET: /Accounting/RptTarazMahane/

        WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService(); 
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();


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
            result.ViewBag.CurrentMonth = CurrentMonth.PadLeft('0',2);
            
            //result.ViewBag.CurrentYear = Convert.ToInt32(servic_Common.GetDate(IP, out ErrCommon).fldTarikh.Substring(0, 4)).ToString();
            if (Year == Convert.ToInt16(servic_Common.GetDate(IP, out ErrCommon).fldTarikh.Substring(0, 4))){
                result.ViewBag.CurrentDay = servic_Common.GetDate(IP, out ErrCommon).fldTarikh;
            }
            else{
                result.ViewBag.CurrentDay = Year + "/12/" + MyLib.Shamsi.LastDayOfShamsiYear(Year);
            }

            return result;
        }
        public ActionResult GetFiles_Coding(short Year, int CodingId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDocFiles(Year, Convert.ToInt32(Session["OrganId"]), CodingId, IP, out Err).OrderBy(l => l.fldName).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName, fldCaseTypeId=c.fldCaseTypeId });
            return this.Store(q);
        }
        public ActionResult GetHeadLines(short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCoding_DetailsWithFilter("Child", Year.ToString(), Session["OrganId"].ToString(), "", 0, IP, out Err).OrderBy(l => l.fldTitle).ToList().Select(c => new { fldId = c.fldId, fldName = "(" + c.fldCode + ")" + c.fldTitle, fldMahiyatId = c.fldMahiyatId });
            return this.Store(q);
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

            var q = servic.GetDocumentRecord_HeaderWithFilter(FieldName, "",Az,Ta,Convert.ToInt32(Session["OrganId"]),  Year,4, 0, IP, out Err).OrderBy(l=>l.fldDocumentNum).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldDocumentNum});
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
            var q = servic.GetDocumentRecord_HeaderWithFilter(FieldName, AzSanad, Az, Ta, Convert.ToInt32(Session["OrganId"]), Year, 4, 0, IP, out Err).OrderByDescending(l => l.fldDocumentNum).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldDocumentNum });
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
        public ActionResult LoadTreeCoding(string nod, short Year, string FilterValue)
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
                asyncNode.Text = item.fldCode + "-" + item.fldTitle;
                asyncNode.Expanded = false;
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
        //public FileResult CreateExcel()
        //{
        //    string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
        //    int index = 0;
            
        //    var Check = "";

        //    var fldName = ""; var fldId = ""; var fldMelli_EconomicCode = ""; var fldKarkard = ""; var tabaee1 = "";
        //    var tabaee2 = ""; var tabaee3 = ""; var fldMoavaghe = "";
        //    var fldDateEjra = ""; var fldHagheDarman = ""; var fldMashmool = ""; var fldShenase = "";

        //    var data = servic.GetDocumentTypeWithFilter("", "", Convert.ToInt32(Session["OrganId"]),0, IP, out Err).ToList();


        //    Workbook wb = new Workbook();
        //    var Checked="fldName;fldId";
        //    var StatusCheck = Checked.Split(';');
        //    Worksheet sheet = wb.Worksheets[0];

        //    foreach (var item in StatusCheck)
        //    {
        //        switch (item)
        //        {
        //            case "fldId":
        //                Check = "Id";
        //                Cell cell = sheet.Cells[alpha[index] + "1"];
        //                cell.PutValue(Check);
        //                int k3 = 0;
        //                foreach (var _item in data)
        //                {
        //                    fldId = _item.fldId.ToString();
        //                    Cell Cell = sheet.Cells[alpha[index] + (k3 + 2)];
        //                    Cell.PutValue(fldId);
        //                    k3++;
        //                }
        //                index++;
        //                break;
        //            case "fldName":
        //                Check = "اسم";
        //                Cell cell6 = sheet.Cells[alpha[index] + "1"];
        //                //wb.Worksheets[0].ActiveCell = alpha[index] + "1";
        //                cell6
        //                wb.Worksheets[0].Split();
        //                cell6.PutValue(Check);
        //                int x = 0;
        //                foreach (var _item in data)
        //                {
        //                    fldName = _item.fldName.ToString();
        //                    Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
        //                    Cell.PutValue(fldName);
        //                    x++;
        //                }
        //                index++;
        //                break;
        //            //case "fldHagheDarman":
        //            //    Check = "حق بیمه";
        //            //    Cell cell5 = sheet.Cells[alpha[index] + "1"];
        //            //    wb.Worksheets[0].ActiveCell = alpha[index] + "1";
        //            //    wb.Worksheets[0].Split();
        //            //    //cell5.PutValue(Check);
        //            //    int z = 0;
        //            //    foreach (var _item in data)
        //            //    {
        //            //        fldHagheDarman = _item.fldHaghDarman.ToString();
        //            //        Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
        //            //        Cell.PutValue(fldHagheDarman);
        //            //        z++;
        //            //    }
        //            //    index++;
        //            //    break;
        //            //case "tabaee1":
        //            //    Check = "تعداد تبعی 1";
        //            //    Cell cell1 = sheet.Cells[alpha[index] + "1"];
        //            //    cell1.PutValue(Check);
        //            //    int j2 = 0;
        //            //    foreach (var _item in data)
        //            //    {
        //            //        tabaee1 = (_item.fldTedadBime1 - 1).ToString();
        //            //        Cell Cell = sheet.Cells[alpha[index] + (j2 + 2)];
        //            //        Cell.PutValue(tabaee1);
        //            //        j2++;
        //            //    }
        //            //    index++;
        //            //    break;
        //            //case "tabaee2":
        //            //    Check = "تعداد تبعی 2";
        //            //    Cell cell2 = sheet.Cells[alpha[index] + "1"];
        //            //    cell2.PutValue(Check);
        //            //    int k = 0;
        //            //    foreach (var _item in data)
        //            //    {
        //            //        tabaee2 = _item.fldTedadBime2.ToString();
        //            //        Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
        //            //        Cell.PutValue(tabaee2);
        //            //        k++;
        //            //    }
        //            //    index++;
        //            //    break;
        //            //case "tabaee3":
        //            //    Check = "تعداد تبعی 3";
        //            //    Cell cell3 = sheet.Cells[alpha[index] + "1"];
        //            //    cell3.PutValue(Check);
        //            //    int q = 0;
        //            //    foreach (var _item in data)
        //            //    {
        //            //        tabaee3 = _item.fldTedadBime3.ToString();
        //            //        Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
        //            //        Cell.PutValue(tabaee3);
        //            //        q++;
        //            //    }
        //            //    index++;
        //            //    break;
        //            //case "fldDateEjra":
        //            //    Check = "تاریخ شروع بکار";
        //            //    Cell cell4 = sheet.Cells[alpha[index] + "1"];
        //            //    cell4.PutValue(Check);
        //            //    int w = 0;
        //            //    foreach (var _item in data)
        //            //    {
        //            //        fldDateEjra = _item.fldTarikhEjra;
        //            //        Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
        //            //        Cell.PutValue(fldDateEjra);
        //            //        w++;
        //            //    }
        //            //    index++;
        //            //    break;
        //            //case "fldName":
        //            //    Check = "نام";
        //            //    Cell cell = sheet.Cells[alpha[index] + "1"];
        //            //    cell.PutValue(Check);
        //            //    int i = 0;
        //            //    foreach (var _item in data)
        //            //    {
        //            //        fldName = _item.fldName;
        //            //        Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
        //            //        Cell.PutValue(fldName);
        //            //        i++;
        //            //    }
        //            //    index++;
        //            //    break;
        //            //case "fldFamily":
        //            //    Check = "نام خانوادگی";
        //            //    Cell cell12 = sheet.Cells[alpha[index] + "1"];
        //            //    cell12.PutValue(Check);
        //            //    int j = 0;
        //            //    foreach (var _item in data)
        //            //    {
        //            //        fldFamily = _item.fldFamily;
        //            //        Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
        //            //        Cell.PutValue(fldFamily);
        //            //        j++;
        //            //    }
        //            //    index++;
        //            //    break;
        //            //case "fldShenase":
        //            //    Check = "شناسه عادی / ایثارگر";
        //            //    Cell cell7 = sheet.Cells[alpha[index] + "1"];
        //            //    cell7.PutValue(Check);
        //            //    int y = 0;
        //            //    foreach (var _item in data)
        //            //    {
        //            //        fldShenase = "";
        //            //        Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
        //            //        Cell.PutValue(fldShenase);
        //            //        y++;
        //            //    }
        //            //    index++;
        //            //    break;
        //            //case "fldKarkard":
        //            //    Check = "تاریخ پایان کار";
        //            //    Cell cell23 = sheet.Cells[alpha[index] + "1"];
        //            //    cell23.PutValue(Check);
        //            //    int k2 = 0;
        //            //    foreach (var _item in data)
        //            //    {
        //            //        fldKarkard = "";// _item.fldKarkard.ToString();
        //            //        Cell Cell = sheet.Cells[alpha[index] + (k2 + 2)];
        //            //        Cell.PutValue(fldKarkard);
        //            //        k2++;
        //            //    }
        //            //    index++;
        //            //    break;
        //            //case "fldMoavaghe":
        //            //    Check = "حقوق معوقه مشمول";
        //            //    Cell cell24 = sheet.Cells[alpha[index] + "1"];
        //            //    cell24.PutValue(Check);
        //            //    int k4 = 0;
        //            //    foreach (var _item in data)
        //            //    {
        //            //        fldMoavaghe = _item.fldMoavaghe.ToString();
        //            //        Cell Cell = sheet.Cells[alpha[index] + (k4 + 2)];
        //            //        Cell.PutValue(fldMoavaghe);
        //            //        k4++;
        //            //    }
        //            //    index++;
        //            //    break;
        //        }
        //    }
        //    MemoryStream stream = new MemoryStream();
        //    wb.Save(stream, SaveFormat.Excel97To2003);
        //    //var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
        //    //var FileName = "حق درمان " + MonthName + " ماه " + "سال " + Year.ToString() + ".xls";
        //    return File(stream.ToArray(), "xls","test");
        //}
        public ActionResult GetAccountingLevel(short Year,string Node)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAccountingLevelWithFilter("fldYear_CodingId", Year.ToString(),Node, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).OrderBy(l => l.fldId).ToList()
                .Select(c => new { fldId = c.fldLevelId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetTaAccountingLevel(short Year, string AzLevel)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAccountingLevelWithFilter("fldYear_AccountLevel", Year.ToString(),AzLevel, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).OrderBy(l => l.fldId).ToList()
                .Select(c => new { fldId = c.fldLevelId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult GeneratePdfRptTarazMahane(short Year, byte Month,string AzTarikh,string TaTarikh,int CodingId,int AzDocNum,int TaDocNum, string MonthName,byte Level, short Taraz,
            string TarazName, bool CheckSath,byte TypeSanad, int SourceId, byte CaseTypeId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                bool ShowSum = CodingId==0?true:false;
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
                WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
                if (Month != 0)
                {
                    AzTarikh=Year+"/"+Month.ToString().PadLeft(2,'0')+"/01";
                    TaTarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/31";
                }
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                DataSet.DataSet1TableAdapters.spr_RptMonthlyBalanceTableAdapter MonthlyBalance = new DataSet.DataSet1TableAdapters.spr_RptMonthlyBalanceTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                MonthlyBalance.Fill(dt.spr_RptMonthlyBalance, Level, CheckSath, Convert.ToInt32(Session["OrganId"]), 4, TypeSanad, AzTarikh, TaTarikh, AzDocNum, TaDocNum, CodingId, SourceId, CaseTypeId);
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "dataSetAccounting");
                Report.RegisterData(dt, "dataSetAccounting");
                if (Taraz == 4)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptTarazMahane_4.frx");
                else if(Taraz == 6)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptTarazMahane_6.frx");
                else if (Taraz == 8)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptTarazMahane.frx");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Mah", MonthName);
                Report.SetParameterValue("Sal", Year);
                Report.SetParameterValue("Taraz", TarazName);
                Report.SetParameterValue("Tarikh1", AzTarikh);
                Report.SetParameterValue("Tarikh2", TaTarikh);
                Report.SetParameterValue("displysum", ShowSum);
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrintTarazMahane(/*string containerId,*/ int Year, string AzTarikh, string TaTarikh, string AzLevel, string TaLevel, string AzSanad, string TaSanad, int StartNodeID, string sanadtype, string Taraz)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var az = servic.GetDocumentRecord_HeaderWithFilter("fldId", AzSanad,"","", Convert.ToInt32(Session["OrganId"]), Convert.ToInt16(Year), 4, 0, IP, out Err).FirstOrDefault();
            var ta = servic.GetDocumentRecord_HeaderWithFilter("fldId", TaSanad, "", "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt16(Year), 4, 0, IP, out Err).FirstOrDefault();
            var ss=servic.GetFiscalYearWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            PartialView.ViewBag.AzTarikh = AzTarikh;
            PartialView.ViewBag.TaTarikh = TaTarikh;
            PartialView.ViewBag.AzLevel = AzLevel;
            PartialView.ViewBag.TaLevel = TaLevel;
            PartialView.ViewBag.AzSanad = az.fldDocumentNum.ToString();
            PartialView.ViewBag.TaSanad = ta.fldDocumentNum.ToString();
            PartialView.ViewBag.StartNodeID = StartNodeID;
            PartialView.ViewBag.sanadtype = sanadtype;
            PartialView.ViewBag.Taraz = Taraz;
            PartialView.ViewBag.salMali = ss.fldId;
            PartialView.ViewBag.OrganId = Convert.ToInt32(Session["OrganId"]);
            PartialView.ViewBag.UserId = Convert.ToInt32(Session["UserId"]);

            return PartialView;
        }
        public ActionResult PdfTarazMahane(short Year, string AzTarikh, string TaTarikh, string AzLevel, string TaLevel, string AzSanad, string TaSanad, int StartNodeID, string sanadtype, string Taraz)
        {
            try
            {
                var Module_OrganId = servic_Common.GetModule_OrganWithFilter("CheckOrganId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP,
                    out ErrCommon).Where(l => l.fldModuleId == 4).FirstOrDefault().fldId;
                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptTarazMahane.frx";
                rep.Load(path);
                rep.SetParameterValue("aztarikh", AzTarikh);
                rep.SetParameterValue("tatarikh", TaTarikh);
                rep.SetParameterValue("salmaliID", Year);
                rep.SetParameterValue("OrganId", Convert.ToInt32(Session["OrganId"]));
                rep.SetParameterValue("azLevel", Convert.ToInt32(AzLevel));
                rep.SetParameterValue("tanLevel", Convert.ToInt32(TaLevel));
                rep.SetParameterValue("azsanad", Convert.ToInt32(AzSanad));
                rep.SetParameterValue("tasanad", Convert.ToInt32(TaSanad));
                rep.SetParameterValue("StartNodeID", Convert.ToInt32(StartNodeID));
                rep.SetParameterValue("sanadtype", Convert.ToInt32(sanadtype));
                rep.SetParameterValue("Taraz", Convert.ToInt32(Taraz));
                rep.SetParameterValue("ReportId", 52);
                rep.SetParameterValue("UserId", Convert.ToInt32(Session["UserId"]));
                rep.SetParameterValue("UserName", Session["Username"]);
                rep.SetParameterValue("fldModule_OrganId", Module_OrganId);                
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


        public ActionResult ReportPage(int Year, string AzTarikh, string TaTarikh, string AzLevel, string TaLevel, string AzSanad, string TaSanad, int StartNodeID, string sanadtype, string Taraz)
        {//باز شدن تب جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Accounting\RptTarazMahane.frx";
            ViewBag.Path = path;
            ViewBag.RId ="Taraz"+ Session["Username"].ToString() + ";" + Session["Password"].ToString() + ";" + Session["OrganId"].ToString();

            var az = servic.GetDocumentRecord_HeaderWithFilter("fldId", AzSanad, "", "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt16(Year), 4, 0, IP, out Err).FirstOrDefault();
            var ta = servic.GetDocumentRecord_HeaderWithFilter("fldId", TaSanad, "", "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt16(Year), 4, 0, IP, out Err).FirstOrDefault();
            
            var ss = servic.GetFiscalYearWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
            ViewBag.AzTarikh = AzTarikh;
            ViewBag.TaTarikh = TaTarikh;
            ViewBag.AzLevel = AzLevel;
            ViewBag.TaLevel = TaLevel;
            ViewBag.AzSanad = az.fldDocumentNum.ToString();
            ViewBag.TaSanad = ta.fldDocumentNum.ToString();
            ViewBag.StartNodeID = StartNodeID;
            ViewBag.sanadtype = sanadtype;
            ViewBag.Taraz = Taraz;
            ViewBag.salMali = ss.fldId;
            ViewBag.OrganId = Convert.ToInt32(Session["OrganId"]);
            ViewBag.UserId = Convert.ToInt32(Session["UserId"]);
           ViewBag.connectionstr=System.Configuration.ConfigurationManager.ConnectionStrings["RasaFMSCommonDBConnectionString"].ConnectionString;
            return View();
        }
        public ActionResult CheckTarikh(string Tarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var sal = Tarikh.Substring(0, 4);
            var dateAz = MyLib.Shamsi.Shamsi2miladiDateTime(sal+"/01/01");
            var dateTa = MyLib.Shamsi.Shamsi2miladiDateTime(sal + "/12/29");
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(sal)))
                dateTa = MyLib.Shamsi.Shamsi2miladiDateTime(sal + "/12/30");
            

            return Json(new
            {
                dateAz = dateAz.ToString("yyyy-MM-dd"),
                dateTa = dateTa.ToString("yyyy-MM-dd")
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
