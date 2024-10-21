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
    public class InsuranceWorkshopController : Controller
    {
        //
        // GET: /PayRoll/InsuranceWorkshop/
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
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult Save(WCF_PayRoll.OBJ_InsuranceWorkshop Workshop)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var fldWorkShopNum = "";
                if (Workshop.fldDesc == null)
                    Workshop.fldDesc = "";
                if (Workshop.fldAddress == null)
                    Workshop.fldAddress = "";
                if (Workshop.fldWorkShopNum.Length < 10) { fldWorkShopNum = Workshop.fldWorkShopNum.PadLeft('0', 10); }
                else { fldWorkShopNum = Workshop.fldWorkShopNum; }

                if (Workshop.fldId == 0)
                {
                    //var q = servic.GetInsuranceWorkshopWithFilter("fldWorkShopNum", fldWorkShopNum, 1, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).FirstOrDefault();
                    //if (q != null)
                    //{
                    //    Msg = "شماره کارگاه بیمه تکراری است.";
                    //}
                    //else
                    //{
                        //ذخیره
                    Msg = servic.InsertInsuranceWorkshop(Workshop.fldWorkShopName, Workshop.fldEmployerName, fldWorkShopNum, Workshop.fldPersent, Workshop.fldAddress, Workshop.fldPeyman, Workshop.fldOrganId, Workshop.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Workshop.fldOrganId, IP, out Err);
                    MsgTitle = "ذخیره موفق";
                    //}
                }
                else
                {
                    //ویرایش
                    Msg = servic.UpdateInsuranceWorkshop(Workshop.fldId, Workshop.fldWorkShopName, Workshop.fldEmployerName, fldWorkShopNum, Workshop.fldPersent, Workshop.fldAddress, Workshop.fldPeyman, Workshop.fldOrganId, Workshop.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Workshop.fldOrganId, IP, out Err);
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
        public ActionResult Delete(int id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                //var q = servic.GetPersonalInfoWithFilter("fldInsuranceWorkShopId", id.ToString(), 1, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).FirstOrDefault();
                //if (q != null)
                //{
                //    Msg = "آیتم مورد نظر جای دیگری استفاده شده است.";
                //    MsgTitle = "خطا";
                //}
                //else
                //{
                Msg = servic.DeleteInsuranceWorkshop(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err);
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Details(int Id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetInsuranceWorkshopDetail(Id, OrganId, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldAddress = q.fldAddress,
                fldPeyman=q.fldPeyman,
                fldEmployerName = q.fldEmployerName,
                fldPersent = q.fldPersent,
                fldWorkShopName = q.fldWorkShopName,
                fldWorkShopNum = q.fldWorkShopNum,
                fldDesc = q.fldDesc,
                fldOrganId=q.fldOrganId
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Read(StoreRequestParameters parameters, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_InsuranceWorkshop> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_InsuranceWorkshop> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldWorkShopName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldWorkShopName";
                            break;
                        case "fldWorkShopNum":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldWorkShopNum";
                            break;
                        case "fldEmployerName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEmployerName";
                            break;
                        case "fldPersent":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersent";
                            break;
                        case "fldPeyman":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPeyman";
                            break;
                        case "fldAddress":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAddress";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetInsuranceWorkshopWithFilter(field, searchtext, OrganId, 100, IP, out Err).ToList();
                    else
                        data = servic.GetInsuranceWorkshopWithFilter(field, searchtext, OrganId, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetInsuranceWorkshopWithFilter("", "", OrganId, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_InsuranceWorkshop> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
