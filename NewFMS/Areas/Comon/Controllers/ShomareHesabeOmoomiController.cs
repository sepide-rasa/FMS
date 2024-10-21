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
    public class ShomareHesabeOmoomiController : Controller
    {
        //
        // GET: /Comon/ShomareHesabeOmoomi/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        /// <summary>
        /// فرم اصلی مربوط به شماره حساب
        /// </summary>
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
        public ActionResult GetBank()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            return Json(servic.GetBankWithFilter("", "", 0, IP,out Err).Select(c => new { fldID = c.fldId, fldName = c.fldBankName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShobe(int ID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Shobe = servic.GetSHobeWithFilter("fldBankId", ID.ToString(), 0, IP, out Err);
            return Json(Shobe.Select(p1 => new { fldID = p1.fldId, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///. از این متد برای  ذخیره اطلاعات جدید یا اطلاعات ویرایش شده استفاده می شود 
        /// </summary>
        ///     <example>     
        ///       <code>
        ///       if(State.Id==0)
        ///          {
        ///              //Insert
        ///          }
        ///       else
        ///          {
        ///              //Update
        ///          }
        ///      </code>
        ///     </example>
        /// <param name="State">. که شامل تمامی فیلد های جدول شماره حساب به همراه مقادیر آنها میشود object</param>
        ///<returns> . موفقیت آمیز باشد پیغام ذخیره موفق یا ویرایش موفق را نمایش میدهد Update و Insert در صورتی که</returns>

        public ActionResult Save(WCF_Common.OBJ_ShomareHesabeOmoomi ShomareHesabe)
        {
            

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (ShomareHesabe.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertShomareHesabeOmoomi(ShomareHesabe.fldShobeId, ShomareHesabe.fldAshkhasId, ShomareHesabe.fldShomareHesab, ShomareHesabe.fldShomareSheba, ShomareHesabe.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateShomareHesabeOmoomi(ShomareHesabe.fldId, ShomareHesabe.fldShobeId, ShomareHesabe.fldAshkhasId, ShomareHesabe.fldShomareHesab, ShomareHesabe.fldShomareSheba, ShomareHesabe.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        /// .اطلاعات مربوط به یک سطر از جدول شماره حساب را براساس کد داده شده پاک میکند
        /// </summary>
        /// <param name="id">کد جدول شماره حساب</param>
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
                Msg = servic.DeleteShomareHesabeOmoomi(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        /// <returns>اطلاعات مربوط به یک سطر از جدول شماره حساب را بر میگرداند</returns>
        public ActionResult Details(int Id)
        {
            var q = servic.GetShomareHesabeOmoomiDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldDesc = q.fldDesc,
                fldAshkhasId = q.fldAshkhasId,
                fldBankId = q.fldBankId.ToString(),
                fldShobeId = q.fldShobeId.ToString(),
                fldShomareHesab = q.fldShomareHesab,
                fldShomareSheba = q.fldShomareSheba,
                fldBankName = q.fldBankName,
                NameAshkhas = q.NameAshkhas,
                nameShobe = q.nameShobe
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///. فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول شماره حساب عمومی و امکان سرچ کردن بر اساس   
        /// </summary>
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_ShomareHesabeOmoomi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_ShomareHesabeOmoomi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "NameAshkhas":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameAshkhas";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                        case "nameShobe":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldnameShobe";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHesab";
                            break;
                        case "fldShomareSheba":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareSheba";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetShomareHesabeOmoomiWithFilter(field, searchtext,"","", 100,IP, out Err).ToList();
                    else
                        data = servic.GetShomareHesabeOmoomiWithFilter(field, searchtext, "", "", 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetShomareHesabeOmoomiWithFilter("", "", "", "", 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_ShomareHesabeOmoomi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
