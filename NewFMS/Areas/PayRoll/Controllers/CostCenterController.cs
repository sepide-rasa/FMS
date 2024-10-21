using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using System.IO;


namespace NewFMS.Areas.PayRoll.Controllers
{
    public class CostCenterController : Controller
    {
        //
        // GET: /PayRoll/CostCenter/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
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
        public ActionResult GetTypeOfCenter()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetTypeOfCostCentersWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetEmployMent()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetTypeOfEmploymentCenterWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Title = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetReportType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetReportTypeWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { Id = c.fldId, Name = c.fldName });
            return this.Store(q);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(WCF_PayRoll.OBJ_CostCenter Center)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Center.fldDesc == null)
                    Center.fldDesc = "";
                if (Center.fldId == 0)
                {
                    //ذخیره
                    Msg = servic.InsertCostCenter(Center.fldTitle, Center.fldReportTypeId, Center.fldTypeOfCostCenterId, Center.fldEmploymentCenterId, Center.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Center.fldorganid), IP, out Err);
                   MsgTitle = "ذخیره موفق";
                }
                else
                {
                    //ویرایش
                    Msg = servic.UpdateCostCenter(Center.fldId, Center.fldTitle, Center.fldReportTypeId, Center.fldTypeOfCostCenterId, Center.fldEmploymentCenterId, Center.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Center.fldorganid), IP, out Err);
                    MsgTitle = "ویرایش موفق";
                }
                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
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
                Err=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                //var q = servic.GetPersonalInfoSelect("fldCostCenterId", id.ToString(), 1).FirstOrDefault();
                //if (q != null)
                //{
                //    Msg = "آیتم مورد نظر جای دیگری استفاده شده است.";
                //    MsgTitle = "خطا";
                //}
                //else
                //{
                Msg = servic.DeleteCostCenter(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "حذف موفق";
                    if (Err.ErrorType == true)
                    {
                        Msg = Err.ErrorMsg;
                        MsgTitle = "خطا";
                        Er = 1;
                    }
                //}
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
        public ActionResult Details(int Id) 
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCostCenterDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldReportTypeId = q.fldReportTypeId.ToString(),
                fldEmploymentCenterId = q.fldEmploymentCenterId.ToString(),
                fldTypeOfCostCenterId = q.fldTypeOfCostCenterId.ToString(),
                fldorganid = q.fldorganid.ToString(),
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult Read(StoreRequestParameters parameters, string OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_CostCenter> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_CostCenter> data1 = null;
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
                        case "TypecenterTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "TypecenterTitle";
                            break;
                        case "EmploymentName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "EmploymentName";
                            break;
                        case "fldReportTypeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldReportTypeName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetCostCenterWithFilter(field, searchtext, OrganId, 100, IP, out Err).ToList();
                    else
                        data = servic.GetCostCenterWithFilter(field, searchtext, OrganId, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetCostCenterWithFilter("fldOrganId",  OrganId, OrganId, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_CostCenter> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
