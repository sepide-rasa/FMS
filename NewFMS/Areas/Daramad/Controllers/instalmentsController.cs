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
using System.Data.SqlClient;
using System.Web.Configuration;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class instalmentsController : Controller
    {
        //
        // GET: /Daramad/instalments/
        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();
        WCF_Common.CommonService servic = new WCF_Common.CommonService();

        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_Accounting.ClsError AccErr = new WCF_Accounting.ClsError();
        WCF_Accounting.AccountingService AccService = new WCF_Accounting.AccountingService();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        public ActionResult Index(int ReplyTaghsitId, int ElameAvarezId, int OrganId,int FiscalYearId)
        {
            if (Session["Username"] == null || Session["OrganId"]==null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q = servicD.GetReplyTaghsitDetail(ReplyTaghsitId, IP, out ErrD);
            var Req=servicD.GetRequestTaghsit_TakhfifWithFilter("Taghsit", ElameAvarezId.ToString(), 0, IP, out ErrD).FirstOrDefault();
            if (ErrD.ErrorType)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = ErrD.ErrorMsg
                });
                DirectResult result = new DirectResult();
                return result;
            }
            var q1 = servicD.GetElamAvarezDetail(ElameAvarezId, Session["OrganId"].ToString(), IP, out ErrD);
            if (ErrD.ErrorType)
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = ErrD.ErrorMsg
                });
                DirectResult result = new DirectResult();
                return result;
            }
            var setting = servicD.GetTanzimateDaramadWithFilter("fldOrganId", OrganId.ToString(),FiscalYearId, 1,IP, out ErrD).FirstOrDefault();
            
            var Date =servic.GetDate(IP, out Err);
            PartialView.ViewBag.Date = Date.fldDateTime;
            PartialView.ViewBag.ReplyTaghsitId = ReplyTaghsitId;
            PartialView.ViewBag.fldMablaghNaghdi = q.fldMablaghNaghdi;
            PartialView.ViewBag.ElameAvarezId = ElameAvarezId;
            PartialView.ViewBag.fldShomareHesabPishfarz = setting.fldShomareHesab;
            PartialView.ViewBag.fldShomareHesabPishfarzId = setting.fldShomareHesabIdPishfarz;
            PartialView.ViewBag.OrganId = OrganId;            
            PartialView.ViewBag.SumTedad = Req.SumTedad;
            PartialView.ViewBag.FiscalYearId = FiscalYearId;
            PartialView.ViewBag.fldNameOrgan = q1.fldNameOrgan;
            PartialView.ViewBag.fldMablaghGHabelPardakht = q1.fldMablaghGHabelPardakht;         
            return PartialView;
        }

        public ActionResult Read(int ReplyTaghsitId)
        {
            if (Session["Username"] == null || Session["OrganId"]==null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Daramad.OBJ_SelectDataForTaghsit> data = null;
            data = servicD.SelectDataForTaghsit("ReplyTaghsitId", ReplyTaghsitId.ToString(),"","", Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrD).ToList();
            return this.Store(data);
        }

        public ActionResult PrintCheck(int OrganId, int CheckId, int? ReplyTaghsitId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            if (ReplyTaghsitId == null)
            {
                ReplyTaghsitId=servicD.GetCheckDetail(CheckId, IP, out ErrD).fldReplyTaghsitId;
            }
            result.ViewBag.OrganId = OrganId;
            result.ViewBag.CheckId = CheckId;
            result.ViewBag.ReplyTaghsitId = ReplyTaghsitId;
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }

        public ActionResult PrintReciept(int ElameAvarezId, int ReplyTaghsitId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.ElameAvarezId = ElameAvarezId;
            result.ViewBag.ReplyTaghsitId = ReplyTaghsitId;
            result.ViewBag.OrganId = OrganId;
            return result;
        }

        public ActionResult GeneratePDFPrintReciept(int ElameAvarezId, int ReplyTaghsitId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

                DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter Filee = new DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
                DataSet.DataSet1TableAdapters.spr_SelectDataForTaghsitTableAdapter Taghsit = new DataSet.DataSet1TableAdapters.spr_SelectDataForTaghsitTableAdapter();
                DataSet.DataSet1TableAdapters.spr_tblElamAvarezSelectTableAdapter ElamAvarez = new DataSet.DataSet1TableAdapters.spr_tblElamAvarezSelectTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_tblRequestTaghsit_TakhfifSelectTableAdapter RequestTaghsit = new DataSet.DataSet1TableAdapters.spr_tblRequestTaghsit_TakhfifSelectTableAdapter();

                string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/instalments/GetPrintReciept?ElameAvarezId=" + ElameAvarezId + "&ReplyTaghsitId=" + ReplyTaghsitId + "&OrganId=" + OrganId;
                dt.EnforceConstraints = false;
                var organ = servic.GetOrganizationDetail(OrganId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1,IP, out Err).FirstOrDefault();

                Taghsit.Fill(dt.spr_SelectDataForTaghsit, ReplyTaghsitId.ToString(), "Ejra", "", "", 0, OrganId);
                ElamAvarez.Fill(dt.spr_tblElamAvarezSelect, "fldId", ElameAvarezId.ToString(), OrganId.ToString(), 0);
                Filee.Fill(dt_Com.spr_tblFileSelect, "fldId", organ.fldFileId.ToString(), 0);
                Date.Fill(dt_Com.spr_GetDate);
                RequestTaghsit.Fill(dt.spr_tblRequestTaghsit_TakhfifSelect, "fldElamAvarezId_Report", ElameAvarezId.ToString(), 0);

                string LetterNumber = "";
                string ShName = "";
                string tarikh = servic.GetDate(IP, out Err).fldTarikh;
                //if (Err.ErrorType)
                //{
                //    return Json(new
                //    {
                //        Msg = Err.ErrorMsg,
                //        MsgTitle = "خطا",
                //        Err = 1
                //    }, JsonRequestBehavior.AllowGet);
                //}
                var NumberFormat = servicD.GetFormatShomareNameWithFilter("fldYear", tarikh.Substring(0, 4), 0, IP, out ErrD).Where(l=>l.fldType==true).FirstOrDefault();
                //if (ErrD.ErrorType)
                //{
                //    return Json(new
                //    {
                //        Msg = Err.ErrorMsg,
                //        MsgTitle = "خطا",
                //        Err = 1
                //    }, JsonRequestBehavior.AllowGet);
                //}
                if (NumberFormat == null)
                {
                    LetterNumber = "";
                    //return Json(new
                    //{
                    //    Msg = "فرمت شماره نامه برای سال جاری تعریف نشده است.",
                    //    MsgTitle = "خطا",
                    //    Err = 1
                    //}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var Format = NumberFormat.fldFormatShomareName.Split('*');
                    for (int i = 0; i < Format.Count() - 1; i++)
                    {
                        switch (Format[i])
                        {
                            case "shomarande":
                                ShName = servicD.InsertShomareNameHa(null, ReplyTaghsitId, NumberFormat.fldShomareShoro, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrD).ToString();
                                LetterNumber += ShName;
                                break;
                            case "sal2":
                                LetterNumber += tarikh.Substring(2, 2);
                                break;
                            case "sal4":
                                LetterNumber += tarikh.Substring(0, 4);
                                break;
                            default:
                                LetterNumber += Format[i];
                                break;
                        }
                    }
                }

                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", (Session["OrganId"]).ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 5).FirstOrDefault();
                var Tarikh = servic.GetDate(IP, out Err).fldTarikh;
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 50, IP, out Err).ToList();
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

                var FileId = 0;
                var k = servicD.GetGozareshatFileWithFilter("fldGozareshatId", "2", (Session["OrganId"]).ToString(), 0, IP, out ErrD).FirstOrDefault();
                if (k != null)
                {
                    FileId = k.fldReportFileId;
                }
                var reportFile = servic.GetFileWithFilter("fldId", FileId.ToString(), 1, IP, out Err).FirstOrDefault();
                

                FastReport.Report Report = new FastReport.Report();
                //if (FileId == 0)
                //    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptResidMoadi.frx");
                //else
                //{
                    MemoryStream m = new MemoryStream(reportFile.fldImage);
                    Report.Load(m);
                //}

                Report.RegisterData(dt, "rasaFMSDaramad");
                Report.RegisterData(dt_Com, "rasaFMSDaramad");
                Report.SetParameterValue("Shomare", LetterNumber);
                Report.SetParameterValue("NameOrgan", organ.fldName);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
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
        public ActionResult GetPrintReciept(int ElameAvarezId, int ReplyTaghsitId, int OrganId)
        {
            ViewBag.ElameAvarezId = ElameAvarezId;
            ViewBag.ReplyTaghsitId = ReplyTaghsitId;
            ViewBag.OrganId = OrganId;
            return View();
        }
        public FileResult DownloadPrintReciept(int ElameAvarezId, int ReplyTaghsitId, int OrganId)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

            DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter Filee = new DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
            DataSet.DataSet1TableAdapters.spr_SelectDataForTaghsitTableAdapter Taghsit = new DataSet.DataSet1TableAdapters.spr_SelectDataForTaghsitTableAdapter();
            DataSet.DataSet1TableAdapters.spr_tblElamAvarezSelectTableAdapter ElamAvarez = new DataSet.DataSet1TableAdapters.spr_tblElamAvarezSelectTableAdapter();
            DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
            DataSet.DataSet1TableAdapters.spr_tblRequestTaghsit_TakhfifSelectTableAdapter RequestTaghsit = new DataSet.DataSet1TableAdapters.spr_tblRequestTaghsit_TakhfifSelectTableAdapter();

            string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/instalments/GetPrintReciept?ElameAvarezId=" + ElameAvarezId + "&ReplyTaghsitId=" + ReplyTaghsitId + "&OrganId=" + OrganId;
            dt.EnforceConstraints = false;
            var organ = servic.GetOrganizationDetail(OrganId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
            var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();

            Taghsit.Fill(dt.spr_SelectDataForTaghsit, ReplyTaghsitId.ToString(), "Ejra", "", "", 0, OrganId);
            ElamAvarez.Fill(dt.spr_tblElamAvarezSelect, "fldId", ElameAvarezId.ToString(), OrganId.ToString(), 0);
            Filee.Fill(dt_Com.spr_tblFileSelect, "fldId", organ.fldFileId.ToString(), 0);
            Date.Fill(dt_Com.spr_GetDate);
            RequestTaghsit.Fill(dt.spr_tblRequestTaghsit_TakhfifSelect, "fldElamAvarezId_Report", ElameAvarezId.ToString(), 0);

            string LetterNumber = "";
            string ShName = "";
            string tarikh = servic.GetDate(IP, out Err).fldTarikh;
            //if (Err.ErrorType)
            //{
            //    return Json(new
            //    {
            //        Msg = Err.ErrorMsg,
            //        MsgTitle = "خطا",
            //        Err = 1
            //    }, JsonRequestBehavior.AllowGet);
            //}
            var NumberFormat = servicD.GetFormatShomareNameWithFilter("fldYear", tarikh.Substring(0, 4), 0, IP, out ErrD).Where(l => l.fldType == true).FirstOrDefault();
            //if (ErrD.ErrorType)
            //{
            //    return Json(new
            //    {
            //        Msg = Err.ErrorMsg,
            //        MsgTitle = "خطا",
            //        Err = 1
            //    }, JsonRequestBehavior.AllowGet);
            //}
            if (NumberFormat == null)
            {
                LetterNumber = "";
                //return Json(new
                //{
                //    Msg = "فرمت شماره نامه برای سال جاری تعریف نشده است.",
                //    MsgTitle = "خطا",
                //    Err = 1
                //}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Format = NumberFormat.fldFormatShomareName.Split('*');
                for (int i = 0; i < Format.Count() - 1; i++)
                {
                    switch (Format[i])
                    {
                        case "shomarande":
                            ShName = servicD.InsertShomareNameHa(null, ReplyTaghsitId, NumberFormat.fldShomareShoro, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrD).ToString();
                            LetterNumber += ShName;
                            break;
                        case "sal2":
                            LetterNumber += tarikh.Substring(2, 2);
                            break;
                        case "sal4":
                            LetterNumber += tarikh.Substring(0, 4);
                            break;
                        default:
                            LetterNumber += Format[i];
                            break;
                    }
                }
            }

            string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
            string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
            var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", (Session["OrganId"]).ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 5).FirstOrDefault();
            var Tarikh = servic.GetDate(IP, out Err).fldTarikh;
            var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 50, IP, out Err).ToList();
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

            var FileId = 0;
            var k = servicD.GetGozareshatFileWithFilter("fldGozareshatId", "2", (Session["OrganId"]).ToString(), 0, IP, out ErrD).FirstOrDefault();
            if (k != null)
            {
                FileId = k.fldReportFileId;
            }
            var reportFile = servic.GetFileWithFilter("fldId", FileId.ToString(), 1, IP, out Err).FirstOrDefault();


            FastReport.Report Report = new FastReport.Report();
            //if (FileId == 0)
            //    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptResidMoadi.frx");
            //else
            //{
            MemoryStream m = new MemoryStream(reportFile.fldImage);
            Report.Load(m);
            //}

            Report.RegisterData(dt, "rasaFMSDaramad");
            Report.RegisterData(dt_Com, "rasaFMSDaramad");
            Report.SetParameterValue("Shomare", LetterNumber);
            Report.SetParameterValue("NameOrgan", organ.fldName);
            Report.SetParameterValue("UserName", User.fldNameEmployee);
            Report.SetParameterValue("s5", s5);
            Report.SetParameterValue("t1", t1);
            Report.SetParameterValue("t2", t2);
            Report.SetParameterValue("t3", t3);
            Report.SetParameterValue("t4", t4);
            Report.SetParameterValue("t5", t5);
            Report.SetParameterValue("s1", s1);
            Report.SetParameterValue("s2", s2);
            Report.SetParameterValue("s3", s3);
            Report.SetParameterValue("s4", s4);
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
        public ActionResult GeneratePDFPrintCheck(int OrganId, int CheckId, int ReplyTaghsitId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

                DataSet.DataSet1TableAdapters.spr_tblTanzimateDaramadSelectTableAdapter Setting = new DataSet.DataSet1TableAdapters.spr_tblTanzimateDaramadSelectTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptCheckTableAdapter RptCheck = new DataSet.DataSet1TableAdapters.spr_RptCheckTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                dt.EnforceConstraints = false;
                Setting.Fill(dt.spr_tblTanzimateDaramadSelect, "fldOrganId", OrganId.ToString(),FiscalYearId, 0);
                if (CheckId == 0)
                {
                    RptCheck.Fill(dt.spr_RptCheck, "ReplyId", ReplyTaghsitId.ToString(),"","");
                }
                else
                {
                    RptCheck.Fill(dt.spr_RptCheck, "CheckId", CheckId.ToString(),"","");
                }
                Date.Fill(dt_Com.spr_GetDate);

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptCheck.frx");
                Report.RegisterData(dt, "rasaFMSDaramad");
                Report.RegisterData(dt_Com, "rasaFMSDaramad");
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
        public ActionResult Delete(int Id, byte Type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NewFMS.Models.RasaNewFMSEntities m=new NewFMS.Models.RasaNewFMSEntities();

            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                if (Type == 1 || Type == 5)/*نقدی یا طلب*/
                {
                    MsgTitle = "حذف موفق";
                    Msg = servicD.DeleteNaghdi_Talab(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = ErrD.ErrorMsg;
                        Er = 1;
                    }
                }
                else if (Type == 2 || Type==6)/*چک*/
                {
                    MsgTitle = "حذف موفق";
                    Msg = servicD.DeleteCheck(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = ErrD.ErrorMsg;
                        Er = 1;
                    }
                    else
                        m.spr_tblCheck_SmsDelete(Id, Convert.ToInt32(Session["UserId"]));
                }
                else if (Type == 3)/*سفته*/
                {
                    MsgTitle = "حذف موفق";
                    Msg = servicD.DeleteSafte(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = ErrD.ErrorMsg;
                        Er = 1;
                    }
                }
                else if (Type == 4)/*برات*/
                {
                    MsgTitle = "حذف موفق";
                    Msg = servicD.DeleteBarat(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = ErrD.ErrorMsg;
                        Er = 1;
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult BackDoc(int Id, byte Type)
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    string Msg = "", MsgTitle = ""; var Er = 0; byte status = 0;

        //    try
        //    {
        //        if (Type == 2 || Type == 6)/*چک*/
        //        {
        //            status = servicD.GetCheckDetail(Id, IP, out ErrD).fldStatus;
        //        }
        //        else if (Type == 3)/*سفته*/
        //        {
        //            status = servicD.GetSafteDetail(Id, IP, out ErrD).fldStatus;
        //        }
        //        else if (Type == 4)/*برات*/
        //        {
        //            status = servicD.GetBaratDetail(Id, IP, out ErrD).fldStatus;
        //        }

        //        if (status == 5)
        //        {
        //            MsgTitle = "خطا";
        //            Msg = "سند مورد نظر قبلا عودت داده شده است.";
        //            Er = 1;
        //        }
        //        else if (status == 2)
        //        {
        //            MsgTitle = "خطا";
        //            Msg = "سند مورد نظر وصول شده و امکان عودت آن وجود ندارد.";
        //            Er = 1;
        //        }
        //        else
        //        {
        //            MsgTitle = "عملیات موفق";
        //            Msg = servicD.UpdateStatusSanad(Type, Id, 5, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrD);
        //            if (ErrD.ErrorType)
        //            {
        //                MsgTitle = "خطا";
        //                Msg = ErrD.ErrorMsg;
        //                Er = 1;
        //            }
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

        public ActionResult Save(List<WCF_Daramad.OBJ_SelectDataForTaghsit> DocumentsArray, int fldReplyTaghsitId,int FiscalYearId,List<WCF_Daramad.OBJ_SelectDataForTaghsit> DelArray)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; var Er = 0; string NotSuccessSerial = "";
            NewFMS.Models.RasaNewFMSEntities mo = new NewFMS.Models.RasaNewFMSEntities();
            try
            {
                if (DelArray != null)
                {
                    foreach (var item in DelArray)
                    {
                        switch (item.fldTypeSanad)
                        {
                            case 1:
                            case 5:
                                {
                                    Msg = servicD.DeleteNaghdi_Talab(item.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                                    if (ErrD.ErrorType)
                                    {
                                        Er = 1;
                                        if (item.fldTypeSanad == 1)
                                        {
                                            NotSuccessSerial = NotSuccessSerial + "سند نقدی: " + ErrD.ErrorMsg + ";";
                                        }
                                        else if (item.fldTypeSanad == 5)
                                        {
                                            NotSuccessSerial = NotSuccessSerial + "سند بدهکاری: " + ErrD.ErrorMsg + ";";
                                        }
                                        else if (item.fldTypeSanad == 7)
                                        {
                                            NotSuccessSerial = NotSuccessSerial + "سند بستانکاری: " + ErrD.ErrorMsg + ";";
                                        }
                                    }
                                }
                                break;
                            case 2:
                            case 6:
                                {
                                    servicD.DeleteCheck(item.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                                    if (ErrD.ErrorType)
                                    {
                                        Er = 1;
                                        NotSuccessSerial = NotSuccessSerial + "چک با شماره " + item.fldShomareSanad + ": " + ErrD.ErrorMsg + ";";
                                    }
                                    else
                                        mo.spr_tblCheck_SmsDelete(item.fldId, Convert.ToInt32(Session["UserId"]));
                                }
                                break;
                            case 3:
                                {
                                    servicD.DeleteSafte(item.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                                    if (ErrD.ErrorType)
                                    {
                                        Er = 1;
                                        NotSuccessSerial = NotSuccessSerial + "سفته با شماره " + item.fldShomareSanad + ": " + ErrD.ErrorMsg + ";";
                                    }
                                }
                                break;
                            case 4:
                                {
                                    servicD.DeleteBarat(item.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                                    if (ErrD.ErrorType)
                                    {
                                        Er = 1;
                                        NotSuccessSerial = NotSuccessSerial + "برات با شماره " + item.fldShomareSanad + ": " + ErrD.ErrorMsg + ";";
                                    }
                                }
                                break;
                        }
                    }
                }
                foreach (var item in DocumentsArray)
                {
                    if(item.fldDesc==null){
                            item.fldDesc="";
                        }
                    if (item.fldId == 0)/*ذخیره*/
                    {
                        if (item.fldTypeSanad == 1 || item.fldTypeSanad == 5 || item.fldTypeSanad == 7)
                        {
                            byte type = 1;
                            if (item.fldTypeSanad == 5)
                            {
                                type = 2;
                            }
                            else if (item.fldTypeSanad == 7)
                            {
                                type =3;
                            }
                            int? ShomareHesabIdOrgan = null;
                            if (item.fldTypeSanad == 1)
                            {
                                ShomareHesabIdOrgan = item.fldShomareHesabIdOrgan;
                            }
                            Msg = servicD.InsertNaghdi_Talab(item.fldMablaghSanad, fldReplyTaghsitId, type, ShomareHesabIdOrgan, item.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                            if (ErrD.ErrorType)
                            {
                                Er=1;
                                if (item.fldTypeSanad == 1)
                                {
                                    NotSuccessSerial = NotSuccessSerial + "سند نقدی: " + ErrD.ErrorMsg+ ";";
                                }
                                else if (item.fldTypeSanad == 5)
                                {
                                    NotSuccessSerial = NotSuccessSerial + "سند بدهکاری: " + ErrD.ErrorMsg + ";";
                                }
                                else if (item.fldTypeSanad == 7)
                                {
                                    NotSuccessSerial = NotSuccessSerial + "سند بستانکاری: " + ErrD.ErrorMsg + ";";
                                }
                            }
                        }
                        else if (item.fldTypeSanad == 2 || item.fldTypeSanad == 6)
                        {
                            var type=false;
                            if(item.fldTypeSanad==6){
                                type=true;
                            }
                            int Checkid = 0;
                            Msg = servicD.InsertCheck(item.fldShomareHesabId, item.fldShomareSanad, fldReplyTaghsitId, item.fldTarikhSarResid,
                                item.fldMablaghSanad, item.fldStatus, type, item.fldShomareHesabIdOrgan, item.fldDesc, 
                                Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP,
                                out Checkid, out ErrD);
                            if (ErrD.ErrorType)
                            {
                                Er = 1;
                                NotSuccessSerial = NotSuccessSerial + "چک با شماره " + item.fldShomareSanad + ": " + ErrD.ErrorMsg + ";";
                            }
                            else
                            {
                                mo.spr_tblCheck_SmsInsert(Checkid, 0, Convert.ToInt32(Session["UserId"]), IP);

                                var company = servicD.GetCompanyWithFilter("fldTitle", "Rasa", 0, IP, out ErrD).FirstOrDefault();
                                if (company != null)//ارسال به رسا
                                {
                                    DataSet.DataSet1TableAdapters.sp_SelectCheckForKhazaneTableAdapter m = new DataSet.DataSet1TableAdapters.sp_SelectCheckForKhazaneTableAdapter();
                                    DataSet.DataSet1.sp_SelectCheckForKhazaneDataTable mm = new DataSet.DataSet1.sp_SelectCheckForKhazaneDataTable();
                                    m.Fill(mm, Convert.ToInt32(Session["OrganId"]), Checkid);
                                    RasaKhazane.CheckDetail[] CDetial = new RasaKhazane.CheckDetail[mm.Rows.Count];
                                    RasaKhazane.Checks check = new RasaKhazane.Checks();
                                    RasaKhazane.KhazaneService rasa = new RasaKhazane.KhazaneService();
                                    if (mm.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < mm.Rows.Count; i++)
                                        {
                                            var code = servicD.GetCodhayeDaramdWithFilter("fldId", mm.Rows[i]["Idcode"].ToString(), 0, Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrD).FirstOrDefault();
                                            var code_hesab = servicD.GetShomareHesabCodeDaramadWithFilter("fldCodeDaramadId", code.fldId.ToString(), "", FiscalYearId, 0, IP, out ErrD).FirstOrDefault();
                                            var hesab = servicD.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId", code_hesab.fldId.ToString(), 0, IP, out ErrD).FirstOrDefault();
                                            if (hesab != null)
                                            {
                                                CDetial[i] =
                                                    new RasaKhazane.CheckDetail
                                                    {

                                                        CodeHesab = hesab.fldHesabId.ToString(),
                                                        Mablagh = Convert.ToInt64(mm.Rows[i]["Mablagh"])

                                                    };
                                            }
                                        }
                                        var ch = servicD.GetCheckWithFilter("fldid", Checkid.ToString(), 0, IP, out ErrD).FirstOrDefault();
                                        var replyTaghsit = servicD.GetReplyTaghsitDetail(ch.fldReplyTaghsitId, IP, out ErrD);
                                        var status = servicD.GetStatusTaghsit_TakhfifDetail(replyTaghsit.fldStatusId, IP, out ErrD);
                                        var request = servicD.GetRequestTaghsit_TakhfifDetail(status.fldRequestId, IP, out ErrD);
                                        var elamAvarez = servicD.GetElamAvarezDetail(request.fldElamAvarezId, "1", IP, out ErrD);                                        
                                        var hesabNum = servic.GetShomareHesabeOmoomiWithFilter("fldid", ch.fldShomareHesabIdOrgan.ToString(), "", "", 0, IP, out Err).FirstOrDefault();
                                        var bank = servic.GetBankWithFilter("fldid", hesabNum.fldBankId.ToString(), 0, IP, out Err).FirstOrDefault();
                                        var hesabShakhs = servic.GetShomareHesabeOmoomiWithFilter("fldid", ch.fldShomareHesabId.ToString(), "", "", 0, IP, out Err).FirstOrDefault();
                                        var ashkhas = servic.GetAshkhasWithFilter("fldid", hesabShakhs.fldAshkhasId.ToString(), "", 0, IP, out Err).FirstOrDefault();
                                        
                                        if (ashkhas.NoeShakhs == "حقیقی")
                                        {
                                            var employe = servic.GetEmployeeWithFilter("fldid", ashkhas.fldHaghighiId.ToString(), 0,
                                                Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), out Err).FirstOrDefault();
                                            check.Name = employe.fldName;
                                            check.Family = employe.fldFamily;
                                            check.CodeMeli = employe.fldCodemeli;
                                            check.TypeShakhs = Convert.ToBoolean(employe.fldTypeShakhs);
                                        }
                                        else
                                        {
                                            var hoghoghi = servic.GetAshkhaseHoghoghiWithFilter("fldid", ashkhas.fldHoghoghiId.ToString(), 0, IP, out Err).FirstOrDefault();
                                            check.Name = hoghoghi.fldName;
                                            check.TypeShakhs = Convert.ToBoolean(hoghoghi.fldTypeShakhs);
                                            check.CodeMeli = hoghoghi.fldShenaseMelli;
                                            check.Family = "";
                                        }
                                        check.checkDetail = CDetial;
                                        check.Babat = elamAvarez.SharhDesc + " پرونده(کدشناسایی) " + elamAvarez.fldId;
                                        check.Bank = hesabNum.fldBankName;
                                        check.CheckNum = ch.fldShomareSanad;                                         
                                        check.CodeShobe = hesabNum.fldCodeSHobe.ToString();                                        
                                        check.HesabDaramadi = hesabNum.fldShomareHesab;
                                        check.Mablagh = item.fldMablaghSanad;                                        
                                        check.ShenaseBank = bank.fldInfinitiveBank;
                                        check.Shobe = hesabNum.nameShobe;
                                        check.ShobeAddress = "";
                                        check.TarikhVosol = ch.fldTarikhSarResid;
                                        string res = rasa.ReciveCheck(company.fldUserNameService, company.fldPassService, check);
                                        var result = res.Split('|');
                                        if (result[1] != "-1")
                                        {
                                            servicD.UpdateSendToMali_Check(Checkid, true, IP, out ErrD);
                                            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\a.txt", "ersal check: " + Checkid + "\n");
                                        }
                                    }
                                    
                                }
                            }
                        }
                        else if (item.fldTypeSanad == 3)
                        {
                            Msg = servicD.InsertSafte(item.fldTarikhSarResid, fldReplyTaghsitId, item.fldShomareSanad, item.fldMablaghSanad, item.fldStatus, item.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                            if (ErrD.ErrorType)
                            {
                                Er = 1;
                                NotSuccessSerial = NotSuccessSerial + "سفته با شماره " + item.fldShomareSanad + ": " + ErrD.ErrorMsg + ";";
                            }
                        }
                        else if (item.fldTypeSanad == 4)
                        {
                            Msg = servicD.InsertBarat(item.fldTarikhSarResid, fldReplyTaghsitId, item.fldShomareSanad, item.fldMablaghSanad, item.fldStatus, item.fldBaratDarId, item.fldMakanPardakht, item.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                            if (ErrD.ErrorType)
                            {
                                Er = 1;
                                NotSuccessSerial = NotSuccessSerial + "برات با شماره " + item.fldShomareSanad + ": " + ErrD.ErrorMsg + ";";
                            }
                        }
                    }
                    else/*ویرایش*/
                    {
                        if (item.fldTypeSanad == 1 || item.fldTypeSanad == 5 || item.fldTypeSanad == 7)
                        {
                            byte type = 1;
                            if (item.fldTypeSanad == 5)
                            {
                                type = 2;
                            }
                            else if (item.fldTypeSanad == 7)
                            {
                                type = 3;
                            }
                            int? ShomareHesabIdOrgan = null;
                            if (item.fldTypeSanad == 1)
                            {
                                ShomareHesabIdOrgan = item.fldShomareHesabIdOrgan;
                            }
                            Msg = servicD.UpdateNaghdi_Talab(item.fldId, item.fldMablaghSanad, fldReplyTaghsitId, type, ShomareHesabIdOrgan, item.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                            if (ErrD.ErrorType)
                            {
                                Er = 1;
                                if (item.fldTypeSanad == 1)
                                {
                                    NotSuccessSerial = NotSuccessSerial + "سند نقدی: " + ErrD.ErrorMsg + ";";
                                }
                                else if (item.fldTypeSanad == 5)
                                {
                                    NotSuccessSerial = NotSuccessSerial + "سند بدهکاری: " + ErrD.ErrorMsg + ";";
                                }
                                else if (item.fldTypeSanad == 7)
                                {
                                    NotSuccessSerial = NotSuccessSerial + "سند بستانکاری: " + ErrD.ErrorMsg + ";";
                                }
                            }
                        }
                        else if (item.fldTypeSanad == 2 || item.fldTypeSanad == 6)
                        {
                            var type = false;
                            if (item.fldTypeSanad == 6)
                            {
                                type = true;
                            }
                            Msg = servicD.UpdateCheck(item.fldId, item.fldShomareHesabId, item.fldShomareSanad, fldReplyTaghsitId, item.fldTarikhSarResid, item.fldMablaghSanad, item.fldStatus, type, item.fldShomareHesabIdOrgan, item.fldDateStatus, item.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                            if (ErrD.ErrorType)
                            {
                                Er = 1;
                                NotSuccessSerial = NotSuccessSerial + "چک با شماره " + item.fldShomareSanad + ": " + ErrD.ErrorMsg + ";";
                            }
                        }
                        else if (item.fldTypeSanad == 3)
                        {
                            Msg = servicD.UpdateSafte(item.fldId, item.fldTarikhSarResid, fldReplyTaghsitId, item.fldShomareSanad, item.fldMablaghSanad, item.fldStatus, item.fldDateStatus, item.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                            if (ErrD.ErrorType)
                            {
                                Er = 1;
                                NotSuccessSerial = NotSuccessSerial + "سفته با شماره " + item.fldShomareSanad + ": " + ErrD.ErrorMsg + ";";
                            }
                        }
                        else if (item.fldTypeSanad == 4)
                        {
                            Msg = servicD.UpdateBarat(item.fldId, item.fldTarikhSarResid, fldReplyTaghsitId, item.fldShomareSanad, item.fldMablaghSanad, item.fldStatus, item.fldBaratDarId, item.fldMakanPardakht, item.fldDateStatus, item.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                            if (ErrD.ErrorType)
                            {
                                Er = 1;
                                NotSuccessSerial = NotSuccessSerial + "برات با شماره " + item.fldShomareSanad + ": " + ErrD.ErrorMsg + ";";
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                NotSuccessSerial = NotSuccessSerial,
                Er = Er,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SodorFish(int ElameAvarezId, long Mablagh, int Id, int OrganId, byte State, int FishId, int ShomareHesabId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; var Er = 0; string MsgTitle = ""; string Desc = "پیش پرداخت تقسیط/نقدی"; int sal = 0, mah = 0; int id = 0;
            try
            {
                var q=servicD.GetNaghdi_TalabWithFilter("", "", 0, IP, out ErrD).ToList();
                var tanzimat = servicD.GetTanzimateDaramadWithFilter("fldOrganId", OrganId.ToString(),FiscalYearId, 1, IP, out ErrD).FirstOrDefault();
                if (State == 1)
                {
                    foreach (var item in q)
                    {
                        if (item.fldFishId != null)
                        {
                            var fish = servicD.GetSodoorFishDetail(Convert.ToInt32(item.fldFishId), IP, out ErrD);
                            if (fish.fldStatusId != "2")
                            {
                                Desc = "پرداخت نقدی بابت تقسیط عودت اقساط";
                                break;
                            }
                        }
                    }

                    id = servicD.InsertSodoorFishForNaghdi_Talab(Convert.ToInt32(Session["OrganId"]), Mablagh, ElameAvarezId, Id,ShomareHesabId, Desc, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        return Json(new
                        {
                            MsgTitle = "خطا",
                            Msg = ErrD.ErrorMsg,
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var organ = servic.GetOrganizationDetail(OrganId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                    if (organ.fldCodAnformatic != null)
                    {
                        var Fish = tanzimat.fldShorooshenaseGhabz + id.ToString();
                        sal = Fish.Length - 2;
                        if (Fish.Length > 8)
                        {
                            string s = Fish.Substring(8).PadRight(2, '0');
                            Fish = Fish.Substring(0, 8);
                            mah = Convert.ToInt32(s);
                        }
                        ghabz gh = new ghabz(Convert.ToInt32(Fish), Convert.ToInt32(organ.fldCodAnformatic), Convert.ToInt32(organ.fldCodKhedmat)
                            , Mablagh, sal, mah);
                        var ShGhabz = gh.ShGhabz;
                        var ShPardakht = gh.ShPardakht;
                        var BarcodeText = gh.BarcodeText;
                        Msg = servicD.UpdateShenaseGhabz_Pardakht(id, ShGhabz, ShPardakht, BarcodeText, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrD);
                        if (ErrD.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = ErrD.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    Msg = "صدور فیش با موفقیت انجام شد.";
                    MsgTitle = "عملیات موفق";
                }
                else
                {
                    var c = servicD.GetPardakhtFishWithFilter("fldFishId", FishId.ToString(), 0, IP, out ErrD).FirstOrDefault();
                    if (c != null)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = "فیش موردنظر پرداخت شده و امکان ابطال آن وجود ندارد.",
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    Msg = servicD.InsertEbtal(FishId, null, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = ErrD.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }

                    Msg = "ابطال فیش با موفقیت انجام شد.";
                    MsgTitle = "عملیات موفق";
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
                FishId = id
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DelInstalments(int ElameAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "خطا";
                Er = 1;
                var c = servicD.GetCheckTaghsit(ElameAvarezId, IP, out ErrD).FirstOrDefault();
                Msg = "برای اعلام عوارض مورد نظر ";
                if (c.fish == 1)
                    Msg =Msg+ "فیش پرداخت شده، ";
                else if(c.fish==2)
                    Msg = Msg+"فیش صادر شده، ";
                if (c.Barat == 1)
                    Msg = Msg+"برات وصول شده، ";
                if (c.check == 1)
                    Msg =Msg+ "چک وصول شده، ";
                else if (c.check == 2)
                   Msg = Msg + "سند، ";
                if (c.Safte == 1)
                    Msg = Msg+"سفته وصول شده، ";

                Msg = Msg + "وجود دارد و قادر به حذف آن نمی باشید.";

                if (c.fish == 0 && c.Barat == 0 && c.check == 0 && c.Safte == 0)
                    Msg = "";

                if (Msg == "")
                {
                    Er = 0;
                    Msg = servicD.DeleteTaghsit(ElameAvarezId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    MsgTitle = "حذف موفق";
                    if (ErrD.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
