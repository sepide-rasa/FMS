using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
namespace NewFMS.Areas.Accounting.Controllers
{
    public class SearchDescDocController : Controller
    {
        //
        // GET: /Accounting/SearchDescDoc/
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
        public ActionResult Index(int State, int rowIdx)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" }); 
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.rowIdx = rowIdx;
            result.ViewBag.State = State;
            return result;
        }

        public ActionResult GetCountParameters(string DescDoc)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q=servic.GetParamDocumentDesc(DescDoc, IP, out Err).ToList();
            return Json(new
            {
                Count = q.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Parameters(string DescDoc, byte State, int rowIdx)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.DescDoc = DescDoc;
            result.ViewBag.State = State;
            result.ViewBag.rowIdx = rowIdx;
            return result;
        }

        public ActionResult ReadParameter(StoreRequestParameters parameters, string DescDoc)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Accounting.OBJ_ParamDocumentDesc> data = null;
            data = servic.GetParamDocumentDesc(DescDoc, IP, out Err).ToList();
            return this.Store(data);
        }

        public ActionResult Read(StoreRequestParameters parameters,byte State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Accounting.OBJ_DocumentDesc> data = null;
            if (State == 2)
            {
                State = 0;
            }
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Accounting.OBJ_DocumentDesc> data1 = null;
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
                        case "fldDocDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDocDesc";
                            break;
                    }
                    if (data != null)
                    {
                        data1 = servic.GetDocumentDescWithFilter(field, searchtext, 100, IP, out Err).Where(l=>l.fldFlag==Convert.ToBoolean(State)).ToList();
                    }
                    else
                    {
                        data = servic.GetDocumentDescWithFilter(field, searchtext, 100, IP, out Err).Where(l => l.fldFlag == Convert.ToBoolean(State)).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetDocumentDescWithFilter("", "", 100, IP, out Err).Where(l => l.fldFlag == Convert.ToBoolean(State)).ToList();
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

            List<WCF_Accounting.OBJ_DocumentDesc> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
