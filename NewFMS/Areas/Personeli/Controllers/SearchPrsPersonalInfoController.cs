using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class SearchPrsPersonalInfoController : Controller
    {
        //
        // GET: /Personeli/SearchPrsPersonalInfo/

        WCF_Personeli.PersoneliService c = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();
        /// <summary>
        /// صفحه اصلی فرم جستجو پرسنل
        /// </summary>
        /// <param name="State">جهت استفاده از یک فرم در حالت های مختلف از این متغیر برای مشخص کردن حالت استفاده میشود</param>
        /// <returns></returns>
        public ActionResult Index(int State,int? OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.OrganId = OrganId != null ? OrganId.ToString() : Session["OrganId"].ToString();
            return PartialView;
        }
        /// <summary>
        ///فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول پرسنل و امکان سرچ کردن بر اساس 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ActionResult Read(StoreRequestParameters parameters, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_PersonalInfo> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_PersonalInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_Personali";
                            break;
                        case "fldNameEmployee":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameEmployee";
                            break;
                        case "fldMadrakTahsiliTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMadrakTahsiliTitle";
                            break;
                        case "fldReshteTahsiliTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldReshteTahsiliTitle";
                            break;

                    }
                    if (data != null)
                        data1 = c.GetPersoneliInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                    else
                        data = c.GetPersoneliInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = c.GetPersoneliInfoWithFilter("", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_PersonalInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
