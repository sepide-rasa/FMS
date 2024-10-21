using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class SearchShomareHesabCodeDaramadController : Controller
    {
        //
        // GET: /Daramad/SearchShomareHesabCodeDaramad/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common.CommonService servicCom = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCom = new WCF_Common.ClsError();
        public ActionResult Index(int State, int id, string DaramadTitle, int ParamId, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.id = id;
            PartialView.ViewBag.DaramadTitle = DaramadTitle;
            PartialView.ViewBag.ParamId = ParamId;
            PartialView.ViewBag.FiscalYearId = FiscalYearId;
            return PartialView;
        }
        public ActionResult HelpCopyKoli()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpCopyParametr()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCom.GetOrganizationWithFilter("fldTreeOrgan", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out ErrCom).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult CopyKoli(int MabdaId,int MaghsadId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "ذخیره موفق";
                Msg = servic.CopyCodeDaramd("Koli", MabdaId, MaghsadId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        public ActionResult CopyParametr(int MabdaId, int MaghsadId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "ذخیره موفق";
                Msg = servic.CopyCodeDaramd("Parametr", MabdaId, MaghsadId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        public ActionResult Read(StoreRequestParameters parameters, int? OrganId, int? id, int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldDaramadTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldDaramadTitle";
                            break;
                        case "fldDaramadCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldDaramadCode";
                            break;
                        case "fldNameVahed":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldNameVahed";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, OrganId.ToString(),FiscalYearId, 100, IP, out Err).Where(l => l.fldId != id).ToList();
                    else
                        data = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, OrganId.ToString(),FiscalYearId, 100, IP, out Err).Where(l => l.fldId != id).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetShomareHesabCodeDaramadWithFilter("LastNode_Organ", OrganId.ToString(), "",FiscalYearId, 100, IP, out Err).Where(l => l.fldId != id).ToList();
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

            List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult NerkhParam(int ParamId, int ShCodeId, string ParamMaghsad)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.Nerkh();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                ViewData = this.ViewData
            };
            result.ViewBag.ParamId = ParamId;
            result.ViewBag.ShCodeId = ShCodeId;
            result.ViewBag.ParamMaghsad = ParamMaghsad;
            return result;
        }
        public ActionResult ReadParamSabet(StoreRequestParameters parameters, int ShomareHesabCodeDaramadId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ParametreSabet> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_ParametreSabet> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldDaramadTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDaramadTitle";
                            break;
                        case "fldNameParametreFa":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameParametreFa";
                            break;
                        case "NoeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "NoeName";
                            break;
                        case "NoeFieldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "NoeFieldName";
                            break;
                        case "VaziyatName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "VaziyatName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetParametreSabetWithFilter(field, searchtext, ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).Where(k => k.fldTypeParametr == false).ToList();
                    else
                        data = servic.GetParametreSabetWithFilter(field, searchtext, ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).Where(k => k.fldTypeParametr == false).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetParametreSabetWithFilter("fldShomareHesabCodeDaramadId", ShomareHesabCodeDaramadId.ToString(), "", 0, IP, out Err).Where(k => k.fldTypeParametr == false).ToList();
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

            List<WCF_Daramad.OBJ_ParametreSabet> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadNerkhParametr(StoreRequestParameters parameters, int ParametreSabetId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ParametreSabet_Nerkh> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_ParametreSabet_Nerkh> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldValue":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldValue";
                            break;
                        case "fldTarikhFaalSazi":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhFaalSazi";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetParametreSabet_NerkhWithFilter(field, searchtext, 0, IP, out Err).Where(k => k.fldParametreSabetId == ParametreSabetId).ToList();
                    else
                        data = servic.GetParametreSabet_NerkhWithFilter(field, searchtext, 0, IP, out Err).Where(k => k.fldParametreSabetId == ParametreSabetId).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetParametreSabet_NerkhWithFilter("fldParametreSabetId", ParametreSabetId.ToString(), 0, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_ParametreSabet_Nerkh> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
