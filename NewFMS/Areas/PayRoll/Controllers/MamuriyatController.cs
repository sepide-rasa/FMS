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
    public class MamuriyatController : Controller
    {
        //
        // GET: /PayRoll/Mamuriyat/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common_Pay.Comon_PayService Cservic = new WCF_Common_Pay.Comon_PayService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common_Pay.ClsError CErr = new WCF_Common_Pay.ClsError();
        public ActionResult Index(string containerId, string Year, string Month,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.Mamuriyat(); 
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
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.OrganId = OrganId;


            var IsKargar = 0;
            var q2 = servic.GetPayPersonalInfoWithFilter("fldId", PersonalId.ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
            var H = Cservic.GetMaxPersonalEstekhdamTypeWithFilter("", q2.fldPrs_PersonalInfoId, "", IP, out CErr).FirstOrDefault();
            if (H.fldNoeEstekhdamId == 1)
                IsKargar = 1;
            PartialView.ViewBag.IsKargar = IsKargar;

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
            result.ViewBag.OrganId = OrganId;

            result.ViewBag.CostCenter_ChartId = CostCenter_ChartId;
            return result;
        }
        public ActionResult Save(WCF_PayRoll.OBJ_Mamuriyat Mamuriyat, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Mamuriyat.fldDesc == null)
                    Mamuriyat.fldDesc = "";

                if (Mamuriyat.fldId == 0)
                { //ذخیره
                    var q = servic.GetMamuriyatWithFilter("CheckSave", Mamuriyat.fldPersonalId.ToString(), 0, Mamuriyat.fldYear, Mamuriyat.fldMonth,
                        Mamuriyat.fldNobatePardakht, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
                    if (q != null)
                    {
                        Msg = "ماموریت شخص مورد نظر قبلا ثبت شده است.";
                        MsgTitle = "اخطار";
                    }
                    else
                    {
                        servic.InsertMamuriyat(Mamuriyat.fldPersonalId, Mamuriyat.fldYear, Mamuriyat.fldMonth, Mamuriyat.fldNobatePardakht, Mamuriyat.fldBaBeytute, Mamuriyat.fldBeduneBeytute
                             , Mamuriyat.fldBa10, Mamuriyat.fldBa20, Mamuriyat.fldBa30, Mamuriyat.fldBe10, Mamuriyat.fldBe20, Mamuriyat.fldBe30, Mamuriyat.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";
                    }
                }
                else
                { //ویرایش
                    var q = servic.GetMamuriyatWithFilter("CheckEdit", Mamuriyat.fldPersonalId.ToString(), Mamuriyat.fldId, Mamuriyat.fldYear, Mamuriyat.fldMonth,
                        Mamuriyat.fldNobatePardakht, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
                    if (q != null)
                    {
                        Msg = "ماموریت شخص مورد نظر تکراری است.";
                        MsgTitle = "اخطار";
                    }
                    else
                    {
                        servic.UpdateMamuriyat(Mamuriyat.fldId, Mamuriyat.fldPersonalId, Mamuriyat.fldYear, Mamuriyat.fldMonth, Mamuriyat.fldNobatePardakht, Mamuriyat.fldBaBeytute, Mamuriyat.fldBeduneBeytute
                            , Mamuriyat.fldBa10, Mamuriyat.fldBa20, Mamuriyat.fldBa30, Mamuriyat.fldBe10, Mamuriyat.fldBe20, Mamuriyat.fldBe30, Mamuriyat.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                        Msg = "ویرایش با موفقیت انجام شد.";
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
                servic.DeleteMamuriyat(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
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
        public ActionResult Details(int Id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMamuriyatDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldPersonalId = q.fldPersonalId,
                fldName = q.fldName_Father,
                fldCodemeli=q.fldCodemeli,
                fldSh_Personali=q.fldSh_Personali,
                fldYear = q.fldYear.ToString(),
                fldMonth = q.fldMonth.ToString(),
                fldBa10 = q.fldBa10,
                fldBa20 = q.fldBa20,
                fldBa30 = q.fldBa30,
                fldBaBeytute = q.fldBaBeytute,
                fldBe10 = q.fldBe10,
                fldBe20 = q.fldBe20,
                fldBe30 = q.fldBe30,
                fldBeduneBeytute = q.fldBeduneBeytute,
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
                        data = servic.GetPayPersonalInfoWithFilter(field, searchtext,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPayPersonalInfoWithFilter("", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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
        public ActionResult Rload(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_Mamuriyat> data = null;

            data = servic.GetMamuriyatWithFilter("fldPersonalId", PersonalId.ToString(),0,0,0,0,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadMamuriyat(StoreRequestParameters parameters, int PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_Mamuriyat> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_Mamuriyat> data1 = null;
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
                        case "fldMonthName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_MonthName";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetMamuriyatWithFilter(field, searchtext, PersonalId, 0, 0, 0,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                    else
                        data = servic.GetMamuriyatWithFilter(field, searchtext, PersonalId, 0, 0, 0,OrganId/* Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMamuriyatWithFilter("fldPersonalId", PersonalId.ToString(), 0, 0, 0, 0,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_Mamuriyat> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveGroup(List<WCF_PayRoll.OBJ_Mamuriyat> MamuriyatVal,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                foreach (var item in MamuriyatVal)
                {
                    if (item.fldDesc == null)
                        item.fldDesc = "";

                    //ذخیره
                    if (item.fldId == 0)
                    {
                        var q = servic.GetMamuriyatWithFilter("CheckSave", item.fldPersonalId.ToString(), 0, item.fldYear, item.fldMonth, item.fldNobatePardakht,
                            OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            Msg = "ماموریت شخص مورد نظر قبلا ثبت شده است.";
                            MsgTitle = "اخطار";
                            Er = 1;
                        }
                        else
                        {
                            servic.InsertMamuriyat(item.fldPersonalId, item.fldYear, item.fldMonth, item.fldNobatePardakht, item.fldBaBeytute, item.fldBeduneBeytute
                            , item.fldBa10, item.fldBa20, item.fldBa30, item.fldBe10, item.fldBe20, item.fldBe30, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                            Msg = "ذخیره با موفقیت انجام شد.";
                            MsgTitle = "ذخیره موفق";
                        }
                    }
                    else
                    {
                        //ویرایش
                        var q = servic.GetMamuriyatWithFilter("CheckEdit", item.fldPersonalId.ToString(), item.fldId, item.fldYear, item.fldMonth,
                            item.fldNobatePardakht, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            Msg = "ماموریت شخص مورد نظر تکراری است.";
                            MsgTitle = "اخطار";
                            Er = 1;
                        }
                        else
                        {
                            servic.UpdateMamuriyat(item.fldId, item.fldPersonalId, item.fldYear, item.fldMonth, item.fldNobatePardakht, item.fldBaBeytute, item.fldBeduneBeytute
                            , item.fldBa10, item.fldBa20, item.fldBa30, item.fldBe10, item.fldBe20, item.fldBe30, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

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
        public ActionResult DeleteGroup(List<WCF_PayRoll.OBJ_Mamuriyat> MamuriyatGroupVal, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                //حذف
                foreach (var item in MamuriyatGroupVal)
                {
                    servic.DeleteMamuriyat(item.fldId, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
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
        public ActionResult ReloadMamuriyatGroup(string FieldName, int CostCenter_ChartId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_Mamuriyat> data = null;

            data = servic.GetMamuriyatGroupWithFilter(FieldName, Convert.ToInt16(Models.CurrentDate.Year), Convert.ToByte(Models.CurrentDate.Month),
                Models.CurrentDate.nobatPardakht, OrganId/*Convert.ToInt32(Session["OrganId"])*/, CostCenter_ChartId, IP, out Err).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
