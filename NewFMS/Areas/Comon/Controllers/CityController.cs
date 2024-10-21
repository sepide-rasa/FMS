using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Comon.Controllers
{
    public class CityController : Controller
    {
        //
        // GET: /Comon/City/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
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
        /// باز شدن فرم جهت ذخیره اطلاعات جدید و ویرایش اطلاعات موجود
        /// </summary>
        /// <param name="id">کد جدول شهرها</param>
        
        public ActionResult New(int id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }
        /// <summary>
        /// تمامی اطلاعات جدول استان در لیست بازشو در فرم شهرها نمایش داده میشود.
        /// </summary>
     
        public ActionResult GetState()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetStateWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }
        /// <summary>
        ///. از این متد برای  ذخیره اطلاعات جدید یا اطلاعات ویرایش شده استفاده می شود 
        /// </summary>
        ///     <example>     
        ///       <code>
        ///       if(City.Id==0)
        ///          {
        ///              //Insert
        ///          }
        ///       else
        ///          {
        ///              //Update
        ///          }
        ///      </code>
        ///     </example>
        /// <param name="City">. که شامل تمامی فیلد های جدول شهرها به همراه مقادیر آنها میشود object</param>
        ///<returns> . موفقیت آمیز باشد پیغام ذخیره موفق یا ویرایش موفق را نمایش میدهد Update و Insert در صورتی که</returns>
        public ActionResult Save(WCF_Common.OBJ_City City)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (City.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertCity(City.fldName, City.fldStateId,City.fldLatitude,City.fldLongitude, City.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateCity(City.fldId, City.fldName, City.fldStateId,City.fldLatitude,City.fldLongitude, City.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                }
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er=1 ;
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
                Err=Er

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
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteCity(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetCityDetail(Id,  IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldStateId = q.fldStateId.ToString(),
                fldStateName = q.fldStateName,
                fldName = q.fldName,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///. فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول شهرها و امکان سرچ کردن بر اساس   
        /// </summary>

        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters, string StateId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_City> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_City> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldStateName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStateName";
                            break;
                    }
                    if (data != null)
                    {
                        if (StateId == null)
                            data1 = servic.GetCityWithFilter(field, searchtext, 100, IP, out Err).ToList();
                        else
                            data1 = servic.GetCityWithFilter(field, searchtext, 0, IP, out Err).Where(l=>l.fldStateId==Convert.ToInt32(StateId)).ToList();
                    }
                    else
                    {
                        if (StateId == null)
                            data = servic.GetCityWithFilter(field, searchtext, 100, IP, out Err).ToList();
                        else
                            data = servic.GetCityWithFilter(field, searchtext, 0, IP, out Err).Where(l => l.fldStateId == Convert.ToInt32(StateId)).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                if (StateId == null)
                    data = servic.GetCityWithFilter("", "", 100,IP, out Err).ToList();
                else
                    data = servic.GetCityWithFilter("", "", 0, IP, out Err).Where(l => l.fldStateId == Convert.ToInt32(StateId)).ToList();
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

            List<WCF_Common.OBJ_City> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
