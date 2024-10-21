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
    public class MorakhasiController : Controller
    {
        //
        // GET: /PayRoll/Morakhasi/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        public ActionResult Index(string containerId, string Year, string Month,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.Morakhasi(); ;
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
            result.ViewBag.OrganId = OrganId;

            return result;
        }
        public ActionResult New(int id, int PersonalId, string Year, string Month,int  OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.OrganId = OrganId;

            return PartialView;
        }
        public ActionResult GroupIndex(string containerId, string Year, string Month, string nobatePardakht, string FieldName, int CostCenter_ChartId, int OrganId)
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
            result.ViewBag.nobatePardakht = nobatePardakht;
            result.ViewBag.FieldName = FieldName;
            result.ViewBag.CostCenter_ChartId = CostCenter_ChartId;
            result.ViewBag.OrganId = OrganId;

            return result;
        }
        public ActionResult Save(WCF_PayRoll.OBJ_Morakhasi morakhsi,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (morakhsi.fldDesc == null)
                    morakhsi.fldDesc = "";
                if (morakhsi.fldId == 0)
                { //ذخیره
                    var q = servic.GetMorakhasiWithFilter("CheckSave", morakhsi.fldPersonalId.ToString(), 0, morakhsi.fldYear, morakhsi.fldMonth,
                        morakhsi.fldNobatePardakht, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err).FirstOrDefault();
                    if (q != null)
                    {
                        Msg = "اطلاعات مرخصی شخص مورد نظر قبلا ثبت شده است.";
                        MsgTitle = "خطا";
                        Er = 1;
                    }
                    else
                    {
                        Msg = servic.InsertMorakhasi(morakhsi.fldPersonalId, morakhsi.fldYear, morakhsi.fldMonth, morakhsi.fldNobatePardakht, morakhsi.fldSalAkharinHokm,
                            morakhsi.fldTedad, morakhsi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                        MsgTitle = "ذخیره موفق";
                    }
                }
                else
                { //ویرایش
                    var q = servic.GetMorakhasiWithFilter("CheckEdit", morakhsi.fldPersonalId.ToString(), morakhsi.fldId, morakhsi.fldYear, morakhsi.fldMonth, morakhsi.fldNobatePardakht,
                        OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err).FirstOrDefault();
                    if (q != null)
                    {
                        Msg = "اطلاعات مرخصی شخص مورد نظر دراین نوبت پرداخت تکراری است.";
                        MsgTitle = "خطا";
                        Er = 1;
                    }
                    else
                    {
                        Msg = servic.UpdateMorakhasi(morakhsi.fldId, morakhsi.fldPersonalId, morakhsi.fldYear, morakhsi.fldMonth, morakhsi.fldNobatePardakht,
                            morakhsi.fldSalAkharinHokm, morakhsi.fldTedad, morakhsi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                        MsgTitle = "ویرایش موفق";
                    }
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
        public ActionResult Delete(int id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                servic.DeleteMorakhasi(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMorakhasiDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldPersonalId = q.fldPersonalId,
                fldTedad = q.fldTedad,
                fldName_Father = q.fldName_Father,
                fldCodemeli=q.fldCodemeli,
                fldSh_Personali=q.fldSh_Personali,
                fldSalAkharinHokm = q.fldSalAkharinHokm,
                fldYear = q.fldYear.ToString(),
                fldMonth = q.fldMonth.ToString(),
                fldNobatePardakht = q.fldNobatePardakht.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters, int OrganId)
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
                        data1 = servic.GetPayPersonalInfoWithFilter(field, searchtext,OrganId/* Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                    else
                        data = servic.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPayPersonalInfoWithFilter("", "",OrganId/* Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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
        public ActionResult ReadMorakhasi(StoreRequestParameters parameters, int PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_Morakhasi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_Morakhasi> data1 = null;
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
                        case "fldMonthName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_MonthName";
                            break;
                        case "fldNobatePardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_NobatePardakht";
                            break;
                        case "fldSalAkharinHokm":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_SalAkharinHokm";
                            break;
                        case "fldTedad":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Tedad";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetMorakhasiWithFilter(field, searchtext, PersonalId, 0, 0, 0,OrganId/* Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                    else
                        data = servic.GetMorakhasiWithFilter(field, searchtext, PersonalId, 0, 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMorakhasiWithFilter("fldPersonalId", PersonalId.ToString(), 0, 0, 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_Morakhasi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Rload(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_Morakhasi> data = null;

            data = servic.GetMorakhasiWithFilter("fldPersonalId", PersonalId.ToString(), 0, 0, 0, 1,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveGroup(List<WCF_PayRoll.OBJ_Morakhasi> morakhsiVal, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                foreach (var item in morakhsiVal)
                {
                    if (item.fldDesc == null)
                        item.fldDesc = "";

                    //ذخیره
                    if (item.fldId == 0)
                    {
                        var q = servic.GetMorakhasiWithFilter("CheckSave", item.fldPersonalId.ToString(), 0, item.fldYear, item.fldMonth, item.fldNobatePardakht,
                            OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            Msg = "اطلاعات مرخصی شخص مورد نظر قبلا ثبت شده است.";
                            MsgTitle = "خطا";
                        }
                        else
                        {
                            servic.InsertMorakhasi(item.fldPersonalId, item.fldYear, item.fldMonth, item.fldNobatePardakht, item.fldSalAkharinHokm, item.fldTedad,
                                item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                            Msg = "ذخیره با موفقیت انجام شد.";
                            MsgTitle = "ذخیره موفق";
                        }
                    }
                    else
                    {
                        //ویرایش
                        var q = servic.GetMorakhasiWithFilter("CheckEdit", item.fldPersonalId.ToString(), item.fldId, item.fldYear, item.fldMonth, item.fldNobatePardakht,
                            OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            Msg = "اطلاعات مرخصی شخص مورد نظر دراین نوبت پرداخت تکراری است.";
                            MsgTitle = "خطا";
                        }
                        else
                        {
                            servic.UpdateMorakhasi(item.fldId, item.fldPersonalId, item.fldYear, item.fldMonth, item.fldNobatePardakht, item.fldSalAkharinHokm,
                                item.fldTedad, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                            Msg = "ویرایش با موفقیت انجام شد.";
                            MsgTitle = "ویرایش موفق";
                        }
                    }
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteGroup(List<WCF_PayRoll.OBJ_Morakhasi> MorakhasiGroupVal,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                //حذف
                foreach (var item in MorakhasiGroupVal)
                {
                    servic.DeleteMorakhasi(item.fldId, Session["Username"].ToString(), (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
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
        public ActionResult ReloadMorakhasiGroup(string FieldName, int CostCenter_ChartId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_Morakhasi> data = null;

            data = servic.GetMorakhasiGroupWithFilter(FieldName, Convert.ToInt16(Models.CurrentDate.Year), Convert.ToByte(Models.CurrentDate.Month),
                Models.CurrentDate.nobatPardakht, OrganId/*Convert.ToInt32(Session["OrganId"])*/, CostCenter_ChartId, IP, out Err).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
