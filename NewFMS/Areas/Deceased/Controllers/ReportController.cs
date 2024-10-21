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

namespace NewFMS.Areas.Deceased.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Deceased/Report/

        WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();

        WCF_Deceased.DeceasedService servic = new WCF_Deceased.DeceasedService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Deceased.ClsError Err = new WCF_Deceased.ClsError();

        public ActionResult Index(string containerId, string State)
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
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult GetVadiSalam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetVadiSalamWithFilter("fldOrganId", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Title = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetGhete(int VadiSalamId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetGheteWithFilter("fldVadiSalamId", VadiSalamId.ToString(), 0, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Title = c.fldNameGhete });
            return this.Store(q);
        }
        public ActionResult GetRadif(int GheteId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetRadifWithFilter("fldGheteId", GheteId.ToString(), 0, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Title = c.fldNameRadif });
            return this.Store(q);
        }
        public ActionResult GetShomare(int RadifId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetShomareWithFilter("fldRadifId", RadifId.ToString(), 0, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Title = c.fldShomare });
            return this.Store(q);
        }
        public ActionResult GeneratePDFGhabrAmanat(string VadiSalamId, string GheteId, string RadifId, string ShomareId, int state)
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
                DataSet.DataSet1TableAdapters.spr_RptGhabrAmanatTableAdapter ListGhabrAmanat = new DataSet.DataSet1TableAdapters.spr_RptGhabrAmanatTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);

                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDeceased");
                Report.RegisterData(dt, "rasaFMSDeceased");

                var Fieldname="Amanat";
                if(state==2)
                    Fieldname = "Fotshode";
                if (GheteId == "" || GheteId == null || GheteId == "null")
                    GheteId = "0";
                if (RadifId == "" || RadifId == null || RadifId == "null")
                    RadifId = "0";
                if (ShomareId == "" || ShomareId == null || ShomareId == "null")
                    ShomareId = "0";
                ListGhabrAmanat.Fill(dt.spr_RptGhabrAmanat, Fieldname,ShomareId,RadifId, GheteId, VadiSalamId, (Session["OrganId"]).ToString());

                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptGhabrAmanat.frx"); 
                if (state == 2)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptMotevafa.frx");
                

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
        public ActionResult GeneratePDFGhete(string VadiSalamId,int TypeRpt)
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
                DataSet.DataSet1TableAdapters.spr_RptGhetePor_KhaliTableAdapter ListGhete = new DataSet.DataSet1TableAdapters.spr_RptGhetePor_KhaliTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);

                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDeceased");
                Report.RegisterData(dt, "rasaFMSDeceased");

                var Fieldname = "nesfe";
                if (TypeRpt ==1)
                    Fieldname = "Por";
                else if (TypeRpt == 0)
                    Fieldname = "Khali";

                ListGhete.Fill(dt.spr_RptGhetePor_Khali, Fieldname,VadiSalamId,(Session["OrganId"]).ToString());

                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptGheteNesfe.frx");
               
                 if (TypeRpt == 1)
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptGhete.frx");
                 else if (TypeRpt == 0)
                     Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptGhetekhali.frx");

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
        public ActionResult GenerateExcelRptGhete(string VadiSalamId, int TypeRpt)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI" };
                int index = 0;
                var Check = "";
                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                string NameGhete = "";
                var Checked = "fldNameRadif;fldShomare;TabaghatKhali;";
                var StatusCheck = Checked.Split(';');

                var Fieldname = "nesfe";
                if (TypeRpt == 1)
                {
                    Fieldname = "Por";
                    Checked = "NameGhete;Name_Family;Meli_Moshakhase;FatherName;Sh_Sh;TarikhFot;";
                }
                else if (TypeRpt == 0)
                {
                    Fieldname = "Khali";
                    Checked = "NameGhete;";
                }


                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet1TableAdapters.spr_RptGhetePor_KhaliTableAdapter ListGhete = new DataSet.DataSet1TableAdapters.spr_RptGhetePor_KhaliTableAdapter();
                ListGhete.Fill(dt.spr_RptGhetePor_Khali, Fieldname, VadiSalamId, (Session["OrganId"]).ToString());
                var data = dt.spr_RptGhetePor_Khali.ToList();

               
            //    var data = servic.RptGhabrPor_Khali(Fieldname, Convert.ToInt32(VadiSalamId), 0, 0, 0, Convert.ToInt32(Session["OrganId"]), IP, out Err).ToList();

                foreach (var item in StatusCheck)
                {
                    switch (item)
                    {
                        case "NameGhete":
                            Check = "نام قطعه";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int i = 0;
                            foreach (var _item in data)
                            {
                                NameGhete = _item.NameGhete;
                                Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                                Cell.PutValue(NameGhete);
                                i++;
                            }
                            index++;
                            break;
                        case "Name_Family": 
                            Check = "نام شخص";
                            Cell cell1 = sheet.Cells[alpha[index] + "1"];
                            cell1.PutValue(Check);
                            int i1 = 0;
                            foreach (var _item in data)
                            {
                                NameGhete = _item.Name_Family;
                                Cell Cell = sheet.Cells[alpha[index] + (i1 + 2)];
                                Cell.PutValue(NameGhete);
                                i1++;
                            }
                            index++;
                            break;
                        case "Meli_Moshakhase":
                            Check = "کدملی/مشخصه";
                            Cell cell2 = sheet.Cells[alpha[index] + "1"];
                            cell2.PutValue(Check);
                            int i2 = 0;
                            foreach (var _item in data)
                            {
                                NameGhete = _item.Meli_Moshakhase;
                                Cell Cell = sheet.Cells[alpha[index] + (i2 + 2)];
                                Cell.PutValue(NameGhete);
                                i2++;
                            }
                            index++;
                            break;
                        case "FatherName":
                            Check = "نام پدر";
                            Cell cell3 = sheet.Cells[alpha[index] + "1"];
                            cell3.PutValue(Check);
                            int i3 = 0;
                            foreach (var _item in data)
                            {
                                NameGhete = _item.FatherName;
                                Cell Cell = sheet.Cells[alpha[index] + (i3 + 2)];
                                Cell.PutValue(NameGhete);
                                i3++;
                            }
                            index++;
                            break;
                        case "Sh_Sh":
                            Check = "شماره شناستامه";
                            Cell cell4 = sheet.Cells[alpha[index] + "1"];
                            cell4.PutValue(Check);
                            int i4 = 0;
                            foreach (var _item in data)
                            {
                                NameGhete = _item.Sh_Sh;
                                Cell Cell = sheet.Cells[alpha[index] + (i4 + 2)];
                                Cell.PutValue(NameGhete);
                                i4++;
                            }
                            index++;
                            break;
                        case "TarikhFot": 
                            Check = "تاریخ فوت";
                            Cell cell5 = sheet.Cells[alpha[index] + "1"];
                            cell5.PutValue(Check);
                            int i5 = 0;
                            foreach (var _item in data)
                            {
                                NameGhete = _item.TarikhFot;
                                Cell Cell = sheet.Cells[alpha[index] + (i5 + 2)];
                                Cell.PutValue(NameGhete);
                                i5++;
                            }
                            index++;
                            break;
                        case "TabaghatKhali":
                            Check = "تعداد طبقه";
                            Cell cell6 = sheet.Cells[alpha[index] + "1"];
                            cell6.PutValue(Check);
                            int i6 = 0;
                            foreach (var _item in data)
                            {
                                NameGhete = _item.TabaghatKhali;
                                Cell Cell = sheet.Cells[alpha[index] + (i6 + 2)];
                                Cell.PutValue(NameGhete);
                                i6++;
                            }
                            index++;
                            break;
                        case "fldNameRadif":
                            Check = "ردیف";
                            Cell cell7 = sheet.Cells[alpha[index] + "1"];
                            cell7.PutValue(Check);
                            int i7 = 0;
                            foreach (var _item in data)
                            {
                                NameGhete = _item.fldNameRadif;
                                Cell Cell = sheet.Cells[alpha[index] + (i7 + 2)];
                                Cell.PutValue(NameGhete);
                                i7++;
                            }
                            index++;
                            break;
                        case "fldShomare":
                            Check = "شماره";
                            Cell cell8 = sheet.Cells[alpha[index] + "1"];
                            cell8.PutValue(Check);
                            int i8 = 0;
                            foreach (var _item in data)
                            {
                                NameGhete = _item.fldShomare;
                                Cell Cell = sheet.Cells[alpha[index] + (i8 + 2)];
                                Cell.PutValue(NameGhete);
                                i8++;
                            }
                            index++;
                            break;
                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "xlsx", "گزارش قطعات.xls");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GeneratePDFRadif(string VadiSalamId, string GheteId, int TypeRpt)
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
                DataSet.DataSet1TableAdapters.spr_RptRadifPor_KhaliTableAdapter ListGhete = new DataSet.DataSet1TableAdapters.spr_RptRadifPor_KhaliTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);

                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDeceased");
                Report.RegisterData(dt, "rasaFMSDeceased");

                var Fieldname = "nesfe";
                if (TypeRpt == 1)
                    Fieldname = "Por";
                else if (TypeRpt == 0)
                    Fieldname = "Khali";

                if (GheteId == "" || GheteId == null || GheteId == "null")
                    GheteId = "0";

                ListGhete.Fill(dt.spr_RptRadifPor_Khali, Fieldname, VadiSalamId, (Session["OrganId"]).ToString(), GheteId);

                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptRadifNesfe.frx");

                if (TypeRpt == 0)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptRadifkhali.frx");
                else if (TypeRpt == 1)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptRadif.frx");

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
        public ActionResult GeneratePDFShomare(string VadiSalamId, string GheteId,string RadifId, int TypeRpt)
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
                DataSet.DataSet1TableAdapters.spr_RptShomarePor_KhaliTableAdapter ListGhete = new DataSet.DataSet1TableAdapters.spr_RptShomarePor_KhaliTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);

                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDeceased");
                Report.RegisterData(dt, "rasaFMSDeceased");

                var Fieldname = "Nesfe";
                if (TypeRpt == 1)
                    Fieldname = "Por";
                else if (TypeRpt == 0)
                    Fieldname = "Khali";

                if (GheteId == "" || GheteId == null || GheteId == "null")
                    GheteId = "0";
                if (RadifId == "" || RadifId == null || RadifId == "null")
                    RadifId = "0";

                ListGhete.Fill(dt.spr_RptShomarePor_Khali, Fieldname, VadiSalamId,  (Session["OrganId"]).ToString(),GheteId, RadifId);

                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptShomareNesfe.frx");

                if (TypeRpt == 1)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptShomare.frx");
                else if (TypeRpt == 0)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptShomarekhali.frx");

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
        public ActionResult GeneratePDFTabaghe(string VadiSalamId, string GheteId, string RadifId,string ShomareId, int TypeRpt)
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
                DataSet.DataSet1TableAdapters.spr_RptGhabrPor_KhaliTableAdapter ListGhete = new DataSet.DataSet1TableAdapters.spr_RptGhabrPor_KhaliTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);

                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDeceased");
                Report.RegisterData(dt, "rasaFMSDeceased");

                var Fieldname = "nesfe";
                if (TypeRpt == 1)
                    Fieldname = "Por";
                else if (TypeRpt == 0)
                    Fieldname = "Khali";

                if (GheteId == "" || GheteId == null || GheteId == "null")
                    GheteId = "0";
                if (RadifId == "" || RadifId == null || RadifId == "null")
                    RadifId = "0";
                if (ShomareId == "" || ShomareId == null || ShomareId == "null")
                    ShomareId = "0";

                ListGhete.Fill(dt.spr_RptGhabrPor_Khali, Fieldname, Convert.ToInt32(VadiSalamId), Convert.ToInt32(GheteId), Convert.ToInt32(RadifId), Convert.ToInt32(ShomareId), Convert.ToInt32(Session["OrganId"]));

                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptTabagheNesfe.frx");

                if (TypeRpt == 1)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptTabaghe.frx");
                if (TypeRpt == 0)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Deceased\RptTabaghekhali.frx");

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
    }
}
