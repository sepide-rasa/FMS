using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Aspose.Cells;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class PardakhthaController : Controller
    {
        //
        // GET: /PayRoll/Pardakhtha/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();

        WCF_Personeli.PersoneliService PersoneliService = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError ErrPersoneli = new WCF_Personeli.ClsError();
        public ActionResult Index(string State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            result.ViewBag.CostCenter = NewFMS.Models.CurrentDate.CostCenter;
            var type = "1";
            if (NewFMS.Models.CurrentDate.noeMostamar)
                type = "2";
            result.ViewBag.noeMostamar = type;
            result.ViewBag.State = State;
            return result;
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetCostCenter(string ID,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetCostCenterWithFilter("fldOrganId", OrganId.ToString(),""/*Session["OrganId"].ToString()*/, 0, IP, out ErrPay).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }

        public void SetYearClass(string Year)
        {
            NewFMS.Models.CurrentDate.Year = Year;
        }

        public void setNobatePardakhtCalss(byte NobatePardakht)
        {
            NewFMS.Models.CurrentDate.nobatPardakht = NobatePardakht;
        }

        public void SetMonthClass(string Month)
        {
            NewFMS.Models.CurrentDate.Month = Month;
        }

        public void SetCostClass(string Value)
        {
            NewFMS.Models.CurrentDate.CostCenter = Value;
        }
        public ActionResult CheckPardakht(int PersonalId, short Year, byte Month, byte NobatPardakht, byte MarhalePardakht, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var FieldName = "CheckPardakht";
            if (PersonalId == 0)
            {
                FieldName = "CheckPardakhtGroup";
            }
            var k = PayServic.GetSayerPardakhtsWithFilter(FieldName, PersonalId.ToString(), 0, Year, Month, NobatPardakht, MarhalePardakht, OrganId, 0, IP, out ErrPay).Count();
            if(k==0)
            {
                 FieldName = "CheckMohasebat_Maliyat_PersonalId";
                if (PersonalId == 0)
                {
                    FieldName = "CheckMohasebat_Maliyat";
                }
                k = PayServic.GetMohasebatWithFilter(FieldName, Year.ToString(), Month.ToString(), "", OrganId, PersonalId, IP, out ErrPay).Count();

            }
            return Json(new
            {
                AllowGenerate = k > 0 ? 0 : 1
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckPardakhtHa(int PersonalId, short Year, byte Month, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var FieldName = "CheckMohasebat_Maliyat_PersonalId";
            if (PersonalId == 0)
            {
                FieldName = "CheckMohasebat_Maliyat";
            }
            var k = PayServic.GetMohasebatWithFilter(FieldName, Year.ToString(), Month.ToString(), "", OrganId, PersonalId, IP, out ErrPay);

            return Json(new
            {
                AllowGenerate = k.Count() > 0 ? 0 : 1
            }, JsonRequestBehavior.AllowGet);
        }
        public void setNoeMostamerCalss(string NoeMostamer)
        {
            bool type = true;
            if (NoeMostamer == "0")
                type = false;
            NewFMS.Models.CurrentDate.noeMostamar = type;
        }
        public ActionResult ReadHeader(StoreRequestParameters parameters, int OrganId)
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
                        data1 = PayServic.GetPayPersonalInfoWithFilter(field, searchtext,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                    else
                        data = PayServic.GetPayPersonalInfoWithFilter(field, searchtext,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = PayServic.GetPayPersonalInfoWithFilter("", "",OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
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
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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
        public ActionResult ReadSayerPardakhts(StoreRequestParameters parameters, int PersonalId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_SayerPardakhts> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_SayerPardakhts> data1 = null;
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
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Title";
                            break;
                        case "fldAmount":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Amount";
                            break;
                        case "fldMaliyat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Maliyat";
                            break;
                        case "fldKhalesPardakhti":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_KhalesPardakhti";
                            break;
                        case "fldNobatePardaktName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_NobatePardaktName";
                            break;
                        case "fldMarhalePardakhtName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_MarhalePardakhtName";
                            break;
                    }
                    if (data != null)
                        data1 = PayServic.GetSayerPardakhtsWithFilter(field, searchtext, PersonalId, 0, 0, 0, 0,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                    else
                        data = PayServic.GetSayerPardakhtsWithFilter(field, searchtext, PersonalId, 0, 0, 0, 0,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = PayServic.GetSayerPardakhtsWithFilter("fldPersonalId", PersonalId.ToString(), 0, 0, 0, 0, 0,OrganId /*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out ErrPay).ToList();
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
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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

            List<WCF_PayRoll.OBJ_SayerPardakhts> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Reload(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_SayerPardakhts> data = null;
            data = PayServic.GetSayerPardakhtsWithFilter("fldPersonalId", PersonalId.ToString(), 0, 0, 0, 0, 0, Convert.ToInt32(Session["OrganId"]), 100, IP, out ErrPay).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReloadPardakhtGroup(short Year, byte Month, string Value, byte nobatPardakht, byte marhalePardakht,
            int CostCenter, string CostCenterType, byte TypeHesab,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var setting = PayServic.GetSettingWithFilter("fldOrganId", OrganId.ToString()/*(Session["OrganId"]).ToString()*/, 0, 1, IP, out ErrPay).FirstOrDefault();
            //var Sh_hesab = PayServic.GetShomareHesabsWithFilter("fldBankId_TypeHesab", setting.fldH_BankFixId.ToString(), TypeHesab.ToString(), 0, 0, 0, 0, 1, Session["Username"].ToString(), Session["Password"].ToString(), out ErrPay).FirstOrDefault();
            List<NewFMS.WCF_PayRoll.OBJ_SayerPardakhtGroup> data = null;
            if (CostCenter == 1)
            {
                data = PayServic.GetSayerPardakhtGroupWithFilter("fldSayerPardakht", Year, Month, nobatPardakht, marhalePardakht, 0, setting.fldH_BankFixId, (TypeHesab),OrganId /*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay).ToList();
            }
            else
            {
                data = PayServic.GetSayerPardakhtGroupWithFilter("fldCostCenterId", Year, Month, nobatPardakht, marhalePardakht, Convert.ToInt32(CostCenterType), setting.fldH_BankFixId,(TypeHesab),OrganId /*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSingle(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;

            try
            {
                PayServic.DeleteSayerPardakhts(Id, Session["Username"].ToString(), Session["Password"].ToString(),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                Msg = "حذف با موفقیت انجام شد.";
                MsgTitle = "حذف موفق";
                if (ErrPay.ErrorType)
                {
                    Msg = ErrPay.ErrorMsg;
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteGroup(List<NewFMS.WCF_PayRoll.OBJ_SayerPardakhts> PardakhtGroupVal,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;
            try
            {
                //حذف
                foreach (var item in PardakhtGroupVal)
                {
                    PayServic.DeleteSayerPardakhts(item.fldId, Session["Username"].ToString(), Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                    if (ErrPay.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = ErrPay.ErrorMsg,
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewSayerPardakhtSingle(int PersonalId, int Id, string Month, string Year,int OrganId)
        {//باز شدن تب

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            var q = PayServic.GetPayPersonalInfoDetail(PersonalId, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
            var q2 = PersoneliService.GetVaziyatEsargariDetail(q.fldEsargariId, IP, out ErrPersoneli);
            result.ViewBag.Id = Id;
            result.ViewBag.PersonalId = PersonalId;
            result.ViewBag.Year = Year;
            result.ViewBag.Month = Month;
            result.ViewBag.OrganId = OrganId;

            result.ViewBag.fldMoafAsMaliyat = q2.fldMoafAsMaliyat;
            return result;
        }

        public ActionResult SingleDetails(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetSayerPardakhtsWithFilter("fldId", Id.ToString(), 0, 0, 0, 0, 0,OrganId/* Convert.ToInt32(Session["OrganId"])*/, 1, IP, out ErrPay).FirstOrDefault();
            var q3 = PayServic.GetMohasebat_PersonalInfoWithFilter("fldSayerPardakhthaId", q.fldId.ToString(), 1, Session["Username"].ToString(), Session["Password"].ToString(),IP, out ErrPay).FirstOrDefault();
            var Sh_hesab = PayServic.GetShomareHesabsWithFilter("fldId", q3.fldShomareHesabId.ToString(), "", q.fldPersonalId.ToString(),OrganId/* Convert.ToInt32(Session["OrganId"])*/, 1, IP, out ErrPay).FirstOrDefault();
            var q1 = PayServic.GetPayPersonalInfoDetail(q.fldPersonalId,OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
            var q2 = PersoneliService.GetVaziyatEsargariDetail(q1.fldEsargariId, IP, out ErrPersoneli);
            return Json(new
            {
                fldId = q.fldId,
                fldPersonalId = q.fldPersonalId,
                fldName = q.fldName_Father,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                fldYear = q.fldYear.ToString(),
                fldMonth = q.fldMonth.ToString(),
                fldTitle = q.fldTitle,
                fldAmount = q.fldAmount,
                fldNobatePardakt = q.fldNobatePardakt.ToString(),
                fldMarhalePardakht = q.fldMarhalePardakht.ToString(),
                fldMaliyat = q.fldMaliyat,
                fldHasMaliyat = q.fldHasMaliyat,
                fldKhalesPardakhti = q.fldKhalesPardakhti,
                fldMoafAzMaliyat = q2.fldMoafAsMaliyat,
                fldDesc = q.fldDesc,
                fldtypehesab = Sh_hesab.fldHesabTypeId.ToString(),
                fldMostamar=q.fldMostamar
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailsHeader(int Id, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetPayPersonalInfoWithFilter("fldId", Id.ToString(),OrganId /*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out ErrPay).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName_Father,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTypeHesab()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetHesabTypeWithFilter("", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult SaveSingle(WCF_PayRoll.OBJ_SayerPardakhts Pardakht, string fldTypeHesab,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;

            try
            {
                if (Pardakht.fldDesc == null)
                    Pardakht.fldDesc = "";

                var setting = PayServic.GetSettingWithFilter("fldOrganId", OrganId.ToString()/*(Session["OrganId"]).ToString()*/, 0, 1, IP, out ErrPay).FirstOrDefault();
                var Sh_hesab = PayServic.GetShomareHesabsWithFilter("fldBankId_HesabTypeId_PayPerson", setting.fldH_BankFixId.ToString(), fldTypeHesab, Pardakht.fldPersonalId.ToString(), OrganId/* Convert.ToInt32(Session["OrganId"])*/, 1, IP, out ErrPay).FirstOrDefault();
                var personal = PayServic.GetPayPersonalInfoDetail(Pardakht.fldPersonalId,OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);

                if (Pardakht.fldId == 0)
                { //ذخیره
                    if (Sh_hesab != null)
                    {
                        var q = PayServic.GetSayerPardakhtsWithFilter("CheckSave", Pardakht.fldPersonalId.ToString(), 0, Pardakht.fldYear, Pardakht.fldMonth, Pardakht.fldNobatePardakt, Pardakht.fldMarhalePardakht,OrganId/* Convert.ToInt32(Session["OrganId"])*/, 1, IP, out ErrPay).FirstOrDefault();
                        if (q != null)
                        {
                            Er = 1;
                            Msg = "اطلاعات پرداخت شخص مورد نظر قبلا ثبت شده است.";
                            MsgTitle = "خطا";
                        }
                        else
                        {
                            PayServic.InsertSayerPardakhts(Pardakht.fldPersonalId, Pardakht.fldYear, Pardakht.fldMonth, Pardakht.fldAmount
                                 , Pardakht.fldTitle, Pardakht.fldNobatePardakt, Pardakht.fldMarhalePardakht, Pardakht.fldHasMaliyat, Pardakht.fldMaliyat, Pardakht.fldKhalesPardakhti, Sh_hesab.fldId, personal.fldCostCenterId,Convert.ToByte( Pardakht.fldMostamar), Pardakht.fldDesc
                                 , Session["Username"].ToString(), Session["Password"].ToString(),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                            Msg = "ذخیره با موفقیت انجام شد.";
                            MsgTitle = "ذخیره موفق";
                            if (ErrPay.ErrorType)
                            {
                                Er = 1;
                                Msg = ErrPay.ErrorMsg;
                                MsgTitle = "خطا";
                            }
                        }
                    }
                    else
                    {
                        Msg = "برای شخص مورد نظر شماره حساب تعریف نشده است.";
                        MsgTitle = "خطا";
                        Er = 1;
                    }
                }
                else
                { //ویرایش
                    if (Sh_hesab != null && Sh_hesab.fldId != null)
                    {
                        var q = PayServic.GetSayerPardakhtsWithFilter("CheckEdit", Pardakht.fldPersonalId.ToString(), Pardakht.fldId, Pardakht.fldYear, Pardakht.fldMonth, Pardakht.fldNobatePardakt, Pardakht.fldMarhalePardakht,OrganId/* Convert.ToInt32(Session["OrganId"])*/, 1,IP, out ErrPay).FirstOrDefault();
                        if (q != null)
                        {
                            Er = 1;
                            Msg = "اطلاعات پرداخت شخص مورد نظر تکراری است.";
                            MsgTitle = "خطا";
                        }
                        else
                        {
                            PayServic.UpdateSayerPardakhts(Pardakht.fldId, Pardakht.fldPersonalId, Pardakht.fldYear, Pardakht.fldMonth, Pardakht.fldAmount
                                , Pardakht.fldTitle, Pardakht.fldNobatePardakt, Pardakht.fldMarhalePardakht, Pardakht.fldHasMaliyat,
                                Pardakht.fldMaliyat, Pardakht.fldKhalesPardakhti, Sh_hesab.fldId, personal.fldCostCenterId, Convert.ToByte(Pardakht.fldMostamar), Pardakht.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                            Msg = "ویرایش با موفقیت انجام شد.";
                            MsgTitle = "ویرایش موفق";
                            if (ErrPay.ErrorType)
                            {
                                Er = 1;
                                Msg = ErrPay.ErrorMsg;
                                MsgTitle = "خطا";
                            }
                        }
                    }
                    else
                    {
                        Msg = "برای شخص مورد نظر شماره حساب تعریف نشده است.";
                        MsgTitle = "خطا";
                        Er = 1;
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveGroup(List<WCF_PayRoll.OBJ_SayerPardakhtGroup> PardakhtGroup, byte? fldCostCenterId,int OrganId,byte fldMostamar)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;
            try
            {
                foreach (var item in PardakhtGroup)
                {
                    if (item.fldSayerPardakhtId == 0)
                    { //ذخیره
                        PayServic.InsertSayerPardakhts(item.fldPersonalInfoId, item.fldYear, item.fldMonth, item.fldAmount
                             , item.fldTitle, item.fldNobatePardakt, item.fldMarhalePardakht, item.fldHasMaliyat, item.fldMaliyat, item.fldKhalesPardakhti, Convert.ToInt32(item.fldShomareHesabsId), fldCostCenterId,fldMostamar, "",
                             Session["Username"].ToString(), Session["Password"].ToString(),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";
                        if (ErrPay.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = ErrPay.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    { //ویرایش
                        PayServic.UpdateSayerPardakhts(item.fldSayerPardakhtId, item.fldPersonalInfoId, item.fldYear, item.fldMonth, item.fldAmount
                            , item.fldTitle, item.fldNobatePardakt, item.fldMarhalePardakht, item.fldHasMaliyat,
                            item.fldMaliyat, item.fldKhalesPardakhti, item.fldShomareHesabsId, fldCostCenterId, fldMostamar, "", Session["Username"].ToString(), Session["Password"].ToString(), OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out ErrPay);
                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";
                        if (ErrPay.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = ErrPay.ErrorMsg,
                                MsgTitle = "خطا"
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SayerPardakhthaSingle(string containerId, short Year, byte Month, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.SayerPardakhts();
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
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpEzafekar()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpTatilKar()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpMamoriyat()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpEydi()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpMorakhasi()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpGhirNaghdi()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult SayerPardakhthaGroup(string containerId, short Year, byte Month, byte marhalePardakht, byte NobatePardakht, byte TypeHesab, string CostCenter, int? CostCenterType, int OrganId, byte Mostamar)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.Year = Year;
            result.ViewBag.Month = Month;
            result.ViewBag.nobatPardakht = NobatePardakht;
            result.ViewBag.marhalePardakht = marhalePardakht;
            result.ViewBag.CostCenter = CostCenter;
            result.ViewBag.TypeHesab = TypeHesab;
            result.ViewBag.CostCenterType = CostCenterType;
            result.ViewBag.OrganId = OrganId;
            result.ViewBag.Mostamar = Mostamar;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
    }
}
