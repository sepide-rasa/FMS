using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using FastMember;

namespace NewFMS.Areas.Accounting.Controllers
{
    public class SettingController : Controller
    {
        // GET: Accounting/Setting
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
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
        public ActionResult AddValue(int SettingId)
        {
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.SettingId = SettingId;
            return PartialView;
        }
        public ActionResult ReadComboboxValuesAccounting(StoreRequestParameters parameters, int SettingId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Common.OBJ_GeneralSetting_ComboBox> data = null;
            data = servic_com.GetGeneralSetting_ComboBoxWithFilter("fldGeneralSettingId", SettingId.ToString(), 100, IP, out Err_com).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }
            List<WCF_Common.OBJ_GeneralSetting_ComboBox> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Save(WCF_Common.OBJ_GeneralSetting GeneralSetting, List<NewFMS.Models.GeneralSetting_Combobox> ComboBox,
            WCF_Common.OBJ_GeneralSetting_Value Value)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable { TableName = "acc.tblGeneralSetting_ComboBox", Columns = { "fldId", "fldTtile", "fldValue" } };
                if (ComboBox != null)
                {
                    using (var reader = FastMember.ObjectReader.Create(ComboBox))
                    {
                        dt.Load(reader);
                    }
                }
                if (GeneralSetting.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic_com.Insert_GeneralSetting(GeneralSetting.fldName, GeneralSetting.fldValue, 4, dt, GeneralSetting.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err_com);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic_com.Update_GeneralSetting(GeneralSetting.fldId, GeneralSetting.fldName, GeneralSetting.fldValue, 4,dt, GeneralSetting.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err_com);

                }
                if (Err_com.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err_com.ErrorMsg;
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
        public ActionResult Delete(byte id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic_com.Delete_GeneralSetting(id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err_com);
                if (Err_com.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err_com.ErrorMsg;
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
            var q = servic_com.GetGeneralSettingDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err_com);
            var q1 = servic_com.GetGeneralSetting_ComboBoxWithFilter("fldGeneralSettingId", Id.ToString(), 0, IP, out Err_com).ToList();
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName,
                fldValue = q.fldValue,
                fldDesc = q.fldDesc,
                comboboxValues = q1
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Common.OBJ_GeneralSetting> data = null;
            data = servic_com.GetGeneralSettingWithFilter("fldModuleId", "4", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err_com).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Common.OBJ_GeneralSetting> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}