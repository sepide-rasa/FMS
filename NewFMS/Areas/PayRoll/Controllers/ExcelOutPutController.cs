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
using Aspose.Cells;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class ExcelOutPutController : Controller
    {
        //
        // GET: /PayRoll/ExcelOutPut/
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
        WCF_Personeli.PersoneliService Pservic = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError PErr = new WCF_Personeli.ClsError();
        WCF_Common_Pay.Comon_PayService Common_Payservic = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError ErrC_P = new WCF_Common_Pay.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetDate(IP, out CErr);
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Sal = q.fldTarikh.Substring(0, 4);
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

        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out CErr).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);

        }

        public ActionResult GetEsargari()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Pservic.GetVaziyatEsargariWithFilter("", "", 0, IP, out PErr).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);

        }
        public ActionResult GetMadrakTahsili()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetMadrakTahsiliWithFilter("", "", 0,IP, out CErr).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);

        }
        public ActionResult GetNoeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetAnvaEstekhdamWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetStatus()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetStatusWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetAnvaeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetAnvaEstekhdamWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetTypeBime()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetTypeBimeWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetCostCenter()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCostCenterWithFilter("", "", (Session["OrganId"]).ToString(), 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetStatusHoghughi()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetStatusWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetInsuranceWorkShop()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetInsuranceWorkshopWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, WorkShopName = c.fldName });
            return this.Store(q);
        }
        public ActionResult ReadItems(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common_Pay.OBJ_ItemsHoghughi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common_Pay.OBJ_ItemsHoghughi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;

                    }
                    if (data != null)
                        data1 = Common_Payservic.GetItemsHoghughiWithFilter(field, searchtext, 100, IP, out ErrC_P).ToList();
                    else
                        data = Common_Payservic.GetItemsHoghughiWithFilter(field, searchtext, 100, IP, out ErrC_P).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = Common_Payservic.GetItemsHoghughiWithFilter("", "",100, IP, out ErrC_P).ToList();
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

            List<WCF_Common_Pay.OBJ_ItemsHoghughi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        private string where(string Status, string Gender, string Esargari, string Madrak, string NoeEstekhdam, string Bime, string Hazine, string Kargah/*, string Status*/)
        {
            try
            {
                string s = "";
                if (Status != "")
                {
                    s = s + " and com.fn_MaxPersonalStatus(Prs.Prs_tblPersonalInfo.fldId,'kargozini')=" + Status;
                }
                if (Gender != "")
                {
                    s = s + " and Com.tblEmployee_Detail.fldJensiyat=" + Gender;
                }
                if (Esargari != "")
                {
                    s = s + " and fldStatusIsargariId=" + Esargari;
                }
                if (Madrak != "")
                {
                    Madrak = Madrak.TrimEnd(',');
                    s = s + " and tblEmployee_Detail.fldMadrakId in (" + Madrak + " )";
                }
                if (NoeEstekhdam != "")
                {
                    NoeEstekhdam = NoeEstekhdam.TrimEnd(',');
                    s = s + " and com.fn_MaxPersonalTypeEstekhdam(Prs.Prs_tblPersonalInfo.fldId) in (" + NoeEstekhdam + " )";
                }
               /* if (Status != "")
                {
                    s = s + " and com.fn_MaxPersonalStatus(Pay.Pay_tblPersonalInfo.fldId,'hoghoghi')=" + Status;
                }*/
                if (Bime != "")
                {
                    s = s + " and Pay.tblMohasebat_PersonalInfo.fldTypeBimeId=" + Bime;
                }
                if (Hazine != "")
                {
                    s = s + " and Pay.tblMohasebat_PersonalInfo.fldCostCenterId=" + Hazine;
                }
                if (Kargah != "")
                {
                    s = s + " and Pay.tblMohasebat_PersonalInfo.fldInsuranceWorkShopId=" + Kargah;
                }
                return s;
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    return x.InnerException.Message;
                return "";
            }
        }
        public FileResult CreateExcel(string Checked, string Year, string Month, string TaYear, string TaMonth, string NobatePardakht,
            string Status, string Gender, string Esargari, string Madrak, string NoeEstekhdam, string Bime, string Hazine, string Kargah/*, string StatusP*/
            , string ItemEstekhdams,string Organ)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA" };
            int index = 0;
            var StatusCheck = Checked.Split(';');
            var Check = "";
            var Name = ""; var FamilyName = ""; var FatherName = ""; var GenderChar = ""; var ShomareShenasname = ""; var Codemeli = "";
            var TarikhTavalod = ""; var CityName = ""; var Address = ""; var CodePosti = ""; var StatusEsargari = "";
            var ShomarePersoneli = ""; var ReshteTahsili = ""; var TitleStatusJesmi = ""; var StatusNezamVazife = ""; var Post = ""; var PostEjraee = "";
            var RasteShoghli = ""; var ReshteShoghli = ""; var TarikhEstekhdam = ""; var ServiceLocation = ""; var Tabaghe = ""; var MeliyatChar = "";
            var ShMojavezEstekhdam = ""; var TMojavezEstekhdam = ""; var MahalKhedmat = ""; var SanavatKhedmat = ""; var Tel = ""; var Mobile = ""; long JameHokm = 0; string Ertegha = "";

            var TypeBime = ""; var ShomareBime = ""; var BimeOmr = ""; var BimeTakmili = ""; var HamsarKarmand = "";
            var MashagheleSakhtVaZianAvar = ""; var CostCenter = ""; var MazadCSal = ""; var PasAndaz = ""; var MasuliyatPayanKhedmat = "";
            var JobeCode = ""; var InsuranceWorkShop = "";

            var TarikhEjra = ""; var TarikhSodoorHokm = ""; var TarikhEtmam = ""; var StatusTaahol = ""; var ShomarePostSazmani = ""; var MoreGroup = 0;
            var Group = ""; var TedadFarzand = ""; var TedadAfradTahteTakafol = ""; var CodeShoghl = ""; var ShomareHokm = "";

            var hpaye = ""; var sanavat = ""; var paye = ""; var sanavatbasiji = ""; var sanavatisar = ""; var foghshoghl = ""; var takhasosi = ""; var made26 = ""; var modiryati = ""; var barjastegi = ""; var tatbigh = ""; var foghisar = "";
            var abohava = ""; var tashilat = ""; var sakhtikar = ""; var tadil = ""; var riali = ""; var jazb9 = ""; var jazb = ""; var makhsos = ""; var vije = ""; var olad = ""; var ayelemandi = ""; var kharobar = ""; var maskan = "";
            var nobatkari = ""; var bon = ""; var jazbtabsare = ""; var talash = ""; var jebhe = "";
            var janbazi = ""; var sayer = ""; var ezafekar = ""; var mamoriat = ""; var tatilkari = ""; var s_payankhedmat = ""; var ghazai = ""; var ashoora = ""; var zaribtadil = ""; var jazbTakhasosi = "";
            var jazbtadil = ""; var hadaghaltadil = "";

             var Karkard = ""; var Gheybat = "";var TedadEzafeKar = "";  var BimeOmrKarFarma = ""; var Mamooriyat = ""; var TedadTatilKar = ""; var BimeOmrM = ""; var BimeTakmily = ""; 
            var BimeTakmilyKarFarma = ""; var HaghDarmanKarfFarma = ""; var HaghDarmanDolat = ""; var HaghDarman = ""; var BimePersonal = ""; var BimeKarFarma = ""; var BimeBikari = ""; 
            var BimeMashaghel  = ""; var TedadNobatKari = ""; var Mosaede = ""; var GhestVam = "";  var PasAndazM = ""; var MashmolBime = ""; var MashmolMaliyat = "";
            var Mogharari = ""; var Maliyat = ""; var khalesPardakhti = ""; var ShomareHesab = "";

            var TarikhSodoor = ""; var CityTavalod = ""; var MadrakTitle = ""; var SharhEsargari = ""; var NoeEstekhdamTitle = ""; var StatusTitle = "";

            string commandText = " SELECT ISNULL(fldName,'')fldName,ISNULL(fldFamily,'')fldFamily, ISNULL(fldCodemeli,'')fldCodemeli, ISNULL(fldStatus,'')fldStatus,ISNULL(fldFatherName,'')fldFatherName, " +
" ISNULL(fldJensiyat,'')fldJensiyat,ISNULL(fldTarikhTavalod,'')fldTarikhTavalod, ISNULL(fldStatusEsargari,'')fldStatusEsargari,  " +
" ISNULL(fldCodePosti,'')fldCodePosti,ISNULL(fldAddress,'')fldAddress,ISNULL(fldTel,'')fldTel,ISNULL(fldMobile,'')fldMobile,ISNULL(fldMadrakTahsili,'')fldMadrakTahsili,    " +
" ISNULL(fldReshteTahsili,'')fldReshteTahsili,ISNULL(fldRasteShoghli,'')fldRasteShoghli,ISNULL(fldReshteShoghli,'')fldReshteShoghli,    " +
" ISNULL(fldOrganizationalPosts,'')fldOrganizationalPosts,ISNULL(fldTabaghe,'')fldTabaghe,ISNULL(fldShomareMojavezEstekhdam,'')fldShomareMojavezEstekhdam,    " +
" ISNULL(fldTarikhMojavezEstekhdam,'')fldTarikhMojavezEstekhdam,ISNULL(fldMahleKhedmat,'')fldMahleKhedmat,ISNULL(fldTarikhEjra,'')fldTarikhEjra,  " +
" ISNULL(fldTarikhSodoorHokm,'')fldTarikhSodoorHokm,ISNULL(fldTarikhEtmam,'')fldTarikhEtmam,ISNULL(fldTitleEstekhdam,'')fldTitleEstekhdam,  " +
" ISNULL(fldGroup,'')fldGroup,ISNULL(fldMoreGroup,'')fldMoreGroup,ISNULL(fldShomarePostSazmani,'')fldShomarePostSazmani,ISNULL(fldTedadFarzand,'')fldTedadFarzand,ISNULL(fldNezamVazifeTitle,'')fldNezamVazifeTitle,  " +
" ISNULL(fldMeliyatName,'') fldMeliyatName,ISNULL(fldBimeOmrName,'')fldBimeOmrName,ISNULL(fldBimeTakmiliName,'')fldBimeTakmiliName,  " +
" ISNULL(fldMashagheleSakhtVaZianAvarName,'')fldMashagheleSakhtVaZianAvarName,ISNULL(fldMazadCSalName,'')fldMazadCSalName,  " +
" ISNULL(fldPasAndazName,'')fldPasAndazName,ISNULL(fldSanavatPayanKhedmatName,'')fldSanavatPayanKhedmatName,ISNULL(fldHamsarKarmandName,'')fldHamsarKarmandName,  " +
" ISNULL(fldTedadAfradTahteTakafol,'')fldTedadAfradTahteTakafol,ISNULL(fldTypehokm,'')fldTypehokm,ISNULL(fldShomareHokm,'')fldShomareHokm,    " +
" ISNULL(fldDescriptionHokm,'')fldDescriptionHokm,ISNULL(fldCodeShoghl,'')fldCodeShoghl,ISNULL(StatusTaaholTitle,'')StatusTaaholTitle,  " +
" ISNULL(fldCodeShoghliBime,'')fldCodeShoghliBime,ISNULL(fldShomareBime,'')fldShomareBime,ISNULL(fldShPasAndazPersonal,'')fldShPasAndazPersonal,    " +
" ISNULL(fldShPasAndazKarFarma,'')fldShPasAndazKarFarma,ISNULL(fldTedadBime1,'')fldTedadBime1,ISNULL(fldTedadBime2,'')fldTedadBime2,    " +
" ISNULL(fldTedadBime3,'')fldTedadBime3,ISNULL(fldT_Asli,'')fldT_Asli,ISNULL(fldT_70,'')fldT_70,ISNULL(fldT_60,'')fldT_60, ISNULL(fldSumItem,'')fldSumItem,isnull(Ertegha,'')as Ertegha,ISNULL(fldOrganEjraeeName,'')fldOrganEjraeeName, " +
" ISNULL(fldHamsareKarmand,'')fldHamsareKarmand,ISNULL(fldMazad30Sal,'')fldMazad30Sal,ISNULL(StatusEsargariTitle,'')StatusEsargariTitle,  " +
" ISNULL(fldShomareHesab,'')fldShomareHesab,ISNULL(fldShomareKart,'')fldShomareKart,ISNULL(fldWorkShopNum,'')fldWorkShopNum,ISNULL(fldWorkShopName,'')+'_'+ISNULL(fldWorkShopNum,'')fldWorkShopName,  " +
" ISNULL(fldEmployerName,'')fldEmployerName,ISNULL(CostCenterTitle,'')CostCenterTitle,ISNULL(TypeBimeTitle,'')TypeBimeTitle, ISNULL(OrganName,'')OrganName,ISNULL(fldSh_Shenasname,'')fldSh_Shenasname,  " +
" ISNULL(MahaleSodoorName,'')MahaleSodoorName,ISNULL(MahalTavalodName,'')MahalTavalodName,ISNULL(fldMahalTavalodId,'')fldMahalTavalodId,ISNULL(fldMahalSodoorId,'')fldMahalSodoorId,    " +
" ISNULL(fldTarikhSodoor,'')fldTarikhSodoor, ISNULL(fldKarkard,'')fldKarkard,ISNULL(fldGheybat,'')fldGheybat,ISNULL(fldTedadEzafeKar,'')fldTedadEzafeKar,    " +
" ISNULL(fldTedadTatilKar,'')fldTedadTatilKar,ISNULL(CAST(Mamooriyat AS INT),0)Mamooriyat,ISNULL(fldBimeOmrKarFarma,'')fldBimeOmrKarFarma,  " +
" ISNULL(fldBimeOmr,'')fldBimeOmr,ISNULL(fldBimeTakmilyKarFarma,'')fldBimeTakmilyKarFarma,ISNULL(fldBimeTakmily,'')fldBimeTakmily,  " +
" ISNULL(fldHaghDarmanKarfFarma,'')fldHaghDarmanKarfFarma,ISNULL(fldHaghDarmanDolat,'')fldHaghDarmanDolat,ISNULL(fldHaghDarman,'')fldHaghDarman,  " +
" ISNULL(fldBimePersonal,'')fldBimePersonal,ISNULL(fldBimeKarFarma,'')fldBimeKarFarma,ISNULL(fldBimeBikari,'')fldBimeBikari,  " +
" ISNULL(fldBimeMashaghel,'')fldBimeMashaghel,ISNULL(fldTedadNobatKari,'')fldTedadNobatKari,ISNULL(fldMosaede,'')fldMosaede,  " +
" ISNULL(fldGhestVam,'')fldGhestVam,ISNULL(fldNobatPardakht,'')fldNobatPardakht, ISNULL(fldPasAndaz,'')fldPasAndaz,  " +
" ISNULL(fldMashmolBime,'')fldMashmolBime,ISNULL(fldMashmolMaliyat,'')fldMashmolMaliyat,ISNULL(fldMaliyat,'')fldMaliyat,  " +
" ISNULL((motalebat-kosurat),'')fldkhalesPardakhti,ISNULL(fldJobeCode,'')fldJobeCode,ISNULL(fldSh_Personali,'')fldSh_Personali,  " +
" ISNULL(fldOrganPostName,'')fldOrganPostName,ISNULL(fldYear,'')fldYear, ISNULL(fldMonth,'')fldMonth, ISNULL(fldJobDesc,'')+'_'+ISNULL(fldJobeCode,'')fldJobDesc,ISNULL(fldMogharari,'')fldMogharari,ISNULL(fldSharhEsargari,'')fldSharhEsargari,  " +
" ISNULL(fldTarikhEstekhdam,'')fldTarikhEstekhdam,ISNULL(fldSh_MojavezEstekhdam,'')fldSh_MojavezEstekhdam,ISNULL(fldTarikhMajavezEstekhdam,'')fldTarikhMajavezEstekhdam,  " +
" ISNULL(fldStatusIsargariId,'')fldStatusIsargariId,ISNULL(fldTypeBimeId,'')fldTypeBimeId,ISNULL(fldInsuranceWorkShopId,'')fldInsuranceWorkShopId,  " +
" ISNULL(fldCostCenterId,'')fldCostCenterId,ISNULL(fldAnvaeEstekhdamId,'')fldAnvaeEstekhdamId,ISNULL(fldStatusTaaholId,'')fldStatusTaaholId,ISNULL(statusPay,'')statusPay,  " +
" ISNULL(statusPrs,'')statusPrs,  " +
" ISNULL(case when statusPay=1  then N'فعال' when statusPay=2  then N'غیرفعال'  when statusPay=3  then N'بازنشسته'  end ,'')statusPayName,  " +
" ISNULL(case when statusPrs=1  then N'فعال' when statusPrs=2  then N'غیرفعال'  when statusPrs=3  then N'بازنشسته'  end,'')statusPrsName,  " +
" ISNULL(fldJensiyatName,'')fldJensiyatName,ISNULL(fldMadrakId,'')fldMadrakId,ISNULL(fldOrganId,'')fldOrganId,  " +
" ISNULL([h-paye],'')[h-paye],ISNULL([sanavat],'')[sanavat],ISNULL([paye],'')[paye],ISNULL([sanavat-basiji],'')[sanavat-basiji],  " +
" ISNULL([sanavat-isar],'')[sanavat-isar],ISNULL([foghshoghl],'')[foghshoghl],ISNULL([takhasosi],'')[takhasosi],ISNULL([made26],'')[made26],  " +
" ISNULL([modiryati],'')[modiryati],ISNULL([barjastegi],'')[barjastegi],ISNULL([tatbigh],'')[tatbigh],ISNULL([fogh-isar],'')[fogh-isar],  " +
" ISNULL([abohava],'')[abohava],ISNULL([tashilat],'')[tashilat],ISNULL([sakhtikar],'')[sakhtikar],ISNULL([tadil],'')[tadil],  " +
" ISNULL([riali],'')[riali],[jazb9]=ISNULL([jazb9],0)+ISNULL([jazb2],0)+ISNULL([jazb3],0),ISNULL([jazb],'')[jazb]," +
" [makhsos]=ISNULL([makhsos],0)+ISNULL([makhsos2],0)+ISNULL([makhsos3],0)," +
"	[vije]=ISNULL([vije],0)+ISNULL([vije2],0)+ISNULL([vije3],0),ISNULL([khas],'')[khas],ISNULL([karane],'')[karane],ISNULL([refahi],'')[refahi]," +
" ISNULL([olad],'')[olad],ISNULL([ayelemandi],'')[ayelemandi],ISNULL([kharobar],'')[kharobar],ISNULL([maskan],'')[maskan],ISNULL([nobatkari],'')[nobatkari],ISNULL([bon],'')[bon],  " +
" ISNULL([jazb-tabsare],'')[jazb-tabsare],ISNULL([talash],'')[talash],ISNULL([jebhe],'')[jebhe],ISNULL([janbazi],'')[janbazi],ISNULL([sayer],'')[sayer],ISNULL([ezafekar],'')[ezafekar] ,  " +
" ISNULL([mamoriat],'')[mamoriat],ISNULL([tatilkari],'')[tatilkari] ,ISNULL([s_payankhedmat],'')[s_payankhedmat],ISNULL([ghazai],'')ghazai,  " +
" ISNULL([ashoora],'')ashoora,ISNULL([zaribtadil],'')zaribtadil,ISNULL([jazbTakhasosi],'') jazbTakhasosi ,ISNULL([jazbtadil],'') jazbtadil,ISNULL([hadaghaltadil],'') hadaghaltadil,ISNULL([shift],'')[shift] , " +
" ISNULL([band_y],0)+ISNULL([joz1],0)+ISNULL([band6],0)as band_y " +
",ISNULL([tarmim],0)tarmim,ISNULL([hemayatGhazaii],0)hemayatGhazaii,ISNULL([tatbighHokmGhabli],0)tatbighHokmGhabli,ISNULL([jazbTarmim],0)jazbTarmim,ISNULL([VizheTarmim],0)VizheTarmim,ISNULL([KhasTarmim],0)KhasTarmim,ISNULL([mazayaRefahi],0)mazayaRefahi " +
",ISNULL([mahdeKodak],0)mahdeKodak,ISNULL([khoraki],0)khoraki,ISNULL([kalaBehdashti],0)kalaBehdashti,ISNULL([Monasebat],0)Monasebat,ISNULL([javani],0) javani" +
" from(   " +
" SELECT     Com.tblEmployee.fldName, Com.tblEmployee.fldFamily, Com.tblEmployee.fldCodemeli, Com.tblEmployee.fldStatus, Com.tblEmployee_Detail.fldFatherName,    " +
   "                 Com.tblEmployee_Detail.fldJensiyat,Com.tblEmployee_Detail.fldTel,Com.tblEmployee_Detail.fldMobile, Com.tblEmployee_Detail.fldTarikhTavalod, Prs.tblHokm_InfoPersonal_History.fldStatusEsargari,    " +
     "               Prs.tblHokm_InfoPersonal_History.fldCodePosti, Prs.tblHokm_InfoPersonal_History.fldAddress, Prs.tblHokm_InfoPersonal_History.fldMadrakTahsili,    " +
       "             Prs.tblHokm_InfoPersonal_History.fldReshteTahsili, Prs.tblHokm_InfoPersonal_History.fldRasteShoghli, Prs.tblHokm_InfoPersonal_History.fldReshteShoghli,    " +
         "           Prs.tblHokm_InfoPersonal_History.fldOrganizationalPosts, Prs.tblHokm_InfoPersonal_History.fldTabaghe,    " +
           "        Prs.tblHokm_InfoPersonal_History.fldShomareMojavezEstekhdam, Prs.tblHokm_InfoPersonal_History.fldTarikhMojavezEstekhdam,    " +
                /*"        Prs.tblHokm_InfoPersonal_History.fldMahleKhedmat"*/
            "(select fldTitle from com.tblChartOrgan where com.tblChartOrgan.fldid in" +
            "(select fldChartOrganId from com.tblOrganizationalPosts where com.tblOrganizationalPosts.fldid=" +
            "(select fldOrganPostId from prs.Prs_tblPersonalInfo as p where p.fldId=Prs.Prs_tblPersonalInfo.fldid))) as fldMahleKhedmat, Prs.tblPersonalHokm.fldTarikhEjra, Prs.tblPersonalHokm.fldTarikhSodoor AS fldTarikhSodoorHokm, Prs.tblPersonalHokm.fldTarikhEtmam,    " +
              "      Com.tblAnvaEstekhdam.fldTitle AS fldTitleEstekhdam, Prs.tblPersonalHokm.fldGroup, Prs.tblPersonalHokm.fldMoreGroup, Prs.tblPersonalHokm.fldShomarePostSazmani,    " +
                "    Prs.tblPersonalHokm.fldTedadFarzand, Prs.tblPersonalHokm.fldTedadAfradTahteTakafol, Prs.tblPersonalHokm.fldTypehokm, Prs.tblPersonalHokm.fldShomareHokm,    " +
"                       Prs.tblPersonalHokm.fldDescriptionHokm, Prs.tblPersonalHokm.fldCodeShoghl, Com.tblStatusTaahol.fldTitle AS StatusTaaholTitle,    " +
"                      Pay.tblMohasebat_PersonalInfo.fldCodeShoghliBime, Pay.tblMohasebat_PersonalInfo.fldShomareBime, Pay.tblMohasebat_PersonalInfo.fldShPasAndazPersonal,    " +
"                    Pay.tblMohasebat_PersonalInfo.fldShPasAndazKarFarma, Pay.tblMohasebat_PersonalInfo.fldTedadBime1, Pay.tblMohasebat_PersonalInfo.fldTedadBime2,    " +
"                   Pay.tblMohasebat_PersonalInfo.fldTedadBime3, Pay.tblMohasebat_PersonalInfo.fldT_Asli, Pay.tblMohasebat_PersonalInfo.fldT_70,    " +
 "                  Pay.tblMohasebat_PersonalInfo.fldT_60, Pay.tblMohasebat_PersonalInfo.fldHamsareKarmand, Pay.tblMohasebat_PersonalInfo.fldMazad30Sal,    " +
  "                 Prs.tblVaziyatEsargari.fldTitle AS StatusEsargariTitle, com.tblShomareHesabeOmoomi.fldShomareHesab,Com.tblShomareHesabOmoomi_Detail.fldShomareKart,    " +
   "                Pay.tblInsuranceWorkshop.fldWorkShopNum, Pay.tblInsuranceWorkshop.fldWorkShopName, Pay.tblInsuranceWorkshop.fldEmployerName,    " +
    "               Pay.tblCostCenter.fldTitle AS CostCenterTitle, Com.tblTypeBime.fldTitle AS TypeBimeTitle,com.fn_stringDecode( Com.tblOrganization.fldName) AS OrganName, Com.tblEmployee_Detail.fldSh_Shenasname,    " +
     "              tblCity_1.fldName AS MahaleSodoorName, Com.tblCity.fldName AS MahalTavalodName, Com.tblEmployee_Detail.fldMahalTavalodId, Com.tblEmployee_Detail.fldMahalSodoorId,    " +
      "             Com.tblEmployee_Detail.fldTarikhSodoor, Pay.tblMohasebat.fldKarkard, Pay.tblMohasebat.fldGheybat, Pay.tblMohasebat.fldTedadEzafeKar,    " +
       "            cast((Pay.tblMohasebat.fldTedadTatilKar/7.33) as int)fldTedadTatilKar, Pay.tblMohasebat.fldBaBeytute+ Pay.tblMohasebat.fldBedunBeytute AS Mamooriyat, Pay.tblMohasebat.fldBimeOmrKarFarma,    " +
        "           Pay.tblMohasebat.fldBimeOmr, Pay.tblMohasebat.fldBimeTakmilyKarFarma, Pay.tblMohasebat.fldBimeTakmily,  " +
         "          CASE WHEN fldMeliyat = 0 THEN N'غیر ایرانی' ELSE N'ایرانی' END AS fldMeliyatName,  " +
          "          Pay.tblMohasebat.fldHaghDarmanKarfFarma  " +
" + ISNULL(m.HaghDarmanKarfFarma/*(SELECT SUM(fldHaghDarmanKarfFarma) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId)*/,0) AS fldHaghDarmanKarfFarma,  " +
" Pay.tblMohasebat.fldHaghDarmanDolat " +
" + ISNULL(m.HaghDarmanDolat/*(SELECT SUM(fldHaghDarmanDolat) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId)*/,0)as fldHaghDarmanDolat   " +
" ,Pay.tblMohasebat.fldHaghDarman " +
" +ISNULL(m.HaghDarman/*(SELECT SUM(fldHaghDarman) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId)*/,0) AS fldHaghDarman  " +
" ,Pay.tblMohasebat.fldBimePersonal " +
" +ISNULL(m.BimePersonal/*(SELECT SUM(fldBimePersonal) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId)*/,0)AS fldBimePersonal        ,  " +
" Pay.tblMohasebat.fldBimeKarFarma " +
" +ISNULL(m.BimeKarFarma/*(SELECT SUM(fldBimeKarFarma) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId)*/,0) AS fldBimeKarFarma        ,  " +
" Pay.tblMohasebat.fldBimeBikari " +
" +ISNULL(m.BimeBikari/*(SELECT SUM(fldBimeBikari) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId)*/,0) AS fldBimeBikari  " +
" ,Pay.tblMohasebat.fldBimeMashaghel " +
" +ISNULL(m.BimeMashaghel /*(SELECT SUM(fldBimeMashaghel) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId)*/,0) AS fldBimeMashaghel  " +
" ,Pay.tblMohasebat.fldTedadNobatKari                      ,Pay.tblMohasebat.fldMosaede,               " +
" Pay.tblMohasebat.fldGhestVam, Pay.tblMohasebat.fldNobatPardakht,           " +
" Pay.tblMohasebat.fldPasAndaz " +
" +ISNULL(m.PasAndaz/*(SELECT SUM(fldPasAndaz) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId)*/,0)AS fldPasAndaz ,      " +
" Pay.tblMohasebat.fldMashmolBime " +
" +ISNULL(m.MashmolBime /*(SELECT SUM(fldMashmolBime) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId)*/,0)AS fldMashmolBime,  " +
" Pay.tblMohasebat.fldMashmolMaliyat " +
" + ISNULL(m.MashmolMaliyat/*(SELECT SUM(fldMashmolMaliyat) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId)*/,0)AS fldMashmolMaliyat,  " +
" Pay.tblMohasebat.fldMogharari, Pay.tblMohasebat.fldMaliyat " +
" +ISNULL(m.Maliyat /*(SELECT SUM(fldMaliyat) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId)*/,0)AS fldMaliyat    " +
       "         ,isnull((select SUM(ISNULL(fldMablagh,0)) FROM Pay.tblMohasebat_Items    " +
        "     	WHERE fldMohasebatId=tblmohasebat.fldid),0)+   " +
          "   	isnull((select SUM(fldMablagh) FROM [pay].[tblMohasebat_kosorat/MotalebatParam]   " +
           "  	WHERE  fldMohasebatId=tblmohasebat.fldid AND fldKosoratId IS NULL),0) +  " +
            " 	+[fldHaghDarmanKarfFarma]+[fldHaghDarmanDolat]+(tblmohasebat.[fldPasAndaz]/(2))+[fldBimeTakmilyKarFarma]+[fldBimeOmrKarFarma]   " +
             "	+isnull((SELECT      sum(fldMablagh)     " +
             "	FROM            Pay.tblMoavaghat INNER JOIN " +
              "        Pay.tblMoavaghat_Items ON Pay.tblMoavaghat.fldId = Pay.tblMoavaghat_Items.fldMoavaghatId   " +
              "   	 where fldMohasebatId=tblmohasebat.fldid group by fldMohasebatId),0)+isnull((select sum([fldHaghDarmanKarfFarma]+[fldHaghDarmanDolat]+([fldPasAndaz]/(2))) from Pay.tblMoavaghat where fldMohasebatId=tblmohasebat.fldid ),0)   " +
              "   AS motalebat,    " +
              "   	fldMaliyat+isnull((select fldMablagh from pay.tblP_MaliyatManfi where fldMohasebeId=tblMohasebat.fldid),isnull((select sum(fldMaliyat) from pay.tblMoavaghat where fldMohasebatId=tblMohasebat.fldid),0))  + " +
