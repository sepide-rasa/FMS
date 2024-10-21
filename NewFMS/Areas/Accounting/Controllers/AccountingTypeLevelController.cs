using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO.Compression;

namespace NewFMS.Areas.Accounting.Controllers
{
    public class AccountingTypeLevelController : Controller
    {
        //
        // GET: /Accounting/AccountingTypeLevel/

        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"]; 
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

        public ActionResult GetAccountingType()
        {
            var q = servic.GetAccountingTypeWithFilter("", "", 0, IP, out Err).ToList().Select(l => new { fldName = l.fldName, fldId = l.fldId});
            return this.Store(q);
        }
        public ActionResult Save(List<WCF_Accounting.OBJ_LevelsAccountingType> AccountingTypeLevelArray, int AccountingTypeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                foreach (var item in AccountingTypeLevelArray)
                {
                    if (item.fldId == 0)
                    {//ذخیره

                        MsgTitle = "ذخیره موفق";
                        Msg = servic.InsertLevelsAccountingType(item.fldName, AccountingTypeId,item.fldArghumNum, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    else
                    {//ویرایش

                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdateLevelsAccountingType(item.fldId, item.fldName, AccountingTypeId, item.fldArghumNum, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
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
        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "حذف موفق";
                Msg = servic.DeleteLevelsAccountingType(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Read(StoreRequestParameters parameters, int AccountingTypeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Accounting.OBJ_LevelsAccountingType> data = null;
            data = servic.selectAccountingTypeLevel(AccountingTypeId, IP, out Err).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Accounting.OBJ_LevelsAccountingType> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
