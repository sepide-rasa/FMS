using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Ext.Net.MVC;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class ShowAhkamController : Controller
    {
        //
        // GET: /Personeli/ShowAhkam/
        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
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
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_PersonalHokm> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_PersonalHokm> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldNoeEstekhdamName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeEstekhdamName";
                            break;
                        case "fldTarikhEjra":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhEjra";
                            break;
                        case "fldTarikhSodoor":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhSodoor";
                            break;
                        case "fldShomareHokm":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHokm";
                            break;
                        case "fldStatusHokmName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStatusHokmName";
                            break;
                        case "fldStatusTaaholName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStatusTaaholName";
                            break;
                        case "fldTypehokm":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypehokm";
                            break;
                        case "fldSumItem":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "Personal_SumItem";
                            break;
                        case "fldPrs_PersonalInfoId":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPrs_PersonalInfoId";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_Personali";
                            break;
                        case "fldNameEmployee":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameEmployee";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetPersonalHokmWithFilter(field, searchtext, "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), 0, IP, out Err).ToList();
                    else
                        data = servic.GetPersonalHokmWithFilter(field, searchtext, "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), 0, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPersonalHokmWithFilter("", "", "", Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_PersonalHokm> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
