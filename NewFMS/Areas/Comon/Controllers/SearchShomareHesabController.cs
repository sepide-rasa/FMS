using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Comon.Controllers
{
    public class SearchShomareHesabController : Controller
    {
        WCF_Common.CommonService service = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        WCF_Chek.ChekService cservic = new WCF_Chek.ChekService();
        WCF_Chek.ClsError cErr = new WCF_Chek.ClsError();
        //
        // GET: /Comon/SearchShomareHesab/

        public ActionResult Index(int State, string AshkhasId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.AshkhasId = AshkhasId;
            return PartialView;
        }
        public ActionResult IndexCheck(int State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            return PartialView;
        }
        public ActionResult IndexHaveCodeDaramad(int State)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Logon", "Account");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            return PartialView;
        }
        public ActionResult IndexShomareHesab_Organ(int State)//شدهlogin شماره حساب های مربوط به سازمان با توجه به موقعیت  
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Logon", "Account");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            return PartialView;
        }
        public ActionResult AddNewShomareHesab(int AshkhasId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.AshkhasId = AshkhasId;
            return PartialView;
        }
        public ActionResult GetBank()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            return Json(service.GetBankWithFilter("", "", 0, IP, out Err).Select(c => new { fldID = c.fldId, fldName = c.fldBankName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShobe(int ID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Shobe = service.GetSHobeWithFilter("fldBankId", ID.ToString(), 0, IP, out Err);
            return Json(Shobe.Select(p1 => new { fldID = p1.fldId, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters, string AshkhasId, int BankId, string OlguChekId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            int olguBank = 0;
            if (OlguChekId != "")
                olguBank = cservic.GetOlgoCheckDetail(Convert.ToInt32(OlguChekId),Convert.ToInt32(Session["OrganId"]), IP, out cErr).fldIdBank;

            List<WCF_Common.OBJ_ShomareHesabeOmoomi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_ShomareHesabeOmoomi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            if (BankId != 0)
                                field = "fldId_AshkhasId";
                            else if (olguBank != 0)
                            {
                                field = "fldId_BankId";
                                BankId = olguBank;
                            }
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHesab";
                            if (BankId != 0)
                                field = "fldShomareHesab_AshkhasId";
                            else if (olguBank != 0)
                            {
                                field = "fldShomareHesab_BankId";
                                BankId = olguBank;
                            }
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            if (BankId != 0)
                                field = "fldBankName_AshkhasId";
                            else if (olguBank != 0)
                            {
                                field = "fldBankName_BankId";
                                BankId = olguBank;
                            }
                            break;
                        case "nameShobe":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldnameShobe";
                            if (BankId != 0)
                                field = "fldnameShobe_AshkhasId";
                            else if (olguBank != 0)
                            {
                                field = "fldnameShobe_BankId";
                                BankId = olguBank;
                            }
                            break;
                        case "NameAshkhas":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameAshkhas";
                            if (BankId != 0)
                                field = "fldNameAshkhas_AshkhasId";
                            else if (olguBank != 0)
                            {
                                field = "fldNameAshkhas_BankId";
                                BankId = olguBank;
                            }
                            break;
                    }
                    if (data != null)
                        data1 = service.GetShomareHesabeOmoomiWithFilter(field, searchtext, AshkhasId, BankId.ToString(), 100, IP, out Err).ToList();
                    else
                        data = service.GetShomareHesabeOmoomiWithFilter(field, searchtext, AshkhasId, BankId.ToString(), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                string field = "";
                if (BankId != 0)
                    field = "fldBank_Ashkhas";
                else if (olguBank != 0)
                {
                    field = "BankId";
                    BankId = olguBank;
                }
                data = service.GetShomareHesabeOmoomiWithFilter(field, "", AshkhasId, BankId.ToString(), 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_ShomareHesabeOmoomi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadHaveCodeDaramad(StoreRequestParameters parameters)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_ShomareHesabeOmoomi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_ShomareHesabeOmoomi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId_HaveCodeDaramad";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHesab_HaveCodeDaramad";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName_HaveCodeDaramad";
                            break;
                        case "nameShobe":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldnameShobe_HaveCodeDaramad";
                            break;
                        case "NameAshkhas":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameAshkhas_HaveCodeDaramad";
                            break;
                    }
                    if (data != null)
                        data1 = service.GetShomareHesabeOmoomiWithFilter(field, searchtext, Session["OrganId"].ToString(), "", 100, IP, out Err).ToList();
                    else
                        data = service.GetShomareHesabeOmoomiWithFilter(field, searchtext, Session["OrganId"].ToString(), "", 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = service.GetShomareHesabeOmoomiWithFilter("HaveCodeDaramad", "", Session["OrganId"].ToString(), "", 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_ShomareHesabeOmoomi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadShomareHesab_Organ(StoreRequestParameters parameters)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_ShomareHesabeOmoomi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_ShomareHesabeOmoomi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldOrganId_fldId";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOrganId_fldShomareHesab";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOrganId_BankName";
                            break;
                        case "nameShobe":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOrganId_nameShobe";
                            break;
                        case "NameAshkhas":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOrganId_NameAshkhas";
                            break;
                    }
                    if (data != null)
                        data1 = service.GetShomareHesabeOmoomiWithFilter(field, searchtext, "", "", 100, IP, out Err).ToList();
                    else
                        data = service.GetShomareHesabeOmoomiWithFilter(field, searchtext, "", "", 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = service.GetShomareHesabeOmoomiWithFilter("fldOrganId", "", Session["OrganId"].ToString(), "", 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_ShomareHesabeOmoomi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadAll(StoreRequestParameters parameters)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_ShomareHesabeOmoomi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_ShomareHesabeOmoomi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHesab";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                        case "nameShobe":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldnameShobe";
                            break;
                        case "NameAshkhas":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameAshkhas";
                            break;
                    }
                    if (data != null)
                        data1 = service.GetShomareHesabeOmoomiWithFilter(field, searchtext, "", "", 100, IP, out Err).ToList();
                    else
                        data = service.GetShomareHesabeOmoomiWithFilter(field, searchtext, "", "", 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = service.GetShomareHesabeOmoomiWithFilter("", "", "", "", 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_ShomareHesabeOmoomi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