"fldBimePersonal+tblmohasebat.fldBimeOmr+fldMogharari+fldGhestVam+fldBimeTakmily+fldHaghDarman+tblmohasebat.fldPasAndaz+fldMosaede+isnull((select sum(tblMoavaghat.fldHaghDarman+tblMoavaghat.fldPasAndaz+tblMoavaghat.fldBimePersonal) from pay.tblMoavaghat where fldMohasebatId=tblMohasebat.fldid),0)  + " +
" isnull((select sum(fldMablagh) from pay.[tblMohasebat_kosorat/MotalebatParam] where fldMohasebatId=tblMohasebat.fldid and fldKosoratId is not null),0)+   " +
"isnull((select sum(fldMablagh) from pay.tblMohasebat_KosoratBank where fldMohasebatId=tblMohasebat.fldId),0) as kosurat,  " +
" Pay.Pay_tblPersonalInfo.fldJobeCode,    " +
         "         Prs.Prs_tblPersonalInfo.fldSh_Personali, (select fldTitle from com.tblOrganizationalPosts where fldId=Prs.Prs_tblPersonalInfo.fldOrganPostId) AS fldOrganPostName, Pay.tblMohasebat.fldYear, Pay.tblMohasebat.fldMonth,    " +
         "          Com.tblItemsHoghughi.fldNameEN, Prs.Prs_tblPersonalInfo.fldTarikhEstekhdam,  " +
          "         Prs.Prs_tblPersonalInfo.fldSh_MojavezEstekhdam, Prs.Prs_tblPersonalInfo.fldTarikhMajavezEstekhdam, Pay.tblMohasebat_PersonalInfo.fldStatusIsargariId,    " +
           "       Pay.tblMohasebat_PersonalInfo.fldTypeBimeId, Pay.tblMohasebat_PersonalInfo.fldInsuranceWorkShopId, Pay.tblMohasebat_PersonalInfo.fldCostCenterId,    " +
          "         Prs.tblPersonalHokm.fldAnvaeEstekhdamId, Prs.tblPersonalHokm.fldStatusTaaholId " +
          ",      /* com.fn_MaxPersonalStatus(Pay.Pay_tblPersonalInfo.fldId,'hoghoghi') AS*/ statusPay   " +
           "        ,/*com.fn_MaxPersonalStatus(Prs.Prs_tblPersonalInfo.fldId,'kargozini') AS*/ statusPrs   " +
           "       ,CASE WHEN fldJensiyat=1 THEN N'مرد' ELSE N'زن' END AS fldJensiyatName,tblHokm_InfoPersonal_History.fldMadrakId   " +
           "  	,(select sum(fldMablagh)+ISNULL((SELECT  sum(fldMablagh) FROM  Pay.tblMoavaghat_Items INNER JOIN   " +
            "          Pay.tblMoavaghat ON Pay.tblMoavaghat_Items.fldMoavaghatId = Pay.tblMoavaghat.fldId where fldMohasebatId=tblMohasebat.fldid and fldItemEstekhdamId=tblItems_Estekhdam.fldid),0) from pay.tblMohasebat_Items where fldMohasebatId=tblMohasebat.fldid and fldItemEstekhdamId=tblItems_Estekhdam.fldId)fldMablagh   " +
           "  	,tblMohasebat_PersonalInfo.fldOrganId,  " +
          "   CASE WHEN (Pay.Pay_tblPersonalInfo.fldBimeOmr = 1) THEN N'دارد' ELSE N'ندارد' END AS fldBimeOmrName,    " +
           "        CASE WHEN (Pay.Pay_tblPersonalInfo.fldBimeTakmili = 1) THEN N'دارد' ELSE N'ندارد' END AS fldBimeTakmiliName,    " +
           "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldMashagheleSakhtVaZianAvar = 1) THEN N'دارد' ELSE N'ندارد' END AS fldMashagheleSakhtVaZianAvarName,    " +
            "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldMazad30Sal = 1) THEN N'دارد' ELSE N'ندارد' END AS fldMazadCSalName,    " +
            "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldPasAndaz = 1) THEN N'دارد' ELSE N'ندارد' END AS fldPasAndazName,    " +
            "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldSanavatPayanKhedmat = 1) THEN N'دارد' ELSE N'ندارد' END AS fldSanavatPayanKhedmatName,    " +
           "        CASE WHEN (Pay.Pay_tblPersonalInfo.fldHamsarKarmand = 1) THEN N'دارد' ELSE N'ندارد' END AS fldHamsarKarmandName,fldSharhEsargari   " +
     "        	,(select fldJobDesc from pay.tblTabJobOfBime where fldJobCode=pay.Pay_tblPersonalInfo.fldJobeCode)fldJobDesc   " +
    "         	,isnull((select fldTitle from com.tblNezamVazife where fldId= Com.tblEmployee_Detail.fldNezamVazifeId),'') AS fldNezamVazifeTitle   " +
     "           ,h.fldSumItem,(SELECT TOP (1) fldTarikh FROM Prs.tblSavabeghGroupTashvighi WHERE (fldPersonalId = Prs.Prs_tblPersonalInfo.fldid)  ORDER BY fldTarikh DESC)as Ertegha,fldPostEjraee AS fldOrganEjraeeName "+/*AND (fldTarikh <= '1400/08/30') آقای ربیعی اینو گذاشته به درخواست شاهرود برداشتیم*/
    "             FROM         Pay.tblMohasebat_PersonalInfo INNER JOIN   " +
    "              Prs.tblPersonalHokm ON Pay.tblMohasebat_PersonalInfo.fldHokmId = Prs.tblPersonalHokm.fldId INNER JOIN   " +
    "               Pay.tblMohasebat ON Pay.tblMohasebat_PersonalInfo.fldMohasebatId = Pay.tblMohasebat.fldId INNER JOIN   " +
     "              Pay.Pay_tblPersonalInfo ON Pay.tblMohasebat.fldPersonalId = Pay.Pay_tblPersonalInfo.fldId INNER JOIN   " +
     "              Prs.Prs_tblPersonalInfo ON Prs.Prs_tblPersonalInfo.fldId = Pay.Pay_tblPersonalInfo.fldPrs_PersonalInfoId INNER JOIN   " +
     "              Com.tblEmployee ON Prs.Prs_tblPersonalInfo.fldEmployeeId = Com.tblEmployee.fldId INNER JOIN   " +
      "             Com.tblEmployee_Detail ON Com.tblEmployee_Detail.fldEmployeeId = Com.tblEmployee.fldId INNER JOIN   " +
       "            Pay.tblMohasebat_Items ON Pay.tblMohasebat.fldId = Pay.tblMohasebat_Items.fldMohasebatId INNER JOIN   " +
      "             Com.tblItems_Estekhdam ON Pay.tblMohasebat_Items.fldItemEstekhdamId = Com.tblItems_Estekhdam.fldId INNER JOIN   " +
      "             Com.tblItemsHoghughi ON Com.tblItemsHoghughi.fldId = Com.tblItems_Estekhdam.fldItemsHoghughiId INNER JOIN   " +
      "             Prs.tblHokm_InfoPersonal_History ON Prs.tblPersonalHokm.fldId = Prs.tblHokm_InfoPersonal_History.fldPersonalHokmId INNER JOIN   " +
      "             Com.tblAnvaEstekhdam ON Prs.tblPersonalHokm.fldAnvaeEstekhdamId = Com.tblAnvaEstekhdam.fldId INNER JOIN   " +
      "             Com.tblStatusTaahol ON Prs.tblPersonalHokm.fldStatusTaaholId = Com.tblStatusTaahol.fldId INNER JOIN   " +
      "             Pay.tblCostCenter ON Pay.tblMohasebat_PersonalInfo.fldCostCenterId = Pay.tblCostCenter.fldId INNER JOIN  " +
      "             Pay.tblInsuranceWorkshop ON Pay.tblMohasebat_PersonalInfo.fldInsuranceWorkShopId = Pay.tblInsuranceWorkshop.fldId INNER JOIN   " +
      "             Com.tblTypeBime ON Pay.tblMohasebat_PersonalInfo.fldTypeBimeId = Com.tblTypeBime.fldId INNER JOIN   " +
      "             com.tblShomareHesabeOmoomi ON Pay.tblMohasebat_PersonalInfo.fldShomareHesabId =com.tblShomareHesabeOmoomi.fldId INNER JOIN   " +
      "             com.tblShomareHesabOmoomi_Detail ON com.tblShomareHesabeOmoomi.fldId=com.tblShomareHesabOmoomi_Detail.fldShomareHesabId INNER JOIN   " +
      "             Prs.tblVaziyatEsargari ON Pay.tblMohasebat_PersonalInfo.fldStatusIsargariId = Prs.tblVaziyatEsargari.fldId INNER JOIN   " +
      "             Com.tblOrganization ON Pay.tblMohasebat_PersonalInfo.fldOrganId = Com.tblOrganization.fldId INNER JOIN   " +
      "             Com.tblCity ON Com.tblEmployee_Detail.fldMahalTavalodId = Com.tblCity.fldId INNER JOIN   " +
      "             Com.tblCity AS tblCity_1 ON Com.tblEmployee_Detail.fldMahalSodoorId = tblCity_1.fldId   " +
      "             outer apply(SELECT SUM(fldHaghDarmanKarfFarma)as HaghDarmanKarfFarma,SUM(fldHaghDarmanDolat)as HaghDarmanDolat,SUM(fldHaghDarman) as HaghDarman,SUM(fldBimePersonal) as BimePersonal  " +
      "	        ,SUM(fldBimeKarFarma) as BimeKarFarma,SUM(fldBimeBikari) as BimeBikari,SUM(fldBimeMashaghel) as BimeMashaghel,SUM(fldPasAndaz) as PasAndaz,SUM(fldMashmolBime) as MashmolBime  " +
      "		        ,SUM(fldMashmolMaliyat) as MashmolMaliyat,SUM(fldMaliyat) as Maliyat,sum([fldHaghDarmanKarfFarma]+[fldHaghDarmanDolat]+([fldPasAndaz]/(2))) as  motalebat  " +
      "	        ,sum(tblMoavaghat.fldHaghDarman+tblMoavaghat.fldPasAndaz+tblMoavaghat.fldBimePersonal) as kosurat  " +
      "            FROM Pay.tblMoavaghat WHERE Pay.tblMoavaghat.fldMohasebatId=Pay.tblMohasebat.fldId)m  " +
      "           outer apply(SELECT SUM(h.fldMablagh) as fldSumItem FROM Prs.tblHokm_Item as h WHERE fldPersonalHokmId=Prs.tblPersonalHokm.fldId and fldItems_EstekhdamId not in(85,86,150,151,152,153,154))h  " +
      "           outer apply(SELECT top 1 fldStatusId as statusPrs FROM Com.tblPersonalStatus as s where s.fldPrsPersonalInfoId=Prs.Prs_tblPersonalInfo.fldId order by s.fldDateTaghirVaziyat desc)PrsStatus  " +
      "          outer apply(SELECT top 1 fldStatusId as statusPay FROM Com.tblPersonalStatus as s where s.fldPrsPersonalInfoId=Pay.Pay_tblPersonalInfo.fldId order by s.fldDateTaghirVaziyat desc)PayStatus  " +

                  " WHERE tblMohasebat_PersonalInfo.fldOrganId in ( " + Organ + " ) and fldYear>=" + Year + " AND fldMonth>=" + Month + " and fldYear<=" + TaYear + " AND fldMonth<=" + TaMonth + " AND fldNobatPardakht=" + NobatePardakht + where(Status, Gender, Esargari, Madrak, NoeEstekhdam, Bime, Hazine, Kargah/*, Status*/) +
                  "     )t " +
               " PIVOT( " +
" MAX(fldMablagh)  " +
" FOR fldNameEN IN ([h-paye],[sanavat],[paye],[sanavat-basiji],[sanavat-isar],[foghshoghl],[takhasosi],[made26],[modiryati],[barjastegi],[tatbigh],[fogh-isar], " +
" [abohava],[tashilat],[sakhtikar],[tadil],[riali],[jazb9],[jazb],[makhsos],[vije],[olad],[ayelemandi],[kharobar],[maskan],[nobatkari],[bon],[jazb-tabsare],[talash],[jebhe] " +
" ,[janbazi],[sayer],[ezafekar],[mamoriat],[tatilkari],[s_payankhedmat],[ghazai],[ashoora],[zaribtadil],[jazbTakhasosi],[jazbtadil],[hadaghaltadil],[shift] ,[band_y],[joz1],[band6],[jazb2],[jazb3],[vije2],[vije3],[makhsos2],[makhsos3],[khas],[karane],[refahi],[tarmim],[hemayatGhazaii],[tatbighHokmGhabli],[jazbTarmim],[VizheTarmim],[KhasTarmim],[mazayaRefahi],[mahdeKodak],[khoraki],[kalaBehdashti],[Monasebat],[javani]))  p order by fldFamily,fldname ";


