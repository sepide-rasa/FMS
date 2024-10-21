using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Chek.Controllers
{
    public class TaeidAghsatChekAmaniController : Controller
    {
        //
        // GET: /Chek/TaeidAghsatChekAmani/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_Chek.ChekService servicChek = new WCF_Chek.ChekService();
        WCF_Chek.ClsError ErrChek = new WCF_Chek.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.ChekVarede();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult StatusAghsat(string Id, string CheckHayeVarede,string State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            if (State == "2")
            {
                var q = servicChek.GetAghsatCheckAmaniDetail(Convert.ToInt32(Id),Convert.ToInt32(Session["OrganId"]), IP, out ErrChek);
                PartialView.ViewBag.fldVaziat = q.fldVaziat.ToString();
                PartialView.ViewBag.fldTarikhVaziat = q.fldTarikhVaziat;
                PartialView.ViewBag.Id = Id;
            }
            else if(State == "1")
            {
                var m = servicChek.GetCheckHayeVaredeDetail(Convert.ToInt32(CheckHayeVarede),Convert.ToInt32(Session["OrganId"]), IP, out ErrChek);
                PartialView.ViewBag.fldVaziat = m.fldvaziat.ToString();
                PartialView.ViewBag.fldTarikhVaziat = m.fldTarikhVaziat;
            }
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.CheckHayeVarede = CheckHayeVarede;
            return PartialView;
        }
        public ActionResult ReloadAghsat(int ChekVaredeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Chek.OBJ_AghsatCheckAmani> data = null;
            data = servicChek.GetAghsatCheckAmaniWithFilter("fldIdCheckHayeVarede", ChekVaredeId.ToString(),Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrChek).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(string fldId, int CheckHayeVarede, string fldTarikhVaziat, byte fldVaziat)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                //ذخیره
                if (fldId == "")
                {
                    MsgTitle = "ذخیره موفق";
                    Msg = servicChek.InsertChekStatus(null, CheckHayeVarede, null, fldVaziat, fldTarikhVaziat, "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out ErrChek);
                }
                else
                {
                    MsgTitle = "ذخیره موفق";
                    Msg = servicChek.InsertChekStatus(null, CheckHayeVarede,Convert.ToInt32(fldId), fldVaziat, fldTarikhVaziat, "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out ErrChek);

                }


                if (ErrChek.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrChek.ErrorMsg;
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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadChekVarede(StoreRequestParameters parameters)
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
                        data1 = servicChek.GetCheckHayeVaredeWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).ToList();
                    else
                        data = servicChek.GetCheckHayeVaredeWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicChek.GetCheckHayeVaredeWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).ToList();
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
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Chek.OBJ_AghsatCheckAmani> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Chek.OBJ_AghsatCheckAmani> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldIdCheckHayeVarede":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldIdCheckHayeVarede";
                            break;
                        case "fldMablagh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablagh";
                            break;
                        case "fldTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikh";
                            break;
                        case "fldTarikhVaziat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhVaziat";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                        case "fldNobat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNobat";
                            break;
                    }
                    if (data != null)
                        data1 = servicChek.GetAghsatCheckAmaniWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).ToList();
                    else
                        data = servicChek.GetAghsatCheckAmaniWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicChek.GetAghsatCheckAmaniWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrChek).ToList();
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

            List<WCF_Chek.OBJ_AghsatCheckAmani> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
