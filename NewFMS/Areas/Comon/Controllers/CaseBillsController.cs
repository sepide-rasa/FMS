using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;

namespace NewFMS.Areas.Comon.Controllers
{
    public class CaseBillsController : Controller
    {
        //
        // GET: /Comon/CaseBills/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
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
        public ActionResult GetBillsType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetBillsTypeWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult Save(WCF_Common.OBJ_CaseBills CaseBills)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (CaseBills.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertCaseBills(CaseBills.fldBillsTypeId, CaseBills.fldFileNum, CaseBills.fldCentercoId, CaseBills.fldOrganChartId, CaseBills.fldAshkhasId, CaseBills.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateCaseBills(CaseBills.fldId, CaseBills.fldBillsTypeId, CaseBills.fldFileNum, CaseBills.fldCentercoId, CaseBills.fldOrganChartId, CaseBills.fldAshkhasId, CaseBills.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteCaseBills(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetCaseBillsDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldBillsTypeId = q.fldBillsTypeId.ToString(),
                fldAshkhasId=q.fldAshkhasId,
                fldCentercoId=q.fldCentercoId,
                fldCodeMelli=q.fldCodeMelli,
                fldFileNum=q.fldFileNum,
                fldName_BillsType=q.fldName_BillsType,
                fldName_Organ=q.fldName_Organ,
                fldName_shakhs=q.fldName_shakhs,
                fldNameCenter=q.fldNameCenter,
                fldOrganChartId=q.fldOrganChartId,
                fldTitle_chartOrgan=q.fldTitle_chartOrgan,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_CaseBills> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_CaseBills> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldFileNum":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFileNum";
                            break;
                        case "fldShakhs_Type":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShakhs_Type";
                            break;
                        case "fldName_shakhs":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName_shakhs";
                            break;
                        case "fldCodeMelli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodeMelli";
                            break;
                        case "fldName_Organ":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName_Organ";
                            break;
                        case "fldTitle_chartOrgan":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle_chartOrgan";
                            break;
                        case "fldNameCenter":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameCenter";
                            break;
                    }
                    if (data != null)
                    {
                        data1 = servic.GetCaseBillsWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    }
                    else
                    {
                        data = servic.GetCaseBillsWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetCaseBillsWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_CaseBills> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
