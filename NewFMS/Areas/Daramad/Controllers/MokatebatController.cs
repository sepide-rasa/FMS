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
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Net.Security;
using System.Text.RegularExpressions;
using Aspose.Cells;
using System.Data;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class MokatebatController : Controller
    {
        //
        // GET: /Daramad/Mokatebat/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common_Pay.Comon_PayService servic_ComPay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_ComPay = new WCF_Common_Pay.ClsError();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();

        public ActionResult Index(int Id, byte HaveFish)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.Mokatebat();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                ViewData = this.ViewData
            };
            result.ViewBag.ElamAvarezId = Id;
            result.ViewBag.HaveFish = HaveFish;
            return result;
        }
        public ActionResult ReadCodeDaramadAvarez(StoreRequestParameters parameters, string elamAvarez)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez> data1 = null;
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
                        data1 = servic.GetCodhayeDaramadiElamAvarezWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetCodhayeDaramadiElamAvarezWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetCodhayeDaramadiElamAvarezWithFilter("fldElamAvarezId", elamAvarez, 100, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadMokatebat(StoreRequestParameters parameters, int elamAvarez)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_Mokatebat> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_Mokatebat> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldSharheCodeDaramad":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldSharheCodeDaramad";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetMokatebatWithFilter(field, searchtext, 0, IP, out Err).Where(l=>l.fldElamAvarezId==elamAvarez).ToList();
                    else
                        data = servic.GetMokatebatWithFilter(field, searchtext, 0, IP, out Err).Where(l => l.fldElamAvarezId == elamAvarez).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMokatebatWithFilter("fldElamAvarezId", elamAvarez.ToString(), 100, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_Mokatebat> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReloadLetterMinut(int ShomareHesabCodeDaramadId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Daramad.OBJ_LetterMinut> data = null;
            data = servic.GetLetterMinutWithFilter("fldShomareHesabCodeDaramadId", ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).ToList();

            return Json(new { data = data}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadParams(int LetterMinutId, int CodeDaramadAvarezId, int ElamAvarezId, byte HaveFish)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var q = servic.GetElamAvarezDetail(ElamAvarezId, (Session["OrganId"]).ToString(),IP, out Err);
            var q2=servic.GetCodhayeDaramadiElamAvarez(CodeDaramadAvarezId,IP,out Err);
            var q3 = servic_Com.GetAshkhasDetail(q.fldAshakhasID, IP, out Err_Com);

            var FishNum = ""; var Tarikh = ""; var Price = "";
            var ShowLetter=0;
            //if (HaveFish != 0)
            //{
            //var q4 = servic_Com.GetsoDetail(q.fldAshakhasID, IP, out Err_Com);

            //}

            List<NewFMS.Models.ParamGrid> Griddata = new List<NewFMS.Models.ParamGrid>();

            var h = servic.GetLetterMinutDetail(LetterMinutId, IP, out Err);

            if (q.fldStatusKoli == "1")/*تسویه نقدی */
            {
                ShowLetter = 1;
            }
            else
            {
                if (h.fldSodoorBadAzVarizNaghdi)/*قبل از تسویه*/
                {
                    if (h.fldSodoorBadAzTaghsit == true)
                    {
                        if (q.fldStatusKoli == "2")/*تقسیط شده*/
                            ShowLetter = 1;
                    }
                    else
                    {
                        if (q.fldStatusKoli != "1")/*تسویه نقدی */
                        {
                            ShowLetter = 1;
                        }
                    }
                }
            }

            var MatnName = "";
            if (ShowLetter==1)
            {
                MatnName = h.fldDescMinut;
                var s = h.fldDescMinut.Split('[');
                for (byte i = 1; i < s.Length; i++)
                {
                    var z = s[i].Split(']');

                    Models.ParamGrid V = new Models.ParamGrid();
                    var e = servic.GetParametreSabetWithFilter("fldNameParametreEn", z[0], q2.fldShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).FirstOrDefault();
                    if (e != null)
                    {
                        V.ParamId = e.fldId;
                        V.ParamEn = e.fldNameParametreEn;
                        V.ParamName = e.fldNameParametreFa;
                        V.Value = "";
                        Griddata.Add(V);
                    }
                    else
                    {
                        switch (z[0])
                        {
                            case "ENumber":
                                MatnName = MatnName.Replace("[ENumber]", q2.fldTedad.ToString());
                                break;
                            case "EName":
                                MatnName = MatnName.Replace("[EName]", q3.fldName.ToString());
                                break;
                            case "EFamily":
                                MatnName = MatnName.Replace("[EFamily]", q3.fldFamily.ToString());
                                break;
                            case "EMeliCode":
                                MatnName = MatnName.Replace("[EMeliCode]", q3.fldShenase_CodeMeli.ToString());
                                break;
                            case "EIdNum":
                                MatnName = MatnName.Replace("[EIdNum]", q3.fldSh_Shenasname.ToString());
                                break;
                            case "EPost":
                                MatnName = MatnName.Replace("[EPost]", q3.fldCodePosti.ToString());
                                break;
                            case "ETel":
                                MatnName = MatnName.Replace("[ETel]", q3.fldShomareTelephone.ToString());
                                break;
                            case "EAddress":
                                MatnName = MatnName.Replace("[EAddress]", q3.fldAddress.ToString());
                                break;
                            case "EFishNumber":
                                MatnName = MatnName.Replace("[EFishNumber]", FishNum);
                                break;
                            case "ECollectionDate":
                                MatnName = MatnName.Replace("[ECollectionDate]", Tarikh);
                                break;
                            case "EFishPrice":
                                MatnName = MatnName.Replace("[EFishPrice]", q2.fldAsliValue.ToString());
                                // patternlist[k] = (fish.fldAmount).ToString("#,###");//(detailfish.fldAmount * detailfish.fldTedad + detailfish.fldValueAdd - detailfish.fldDiscount).ToString("#,###");
                                break;
                            case "EMaliatPrice":
                                MatnName = MatnName.Replace("[EMaliatPrice]", q2.fldMaliyatValue.ToString());
                                //   patternlist[k] = (fish.fldAmount).ToString("#,###");//(detailfish.fldAmount * detailfish.fldTedad + detailfish.fldValueAdd - detailfish.fldDiscount).ToString("#,###");
                                break;
                            case "EAvarezPrice":
                                MatnName = MatnName.Replace("[EAvarezPrice]", q2.fldAvarezValue.ToString());
                                // patternlist[k] = (fish.fldAmount).ToString("#,###");//(detailfish.fldAmount * detailfish.fldTedad + detailfish.fldValueAdd - detailfish.fldDiscount).ToString("#,###");
                                break;
                        }
                    }
                    if (Err.ErrorType)
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Griddata = Griddata, DescMinut = MatnName, ShowLetter = ShowLetter }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintPreview(string MatnName, int ShomareHesabCodeDaramadId, int CodeDaramadElamAvarez, int LetterMinutId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.MatnName = MatnName;
            PartialView.ViewBag.ShomareHesabCodeDaramadId = ShomareHesabCodeDaramadId;
            PartialView.ViewBag.CodeDaramadElamAvarez = CodeDaramadElamAvarez;
            PartialView.ViewBag.LetterMinutId = LetterMinutId;
            return PartialView;
        }
        public ActionResult Save(string MatnName, int ShomareHesabCodeDaramadId,int CodeDaramadElamAvarez ,int LetterMinutId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; var FileId = 0;
            string LetterNumber = "";
            //bool HaveTanzimKonande=false;
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_tblRooneveshtSelectTableAdapter Roonevesht = new DataSet.DataSet1TableAdapters.spr_tblRooneveshtSelectTableAdapter();
                DataSet.DataSet1TableAdapters.spr_LetterSignersSelectTableAdapter Signers = new DataSet.DataSet1TableAdapters.spr_LetterSignersSelectTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                var User = servic_Com.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(),1,IP, out Err_Com).FirstOrDefault();
                Roonevesht.Fill(dt.spr_tblRooneveshtSelect, "fldShomareHesabCodeDaramadId", ShomareHesabCodeDaramadId.ToString(), 0);
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Signers.Fill(dt.spr_LetterSignersSelect, LetterMinutId, Convert.ToInt32(Session["UserId"]));
                GetDate.Fill(dt_Com.spr_GetDate);

                //var kk = servic.GetLetterMinutDetail(LetterMinutId, IP, out Err);
                //if (kk.fldTanzimkonande)
                //    HaveTanzimKonande = true;

                int MokatebatId = servic.InsertMokatebat(CodeDaramadElamAvarez, "", Session["Username"].ToString(), Session["Password"].ToString(),Convert.ToInt32(Session["OrganId"]), IP, out Err);

                string tarikh=servic_Com.GetDate(IP, out Err_Com).fldTarikh;
                if (Err_Com.ErrorType)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                var NumberFormat = servic.GetFormatShomareNameWithFilter("fldYear", tarikh.Substring(0, 4), 0, IP, out Err).Where(l => l.fldType == false).FirstOrDefault();
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                if (NumberFormat == null)
                {
                    return Json(new
                    {
                        Msg = "فرمت شماره نامه برای سال جاری تعریف نشده است.",
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var Format = NumberFormat.fldFormatShomareName.Split('*');
                    for (int i = 0; i < Format.Count() - 1; i++)
                    {
                        switch (Format[i])
                        {
                            case "shomarande":
                                LetterNumber = servic.InsertShomareNameHa(MokatebatId, null, NumberFormat.fldShomareShoro, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToString();
                                LetterNumber += LetterNumber;
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
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptOlgoName.frx");
                Report.RegisterData(dt_Com, "rasaFMSDaramad");
                Report.RegisterData(dt, "rasaFMSDaramad");
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Matn", MatnName);
                Report.SetParameterValue("LetterNumber", LetterNumber);
                //Report.SetParameterValue("HaveTanzimKonande", HaveTanzimKonande);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
              //  return File(stream.ToArray(), "application/pdf");

                FileId = servic.UpdateMokatebat(MokatebatId,CodeDaramadElamAvarez, stream.ToArray(), ".frx", "", Session["Username"].ToString(), Session["Password"].ToString(),Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Err = Er,
                FileId = FileId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteMokatebat(id, Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult PrintLetter(int FileId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.FileId = FileId;
            return PartialView;
        }
        public ActionResult GeneratePDF(int FileId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var f=servic_Com.GetFileWithFilter("fldId", FileId.ToString(), 0, IP, out Err_Com).FirstOrDefault();

                MemoryStream stream1 = new MemoryStream(f.fldImage);
                return File(f.fldImage, "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
