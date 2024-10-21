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
    public class SearchUserGroup_ModuleOrganController : Controller
    {
        //
        // GET: /Comon/SearchUserGroup_ModuleOrgan/

        WCF_Common.CommonService c = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        public ActionResult Index(int State, int UserGroup)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.UserGroup = UserGroup;
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters, int userGroup)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_UserGroup_ModuleOrgan> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_UserGroup_ModuleOrgan> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldModuleOrganName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldModuleOrganName";
                            break;

                    }
                    if (data != null)
                        data1 = c.GetUserGroup_ModuleOrganWithFilter(field, searchtext, 100, IP, out Err).Where(l=>l.fldUserGroupId==userGroup).ToList();
                    else
                        data = c.GetUserGroup_ModuleOrganWithFilter(field, searchtext, 100, IP, out Err).Where(l => l.fldUserGroupId == userGroup).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = c.GetUserGroup_ModuleOrganWithFilter("fldUserGroupId", userGroup.ToString(), 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_UserGroup_ModuleOrgan> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
