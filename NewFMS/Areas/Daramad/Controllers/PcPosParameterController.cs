using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
namespace NewFMS.Areas.Daramad.Controllers
{
    public class PcPosParameterController : Controller
    {
        //
        // GET: /Daramad/PcPosParameter/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();
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
        public ActionResult GetPSP()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicD.GetPcPosInfoWithFilter("", "", Session["OrganId"].ToString(), 0, IP, out ErrD).ToList().Select(c => new { fldId = c.fldPspId, fldName = c.fldPspName });
            return this.Store(q);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_PcPosParametr> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_PcPosParametr> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldBankId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldBankId";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                        case "fldFaName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFaName";
                            break;
                        case "fldEnName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEnName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servicD.GetPcPosParametrWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrD).ToList();
                    else
                        data = servicD.GetPcPosParametrWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrD).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicD.GetPcPosParametrWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrD).ToList();
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

            List<WCF_Daramad.OBJ_PcPosParametr> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(WCF_Daramad.OBJ_PcPosParametr PcPosParametr)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            if (PcPosParametr.fldDesc == null)
            {
                PcPosParametr.fldDesc = "";
            }
            try
            {
                if (PcPosParametr.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    Msg = servicD.Insertpcposparametr(PcPosParametr.fldFaName, PcPosParametr.fldEnName, PcPosParametr.fldPspId, PcPosParametr.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = servicD.Updatepcposparametr(PcPosParametr.fldId, PcPosParametr.fldFaName, PcPosParametr.fldEnName, PcPosParametr.fldPspId, PcPosParametr.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);

                }
                if (ErrD.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrD.ErrorMsg;
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
            var q = servicD.GetPcPosParametrDetails(Id,Convert.ToInt32(Session["OrganId"]),IP, out ErrD);
            return Json(new
            {
                fldId = q.fldId,
                fldPspId = q.fldPspId.ToString(),
                fldFaName = q.fldFaName,
                fldEnName=q.fldEnName,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MsgTitle = "حذف موفق";
                Msg = servicD.DeletePcPosParametr(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                if (ErrD.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrD.ErrorMsg;
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
    }
}
