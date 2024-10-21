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
    public class SearchUserController : Controller
    {
        //
        // GET: /Comon/SearchUser/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();
        public ActionResult Index(int Organ,int State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            if (State == 2)
                PartialView.ViewBag.Organ = Session["OrganId"].ToString();
            else
                PartialView.ViewBag.Organ = Organ;
            PartialView.ViewBag.State = State;
            return PartialView;
        }

        public ActionResult Read(int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            /*var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);*/

            List<WCF_Common.OBJ_User> data = null;
            data = servic.GetUserWithFilter("fldOrganId", OrganId.ToString(), "",0, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

            /*if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_User> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId_Search";
                            break;
                        case "fldNameEmployee":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameEmployee";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;
                        case "fldActive_DeactiveName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldActive_DeactiveName";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetUserWithFilter(field,searchtext,  100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                    else
                        data = servic.GetUserWithFilter(field, searchtext, 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetUserWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
            }*/

            //var fc = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            //FilterConditions fc = parameters.GridFilters;

            //-- start filtering ------------------------------------------------------------
            //if (fc != null)
            //{
            //    foreach (var condition in fc.Conditions)
            //    {
            //        string field = condition.FilterProperty.Name;
            //        var value = (Newtonsoft.Json.Linq.JValue)condition.ValueProperty.Value;

            //        data.RemoveAll(
            //            item =>
            //            {
            //                object oValue = item.GetType().GetProperty(field).GetValue(item, null);
            //                return !oValue.ToString().Contains(value.ToString());
            //            }
            //        );
            //    }
            //}
            //-- end filtering ------------------------------------------------------------

            //-- start paging ------------------------------------------------------------
            //int limit = parameters.Limit;

            //if ((parameters.Start + parameters.Limit) > data.Count)
            //{
            //    limit = data.Count - parameters.Start;
            //}

            //List<WCF_Common.OBJ_User> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            //return this.Store(rangeData, data.Count);
        }
    }
}
