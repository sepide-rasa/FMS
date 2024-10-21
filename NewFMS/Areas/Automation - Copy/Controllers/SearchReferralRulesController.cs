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
using NewFMS.Models;

namespace NewFMS.Areas.Automation.Controllers
{
    public class SearchReferralRulesController : Controller
    {
        //
        // GET: /Automation/SearchReferralRules/
        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();

        public ActionResult Index(int State, string CommId,int SearchType)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.SearchType = SearchType;//0=All 1=haghighi 2=hoghughi
            PartialView.ViewBag.CommId = CommId;
            return PartialView;
        }

        public ActionResult Read(StoreRequestParameters parameters, int CommId, int SearchType)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            Models.AutomationEntities m = new Models.AutomationEntities();
            List<Models.spr_tblReferralRulesSelectRecivers> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.spr_tblReferralRulesSelectRecivers> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldReceiverComisionName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldReceiverComisionName";
                            break;
                    }
                    if (data != null)
                        data1 = m.spr_tblReferralRulesSelectRecivers(field, searchtext, 100, CommId).ToList();
                    else
                        data = m.spr_tblReferralRulesSelectRecivers(field, searchtext, 100, CommId).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.spr_tblReferralRulesSelectRecivers("", "", 100, CommId).ToList();
            }

            if (SearchType > 0)
                data = data.Where(l => l.fldType == SearchType).ToList();

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

            List<Models.spr_tblReferralRulesSelectRecivers> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