//            string commandText = " SELECT ISNULL(fldName,'')fldName,ISNULL(fldFamily,'')fldFamily, ISNULL(fldCodemeli,'')fldCodemeli, ISNULL(fldStatus,'')fldStatus,ISNULL(fldFatherName,'')fldFatherName, " +
// " ISNULL(fldJensiyat,'')fldJensiyat,ISNULL(fldTarikhTavalod,'')fldTarikhTavalod, ISNULL(fldStatusEsargari,'')fldStatusEsargari,  " +
// " ISNULL(fldCodePosti,'')fldCodePosti,ISNULL(fldAddress,'')fldAddress,ISNULL(fldTel,'')fldTel,ISNULL(fldMobile,'')fldMobile,ISNULL(fldMadrakTahsili,'')fldMadrakTahsili,    " +
// " ISNULL(fldReshteTahsili,'')fldReshteTahsili,ISNULL(fldRasteShoghli,'')fldRasteShoghli,ISNULL(fldReshteShoghli,'')fldReshteShoghli,    " +
// " ISNULL(fldOrganizationalPosts,'')fldOrganizationalPosts,ISNULL(fldTabaghe,'')fldTabaghe,ISNULL(fldShomareMojavezEstekhdam,'')fldShomareMojavezEstekhdam,    " +
// " ISNULL(fldTarikhMojavezEstekhdam,'')fldTarikhMojavezEstekhdam,ISNULL(fldMahleKhedmat,'')fldMahleKhedmat,ISNULL(fldTarikhEjra,'')fldTarikhEjra,  " +
// " ISNULL(fldTarikhSodoorHokm,'')fldTarikhSodoorHokm,ISNULL(fldTarikhEtmam,'')fldTarikhEtmam,ISNULL(fldTitleEstekhdam,'')fldTitleEstekhdam,  " +
// " ISNULL(fldGroup,'')fldGroup,ISNULL(fldMoreGroup,'')fldMoreGroup,ISNULL(fldShomarePostSazmani,'')fldShomarePostSazmani,ISNULL(fldTedadFarzand,'')fldTedadFarzand,ISNULL(fldNezamVazifeTitle,'')fldNezamVazifeTitle,  " +
// " ISNULL(fldMeliyatName,'') fldMeliyatName,ISNULL(fldBimeOmrName,'')fldBimeOmrName,ISNULL(fldBimeTakmiliName,'')fldBimeTakmiliName,  " +
// " ISNULL(fldMashagheleSakhtVaZianAvarName,'')fldMashagheleSakhtVaZianAvarName,ISNULL(fldMazadCSalName,'')fldMazadCSalName,  " +
// " ISNULL(fldPasAndazName,'')fldPasAndazName,ISNULL(fldSanavatPayanKhedmatName,'')fldSanavatPayanKhedmatName,ISNULL(fldHamsarKarmandName,'')fldHamsarKarmandName,  " +
// " ISNULL(fldTedadAfradTahteTakafol,'')fldTedadAfradTahteTakafol,ISNULL(fldTypehokm,'')fldTypehokm,ISNULL(fldShomareHokm,'')fldShomareHokm,    " +
// " ISNULL(fldDescriptionHokm,'')fldDescriptionHokm,ISNULL(fldCodeShoghl,'')fldCodeShoghl,ISNULL(StatusTaaholTitle,'')StatusTaaholTitle,  " +
// " ISNULL(fldCodeShoghliBime,'')fldCodeShoghliBime,ISNULL(fldShomareBime,'')fldShomareBime,ISNULL(fldShPasAndazPersonal,'')fldShPasAndazPersonal,    " +
// " ISNULL(fldShPasAndazKarFarma,'')fldShPasAndazKarFarma,ISNULL(fldTedadBime1,'')fldTedadBime1,ISNULL(fldTedadBime2,'')fldTedadBime2,    " +
// " ISNULL(fldTedadBime3,'')fldTedadBime3,ISNULL(fldT_Asli,'')fldT_Asli,ISNULL(fldT_70,'')fldT_70,ISNULL(fldT_60,'')fldT_60, ISNULL(fldSumItem,'')fldSumItem,isnull(Ertegha,'')as Ertegha,ISNULL(fldOrganEjraeeName,'')fldOrganEjraeeName, " +
// " ISNULL(fldHamsareKarmand,'')fldHamsareKarmand,ISNULL(fldMazad30Sal,'')fldMazad30Sal,ISNULL(StatusEsargariTitle,'')StatusEsargariTitle,  " +
// " ISNULL(fldShomareHesab,'')fldShomareHesab,ISNULL(fldShomareKart,'')fldShomareKart,ISNULL(fldWorkShopNum,'')fldWorkShopNum,ISNULL(fldWorkShopName,'')+'_'+ISNULL(fldWorkShopNum,'')fldWorkShopName,  " +
// " ISNULL(fldEmployerName,'')fldEmployerName,ISNULL(CostCenterTitle,'')CostCenterTitle,ISNULL(TypeBimeTitle,'')TypeBimeTitle, ISNULL(OrganName,'')OrganName,ISNULL(fldSh_Shenasname,'')fldSh_Shenasname,  " +
// " ISNULL(MahaleSodoorName,'')MahaleSodoorName,ISNULL(MahalTavalodName,'')MahalTavalodName,ISNULL(fldMahalTavalodId,'')fldMahalTavalodId,ISNULL(fldMahalSodoorId,'')fldMahalSodoorId,    " +
// " ISNULL(fldTarikhSodoor,'')fldTarikhSodoor, ISNULL(fldKarkard,'')fldKarkard,ISNULL(fldGheybat,'')fldGheybat,ISNULL(fldTedadEzafeKar,'')fldTedadEzafeKar,    " +
// " ISNULL(fldTedadTatilKar,'')fldTedadTatilKar,ISNULL(CAST(Mamooriyat AS INT),0)Mamooriyat,ISNULL(fldBimeOmrKarFarma,'')fldBimeOmrKarFarma,  " +
// " ISNULL(fldBimeOmr,'')fldBimeOmr,ISNULL(fldBimeTakmilyKarFarma,'')fldBimeTakmilyKarFarma,ISNULL(fldBimeTakmily,'')fldBimeTakmily,  " +
// " ISNULL(fldHaghDarmanKarfFarma,'')fldHaghDarmanKarfFarma,ISNULL(fldHaghDarmanDolat,'')fldHaghDarmanDolat,ISNULL(fldHaghDarman,'')fldHaghDarman,  " +
// " ISNULL(fldBimePersonal,'')fldBimePersonal,ISNULL(fldBimeKarFarma,'')fldBimeKarFarma,ISNULL(fldBimeBikari,'')fldBimeBikari,  " +
// " ISNULL(fldBimeMashaghel,'')fldBimeMashaghel,ISNULL(fldTedadNobatKari,'')fldTedadNobatKari,ISNULL(fldMosaede,'')fldMosaede,  " +
// " ISNULL(fldGhestVam,'')fldGhestVam,ISNULL(fldNobatPardakht,'')fldNobatPardakht, ISNULL(fldPasAndaz,'')fldPasAndaz,  " +
// " ISNULL(fldMashmolBime,'')fldMashmolBime,ISNULL(fldMashmolMaliyat,'')fldMashmolMaliyat,ISNULL(fldMaliyat,'')fldMaliyat,  " +
// " ISNULL((motalebat-kosurat),'')fldkhalesPardakhti,ISNULL(fldJobeCode,'')fldJobeCode,ISNULL(fldSh_Personali,'')fldSh_Personali,  " +
// " ISNULL(fldOrganPostName,'')fldOrganPostName,ISNULL(fldYear,'')fldYear, ISNULL(fldMonth,'')fldMonth, ISNULL(fldJobDesc,'')+'_'+ISNULL(fldJobeCode,'')fldJobDesc,ISNULL(fldMogharari,'')fldMogharari,ISNULL(fldSharhEsargari,'')fldSharhEsargari,  " +
// " ISNULL(fldTarikhEstekhdam,'')fldTarikhEstekhdam,ISNULL(fldSh_MojavezEstekhdam,'')fldSh_MojavezEstekhdam,ISNULL(fldTarikhMajavezEstekhdam,'')fldTarikhMajavezEstekhdam,  " +
// " ISNULL(fldStatusIsargariId,'')fldStatusIsargariId,ISNULL(fldTypeBimeId,'')fldTypeBimeId,ISNULL(fldInsuranceWorkShopId,'')fldInsuranceWorkShopId,  " +
// " ISNULL(fldCostCenterId,'')fldCostCenterId,ISNULL(fldAnvaeEstekhdamId,'')fldAnvaeEstekhdamId,ISNULL(fldStatusTaaholId,'')fldStatusTaaholId,ISNULL(statusPay,'')statusPay,  " +
// " ISNULL(statusPrs,'')statusPrs,  " +
// " ISNULL(case when statusPay=1  then N'فعال' when statusPay=2  then N'غیرفعال'  when statusPay=3  then N'بازنشسته'  end ,'')statusPayName,  " +
// " ISNULL(case when statusPrs=1  then N'فعال' when statusPrs=2  then N'غیرفعال'  when statusPrs=3  then N'بازنشسته'  end,'')statusPrsName,  " +
// " ISNULL(fldJensiyatName,'')fldJensiyatName,ISNULL(fldMadrakId,'')fldMadrakId,ISNULL(fldOrganId,'')fldOrganId,  " +
// " ISNULL([h-paye],'')[h-paye],ISNULL([sanavat],'')[sanavat],ISNULL([paye],'')[paye],ISNULL([sanavat-basiji],'')[sanavat-basiji],  " +
// " ISNULL([sanavat-isar],'')[sanavat-isar],ISNULL([foghshoghl],'')[foghshoghl],ISNULL([takhasosi],'')[takhasosi],ISNULL([made26],'')[made26],  " +
// " ISNULL([modiryati],'')[modiryati],ISNULL([barjastegi],'')[barjastegi],ISNULL([tatbigh],'')[tatbigh],ISNULL([fogh-isar],'')[fogh-isar],  " +
// " ISNULL([abohava],'')[abohava],ISNULL([tashilat],'')[tashilat],ISNULL([sakhtikar],'')[sakhtikar],ISNULL([tadil],'')[tadil],  " +
// " ISNULL([riali],'')[riali],[jazb9]=ISNULL([jazb9],0)+ISNULL([jazb2],0)+ISNULL([jazb3],0),ISNULL([jazb],'')[jazb]," +
//  " [makhsos]=ISNULL([makhsos],0)+ISNULL([makhsos2],0)+ISNULL([makhsos3],0)," +
//"	[vije]=ISNULL([vije],0)+ISNULL([vije2],0)+ISNULL([vije3],0),ISNULL([khas],'')[khas],ISNULL([karane],'')[karane],ISNULL([refahi],'')[refahi]," +
// " ISNULL([olad],'')[olad],ISNULL([ayelemandi],'')[ayelemandi],ISNULL([kharobar],'')[kharobar],ISNULL([maskan],'')[maskan],ISNULL([nobatkari],'')[nobatkari],ISNULL([bon],'')[bon],  " +
// " ISNULL([jazb-tabsare],'')[jazb-tabsare],ISNULL([talash],'')[talash],ISNULL([jebhe],'')[jebhe],ISNULL([janbazi],'')[janbazi],ISNULL([sayer],'')[sayer],ISNULL([ezafekar],'')[ezafekar] ,  " +
// " ISNULL([mamoriat],'')[mamoriat],ISNULL([tatilkari],'')[tatilkari] ,ISNULL([s_payankhedmat],'')[s_payankhedmat],ISNULL([ghazai],'')ghazai,  " +
// " ISNULL([ashoora],'')ashoora,ISNULL([zaribtadil],'')zaribtadil,ISNULL([jazbTakhasosi],'') jazbTakhasosi ,ISNULL([jazbtadil],'') jazbtadil,ISNULL([hadaghaltadil],'') hadaghaltadil,ISNULL([shift],'')[shift] , " +
// " ISNULL([band_y],0)+ISNULL([joz1],0)+ISNULL([band6],0)as band_y " +
// " from(   " +
// " SELECT     Com.tblEmployee.fldName, Com.tblEmployee.fldFamily, Com.tblEmployee.fldCodemeli, Com.tblEmployee.fldStatus, Com.tblEmployee_Detail.fldFatherName,    " +
//       "                 Com.tblEmployee_Detail.fldJensiyat,Com.tblEmployee_Detail.fldTel,Com.tblEmployee_Detail.fldMobile, Com.tblEmployee_Detail.fldTarikhTavalod, Prs.tblHokm_InfoPersonal_History.fldStatusEsargari,    " +
//         "               Prs.tblHokm_InfoPersonal_History.fldCodePosti, Prs.tblHokm_InfoPersonal_History.fldAddress, Prs.tblHokm_InfoPersonal_History.fldMadrakTahsili,    " +
//           "             Prs.tblHokm_InfoPersonal_History.fldReshteTahsili, Prs.tblHokm_InfoPersonal_History.fldRasteShoghli, Prs.tblHokm_InfoPersonal_History.fldReshteShoghli,    " +
//             "           Prs.tblHokm_InfoPersonal_History.fldOrganizationalPosts, Prs.tblHokm_InfoPersonal_History.fldTabaghe,    " +
//               "        Prs.tblHokm_InfoPersonal_History.fldShomareMojavezEstekhdam, Prs.tblHokm_InfoPersonal_History.fldTarikhMojavezEstekhdam,    " +
//                /*"        Prs.tblHokm_InfoPersonal_History.fldMahleKhedmat"*/
//                "(select fldTitle from com.tblChartOrgan where com.tblChartOrgan.fldid in" +
//                "(select fldChartOrganId from com.tblOrganizationalPosts where com.tblOrganizationalPosts.fldid=" +
//                "(select fldOrganPostId from prs.Prs_tblPersonalInfo as p where p.fldId=Prs.Prs_tblPersonalInfo.fldid))) as fldMahleKhedmat, Prs.tblPersonalHokm.fldTarikhEjra, Prs.tblPersonalHokm.fldTarikhSodoor AS fldTarikhSodoorHokm, Prs.tblPersonalHokm.fldTarikhEtmam,    " +
//                  "      Com.tblAnvaEstekhdam.fldTitle AS fldTitleEstekhdam, Prs.tblPersonalHokm.fldGroup, Prs.tblPersonalHokm.fldMoreGroup, Prs.tblPersonalHokm.fldShomarePostSazmani,    " +
//                    "    Prs.tblPersonalHokm.fldTedadFarzand, Prs.tblPersonalHokm.fldTedadAfradTahteTakafol, Prs.tblPersonalHokm.fldTypehokm, Prs.tblPersonalHokm.fldShomareHokm,    " +
// "                       Prs.tblPersonalHokm.fldDescriptionHokm, Prs.tblPersonalHokm.fldCodeShoghl, Com.tblStatusTaahol.fldTitle AS StatusTaaholTitle,    " +
//  "                      Pay.tblMohasebat_PersonalInfo.fldCodeShoghliBime, Pay.tblMohasebat_PersonalInfo.fldShomareBime, Pay.tblMohasebat_PersonalInfo.fldShPasAndazPersonal,    " +
//   "                    Pay.tblMohasebat_PersonalInfo.fldShPasAndazKarFarma, Pay.tblMohasebat_PersonalInfo.fldTedadBime1, Pay.tblMohasebat_PersonalInfo.fldTedadBime2,    " +
//    "                   Pay.tblMohasebat_PersonalInfo.fldTedadBime3, Pay.tblMohasebat_PersonalInfo.fldT_Asli, Pay.tblMohasebat_PersonalInfo.fldT_70,    " +
//     "                  Pay.tblMohasebat_PersonalInfo.fldT_60, Pay.tblMohasebat_PersonalInfo.fldHamsareKarmand, Pay.tblMohasebat_PersonalInfo.fldMazad30Sal,    " +
//      "                 Prs.tblVaziyatEsargari.fldTitle AS StatusEsargariTitle, com.tblShomareHesabeOmoomi.fldShomareHesab,Com.tblShomareHesabOmoomi_Detail.fldShomareKart,    " +
//       "                Pay.tblInsuranceWorkshop.fldWorkShopNum, Pay.tblInsuranceWorkshop.fldWorkShopName, Pay.tblInsuranceWorkshop.fldEmployerName,    " +
//        "               Pay.tblCostCenter.fldTitle AS CostCenterTitle, Com.tblTypeBime.fldTitle AS TypeBimeTitle,com.fn_stringDecode( Com.tblOrganization.fldName) AS OrganName, Com.tblEmployee_Detail.fldSh_Shenasname,    " +
//         "              tblCity_1.fldName AS MahaleSodoorName, Com.tblCity.fldName AS MahalTavalodName, Com.tblEmployee_Detail.fldMahalTavalodId, Com.tblEmployee_Detail.fldMahalSodoorId,    " +
//          "             Com.tblEmployee_Detail.fldTarikhSodoor, Pay.tblMohasebat.fldKarkard, Pay.tblMohasebat.fldGheybat, Pay.tblMohasebat.fldTedadEzafeKar,    " +
//           "            cast((Pay.tblMohasebat.fldTedadTatilKar/7.33) as int)fldTedadTatilKar, Pay.tblMohasebat.fldBaBeytute+ Pay.tblMohasebat.fldBedunBeytute AS Mamooriyat, Pay.tblMohasebat.fldBimeOmrKarFarma,    " +
//            "           Pay.tblMohasebat.fldBimeOmr, Pay.tblMohasebat.fldBimeTakmilyKarFarma, Pay.tblMohasebat.fldBimeTakmily,  " +
//             "          CASE WHEN fldMeliyat = 0 THEN N'غیر ایرانی' ELSE N'ایرانی' END AS fldMeliyatName,  " +
//              "          Pay.tblMohasebat.fldHaghDarmanKarfFarma+ ISNULL((SELECT SUM(fldHaghDarmanKarfFarma) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId),0) AS fldHaghDarmanKarfFarma,    " +
//               "        Pay.tblMohasebat.fldHaghDarmanDolat+ ISNULL((SELECT SUM(fldHaghDarmanDolat) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId),0)as fldHaghDarmanDolat   " +
//                "       ,Pay.tblMohasebat.fldHaghDarman+ISNULL((SELECT SUM(fldHaghDarman) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId),0)AS fldHaghDarman   " +
//                 "      ,Pay.tblMohasebat.fldBimePersonal+ISNULL((SELECT SUM(fldBimePersonal) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId),0)AS fldBimePersonal   " +
//                  "     ,Pay.tblMohasebat.fldBimeKarFarma+ISNULL((SELECT SUM(fldBimeKarFarma) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId),0) AS fldBimeKarFarma    " +
//                   "    ,Pay.tblMohasebat.fldBimeBikari+ISNULL((SELECT SUM(fldBimeBikari) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId),0) AS fldBimeBikari   " +
// "                      ,Pay.tblMohasebat.fldBimeMashaghel+ ISNULL((SELECT SUM(fldBimeMashaghel) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId),0) AS fldBimeMashaghel   " +
//  "                     ,Pay.tblMohasebat.fldTedadNobatKari   " +
//    "                   ,Pay.tblMohasebat.fldMosaede,    " +
//     "                  Pay.tblMohasebat.fldGhestVam, Pay.tblMohasebat.fldNobatPardakht,    " +
//       "                Pay.tblMohasebat.fldPasAndaz+ISNULL((SELECT SUM(fldPasAndaz) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId),0)AS fldPasAndaz   " +
//        "               , Pay.tblMohasebat.fldMashmolBime+ ISNULL((SELECT SUM(fldMashmolBime) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId),0)AS fldMashmolBime,    " +
//         "              Pay.tblMohasebat.fldMashmolMaliyat+ ISNULL((SELECT SUM(fldMashmolMaliyat) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId),0)AS fldMashmolMaliyat,    " +
//          "             Pay.tblMohasebat.fldMogharari, Pay.tblMohasebat.fldMaliyat+ ISNULL((SELECT SUM(fldMaliyat) FROM Pay.tblMoavaghat WHERE fldMohasebatId=Pay.tblMohasebat.fldId),0)AS fldMaliyat   " +
//           "         ,isnull((select SUM(ISNULL(fldMablagh,0)) FROM Pay.tblMohasebat_Items    " +
//            "     	WHERE fldMohasebatId=tblmohasebat.fldid),0)+   " +
//              "   	isnull((select SUM(fldMablagh) FROM [pay].[tblMohasebat_kosorat/MotalebatParam]   " +
//               "  	WHERE  fldMohasebatId=tblmohasebat.fldid AND fldKosoratId IS NULL),0) +  " +
//                " 	+[fldHaghDarmanKarfFarma]+[fldHaghDarmanDolat]+(tblmohasebat.[fldPasAndaz]/(2))+[fldBimeTakmilyKarFarma]+[fldBimeOmrKarFarma]   " +
//                 "	+isnull((SELECT      sum(fldMablagh)     " +
//                 "	FROM            Pay.tblMoavaghat INNER JOIN " +
//                  "        Pay.tblMoavaghat_Items ON Pay.tblMoavaghat.fldId = Pay.tblMoavaghat_Items.fldMoavaghatId   " +
//                  "   	 where fldMohasebatId=tblmohasebat.fldid group by fldMohasebatId),0)+isnull((select sum([fldHaghDarmanKarfFarma]+[fldHaghDarmanDolat]+([fldPasAndaz]/(2))) from Pay.tblMoavaghat where fldMohasebatId=tblmohasebat.fldid ),0)   " +
//                  "   AS motalebat,    " +
//                  "   	fldMaliyat+isnull((select fldMablagh from pay.tblP_MaliyatManfi where fldMohasebeId=tblMohasebat.fldid),isnull((select sum(fldMaliyat) from pay.tblMoavaghat where fldMohasebatId=tblMohasebat.fldid),0))  + " +
// "fldBimePersonal+tblmohasebat.fldBimeOmr+fldMogharari+fldGhestVam+fldBimeTakmily+fldHaghDarman+tblmohasebat.fldPasAndaz+fldMosaede+isnull((select sum(tblMoavaghat.fldHaghDarman+tblMoavaghat.fldPasAndaz+tblMoavaghat.fldBimePersonal) from pay.tblMoavaghat where fldMohasebatId=tblMohasebat.fldid),0)  + " +
//" isnull((select sum(fldMablagh) from pay.[tblMohasebat_kosorat/MotalebatParam] where fldMohasebatId=tblMohasebat.fldid and fldKosoratId is null),0)+   " +
// "isnull((select sum(fldMablagh) from pay.tblMohasebat_KosoratBank where fldMohasebatId=tblMohasebat.fldId),0) as kosurat,  " +
// " Pay.Pay_tblPersonalInfo.fldJobeCode,    " +
//             "         Prs.Prs_tblPersonalInfo.fldSh_Personali, (select fldTitle from com.tblOrganizationalPosts where fldId=Prs.Prs_tblPersonalInfo.fldOrganPostId) AS fldOrganPostName, Pay.tblMohasebat.fldYear, Pay.tblMohasebat.fldMonth,    " +
//             "          Com.tblItemsHoghughi.fldNameEN, Prs.Prs_tblPersonalInfo.fldTarikhEstekhdam,  " +
//              "         Prs.Prs_tblPersonalInfo.fldSh_MojavezEstekhdam, Prs.Prs_tblPersonalInfo.fldTarikhMajavezEstekhdam, Pay.tblMohasebat_PersonalInfo.fldStatusIsargariId,    " +
//               "       Pay.tblMohasebat_PersonalInfo.fldTypeBimeId, Pay.tblMohasebat_PersonalInfo.fldInsuranceWorkShopId, Pay.tblMohasebat_PersonalInfo.fldCostCenterId,    " +
//              "         Prs.tblPersonalHokm.fldAnvaeEstekhdamId, Prs.tblPersonalHokm.fldStatusTaaholId,com.fn_MaxPersonalStatus(Pay.Pay_tblPersonalInfo.fldId,'hoghoghi') AS statusPay   " +
//               "        ,com.fn_MaxPersonalStatus(Prs.Prs_tblPersonalInfo.fldId,'kargozini') AS statusPrs   " +
//               "       ,CASE WHEN fldJensiyat=1 THEN N'مرد' ELSE N'زن' END AS fldJensiyatName,tblHokm_InfoPersonal_History.fldMadrakId   " +
//               "  	,(select fldMablagh+ISNULL((SELECT  sum(fldMablagh) FROM  Pay.tblMoavaghat_Items INNER JOIN   " +
//                "          Pay.tblMoavaghat ON Pay.tblMoavaghat_Items.fldMoavaghatId = Pay.tblMoavaghat.fldId where fldMohasebatId=tblMohasebat.fldid and fldItemEstekhdamId=tblItems_Estekhdam.fldid),0) from pay.tblMohasebat_Items where fldMohasebatId=tblMohasebat.fldid and fldItemEstekhdamId=tblItems_Estekhdam.fldId)fldMablagh   " +
//               "  	,tblMohasebat_PersonalInfo.fldOrganId,  " +
//              "   CASE WHEN (Pay.Pay_tblPersonalInfo.fldBimeOmr = 1) THEN N'دارد' ELSE N'ندارد' END AS fldBimeOmrName,    " +
//               "        CASE WHEN (Pay.Pay_tblPersonalInfo.fldBimeTakmili = 1) THEN N'دارد' ELSE N'ندارد' END AS fldBimeTakmiliName,    " +
//               "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldMashagheleSakhtVaZianAvar = 1) THEN N'دارد' ELSE N'ندارد' END AS fldMashagheleSakhtVaZianAvarName,    " +
//                "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldMazad30Sal = 1) THEN N'دارد' ELSE N'ندارد' END AS fldMazadCSalName,    " +
//                "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldPasAndaz = 1) THEN N'دارد' ELSE N'ندارد' END AS fldPasAndazName,    " +
//                "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldSanavatPayanKhedmat = 1) THEN N'دارد' ELSE N'ندارد' END AS fldSanavatPayanKhedmatName,    " +
//               "        CASE WHEN (Pay.Pay_tblPersonalInfo.fldHamsarKarmand = 1) THEN N'دارد' ELSE N'ندارد' END AS fldHamsarKarmandName,fldSharhEsargari   " +
//         "        	,(select fldJobDesc from pay.tblTabJobOfBime where fldJobCode=pay.Pay_tblPersonalInfo.fldJobeCode)fldJobDesc   " +
//        "         	,isnull((select fldTitle from com.tblNezamVazife where fldId= Com.tblEmployee_Detail.fldNezamVazifeId),'') AS fldNezamVazifeTitle   " +
//         "           ,fldSumItem,(SELECT TOP (1) fldTarikh FROM Prs.tblSavabeghGroupTashvighi WHERE (fldPersonalId = Prs.Prs_tblPersonalInfo.fldid) AND (fldTarikh <= '1400/08/30')ORDER BY fldTarikh DESC)as Ertegha,fldPostEjraee AS fldOrganEjraeeName " +
//        "             FROM         Pay.tblMohasebat_PersonalInfo INNER JOIN   " +
//        "              Prs.tblPersonalHokm ON Pay.tblMohasebat_PersonalInfo.fldHokmId = Prs.tblPersonalHokm.fldId INNER JOIN   " +
//        "               Pay.tblMohasebat ON Pay.tblMohasebat_PersonalInfo.fldMohasebatId = Pay.tblMohasebat.fldId INNER JOIN   " +
//         "              Pay.Pay_tblPersonalInfo ON Pay.tblMohasebat.fldPersonalId = Pay.Pay_tblPersonalInfo.fldId INNER JOIN   " +
//         "              Prs.Prs_tblPersonalInfo ON Prs.Prs_tblPersonalInfo.fldId = Pay.Pay_tblPersonalInfo.fldPrs_PersonalInfoId INNER JOIN   " +
//         "              Com.tblEmployee ON Prs.Prs_tblPersonalInfo.fldEmployeeId = Com.tblEmployee.fldId INNER JOIN   " +
//          "             Com.tblEmployee_Detail ON Com.tblEmployee_Detail.fldEmployeeId = Com.tblEmployee.fldId INNER JOIN   " +
//           "            Pay.tblMohasebat_Items ON Pay.tblMohasebat.fldId = Pay.tblMohasebat_Items.fldMohasebatId INNER JOIN   " +
//          "             Com.tblItems_Estekhdam ON Pay.tblMohasebat_Items.fldItemEstekhdamId = Com.tblItems_Estekhdam.fldId INNER JOIN   " +
//          "             Com.tblItemsHoghughi ON Com.tblItemsHoghughi.fldId = Com.tblItems_Estekhdam.fldItemsHoghughiId INNER JOIN   " +
//          "             Prs.tblHokm_InfoPersonal_History ON Prs.tblPersonalHokm.fldId = Prs.tblHokm_InfoPersonal_History.fldPersonalHokmId INNER JOIN   " +
//          "             Com.tblAnvaEstekhdam ON Prs.tblPersonalHokm.fldAnvaeEstekhdamId = Com.tblAnvaEstekhdam.fldId INNER JOIN   " +
//          "             Com.tblStatusTaahol ON Prs.tblPersonalHokm.fldStatusTaaholId = Com.tblStatusTaahol.fldId INNER JOIN   " +
//          "             Pay.tblCostCenter ON Pay.tblMohasebat_PersonalInfo.fldCostCenterId = Pay.tblCostCenter.fldId INNER JOIN  " +
//          "             Pay.tblInsuranceWorkshop ON Pay.tblMohasebat_PersonalInfo.fldInsuranceWorkShopId = Pay.tblInsuranceWorkshop.fldId INNER JOIN   " +
//          "             Com.tblTypeBime ON Pay.tblMohasebat_PersonalInfo.fldTypeBimeId = Com.tblTypeBime.fldId INNER JOIN   " +
//          "             com.tblShomareHesabeOmoomi ON Pay.tblMohasebat_PersonalInfo.fldShomareHesabId =com.tblShomareHesabeOmoomi.fldId INNER JOIN   " +
//          "             com.tblShomareHesabOmoomi_Detail ON com.tblShomareHesabeOmoomi.fldId=com.tblShomareHesabOmoomi_Detail.fldShomareHesabId INNER JOIN   " +
//          "             Prs.tblVaziyatEsargari ON Pay.tblMohasebat_PersonalInfo.fldStatusIsargariId = Prs.tblVaziyatEsargari.fldId INNER JOIN   " +
//          "             Com.tblOrganization ON Pay.tblMohasebat_PersonalInfo.fldOrganId = Com.tblOrganization.fldId INNER JOIN   " +
//          "             Com.tblCity ON Com.tblEmployee_Detail.fldMahalTavalodId = Com.tblCity.fldId INNER JOIN   " +
//          "             Com.tblCity AS tblCity_1 ON Com.tblEmployee_Detail.fldMahalSodoorId = tblCity_1.fldId   " +
//                      " WHERE tblMohasebat_PersonalInfo.fldOrganId = " + (Session["OrganId"]).ToString() + " and fldYear>=" + Year + " AND fldMonth>=" + Month + " and fldYear<=" + TaYear + " AND fldMonth<=" + TaMonth + " AND fldNobatPardakht=" + NobatePardakht + where(Status, Gender, Esargari, Madrak, NoeEstekhdam, Bime, Hazine, Kargah/*, Status*/) +
//                      "     )t " +
//                   " PIVOT( " +
// " MAX(fldMablagh)  " +
// " FOR fldNameEN IN ([h-paye],[sanavat],[paye],[sanavat-basiji],[sanavat-isar],[foghshoghl],[takhasosi],[made26],[modiryati],[barjastegi],[tatbigh],[fogh-isar], " +
//" [abohava],[tashilat],[sakhtikar],[tadil],[riali],[jazb9],[jazb],[makhsos],[vije],[olad],[ayelemandi],[kharobar],[maskan],[nobatkari],[bon],[jazb-tabsare],[talash],[jebhe] " +
//" ,[janbazi],[sayer],[ezafekar],[mamoriat],[tatilkari],[s_payankhedmat],[ghazai],[ashoora],[zaribtadil],[jazbTakhasosi],[jazbtadil],[hadaghaltadil],[shift] ,[band_y],[joz1],[band6],[jazb2],[jazb3],[vije2],[vije3],[makhsos2],[makhsos3],[khas],[karane],[refahi]))  p order by fldFamily,fldname ";


            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula(); 
            var per = service.GetData(commandText).Tables[0];
            
            List<NewFMS.Models.AllPersonField> p = new List<NewFMS.Models.AllPersonField>();
            for (int i = 0; i < per.Rows.Count; i++)
            {
                NewFMS.Models.AllPersonField l = new NewFMS.Models.AllPersonField();
                // l.fldId = (int)(per.Rows[i]["fldId"]);
                l.fldName = per.Rows[i]["fldName"] == null ? "" : per.Rows[i]["fldName"].ToString();
                l.fldFamily = per.Rows[i]["fldFamily"] == null ? "" : per.Rows[i]["fldFamily"].ToString();
                l.fldFatherName = per.Rows[i]["fldFatherName"] == null ? "" : per.Rows[i]["fldFatherName"].ToString();
                l.fldCodemeli = per.Rows[i]["fldCodemeli"] == null ? "" : per.Rows[i]["fldCodemeli"].ToString();
                l.fldNameJensiyat = per.Rows[i]["fldJensiyatName"] == null ? "" : per.Rows[i]["fldJensiyatName"].ToString();
                l.fldSh_Shenasname = per.Rows[i]["fldSh_Shenasname"] == null ? "" : per.Rows[i]["fldSh_Shenasname"].ToString();
                l.fldTarikhTavalod = per.Rows[i]["fldTarikhTavalod"] == null ? "" : per.Rows[i]["fldTarikhTavalod"].ToString();
                l.fldNameMahlSodoor = per.Rows[i]["MahaleSodoorName"] == null ? "" : per.Rows[i]["MahaleSodoorName"].ToString();
                l.fldAddress = per.Rows[i]["fldAddress"] == null ? "" : per.Rows[i]["fldAddress"].ToString();
                l.fldTel = per.Rows[i]["fldTel"] == null ? "" : per.Rows[i]["fldTel"].ToString();
                l.fldMobile = per.Rows[i]["fldMobile"] == null ? "" : per.Rows[i]["fldMobile"].ToString();
                l.fldCodePosti = per.Rows[i]["fldCodePosti"] == null ? "" : per.Rows[i]["fldCodePosti"].ToString();
                l.fldVaziyatEsargariTitle = per.Rows[i]["StatusEsargariTitle"] == null ? "" : per.Rows[i]["StatusEsargariTitle"].ToString();
                l.fldSh_Personali = per.Rows[i]["fldSh_Personali"] == null ? "" : per.Rows[i]["fldSh_Personali"].ToString();
                l.fldReshteTahsiliTitle = per.Rows[i]["fldReshteTahsili"] == null ? "" : per.Rows[i]["fldReshteTahsili"].ToString();
                l.fldNezamVazifeTitle = per.Rows[i]["fldNezamVazifeTitle"] == null ? "" : per.Rows[i]["fldNezamVazifeTitle"].ToString();
                l.fldOrganPostName = per.Rows[i]["fldOrganPostName"] == null ? "" : per.Rows[i]["fldOrganPostName"].ToString();
                l.fldNameOrganEjraee = per.Rows[i]["fldOrganEjraeeName"] == null ? "" : per.Rows[i]["fldOrganEjraeeName"].ToString();
                l.fldMahalKhedmat = per.Rows[i]["fldMahleKhedmat"] == null ? "" : per.Rows[i]["fldMahleKhedmat"].ToString();
                l.fldRasteShoghli = per.Rows[i]["fldRasteShoghli"] == null ? "" : per.Rows[i]["fldRasteShoghli"].ToString();
                l.fldReshteShoghli = per.Rows[i]["fldReshteShoghli"] == null ? "" : per.Rows[i]["fldReshteShoghli"].ToString();
                l.fldTarikhEstekhdam = per.Rows[i]["fldTarikhEstekhdam"] == null ? "" : per.Rows[i]["fldTarikhEstekhdam"].ToString();
                l.fldTabaghe = per.Rows[i]["fldTabaghe"] == null ? "" : per.Rows[i]["fldTabaghe"].ToString();
                l.fldMeliyatName = per.Rows[i]["fldMeliyatName"] == null ? "" : per.Rows[i]["fldMeliyatName"].ToString();
                l.fldSh_MojavezEstekhdam = per.Rows[i]["fldSh_MojavezEstekhdam"] == null ? "" : per.Rows[i]["fldSh_MojavezEstekhdam"].ToString();
                l.fldTarikhMajavezEstekhdam = per.Rows[i]["fldTarikhMajavezEstekhdam"] == null ? "" : per.Rows[i]["fldTarikhMajavezEstekhdam"].ToString(); 
                l.fldTarikhSodoor = per.Rows[i]["fldTarikhSodoor"] == null ? "" : per.Rows[i]["fldTarikhSodoor"].ToString();
                l.fldNameMahalTavalod = per.Rows[i]["MahalTavalodName"] == null ? "" : per.Rows[i]["MahalTavalodName"].ToString();
                l.fldMadrakTahsiliTitle = per.Rows[i]["fldMadrakTahsili"] == null ? "" : per.Rows[i]["fldMadrakTahsili"].ToString();
                l.fldSharhEsargari = per.Rows[i]["fldSharhEsargari"] == null ? "" : per.Rows[i]["fldSharhEsargari"].ToString();
                l.fldTitleStatus = per.Rows[i]["statusPrsName"] == null ? "" : per.Rows[i]["statusPrsName"].ToString();

                l.fldTitleTypeBime = per.Rows[i]["TypeBimeTitle"] == null ? "" : per.Rows[i]["TypeBimeTitle"].ToString();
                l.fldShomareBime = per.Rows[i]["fldShomareBime"] == null ? "" : per.Rows[i]["fldShomareBime"].ToString();
                l.fldBimeOmrName = per.Rows[i]["fldBimeOmrName"] == null ? "" : per.Rows[i]["fldBimeOmrName"].ToString();
                l.fldBimeTakmiliName = per.Rows[i]["fldBimeTakmiliName"] == null ? "" : per.Rows[i]["fldBimeTakmiliName"].ToString(); 
                l.fldMashagheleSakhtVaZianAvarName = per.Rows[i]["fldMashagheleSakhtVaZianAvarName"] == null ? "" : per.Rows[i]["fldMashagheleSakhtVaZianAvarName"].ToString();
                l.fldTitleCostCenter = per.Rows[i]["CostCenterTitle"] == null ? "" : per.Rows[i]["CostCenterTitle"].ToString();
                l.fldMazadCSalName = per.Rows[i]["fldMazadCSalName"] == null ? "" : per.Rows[i]["fldMazadCSalName"].ToString();
                l.fldPasAndazName = per.Rows[i]["fldPasAndazName"] == null ? "" : per.Rows[i]["fldPasAndazName"].ToString(); 
                l.fldSanavatPayanKhedmatName = per.Rows[i]["fldSanavatPayanKhedmatName"] == null ? "" : per.Rows[i]["fldSanavatPayanKhedmatName"].ToString();
                l.fldJobDesc = per.Rows[i]["fldJobDesc"] == null ? "" : per.Rows[i]["fldJobDesc"].ToString();
                l.fldJobeCode = per.Rows[i]["fldJobeCode"] == null ? "" : per.Rows[i]["fldJobeCode"].ToString(); 
                l.fldWorkShopName = per.Rows[i]["fldWorkShopName"] == null ? "" : per.Rows[i]["fldWorkShopName"].ToString();
                l.fldHamsarKarmandName = per.Rows[i]["fldHamsarKarmandName"] == null ? "" : per.Rows[i]["fldHamsarKarmandName"].ToString();

                l.fldTarikhEjra = per.Rows[i]["fldTarikhEjra"] == null ? "" : per.Rows[i]["fldTarikhEjra"].ToString();
                l.fldTarikhSodoorHokm = per.Rows[i]["fldTarikhSodoorHokm"] == null ? "" : per.Rows[i]["fldTarikhSodoorHokm"].ToString(); 
                l.fldTarikhEtmam = per.Rows[i]["fldTarikhEtmam"] == null ? "" : per.Rows[i]["fldTarikhEtmam"].ToString(); 
                l.StatusTaaholTitle = per.Rows[i]["StatusTaaholTitle"] == null ? "" : per.Rows[i]["StatusTaaholTitle"].ToString(); 
                l.fldShomarePostSazmani = per.Rows[i]["fldShomarePostSazmani"] == null ? "" : per.Rows[i]["fldShomarePostSazmani"].ToString();
                l.fldMoreGroup = per.Rows[i]["fldMoreGroup"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldMoreGroup"]);
                l.fldGroup = per.Rows[i]["fldGroup"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldGroup"]); 
                l.fldTedadFarzand = per.Rows[i]["fldTedadFarzand"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldTedadFarzand"]);
                l.fldTedadAfradTahteTakafol = per.Rows[i]["fldTedadAfradTahteTakafol"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldTedadAfradTahteTakafol"]);
                l.fldCodeShoghl = per.Rows[i]["fldCodeShoghl"] == null ? "" : per.Rows[i]["fldCodeShoghl"].ToString();
                l.fldShomareHokm = per.Rows[i]["fldShomareHokm"] == null ? "" : per.Rows[i]["fldShomareHokm"].ToString();
                l.TitleNoeEstekhdam = per.Rows[i]["fldTitleEstekhdam"] == null ? "" : per.Rows[i]["fldTitleEstekhdam"].ToString();
                l.fldJameHokm = per.Rows[i]["fldSumItem"] == null ?0 :Convert.ToInt64( per.Rows[i]["fldSumItem"]);
                l.Ertegha = per.Rows[i]["Ertegha"] == null ? "" : per.Rows[i]["Ertegha"].ToString();
                l.hpaye = per.Rows[i]["h-paye"] == null ? 0 : Convert.ToInt32( per.Rows[i]["h-paye"]);
                l.sanavat = per.Rows[i]["sanavat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["sanavat"]);
                l.paye = per.Rows[i]["paye"] == null ? 0 : Convert.ToInt32(per.Rows[i]["paye"]);
                l.sanavatbasiji = per.Rows[i]["sanavat-basiji"] == null ? 0 : Convert.ToInt32(per.Rows[i]["sanavat-basiji"]); 
                l.sanavatisar = per.Rows[i]["sanavat-isar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["sanavat-isar"]); 
                l.foghshoghl = per.Rows[i]["foghshoghl"] == null ? 0 : Convert.ToInt32(per.Rows[i]["foghshoghl"]); 
                l.takhasosi = per.Rows[i]["takhasosi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["takhasosi"]); 
                l.made26 = per.Rows[i]["made26"] == null ? 0 : Convert.ToInt32(per.Rows[i]["made26"]);
                l.modiryati = per.Rows[i]["modiryati"] == null ? 0 : Convert.ToInt32(per.Rows[i]["modiryati"]); 
                l.barjastegi = per.Rows[i]["barjastegi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["barjastegi"]);
                l.tatbigh = per.Rows[i]["tatbigh"] == null ? 0 : Convert.ToInt32(per.Rows[i]["tatbigh"]); 
                l.foghisar = per.Rows[i]["fogh-isar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fogh-isar"]);
                l.abohava = per.Rows[i]["abohava"] == null ? 0 : Convert.ToInt32(per.Rows[i]["abohava"]);
                l.tashilat = per.Rows[i]["tashilat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["tashilat"]);
                l.sakhtikar = per.Rows[i]["sakhtikar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["sakhtikar"]);
                l.tadil = per.Rows[i]["tadil"] == null ? 0 : Convert.ToInt32(per.Rows[i]["tadil"]); 
                l.riali = per.Rows[i]["riali"] == null ? 0 : Convert.ToInt32(per.Rows[i]["riali"]);
                l.jazb9 = per.Rows[i]["jazb9"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jazb9"]); 
                l.jazb = per.Rows[i]["jazb"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jazb"]);
                l.makhsos = per.Rows[i]["makhsos"] == null ? 0 : Convert.ToInt32(per.Rows[i]["makhsos"]); 
                l.vije = per.Rows[i]["vije"] == null ? 0 : Convert.ToInt32(per.Rows[i]["vije"]); 
                l.olad = per.Rows[i]["olad"] == null ? 0 : Convert.ToInt32(per.Rows[i]["olad"]);
                l.ayelemandi = per.Rows[i]["ayelemandi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["ayelemandi"]); 
                l.kharobar = per.Rows[i]["kharobar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["kharobar"]);
                l.maskan = per.Rows[i]["maskan"] == null ? 0 : Convert.ToInt32(per.Rows[i]["maskan"]);
                l.nobatkari = per.Rows[i]["nobatkari"] == null ? 0 : Convert.ToInt32(per.Rows[i]["nobatkari"]);
                l.bon = per.Rows[i]["bon"] == null ? 0 : Convert.ToInt32(per.Rows[i]["bon"]); 
                l.jazbtabsare = per.Rows[i]["jazb-tabsare"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jazb-tabsare"]); 
                l.talash = per.Rows[i]["talash"] == null ? 0 : Convert.ToInt32(per.Rows[i]["talash"]);
                l.jebhe = per.Rows[i]["jebhe"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jebhe"]);
                l.janbazi = per.Rows[i]["janbazi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["janbazi"]);
                l.sayer = per.Rows[i]["sayer"] == null ? 0 : Convert.ToInt32(per.Rows[i]["sayer"]); 
                l.ezafekar = per.Rows[i]["ezafekar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["ezafekar"]);
                l.mamoriat = per.Rows[i]["mamoriat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["mamoriat"]);
                l.tatilkari = per.Rows[i]["tatilkari"] == null ? 0 : Convert.ToInt32(per.Rows[i]["tatilkari"]);
                l.s_payankhedmat = per.Rows[i]["s_payankhedmat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["s_payankhedmat"]);
                l.ghazai = per.Rows[i]["ghazai"] == null ? 0 : Convert.ToInt32(per.Rows[i]["ghazai"]);
                l.ashoora = per.Rows[i]["ashoora"] == null ? 0 : Convert.ToInt32(per.Rows[i]["ashoora"]);
                l.zaribtadil = per.Rows[i]["zaribtadil"] == null ? 0 : Convert.ToInt32(per.Rows[i]["zaribtadil"]); 
                l.jazbTakhasosi = per.Rows[i]["jazbTakhasosi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jazbTakhasosi"]); 
                l.jazbtadil = per.Rows[i]["jazbtadil"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jazbtadil"]);
                l.hadaghaltadil = per.Rows[i]["hadaghaltadil"] == null ? 0 : Convert.ToInt32(per.Rows[i]["hadaghaltadil"]);
                l.refahi = per.Rows[i]["refahi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["refahi"]);
                l.tarmim = per.Rows[i]["tarmim"] == null ? 0 : Convert.ToInt32(per.Rows[i]["tarmim"]);
                l.hemayatGhazaii = per.Rows[i]["hemayatGhazaii"] == null ? 0 : Convert.ToInt32(per.Rows[i]["hemayatGhazaii"]);
                l.tatbighHokmGhabli = per.Rows[i]["tatbighHokmGhabli"] == null ? 0 : Convert.ToInt32(per.Rows[i]["tatbighHokmGhabli"]);
                l.jazbTarmim = per.Rows[i]["jazbTarmim"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jazbTarmim"]);
                l.VizheTarmim = per.Rows[i]["VizheTarmim"] == null ? 0 : Convert.ToInt32(per.Rows[i]["VizheTarmim"]);
                l.KhasTarmim = per.Rows[i]["KhasTarmim"] == null ? 0 : Convert.ToInt32(per.Rows[i]["KhasTarmim"]);
                l.mazayaRefahi = per.Rows[i]["mazayaRefahi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["mazayaRefahi"]);
                l.mahdeKodak = per.Rows[i]["mahdeKodak"] == null ? 0 : Convert.ToInt32(per.Rows[i]["mahdeKodak"]);
                l.khoraki = per.Rows[i]["khoraki"] == null ? 0 : Convert.ToInt32(per.Rows[i]["khoraki"]);
                l.kalaBehdashti = per.Rows[i]["kalaBehdashti"] == null ? 0 : Convert.ToInt32(per.Rows[i]["kalaBehdashti"]);
                l.Monasebat = per.Rows[i]["Monasebat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["Monasebat"]);
                l.javani = per.Rows[i]["javani"] == null ? 0 : Convert.ToInt32(per.Rows[i]["javani"]); 

                l.fldKarkard = per.Rows[i]["fldKarkard"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldKarkard"]); 
                l.fldGheybat = per.Rows[i]["fldGheybat"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldGheybat"]);
                l.fldTedadEzafeKar = per.Rows[i]["fldTedadEzafeKar"] == null ? 0 : Convert.ToDecimal(per.Rows[i]["fldTedadEzafeKar"]);
                l.fldBimeOmrKarFarma = per.Rows[i]["fldBimeOmrKarFarma"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeOmrKarFarma"]); 
                l.Mamooriyat = per.Rows[i]["Mamooriyat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["Mamooriyat"]); 
                l.fldTedadTatilKar = per.Rows[i]["fldTedadTatilKar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldTedadTatilKar"]); 
                l.fldBimeOmrM = per.Rows[i]["fldBimeOmr"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeOmr"]);
                l.fldBimeTakmily = per.Rows[i]["fldBimeTakmily"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeTakmily"]); 
                l.fldBimeTakmilyKarFarma = per.Rows[i]["fldBimeTakmilyKarFarma"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeTakmilyKarFarma"]);
                l.fldHaghDarmanKarfFarma = per.Rows[i]["fldHaghDarmanKarfFarma"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldHaghDarmanKarfFarma"]);
                l.fldHaghDarmanDolat = per.Rows[i]["fldHaghDarmanDolat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldHaghDarmanDolat"]); 
                l.fldHaghDarman = per.Rows[i]["fldHaghDarman"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldHaghDarman"]);
                l.fldBimePersonal = per.Rows[i]["fldBimePersonal"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimePersonal"]); 
                l.fldBimeKarFarma = per.Rows[i]["fldBimeKarFarma"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeKarFarma"]);
                l.fldBimeBikari = per.Rows[i]["fldBimeBikari"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeBikari"]);
                l.fldBimeMashaghel = per.Rows[i]["fldBimeMashaghel"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeMashaghel"]);
                l.fldTedadNobatKari = per.Rows[i]["fldTedadNobatKari"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldTedadNobatKari"]);
                l.fldMosaede = per.Rows[i]["fldMosaede"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldMosaede"]);
                l.fldGhestVam = per.Rows[i]["fldGhestVam"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldGhestVam"]);
                l.fldPasAndazM = per.Rows[i]["fldPasAndaz"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldPasAndaz"]);
                l.fldMashmolBime = per.Rows[i]["fldMashmolBime"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldMashmolBime"]); 
                l.fldMashmolMaliyat = per.Rows[i]["fldMashmolMaliyat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldMashmolMaliyat"]); 
                l.fldMogharari = per.Rows[i]["fldMogharari"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldMogharari"]); 
                l.fldMaliyat = per.Rows[i]["fldMaliyat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldMaliyat"]); 
                l.fldkhalesPardakhti = per.Rows[i]["fldkhalesPardakhti"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldkhalesPardakhti"]);
                l.fldShomareHesab = per.Rows[i]["fldShomareHesab"] == null ? "" : per.Rows[i]["fldShomareHesab"].ToString(); 

                var prs = Pservic.GetPersoneliInfoWithFilter("fldCodemeli", l.fldCodemeli, Convert.ToInt32(Session["OrganId"]), 0, IP, out PErr).FirstOrDefault();
                var Sanavat = Pservic.GetSavabegheSanavateKHedmatWithFilter("fldPersonalId", prs.fldId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out PErr).ToList();
                int CountTarikh = 0;
                foreach (var item in Sanavat)
                {
                    CountTarikh += MyLib.Shamsi.DiffOfShamsiDate(item.fldAzTarikh, item.fldTaTarikh);
                }

                CountTarikh += MyLib.Shamsi.DiffOfShamsiDate(prs.fldTarikhEstekhdam, l.fldTarikhEjra);

                var DayMahSal = Common_Payservic.GetDiffDayMahSalWithFilter(CountTarikh, IP, out ErrC_P).FirstOrDefault();
                var SanavatRasmi = DayMahSal.CountString;
                l.fldSanavtKhedmat = SanavatRasmi;
                p.Add(l);
            }
            var Personal = p.ToList();
            /////////////////////////////////////
            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];

            foreach (var item in StatusCheck)
            {
                // var Personal = servic.GetPersoneliInfoWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 0, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).ToList();
                switch (item)
                {
                    case "Name":
                        Check = "نام";
                        Cell cell = sheet.Cells[alpha[index] + "1"];
                        cell.PutValue(Check);
                        int i = 0;
                        foreach (var _item in Personal)
                        {
                            Name = _item.fldName;
                            Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                            Cell.PutValue(Name);
                            i++;
                        }
                        index++;
                        break;
                    case "Family":
                        Check = "نام خانوادگی";
                        Cell cell1 = sheet.Cells[alpha[index] + "1"];
                        cell1.PutValue(Check);
                        int i1 = 0;
                        foreach (var _item in Personal)
                        {
                            FamilyName = _item.fldFamily;
                            Cell Cell = sheet.Cells[alpha[index] + (i1 + 2)];
                            Cell.PutValue(FamilyName);
                            i1++;
                        }
                        index++;
                        break;
                    case "Father":
                        Check = "نام پدر";
                        Cell cell2 = sheet.Cells[alpha[index] + "1"];
                        cell2.PutValue(Check);
                        int i2 = 0;
                        foreach (var _item in Personal)
                        {
                            FatherName = _item.fldFatherName;
                            Cell Cell = sheet.Cells[alpha[index] + (i2 + 2)];
                            Cell.PutValue(FatherName);
                            i2++;
                        }
                        index++;
                        break;
                    case "MeliCode":
                        Check = "کدملی";
                        Cell cell3 = sheet.Cells[alpha[index] + "1"];
                        cell3.PutValue(Check);
                        int i3 = 0;
                        foreach (var _item in Personal)
                        {
                            Codemeli = _item.fldCodemeli;
                            Cell Cell = sheet.Cells[alpha[index] + (i3 + 2)];
                            Cell.PutValue(Codemeli);
                            i3++;
                        }
                        index++;
                        break;
                    case "Jensiyat":
                        Check = "جنسیت";
                        Cell cell4 = sheet.Cells[alpha[index] + "1"];
                        cell4.PutValue(Check);
                        int i4 = 0;
                        foreach (var _item in Personal)
                        {
                            GenderChar = _item.fldNameJensiyat;
                            Cell Cell = sheet.Cells[alpha[index] + (i4 + 2)];
                            Cell.PutValue(GenderChar);
                            i4++;
                        }
                        index++;
                        break;
                    case "ShomareShenasname":
                        Check = "شماره شناسنامه";
                        Cell cell5 = sheet.Cells[alpha[index] + "1"];
                        cell5.PutValue(Check);
                        int i5 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomareShenasname = _item.fldSh_Shenasname;
                            Cell Cell = sheet.Cells[alpha[index] + (i5 + 2)];
                            Cell.PutValue(ShomareShenasname);
                            i5++;
                        }
                        index++;
                        break;
                    case "TarikhTavalod":
                        Check = "تاریخ تولد";
                        Cell cell6 = sheet.Cells[alpha[index] + "1"];
                        cell6.PutValue(Check);
                        int i6 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhTavalod = _item.fldTarikhTavalod;
                            Cell Cell = sheet.Cells[alpha[index] + (i6 + 2)];
                            Cell.PutValue(TarikhTavalod);
                            i6++;
                        }
                        index++;
                        break;
                    case "City":
                        Check = "شهر محل صدور";
                        Cell cell7 = sheet.Cells[alpha[index] + "1"];
                        cell7.PutValue(Check);
                        int i7 = 0;
                        foreach (var _item in Personal)
                        {
                            CityName = _item.fldNameMahlSodoor;
                            Cell Cell = sheet.Cells[alpha[index] + (i7 + 2)];
                            Cell.PutValue(CityName);
                            i7++;
                        }
                        index++;
                        break;
                    case "Addres":
                        Check = "آدرس";
                        Cell cell8 = sheet.Cells[alpha[index] + "1"];
                        cell8.PutValue(Check);
                        int i8 = 0;
                        foreach (var _item in Personal)
                        {
                            Address = _item.fldAddress;
                            Cell Cell = sheet.Cells[alpha[index] + (i8 + 2)];
                            Cell.PutValue(Address);
                            i8++;
                        }
                        index++;
                        break;
                    case "CodePosti":
                        Check = "کد پستی";
                        Cell cell9 = sheet.Cells[alpha[index] + "1"];
                        cell9.PutValue(Check);
                        int i9 = 0;
                        foreach (var _item in Personal)
                        {
                            CodePosti = _item.fldCodePosti;
                            Cell Cell = sheet.Cells[alpha[index] + (i9 + 2)];
                            Cell.PutValue(CodePosti);
                            i9++;
                        }
                        index++;
                        break;
                    case "StatusEsargari":
                        Check = "وضعیت ایثارگری";
                        Cell cell10 = sheet.Cells[alpha[index] + "1"];
                        cell10.PutValue(Check);
                        int i10 = 0;
                        foreach (var _item in Personal)
                        {
                            StatusEsargari = _item.fldVaziyatEsargariTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (i10 + 2)];
                            Cell.PutValue(StatusEsargari);
                            i10++;
                        }
                        index++;
                        break;

                    case "ShomarePersoneli":
                        Check = "شماره پرسنلی";
                        Cell cell11 = sheet.Cells[alpha[index] + "1"];
                        cell11.PutValue(Check);
                        int i11 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomarePersoneli = _item.fldSh_Personali;
                            Cell Cell = sheet.Cells[alpha[index] + (i11 + 2)];
                            Cell.PutValue(ShomarePersoneli);
                            i11++;
                        }
                        index++;
                        break;
                    case "ReshteTahsili":
                        Check = "رشته تحصیلی";
                        Cell cell12 = sheet.Cells[alpha[index] + "1"];
                        cell12.PutValue(Check);
                        int i12 = 0;
                        foreach (var _item in Personal)
                        {
                            ReshteTahsili = _item.fldReshteTahsiliTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (i12 + 2)];
                            Cell.PutValue(ReshteTahsili);
                            i12++;
                        }
                        index++;
                        break;
                    case "StatusNezamVazife":
                        Check = "وضعیت نظام وظیفه";
                        Cell cell13 = sheet.Cells[alpha[index] + "1"];
                        cell13.PutValue(Check);
                        int i13 = 0;
                        foreach (var _item in Personal)
                        {
                            StatusNezamVazife = _item.fldNezamVazifeTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (i13 + 2)];
                            Cell.PutValue(StatusNezamVazife);
                            i13++;
                        }
                        index++;
                        break;
                    case "OrganizationalPosts":
                        Check = "پست مصوب";
                        Cell cell14 = sheet.Cells[alpha[index] + "1"];
                        cell14.PutValue(Check);
                        int i14 = 0;
                        foreach (var _item in Personal)
                        {
                            Post = _item.fldOrganPostName;
                            Cell Cell = sheet.Cells[alpha[index] + (i14 + 2)];
                            Cell.PutValue(Post);
                            i14++;
                        }
                        index++;
                        break;
                    case "RasteShoghli":
                        Check = "رسته شغلی";
                        Cell cell15 = sheet.Cells[alpha[index] + "1"];
                        cell15.PutValue(Check);
                        int i15 = 0;
                        foreach (var _item in Personal)
                        {
                            RasteShoghli = _item.fldRasteShoghli;
                            Cell Cell = sheet.Cells[alpha[index] + (i15 + 2)];
                            Cell.PutValue(RasteShoghli);
                            i15++;
                        }
                        index++;
                        break;
                    case "ReshteShoghli":
                        Check = "رشته شغلی";
                        Cell cell16 = sheet.Cells[alpha[index] + "1"];
                        cell16.PutValue(Check);
                        int i16 = 0;
                        foreach (var _item in Personal)
                        {
                            ReshteShoghli = _item.fldReshteShoghli;
                            Cell Cell = sheet.Cells[alpha[index] + (i16 + 2)];
                            Cell.PutValue(ReshteShoghli);
                            i16++;
                        }
                        index++;
                        break;
                    case "TarikhEstekhdam":
                        Check = "تاریخ استخدام";
                        Cell cell17 = sheet.Cells[alpha[index] + "1"];
                        cell17.PutValue(Check);
                        int i17 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhEstekhdam = _item.fldTarikhEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (i17 + 2)];
                            Cell.PutValue(TarikhEstekhdam);
                            i17++;
                        }
                        index++;
                        break;

                    case "Tabaghe":
                        Check = "طبقه";
                        Cell cell18 = sheet.Cells[alpha[index] + "1"];
                        cell18.PutValue(Check);
                        int i18 = 0;
                        foreach (var _item in Personal)
                        {
                            Tabaghe = _item.fldTabaghe;
                            Cell Cell = sheet.Cells[alpha[index] + (i18 + 2)];
                            Cell.PutValue(Tabaghe);
                            i18++;
                        }
                        index++;
                        break;
                    case "Meliyat":
                        Check = "ملیت";
                        Cell cell19 = sheet.Cells[alpha[index] + "1"];
                        cell19.PutValue(Check);
                        int i19 = 0;
                        foreach (var _item in Personal)
                        {
                            MeliyatChar = _item.fldMeliyatName;
                            Cell Cell = sheet.Cells[alpha[index] + (i19 + 2)];
                            Cell.PutValue(MeliyatChar);
                            i19++;
                        }
                        index++;
                        break;
                    case "ShMojavezEstekhdam":
                        Check = "شماره مجوز استخدام";
                        Cell cell20 = sheet.Cells[alpha[index] + "1"];
                        cell20.PutValue(Check);
                        int i20 = 0;
                        foreach (var _item in Personal)
                        {
                            ShMojavezEstekhdam = _item.fldSh_MojavezEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (i20 + 2)];
                            Cell.PutValue(ShMojavezEstekhdam);
                            i20++;
                        }
                        index++;
                        break;
                    case "TMojavezEstekhdam":
                        Check = "تاریخ مجوز استخدام";
                        Cell cell21 = sheet.Cells[alpha[index] + "1"];
                        cell21.PutValue(Check);
                        int i21 = 0;
                        foreach (var _item in Personal)
                        {
                            TMojavezEstekhdam = _item.fldTarikhMajavezEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (i21 + 2)];
                            Cell.PutValue(TMojavezEstekhdam);
                            i21++;
                        }
                        index++;
                        break;
                    case "StatusTitle":
                        Check = "وضعیت";
                        Cell cell23 = sheet.Cells[alpha[index] + "1"];
                        cell23.PutValue(Check);
                        int i23 = 0;
                        foreach (var _item in Personal)
                        {
                            StatusTitle = _item.fldTitleStatus;
                            Cell Cell = sheet.Cells[alpha[index] + (i23 + 2)];
                            Cell.PutValue(StatusTitle);
                            i23++;
                        }
                        index++;
                        break;
                    case "TarikhSodoor":
                        Check = "تاریخ صدور";
                        Cell cell24 = sheet.Cells[alpha[index] + "1"];
                        cell24.PutValue(Check);
                        int i24 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhSodoor = _item.fldTarikhSodoor;
                            Cell Cell = sheet.Cells[alpha[index] + (i24 + 2)];
                            Cell.PutValue(TarikhSodoor);
                            i24++;
                        }
                        index++;
                        break;
                    case "CityTavalod":
                        Check = "شهر محل تولد";
                        Cell cell25 = sheet.Cells[alpha[index] + "1"];
                        cell25.PutValue(Check);
                        int i25 = 0;
                        foreach (var _item in Personal)
                        {
                            CityTavalod = _item.fldNameMahalTavalod;
                            Cell Cell = sheet.Cells[alpha[index] + (i25 + 2)];
                            Cell.PutValue(CityTavalod);
                            i25++;
                        }
                        index++;
                        break;
                    case "MadrakTitle":
                        Check = "مقطع تحصیلی";
                        Cell cell26 = sheet.Cells[alpha[index] + "1"];
                        cell26.PutValue(Check);
                        int i26 = 0;
                        foreach (var _item in Personal)
                        {
                            MadrakTitle = _item.fldMadrakTahsiliTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (i26 + 2)];
                            Cell.PutValue(MadrakTitle);
                            i26++;
                        }
                        index++;
                        break;
                    case "SharhEsargari":
                        Check = "شرح ایثارگری";
                        Cell cell27 = sheet.Cells[alpha[index] + "1"];
                        cell27.PutValue(Check);
                        int i27 = 0;
                        foreach (var _item in Personal)
                        {
                            SharhEsargari = _item.fldSharhEsargari;
                            Cell Cell = sheet.Cells[alpha[index] + (i27 + 2)];
                            Cell.PutValue(SharhEsargari);
                            i27++;
                        }
                        index++;
                        break;
                    case "OrganizationalPostsEjraee":
                        Check = "پست اجرائی";
                        Cell cell28 = sheet.Cells[alpha[index] + "1"];
                        cell28.PutValue(Check);
                        int i28 = 0;
                        foreach (var _item in Personal)
                        {
                            PostEjraee = _item.fldNameOrganEjraee;
                            Cell Cell = sheet.Cells[alpha[index] + (i28 + 2)];
                            Cell.PutValue(PostEjraee);
                            i28++;
                        }
                        index++;
                        break;
                    case "MahalKhedmat":
                        Check = "محل خدمت";
                        Cell cell29 = sheet.Cells[alpha[index] + "1"];
                        cell29.PutValue(Check);
                        int i29 = 0;
                        foreach (var _item in Personal)
                        {
                            MahalKhedmat = _item.fldMahalKhedmat;
                            Cell Cell = sheet.Cells[alpha[index] + (i29 + 2)];
                            Cell.PutValue(MahalKhedmat);
                            i29++;
                        }
                        index++;
                        break;
                    case "SanavatKhedmat":
                        Check = "سنوات خدمت";
                        Cell cell30 = sheet.Cells[alpha[index] + "1"];
                        cell30.PutValue(Check);
                        int i30 = 0;
                        foreach (var _item in Personal)
                        {
                            SanavatKhedmat = _item.fldSanavtKhedmat;
                            Cell Cell = sheet.Cells[alpha[index] + (i30 + 2)];
                            Cell.PutValue(SanavatKhedmat);
                            i30++;
                        }
                        index++;
                        break;
                    case "Tel":
                        Check = "تلفن";
                        Cell cell31 = sheet.Cells[alpha[index] + "1"];
                        cell31.PutValue(Check);
                        int i31 = 0;
                        foreach (var _item in Personal)
                        {
                            Tel = _item.fldTel;
                            Cell Cell = sheet.Cells[alpha[index] + (i31 + 2)];
                            Cell.PutValue(Tel);
                            i31++;
                        }
                        index++;
                        break;
                    case "Mobile":
                        Check = "موبایل";
                        Cell cell32 = sheet.Cells[alpha[index] + "1"];
                        cell32.PutValue(Check);
                        int i32 = 0;
                        foreach (var _item in Personal)
                        {
                            Mobile = _item.fldMobile;
                            Cell Cell = sheet.Cells[alpha[index] + (i32 + 2)];
                            Cell.PutValue(Mobile);
                            i32++;
                        }
                        index++;
                        break;
                    /////////////////
                    case "TypeBime":
                        Check = "نوع بیمه";
                        Cell pcell1 = sheet.Cells[alpha[index] + "1"];
                        pcell1.PutValue(Check);
                        int j1 = 0;
                        foreach (var _item in Personal)
                        {
                            TypeBime = _item.fldTitleTypeBime;
                            Cell Cell = sheet.Cells[alpha[index] + (j1 + 2)];
                            Cell.PutValue(TypeBime);
                            j1++;
                        }
                        index++;
                        break;
                    case "ShomareBime":
                        Check = "شماره بیمه";
                        Cell pcell2 = sheet.Cells[alpha[index] + "1"];
                        pcell2.PutValue(Check);
                        int j2 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomareBime = _item.fldShomareBime;
                            Cell Cell = sheet.Cells[alpha[index] + (j2 + 2)];
                            Cell.PutValue(ShomareBime);
                            j2++;
                        }
                        index++;
                        break;
                    case "BimeOmr":
                        Check = "بیمه عمر";
                        Cell pcell3 = sheet.Cells[alpha[index] + "1"];
                        pcell3.PutValue(Check);
                        int j3 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeOmr = _item.fldBimeOmrName;
                            Cell Cell = sheet.Cells[alpha[index] + (j3 + 2)];
                            Cell.PutValue(BimeOmr);
                            j3++;
                        }
                        index++;
                        break;
                    case "BimeTakmili":
                        Check = "بیمه تکمیلی";
                        Cell pcell4 = sheet.Cells[alpha[index] + "1"];
                        pcell4.PutValue(Check);
                        int j4 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeTakmili = _item.fldBimeTakmiliName;
                            Cell Cell = sheet.Cells[alpha[index] + (j4 + 2)];
                            Cell.PutValue(BimeTakmili);
                            j4++;
                        }
                        index++;
                        break;
                    case "MashagheleSakhtVaZianAvar":
                        Check = "مشاغل سخت و زیان آور";
                        Cell pcell5 = sheet.Cells[alpha[index] + "1"];
                        pcell5.PutValue(Check);
                        int j5 = 0;
                        foreach (var _item in Personal)
                        {
                            MashagheleSakhtVaZianAvar = _item.fldMashagheleSakhtVaZianAvarName;
                            Cell Cell = sheet.Cells[alpha[index] + (j5 + 2)];
                            Cell.PutValue(MashagheleSakhtVaZianAvar);
                            j5++;
                        }
                        index++;
                        break;
                    case "CostCenter":
                        Check = "مرکز هزینه";
                        Cell pcell6 = sheet.Cells[alpha[index] + "1"];
                        pcell6.PutValue(Check);
                        int j6 = 0;
                        foreach (var _item in Personal)
                        {
                            CostCenter = _item.fldTitleCostCenter;
                            Cell Cell = sheet.Cells[alpha[index] + (j6 + 2)];
                            Cell.PutValue(CostCenter);
                            j6++;
                        }
                        index++;
                        break;
                    case "MazadCSal":
                        Check = "مازاد 30 سال";
                        Cell pcell7 = sheet.Cells[alpha[index] + "1"];
                        pcell7.PutValue(Check);
                        int j7 = 0;
                        foreach (var _item in Personal)
                        {
                            MazadCSal = _item.fldMazadCSalName;
                            Cell Cell = sheet.Cells[alpha[index] + (j7 + 2)];
                            Cell.PutValue(MazadCSal);
                            j7++;
                        }
                        index++;
                        break;
                    case "PasAndaz":
                        Check = "پس انداز";
                        Cell pcell8 = sheet.Cells[alpha[index] + "1"];
                        pcell8.PutValue(Check);
                        int j8 = 0;
                        foreach (var _item in Personal)
                        {
                            PasAndaz = _item.fldPasAndazName;
                            Cell Cell = sheet.Cells[alpha[index] + (j8 + 2)];
                            Cell.PutValue(PasAndaz);
                            j8++;
                        }
                        index++;
                        break;
                    case "MasuliyatPayanKhedmat":
                        Check = "سنوات پایان خدمت";
                        Cell pcell9 = sheet.Cells[alpha[index] + "1"];
                        pcell9.PutValue(Check);
                        int j9 = 0;
                        foreach (var _item in Personal)
                        {
                            MasuliyatPayanKhedmat = _item.fldSanavatPayanKhedmatName;
                            Cell Cell = sheet.Cells[alpha[index] + (j9 + 2)];
                            Cell.PutValue(MasuliyatPayanKhedmat);
                            j9++;
                        }
                        index++;
                        break;
                    case "JobeCode":
                        Check = "کد شغلی بیمه";
                        Cell pcell10 = sheet.Cells[alpha[index] + "1"];
                        pcell10.PutValue(Check);
                        int j10 = 0;
                        foreach (var _item in Personal)
                        {
                            JobeCode = _item.fldJobDesc;
                            Cell Cell = sheet.Cells[alpha[index] + (j10 + 2)];
                            Cell.PutValue(JobeCode);
                            j10++;
                        }
                        index++;
                        break;
                    case "InsuranceWorkShop":
                        Check = "کارگاه بیمه";
                        Cell pcell11 = sheet.Cells[alpha[index] + "1"];
                        pcell11.PutValue(Check);
                        int j11 = 0;
                        foreach (var _item in Personal)
                        {
                            InsuranceWorkShop = _item.fldWorkShopName;
                            Cell Cell = sheet.Cells[alpha[index] + (j11 + 2)];
                            Cell.PutValue(InsuranceWorkShop);
                            j11++;
                        }
                        index++;
                        break;
                    case "HamsarKarmand":
                        Check = "همسر کارمند";
                        Cell pcell12 = sheet.Cells[alpha[index] + "1"];
                        pcell12.PutValue(Check);
                        int j12 = 0;
                        foreach (var _item in Personal)
                        {
                            HamsarKarmand = _item.fldHamsarKarmandName;
                            Cell Cell = sheet.Cells[alpha[index] + (j12 + 2)];
                            Cell.PutValue(HamsarKarmand);
                            j12++;
                        }
                        index++;
                        break;
                    /////////////////////////
                    case "TarikhEjra":
                        Check = "تاریخ اجرا";
                        Cell hcell1 = sheet.Cells[alpha[index] + "1"];
                        hcell1.PutValue(Check);
                        int k1 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhEjra = _item.fldTarikhEjra;
                            Cell Cell = sheet.Cells[alpha[index] + (k1 + 2)];
                            Cell.PutValue(TarikhEjra);
                            k1++;
                        }
                        index++;
                        break;
                    case "TarikhSodoorHokm":
                        Check = "تاریخ صدور";
                        Cell hcell2 = sheet.Cells[alpha[index] + "1"];
                        hcell2.PutValue(Check);
                        int k2 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhSodoorHokm = _item.fldTarikhSodoorHokm;
                            Cell Cell = sheet.Cells[alpha[index] + (k2 + 2)];
                            Cell.PutValue(TarikhSodoorHokm);
                            k2++;
                        }
                        index++;
                        break;
                    case "TarikhEtmam":
                        Check = "تاریخ اتمام";
                        Cell hcell3 = sheet.Cells[alpha[index] + "1"];
                        hcell3.PutValue(Check);
                        int k3 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhEtmam = _item.fldTarikhEtmam;
                            Cell Cell = sheet.Cells[alpha[index] + (k3 + 2)];
                            Cell.PutValue(TarikhEtmam);
                            k3++;
                        }
                        index++;
                        break;
                    case "StatusTaahol":
                        Check = "وضعیت تاهل";
                        Cell hcell4 = sheet.Cells[alpha[index] + "1"];
                        hcell4.PutValue(Check);
                        int k4 = 0;
                        foreach (var _item in Personal)
                        {
                            StatusTaahol = _item.StatusTaaholTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (k4 + 2)];
                            Cell.PutValue(StatusTaahol);
                            k4++;
                        }
                        index++;
                        break;
                    case "ShomarePostSazmani":
                        Check = "شماره پست سازمانی";
                        Cell hcell5 = sheet.Cells[alpha[index] + "1"];
                        hcell5.PutValue(Check);
                        int k5 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomarePostSazmani = _item.fldShomarePostSazmani;
                            Cell Cell = sheet.Cells[alpha[index] + (k5 + 2)];
                            Cell.PutValue(ShomarePostSazmani);
                            k5++;
                        }
                        index++;
                        break;
                    case "MoreGroup":
                        Check = "گروه بالاتر";
                        Cell hcell6 = sheet.Cells[alpha[index] + "1"];
                        hcell6.PutValue(Check);
                        int k6 = 0;
                        foreach (var _item in Personal)
                        {
                            MoreGroup = _item.fldMoreGroup;
                            Cell Cell = sheet.Cells[alpha[index] + (k6 + 2)];
                            Cell.PutValue(MoreGroup);
                            k6++;
                        }
                        index++;
                        break;
                    case "Group":
                        Check = "گروه";
                        Cell hcell7 = sheet.Cells[alpha[index] + "1"];
                        hcell7.PutValue(Check);
                        int k7 = 0;
                        foreach (var _item in Personal)
                        {
                            Group = _item.fldGroup.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (k7 + 2)];
                            Cell.PutValue(Group);
                            k7++;
                        }
                        index++;
                        break;
                    case "TedadFarzand":
                        Check = "تعداد فرزند";
                        Cell hcell8 = sheet.Cells[alpha[index] + "1"];
                        hcell8.PutValue(Check);
                        int k8 = 0;
                        foreach (var _item in Personal)
                        {
                            TedadFarzand = _item.fldTedadFarzand.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (k8 + 2)];
                            Cell.PutValue(TedadFarzand);
                            k8++;
                        }
                        index++;
                        break;
                    case "TedadAfradTahteTakafol":
                        Check = "تعداد افراد تحت تکفل";
                        Cell hcell9 = sheet.Cells[alpha[index] + "1"];
                        hcell9.PutValue(Check);
                        int k9 = 0;
                        foreach (var _item in Personal)
                        {
                            TedadAfradTahteTakafol = _item.fldTedadAfradTahteTakafol.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (k9 + 2)];
                            Cell.PutValue(TedadAfradTahteTakafol);
                            k9++;
                        }
                        index++;
                        break;
                    case "CodeShoghl":
                        Check = "کد شغل";
                        Cell hcell10 = sheet.Cells[alpha[index] + "1"];
                        hcell10.PutValue(Check);
                        int k10 = 0;
                        foreach (var _item in Personal)
                        {
                            CodeShoghl = _item.fldCodeShoghl;
                            Cell Cell = sheet.Cells[alpha[index] + (k10 + 2)];
                            Cell.PutValue(CodeShoghl);
                            k10++;
                        }
                        index++;
                        break;
                    case "ShomareHokm":
                        Check = "شماره حکم";
                        Cell hcell11 = sheet.Cells[alpha[index] + "1"];
                        hcell11.PutValue(Check);
                        int k11 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomareHokm = _item.fldShomareHokm;
                            Cell Cell = sheet.Cells[alpha[index] + (k11 + 2)];
                            Cell.PutValue(ShomareHokm);
                            k11++;
                        }
                        index++;
                        break;
                    case "NoeEstekhdamTitle":
                        Check = "نوع استخدام";
                        Cell hcell12 = sheet.Cells[alpha[index] + "1"];
                        hcell12.PutValue(Check);
                        int k12 = 0;
                        foreach (var _item in Personal)
                        {
                            NoeEstekhdamTitle = _item.TitleNoeEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (k12 + 2)];
                            Cell.PutValue(NoeEstekhdamTitle);
                            k12++;
                        }
                        index++;
                        break;
                    case "JameHokm":
                        Check = "جمع حکم";
                        Cell hcell13 = sheet.Cells[alpha[index] + "1"];
                        hcell13.PutValue(Check);
                        int k13 = 0;
                        foreach (var _item in Personal)
                        {
                            JameHokm = _item.fldJameHokm;
                            Cell Cell = sheet.Cells[alpha[index] + (k13 + 2)];
                            Cell.PutValue(JameHokm);
                            k13++;
                        }
                        index++;
                        break;
                    case "Ertegha":
                        Check = "تاریخ ارتقا گروه";
                        Cell hcell14 = sheet.Cells[alpha[index] + "1"];
                        hcell14.PutValue(Check);
                        int k14 = 0;
                        foreach (var _item in Personal)
                        {
                            Ertegha = _item.Ertegha;
                            Cell Cell = sheet.Cells[alpha[index] + (k14 + 2)];
                            Cell.PutValue(Ertegha);
                            k14++;
                        }
                        index++;
                        break;
                    ////////////////////////////////////
                    case "h-paye":
                        Check = "حقوق مبنا";
                        Cell icell1 = sheet.Cells[alpha[index] + "1"];
                        icell1.PutValue(Check);
                        int m1 = 0;
                        foreach (var _item in Personal)
                        {
                            hpaye = _item.hpaye.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m1 + 2)];
                            Cell.PutValue(hpaye);
                            m1++;
                        }
                        index++;
                        break;
                    case "sanavat":
                        Check = "سنوات";
                        Cell icell2 = sheet.Cells[alpha[index] + "1"];
                        icell2.PutValue(Check);
                        int m2 = 0;
                        foreach (var _item in Personal)
                        {
                            sanavat = _item.sanavat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m2 + 2)];
                            Cell.PutValue(sanavat);
                            m2++;
                        }
                        index++;
                        break;
                    case "paye":
                        Check = "پایه";
                        Cell icell3 = sheet.Cells[alpha[index] + "1"];
                        icell3.PutValue(Check);
                        int m3 = 0;
                        foreach (var _item in Personal)
                        {
                            paye = _item.paye.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m3 + 2)];
                            Cell.PutValue(paye);
                            m3++;
                        }
                        index++;
                        break;
                    case "sanavat-basiji":
                        Check = "سنوات بسیجی";
                        Cell icell4 = sheet.Cells[alpha[index] + "1"];
                        icell4.PutValue(Check);
                        int m4 = 0;
                        foreach (var _item in Personal)
                        {
                            sanavatbasiji = _item.sanavatbasiji.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m4 + 2)];
                            Cell.PutValue(sanavatbasiji);
                            m4++;
                        }
                        index++;
                        break;
                    case "sanavat-isar":
                        Check = "سنوات ایثارگری";
                        Cell icell5 = sheet.Cells[alpha[index] + "1"];
                        icell5.PutValue(Check);
                        int m5 = 0;
                        foreach (var _item in Personal)
                        {
                            sanavatisar = _item.sanavatisar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m5 + 2)];
                            Cell.PutValue(sanavatisar);
                            m5++;
                        }
                        index++;
                        break;
                    case "foghshoghl":
                        Check = "فوق العاده شغل";
                        Cell icell6 = sheet.Cells[alpha[index] + "1"];
                        icell6.PutValue(Check);
                        int m6 = 0;
                        foreach (var _item in Personal)
                        {
                            foghshoghl = _item.foghshoghl.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m6 + 2)];
                            Cell.PutValue(foghshoghl);
                            m6++;
                        }
                        index++;
                        break;
                    case "takhasosi":
                        Check = "تحقیقی و تخصصی";
                        Cell icell7 = sheet.Cells[alpha[index] + "1"];
                        icell7.PutValue(Check);
                        int m7 = 0;
                        foreach (var _item in Personal)
                        {
                            takhasosi = _item.takhasosi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m7 + 2)];
                            Cell.PutValue(takhasosi);
                            m7++;
                        }
                        index++;
                        break;
                    case "made26":
                        Check = "فوق العاده ماده26";
                        Cell icell8 = sheet.Cells[alpha[index] + "1"];
                        icell8.PutValue(Check);
                        int m8 = 0;
                        foreach (var _item in Personal)
                        {
                            made26 = _item.made26.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m8 + 2)];
                            Cell.PutValue(made26);
                            m8++;
                        }
                        index++;
                        break;
                    case "modiryati":
                        Check = "مدیریتی و سرپرستی";
                        Cell icell9 = sheet.Cells[alpha[index] + "1"];
                        icell9.PutValue(Check);
                        int m9 = 0;
                        foreach (var _item in Personal)
                        {
                            modiryati = _item.modiryati.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m9 + 2)];
                            Cell.PutValue(modiryati);
                            m9++;
                        }
                        index++;
                        break;
                    case "barjastegi":
                        Check = "برجستگی";
                        Cell icell10 = sheet.Cells[alpha[index] + "1"];
                        icell10.PutValue(Check);
                        int m10 = 0;
                        foreach (var _item in Personal)
                        {
                            barjastegi = _item.barjastegi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m10 + 2)];
                            Cell.PutValue(barjastegi);
                            m10++;
                        }
                        index++;
                        break;
                    case "tatbigh":
                        Check = "تفاوت تطبیق";
                        Cell icell11 = sheet.Cells[alpha[index] + "1"];
                        icell11.PutValue(Check);
                        int m11 = 0;
                        foreach (var _item in Personal)
                        {
                            tatbigh = _item.tatbigh.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m11 + 2)];
                            Cell.PutValue(tatbigh);
                            m11++;
                        }
                        index++;
                        break;
                    case "fogh-isar":
                        Check = "فوق العاده ایثارگری";
                        Cell icell12 = sheet.Cells[alpha[index] + "1"];
                        icell12.PutValue(Check);
                        int m12 = 0;
                        foreach (var _item in Personal)
                        {
                            foghisar = _item.foghisar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m12 + 2)];
                            Cell.PutValue(foghisar);
                            m12++;
                        }
                        index++;
                        break;
                    case "abohava":
                        Check = "بدی آب و هوا";
                        Cell icell13 = sheet.Cells[alpha[index] + "1"];
                        icell13.PutValue(Check);
                        int m13 = 0;
                        foreach (var _item in Personal)
                        {
                            abohava = _item.abohava.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m13 + 2)];
                            Cell.PutValue(abohava);
                            m13++;
                        }
                        index++;
                        break;
                    case "tashilat":
                        Check = "تسهیلات زندگی";
                        Cell icell14 = sheet.Cells[alpha[index] + "1"];
                        icell14.PutValue(Check);
                        int m14 = 0;
                        foreach (var _item in Personal)
                        {
                            tashilat = _item.tashilat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m14 + 2)];
                            Cell.PutValue(tashilat);
                            m14++;
                        }
                        index++;
                        break;
                    case "sakhtikar":
                        Check = "سختی کار";
                        Cell icell15 = sheet.Cells[alpha[index] + "1"];
                        icell15.PutValue(Check);
                        int m15 = 0;
                        foreach (var _item in Personal)
                        {
                            sakhtikar = _item.sakhtikar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m15 + 2)];
                            Cell.PutValue(sakhtikar);
                            m15++;
                        }
                        index++;
                        break;
                    case "tadil":
                        Check = "فوق العاده تعدیل";
                        Cell icell16 = sheet.Cells[alpha[index] + "1"];
                        icell16.PutValue(Check);
                        int m16 = 0;
                        foreach (var _item in Personal)
                        {
                            tadil = _item.tadil.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m16 + 2)];
                            Cell.PutValue(tadil);
                            m16++;
                        }
                        index++;
                        break;
                    case "riali":
                        Check = "مزایای ریالی گروه تشویقی";
                        Cell icell17 = sheet.Cells[alpha[index] + "1"];
                        icell17.PutValue(Check);
                        int m17 = 0;
                        foreach (var _item in Personal)
                        {
                            riali = _item.riali.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m17 + 2)];
                            Cell.PutValue(riali);
                            m17++;
                        }
                        index++;
                        break;
                    case "jazb9":
                        Check = "حق جذب بند9";
                        Cell icell18 = sheet.Cells[alpha[index] + "1"];
                        icell18.PutValue(Check);
                        int m18 = 0;
                        foreach (var _item in Personal)
                        {
                            jazb9 = _item.jazb9.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m18 + 2)];
                            Cell.PutValue(jazb9);
                            m18++;
                        }
                        index++;
                        break;
                    case "jazb":
                        Check = "فوق العاده جذب";
                        Cell icell19 = sheet.Cells[alpha[index] + "1"];
                        icell19.PutValue(Check);
                        int m19 = 0;
                        foreach (var _item in Personal)
                        {
                            jazb = _item.jazb.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m19 + 2)];
                            Cell.PutValue(jazb);
                            m19++;
                        }
                        index++;
                        break;
                    case "makhsos":
                        Check = "فوق العاده مخصوص";
                        Cell icell20 = sheet.Cells[alpha[index] + "1"];
                        icell20.PutValue(Check);
                        int m20 = 0;
                        foreach (var _item in Personal)
                        {
                            makhsos = _item.makhsos.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m20 + 2)];
                            Cell.PutValue(makhsos);
                            m20++;
                        }
                        index++;
                        break;
                    case "vije":
                        Check = "فوق العاده ویژه";
                        Cell icell21 = sheet.Cells[alpha[index] + "1"];
                        icell21.PutValue(Check);
                        int m21 = 0;
                        foreach (var _item in Personal)
                        {
                            vije = _item.vije.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m21 + 2)];
                            Cell.PutValue(vije);
                            m21++;
                        }
                        index++;
                        break;
                    case "olad":
                        Check = "حق اولاد";
                        Cell icell22 = sheet.Cells[alpha[index] + "1"];
                        icell22.PutValue(Check);
                        int m22 = 0;
                        foreach (var _item in Personal)
                        {
                            olad = _item.olad.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m22 + 2)];
                            Cell.PutValue(olad);
                            m22++;
                        }
                        index++;
                        break;
                    case "ayelemandi":
                        Check = "حق عائله مندی";
                        Cell icell23 = sheet.Cells[alpha[index] + "1"];
                        icell23.PutValue(Check);
                        int m23 = 0;
                        foreach (var _item in Personal)
                        {
                            ayelemandi = _item.ayelemandi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m23 + 2)];
                            Cell.PutValue(ayelemandi);
                            m23++;
                        }
                        index++;
                        break;
                    case "kharobar":
                        Check = "خوار و بار";
                        Cell icell24 = sheet.Cells[alpha[index] + "1"];
                        icell24.PutValue(Check);
                        int m24 = 0;
                        foreach (var _item in Personal)
                        {
                            kharobar = _item.kharobar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m24 + 2)];
                            Cell.PutValue(kharobar);
                            m24++;
                        }
                        index++;
                        break;
                    case "maskan":
                        Check = "حق مسکن";
                        Cell icell25 = sheet.Cells[alpha[index] + "1"];
                        icell25.PutValue(Check);
                        int m25 = 0;
                        foreach (var _item in Personal)
                        {
                            maskan = _item.maskan.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m25 + 2)];
                            Cell.PutValue(maskan);
                            m25++;
                        }
                        index++;
                        break;
                    case "nobatkari":
                        Check = "نوبت کاری";
                        Cell icell26 = sheet.Cells[alpha[index] + "1"];
                        icell26.PutValue(Check);
                        int m26 = 0;
                        foreach (var _item in Personal)
                        {
                            nobatkari = _item.nobatkari.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m26 + 2)];
                            Cell.PutValue(nobatkari);
                            m26++;
                        }
                        index++;
                        break;
                    case "bon":
                        Check = "بن ماهیانه";
                        Cell icell27 = sheet.Cells[alpha[index] + "1"];
                        icell27.PutValue(Check);
                        int m27 = 0;
                        foreach (var _item in Personal)
                        {
                            bon = _item.bon.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m27 + 2)];
                            Cell.PutValue(bon);
                            m27++;
                        }
                        index++;
                        break;
                    case "jazb-tabsare":
                        Check = "حق جذب تبصره 7 ماده یک";
                        Cell icell28 = sheet.Cells[alpha[index] + "1"];
                        icell28.PutValue(Check);
                        int m28 = 0;
                        foreach (var _item in Personal)
                        {
                            jazbtabsare = _item.jazbtabsare.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m28 + 2)];
                            Cell.PutValue(jazbtabsare);
                            m28++;
                        }
                        index++;
                        break;
                    case "talash":
                        Check = "فوق العاده تلاش";
                        Cell icell29 = sheet.Cells[alpha[index] + "1"];
                        icell29.PutValue(Check);
                        int m29 = 0;
                        foreach (var _item in Personal)
                        {
                            talash = _item.talash.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m29 + 2)];
                            Cell.PutValue(talash);
                            m29++;
                        }
                        index++;
                        break;
                    case "jebhe":
                        Check = "حق جبهه";
                        Cell icell30 = sheet.Cells[alpha[index] + "1"];
                        icell30.PutValue(Check);
                        int m30 = 0;
                        foreach (var _item in Personal)
                        {
                            jebhe = _item.jebhe.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m30 + 2)];
                            Cell.PutValue(jebhe);
                            m30++;
                        }
                        index++;
                        break;
                    case "janbazi":
                        Check = "حق جانبازی";
                        Cell icell31 = sheet.Cells[alpha[index] + "1"];
                        icell31.PutValue(Check);
                        int m31 = 0;
                        foreach (var _item in Personal)
                        {
                            janbazi = _item.janbazi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m31 + 2)];
                            Cell.PutValue(janbazi);
                            m31++;
                        }
                        index++;
                        break;
                    case "sayer":
                        Check = "سایر مزایا";
                        Cell icell32 = sheet.Cells[alpha[index] + "1"];
                        icell32.PutValue(Check);
                        int m32 = 0;
                        foreach (var _item in Personal)
                        {
                            sayer = _item.sayer.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m32 + 2)];
                            Cell.PutValue(sayer);
                            m32++;
                        }
                        index++;
                        break;
                    case "ezafekar":
                        Check = "اضافه کاری";
                        Cell icell33 = sheet.Cells[alpha[index] + "1"];
                        icell33.PutValue(Check);
                        int m33 = 0;
                        foreach (var _item in Personal)
                        {
                            ezafekar = _item.ezafekar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m33 + 2)];
                            Cell.PutValue(ezafekar);
                            m33++;
                        }
                        index++;
                        break;
                    case "mamoriat":
                        Check = "ماموریت";
                        Cell icell34 = sheet.Cells[alpha[index] + "1"];
                        icell34.PutValue(Check);
                        int m34 = 0;
                        foreach (var _item in Personal)
                        {
                            mamoriat = _item.mamoriat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m34 + 2)];
                            Cell.PutValue(mamoriat);
                            m34++;
                        }
                        index++;
                        break;
                    case "tatilkari":
                        Check = "تعطیل کاری";
                        Cell icell35 = sheet.Cells[alpha[index] + "1"];
                        icell35.PutValue(Check);
                        int m35 = 0;
                        foreach (var _item in Personal)
                        {
                            tatilkari = _item.tatilkari.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m35 + 2)];
                            Cell.PutValue(tatilkari);
                            m35++;
                        }
                        index++;
                        break;
                    case "s_payankhedmat":
                        Check = "سنوات پایان خدمت";
                        Cell icell36 = sheet.Cells[alpha[index] + "1"];
                        icell36.PutValue(Check);
                        int m36 = 0;
                        foreach (var _item in Personal)
                        {
                            s_payankhedmat = _item.s_payankhedmat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m36 + 2)];
                            Cell.PutValue(s_payankhedmat);
                            m36++;
                        }
                        index++;
                        break;
                    case "ghazai":
                        Check = "حق جذب قضایی";
                        Cell icell37 = sheet.Cells[alpha[index] + "1"];
                        icell37.PutValue(Check);
                        int m37 = 0;
                        foreach (var _item in Personal)
                        {
                            ghazai = _item.ghazai.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m37 + 2)];
                            Cell.PutValue(ghazai);
                            m37++;
                        }
                        index++;
                        break;
                    case "ashoora":
                        Check = "گردان عاشورا";
                        Cell icell38 = sheet.Cells[alpha[index] + "1"];
                        icell38.PutValue(Check);
                        int m38 = 0;
                        foreach (var _item in Personal)
                        {
                            ashoora = _item.ashoora.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m38 + 2)];
                            Cell.PutValue(ashoora);
                            m38++;
                        }
                        index++;
                        break;
                    case "zaribtadil":
                        Check = "ضریب تعدیل";
                        Cell icell39 = sheet.Cells[alpha[index] + "1"];
                        icell39.PutValue(Check);
                        int m39 = 0;
                        foreach (var _item in Personal)
                        {
                            zaribtadil = _item.zaribtadil.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m39 + 2)];
                            Cell.PutValue(zaribtadil);
                            m39++;
                        }
                        index++;
                        break;
                    case "jazbTakhasosi":
                        Check = "جذب مشاغل تخصصی";
                        Cell icell40 = sheet.Cells[alpha[index] + "1"];
                        icell40.PutValue(Check);
                        int m40 = 0;
                        foreach (var _item in Personal)
                        {
                            jazbTakhasosi = _item.jazbTakhasosi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m40 + 2)];
                            Cell.PutValue(jazbTakhasosi);
                            m40++;
                        }
                        index++;
                        break;
                    case "jazbtadil":
                        Check = "جذب تعدیل";
                        Cell icell41 = sheet.Cells[alpha[index] + "1"];
                        icell41.PutValue(Check);
                        int m41 = 0;
                        foreach (var _item in Personal)
                        {
                            jazbtadil = _item.jazbtadil.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m41 + 2)];
                            Cell.PutValue(jazbtadil);
                            m41++;
                        }
                        index++;
                        break;
                    case "hadaghaltadil":
                        Check = "تفاوت ناشی از ضریب تعدیل";
                        Cell icell42 = sheet.Cells[alpha[index] + "1"];
                        icell42.PutValue(Check);
                        int m42 = 0;
                        foreach (var _item in Personal)
                        {
                            hadaghaltadil = _item.hadaghaltadil.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m42 + 2)];
                            Cell.PutValue(hadaghaltadil);
                            m42++;
                        }
                        index++;
                        break;
                    case "khas":
                        Check = "خاص";
                        Cell icell421 = sheet.Cells[alpha[index] + "1"];
                        icell421.PutValue(Check);
                        int m421 = 0;
                        foreach (var _item in Personal)
                        {
                            var khas = _item.khas.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m421 + 2)];
                            Cell.PutValue(khas);
                            m421++;
                        }
                        index++;
                        break;
                    case "karane":
                        Check = "کارانه";
                        Cell icell422 = sheet.Cells[alpha[index] + "1"];
                        icell422.PutValue(Check);
                        int m422 = 0;
                        foreach (var _item in Personal)
                        {
                            var karane = _item.karane.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m422 + 2)];
                            Cell.PutValue(karane);
                            m422++;
                        }
                        index++;
                        break;
                    case "refahi":
                        Check = "رفاهی";
                        Cell icell423 = sheet.Cells[alpha[index] + "1"];
                        icell423.PutValue(Check);
                        int m423 = 0;
                        foreach (var _item in Personal)
                        {
                            var refahi = _item.refahi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m423 + 2)];
                            Cell.PutValue(refahi);
                            m423++;
                        }
                        index++;
                        break;
                    ///////////////////////////////////
                    case "Karkard":
                        Check = "کارکرد";
                        Cell dcell1 = sheet.Cells[alpha[index] + "1"];
                        dcell1.PutValue(Check);
                        int h1 = 0;
                        foreach (var _item in Personal)
                        {
                            Karkard = _item.fldKarkard.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h1 + 2)];
                            Cell.PutValue(Karkard);
                            h1++;
                        }
                        index++;
                        break;
                    case "Gheybat":
                        Check = "غیبت";
                        Cell dcell2 = sheet.Cells[alpha[index] + "1"];
                        dcell2.PutValue(Check);
                        int h2 = 0;
                        foreach (var _item in Personal)
                        {
                            Gheybat = _item.fldGheybat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h2 + 2)];
                            Cell.PutValue(Gheybat);
                            h2++;
                        }
                        index++;
                        break;
                    case "TedadEzafeKar":
                        Check = "تعداد اضافه کاری";
                        Cell dcell3 = sheet.Cells[alpha[index] + "1"];
                        dcell3.PutValue(Check);
                        int h3 = 0;
                        foreach (var _item in Personal)
                        {
                            TedadEzafeKar = _item.fldTedadEzafeKar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h3 + 2)];
                            Cell.PutValue(TedadEzafeKar);
                            h3++;
                        }
                        index++;
                        break;
                    case "BimeOmrKarFarma":
                        Check = "مبلغ بیمه عمر سهم کارفرما";
                        Cell dcell4 = sheet.Cells[alpha[index] + "1"];
                        dcell4.PutValue(Check);
                        int h4 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeOmrKarFarma = _item.fldBimeOmrKarFarma.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h4 + 2)];
                            Cell.PutValue(BimeOmrKarFarma);
                            h4++;
                        }
                        index++;
                        break;
                    case "Mamooriyat":
                        Check = "تعداد ماموریت";
                        Cell dcell5 = sheet.Cells[alpha[index] + "1"];
                        dcell5.PutValue(Check);
                        int h5 = 0;
                        foreach (var _item in Personal)
                        {
                            Mamooriyat = _item.Mamooriyat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h5 + 2)];
                            Cell.PutValue(Mamooriyat);
                            h5++;
                        }
                        index++;
                        break;
                    case "TedadTatilKar":
                        Check = "تعداد تعطیل کاری";
                        Cell dcell6 = sheet.Cells[alpha[index] + "1"];
                        dcell6.PutValue(Check);
                        int h6 = 0;
                        foreach (var _item in Personal)
                        {
                            TedadTatilKar = _item.fldTedadTatilKar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h6 + 2)];
                            Cell.PutValue(TedadTatilKar);
                            h6++;
                        }
                        index++;
                        break;
                    case "BimeOmrM":
                        Check = "مبلغ بیمه عمر";
                        Cell dcell7 = sheet.Cells[alpha[index] + "1"];
                        dcell7.PutValue(Check);
                        int h7 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeOmrM = _item.fldBimeOmrM.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h7 + 2)];
                            Cell.PutValue(BimeOmrM);
                            h7++;
                        }
                        index++;
                        break;
                    case "BimeTakmily":
                        Check = "مبلغ بیمه تکمیلی";
                        Cell dcell8 = sheet.Cells[alpha[index] + "1"];
                        dcell8.PutValue(Check);
                        int h8 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeTakmily = _item.fldBimeTakmily.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h8 + 2)];
                            Cell.PutValue(BimeTakmily);
                            h8++;
                        }
                        index++;
                        break;
                    case "BimeTakmilyKarFarma":
                        Check = "مبلغ تکمیلی سهم کارفرما";
                        Cell dcell9 = sheet.Cells[alpha[index] + "1"];
                        dcell9.PutValue(Check);
                        int h9 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeTakmilyKarFarma = _item.fldBimeTakmilyKarFarma.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h9 + 2)];
                            Cell.PutValue(BimeTakmilyKarFarma);
                            h9++;
                        }
                        index++;
                        break;
                    case "HaghDarmanKarfFarma ":
                        Check = "مبلغ درمان سهم کارفرما";
                        Cell dcell10 = sheet.Cells[alpha[index] + "1"];
                        dcell10.PutValue(Check);
                        int h10 = 0;
                        foreach (var _item in Personal)
                        {
                            HaghDarmanKarfFarma = _item.fldHaghDarmanKarfFarma.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h10 + 2)];
                            Cell.PutValue(HaghDarmanKarfFarma);
                            h10++;
                        }
                        index++;
                        break;
                    case "HaghDarmanDolat":
                        Check = "مبلغ درمان سهم دولت";
                        Cell dcell11 = sheet.Cells[alpha[index] + "1"];
                        dcell11.PutValue(Check);
                        int h11 = 0;
                        foreach (var _item in Personal)
                        {
                            HaghDarmanDolat = _item.fldHaghDarmanDolat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h11 + 2)];
                            Cell.PutValue(HaghDarmanDolat);
                            h11++;
                        }
                        index++;
                        break;
                    case "HaghDarman":
                        Check = "حق درمان";
                        Cell dcell12 = sheet.Cells[alpha[index] + "1"];
                        dcell12.PutValue(Check);
                        int h12 = 0;
                        foreach (var _item in Personal)
                        {
                            HaghDarman = _item.fldHaghDarman.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h12 + 2)];
                            Cell.PutValue(HaghDarman);
                            h12++;
                        }
                        index++;
                        break;
                    case "BimePersonal":
                        Check = "مبلغ بیمه پرسنل";
                        Cell dcell13 = sheet.Cells[alpha[index] + "1"];
                        dcell13.PutValue(Check);
                        int h13 = 0;
                        foreach (var _item in Personal)
                        {
                            BimePersonal = _item.fldBimePersonal.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h13 + 2)];
                            Cell.PutValue(BimePersonal);
                            h13++;
                        }
                        index++;
                        break;
                    case "BimeKarFarma":
                        Check = "مبلغ بیمه سهم کارفرما";
                        Cell dcell14 = sheet.Cells[alpha[index] + "1"];
                        dcell14.PutValue(Check);
                        int h14 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeKarFarma = _item.fldBimeKarFarma.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h14 + 2)];
                            Cell.PutValue(BimeKarFarma);
                            h14++;
                        }
                        index++;
                        break;
                    case "BimeBikari":
                        Check = "مبلغ بیمه بیکاری";
                        Cell dcell15 = sheet.Cells[alpha[index] + "1"];
                        dcell15.PutValue(Check);
                        int h15 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeBikari = _item.fldBimeBikari.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h15 + 2)];
                            Cell.PutValue(BimeBikari);
                            h15++;
                        }
                        index++;
                        break;
                    case "BimeMashaghel":
                        Check = "مبلغ بیمه مشاغل";
                        Cell dcell16 = sheet.Cells[alpha[index] + "1"];
                        dcell16.PutValue(Check);
                        int h16 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeMashaghel = _item.fldBimeMashaghel.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h16 + 2)];
                            Cell.PutValue(BimeMashaghel);
                            h16++;
                        }
                        index++;
                        break;
                    case "TedadNobatKari":
                        Check = "تعداد نوبت کاری";
                        Cell dcell17 = sheet.Cells[alpha[index] + "1"];
                        dcell17.PutValue(Check);
                        int h17 = 0;
                        foreach (var _item in Personal)
                        {
                            TedadNobatKari = _item.fldTedadNobatKari.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h17 + 2)];
                            Cell.PutValue(TedadNobatKari);
                            h17++;
                        }
                        index++;
                        break;
                    case "Mosaede":
                        Check = "مساعده";
                        Cell dcell18 = sheet.Cells[alpha[index] + "1"];
                        dcell18.PutValue(Check);
                        int h18 = 0;
                        foreach (var _item in Personal)
                        {
                            Mosaede = _item.fldMosaede.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h18 + 2)];
                            Cell.PutValue(Mosaede);
                            h18++;
                        }
                        index++;
                        break;
                    case "GhestVam":
                        Check = "قسط وام";
                        Cell dcell19 = sheet.Cells[alpha[index] + "1"];
                        dcell19.PutValue(Check);
                        int h19 = 0;
                        foreach (var _item in Personal)
                        {
                            GhestVam = _item.fldGhestVam.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h19 + 2)];
                            Cell.PutValue(GhestVam);
                            h19++;
                        }
                        index++;
                        break;
                    case "PasAndazM":
                        Check = "مبلغ پس انداز";
                        Cell dcell20 = sheet.Cells[alpha[index] + "1"];
                        dcell20.PutValue(Check);
                        int h20 = 0;
                        foreach (var _item in Personal)
                        {
                            PasAndazM = _item.fldPasAndazM.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h20 + 2)];
                            Cell.PutValue(PasAndazM);
                            h20++;
                        }
                        index++;
                        break;
                    case "MashmolBime":
                        Check = "مشمول بیمه";
                        Cell dcell21 = sheet.Cells[alpha[index] + "1"];
                        dcell21.PutValue(Check);
                        int h21 = 0;
                        foreach (var _item in Personal)
                        {
                            MashmolBime = _item.fldMashmolBime.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h21 + 2)];
                            Cell.PutValue(MashmolBime);
                            h21++;
                        }
                        index++;
                        break;
                    case "MashmolMaliyat":
                        Check = "مشمول مالیات";
                        Cell dcell22 = sheet.Cells[alpha[index] + "1"];
                        dcell22.PutValue(Check);
                        int h22 = 0;
                        foreach (var _item in Personal)
                        {
                            MashmolMaliyat = _item.fldMashmolMaliyat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h22 + 2)];
                            Cell.PutValue(MashmolMaliyat);
                            h22++;
                        }
                        index++;
                        break;
                    case "Mogharari":
                        Check = "مبلغ مقرری";
                        Cell dcell23 = sheet.Cells[alpha[index] + "1"];
                        dcell23.PutValue(Check);
                        int h23 = 0;
                        foreach (var _item in Personal)
                        {
                            Mogharari = _item.fldMogharari.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h23 + 2)];
                            Cell.PutValue(Mogharari);
                            h23++;
                        }
                        index++;
                        break;
                    case "Maliyat":
                        Check = "مبلغ مالیات";
                        Cell dcell24 = sheet.Cells[alpha[index] + "1"];
                        dcell24.PutValue(Check);
                        int h24 = 0;
                        foreach (var _item in Personal)
                        {
                            Maliyat = _item.fldMaliyat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h24 + 2)];
                            Cell.PutValue(Maliyat);
                            h24++;
                        }
                        index++;
                        break;
                    case "khalesPardakhti":
                        Check = "مبلغ خالص پرداختی";
                        Cell dcell25 = sheet.Cells[alpha[index] + "1"];
                        dcell25.PutValue(Check);
                        int h25 = 0;
                        foreach (var _item in Personal)
                        {
                            khalesPardakhti = _item.fldkhalesPardakhti.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h25 + 2)];
                            Cell.PutValue(khalesPardakhti);
                            h25++;
                        }
                        index++;
                        break;
                    case "ShomareHesab":
                        Check = "شماره حساب";
                        Cell dcell26 = sheet.Cells[alpha[index] + "1"];
                        dcell26.PutValue(Check);
                        int h26 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomareHesab = _item.fldShomareHesab.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h26 + 2)];
                            Cell.PutValue(ShomareHesab);
                            h26++;
                        }
                        index++;
                        break;
                    case "tarmim":
                        Check = "ترمیم حقوق";
                        Cell icell424 = sheet.Cells[alpha[index] + "1"];
                        icell424.PutValue(Check);
                        int m424 = 0;
                        foreach (var _item in Personal)
                        {
                            var tarmim = _item.tarmim.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m424 + 2)];
                            Cell.PutValue(tarmim);
                            m424++;
                        }
                        index++;
                        break;
                        ///////////////////////////
                    case "hemayatGhazaii":
                        Check = "فوق العاده حمایت قضایی";
                        Cell icell425 = sheet.Cells[alpha[index] + "1"];
                        icell425.PutValue(Check);
                        int m425 = 0;
                        foreach (var _item in Personal)
                        {
                            var hemayatGhazaii = _item.hemayatGhazaii.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m425 + 2)];
                            Cell.PutValue(hemayatGhazaii);
                            m425++;
                        }
                        index++;
                        break;
                    case "tatbighHokmGhabli":
                        Check = "تفاوت تطبیق(با حکم قبلی)";
                        Cell icell426 = sheet.Cells[alpha[index] + "1"];
                        icell426.PutValue(Check);
                        int m426 = 0;
                        foreach (var _item in Personal)
                        {
                            var tatbighHokmGhabli = _item.tatbighHokmGhabli.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m426 + 2)];
                            Cell.PutValue(tatbighHokmGhabli);
                            m426++;
                        }
                        index++;
                        break;
                    case "jazbTarmim":
                        Check = "جذب ناشی از ترمیم حقوق";
                        Cell icell427 = sheet.Cells[alpha[index] + "1"];
                        icell427.PutValue(Check);
                        int m427 = 0;
                        foreach (var _item in Personal)
                        {
                            var jazbTarmim = _item.jazbTarmim.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m427 + 2)];
                            Cell.PutValue(jazbTarmim);
                            m427++;
                        }
                        index++;
                        break;
                    case "VizheTarmim":
                        Check = "ویژه ناشی از ترمیم حقوق";
                        Cell icell428 = sheet.Cells[alpha[index] + "1"];
                        icell428.PutValue(Check);
                        int m428 = 0;
                        foreach (var _item in Personal)
                        {
                            var VizheTarmim = _item.VizheTarmim.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m428 + 2)];
                            Cell.PutValue(VizheTarmim);
                            m428++;
                        }
                        index++;
                        break;
                    case "KhasTarmim":
                        Check = "خاص ناشی از ترمیم حقوق";
                        Cell icell429 = sheet.Cells[alpha[index] + "1"];
                        icell429.PutValue(Check);
                        int m429 = 0;
                        foreach (var _item in Personal)
                        {
                            var KhasTarmim = _item.KhasTarmim.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m429 + 2)];
                            Cell.PutValue(KhasTarmim);
                            m429++;
                        }
                        index++;
                        break;
                    case "mazayaRefahi":
                        Check = "مزایای جانبی رفاهی";
                        Cell icell430 = sheet.Cells[alpha[index] + "1"];
                        icell430.PutValue(Check);
                        int m430 = 0;
                        foreach (var _item in Personal)
                        {
                            var mazayaRefahi = _item.mazayaRefahi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m430 + 2)];
                            Cell.PutValue(mazayaRefahi);
                            m430++;
                        }
                        index++;
                        break;
                    case "mahdeKodak":
                        Check = "هزینه مهد کودک";
                        Cell icell431 = sheet.Cells[alpha[index] + "1"];
                        icell431.PutValue(Check);
                        int m431 = 0;
                        foreach (var _item in Personal)
                        {
                            var mahdeKodak = _item.mahdeKodak.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m431 + 2)];
                            Cell.PutValue(mahdeKodak);
                            m431++;
                        }
                        index++;
                        break;
                    case "khoraki":
                        Check = "اقلام خوراکی";
                        Cell icell432 = sheet.Cells[alpha[index] + "1"];
                        icell432.PutValue(Check);
                        int m432 = 0;
                        foreach (var _item in Personal)
                        {
                            var khoraki = _item.khoraki.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m432 + 2)];
                            Cell.PutValue(khoraki);
                            m432++;
                        }
                        index++;
                        break;
                    case "kalaBehdashti":
                        Check = "کالاهای بهداشتی";
                        Cell icell433 = sheet.Cells[alpha[index] + "1"];
                        icell433.PutValue(Check);
                        int m433 = 0;
                        foreach (var _item in Personal)
                        {
                            var kalaBehdashti = _item.kalaBehdashti.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m433 + 2)];
                            Cell.PutValue(kalaBehdashti);
                            m433++;
                        }
                        index++;
                        break;
                    case "Monasebat":
                        Check = "مناسبت";
                        Cell icell434 = sheet.Cells[alpha[index] + "1"];
                        icell434.PutValue(Check);
                        int m434 = 0;
                        foreach (var _item in Personal)
                        {
                            var Monasebat = _item.Monasebat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m434 + 2)];
                            Cell.PutValue(Monasebat);
                            m434++;
                        }
                        index++;
                        break;
                    case "javani":
                        Check = "مزایای جوانی جمعیت";
                        Cell icell435 = sheet.Cells[alpha[index] + "1"];
                        icell435.PutValue(Check);
                        int m435 = 0;
                        foreach (var _item in Personal)
                        {
                            var javani = _item.javani.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m435 + 2)];
                            Cell.PutValue(javani);
                            m435++;
                        }
                        index++;
                        break;
                
                }
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            return File(stream.ToArray(), "xls", "All.xls");
        }
        public FileResult CreateExcelBedoneMoavaghe(string Checked, string Year, string Month, string TaYear, string TaMonth, string NobatePardakht,
           string Status, string Gender, string Esargari, string Madrak, string NoeEstekhdam, string Bime, string Hazine, string Kargah/*, string StatusP*/
           , string ItemEstekhdams, string Organ)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA" };
            int index = 0;
            var StatusCheck = Checked.Split(';');
            var Check = "";
            var Name = ""; var FamilyName = ""; var FatherName = ""; var GenderChar = ""; var ShomareShenasname = ""; var Codemeli = "";
            var TarikhTavalod = ""; var CityName = ""; var Address = ""; var CodePosti = ""; var StatusEsargari = "";
            var ShomarePersoneli = ""; var ReshteTahsili = ""; var TitleStatusJesmi = ""; var StatusNezamVazife = ""; var Post = ""; var PostEjraee = "";
            var RasteShoghli = ""; var ReshteShoghli = ""; var TarikhEstekhdam = ""; var ServiceLocation = ""; var Tabaghe = ""; var MeliyatChar = "";
            var ShMojavezEstekhdam = ""; var TMojavezEstekhdam = ""; var MahalKhedmat = ""; var SanavatKhedmat = ""; var Tel = ""; var Mobile = ""; long JameHokm = 0; string Ertegha = "";

            var TypeBime = ""; var ShomareBime = ""; var BimeOmr = ""; var BimeTakmili = ""; var HamsarKarmand = "";
            var MashagheleSakhtVaZianAvar = ""; var CostCenter = ""; var MazadCSal = ""; var PasAndaz = ""; var MasuliyatPayanKhedmat = "";
            var JobeCode = ""; var InsuranceWorkShop = "";

            var TarikhEjra = ""; var TarikhSodoorHokm = ""; var TarikhEtmam = ""; var StatusTaahol = ""; var ShomarePostSazmani = ""; var MoreGroup = 0;
            var Group = ""; var TedadFarzand = ""; var TedadAfradTahteTakafol = ""; var CodeShoghl = ""; var ShomareHokm = "";

            var hpaye = ""; var sanavat = ""; var paye = ""; var sanavatbasiji = ""; var sanavatisar = ""; var foghshoghl = ""; var takhasosi = ""; var made26 = ""; var modiryati = ""; var barjastegi = ""; var tatbigh = ""; var foghisar = "";
            var abohava = ""; var tashilat = ""; var sakhtikar = ""; var tadil = ""; var riali = ""; var jazb9 = ""; var jazb = ""; var makhsos = ""; var vije = ""; var olad = ""; var ayelemandi = ""; var kharobar = ""; var maskan = "";
            var nobatkari = ""; var bon = ""; var jazbtabsare = ""; var talash = ""; var jebhe = "";
            var janbazi = ""; var sayer = ""; var ezafekar = ""; var mamoriat = ""; var tatilkari = ""; var s_payankhedmat = ""; var ghazai = ""; var ashoora = ""; var zaribtadil = ""; var jazbTakhasosi = "";
            var jazbtadil = ""; var hadaghaltadil = "";

            var Karkard = ""; var Gheybat = ""; var TedadEzafeKar = ""; var BimeOmrKarFarma = ""; var Mamooriyat = ""; var TedadTatilKar = ""; var BimeOmrM = ""; var BimeTakmily = "";
            var BimeTakmilyKarFarma = ""; var HaghDarmanKarfFarma = ""; var HaghDarmanDolat = ""; var HaghDarman = ""; var BimePersonal = ""; var BimeKarFarma = ""; var BimeBikari = "";
            var BimeMashaghel = ""; var TedadNobatKari = ""; var Mosaede = ""; var GhestVam = ""; var PasAndazM = ""; var MashmolBime = ""; var MashmolMaliyat = "";
            var Mogharari = ""; var Maliyat = ""; var khalesPardakhti = ""; var ShomareHesab = "";

            var TarikhSodoor = ""; var CityTavalod = ""; var MadrakTitle = ""; var SharhEsargari = ""; var NoeEstekhdamTitle = ""; var StatusTitle = "";

            string commandText = " SELECT ISNULL(fldName,'')fldName,ISNULL(fldFamily,'')fldFamily, ISNULL(fldCodemeli,'')fldCodemeli, ISNULL(fldStatus,'')fldStatus,ISNULL(fldFatherName,'')fldFatherName, " +
