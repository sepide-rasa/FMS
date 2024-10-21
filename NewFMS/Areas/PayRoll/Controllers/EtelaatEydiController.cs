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
    public class EtelaatEydiController : Controller
    {
        //
        // GET: /PayRoll/EtelaatEydi/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
        public ActionResult Index(string containerId, string Year, string Month,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.EtelaatEydi(); 
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
        public ActionResult New(int id, int PersonalId, string Year, string Month,int OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            //var d = Cservic.GetDate(IP, out CErr);
            //var sal = d.fldTarikh.Substring(0, 4);
            var DayCount = 365;
            if (MyLib.Shamsi.Iskabise(Convert.ToInt32(Year)))
                DayCount = 366;
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.DayCount = DayCount;
            PartialView.ViewBag.OrganId = OrganId;

            return PartialView;
        }
        public ActionResult GroupIndex(string containerId, string Year, string Month, string nobatePardakht, string FieldName, int CostCenter_ChartId,int OrganId)
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
        public ActionResult Save(WCF_PayRoll.OBJ_EtelaatEydi Eydi, int OrganId)
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
                    var q = servic.GetEtelaatEydiWithFilter("CheckSave", Eydi.fldPersonalId.ToString(), 0, Eydi.fldYear, Eydi.fldNobatePardakht,
                        OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err).FirstOrDefault();
                    if (q != null)
                    {
                        Msg = "اطلاعات عیدی شخص مورد نظر قبلا ثبت شده است.";
                        MsgTitle = "خطا";
                    }
                    else
                    {
                        Msg = servic.InsertEtelaatEydi(Eydi.fldPersonalId, Eydi.fldYear, Eydi.fldDayCount, Eydi.fldKosurat, Eydi.fldNobatePardakht, Eydi.fldDesc,
                            Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                        MsgTitle = "ذخیره موفق";
                    }
                }
                else
                { //ویرایش
                    var q = servic.GetEtelaatEydiWithFilter("CheckEdit", Eydi.fldPersonalId.ToString(), Eydi.fldId, Eydi.fldYear, Eydi.fldNobatePardakht,
                        OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err).FirstOrDefault();
                    if (q != null)
                    {
                        Msg = "اطلاعات عیدی شخص مورد نظر دراین نوبت پرداخت ثبت شده است.";
                        MsgTitle = "خطا";
                    }
                    else
                    {
                        Msg = servic.UpdateEtelaatEydi(Eydi.fldId, Eydi.fldPersonalId, Eydi.fldYear, Eydi.fldDayCount, Eydi.fldKosurat, Eydi.fldNobatePardakht,
                            Eydi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

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
        public ActionResult Delete(int id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                servic.DeleteEtelaatEydi(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
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
            var q = servic.GetEtelaatEydiDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldPersonalId = q.fldPersonalId,
                fldDayCount = q.fldDayCount,
                fldName_Father = q.fldName_Father,
                fldSh_Personali=q.fldSh_Personali,
                fldCodemeli=q.fldCodemeli,
                fldKosurat = q.fldKosurat,
                fldYear = q.fldYear.ToString(),
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
                        data1 = servic.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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
        public ActionResult ReadEtelaatEydi(StoreRequestParameters parameters, int PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_EtelaatEydi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_EtelaatEydi> data1 = null;
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
                        case "fldKosurat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Kosurat";
                            break;
                        case "fldDayCount":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_DayCount";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetEtelaatEydiWithFilter(field, searchtext, PersonalId, 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                    else
                        data = servic.GetEtelaatEydiWithFilter(field, searchtext, PersonalId, 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetEtelaatEydiWithFilter("fldPersonalId", PersonalId.ToString(), 0, 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_EtelaatEydi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Rload(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_EtelaatEydi> data = null;

            data = servic.GetEtelaatEydiWithFilter("fldPersonalId", PersonalId.ToString(),0,0,0,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveGroup(List<WCF_PayRoll.OBJ_EtelaatEydi> EydiVal,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                foreach (var item in EydiVal)
                {
                    if (item.fldDesc == null)
                        item.fldDesc = "";

                    //ذخیره
                    if (item.fldId == 0)
                    {
                        var q = servic.GetEtelaatEydiWithFilter("CheckSave", item.fldPersonalId.ToString(), 0, item.fldYear, item.fldNobatePardakht,
                            OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            Msg = "اطلاعات عیدی شخص مورد نظر قبلا ثبت شده است.";
                            MsgTitle = "اخطار";
                        }
                        else
                        {
                            servic.InsertEtelaatEydi(item.fldPersonalId, item.fldYear, item.fldDayCount, item.fldKosurat, item.fldNobatePardakht, item.fldDesc,
                                Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                            Msg = "ذخیره با موفقیت انجام شد.";
                            MsgTitle = "ذخیره موفق";
                        }
                    }
                    else
                    {
                        //ویرایش
                        var q = servic.GetEtelaatEydiWithFilter("CheckEdit", item.fldPersonalId.ToString(), item.fldId, item.fldYear, item.fldNobatePardakht,
                            OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            Msg = "اطلاعات عیدی شخص مورد نظر تکراری است.";
                            MsgTitle = "اخطار";
                        }
                        else
                        {
                            servic.UpdateEtelaatEydi(item.fldId, item.fldPersonalId, item.fldYear, item.fldDayCount, item.fldKosurat, item.fldNobatePardakht, item.fldDesc,
                                Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                            Msg = "ویرایش با موفقیت انجام شد.";
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
        public ActionResult DeleteGroup(List<WCF_PayRoll.OBJ_EtelaatEydi> EydiGroupVal,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                //حذف
                foreach (var item in EydiGroupVal)
                {
                    servic.DeleteEtelaatEydi(item.fldId, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
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
        public ActionResult ReloadEtelaatEydiGroup(string FieldName, int CostCenter_ChartId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_EtelaatEydi> data = null;

            data = servic.GetEtelaatEydiGroupWithFilter(FieldName, Convert.ToInt16(Models.CurrentDate.Year), Models.CurrentDate.nobatPardakht,
                Models.CurrentDate.CostCenter, OrganId/*Convert.ToInt32(Session["OrganId"])*/, CostCenter_ChartId, IP, out Err).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
