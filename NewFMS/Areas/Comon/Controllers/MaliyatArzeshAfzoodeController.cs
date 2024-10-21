using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
namespace NewFMS.Areas.Comon.Controllers
{
    public class MaliyatArzeshAfzoodeController : Controller
    {
        //
        // GET: /Comon/MaliyatArzeshAfzoode/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            return new Ext.Net.MVC.PartialViewResult();

        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
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
       

        public ActionResult New(int id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }
        

        public ActionResult Save(WCF_Common.OBJ_MaliyatArzesheAfzoode Maliyat)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Maliyat.fldDesc == null)
                    Maliyat.fldDesc = "";

                if (Maliyat.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertMaliyatArzesheAfzoode(Maliyat.fldFromDate, Maliyat.fldEndDate, Maliyat.fldDarsadeAvarez, Maliyat.fldDarsadeMaliyat, Maliyat.fldDarsadAmuzeshParvaresh, Maliyat.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateMaliyatArzesheAfzoode(Maliyat.fldId, Maliyat.fldFromDate, Maliyat.fldEndDate, Maliyat.fldDarsadeAvarez, Maliyat.fldDarsadeMaliyat, Maliyat.fldDarsadAmuzeshParvaresh, Maliyat.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Msg = servic.DeleteMaliyatArzesheAfzoode(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetMaliyatArzesheAfzoodeDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldFromDate = q.fldFromDate,
                fldEndDate = q.fldEndDate,
                fldDarsadeAvarez = q.fldDarsadeAvarez,
                fldDarsadeMaliyat = q.fldDarsadeMaliyat,
                fldDarsadAmuzeshParvaresh=q.fldDarsadAmuzeshParvaresh,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult Read(StoreRequestParameters parameters)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_MaliyatArzesheAfzoode> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_MaliyatArzesheAfzoode> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldFromDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFromDate";
                            break;
                        case "fldEndDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEndDate";
                            break;
                        case "fldDarsadeAvarez":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDarsadeAvarez";
                            break;
                        case "fldDarsadeMaliyat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDarsadeMaliyat";
                            break;
                        case "fldDarsadAmuzeshParvaresh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDarsadAmuzeshParvaresh";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetMaliyatArzesheAfzoodeWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetMaliyatArzesheAfzoodeWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMaliyatArzesheAfzoodeWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_MaliyatArzesheAfzoode> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