" ISNULL(fldJensiyat,'')fldJensiyat,ISNULL(fldTarikhTavalod,'')fldTarikhTavalod, ISNULL(fldStatusEsargari,'')fldStatusEsargari,  " +
" ISNULL(fldCodePosti,'')fldCodePosti,ISNULL(fldAddress,'')fldAddress,ISNULL(fldTel,'')fldTel,ISNULL(fldMobile,'')fldMobile,ISNULL(fldMadrakTahsili,'')fldMadrakTahsili,    " +
" ISNULL(fldReshteTahsili,'')fldReshteTahsili,ISNULL(fldRasteShoghli,'')fldRasteShoghli,ISNULL(fldReshteShoghli,'')fldReshteShoghli,    " +
" ISNULL(fldOrganizationalPosts,'')fldOrganizationalPosts,ISNULL(fldTabaghe,'')fldTabaghe,ISNULL(fldShomareMojavezEstekhdam,'')fldShomareMojavezEstekhdam,    " +
" ISNULL(fldTarikhMojavezEstekhdam,'')fldTarikhMojavezEstekhdam,ISNULL(fldMahleKhedmat,'')fldMahleKhedmat,ISNULL(fldTarikhEjra,'')fldTarikhEjra,  " +
" ISNULL(fldTarikhSodoorHokm,'')fldTarikhSodoorHokm,ISNULL(fldTarikhEtmam,'')fldTarikhEtmam,ISNULL(fldTitleEstekhdam,'')fldTitleEstekhdam,  " +
" ISNULL(fldGroup,'')fldGroup,ISNULL(fldMoreGroup,'')fldMoreGroup,ISNULL(fldShomarePostSazmani,'')fldShomarePostSazmani,ISNULL(fldTedadFarzand,'')fldTedadFarzand,ISNULL(fldNezamVazifeTitle,'')fldNezamVazifeTitle,  " +
" ISNULL(fldMeliyatName,'') fldMeliyatName,ISNULL(fldBimeOmrName,'')fldBimeOmrName,ISNULL(fldBimeTakmiliName,'')fldBimeTakmiliName,  " +
" ISNULL(fldMashagheleSakhtVaZianAvarName,'')fldMashagheleSakhtVaZianAvarName,ISNULL(fldMazadCSalName,'')fldMazadCSalName,  " +
" ISNULL(fldPasAndazName,'')fldPasAndazName,ISNULL(fldSanavatPayanKhedmatName,'')fldSanavatPayanKhedmatName,ISNULL(fldHamsarKarmandName,'')fldHamsarKarmandName,  " +
" ISNULL(fldTedadAfradTahteTakafol,'')fldTedadAfradTahteTakafol,ISNULL(fldTypehokm,'')fldTypehokm,ISNULL(fldShomareHokm,'')fldShomareHokm,    " +
" ISNULL(fldDescriptionHokm,'')fldDescriptionHokm,ISNULL(fldCodeShoghl,'')fldCodeShoghl,ISNULL(StatusTaaholTitle,'')StatusTaaholTitle,  " +
" ISNULL(fldCodeShoghliBime,'')fldCodeShoghliBime,ISNULL(fldShomareBime,'')fldShomareBime,ISNULL(fldShPasAndazPersonal,'')fldShPasAndazPersonal,    " +
" ISNULL(fldShPasAndazKarFarma,'')fldShPasAndazKarFarma,ISNULL(fldTedadBime1,'')fldTedadBime1,ISNULL(fldTedadBime2,'')fldTedadBime2,    " +
" ISNULL(fldTedadBime3,'')fldTedadBime3,ISNULL(fldT_Asli,'')fldT_Asli,ISNULL(fldT_70,'')fldT_70,ISNULL(fldT_60,'')fldT_60, ISNULL(fldSumItem,'')fldSumItem,isnull(Ertegha,'')as Ertegha,ISNULL(fldOrganEjraeeName,'')fldOrganEjraeeName, " +
" ISNULL(fldHamsareKarmand,'')fldHamsareKarmand,ISNULL(fldMazad30Sal,'')fldMazad30Sal,ISNULL(StatusEsargariTitle,'')StatusEsargariTitle,  " +
" ISNULL(fldShomareHesab,'')fldShomareHesab,ISNULL(fldShomareKart,'')fldShomareKart,ISNULL(fldWorkShopNum,'')fldWorkShopNum,ISNULL(fldWorkShopName,'')+'_'+ISNULL(fldWorkShopNum,'')fldWorkShopName,  " +
" ISNULL(fldEmployerName,'')fldEmployerName,ISNULL(CostCenterTitle,'')CostCenterTitle,ISNULL(TypeBimeTitle,'')TypeBimeTitle, ISNULL(OrganName,'')OrganName,ISNULL(fldSh_Shenasname,'')fldSh_Shenasname,  " +
" ISNULL(MahaleSodoorName,'')MahaleSodoorName,ISNULL(MahalTavalodName,'')MahalTavalodName,ISNULL(fldMahalTavalodId,'')fldMahalTavalodId,ISNULL(fldMahalSodoorId,'')fldMahalSodoorId,    " +
" ISNULL(fldTarikhSodoor,'')fldTarikhSodoor, ISNULL(fldKarkard,'')fldKarkard,ISNULL(fldGheybat,'')fldGheybat,ISNULL(fldTedadEzafeKar,'')fldTedadEzafeKar,    " +
" ISNULL(fldTedadTatilKar,'')fldTedadTatilKar,ISNULL(CAST(Mamooriyat AS INT),0)Mamooriyat,ISNULL(fldBimeOmrKarFarma,'')fldBimeOmrKarFarma,  " +
" ISNULL(fldBimeOmr,'')fldBimeOmr,ISNULL(fldBimeTakmilyKarFarma,'')fldBimeTakmilyKarFarma,ISNULL(fldBimeTakmily,'')fldBimeTakmily,  " +
" ISNULL(fldHaghDarmanKarfFarma,'')fldHaghDarmanKarfFarma,ISNULL(fldHaghDarmanDolat,'')fldHaghDarmanDolat,ISNULL(fldHaghDarman,'')fldHaghDarman,  " +
" ISNULL(fldBimePersonal,'')fldBimePersonal,ISNULL(fldBimeKarFarma,'')fldBimeKarFarma,ISNULL(fldBimeBikari,'')fldBimeBikari,  " +
" ISNULL(fldBimeMashaghel,'')fldBimeMashaghel,ISNULL(fldTedadNobatKari,'')fldTedadNobatKari,ISNULL(fldMosaede,'')fldMosaede,  " +
" ISNULL(fldGhestVam,'')fldGhestVam,ISNULL(fldNobatPardakht,'')fldNobatPardakht, ISNULL(fldPasAndaz,'')fldPasAndaz,  " +
" ISNULL(fldMashmolBime,'')fldMashmolBime,ISNULL(fldMashmolMaliyat,'')fldMashmolMaliyat,ISNULL(fldMaliyat,'')fldMaliyat,  " +
" ISNULL((motalebat-kosurat),'')fldkhalesPardakhti,ISNULL(fldJobeCode,'')fldJobeCode,ISNULL(fldSh_Personali,'')fldSh_Personali,  " +
" ISNULL(fldOrganPostName,'')fldOrganPostName,ISNULL(fldYear,'')fldYear, ISNULL(fldMonth,'')fldMonth, ISNULL(fldJobDesc,'')+'_'+ISNULL(fldJobeCode,'')fldJobDesc,ISNULL(fldMogharari,'')fldMogharari,ISNULL(fldSharhEsargari,'')fldSharhEsargari,  " +
" ISNULL(fldTarikhEstekhdam,'')fldTarikhEstekhdam,ISNULL(fldSh_MojavezEstekhdam,'')fldSh_MojavezEstekhdam,ISNULL(fldTarikhMajavezEstekhdam,'')fldTarikhMajavezEstekhdam,  " +
" ISNULL(fldStatusIsargariId,'')fldStatusIsargariId,ISNULL(fldTypeBimeId,'')fldTypeBimeId,ISNULL(fldInsuranceWorkShopId,'')fldInsuranceWorkShopId,  " +
" ISNULL(fldCostCenterId,'')fldCostCenterId,ISNULL(fldAnvaeEstekhdamId,'')fldAnvaeEstekhdamId,ISNULL(fldStatusTaaholId,'')fldStatusTaaholId,ISNULL(statusPay,'')statusPay,  " +
" ISNULL(statusPrs,'')statusPrs,  " +
" ISNULL(case when statusPay=1  then N'فعال' when statusPay=2  then N'غیرفعال'  when statusPay=3  then N'بازنشسته'  end ,'')statusPayName,  " +
" ISNULL(case when statusPrs=1  then N'فعال' when statusPrs=2  then N'غیرفعال'  when statusPrs=3  then N'بازنشسته'  end,'')statusPrsName,  " +
" ISNULL(fldJensiyatName,'')fldJensiyatName,ISNULL(fldMadrakId,'')fldMadrakId,ISNULL(fldOrganId,'')fldOrganId,  " +
" ISNULL([h-paye],'')[h-paye],ISNULL([sanavat],'')[sanavat],ISNULL([paye],'')[paye],ISNULL([sanavat-basiji],'')[sanavat-basiji],  " +
" ISNULL([sanavat-isar],'')[sanavat-isar],ISNULL([foghshoghl],'')[foghshoghl],ISNULL([takhasosi],'')[takhasosi],ISNULL([made26],'')[made26],  " +
" ISNULL([modiryati],'')[modiryati],ISNULL([barjastegi],'')[barjastegi],ISNULL([tatbigh],'')[tatbigh],ISNULL([fogh-isar],'')[fogh-isar],  " +
" ISNULL([abohava],'')[abohava],ISNULL([tashilat],'')[tashilat],ISNULL([sakhtikar],'')[sakhtikar],ISNULL([tadil],'')[tadil],  " +
" ISNULL([riali],'')[riali],[jazb9]=ISNULL([jazb9],0)+ISNULL([jazb2],0)+ISNULL([jazb3],0),ISNULL([jazb],'')[jazb]," +
" [makhsos]=ISNULL([makhsos],0)+ISNULL([makhsos2],0)+ISNULL([makhsos3],0)," +
"	[vije]=ISNULL([vije],0)+ISNULL([vije2],0)+ISNULL([vije3],0),ISNULL([khas],'')[khas],ISNULL([karane],'')[karane],ISNULL([refahi],'')[refahi]," +
" ISNULL([olad],'')[olad],ISNULL([ayelemandi],'')[ayelemandi],ISNULL([kharobar],'')[kharobar],ISNULL([maskan],'')[maskan],ISNULL([nobatkari],'')[nobatkari],ISNULL([bon],'')[bon],  " +
" ISNULL([jazb-tabsare],'')[jazb-tabsare],ISNULL([talash],'')[talash],ISNULL([jebhe],'')[jebhe],ISNULL([janbazi],'')[janbazi],ISNULL([sayer],'')[sayer],ISNULL([ezafekar],'')[ezafekar] ,  " +
" ISNULL([mamoriat],'')[mamoriat],ISNULL([tatilkari],'')[tatilkari] ,ISNULL([s_payankhedmat],'')[s_payankhedmat],ISNULL([ghazai],'')ghazai,  " +
" ISNULL([ashoora],'')ashoora,ISNULL([zaribtadil],'')zaribtadil,ISNULL([jazbTakhasosi],'') jazbTakhasosi ,ISNULL([jazbtadil],'') jazbtadil,ISNULL([hadaghaltadil],'') hadaghaltadil,ISNULL([shift],'')[shift] , " +
" ISNULL([band_y],0)+ISNULL([joz1],0)+ISNULL([band6],0)as band_y " +
" from(   " +
" SELECT     Com.tblEmployee.fldName, Com.tblEmployee.fldFamily, Com.tblEmployee.fldCodemeli, Com.tblEmployee.fldStatus, Com.tblEmployee_Detail.fldFatherName,    " +
   "                 Com.tblEmployee_Detail.fldJensiyat,Com.tblEmployee_Detail.fldTel,Com.tblEmployee_Detail.fldMobile, Com.tblEmployee_Detail.fldTarikhTavalod, Prs.tblHokm_InfoPersonal_History.fldStatusEsargari,    " +
     "               Prs.tblHokm_InfoPersonal_History.fldCodePosti, Prs.tblHokm_InfoPersonal_History.fldAddress, Prs.tblHokm_InfoPersonal_History.fldMadrakTahsili,    " +
       "             Prs.tblHokm_InfoPersonal_History.fldReshteTahsili, Prs.tblHokm_InfoPersonal_History.fldRasteShoghli, Prs.tblHokm_InfoPersonal_History.fldReshteShoghli,    " +
         "           Prs.tblHokm_InfoPersonal_History.fldOrganizationalPosts, Prs.tblHokm_InfoPersonal_History.fldTabaghe,    " +
           "        Prs.tblHokm_InfoPersonal_History.fldShomareMojavezEstekhdam, Prs.tblHokm_InfoPersonal_History.fldTarikhMojavezEstekhdam,    " +
                /*"        Prs.tblHokm_InfoPersonal_History.fldMahleKhedmat"*/
            "(select fldTitle from com.tblChartOrgan where com.tblChartOrgan.fldid in" +
            "(select fldChartOrganId from com.tblOrganizationalPosts where com.tblOrganizationalPosts.fldid=" +
            "(select fldOrganPostId from prs.Prs_tblPersonalInfo as p where p.fldId=Prs.Prs_tblPersonalInfo.fldid))) as fldMahleKhedmat, Prs.tblPersonalHokm.fldTarikhEjra, Prs.tblPersonalHokm.fldTarikhSodoor AS fldTarikhSodoorHokm, Prs.tblPersonalHokm.fldTarikhEtmam,    " +
              "      Com.tblAnvaEstekhdam.fldTitle AS fldTitleEstekhdam, Prs.tblPersonalHokm.fldGroup, Prs.tblPersonalHokm.fldMoreGroup, Prs.tblPersonalHokm.fldShomarePostSazmani,    " +
                "    Prs.tblPersonalHokm.fldTedadFarzand, Prs.tblPersonalHokm.fldTedadAfradTahteTakafol, Prs.tblPersonalHokm.fldTypehokm, Prs.tblPersonalHokm.fldShomareHokm,    " +
