using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Ext.Net.MVC;
using System.IO;
using System.Data;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Collections;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class SearchPersonMohasebeController : Controller
    {
        //
        // GET: /PayRoll/SearchPersonMohasebe/

        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        public ActionResult Index(int State, int Year, int Month, int NobatePardakht, int NoePardakht, int Ezafe_TarilKari, string CostCenter, string AnvaeEstekhdam, string Tarikh,int OrganId)
        {//باز شدن پنجر
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.NobatePardakht = NobatePardakht;
            PartialView.ViewBag.NoePardakht = NoePardakht;
            PartialView.ViewBag.Ezafe_TarilKari = Ezafe_TarilKari;
            PartialView.ViewBag.CostCenter = CostCenter;
            PartialView.ViewBag.AnvaeEstekhdam = AnvaeEstekhdam;
            PartialView.ViewBag.Tarikh = Tarikh;
            PartialView.ViewBag.OrganId = OrganId;
            return PartialView;
        }

       /* public ActionResult EzafeKari_Tatilkari(int State, int Year, int Month, int NobatePardakht)
        {//باز شدن پنجره
            if (Session["UserName"] == null)
                return RedirectToAction("Logon", "Account");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.NobatePardakht = NobatePardakht;
            return PartialView;
        }

       
        public ActionResult Eydi(int State, int Year, int Month, int NobatePardakht)
        {//باز شدن پنجره
            if (Session["UserName"] == null)
                return RedirectToAction("Logon", "Account");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.NobatePardakht = NobatePardakht;
            return PartialView;
        }

        public ActionResult Mamuriyat(int State, int Year, int Month, int NobatePardakht)
        {//باز شدن پنجره
            if (Session["UserName"] == null)
                return RedirectToAction("Logon", "Account");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.NobatePardakht = NobatePardakht;
            return PartialView;
        }

        public ActionResult Morakhasi(int State, int Year, int Month, int NobatePardakht)
        {//باز شدن پنجره
            if (Session["UserName"] == null)
                return RedirectToAction("Logon", "Account");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.NobatePardakht = NobatePardakht;
            return PartialView;
        }*/

        public ActionResult Read(StoreRequestParameters parameters, string Year, string Month, string NobatePardakht, string NoePardakht, string Ezafe_TarilKari, string CostCenter, string AnvaeEstekhdam, string Tarikh, int State, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            byte Ezafe_Taril = 1;
            if (Ezafe_TarilKari == "3")
                Ezafe_Taril = 2;
            string fieldName="Calc";
            if(State!=1)
                fieldName = "DelCalc";
            List<WCF_PayRoll.OBJ_PersonalInfo_Mohasebe> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_PersonalInfo_Mohasebe> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetPersonalInfo_MohasebeWithFilter(/*field,searchtext,*/fieldName, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), Convert.ToByte(NoePardakht), Ezafe_Taril, OrganId, CostCenter, AnvaeEstekhdam, Tarikh, IP, out Err).ToList();
                    else
                        data = servic.GetPersonalInfo_MohasebeWithFilter(fieldName, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), Convert.ToByte(NoePardakht), Ezafe_Taril, OrganId, CostCenter, AnvaeEstekhdam, Tarikh, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPersonalInfo_MohasebeWithFilter(fieldName, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), Convert.ToByte(NoePardakht), Ezafe_Taril, OrganId, CostCenter, AnvaeEstekhdam, Tarikh, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_PersonalInfo_Mohasebe> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

       /* public ActionResult ReadKarkard(StoreRequestParameters parameters, string Year, string Month, string NobatePardakht)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_KarKardeMahane> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_KarKardeMahane> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetKarKardeMahaneWithFilter(field, searchtext,Year,Month,Convert.ToByte(NobatePardakht),0,100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                    else
                        data = servic.GetKarKardeMahaneWithFilter(field, searchtext, Year, Month, Convert.ToByte(NobatePardakht),0,100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetKarKardeMahaneWithFilter("Mohasebe", "", Year, Month, Convert.ToByte(NobatePardakht),0,100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
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

            List<WCF_PayRoll.OBJ_KarKardeMahane> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult ReadEzafeKari_TatilKari(StoreRequestParameters parameters,string state, string Year, string Month, string NobatePardakht)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            byte type = 1;
            if (state == "3")
                type = 2;
            List<WCF_PayRoll.OBJ_EzafeKari_TatilKari> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_EzafeKari_TatilKari> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetEzafeKari_TatilKariWithFilter(field, searchtext, 0, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), type, 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                    else
                        data = servic.GetEzafeKari_TatilKariWithFilter(field, searchtext, 0, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), type, 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetEzafeKari_TatilKariWithFilter("", "", 0, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), type, 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
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

            List<WCF_PayRoll.OBJ_EzafeKari_TatilKari> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult ReadEydi(StoreRequestParameters parameters, string Year, string NobatePardakht)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
           
            List<WCF_PayRoll.OBJ_EtelaatEydi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_EtelaatEydi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetEtelaatEydiWithFilter(field, searchtext, 0, Convert.ToInt16(Year), Convert.ToByte(NobatePardakht),  100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                    else
                        data = servic.GetEtelaatEydiWithFilter(field, searchtext, 0, Convert.ToInt16(Year),  Convert.ToByte(NobatePardakht),  100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetEtelaatEydiWithFilter("", "", 0, Convert.ToInt16(Year),  Convert.ToByte(NobatePardakht),  100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
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

            List<WCF_PayRoll.OBJ_EtelaatEydi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult ReadMamuriyat(StoreRequestParameters parameters,  string Year, string Month, string NobatePardakht)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_Mamuriyat> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_Mamuriyat> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetMamuriyatWithFilter(field, searchtext, 0, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                    else
                        data = servic.GetMamuriyatWithFilter(field, searchtext, 0, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMamuriyatWithFilter("", "", 0, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
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

            List<WCF_PayRoll.OBJ_Mamuriyat> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult ReadMorakhasi(StoreRequestParameters parameters, string Year, string Month, string NobatePardakht)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_Morakhasi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_Morakhasi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetMorakhasiWithFilter(field, searchtext, 0, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                    else
                        data = servic.GetMorakhasiWithFilter(field, searchtext, 0, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMorakhasiWithFilter("", "", 0, Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatePardakht), 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
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

            List<WCF_PayRoll.OBJ_Morakhasi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }*/
    }
}
