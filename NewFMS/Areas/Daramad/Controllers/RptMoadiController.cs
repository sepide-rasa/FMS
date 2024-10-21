using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using System.IO;
using System.Web.Configuration;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class RptMoadiController : Controller
    {
        //
        // GET: /Daramad/RptMoadi/

        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        public ActionResult Index(string containerId, int FiscalYearId)
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
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult ShowFishWin(int ElamAvarezId, int FiscalYearId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ElamAvarezId = ElamAvarezId;
            PartialView.ViewBag.FiscalYearId = FiscalYearId;
            return PartialView;
        }
        public ActionResult ShowPrint(int fldShakhsId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.fldShakhsId = fldShakhsId;
            return PartialView;
        }
        public ActionResult GeneratePdf(int fldShakhsId)
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
                string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/RptMoadi/GeneratePdf?fldShakhsId=" + fldShakhsId;
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                DataSet.DataSet1TableAdapters.sp_RptAshkhasCheckStatusTableAdapter AshkhasCheck = new DataSet.DataSet1TableAdapters.sp_RptAshkhasCheckStatusTableAdapter();
                DataSet.DataSet1TableAdapters.sp_RptAshkhasFishStatusTableAdapter Ashkhasfish = new DataSet.DataSet1TableAdapters.sp_RptAshkhasFishStatusTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                AshkhasCheck.Fill(dt.sp_RptAshkhasCheckStatus, fldShakhsId);
                Ashkhasfish.Fill(dt.sp_RptAshkhasFishStatus, fldShakhsId);
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaDaramad");
                Report.RegisterData(dt, "rasaDaramad");

                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAshkhasAmalkard.frx");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("barcode", barcode);
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult GenerateXls(int fldShakhsId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return null;
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
                WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
                string barcode = WebConfigurationManager.AppSettings["PrintURL"] + "/Daramad/RptMoadi/GeneratePdf?fldShakhsId=" + fldShakhsId;
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                DataSet.DataSet1TableAdapters.sp_RptAshkhasCheckStatusTableAdapter AshkhasCheck = new DataSet.DataSet1TableAdapters.sp_RptAshkhasCheckStatusTableAdapter();
                DataSet.DataSet1TableAdapters.sp_RptAshkhasFishStatusTableAdapter Ashkhasfish = new DataSet.DataSet1TableAdapters.sp_RptAshkhasFishStatusTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                AshkhasCheck.Fill(dt.sp_RptAshkhasCheckStatus, fldShakhsId);
                Ashkhasfish.Fill(dt.sp_RptAshkhasFishStatus, fldShakhsId);
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaDaramad");
                Report.RegisterData(dt, "rasaDaramad");

                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Daramad\RptAshkhasAmalkard.frx");
             
                Report.Prepare();
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("barcode", barcode);
          

                FastReport.Export.OoXML.Excel2007Export xlsExport = new FastReport.Export.OoXML.Excel2007Export();


                MemoryStream stream = new MemoryStream();

                Report.Prepare();

                Report.Export(xlsExport, stream);
                return File(stream.ToArray(), "xls", "AshkhasAmalkard.xls");
            }
            catch (Exception x)
            {
                return null;
            }
        }
        public ActionResult LoadAllFish(int ElamAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var Data = servic.GetAshkhas_Fish(ElamAvarezId, IP, out Err).ToList();
                return Json(new
                {
                    Data = Data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult ShowTaghsitWin(int ElamAvarezId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.ElamAvarezId = ElamAvarezId;
            return PartialView;
        }
        public ActionResult LoadAllTaghsit(int ElamAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var Data = servic.GetAshkhas_Taghsit(ElamAvarezId, IP, out Err).ToList();
                return Json(new
                {
                    Data = Data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult getReply(int ElamAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var ReplyTaghsit = servic.GetAshkhas_Taghsit(ElamAvarezId, IP, out Err).FirstOrDefault();
                return Json(new
                {
                    ReplyTaghsitId = ReplyTaghsit.fldReplyTaghsitId
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult Read(StoreRequestParameters parameters,int ashkhasID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_Ashkhas_ElamAvarez> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_Ashkhas_ElamAvarez> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "Noe":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Noe";
                            break;
                        case "fldTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikh";
                            break;
                        case "fldMablaghKoli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghKoli";
                            break;
                        case "fldMablaghTakhfif":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghTakhfif";
                            break;
                        case "fldMablaghGHabelPardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghGHabelPardakht";
                            break;
                        case "SharhDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "SharhDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetAshkhas_ElamAvarez(field,searchtext,0, ashkhasID, IP, out Err).ToList();
                    else
                        data = servic.GetAshkhas_ElamAvarez(field, searchtext,0, ashkhasID, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetAshkhas_ElamAvarez("","",0,ashkhasID, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_Ashkhas_ElamAvarez> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadAllTaghsit(StoreRequestParameters parameters, int ElamAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);


            var data = servic.GetAshkhas_Taghsit(ElamAvarezId, IP, out Err).ToList();

                return this.Store(data);
        }
        public ActionResult ReadAllFish(StoreRequestParameters parameters, int ElamAvarezId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);


            var data = servic.GetAshkhas_Fish(ElamAvarezId, IP, out Err).ToList();

            return this.Store(data);
        }
    }
}
