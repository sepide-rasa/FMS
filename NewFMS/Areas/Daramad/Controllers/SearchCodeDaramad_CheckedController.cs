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
    public class SearchCodeDaramad_CheckedController : Controller
    {
        //
        // GET: /Daramad/SearchCodeDaramad_Checked/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        public ActionResult Index(string State,int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.FiscalYearId = FiscalYearId;
            return PartialView;
        }

        public ActionResult ReadTreeDaramadSearch(StoreRequestParameters parameters, int FiscalYearId)
        {
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
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldShomareHesab";
                            break;
                        case "fldShorooshenaseGhabz":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "LastNode_fldShorooshenaseGhabz";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext, Session["OrganId"].ToString(),FiscalYearId, 0, IP, out Err).ToList();
                    else
                        data = servic.GetShomareHesabCodeDaramadWithFilter(field, searchtext,Session["OrganId"].ToString(),FiscalYearId, 0, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetShomareHesabCodeDaramadWithFilter("LastNode_Organ", Session["OrganId"].ToString(), "",FiscalYearId, 0, IP, out Err).ToList();
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
    }
}
