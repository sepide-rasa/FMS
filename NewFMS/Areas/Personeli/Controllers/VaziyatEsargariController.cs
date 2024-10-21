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
    public class VaziyatEsargariController : Controller
    {
        //
        // GET: /Personeli/VaziyatEsargari/

        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();
        /// <summary>
        /// صفحه اصلی فرم وضعیت ایثارگری
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
        /// <param name="id">کد جدول وضعیت ایثارگری</param>
        /// <returns></returns>
        public ActionResult New(int id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
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
        /// <summary>
        /// از این متد برای ذخیره اطلاعات جدید یا اطلاعات ویرایش شده استفاده می شود
        /// </summary>
        /// <example>
        /// <code>
        /// if(VaziyatEsargari.Id==0)
        ///          {
        ///              //Insert
        ///          }
        ///       else
        ///          {
        ///              //Update
        ///          }
        /// </code>
        /// </example>
        /// <param name="VaziyatEsargari">کد جدول وضعیت ایثارگری</param>
        /// <returns>موفقیت آمیز باشد پیغام ذخیره موفق یا ویرایش موفق را نمایش میدهد Update و Insert در صورتی که</returns>
        public ActionResult Save(WCF_Personeli.OBJ_VaziyatEsargari VaziyatEsargari)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (VaziyatEsargari.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertVaziyatEsargari(VaziyatEsargari.fldTitle, VaziyatEsargari.fldMoafAsBime, VaziyatEsargari.fldMoafAsMaliyat, VaziyatEsargari.fldMoafAsBimeOmr, VaziyatEsargari.fldMoafAsBimeTakmili, VaziyatEsargari.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateVaziyatEsargari(VaziyatEsargari.fldId, VaziyatEsargari.fldTitle, VaziyatEsargari.fldMoafAsBime, VaziyatEsargari.fldMoafAsMaliyat, VaziyatEsargari.fldMoafAsBimeOmr, VaziyatEsargari.fldMoafAsBimeTakmili, VaziyatEsargari.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        /// <summary>
        /// .برای حذف یک سطر از جدول وضعیت ایثارگری بر اساس کد فرستاده شده استفاده میشود
        /// </summary>
        /// <param name="id">کد جدول وضعیت ایثارگری</param>
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
                Msg = servic.DeleteVaziyatEsargari(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        /// .برای دسترسی به اطلاعات جدول وضعیت ایثارگری بر اساس کد فرستاده شده استفاده می شود
        /// </summary>
        /// <param name="Id">کد جدول وضعیت ایثارگری</param>
        /// <returns>.اطلاعات مربوط به یک سطر از جدول وضعیت ایثارگری را بر میگرداند</returns>
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetVaziyatEsargariDetail(Id, IP,out Err);

            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldMoafAsBime=q.fldMoafAsBime,
                fldMoafAsBimeName = q.fldMoafAsBimeName,
                fldMoafAsMaliyat=q.fldMoafAsMaliyat,
                fldMoafAsMaliyatName=q.fldMoafAsMaliyatName,
                fldMoafAsBimeOmr=q.fldMoafAsBimeOmr,
                fldMoafAsBimeOmrName=q.fldMoafAsBimeOmrName,
                fldMoafAsBimeTakmili=q.fldMoafAsBimeTakmili,
                fldMoafAsBimeTakmiliName=q.fldMoafAsBimeTakmiliName,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول وضعیت ایثارگری و امکان سرچ کردن بر اساس 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_VaziyatEsargari> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_VaziyatEsargari> data1 = null;
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
                        data1 = servic.GetVaziyatEsargariWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetVaziyatEsargariWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetVaziyatEsargariWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_VaziyatEsargari> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
