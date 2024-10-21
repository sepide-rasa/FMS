using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class AnvaGroupTashvighiController : Controller
    {
        //
        // GET: /Personeli/AnvaGroupTashvighi/

        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();
        /// <summary>
        /// صفحه اصلی فرم انواع گروه تشویقی
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            return new Ext.Net.MVC.PartialViewResult();

        }
        /// <summary>
        ///جهت ذخیره اطلاعات جدید و ویرایش اطلاعات قبلی new باز شدن پنجره  
        /// </summary>
        /// <param name="id">کد جدول انواع گروه های تشویقی</param>
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
        /// <summary>
        ///از این متد برای ذخیره اطلاعات جدید یا اطلاعات ویرایش شده استفاده می شود  
        /// </summary>
        /// <example>
        /// <code>
        /// if(GroupTashvighi.Id==0)
        ///          {
        ///              //Insert
        ///          }
        ///       else
        ///          {
        ///              //Update
        ///          }
        /// </code>
        /// </example>
        /// <param name="GroupTashvighi">که شامل فیلدهای جدول گروه های تشویقی به همراه مقادیر آن ها Object</param>
        ///<returns>موفقیت آمیز باشد پیغام ذخیره موفق یا ویرایش موفق را نمایش میدهد Update و Insert در صورتی که</returns>
        public ActionResult Save(WCF_Personeli.OBJ_AnvaGroupTashvighi GroupTashvighi)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;
            try
            {
                if (GroupTashvighi.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertAnvaGroupTashvighi(GroupTashvighi.fldTitle, GroupTashvighi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateAnvaGroupTashvighi(GroupTashvighi.fldId, GroupTashvighi.fldTitle, GroupTashvighi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        /// <summary>
        /// برای حذف یک سطر از جدول انواع گروه تشویقی بر اساس کد فرستاده شده استفاده میشود
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteAnvaGroupTashvighi(Convert.ToByte(id), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        /// <summary>
        /// .برای دسترسی به اطلاعات جدول انواع گروه تشویقی بر اساس کد فرستاده شده استفاده می شود
        /// </summary>
        /// <param name="Id">کد جدول انواع گروه تشویقی</param>
        /// <returns>.اطلاعات مربوط به یک سطر از جدول انواع استخدام را بر میگرداند</returns>
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAnvaGroupTashvighiDetail(Convert.ToByte(Id), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldTitle,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        /// <summary>
        /// فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول انواع گروه تشویقی و امکان سرچ کردن بر اساس
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_AnvaGroupTashvighi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_AnvaGroupTashvighi> data1 = null;
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
                        data1 = servic.GetAnvaGroupTashvighiWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetAnvaGroupTashvighiWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetAnvaGroupTashvighiWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_AnvaGroupTashvighi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