"                       Prs.tblPersonalHokm.fldDescriptionHokm, Prs.tblPersonalHokm.fldCodeShoghl, Com.tblStatusTaahol.fldTitle AS StatusTaaholTitle,    " +
"                      Pay.tblMohasebat_PersonalInfo.fldCodeShoghliBime, Pay.tblMohasebat_PersonalInfo.fldShomareBime, Pay.tblMohasebat_PersonalInfo.fldShPasAndazPersonal,    " +
"                    Pay.tblMohasebat_PersonalInfo.fldShPasAndazKarFarma, Pay.tblMohasebat_PersonalInfo.fldTedadBime1, Pay.tblMohasebat_PersonalInfo.fldTedadBime2,    " +
"                   Pay.tblMohasebat_PersonalInfo.fldTedadBime3, Pay.tblMohasebat_PersonalInfo.fldT_Asli, Pay.tblMohasebat_PersonalInfo.fldT_70,    " +
 "                  Pay.tblMohasebat_PersonalInfo.fldT_60, Pay.tblMohasebat_PersonalInfo.fldHamsareKarmand, Pay.tblMohasebat_PersonalInfo.fldMazad30Sal,    " +
  "                 Prs.tblVaziyatEsargari.fldTitle AS StatusEsargariTitle, com.tblShomareHesabeOmoomi.fldShomareHesab,Com.tblShomareHesabOmoomi_Detail.fldShomareKart,    " +
   "                Pay.tblInsuranceWorkshop.fldWorkShopNum, Pay.tblInsuranceWorkshop.fldWorkShopName, Pay.tblInsuranceWorkshop.fldEmployerName,    " +
    "               Pay.tblCostCenter.fldTitle AS CostCenterTitle, Com.tblTypeBime.fldTitle AS TypeBimeTitle,com.fn_stringDecode( Com.tblOrganization.fldName) AS OrganName, Com.tblEmployee_Detail.fldSh_Shenasname,    " +
     "              tblCity_1.fldName AS MahaleSodoorName, Com.tblCity.fldName AS MahalTavalodName, Com.tblEmployee_Detail.fldMahalTavalodId, Com.tblEmployee_Detail.fldMahalSodoorId,    " +
      "             Com.tblEmployee_Detail.fldTarikhSodoor, Pay.tblMohasebat.fldKarkard, Pay.tblMohasebat.fldGheybat, Pay.tblMohasebat.fldTedadEzafeKar,    " +
       "            cast((Pay.tblMohasebat.fldTedadTatilKar/7.33) as int)fldTedadTatilKar, Pay.tblMohasebat.fldBaBeytute+ Pay.tblMohasebat.fldBedunBeytute AS Mamooriyat, Pay.tblMohasebat.fldBimeOmrKarFarma,    " +
        "           Pay.tblMohasebat.fldBimeOmr, Pay.tblMohasebat.fldBimeTakmilyKarFarma, Pay.tblMohasebat.fldBimeTakmily,  " +
         "          CASE WHEN fldMeliyat = 0 THEN N'غیر ایرانی' ELSE N'ایرانی' END AS fldMeliyatName,  " +
          "          Pay.tblMohasebat.fldHaghDarmanKarfFarma  " +
