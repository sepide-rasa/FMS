using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Accounting.Controllers
{
    public class SearchParvandeInfoController : Controller
    {
        //
        // GET: /Accounting/SearchParvandeInfo/

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService(); 
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
        public ActionResult Index(int State, byte? FileType, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.FileType = FileType;
            PartialView.ViewBag.Year = Year;
            return PartialView;
        }
        public ActionResult GetFileType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCaseTypeWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult ReadFiles(StoreRequestParameters parameters, byte Type, short Year)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<NewFMS.WCF_Accounting.OBJ_ParvandeInfo> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<NewFMS.WCF_Accounting.OBJ_ParvandeInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;
                        case "fldStartContract":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStartContract";
                            break;
                        case "fldEndContract":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEndContract";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetParvandeInfo(field, searchtext, Type,Convert.ToInt32(Session["OrganId"]),Year, 100, IP, out Err).ToList();
                    else
                        data = servic.GetParvandeInfo(field, searchtext, Type,Convert.ToInt32(Session["OrganId"]),Year, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetParvandeInfo("", "", Type,Convert.ToInt32(Session["OrganId"]),Year, 100, IP, out Err).ToList();
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
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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

            List<NewFMS.WCF_Accounting.OBJ_ParvandeInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
