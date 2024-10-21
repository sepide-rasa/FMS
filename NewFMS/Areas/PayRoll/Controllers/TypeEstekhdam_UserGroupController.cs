using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class TypeEstekhdam_UserGroupController : Controller
    {
        //
        // GET: /PayRoll/TypeEstekhdam_UserGroup/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();

        WCF_Common_Pay.Comon_PayService Comon_PayServic = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError ErrComon_Pay = new WCF_Common_Pay.ClsError();
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
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_TypeEstekhdam_UserGroup> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_TypeEstekhdam_UserGroup> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;

                        case "fldTitleUserGroup":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitleUserGroup";
                            break;

                        case "fldTypeEstekhdamTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeEstekhdamTitle";
                            break;

                        
                    }
                    if (data != null)
                        data1 = PayServic.GetTypeEstekhdam_UserGroupWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrPay).ToList();
                    else
                        data = PayServic.GetTypeEstekhdam_UserGroupWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrPay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = PayServic.GetTypeEstekhdam_UserGroupWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrPay).ToList();
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

            List<WCF_PayRoll.OBJ_TypeEstekhdam_UserGroup> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult GetUserGroup()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetUserGroupWithFilter("ByUserId", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldTitle });
            return this.Store(q);
        }
        public ActionResult New(int Id)
        {//باز شدن تب
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Id = Id;
            return result;
        }
       
        public ActionResult Save(WCF_PayRoll.OBJ_TypeEstekhdam_UserGroup Type_UserGroup)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                //ذخیره
                MsgTitle = "ذخیره موفق";
                Msg = PayServic.InsertTypeEstekhdam_UserGroup(Type_UserGroup.fldTypeEstekhamId, Type_UserGroup.fldUseGroupId, Type_UserGroup.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);


                if (ErrPay.ErrorType)
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
        public ActionResult Delete(int UserGroupId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = PayServic.DeleteTypeEstekhdam_UserGroup(UserGroupId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                if (ErrPay.ErrorType)
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
            int[] TypeEstekhamId = null;
            var q = PayServic.GetTypeEstekhdam_UserGroupWithFilter("fldId", Id.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).FirstOrDefault();
            var check = PayServic.GetTypeEstekhdam_UserGroupWithFilter("fldUserGroupId", q.fldUseGroupId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();
            TypeEstekhamId = new int[check.Count];
            for (int i = 0; i < check.Count; i++)
            {
                TypeEstekhamId[i] = Convert.ToInt32(check[i].fldTypeEstekhamId);
            }
            return Json(new
            {
                fldUserGroupId = q.fldUseGroupId.ToString(),
                TypeEstekhamId = TypeEstekhamId,
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadDataEstekhdam(int UserGroupId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });


            List<WCF_Common_Pay.OBJ_TypeEstekhdam> Data = null;

            int[] checkId = null;

            Data = Comon_PayServic.GetTypeEstekhdamWithFilter("", "", 0, IP, out ErrComon_Pay).ToList();
            var check = PayServic.GetTypeEstekhdam_UserGroupWithFilter("fldUserGroupId", UserGroupId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();
            checkId = new int[check.Count];
            for (int i = 0; i < check.Count; i++)
            {
                checkId[i] = Convert.ToInt32(check[i].fldTypeEstekhamId);
            }
            return new JsonResult()
            {
                Data = new
                {
                    checkId = checkId,
                    Data = Data
                },
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