"  AS fldHaghDarmanKarfFarma,  " +
" Pay.tblMohasebat.fldHaghDarmanDolat " +
"as fldHaghDarmanDolat   " +
" ,Pay.tblMohasebat.fldHaghDarman " +
"  AS fldHaghDarman  " +
" ,Pay.tblMohasebat.fldBimePersonal " +
" AS fldBimePersonal        ,  " +
" Pay.tblMohasebat.fldBimeKarFarma " +
"  AS fldBimeKarFarma        ,  " +
" Pay.tblMohasebat.fldBimeBikari " +
"  AS fldBimeBikari  " +
" ,Pay.tblMohasebat.fldBimeMashaghel " +
" AS fldBimeMashaghel  " +
" ,Pay.tblMohasebat.fldTedadNobatKari                      ,Pay.tblMohasebat.fldMosaede,               " +
" Pay.tblMohasebat.fldGhestVam, Pay.tblMohasebat.fldNobatPardakht,           " +
" Pay.tblMohasebat.fldPasAndaz " +
" AS fldPasAndaz ,      " +
" Pay.tblMohasebat.fldMashmolBime " +
"AS fldMashmolBime,  " +
" Pay.tblMohasebat.fldMashmolMaliyat " +
"AS fldMashmolMaliyat,  " +
" Pay.tblMohasebat.fldMogharari, Pay.tblMohasebat.fldMaliyat " +
" AS fldMaliyat    " +
       "         ,isnull((select SUM(ISNULL(fldMablagh,0)) FROM Pay.tblMohasebat_Items    " +
        "     	WHERE fldMohasebatId=tblmohasebat.fldid),0)+   " +
          "   	isnull((select SUM(fldMablagh) FROM [pay].[tblMohasebat_kosorat/MotalebatParam]   " +
           "  	WHERE  fldMohasebatId=tblmohasebat.fldid AND fldKosoratId IS NULL),0) +  " +
            " 	+[fldHaghDarmanKarfFarma]+[fldHaghDarmanDolat]+(tblmohasebat.[fldPasAndaz]/(2))+[fldBimeTakmilyKarFarma]+[fldBimeOmrKarFarma]   " +
              "   AS motalebat,    " +
              "   	fldMaliyat+ " +
"fldBimePersonal+tblmohasebat.fldBimeOmr+fldMogharari+fldGhestVam+fldBimeTakmily+fldHaghDarman+tblmohasebat.fldPasAndaz+fldMosaede  + " +
" isnull((select sum(fldMablagh) from pay.[tblMohasebat_kosorat/MotalebatParam] where fldMohasebatId=tblMohasebat.fldid and fldKosoratId is not null),0)+   " +
"isnull((select sum(fldMablagh) from pay.tblMohasebat_KosoratBank where fldMohasebatId=tblMohasebat.fldId),0) as kosurat,  " +
" Pay.Pay_tblPersonalInfo.fldJobeCode,    " +
         "         Prs.Prs_tblPersonalInfo.fldSh_Personali, (select fldTitle from com.tblOrganizationalPosts where fldId=Prs.Prs_tblPersonalInfo.fldOrganPostId) AS fldOrganPostName, Pay.tblMohasebat.fldYear, Pay.tblMohasebat.fldMonth,    " +
         "          Com.tblItemsHoghughi.fldNameEN, Prs.Prs_tblPersonalInfo.fldTarikhEstekhdam,  " +
          "         Prs.Prs_tblPersonalInfo.fldSh_MojavezEstekhdam, Prs.Prs_tblPersonalInfo.fldTarikhMajavezEstekhdam, Pay.tblMohasebat_PersonalInfo.fldStatusIsargariId,    " +
           "       Pay.tblMohasebat_PersonalInfo.fldTypeBimeId, Pay.tblMohasebat_PersonalInfo.fldInsuranceWorkShopId, Pay.tblMohasebat_PersonalInfo.fldCostCenterId,    " +
          "         Prs.tblPersonalHokm.fldAnvaeEstekhdamId, Prs.tblPersonalHokm.fldStatusTaaholId " +
          ",      /* com.fn_MaxPersonalStatus(Pay.Pay_tblPersonalInfo.fldId,'hoghoghi') AS*/ statusPay   " +
           "        ,/*com.fn_MaxPersonalStatus(Prs.Prs_tblPersonalInfo.fldId,'kargozini') AS*/ statusPrs   " +
           "       ,CASE WHEN fldJensiyat=1 THEN N'مرد' ELSE N'زن' END AS fldJensiyatName,tblHokm_InfoPersonal_History.fldMadrakId   " +
           "  	,(select fldMablagh from pay.tblMohasebat_Items where fldMohasebatId=tblMohasebat.fldid and fldItemEstekhdamId=tblItems_Estekhdam.fldId)fldMablagh   " +
           "  	,tblMohasebat_PersonalInfo.fldOrganId,  " +
          "   CASE WHEN (Pay.Pay_tblPersonalInfo.fldBimeOmr = 1) THEN N'دارد' ELSE N'ندارد' END AS fldBimeOmrName,    " +
           "        CASE WHEN (Pay.Pay_tblPersonalInfo.fldBimeTakmili = 1) THEN N'دارد' ELSE N'ندارد' END AS fldBimeTakmiliName,    " +
           "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldMashagheleSakhtVaZianAvar = 1) THEN N'دارد' ELSE N'ندارد' END AS fldMashagheleSakhtVaZianAvarName,    " +
            "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldMazad30Sal = 1) THEN N'دارد' ELSE N'ندارد' END AS fldMazadCSalName,    " +
            "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldPasAndaz = 1) THEN N'دارد' ELSE N'ندارد' END AS fldPasAndazName,    " +
            "       CASE WHEN (Pay.Pay_tblPersonalInfo.fldSanavatPayanKhedmat = 1) THEN N'دارد' ELSE N'ندارد' END AS fldSanavatPayanKhedmatName,    " +
           "        CASE WHEN (Pay.Pay_tblPersonalInfo.fldHamsarKarmand = 1) THEN N'دارد' ELSE N'ندارد' END AS fldHamsarKarmandName,fldSharhEsargari   " +
     "        	,(select fldJobDesc from pay.tblTabJobOfBime where fldJobCode=pay.Pay_tblPersonalInfo.fldJobeCode)fldJobDesc   " +
    "         	,isnull((select fldTitle from com.tblNezamVazife where fldId= Com.tblEmployee_Detail.fldNezamVazifeId),'') AS fldNezamVazifeTitle   " +
     "           ,h.fldSumItem,(SELECT TOP (1) fldTarikh FROM Prs.tblSavabeghGroupTashvighi WHERE (fldPersonalId = Prs.Prs_tblPersonalInfo.fldid) ORDER BY fldTarikh DESC)as Ertegha,fldPostEjraee AS fldOrganEjraeeName " +/*AND (fldTarikh <= '1400/08/30') به درخواست شاهرود*/
    "             FROM         Pay.tblMohasebat_PersonalInfo INNER JOIN   " +
    "              Prs.tblPersonalHokm ON Pay.tblMohasebat_PersonalInfo.fldHokmId = Prs.tblPersonalHokm.fldId INNER JOIN   " +
    "               Pay.tblMohasebat ON Pay.tblMohasebat_PersonalInfo.fldMohasebatId = Pay.tblMohasebat.fldId INNER JOIN   " +
     "              Pay.Pay_tblPersonalInfo ON Pay.tblMohasebat.fldPersonalId = Pay.Pay_tblPersonalInfo.fldId INNER JOIN   " +
     "              Prs.Prs_tblPersonalInfo ON Prs.Prs_tblPersonalInfo.fldId = Pay.Pay_tblPersonalInfo.fldPrs_PersonalInfoId INNER JOIN   " +
     "              Com.tblEmployee ON Prs.Prs_tblPersonalInfo.fldEmployeeId = Com.tblEmployee.fldId INNER JOIN   " +
      "             Com.tblEmployee_Detail ON Com.tblEmployee_Detail.fldEmployeeId = Com.tblEmployee.fldId INNER JOIN   " +
       "            Pay.tblMohasebat_Items ON Pay.tblMohasebat.fldId = Pay.tblMohasebat_Items.fldMohasebatId INNER JOIN   " +
      "             Com.tblItems_Estekhdam ON Pay.tblMohasebat_Items.fldItemEstekhdamId = Com.tblItems_Estekhdam.fldId INNER JOIN   " +
      "             Com.tblItemsHoghughi ON Com.tblItemsHoghughi.fldId = Com.tblItems_Estekhdam.fldItemsHoghughiId INNER JOIN   " +
      "             Prs.tblHokm_InfoPersonal_History ON Prs.tblPersonalHokm.fldId = Prs.tblHokm_InfoPersonal_History.fldPersonalHokmId INNER JOIN   " +
      "             Com.tblAnvaEstekhdam ON Prs.tblPersonalHokm.fldAnvaeEstekhdamId = Com.tblAnvaEstekhdam.fldId INNER JOIN   " +
      "             Com.tblStatusTaahol ON Prs.tblPersonalHokm.fldStatusTaaholId = Com.tblStatusTaahol.fldId INNER JOIN   " +
      "             Pay.tblCostCenter ON Pay.tblMohasebat_PersonalInfo.fldCostCenterId = Pay.tblCostCenter.fldId INNER JOIN  " +
      "             Pay.tblInsuranceWorkshop ON Pay.tblMohasebat_PersonalInfo.fldInsuranceWorkShopId = Pay.tblInsuranceWorkshop.fldId INNER JOIN   " +
      "             Com.tblTypeBime ON Pay.tblMohasebat_PersonalInfo.fldTypeBimeId = Com.tblTypeBime.fldId INNER JOIN   " +
      "             com.tblShomareHesabeOmoomi ON Pay.tblMohasebat_PersonalInfo.fldShomareHesabId =com.tblShomareHesabeOmoomi.fldId INNER JOIN   " +
      "             com.tblShomareHesabOmoomi_Detail ON com.tblShomareHesabeOmoomi.fldId=com.tblShomareHesabOmoomi_Detail.fldShomareHesabId INNER JOIN   " +
      "             Prs.tblVaziyatEsargari ON Pay.tblMohasebat_PersonalInfo.fldStatusIsargariId = Prs.tblVaziyatEsargari.fldId INNER JOIN   " +
      "             Com.tblOrganization ON Pay.tblMohasebat_PersonalInfo.fldOrganId = Com.tblOrganization.fldId INNER JOIN   " +
      "             Com.tblCity ON Com.tblEmployee_Detail.fldMahalTavalodId = Com.tblCity.fldId INNER JOIN   " +
      "             Com.tblCity AS tblCity_1 ON Com.tblEmployee_Detail.fldMahalSodoorId = tblCity_1.fldId   " +
      "           outer apply(SELECT SUM(h.fldMablagh) as fldSumItem FROM Prs.tblHokm_Item as h WHERE fldPersonalHokmId=Prs.tblPersonalHokm.fldId and fldItems_EstekhdamId not in(85,86,150,151,152,153,154))h  " +
      "           outer apply(SELECT top 1 fldStatusId as statusPrs FROM Com.tblPersonalStatus as s where s.fldPrsPersonalInfoId=Prs.Prs_tblPersonalInfo.fldId order by s.fldDateTaghirVaziyat desc)PrsStatus  " +
      "          outer apply(SELECT top 1 fldStatusId as statusPay FROM Com.tblPersonalStatus as s where s.fldPrsPersonalInfoId=Pay.Pay_tblPersonalInfo.fldId order by s.fldDateTaghirVaziyat desc)PayStatus  " +

                  " WHERE tblMohasebat_PersonalInfo.fldOrganId in ( " + Organ + " ) and fldYear>=" + Year + " AND fldMonth>=" + Month + " and fldYear<=" + TaYear + " AND fldMonth<=" + TaMonth + " AND fldNobatPardakht=" + NobatePardakht + where(Status, Gender, Esargari, Madrak, NoeEstekhdam, Bime, Hazine, Kargah/*, Status*/) +
                  "     )t " +
               " PIVOT( " +
