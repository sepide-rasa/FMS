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
    public class AnvaEstekhdamController : Controller
    {
        //
        // GET: /Personeli/AnvaEstekhdam/

        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();

        WCF_Common_Pay.Comon_PayService Common_Payservic = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError ErrC_P = new WCF_Common_Pay.ClsError();
        /// <summary>
        /// صفحه اصلی فرم انواع استخدام
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
        /// <param name="id">کد جدول انواع استخدام</param>
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
        public ActionResult GetAnvaeEstekhdamPattern()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetAnvaEstekhdamWithFilter("fldIsPattern", "", 0, IP, out ErrC_P).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        /// <summary>
        ///.تمامی اطلاعات جدول نوع استخدام را در لیست بازشو نمایش میدهد
        /// </summary>
        /// <returns></returns>
        public ActionResult GetNoeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetTypeEstekhdamWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }


        public ActionResult GetTypeEstekhdamFormul()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetTypeEstekhdam_FormulWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        /// <summary>
        /// از این متد برای ذخیره اطلاعات جدید یا اطلاعات ویرایش شده استفاده می شود 
        /// </summary>
        /// <example>
        /// <code>
        /// if(AnvaEstekhdam.Id==0)
        ///          {
        ///              //Insert
        ///          }
        ///       else
        ///          {
        ///              //Update
        ///          }
        /// </code>
        /// </example>
        /// <param name="AnvaEstekhdam"> که شامل فیلدهای جدول انواع استخدام به همراه مقادیر آن ها Object</param>
        /// <returns>موفقیت آمیز باشد پیغام ذخیره موفق یا ویرایش موفق را نمایش میدهد Update و Insert در صورتی که</returns>
        public ActionResult Save(WCF_Common_Pay.OBJ_AnvaEstekhdam AnvaEstekhdam)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;
            try
            {
                if (AnvaEstekhdam.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = Common_Payservic.InsertAnvaEstekhdam(AnvaEstekhdam.fldTitle, AnvaEstekhdam.fldNoeEstekhdamId,AnvaEstekhdam.fldPatternNoeEstekhdamId,AnvaEstekhdam.fldTypeEstekhdamFormul, AnvaEstekhdam.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrC_P);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = Common_Payservic.UpdateAnvaEstekhdam(AnvaEstekhdam.fldId, AnvaEstekhdam.fldTitle, AnvaEstekhdam.fldNoeEstekhdamId, AnvaEstekhdam.fldPatternNoeEstekhdamId, AnvaEstekhdam.fldTypeEstekhdamFormul, AnvaEstekhdam.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrC_P);

                }
                if (ErrC_P.ErrorType)
                { 
                    MsgTitle = "خطا";
                    Msg = ErrC_P.ErrorMsg;
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
        ///.برای حذف یک سطر از جدول انواع استخدام بر اساس کد فرستاده شده استفاده میشود
        /// </summary>
        /// <param name="id">کد انواع استخدام</param>
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
                Msg = Common_Payservic.DeleteAnvaEstekhdam(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrC_P);
                if (ErrC_P.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrC_P.ErrorMsg;
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
        ///.برای دسترسی به اطلاعات جدول انواع استخدام بر اساس کد فرستاده شده استفاده می شود
        /// </summary>
        /// <param name="Id">کد جدول انواع استخدام</param>
        /// <returns>.اطلاعات مربوط به یک سطر از جدول انواع استخدام را بر میگرداند</returns>
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetAnvaEstekhdamDetail(Id, IP, out ErrC_P);
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldNoeEstekhdamId = q.fldNoeEstekhdamId.ToString(),
                fldTitleNoeEstekhdam = q.fldTitleNoeEstekhdam,
                fldPatternNoeEstekhdamId=q.fldPatternNoeEstekhdamId.ToString(),
                fldTitlePattern=q.fldTitlePattern,
                fldDesc = q.fldDesc,
                fldTypeEstekhdamFormul = q.fldTypeEstekhdamFormul.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول انواع استخدام و امکان سرچ کردن بر اساس     
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
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

            List<WCF_Common_Pay.OBJ_AnvaEstekhdam> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common_Pay.OBJ_AnvaEstekhdam> data1 = null;
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
                        case "fldTitleNoeEstekhdam":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitleNoeEstekhdam";
                            break;
                        case "fldTitlePattern":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitlePattern";
                            break;
                    }
                    if (data != null)
                        data1 = Common_Payservic.GetAnvaEstekhdamWithFilter(field, searchtext, 100, IP, out ErrC_P).ToList();
                    else
                        data = Common_Payservic.GetAnvaEstekhdamWithFilter(field, searchtext, 100, IP, out ErrC_P).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = Common_Payservic.GetAnvaEstekhdamWithFilter("", "", 100, IP, out ErrC_P).ToList();
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

            List<WCF_Common_Pay.OBJ_AnvaEstekhdam> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
