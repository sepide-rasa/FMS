using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Ext.Net.MVC;
using System.IO;
using System.Data;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Collections;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class SavabegheSanavateKHedmatController : Controller
    {
        //
        // GET: /Personeli/SavabegheSanavateKHedmat/

        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();

        WCF_Common_Pay.Comon_PayService servic_Com_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_Com_Pay = new WCF_Common_Pay.ClsError();

        WCF_PayRoll.PayRolService service_Pay = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError Err_Pay = new WCF_PayRoll.ClsError();
        /// <summary>
        /// فرم اصلی مربوط به شهر ها
        /// </summary>
        /// <returns></returns>

        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            return new Ext.Net.MVC.PartialViewResult();

        }
        public ActionResult IndexNew(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.SavabegheSanavateKHedmat();
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
        /// <summary>
        /// باز شدن فرم جهت ذخیره اطلاعات جدید و ویرایش اطلاعات موجود
        /// </summary>
        /// <param name="id">کد جدول شهرها</param>

        public ActionResult New(int id, string containerId, int PersonalId, bool Jensiyat)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var Gender = "0";
            if (Jensiyat == true)
                Gender = "1";
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.Gender = Gender;
            return PartialView;
        }
        /// <summary>
        /// تمامی اطلاعات جدول استان در لیست بازشو در فرم شهرها نمایش داده میشود.
        /// </summary>


        public ActionResult Save(WCF_Personeli.OBJ_SavabegheSanavateKHedmat savabegh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                if (savabegh.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertSavabegheSanavateKHedmat(savabegh.fldPersonalId, savabegh.fldNoeSabeghe, savabegh.fldAzTarikh, savabegh.fldTaTarikh, savabegh.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateSavabegheSanavateKHedmat(savabegh.fldId, savabegh.fldPersonalId, savabegh.fldNoeSabeghe, savabegh.fldAzTarikh, savabegh.fldTaTarikh, savabegh.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()),Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
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
                Err = Er

            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// .اطلاعات مربوط به یک سطر از جدول شهرها را براساس کد داده شده پاک میکند
        /// </summary>
        /// <param name="id">کد جدول شهرها</param>
        /// <returns>. درصورتیکه حذف با موفقیت انجام شود پیغام حذف موفق نمایش داده میشود</returns>
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteSavabegheSanavateKHedmat(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        /// <summary>
        ///. از این متد برای قسمت ویرایش فرم استفاده میشود.زمانی که فرم در حالت ویرایش باشد اطلاعات مربوط به آن قسمت را نشان میدهد
        /// </summary>
        /// <param name="Id">کد مربوط به سطر در حال ویرایش</param>
        /// <returns>اطلاعات مربوط به یک سطر از جدول شهرها را بر میگرداند</returns>
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSavabegheSanavateKHedmatDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
           var p = service_Pay.GetPayPersonalInfoDetail(q.fldPersonalId,Convert.ToInt32(Session["OrganId"]), IP, out Err_Pay);

            var NoeSabeghe = "0";
            if (q.fldNoeSabeghe == true)
                NoeSabeghe = "1";
            return Json(new
            {
                fldName = p.fldName_Father,
                fldNationalCode = p.fldCodemeli,
                fldShomarePersoneli = p.fldSh_Personali,
                fldId = q.fldId,
                fldNoeSabeghe = NoeSabeghe,
                fldPersonalId=q.fldPersonalId,
                fldAzTarikh = q.fldAzTarikh,
                fldTaTarikh = q.fldTaTarikh,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailsHeader(int Id)
        {

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var IsKargar = 0;
            var q = servic.GetPersoneliInfoDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            var H = servic_Com_Pay.GetMaxPersonalEstekhdamTypeWithFilter("", q.fldId, "", IP, out Err_Com_Pay).FirstOrDefault();
            if (H.fldNoeEstekhdamId == 1)
                IsKargar = 1;
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldNameEmployee,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                IsKargar = IsKargar,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///. فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول شهرها و امکان سرچ کردن بر اساس   
        /// </summary>

        public ActionResult ReadAshkhas(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_PersonalInfo> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_PersonalInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName_FamilyEmployee":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName_FamilyEmployee";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_Personali";
                            break;


                    }
                    if (data != null)
                        data1 = servic.GetPersoneliInfoWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetPersoneliInfoWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPersoneliInfoWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_PersonalInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Rload(string PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_SavabegheSanavateKHedmat> data = null;

            data = servic.GetSavabegheSanavateKHedmatWithFilter("fldPersonalId", PersonalId,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }      
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_SavabegheSanavateKHedmat> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_SavabegheSanavateKHedmat> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldNoeSabegheName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeSabegheName";
                            break;
                        case "fldAzTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAzTarikh";
                            break;
                        case "fldTaTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTaTarikh";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetSavabegheSanavateKHedmatWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetSavabegheSanavateKHedmatWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetSavabegheSanavateKHedmatWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_SavabegheSanavateKHedmat> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }


    }
}
