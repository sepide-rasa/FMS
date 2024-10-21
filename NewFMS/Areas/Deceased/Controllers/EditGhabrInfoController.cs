using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Deceased.Controllers
{
    public class EditGhabrInfoController : Controller
    {
        // GET: Deceased/EditGhabrInfo
        WCF_Deceased.DeceasedService servic = new WCF_Deceased.DeceasedService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Deceased.ClsError Err = new WCF_Deceased.ClsError();

        WCF_Common.CommonService servic_C = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_C = new WCF_Common.ClsError();
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
        public ActionResult ShowMap()
        {

            return View();
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Deceased.Models.RasaNewFMSEntities1 m = new Deceased.Models.RasaNewFMSEntities1();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.spr_tblGhabrInfoSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.spr_tblGhabrInfoSelect> data1 = null;
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
                            field = "fldName";
                            break;
                        case "fldFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamily";
                            break;
                        case "fldNameFather":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameFather";
                            break;
                        case "fldBDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBDate";
                            break;
                        case "fldDeathDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDeathDate";
                            break;
                    }
                    if (data != null)
                        data1 = m.spr_tblGhabrInfoSelect(field, searchtext,100).ToList();
                    else
                        data = m.spr_tblGhabrInfoSelect(field, searchtext, 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = m.spr_tblGhabrInfoSelect("", "",  100).ToList();
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

            List<Models.spr_tblGhabrInfoSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Save(Models.spr_tblGhabrInfoSelect h)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Deceased.Models.RasaNewFMSEntities1 m = new Deceased.Models.RasaNewFMSEntities1();
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
               
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg ="ویرایش با موفقیت انجام شد.";
                m.spr_tblGhabrInfoUpdate(h.fldId, h.fldName, h.fldFamily, h.fldNameFather, h.fldBDate, h.fldDeathDate, Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]));

                
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
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Deceased.Models.RasaNewFMSEntities1 m = new Deceased.Models.RasaNewFMSEntities1();
            var q = m.spr_tblGhabrInfoSelect("fldId",Id.ToString(), 0).FirstOrDefault();

           
            return Json(new
            {
                fldId = q.fldId,
                fldBDate = q.fldBDate,
                fldFamily = q.fldFamily,
                fldDeathDate = q.fldDeathDate,
                fldName = q.fldName,
                fldNameFather = q.fldNameFather
            }, JsonRequestBehavior.AllowGet);
        }
    }
}