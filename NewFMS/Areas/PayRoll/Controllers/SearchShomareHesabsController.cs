using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class SearchShomareHesabsController : Controller
    {
        //
        // GET: /PayRoll/SearchShomareHesabs/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        public ActionResult Index(int State, string PersonalId,int? OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.OrganId = OrganId != null ? OrganId.ToString() : Session["OrganId"].ToString();
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters, string PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_ShomareHesabs> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_ShomareHesabs> data1 = null;
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
                            field = "fldName_Personal";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_Personali_Personal";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHesab_Personal";
                            break;
                        case "fldShomareKart":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareKart_Personal";
                            break;
                        case "fldTypeHesabName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeHesabName_Personal";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName_Personal";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetShomareHesabsWithFilter(field, searchtext, PersonalId, "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                    else
                        data = servic.GetShomareHesabsWithFilter(field, searchtext, PersonalId, "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetShomareHesabsWithFilter("fldPersonalId", PersonalId, "", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_ShomareHesabs> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
