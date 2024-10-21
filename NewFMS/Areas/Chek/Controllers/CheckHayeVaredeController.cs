using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Chek.Controllers
{
    public class CheckHayeVaredeController : Controller
    {
        //
        // GET: /Chek/CheckHayeVarede/
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Chek.ChekService servic = new WCF_Chek.ChekService();
        WCF_Chek.ClsError Err = new WCF_Chek.ClsError();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();

        WCF_Accounting.AccountingService AccService = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError AccErr = new WCF_Accounting.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            var CurrentDate = Cservic.GetDate(IP, out CErr).fldTarikh;
            var FiscalYear = AccService.GetFiscalYearWithFilter("fldYear", CurrentDate.Substring(0, 4), Convert.ToInt32(Session["OrganId"]), 1, IP, out AccErr).FirstOrDefault();
            var q = AccService.GetCoding_DetailsWithFilter("fldItemId", FiscalYear.fldId.ToString(), "25", "", 1, IP, out AccErr).FirstOrDefault();
            var q1 = AccService.GetCoding_DetailsWithFilter("fldItemId", FiscalYear.fldId.ToString(), "29", "", 1, IP, out AccErr).FirstOrDefault();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.CodingEntezami = q.fldId;
            result.ViewBag.CodingTarafEntezami = q1.fldId;
            result.ViewBag.Year = CurrentDate.Substring(0, 4);
            result.ViewBag.CurrentDate = CurrentDate;
            result.ViewBag.FiscalYearId = FiscalYear.fldId;
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetBank()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetBankWithFilter("", "", 0, IP, out CErr).ToList().Select(n => new { fldId = n.fldId, fldBankName = n.fldBankName });
            return this.Store(q);
        }
        public ActionResult GetShobe(string ID)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetSHobeWithFilter("fldBankId", ID, 0, IP, out CErr).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldName });
            return this.Store(q);
        }
        //public ActionResult Save(WCF_Chek.OBJ_CheckHayeVarede ch)
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    string Msg = "", MsgTitle = ""; var Er = 0;
        //    try
        //    {
        //        if (ch.fldId == 0)
        //        {
        //            //ذخیره
        //            MsgTitle = "ذخیره موفق";
        //            Msg = servic.InsertCheckHayeVarede(ch.fldIdShobe, ch.fldMablagh, ch.fldAshkhasId, ch.fldTarikhVosolCheck, ch.fldTarikhDaryaftCheck, ch.fldShenaseKamelCheck, ch.fldBabat, ch.fldNoeeCheck, ch.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
        //        }
        //        else
        //        {
        //            //ویرایش
        //            MsgTitle = "ویرایش موفق";
        //            Msg = servic.UpdateCheckHayeVarede(ch.fldId, ch.fldIdShobe, ch.fldMablagh, ch.fldAshkhasId, ch.fldTarikhVosolCheck, ch.fldTarikhDaryaftCheck, ch.fldShenaseKamelCheck, ch.fldBabat, ch.fldNoeeCheck, ch.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

        //        }
        //        if (Err.ErrorType)
        //        {
        //            MsgTitle = "خطا";
        //            Msg = Err.ErrorMsg;
        //            Er = 1;
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
        //        Err = Er
        //    }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Mablagh_Horof(string Mablagh)
        {
           var Mablagh_H= MyLib.NumberTool.Num2Str(Convert.ToUInt64(Mablagh), 1);
           return Json(new
           {
               Mablagh_H = Mablagh_H
           },JsonRequestBehavior.AllowGet);
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
                Msg = servic.DeleteCheckHayeVarede(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCheckHayeVaredeDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            var noeeChek = "0";
            if (q.fldNoeeCheck)
                noeeChek = "1";
            return Json(new
            {
                fldId = q.fldId,
                fldBabat = q.fldBabat,
                fldIdShobe = q.fldIdShobe.ToString(),
                fldBankId = q.fldBankId.ToString(),
                fldMablagh = q.fldMablagh,
                fldNoeeCheck = noeeChek,
                fldSaderKonandeh = q.fldSaderKonandeh,
                fldShenaseKamelCheck = q.fldShenaseKamelCheck,
                fldTarikhDaryaftCheck = q.fldTarikhDaryaftCheck,
                fldTarikhVosolCheck = q.fldTarikhVosolCheck,
                fldAshkhasId = q.fldAshkhasId,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///. فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول استان و امکان سرچ کردن بر اساس   
        /// </summary>

        public ActionResult Read(StoreRequestParameters parameters,byte Type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Chek.OBJ_CheckHayeVarede> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Chek.OBJ_CheckHayeVarede> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                        case "fldShobeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShobeName";
                            break;
                        case "NoeeCheckName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "NoeeCheckName";
                            break;
                        case "fldSaderKonandeh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSaderKonandeh";
                            break;
                        case "fldTarikhVosolCheck":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhVosolCheck";
                            break;
                        case "fldTarikhDaryaftCheck":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhDaryaftCheck";
                            break;
                        case "fldShenaseKamelCheck":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenaseKamelCheck";
                            break;
                        case "fldBabat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBabat";
                            break;
                    }
                    if (data != null)
                    {
                        if (Type == 2)
                        {
                            data1 = servic.GetCheckHayeVaredeWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                        }
                        else
                        {
                            data1 = servic.GetCheckHayeVaredeWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err)
                                .Where(l => l.fldReceive==Convert.ToBoolean(Type)).ToList();
                        }
                    }
                    else
                    {
                        if (Type == 2)
                        {
                            data = servic.GetCheckHayeVaredeWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                        }
                        else
                        {
                            data = servic.GetCheckHayeVaredeWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err)
                                .Where(l => l.fldReceive == Convert.ToBoolean(Type)).ToList();
                        }
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                if (Type == 2)
                {
                    data = servic.GetCheckHayeVaredeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                else
                {
                    data = servic.GetCheckHayeVaredeWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err)
                        .Where(l => l.fldReceive == Convert.ToBoolean(Type)).ToList();
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

            List<WCF_Chek.OBJ_CheckHayeVarede> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
