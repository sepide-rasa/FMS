using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class ParametreSabetController : Controller
    {
        // 
        // GET: /Daramad/ParametreSabet/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
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
        public ActionResult IndexWin(int id, int FiscalYearId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q = servic.GetShomareHesabCodeDaramadWithFilter("fldId", id.ToString(),"",FiscalYearId, 0, IP, out Err).FirstOrDefault();
            PartialView.ViewBag.ShomareHesabCodeDaramadId = id;
            PartialView.ViewBag.DaramadTitle = q.fldDaramadTitle;// +"(" + q.fldDaramadCode + ")";
            return PartialView;
        }
        public ActionResult HelpIndexWin()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult NerkhParam(int id, int FiscalYearId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.Nerkh();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                ViewData = this.ViewData
            };
            var q = servic.GetShomareHesabCodeDaramadWithFilter("fldId", id.ToString(), "",FiscalYearId, 0, IP, out Err).FirstOrDefault();
            result.ViewBag.ShomareHesabCodeDaramadId = id;
            result.ViewBag.DaramadTitle = q.fldDaramadTitle;// +"(" + q.fldDaramadCode + ")";
            return result;
        }
        public ActionResult HelpNerkhParam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult newNerkh(int id, int ParamId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.ParamId = ParamId;
            var q=servic.GetParametreSabetWithFilter("fldId", ParamId.ToString(), "", 0, IP, out Err).FirstOrDefault();
            PartialView.ViewBag.NoeField = q.fldNoeField.ToString();
            return PartialView;
        }
        public ActionResult GetCodhayeDaramad()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCodhayeDaramdWithFilter("", "",0,0, 0,IP ,out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldDaramadTitle });
            return this.Store(q);
        }
        public ActionResult GetComboBox()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetComboBoxWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult Save(WCF_Daramad.OBJ_ParametreSabet Param)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Param.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertParametreSabet(Param.fldShomareHesabCodeDaramadId, Param.fldNameParametreFa, Param.fldNameParametreEn, Param.fldNoe, Param.fldNoeField,
                        Param.fldVaziyat, Param.fldComboBaxId, Param.fldTypeParametr, Param.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateParametreSabet(Param.fldId, Param.fldShomareHesabCodeDaramadId, Param.fldNameParametreFa, Param.fldNameParametreEn, Param.fldNoe, Param.fldNoeField,
                        Param.fldVaziyat, Param.fldComboBaxId, Param.fldTypeParametr, Param.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteParametreSabet(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Details(int Id)
        {
            var q = servic.GetParametreSabetDetail(Id, IP, out Err);
            var Noe = "0";
            if (q.fldNoe)
                Noe = "1";

            var Vaziyat = "0";
            if (q.fldVaziyat)
                Vaziyat = "1";

            var TypeParametr = "0";
            if (q.fldTypeParametr)
                TypeParametr = "1";
            return Json(new
            {
                fldId = q.fldId,
                fldShomareHesabCodeDaramadId = q.fldShomareHesabCodeDaramadId.ToString(), 
                fldNameParametreFa=q.fldNameParametreFa, 
                fldNameParametreEn=q.fldNameParametreEn,
                fldNoe = Noe,
                fldNoeField = q.fldNoeField.ToString(),
                fldVaziyat = Vaziyat,
                fldFormulId = q.fldFormulId, 
                fldComboBaxId=q.fldComboBaxId.ToString(),
                fldTypeParametr=TypeParametr,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters, int ShomareHesabCodeDaramadId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ParametreSabet> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_ParametreSabet> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldDaramadTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDaramadTitle";
                            break;
                        case "fldNameParametreFa":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameParametreFa";
                            break;
                        case "NoeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "NoeName";
                            break;
                        case "NoeFieldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "NoeFieldName";
                            break;
                        case "VaziyatName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "VaziyatName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetParametreSabetWithFilter(field, searchtext, ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).ToList();
                    else
                        data = servic.GetParametreSabetWithFilter(field, searchtext, ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetParametreSabetWithFilter("fldShomareHesabCodeDaramadId", ShomareHesabCodeDaramadId.ToString(),"",  0, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_ParametreSabet> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadParamSabet(StoreRequestParameters parameters, int ShomareHesabCodeDaramadId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ParametreSabet> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_ParametreSabet> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldDaramadTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDaramadTitle";
                            break;
                        case "fldNameParametreFa":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameParametreFa";
                            break;
                        case "NoeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "NoeName";
                            break;
                        case "NoeFieldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "NoeFieldName";
                            break;
                        case "VaziyatName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "VaziyatName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetParametreSabetWithFilter(field, searchtext, ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).Where(k => k.fldTypeParametr == false).ToList();
                    else
                        data = servic.GetParametreSabetWithFilter(field, searchtext, ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).Where(k => k.fldTypeParametr == false).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetParametreSabetWithFilter("fldShomareHesabCodeDaramadId",ShomareHesabCodeDaramadId.ToString(), "",  0, IP, out Err).Where(k => k.fldTypeParametr == false).ToList();
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

            List<WCF_Daramad.OBJ_ParametreSabet> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadNerkhParametr(StoreRequestParameters parameters, int ParametreSabetId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ParametreSabet_Nerkh> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_ParametreSabet_Nerkh> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldValue":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldValue";
                            break;
                        case "fldTarikhFaalSazi":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhFaalSazi";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetParametreSabet_NerkhWithFilter(field, searchtext, 0, IP, out Err).Where(k => k.fldParametreSabetId == ParametreSabetId).ToList();
                    else
                        data = servic.GetParametreSabet_NerkhWithFilter(field, searchtext, 0, IP, out Err).Where(k => k.fldParametreSabetId == ParametreSabetId).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetParametreSabet_NerkhWithFilter("fldParametreSabetId", ParametreSabetId.ToString(), 0, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_ParametreSabet_Nerkh> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveNerkh(WCF_Daramad.OBJ_ParametreSabet_Nerkh nerkh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {

                if (nerkh.fldDesc == null)
                    nerkh.fldDesc = "";

                if (nerkh.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertParametreSabet_Nerkh(nerkh.fldParametreSabetId, nerkh.fldTarikhFaalSazi, nerkh.fldValue, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateParametreSabet_Nerkh(nerkh.fldId, nerkh.fldParametreSabetId, nerkh.fldTarikhFaalSazi, nerkh.fldValue, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteNerkh(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
              //حذف
                servic.DeleteParametreSabet_Nerkh(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Msg = "حذف با موفقیت انجام شد.";
                MsgTitle = "حذف موفق";
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
        public ActionResult NerkhDetails(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var q = servic.GetParametreSabet_NerkhWithFilter("fldId", Id.ToString(), 0,IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldTarikhFaalSazi = q.fldTarikhFaalSazi,
                fldValue = q.fldValue
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CopyNerkh(int MabdaNerkhId, int MaghsadParamId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {

                var nerkh = servic.GetParametreSabet_NerkhWithFilter("fldId", MabdaNerkhId.ToString(), 0, IP, out Err).FirstOrDefault();

                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                     servic.InsertParametreSabet_Nerkh(MaghsadParamId, nerkh.fldTarikhFaalSazi, nerkh.fldValue, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                     Msg = "نرخ پارامتر با موفقیت کپی شد.";

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
    }
}
