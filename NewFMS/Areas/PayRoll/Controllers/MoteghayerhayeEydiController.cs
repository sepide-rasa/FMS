using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using System.IO;
namespace NewFMS.Areas.PayRoll.Controllers
{
    public class MoteghayerhayeEydiController : Controller
    {
        //
        // GET: /PayRoll/MoteghayerhayeEydi/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
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
            var q = Cservic.GetDate(IP, out CErr);
            string year = q.fldTarikh.Substring(0, 4);
            result.ViewBag.Year = year;
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
        public ActionResult GetSal()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            var q = Cservic.GetDate(IP, out CErr);
            int fldsal = Convert.ToInt32(q.fldTarikh.Substring(0, 4)) - 7;
            for (int i = fldsal; i < fldsal + 16; i++)
            {
                Models.CurrentDate CboSal = new Models.CurrentDate();

                CboSal.fldYear = i;
                sh.Add(CboSal);
            }
            return Json(sh, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(WCF_PayRoll.OBJ_MoteghayerhayeEydi Eydi)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Eydi.fldDesc == null)
                    Eydi.fldDesc = "";
                if (Eydi.fldId == 0)
                { //ذخیره
                    var q = servic.GetMoteghayerhayeEydiWithFilter("fldYear", Eydi.fldYear.ToString(), 1, IP, out Err).FirstOrDefault();
                    if (q != null)
                    {
                        Msg = "اطلاعات قبلا در سال مورد نظر ثبت شده است.";
                        MsgTitle = "اخطار";
                        Er = 1;
                    }
                    else
                    {
                        Msg = servic.InsertMoteghayerhayeEydi(Eydi.fldYear, Eydi.fldMaxEydiKarmand, Eydi.fldMaxEydiKargar, Eydi.fldZaribEydiKargari,
                            Eydi.fldTypeMohasebatMaliyat, Eydi.fldMablaghMoafiatKarmand, Eydi.fldMablaghMoafiatKargar, Eydi.fldDarsadMaliyatKarmand,
                            Eydi.fldDarsadMaliyatKargar, Eydi.fldTypeMohasebe, Eydi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        MsgTitle = "ذخیره موفق";
                    }
                }
                else
                { //ویرایش
                    Msg = servic.UpdateMoteghayerhayeEydi(Eydi.fldId, Eydi.fldYear, Eydi.fldMaxEydiKarmand, Eydi.fldMaxEydiKargar, Eydi.fldZaribEydiKargari,
                Eydi.fldTypeMohasebatMaliyat, Eydi.fldMablaghMoafiatKarmand, Eydi.fldMablaghMoafiatKargar, Eydi.fldDarsadMaliyatKarmand,
                Eydi.fldDarsadMaliyatKargar, Eydi.fldTypeMohasebe, Eydi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ویرایش موفق";
                }
                if (Err.ErrorType)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
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
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                Msg = servic.DeleteMoteghayerhayeEydi(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                MsgTitle = "حذف موفق";
                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
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
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMoteghayerhayeEydiDetail(Id, IP, out Err);
            var TypeMohasebatMaliyat = "0";
            var TypeMohasebe = "0";
            if (q.fldTypeMohasebatMaliyat)
                TypeMohasebatMaliyat = "1";
            if (q.fldTypeMohasebe)
                TypeMohasebe = "1";
            return Json(new
            {
                fldId = q.fldId,
                fldYear = q.fldYear.ToString(),
                fldMaxEydiKarmand = q.fldMaxEydiKarmand,
                fldMaxEydiKargar = q.fldMaxEydiKargar,
                fldZaribEydiKargari = q.fldZaribEydiKargari,
                fldTypeMohasebatMaliyat = TypeMohasebatMaliyat,
                fldMablaghMoafiatKarmand = q.fldMablaghMoafiatKarmand,
                fldMablaghMoafiatKargar = q.fldMablaghMoafiatKargar,
                fldDarsadMaliyatKarmand = q.fldDarsadMaliyatKarmand,
                fldDarsadMaliyatKargar = q.fldDarsadMaliyatKargar,
                fldTypeMohasebe = TypeMohasebe,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_MoteghayerhayeEydi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_MoteghayerhayeEydi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldYear":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldYear";
                            break;
                        case "fldMaxEydiKarmand":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMaxEydiKarmand";
                            break;
                        case "fldMaxEydiKargar":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMaxEydiKargar";
                            break;
                        case "fldZaribEydiKargari":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldZaribEydiKargari";
                            break;
                        case "fldTypeMohasebatMaliyatName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeMohasebatMaliyatName";
                            break;
                        case "fldMablaghMoafiatKarmand":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghMoafiatKarmand";
                            break;
                        case "fldMablaghMoafiatKargar":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMablaghMoafiatKargar";
                            break;
                        case "fldDarsadMaliyatKarmand":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDarsadMaliyatKarmand";
                            break;
                        case "fldDarsadMaliyatKargar":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDarsadMaliyatKargar";
                            break;
                        case "fldTypeMohasebeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeMohasebeName";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetMoteghayerhayeEydiWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetMoteghayerhayeEydiWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMoteghayerhayeEydiWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_MoteghayerhayeEydi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
