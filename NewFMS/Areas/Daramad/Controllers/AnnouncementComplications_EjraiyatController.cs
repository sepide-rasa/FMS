using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Text.RegularExpressions;
using System.Globalization;
using NewFMS.Models;
using System.IO;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class AnnouncementComplications_EjraiyatController : Controller
    {
        //
        // GET: /Daramad/AnnouncementComplications_Ejraiyat/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        WCF_Accounting.AccountingService Accservic = new WCF_Accounting.AccountingService();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();


        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Accounting.ClsError AccErr = new WCF_Accounting.ClsError();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();



        public ActionResult Index(string containerId,int FiscalYearId)
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
            //this.GetCmp<TabPanel>(containerId).SetActiveTab(2);
            //this.GetCmp<TabPanel>(containerId).SetActiveTab(0);
            result.ViewBag.OrganId = Session["OrganId"].ToString();
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult PrintCheck(string AzTarikh, string TaTarikh,int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.AzTarikh = AzTarikh;
            PartialView.ViewBag.TaTarikh = TaTarikh;
            PartialView.ViewBag.FiscalYearId = FiscalYearId;
            return PartialView;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GeneratePDFPrintCheck(string AzTarikh, string TaTarikh,int FiscalYearId)
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
                Setting.Fill(dt.spr_tblTanzimateDaramadSelect, "fldOrganId", Session["OrganId"].ToString(),FiscalYearId, 0);
                RptCheck.Fill(dt.spr_RptCheck, "AzTarikh_TaTarikh", Session["OrganId"].ToString(), AzTarikh, TaTarikh);
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

        public ActionResult StatusEjra(string id, string TypeSanad, int fldElamAvarezId, long fldMablaghSanad, string fldNameSaderkonandeCheck, string fldShomareSanad)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();            
            PartialView.ViewBag.TypeSanad = TypeSanad;
            PartialView.ViewBag.id = id;
            PartialView.ViewBag.fldElamAvarezId = fldElamAvarezId;
            PartialView.ViewBag.fldMablaghSanad = fldMablaghSanad;
            PartialView.ViewBag.fldNameSaderkonandeCheck = fldNameSaderkonandeCheck;
            PartialView.ViewBag.fldShomareSanad = fldShomareSanad;
            var HaveKhazane = servic_Com.GetModule_OrganWithFilter("CheckOrganId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_Com)
                .Where(l => l.fldModuleId == 10).Any();

            PartialView.ViewBag.HaveKhazane = HaveKhazane;
            return PartialView;
      }
        public ActionResult Save(byte TypeSanad, int id, byte Status, string TarikhStatus, bool HaveKhazane)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; var CodingEn_Vosoul = 0; var CodingTarafEn_Vosoul = 0; var CodingEn_nokoul = 0; var CodingTarafEn_nokoul = 0;
            var FiscalYearId = 0; var CodingTarafEn_SarresidNashode = 0; var CodingEn_SarresidNashode = 0;
            try
            {
                if (HaveKhazane==true && Status == 3 && TypeSanad == 2)
                {
                    var FiscalYear = Accservic.GetFiscalYearWithFilter("fldYear", TarikhStatus.Substring(0, 4), Convert.ToInt32(Session["OrganId"]), 1, IP, out AccErr).FirstOrDefault();
                    var q1 = Accservic.GetCoding_DetailsWithFilter("fldItemId", FiscalYear.fldId.ToString(), "27", "", 1, IP, out AccErr).FirstOrDefault();
                    var q2 = Accservic.GetCoding_DetailsWithFilter("fldItemId", FiscalYear.fldId.ToString(), "31", "", 1, IP, out AccErr).FirstOrDefault();
                    var q3 = Accservic.GetCoding_DetailsWithFilter("fldItemId", FiscalYear.fldId.ToString(), "26", "", 1, IP, out AccErr).FirstOrDefault();
                    var q4 = Accservic.GetCoding_DetailsWithFilter("fldItemId", FiscalYear.fldId.ToString(), "30", "", 1, IP, out AccErr).FirstOrDefault();
                    var q5 = Accservic.GetCoding_DetailsWithFilter("fldItemId", FiscalYear.fldId.ToString(), "25", "", 1, IP, out AccErr).FirstOrDefault();
                    var q6 = Accservic.GetCoding_DetailsWithFilter("fldItemId", FiscalYear.fldId.ToString(), "29", "", 1, IP, out AccErr).FirstOrDefault();

                    CodingEn_nokoul = q1.fldId;
                    CodingTarafEn_nokoul = q2.fldId;
                    CodingEn_Vosoul = q3.fldId;
                    CodingTarafEn_Vosoul = q4.fldId;
                    CodingEn_SarresidNashode = q5.fldId;
                    CodingTarafEn_SarresidNashode = q6.fldId;

                    FiscalYearId = FiscalYear.fldId;
                }

                //ذخیره
                Msg = servic.UpdateStatusSanad(TypeSanad, id, Status, TarikhStatus, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                MsgTitle = "ذخیره موفق";
                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
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
                CodingEn_nokoul = CodingEn_nokoul,
                CodingTarafEn_nokoul = CodingTarafEn_nokoul,
                CodingEn_Vosoul = CodingEn_Vosoul,
                CodingTarafEn_Vosoul = CodingTarafEn_Vosoul,
                CodingTarafEn_SarresidNashode=CodingTarafEn_SarresidNashode,
                CodingEn_SarresidNashode=CodingEn_SarresidNashode,
                FiscalYearId = FiscalYearId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FilterDate(string AzTarikh, string TaTarikh, string TypeSanad/*, int FiscalYearId*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Daramad.OBJ_DataForElamAvarez> data = null;
            if (TypeSanad == "1")
                data = servic.GetDataForElamAvarezWithFilter("Ejra", "", AzTarikh, TaTarikh, Convert.ToByte(TypeSanad), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            else if (TypeSanad == "2")
                data = servic.GetDataForElamAvarez_CheckWithFilter("Ejra", "", AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            else if (TypeSanad == "3")
                data = servic.GetDataForElamAvarez_SafteWithFilter("Ejra", "", AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            else if (TypeSanad == "4")
                data = servic.GetDataForElamAvarez_BaratWithFilter("Ejra", "", AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();

            return Json(new { data = data.ToList() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadEjraiyat(StoreRequestParameters parameters, string AzTarikh, string TaTarikh, string TypeSanad/*, int FiscalYearId*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_DataForElamAvarez> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_DataForElamAvarez> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "Ejra_fldId";
                            break;
                        case "fldElamAvarezId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "Ejra_fldElamAvarezId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldName";
                            break;
                        case "fldShenaseMeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldShenaseMeli";
                            break;
                        case "fldFather_Sabt":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldFather_Sabt";
                            break;
                        case "fldShomareSanad":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldShomareSanad";
                            break;
                        case "fldTarikhSarResid":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldTarikhSarResid";
                            break;
                        case "fldStatusName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldStatusName";
                            break;
                        case "fldTypeSanadName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldTypeSanadName";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldBankName";
                            break;
                        case "fldNameShobe":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldNameShobe";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldShomareHesab";
                            break;
                        case "fldNameBaratDar":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldNameBaratDar";
                            break;
                        case "fldMablaghSanad":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldMablaghSanad";
                            break;
                        case "fldDateStatus":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldDateStatus";
                            break;
                        case "fldTarikhAkhz":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Ejra_fldTarikhAkhz";
                            break;
                    }
                    if (data != null)
                    {
                        if (TypeSanad == "1")
                            data1 = servic.GetDataForElamAvarezWithFilter(field, searchtext, AzTarikh, TaTarikh, Convert.ToByte(TypeSanad), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                        else if (TypeSanad == "2")
                            data1 = servic.GetDataForElamAvarez_CheckWithFilter(field, searchtext, AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                        else if (TypeSanad == "3")
                            data1 = servic.GetDataForElamAvarez_SafteWithFilter(field, searchtext, AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                        else if (TypeSanad == "4")
                            data1 = servic.GetDataForElamAvarez_BaratWithFilter(field, searchtext, AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                    }
                    else
                    {
                        if (TypeSanad == "1")
                            data = servic.GetDataForElamAvarezWithFilter(field, searchtext, AzTarikh, TaTarikh, Convert.ToByte(TypeSanad), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                        else if (TypeSanad == "2")
                            data = servic.GetDataForElamAvarez_CheckWithFilter(field, searchtext, AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                        else if (TypeSanad == "3")
                            data = servic.GetDataForElamAvarez_SafteWithFilter(field, searchtext, AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                        else if (TypeSanad == "4")
                            data = servic.GetDataForElamAvarez_BaratWithFilter(field, searchtext, AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                    }

                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                if (TypeSanad == "1")
                    data = servic.GetDataForElamAvarezWithFilter("Ejra", "", AzTarikh, TaTarikh, Convert.ToByte(TypeSanad), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                else if (TypeSanad == "2")
                    data = servic.GetDataForElamAvarez_CheckWithFilter("Ejra", "", AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                else if (TypeSanad == "3")
                    data = servic.GetDataForElamAvarez_SafteWithFilter("Ejra", "", AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                else if (TypeSanad == "4")
                    data = servic.GetDataForElamAvarez_BaratWithFilter("Ejra", "", AzTarikh, TaTarikh, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_DataForElamAvarez> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult GetStatus(string TypeSand)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<Models.Status> groups = new List<Models.Status>();
            Models.Status V = new Models.Status();
            V = new Models.Status();
            if (TypeSand == "2" || TypeSand == "6")
            {
                V.fldId = 1;
                V.fldTitle = "در انتظار وصول";
                groups.Add(V);

                V = new Models.Status();
                V.fldId = 2;
                V.fldTitle = "وصول شده";
                groups.Add(V);

                V = new Models.Status();
                V.fldId = 3;
                V.fldTitle = "برگشت خورده";
                groups.Add(V);

                V = new Models.Status();
                V.fldId = 4;
                V.fldTitle = "حقوقی شده";
                groups.Add(V);

                V = new Models.Status();
                V.fldId = 5;
                V.fldTitle = "عودت داده شده";
                groups.Add(V);
            }
            else if (TypeSand == "3" || TypeSand == "4")
            {
                V.fldId = 1;
                V.fldTitle = "در انتظار وصول";
                groups.Add(V);

                V = new Models.Status();
                V.fldId = 2;
                V.fldTitle = "وصول شده";
                groups.Add(V);

                V = new Models.Status();
                V.fldId = 3;
                V.fldTitle = "نکول شده";
                groups.Add(V);

                V = new Models.Status();
                V.fldId = 4;
                V.fldTitle = "حقوقی شده";
                groups.Add(V);

                V = new Models.Status();
                V.fldId = 5;
                V.fldTitle = "عودت داده شده";
                groups.Add(V);

            }

            return this.Store(groups.ToList());
        }
    }
}
