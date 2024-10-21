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
    public class KomakGheyerNaghdiController : Controller
    {
        //
        // GET: /PayRoll/KomakGheyerNaghdi/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common_Pay.Comon_PayService CPservic = new WCF_Common_Pay.Comon_PayService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
        WCF_Common_Pay.ClsError CPErr = new WCF_Common_Pay.ClsError();
        public ActionResult Index(string containerId, string Year, string Month,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.KomakGheyerNaghdi();
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
        public ActionResult New(int id, int PersonalId, string Year, string Month, int OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q = Cservic.GetDate(IP, out CErr);
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.PersonalId = PersonalId;
            //PartialView.ViewBag.Sal = q.fldTarikh.Substring(0, 4);
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.OrganId = OrganId;
            return PartialView;
        }
        public ActionResult GroupIndex(string containerId, string Year, string Month, string NoeMostamer, string FieldName, int CostCenter_ChartId, int OrganId)
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
            result.ViewBag.NoeMostamer = NoeMostamer;
            result.ViewBag.FieldName = FieldName;
            result.ViewBag.CostCenter_ChartId = CostCenter_ChartId;
            result.ViewBag.OrganId = OrganId;

            return result;
        }
        public ActionResult CheckPardakht(int PersonalId, short Year, byte Month, int OrganId, string NoeMostamer)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var FieldName = "CheckPardakht";
            if (PersonalId == 0)
            {
                FieldName = "CheckPardakhtGroup";
            }
            var k = servic.GetKomakGheyerNaghdiWithFilter(FieldName, NoeMostamer,0, PersonalId, Year,Month, OrganId, 0, IP, out Err).Count();
            if (k == 0)
            {
                FieldName = "CheckMohasebat_Maliyat_PersonalId";
                if (PersonalId == 0)
                {
                    FieldName = "CheckMohasebat_Maliyat";
                }
                k = servic.GetMohasebatWithFilter(FieldName, Year.ToString(), Month.ToString(), "", OrganId, PersonalId, IP, out Err).Count();

            }
            return Json(new
            {
                AllowGenerate = k > 0 ? 0 : 1
            }, JsonRequestBehavior.AllowGet);
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
        public ActionResult Save(WCF_PayRoll.OBJ_KomakGheyerNaghdi komak, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (komak.fldDesc == null)
                    komak.fldDesc = "";
                komak.fldKhalesPardakhti = komak.fldMablagh - komak.fldMaliyat;
                if (komak.fldId == 0)
                { //ذخیره
                        Msg = servic.InsertKomakGheyerNaghdi(komak.fldPersonalId, komak.fldYear, komak.fldMonth, komak.fldNoeMostamer, komak.fldMablagh,komak.fldKhalesPardakhti,
                            komak.fldMaliyat, komak.fldShomareHesabId, komak.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                        MsgTitle = "ذخیره موفق";
                }
                else
                { //ویرایش
                    Msg = servic.UpdateKomakGheyerNaghdi(komak.fldId, komak.fldPersonalId, komak.fldYear, komak.fldMonth, komak.fldNoeMostamer, komak.fldMablagh, komak.fldKhalesPardakhti,
                        komak.fldMaliyat, komak.fldShomareHesabId, komak.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                        MsgTitle = "ویرایش موفق";
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
                servic.DeleteKomakGheyerNaghdi(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
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
        public ActionResult Details(int Id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetKomakGheyerNaghdiDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
            var NoeMostamar = "0";
            if (q.fldNoeMostamer)
                NoeMostamar = "1";
            return Json(new
            {
                fldId = q.fldId,
                fldPersonalId = q.fldPersonalId,
                fldMablagh = q.fldMablagh,
                fldNoeMostamer = NoeMostamar,
                fldMaliyat = q.fldMaliyat,
                fldYear = q.fldYear.ToString(),
                fldMonth = q.fldMonth.ToString(),
                fldShomareHesabId = q.fldShomareHesabId.ToString(),
                fldShomareHesab=q.fldShomareHesab
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
                        case "Personali":
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
        public ActionResult ReadKomakGheyerNaghdi(StoreRequestParameters parameters, int PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_KomakGheyerNaghdi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_KomakGheyerNaghdi> data1 = null;
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
                        case "fldNameNoeMostamer":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_NameNoeMostamer";
                            break;
                        case "fldMablagh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Mablagh";
                            break;
                        case "fldMaliyat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Maliyat";
                            break;
                        case "fldKhalesPardakhti":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_KhalesPardakhti";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_ShomareHesab";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetKomakGheyerNaghdiWithFilter(field, searchtext, PersonalId, 0, 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                    else
                        data = servic.GetKomakGheyerNaghdiWithFilter(field, searchtext, PersonalId, 0, 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetKomakGheyerNaghdiWithFilter("fldPersonalId", PersonalId.ToString(), 0, 0, 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_KomakGheyerNaghdi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Rload(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_KomakGheyerNaghdi> data = null;

            data = servic.GetKomakGheyerNaghdiWithFilter("fldPersonalId", PersonalId.ToString(),0,0,0,0,Convert.ToInt32(Session["OrganId"]),  100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CalcMaliat(string sal,int mablagh,int personalId,byte mah,bool Type,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string Msg = "", MsgTitle = ""; int MablaghMaliat = 0;
            var k = servic.GetPayPersonalInfoDetail(personalId, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
            var H = CPservic.GetMaxPersonalEstekhdamTypeWithFilter("", k.fldPrs_PersonalInfoId, "", IP, out CPErr).FirstOrDefault();            
            var data = servic.GetSumKomakGheyerNaghdiWithFilter(personalId, Type, mah, Convert.ToInt16(sal), IP, out Err).FirstOrDefault();

            var q = servic.GetCalcMaliyatGheyerNaghdiWithFilter("%"+sal+"%", mablagh, H.fldNoeEstekhdamId, IP, out Err).FirstOrDefault();
            if (q == null)
            {
                Msg = "لطفا جدول مالیاتی سال " + sal + " را وارد کنید.";
                MsgTitle = "هشدار";
            }
            else
            {
                var m = mablagh + data.Jam;
                MablaghMaliat = maliat(m, sal, mah, Type, personalId, H.fldNoeEstekhdamId) - data.JamMaliyat;
            }

            return Json(new
            {
                MablaghMaliat = MablaghMaliat,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        int maliat(int mablagh, string sal, byte mah, bool Type, int personalId, int noeEstekhdam)
        {
            var data = servic.GetSumKomakGheyerNaghdiWithFilter(personalId, Type, mah, Convert.ToInt16(sal), IP, out Err).FirstOrDefault();
            var q = servic.GetCalcMaliyatGheyerNaghdiWithFilter("%" + sal + "%", mablagh, noeEstekhdam, IP, out Err).FirstOrDefault();
            decimal k = 0;
            if (mablagh >= (q.fldAmountFrom * 2))
                k = (mablagh - (q.fldAmountFrom * 2)) * q.Darsad / 100;

            return Convert.ToInt32(k);
        }
        public ActionResult SaveGroup(List<WCF_PayRoll.OBJ_KomakGheyerNaghdi> KomakGheyerNaghdiVal,int  OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                foreach (var item in KomakGheyerNaghdiVal)
                {
                    var k = servic.GetPayPersonalInfoDetail(item.fldPersonalId, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                    var H = CPservic.GetMaxPersonalEstekhdamTypeWithFilter("", k.fldPrs_PersonalInfoId, "", IP, out CPErr).FirstOrDefault();
                    var J = servic.GetCalcMaliyatGheyerNaghdiWithFilter("%" + item.fldYear + "%", item.fldMablagh, H.fldNoeEstekhdamId, IP, out Err).FirstOrDefault();
                    if (J == null)
                    {
                        return Json(new
                        {
                            Msg = "لطفا جدول مالیاتی سال " + item.fldYear + " را وارد کنید.",
                            MsgTitle = "هشدار",
                            Err=0
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                foreach (var item in KomakGheyerNaghdiVal)
                {
                    if (item.fldDesc == null)
                        item.fldDesc = "";
                    var Type = "0";
                    if (item.fldNoeMostamer)
                        Type = "1";

                    item.fldKhalesPardakhti = item.fldMablagh - item.fldMaliyat;
                    //ذخیره
                    if (item.fldId == 0)
                    {
                        var q = servic.GetKomakGheyerNaghdiWithFilter("CheckSave", Type, 0, item.fldPersonalId, item.fldYear, item.fldMonth, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            Msg = "ماموریت شخص مورد نظر قبلا ثبت شده است.";
                            MsgTitle = "اخطار";
                            Er = 1;
                        }
                        else
                        {
                            servic.InsertKomakGheyerNaghdi(item.fldPersonalId, item.fldYear, item.fldMonth, item.fldNoeMostamer, item.fldMablagh, item.fldKhalesPardakhti,
                            item.fldMaliyat, item.fldShomareHesabId, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                            Msg = "ذخیره با موفقیت انجام شد.";
                            MsgTitle = "ذخیره موفق";
                        }
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا",
                                Err = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //ویرایش
                        var q = servic.GetKomakGheyerNaghdiWithFilter("CheckEdit", Type, item.fldId, item.fldPersonalId, item.fldYear, item.fldMonth, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
                        if (q != null)
                        {
                            Msg = "ماموریت شخص مورد نظر تکراری است.";
                            MsgTitle = "اخطار";
                            Er = 1;
                        }
                        else
                        {
                            servic.UpdateKomakGheyerNaghdi(item.fldId, item.fldPersonalId, item.fldYear, item.fldMonth, item.fldNoeMostamer, item.fldMablagh, item.fldKhalesPardakhti,
                            item.fldMaliyat, item.fldShomareHesabId, item.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);

                            Msg = "ویرایش با موفقیت انجام شد.";
                            MsgTitle = "ویرایش موفق";
                        }
                        if (Err.ErrorType)
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
        public ActionResult DeleteGroup(List<WCF_PayRoll.OBJ_KomakGheyerNaghdi> KomakGheyerNaghdiGroupVal, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                //حذف
                foreach (var item in KomakGheyerNaghdiGroupVal)
                {
                    servic.DeleteKomakGheyerNaghdi(item.fldId, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                if (Err.ErrorType == true)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Er=1
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
        public ActionResult ReloadKomakGheyerNaghdiGroup(string FieldName, int CostCenter_ChartId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_KomakGheyerNaghdi> data = null;

            data = servic.GetKomakGheyerNaghdiGroupWithFilter( FieldName, Convert.ToByte(Models.CurrentDate.Month),Convert.ToInt16(Models.CurrentDate.Year),
                Models.CurrentDate.noeMostamar, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, CostCenter_ChartId, IP, out Err).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
