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
    public class ItemsEstekhdamController : Controller
    {
        //
        // GET: /Personeli/ItemsEstekhdam/
        WCF_Common_Pay.Comon_PayService servic = new WCF_Common_Pay.Comon_PayService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common_Pay.ClsError Err = new WCF_Common_Pay.ClsError();


        public ActionResult IndexNew(string containerId)
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
        public ActionResult Save(WCF_Common_Pay.OBJ_Items_Estekhdam item)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (item.fldDesc == null)
                    item.fldDesc = "";
                if (item.fldId != 0)
                {
                    //ویرایش
                    servic.UpdateItems_Estekhdam(item.fldId, item.fldTitle, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";
                }
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }

            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetItems_EstekhdamWithFilter("fldId", Id.ToString(), 0, 0, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldTypeEstekhdamTitle = q.fldTypeEstekhdamTitle,
                fldTitleItemsHoghughi = q.fldTitleItemsHoghughi,
                fldHasZaribstring = q.fldHasZaribstring,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common_Pay.OBJ_Items_Estekhdam> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common_Pay.OBJ_Items_Estekhdam> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";

                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                        case "fldTypeEstekhdamTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeEstekhdamTitle";
                            break;
                        case "fldTitleItemsHoghughi":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitleItemsHoghughi";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetItems_EstekhdamWithFilter(field, searchtext, 0, 100, IP, out Err).ToList();
                    else
                        data = servic.GetItems_EstekhdamWithFilter(field, searchtext, 0, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetItems_EstekhdamWithFilter("", "", 0, 100, IP, out Err).ToList();
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

            List<WCF_Common_Pay.OBJ_Items_Estekhdam> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
