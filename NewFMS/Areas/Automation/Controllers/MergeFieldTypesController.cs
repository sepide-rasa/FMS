using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Automation.Controllers
{
    public class MergeFieldTypesController : Controller
    {
        // GET: Automation/MergeFieldTypes
        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();
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
        public ActionResult Save(WCF_Automation.OBJ_MergeFieldTypes MergeFieldTypes)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Models.AutomationEntities p = new Models.AutomationEntities();
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (MergeFieldTypes.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = "ذخیره با موفقیت انجام شد.";
                        p.spr_tblMergeFieldTypesInsert(MergeFieldTypes.fldFaName, MergeFieldTypes.fldEnName, Convert.ToInt32(Session["OrganId"]),Convert.ToInt32(Session["UserId"]), true);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = "ویرایش با موفقیت انجام شد.";
                    p.spr_tblMergeFieldTypesUpdate(MergeFieldTypes.fldId, MergeFieldTypes.fldFaName, MergeFieldTypes.fldEnName, Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), true);

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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Models.AutomationEntities p = new Models.AutomationEntities();
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                p.spr_tblMergeFieldTypesDelete(id, Convert.ToInt32(Session["UserId"]));
                Msg = "حذف با موفقیت انجام شد.";

                MsgTitle = "حذف موفق";
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Models.AutomationEntities p = new Models.AutomationEntities();
            var q = p.spr_tblMergeFieldTypesSelect("fldId",Id.ToString(), Convert.ToInt32(Session["OrganId"]),0).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldEnName = q.fldEnName,
                fldFaName = q.fldFaName
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Models.AutomationEntities p = new Models.AutomationEntities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.spr_tblMergeFieldTypesSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.spr_tblMergeFieldTypesSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldEnName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEnName";
                            break;
                        case "fldFaName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFaName";
                            break;
                    }
                    if (data != null)
                        data1 = p.spr_tblMergeFieldTypesSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100).ToList();
                    else
                        data = p.spr_tblMergeFieldTypesSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.spr_tblMergeFieldTypesSelect("", "", Convert.ToInt32(Session["OrganId"]), 100).ToList();
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

            List<Models.spr_tblMergeFieldTypesSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}