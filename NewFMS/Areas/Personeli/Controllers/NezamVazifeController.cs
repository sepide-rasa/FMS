using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class NezamVazifeController : Controller
    {
        //
        // GET: /Personeli/NezamVazife/

        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
        /// <summary>
        /// صفحه اصلی فرم نظام وظیفه
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            return new Ext.Net.MVC.PartialViewResult();

        }
        /// <summary>
        /// باز شدن فرم جهت ذخیره اطلاعات جدید و ویرایش اطلاعات موجود
        /// </summary>
        /// <param name="id">کد جدول نظام وظیفه</param>
        /// <returns></returns>
        public ActionResult New(int id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
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
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }

        /// <summary>
        /// از این متد برای ذخیره اطلاعات جدید یا اطلاعات ویرایش شده استفاده می شود
        /// </summary>
        /// <example>
        /// <code>
        /// if(NezamVazife.Id==0)
        ///          {
        ///              //Insert
        ///          }
        ///       else
        ///          {
        ///              //Update
        ///          }
        /// </code>
        /// </example>
        /// <param name="NezamVazife">که شامل فیلدهای جدول نظام وظیفه به همراه مقادیر آن ها Object</param>
        ///  <returns>موفقیت آمیز باشد پیغام ذخیره موفق یا ویرایش موفق را نمایش میدهد Update و Insert در صورتی که</returns>
        public ActionResult Save(WCF_Common.OBJ_NezamVazife NezamVazife)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (NezamVazife.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = Cservic.InsertNezamVazife(NezamVazife.fldTitle, NezamVazife.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out CErr);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = Cservic.UpdateNezamVazife(NezamVazife.fldId, NezamVazife.fldTitle, NezamVazife.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out CErr);

                }
                if (CErr.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = CErr.ErrorMsg;
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
        /// <summary>
        /// .برای حذف یک سطر از جدول نظام وظیفه بر اساس کد فرستاده شده استفاده میشود
        /// </summary>
        /// <param name="id">کد جدول نظام وظیفه</param>
        /// <returns></returns>
        public ActionResult Delete(byte id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = Cservic.DeleteNezamVazife(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out CErr);
                if (CErr.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = CErr.ErrorMsg;
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
        /// <summary>
        /// برای دسترسی به اطلاعات جدول نظام وظیفه بر اساس کد فرستاده شده استفاده می شود
        /// </summary>
        /// <param name="Id">کد جدول نظام وظیفه</param>
        /// <returns>.اطلاعات مربوط به یک سطر از جدول انواع استخدام را بر میگرداند</returns>
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetNezamVazifeDetail(Id,  IP, out CErr);
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول نظام وظیفه و امکان سرچ کردن بر اساس      
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_NezamVazife> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_NezamVazife> data1 = null;
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
                        data1 = Cservic.GetNezamVazifeWithFilter(field, searchtext, 100, IP, out CErr).ToList();
                    else
                        data = Cservic.GetNezamVazifeWithFilter(field, searchtext, 100, IP,out CErr).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = Cservic.GetNezamVazifeWithFilter("", "", 100, IP, out CErr).ToList();
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

            List<WCF_Common.OBJ_NezamVazife> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