" MAX(fldMablagh)  " +
" FOR fldNameEN IN ([h-paye],[sanavat],[paye],[sanavat-basiji],[sanavat-isar],[foghshoghl],[takhasosi],[made26],[modiryati],[barjastegi],[tatbigh],[fogh-isar], " +
" [abohava],[tashilat],[sakhtikar],[tadil],[riali],[jazb9],[jazb],[makhsos],[vije],[olad],[ayelemandi],[kharobar],[maskan],[nobatkari],[bon],[jazb-tabsare],[talash],[jebhe] " +
" ,[janbazi],[sayer],[ezafekar],[mamoriat],[tatilkari],[s_payankhedmat],[ghazai],[ashoora],[zaribtadil],[jazbTakhasosi],[jazbtadil],[hadaghaltadil],[shift] ,[band_y],[joz1],[band6],[jazb2],[jazb3],[vije2],[vije3],[makhsos2],[makhsos3],[khas],[karane],[refahi]))  p order by fldFamily,fldname ";


           

            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var per = service.GetData(commandText).Tables[0];

            List<NewFMS.Models.AllPersonField> p = new List<NewFMS.Models.AllPersonField>();
            for (int i = 0; i < per.Rows.Count; i++)
            {
                NewFMS.Models.AllPersonField l = new NewFMS.Models.AllPersonField();
                // l.fldId = (int)(per.Rows[i]["fldId"]);
                l.fldName = per.Rows[i]["fldName"] == null ? "" : per.Rows[i]["fldName"].ToString();
                l.fldFamily = per.Rows[i]["fldFamily"] == null ? "" : per.Rows[i]["fldFamily"].ToString();
                l.fldFatherName = per.Rows[i]["fldFatherName"] == null ? "" : per.Rows[i]["fldFatherName"].ToString();
                l.fldCodemeli = per.Rows[i]["fldCodemeli"] == null ? "" : per.Rows[i]["fldCodemeli"].ToString();
                l.fldNameJensiyat = per.Rows[i]["fldJensiyatName"] == null ? "" : per.Rows[i]["fldJensiyatName"].ToString();
                l.fldSh_Shenasname = per.Rows[i]["fldSh_Shenasname"] == null ? "" : per.Rows[i]["fldSh_Shenasname"].ToString();
                l.fldTarikhTavalod = per.Rows[i]["fldTarikhTavalod"] == null ? "" : per.Rows[i]["fldTarikhTavalod"].ToString();
                l.fldNameMahlSodoor = per.Rows[i]["MahaleSodoorName"] == null ? "" : per.Rows[i]["MahaleSodoorName"].ToString();
                l.fldAddress = per.Rows[i]["fldAddress"] == null ? "" : per.Rows[i]["fldAddress"].ToString();
                l.fldTel = per.Rows[i]["fldTel"] == null ? "" : per.Rows[i]["fldTel"].ToString();
                l.fldMobile = per.Rows[i]["fldMobile"] == null ? "" : per.Rows[i]["fldMobile"].ToString();
                l.fldCodePosti = per.Rows[i]["fldCodePosti"] == null ? "" : per.Rows[i]["fldCodePosti"].ToString();
                l.fldVaziyatEsargariTitle = per.Rows[i]["StatusEsargariTitle"] == null ? "" : per.Rows[i]["StatusEsargariTitle"].ToString();
                l.fldSh_Personali = per.Rows[i]["fldSh_Personali"] == null ? "" : per.Rows[i]["fldSh_Personali"].ToString();
                l.fldReshteTahsiliTitle = per.Rows[i]["fldReshteTahsili"] == null ? "" : per.Rows[i]["fldReshteTahsili"].ToString();
                l.fldNezamVazifeTitle = per.Rows[i]["fldNezamVazifeTitle"] == null ? "" : per.Rows[i]["fldNezamVazifeTitle"].ToString();
                l.fldOrganPostName = per.Rows[i]["fldOrganPostName"] == null ? "" : per.Rows[i]["fldOrganPostName"].ToString();
                l.fldNameOrganEjraee = per.Rows[i]["fldOrganEjraeeName"] == null ? "" : per.Rows[i]["fldOrganEjraeeName"].ToString();
                l.fldMahalKhedmat = per.Rows[i]["fldMahleKhedmat"] == null ? "" : per.Rows[i]["fldMahleKhedmat"].ToString();
                l.fldRasteShoghli = per.Rows[i]["fldRasteShoghli"] == null ? "" : per.Rows[i]["fldRasteShoghli"].ToString();
                l.fldReshteShoghli = per.Rows[i]["fldReshteShoghli"] == null ? "" : per.Rows[i]["fldReshteShoghli"].ToString();
                l.fldTarikhEstekhdam = per.Rows[i]["fldTarikhEstekhdam"] == null ? "" : per.Rows[i]["fldTarikhEstekhdam"].ToString();
                l.fldTabaghe = per.Rows[i]["fldTabaghe"] == null ? "" : per.Rows[i]["fldTabaghe"].ToString();
                l.fldMeliyatName = per.Rows[i]["fldMeliyatName"] == null ? "" : per.Rows[i]["fldMeliyatName"].ToString();
                l.fldSh_MojavezEstekhdam = per.Rows[i]["fldSh_MojavezEstekhdam"] == null ? "" : per.Rows[i]["fldSh_MojavezEstekhdam"].ToString();
                l.fldTarikhMajavezEstekhdam = per.Rows[i]["fldTarikhMajavezEstekhdam"] == null ? "" : per.Rows[i]["fldTarikhMajavezEstekhdam"].ToString();
                l.fldTarikhSodoor = per.Rows[i]["fldTarikhSodoor"] == null ? "" : per.Rows[i]["fldTarikhSodoor"].ToString();
                l.fldNameMahalTavalod = per.Rows[i]["MahalTavalodName"] == null ? "" : per.Rows[i]["MahalTavalodName"].ToString();
                l.fldMadrakTahsiliTitle = per.Rows[i]["fldMadrakTahsili"] == null ? "" : per.Rows[i]["fldMadrakTahsili"].ToString();
                l.fldSharhEsargari = per.Rows[i]["fldSharhEsargari"] == null ? "" : per.Rows[i]["fldSharhEsargari"].ToString();
                l.fldTitleStatus = per.Rows[i]["statusPrsName"] == null ? "" : per.Rows[i]["statusPrsName"].ToString();

                l.fldTitleTypeBime = per.Rows[i]["TypeBimeTitle"] == null ? "" : per.Rows[i]["TypeBimeTitle"].ToString();
                l.fldShomareBime = per.Rows[i]["fldShomareBime"] == null ? "" : per.Rows[i]["fldShomareBime"].ToString();
                l.fldBimeOmrName = per.Rows[i]["fldBimeOmrName"] == null ? "" : per.Rows[i]["fldBimeOmrName"].ToString();
                l.fldBimeTakmiliName = per.Rows[i]["fldBimeTakmiliName"] == null ? "" : per.Rows[i]["fldBimeTakmiliName"].ToString();
                l.fldMashagheleSakhtVaZianAvarName = per.Rows[i]["fldMashagheleSakhtVaZianAvarName"] == null ? "" : per.Rows[i]["fldMashagheleSakhtVaZianAvarName"].ToString();
                l.fldTitleCostCenter = per.Rows[i]["CostCenterTitle"] == null ? "" : per.Rows[i]["CostCenterTitle"].ToString();
                l.fldMazadCSalName = per.Rows[i]["fldMazadCSalName"] == null ? "" : per.Rows[i]["fldMazadCSalName"].ToString();
                l.fldPasAndazName = per.Rows[i]["fldPasAndazName"] == null ? "" : per.Rows[i]["fldPasAndazName"].ToString();
                l.fldSanavatPayanKhedmatName = per.Rows[i]["fldSanavatPayanKhedmatName"] == null ? "" : per.Rows[i]["fldSanavatPayanKhedmatName"].ToString();
                l.fldJobDesc = per.Rows[i]["fldJobDesc"] == null ? "" : per.Rows[i]["fldJobDesc"].ToString();
                l.fldJobeCode = per.Rows[i]["fldJobeCode"] == null ? "" : per.Rows[i]["fldJobeCode"].ToString();
                l.fldWorkShopName = per.Rows[i]["fldWorkShopName"] == null ? "" : per.Rows[i]["fldWorkShopName"].ToString();
                l.fldHamsarKarmandName = per.Rows[i]["fldHamsarKarmandName"] == null ? "" : per.Rows[i]["fldHamsarKarmandName"].ToString();

                l.fldTarikhEjra = per.Rows[i]["fldTarikhEjra"] == null ? "" : per.Rows[i]["fldTarikhEjra"].ToString();
                l.fldTarikhSodoorHokm = per.Rows[i]["fldTarikhSodoorHokm"] == null ? "" : per.Rows[i]["fldTarikhSodoorHokm"].ToString();
                l.fldTarikhEtmam = per.Rows[i]["fldTarikhEtmam"] == null ? "" : per.Rows[i]["fldTarikhEtmam"].ToString();
                l.StatusTaaholTitle = per.Rows[i]["StatusTaaholTitle"] == null ? "" : per.Rows[i]["StatusTaaholTitle"].ToString();
                l.fldShomarePostSazmani = per.Rows[i]["fldShomarePostSazmani"] == null ? "" : per.Rows[i]["fldShomarePostSazmani"].ToString();
                l.fldMoreGroup = per.Rows[i]["fldMoreGroup"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldMoreGroup"]);
                l.fldGroup = per.Rows[i]["fldGroup"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldGroup"]);
                l.fldTedadFarzand = per.Rows[i]["fldTedadFarzand"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldTedadFarzand"]);
                l.fldTedadAfradTahteTakafol = per.Rows[i]["fldTedadAfradTahteTakafol"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldTedadAfradTahteTakafol"]);
                l.fldCodeShoghl = per.Rows[i]["fldCodeShoghl"] == null ? "" : per.Rows[i]["fldCodeShoghl"].ToString();
                l.fldShomareHokm = per.Rows[i]["fldShomareHokm"] == null ? "" : per.Rows[i]["fldShomareHokm"].ToString();
                l.TitleNoeEstekhdam = per.Rows[i]["fldTitleEstekhdam"] == null ? "" : per.Rows[i]["fldTitleEstekhdam"].ToString();
                l.fldJameHokm = per.Rows[i]["fldSumItem"] == null ? 0 : Convert.ToInt64(per.Rows[i]["fldSumItem"]);
                l.Ertegha = per.Rows[i]["Ertegha"] == null ? "" : per.Rows[i]["Ertegha"].ToString();
                l.hpaye = per.Rows[i]["h-paye"] == null ? 0 : Convert.ToInt32(per.Rows[i]["h-paye"]);
                l.sanavat = per.Rows[i]["sanavat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["sanavat"]);
                l.paye = per.Rows[i]["paye"] == null ? 0 : Convert.ToInt32(per.Rows[i]["paye"]);
                l.sanavatbasiji = per.Rows[i]["sanavat-basiji"] == null ? 0 : Convert.ToInt32(per.Rows[i]["sanavat-basiji"]);
                l.sanavatisar = per.Rows[i]["sanavat-isar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["sanavat-isar"]);
                l.foghshoghl = per.Rows[i]["foghshoghl"] == null ? 0 : Convert.ToInt32(per.Rows[i]["foghshoghl"]);
                l.takhasosi = per.Rows[i]["takhasosi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["takhasosi"]);
                l.made26 = per.Rows[i]["made26"] == null ? 0 : Convert.ToInt32(per.Rows[i]["made26"]);
                l.modiryati = per.Rows[i]["modiryati"] == null ? 0 : Convert.ToInt32(per.Rows[i]["modiryati"]);
                l.barjastegi = per.Rows[i]["barjastegi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["barjastegi"]);
                l.tatbigh = per.Rows[i]["tatbigh"] == null ? 0 : Convert.ToInt32(per.Rows[i]["tatbigh"]);
                l.foghisar = per.Rows[i]["fogh-isar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fogh-isar"]);
                l.abohava = per.Rows[i]["abohava"] == null ? 0 : Convert.ToInt32(per.Rows[i]["abohava"]);
                l.tashilat = per.Rows[i]["tashilat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["tashilat"]);
                l.sakhtikar = per.Rows[i]["sakhtikar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["sakhtikar"]);
                l.tadil = per.Rows[i]["tadil"] == null ? 0 : Convert.ToInt32(per.Rows[i]["tadil"]);
                l.riali = per.Rows[i]["riali"] == null ? 0 : Convert.ToInt32(per.Rows[i]["riali"]);
                l.jazb9 = per.Rows[i]["jazb9"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jazb9"]);
                l.jazb = per.Rows[i]["jazb"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jazb"]);
                l.makhsos = per.Rows[i]["makhsos"] == null ? 0 : Convert.ToInt32(per.Rows[i]["makhsos"]);
                l.vije = per.Rows[i]["vije"] == null ? 0 : Convert.ToInt32(per.Rows[i]["vije"]);
                l.olad = per.Rows[i]["olad"] == null ? 0 : Convert.ToInt32(per.Rows[i]["olad"]);
                l.ayelemandi = per.Rows[i]["ayelemandi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["ayelemandi"]);
                l.kharobar = per.Rows[i]["kharobar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["kharobar"]);
                l.maskan = per.Rows[i]["maskan"] == null ? 0 : Convert.ToInt32(per.Rows[i]["maskan"]);
                l.nobatkari = per.Rows[i]["nobatkari"] == null ? 0 : Convert.ToInt32(per.Rows[i]["nobatkari"]);
                l.bon = per.Rows[i]["bon"] == null ? 0 : Convert.ToInt32(per.Rows[i]["bon"]);
                l.jazbtabsare = per.Rows[i]["jazb-tabsare"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jazb-tabsare"]);
                l.talash = per.Rows[i]["talash"] == null ? 0 : Convert.ToInt32(per.Rows[i]["talash"]);
                l.jebhe = per.Rows[i]["jebhe"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jebhe"]);
                l.janbazi = per.Rows[i]["janbazi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["janbazi"]);
                l.sayer = per.Rows[i]["sayer"] == null ? 0 : Convert.ToInt32(per.Rows[i]["sayer"]);
                l.ezafekar = per.Rows[i]["ezafekar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["ezafekar"]);
                l.mamoriat = per.Rows[i]["mamoriat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["mamoriat"]);
                l.tatilkari = per.Rows[i]["tatilkari"] == null ? 0 : Convert.ToInt32(per.Rows[i]["tatilkari"]);
                l.s_payankhedmat = per.Rows[i]["s_payankhedmat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["s_payankhedmat"]);
                l.ghazai = per.Rows[i]["ghazai"] == null ? 0 : Convert.ToInt32(per.Rows[i]["ghazai"]);
                l.ashoora = per.Rows[i]["ashoora"] == null ? 0 : Convert.ToInt32(per.Rows[i]["ashoora"]);
                l.zaribtadil = per.Rows[i]["zaribtadil"] == null ? 0 : Convert.ToInt32(per.Rows[i]["zaribtadil"]);
                l.jazbTakhasosi = per.Rows[i]["jazbTakhasosi"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jazbTakhasosi"]);
                l.jazbtadil = per.Rows[i]["jazbtadil"] == null ? 0 : Convert.ToInt32(per.Rows[i]["jazbtadil"]);
                l.hadaghaltadil = per.Rows[i]["hadaghaltadil"] == null ? 0 : Convert.ToInt32(per.Rows[i]["hadaghaltadil"]);

                l.fldKarkard = per.Rows[i]["fldKarkard"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldKarkard"]);
                l.fldGheybat = per.Rows[i]["fldGheybat"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldGheybat"]);
                l.fldTedadEzafeKar = per.Rows[i]["fldTedadEzafeKar"] == null ? 0 : Convert.ToDecimal(per.Rows[i]["fldTedadEzafeKar"]);
                l.fldBimeOmrKarFarma = per.Rows[i]["fldBimeOmrKarFarma"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeOmrKarFarma"]);
                l.Mamooriyat = per.Rows[i]["Mamooriyat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["Mamooriyat"]);
                l.fldTedadTatilKar = per.Rows[i]["fldTedadTatilKar"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldTedadTatilKar"]);
                l.fldBimeOmrM = per.Rows[i]["fldBimeOmr"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeOmr"]);
                l.fldBimeTakmily = per.Rows[i]["fldBimeTakmily"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeTakmily"]);
                l.fldBimeTakmilyKarFarma = per.Rows[i]["fldBimeTakmilyKarFarma"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeTakmilyKarFarma"]);
                l.fldHaghDarmanKarfFarma = per.Rows[i]["fldHaghDarmanKarfFarma"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldHaghDarmanKarfFarma"]);
                l.fldHaghDarmanDolat = per.Rows[i]["fldHaghDarmanDolat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldHaghDarmanDolat"]);
                l.fldHaghDarman = per.Rows[i]["fldHaghDarman"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldHaghDarman"]);
                l.fldBimePersonal = per.Rows[i]["fldBimePersonal"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimePersonal"]);
                l.fldBimeKarFarma = per.Rows[i]["fldBimeKarFarma"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeKarFarma"]);
                l.fldBimeBikari = per.Rows[i]["fldBimeBikari"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeBikari"]);
                l.fldBimeMashaghel = per.Rows[i]["fldBimeMashaghel"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldBimeMashaghel"]);
                l.fldTedadNobatKari = per.Rows[i]["fldTedadNobatKari"] == null ? Convert.ToByte(0) : Convert.ToByte(per.Rows[i]["fldTedadNobatKari"]);
                l.fldMosaede = per.Rows[i]["fldMosaede"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldMosaede"]);
                l.fldGhestVam = per.Rows[i]["fldGhestVam"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldGhestVam"]);
                l.fldPasAndazM = per.Rows[i]["fldPasAndaz"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldPasAndaz"]);
                l.fldMashmolBime = per.Rows[i]["fldMashmolBime"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldMashmolBime"]);
                l.fldMashmolMaliyat = per.Rows[i]["fldMashmolMaliyat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldMashmolMaliyat"]);
                l.fldMogharari = per.Rows[i]["fldMogharari"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldMogharari"]);
                l.fldMaliyat = per.Rows[i]["fldMaliyat"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldMaliyat"]);
                l.fldkhalesPardakhti = per.Rows[i]["fldkhalesPardakhti"] == null ? 0 : Convert.ToInt32(per.Rows[i]["fldkhalesPardakhti"]);
                l.fldShomareHesab = per.Rows[i]["fldShomareHesab"] == null ? "" : per.Rows[i]["fldShomareHesab"].ToString();

                var prs = Pservic.GetPersoneliInfoWithFilter("fldCodemeli", l.fldCodemeli, Convert.ToInt32(Session["OrganId"]), 0, IP, out PErr).FirstOrDefault();
                var Sanavat = Pservic.GetSavabegheSanavateKHedmatWithFilter("fldPersonalId", prs.fldId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out PErr).ToList();
                int CountTarikh = 0;
                foreach (var item in Sanavat)
                {
                    CountTarikh += MyLib.Shamsi.DiffOfShamsiDate(item.fldAzTarikh, item.fldTaTarikh);
                }

                CountTarikh += MyLib.Shamsi.DiffOfShamsiDate(prs.fldTarikhEstekhdam, l.fldTarikhEjra);

                var DayMahSal = Common_Payservic.GetDiffDayMahSalWithFilter(CountTarikh, IP, out ErrC_P).FirstOrDefault();
                var SanavatRasmi = DayMahSal.CountString;
                l.fldSanavtKhedmat = SanavatRasmi;
                p.Add(l);
            }
            var Personal = p.ToList();
            /////////////////////////////////////
            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];

            foreach (var item in StatusCheck)
            {
                // var Personal = servic.GetPersoneliInfoWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 0, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).ToList();
                switch (item)
                {
                    case "Name":
                        Check = "نام";
                        Cell cell = sheet.Cells[alpha[index] + "1"];
                        cell.PutValue(Check);
                        int i = 0;
                        foreach (var _item in Personal)
                        {
                            Name = _item.fldName;
                            Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                            Cell.PutValue(Name);
                            i++;
                        }
                        index++;
                        break;
                    case "Family":
                        Check = "نام خانوادگی";
                        Cell cell1 = sheet.Cells[alpha[index] + "1"];
                        cell1.PutValue(Check);
                        int i1 = 0;
                        foreach (var _item in Personal)
                        {
                            FamilyName = _item.fldFamily;
                            Cell Cell = sheet.Cells[alpha[index] + (i1 + 2)];
                            Cell.PutValue(FamilyName);
                            i1++;
                        }
                        index++;
                        break;
                    case "Father":
                        Check = "نام پدر";
                        Cell cell2 = sheet.Cells[alpha[index] + "1"];
                        cell2.PutValue(Check);
                        int i2 = 0;
                        foreach (var _item in Personal)
                        {
                            FatherName = _item.fldFatherName;
                            Cell Cell = sheet.Cells[alpha[index] + (i2 + 2)];
                            Cell.PutValue(FatherName);
                            i2++;
                        }
                        index++;
                        break;
                    case "MeliCode":
                        Check = "کدملی";
                        Cell cell3 = sheet.Cells[alpha[index] + "1"];
                        cell3.PutValue(Check);
                        int i3 = 0;
                        foreach (var _item in Personal)
                        {
                            Codemeli = _item.fldCodemeli;
                            Cell Cell = sheet.Cells[alpha[index] + (i3 + 2)];
                            Cell.PutValue(Codemeli);
                            i3++;
                        }
                        index++;
                        break;
                    case "Jensiyat":
                        Check = "جنسیت";
                        Cell cell4 = sheet.Cells[alpha[index] + "1"];
                        cell4.PutValue(Check);
                        int i4 = 0;
                        foreach (var _item in Personal)
                        {
                            GenderChar = _item.fldNameJensiyat;
                            Cell Cell = sheet.Cells[alpha[index] + (i4 + 2)];
                            Cell.PutValue(GenderChar);
                            i4++;
                        }
                        index++;
                        break;
                    case "ShomareShenasname":
                        Check = "شماره شناسنامه";
                        Cell cell5 = sheet.Cells[alpha[index] + "1"];
                        cell5.PutValue(Check);
                        int i5 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomareShenasname = _item.fldSh_Shenasname;
                            Cell Cell = sheet.Cells[alpha[index] + (i5 + 2)];
                            Cell.PutValue(ShomareShenasname);
                            i5++;
                        }
                        index++;
                        break;
                    case "TarikhTavalod":
                        Check = "تاریخ تولد";
                        Cell cell6 = sheet.Cells[alpha[index] + "1"];
                        cell6.PutValue(Check);
                        int i6 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhTavalod = _item.fldTarikhTavalod;
                            Cell Cell = sheet.Cells[alpha[index] + (i6 + 2)];
                            Cell.PutValue(TarikhTavalod);
                            i6++;
                        }
                        index++;
                        break;
                    case "City":
                        Check = "شهر محل صدور";
                        Cell cell7 = sheet.Cells[alpha[index] + "1"];
                        cell7.PutValue(Check);
                        int i7 = 0;
                        foreach (var _item in Personal)
                        {
                            CityName = _item.fldNameMahlSodoor;
                            Cell Cell = sheet.Cells[alpha[index] + (i7 + 2)];
                            Cell.PutValue(CityName);
                            i7++;
                        }
                        index++;
                        break;
                    case "Addres":
                        Check = "آدرس";
                        Cell cell8 = sheet.Cells[alpha[index] + "1"];
                        cell8.PutValue(Check);
                        int i8 = 0;
                        foreach (var _item in Personal)
                        {
                            Address = _item.fldAddress;
                            Cell Cell = sheet.Cells[alpha[index] + (i8 + 2)];
                            Cell.PutValue(Address);
                            i8++;
                        }
                        index++;
                        break;
                    case "CodePosti":
                        Check = "کد پستی";
                        Cell cell9 = sheet.Cells[alpha[index] + "1"];
                        cell9.PutValue(Check);
                        int i9 = 0;
                        foreach (var _item in Personal)
                        {
                            CodePosti = _item.fldCodePosti;
                            Cell Cell = sheet.Cells[alpha[index] + (i9 + 2)];
                            Cell.PutValue(CodePosti);
                            i9++;
                        }
                        index++;
                        break;
                    case "StatusEsargari":
                        Check = "وضعیت ایثارگری";
                        Cell cell10 = sheet.Cells[alpha[index] + "1"];
                        cell10.PutValue(Check);
                        int i10 = 0;
                        foreach (var _item in Personal)
                        {
                            StatusEsargari = _item.fldVaziyatEsargariTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (i10 + 2)];
                            Cell.PutValue(StatusEsargari);
                            i10++;
                        }
                        index++;
                        break;

                    case "ShomarePersoneli":
                        Check = "شماره پرسنلی";
                        Cell cell11 = sheet.Cells[alpha[index] + "1"];
                        cell11.PutValue(Check);
                        int i11 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomarePersoneli = _item.fldSh_Personali;
                            Cell Cell = sheet.Cells[alpha[index] + (i11 + 2)];
                            Cell.PutValue(ShomarePersoneli);
                            i11++;
                        }
                        index++;
                        break;
                    case "ReshteTahsili":
                        Check = "رشته تحصیلی";
                        Cell cell12 = sheet.Cells[alpha[index] + "1"];
                        cell12.PutValue(Check);
                        int i12 = 0;
                        foreach (var _item in Personal)
                        {
                            ReshteTahsili = _item.fldReshteTahsiliTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (i12 + 2)];
                            Cell.PutValue(ReshteTahsili);
                            i12++;
                        }
                        index++;
                        break;
                    case "StatusNezamVazife":
                        Check = "وضعیت نظام وظیفه";
                        Cell cell13 = sheet.Cells[alpha[index] + "1"];
                        cell13.PutValue(Check);
                        int i13 = 0;
                        foreach (var _item in Personal)
                        {
                            StatusNezamVazife = _item.fldNezamVazifeTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (i13 + 2)];
                            Cell.PutValue(StatusNezamVazife);
                            i13++;
                        }
                        index++;
                        break;
                    case "OrganizationalPosts":
                        Check = "پست مصوب";
                        Cell cell14 = sheet.Cells[alpha[index] + "1"];
                        cell14.PutValue(Check);
                        int i14 = 0;
                        foreach (var _item in Personal)
                        {
                            Post = _item.fldOrganPostName;
                            Cell Cell = sheet.Cells[alpha[index] + (i14 + 2)];
                            Cell.PutValue(Post);
                            i14++;
                        }
                        index++;
                        break;
                    case "RasteShoghli":
                        Check = "رسته شغلی";
                        Cell cell15 = sheet.Cells[alpha[index] + "1"];
                        cell15.PutValue(Check);
                        int i15 = 0;
                        foreach (var _item in Personal)
                        {
                            RasteShoghli = _item.fldRasteShoghli;
                            Cell Cell = sheet.Cells[alpha[index] + (i15 + 2)];
                            Cell.PutValue(RasteShoghli);
                            i15++;
                        }
                        index++;
                        break;
                    case "ReshteShoghli":
                        Check = "رشته شغلی";
                        Cell cell16 = sheet.Cells[alpha[index] + "1"];
                        cell16.PutValue(Check);
                        int i16 = 0;
                        foreach (var _item in Personal)
                        {
                            ReshteShoghli = _item.fldReshteShoghli;
                            Cell Cell = sheet.Cells[alpha[index] + (i16 + 2)];
                            Cell.PutValue(ReshteShoghli);
                            i16++;
                        }
                        index++;
                        break;
                    case "TarikhEstekhdam":
                        Check = "تاریخ استخدام";
                        Cell cell17 = sheet.Cells[alpha[index] + "1"];
                        cell17.PutValue(Check);
                        int i17 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhEstekhdam = _item.fldTarikhEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (i17 + 2)];
                            Cell.PutValue(TarikhEstekhdam);
                            i17++;
                        }
                        index++;
                        break;

                    case "Tabaghe":
                        Check = "طبقه";
                        Cell cell18 = sheet.Cells[alpha[index] + "1"];
                        cell18.PutValue(Check);
                        int i18 = 0;
                        foreach (var _item in Personal)
                        {
                            Tabaghe = _item.fldTabaghe;
                            Cell Cell = sheet.Cells[alpha[index] + (i18 + 2)];
                            Cell.PutValue(Tabaghe);
                            i18++;
                        }
                        index++;
                        break;
                    case "Meliyat":
                        Check = "ملیت";
                        Cell cell19 = sheet.Cells[alpha[index] + "1"];
                        cell19.PutValue(Check);
                        int i19 = 0;
                        foreach (var _item in Personal)
                        {
                            MeliyatChar = _item.fldMeliyatName;
                            Cell Cell = sheet.Cells[alpha[index] + (i19 + 2)];
                            Cell.PutValue(MeliyatChar);
                            i19++;
                        }
                        index++;
                        break;
                    case "ShMojavezEstekhdam":
                        Check = "شماره مجوز استخدام";
                        Cell cell20 = sheet.Cells[alpha[index] + "1"];
                        cell20.PutValue(Check);
                        int i20 = 0;
                        foreach (var _item in Personal)
                        {
                            ShMojavezEstekhdam = _item.fldSh_MojavezEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (i20 + 2)];
                            Cell.PutValue(ShMojavezEstekhdam);
                            i20++;
                        }
                        index++;
                        break;
                    case "TMojavezEstekhdam":
                        Check = "تاریخ مجوز استخدام";
                        Cell cell21 = sheet.Cells[alpha[index] + "1"];
                        cell21.PutValue(Check);
                        int i21 = 0;
                        foreach (var _item in Personal)
                        {
                            TMojavezEstekhdam = _item.fldTarikhMajavezEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (i21 + 2)];
                            Cell.PutValue(TMojavezEstekhdam);
                            i21++;
                        }
                        index++;
                        break;
                    case "StatusTitle":
                        Check = "وضعیت";
                        Cell cell23 = sheet.Cells[alpha[index] + "1"];
                        cell23.PutValue(Check);
                        int i23 = 0;
                        foreach (var _item in Personal)
                        {
                            StatusTitle = _item.fldTitleStatus;
                            Cell Cell = sheet.Cells[alpha[index] + (i23 + 2)];
                            Cell.PutValue(StatusTitle);
                            i23++;
                        }
                        index++;
                        break;
                    case "TarikhSodoor":
                        Check = "تاریخ صدور";
                        Cell cell24 = sheet.Cells[alpha[index] + "1"];
                        cell24.PutValue(Check);
                        int i24 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhSodoor = _item.fldTarikhSodoor;
                            Cell Cell = sheet.Cells[alpha[index] + (i24 + 2)];
                            Cell.PutValue(TarikhSodoor);
                            i24++;
                        }
                        index++;
                        break;
                    case "CityTavalod":
                        Check = "شهر محل تولد";
                        Cell cell25 = sheet.Cells[alpha[index] + "1"];
                        cell25.PutValue(Check);
                        int i25 = 0;
                        foreach (var _item in Personal)
                        {
                            CityTavalod = _item.fldNameMahalTavalod;
                            Cell Cell = sheet.Cells[alpha[index] + (i25 + 2)];
                            Cell.PutValue(CityTavalod);
                            i25++;
                        }
                        index++;
                        break;
                    case "MadrakTitle":
                        Check = "مقطع تحصیلی";
                        Cell cell26 = sheet.Cells[alpha[index] + "1"];
                        cell26.PutValue(Check);
                        int i26 = 0;
                        foreach (var _item in Personal)
                        {
                            MadrakTitle = _item.fldMadrakTahsiliTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (i26 + 2)];
                            Cell.PutValue(MadrakTitle);
                            i26++;
                        }
                        index++;
                        break;
                    case "SharhEsargari":
                        Check = "شرح ایثارگری";
                        Cell cell27 = sheet.Cells[alpha[index] + "1"];
                        cell27.PutValue(Check);
                        int i27 = 0;
                        foreach (var _item in Personal)
                        {
                            SharhEsargari = _item.fldSharhEsargari;
                            Cell Cell = sheet.Cells[alpha[index] + (i27 + 2)];
                            Cell.PutValue(SharhEsargari);
                            i27++;
                        }
                        index++;
                        break;
                    case "OrganizationalPostsEjraee":
                        Check = "پست اجرائی";
                        Cell cell28 = sheet.Cells[alpha[index] + "1"];
                        cell28.PutValue(Check);
                        int i28 = 0;
                        foreach (var _item in Personal)
                        {
                            PostEjraee = _item.fldNameOrganEjraee;
                            Cell Cell = sheet.Cells[alpha[index] + (i28 + 2)];
                            Cell.PutValue(PostEjraee);
                            i28++;
                        }
                        index++;
                        break;
                    case "MahalKhedmat":
                        Check = "محل خدمت";
                        Cell cell29 = sheet.Cells[alpha[index] + "1"];
                        cell29.PutValue(Check);
                        int i29 = 0;
                        foreach (var _item in Personal)
                        {
                            MahalKhedmat = _item.fldMahalKhedmat;
                            Cell Cell = sheet.Cells[alpha[index] + (i29 + 2)];
                            Cell.PutValue(MahalKhedmat);
                            i29++;
                        }
                        index++;
                        break;
                    case "SanavatKhedmat":
                        Check = "سنوات خدمت";
                        Cell cell30 = sheet.Cells[alpha[index] + "1"];
                        cell30.PutValue(Check);
                        int i30 = 0;
                        foreach (var _item in Personal)
                        {
                            SanavatKhedmat = _item.fldSanavtKhedmat;
                            Cell Cell = sheet.Cells[alpha[index] + (i30 + 2)];
                            Cell.PutValue(SanavatKhedmat);
                            i30++;
                        }
                        index++;
                        break;
                    case "Tel":
                        Check = "تلفن";
                        Cell cell31 = sheet.Cells[alpha[index] + "1"];
                        cell31.PutValue(Check);
                        int i31 = 0;
                        foreach (var _item in Personal)
                        {
                            Tel = _item.fldTel;
                            Cell Cell = sheet.Cells[alpha[index] + (i31 + 2)];
                            Cell.PutValue(Tel);
                            i31++;
                        }
                        index++;
                        break;
                    case "Mobile":
                        Check = "موبایل";
                        Cell cell32 = sheet.Cells[alpha[index] + "1"];
                        cell32.PutValue(Check);
                        int i32 = 0;
                        foreach (var _item in Personal)
                        {
                            Mobile = _item.fldMobile;
                            Cell Cell = sheet.Cells[alpha[index] + (i32 + 2)];
                            Cell.PutValue(Mobile);
                            i32++;
                        }
                        index++;
                        break;
                    /////////////////
                    case "TypeBime":
                        Check = "نوع بیمه";
                        Cell pcell1 = sheet.Cells[alpha[index] + "1"];
                        pcell1.PutValue(Check);
                        int j1 = 0;
                        foreach (var _item in Personal)
                        {
                            TypeBime = _item.fldTitleTypeBime;
                            Cell Cell = sheet.Cells[alpha[index] + (j1 + 2)];
                            Cell.PutValue(TypeBime);
                            j1++;
                        }
                        index++;
                        break;
                    case "ShomareBime":
                        Check = "شماره بیمه";
                        Cell pcell2 = sheet.Cells[alpha[index] + "1"];
                        pcell2.PutValue(Check);
                        int j2 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomareBime = _item.fldShomareBime;
                            Cell Cell = sheet.Cells[alpha[index] + (j2 + 2)];
                            Cell.PutValue(ShomareBime);
                            j2++;
                        }
                        index++;
                        break;
                    case "BimeOmr":
                        Check = "بیمه عمر";
                        Cell pcell3 = sheet.Cells[alpha[index] + "1"];
                        pcell3.PutValue(Check);
                        int j3 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeOmr = _item.fldBimeOmrName;
                            Cell Cell = sheet.Cells[alpha[index] + (j3 + 2)];
                            Cell.PutValue(BimeOmr);
                            j3++;
                        }
                        index++;
                        break;
                    case "BimeTakmili":
                        Check = "بیمه تکمیلی";
                        Cell pcell4 = sheet.Cells[alpha[index] + "1"];
                        pcell4.PutValue(Check);
                        int j4 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeTakmili = _item.fldBimeTakmiliName;
                            Cell Cell = sheet.Cells[alpha[index] + (j4 + 2)];
                            Cell.PutValue(BimeTakmili);
                            j4++;
                        }
                        index++;
                        break;
                    case "MashagheleSakhtVaZianAvar":
                        Check = "مشاغل سخت و زیان آور";
                        Cell pcell5 = sheet.Cells[alpha[index] + "1"];
                        pcell5.PutValue(Check);
                        int j5 = 0;
                        foreach (var _item in Personal)
                        {
                            MashagheleSakhtVaZianAvar = _item.fldMashagheleSakhtVaZianAvarName;
                            Cell Cell = sheet.Cells[alpha[index] + (j5 + 2)];
                            Cell.PutValue(MashagheleSakhtVaZianAvar);
                            j5++;
                        }
                        index++;
                        break;
                    case "CostCenter":
                        Check = "مرکز هزینه";
                        Cell pcell6 = sheet.Cells[alpha[index] + "1"];
                        pcell6.PutValue(Check);
                        int j6 = 0;
                        foreach (var _item in Personal)
                        {
                            CostCenter = _item.fldTitleCostCenter;
                            Cell Cell = sheet.Cells[alpha[index] + (j6 + 2)];
                            Cell.PutValue(CostCenter);
                            j6++;
                        }
                        index++;
                        break;
                    case "MazadCSal":
                        Check = "مازاد 30 سال";
                        Cell pcell7 = sheet.Cells[alpha[index] + "1"];
                        pcell7.PutValue(Check);
                        int j7 = 0;
                        foreach (var _item in Personal)
                        {
                            MazadCSal = _item.fldMazadCSalName;
                            Cell Cell = sheet.Cells[alpha[index] + (j7 + 2)];
                            Cell.PutValue(MazadCSal);
                            j7++;
                        }
                        index++;
                        break;
                    case "PasAndaz":
                        Check = "پس انداز";
                        Cell pcell8 = sheet.Cells[alpha[index] + "1"];
                        pcell8.PutValue(Check);
                        int j8 = 0;
                        foreach (var _item in Personal)
                        {
                            PasAndaz = _item.fldPasAndazName;
                            Cell Cell = sheet.Cells[alpha[index] + (j8 + 2)];
                            Cell.PutValue(PasAndaz);
                            j8++;
                        }
                        index++;
                        break;
                    case "MasuliyatPayanKhedmat":
                        Check = "سنوات پایان خدمت";
                        Cell pcell9 = sheet.Cells[alpha[index] + "1"];
                        pcell9.PutValue(Check);
                        int j9 = 0;
                        foreach (var _item in Personal)
                        {
                            MasuliyatPayanKhedmat = _item.fldSanavatPayanKhedmatName;
                            Cell Cell = sheet.Cells[alpha[index] + (j9 + 2)];
                            Cell.PutValue(MasuliyatPayanKhedmat);
                            j9++;
                        }
                        index++;
                        break;
                    case "JobeCode":
                        Check = "کد شغلی بیمه";
                        Cell pcell10 = sheet.Cells[alpha[index] + "1"];
                        pcell10.PutValue(Check);
                        int j10 = 0;
                        foreach (var _item in Personal)
                        {
                            JobeCode = _item.fldJobDesc;
                            Cell Cell = sheet.Cells[alpha[index] + (j10 + 2)];
                            Cell.PutValue(JobeCode);
                            j10++;
                        }
                        index++;
                        break;
                    case "InsuranceWorkShop":
                        Check = "کارگاه بیمه";
                        Cell pcell11 = sheet.Cells[alpha[index] + "1"];
                        pcell11.PutValue(Check);
                        int j11 = 0;
                        foreach (var _item in Personal)
                        {
                            InsuranceWorkShop = _item.fldWorkShopName;
                            Cell Cell = sheet.Cells[alpha[index] + (j11 + 2)];
                            Cell.PutValue(InsuranceWorkShop);
                            j11++;
                        }
                        index++;
                        break;
                    case "HamsarKarmand":
                        Check = "همسر کارمند";
                        Cell pcell12 = sheet.Cells[alpha[index] + "1"];
                        pcell12.PutValue(Check);
                        int j12 = 0;
                        foreach (var _item in Personal)
                        {
                            HamsarKarmand = _item.fldHamsarKarmandName;
                            Cell Cell = sheet.Cells[alpha[index] + (j12 + 2)];
                            Cell.PutValue(HamsarKarmand);
                            j12++;
                        }
                        index++;
                        break;
                    /////////////////////////
                    case "TarikhEjra":
                        Check = "تاریخ اجرا";
                        Cell hcell1 = sheet.Cells[alpha[index] + "1"];
                        hcell1.PutValue(Check);
                        int k1 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhEjra = _item.fldTarikhEjra;
                            Cell Cell = sheet.Cells[alpha[index] + (k1 + 2)];
                            Cell.PutValue(TarikhEjra);
                            k1++;
                        }
                        index++;
                        break;
                    case "TarikhSodoorHokm":
                        Check = "تاریخ صدور";
                        Cell hcell2 = sheet.Cells[alpha[index] + "1"];
                        hcell2.PutValue(Check);
                        int k2 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhSodoorHokm = _item.fldTarikhSodoorHokm;
                            Cell Cell = sheet.Cells[alpha[index] + (k2 + 2)];
                            Cell.PutValue(TarikhSodoorHokm);
                            k2++;
                        }
                        index++;
                        break;
                    case "TarikhEtmam":
                        Check = "تاریخ اتمام";
                        Cell hcell3 = sheet.Cells[alpha[index] + "1"];
                        hcell3.PutValue(Check);
                        int k3 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhEtmam = _item.fldTarikhEtmam;
                            Cell Cell = sheet.Cells[alpha[index] + (k3 + 2)];
                            Cell.PutValue(TarikhEtmam);
                            k3++;
                        }
                        index++;
                        break;
                    case "StatusTaahol":
                        Check = "وضعیت تاهل";
                        Cell hcell4 = sheet.Cells[alpha[index] + "1"];
                        hcell4.PutValue(Check);
                        int k4 = 0;
                        foreach (var _item in Personal)
                        {
                            StatusTaahol = _item.StatusTaaholTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (k4 + 2)];
                            Cell.PutValue(StatusTaahol);
                            k4++;
                        }
                        index++;
                        break;
                    case "ShomarePostSazmani":
                        Check = "شماره پست سازمانی";
                        Cell hcell5 = sheet.Cells[alpha[index] + "1"];
                        hcell5.PutValue(Check);
                        int k5 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomarePostSazmani = _item.fldShomarePostSazmani;
                            Cell Cell = sheet.Cells[alpha[index] + (k5 + 2)];
                            Cell.PutValue(ShomarePostSazmani);
                            k5++;
                        }
                        index++;
                        break;
                    case "MoreGroup":
                        Check = "گروه بالاتر";
                        Cell hcell6 = sheet.Cells[alpha[index] + "1"];
                        hcell6.PutValue(Check);
                        int k6 = 0;
                        foreach (var _item in Personal)
                        {
                            MoreGroup = _item.fldMoreGroup;
                            Cell Cell = sheet.Cells[alpha[index] + (k6 + 2)];
                            Cell.PutValue(MoreGroup);
                            k6++;
                        }
                        index++;
                        break;
                    case "Group":
                        Check = "گروه";
                        Cell hcell7 = sheet.Cells[alpha[index] + "1"];
                        hcell7.PutValue(Check);
                        int k7 = 0;
                        foreach (var _item in Personal)
                        {
                            Group = _item.fldGroup.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (k7 + 2)];
                            Cell.PutValue(Group);
                            k7++;
                        }
                        index++;
                        break;
                    case "TedadFarzand":
                        Check = "تعداد فرزند";
                        Cell hcell8 = sheet.Cells[alpha[index] + "1"];
                        hcell8.PutValue(Check);
                        int k8 = 0;
                        foreach (var _item in Personal)
                        {
                            TedadFarzand = _item.fldTedadFarzand.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (k8 + 2)];
                            Cell.PutValue(TedadFarzand);
                            k8++;
                        }
                        index++;
                        break;
                    case "TedadAfradTahteTakafol":
                        Check = "تعداد افراد تحت تکفل";
                        Cell hcell9 = sheet.Cells[alpha[index] + "1"];
                        hcell9.PutValue(Check);
                        int k9 = 0;
                        foreach (var _item in Personal)
                        {
                            TedadAfradTahteTakafol = _item.fldTedadAfradTahteTakafol.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (k9 + 2)];
                            Cell.PutValue(TedadAfradTahteTakafol);
                            k9++;
                        }
                        index++;
                        break;
                    case "CodeShoghl":
                        Check = "کد شغل";
                        Cell hcell10 = sheet.Cells[alpha[index] + "1"];
                        hcell10.PutValue(Check);
                        int k10 = 0;
                        foreach (var _item in Personal)
                        {
                            CodeShoghl = _item.fldCodeShoghl;
                            Cell Cell = sheet.Cells[alpha[index] + (k10 + 2)];
                            Cell.PutValue(CodeShoghl);
                            k10++;
                        }
                        index++;
                        break;
                    case "ShomareHokm":
                        Check = "شماره حکم";
                        Cell hcell11 = sheet.Cells[alpha[index] + "1"];
                        hcell11.PutValue(Check);
                        int k11 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomareHokm = _item.fldShomareHokm;
                            Cell Cell = sheet.Cells[alpha[index] + (k11 + 2)];
                            Cell.PutValue(ShomareHokm);
                            k11++;
                        }
                        index++;
                        break;
                    case "NoeEstekhdamTitle":
                        Check = "نوع استخدام";
                        Cell hcell12 = sheet.Cells[alpha[index] + "1"];
                        hcell12.PutValue(Check);
                        int k12 = 0;
                        foreach (var _item in Personal)
                        {
                            NoeEstekhdamTitle = _item.TitleNoeEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (k12 + 2)];
                            Cell.PutValue(NoeEstekhdamTitle);
                            k12++;
                        }
                        index++;
                        break;
                    case "JameHokm":
                        Check = "جمع حکم";
                        Cell hcell13 = sheet.Cells[alpha[index] + "1"];
                        hcell13.PutValue(Check);
                        int k13 = 0;
                        foreach (var _item in Personal)
                        {
                            JameHokm = _item.fldJameHokm;
                            Cell Cell = sheet.Cells[alpha[index] + (k13 + 2)];
                            Cell.PutValue(JameHokm);
                            k13++;
                        }
                        index++;
                        break;
                    case "Ertegha":
                        Check = "تاریخ ارتقا گروه";
                        Cell hcell14 = sheet.Cells[alpha[index] + "1"];
                        hcell14.PutValue(Check);
                        int k14 = 0;
                        foreach (var _item in Personal)
                        {
                            Ertegha = _item.Ertegha;
                            Cell Cell = sheet.Cells[alpha[index] + (k14 + 2)];
                            Cell.PutValue(Ertegha);
                            k14++;
                        }
                        index++;
                        break;
                    ////////////////////////////////////
                    case "h-paye":
                        Check = "حقوق مبنا";
                        Cell icell1 = sheet.Cells[alpha[index] + "1"];
                        icell1.PutValue(Check);
                        int m1 = 0;
                        foreach (var _item in Personal)
                        {
                            hpaye = _item.hpaye.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m1 + 2)];
                            Cell.PutValue(hpaye);
                            m1++;
                        }
                        index++;
                        break;
                    case "sanavat":
                        Check = "سنوات";
                        Cell icell2 = sheet.Cells[alpha[index] + "1"];
                        icell2.PutValue(Check);
                        int m2 = 0;
                        foreach (var _item in Personal)
                        {
                            sanavat = _item.sanavat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m2 + 2)];
                            Cell.PutValue(sanavat);
                            m2++;
                        }
                        index++;
                        break;
                    case "paye":
                        Check = "پایه";
                        Cell icell3 = sheet.Cells[alpha[index] + "1"];
                        icell3.PutValue(Check);
                        int m3 = 0;
                        foreach (var _item in Personal)
                        {
                            paye = _item.paye.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m3 + 2)];
                            Cell.PutValue(paye);
                            m3++;
                        }
                        index++;
                        break;
                    case "sanavat-basiji":
                        Check = "سنوات بسیجی";
                        Cell icell4 = sheet.Cells[alpha[index] + "1"];
                        icell4.PutValue(Check);
                        int m4 = 0;
                        foreach (var _item in Personal)
                        {
                            sanavatbasiji = _item.sanavatbasiji.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m4 + 2)];
                            Cell.PutValue(sanavatbasiji);
                            m4++;
                        }
                        index++;
                        break;
                    case "sanavat-isar":
                        Check = "سنوات ایثارگری";
                        Cell icell5 = sheet.Cells[alpha[index] + "1"];
                        icell5.PutValue(Check);
                        int m5 = 0;
                        foreach (var _item in Personal)
                        {
                            sanavatisar = _item.sanavatisar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m5 + 2)];
                            Cell.PutValue(sanavatisar);
                            m5++;
                        }
                        index++;
                        break;
                    case "foghshoghl":
                        Check = "فوق العاده شغل";
                        Cell icell6 = sheet.Cells[alpha[index] + "1"];
                        icell6.PutValue(Check);
                        int m6 = 0;
                        foreach (var _item in Personal)
                        {
                            foghshoghl = _item.foghshoghl.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m6 + 2)];
                            Cell.PutValue(foghshoghl);
                            m6++;
                        }
                        index++;
                        break;
                    case "takhasosi":
                        Check = "تحقیقی و تخصصی";
                        Cell icell7 = sheet.Cells[alpha[index] + "1"];
                        icell7.PutValue(Check);
                        int m7 = 0;
                        foreach (var _item in Personal)
                        {
                            takhasosi = _item.takhasosi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m7 + 2)];
                            Cell.PutValue(takhasosi);
                            m7++;
                        }
                        index++;
                        break;
                    case "made26":
                        Check = "فوق العاده ماده26";
                        Cell icell8 = sheet.Cells[alpha[index] + "1"];
                        icell8.PutValue(Check);
                        int m8 = 0;
                        foreach (var _item in Personal)
                        {
                            made26 = _item.made26.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m8 + 2)];
                            Cell.PutValue(made26);
                            m8++;
                        }
                        index++;
                        break;
                    case "modiryati":
                        Check = "مدیریتی و سرپرستی";
                        Cell icell9 = sheet.Cells[alpha[index] + "1"];
                        icell9.PutValue(Check);
                        int m9 = 0;
                        foreach (var _item in Personal)
                        {
                            modiryati = _item.modiryati.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m9 + 2)];
                            Cell.PutValue(modiryati);
                            m9++;
                        }
                        index++;
                        break;
                    case "barjastegi":
                        Check = "برجستگی";
                        Cell icell10 = sheet.Cells[alpha[index] + "1"];
                        icell10.PutValue(Check);
                        int m10 = 0;
                        foreach (var _item in Personal)
                        {
                            barjastegi = _item.barjastegi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m10 + 2)];
                            Cell.PutValue(barjastegi);
                            m10++;
                        }
                        index++;
                        break;
                    case "tatbigh":
                        Check = "تفاوت تطبیق";
                        Cell icell11 = sheet.Cells[alpha[index] + "1"];
                        icell11.PutValue(Check);
                        int m11 = 0;
                        foreach (var _item in Personal)
                        {
                            tatbigh = _item.tatbigh.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m11 + 2)];
                            Cell.PutValue(tatbigh);
                            m11++;
                        }
                        index++;
                        break;
                    case "fogh-isar":
                        Check = "فوق العاده ایثارگری";
                        Cell icell12 = sheet.Cells[alpha[index] + "1"];
                        icell12.PutValue(Check);
                        int m12 = 0;
                        foreach (var _item in Personal)
                        {
                            foghisar = _item.foghisar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m12 + 2)];
                            Cell.PutValue(foghisar);
                            m12++;
                        }
                        index++;
                        break;
                    case "abohava":
                        Check = "بدی آب و هوا";
                        Cell icell13 = sheet.Cells[alpha[index] + "1"];
                        icell13.PutValue(Check);
                        int m13 = 0;
                        foreach (var _item in Personal)
                        {
                            abohava = _item.abohava.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m13 + 2)];
                            Cell.PutValue(abohava);
                            m13++;
                        }
                        index++;
                        break;
                    case "tashilat":
                        Check = "تسهیلات زندگی";
                        Cell icell14 = sheet.Cells[alpha[index] + "1"];
                        icell14.PutValue(Check);
                        int m14 = 0;
                        foreach (var _item in Personal)
                        {
                            tashilat = _item.tashilat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m14 + 2)];
                            Cell.PutValue(tashilat);
                            m14++;
                        }
                        index++;
                        break;
                    case "sakhtikar":
                        Check = "سختی کار";
                        Cell icell15 = sheet.Cells[alpha[index] + "1"];
                        icell15.PutValue(Check);
                        int m15 = 0;
                        foreach (var _item in Personal)
                        {
                            sakhtikar = _item.sakhtikar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m15 + 2)];
                            Cell.PutValue(sakhtikar);
                            m15++;
                        }
                        index++;
                        break;
                    case "tadil":
                        Check = "فوق العاده تعدیل";
                        Cell icell16 = sheet.Cells[alpha[index] + "1"];
                        icell16.PutValue(Check);
                        int m16 = 0;
                        foreach (var _item in Personal)
                        {
                            tadil = _item.tadil.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m16 + 2)];
                            Cell.PutValue(tadil);
                            m16++;
                        }
                        index++;
                        break;
                    case "riali":
                        Check = "مزایای ریالی گروه تشویقی";
                        Cell icell17 = sheet.Cells[alpha[index] + "1"];
                        icell17.PutValue(Check);
                        int m17 = 0;
                        foreach (var _item in Personal)
                        {
                            riali = _item.riali.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m17 + 2)];
                            Cell.PutValue(riali);
                            m17++;
                        }
                        index++;
                        break;
                    case "jazb9":
                        Check = "حق جذب بند9";
                        Cell icell18 = sheet.Cells[alpha[index] + "1"];
                        icell18.PutValue(Check);
                        int m18 = 0;
                        foreach (var _item in Personal)
                        {
                            jazb9 = _item.jazb9.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m18 + 2)];
                            Cell.PutValue(jazb9);
                            m18++;
                        }
                        index++;
                        break;
                    case "jazb":
                        Check = "فوق العاده جذب";
                        Cell icell19 = sheet.Cells[alpha[index] + "1"];
                        icell19.PutValue(Check);
                        int m19 = 0;
                        foreach (var _item in Personal)
                        {
                            jazb = _item.jazb.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m19 + 2)];
                            Cell.PutValue(jazb);
                            m19++;
                        }
                        index++;
                        break;
                    case "makhsos":
                        Check = "فوق العاده مخصوص";
                        Cell icell20 = sheet.Cells[alpha[index] + "1"];
                        icell20.PutValue(Check);
                        int m20 = 0;
                        foreach (var _item in Personal)
                        {
                            makhsos = _item.makhsos.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m20 + 2)];
                            Cell.PutValue(makhsos);
                            m20++;
                        }
                        index++;
                        break;
                    case "vije":
                        Check = "فوق العاده ویژه";
                        Cell icell21 = sheet.Cells[alpha[index] + "1"];
                        icell21.PutValue(Check);
                        int m21 = 0;
                        foreach (var _item in Personal)
                        {
                            vije = _item.vije.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m21 + 2)];
                            Cell.PutValue(vije);
                            m21++;
                        }
                        index++;
                        break;
                    case "olad":
                        Check = "حق اولاد";
                        Cell icell22 = sheet.Cells[alpha[index] + "1"];
                        icell22.PutValue(Check);
                        int m22 = 0;
                        foreach (var _item in Personal)
                        {
                            olad = _item.olad.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m22 + 2)];
                            Cell.PutValue(olad);
                            m22++;
                        }
                        index++;
                        break;
                    case "ayelemandi":
                        Check = "حق عائله مندی";
                        Cell icell23 = sheet.Cells[alpha[index] + "1"];
                        icell23.PutValue(Check);
                        int m23 = 0;
                        foreach (var _item in Personal)
                        {
                            ayelemandi = _item.ayelemandi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m23 + 2)];
                            Cell.PutValue(ayelemandi);
                            m23++;
                        }
                        index++;
                        break;
                    case "kharobar":
                        Check = "خوار و بار";
                        Cell icell24 = sheet.Cells[alpha[index] + "1"];
                        icell24.PutValue(Check);
                        int m24 = 0;
                        foreach (var _item in Personal)
                        {
                            kharobar = _item.kharobar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m24 + 2)];
                            Cell.PutValue(kharobar);
                            m24++;
                        }
                        index++;
                        break;
                    case "maskan":
                        Check = "حق مسکن";
                        Cell icell25 = sheet.Cells[alpha[index] + "1"];
                        icell25.PutValue(Check);
                        int m25 = 0;
                        foreach (var _item in Personal)
                        {
                            maskan = _item.maskan.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m25 + 2)];
                            Cell.PutValue(maskan);
                            m25++;
                        }
                        index++;
                        break;
                    case "nobatkari":
                        Check = "نوبت کاری";
                        Cell icell26 = sheet.Cells[alpha[index] + "1"];
                        icell26.PutValue(Check);
                        int m26 = 0;
                        foreach (var _item in Personal)
                        {
                            nobatkari = _item.nobatkari.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m26 + 2)];
                            Cell.PutValue(nobatkari);
                            m26++;
                        }
                        index++;
                        break;
                    case "bon":
                        Check = "بن ماهیانه";
                        Cell icell27 = sheet.Cells[alpha[index] + "1"];
                        icell27.PutValue(Check);
                        int m27 = 0;
                        foreach (var _item in Personal)
                        {
                            bon = _item.bon.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m27 + 2)];
                            Cell.PutValue(bon);
                            m27++;
                        }
                        index++;
                        break;
                    case "jazb-tabsare":
                        Check = "حق جذب تبصره 7 ماده یک";
                        Cell icell28 = sheet.Cells[alpha[index] + "1"];
                        icell28.PutValue(Check);
                        int m28 = 0;
                        foreach (var _item in Personal)
                        {
                            jazbtabsare = _item.jazbtabsare.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m28 + 2)];
                            Cell.PutValue(jazbtabsare);
                            m28++;
                        }
                        index++;
                        break;
                    case "talash":
                        Check = "فوق العاده تلاش";
                        Cell icell29 = sheet.Cells[alpha[index] + "1"];
                        icell29.PutValue(Check);
                        int m29 = 0;
                        foreach (var _item in Personal)
                        {
                            talash = _item.talash.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m29 + 2)];
                            Cell.PutValue(talash);
                            m29++;
                        }
                        index++;
                        break;
                    case "jebhe":
                        Check = "حق جبهه";
                        Cell icell30 = sheet.Cells[alpha[index] + "1"];
                        icell30.PutValue(Check);
                        int m30 = 0;
                        foreach (var _item in Personal)
                        {
                            jebhe = _item.jebhe.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m30 + 2)];
                            Cell.PutValue(jebhe);
                            m30++;
                        }
                        index++;
                        break;
                    case "janbazi":
                        Check = "حق جانبازی";
                        Cell icell31 = sheet.Cells[alpha[index] + "1"];
                        icell31.PutValue(Check);
                        int m31 = 0;
                        foreach (var _item in Personal)
                        {
                            janbazi = _item.janbazi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m31 + 2)];
                            Cell.PutValue(janbazi);
                            m31++;
                        }
                        index++;
                        break;
                    case "sayer":
                        Check = "سایر مزایا";
                        Cell icell32 = sheet.Cells[alpha[index] + "1"];
                        icell32.PutValue(Check);
                        int m32 = 0;
                        foreach (var _item in Personal)
                        {
                            sayer = _item.sayer.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m32 + 2)];
                            Cell.PutValue(sayer);
                            m32++;
                        }
                        index++;
                        break;
                    case "ezafekar":
                        Check = "اضافه کاری";
                        Cell icell33 = sheet.Cells[alpha[index] + "1"];
                        icell33.PutValue(Check);
                        int m33 = 0;
                        foreach (var _item in Personal)
                        {
                            ezafekar = _item.ezafekar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m33 + 2)];
                            Cell.PutValue(ezafekar);
                            m33++;
                        }
                        index++;
                        break;
                    case "mamoriat":
                        Check = "ماموریت";
                        Cell icell34 = sheet.Cells[alpha[index] + "1"];
                        icell34.PutValue(Check);
                        int m34 = 0;
                        foreach (var _item in Personal)
                        {
                            mamoriat = _item.mamoriat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m34 + 2)];
                            Cell.PutValue(mamoriat);
                            m34++;
                        }
                        index++;
                        break;
                    case "tatilkari":
                        Check = "تعطیل کاری";
                        Cell icell35 = sheet.Cells[alpha[index] + "1"];
                        icell35.PutValue(Check);
                        int m35 = 0;
                        foreach (var _item in Personal)
                        {
                            tatilkari = _item.tatilkari.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m35 + 2)];
                            Cell.PutValue(tatilkari);
                            m35++;
                        }
                        index++;
                        break;
                    case "s_payankhedmat":
                        Check = "سنوات پایان خدمت";
                        Cell icell36 = sheet.Cells[alpha[index] + "1"];
                        icell36.PutValue(Check);
                        int m36 = 0;
                        foreach (var _item in Personal)
                        {
                            s_payankhedmat = _item.s_payankhedmat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m36 + 2)];
                            Cell.PutValue(s_payankhedmat);
                            m36++;
                        }
                        index++;
                        break;
                    case "ghazai":
                        Check = "حق جذب قضایی";
                        Cell icell37 = sheet.Cells[alpha[index] + "1"];
                        icell37.PutValue(Check);
                        int m37 = 0;
                        foreach (var _item in Personal)
                        {
                            ghazai = _item.ghazai.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m37 + 2)];
                            Cell.PutValue(ghazai);
                            m37++;
                        }
                        index++;
                        break;
                    case "ashoora":
                        Check = "گردان عاشورا";
                        Cell icell38 = sheet.Cells[alpha[index] + "1"];
                        icell38.PutValue(Check);
                        int m38 = 0;
                        foreach (var _item in Personal)
                        {
                            ashoora = _item.ashoora.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m38 + 2)];
                            Cell.PutValue(ashoora);
                            m38++;
                        }
                        index++;
                        break;
                    case "zaribtadil":
                        Check = "ضریب تعدیل";
                        Cell icell39 = sheet.Cells[alpha[index] + "1"];
                        icell39.PutValue(Check);
                        int m39 = 0;
                        foreach (var _item in Personal)
                        {
                            zaribtadil = _item.zaribtadil.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m39 + 2)];
                            Cell.PutValue(zaribtadil);
                            m39++;
                        }
                        index++;
                        break;
                    case "jazbTakhasosi":
                        Check = "جذب مشاغل تخصصی";
                        Cell icell40 = sheet.Cells[alpha[index] + "1"];
                        icell40.PutValue(Check);
                        int m40 = 0;
                        foreach (var _item in Personal)
                        {
                            jazbTakhasosi = _item.jazbTakhasosi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m40 + 2)];
                            Cell.PutValue(jazbTakhasosi);
                            m40++;
                        }
                        index++;
                        break;
                    case "jazbtadil":
                        Check = "جذب تعدیل";
                        Cell icell41 = sheet.Cells[alpha[index] + "1"];
                        icell41.PutValue(Check);
                        int m41 = 0;
                        foreach (var _item in Personal)
                        {
                            jazbtadil = _item.jazbtadil.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m41 + 2)];
                            Cell.PutValue(jazbtadil);
                            m41++;
                        }
                        index++;
                        break;
                    case "hadaghaltadil":
                        Check = "تفاوت ناشی از ضریب تعدیل";
                        Cell icell42 = sheet.Cells[alpha[index] + "1"];
                        icell42.PutValue(Check);
                        int m42 = 0;
                        foreach (var _item in Personal)
                        {
                            hadaghaltadil = _item.hadaghaltadil.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m42 + 2)];
                            Cell.PutValue(hadaghaltadil);
                            m42++;
                        }
                        index++;
                        break;
                    case "khas":
                        Check = "خاص";
                        Cell icell421 = sheet.Cells[alpha[index] + "1"];
                        icell421.PutValue(Check);
                        int m421 = 0;
                        foreach (var _item in Personal)
                        {
                            var khas = _item.khas.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m421 + 2)];
                            Cell.PutValue(khas);
                            m421++;
                        }
                        index++;
                        break;
                    case "karane":
                        Check = "کارانه";
                        Cell icell422 = sheet.Cells[alpha[index] + "1"];
                        icell422.PutValue(Check);
                        int m422 = 0;
                        foreach (var _item in Personal)
                        {
                            var karane = _item.karane.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m422 + 2)];
                            Cell.PutValue(karane);
                            m422++;
                        }
                        index++;
                        break;
                    case "refahi":
                        Check = "رفاهی";
                        Cell icell423 = sheet.Cells[alpha[index] + "1"];
                        icell423.PutValue(Check);
                        int m423 = 0;
                        foreach (var _item in Personal)
                        {
                            var refahi = _item.refahi.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (m423 + 2)];
                            Cell.PutValue(refahi);
                            m423++;
                        }
                        index++;
                        break;
                    ///////////////////////////////////
                    case "Karkard":
                        Check = "کارکرد";
                        Cell dcell1 = sheet.Cells[alpha[index] + "1"];
                        dcell1.PutValue(Check);
                        int h1 = 0;
                        foreach (var _item in Personal)
                        {
                            Karkard = _item.fldKarkard.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h1 + 2)];
                            Cell.PutValue(Karkard);
                            h1++;
                        }
                        index++;
                        break;
                    case "Gheybat":
                        Check = "غیبت";
                        Cell dcell2 = sheet.Cells[alpha[index] + "1"];
                        dcell2.PutValue(Check);
                        int h2 = 0;
                        foreach (var _item in Personal)
                        {
                            Gheybat = _item.fldGheybat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h2 + 2)];
                            Cell.PutValue(Gheybat);
                            h2++;
                        }
                        index++;
                        break;
                    case "TedadEzafeKar":
                        Check = "تعداد اضافه کاری";
                        Cell dcell3 = sheet.Cells[alpha[index] + "1"];
                        dcell3.PutValue(Check);
                        int h3 = 0;
                        foreach (var _item in Personal)
                        {
                            TedadEzafeKar = _item.fldTedadEzafeKar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h3 + 2)];
                            Cell.PutValue(TedadEzafeKar);
                            h3++;
                        }
                        index++;
                        break;
                    case "BimeOmrKarFarma":
                        Check = "مبلغ بیمه عمر سهم کارفرما";
                        Cell dcell4 = sheet.Cells[alpha[index] + "1"];
                        dcell4.PutValue(Check);
                        int h4 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeOmrKarFarma = _item.fldBimeOmrKarFarma.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h4 + 2)];
                            Cell.PutValue(BimeOmrKarFarma);
                            h4++;
                        }
                        index++;
                        break;
                    case "Mamooriyat":
                        Check = "تعداد ماموریت";
                        Cell dcell5 = sheet.Cells[alpha[index] + "1"];
                        dcell5.PutValue(Check);
                        int h5 = 0;
                        foreach (var _item in Personal)
                        {
                            Mamooriyat = _item.Mamooriyat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h5 + 2)];
                            Cell.PutValue(Mamooriyat);
                            h5++;
                        }
                        index++;
                        break;
                    case "TedadTatilKar":
                        Check = "تعداد تعطیل کاری";
                        Cell dcell6 = sheet.Cells[alpha[index] + "1"];
                        dcell6.PutValue(Check);
                        int h6 = 0;
                        foreach (var _item in Personal)
                        {
                            TedadTatilKar = _item.fldTedadTatilKar.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h6 + 2)];
                            Cell.PutValue(TedadTatilKar);
                            h6++;
                        }
                        index++;
                        break;
                    case "BimeOmrM":
                        Check = "مبلغ بیمه عمر";
                        Cell dcell7 = sheet.Cells[alpha[index] + "1"];
                        dcell7.PutValue(Check);
                        int h7 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeOmrM = _item.fldBimeOmrM.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h7 + 2)];
                            Cell.PutValue(BimeOmrM);
                            h7++;
                        }
                        index++;
                        break;
                    case "BimeTakmily":
                        Check = "مبلغ بیمه تکمیلی";
                        Cell dcell8 = sheet.Cells[alpha[index] + "1"];
                        dcell8.PutValue(Check);
                        int h8 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeTakmily = _item.fldBimeTakmily.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h8 + 2)];
                            Cell.PutValue(BimeTakmily);
                            h8++;
                        }
                        index++;
                        break;
                    case "BimeTakmilyKarFarma":
                        Check = "مبلغ تکمیلی سهم کارفرما";
                        Cell dcell9 = sheet.Cells[alpha[index] + "1"];
                        dcell9.PutValue(Check);
                        int h9 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeTakmilyKarFarma = _item.fldBimeTakmilyKarFarma.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h9 + 2)];
                            Cell.PutValue(BimeTakmilyKarFarma);
                            h9++;
                        }
                        index++;
                        break;
                    case "HaghDarmanKarfFarma ":
                        Check = "مبلغ درمان سهم کارفرما";
                        Cell dcell10 = sheet.Cells[alpha[index] + "1"];
                        dcell10.PutValue(Check);
                        int h10 = 0;
                        foreach (var _item in Personal)
                        {
                            HaghDarmanKarfFarma = _item.fldHaghDarmanKarfFarma.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h10 + 2)];
                            Cell.PutValue(HaghDarmanKarfFarma);
                            h10++;
                        }
                        index++;
                        break;
                    case "HaghDarmanDolat":
                        Check = "مبلغ درمان سهم دولت";
                        Cell dcell11 = sheet.Cells[alpha[index] + "1"];
                        dcell11.PutValue(Check);
                        int h11 = 0;
                        foreach (var _item in Personal)
                        {
                            HaghDarmanDolat = _item.fldHaghDarmanDolat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h11 + 2)];
                            Cell.PutValue(HaghDarmanDolat);
                            h11++;
                        }
                        index++;
                        break;
                    case "HaghDarman":
                        Check = "حق درمان";
                        Cell dcell12 = sheet.Cells[alpha[index] + "1"];
                        dcell12.PutValue(Check);
                        int h12 = 0;
                        foreach (var _item in Personal)
                        {
                            HaghDarman = _item.fldHaghDarman.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h12 + 2)];
                            Cell.PutValue(HaghDarman);
                            h12++;
                        }
                        index++;
                        break;
                    case "BimePersonal":
                        Check = "مبلغ بیمه پرسنل";
                        Cell dcell13 = sheet.Cells[alpha[index] + "1"];
                        dcell13.PutValue(Check);
                        int h13 = 0;
                        foreach (var _item in Personal)
                        {
                            BimePersonal = _item.fldBimePersonal.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h13 + 2)];
                            Cell.PutValue(BimePersonal);
                            h13++;
                        }
                        index++;
                        break;
                    case "BimeKarFarma":
                        Check = "مبلغ بیمه سهم کارفرما";
                        Cell dcell14 = sheet.Cells[alpha[index] + "1"];
                        dcell14.PutValue(Check);
                        int h14 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeKarFarma = _item.fldBimeKarFarma.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h14 + 2)];
                            Cell.PutValue(BimeKarFarma);
                            h14++;
                        }
                        index++;
                        break;
                    case "BimeBikari":
                        Check = "مبلغ بیمه بیکاری";
                        Cell dcell15 = sheet.Cells[alpha[index] + "1"];
                        dcell15.PutValue(Check);
                        int h15 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeBikari = _item.fldBimeBikari.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h15 + 2)];
                            Cell.PutValue(BimeBikari);
                            h15++;
                        }
                        index++;
                        break;
                    case "BimeMashaghel":
                        Check = "مبلغ بیمه مشاغل";
                        Cell dcell16 = sheet.Cells[alpha[index] + "1"];
                        dcell16.PutValue(Check);
                        int h16 = 0;
                        foreach (var _item in Personal)
                        {
                            BimeMashaghel = _item.fldBimeMashaghel.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h16 + 2)];
                            Cell.PutValue(BimeMashaghel);
                            h16++;
                        }
                        index++;
                        break;
                    case "TedadNobatKari":
                        Check = "تعداد نوبت کاری";
                        Cell dcell17 = sheet.Cells[alpha[index] + "1"];
                        dcell17.PutValue(Check);
                        int h17 = 0;
                        foreach (var _item in Personal)
                        {
                            TedadNobatKari = _item.fldTedadNobatKari.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h17 + 2)];
                            Cell.PutValue(TedadNobatKari);
                            h17++;
                        }
                        index++;
                        break;
                    case "Mosaede":
                        Check = "مساعده";
                        Cell dcell18 = sheet.Cells[alpha[index] + "1"];
                        dcell18.PutValue(Check);
                        int h18 = 0;
                        foreach (var _item in Personal)
                        {
                            Mosaede = _item.fldMosaede.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h18 + 2)];
                            Cell.PutValue(Mosaede);
                            h18++;
                        }
                        index++;
                        break;
                    case "GhestVam":
                        Check = "قسط وام";
                        Cell dcell19 = sheet.Cells[alpha[index] + "1"];
                        dcell19.PutValue(Check);
                        int h19 = 0;
                        foreach (var _item in Personal)
                        {
                            GhestVam = _item.fldGhestVam.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h19 + 2)];
                            Cell.PutValue(GhestVam);
                            h19++;
                        }
                        index++;
                        break;
                    case "PasAndazM":
                        Check = "مبلغ پس انداز";
                        Cell dcell20 = sheet.Cells[alpha[index] + "1"];
                        dcell20.PutValue(Check);
                        int h20 = 0;
                        foreach (var _item in Personal)
                        {
                            PasAndazM = _item.fldPasAndazM.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h20 + 2)];
                            Cell.PutValue(PasAndazM);
                            h20++;
                        }
                        index++;
                        break;
                    case "MashmolBime":
                        Check = "مشمول بیمه";
                        Cell dcell21 = sheet.Cells[alpha[index] + "1"];
                        dcell21.PutValue(Check);
                        int h21 = 0;
                        foreach (var _item in Personal)
                        {
                            MashmolBime = _item.fldMashmolBime.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h21 + 2)];
                            Cell.PutValue(MashmolBime);
                            h21++;
                        }
                        index++;
                        break;
                    case "MashmolMaliyat":
                        Check = "مشمول مالیات";
                        Cell dcell22 = sheet.Cells[alpha[index] + "1"];
                        dcell22.PutValue(Check);
                        int h22 = 0;
                        foreach (var _item in Personal)
                        {
                            MashmolMaliyat = _item.fldMashmolMaliyat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h22 + 2)];
                            Cell.PutValue(MashmolMaliyat);
                            h22++;
                        }
                        index++;
                        break;
                    case "Mogharari":
                        Check = "مبلغ مقرری";
                        Cell dcell23 = sheet.Cells[alpha[index] + "1"];
                        dcell23.PutValue(Check);
                        int h23 = 0;
                        foreach (var _item in Personal)
                        {
                            Mogharari = _item.fldMogharari.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h23 + 2)];
                            Cell.PutValue(Mogharari);
                            h23++;
                        }
                        index++;
                        break;
                    case "Maliyat":
                        Check = "مبلغ مالیات";
                        Cell dcell24 = sheet.Cells[alpha[index] + "1"];
                        dcell24.PutValue(Check);
                        int h24 = 0;
                        foreach (var _item in Personal)
                        {
                            Maliyat = _item.fldMaliyat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h24 + 2)];
                            Cell.PutValue(Maliyat);
                            h24++;
                        }
                        index++;
                        break;
                    case "khalesPardakhti":
                        Check = "مبلغ خالص پرداختی";
                        Cell dcell25 = sheet.Cells[alpha[index] + "1"];
                        dcell25.PutValue(Check);
                        int h25 = 0;
                        foreach (var _item in Personal)
                        {
                            khalesPardakhti = _item.fldkhalesPardakhti.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h25 + 2)];
                            Cell.PutValue(khalesPardakhti);
                            h25++;
                        }
                        index++;
                        break;
                    case "ShomareHesab":
                        Check = "شماره حساب";
                        Cell dcell26 = sheet.Cells[alpha[index] + "1"];
                        dcell26.PutValue(Check);
                        int h26 = 0;
                        foreach (var _item in Personal)
                        {
                            ShomareHesab = _item.fldShomareHesab.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (h26 + 2)];
                            Cell.PutValue(ShomareHesab);
                            h26++;
                        }
                        index++;
                        break;
                }
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            return File(stream.ToArray(), "xls", "All.xls");
        }
    }
}
