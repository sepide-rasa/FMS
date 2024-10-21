using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Automation.Controllers 
{
    public class NumberingFormatController : Controller
    {
        //
        // GET: /Automation/NumberingFormat/

        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();

        WCF_Common.CommonService servic_C = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_C = new WCF_Common.ClsError();

        public ActionResult Index(string containerId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.NumberFormat();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult New(int SecretariatID,int id)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q = servic_C.GetDate(IP, out Err_C);
            string year = q.fldTarikh.Substring(0, 4);
            
            PartialView.ViewBag.SecretariatID = SecretariatID;
            PartialView.ViewBag.id = id;
            PartialView.ViewBag.Year = year;
            return PartialView;
        }
        public ActionResult GetSal()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            var q = servic_C.GetDate(IP, out Err_C);
            int fldsal = Convert.ToInt32(q.fldTarikh.Substring(0, 4)) - 7;
            for (int i = fldsal; i < fldsal + 16; i++)
            {
                Models.CurrentDate CboSal = new Models.CurrentDate();

                CboSal.fldYear = i;
                sh.Add(CboSal);
            }
            return Json(sh, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(WCF_Automation.OBJ_NumberingFormat format)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (format.fldDesc == null)
                    format.fldDesc = "";
                if (format.fldID == 0)
                {
                    //ویرایش
                    Msg = servic.InsertNumberingFormat(format.fldYear, format.fldSecretariatID, format.fldNumberFormat, format.fldStartNumber, format.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ذخیره موفق";
                }
                else
                {
                    //ویرایش
                    Msg = servic.UpdateNumberingFormat(format.fldID, format.fldYear, format.fldSecretariatID, format.fldNumberFormat, format.fldStartNumber, format.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ویرایش موفق";
                }
                if (Err.ErrorType)
                {
                    Er = 1;
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
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
            var q = servic.GetNumberingFormatDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);

            return Json(new
            {
                fldId = q.fldID,
                fldNumberFormat = q.fldNumberFormat,
                fldStartNumber = q.fldStartNumber,
                fldYear = q.fldYear.ToString(),
                fldDesc = q.fldDesc
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
                Msg = servic.DeleteNumberingFormat(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                MsgTitle = "حذف موفق";
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadHeader(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Automation.OBJ_Secretariat_OrganizationUnit> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Automation.OBJ_Secretariat_OrganizationUnit> data1 = null;
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
                    }
                    if (data != null)
                        data1 = servic.GetSecretariat_OrganizationUnitWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetSecretariat_OrganizationUnitWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetSecretariat_OrganizationUnitWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100,  IP, out Err).ToList();
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

            List<WCF_Automation.OBJ_Secretariat_OrganizationUnit> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReloadDetail(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Automation.OBJ_NumberingFormat> data = null;
            //data = servic.GetMasoulin_DetailList(HeaderId, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).ToList();
            data = servic.GetNumberingFormatWithFilter("fldSecretariatID", HeaderId.ToString(), Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
