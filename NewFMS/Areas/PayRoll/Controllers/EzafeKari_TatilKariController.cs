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
    public class EzafeKari_TatilKariController : Controller
    {
        //
        // GET: /PayRoll/EzafeKari_TatilKari/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common_Pay.Comon_PayService Cservic = new WCF_Common_Pay.Comon_PayService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common_Pay.ClsError CErr = new WCF_Common_Pay.ClsError();
        public ActionResult Index(string containerId, string Year, string Month, string Type,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.EzafeKari_TatilKari(); ;
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = Year;
            result.ViewBag.Month = Month;
            result.ViewBag.Type = Type;
            result.ViewBag.OrganId = OrganId;

            return result;
        }
        public ActionResult New(int id, int PersonalId, string Year, string Month, string Type,int OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var DayCount = 365;
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(Year)))
                DayCount = 366;
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.Type = Type;
            PartialView.ViewBag.DayCount = DayCount;
            PartialView.ViewBag.OrganId = OrganId;

            return PartialView;
        }
        public ActionResult GroupIndex(string containerId, string Year, string Month, string nobatePardakht, string Type, string FieldName, int CostCenter_ChartId, int OrganId)
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
            result.ViewBag.Year = Year;
            result.ViewBag.Month = Month;
            result.ViewBag.OrganId = OrganId;

            result.ViewBag.nobatePardakht = nobatePardakht;
            result.ViewBag.Type = Type;
            result.ViewBag.FieldName = FieldName;
            result.ViewBag.CostCenter_ChartId = CostCenter_ChartId;
            return result;
        }
        public ActionResult Save(WCF_PayRoll.OBJ_EzafeKari_TatilKari ezafe,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (ezafe.fldDesc == null)
                    ezafe.fldDesc = "";
                if (ezafe.fldId == 0)
                { //ذخیره
                    var q = servic.GetEzafeKari_TatilKariWithFilter("CheckSave", ezafe.fldPersonalId.ToString(), 0, ezafe.fldYear, ezafe.fldMonth, ezafe.fldNobatePardakht,
                        ezafe.fldType, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
                    if (q != null)
                    {
                        if (ezafe.fldType == 1)
                        {
                            Msg = "اضافه کاری شخص مورد نظر قبلا ثبت شده است.";
                            MsgTitle = "خطا";
                        }
                        else if (ezafe.fldType == 2)
                        {
                            Msg = "تعطیل کاری شخص مورد نظر قبلا ثبت شده است.";
                            MsgTitle = "خطا";
                        }
                    }
                    else
                    {
                        Msg = servic.InsertEzafeKari_TatilKari(ezafe.fldPersonalId, ezafe.fldYear, ezafe.fldMonth, ezafe.fldNobatePardakht, ezafe.fldCount, ezafe.fldType, 
                            ezafe.fldHasBime, ezafe.fldHasMaliyat, ezafe.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                        MsgTitle = "ذخیره موفق";
                    }
                }
                else
                { //ویرایش
                    var q = servic.GetEzafeKari_TatilKariWithFilter("CheckEdit", ezafe.fldPersonalId.ToString(), ezafe.fldId, ezafe.fldYear, ezafe.fldMonth, ezafe.fldNobatePardakht, 
                        ezafe.fldType,OrganId/* Convert.ToInt32(Session["OrganId"])*/, 0,IP, out Err).FirstOrDefault();
                    if (q != null)
                    {
                        if (ezafe.fldType == 1)
                        {
                            Msg = "اضافه کاری شخص مورد نظر تکراری است.";
                            MsgTitle = "خطا";
                        }
                        else if (ezafe.fldType == 2)
                        {
                            Msg = "تعطیل کاری شخص مورد نظر تکراری است.";
                            MsgTitle = "خطا";
                        }
                    }
                    else
                    {
                        Msg = servic.UpdateEzafeKari_TatilKari(ezafe.fldId, ezafe.fldPersonalId, ezafe.fldYear, ezafe.fldMonth, ezafe.fldNobatePardakht, ezafe.fldCount, ezafe.fldType,
                            ezafe.fldHasBime, ezafe.fldHasMaliyat, ezafe.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                        MsgTitle = "ویرایش موفق";
                    }
                }
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
                Err=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                servic.DeleteEzafeKari_TatilKari(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                Msg = "حذف با موفقیت انجام شد.";
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
                Err=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetEzafeKari_TatilKariDetail(Id,OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
            var IsKargar = 0;
            var q2 = servic.GetPayPersonalInfoWithFilter("fldId", q.fldPersonalId.ToString(),OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0,IP, out Err).FirstOrDefault();
            var H = Cservic.GetMaxPersonalEstekhdamTypeWithFilter("", q2.fldPrs_PersonalInfoId, "", IP, out CErr).FirstOrDefault();
            if (H.fldNoeEstekhdamId == 1)
                IsKargar = 1;
            var HasBime = 0;
            if (q.fldHasBime)
                HasBime = 1;
            var HasMaliyat = 0;
            if (q.fldHasMaliyat)
                HasMaliyat = 1;
            return Json(new
            {
                fldId = q.fldId,
                fldPersonalId = q.fldPersonalId,
                fldName = q.fldName_Father,
                fldCount = q.fldCount,
                fldCodemeli = q.fldCodemeli,
                fldSh_Personali = q.fldSh_Personali,
                fldYear = q.fldYear.ToString(),
                fldMonth = q.fldMonth.ToString(),
                fldNobatePardakht = q.fldNobatePardakht.ToString(),
                fldHasBime = HasBime.ToString(),
                fldHasMaliyat = HasMaliyat.ToString(),
                IsKargar = IsKargar
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailsHeader(int Id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPayPersonalInfoWithFilter("fldId", Id.ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName_Father,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_PayPersonalInfo> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_PayPersonalInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName_Father":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName_Father";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNationalCode";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomarePersoneli";
                            break;
                        case "fldJobeCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldJobeCode";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetPayPersonalInfoWithFilter(field, searchtext,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                    else
                        data = servic.GetPayPersonalInfoWithFilter(field, searchtext,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPayPersonalInfoWithFilter("", "",OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_PayPersonalInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadEzafeKari_TatilKari(StoreRequestParameters parameters, int PersonalId, byte Type,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_EzafeKari_TatilKari> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_EzafeKari_TatilKari> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldPersonalId_Id";
                            break;
                        case "fldYear":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Year";
                            break;
                        case "fldNobatePardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_NobatePardakht";
                            break;
                        case "fldCount":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Count";
                            break;
                        case "fldHasBimeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_HasBimeName";
                            break;
                        case "fldHasMaliyatName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_HasMaliyatName";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetEzafeKari_TatilKariWithFilter(field, searchtext, PersonalId, 0, 0, 0, Type,OrganId/* Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                    else
                        data = servic.GetEzafeKari_TatilKariWithFilter(field, searchtext, PersonalId, 0, 0, 0, Type,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetEzafeKari_TatilKariWithFilter("fldPersonalId", PersonalId.ToString(), 0, 0, 0, 0, Type,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_EzafeKari_TatilKari> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Rload(int PersonalId, byte Type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_EzafeKari_TatilKari> data = null;

            data = servic.GetEzafeKari_TatilKariWithFilter("fldPersonalId", PersonalId.ToString(), 0, 0, 0, 0, Type, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveGroup(List<WCF_PayRoll.OBJ_EzafeKari_TatilKari> EzafeKari_TatilKariVal, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                foreach (var item in EzafeKari_TatilKariVal)
                {
                    if (item.fldDesc == null)
                        item.fldDesc = "";

                    //ذخیره
                    if (item.fldId == 0)
                    {
                        var q = servic.GetEzafeKari_TatilKariWithFilter("CheckSave", item.fldPersonalId.ToString(), 0, item.fldYear, item.fldMonth, item.fldNobatePardakht,
                            item.fldType, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            if (item.fldType == 1)
                            {
                                Msg = "اضافه کاری شخص مورد نظر قبلا ثبت شده است.";
                                MsgTitle = "خطا";
                            }
                            else if (item.fldType == 2)
                            {
                                Msg = "تعطیل کاری شخص مورد نظر قبلا ثبت شده است.";
                                MsgTitle = "خطا";
                            }
                        }
                        else
                        {
                            Msg = servic.InsertEzafeKari_TatilKari(item.fldPersonalId, item.fldYear, item.fldMonth, item.fldNobatePardakht, item.fldCount, item.fldType, item.fldHasBime,
                                item.fldHasMaliyat, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                            MsgTitle = "ذخیره موفق";
                        }
                    }
                    else
                    {
                        //ویرایش
                        var q = servic.GetEzafeKari_TatilKariWithFilter("CheckEdit", item.fldPersonalId.ToString(), item.fldId, item.fldYear, item.fldMonth, item.fldNobatePardakht,
                            item.fldType, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            if (item.fldType == 1)
                            {
                                Msg = "اضافه کاری شخص مورد نظر تکراری است.";
                                MsgTitle = "خطا";
                            }
                            else if (item.fldType == 2)
                            {
                                Msg = "تعطیل کاری شخص مورد نظر تکراری است.";
                                MsgTitle = "خطا";
                            }
                        }
                        else
                        {
                            Msg = servic.UpdateEzafeKari_TatilKari(item.fldId, item.fldPersonalId, item.fldYear, item.fldMonth, item.fldNobatePardakht, item.fldCount, item.fldType,
                                item.fldHasBime, item.fldHasMaliyat, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                            MsgTitle = "ویرایش موفق";
                        }
                    }
                    if (Err.ErrorType == true)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Err = 1
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
                Err=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteGroup(List<WCF_PayRoll.OBJ_EzafeKari_TatilKari> EzafeKari_TatilKariGroupVal,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                //حذف
                foreach (var item in EzafeKari_TatilKariGroupVal)
                {
                    servic.DeleteEzafeKari_TatilKari(item.fldId, Session["Username"].ToString(), (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                    if (Err.ErrorType == true)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadEzafeKari_TatilKariGroup(byte Type, string FieldName, int CostCenter_ChartId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_EzafeKari_TatilKari> data = null;

            data = servic.GetEzafeKari_TatilKariGroupWithFilter(FieldName, Models.CurrentDate.Year, Models.CurrentDate.Month, Models.CurrentDate.nobatPardakht, Type,
                OrganId/*Convert.ToInt32(Session["OrganId"])*/, CostCenter_ChartId, IP, out Err).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
