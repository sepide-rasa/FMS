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
    public class SearchCompanyPostController : Controller
    {
        // GET: Comon/SearchCompanyPost
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        public ActionResult Index(int State)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            return PartialView;
        }
        public ActionResult New(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = Id;
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_CompanyPost> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_CompanyPost> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "id":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldid";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetCompanyPostWithFilter(field, searchtext,0, IP, out Err).ToList();
                    else
                        data = servic.GetCompanyPostWithFilter(field, searchtext, 0, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetCompanyPostWithFilter("", "", 0, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_CompanyPost> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}