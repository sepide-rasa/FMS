using NewFMS.Models.Mohasebat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFMS.Controllers.TestFormula
{
    public class FormulHoghughController : Controller
    {
        //
        // GET: /FormulHoghugh/
        struct param_motalebe
        {
            public int id;
            public decimal mablagh;
        }
        Mohasebat mohasebe;
        Mohasebat mohasebe1;
        int _Month; int _Year;
        long SumBime, SumBimeMoavaghe,MaxBime;int MogharariMoavagh;
        WCF_Common_Pay.Comon_PayService servic_Com_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_Com_Pay = new WCF_Common_Pay.ClsError();
        WCF_PayRoll.PayRolService service_Pay = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError Err_Pay = new WCF_PayRoll.ClsError();
        WCF_Personeli.PersoneliService servic_Prs = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError Err_Prs = new WCF_Personeli.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        public Mohasebat Formulator(int PayPersonalId, int? PrsPersonalId, int Year, int Month, bool Moavaghe, bool Mogharari
            , byte NobatPardakht, int TypeEstekhdam, int TypebimeId, int? AnvaeEstekhdamId, string start_mah, int fldNoePardakht)
        {
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            try
            {
                mohasebe = new Mohasebat();
                mohasebe1 = new Mohasebat();
                byte Ezafe_Tatil = 1;
                if (fldNoePardakht == 3)
                    Ezafe_Tatil = 2;
                DataSet.DataSet1TableAdapters.QueriesTableAdapter qd = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                var kar = service_Pay.GetKarKardeMahaneWithFilter("fldNobatePardakht_YM_Id", NobatPardakht.ToString(), Year.ToString(), Month.ToString(), NobatPardakht, Convert.ToInt32(PayPersonalId), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                bool Moavagh = false; string TaTarikh = "";int  TarikhCalc =Convert.ToInt32( Year.ToString() +  Month.ToString().PadLeft(2, '0'));
                if (kar!=null && kar.fldMoavaghe == true)
                {
                    Moavagh = Convert.ToBoolean(kar.fldMoavaghe);
                    start_mah = kar.fldAzTarikhMoavaghe.ToString().Substring(0, 4) + "/" + kar.fldAzTarikhMoavaghe.ToString().Substring(4);
                    TaTarikh = kar.fldTaTarikhMoavaghe.ToString().Substring(0, 4) + "/" + kar.fldTaTarikhMoavaghe.ToString().Substring(4);

                }
                else
                {
                    Moavagh = Convert.ToBoolean(Moavaghe);
                    TaTarikh = Year.ToString() + "/" + Month.ToString().PadLeft(2, '0');
                }
              
                //var Personal = service_Pay.GetPersonalInfo_MohasebeWithFilter(Convert.ToInt16(Year), Convert.ToByte(Month), Convert.ToByte(NobatPardakht), Convert.ToByte(fldNoePardakht), Ezafe_Tatil, Convert.ToInt32(Session["OrganId"]), IP, out Err_Pay).ToList();
                //foreach (var item in Personal)
                //{
                var PayPersonalInfo = service_Pay.GetPayPersonalInfoDetail(Convert.ToInt32(PayPersonalId/*item.PayId*/), Convert.ToInt32(Session["OrganId"]), IP, out Err_Pay);
                PrsPersonalId = PayPersonalInfo.fldPrs_PersonalInfoId;
                var f = servic_Com_Pay.GetMaxPersonalEstekhdamTypeWithFilter("", Convert.ToInt32(PrsPersonalId), "", IP, out Err_Com_Pay).FirstOrDefault();
                AnvaeEstekhdamId = f.fldAnvaEstekhdamId; //10 نوع استخدام

                int calc_type = fldNoePardakht;
                var z = servic_Com_Pay.GetAnvaEstekhdamDetail(Convert.ToInt32(AnvaeEstekhdamId), IP, out Err_Com_Pay);

                TypeEstekhdam = z.fldNoeEstekhdamId;//نوع 1=کارگر نوع 2=کارمند رسمی 3=کارمند پیمانی 4= شهردار
                var Typebime = PayPersonalInfo.fldTypeBimeId;
                TypebimeId = PayPersonalInfo.fldTypeBimeId;
                //AnvaeEstekhdamId=10 نوع استخدام
                //TypeEstekhdam=نوع 1=کارگر نوع 2=کارمند رسمی 3=کارمند پیمانی 4= شهردار
                int Day = 30;
                Day = 30;
                if (Month <= 6)
                    Day = 31;
                if (MyLib.Shamsi.Iskabise(Year) == true && Month > 11)
                    Day = 30;
                else if (MyLib.Shamsi.Iskabise(Year) == false && Month > 11)
                    Day = 29;
                if (TypeEstekhdam > 1)
                    Day = 30;
                var PersonalInfo = GetPersonalInfo(PayPersonalId);
                
                var bime = service_Pay.GetMaxBimeWithFilter("fldYear", Year.ToString(), 0, IP, out Err_Pay).FirstOrDefault();                
                var karkard = GetKarkardMahane(Year, Month, PayPersonalId, NobatPardakht);
                MaxBime = bime.fldMablaghBime * Convert.ToByte(karkard.fldKarkard); 
                if (fldNoePardakht == 1)
                {
                    string commandText = "SELECT Pay.tblMaliyatManfi.fldId FROM Pay.tblMaliyatManfi INNER " +
                        "JOIN Pay.tblMohasebat ON Pay.tblMaliyatManfi.fldMohasebeId = " +
                        "Pay.tblMohasebat.fldId where Pay.tblMohasebat.fldPersonalId=" + PayPersonalId +
                        " and Pay.tblMohasebat.fldYear=" + Year + " and Pay.tblMohasebat.fldMonth=" + Month;
                    var tblmaliatManfi = service.GetData(commandText).Tables[0];
                    if (tblmaliatManfi.Rows.Count > 0)
                    {
                        service_Pay.DeleteMaliyatManfi(Convert.ToInt32(tblmaliatManfi.Rows[0]["fldId"]),
                            Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Pay);
                    }
                    commandText = "SELECT Pay.tblP_MaliyatManfi.fldId FROM Pay.tblP_MaliyatManfi INNER JOIN " +
                        "Pay.tblMohasebat ON Pay.tblP_MaliyatManfi.fldMohasebeId = Pay.tblMohasebat.fldId " +
                        "where Pay.tblMohasebat.fldPersonalId=" + PayPersonalId +
                         " and Pay.tblMohasebat.fldYear=" + Year + " and Pay.tblMohasebat.fldMonth=" + Month;
                    var tblpmaliatManfi = service.GetData(commandText).Tables[0];
                    if (tblpmaliatManfi.Rows.Count > 0)
                    {
                        service_Pay.DeleteP_MaliyatManfi(Convert.ToInt32(tblpmaliatManfi.Rows[0]["fldId"]),
                            Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Pay);
                    }

                    //service_Pay.MohasebatDelete("Hoghogh_Personal",Convert.ToInt32(fldPayPersonalId),Convert.ToByte(fldMonth),Convert.ToInt16(fldYear), fldNobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err_Pay);
                    //DataSet.DataSet1TableAdapters.QueriesTableAdapter qd = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                    //qd.spr_DeleteMohasebat("Hoghogh_Personal", Convert.ToInt32(PayPersonalId), Convert.ToByte(Month), Convert.ToInt16(Year),Convert.ToByte( NobatPardakht), Convert.ToInt32(Session["OrganId"]), "", "");
                }
               
                var MoteghayerHoghughi = GetMoteghayerHoghughi(TypebimeId, Year, Month, Day,Convert.ToInt32( AnvaeEstekhdamId));
                mohasebe.fldMoteghayerHoghoghiId = MoteghayerHoghughi.fldId;
                var Fiscal = GetFiscal(Year, Month, Day, Convert.ToInt32(AnvaeEstekhdamId));
                mohasebe.fldFiscalHeaderId = Fiscal.fldId;
                var hokms = HokmsInCurentMonth(PayPersonalId, Convert.ToInt32(PrsPersonalId), Year, Month, Day, TypeEstekhdam);
                foreach (var item in hokms)
                {
                    hokm_roz_hog(item, karkard, TypeEstekhdam, TypebimeId, Day, Year, Month, PersonalInfo, hokms[hokms.Count - 1]);
                }
               if(calc_type==1){
                   qd.spr_DeleteMohasebat("Hoghogh_Personal", Convert.ToInt32(PayPersonalId), Convert.ToByte(Month), Convert.ToInt16(Year), Convert.ToByte(NobatPardakht), Convert.ToInt32(Session["OrganId"]), "", "");
                   motalebat_hog(hokms[hokms.Count - 1], MoteghayerHoghughi, karkard, TypeEstekhdam, TypebimeId, Day, Moavaghe, Year, Month, PayPersonalId, PersonalInfo.fldJensiyat, Convert.ToInt32(PrsPersonalId));
        kosorat_hog(MoteghayerHoghughi, Fiscal, hokms[hokms.Count - 1], PersonalInfo, Year, Month, PayPersonalId, Mogharari,Convert.ToInt32( PrsPersonalId), TypeEstekhdam
                       , TypebimeId, Moavaghe, NobatPardakht, 0, 0);
      }
      else if (calc_type == 2)
      {
          ezafekari(PayPersonalId, Year, Month, NobatPardakht, TypeEstekhdam, PersonalInfo, hokms[hokms.Count - 1],
                     MoteghayerHoghughi, Fiscal, TypebimeId);
      }
      else if (calc_type == 3)
      {
      }
      else if (calc_type == 4) {
          eydi(TypeEstekhdam, PersonalInfo.fldJensiyat, PayPersonalId, Year, NobatPardakht, hokms[hokms.Count - 1], PersonalInfo.fldMoafAsMaliyat);
      }
      mohasebe.fldHokmId = hokms[hokms.Count - 1].HokmId;
      mohasebe.fldMazad30Sal = PersonalInfo.fldMazad30Sal;
      mohasebe.fldYear = Convert.ToInt16(Year);
      mohasebe.fldMonth = Convert.ToByte(Month);
      mohasebe.fldKarkard = Convert.ToByte(karkard.fldKarkard);
      mohasebe.fldGheybat=Convert.ToByte(karkard.fldGheybat);
      mohasebe.fldNobatPardakht = NobatPardakht;
      mohasebe1 = (Mohasebat)mohasebe.Clone();
      if (calc_type == 1)
      {
          if (Moavaghe == true)
          {
              bool Auto = false;
              if (start_mah == "" || start_mah == null)
              {
                  start_mah = "1393/01";
                  Auto = true;
              }
              int mah_moavaghe = Convert.ToInt32(start_mah.Substring(5, 2)), sal_moavaghe = Convert.ToInt32(start_mah.Substring(0, 4));
              for (int i = Convert.ToInt32(start_mah.Replace("/", "")); i <= Convert.ToInt32(TaTarikh.Replace("/", "")) && i != TarikhCalc; i++)
              {
                  mohasebe = new Mohasebat();
                  if (Convert.ToInt32(i.ToString().Substring(4, 2)) > 12)
                  {
                      i = Convert.ToInt32((Convert.ToInt32(i.ToString().Substring(0, 4)) + 1).ToString() + (1).ToString().PadLeft(2, '0'));
                      //sal_moavaghe++;
                  }
                  int sal = Convert.ToInt32(i.ToString().Substring(0, 4)), mah = Convert.ToInt32(i.ToString().Substring(4, 2));
                  if (Auto == true && havemoavaghe(PayPersonalId, sal, mah) == true)
                  {
                      string commandText = "SELECT Pay.tblMohasebat_PersonalInfo.fldMazad30Sal, Pay.tblMohasebat_PersonalInfo.fldStatusIsargariId, Pay.tblMohasebat.fldNobatPardakht," +
                        " Pay.tblMohasebat_PersonalInfo.fldFiscalHeaderId, Pay.tblMohasebat_PersonalInfo.fldMoteghayerHoghoghiId, " +
                        "Pay.tblMohasebat_PersonalInfo.fldHokmId, Pay.tblMohasebat_PersonalInfo.fldAnvaEstekhdamId,Pay.tblMohasebat_PersonalInfo.fldTypeBimeId," +
                        " Com.tblAnvaEstekhdam.fldNoeEstekhdamId FROM Pay.tblMohasebat INNER JOIN Pay.tblMohasebat_PersonalInfo ON Pay.tblMohasebat.fldId " +
                        "= Pay.tblMohasebat_PersonalInfo.fldMohasebatId INNER JOIN Com.tblAnvaEstekhdam ON Pay.tblMohasebat_PersonalInfo.fldAnvaEstekhdamId " +
                        "= Com.tblAnvaEstekhdam.fldId AND Pay.tblMohasebat_PersonalInfo.fldAnvaEstekhdamId = Com.tblAnvaEstekhdam.fldId WHERE "
                        + "Pay.tblMohasebat.fldPersonalId=" + PayPersonalId + " and Pay.tblMohasebat.fldYear=" + sal + " and Pay.tblMohasebat.fldMonth=" + mah;
                      var tblMohasebe1 = service.GetData(commandText).Tables[0];
                      int Day1 = 30;
                      Day1 = 30;
                      if (mah <= 6)
                          Day1 = 31;
                      if (MyLib.Shamsi.Iskabise(sal) == true && mah > 11)
                          Day1 = 30;
                      else if (MyLib.Shamsi.Iskabise(sal) == false && mah > 11)
                          Day1 = 29;
                      if (TypeEstekhdam > 1)
                          Day1 = 30;
                      int anvaEstekhdam = /*AnvaeEstekhdamId;//*/Convert.ToInt32(tblMohasebe1.Rows[0]["fldAnvaEstekhdamId"]);
                      int typebime = Convert.ToInt32(tblMohasebe1.Rows[0]["fldTypeBimeId"]);
                      int typeEstekhdam = /*TypeEstekhdam;//*/Convert.ToInt32(tblMohasebe1.Rows[0]["fldNoeEstekhdamId"]);
                      if (typeEstekhdam > 1)
                          Day1 = 30;
                      int nobatPardakht = Convert.ToInt32(tblMohasebe1.Rows[0]["fldNobatPardakht"]);
                      var karkard1 = GetKarkardMahane(sal, mah, PayPersonalId, nobatPardakht);
                      var MoteghayerHoghughi1 = GetMoteghayerHoghughi(typebime, sal, mah, Day1, anvaEstekhdam);
                      var Fiscal1 = GetFiscal(sal, mah, Day1, anvaEstekhdam);
                      var hokms1 = HokmsInCurentMonth(PayPersonalId, Convert.ToInt32(PrsPersonalId), sal, mah, Day1, typeEstekhdam);
                      foreach (var item in hokms1)
                      {
                          hokm_roz_hog(item, karkard1, typeEstekhdam, typebime, Day1, sal, mah, PersonalInfo, hokms1[hokms1.Count - 1]);
                      }
                      motalebat_hog(hokms1[hokms1.Count - 1], MoteghayerHoghughi1, karkard1, typeEstekhdam, typebime, Day1, Moavaghe, sal, mah, PayPersonalId, PersonalInfo.fldJensiyat,Convert.ToInt32(PrsPersonalId));
                      kosorat_hog(MoteghayerHoghughi1, Fiscal1, hokms1[hokms1.Count - 1], PersonalInfo, sal, mah, PayPersonalId, Mogharari, Convert.ToInt32(PrsPersonalId), typeEstekhdam
                            , typebime, Moavaghe, nobatPardakht, sal, mah);
                      mohasebe.fldHokmId = hokms[hokms.Count - 1].HokmId;
                      m_Moavaghe(sal, mah, PayPersonalId, hokms[hokms.Count - 1], MoteghayerHoghughi1, Fiscal1, PersonalInfo, Convert.ToInt32(PrsPersonalId), anvaEstekhdam, typebime, typeEstekhdam, hokms1[hokms1.Count - 1].HokmId);
                  }
                  else
                  {
                      string commandText = "SELECT Pay.tblMohasebat_PersonalInfo.fldMazad30Sal, Pay.tblMohasebat_PersonalInfo.fldStatusIsargariId, Pay.tblMohasebat.fldNobatPardakht," +
                        " Pay.tblMohasebat_PersonalInfo.fldFiscalHeaderId, Pay.tblMohasebat_PersonalInfo.fldMoteghayerHoghoghiId, " +
                        "Pay.tblMohasebat_PersonalInfo.fldHokmId, Pay.tblMohasebat_PersonalInfo.fldAnvaEstekhdamId,Pay.tblMohasebat_PersonalInfo.fldTypeBimeId," +
                        " Com.tblAnvaEstekhdam.fldNoeEstekhdamId FROM Pay.tblMohasebat INNER JOIN Pay.tblMohasebat_PersonalInfo ON Pay.tblMohasebat.fldId " +
                        "= Pay.tblMohasebat_PersonalInfo.fldMohasebatId INNER JOIN Com.tblAnvaEstekhdam ON Pay.tblMohasebat_PersonalInfo.fldAnvaEstekhdamId " +
                        "= Com.tblAnvaEstekhdam.fldId AND Pay.tblMohasebat_PersonalInfo.fldAnvaEstekhdamId = Com.tblAnvaEstekhdam.fldId WHERE "
                        + "Pay.tblMohasebat.fldPersonalId=" + PayPersonalId + " and Pay.tblMohasebat.fldYear=" + sal + " and Pay.tblMohasebat.fldMonth=" + mah;
                      var tblMohasebe1 = service.GetData(commandText).Tables[0];
                      int Day1 = 30;
                      Day1 = 30;
                      if (mah <= 6)
                          Day1 = 31;
                      if (MyLib.Shamsi.Iskabise(sal) == true && mah > 11)
                          Day1 = 30;
                      else if (MyLib.Shamsi.Iskabise(sal) == false && mah > 11)
                          Day1 = 29;
                      int anvaEstekhdam = /*AnvaeEstekhdamId;//*/Convert.ToInt32(tblMohasebe1.Rows[0]["fldAnvaEstekhdamId"]);
                      int typebime = Convert.ToInt32(tblMohasebe1.Rows[0]["fldTypeBimeId"]);
                      int typeEstekhdam = /*TypeEstekhdam;//*/Convert.ToInt32(tblMohasebe1.Rows[0]["fldNoeEstekhdamId"]);
                      if (typeEstekhdam > 1)
                          Day1 = 30;
                      int nobatPardakht = Convert.ToInt32(tblMohasebe1.Rows[0]["fldNobatPardakht"]);
                      var karkard1 = GetKarkardMahane(sal, mah, PayPersonalId, nobatPardakht);
                      var MoteghayerHoghughi1 = GetMoteghayerHoghughi(typebime, sal, mah, Day1, anvaEstekhdam);
                      var Fiscal1 = GetFiscal(sal, mah, Day1, anvaEstekhdam);
                      var hokms1 = HokmsInCurentMonth(PayPersonalId, Convert.ToInt32(PrsPersonalId), sal, mah, Day1, typeEstekhdam);
                      foreach (var item in hokms1)
                      {
                          hokm_roz_hog(item, karkard1, typeEstekhdam, typebime, Day1, sal, mah, PersonalInfo, hokms1[hokms1.Count - 1]);
                      }
                      motalebat_hog(hokms1[hokms1.Count - 1], MoteghayerHoghughi1, karkard1, typeEstekhdam, typebime, Day1, Moavaghe, sal, mah, PayPersonalId, PersonalInfo.fldJensiyat, Convert.ToInt32(PrsPersonalId));
                      kosorat_hog(MoteghayerHoghughi1, Fiscal1, hokms1[hokms1.Count - 1], PersonalInfo, sal, mah, PayPersonalId, Mogharari, Convert.ToInt32(PrsPersonalId), typeEstekhdam
                             , typebime, Moavaghe, nobatPardakht, sal, mah);
                      mohasebe.fldHokmId = hokms[hokms.Count - 1].HokmId;
                      m_Moavaghe(sal, mah, PayPersonalId, hokms[hokms.Count - 1], MoteghayerHoghughi1, Fiscal1, PersonalInfo, Convert.ToInt32(PrsPersonalId), anvaEstekhdam, typebime, typeEstekhdam, hokms1[hokms1.Count - 1].HokmId);
                  }
              }
          }
      }
      return mohasebe1;
            }
            catch (Exception x)
            {
                DateTime d1;
                d1 = DateTime.Now;
                string excep = "";
                if (x.InnerException != null)
                    excep = "'DynamicCode " + x.InnerException.Message + "'";
                else
                    excep = "'DynamicCode " + x.Message + "'";
                string commandText1 = "exec [Com].[spr_tblErrorInsert] 	0,'admin'," + excep + ",'" + d1.ToString() + "',1,1,''";
                var _Hokm1 = service.GetData(commandText1).Tables[0];
                return mohasebe1;
            }
        }




        private void Mamoriat(int code, int sal, int mah, int nobat, int typeEstekhdam,
                          PersonalInfo personalinf, int PrsPersonalId, Hokm lastHokm, MoteghayerhayeHoghoghi moteghayer, Fiscal fiscal, int TypebimeId)
        {
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            try
            {
                double mashmol_bime = 0;
                double t_mamoriyat = 0, mamoriat = 0;
                double t_mamoriyat_baranandegi = 0, t_mamoriyat_bedonranandegi = 0;
                double hoghogh_sanavat = 0;
                int zarib = moteghayer.fldZaribHoghoghiSal;
                string commandText = "SELECT fldId, fldPersonalId, fldYear, fldMonth, fldNobatePardakht, fldBaBeytute, fldBeduneBeytute, fldBa10, fldBa20, fldBa30, fldBe10, fldBe20, fldBe30, fldUserId, fldDate, fldDesc FROM Pay.tblMamuriyat where fldPersonalId=" + code + " and fldYear=" + sal
                  + " and fldMonth=" + mah + " and fldNobatePardakht=" + nobat;
                var tblmamoriat = service.GetData(commandText).Tables[0];
                if (tblmamoriat.Rows.Count > 0)
                {
                    int m_maliat = 0, m_bime = 0, b_prs = 0, b_kar = 0, b_bikari = 0, mo_maliat = 0;
                    commandText = "SELECT fldId, fldPersonalId, fldYear, fldMonth, fldKarkard, fldGheybat, fldTedadEzafeKar," +
                      " fldTedadTatilKar, fldBaBeytute, fldBedunBeytute, fldBimeOmrKarFarma, fldBimeOmr, fldBimeTakmilyKarFarma," +
                      " fldBimeTakmily, fldHaghDarmanKarfFarma, fldHaghDarmanDolat, fldHaghDarman, fldBimePersonal, fldBimeKarFarma," +
                      " fldBimeBikari, fldBimeMashaghel, fldDarsadBimePersonal, fldDarsadBimeKarFarma, fldDarsadBimeBikari,"
                      + " fldDarsadBimeSakht, fldTedadNobatKari, fldMosaede, fldNobatPardakht, fldGhestVam, fldPasAndaz, fldMashmolBime," +
                      " fldMashmolMaliyat, fldFlag, fldMogharari, fldMaliyat, fldkhalesPardakhti, fldUserId, fldDesc, fldDate, fldShift " +
                      "FROM Pay.tblMohasebat WHERE (fldPersonalId = " + code + ") AND (fldYear = " + sal + ") AND (fldMonth = " + mah + ")";
                    var tblmohasebat = service.GetData(commandText).Tables[0];
                    if (tblmohasebat.Rows.Count > 0)
                    {
                        mashmol_bime = Convert.ToDouble(tblmohasebat.Rows[0]["fldMashmolBime"]);
                        m_maliat = Convert.ToInt32(tblmohasebat.Rows[0]["fldMashmolMaliyat"]);
                        m_bime = Convert.ToInt32(tblmohasebat.Rows[0]["fldMashmolBime"]);
                        b_prs = Convert.ToInt32(tblmohasebat.Rows[0]["fldBimePersonal"]);
                        b_kar = Convert.ToInt32(tblmohasebat.Rows[0]["fldBimeKarFarma"]);
                        b_bikari = Convert.ToInt32(tblmohasebat.Rows[0]["fldBimeBikari"]);
                        mo_maliat = Convert.ToInt32(tblmohasebat.Rows[0]["fldMaliyat"]);
                        if (typeEstekhdam == 1)
                        {
                            //--------------------ماموریت کارگر   
                            var hoghogh = lastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault();
                            if (hoghogh != null)
                                hoghogh_sanavat += hoghogh.Mablagh;
                            var sanavat = lastHokm.Items.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();
                            if (sanavat != null)
                                hoghogh_sanavat += sanavat.Mablagh;
                            var paye = lastHokm.Items.Where(l => l.ItemHoghughiId == 3).FirstOrDefault();
                            if (paye != null)
                                hoghogh_sanavat += paye.Mablagh;
                            var jebhe = lastHokm.Items.Where(l => l.ItemHoghughiId == 30).FirstOrDefault();
                            if (jebhe != null)
                                hoghogh_sanavat += jebhe.Mablagh;
                            var janbazi = lastHokm.Items.Where(l => l.ItemHoghughiId == 31).FirstOrDefault();
                            if (janbazi != null)
                                hoghogh_sanavat += janbazi.Mablagh;
                            var ashora = lastHokm.Items.Where(l => l.ItemHoghughiId == 38).FirstOrDefault();
                            if (ashora != null)
                                hoghogh_sanavat += ashora.Mablagh;
                            hoghogh_sanavat = hoghogh_sanavat / 30;
                            t_mamoriyat = Convert.ToDouble(tblmamoriat.Rows[0]["fldBaBeytute"]);
                            mamoriat = Math.Round(Convert.ToDouble(t_mamoriyat) * hoghogh_sanavat, 0);
                        }
                        else if (typeEstekhdam > 1)
                        {
                            //--------------------ماموریت کارمند
                            int MoteghayerhayeHoghoghiId = moteghayer.fldId, mablaghKasrMashmool = 0;
                            commandText = "select e.fldItemsHoghughiId from pay.tblMoteghayerhayeHoghoghi_Detail as d " +
                            "inner join com.tblItems_Estekhdam as e on e.fldId=d.fldItemEstekhdamId " +
                            "inner join com.tblItemsHoghughi as i on i.fldId=e.fldItemsHoghughiId " +
                            "where d.fldMoteghayerhayeHoghoghiId=" + MoteghayerhayeHoghoghiId + " and i.fldType =1";
                            var moteghayerDetails = service.GetData(commandText).Tables[0];
                            for (int i = 0; i < moteghayerDetails.Rows.Count; i++)
                            {

                                if (lastHokm.Items.Where(l => l.ItemHoghughiId == Convert.ToInt32(moteghayerDetails.Rows[i]["fldItemsHoghughiId"])).FirstOrDefault() != null)
                                    mablaghKasrMashmool += lastHokm.Items.Where(l => l.ItemHoghughiId == Convert.ToInt32(moteghayerDetails.Rows[i]["fldItemsHoghughiId"])).FirstOrDefault().Mablagh;
                            }
                            t_mamoriyat_baranandegi = Convert.ToDouble(tblmamoriat.Rows[0]["fldBaBeytute"]);
                            t_mamoriyat_bedonranandegi = Convert.ToDouble(tblmamoriat.Rows[0]["fldBeduneBeytute"]);
                            t_mamoriyat = t_mamoriyat_baranandegi + t_mamoriyat_bedonranandegi;
                            double ba = 0, be = 0, ba10 = 0, ba20 = 0, ba30 = 0, be10 = 0, be20 = 0, be30 = 0;
                            commandText = "SELECT      top(1)  * " +
                              "FROM            Prs.tblMoteghayerhaAhkam " +
                              "WHERE fldYear<=" + sal + "AND fldType=1 order by fldYear desc";
                            //           NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                            var tblMoteghayerhaAhkam = service.GetData(commandText).Tables[0];
                            if (tblMoteghayerhaAhkam.Rows.Count != 0)
                            {
                                double Hadeaghal = Convert.ToDouble(tblMoteghayerhaAhkam.Rows[0]["fldHadaghalTadil"]);
                                //ba = ((Hadeaghal / 25) + ((mashmol_bime - Hadeaghal) / 50)) * t_mamoriyat_baranandegi;
                                //be = (((Hadeaghal / 25) + ((mashmol_bime - Hadeaghal) / 50)) / 2) * t_mamoriyat_bedonranandegi;
                                ba = ((Hadeaghal / 25) + ((mablaghKasrMashmool - Hadeaghal) / 50)) * t_mamoriyat_baranandegi;
                                be = (((Hadeaghal / 25) + ((mablaghKasrMashmool - Hadeaghal) / 50)) / 2) * t_mamoriyat_bedonranandegi;
                                mamoriat = Math.Round(ba + be);
                            }
                        }
                        else
                        {
                            t_mamoriyat = 0;
                            mamoriat = 0;
                        }
                        mohasebe.fldBaBeytute = Convert.ToByte(t_mamoriyat);
                        mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(mamoriat), fldItemEstekhdamId = GetItem_Estekhdam(34, typeEstekhdam), fldItemHoghoghiId = 34 });
                        mashmol_bime = 0;
                        var moteghayer_Item = moteghayer.Items.Where(l => l.fldItemHoghughiId == 34).FirstOrDefault();
                        if (moteghayer_Item != null)
                            mashmol_bime += mamoriat;
                        mashmol_bime += Convert.ToInt32(tblmohasebat.Rows[0]["fldMashmolBime"]);
                        //--------------------حق بیمه
                        double b_p = Convert.ToDouble(moteghayer.fldDarsadBimePersonal);
                        double b_k = Convert.ToDouble(moteghayer.fldDarsadbimeKarfarma);
                        double b_b = Convert.ToDouble(moteghayer.fldDarsadBimeBikari);
                        double b_k_j = Convert.ToDouble(moteghayer.fldDarsadBimeJanbazan);
                        double saghfebime = 0,
                        bimepersenel = 0, bimekarfarma = 0, bimebikari = 0;
                        double saghfebime_max = 0;
                        if (mashmol_bime > MaxBime && TypebimeId == 1)
                        {
                            saghfebime_max = mashmol_bime;
                            mashmol_bime = MaxBime;
                        }
                        if (TypebimeId == 1)
                            SumBime = (int)mashmol_bime;
                        if ((personalinf.fldEsargariId == 1 || personalinf.fldEsargariId == 4) && personalinf.fldMazad30Sal == false)
                        {
                            bimepersenel = Math.Round((mashmol_bime * Convert.ToDouble(b_p)) / 100);
                            bimekarfarma = Math.Round((mashmol_bime * Convert.ToDouble(b_k)) / 100);
                            bimebikari = Math.Round((mashmol_bime * Convert.ToDouble(b_b)) / 100);
                        }
                        else if (personalinf.fldMazad30Sal == true && (personalinf.fldEsargariId == 1 || personalinf.fldEsargariId == 4))
                        {
                            bimepersenel = 0;
                            bimekarfarma = Math.Round((mashmol_bime * Convert.ToDouble(b_k)) / 100);
                            bimebikari = Math.Round((mashmol_bime * Convert.ToDouble(b_b)) / 100);
                        }
                        else if (personalinf.fldEsargariId == 2 || personalinf.fldEsargariId == 3 || personalinf.fldEsargariId == 5)
                        {
                            bimepersenel = 0;
                            bimekarfarma = Math.Round((mashmol_bime * Convert.ToDouble(b_k_j)) / 100);
                            bimebikari = Math.Round((mashmol_bime * Convert.ToDouble(b_b)) / 100);
                        }
                        mohasebe.fldBimeKarFarma = Convert.ToInt32(bimekarfarma) - Convert.ToInt32(tblmohasebat.Rows[0]["fldBimeKarFarma"]);
                        mohasebe.fldBimePersonal = Convert.ToInt32(bimepersenel) - Convert.ToInt32(tblmohasebat.Rows[0]["fldBimePersonal"]);
                        mohasebe.fldBimeBikari = Convert.ToInt32(bimebikari) - Convert.ToInt32(tblmohasebat.Rows[0]["fldBimeBikari"]);
                        mohasebe.fldMashmolBime = Convert.ToInt32(mashmol_bime) - Convert.ToInt32(tblmohasebat.Rows[0]["fldMashmolBime"]);
                        mohasebe.fldMashmolBimeNaKhales = Convert.ToInt32(saghfebime_max) - Convert.ToInt32(tblmohasebat.Rows[0]["fldMashmolBime"]);
                        if (personalinf.fldEsargariId == 1)
                            mohasebe.fldDarsadBimeKarFarma = Convert.ToDecimal(b_b + b_k);
                        else
                            mohasebe.fldDarsadBimeKarFarma = Convert.ToDecimal(b_b + b_k_j);
                        mohasebe.fldDarsadBimePersonal = (decimal)b_p;
                        mohasebe.fldDarsadBimeBikari = (decimal)b_b;
                        //-------------------مشمولی مالیات                                                
                        double mashmol_maliyat = 0;
                        mashmol_maliyat = Convert.ToDouble(tblmohasebat.Rows[0]["fldMashmolMaliyat"]);
                        var Mohasebe_Item = fiscal.Items_Title.Where(l => l.fldItemHoghughiId == 34).FirstOrDefault();
                        if (Mohasebe_Item != null)
                            mashmol_maliyat += mamoriat;
                        //--------------------مالیات  
                        double darsad_maliyat = 0, d_karmand = 0;
                        double zarib_maliyat = 0, az_mablagh_k = 0, az_mablagh = 0;
                        int CountChild = 0, CountChildAll = 0; decimal Percent = 0;
                        CountChildAll = lastHokm.TedadFarzand;
                        if (CountChildAll >= 3)
                        {
                            commandText = "select COUNT(*) as fldCount from prs.tblAfradTahtePooshesh " +
                              "where fldPersonalId=" + PrsPersonalId + " and fldBirthDate>='1400/08/01' and fldNesbat=1 ";
                            var c = service.GetData(commandText).Tables[0];
                            if (c.Rows.Count > 0)
                            {
                                CountChild = Convert.ToInt32(c.Rows[0]["fldCount"]);
                            }
                            Percent = CountChild * Convert.ToDecimal(15 / 100.0);
                        }
                        foreach (var item in fiscal.Items_Detail)
                        {
                            if (mashmol_maliyat <= Convert.ToDouble(item.fldAmountTo + item.fldAmountTo * Percent) && mashmol_maliyat >= Convert.ToDouble(item.fldAmountFrom + item.fldAmountFrom * Percent))
                            {
                                darsad_maliyat = Convert.ToDouble(item.fldPercentTaxOnWorkers);
                                d_karmand = Convert.ToDouble(item.fldTaxationOfEmployees);
                                zarib_maliyat = Convert.ToDouble(item.fldTax);
                                az_mablagh = Convert.ToDouble(item.fldAmountFrom + item.fldAmountFrom * Percent);
                            }
                        }
                        if (az_mablagh != 0)
                        {
                            foreach (var item in fiscal.Items_Detail)
                            {
                                if (Convert.ToDouble(item.fldAmountFrom + item.fldAmountFrom * Percent) < az_mablagh)
                                {
                                    az_mablagh_k = Convert.ToDouble(item.fldAmountTo + item.fldAmountTo * Percent);
                                    if (typeEstekhdam > 1)
                                    {
                                        zarib_maliyat = 0;
                                        break;
                                    }
                                }
                            }
                        }
                        if (typeEstekhdam > 1)
                            darsad_maliyat = d_karmand;
                        double maliyat = 0;
                        if (personalinf.fldEsargariId == 1)
                        {
                            maliyat = Math.Round(((mashmol_maliyat - az_mablagh_k) * Convert.ToDouble(darsad_maliyat)) / 100, 0) + zarib_maliyat;
                        }
                        else
                            maliyat = 0;
                        mohasebe.fldMashmolMaliyat = 0;//(int)mashmol_maliyat - Convert.ToInt32(tblmohasebat.Rows[0]["fldMashmolMaliyat"]);
                        mohasebe.fldMaliyat = 0;//(int)maliyat - Convert.ToInt32(tblmohasebat.Rows[0]["fldMaliyat"]);
                    }
                    decimal khales_pardakhti = 0;
                    mohasebe.fldkhalesPardakhti = Convert.ToInt32(mamoriat - mohasebe.fldBimePersonal - mohasebe.fldMaliyat);
                }
            }
            catch (Exception x)
            {
                DateTime d1;
                d1 = DateTime.Now;
                string excep = "";
                if (x.InnerException != null)
                    excep = "'Mamoriat " + x.InnerException.Message + "'";
                else
                    excep = "'Mamoriat " + x.Message + "'";
                string commandText1 = "exec [Com].[spr_tblErrorInsert] 	0,'admin'," + excep + ",'" + d1.ToString() + "',1,1,''";
                var _Hokm1 = service.GetData(commandText1).Tables[0];
            }
        }
        private void ezafekari(int code, int sal, int mah, int nobat, int typeEstekhdam,
                               PersonalInfo personalinf, Hokm lastHokm, MoteghayerhayeHoghoghi moteghayer, Fiscal fiscal, int TypebimeId)
        {
            string commandText = "SELECT fldId, fldPersonalId, fldYear, fldMonth, fldNobatePardakht, " +
              "fldCount, fldType, fldHasBime, fldHasMaliyat, fldUserId, fldDate, fldDesc " +
              "FROM Pay.tblEzafeKari_TatilKari where fldPersonalId=" + code + " and fldYear=" + sal
              + " and fldMonth=" + mah + " and fldNobatePardakht=" + nobat;
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var tblezafekari = service.GetData(commandText).Tables[0];
            if (tblezafekari.Rows.Count > 0)
            {
                decimal zarib = 0, _satroz = 0, _zarib = 0;
                decimal t_ezafekari = 0, h_rozane2 = 0, e160 = 0, e111 = 0;
                bool h = false, f = false, t = false, h_n = false, h_j = false, z = false, ab = false, f_v = false;
                decimal ezafekari160 = 0, ezafekari111 = 0, ezafekari = 0;
                decimal t_ezafekari160 = 0, t_ezafekari111 = 0;
                commandText = "SELECT fldId, fldPersonalId, fldYear, fldMonth, fldKarkard, fldGheybat, fldTedadEzafeKar," +
                  " fldTedadTatilKar, fldBaBeytute, fldBedunBeytute, fldBimeOmrKarFarma, fldBimeOmr, fldBimeTakmilyKarFarma," +
                  " fldBimeTakmily, fldHaghDarmanKarfFarma, fldHaghDarmanDolat, fldHaghDarman, fldBimePersonal, fldBimeKarFarma," +
                  " fldBimeBikari, fldBimeMashaghel, fldDarsadBimePersonal, fldDarsadBimeKarFarma, fldDarsadBimeBikari,"
                  + " fldDarsadBimeSakht, fldTedadNobatKari, fldMosaede, fldNobatPardakht, fldGhestVam, fldPasAndaz, fldMashmolBime," +
                  " fldMashmolMaliyat, fldFlag, fldMogharari, fldMaliyat, fldkhalesPardakhti, fldUserId, fldDesc, fldDate, fldShift " +
                  "FROM Pay.tblMohasebat WHERE (fldPersonalId = " + code + ") AND (fldYear = " + sal + ") AND (fldMonth = " + mah + ")";
                var tblmohasebat = service.GetData(commandText).Tables[0];
                if (tblmohasebat.Rows.Count > 0)
                {
                    zarib = Convert.ToDecimal(moteghayer.fldZaribHoghoghiSal);
                    if (typeEstekhdam == 1)
                    {
                        //--------------------اضافه کاری کارگر   
                        var hoghogh = lastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault();
                        if (hoghogh != null)
                            h_rozane2 += hoghogh.Mablagh;
                        var sanavat = lastHokm.Items.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();
                        if (sanavat != null)
                            h_rozane2 += sanavat.Mablagh;
                        var paye = lastHokm.Items.Where(l => l.ItemHoghughiId == 3).FirstOrDefault();
                        if (paye != null)
                            h_rozane2 += paye.Mablagh;
                        var jebhe = lastHokm.Items.Where(l => l.ItemHoghughiId == 30).FirstOrDefault();
                        if (jebhe != null)
                            h_rozane2 += jebhe.Mablagh;
                        var janbazi = lastHokm.Items.Where(l => l.ItemHoghughiId == 31).FirstOrDefault();
                        if (janbazi != null)
                            h_rozane2 += janbazi.Mablagh;
                        var ashora = lastHokm.Items.Where(l => l.ItemHoghughiId == 38).FirstOrDefault();
                        if (ashora != null)
                            h_rozane2 += ashora.Mablagh;
                        h_rozane2 = (h_rozane2) / 30;
                        _satroz = Convert.ToDecimal(moteghayer.fldSaatKari);
                        _zarib = Convert.ToDecimal(moteghayer.fldZaribEzafeKar);
                        t_ezafekari = Convert.ToDecimal(tblezafekari.Rows[0]["fldCount"]);
                        ezafekari = Math.Round((((Convert.ToDecimal(h_rozane2) / Convert.ToDecimal(_satroz)) * Convert.ToDecimal(_zarib)) / 100) * Convert.ToDecimal(t_ezafekari), 0);
                    }
                    else if (typeEstekhdam > 1)
                    {
                        //--------------------اضافه کاری کارمند
                        h = Convert.ToBoolean(moteghayer.fldHoghogh);
                        f = Convert.ToBoolean(moteghayer.fldFoghShoghl);
                        t = Convert.ToBoolean(moteghayer.fldTafavotTatbigh);
                        bool Vizhe = Convert.ToBoolean(moteghayer.fldFoghVizhe);
                        h_j = Convert.ToBoolean(moteghayer.fldHaghJazb);
                        bool Tadil = Convert.ToBoolean(moteghayer.fldTadil);
                        bool Sanavat = Convert.ToBoolean(moteghayer.fldSanavat);
                        bool BarJastegi = Convert.ToBoolean(moteghayer.fldBarJastegi);
                        var Talash = Convert.ToBoolean(moteghayer.fldFoghTalash);
                        if (h == true)
                        {
                            e160 = e160 + lastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
                            e111 = e111 + lastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
                        }
                        if (f == true)
                        {
                            var fogh = lastHokm.Items.Where(l => l.ItemHoghughiId == 6).FirstOrDefault();//فوق العاده شغل
                            if (fogh != null)
                            {
                                e160 += fogh.Mablagh;
                                e111 += fogh.Mablagh;
                            }
                            var takhsos = lastHokm.Items.Where(l => l.ItemHoghughiId == 7).FirstOrDefault();//تخصصی
                            if (takhsos != null)
                            {
                                e160 += takhsos.Mablagh;
                                e111 += takhsos.Mablagh;
                            }
                            var made = lastHokm.Items.Where(l => l.ItemHoghughiId == 8).FirstOrDefault();//ماده 26
                            if (made != null)
                            {
                                e160 += made.Mablagh;
                                e111 += made.Mablagh;
                            }
                            var modiriyati = lastHokm.Items.Where(l => l.ItemHoghughiId == 9).FirstOrDefault();// مدیریتی
                            if (modiriyati != null)
                            {
                                e160 += modiriyati.Mablagh;
                                e111 += modiriyati.Mablagh;
                            }
                        }
                        if (t == true)
                        {
                            var tatbigh = lastHokm.Items.Where(l => l.ItemHoghughiId == 11).FirstOrDefault();
                            if (tatbigh != null)
                            {
                                e160 += tatbigh.Mablagh;
                                e111 += tatbigh.Mablagh;
                            }
                            var hadaghalTadil = lastHokm.Items.Where(l => l.ItemHoghughiId == 42).FirstOrDefault();
                            if (hadaghalTadil != null)
                            {
                                e160 += hadaghalTadil.Mablagh;
                                e111 += hadaghalTadil.Mablagh;
                            }
                        }
                        if (Vizhe == true)
                        {
                            var foghVizhe = lastHokm.Items.Where(l => l.ItemHoghughiId == 21).FirstOrDefault();
                            if (foghVizhe != null)
                            {
                                e160 = e160 + foghVizhe.Mablagh;
                                e111 = e111 + foghVizhe.Mablagh;
                            }
                        }
                        if (h_j == true)
                        {
                            var foghJazb = lastHokm.Items.Where(l => l.ItemHoghughiId == 19).FirstOrDefault();
                            if (foghJazb != null)
                            {
                                e160 = e160 + foghJazb.Mablagh;
                                e111 = e111 + foghJazb.Mablagh;
                            }
                            var ghazai = lastHokm.Items.Where(l => l.ItemHoghughiId == 37).FirstOrDefault();
                            if (ghazai != null)
                            {
                                e160 = e160 + ghazai.Mablagh;
                                e111 = e111 + ghazai.Mablagh;
                            }
                            var jazbTakhasosi = lastHokm.Items.Where(l => l.ItemHoghughiId == 40).FirstOrDefault();
                            if (jazbTakhasosi != null)
                            {
                                e160 = e160 + jazbTakhasosi.Mablagh;
                                e111 = e111 + jazbTakhasosi.Mablagh;
                            }
                            var zajbTadil = lastHokm.Items.Where(l => l.ItemHoghughiId == 41).FirstOrDefault();
                            if (zajbTadil != null)
                            {
                                e160 = e160 + zajbTadil.Mablagh;
                                e111 = e111 + zajbTadil.Mablagh;
                            }
                            var jazb2 = lastHokm.Items.Where(l => l.ItemHoghughiId == 47).FirstOrDefault();
                            if (jazb2 != null)
                            {
                                e160 = e160 + jazb2.Mablagh;
                                e111 = e111 + jazb2.Mablagh;
                            }
                            var jazb3 = lastHokm.Items.Where(l => l.ItemHoghughiId == 48).FirstOrDefault();
                            if (jazb3 != null)
                            {
                                e160 = e160 + jazb3.Mablagh;
                                e111 = e111 + jazb3.Mablagh;
                            }
                        }
                        if (Tadil == true)
                        {
                            var foghTadil = lastHokm.Items.Where(l => l.ItemHoghughiId == 16).FirstOrDefault();
                            if (foghTadil != null)
                            {
                                e160 = e160 + foghTadil.Mablagh;
                                e111 = e111 + foghTadil.Mablagh;
                            }
                            var zaribtadil = lastHokm.Items.Where(l => l.ItemHoghughiId == 39).FirstOrDefault();
                            if (zaribtadil != null)
                            {
                                e160 = e160 + zaribtadil.Mablagh;
                                e111 = e111 + zaribtadil.Mablagh;
                            }
                        }
                        if (Sanavat == true)
                        {
                            var sanavat1 = lastHokm.Items.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();//سنوات
                            if (sanavat1 != null)
                            {
                                e160 = e160 + sanavat1.Mablagh;
                                e111 = e111 + sanavat1.Mablagh;
                            }
                            var SanavatEsar = lastHokm.Items.Where(l => l.ItemHoghughiId == 5).FirstOrDefault();//سنوات ایثارگری
                            if (SanavatEsar != null)
                            {
                                e160 = e160 + SanavatEsar.Mablagh;
                                e111 = e111 + SanavatEsar.Mablagh;
                            }
                            var SanavatBasij = lastHokm.Items.Where(l => l.ItemHoghughiId == 4).FirstOrDefault();//سنوات ایثارگری
                            if (SanavatBasij != null)
                            {
                                e160 = e160 + SanavatBasij.Mablagh;
                                e111 = e111 + SanavatBasij.Mablagh;
                            }
                        }
                        if (BarJastegi == true)
                        {
                            var barJestegi = lastHokm.Items.Where(l => l.ItemHoghughiId == 10).FirstOrDefault();
                            if (barJestegi != null)
                            {
                                e160 = e160 + barJestegi.Mablagh;
                                e111 = e111 + barJestegi.Mablagh;
                            }
                        }
                        if (Talash == true)
                        {
                            var foghTalash = lastHokm.Items.Where(l => l.ItemHoghughiId == 29).FirstOrDefault();
                            if (foghTalash != null)
                            {
                                e160 = e160 + foghTalash.Mablagh;
                                e111 = e111 + foghTalash.Mablagh;
                            }
                        }
                        var band_y = lastHokm.Items.Where(l => l.ItemHoghughiId == 44).FirstOrDefault();
                        if (band_y != null)
                        {
                            e160 = e160 + band_y.Mablagh;
                            e111 = e111 + band_y.Mablagh;
                        }
                        var joz1 = lastHokm.Items.Where(l => l.ItemHoghughiId == 45).FirstOrDefault();
                        if (joz1 != null)
                        {
                            e160 = e160 + joz1.Mablagh;
                            e111 = e111 + joz1.Mablagh;
                        }
                        var Tarmim_j = lastHokm.Items.Where(l => l.ItemHoghughiId == 60).FirstOrDefault();/*جذب ناشی از ترمیم حقوق*/
                        if (Tarmim_j != null)
                        {
                            e160 = e160 + Tarmim_j.Mablagh;
                            e111 = e111 + Tarmim_j.Mablagh;
                        }
                        ezafekari160 = e160 / 160;
                     
                        ezafekari111 = e111 / 111;
                        t_ezafekari160 = Convert.ToDecimal(tblezafekari.Rows[0]["fldCount"]);
                        t_ezafekari = t_ezafekari160 + t_ezafekari111;
                        ezafekari = Math.Round((Convert.ToDecimal(ezafekari160) * Convert.ToDecimal(t_ezafekari160)) + (Convert.ToDecimal(ezafekari111) * Convert.ToDecimal(t_ezafekari111)), 0);
                    }
                    else
                    {
                        t_ezafekari = 0;
                        ezafekari = 0;
                    }
                    mohasebe.fldTedadEzafeKar = (byte)t_ezafekari;
                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(ezafekari), fldItemEstekhdamId = GetItem_Estekhdam(33, typeEstekhdam), fldItemHoghoghiId = 33 });
                    decimal mashmol_bime = ezafekari;
                    mashmol_bime = mashmol_bime + Convert.ToInt32(tblmohasebat.Rows[0]["fldMashmolBime"]);
                    bool bool_ezafekari = Convert.ToBoolean(tblezafekari.Rows[0]["fldHasBime"]);
                    if (bool_ezafekari == false)
                        mashmol_bime = mashmol_bime - ezafekari;
                    //--------------------حق بیمه
                    decimal b_p = Convert.ToDecimal(moteghayer.fldDarsadBimePersonal);
                    decimal b_k = Convert.ToDecimal(moteghayer.fldDarsadbimeKarfarma);
                    decimal b_b = Convert.ToDecimal(moteghayer.fldDarsadBimeBikari);
                    decimal b_k_j = Convert.ToDecimal(moteghayer.fldDarsadBimeJanbazan);
                    decimal saghfebime = 0,
                    bimepersenel = 0, bimekarfarma = 0, bimebikari = 0;
                    decimal saghfebime_max = 0;
                    if (mashmol_bime > MaxBime && TypebimeId == 1)
                    {
                        saghfebime_max = mashmol_bime;
                        mashmol_bime = MaxBime;
                    }
                    //           if (SumBime == 0)
                    //             SumBime = SumBime + (int)mashmol_bime;
                    if (TypebimeId == 1)
                        SumBime = (int)mashmol_bime;
                    if ((personalinf.fldEsargariId == 1 || personalinf.fldEsargariId == 4 || personalinf.fldEsargariId == 6 || personalinf.fldEsargariId == 7) && personalinf.fldMazad30Sal == false)
                    {
                        bimepersenel = Math.Round((mashmol_bime * Convert.ToDecimal(b_p)) / 100);
                        bimekarfarma = Math.Round((mashmol_bime * Convert.ToDecimal(b_k)) / 100);
                        bimebikari = Math.Round((mashmol_bime * Convert.ToDecimal(b_b)) / 100);
                    }
                    else if (personalinf.fldMazad30Sal == true && (personalinf.fldEsargariId == 1 || personalinf.fldEsargariId == 4 || personalinf.fldEsargariId == 6 || personalinf.fldEsargariId == 7))
                    {
                        bimepersenel = 0;
                        bimekarfarma = Math.Round((mashmol_bime * Convert.ToDecimal(b_k)) / 100);
                        bimebikari = Math.Round((mashmol_bime * Convert.ToDecimal(b_b)) / 100);
                    }
                    else if (personalinf.fldEsargariId == 2 || personalinf.fldEsargariId == 3 || personalinf.fldEsargariId == 5)
                    {
                        bimepersenel = 0;
                        bimekarfarma = Math.Round((mashmol_bime * Convert.ToDecimal(b_k_j)) / 100);
                        bimebikari = Math.Round((mashmol_bime * Convert.ToDecimal(b_b)) / 100);
                    }
                    mohasebe.fldBimeKarFarma = Convert.ToInt32(bimekarfarma) - Convert.ToInt32(tblmohasebat.Rows[0]["fldBimeKarFarma"]);
                    mohasebe.fldBimePersonal = Convert.ToInt32(bimepersenel) - Convert.ToInt32(tblmohasebat.Rows[0]["fldBimePersonal"]);
                    mohasebe.fldBimeBikari = Convert.ToInt32(bimebikari) - Convert.ToInt32(tblmohasebat.Rows[0]["fldBimeBikari"]);
                    mohasebe.fldMashmolBime = Convert.ToInt32(mashmol_bime) - Convert.ToInt32(tblmohasebat.Rows[0]["fldMashmolBime"]);
                    if (personalinf.fldEsargariId == 1 || personalinf.fldEsargariId == 6 || personalinf.fldEsargariId == 7)
                        mohasebe.fldDarsadBimeKarFarma = Convert.ToDecimal(b_b + b_k);
                    else
                        mohasebe.fldDarsadBimeKarFarma = Convert.ToDecimal(b_b + b_k_j);
                    mohasebe.fldDarsadBimePersonal = (decimal)b_p;
                    mohasebe.fldDarsadBimeBikari = (decimal)b_b;
                    //-------------------مشمولی مالیات                                                
                    decimal mashmol_maliyat = 0;
                    mashmol_maliyat = ezafekari + Convert.ToInt32(tblmohasebat.Rows[0]["fldMashmolMaliyat"]);
                    bool bool_ezafekari1 = Convert.ToBoolean(tblezafekari.Rows[0]["fldHasMaliyat"]);
                    if (bool_ezafekari1 == false)
                        mashmol_maliyat = mashmol_maliyat - ezafekari;
                    //--------------------مالیات  
                    decimal darsad_maliyat = 0, d_karmand = 0;
                    decimal zarib_maliyat = 0, az_mablagh_k = 0, az_mablagh = 0;
                    foreach (var item in fiscal.Items_Detail)
                    {
                        if (mashmol_maliyat <= Convert.ToDecimal(item.fldAmountTo) && mashmol_maliyat >= Convert.ToDecimal(item.fldAmountFrom))
                        {
                            darsad_maliyat = Convert.ToDecimal(item.fldPercentTaxOnWorkers);
                            d_karmand = Convert.ToDecimal(item.fldTaxationOfEmployees);
                            zarib_maliyat = Convert.ToDecimal(item.fldTax);
                            az_mablagh = Convert.ToDecimal(item.fldAmountFrom);
                        }
                    }
                    if (az_mablagh != 0)
                    {
                        foreach (var item in fiscal.Items_Detail)
                        {
                            if (Convert.ToDecimal(item.fldAmountFrom) < az_mablagh)
                            {
                                az_mablagh_k = Convert.ToDecimal(item.fldAmountTo);
                                /*if (typeEstekhdam > 1)
                {
                zarib_maliyat = 0;
                break;
                */
                            }
                        }
                    }
                    if (typeEstekhdam > 1)
                        darsad_maliyat = d_karmand;
                    decimal maliyat = 0;
                    if (personalinf.fldEsargariId == 1 || personalinf.fldEsargariId == 6 || personalinf.fldEsargariId == 7)
                    {
                        maliyat = Math.Round(((mashmol_maliyat - az_mablagh_k) * Convert.ToDecimal(darsad_maliyat)) / 100, 0) + zarib_maliyat;
                    }
                    else
                        maliyat = 0;
                    mohasebe.fldMashmolMaliyat = (int)mashmol_maliyat - Convert.ToInt32(tblmohasebat.Rows[0]["fldMashmolMaliyat"]);
                    mohasebe.fldMaliyat = (int)maliyat - Convert.ToInt32(tblmohasebat.Rows[0]["fldMaliyat"]);
                }
                decimal khales_pardakhti = 0;
                mohasebe.fldkhalesPardakhti = Convert.ToInt32(ezafekari - mohasebe.fldBimePersonal - mohasebe.fldMaliyat);
            }
        }
        //     private void eydi(int TypeEstekhdam,int code,int sal,int nobat,Hokm lasthokm,int isargriId)
        //     {
        //       string commandText = "SELECT fldId, fldPersonalId, fldYear, fldDayCount, fldKosurat, fldNobatePardakht, fldUserId, fldDate, fldDesc"+
        //         " FROM Pay.tblEtelaatEydi where fldPersonalId=" + code + " and fldYear=" + sal + " and fldNobatePardakht=" + nobat;
        //       NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
        //       var tbleydi = service.GetData(commandText).Tables[0];
        //       if (tbleydi.Rows.Count!=0)
        //       {
        //         decimal hoghogh_paye = 0, h_rozane1 = 0, sanavat = 0, hoghogh = 0, paye = 0, jebhe = 0, janbazi = 0, ashora=0;
        //         //--------------------حقوق ماهانه               
        //         var h_p=lasthokm.Items.Where(k => k.ItemHoghughiId == 1).FirstOrDefault();
        //         if (h_p != null)
        //           hoghogh_paye = h_p.Mablagh;
        //         var sa = lasthokm.Items.Where(k => k.ItemHoghughiId == 2).FirstOrDefault();
        //         if (sa != null) 
        //           sanavat = sa.Mablagh;
        //         var pa = lasthokm.Items.Where(k => k.ItemHoghughiId == 3).FirstOrDefault();
        //         if (pa != null)
        //           paye = pa.Mablagh;
        //         var je = lasthokm.Items.Where(k => k.ItemHoghughiId == 30).FirstOrDefault();
        //         if (je != null)
        //           jebhe = je.Mablagh;
        //         var ja = lasthokm.Items.Where(k => k.ItemHoghughiId == 31).FirstOrDefault();
        //         if (ja != null)
        //           janbazi = ja.Mablagh;
        //         var ash= lasthokm.Items.Where(k => k.ItemHoghughiId == 38).FirstOrDefault();
        //         if (ash != null)
        //           ashora = ash.Mablagh;
        //         string commandText1 = "SELECT fldId, fldYear, fldMaxEydiKarmand, fldMaxEydiKargar, fldZaribEydiKargari, "+
        //           "fldTypeMohasebatMaliyat, fldMablaghMoafiatKarmand, fldMablaghMoafiatKargar, fldDarsadMaliyatKarmand,"+
        //           " fldDarsadMaliyatKargar, fldTypeMohasebe, fldUserId, fldDate, fldDesc FROM Pay.tblMoteghayerhayeEydi"+
        //           " where fldYear=" + sal;
        //         var tblmoeydi = service.GetData(commandText1).Tables[0];
        //         int Noo_Maliat = Convert.ToInt32(tblmoeydi.Rows[0]["fldTypeMohasebatMaliyat"]);
        //         int Noo_Calc = Convert.ToInt32(tblmoeydi.Rows[0]["fldTypeMohasebe"]);
        //         if (Noo_Calc == 0)
        //           hoghogh = hoghogh_paye + sanavat + paye + jebhe + janbazi + ashora;
        //         else
        //           hoghogh = hoghogh_paye;
        //         //-------------------عیدی
        //         decimal eidi = 0;
        //         decimal eidi_kargar = 0, eidi_karmand = 0;
        //         decimal zarib = 0;
        //         int t_mah = 0;
        //         t_mah = Convert.ToInt32(tbleydi.Rows[0]["fldDayCount"]);
        //         zarib = Convert.ToDecimal(tblmoeydi.Rows[0]["fldZaribEydiKargari"]);
        //         eidi_kargar = Convert.ToDecimal(tblmoeydi.Rows[0]["fldMaxEydiKargar"]);
        //         eidi_karmand = Convert.ToDecimal(tblmoeydi.Rows[0]["fldMaxEydiKarmand"]);
        //         if (TypeEstekhdam == 1)
        //         {
        //           eidi = hoghogh * zarib;
        //           if (eidi > eidi_kargar)
        //             eidi = eidi_kargar;
        //         }
        //         else
        //         {
        //           eidi = eidi_karmand;
        //           //ابتدای فرمول جدید سال 1400
        //           if(sal==1400){
        //             if(lasthokm.TedadFarzand>0){
        //               eidi=eidi+(lasthokm.TedadFarzand*1500000);
        //             }
        //             if(lasthokm.NoeTaahol>1)
        //             {
        //               eidi=eidi+4000000;
        //             }
        //           }
        //           //انتهای فرمول جدید سال 1400
        //         }
        //         eidi = Math.Round((eidi / 365) * t_mah);
        //         //-------------------مشمولی مالیات
        //         //SqlConnection con_maliat = new SqlConnection(Program.connectiontext);
        //         decimal mashmol_maliyat = 0;
        //         mashmol_maliyat = eidi;
        //         //--------------------مالیات
        //         decimal maliyat = 0;
        //         if (Noo_Maliat == 1)
        //         {
        //           //decimal darsad_maliyat = 0, d_karmand = 0;
        //           //decimal zarib_maliyat = 0, az_mablagh_k = 0, az_mablagh = 0;
        //           //SqlCommand com_maliat = new SqlCommand("select * from maliyat where [group]=" + this.group_maliat.ToString(), con_maliat);
        //           //con_maliat.Open();
        //           //SqlDataReader read_maliat = com_maliat.ExecuteReader();
        //           //while (read_maliat.Read())
        //           //{
        //           //    if (mashmol_maliyat <= Convert.ToDecimal(read_maliat["saghfhoghogh"]) && mashmol_maliyat >= Convert.ToDecimal(read_maliat["as_hoghogh"]))
        //           //    {
        //           //        darsad_maliyat = Convert.ToDecimal(read_maliat["darsadmaliyat"]);
        //           //        d_karmand = Convert.ToDecimal(read_maliat["d_karmand"]);
        //           //        zarib_maliyat = Convert.ToDecimal(read_maliat["sabet"]);
        //           //        az_mablagh = Convert.ToDecimal(read_maliat["as_hoghogh"]);
        //           //    }
        //           //}
        //           //con_maliat.Close();
        //           //com_maliat = new SqlCommand("select * from maliyat where [group]=" + this.group_maliat.ToString(), con_maliat);
        //           //con_maliat.Open();
        //           //read_maliat = com_maliat.ExecuteReader();
        //           //if (az_mablagh != 0)
        //           //{
        //           //    while (read_maliat.Read())
        //           //    {
        //           //        if (Convert.ToDecimal(read_maliat["as_hoghogh"]) < az_mablagh)
        //           //        {
        //           //            az_mablagh_k = Convert.ToDecimal(read_maliat["saghfhoghogh"]);
        //           //            if (estekhdam == 7 || estekhdam == 6 || estekhdam == 10)
        //           //            {
        //           //                zarib_maliyat = 0;
        //           //                break;
        //           //            }
        //           //        }
        //           //    }
        //           //}
        //           //if (estekhdam == 7 || estekhdam == 6 || estekhdam == 10 || estekhdam == 11)
        //           //    darsad_maliyat = d_karmand;
        //           //if (p.vaziyat_jesmi == 1)
        //           //{
        //           //    maliyat = Math.Round(((mashmol_maliyat - az_mablagh_k) * Convert.ToDecimal(darsad_maliyat)) / 100, 0) + zarib_maliyat;
        //           //}
        //           //else
        //           //    maliyat = 0;
        //         }
        //         else
        //         {
        //           decimal moaf = 0;
        //           if (TypeEstekhdam==1)
        //           {
        //             moaf = Convert.ToDecimal(tblmoeydi.Rows[0]["fldMablaghMoafiatKargar"]);
        //           }
        //           else
        //           {
        //             moaf = Convert.ToDecimal(tblmoeydi.Rows[0]["fldMablaghMoafiatKarmand"]);
        //           }
        //           float darsad_karmandi = 0, darsad_kargari = 0;
        //           darsad_kargari = Convert.ToSingle(tblmoeydi.Rows[0]["fldDarsadMaliyatKargar"]);
        //           darsad_karmandi = Convert.ToSingle(tblmoeydi.Rows[0]["fldDarsadMaliyatKarmand"]);
        //           //if (eidi > moaf)
        //           mashmol_maliyat = eidi - moaf;
        //           if (isargriId == 1 && mashmol_maliyat > 0)
        //           {
        //             if (TypeEstekhdam==1)
        //               maliyat = Math.Round((mashmol_maliyat * Convert.ToDecimal(darsad_kargari)) / 100, 0);
        //             else
        //               maliyat = Math.Round((mashmol_maliyat * Convert.ToDecimal(darsad_karmandi)) / 100, 0);
        //           }
        //           else
        //             maliyat = 0;
        //         }
        //         //--------------------سایر کسورات
        //         decimal kosorat = 0;
        //         kosorat = Convert.ToDecimal(tbleydi.Rows[0]["fldKosurat"]);
        //         mohasebe.fldMashmolMaliyat = (int)mashmol_maliyat;
        //         mohasebe.fldMaliyat = (int)maliyat;
        //         mohasebe.fldMablagh = (int)eidi;
        //         mohasebe.fldDayCount = t_mah;
        //         mohasebe.fldMablaghKosorat = (int)kosorat;
        //         //--------------------خالص پرداختی
        //         decimal khales_pardakhti = 0;
        //         khales_pardakhti = eidi - mohasebe.fldMaliyat - mohasebe.fldMablaghKosorat;
        //         mohasebe.fldkhalesPardakhti = (int)khales_pardakhti;
        //       }
        //     }
        private void eydi(int TypeEstekhdam, int Jensiyat, int code, int sal, int nobat, Hokm lasthokm, bool fldMoafAsMaliyat)
        {
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            try
            {
                string commandText = "SELECT fldId, fldPersonalId, fldYear, fldDayCount, fldKosurat, fldNobatePardakht, fldUserId, fldDate, fldDesc" +
                  " FROM Pay.tblEtelaatEydi where fldPersonalId=" + code + " and fldYear=" + sal + " and fldNobatePardakht=" + nobat;
                var tbleydi = service.GetData(commandText).Tables[0];
                if (tbleydi.Rows.Count != 0)
                {
                    decimal hoghogh_paye = 0, h_rozane1 = 0, sanavat = 0, hoghogh = 0, paye = 0, jebhe = 0, janbazi = 0, ashora = 0;
                    //--------------------حقوق ماهانه               
                    var h_p = lasthokm.Items.Where(k => k.ItemHoghughiId == 1).FirstOrDefault();
                    if (h_p != null)
                        hoghogh_paye = h_p.Mablagh;
                    var sa = lasthokm.Items.Where(k => k.ItemHoghughiId == 2).FirstOrDefault();
                    if (sa != null)
                        sanavat = sa.Mablagh;
                    var pa = lasthokm.Items.Where(k => k.ItemHoghughiId == 3).FirstOrDefault();
                    if (pa != null)
                        paye = pa.Mablagh;
                    var je = lasthokm.Items.Where(k => k.ItemHoghughiId == 30).FirstOrDefault();
                    if (je != null)
                        jebhe = je.Mablagh;
                    var ja = lasthokm.Items.Where(k => k.ItemHoghughiId == 31).FirstOrDefault();
                    if (ja != null)
                        janbazi = ja.Mablagh;
                    var ash = lasthokm.Items.Where(k => k.ItemHoghughiId == 38).FirstOrDefault();
                    if (ash != null)
                        ashora = ash.Mablagh;
                    string commandText1 = "SELECT fldId, fldYear, fldMaxEydiKarmand, fldMaxEydiKargar, fldZaribEydiKargari, " +
                      "fldTypeMohasebatMaliyat, fldMablaghMoafiatKarmand, fldMablaghMoafiatKargar, fldDarsadMaliyatKarmand," +
                      " fldDarsadMaliyatKargar, fldTypeMohasebe, fldUserId, fldDate, fldDesc FROM Pay.tblMoteghayerhayeEydi" +
                      " where fldYear=" + sal;
                    var tblmoeydi = service.GetData(commandText1).Tables[0];
                    int Noo_Maliat = Convert.ToInt32(tblmoeydi.Rows[0]["fldTypeMohasebatMaliyat"]);
                    int Noo_Calc = Convert.ToInt32(tblmoeydi.Rows[0]["fldTypeMohasebe"]);
                    if (Noo_Calc == 0)
                        hoghogh = hoghogh_paye + sanavat + paye + jebhe + janbazi + ashora;
                    else
                        hoghogh = hoghogh_paye;
                    decimal eidi = 0, eidiKargar = 0, eidiKarmand = 0;
                    decimal eidi_kargar = 0, eidi_karmand = 0;
                    decimal zarib = 0;
                    int t_mah = 0, TedadRozKargar = 0, TedadRozKarmand = 0;
                    t_mah = Convert.ToInt32(tbleydi.Rows[0]["fldDayCount"]);
                    zarib = Convert.ToDecimal(tblmoeydi.Rows[0]["fldZaribEydiKargari"]);
                    eidi_kargar = Convert.ToDecimal(tblmoeydi.Rows[0]["fldMaxEydiKargar"]);
                    eidi_karmand = Convert.ToDecimal(tblmoeydi.Rows[0]["fldMaxEydiKarmand"]);
                    //عیدی براساس کارگری و کارمندی
                    commandText1 = "select sum(fldKarkard) as fldKarkard,fldTypeEstekhdamId,max(fldId) as fldId from (	 " +
                      "select distinct  fldMonth, fldKarkard,min(case when t.fldTypeEstekhdamId=3 then 2 else t.fldTypeEstekhdamId end)fldTypeEstekhdamId,m.fldId from pay.tblMohasebat as m " +
                      "inner join pay.tblMohasebat_Items as i on i.fldMohasebatId=m.fldId " +
                      "inner join com.tblItems_Estekhdam as t on t.fldId=i.fldItemEstekhdamId " +
                      "where fldPersonalId=" + code +
                      "and fldYear=" + sal +
                      "group by fldMonth,fldKarkard,m.fldId)t " +
                      "group by fldTypeEstekhdamId";
                    var tblMohasebat = service.GetData(commandText1).Tables[0];
                    if (tblMohasebat.Rows.Count > 1)
                        for (int i = 0; i < tblMohasebat.Rows.Count; i++)
                        {
                            if (Noo_Calc == 0)
                                commandText1 = "  select  sum(fldMablagh) as fldMablagh,fldTedadFarzand,fldStatusTaaholId  from pay.tblMohasebat_PersonalInfo as p " +
                                "inner join prs.tblPersonalHokm as h on h.fldId=p.fldHokmId " +
                                "inner join prs.tblHokm_Item as i on i.fldPersonalHokmId=h.fldId " +
                                "inner join com.tblItems_Estekhdam as e on e.fldid=i.fldItems_EstekhdamId " +
                                "where p.fldMohasebatId=" + Convert.ToInt32(tblMohasebat.Rows[i]["fldId"]) + " and e.fldItemsHoghughiId in (1,2,3,30,31,38) " +
                                " group by fldTedadFarzand,fldStatusTaaholId  ";
                            else
                                commandText1 = "  select  sum(fldMablagh) as fldMablagh,fldTedadFarzand,fldStatusTaaholId from pay.tblMohasebat_PersonalInfo as p " +
                                "inner join prs.tblPersonalHokm as h on h.fldId=p.fldHokmId " +
                                "inner join prs.tblHokm_Item as i on i.fldPersonalHokmId=h.fldId " +
                                "inner join com.tblItems_Estekhdam as e on e.fldid=i.fldItems_EstekhdamId " +
                                "where p.fldMohasebatId=" + Convert.ToInt32(tblMohasebat.Rows[i]["fldId"]) + " and e.fldItemsHoghughiId in (1) group by   fldTedadFarzand,fldStatusTaaholId";
                            var m = service.GetData(commandText1).Tables[0];
                            if (Convert.ToInt32(tblMohasebat.Rows[i]["fldTypeEstekhdamId"]) == 1)
                            {
                                TedadRozKargar = Convert.ToInt32(tblMohasebat.Rows[i]["fldKarkard"]);
                                eidiKargar = Convert.ToInt32(m.Rows[0]["fldMablagh"]) * zarib;
                                if (eidiKargar > eidi_kargar)
                                    eidiKargar = eidi_kargar;
                                eidiKargar = Math.Round((eidiKargar / 365) * TedadRozKargar);
                            }
                            else
                            {
                                TedadRozKarmand = Convert.ToInt32(tblMohasebat.Rows[i]["fldKarkard"]);
                                eidiKarmand = eidi_karmand;
                                if (sal == 1401)
                                {
                                    if (lasthokm.TedadFarzand > 0 && Jensiyat == 1)
                                    {
                                        eidiKarmand = eidiKarmand + (Convert.ToInt32(m.Rows[0]["fldTedadFarzand"]) * 2000000);
                                    }
                                    if (Convert.ToInt32(m.Rows[0]["fldStatusTaaholId"]) > 1)
                                    {
                                        eidiKarmand = eidiKarmand + 5000000;
                                    }
                                    eidiKarmand = Math.Round((eidiKarmand / 365) * TedadRozKarmand);
                                }
                                if (sal == 1402)
                                {
                                    if (lasthokm.TedadFarzand > 0 && Jensiyat == 1)
                                    {
                                        eidiKarmand = eidiKarmand + (Convert.ToInt32(m.Rows[0]["fldTedadFarzand"]) * 3000000);
                                    }
                                    if (Convert.ToInt32(m.Rows[0]["fldStatusTaaholId"]) > 1)
                                    {
                                        eidiKarmand = eidiKarmand + 7000000;
                                    }
                                    eidiKarmand = Math.Round((eidiKarmand / 365) * TedadRozKarmand);
                                }
                            }
                        }
                    //-------------------عیدی
                    t_mah = t_mah - (TedadRozKargar + TedadRozKarmand);
                    if (TypeEstekhdam == 1)
                    {
                        eidi = hoghogh * zarib;
                        eidi = Math.Round((eidi / 365) * t_mah);
                        if (eidi > eidi_kargar)
                            eidi = eidi_kargar;
                    }
                    else
                    {
                        eidi = eidi_karmand;
                        //ابتدای فرمول جدید سال 1400
                        if (sal == 1400)
                        {
                            if (lasthokm.TedadFarzand > 0)
                            {
                                eidi = eidi + (lasthokm.TedadFarzand * 1500000);
                            }
                            if (lasthokm.NoeTaahol > 1)
                            {
                                eidi = eidi + 4000000;
                            }
                        }
                        //انتهای فرمول جدید سال 1400
                        //ابتدای فرمول جدید سال1401
                        else if (sal == 1401 && TypeEstekhdam != 1)
                        {
                            if (lasthokm.TedadFarzand > 0 && Jensiyat == 1)
                            {
                                eidi = eidi + (lasthokm.TedadFarzand * 2000000);
                            }
                            if (lasthokm.NoeTaahol > 1)
                            {
                                eidi = eidi + 5000000;
                            }
                        }
                        //انتهای فرمول جدید سال1401
                        else if (sal == 1402 && TypeEstekhdam != 1)
                        {
                            if (lasthokm.TedadFarzand > 0 && Jensiyat == 1)
                            {
                                eidi = eidi + (lasthokm.TedadFarzand * 3000000);
                            }
                            if (lasthokm.NoeTaahol > 1)
                            {
                                eidi = eidi + 7000000;
                            }
                        }
                        //string commandText2 = "select sum(fldMablagh)as sumhokm from prs.tblHokm_Item where tblHokm_Item.fldPersonalHokmId=" + lasthokm.HokmId + " and fldItems_EstekhdamId not in(85,86,150,151,152,153,154)";
                        //var tblhokm = service.GetData(commandText2).Tables[0];
                        //eidi = eidi + Convert.ToDecimal(tblhokm.Rows[0]["sumhokm"]);
                        eidi = Math.Round((eidi / 365) * t_mah);
                    }
                    //-------------------مشمولی مالیات
                    //SqlConnection con_maliat = new SqlConnection(Program.connectiontext);
                    decimal mashmol_maliyat = 0, mashmol_maliyatKargar = 0, mashmol_maliyatKarmand = 0;
                    mashmol_maliyat = eidi;
                    //--------------------مالیات
                    decimal maliyat = 0, maliyatKargar = 0, maliyatKarmand = 0;
                    if (Noo_Maliat == 1)
                    {
                        //decimal darsad_maliyat = 0, d_karmand = 0;
                        //decimal zarib_maliyat = 0, az_mablagh_k = 0, az_mablagh = 0;
                        //SqlCommand com_maliat = new SqlCommand("select * from maliyat where [group]=" + this.group_maliat.ToString(), con_maliat);
                        //con_maliat.Open();
                        //SqlDataReader read_maliat = com_maliat.ExecuteReader();
                        //while (read_maliat.Read())
                        //{
                        //    if (mashmol_maliyat <= Convert.ToDecimal(read_maliat["saghfhoghogh"]) && mashmol_maliyat >= Convert.ToDecimal(read_maliat["as_hoghogh"]))
                        //    {
                        //        darsad_maliyat = Convert.ToDecimal(read_maliat["darsadmaliyat"]);
                        //        d_karmand = Convert.ToDecimal(read_maliat["d_karmand"]);
                        //        zarib_maliyat = Convert.ToDecimal(read_maliat["sabet"]);
                        //        az_mablagh = Convert.ToDecimal(read_maliat["as_hoghogh"]);
                        //    }
                        //}
                        //con_maliat.Close();
                        //com_maliat = new SqlCommand("select * from maliyat where [group]=" + this.group_maliat.ToString(), con_maliat);
                        //con_maliat.Open();
                        //read_maliat = com_maliat.ExecuteReader();
                        //if (az_mablagh != 0)
                        //{
                        //    while (read_maliat.Read())
                        //    {
                        //        if (Convert.ToDecimal(read_maliat["as_hoghogh"]) < az_mablagh)
                        //        {
                        //            az_mablagh_k = Convert.ToDecimal(read_maliat["saghfhoghogh"]);
                        //            if (estekhdam == 7 || estekhdam == 6 || estekhdam == 10)
                        //            {
                        //                zarib_maliyat = 0;
                        //                break;
                        //            }
                        //        }
                        //    }
                        //}
                        //if (estekhdam == 7 || estekhdam == 6 || estekhdam == 10)
                        //    darsad_maliyat = d_karmand;
                        //if (p.vaziyat_jesmi == 1)
                        //{
                        //    maliyat = Math.Round(((mashmol_maliyat - az_mablagh_k) * Convert.ToDecimal(darsad_maliyat)) / 100, 0) + zarib_maliyat;
                        //}
                        //else
                        //    maliyat = 0;
                    }
                    else
                    {
                        decimal moaf = 0, moafKargar = 0, moafKarmand = 0;
                        moafKargar = Convert.ToDecimal(tblmoeydi.Rows[0]["fldMablaghMoafiatKargar"]);
                        moafKarmand = Convert.ToDecimal(tblmoeydi.Rows[0]["fldMablaghMoafiatKarmand"]);
                        if (TypeEstekhdam == 1)
                        {
                            moaf = Convert.ToDecimal(tblmoeydi.Rows[0]["fldMablaghMoafiatKargar"]);
                        }
                        else
                        {
                            moaf = Convert.ToDecimal(tblmoeydi.Rows[0]["fldMablaghMoafiatKarmand"]);
                        }
                        float darsad_karmandi = 0, darsad_kargari = 0;
                        darsad_kargari = Convert.ToSingle(tblmoeydi.Rows[0]["fldDarsadMaliyatKargar"]);
                        darsad_karmandi = Convert.ToSingle(tblmoeydi.Rows[0]["fldDarsadMaliyatKarmand"]);
                        //if (eidi > moaf)
                        if (TypeEstekhdam == 1)
                        {
                            mashmol_maliyat = (eidi + eidiKargar) - moaf;
                            mashmol_maliyatKarmand = eidiKarmand - moafKarmand;
                        }
                        else
                        {
                            mashmol_maliyat = (eidi + eidiKarmand) - moaf;
                            mashmol_maliyatKargar = eidiKargar - moafKargar;
                        }
                        //mashmol_maliyatKargar = eidiKargar - moafKargar;
                        //mashmol_maliyatKarmand = eidiKarmand - moafKarmand;
                        if ((fldMoafAsMaliyat == false))
                        {
                            if (mashmol_maliyat > 0)
                            {
                                if (TypeEstekhdam == 1)
                                    maliyat = Math.Round(((mashmol_maliyat) * Convert.ToDecimal(darsad_kargari)) / 100, 0);
                                else
                                    maliyat = Math.Round(((mashmol_maliyat) * Convert.ToDecimal(darsad_karmandi)) / 100, 0);
                            }
                            else
                                mashmol_maliyat = 0;
                            if (mashmol_maliyatKargar > 0)
                                maliyatKargar = Math.Round((mashmol_maliyatKargar * Convert.ToDecimal(darsad_kargari)) / 100, 0);
                            else
                                mashmol_maliyatKargar = 0;
                            if (mashmol_maliyatKarmand > 0)
                                maliyatKarmand = Math.Round((mashmol_maliyatKarmand * Convert.ToDecimal(darsad_karmandi)) / 100, 0);
                            else
                                mashmol_maliyatKarmand = 0;
                        }
                        else
                        {
                            maliyat = 0;
                            mashmol_maliyatKargar = 0;
                            mashmol_maliyatKarmand = 0;
                        }
                    }
                    //--------------------سایر کسورات
                    decimal kosorat = 0;
                    kosorat = Convert.ToDecimal(tbleydi.Rows[0]["fldKosurat"]);
                    mohasebe.fldMashmolMaliyat = (int)(mashmol_maliyat + mashmol_maliyatKargar + mashmol_maliyatKarmand);
                    mohasebe.fldMaliyat = (int)(maliyat + maliyatKargar + maliyatKarmand);
                    mohasebe.fldMablagh = (int)(eidi + eidiKargar + eidiKarmand);
                    mohasebe.fldDayCount = t_mah + TedadRozKargar + TedadRozKarmand;
                    mohasebe.fldMablaghKosorat = (int)kosorat;
                    //--------------------خالص پرداختی
                    decimal khales_pardakhti = 0;
                    khales_pardakhti = (eidi + eidiKargar + eidiKarmand) - mohasebe.fldMaliyat - mohasebe.fldMablaghKosorat;
                    mohasebe.fldkhalesPardakhti = (int)khales_pardakhti;
                }
            }
            catch (Exception x)
            {
                DateTime d1;
                d1 = DateTime.Now;
                string excep = "";
                if (x.InnerException != null)
                    excep = "'eydi " + x.InnerException.Message + "'";
                else
                    excep = "'eydi " + x.Message + "'";
                string commandText1 = "exec [Com].[spr_tblErrorInsert] 	0,'admin'," + excep + ",'" + d1.ToString() + "',1,1,''";
                var _Hokm1 = service.GetData(commandText1).Tables[0];
            }
        }
        private bool havemoavaghe(int payId, int sal, int mah)
        {
            bool result = false;
            string commandText = "select isnull(max(Pay.tblMoavaghat.fldHokmId),0) as id FROM Pay.tblMoavaghat INNER JOIN Pay.tblMohasebat" +
              " ON Pay.tblMoavaghat.fldMohasebatId = Pay.tblMohasebat.fldId WHERE tblMoavaghat.fldYear=" + sal + " AND tblMoavaghat.fldMonth=" + mah
              + " AND Pay.tblMohasebat.fldPersonalId=" + payId;
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var tblPersonalHokm = service.GetData(commandText).Tables[0];
            int id_moavaghe = 0, id_hokm = 0;
            if (tblPersonalHokm.Rows.Count > 0)
            {
                if (Convert.ToInt32(tblPersonalHokm.Rows[0]["id"].ToString()) > 0)
                {
                    id_moavaghe = (int)tblPersonalHokm.Rows[0]["id"];
                    id_hokm = end_dateofhokm(sal, mah, payId);
                    if (id_moavaghe == id_hokm)
                        result = false;
                    else
                        result = true;
                }
                else
                {
                    commandText = "SELECT Pay.tblMohasebat_PersonalInfo.fldHokmId FROM Pay.tblMohasebat INNER " +
                      "JOIN Pay.tblMohasebat_PersonalInfo ON Pay.tblMohasebat.fldId = Pay.tblMohasebat_PersonalInfo.fldMohasebatId" +
                      " WHERE fldYear=" + sal + " AND fldMonth=" + mah + " AND fldPersonalId=" + payId;
                    NewFMS.FormulaService.WebServiceFormula service1 = new NewFMS.FormulaService.WebServiceFormula();
                    var tblPersonalHokm1 = service.GetData(commandText).Tables[0];
                    if (tblPersonalHokm1.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(tblPersonalHokm1.Rows[0]["fldHokmId"].ToString()) > 0)
                        {
                            id_moavaghe = (int)tblPersonalHokm1.Rows[0]["fldHokmId"];
                            id_hokm = end_dateofhokm(sal, mah, payId);
                            if (id_moavaghe == id_hokm)
                                result = false;
                            else
                                result = true;
                        }
                        else
                            result = false;
                    }
                }
            }
            return result;
        }
        public int end_dateofhokm(int _sal, int _mah, int _code)
        {
            string commandText = "SELECT   top(1)     fldId, fldTarikhEjra, fldTarikhSodoor, fldAnvaeEstekhdamId, fldStatusTaaholId, fldTedadFarzand,fldTedadAfradTahteTakafol " +
              "FROM  Prs.tblPersonalHokm " +
              " WHERE fldPrs_PersonalInfoId=" + _code + " and  fldTarikhEjra<='" + _sal + "/" + _mah.ToString().PadLeft(2, '0') + "/31' and fldStatusHokm=1 " +
              " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var tblPersonalHokm = service.GetData(commandText).Tables[0];
            int s = 0;
            if (tblPersonalHokm.Rows.Count > 0)
            {
                s = (int)tblPersonalHokm.Rows[0]["fldid"];
            }
            return s;
        }
        public Hokm getHokmByid(int id)
        {
            string commandText = "SELECT * FROM  Prs.tblPersonalHokm WHERE fldid=" + id;
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var tblPersonalHokm = service.GetData(commandText).Tables[0];
            Hokm hokm = new Hokm();
            if (tblPersonalHokm.Rows.Count > 0)
            {
                commandText = "SELECT        fldItems_EstekhdamId,fldMablagh,fldItemsHoghughiId FROM            Prs.tblHokm_Item INNER JOIN" +
                  " Com.tblItems_Estekhdam ON Prs.tblHokm_Item.fldItems_EstekhdamId = Com.tblItems_Estekhdam.fldId " +
                  "WHERE fldPersonalHokmId=" + Convert.ToInt32(tblPersonalHokm.Rows[0]["fldId"]);
                var Hokm_Item = service.GetData(commandText).Tables[0];
                hokm.HokmId = Convert.ToInt32(tblPersonalHokm.Rows[0]["fldId"]);
                hokm.NoeEstekhdam = Convert.ToInt32(tblPersonalHokm.Rows[0]["fldAnvaeEstekhdamId"]);
                hokm.NoeTaahol = Convert.ToInt32(tblPersonalHokm.Rows[0]["fldStatusTaaholId"]);
                hokm.TarikhEjra = (tblPersonalHokm.Rows[0]["fldTarikhEjra"]).ToString();
                hokm.TarikhSodur = (tblPersonalHokm.Rows[0]["fldTarikhSodoor"]).ToString();
                hokm.TedadAfradTakafol = Convert.ToInt32(tblPersonalHokm.Rows[0]["fldTedadAfradTahteTakafol"]);
                hokm.TedadFarzand = Convert.ToInt32(tblPersonalHokm.Rows[0]["fldTedadFarzand"]);
                if (Hokm_Item.Rows.Count > 0)
                {
                    for (int i = 0; i < Hokm_Item.Rows.Count; i++)
                    {
                        hokm.Items.Add(new HokmItems { ItemEstekhdamId = Convert.ToInt32(Hokm_Item.Rows[i]["fldItems_EstekhdamId"]), ItemHoghughiId = Convert.ToInt32(Hokm_Item.Rows[i]["fldItemsHoghughiId"]), Mablagh = Convert.ToInt32(Hokm_Item.Rows[i]["fldMablagh"]) });
                    }
                }
            }
            return hokm;
        }
        //     public void m_Moavaghe(int sal, int mah, int payId, Hokm newHokm, MoteghayerhayeHoghoghi newMoteghayer,
        //                            Fiscal newFiscal, PersonalInfo PersonalInfo, int prsId, int anvaEstekhdam, int typebime, int typeEstekhdam)
        //     {
        //       try
        //       {
        //         Mohasebat cha = new Mohasebat();
        //         Mohasebat chb = new Mohasebat();
        //         Mohasebat chc = new Mohasebat();
        //         cha = mohasebe;
        //         decimal h_d_k = 0;
        //         decimal h_d_p = 0;
        //         decimal b_p = 0, b_k = 0, b_b = 0;
        //         decimal old_mazad = 0, old_takafol = 0;
        //         decimal new_mazad = 0, new_takafol = 0;
        //         bool mazad_30sal = false;
        //         int vaziat_jesmi = 0;
        //         string commandText = "SELECT Pay.tblMohasebat_Items.fldMablagh, Com.tblItems_Estekhdam.fldItemsHoghughiId, Com.tblItems_Estekhdam.fldId AS fldItems_EstekhdamId " +
        //           "FROM Pay.tblMohasebat INNER JOIN Pay.tblMohasebat_Items ON Pay.tblMohasebat.fldId = Pay.tblMohasebat_Items.fldMohasebatId INNER JOIN" +
        //           " Com.tblItems_Estekhdam ON Pay.tblMohasebat_Items.fldItemEstekhdamId = Com.tblItems_Estekhdam.fldId where" +
        //           " Pay.tblMohasebat.fldPersonalId=" + payId + " and Pay.tblMohasebat.fldYear=" + sal + " and Pay.tblMohasebat.fldMonth=" + mah;
        //         NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
        //         var tblMohasebe = service.GetData(commandText).Tables[0];
        //         int s = 0;
        //         if (tblMohasebe.Rows.Count > 0)
        //         {
        //           for (int i = 0; i < tblMohasebe.Rows.Count; i++)
        //           {
        //             chb.Items.Add(new Mohasebat_Items
        //                           {
        //               fldItemEstekhdamId = Convert.ToInt32(tblMohasebe.Rows[i]["fldItems_EstekhdamId"]),
        //               fldItemHoghoghiId = Convert.ToInt32(tblMohasebe.Rows[i]["fldItemsHoghughiId"]),
        //               fldMablagh = Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"])
        //             });
        //           }
        //           string com = "select * from Com.tblItems_Estekhdam";
        //           var mm = service.GetData(com).Tables[0];
        //           for (int i = 0; i < mm.Rows.Count; i++)
        //           {
        //             var m1 = chb.Items.Where(k => k.fldItemEstekhdamId == (int)mm.Rows[i]["fldId"]).FirstOrDefault();
        //             if (m1 == null)
        //               chb.Items.Add(new Mohasebat_Items
        //                             {
        //                 fldItemEstekhdamId = (int)mm.Rows[i]["fldId"],
        //                 fldItemHoghoghiId = (int)mm.Rows[i]["fldItemsHoghughiId"],
        //                 fldMablagh = 0
        //               });
        //             var m2 = cha.Items.Where(k => k.fldItemEstekhdamId == (int)mm.Rows[i]["fldId"]).FirstOrDefault();
        //             if (m2 == null)
        //               cha.Items.Add(new Mohasebat_Items
        //                             {
        //                 fldItemEstekhdamId = (int)mm.Rows[i]["fldId"],
        //                 fldItemHoghoghiId = (int)mm.Rows[i]["fldItemsHoghughiId"],
        //                 fldMablagh = 0
        //               });
        //           }
        //           commandText = "SELECT Pay.tblMohasebat_PersonalInfo.fldMazad30Sal, Pay.tblMohasebat_PersonalInfo.fldStatusIsargariId,Pay.tblMohasebat_PersonalInfo.fldAnvaEstekhdamId, Pay.tblMohasebat_PersonalInfo.fldHokmId, " +
        //             "Pay.tblMohasebat_PersonalInfo.fldFiscalHeaderId,pay.tblMohasebat.fldMaliyat,pay.tblMohasebat.fldMashmolBime,pay.tblMohasebat.fldBimePersonal,pay.tblMohasebat.fldBimeKarFarma,pay.tblMohasebat.fldBimeBikari,"+
        //             "pay.tblMohasebat.fldMashmolMaliyat, Pay.tblMohasebat_PersonalInfo.fldMoteghayerHoghoghiId,pay.tblMohasebat.fldHaghDarman,pay.tblMohasebat.fldHaghDarmanKarfFarma,pay.tblMohasebat.fldHaghDarmanDolat,pay.tblMohasebat.fldPasAndaz FROM Pay.tblMohasebat " +
        //             "INNER JOIN Pay.tblMohasebat_PersonalInfo ON Pay.tblMohasebat.fldId = Pay.tblMohasebat_PersonalInfo.fldMohasebatId WHERE " +
        //             "Pay.tblMohasebat.fldPersonalId=" + payId + " and Pay.tblMohasebat.fldYear=" + sal + " and Pay.tblMohasebat.fldMonth=" + mah;
        //           var tblMohasebe1 = service.GetData(commandText).Tables[0];
        //           mazad_30sal = Convert.ToBoolean(tblMohasebe1.Rows[0]["fldMazad30Sal"]);
        //           vaziat_jesmi = Convert.ToInt32(tblMohasebe1.Rows[0]["fldStatusIsargariId"]);
        //           int _group = Convert.ToInt32(tblMohasebe1.Rows[0]["fldFiscalHeaderId"]);
        //           int old_moteghayerid = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMoteghayerHoghoghiId"]);
        //           int old_hokmid = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHokmId"]);
        //           commandText = "SELECT     fldHaghDarmanMazad, fldHaghDarmanTahteTakaffol FROM         Pay.tblMoteghayerhayeHoghoghi WHERE     (fldId = " + old_moteghayerid + ")";
        //           var oldMoteghayer = service.GetData(commandText).Tables[0];
        //           if (oldMoteghayer.Rows.Count > 0)
        //           {
        //             var oldhokm = getHokmByid(old_hokmid);
        //             int tedad_afradekhanevade = 0, t_takafol_60 = 0, t_takafol_70 = 0, t_takafol_60_t = 0, t_takafol_70_t = 0;
        //             switch (oldhokm.NoeTaahol)
        //             {
        //               case 1:
        //                 tedad_afradekhanevade = 1;
        //                 break;
        //               case 2:
        //                 tedad_afradekhanevade = 1 + oldhokm.TedadFarzand;
        //                 break;
        //               case 3:
        //                 tedad_afradekhanevade = 2;
        //                 break;
        //               case 4:
        //                 tedad_afradekhanevade = 2 + oldhokm.TedadFarzand;
        //                 break;
        //             }
        //             t_takafol_60 = oldhokm.TedadAfradTakafol;
        //             old_mazad = Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanMazad"]);
        //             old_takafol = Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanTahteTakaffol"]);
        //             int tedad_t_takalof = 0;
        //             int ezafe_farzand = 0;
        //             if (PersonalInfo.fldHamsarKarmand == true)
        //               tedad_afradekhanevade = tedad_afradekhanevade - 1;
        //             tedad_t_takalof = t_takafol_60 + t_takafol_70;
        //             if (anvaEstekhdam <= 10 && anvaEstekhdam >= 1 && typebime == 2)
        //             {
        //               if (oldhokm.TedadFarzand > 3 && PersonalInfo.fldJensiyat == 1)
        //               {
        //                 if (sal >= 1392 && mah >= 9)
        //                   old_mazad = 0;
        //                 else if (sal > 1392)
        //                   old_mazad = 0;
        //                 else
        //                 {
        //                   ezafe_farzand = oldhokm.TedadFarzand - 3;
        //                   old_mazad = old_mazad * Convert.ToDecimal(ezafe_farzand);
        //                 }
        //               }
        //               else if (PersonalInfo.fldJensiyat == 2)//------تغییرات
        //               {//------تغییرات
        //                 ezafe_farzand = tedad_afradekhanevade - 1;//------تغییرات
        //                 old_mazad = old_mazad * Convert.ToDecimal(ezafe_farzand);//------تغییرات
        //               }//------تغییرات
        //               else
        //                 old_mazad = 0;
        //               commandText = "select count(*) as tedad from [Prs].[tblAfradTahtePooshesh]  where fldNesbat=4 and [fldPersonalId]=" + prsId;
        //               var Tabae = service.GetData(commandText).Tables[0];
        //               if (Tabae.Rows.Count > 0)
        //               {
        //                 old_mazad += Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanMazad"]) * Convert.ToDecimal(Tabae.Rows[0]["tedad"]);
        //               }
        //               old_takafol = old_takafol * Convert.ToDecimal(tedad_t_takalof);
        //             }
        //             else
        //             {
        //               old_takafol = old_mazad = 0;
        //             }
        //           }
        //           if (newMoteghayer != null)
        //           {
        //             int tedad_afradekhanevade = 0, t_takafol_60 = 0, t_takafol_70 = 0, t_takafol_60_t = 0, t_takafol_70_t = 0;
        //             switch (newHokm.NoeTaahol)
        //             {
        //               case 1:
        //                 tedad_afradekhanevade = 1;
        //                 break;
        //               case 2:
        //                 tedad_afradekhanevade = 1 + newHokm.TedadFarzand;
        //                 break;
        //               case 3:
        //                 tedad_afradekhanevade = 2;
        //                 break;
        //               case 4:
        //                 tedad_afradekhanevade = 2 + newHokm.TedadFarzand;
        //                 break;
        //             }
        //             t_takafol_60 = newHokm.TedadAfradTakafol;
        //             new_mazad = Convert.ToDecimal(newMoteghayer.fldHaghDarmanMazad);
        //             new_takafol = Convert.ToDecimal(newMoteghayer.fldHaghDarmanTahteTakaffol);
        //             int tedad_t_takalof = 0;
        //             int ezafe_farzand = 0;
        //             if (PersonalInfo.fldHamsarKarmand == true)
        //               tedad_afradekhanevade = tedad_afradekhanevade - 1;
        //             tedad_t_takalof = t_takafol_60 + t_takafol_70;
        //             if (anvaEstekhdam <= 10 && anvaEstekhdam >= 1 && typebime == 2)
        //             {
        //               if (newHokm.TedadFarzand > 3 && PersonalInfo.fldJensiyat == 1)
        //               {
        //                 if (sal >= 1392 && mah >= 9)
        //                   new_mazad = 0;
        //                 else if (sal > 1392)
        //                   new_mazad = 0;
        //                 else
        //                 {
        //                   ezafe_farzand = newHokm.TedadFarzand - 3;
        //                   new_mazad = new_mazad * Convert.ToDecimal(ezafe_farzand);
        //                 }
        //               }
        //               else if (PersonalInfo.fldJensiyat == 2)//------تغییرات
        //               {//------تغییرات
        //                 ezafe_farzand = tedad_afradekhanevade - 1;//------تغییرات
        //                 new_mazad = new_mazad * Convert.ToDecimal(ezafe_farzand);//------تغییرات
        //               }//------تغییرات
        //               else
        //                 new_mazad = 0;
        //               commandText = "select count(*) as tedad from [Prs].[tblAfradTahtePooshesh]  where fldNesbat=4 and [fldPersonalId]=" + prsId;
        //               var Tabae = service.GetData(commandText).Tables[0];
        //               if (Tabae.Rows.Count > 0)
        //               {
        //                 new_mazad += Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanMazad"]) * Convert.ToDecimal(Tabae.Rows[0]["tedad"]);
        //               }
        //               new_takafol = new_takafol * Convert.ToDecimal(tedad_t_takalof);
        //             }
        //             else
        //             {
        //               new_takafol = new_mazad = 0;
        //             }
        //           }
        //           chb.fldMashmolMaliyat = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMashmolMaliyat"]);
        //           //--------------------مالیات  
        //           chb.fldMaliyat = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMaliyat"]); //maliyat;
        //           //mashmol_bime = mashmol_bime - hog.mogharari;
        //           //--------------------حق بیمه
        //           chb.fldMashmolBime = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMashmolBime"]);
        //           chb.fldBimeBikari = Convert.ToInt32(tblMohasebe1.Rows[0]["fldBimeBikari"]);
        //           chb.fldBimeKarFarma = Convert.ToInt32(tblMohasebe1.Rows[0]["fldBimeKarFarma"]);
        //           chb.fldBimePersonal = Convert.ToInt32(tblMohasebe1.Rows[0]["fldBimePersonal"]);
        //           chb.fldHaghDarman = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHaghDarman"]);
        //           chb.fldHaghDarmanKarfFarma = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHaghDarmanKarfFarma"]);
        //           chb.fldHaghDarmanDolat = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHaghDarmanDolat"]);
        //           chb.fldPasAndaz = Convert.ToInt32(tblMohasebe1.Rows[0]["fldPasAndaz"]);
        //         }
        //         commandText = "SELECT isnull(sum(Pay.tblMoavaghat.fldHaghDarmanKarfFarma),0)as fldHaghDarmanKarfFarma, " +
        //           "isnull(sum(Pay.tblMoavaghat.fldHaghDarmanDolat),0)as fldHaghDarmanDolat, isnull(sum(Pay.tblMoavaghat.fldHaghDarman),0)" +
        //           "as fldHaghDarman, isnull(sum(Pay.tblMoavaghat.fldBimePersonal),0)as fldBimePersonal, isnull(sum(Pay.tblMoavaghat.fldBimeKarFarma)" +
        //           ",0)as fldBimeKarFarma, isnull(sum(Pay.tblMoavaghat.fldBimeBikari),0)as fldBimeBikari, isnull(sum(Pay.tblMoavaghat.fldBimeMashaghel),0)as" +
        //           " fldBimeMashaghel, isnull(sum(Pay.tblMoavaghat.fldPasAndaz),0)as fldPasAndaz, isnull(sum(Pay.tblMoavaghat.fldMashmolBime),0)" +
        //           "as fldMashmolBime, isnull(sum(Pay.tblMoavaghat.fldMashmolMaliyat),0)as fldMashmolMaliyat, isnull(sum(Pay.tblMoavaghat.fldMaliyat),0)as" +
        //           " fldMaliyat FROM Pay.tblMoavaghat INNER JOIN Pay.tblMohasebat ON Pay.tblMoavaghat.fldMohasebatId = Pay.tblMohasebat.fldId " +
        //           "WHERE (Pay.tblMoavaghat.fldYear = " + sal + ") AND (Pay.tblMoavaghat.fldMonth = " + mah + ") AND (Pay.tblMohasebat.fldPersonalId = " + payId + ")";
        //         var mov = service.GetData(commandText).Tables[0];
        //         if (mov.Rows.Count > 0)
        //         {
        //           chc.fldHaghDarman = Convert.ToInt32(mov.Rows[0]["fldHaghDarman"]);
        //           chc.fldHaghDarmanKarfFarma = Convert.ToInt32(mov.Rows[0]["fldHaghDarmanKarfFarma"]);
        //           chc.fldHaghDarmanDolat = Convert.ToInt32(mov.Rows[0]["fldHaghDarmanDolat"]);
        //           chc.fldBimeKarFarma = Convert.ToInt32(mov.Rows[0]["fldBimeKarFarma"]);
        //           chc.fldBimePersonal = Convert.ToInt32(mov.Rows[0]["fldBimePersonal"]);
        //           chc.fldBimeBikari = Convert.ToInt32(mov.Rows[0]["fldBimeBikari"]);
        //           chc.fldMaliyat = Convert.ToInt32(mov.Rows[0]["fldMaliyat"]);
        //           chc.fldMashmolBime = Convert.ToInt32(mov.Rows[0]["fldMashmolBime"]);
        //           chc.fldMashmolMaliyat = Convert.ToInt32(mov.Rows[0]["fldMashmolMaliyat"]);
        //           chc.fldPasAndaz = Convert.ToInt32(mov.Rows[0]["fldPasAndaz"]);
        //           commandText = "SELECT Pay.tblMoavaghat_Items.fldItemEstekhdamId, SUM(Pay.tblMoavaghat_Items.fldMablagh) AS fldMablagh," +
        //             " Com.tblItems_Estekhdam.fldItemsHoghughiId FROM Pay.tblMoavaghat_Items INNER JOIN Pay.tblMoavaghat INNER JOIN " +
        //             "Pay.tblMohasebat ON Pay.tblMoavaghat.fldMohasebatId = Pay.tblMohasebat.fldId ON Pay.tblMoavaghat_Items.fldMoavaghatId =" +
        //             " Pay.tblMoavaghat.fldId INNER JOIN Com.tblItems_Estekhdam ON Pay.tblMoavaghat_Items.fldItemEstekhdamId = Com.tblItems_Estekhdam.fldId WHERE " +
        //             "(Pay.tblMoavaghat.fldYear = " + sal + ") AND (Pay.tblMoavaghat.fldMonth = " + mah + ") AND (Pay.tblMohasebat.fldPersonalId = " + payId + ") " +
        //             "GROUP BY Pay.tblMoavaghat_Items.fldItemEstekhdamId, Com.tblItems_Estekhdam.fldItemsHoghughiId ORDER BY Com.tblItems_Estekhdam.fldItemsHoghughiId";
        //           var mov_items = service.GetData(commandText).Tables[0];
        //           if (mov_items.Rows.Count > 0)
        //           {
        //             for (int i = 0; i < mov_items.Rows.Count; i++)
        //             {
        //               chc.Items.Add(new Mohasebat_Items
        //                             {
        //                 fldItemEstekhdamId = Convert.ToInt32(mov_items.Rows[i]["fldItemEstekhdamId"]),
        //                 fldItemHoghoghiId = Convert.ToInt32(mov_items.Rows[i]["fldItemsHoghughiId"]),
        //                 fldMablagh = Convert.ToInt32(mov_items.Rows[i]["fldMablagh"])
        //               });
        //             }
        //             string com = "select * from Com.tblItems_Estekhdam";
        //             var mm = service.GetData(com).Tables[0];
        //             for (int i = 0; i < mm.Rows.Count; i++)
        //             {
        //               var m1 = chc.Items.Where(k => k.fldItemEstekhdamId == (int)mm.Rows[i]["fldId"]).FirstOrDefault();
        //               if (m1 == null)
        //                 chc.Items.Add(new Mohasebat_Items
        //                               {
        //                   fldItemEstekhdamId = (int)mm.Rows[i]["fldId"],
        //                   fldItemHoghoghiId = (int)mm.Rows[i]["fldItemsHoghughiId"],
        //                   fldMablagh = 0
        //                 });
        //             }
        //           }
        //         }
        //         mohasebe1.Movaghat.Add(new Movaghat
        //                                {
        //           fldBimeBikari = cha.fldBimeBikari - (chb.fldBimeBikari + chc.fldBimeBikari),
        //           fldBimeKarFarma = cha.fldBimeKarFarma - (chb.fldBimeKarFarma + chc.fldBimeKarFarma),
        //           fldBimePersonal = cha.fldBimePersonal - (chb.fldBimePersonal + chc.fldBimePersonal),
        //           fldHaghDarman = cha.fldHaghDarman - (chb.fldHaghDarman + chc.fldHaghDarman),
        //           fldHaghDarmanDolat = cha.fldHaghDarmanDolat - (chb.fldHaghDarmanDolat + chc.fldHaghDarmanDolat),
        //           fldHaghDarmanKarfFarma = cha.fldHaghDarmanKarfFarma - (chb.fldHaghDarmanKarfFarma + chc.fldHaghDarmanKarfFarma),
        //           fldMaliyat = cha.fldMaliyat - (chb.fldMaliyat + chc.fldMaliyat),
        //           fldMashmolBime = cha.fldMashmolBime - (chb.fldMashmolBime + chc.fldMashmolBime),
        //           fldMashmolMaliyat = cha.fldMashmolMaliyat - (chb.fldMashmolMaliyat + chc.fldMashmolMaliyat),
        //           fldPasAndaz = cha.fldPasAndaz - (chb.fldPasAndaz + chc.fldPasAndaz),
        //           fldMonth = mah,
        //           fldYear = sal
        //         });
        //         foreach (var item in cha.Items)
        //         {
        //           var chbitem = chb.Items.Where(k => k.fldItemEstekhdamId == item.fldItemEstekhdamId && k.fldItemHoghoghiId == item.fldItemHoghoghiId).FirstOrDefault();
        //           var chcitem = chc.Items.Where(k => k.fldItemEstekhdamId == item.fldItemEstekhdamId && k.fldItemHoghoghiId == item.fldItemHoghoghiId).FirstOrDefault();
        //           int moavagh=0;
        //           if(chcitem!=null)
        //             moavagh = item.fldMablagh - (chbitem.fldMablagh + chcitem.fldMablagh);
        //           else
        //             moavagh=item.fldMablagh - (chbitem.fldMablagh);
        //           if (moavagh != 0)
        //             mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
        //                                                                        {
        //               fldItemEstekhdamId = item.fldItemEstekhdamId,
        //               fldItemHoghoghiId = item.fldItemHoghoghiId,
        //               fldMablagh = moavagh
        //             });
        //         }
        //         //ch.hoghogh = cha.hoghogh - (chb.hoghogh + chc.hoghogh);
        //         //ch.pasandaz = cha.pasandaz * 2 - (chb.pasandaz + chc.pasandaz);
        //         //ch.sanavat = cha.sanavat - (chb.sanavat + chc.sanavat);
        //         //ch.fogh_shoghl = cha.fogh_shoghl - (chb.fogh_shoghl + chc.fogh_shoghl);
        //         //ch.tafavot_tatbigh = cha.tafavot_tatbigh - (chb.tafavot_tatbigh + chc.tafavot_tatbigh);
        //         //ch.hadaghalTadil = cha.hadaghalTadil - (chb.hadaghalTadil + chc.hadaghalTadil);
        //         //ch.hagholad = cha.hagholad - (chb.hagholad + chc.hagholad);
        //         //ch.arzeshyabi = cha.arzeshyabi - (chb.arzeshyabi + chc.arzeshyabi);
        //         //ch.ayelemandi = cha.ayelemandi - (chb.ayelemandi + chc.ayelemandi);
        //         //ch.nobatkari = cha.nobatkari - (chb.nobatkari + chc.nobatkari);
        //         //ch.maskan = cha.maskan - (chb.maskan + chc.maskan);
        //         //ch.sakhtikar = cha.sakhtikar - (chb.sakhtikar + chc.sakhtikar);
        //         //ch.kharobar = cha.kharobar - (chb.kharobar + chc.kharobar);
        //         //ch.jazb = cha.jazb - (chb.jazb + chc.jazb);
        //         //ch.jazbTakhasosi = cha.jazbTakhasosi - (chb.jazbTakhasosi + chc.jazbTakhasosi);
        //         //ch.zajbTadil = cha.zajbTadil - (chb.zajbTadil + chc.zajbTadil);
        //         //ch.tashilatzendegi = cha.tashilatzendegi - (chb.tashilatzendegi + chc.tashilatzendegi);
        //         //ch.badiabohava = cha.badiabohava - (chb.badiabohava + chc.badiabohava);
        //         //ch.tarmim = cha.tarmim - (chb.tarmim + chc.tarmim);
        //         //ch.mashmol_bime = cha.mashmol_bime - (chb.mashmol_bime + chc.mashmol_bime);
        //         //ch.darsad_p = cha.darsad_p;
        //         //ch.darsad_k = cha.darsad_k;
        //         //if ((vaziat_jesmi == 1 || vaziat_jesmi == 4) && mazad_30sal == false)
        //         //{
        //         //    ch.bime_karfarma = Math.Round(ch.mashmol_bime * ch.darsad_k / 100);
        //         //    ch.bime_persenel = Math.Round((ch.mashmol_bime) * ch.darsad_p / 100);
        //         //    ch.bime_bikari = Math.Round(ch.mashmol_bime * b_b / 100);
        //         //}
        //         //else if (mazad_30sal == true && (vaziat_jesmi == 1 || vaziat_jesmi == 4))
        //         //{
        //         //    if (p.nobime == 2)
        //         //    {
        //         //        ch.bime_karfarma = Math.Round(ch.mashmol_bime * ch.darsad_k / 100);
        //         //        ch.bime_persenel = 0;
        //         //        ch.bime_bikari = Math.Round(ch.mashmol_bime * b_b / 100);
        //         //    }
        //         //    else
        //         //    {
        //         //        ch.bime_karfarma = Math.Round(ch.mashmol_bime * ch.darsad_k / 100);
        //         //        ch.bime_persenel = Math.Round((ch.mashmol_bime) * ch.darsad_p / 100);
        //         //        ch.bime_bikari = Math.Round(ch.mashmol_bime * b_b / 100);
        //         //    }                
        //         //}
        //         //else if (vaziat_jesmi == 2 ||vaziat_jesmi == 3 || vaziat_jesmi == 5)
        //         //{
        //         //    ch.bime_karfarma = Math.Round(ch.mashmol_bime * ch.darsad_k / 100);
        //         //    ch.bime_persenel = 0;
        //         //    ch.bime_bikari = Math.Round(ch.mashmol_bime * b_b / 100);
        //         //}
        //         //ch.bime_karfarma = cha.bime_karfarma - (chb.bime_karfarma + chc.bime_karfarma);
        //         //ch.bime_persenel = cha.bime_persenel - (chb.bime_persenel + chc.bime_persenel);
        //         //ch.bime_bikari = cha.bime_bikari - (chb.bime_bikari + chc.bime_bikari);
        //         //ch.maliyat = cha.maliyat - (chb.maliyat + chc.maliyat);
        //         //ch.ezafekari = cha.ezafekari - (chb.ezafekari + chc.ezafekari);
        //         //ch.s_ezafekari = cha.s_ezafekari;
        //         //ch.sayer_fogholadeha = cha.sayer_fogholadeha - (chb.sayer_fogholadeha + chc.sayer_fogholadeha);
        //         //ch.talash = cha.talash - (chb.talash + chc.talash);
        //         //ch.jazb_tabsare = cha.jazb_tabsare - (chb.jazb_tabsare + chc.jazb_tabsare);
        //         //ch.haghdarman = cha.haghdarman - (chb.haghdarman + chc.haghdarman);
        //         //ch.haghdarman_k = cha.haghdarman_k - (chb.haghdarman_k + chc.haghdarman_k);
        //         //if (p.vaziyat_jesmi == 1 && p.nobime == 2)
        //         //{
        //         //    ch.haghdarman = Math.Round(ch.mashmol_bime * (h_d_k + h_d_p) / 100 + (new_mazad - old_mazad) + (new_takafol - old_takafol));
        //         //    ch.haghdarman_k = Math.Round(ch.mashmol_bime * h_d_k / 100);
        //         //}
        //         //else if ((p.vaziyat_jesmi == 2 || p.vaziyat_jesmi == 3) && p.nobime == 2)
        //         //{
        //         //    ch.haghdarman = Math.Round((new_mazad - old_mazad) + (new_takafol - old_takafol));
        //         //    ch.haghdarman_k = Math.Round((ch.mashmol_bime * h_d_k / 100) + (ch.mashmol_bime * h_d_p / 100));
        //         //}
        //         //ch.barjastegi = cha.barjastegi - (chb.barjastegi + chc.barjastegi);
        //         //ch.tadil = cha.tadil - (chb.tadil + chc.tadil);
        //         //ch.paye = cha.paye - (chb.paye + chc.paye);
        //         //ch.jebhe = cha.jebhe - (chb.jebhe + chc.jebhe);
        //         //ch.sanavat_isar = cha.sanavat_isar - (chb.sanavat_isar + chc.sanavat_isar);
        //         //ch.sanavat_basij = cha.sanavat_basij - (chb.sanavat_basij + chc.sanavat_basij);
        //         //ch.riali = cha.riali - (chb.riali + chc.riali);
        //         //ch.janbazi = cha.janbazi - (chb.janbazi + chc.janbazi);
        //         //ch.tatilkari = cha.tatilkari - (chb.tatilkari + chc.tatilkari);
        //         //ch.sayer = cha.sayer - (chb.sayer + chc.sayer);
        //         //ch.FoghIsar = cha.FoghIsar - (chb.FoghIsar + chc.FoghIsar);
        //         //ch.Made26 = cha.Made26 - (chb.Made26 + chc.Made26);
        //         //ch.Modiriati = cha.Modiriati - (chb.Modiriati + chc.Modiriati);
        //         //ch.Takhasosi = cha.Takhasosi - (chb.Takhasosi + chc.Takhasosi);
        //         //ch.zaribtadil = cha.zaribtadil - (chb.zaribtadil + chc.zaribtadil);
        //         //ch.ashora = cha.ashora - (chb.ashora + chc.ashora);
        //         //ch.ghazai = cha.ghazai - (chb.ghazai + chc.ghazai);
        //         //ch.t_tatilkari = cha.t_tatilkari;
        //         //ch.sanavat_payan_khedmat = cha.sanavat_payan_khedmat - (chb.sanavat_payan_khedmat + chc.sanavat_payan_khedmat);
        //         //ch.Bon = cha.Bon - (chb.Bon + chc.Bon);
        //         ////ch.mogharari = cha.mogharari;
        //         //ch.mashmol_maliat = cha.mashmol_maliat - (chb.mashmol_maliat + chc.mashmol_maliat);
        //         //decimal koltafavot = ch.hoghogh + ch.sanavat + ch.fogh_shoghl +
        //         //    ch.tafavot_tatbigh + ch.hagholad + ch.arzeshyabi + ch.ayelemandi +
        //         //    ch.nobatkari + ch.maskan + ch.sakhtikar + ch.kharobar + ch.jazb +
        //         //    ch.tashilatzendegi + ch.badiabohava + ch.tarmim + ch.ezafekari +
        //         //    ch.sayer_fogholadeha + ch.jazb_tabsare + ch.talash + ch.haghdarman_k +
        //         //    ch.barjastegi + ch.tadil + ch.paye + ch.jebhe + ch.sanavat_isar +
        //         //    ch.janbazi + ch.riali + ch.sanavat_basij + ch.pasandaz / 2 + ch.sayer
        //         //    + ch.tatilkari + ch.sanavat_payan_khedmat + ch.Bon + ch.FoghIsar +
        //         //    ch.Made26 + ch.Modiriati + ch.Takhasosi + ch.ghazai + ch.zaribtadil +
        //         //    ch.ashora + ch.jazbTakhasosi + ch.zajbTadil + ch.hadaghalTadil;// +ch.mamoriat;
        //         //if (ch.maliyat < 0)
        //         //    koltafavot = -(ch.maliyat) + koltafavot;
        //         //else
        //         //    koltafavot = koltafavot - ch.maliyat;
        //         //if (ch.pasandaz < 0)
        //         //    koltafavot = -(ch.pasandaz) + koltafavot;
        //         //else
        //         //    koltafavot = koltafavot - ch.pasandaz;
        //         //if (ch.bime_persenel < 0)
        //         //    koltafavot = -(ch.bime_persenel) + koltafavot;
        //         //else
        //         //    koltafavot = koltafavot - ch.bime_persenel;
        //         //if (ch.haghdarman < 0)
        //         //    koltafavot = -(ch.haghdarman) + koltafavot;
        //         //else
        //         //    koltafavot = koltafavot - ch.haghdarman;
        //         //decimal khales = koltafavot;// -ch.mogharari;
        //         //ch.jame_hoghogh = khales;
        //         //con.Close();
        //       }
        //       catch (Exception x)
        //       {
        //       }
        //     }
        public void m_Moavaghe(int sal, int mah, int payId, Hokm newHokm, MoteghayerhayeHoghoghi newMoteghayer,
                               Fiscal newFiscal, PersonalInfo PersonalInfo, int prsId, int anvaEstekhdam, int typebime, int typeEstekhdam, int HokmMoavaghe)
        {
            try
            {
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                Mohasebat cha = new Mohasebat();
                Mohasebat cha1 = new Mohasebat();
                Mohasebat chb = new Mohasebat();
                Mohasebat chc = new Mohasebat();
                cha = mohasebe;
                int a_h_paye = 0, a_sanavat = 0, a_paye = 0, a_sanavat_basiji = 0, a_sanavat_isar = 0, a_foghshoghl = 0,
                a_takhasosi = 0, a_made26 = 0, a_modiryati = 0, a_barjastegi = 0, a_tatbigh = 0, a_fogh_isar = 0, a_abohava = 0,
                a_tashilat = 0, a_sakhtikar = 0, a_tadil = 0, a_riali = 0, a_jazb9 = 0, a_jazb = 0, a_makhsos = 0, a_vije = 0, a_olad = 0,
                a_ayelemandi = 0, a_kharobar = 0, a_maskan = 0, a_nobatkari = 0, a_bon = 0, a_jazb_tabsare = 0, a_talash = 0, a_jebhe = 0,
                a_janbazi = 0, a_sayer = 0, a_ezafekar = 0, a_mamoriat = 0, a_tatilkari = 0, a_s_payankhedmat = 0, a_ghazai = 0, a_ashoora = 0,
                a_zaribtadil = 0, a_jazbTakhasosi = 0, a_jazbtadil = 0, a_hadaghaltadil = 0, a_shift = 0, a_band_y = 0, a_joz1 = 0, a_band6 = 0,
                a_jazb2 = 0, a_jazb3 = 0, a_vije2 = 0, a_vije3 = 0, a_makhsos2 = 0, a_makhsos3 = 0, a_tatbigh1 = 0, a_khas = 0, a_karane = 0, a_refahi = 0, a_tarmim = 0
                  , a_tarmim_j = 0, a_tarmim_v = 0, a_tarmim_k = 0, a_aghlamKhoraki = 0, a_Monasebat = 0, a_JavaniJameiyat = 0, a_MahadKoodak = 0, sourceId_m = 0, sourceId_j = 0;
                int b_h_paye = 0, b_sanavat = 0, b_paye = 0, b_sanavat_basiji = 0, b_sanavat_isar = 0, b_foghshoghl = 0,
                b_takhasosi = 0, b_made26 = 0, b_modiryati = 0, b_barjastegi = 0, b_tatbigh = 0, b_fogh_isar = 0, b_abohava = 0,
                b_tashilat = 0, b_sakhtikar = 0, b_tadil = 0, b_riali = 0, b_jazb9 = 0, b_jazb = 0, b_makhsos = 0, b_vije = 0, b_olad = 0,
                b_ayelemandi = 0, b_kharobar = 0, b_maskan = 0, b_nobatkari = 0, b_bon = 0, b_jazb_tabsare = 0, b_talash = 0, b_jebhe = 0,
                b_janbazi = 0, b_sayer = 0, b_ezafekar = 0, b_mamoriat = 0, b_tatilkari = 0, b_s_payankhedmat = 0, b_ghazai = 0, b_ashoora = 0,
                b_zaribtadil = 0, b_jazbTakhasosi = 0, b_jazbtadil = 0, b_hadaghaltadil = 0, b_shift = 0, b_band_y = 0, b_joz1 = 0, b_band6 = 0,
                b_jazb2 = 0, b_jazb3 = 0, b_vije2 = 0, b_vije3 = 0, b_makhsos2 = 0, b_makhsos3 = 0, b_tatbigh1 = 0, b_khas = 0, b_karane = 0, b_refahi = 0, b_tarmim = 0
                  , b_tarmim_j = 0, b_tarmim_v = 0, b_tarmim_k = 0, b_aghlamKhoraki=0,b_Monasebat = 0, b_JavaniJameiyat=0, b_MahadKoodak=0;
                int c_h_paye = 0, c_sanavat = 0, c_paye = 0, c_sanavat_basiji = 0, c_sanavat_isar = 0, c_foghshoghl = 0,
                c_takhasosi = 0, c_made26 = 0, c_modiryati = 0, c_barjastegi = 0, c_tatbigh = 0, c_fogh_isar = 0, c_abohava = 0,
                c_tashilat = 0, c_sakhtikar = 0, c_tadil = 0, c_riali = 0, c_jazb9 = 0, c_jazb = 0, c_makhsos = 0, c_vije = 0, c_olad = 0,
                c_ayelemandi = 0, c_kharobar = 0, c_maskan = 0, c_nobatkari = 0, c_bon = 0, c_jazb_tabsare = 0, c_talash = 0, c_jebhe = 0,
                c_janbazi = 0, c_sayer = 0, c_ezafekar = 0, c_mamoriat = 0, c_tatilkari = 0, c_s_payankhedmat = 0, c_ghazai = 0, c_ashoora = 0,
                c_zaribtadil = 0, c_jazbTakhasosi = 0, c_jazbtadil = 0, c_hadaghaltadil = 0, c_shift = 0, c_band_y = 0, c_joz1 = 0, c_band6 = 0,
                c_jazb2 = 0, c_jazb3 = 0, c_vije2 = 0, c_vije3 = 0, c_makhsos2 = 0, c_makhsos3 = 0, c_tatbigh1 = 0, c_khas = 0, c_karane = 0, c_refahi = 0, c_tarmim = 0
                  , c_tarmim_j = 0, c_tarmim_v = 0, c_tarmim_k = 0, c_aghlamKhoraki=0, c_Monasebat = 0, c_JavaniJameiyat=0, c_MahadKoodak=0;
                foreach (var item in cha.Items)
                {
                    int mab = item.fldMablagh;
                    switch (item.fldItemHoghoghiId)
                    {
                        case 1: a_h_paye = mab; break;
                        case 2: a_sanavat = mab; break;
                        case 3: a_paye = mab; break;
                        case 4: a_sanavat_basiji = mab; break;
                        case 5: a_sanavat_isar = mab; break;
                        case 6: a_foghshoghl = mab; break;
                        case 7: a_takhasosi = mab; break;
                        case 8: a_made26 = mab; break;
                        case 9: a_modiryati = mab; break;
                        case 10: a_barjastegi = mab; break;
                        case 11: a_tatbigh = mab; break;
                        case 12: a_fogh_isar = mab; break;
                        case 13: a_abohava = mab; break;
                        case 14: a_tashilat = mab; break;
                        case 15: a_sakhtikar = mab; break;
                        case 16: a_tadil = mab; break;
                        case 17: a_riali = mab; break;
                        case 18: a_jazb9 = mab; break;
                        case 19: a_jazb = mab; break;
                        case 20: a_makhsos = mab; break;
                        case 21: a_vije = mab; break;
                        case 22: a_olad = mab; break;
                        case 23: a_ayelemandi = mab; break;
                        case 24: a_kharobar = mab; break;
                        case 25: a_maskan = mab; break;
                        case 26: a_nobatkari = mab; break;
                        case 27: a_bon = mab; break;
                        case 28: a_jazb_tabsare = mab; break;
                        case 29: a_talash = mab; break;
                        case 30: a_jebhe = mab; break;
                        case 31: a_janbazi = mab; break;
                        case 32: a_sayer = mab; break;
                        case 33: a_ezafekar = mab; break;
                        case 34: a_mamoriat = mab; break;
                        case 35: a_tatilkari = mab; break;
                        case 36: a_s_payankhedmat = mab; break;
                        case 37: a_ghazai = mab; break;
                        case 38: a_ashoora = mab; break;
                        case 39: a_zaribtadil = mab; break;
                        case 40: a_jazbTakhasosi = mab; break;
                        case 41: a_jazbtadil = mab; break;
                        case 42: a_hadaghaltadil = mab; break;
                        case 43: a_shift = mab; break;
                        case 44: a_band_y = mab; break;
                        case 45: a_joz1 = mab; break;
                        case 46: a_band6 = mab; break;
                        case 47: a_jazb2 = mab; break;
                        case 48: a_jazb3 = mab; break;
                        case 49: a_vije2 = mab; break;
                        case 50: a_vije3 = mab; break;
                        case 51: a_makhsos2 = mab; break;
                        case 52: a_makhsos3 = mab; break;
                        case 53: a_tatbigh1 = mab; break;
                        case 54: a_khas = mab; break;
                        case 55: a_karane = mab; break;
                        case 56: a_refahi = mab; break;
                        case 57: a_tarmim = mab; break;
                        case 60: a_tarmim_j = mab; break;
                        case 61: a_tarmim_v = mab; break;
                        case 62: a_tarmim_k = mab; break;
                        case 64: a_MahadKoodak = mab; break;
                        case 65: a_aghlamKhoraki = mab; break;
                        case 67: { a_Monasebat = mab; sourceId_m = item.fldSourceId; } break;
                        case 68: { a_JavaniJameiyat = mab; sourceId_j = item.fldSourceId; } break;
                    }
                }
                decimal h_d_k = 0;
                decimal h_d_p = 0;
                decimal b_p = 0, b_k = 0, b_b = 0;
                decimal old_mazad = 0, old_takafol = 0;
                decimal new_mazad = 0, new_takafol = 0;
                bool mazad_30sal = false;
                int vaziat_jesmi = 0;
                string commandText = "SELECT Pay.tblMohasebat_Items.fldMablagh, Com.tblItems_Estekhdam.fldItemsHoghughiId, Com.tblItems_Estekhdam.fldId AS fldItems_EstekhdamId " +
                  "FROM Pay.tblMohasebat INNER JOIN Pay.tblMohasebat_Items ON Pay.tblMohasebat.fldId = Pay.tblMohasebat_Items.fldMohasebatId INNER JOIN" +
                  " Com.tblItems_Estekhdam ON Pay.tblMohasebat_Items.fldItemEstekhdamId = Com.tblItems_Estekhdam.fldId where" +
                  " Pay.tblMohasebat.fldPersonalId=" + payId + " and Pay.tblMohasebat.fldYear=" + sal + " and Pay.tblMohasebat.fldMonth=" + mah;
                var tblMohasebe = service.GetData(commandText).Tables[0];
                int s = 0;
                int e_type = 0;
                //         if (newHokm.NoeEstekhdam <= 5)
                //           e_type = 1;
                //         else if (newHokm.NoeEstekhdam == 6)
                //           e_type = 2;
                //         else if (newHokm.NoeEstekhdam >= 7 && newHokm.NoeEstekhdam <= 9)
                //           e_type = 3;
                //         else e_type = 4;
                commandText = "select * from com.tblAnvaEstekhdam where fldid=" + newHokm.NoeEstekhdam;
                var t = service.GetData(commandText).Tables[0];
                e_type = Convert.ToInt32(t.Rows[0]["fldNoeEstekhdamId"]);
                if (tblMohasebe.Rows.Count > 0)
                {
                    for (int i = 0; i < tblMohasebe.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"]) > 0)
                        {
                            int mab = Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"]);
                            int item_id = Convert.ToInt32(tblMohasebe.Rows[i]["fldItemsHoghughiId"]);
                            switch (item_id)
                            {
                                case 1: b_h_paye = mab; break;
                                case 2: b_sanavat = mab; break;
                                case 3: b_paye = mab; break;
                                case 4: b_sanavat_basiji = mab; break;
                                case 5: b_sanavat_isar = mab; break;
                                case 6: b_foghshoghl = mab; break;
                                case 7: b_takhasosi = mab; break;
                                case 8: b_made26 = mab; break;
                                case 9: b_modiryati = mab; break;
                                case 10: b_barjastegi = mab; break;
                                case 11: b_tatbigh = mab; break;
                                case 12: b_fogh_isar = mab; break;
                                case 13: b_abohava = mab; break;
                                case 14: b_tashilat = mab; break;
                                case 15: b_sakhtikar = mab; break;
                                case 16: b_tadil = mab; break;
                                case 17: b_riali = mab; break;
                                case 18: b_jazb9 = mab; break;
                                case 19: b_jazb = mab; break;
                                case 20: b_makhsos = mab; break;
                                case 21: b_vije = mab; break;
                                case 22: b_olad = mab; break;
                                case 23: b_ayelemandi = mab; break;
                                case 24: b_kharobar = mab; break;
                                case 25: b_maskan = mab; break;
                                case 26: b_nobatkari = mab; break;
                                case 27: b_bon = mab; break;
                                case 28: b_jazb_tabsare = mab; break;
                                case 29: b_talash = mab; break;
                                case 30: b_jebhe = mab; break;
                                case 31: b_janbazi = mab; break;
                                case 32: b_sayer = mab; break;
                                case 33: b_ezafekar = mab; break;
                                case 34: b_mamoriat = mab; break;
                                case 35: b_tatilkari = mab; break;
                                case 36: b_s_payankhedmat = mab; break;
                                case 37: b_ghazai = mab; break;
                                case 38: b_ashoora = mab; break;
                                case 39: b_zaribtadil = mab; break;
                                case 40: b_jazbTakhasosi = mab; break;
                                case 41: b_jazbtadil = mab; break;
                                case 42: b_hadaghaltadil = mab; break;
                                case 43: b_shift = mab; break;
                                case 44: b_band_y = mab; break;
                                case 45: b_joz1 = mab; break;
                                case 46: b_band6 = mab; break;
                                case 47: b_jazb2 = mab; break;
                                case 48: b_jazb3 = mab; break;
                                case 49: b_vije2 = mab; break;
                                case 50: b_vije3 = mab; break;
                                case 51: b_makhsos2 = mab; break;
                                case 52: b_makhsos3 = mab; break;
                                case 53: b_tatbigh1 = mab; break;
                                case 54: b_khas = mab; break;
                                case 55: b_karane = mab; break;
                                case 56: b_refahi = mab; break;
                                case 57: b_tarmim = mab; break;
                                case 60: b_tarmim_j = mab; break;
                                case 61: b_tarmim_v = mab; break;
                                case 62: b_tarmim_k = mab; break;
                                case 64: b_MahadKoodak = mab; break;
                                case 65: b_aghlamKhoraki = mab; break;
                                case 67: b_Monasebat = mab; break;
                                case 68: b_JavaniJameiyat = mab; break;
                            }
                            /*if (item_id == 3 || item_id == 24 || item_id == 27 || item_id == 38 || item_id == 30 || item_id == 31)
              {
              if (e_type != 1)
              {//غیر کارگری
              b_sayer += mab;
              }
              }
              else if (item_id == 4 || item_id == 5 || item_id == 6 || item_id == 7 || item_id == 8 || item_id == 9 || item_id == 10 || item_id == 11 || item_id == 12 || item_id == 13 || item_id == 14 || item_id == 16 || item_id == 17 || item_id == 18 || item_id == 21 || item_id == 23 || item_id == 28 || item_id == 29 || item_id == 37 || item_id == 39 || item_id == 40 || item_id == 41 || item_id == 42 || item_id == 43 || item_id == 44 || item_id == 45 || item_id == 46 || item_id == 47 || item_id == 48 || item_id == 49 || item_id == 50 || item_id == 53 || item_id == 54 || item_id == 55 || item_id == 56 || item_id == 57)
              {
              if (e_type == 1)
              {//فقط کارگری
              b_sayer += mab;
              }
              }
              else if (item_id == 20)
              {
              if (e_type <= 2)
              {//کارگر رسمی و کارمند رسمی
              b_sayer += mab;
              }
              }
              else if (item_id == 51 || item_id == 52)
              {
              if (e_type != 3)
              {//غیر پیمانی 
              b_sayer += mab;
              }
              }*/
                        }
                    }
                    string com = "select * from Com.tblItems_Estekhdam where fldTypeEstekhdamId=" + e_type;
                    var mm = service.GetData(com).Tables[0];
                    for (int i = 0; i < mm.Rows.Count; i++)
                    {
                        //var m1 = chb.Items.Where(k => k.fldItemEstekhdamId == (int)mm.Rows[i]["fldId"]).FirstOrDefault();
                        //if (m1 == null)
                        //    chb.Items.Add(new Mohasebat_Items
                        //    {
                        //        fldItemEstekhdamId = (int)mm.Rows[i]["fldId"],
                        //        fldItemHoghoghiId = (int)mm.Rows[i]["fldItemsHoghughiId"],
                        //        fldMablagh = 0
                        //    });
                        var m2 = cha1.Items.Where(k => k.fldItemEstekhdamId == (int)mm.Rows[i]["fldId"]).FirstOrDefault();
                        if (m2 == null)
                            cha1.Items.Add(new Mohasebat_Items
                            {
                                fldItemEstekhdamId = (int)mm.Rows[i]["fldId"],
                                fldItemHoghoghiId = (int)mm.Rows[i]["fldItemsHoghughiId"],
                                fldMablagh = 0
                            });
                    }
                    commandText = "SELECT Pay.tblMohasebat_PersonalInfo.fldMazad30Sal, Pay.tblMohasebat_PersonalInfo.fldStatusIsargariId,Pay.tblMohasebat_PersonalInfo.fldAnvaEstekhdamId, Pay.tblMohasebat_PersonalInfo.fldHokmId, " +
                      "Pay.tblMohasebat_PersonalInfo.fldFiscalHeaderId,pay.tblMohasebat.fldMaliyat,pay.tblMohasebat.fldMashmolBime,pay.tblMohasebat.fldBimePersonal,pay.tblMohasebat.fldBimeKarFarma,pay.tblMohasebat.fldBimeBikari," +
                      "pay.tblMohasebat.fldMashmolMaliyat, Pay.tblMohasebat_PersonalInfo.fldMoteghayerHoghoghiId,pay.tblMohasebat.fldHaghDarman,pay.tblMohasebat.fldHaghDarmanKarfFarma,pay.tblMohasebat.fldHaghDarmanDolat,pay.tblMohasebat.fldPasAndaz FROM Pay.tblMohasebat " +
                      "INNER JOIN Pay.tblMohasebat_PersonalInfo ON Pay.tblMohasebat.fldId = Pay.tblMohasebat_PersonalInfo.fldMohasebatId WHERE " +
                      "Pay.tblMohasebat.fldPersonalId=" + payId + " and Pay.tblMohasebat.fldYear=" + sal + " and Pay.tblMohasebat.fldMonth=" + mah;
                    var tblMohasebe1 = service.GetData(commandText).Tables[0];
                    mazad_30sal = Convert.ToBoolean(tblMohasebe1.Rows[0]["fldMazad30Sal"]);
                    vaziat_jesmi = Convert.ToInt32(tblMohasebe1.Rows[0]["fldStatusIsargariId"]);
                    int _group = Convert.ToInt32(tblMohasebe1.Rows[0]["fldFiscalHeaderId"]);
                    int old_moteghayerid = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMoteghayerHoghoghiId"]);
                    int old_hokmid = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHokmId"]);
                    commandText = "SELECT     fldHaghDarmanMazad, fldHaghDarmanTahteTakaffol FROM         Pay.tblMoteghayerhayeHoghoghi WHERE     (fldId = " + old_moteghayerid + ")";
                    var oldMoteghayer = service.GetData(commandText).Tables[0];
                    if (oldMoteghayer.Rows.Count > 0)
                    {
                        var oldhokm = getHokmByid(old_hokmid);
                        int tedad_afradekhanevade = 0, t_takafol_60 = 0, t_takafol_70 = 0, t_takafol_60_t = 0, t_takafol_70_t = 0;
                        switch (oldhokm.NoeTaahol)
                        {
                            case 1:
                                tedad_afradekhanevade = 1;
                                break;
                            case 2:
                                tedad_afradekhanevade = 1 + oldhokm.TedadFarzand;
                                break;
                            case 3:
                                tedad_afradekhanevade = 2;
                                break;
                            case 4:
                                tedad_afradekhanevade = 2 + oldhokm.TedadFarzand;
                                break;
                        }
                        t_takafol_60 = oldhokm.TedadAfradTakafol;
                        old_mazad = Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanMazad"]);
                        old_takafol = Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanTahteTakaffol"]);
                        int tedad_t_takalof = 0;
                        int ezafe_farzand = 0;
                        if (PersonalInfo.fldHamsarKarmand == true)
                            tedad_afradekhanevade = tedad_afradekhanevade - 1;
                        tedad_t_takalof = t_takafol_60 + t_takafol_70;
                        if (anvaEstekhdam <= 10 && anvaEstekhdam >= 1 && typebime == 2)
                        {
                            if (oldhokm.TedadFarzand > 3 && PersonalInfo.fldJensiyat == 1)
                            {
                                if (sal >= 1392 && mah >= 9)
                                    old_mazad = 0;
                                else if (sal > 1392)
                                    old_mazad = 0;
                                else
                                {
                                    ezafe_farzand = oldhokm.TedadFarzand - 3;
                                    old_mazad = old_mazad * Convert.ToDecimal(ezafe_farzand);
                                }
                            }
                            else if (PersonalInfo.fldJensiyat == 2)//------تغییرات
                            {//------تغییرات
                                ezafe_farzand = tedad_afradekhanevade - 1;//------تغییرات
                                old_mazad = old_mazad * Convert.ToDecimal(ezafe_farzand);//------تغییرات
                            }//------تغییرات
                            else
                                old_mazad = 0;
                            commandText = "select count(*) as tedad from [Prs].[tblAfradTahtePooshesh]  where fldNesbat=4 and [fldPersonalId]=" + prsId;
                            var Tabae = service.GetData(commandText).Tables[0];
                            if (Tabae.Rows.Count > 0)
                            {
                                old_mazad += Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanMazad"]) * Convert.ToDecimal(Tabae.Rows[0]["tedad"]);
                            }
                            old_takafol = old_takafol * Convert.ToDecimal(tedad_t_takalof);
                        }
                        else
                        {
                            old_takafol = old_mazad = 0;
                        }
                    }
                    if (newMoteghayer != null)
                    {
                        int tedad_afradekhanevade = 0, t_takafol_60 = 0, t_takafol_70 = 0, t_takafol_60_t = 0, t_takafol_70_t = 0;
                        switch (newHokm.NoeTaahol)
                        {
                            case 1:
                                tedad_afradekhanevade = 1;
                                break;
                            case 2:
                                tedad_afradekhanevade = 1 + newHokm.TedadFarzand;
                                break;
                            case 3:
                                tedad_afradekhanevade = 2;
                                break;
                            case 4:
                                tedad_afradekhanevade = 2 + newHokm.TedadFarzand;
                                break;
                        }
                        t_takafol_60 = newHokm.TedadAfradTakafol;
                        new_mazad = Convert.ToDecimal(newMoteghayer.fldHaghDarmanMazad);
                        new_takafol = Convert.ToDecimal(newMoteghayer.fldHaghDarmanTahteTakaffol);
                        int tedad_t_takalof = 0;
                        int ezafe_farzand = 0;
                        if (PersonalInfo.fldHamsarKarmand == true)
                            tedad_afradekhanevade = tedad_afradekhanevade - 1;
                        tedad_t_takalof = t_takafol_60 + t_takafol_70;
                        if (anvaEstekhdam <= 10 && anvaEstekhdam >= 1 && typebime == 2)
                        {
                            if (newHokm.TedadFarzand > 3 && PersonalInfo.fldJensiyat == 1)
                            {
                                if (sal >= 1392 && mah >= 9)
                                    new_mazad = 0;
                                else if (sal > 1392)
                                    new_mazad = 0;
                                else
                                {
                                    ezafe_farzand = newHokm.TedadFarzand - 3;
                                    new_mazad = new_mazad * Convert.ToDecimal(ezafe_farzand);
                                }
                            }
                            else if (PersonalInfo.fldJensiyat == 2)//------تغییرات
                            {//------تغییرات
                                ezafe_farzand = tedad_afradekhanevade - 1;//------تغییرات
                                new_mazad = new_mazad * Convert.ToDecimal(ezafe_farzand);//------تغییرات
                            }//------تغییرات
                            else
                                new_mazad = 0;
                            commandText = "select count(*) as tedad from [Prs].[tblAfradTahtePooshesh]  where fldNesbat=4 and [fldPersonalId]=" + prsId;
                            var Tabae = service.GetData(commandText).Tables[0];
                            if (Tabae.Rows.Count > 0)
                            {
                                new_mazad += Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanMazad"]) * Convert.ToDecimal(Tabae.Rows[0]["tedad"]);
                            }
                            new_takafol = new_takafol * Convert.ToDecimal(tedad_t_takalof);
                        }
                        else
                        {
                            new_takafol = new_mazad = 0;
                        }
                    }
                    chb.fldMashmolMaliyat = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMashmolMaliyat"]);
                    //--------------------مالیات  
                    chb.fldMaliyat = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMaliyat"]); //maliyat;
                    //mashmol_bime = mashmol_bime - hog.mogharari;
                    //--------------------حق بیمه
                    chb.fldMashmolBime = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMashmolBime"]);
                    chb.fldBimeBikari = Convert.ToInt32(tblMohasebe1.Rows[0]["fldBimeBikari"]);
                    chb.fldBimeKarFarma = Convert.ToInt32(tblMohasebe1.Rows[0]["fldBimeKarFarma"]);
                    chb.fldBimePersonal = Convert.ToInt32(tblMohasebe1.Rows[0]["fldBimePersonal"]);
                    chb.fldHaghDarman = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHaghDarman"]);
                    chb.fldHaghDarmanKarfFarma = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHaghDarmanKarfFarma"]);
                    chb.fldHaghDarmanDolat = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHaghDarmanDolat"]);
                    chb.fldPasAndaz = Convert.ToInt32(tblMohasebe1.Rows[0]["fldPasAndaz"]);
                }
                commandText = "SELECT isnull(sum(Pay.tblMoavaghat.fldHaghDarmanKarfFarma),0)as fldHaghDarmanKarfFarma, " +
                  "isnull(sum(Pay.tblMoavaghat.fldHaghDarmanDolat),0)as fldHaghDarmanDolat, isnull(sum(Pay.tblMoavaghat.fldHaghDarman),0)" +
                  "as fldHaghDarman, isnull(sum(Pay.tblMoavaghat.fldBimePersonal),0)as fldBimePersonal, isnull(sum(Pay.tblMoavaghat.fldBimeKarFarma)" +
                  ",0)as fldBimeKarFarma, isnull(sum(Pay.tblMoavaghat.fldBimeBikari),0)as fldBimeBikari, isnull(sum(Pay.tblMoavaghat.fldBimeMashaghel),0)as" +
                  " fldBimeMashaghel, isnull(sum(Pay.tblMoavaghat.fldPasAndaz),0)as fldPasAndaz, isnull(sum(Pay.tblMoavaghat.fldMashmolBime),0)" +
                  "as fldMashmolBime, isnull(sum(Pay.tblMoavaghat.fldMashmolMaliyat),0)as fldMashmolMaliyat, isnull(sum(Pay.tblMoavaghat.fldMaliyat),0)as" +
                  " fldMaliyat FROM Pay.tblMoavaghat INNER JOIN Pay.tblMohasebat ON Pay.tblMoavaghat.fldMohasebatId = Pay.tblMohasebat.fldId " +
                  "WHERE (Pay.tblMoavaghat.fldYear = " + sal + ") AND (Pay.tblMoavaghat.fldMonth = " + mah + ") AND (Pay.tblMohasebat.fldPersonalId = " + payId + ")";
                var mov = service.GetData(commandText).Tables[0];
                if (mov.Rows.Count > 0)
                {
                    chc.fldHaghDarman = Convert.ToInt32(mov.Rows[0]["fldHaghDarman"]);
                    chc.fldHaghDarmanKarfFarma = Convert.ToInt32(mov.Rows[0]["fldHaghDarmanKarfFarma"]);
                    chc.fldHaghDarmanDolat = Convert.ToInt32(mov.Rows[0]["fldHaghDarmanDolat"]);
                    chc.fldBimeKarFarma = Convert.ToInt32(mov.Rows[0]["fldBimeKarFarma"]);
                    chc.fldBimePersonal = Convert.ToInt32(mov.Rows[0]["fldBimePersonal"]);
                    chc.fldBimeBikari = Convert.ToInt32(mov.Rows[0]["fldBimeBikari"]);
                    chc.fldMaliyat = Convert.ToInt32(mov.Rows[0]["fldMaliyat"]);
                    chc.fldMashmolBime = Convert.ToInt32(mov.Rows[0]["fldMashmolBime"]);
                    chc.fldMashmolMaliyat = Convert.ToInt32(mov.Rows[0]["fldMashmolMaliyat"]);
                    chc.fldPasAndaz = Convert.ToInt32(mov.Rows[0]["fldPasAndaz"]);
                    commandText = "SELECT Pay.tblMoavaghat_Items.fldItemEstekhdamId, SUM(Pay.tblMoavaghat_Items.fldMablagh) AS fldMablagh," +
                      " Com.tblItems_Estekhdam.fldItemsHoghughiId FROM Pay.tblMoavaghat_Items INNER JOIN Pay.tblMoavaghat INNER JOIN " +
                      "Pay.tblMohasebat ON Pay.tblMoavaghat.fldMohasebatId = Pay.tblMohasebat.fldId ON Pay.tblMoavaghat_Items.fldMoavaghatId =" +
                      " Pay.tblMoavaghat.fldId INNER JOIN Com.tblItems_Estekhdam ON Pay.tblMoavaghat_Items.fldItemEstekhdamId = Com.tblItems_Estekhdam.fldId WHERE " +
                      "(Pay.tblMoavaghat.fldYear = " + sal + ") AND (Pay.tblMoavaghat.fldMonth = " + mah + ") AND (Pay.tblMohasebat.fldPersonalId = " + payId + ") " +
                      "GROUP BY Pay.tblMoavaghat_Items.fldItemEstekhdamId, Com.tblItems_Estekhdam.fldItemsHoghughiId ORDER BY Com.tblItems_Estekhdam.fldItemsHoghughiId";
                    var mov_items = service.GetData(commandText).Tables[0];
                    if (mov_items.Rows.Count > 0)
                    {
                        for (int i = 0; i < mov_items.Rows.Count; i++)
                        {
                            if (Convert.ToInt32(mov_items.Rows[i]["fldMablagh"]) != 0)
                            {
                                int mab = Convert.ToInt32(mov_items.Rows[i]["fldMablagh"]);
                                int item_id = Convert.ToInt32(mov_items.Rows[i]["fldItemsHoghughiId"]);
                                switch (item_id)
                                {
                                    case 1: c_h_paye = mab; break;
                                    case 2: c_sanavat = mab; break;
                                    case 3: c_paye = mab; break;
                                    case 4: c_sanavat_basiji = mab; break;
                                    case 5: c_sanavat_isar = mab; break;
                                    case 6: c_foghshoghl = mab; break;
                                    case 7: c_takhasosi = mab; break;
                                    case 8: c_made26 = mab; break;
                                    case 9: c_modiryati = mab; break;
                                    case 10: c_barjastegi = mab; break;
                                    case 11: c_tatbigh = mab; break;
                                    case 12: c_fogh_isar = mab; break;
                                    case 13: c_abohava = mab; break;
                                    case 14: c_tashilat = mab; break;
                                    case 15: c_sakhtikar = mab; break;
                                    case 16: c_tadil = mab; break;
                                    case 17: c_riali = mab; break;
                                    case 18: c_jazb9 = mab; break;
                                    case 19: c_jazb = mab; break;
                                    case 20: c_makhsos = mab; break;
                                    case 21: c_vije = mab; break;
                                    case 22: c_olad = mab; break;
                                    case 23: c_ayelemandi = mab; break;
                                    case 24: c_kharobar = mab; break;
                                    case 25: c_maskan = mab; break;
                                    case 26: c_nobatkari = mab; break;
                                    case 27: c_bon = mab; break;
                                    case 28: c_jazb_tabsare = mab; break;
                                    case 29: c_talash = mab; break;
                                    case 30: c_jebhe = mab; break;
                                    case 31: c_janbazi = mab; break;
                                    case 32: c_sayer = mab; break;
                                    case 33: c_ezafekar = mab; break;
                                    case 34: c_mamoriat = mab; break;
                                    case 35: c_tatilkari = mab; break;
                                    case 36: c_s_payankhedmat = mab; break;
                                    case 37: c_ghazai = mab; break;
                                    case 38: c_ashoora = mab; break;
                                    case 39: c_zaribtadil = mab; break;
                                    case 40: c_jazbTakhasosi = mab; break;
                                    case 41: c_jazbtadil = mab; break;
                                    case 42: c_hadaghaltadil = mab; break;
                                    case 43: c_shift = mab; break;
                                    case 44: c_band_y = mab; break;
                                    case 45: c_joz1 = mab; break;
                                    case 46: c_band6 = mab; break;
                                    case 47: c_jazb2 = mab; break;
                                    case 48: c_jazb3 = mab; break;
                                    case 49: c_vije2 = mab; break;
                                    case 50: c_vije3 = mab; break;
                                    case 51: c_makhsos2 = mab; break;
                                    case 52: c_makhsos3 = mab; break;
                                    case 53: c_tatbigh1 = mab; break;
                                    case 54: c_khas = mab; break;
                                    case 55: c_karane = mab; break;
                                    case 56: c_refahi = mab; break;
                                    case 57: c_tarmim = mab; break;
                                    case 60: c_tarmim_j = mab; break;
                                    case 61: c_tarmim_v = mab; break;
                                    case 62: c_tarmim_k = mab; break;
                                    case 64: c_MahadKoodak = mab; break;
                                    case 65: c_aghlamKhoraki = mab; break;
                                    case 67: c_Monasebat = mab; break;
                                    case 68: c_JavaniJameiyat = mab; break;
                                }
                                /*if (item_id == 3 || item_id == 24 || item_id == 27 || item_id == 38 || item_id == 30 || item_id == 31)
                {
                if (e_type != 1)
                {//غیر کارگری
                c_sayer += mab;
                }
                }
                else if (item_id == 4 || item_id == 5 || item_id == 6 || item_id == 7 || item_id == 8 || item_id == 9 || item_id == 10 || item_id == 11 || item_id == 12 || item_id == 13 || item_id == 14 || item_id == 16 || item_id == 17 || item_id == 18 || item_id == 21 || item_id == 23 || item_id == 28 || item_id == 29 || item_id == 37 || item_id == 39 || item_id == 40 || item_id == 41 || item_id == 42 || item_id == 43 || item_id == 44 || item_id == 45 || item_id == 46 || item_id == 47 || item_id == 48 || item_id == 49 || item_id == 50 || item_id == 53 || item_id == 54 || item_id == 55 || item_id == 56 || item_id == 57)
                {
                if (e_type == 1)
                {//فقط کارگری
                c_sayer += mab;
                }
                }
                else if (item_id == 20)
                {
                if (e_type <= 2)
                {//کارگر رسمی و کارمند رسمی
                c_sayer += mab;
                }
                }
                else if (item_id == 51 || item_id == 52)
                {
                if (e_type != 3)
                {//غیر پیمانی 
                c_sayer += mab;
                }
                }*/
                            }
                        }
                        string com = "select * from Com.tblItems_Estekhdam where fldTypeEstekhdamId=" + e_type;
                        var mm = service.GetData(com).Tables[0];
                        for (int i = 0; i < mm.Rows.Count; i++)
                        {
                            var m1 = chc.Items.Where(k => k.fldItemEstekhdamId == (int)mm.Rows[i]["fldId"]).FirstOrDefault();
                            if (m1 == null)
                                chc.Items.Add(new Mohasebat_Items
                                {
                                    fldItemEstekhdamId = (int)mm.Rows[i]["fldId"],
                                    fldItemHoghoghiId = (int)mm.Rows[i]["fldItemsHoghughiId"],
                                    fldMablagh = 0
                                });
                        }
                    }
                }
                int BimeMoavaghe = 0, BimeMoavagheNakhales = 0, BimeBikari = 0, BimePersonal = 0, BimeKarFarma = 0; long FinalBime = 0;
                FinalBime = SumBime + SumBimeMoavaghe;
                SumBimeMoavaghe = SumBimeMoavaghe + (cha.fldMashmolBime - (chb.fldMashmolBime + chc.fldMashmolBime));
                b_p = Convert.ToDecimal(newMoteghayer.fldDarsadBimePersonal);
                b_k = Convert.ToDecimal(newMoteghayer.fldDarsadbimeKarfarma);
                b_b = Convert.ToDecimal(newMoteghayer.fldDarsadBimeBikari);
                decimal b_k_j = Convert.ToDecimal(newMoteghayer.fldDarsadBimeJanbazan);
                if ((FinalBime) >= MaxBime && typebime == 1)
                {
                    BimeMoavaghe = Convert.ToInt32(MaxBime - (FinalBime));
                    if (BimeMoavaghe < 0)
                    {
                        BimeMoavaghe = 0;
                        BimeBikari = 0;
                        BimeKarFarma = 0;
                        BimePersonal = 0;
                    }
                    else
                    {
                        if (cha.fldBimeBikari != 0)
                            BimeBikari = Convert.ToInt32(Math.Round(((BimeMoavaghe/* - MogharariMoavagh*/) * Convert.ToDecimal(b_b)) / 100));
                        //if (cha.fldBimeBikari - (chb.fldBimeBikari + chc.fldBimeBikari) > 0 && BimeBikari < 0)
                        //    BimeBikari = 0;
                        if (cha.fldBimePersonal != 0)
                            BimePersonal = Convert.ToInt32(Math.Round(((BimeMoavaghe/* - MogharariMoavagh*/) * Convert.ToDecimal(b_p)) / 100));
                        //if (cha.fldBimePersonal - (chb.fldBimePersonal + chc.fldBimePersonal) > 0 && BimePersonal < 0)
                        //    BimePersonal = 0;
                        if (cha.fldBimeKarFarma != 0)
                            BimeKarFarma = Convert.ToInt32(Math.Round(((BimeMoavaghe/* - MogharariMoavagh*/) * Convert.ToDecimal(b_k)) / 100));
                        //if (cha.fldBimeKarFarma - (chb.fldBimeKarFarma + chc.fldBimeKarFarma) > 0 && BimeKarFarma < 0)
                        //    BimeKarFarma = 0;
                    }
                    BimeMoavagheNakhales = cha.fldMashmolBime - (chb.fldMashmolBime + chc.fldMashmolBime);
                }
                else if (((SumBime + SumBimeMoavaghe)) > MaxBime && typebime == 1)
                {
                    BimeMoavaghe = Convert.ToInt32(MaxBime - (FinalBime));
                    if (BimeMoavaghe < 0)
                    {
                        BimeMoavaghe = 0;
                        BimeBikari = 0;
                        BimeKarFarma = 0;
                        BimePersonal = 0;
                    }
                    else
                    {
                        if (cha.fldBimeBikari != 0)
                            BimeBikari = Convert.ToInt32(Math.Round(((BimeMoavaghe/* - MogharariMoavagh*/) * Convert.ToDecimal(b_b)) / 100));
                        //if (cha.fldBimeBikari - (chb.fldBimeBikari + chc.fldBimeBikari) > 0 && BimeBikari < 0)
                        //    BimeBikari = 0;
                        if (cha.fldBimePersonal != 0)
                            BimePersonal = Convert.ToInt32(Math.Round(((BimeMoavaghe/* - MogharariMoavagh*/) * Convert.ToDecimal(b_p)) / 100));
                        //if (cha.fldBimePersonal - (chb.fldBimePersonal + chc.fldBimePersonal) > 0 && BimePersonal < 0)
                        //    BimePersonal = 0;
                        if (cha.fldBimeKarFarma != 0)
                            BimeKarFarma = Convert.ToInt32(Math.Round(((BimeMoavaghe/* - MogharariMoavagh*/) * Convert.ToDecimal(b_k)) / 100));
                        //if (cha.fldBimeKarFarma - (chb.fldBimeKarFarma + chc.fldBimeKarFarma) > 0 && BimeKarFarma < 0)
                        //    BimeKarFarma = 0;
                    }
                    BimeMoavagheNakhales = cha.fldMashmolBime - (chb.fldMashmolBime + chc.fldMashmolBime);
                }
                else
                {
                    BimeMoavaghe = cha.fldMashmolBime - (chb.fldMashmolBime + chc.fldMashmolBime);
                    BimeBikari = cha.fldBimeBikari - (chb.fldBimeBikari + chc.fldBimeBikari);
                    BimeKarFarma = cha.fldBimeKarFarma - (chb.fldBimeKarFarma + chc.fldBimeKarFarma);
                    BimePersonal = cha.fldBimePersonal - (chb.fldBimePersonal + chc.fldBimePersonal);
                }
                mohasebe1.Movaghat.Add(new Movaghat
                {
                    fldBimeBikari = BimeBikari,
                    fldBimeKarFarma = BimeKarFarma,
                    fldBimePersonal = BimePersonal,
                    fldHaghDarman = cha.fldHaghDarman - (chb.fldHaghDarman + chc.fldHaghDarman),
                    fldHaghDarmanDolat = cha.fldHaghDarmanDolat - (chb.fldHaghDarmanDolat + chc.fldHaghDarmanDolat),
                    fldHaghDarmanKarfFarma = cha.fldHaghDarmanKarfFarma - (chb.fldHaghDarmanKarfFarma + chc.fldHaghDarmanKarfFarma),
                    fldMaliyat = cha.fldMaliyat - (chb.fldMaliyat + chc.fldMaliyat),
                    //fldMashmolBime = cha.fldMashmolBime - (chb.fldMashmolBime + chc.fldMashmolBime),
                    fldMashmolBime = BimeMoavaghe,
                    fldMashmolBimeNaKhales = BimeMoavagheNakhales,
                    fldMashmolMaliyat = cha.fldMashmolMaliyat - (chb.fldMashmolMaliyat + chc.fldMashmolMaliyat),
                    fldPasAndaz = cha.fldPasAndaz - (chb.fldPasAndaz + chc.fldPasAndaz),
                    fldMonth = mah,
                    fldYear = sal,
                    fldHokmId = HokmMoavaghe
                });
                int m_h_paye = a_h_paye - (b_h_paye + c_h_paye);
                int itemid = 1;
                if (m_h_paye != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_h_paye
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_h_paye
                        });
                    }
                }
                int m_sanavat = a_sanavat - (b_sanavat + c_sanavat);
                itemid = 2;
                if (m_sanavat != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_sanavat
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_sanavat
                        });
                    }
                }
                int m_paye = a_paye - (b_paye + c_paye);
                itemid = 3;
                if (m_paye != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_paye
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_paye
                        });
                    }
                }
                int m_sanavat_basiji = a_sanavat_basiji - (b_sanavat_basiji + c_sanavat_basiji);
                itemid = 4;
                if (m_sanavat_basiji != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_sanavat_basiji
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_sanavat_basiji
                        });
                    }
                }
                int m_sanavat_isar = a_sanavat_isar - (b_sanavat_isar + c_sanavat_isar);
                itemid = 5;
                if (m_sanavat_isar != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_sanavat_isar
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_sanavat_isar
                        });
                    }
                }
                int m_foghshoghl = a_foghshoghl - (b_foghshoghl + c_foghshoghl);
                itemid = 6;
                if (m_foghshoghl != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_foghshoghl
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_foghshoghl
                        });
                    }
                }
                int m_takhasosi = a_takhasosi - (b_takhasosi + c_takhasosi);
                itemid = 7;
                if (m_takhasosi != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_takhasosi
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_takhasosi
                        });
                    }
                }
                int m_made26 = a_made26 - (b_made26 + c_made26);
                itemid = 8;
                if (m_made26 != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_made26
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_made26
                        });
                    }
                }
                int m_modiryati = a_modiryati - (b_modiryati + c_modiryati);
                itemid = 9;
                if (m_modiryati != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_modiryati
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_modiryati
                        });
                    }
                }
                int m_barjastegi = a_barjastegi - (b_barjastegi + c_barjastegi);
                itemid = 10;
                if (m_barjastegi != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_barjastegi
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_barjastegi
                        });
                    }
                }
                int m_tatbigh = a_tatbigh - (b_tatbigh + c_tatbigh);
                itemid = 11;
                if (m_tatbigh != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_tatbigh
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_tatbigh
                        });
                    }
                }
                int m_fogh_isar = a_fogh_isar - (b_fogh_isar + c_fogh_isar);
                itemid = 12;
                if (m_fogh_isar != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_fogh_isar
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_fogh_isar
                        });
                    }
                }
                int m_abohava = a_abohava - (b_abohava + c_abohava);
                itemid = 13;
                if (m_abohava != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_abohava
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_abohava
                        });
                    }
                }
                int m_tashilat = a_tashilat - (b_tashilat + c_tashilat);
                itemid = 14;
                if (m_tashilat != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_tashilat
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_tashilat
                        });
                    }
                }
                int m_sakhtikar = a_sakhtikar - (b_sakhtikar + c_sakhtikar);
                itemid = 15;
                if (m_sakhtikar != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_sakhtikar
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_sakhtikar
                        });
                    }
                }
                int m_tadil = a_tadil - (b_tadil + c_tadil);
                itemid = 16;
                if (m_tadil != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_tadil
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_tadil
                        });
                    }
                }
                int m_riali = a_riali - (b_riali + c_riali);
                itemid = 17;
                if (m_riali != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_riali
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_riali
                        });
                    }
                }
                int m_jazb9 = a_jazb9 - (b_jazb9 + c_jazb9);
                itemid = 18;
                if (m_jazb9 != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_jazb9
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_jazb9
                        });
                    }
                }
                int m_jazb = a_jazb - (b_jazb + c_jazb);
                itemid = 19;
                if (m_jazb != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_jazb
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_jazb
                        });
                    }
                }
                int m_makhsos = a_makhsos - (b_makhsos + c_makhsos);
                itemid = 20;
                if (m_makhsos != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_makhsos
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_makhsos
                        });
                    }
                }
                int m_vije = a_vije - (b_vije + c_vije);
                itemid = 21;
                if (m_vije != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_vije
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_vije
                        });
                    }
                }
                int m_olad = a_olad - (b_olad + c_olad);
                itemid = 22;
                if (m_olad != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_olad
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_olad
                        });
                    }
                }
                int m_ayelemandi = a_ayelemandi - (b_ayelemandi + c_ayelemandi);
                itemid = 23;
                if (m_ayelemandi != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_ayelemandi
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_ayelemandi
                        });
                    }
                }
                int m_kharobar = a_kharobar - (b_kharobar + c_kharobar);
                itemid = 24;
                if (m_kharobar != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_kharobar
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_kharobar
                        });
                    }
                }
                int m_maskan = a_maskan - (b_maskan + c_maskan);
                itemid = 25;
                if (m_maskan != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_maskan
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_maskan
                        });
                    }
                }
                int m_nobatkari = a_nobatkari - (b_nobatkari + c_nobatkari);
                itemid = 26;
                if (m_nobatkari != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_nobatkari
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_nobatkari
                        });
                    }
                }
                int m_bon = a_bon - (b_bon + c_bon);
                itemid = 27;
                if (m_bon != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_bon
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_bon
                        });
                    }
                }
                int m_jazb_tabsare = a_jazb_tabsare - (b_jazb_tabsare + c_jazb_tabsare);
                itemid = 28;
                if (m_jazb_tabsare != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_jazb_tabsare
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_jazb_tabsare
                        });
                    }
                }
                int m_talash = a_talash - (b_talash + c_talash);
                itemid = 29;
                if (m_talash != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_talash
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_talash
                        });
                    }
                }
                int m_jebhe = a_jebhe - (b_jebhe + c_jebhe);
                itemid = 30;
                if (m_jebhe != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_jebhe
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_jebhe
                        });
                    }
                }
                int m_janbazi = a_janbazi - (b_janbazi + c_janbazi);
                itemid = 31;
                if (m_janbazi != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_janbazi
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_janbazi
                        });
                    }
                }
                int m_sayer = a_sayer - (b_sayer + c_sayer);
                itemid = 32;
                if (m_sayer != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_sayer
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_sayer
                        });
                    }
                }
                int m_ezafekar = a_ezafekar - (b_ezafekar + c_ezafekar);
                itemid = 33;
                if (m_ezafekar != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_ezafekar
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_ezafekar
                        });
                    }
                }
                int m_mamoriat = a_mamoriat - (b_mamoriat + c_mamoriat);
                itemid = 34;
                if (m_mamoriat != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_mamoriat
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_mamoriat
                        });
                    }
                }
                int m_tatilkari = a_tatilkari - (b_tatilkari + c_tatilkari);
                itemid = 35;
                if (m_tatilkari != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_tatilkari
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_tatilkari
                        });
                    }
                }
                int m_s_payankhedmat = a_s_payankhedmat - (b_s_payankhedmat + c_s_payankhedmat);
                itemid = 36;
                if (m_s_payankhedmat > 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_s_payankhedmat
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_s_payankhedmat
                        });
                    }
                }
                int m_ghazai = a_ghazai - (b_ghazai + c_ghazai);
                itemid = 37;
                if (m_ghazai != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_ghazai
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_ghazai
                        });
                    }
                }
                int m_ashoora = a_ashoora - (b_ashoora + c_ashoora);
                itemid = 38;
                if (m_ashoora != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_ashoora
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_ashoora
                        });
                    }
                }
                int m_zaribtadil = a_zaribtadil - (b_zaribtadil + c_zaribtadil);
                itemid = 39;
                if (m_zaribtadil != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_zaribtadil
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_zaribtadil
                        });
                    }
                }
                int m_jazbTakhasosi = a_jazbTakhasosi - (b_jazbTakhasosi + c_jazbTakhasosi);
                itemid = 40;
                if (m_jazbTakhasosi != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_jazbTakhasosi
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_jazbTakhasosi
                        });
                    }
                }
                int m_jazbtadil = a_jazbtadil - (b_jazbtadil + c_jazbtadil);
                itemid = 41;
                if (m_jazbtadil != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_jazbtadil
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_jazbtadil
                        });
                    }
                }
                int m_hadaghaltadil = a_hadaghaltadil - (b_hadaghaltadil + c_hadaghaltadil);
                itemid = 42;
                if (m_hadaghaltadil != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_hadaghaltadil
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_hadaghaltadil
                        });
                    }
                }
                int m_shift = a_shift - (b_shift + c_shift);
                itemid = 43;
                if (m_shift != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_shift
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_shift
                        });
                    }
                }
                int m_band_y = a_band_y - (b_band_y + c_band_y);
                itemid = 44;
                if (m_band_y != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_band_y
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_band_y
                        });
                    }
                }
                int m_joz1 = a_joz1 - (b_joz1 + c_joz1);
                itemid = 45;
                if (m_joz1 != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_joz1
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_joz1
                        });
                    }
                }
                int m_band6 = a_band6 - (b_band6 + c_band6);
                itemid = 46;
                if (m_band6 != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_band6
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_band6
                        });
                    }
                }
                int m_jazb2 = a_jazb2 - (b_jazb2 + c_jazb2);
                itemid = 47;
                if (m_jazb2 != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_jazb2
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_jazb2
                        });
                    }
                }
                int m_jazb3 = a_jazb3 - (b_jazb3 + c_jazb3);
                itemid = 48;
                if (m_jazb3 != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_jazb3
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_jazb3
                        });
                    }
                }
                int m_vije2 = a_vije2 - (b_vije2 + c_vije2);
                itemid = 49;
                if (m_vije2 != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_vije2
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_vije2
                        });
                    }
                }
                int m_vije3 = a_vije3 - (b_vije3 + c_vije3);
                itemid = 50;
                if (m_vije3 != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_vije3
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_vije3
                        });
                    }
                }
                int m_makhsos2 = a_makhsos2 - (b_makhsos2 + c_makhsos2);
                itemid = 51;
                if (m_makhsos2 != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_makhsos2
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_makhsos2
                        });
                    }
                }
                int m_makhsos3 = a_makhsos3 - (b_makhsos3 + c_makhsos3);
                itemid = 52;
                if (m_makhsos3 != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_makhsos3
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_makhsos3
                        });
                    }
                }
                int m_tatbigh1 = a_tatbigh1 - (b_tatbigh1 + c_tatbigh1);
                itemid = 53;
                if (m_tatbigh1 != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_tatbigh1
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_tatbigh1
                        });
                    }
                }
                int m_khas = a_khas - (b_khas + c_khas);
                itemid = 54;
                if (m_khas != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_khas
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_khas
                        });
                    }
                }
                int m_karane = a_karane - (b_karane + c_karane);
                itemid = 55;
                if (m_karane != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_karane
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_karane
                        });
                    }
                }
                int m_refahi = a_refahi - (b_refahi + c_refahi);
                itemid = 56;
                if (m_refahi != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_refahi
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_refahi
                        });
                    }
                }
                int m_tarmim = a_tarmim - (b_tarmim + c_tarmim);
                itemid = 57;
                if (m_tarmim != 0)
                {
                    var m2 = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (m2 == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = m2.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_tarmim
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_tarmim
                        });
                    }
                }
                int m_tarmim_j = a_tarmim_j - (b_tarmim_j + c_tarmim_j);
                itemid = 60;
                if (m_tarmim_j != 0)
                {
                    var mj = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (mj == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = mj.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_tarmim_j
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_tarmim_j
                        });
                    }
                }
                int m_tarmim_v = a_tarmim_v - (b_tarmim_v + c_tarmim_v);
                itemid = 61;
                if (m_tarmim_v != 0)
                {
                    var mv = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (mv == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = mv.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_tarmim_v
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_tarmim_v
                        });
                    }
                }
                int m_tarmim_k = a_tarmim_k - (b_tarmim_k + c_tarmim_k);
                itemid = 62;
                if (m_tarmim_k != 0)
                {
                    var mk = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (mk == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = mk.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_tarmim_k
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_tarmim_k
                        });
                    }
                }
                int m_MahadKoodak = a_MahadKoodak - (b_MahadKoodak + c_MahadKoodak);
                itemid = 64;
                if (m_MahadKoodak != 0)
                {
                    var mak = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (mak == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = mak.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_MahadKoodak
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_MahadKoodak
                        });
                    }
                }
                int m_aghlamKhoraki = a_aghlamKhoraki - (b_aghlamKhoraki + c_aghlamKhoraki);
                itemid = 65;
                if (m_aghlamKhoraki != 0)
                {
                    var agh = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (agh == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = agh.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_aghlamKhoraki
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_aghlamKhoraki
                        });
                    }
                }
                int m_Monasebat = a_Monasebat - (b_Monasebat + c_Monasebat);
                itemid = 67;
                if (m_Monasebat != 0)
                {
                    var mo = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (mo == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = mo.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_Monasebat,
                            fldSourceId=sourceId_m
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_Monasebat,
                            fldSourceId = sourceId_m
                        });
                    }
                }
                int m_JavaniJameiyat = a_JavaniJameiyat - (b_JavaniJameiyat + c_JavaniJameiyat);
                itemid = 68;
                if (m_JavaniJameiyat != 0)
                {
                    var j = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault();
                    if (j == null)
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = j.fldItemEstekhdamId,
                            fldItemHoghoghiId = itemid,
                            fldMablagh = m_JavaniJameiyat,
                            fldSourceId = sourceId_j
                        });
                    }
                    else
                    {
                        mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                        {
                            fldItemEstekhdamId = cha1.Items.Where(k => k.fldItemHoghoghiId == itemid).FirstOrDefault().fldItemEstekhdamId,
                            fldItemHoghoghiId = 32,
                            fldMablagh = m_JavaniJameiyat,
                            fldSourceId = sourceId_j
                        });
                    }
                }
                //         Mohasebat cha = new Mohasebat();
                //         Mohasebat chb = new Mohasebat();
                //         Mohasebat chc = new Mohasebat();
                //         cha = mohasebe;
                //         decimal h_d_k = 0;
                //         decimal h_d_p = 0;
                //         decimal b_p = 0, b_k = 0, b_b = 0;
                //         decimal old_mazad = 0, old_takafol = 0;
                //         decimal new_mazad = 0, new_takafol = 0;
                //         bool mazad_30sal = false;
                //         int vaziat_jesmi = 0;
                //         string commandText = "SELECT Pay.tblMohasebat_Items.fldMablagh, Com.tblItems_Estekhdam.fldItemsHoghughiId, Com.tblItems_Estekhdam.fldId AS fldItems_EstekhdamId " +
                //           "FROM Pay.tblMohasebat INNER JOIN Pay.tblMohasebat_Items ON Pay.tblMohasebat.fldId = Pay.tblMohasebat_Items.fldMohasebatId INNER JOIN" +
                //           " Com.tblItems_Estekhdam ON Pay.tblMohasebat_Items.fldItemEstekhdamId = Com.tblItems_Estekhdam.fldId where" +
                //           " Pay.tblMohasebat.fldPersonalId=" + payId + " and Pay.tblMohasebat.fldYear=" + sal + " and Pay.tblMohasebat.fldMonth=" + mah;
                //         NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                //         var tblMohasebe = service.GetData(commandText).Tables[0];
                //         int s = 0;
                //         int e_type = 0;
                //         if (newHokm.NoeEstekhdam <= 5)
                //           e_type = 1;
                //         else if (newHokm.NoeEstekhdam == 6)
                //           e_type = 2;
                //         else if (newHokm.NoeEstekhdam >= 7 && newHokm.NoeEstekhdam <= 9)
                //           e_type = 3;
                //         else e_type = 4;
                //         if (tblMohasebe.Rows.Count > 0)
                //         {
                //           /*for (int i = 0; i < tblMohasebe.Rows.Count; i++)
                // {
                // chb.Items.Add(new Mohasebat_Items
                // {
                // fldItemEstekhdamId = Convert.ToInt32(tblMohasebe.Rows[i]["fldItems_EstekhdamId"]),
                // fldItemHoghoghiId = Convert.ToInt32(tblMohasebe.Rows[i]["fldItemsHoghughiId"]),
                // fldMablagh = Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"])
                // });
                // }*/
                //           string com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId=32 and fldTypeEstekhdamId=" + e_type;
                //           var mm2 = service.GetData(com1).Tables[0];
                //           chb.Items.Add(new Mohasebat_Items
                //                         {
                //             fldItemEstekhdamId = Convert.ToInt32(mm2.Rows[0]["fldId"]),
                //             fldItemHoghoghiId = 32,
                //             fldMablagh = 0
                //           });
                //           for (int i = 0; i < tblMohasebe.Rows.Count; i++)
                //           {
                //             if (Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"]) > 0)
                //             {
                //               int item_id=Convert.ToInt32(tblMohasebe.Rows[i]["fldItemsHoghughiId"]);
                //               if(item_id==1 || item_id==2 || item_id==15 || item_id==19 || item_id==22 || item_id==25 || item_id==26 || item_id==32 || item_id==33 || item_id==34 || item_id==35 || item_id==36 ||item_id==56)
                //               {//همه
                //                 com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId="+item_id+" and fldTypeEstekhdamId=" + e_type;
                //                 var mm1 = service.GetData(com1).Tables[0];
                //                 chb.Items.Add(new Mohasebat_Items
                //                               {
                //                   fldItemEstekhdamId = Convert.ToInt32(mm1.Rows[0]["fldId"]),
                //                   fldItemHoghoghiId =item_id,
                //                   fldMablagh = Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"])
                //                 });
                //               }
                //               else if(item_id==3 || item_id==24 || item_id==27 || item_id==38 ||item_id==30||item_id==31||item_id==38){
                //                 if (e_type == 1)
                //                 {//فقط کارگری
                //                   com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId="+item_id+" and fldTypeEstekhdamId=" + e_type;
                //                   var mm1 = service.GetData(com1).Tables[0];
                //                   chb.Items.Add(new Mohasebat_Items
                //                                 {
                //                     fldItemEstekhdamId = Convert.ToInt32(mm1.Rows[0]["fldId"]),
                //                     fldItemHoghoghiId =item_id,
                //                     fldMablagh = Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"])
                //                   });
                //                 }
                //                 else
                //                 {
                //                   var sayer = chb.Items.Where(k => k.fldItemEstekhdamId == (int)mm2.Rows[0]["fldId"]).FirstOrDefault();
                //                   sayer.fldMablagh += Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"]);
                //                 }                                                              
                //               }
                //               else if(item_id==4 || item_id==5 || item_id==6 || item_id==7 || item_id==8 || item_id==9 || item_id==10 || item_id==11 || item_id==12 || item_id==13 || item_id==14 || item_id==16 || item_id==17 || item_id==18 || item_id==21 || item_id==23 || item_id==28 || item_id==29 || item_id==37 || item_id==39 || item_id==40 || item_id==41 || item_id==42 || item_id==43 || item_id==44 || item_id==45 || item_id==46 || item_id==47 || item_id==48 || item_id==49 || item_id==50 || item_id==53|| item_id==54||item_id==55||item_id==56){
                //                 if (e_type != 1)
                //                 {//کارمند رسمی و پیمانی و شهردار
                //                   com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId="+item_id+" and fldTypeEstekhdamId=" + e_type;
                //                   var mm1 = service.GetData(com1).Tables[0];
                //                   chb.Items.Add(new Mohasebat_Items
                //                                 {
                //                     fldItemEstekhdamId = Convert.ToInt32(mm1.Rows[0]["fldId"]),
                //                     fldItemHoghoghiId =item_id,
                //                     fldMablagh = Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"])
                //                   });
                //                 }
                //                 else
                //                 {
                //                   var sayer = chb.Items.Where(k => k.fldItemEstekhdamId == (int)mm2.Rows[0]["fldId"]).FirstOrDefault();
                //                   sayer.fldMablagh += Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"]);
                //                 }                                                              
                //               }
                //               else if(item_id==20) {
                //                 if (e_type >2)
                //                 {//پیمانی و شهردار
                //                   com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId="+item_id+" and fldTypeEstekhdamId=" + e_type;
                //                   var mm1 = service.GetData(com1).Tables[0];
                //                   chb.Items.Add(new Mohasebat_Items
                //                                 {
                //                     fldItemEstekhdamId = Convert.ToInt32(mm1.Rows[0]["fldId"]),
                //                     fldItemHoghoghiId =item_id,
                //                     fldMablagh = Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"])
                //                   });
                //                 }
                //                 else
                //                 {
                //                   var sayer = chb.Items.Where(k => k.fldItemEstekhdamId == (int)mm2.Rows[0]["fldId"]).FirstOrDefault();
                //                   sayer.fldMablagh += Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"]);
                //                 }                                                              
                //               }
                //               else if(item_id==51||item_id==52) {
                //                 if (e_type ==3)
                //                 {//پیمانی 
                //                   com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId="+item_id+" and fldTypeEstekhdamId=" + e_type;
                //                   var mm1 = service.GetData(com1).Tables[0];
                //                   chb.Items.Add(new Mohasebat_Items
                //                                 {
                //                     fldItemEstekhdamId = Convert.ToInt32(mm1.Rows[0]["fldId"]),
                //                     fldItemHoghoghiId =item_id,
                //                     fldMablagh = Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"])
                //                   });
                //                 }
                //                 else
                //                 {
                //                   var sayer = chb.Items.Where(k => k.fldItemEstekhdamId == (int)mm2.Rows[0]["fldId"]).FirstOrDefault();
                //                   sayer.fldMablagh += Convert.ToInt32(tblMohasebe.Rows[i]["fldMablagh"]);
                //                 }                                                              
                //               }
                //             }            
                //           }          
                //           string com = "select * from Com.tblItems_Estekhdam where fldTypeEstekhdamId=" + e_type;
                //           var mm = service.GetData(com).Tables[0];
                //           for (int i = 0; i < mm.Rows.Count; i++)
                //           {
                //             var m1 = chb.Items.Where(k => k.fldItemEstekhdamId == (int)mm.Rows[i]["fldId"]).FirstOrDefault();
                //             if (m1 == null)
                //               chb.Items.Add(new Mohasebat_Items
                //                             {
                //                 fldItemEstekhdamId = (int)mm.Rows[i]["fldId"],
                //                 fldItemHoghoghiId = (int)mm.Rows[i]["fldItemsHoghughiId"],
                //                 fldMablagh = 0
                //               });
                //             var m2 = cha.Items.Where(k => k.fldItemEstekhdamId == (int)mm.Rows[i]["fldId"]).FirstOrDefault();
                //             if (m2 == null)
                //               cha.Items.Add(new Mohasebat_Items
                //                             {
                //                 fldItemEstekhdamId = (int)mm.Rows[i]["fldId"],
                //                 fldItemHoghoghiId = (int)mm.Rows[i]["fldItemsHoghughiId"],
                //                 fldMablagh = 0
                //               });
                //           }
                //           commandText = "SELECT Pay.tblMohasebat_PersonalInfo.fldMazad30Sal, Pay.tblMohasebat_PersonalInfo.fldStatusIsargariId,Pay.tblMohasebat_PersonalInfo.fldAnvaEstekhdamId, Pay.tblMohasebat_PersonalInfo.fldHokmId, " +
                //             "Pay.tblMohasebat_PersonalInfo.fldFiscalHeaderId,pay.tblMohasebat.fldMaliyat,pay.tblMohasebat.fldMashmolBime,pay.tblMohasebat.fldBimePersonal,pay.tblMohasebat.fldBimeKarFarma,pay.tblMohasebat.fldBimeBikari,"+
                //             "pay.tblMohasebat.fldMashmolMaliyat, Pay.tblMohasebat_PersonalInfo.fldMoteghayerHoghoghiId,pay.tblMohasebat.fldHaghDarman,pay.tblMohasebat.fldHaghDarmanKarfFarma,pay.tblMohasebat.fldHaghDarmanDolat,pay.tblMohasebat.fldPasAndaz FROM Pay.tblMohasebat " +
                //             "INNER JOIN Pay.tblMohasebat_PersonalInfo ON Pay.tblMohasebat.fldId = Pay.tblMohasebat_PersonalInfo.fldMohasebatId WHERE " +
                //             "Pay.tblMohasebat.fldPersonalId=" + payId + " and Pay.tblMohasebat.fldYear=" + sal + " and Pay.tblMohasebat.fldMonth=" + mah;
                //           var tblMohasebe1 = service.GetData(commandText).Tables[0];
                //           mazad_30sal = Convert.ToBoolean(tblMohasebe1.Rows[0]["fldMazad30Sal"]);
                //           vaziat_jesmi = Convert.ToInt32(tblMohasebe1.Rows[0]["fldStatusIsargariId"]);
                //           int _group = Convert.ToInt32(tblMohasebe1.Rows[0]["fldFiscalHeaderId"]);
                //           int old_moteghayerid = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMoteghayerHoghoghiId"]);
                //           int old_hokmid = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHokmId"]);
                //           commandText = "SELECT     fldHaghDarmanMazad, fldHaghDarmanTahteTakaffol FROM         Pay.tblMoteghayerhayeHoghoghi WHERE     (fldId = " + old_moteghayerid + ")";
                //           var oldMoteghayer = service.GetData(commandText).Tables[0];
                //           if (oldMoteghayer.Rows.Count > 0)
                //           {
                //             var oldhokm = getHokmByid(old_hokmid);
                //             int tedad_afradekhanevade = 0, t_takafol_60 = 0, t_takafol_70 = 0, t_takafol_60_t = 0, t_takafol_70_t = 0;
                //             switch (oldhokm.NoeTaahol)
                //             {
                //               case 1:
                //                 tedad_afradekhanevade = 1;
                //                 break;
                //               case 2:
                //                 tedad_afradekhanevade = 1 + oldhokm.TedadFarzand;
                //                 break;
                //               case 3:
                //                 tedad_afradekhanevade = 2;
                //                 break;
                //               case 4:
                //                 tedad_afradekhanevade = 2 + oldhokm.TedadFarzand;
                //                 break;
                //             }
                //             t_takafol_60 = oldhokm.TedadAfradTakafol;
                //             old_mazad = Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanMazad"]);
                //             old_takafol = Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanTahteTakaffol"]);
                //             int tedad_t_takalof = 0;
                //             int ezafe_farzand = 0;
                //             if (PersonalInfo.fldHamsarKarmand == true)
                //               tedad_afradekhanevade = tedad_afradekhanevade - 1;
                //             tedad_t_takalof = t_takafol_60 + t_takafol_70;
                //             if (anvaEstekhdam <= 10 && anvaEstekhdam >= 1 && typebime == 2)
                //             {
                //               if (oldhokm.TedadFarzand > 3 && PersonalInfo.fldJensiyat == 1)
                //               {
                //                 if (sal >= 1392 && mah >= 9)
                //                   old_mazad = 0;
                //                 else if (sal > 1392)
                //                   old_mazad = 0;
                //                 else
                //                 {
                //                   ezafe_farzand = oldhokm.TedadFarzand - 3;
                //                   old_mazad = old_mazad * Convert.ToDecimal(ezafe_farzand);
                //                 }
                //               }
                //               else if (PersonalInfo.fldJensiyat == 2)//------تغییرات
                //               {//------تغییرات
                //                 ezafe_farzand = tedad_afradekhanevade - 1;//------تغییرات
                //                 old_mazad = old_mazad * Convert.ToDecimal(ezafe_farzand);//------تغییرات
                //               }//------تغییرات
                //               else
                //                 old_mazad = 0;
                //               commandText = "select count(*) as tedad from [Prs].[tblAfradTahtePooshesh]  where fldNesbat=4 and [fldPersonalId]=" + prsId;
                //               var Tabae = service.GetData(commandText).Tables[0];
                //               if (Tabae.Rows.Count > 0)
                //               {
                //                 old_mazad += Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanMazad"]) * Convert.ToDecimal(Tabae.Rows[0]["tedad"]);
                //               }
                //               old_takafol = old_takafol * Convert.ToDecimal(tedad_t_takalof);
                //             }
                //             else
                //             {
                //               old_takafol = old_mazad = 0;
                //             }
                //           }
                //           if (newMoteghayer != null)
                //           {
                //             int tedad_afradekhanevade = 0, t_takafol_60 = 0, t_takafol_70 = 0, t_takafol_60_t = 0, t_takafol_70_t = 0;
                //             switch (newHokm.NoeTaahol)
                //             {
                //               case 1:
                //                 tedad_afradekhanevade = 1;
                //                 break;
                //               case 2:
                //                 tedad_afradekhanevade = 1 + newHokm.TedadFarzand;
                //                 break;
                //               case 3:
                //                 tedad_afradekhanevade = 2;
                //                 break;
                //               case 4:
                //                 tedad_afradekhanevade = 2 + newHokm.TedadFarzand;
                //                 break;
                //             }
                //             t_takafol_60 = newHokm.TedadAfradTakafol;
                //             new_mazad = Convert.ToDecimal(newMoteghayer.fldHaghDarmanMazad);
                //             new_takafol = Convert.ToDecimal(newMoteghayer.fldHaghDarmanTahteTakaffol);
                //             int tedad_t_takalof = 0;
                //             int ezafe_farzand = 0;
                //             if (PersonalInfo.fldHamsarKarmand == true)
                //               tedad_afradekhanevade = tedad_afradekhanevade - 1;
                //             tedad_t_takalof = t_takafol_60 + t_takafol_70;
                //             if (anvaEstekhdam <= 10 && anvaEstekhdam >= 1 && typebime == 2)
                //             {
                //               if (newHokm.TedadFarzand > 3 && PersonalInfo.fldJensiyat == 1)
                //               {
                //                 if (sal >= 1392 && mah >= 9)
                //                   new_mazad = 0;
                //                 else if (sal > 1392)
                //                   new_mazad = 0;
                //                 else
                //                 {
                //                   ezafe_farzand = newHokm.TedadFarzand - 3;
                //                   new_mazad = new_mazad * Convert.ToDecimal(ezafe_farzand);
                //                 }
                //               }
                //               else if (PersonalInfo.fldJensiyat == 2)//------تغییرات
                //               {//------تغییرات
                //                 ezafe_farzand = tedad_afradekhanevade - 1;//------تغییرات
                //                 new_mazad = new_mazad * Convert.ToDecimal(ezafe_farzand);//------تغییرات
                //               }//------تغییرات
                //               else
                //                 new_mazad = 0;
                //               commandText = "select count(*) as tedad from [Prs].[tblAfradTahtePooshesh]  where fldNesbat=4 and [fldPersonalId]=" + prsId;
                //               var Tabae = service.GetData(commandText).Tables[0];
                //               if (Tabae.Rows.Count > 0)
                //               {
                //                 new_mazad += Convert.ToDecimal(oldMoteghayer.Rows[0]["fldHaghDarmanMazad"]) * Convert.ToDecimal(Tabae.Rows[0]["tedad"]);
                //               }
                //               new_takafol = new_takafol * Convert.ToDecimal(tedad_t_takalof);
                //             }
                //             else
                //             {
                //               new_takafol = new_mazad = 0;
                //             }
                //           }
                //           chb.fldMashmolMaliyat = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMashmolMaliyat"]);
                //           //--------------------مالیات  
                //           chb.fldMaliyat = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMaliyat"]); //maliyat;
                //           //mashmol_bime = mashmol_bime - hog.mogharari;
                //           //--------------------حق بیمه
                //           chb.fldMashmolBime = Convert.ToInt32(tblMohasebe1.Rows[0]["fldMashmolBime"]);
                //           chb.fldBimeBikari = Convert.ToInt32(tblMohasebe1.Rows[0]["fldBimeBikari"]);
                //           chb.fldBimeKarFarma = Convert.ToInt32(tblMohasebe1.Rows[0]["fldBimeKarFarma"]);
                //           chb.fldBimePersonal = Convert.ToInt32(tblMohasebe1.Rows[0]["fldBimePersonal"]);
                //           chb.fldHaghDarman = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHaghDarman"]);
                //           chb.fldHaghDarmanKarfFarma = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHaghDarmanKarfFarma"]);
                //           chb.fldHaghDarmanDolat = Convert.ToInt32(tblMohasebe1.Rows[0]["fldHaghDarmanDolat"]);
                //           chb.fldPasAndaz = Convert.ToInt32(tblMohasebe1.Rows[0]["fldPasAndaz"]);
                //         }
                //         commandText = "SELECT isnull(sum(Pay.tblMoavaghat.fldHaghDarmanKarfFarma),0)as fldHaghDarmanKarfFarma, " +
                //           "isnull(sum(Pay.tblMoavaghat.fldHaghDarmanDolat),0)as fldHaghDarmanDolat, isnull(sum(Pay.tblMoavaghat.fldHaghDarman),0)" +
                //           "as fldHaghDarman, isnull(sum(Pay.tblMoavaghat.fldBimePersonal),0)as fldBimePersonal, isnull(sum(Pay.tblMoavaghat.fldBimeKarFarma)" +
                //           ",0)as fldBimeKarFarma, isnull(sum(Pay.tblMoavaghat.fldBimeBikari),0)as fldBimeBikari, isnull(sum(Pay.tblMoavaghat.fldBimeMashaghel),0)as" +
                //           " fldBimeMashaghel, isnull(sum(Pay.tblMoavaghat.fldPasAndaz),0)as fldPasAndaz, isnull(sum(Pay.tblMoavaghat.fldMashmolBime),0)" +
                //           "as fldMashmolBime, isnull(sum(Pay.tblMoavaghat.fldMashmolMaliyat),0)as fldMashmolMaliyat, isnull(sum(Pay.tblMoavaghat.fldMaliyat),0)as" +
                //           " fldMaliyat FROM Pay.tblMoavaghat INNER JOIN Pay.tblMohasebat ON Pay.tblMoavaghat.fldMohasebatId = Pay.tblMohasebat.fldId " +
                //           "WHERE (Pay.tblMoavaghat.fldYear = " + sal + ") AND (Pay.tblMoavaghat.fldMonth = " + mah + ") AND (Pay.tblMohasebat.fldPersonalId = " + payId + ")";
                //         var mov = service.GetData(commandText).Tables[0];
                //         if (mov.Rows.Count > 0)
                //         {
                //           chc.fldHaghDarman = Convert.ToInt32(mov.Rows[0]["fldHaghDarman"]);
                //           chc.fldHaghDarmanKarfFarma = Convert.ToInt32(mov.Rows[0]["fldHaghDarmanKarfFarma"]);
                //           chc.fldHaghDarmanDolat = Convert.ToInt32(mov.Rows[0]["fldHaghDarmanDolat"]);
                //           chc.fldBimeKarFarma = Convert.ToInt32(mov.Rows[0]["fldBimeKarFarma"]);
                //           chc.fldBimePersonal = Convert.ToInt32(mov.Rows[0]["fldBimePersonal"]);
                //           chc.fldBimeBikari = Convert.ToInt32(mov.Rows[0]["fldBimeBikari"]);
                //           chc.fldMaliyat = Convert.ToInt32(mov.Rows[0]["fldMaliyat"]);
                //           chc.fldMashmolBime = Convert.ToInt32(mov.Rows[0]["fldMashmolBime"]);
                //           chc.fldMashmolMaliyat = Convert.ToInt32(mov.Rows[0]["fldMashmolMaliyat"]);
                //           chc.fldPasAndaz = Convert.ToInt32(mov.Rows[0]["fldPasAndaz"]);
                //           commandText = "SELECT Pay.tblMoavaghat_Items.fldItemEstekhdamId, SUM(Pay.tblMoavaghat_Items.fldMablagh) AS fldMablagh," +
                //             " Com.tblItems_Estekhdam.fldItemsHoghughiId FROM Pay.tblMoavaghat_Items INNER JOIN Pay.tblMoavaghat INNER JOIN " +
                //             "Pay.tblMohasebat ON Pay.tblMoavaghat.fldMohasebatId = Pay.tblMohasebat.fldId ON Pay.tblMoavaghat_Items.fldMoavaghatId =" +
                //             " Pay.tblMoavaghat.fldId INNER JOIN Com.tblItems_Estekhdam ON Pay.tblMoavaghat_Items.fldItemEstekhdamId = Com.tblItems_Estekhdam.fldId WHERE " +
                //             "(Pay.tblMoavaghat.fldYear = " + sal + ") AND (Pay.tblMoavaghat.fldMonth = " + mah + ") AND (Pay.tblMohasebat.fldPersonalId = " + payId + ") " +
                //             "GROUP BY Pay.tblMoavaghat_Items.fldItemEstekhdamId, Com.tblItems_Estekhdam.fldItemsHoghughiId ORDER BY Com.tblItems_Estekhdam.fldItemsHoghughiId";
                //           var mov_items = service.GetData(commandText).Tables[0];
                //           if (mov_items.Rows.Count > 0)
                //           {
                //             var com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId=32 and fldTypeEstekhdamId=" + e_type;
                //             var mm2 = service.GetData(com1).Tables[0];
                //             chc.Items.Add(new Mohasebat_Items
                //                           {
                //               fldItemEstekhdamId = Convert.ToInt32(mm2.Rows[0]["fldId"]),
                //               fldItemHoghoghiId = 32,
                //               fldMablagh = 0
                //             });
                //             /*for (int i = 0; i < mov_items.Rows.Count; i++)
                // {
                // chc.Items.Add(new Mohasebat_Items
                // {
                // fldItemEstekhdamId = Convert.ToInt32(mov_items.Rows[i]["fldItemEstekhdamId"]),
                // fldItemHoghoghiId = Convert.ToInt32(mov_items.Rows[i]["fldItemsHoghughiId"]),
                // fldMablagh = Convert.ToInt32(mov_items.Rows[i]["fldMablagh"])
                // });
                // }*/
                //             for (int i = 0; i < mov_items.Rows.Count; i++)
                //             {
                //               if (Convert.ToInt32(mov_items.Rows[i]["fldMablagh"]) != 0)
                //               {
                //                 int item_id=Convert.ToInt32(mov_items.Rows[i]["fldItemsHoghughiId"]);
                //                 if(item_id==1 || item_id==2 || item_id==15 || item_id==19 || item_id==22 || item_id==25 || item_id==26 || item_id==32 ||item_id==33 || item_id==34 || item_id==35 || item_id==36||item_id==56)
                //                 {//همه                                
                //                   com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId="+item_id+" and fldTypeEstekhdamId=" + e_type;
                //                   var mm1 = service.GetData(com1).Tables[0];
                //                   chc.Items.Add(new Mohasebat_Items
                //                                 {
                //                     fldItemEstekhdamId = Convert.ToInt32(mm1.Rows[0]["fldId"]),
                //                     fldItemHoghoghiId =item_id,
                //                     fldMablagh = Convert.ToInt32(mov_items.Rows[i]["fldMablagh"])
                //                   });
                //                   if(item_id==32){
                //                     var sayer1 = chc.Items.Where(k => k.fldItemEstekhdamId == (int)mm2.Rows[0]["fldId"]).FirstOrDefault();
                //                     sayer1.fldMablagh += Convert.ToInt32(mov_items.Rows[i]["fldMablagh"]);
                //                   }
                //                 }
                //                 else if(item_id==3 || item_id==24 || item_id==27 || item_id==38 ||item_id==30||item_id==31||item_id==38){
                //                   if (e_type == 1)
                //                   {//فقط کارگری
                //                     com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId="+item_id+" and fldTypeEstekhdamId=" + e_type;
                //                     var mm1 = service.GetData(com1).Tables[0];
                //                     chc.Items.Add(new Mohasebat_Items
                //                                   {
                //                       fldItemEstekhdamId = Convert.ToInt32(mm1.Rows[0]["fldId"]),
                //                       fldItemHoghoghiId =item_id,
                //                       fldMablagh = Convert.ToInt32(mov_items.Rows[i]["fldMablagh"])
                //                     });
                //                   }
                //                   else
                //                   {
                //                     var sayer = chc.Items.Where(k => k.fldItemEstekhdamId == (int)mm2.Rows[0]["fldId"]).FirstOrDefault();
                //                     sayer.fldMablagh += Convert.ToInt32(mov_items.Rows[i]["fldMablagh"]);
                //                   }                                                              
                //                 }                          
                //                 else if(item_id==4 || item_id==5 || item_id==6 || item_id==7 || item_id==8 || item_id==9 || item_id==10 || item_id==11 || item_id==12 || item_id==13 || item_id==14 || item_id==16 || item_id==17 || item_id==18 || item_id==21 || item_id==23 || item_id==28 || item_id==29 || item_id==37 || item_id==39 || item_id==40 || item_id==41 || item_id==42 || item_id==43 || item_id==44 || item_id==45 || item_id==46 || item_id==47 || item_id==48 || item_id==49 || item_id==50|| item_id==53|| item_id==54||item_id==55||item_id==56){
                //                   if (e_type != 1)
                //                   {//کارمند رسمی و پیمانی و شهردار
                //                     com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId="+item_id+" and fldTypeEstekhdamId=" + e_type;
                //                     var mm1 = service.GetData(com1).Tables[0];
                //                     chc.Items.Add(new Mohasebat_Items
                //                                   {
                //                       fldItemEstekhdamId = Convert.ToInt32(mm1.Rows[0]["fldId"]),
                //                       fldItemHoghoghiId =item_id,
                //                       fldMablagh = Convert.ToInt32(mov_items.Rows[i]["fldMablagh"])
                //                     });
                //                   }
                //                   else
                //                   {
                //                     var sayer = chc.Items.Where(k => k.fldItemEstekhdamId == (int)mm2.Rows[0]["fldId"]).FirstOrDefault();
                //                     sayer.fldMablagh += Convert.ToInt32(mov_items.Rows[i]["fldMablagh"]);
                //                   }                                                              
                //                 }
                //                 else if(item_id==20) {
                //                   if (e_type >2)
                //                   {//پیمانی و شهردار
                //                     com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId="+item_id+" and fldTypeEstekhdamId=" + e_type;
                //                     var mm1 = service.GetData(com1).Tables[0];
                //                     chc.Items.Add(new Mohasebat_Items
                //                                   {
                //                       fldItemEstekhdamId = Convert.ToInt32(mm1.Rows[0]["fldId"]),
                //                       fldItemHoghoghiId =item_id,
                //                       fldMablagh = Convert.ToInt32(mov_items.Rows[i]["fldMablagh"])
                //                     });
                //                   }
                //                   else
                //                   {
                //                     var sayer = chc.Items.Where(k => k.fldItemEstekhdamId == (int)mm2.Rows[0]["fldId"]).FirstOrDefault();
                //                     sayer.fldMablagh += Convert.ToInt32(mov_items.Rows[i]["fldMablagh"]);
                //                   }                                                              
                //                 }
                //                 else if(item_id==51||item_id==52) {
                //                   if (e_type ==3)
                //                   {//پیمانی 
                //                     com1 = "select * from Com.tblItems_Estekhdam where fldItemsHoghughiId="+item_id+" and fldTypeEstekhdamId=" + e_type;
                //                     var mm1 = service.GetData(com1).Tables[0];
                //                     chc.Items.Add(new Mohasebat_Items
                //                                   {
                //                       fldItemEstekhdamId = Convert.ToInt32(mm1.Rows[0]["fldId"]),
                //                       fldItemHoghoghiId =item_id,
                //                       fldMablagh = Convert.ToInt32(mov_items.Rows[i]["fldMablagh"])
                //                     });
                //                   }
                //                   else
                //                   {
                //                     var sayer = chc.Items.Where(k => k.fldItemEstekhdamId == (int)mm2.Rows[0]["fldId"]).FirstOrDefault();
                //                     sayer.fldMablagh += Convert.ToInt32(mov_items.Rows[i]["fldMablagh"]);
                //                   }                                                              
                //                 }
                //               }
                //             }
                //             string com = "select * from Com.tblItems_Estekhdam where fldTypeEstekhdamId=" + e_type;
                //             var mm = service.GetData(com).Tables[0];
                //             for (int i = 0; i < mm.Rows.Count; i++)
                //             {
                //               var m1 = chc.Items.Where(k => k.fldItemEstekhdamId == (int)mm.Rows[i]["fldId"]).FirstOrDefault();
                //               if (m1 == null)
                //                 chc.Items.Add(new Mohasebat_Items
                //                               {
                //                   fldItemEstekhdamId = (int)mm.Rows[i]["fldId"],
                //                   fldItemHoghoghiId = (int)mm.Rows[i]["fldItemsHoghughiId"],
                //                   fldMablagh = 0
                //                 });
                //             }
                //           }
                //         }
                //         mohasebe1.Movaghat.Add(new Movaghat
                //                                {
                //           fldBimeBikari = cha.fldBimeBikari - (chb.fldBimeBikari + chc.fldBimeBikari),
                //           fldBimeKarFarma = cha.fldBimeKarFarma - (chb.fldBimeKarFarma + chc.fldBimeKarFarma),
                //           fldBimePersonal = cha.fldBimePersonal - (chb.fldBimePersonal + chc.fldBimePersonal),
                //           fldHaghDarman = cha.fldHaghDarman - (chb.fldHaghDarman + chc.fldHaghDarman),
                //           fldHaghDarmanDolat = cha.fldHaghDarmanDolat - (chb.fldHaghDarmanDolat + chc.fldHaghDarmanDolat),
                //           fldHaghDarmanKarfFarma = cha.fldHaghDarmanKarfFarma - (chb.fldHaghDarmanKarfFarma + chc.fldHaghDarmanKarfFarma),
                //           fldMaliyat = cha.fldMaliyat - (chb.fldMaliyat + chc.fldMaliyat),
                //           fldMashmolBime = cha.fldMashmolBime - (chb.fldMashmolBime + chc.fldMashmolBime),
                //           fldMashmolMaliyat = cha.fldMashmolMaliyat - (chb.fldMashmolMaliyat + chc.fldMashmolMaliyat),
                //           fldPasAndaz = cha.fldPasAndaz - (chb.fldPasAndaz + chc.fldPasAndaz),
                //           fldMonth = mah,
                //           fldYear = sal
                //         });
                //         foreach (var item in cha.Items)
                //         {
                //           var chbitem = chb.Items.Where(k => k.fldItemEstekhdamId == item.fldItemEstekhdamId && k.fldItemHoghoghiId == item.fldItemHoghoghiId).FirstOrDefault();
                //           var chcitem = chc.Items.Where(k => k.fldItemEstekhdamId == item.fldItemEstekhdamId && k.fldItemHoghoghiId == item.fldItemHoghoghiId).FirstOrDefault();
                //           int moavagh=0;
                //           if(chcitem!=null)
                //             moavagh = item.fldMablagh - (chbitem.fldMablagh + chcitem.fldMablagh);
                //           else
                //             moavagh=item.fldMablagh - (chbitem.fldMablagh);
                //           if (moavagh != 0)
                //             mohasebe1.Movaghat[mohasebe1.Movaghat.Count - 1].Items.Add(new Moavaghat_Items
                //                                                                        {
                //               fldItemEstekhdamId = item.fldItemEstekhdamId,
                //               fldItemHoghoghiId = item.fldItemHoghoghiId,
                //               fldMablagh = moavagh
                //             });
                //         }
                //ch.hoghogh = cha.hoghogh - (chb.hoghogh + chc.hoghogh);
                //ch.pasandaz = cha.pasandaz * 2 - (chb.pasandaz + chc.pasandaz);
                //ch.sanavat = cha.sanavat - (chb.sanavat + chc.sanavat);
                //ch.fogh_shoghl = cha.fogh_shoghl - (chb.fogh_shoghl + chc.fogh_shoghl);
                //ch.tafavot_tatbigh = cha.tafavot_tatbigh - (chb.tafavot_tatbigh + chc.tafavot_tatbigh);
                //ch.hadaghalTadil = cha.hadaghalTadil - (chb.hadaghalTadil + chc.hadaghalTadil);
                //ch.hagholad = cha.hagholad - (chb.hagholad + chc.hagholad);
                //ch.arzeshyabi = cha.arzeshyabi - (chb.arzeshyabi + chc.arzeshyabi);
                //ch.ayelemandi = cha.ayelemandi - (chb.ayelemandi + chc.ayelemandi);
                //ch.nobatkari = cha.nobatkari - (chb.nobatkari + chc.nobatkari);
                //ch.maskan = cha.maskan - (chb.maskan + chc.maskan);
                //ch.sakhtikar = cha.sakhtikar - (chb.sakhtikar + chc.sakhtikar);
                //ch.kharobar = cha.kharobar - (chb.kharobar + chc.kharobar);
                //ch.jazb = cha.jazb - (chb.jazb + chc.jazb);
                //ch.jazbTakhasosi = cha.jazbTakhasosi - (chb.jazbTakhasosi + chc.jazbTakhasosi);
                //ch.zajbTadil = cha.zajbTadil - (chb.zajbTadil + chc.zajbTadil);
                //ch.tashilatzendegi = cha.tashilatzendegi - (chb.tashilatzendegi + chc.tashilatzendegi);
                //ch.badiabohava = cha.badiabohava - (chb.badiabohava + chc.badiabohava);
                //ch.tarmim = cha.tarmim - (chb.tarmim + chc.tarmim);
                //ch.mashmol_bime = cha.mashmol_bime - (chb.mashmol_bime + chc.mashmol_bime);
                //ch.darsad_p = cha.darsad_p;
                //ch.darsad_k = cha.darsad_k;
                //if ((vaziat_jesmi == 1 || vaziat_jesmi == 4) && mazad_30sal == false)
                //{
                //    ch.bime_karfarma = Math.Round(ch.mashmol_bime * ch.darsad_k / 100);
                //    ch.bime_persenel = Math.Round((ch.mashmol_bime) * ch.darsad_p / 100);
                //    ch.bime_bikari = Math.Round(ch.mashmol_bime * b_b / 100);
                //}
                //else if (mazad_30sal == true && (vaziat_jesmi == 1 || vaziat_jesmi == 4))
                //{
                //    if (p.nobime == 2)
                //    {
                //        ch.bime_karfarma = Math.Round(ch.mashmol_bime * ch.darsad_k / 100);
                //        ch.bime_persenel = 0;
                //        ch.bime_bikari = Math.Round(ch.mashmol_bime * b_b / 100);
                //    }
                //    else
                //    {
                //        ch.bime_karfarma = Math.Round(ch.mashmol_bime * ch.darsad_k / 100);
                //        ch.bime_persenel = Math.Round((ch.mashmol_bime) * ch.darsad_p / 100);
                //        ch.bime_bikari = Math.Round(ch.mashmol_bime * b_b / 100);
                //    }                
                //}
                //else if (vaziat_jesmi == 2 ||vaziat_jesmi == 3 || vaziat_jesmi == 5)
                //{
                //    ch.bime_karfarma = Math.Round(ch.mashmol_bime * ch.darsad_k / 100);
                //    ch.bime_persenel = 0;
                //    ch.bime_bikari = Math.Round(ch.mashmol_bime * b_b / 100);
                //}
                //ch.bime_karfarma = cha.bime_karfarma - (chb.bime_karfarma + chc.bime_karfarma);
                //ch.bime_persenel = cha.bime_persenel - (chb.bime_persenel + chc.bime_persenel);
                //ch.bime_bikari = cha.bime_bikari - (chb.bime_bikari + chc.bime_bikari);
                //ch.maliyat = cha.maliyat - (chb.maliyat + chc.maliyat);
                //ch.ezafekari = cha.ezafekari - (chb.ezafekari + chc.ezafekari);
                //ch.s_ezafekari = cha.s_ezafekari;
                //ch.sayer_fogholadeha = cha.sayer_fogholadeha - (chb.sayer_fogholadeha + chc.sayer_fogholadeha);
                //ch.talash = cha.talash - (chb.talash + chc.talash);
                //ch.jazb_tabsare = cha.jazb_tabsare - (chb.jazb_tabsare + chc.jazb_tabsare);
                //ch.haghdarman = cha.haghdarman - (chb.haghdarman + chc.haghdarman);
                //ch.haghdarman_k = cha.haghdarman_k - (chb.haghdarman_k + chc.haghdarman_k);
                //if (p.vaziyat_jesmi == 1 && p.nobime == 2)
                //{
                //    ch.haghdarman = Math.Round(ch.mashmol_bime * (h_d_k + h_d_p) / 100 + (new_mazad - old_mazad) + (new_takafol - old_takafol));
                //    ch.haghdarman_k = Math.Round(ch.mashmol_bime * h_d_k / 100);
                //}
                //else if ((p.vaziyat_jesmi == 2 || p.vaziyat_jesmi == 3) && p.nobime == 2)
                //{
                //    ch.haghdarman = Math.Round((new_mazad - old_mazad) + (new_takafol - old_takafol));
                //    ch.haghdarman_k = Math.Round((ch.mashmol_bime * h_d_k / 100) + (ch.mashmol_bime * h_d_p / 100));
                //}
                //ch.barjastegi = cha.barjastegi - (chb.barjastegi + chc.barjastegi);
                //ch.tadil = cha.tadil - (chb.tadil + chc.tadil);
                //ch.paye = cha.paye - (chb.paye + chc.paye);
                //ch.jebhe = cha.jebhe - (chb.jebhe + chc.jebhe);
                //ch.sanavat_isar = cha.sanavat_isar - (chb.sanavat_isar + chc.sanavat_isar);
                //ch.sanavat_basij = cha.sanavat_basij - (chb.sanavat_basij + chc.sanavat_basij);
                //ch.riali = cha.riali - (chb.riali + chc.riali);
                //ch.janbazi = cha.janbazi - (chb.janbazi + chc.janbazi);
                //ch.tatilkari = cha.tatilkari - (chb.tatilkari + chc.tatilkari);
                //ch.sayer = cha.sayer - (chb.sayer + chc.sayer);
                //ch.FoghIsar = cha.FoghIsar - (chb.FoghIsar + chc.FoghIsar);
                //ch.Made26 = cha.Made26 - (chb.Made26 + chc.Made26);
                //ch.Modiriati = cha.Modiriati - (chb.Modiriati + chc.Modiriati);
                //ch.Takhasosi = cha.Takhasosi - (chb.Takhasosi + chc.Takhasosi);
                //ch.zaribtadil = cha.zaribtadil - (chb.zaribtadil + chc.zaribtadil);
                //ch.ashora = cha.ashora - (chb.ashora + chc.ashora);
                //ch.ghazai = cha.ghazai - (chb.ghazai + chc.ghazai);
                //ch.t_tatilkari = cha.t_tatilkari;
                //ch.sanavat_payan_khedmat = cha.sanavat_payan_khedmat - (chb.sanavat_payan_khedmat + chc.sanavat_payan_khedmat);
                //ch.Bon = cha.Bon - (chb.Bon + chc.Bon);
                ////ch.mogharari = cha.mogharari;
                //ch.mashmol_maliat = cha.mashmol_maliat - (chb.mashmol_maliat + chc.mashmol_maliat);
                //decimal koltafavot = ch.hoghogh + ch.sanavat + ch.fogh_shoghl +
                //    ch.tafavot_tatbigh + ch.hagholad + ch.arzeshyabi + ch.ayelemandi +
                //    ch.nobatkari + ch.maskan + ch.sakhtikar + ch.kharobar + ch.jazb +
                //    ch.tashilatzendegi + ch.badiabohava + ch.tarmim + ch.ezafekari +
                //    ch.sayer_fogholadeha + ch.jazb_tabsare + ch.talash + ch.haghdarman_k +
                //    ch.barjastegi + ch.tadil + ch.paye + ch.jebhe + ch.sanavat_isar +
                //    ch.janbazi + ch.riali + ch.sanavat_basij + ch.pasandaz / 2 + ch.sayer
                //    + ch.tatilkari + ch.sanavat_payan_khedmat + ch.Bon + ch.FoghIsar +
                //    ch.Made26 + ch.Modiriati + ch.Takhasosi + ch.ghazai + ch.zaribtadil +
                //    ch.ashora + ch.jazbTakhasosi + ch.zajbTadil + ch.hadaghalTadil;// +ch.mamoriat;
                //if (ch.maliyat < 0)
                //    koltafavot = -(ch.maliyat) + koltafavot;
                //else
                //    koltafavot = koltafavot - ch.maliyat;
                //if (ch.pasandaz < 0)
                //    koltafavot = -(ch.pasandaz) + koltafavot;
                //else
                //    koltafavot = koltafavot - ch.pasandaz;
                //if (ch.bime_persenel < 0)
                //    koltafavot = -(ch.bime_persenel) + koltafavot;
                //else
                //    koltafavot = koltafavot - ch.bime_persenel;
                //if (ch.haghdarman < 0)
                //    koltafavot = -(ch.haghdarman) + koltafavot;
                //else
                //    koltafavot = koltafavot - ch.haghdarman;
                //decimal khales = koltafavot;// -ch.mogharari;
                //ch.jame_hoghogh = khales;
                //con.Close();
            }
            catch (Exception x)
            {
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                DateTime d1;
                d1 = DateTime.Now;
                string excep = "";
                if (x.InnerException != null)
                    excep = "'m_Moavaghe " + x.InnerException.Message + "'";
                else
                    excep = "'m_Moavaghe " + x.Message + "'";
                string commandText1 = "exec [Com].[spr_tblErrorInsert] 	0,'admin'," + excep + ",'" + d1.ToString() + "',1,1,''";
                var _Hokm1 = service.GetData(commandText1).Tables[0];
            }
        }
        public static List<NewFMS.Models.Mohasebat.Hokm> HokmsInCurentMonth(int PayPersonalId, int PrsPersonalId, int Year, int Month, int Day, int TypeEstekhdam)
        {
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            List<Hokm> Hokms = new List<Hokm>();
            try
            {
                string commandText = "SELECT   top(1)     fldId, fldTarikhEjra, fldTarikhSodoor, fldAnvaeEstekhdamId, fldStatusTaaholId, fldTedadFarzand,fldTedadAfradTahteTakafol " +
                  "FROM  Prs.tblPersonalHokm " +
                  " WHERE fldPrs_PersonalInfoId=" + PrsPersonalId + " and  fldTarikhEjra<='" + Year + "/" + Month.ToString().PadLeft(2, '0') + "/01' and fldStatusHokm=1 " +
                  " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
                var tblPersonalHokm = service.GetData(commandText).Tables[0];
                Hokm hokm = new Hokm();
                if (tblPersonalHokm.Rows.Count > 0)
                {
                    commandText = "SELECT        fldItems_EstekhdamId,fldMablagh,fldItemsHoghughiId FROM            Prs.tblHokm_Item INNER JOIN" +
                      " Com.tblItems_Estekhdam ON Prs.tblHokm_Item.fldItems_EstekhdamId = Com.tblItems_Estekhdam.fldId " +
                      "WHERE fldPersonalHokmId=" + Convert.ToInt32(tblPersonalHokm.Rows[0]["fldId"]);
                    var Hokm_Item = service.GetData(commandText).Tables[0];
                    hokm.HokmId = Convert.ToInt32(tblPersonalHokm.Rows[0]["fldId"]);
                    hokm.NoeEstekhdam = Convert.ToInt32(tblPersonalHokm.Rows[0]["fldAnvaeEstekhdamId"]);
                    hokm.NoeTaahol = Convert.ToInt32(tblPersonalHokm.Rows[0]["fldStatusTaaholId"]);
                    hokm.TarikhEjra = (tblPersonalHokm.Rows[0]["fldTarikhEjra"]).ToString();
                    hokm.TarikhSodur = (tblPersonalHokm.Rows[0]["fldTarikhSodoor"]).ToString();
                    hokm.TedadAfradTakafol = Convert.ToInt32(tblPersonalHokm.Rows[0]["fldTedadAfradTahteTakafol"]);
                    hokm.TedadFarzand = Convert.ToInt32(tblPersonalHokm.Rows[0]["fldTedadFarzand"]);
                    if (Hokm_Item.Rows.Count > 0)
                    {
                        for (int i = 0; i < Hokm_Item.Rows.Count; i++)
                        {
                            hokm.Items.Add(new HokmItems { ItemEstekhdamId = Convert.ToInt32(Hokm_Item.Rows[i]["fldItems_EstekhdamId"]), ItemHoghughiId = Convert.ToInt32(Hokm_Item.Rows[i]["fldItemsHoghughiId"]), Mablagh = Convert.ToInt32(Hokm_Item.Rows[i]["fldMablagh"]) });
                        }
                    }
                    Hokms.Add(hokm);
                }
                commandText = "select fldId, fldTarikhEjra, (fldTarikhSodoor), fldAnvaeEstekhdamId, fldStatusTaaholId, fldTedadFarzand,fldTedadAfradTahteTakafol " +
                  "from   Prs.tblPersonalHokm  WHERE fldPrs_PersonalInfoId=" + PrsPersonalId + " and  fldTarikhEjra<='" + Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Day + "' and fldStatusHokm=1 and " +
                  "fldTarikhEjra>'" + Year + "/" + Month.ToString().PadLeft(2, '0') + "/01' and" +
                  " fldTarikhSodoor in (select t.fldTarikhSodoor from(select max(fldTarikhSodoor) as fldTarikhSodoor, fldTarikhEjra FROM  Prs.tblPersonalHokm  " +
                  " WHERE fldPrs_PersonalInfoId=" + PrsPersonalId + " and  fldTarikhEjra<='" + Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Day + "' and fldStatusHokm=1  " +
                  "group by fldTarikhEjra)t)";
                //         commandText = "SELECT    fldId, fldTarikhEjra, fldTarikhSodoor, fldAnvaeEstekhdamId, fldStatusTaaholId, fldTedadFarzand,fldTedadAfradTahteTakafol " +
                //           "FROM  Prs.tblPersonalHokm " +
                //           " WHERE fldPrs_PersonalInfoId=" + PrsPersonalId + " and  fldTarikhEjra<='" + Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Day + "' and fldStatusHokm=1 and " +
                //           "fldTarikhEjra>'" + Year + "/" + Month.ToString().PadLeft(2, '0') + "/01' " +
                //           " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
                var tblPersonalHokm2 = service.GetData(commandText).Tables[0];
                if (tblPersonalHokm2.Rows.Count > 0)
                {
                    for (int i = 0; i < tblPersonalHokm2.Rows.Count; i++)
                    {
                        Hokm hokm2 = new Hokm();
                        commandText = "SELECT        fldItems_EstekhdamId,fldMablagh,fldItemsHoghughiId FROM            Prs.tblHokm_Item INNER JOIN" +
                          " Com.tblItems_Estekhdam ON Prs.tblHokm_Item.fldItems_EstekhdamId = Com.tblItems_Estekhdam.fldId " +
                          " WHERE fldPersonalHokmId=" + Convert.ToInt32(tblPersonalHokm2.Rows[i]["fldId"]);
                        var Hokm_Item2 = service.GetData(commandText).Tables[0];
                        hokm2.HokmId = Convert.ToInt32(tblPersonalHokm2.Rows[i]["fldId"]);
                        hokm2.NoeEstekhdam = Convert.ToInt32(tblPersonalHokm2.Rows[i]["fldAnvaeEstekhdamId"]);
                        hokm2.NoeTaahol = Convert.ToInt32(tblPersonalHokm2.Rows[i]["fldStatusTaaholId"]);
                        hokm2.TarikhEjra = (tblPersonalHokm2.Rows[i]["fldTarikhEjra"]).ToString();
                        hokm2.TarikhSodur = (tblPersonalHokm2.Rows[i]["fldTarikhSodoor"]).ToString();
                        hokm2.TedadAfradTakafol = Convert.ToInt32(tblPersonalHokm2.Rows[i]["fldTedadAfradTahteTakafol"]);
                        hokm2.TedadFarzand = Convert.ToInt32(tblPersonalHokm2.Rows[i]["fldTedadFarzand"]);
                        if (Hokm_Item2.Rows.Count > 0)
                        {
                            for (int j = 0; j < Hokm_Item2.Rows.Count; j++)
                            {
                                hokm2.Items.Add(new HokmItems { ItemEstekhdamId = Convert.ToInt32(Hokm_Item2.Rows[j]["fldItems_EstekhdamId"]), ItemHoghughiId = Convert.ToInt32(Hokm_Item2.Rows[j]["fldItemsHoghughiId"]), Mablagh = Convert.ToInt32(Hokm_Item2.Rows[j]["fldMablagh"]) });
                            }
                        }
                        Hokms.Add(hokm2);
                    }
                }
                int r = 0;
                Hokms = Hokms.OrderBy(l => l.TarikhEjra).ThenBy(l => l.TarikhSodur).ToList();
                if (Hokms.Count > 1)
                {
                    Hokm hokm3 = new Hokm();
                    Hokm hokm4 = new Hokm();
                    for (int i = 0; i < Hokms.Count; i++)
                    {
                        hokm3 = Hokms[i];
                        hokm4 = Hokms[i];
                        if (i < Hokms.Count - 1)
                            hokm4 = Hokms[i + 1];
                        if (TypeEstekhdam == 1)
                        {
                            if (Convert.ToInt32(hokm3.TarikhEjra.Replace("/", "")) <= (Convert.ToInt32(Year.ToString() + Month.ToString().PadLeft(2, '0') + "01")))
                            {
                                r = MyLib.Shamsi.DiffOfShamsiDate(Year.ToString() + "/" + Month.ToString().PadLeft(2, '0') + "/" + "01", hokm4.TarikhEjra);
                            }
                            else if (Convert.ToInt32(hokm3.TarikhEjra.Replace("/", "")) < Convert.ToInt32(hokm4.TarikhEjra.Replace("/", "")))
                            {
                                r = MyLib.Shamsi.DiffOfShamsiDate(hokm3.TarikhEjra, hokm4.TarikhEjra);
                            }
                            else if (Convert.ToInt32(hokm3.TarikhEjra.Replace("/", "")) <= (Convert.ToInt32(Year.ToString() + Month.ToString().PadLeft(2, '0') + Day.ToString().PadLeft(2, '0'))))
                            {
                                if (Month == 12 && Day >= 30)
                                    r = MyLib.Shamsi.DiffOfShamsiDate(hokm3.TarikhEjra, Year.ToString() + "/" + Month.ToString().PadLeft(2, '0') + "/" + (Day - 1).ToString().PadLeft(2, '0')) + 1;
                                else
                                    r = MyLib.Shamsi.DiffOfShamsiDate(hokm3.TarikhEjra, Year.ToString() + "/" + Month.ToString().PadLeft(2, '0') + "/" + Day.ToString().PadLeft(2, '0')) + 1;
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(hokm3.TarikhEjra.Replace("/", "")) <= (Convert.ToInt32(Year.ToString() + Month.ToString().PadLeft(2, '0') + "01")))
                            {
                                r = MyLib.Shamsi.DiffOfShamsiDate(Year.ToString() + "/" + Month.ToString().PadLeft(2, '0') + "/" + "01", hokm4.TarikhEjra);
                            }
                            else if (Convert.ToInt32(hokm3.TarikhEjra.Replace("/", "")) < Convert.ToInt32(hokm4.TarikhEjra.Replace("/", "")))
                            {
                                r = MyLib.Shamsi.DiffOfShamsiDate(hokm3.TarikhEjra, hokm4.TarikhEjra);
                            }
                            else if (Convert.ToInt32(hokm3.TarikhEjra.Replace("/", "")) <= (Convert.ToInt32(Year.ToString() + Month.ToString().PadLeft(2, '0') + Day.ToString().PadLeft(2, '0'))))
                            {
                                if (Month == 12 && Day >= 30)
                                    r = MyLib.Shamsi.DiffOfShamsiDate(hokm3.TarikhEjra, Year.ToString() + "/" + Month.ToString().PadLeft(2, '0') + "/" + (Day - 1).ToString().PadLeft(2, '0')) + 2;
                                else
                                    r = MyLib.Shamsi.DiffOfShamsiDate(hokm3.TarikhEjra, Year.ToString() + "/" + Month.ToString().PadLeft(2, '0') + "/" + Day.ToString().PadLeft(2, '0')) + 1;
                            }
                        }
                        var u = Hokms[i];
                        u.Days = r;
                    }
                }
                else if (Hokms.Count == 1)
                {
                    if (TypeEstekhdam == 1)
                        r = MyLib.Shamsi.DiffOfShamsiDate(Year.ToString() + "/" + Month.ToString().PadLeft(2, '0') + "/" + "01", Year.ToString() + "/" + Month.ToString().PadLeft(2, '0') + "/" + Day.ToString().PadLeft(2, '0')) + 1;
                    else
                        r = MyLib.Shamsi.DiffOfShamsiDate(Year.ToString() + "/11" + "/" + "01", Year.ToString() + "/11" + "/" + Day.ToString().PadLeft(2, '0')) + 1;
                    var u = Hokms[0];
                    u.Days = r;
                }
                var hh = Hokms.OrderBy(l => l.TarikhEjra).ThenBy(l => l.TarikhSodur).ToList();
                //         if (hh.Count > 0)
                //         {
                //           int maxhokm = Convert.ToInt32(hh[hh.Count - 1].TarikhSodur.Replace("/", ""));
                //           for (int i = hh.Count - 2; i > 0; i--)
                //           {
                //             if (Convert.ToInt32(hh[i].TarikhSodur.Replace("/", "")) < maxhokm)
                //               hh.RemoveAt(i);
                //             else
                //               maxhokm = Convert.ToInt32(hh[i].TarikhSodur.Replace("/", ""));
                //           }
                //         }
                return hh;
            }
            catch (Exception x)
            {
                DateTime d1;
                d1 = DateTime.Now;
                string excep = "";
                if (x.InnerException != null)
                    excep = "'HokmsInCurentMonth " + x.InnerException.Message + "'";
                else
                    excep = "'HokmsInCurentMonth " + x.Message + "'";
                string commandText1 = "exec [Com].[spr_tblErrorInsert] 	0,'admin'," + excep + ",'" + d1.ToString() + "',1,1,''";
                var _Hokm1 = service.GetData(commandText1).Tables[0];
                return null;
            }
        }
        public static MoteghayerhayeHoghoghi GetMoteghayerHoghughi(int TypeBime, int Year, int Month, int Day, int AnvaEstekhdam)
        {
            try
            {
                List<MoteghayerhayeHoghoghi> M_hoghugh = new List<MoteghayerhayeHoghoghi>();
                string commandText = "SELECT    TOP(1)    fldId, fldTarikhEjra, fldTarikhSodur, fldAnvaeEstekhdamId, fldTypeBimeId, fldZaribEzafeKar, fldSaatKari, fldDarsadBimePersonal, fldDarsadbimeKarfarma, fldDarsadBimeBikari, fldDarsadBimeJanbazan, " +
                  "fldHaghDarmanKarmand, fldHaghDarmanKarfarma, fldHaghDarmanDolat, fldHaghDarmanMazad, fldHaghDarmanTahteTakaffol, fldDarsadBimeMashagheleZiyanAvar, fldMaxHaghDarman, fldZaribHoghoghiSal,  " +
                  "fldHoghogh, fldFoghShoghl, fldTafavotTatbigh, fldFoghVizhe, fldHaghJazb, fldTadil, fldBarJastegi, fldSanavat,fldFoghTalash, fldUserId, fldDate, fldDesc " +
                  "FROM            Pay.tblMoteghayerhayeHoghoghi " +
                  "WHERE fldAnvaeEstekhdamId=" + AnvaEstekhdam + "AND fldTypeBimeId=" + TypeBime + "AND SUBSTRING(fldTarikhEjra,1,4)<='" + Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Day + "'" +
                  "ORDER BY fldTarikhSodur DESC,fldTarikhEjra DESC";
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                var tblMoteghayerhayeHoghoghi = service.GetData(commandText).Tables[0];
                MoteghayerhayeHoghoghi Moteghayer_H = new MoteghayerhayeHoghoghi();
                if (tblMoteghayerhayeHoghoghi.Rows.Count > 0)
                {
                    commandText = "SELECT     Pay.tblMoteghayerhayeHoghoghi_Detail.*, Com.tblItems_Estekhdam.fldItemsHoghughiId FROM " +
                      "Pay.tblMoteghayerhayeHoghoghi_Detail INNER JOIN Com.tblItems_Estekhdam ON Pay.tblMoteghayerhayeHoghoghi_Detail.fldItemEstekhdamId " +
                      "= Com.tblItems_Estekhdam.fldId " +
                      "WHERE Pay.tblMoteghayerhayeHoghoghi_Detail.fldMoteghayerhayeHoghoghiId=" + Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldId"]);
                    var MoteghayerhayeHoghoghi_Detail = service.GetData(commandText).Tables[0];
                    Moteghayer_H.fldId = Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldId"]);
                    Moteghayer_H.fldTarikhEjra = (tblMoteghayerhayeHoghoghi.Rows[0]["fldTarikhEjra"]).ToString();
                    Moteghayer_H.fldTarikhSodur = (tblMoteghayerhayeHoghoghi.Rows[0]["fldTarikhSodur"]).ToString();
                    Moteghayer_H.fldAnvaeEstekhdamId = Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldAnvaeEstekhdamId"]);
                    Moteghayer_H.fldTypeBimeId = Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldTypeBimeId"]);
                    Moteghayer_H.fldZaribEzafeKar = Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldZaribEzafeKar"]);
                    Moteghayer_H.fldSaatKari = Convert.ToDouble(tblMoteghayerhayeHoghoghi.Rows[0]["fldSaatKari"]);
                    Moteghayer_H.fldDarsadBimePersonal = Convert.ToDouble(tblMoteghayerhayeHoghoghi.Rows[0]["fldDarsadBimePersonal"]);
                    Moteghayer_H.fldDarsadbimeKarfarma = Convert.ToDouble(tblMoteghayerhayeHoghoghi.Rows[0]["fldDarsadbimeKarfarma"]);
                    Moteghayer_H.fldDarsadBimeBikari = Convert.ToDouble(tblMoteghayerhayeHoghoghi.Rows[0]["fldDarsadBimeBikari"]);
                    Moteghayer_H.fldDarsadBimeJanbazan = Convert.ToDouble(tblMoteghayerhayeHoghoghi.Rows[0]["fldDarsadBimeJanbazan"]);
                    Moteghayer_H.fldHaghDarmanKarmand = Convert.ToDouble(tblMoteghayerhayeHoghoghi.Rows[0]["fldHaghDarmanKarmand"]);
                    Moteghayer_H.fldHaghDarmanKarfarma = Convert.ToDouble(tblMoteghayerhayeHoghoghi.Rows[0]["fldHaghDarmanKarfarma"]);
                    Moteghayer_H.fldHaghDarmanDolat = Convert.ToDouble(tblMoteghayerhayeHoghoghi.Rows[0]["fldHaghDarmanDolat"]);
                    Moteghayer_H.fldHaghDarmanMazad = Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldHaghDarmanMazad"]);
                    Moteghayer_H.fldHaghDarmanTahteTakaffol = Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldHaghDarmanTahteTakaffol"]);
                    Moteghayer_H.fldDarsadBimeMashagheleZiyanAvar = Convert.ToDouble(tblMoteghayerhayeHoghoghi.Rows[0]["fldDarsadBimeMashagheleZiyanAvar"]);
                    Moteghayer_H.fldMaxHaghDarman = Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldMaxHaghDarman"]);
                    Moteghayer_H.fldZaribHoghoghiSal = Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldZaribHoghoghiSal"]);
                    Moteghayer_H.fldHoghogh = Convert.ToBoolean(tblMoteghayerhayeHoghoghi.Rows[0]["fldHoghogh"]);
                    Moteghayer_H.fldFoghShoghl = Convert.ToBoolean(tblMoteghayerhayeHoghoghi.Rows[0]["fldFoghShoghl"]);
                    Moteghayer_H.fldTafavotTatbigh = Convert.ToBoolean(tblMoteghayerhayeHoghoghi.Rows[0]["fldTafavotTatbigh"]);
                    Moteghayer_H.fldFoghVizhe = Convert.ToBoolean(tblMoteghayerhayeHoghoghi.Rows[0]["fldFoghVizhe"]);
                    Moteghayer_H.fldHaghJazb = Convert.ToBoolean(tblMoteghayerhayeHoghoghi.Rows[0]["fldHaghJazb"]);
                    Moteghayer_H.fldTadil = Convert.ToBoolean(tblMoteghayerhayeHoghoghi.Rows[0]["fldTadil"]);
                    Moteghayer_H.fldBarJastegi = Convert.ToBoolean(tblMoteghayerhayeHoghoghi.Rows[0]["fldBarJastegi"]);
                    Moteghayer_H.fldSanavat = Convert.ToBoolean(tblMoteghayerhayeHoghoghi.Rows[0]["fldSanavat"]);
                    Moteghayer_H.fldFoghTalash = Convert.ToBoolean(tblMoteghayerhayeHoghoghi.Rows[0]["fldFoghTalash"]);
                    if (MoteghayerhayeHoghoghi_Detail.Rows.Count > 0)
                    {
                        for (int i = 0; i < MoteghayerhayeHoghoghi_Detail.Rows.Count; i++)
                        {
                            Moteghayer_H.Items.Add(new MoteghayerhayeHoghoghi_Detail
                            {
                                fldId = Convert.ToInt32(MoteghayerhayeHoghoghi_Detail.Rows[i]["fldId"]),
                                fldMoteghayerhayeHoghoghiId = Convert.ToInt32(MoteghayerhayeHoghoghi_Detail.Rows[i]["fldMoteghayerhayeHoghoghiId"])
         ,
                                fldItemHoghughiId = Convert.ToInt32(MoteghayerhayeHoghoghi_Detail.Rows[i]["fldItemsHoghughiId"]),
                                fldItemEstekhdamId = Convert.ToInt32(MoteghayerhayeHoghoghi_Detail.Rows[i]["fldItemEstekhdamId"])
                            });
                        }
                    }
                    M_hoghugh.Add(Moteghayer_H);
                }
                return M_hoghugh.OrderBy(l => l.fldTarikhEjra).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static NewFMS.Models.Mohasebat.Fiscal GetFiscal(int Year, int Month, int Day, int AnvaEstakhdam)
        {
            try
            {
                List<Fiscal> Fiscal = new List<Fiscal>();
                string commandText = "SELECT    TOP(1)    fldId, fldEffectiveDate, fldDateOfIssue, fldUserId, fldDate, fldDesc " +
                  "FROM            Pay.tblFiscal_Header " +
                  "WHERE SUBSTRING(fldEffectiveDate,1,4)<='" + Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Day + "'" +
                  "ORDER BY fldDateOfIssue DESC,fldEffectiveDate DESC";
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                var tblFiscal_Header = service.GetData(commandText).Tables[0];
                Fiscal Fiscal_H = new Fiscal();
                if (tblFiscal_Header.Rows.Count > 0)
                {
                    commandText = "SELECT  fldId, fldFiscalHeaderId, fldAmountFrom, fldAmountTo, fldPercentTaxOnWorkers, fldTaxationOfEmployees, fldTax, fldUserId, fldDate, fldDesc " +
                      "from Pay.tblFiscal_Detail " +
                      "WHERE fldFiscalHeaderId=" + Convert.ToInt32(tblFiscal_Header.Rows[0]["fldId"]);
                    var tblFiscal_Detail = service.GetData(commandText).Tables[0];
                    commandText = "SELECT     Pay.tblFiscalTitle.fldId, Pay.tblFiscalTitle.fldFiscalHeaderId, Pay.tblFiscalTitle.fldItemEstekhdamId," +
                      " Pay.tblFiscalTitle.fldAnvaEstekhdamId, Pay.tblFiscalTitle.fldUserId, Pay.tblFiscalTitle.fldDate, Pay.tblFiscalTitle.fldDesc," +
                      " Com.tblItems_Estekhdam.fldItemsHoghughiId FROM Pay.tblFiscalTitle INNER JOIN Com.tblItems_Estekhdam ON " +
                      "Pay.tblFiscalTitle.fldItemEstekhdamId = Com.tblItems_Estekhdam.fldId " +
                      "WHERE Pay.tblFiscalTitle.fldFiscalHeaderId=" + Convert.ToInt32(tblFiscal_Header.Rows[0]["fldId"]) + " and Pay.tblFiscalTitle.fldAnvaEstekhdamId=" + AnvaEstakhdam;
                    var tblFiscalTitle = service.GetData(commandText).Tables[0];
                    Fiscal_H.fldId = Convert.ToInt32(tblFiscal_Header.Rows[0]["fldId"]);
                    Fiscal_H.fldTarikhEjra = (tblFiscal_Header.Rows[0]["fldEffectiveDate"]).ToString();
                    Fiscal_H.fldTarikhSodur = (tblFiscal_Header.Rows[0]["fldDateOfIssue"]).ToString();
                    if (tblFiscal_Detail.Rows.Count > 0)
                    {
                        for (int i = 0; i < tblFiscal_Detail.Rows.Count; i++)
                        {
                            Fiscal_H.Items_Detail.Add(new Fiscal_Detail
                            {
                                fldId = Convert.ToInt32(tblFiscal_Detail.Rows[i]["fldId"]),
                                fldFiscalHeaderId = Convert.ToInt32(tblFiscal_Detail.Rows[i]["fldFiscalHeaderId"]),
                                fldAmountFrom = Convert.ToInt32(tblFiscal_Detail.Rows[i]["fldAmountFrom"]),
                                fldAmountTo = Convert.ToInt32(tblFiscal_Detail.Rows[i]["fldAmountTo"]),
                                fldPercentTaxOnWorkers = Convert.ToDouble(tblFiscal_Detail.Rows[i]["fldPercentTaxOnWorkers"]),
                                fldTaxationOfEmployees = Convert.ToDouble(tblFiscal_Detail.Rows[i]["fldTaxationOfEmployees"]),
                                fldTax = Convert.ToInt32(tblFiscal_Detail.Rows[i]["fldTax"])
                            });
                        }
                    }
                    if (tblFiscalTitle.Rows.Count > 0)
                    {
                        for (int i = 0; i < tblFiscalTitle.Rows.Count; i++)
                        {
                            Fiscal_H.Items_Title.Add(new FiscalTitle
                            {
                                fldId = Convert.ToInt32(tblFiscalTitle.Rows[i]["fldId"]),
                                fldFiscalHeaderId = Convert.ToInt32(tblFiscalTitle.Rows[i]["fldFiscalHeaderId"]),
                                fldItemEstekhdamId = Convert.ToInt32(tblFiscalTitle.Rows[i]["fldItemEstekhdamId"]),
                                fldAnvaEstekhdamId = Convert.ToInt32(tblFiscalTitle.Rows[i]["fldAnvaEstekhdamId"]),
                                fldItemHoghughiId = Convert.ToInt32(tblFiscalTitle.Rows[i]["fldItemsHoghughiId"])
                            });
                        }
                    }
                    Fiscal.Add(Fiscal_H);
                }
                return Fiscal.OrderBy(l => l.fldTarikhEjra).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public NewFMS.WCF_PayRoll.OBJ_KarKardeMahane GetKarkardMahane(int Year, int Month, int PersonalId, int Nobat)
        {
            try
            {
                string commandText = "SELECT        fldId, fldPersonalId, fldYear, fldMah, fldKarkard, fldGheybat, fldNobateKari, fldEzafeKari, fldTatileKari, fldMamoriatBaBeitote, fldMamoriatBedoneBeitote, fldMosaedeh, fldNobatePardakht, fldFlag, fldGhati, fldBa10, " +
                  "fldBa20, fldBa30, fldBe10, fldBe20, fldBe30, fldUserId, fldDate, fldDesc, fldShift " +
                  "  FROM            Pay.tblKarKardeMahane " +
                  "  WHERE fldPersonalId= " + PersonalId + " AND fldYear=" + Year + " AND fldMah=" + Month + " AND fldNobatePardakht=" + Nobat;
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                var tblKarkard = service.GetData(commandText).Tables[0];
                NewFMS.WCF_PayRoll.OBJ_KarKardeMahane karkard = new NewFMS.WCF_PayRoll.OBJ_KarKardeMahane();
                if (tblKarkard.Rows.Count != 0)
                {
                    karkard.fldBa10 = Convert.ToByte(tblKarkard.Rows[0]["fldBa10"]);
                    karkard.fldBa20 = Convert.ToByte(tblKarkard.Rows[0]["fldBa20"]);
                    karkard.fldBa30 = Convert.ToByte(tblKarkard.Rows[0]["fldBa30"]);
                    karkard.fldBe10 = Convert.ToByte(tblKarkard.Rows[0]["fldBe10"]);
                    karkard.fldBe20 = Convert.ToByte(tblKarkard.Rows[0]["fldBe20"]);
                    karkard.fldBe30 = Convert.ToByte(tblKarkard.Rows[0]["fldBe30"]);
                    karkard.fldPersonalId = Convert.ToInt32(tblKarkard.Rows[0]["fldPersonalId"]);
                    karkard.fldId = Convert.ToInt32(tblKarkard.Rows[0]["fldId"]);
                    karkard.fldYear = Convert.ToInt16(tblKarkard.Rows[0]["fldYear"]);
                    karkard.fldMah = Convert.ToByte(tblKarkard.Rows[0]["fldMah"]);
                    karkard.fldKarkard = Convert.ToByte(tblKarkard.Rows[0]["fldKarkard"]);
                    karkard.fldGheybat = Convert.ToByte(tblKarkard.Rows[0]["fldGheybat"]);
                    karkard.fldNobateKari = Convert.ToByte(tblKarkard.Rows[0]["fldNobateKari"]);
                    karkard.fldEzafeKari = Convert.ToDecimal(tblKarkard.Rows[0]["fldEzafeKari"]);
                    karkard.fldTatileKari = Convert.ToDecimal(tblKarkard.Rows[0]["fldTatileKari"]);
                    karkard.fldMamoriatBaBeitote = Convert.ToByte(tblKarkard.Rows[0]["fldMamoriatBaBeitote"]);
                    karkard.fldMamoriatBedoneBeitote = Convert.ToByte(tblKarkard.Rows[0]["fldMamoriatBedoneBeitote"]);
                    karkard.fldMosaedeh = Convert.ToInt32(tblKarkard.Rows[0]["fldMosaedeh"]);
                    karkard.fldNobatePardakht = Convert.ToByte(tblKarkard.Rows[0]["fldNobatePardakht"]);
                    karkard.fldFlag = Convert.ToBoolean(tblKarkard.Rows[0]["fldFlag"]);
                    karkard.fldGhati = Convert.ToBoolean(tblKarkard.Rows[0]["fldGhati"]);
                    karkard.fldShift = Convert.ToInt32(tblKarkard.Rows[0]["fldShift"]);
                }
                return karkard;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PersonalInfo GetPersonalInfo(int PayPersonalId)
        {
            try
            {
                string commandText = "SELECT        Pay.Pay_tblPersonalInfo.fldBimeOmr, Pay.Pay_tblPersonalInfo.fldBimeTakmili, " +
                  "Pay.Pay_tblPersonalInfo.fldMazad30Sal, Pay.Pay_tblPersonalInfo.fldPasAndaz, Pay.Pay_tblPersonalInfo.fldSanavatPayanKhedmat," +
                  " Pay.Pay_tblPersonalInfo.fldHamsarKarmand, Pay.Pay_tblPersonalInfo.fldMoafDarman, Prs.tblVaziyatEsargari.fldMoafAsBime," +
                  " Prs.tblVaziyatEsargari.fldMoafAsMaliyat, Com.fn_MaxPersonalStatus(Pay.Pay_tblPersonalInfo.fldId, N'hoghoghi') AS fldStatus," +
                  " Prs.Prs_tblPersonalInfo.fldEsargariId, Com.tblEmployee_Detail.fldJensiyat FROM Pay.Pay_tblPersonalInfo INNER JOIN " +
                  " Prs.Prs_tblPersonalInfo ON Pay.Pay_tblPersonalInfo.fldPrs_PersonalInfoId = Prs.Prs_tblPersonalInfo.fldId INNER JOIN " +
                  "Prs.tblVaziyatEsargari ON Prs.Prs_tblPersonalInfo.fldEsargariId = Prs.tblVaziyatEsargari.fldId INNER JOIN Com.tblEmployee" +
                  " ON Prs.Prs_tblPersonalInfo.fldEmployeeId = Com.tblEmployee.fldId INNER JOIN Com.tblEmployee_Detail ON Com.tblEmployee.fldId" +
                  " = Com.tblEmployee_Detail.fldEmployeeId AND Com.tblEmployee.fldId = Com.tblEmployee_Detail.fldEmployeeId AND " +
                  "Com.tblEmployee.fldId = Com.tblEmployee_Detail.fldEmployeeId WHERE        (Pay.Pay_tblPersonalInfo.fldId = " + PayPersonalId + ")";
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                var tblPersonalInfo = service.GetData(commandText).Tables[0];
                PersonalInfo perInfo = new PersonalInfo();
                if (tblPersonalInfo.Rows.Count != 0)
                {
                    perInfo.fldBimeOmr = Convert.ToBoolean(tblPersonalInfo.Rows[0]["fldBimeOmr"]);
                    perInfo.fldBimeTakmili = Convert.ToBoolean(tblPersonalInfo.Rows[0]["fldBimeTakmili"]);
                    perInfo.fldMazad30Sal = Convert.ToBoolean(tblPersonalInfo.Rows[0]["fldMazad30Sal"]);
                    perInfo.fldPasAndaz = Convert.ToBoolean(tblPersonalInfo.Rows[0]["fldPasAndaz"]);
                    perInfo.fldSanavatPayanKhedmat = Convert.ToBoolean(tblPersonalInfo.Rows[0]["fldSanavatPayanKhedmat"]);
                    perInfo.fldHamsarKarmand = Convert.ToBoolean(tblPersonalInfo.Rows[0]["fldHamsarKarmand"]);
                    perInfo.fldMoafDarman = Convert.ToBoolean(tblPersonalInfo.Rows[0]["fldMoafDarman"]);
                    perInfo.fldMoafAsBime = Convert.ToBoolean(tblPersonalInfo.Rows[0]["fldMoafAsBime"]);
                    perInfo.fldMoafAsMaliyat = Convert.ToBoolean(tblPersonalInfo.Rows[0]["fldMoafAsMaliyat"]);
                    perInfo.fldStatus = Convert.ToByte(tblPersonalInfo.Rows[0]["fldStatus"]);
                    perInfo.fldEsargariId = Convert.ToInt32(tblPersonalInfo.Rows[0]["fldEsargariId"]);
                    perInfo.fldJensiyat = Convert.ToInt32(tblPersonalInfo.Rows[0]["fldJensiyat"]);
                }
                return perInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetItem_Estekhdam(int ItemHoghoghiId, int TypeEstekhdam)
        {
            try
            {
                string commandText = "SELECT        fldId, fldItemsHoghughiId, fldTypeEstekhdamId, fldTitle, fldHasZarib " +
                  "FROM            Com.tblItems_Estekhdam " +
                  "WHERE fldItemsHoghughiId=" + ItemHoghoghiId + "AND fldTypeEstekhdamId=" + TypeEstekhdam;
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                var tblItems_Estekhdam = service.GetData(commandText).Tables[0];
                int TypeEstekhdamId = 0;
                if (tblItems_Estekhdam.Rows.Count != 0)
                {
                    TypeEstekhdamId = Convert.ToInt32(tblItems_Estekhdam.Rows[0]["fldId"]);
                }
                return TypeEstekhdamId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void hokm_roz_hog(Hokm Hokm,
                                 NewFMS.WCF_PayRoll.OBJ_KarKardeMahane Karkard
                                 , int TypeEstekhdam, int TypebimeId, int Days, int Year, int Month,
                                 PersonalInfo PersonalInfo, Hokm LastHokm)
        {
            try
            {
                //---------------            
                double hoghogh = 0, sanavat = 0, fogh_shoghl = 0,
                tafavot_tatbigh = 0, hagholad = 0, hagh_olad = 0, arzeshyabi = 0,
                ayelemandi = 0, nobatkari = 0, maskan = 0, sakhtikar = 0,
                kharobar = 0, jazb = 0, tashilatzendegi = 0, badiabohava = 0,
                tarmim = 0, sayer_fogholadeha = 0,
                talash = 0, jazb_tabsare = 0, barjastegi = 0,
                tadil = 0, paye = 0, jebhe = 0, sanavat_isar = 0,
                sanavat_basij = 0, riali = 0, janbazi = 0, sayer = 0,
                foghIsar = 0, Takhasosi = 0, Made26 = 0, Modiriati = 0,
                Bon = 0;
                double sanavat_payanKhedmat = 0;
                double ghibat = 0, _ghibat = 0;
                int t_farzand = 0;
                double _roz_nobatkari = 0;
                double hoghogh_paye = 0, h_rozane = 0;
                //----------------
                //Mohasebat mohasebe = new Mohasebat();
                t_farzand = Convert.ToInt32(Hokm.TedadFarzand);
                double k = Convert.ToDouble(Karkard.fldKarkard), k2 = k;
                ghibat = Convert.ToDouble(Karkard.fldGheybat);
                if (TypeEstekhdam > 1 && k + ghibat < 30 && TypebimeId == 1)
                {
                    k = k + ghibat;
                    ghibat = 0;
                }
                _ghibat = (Hokm.Days * ghibat) / Days;
                if (TypeEstekhdam == 1 && Month > 6 && k > 30)
                    k = 30;
                if (TypeEstekhdam == 1 && Month > 11 && k >= 30 && MyLib.Shamsi.Iskabise(Year) == false)
                    k = 29;
                //if (TypeEstekhdam > 5 && p.TypebimeId == 2)
                //    k = 30;
                double bazan = 1;
                if (TypeEstekhdam > 1 && k2 < 30 && PersonalInfo.fldStatus == 3)
                {
                    bazan = k2 / 30;
                    k = k2;
                }
                double rr = (Hokm.Days * (double)30.0) / Days;
                double rr_ghibat = (_ghibat * (double)30.0) / Days;
                double newroz = Hokm.Days * Convert.ToDouble(k) / Days;
                foreach (var item in Hokm.Items)
                {
                    switch (item.ItemHoghughiId)
                    {
                        //--------------------حقوق ماهانه 
                        case 1:
                            hoghogh_paye = Convert.ToDouble(item.Mablagh);
                            h_rozane = hoghogh_paye / 30.0;
                            hoghogh = Math.Round(Days * h_rozane, 0);
                            hoghogh = Math.Round((newroz * hoghogh) / Days);
                            if (hoghogh != 0)
                            {
                                var H = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (H != null)
                                    H.fldMablagh += Convert.ToInt32(hoghogh);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(hoghogh), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        case 2:
                            //--------------------سنوات
                            sanavat = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            sanavat = Math.Round((newroz * sanavat) / Days);
                            if (sanavat != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(sanavat);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(sanavat), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        case 3:
                            //--------------------پایه
                            paye = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            paye = Math.Round((newroz * paye) / Days);
                            if (paye != 0)
                            {
                                var p = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (p != null)
                                    p.fldMablagh += Convert.ToInt32(paye);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(paye), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------سنوات بسیجی
                        case 4:
                            sanavat_basij = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            sanavat_basij = Math.Round((newroz * sanavat_basij) / Days);
                            if (sanavat_basij != 0)
                            {
                                var sb = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (sb != null)
                                    sb.fldMablagh += Convert.ToInt32(sanavat_basij);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(sanavat_basij), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------سنوات ایثارگری
                        case 5:
                            sanavat_isar = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            sanavat_isar = Math.Round((newroz * sanavat_isar) / Days);
                            if (sanavat_isar != 0)
                            {
                                var si = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (si != null)
                                    si.fldMablagh += Convert.ToInt32(sanavat_basij);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(sanavat_isar), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------فوق العاده شغل 
                        case 6:
                            fogh_shoghl = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            fogh_shoghl = Math.Round((newroz * fogh_shoghl) / Days);
                            if (fogh_shoghl != 0)
                            {
                                var fogh = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (fogh != null)
                                    fogh.fldMablagh += Convert.ToInt32(fogh_shoghl);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(fogh_shoghl), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------تحقیقی و تخصصی
                        case 7:
                            Takhasosi = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            Takhasosi = Math.Round((newroz * Takhasosi) / Days);
                            if (Takhasosi != 0)
                            {
                                var t = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (t != null)
                                    t.fldMablagh += Convert.ToInt32(Takhasosi);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(Takhasosi), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------ماده 26
                        case 8:
                            Made26 = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            Made26 = Math.Round((newroz * Made26) / Days);
                            if (Made26 != 0)
                            {
                                var m = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (m != null)
                                    m.fldMablagh += Convert.ToInt32(Made26);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(Made26), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------مدیریتی و سرپرستی
                        case 9:
                            Modiriati = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            Modiriati = Math.Round((newroz * Modiriati) / Days);
                            if (Modiriati != 0)
                            {
                                var Mo = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (Mo != null)
                                    Mo.fldMablagh += Convert.ToInt32(Modiriati);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(Modiriati), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------برجستگی
                        case 10:
                            barjastegi = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            barjastegi = Math.Round((newroz * barjastegi) / Days);
                            if (barjastegi != 0)
                            {
                                var b = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (b != null)
                                    b.fldMablagh += Convert.ToInt32(barjastegi);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(barjastegi), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------تفاوت تطبیق  
                        case 11:
                            tafavot_tatbigh = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            tafavot_tatbigh = Math.Round((newroz * tafavot_tatbigh) / Days);
                            if (tafavot_tatbigh != 0)
                            {
                                var t = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (t != null)
                                    t.fldMablagh += Convert.ToInt32(tafavot_tatbigh);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(tafavot_tatbigh), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------فوق العاده ایثارگری
                        case 12:
                            foghIsar = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            foghIsar = Math.Round((newroz * foghIsar) / Days);
                            if (foghIsar != 0)
                            {
                                var f = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (f != null)
                                    f.fldMablagh += Convert.ToInt32(foghIsar);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(foghIsar), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------بدی آب وهوا
                        case 13:
                            badiabohava = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            badiabohava = Math.Round((newroz * badiabohava) / Days);
                            if (badiabohava != 0)
                            {
                                var b = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (b != null)
                                    b.fldMablagh += Convert.ToInt32(badiabohava);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(badiabohava), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------تسهیلات زندگی
                        case 14:
                            tashilatzendegi = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            tashilatzendegi = Math.Round((newroz * tashilatzendegi) / Days);
                            if (tashilatzendegi != 0)
                            {
                                var t = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (t != null)
                                    t.fldMablagh += Convert.ToInt32(tashilatzendegi);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(tashilatzendegi), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------سختی کار
                        case 15:
                            sakhtikar = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            sakhtikar = Math.Round((newroz * sakhtikar) / Days);
                            if (sakhtikar != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(sakhtikar);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(sakhtikar), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------تعدیل
                        case 16:
                            tadil = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            tadil = Math.Round((newroz * tadil) / Days);
                            if (tadil != 0)
                            {
                                var t = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (t != null)
                                    t.fldMablagh += Convert.ToInt32(tadil);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(tadil), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------مزایای ریالی
                        case 17:
                            riali = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            riali = Math.Round((newroz * riali) / Days);
                            if (riali != 0)
                            {
                                var r = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (r != null)
                                    r.fldMablagh += Convert.ToInt32(riali);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(riali), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            } break;
                        //--------------------حق جذب(بند 9)
                        case 18:
                            arzeshyabi = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            arzeshyabi = Math.Round((newroz * arzeshyabi) / Days);
                            if (arzeshyabi != 0)
                            {
                                var a = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (a != null)
                                    a.fldMablagh += Convert.ToInt32(arzeshyabi);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(arzeshyabi), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------حق جذب
                        case 19:
                            jazb = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            jazb = Math.Round((newroz * jazb) / Days);
                            if (jazb != 0)
                            {
                                var j = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (j != null)
                                    j.fldMablagh += Convert.ToInt32(jazb);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(jazb), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------فوق العاده مخصوص
                        case 20:
                            sayer_fogholadeha = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            sayer_fogholadeha = Math.Round((newroz * sayer_fogholadeha) / Days);
                            if (sayer_fogholadeha != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(sayer_fogholadeha);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(sayer_fogholadeha), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------فوق العاده ویژه
                        case 21:
                            tarmim = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            tarmim = Math.Round((newroz * tarmim) / Days);
                            if (tarmim != 0)
                            {
                                var t = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (t != null)
                                    t.fldMablagh += Convert.ToInt32(tarmim);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(tarmim), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------حق اولاد
                        case 22:
                            hagh_olad = Convert.ToInt32(Math.Round((Convert.ToDouble(item.Mablagh) * (rr - rr_ghibat)) / (double)30, 0));
                            if (hagh_olad != 0)
                            {
                                var h = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (h != null)
                                    h.fldMablagh += Convert.ToInt32(hagh_olad);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(hagh_olad), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------عائله مندی 
                        case 23:
                            ayelemandi = Convert.ToInt32(Math.Round((Convert.ToDouble(item.Mablagh) / 30.0) * (rr - rr_ghibat), 0));
                            if (ayelemandi != 0)
                            {
                                var a = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (a != null)
                                    a.fldMablagh += Convert.ToInt32(ayelemandi);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(ayelemandi), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------خواروبار
                        case 24:
                            kharobar = Convert.ToInt32(Math.Round((Convert.ToDouble(item.Mablagh) / 30.0) * (rr - rr_ghibat), 0));
                            if (kharobar != 0)
                            {
                                var kh = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (kh != null)
                                    kh.fldMablagh += Convert.ToInt32(kharobar);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(kharobar), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------حق مسکن
                        case 25:
                            maskan = Convert.ToInt32(Math.Round((Convert.ToDouble(item.Mablagh) / 30.0) * (rr - rr_ghibat), 0));
                            if (maskan != 0)
                            {
                                var m = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (m != null)
                                    m.fldMablagh += Convert.ToInt32(maskan);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(maskan), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            } break;
                        //--------------------نوبت کاری
                        case 26:
                            if (TypeEstekhdam == 1)
                            {
                                _roz_nobatkari = Hokm.Days * Convert.ToDouble(Karkard.fldNobateKari) / Days;
                                if (_roz_nobatkari > 0)
                                {
                                    nobatkari = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                                    nobatkari = Math.Round(_roz_nobatkari * nobatkari / Days);
                                }
                            }
                            else if (TypeEstekhdam > 1)
                            {
                                nobatkari = Math.Round(Convert.ToDouble(item.Mablagh));
                            }
                            if (nobatkari != 0)
                            {
                                var n = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (n != null)
                                    n.fldMablagh += Convert.ToInt32(nobatkari);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(nobatkari), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------حق بن 
                        case 27:
                            Bon = Convert.ToInt32(Math.Round((Convert.ToDouble(item.Mablagh) / 30.0) * (rr - rr_ghibat), 0));
                            if (Bon != 0)
                            {
                                var b = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (b != null)
                                    b.fldMablagh += Convert.ToInt32(Bon);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(Bon), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------جذب تبصره 7
                        case 28:
                            jazb_tabsare = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            jazb_tabsare = Math.Round((newroz * jazb_tabsare) / Days);
                            if (jazb_tabsare != 0)
                            {
                                var j = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (j != null)
                                    j.fldMablagh += Convert.ToInt32(jazb_tabsare);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(jazb_tabsare), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------تلاش
                        case 29:
                            talash = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            talash = Math.Round((newroz * talash) / Days);
                            if (talash != 0)
                            {
                                var t = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (t != null)
                                    t.fldMablagh += Convert.ToInt32(talash);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(talash), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------جبهه
                        case 30:
                            jebhe = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            jebhe = Math.Round((newroz * jebhe) / Days);
                            if (jebhe != 0)
                            {
                                var j = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (j != null)
                                    j.fldMablagh += Convert.ToInt32(jebhe);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(jebhe), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------جانبازی
                        case 31:
                            janbazi = Math.Round((Convert.ToDouble(item.Mablagh) / 30) * Days, 0);
                            janbazi = Math.Round((newroz * janbazi) / Days);
                            if (janbazi != 0)
                            {
                                var j = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (j != null)
                                    j.fldMablagh += Convert.ToInt32(janbazi);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(janbazi), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------سایر مزایا
                        case 32:
                            //sayer = Math.Round((Convert.ToDouble(read_hokm["sayer"]) / 30) * Days, 0);
                            sayer = Convert.ToDouble(item.Mablagh);
                            sayer = Math.Round((newroz * sayer) / Days);
                            if (sayer != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(sayer);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(sayer), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------حق جذب قضایی
                        case 37:
                            var Ghazaye = Convert.ToDouble(item.Mablagh);
                            Ghazaye = Math.Round((newroz * Ghazaye) / Days);
                            if (Ghazaye != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(Ghazaye);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(Ghazaye), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------گردان عاشورا
                        case 38:
                            var Ashura = Convert.ToDouble(item.Mablagh);
                            Ashura = Math.Round((newroz * Ashura) / Days);
                            if (Ashura != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(Ashura);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(Ashura), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------ضریب تعدیل
                        case 39:
                            var Tadil = Convert.ToDouble(item.Mablagh);
                            Tadil = Math.Round((newroz * Tadil) / Days);
                            if (Tadil != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(Tadil);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(Tadil), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------جذب مشاغل تخصصی
                        case 40:
                            var Takhasos = Convert.ToDouble(item.Mablagh);
                            Takhasos = Math.Round((newroz * Takhasos) / Days);
                            if (Takhasos != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(Takhasos);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(Takhasos), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------جذب تعدیل
                        case 41:
                            var jazbTadil = Convert.ToDouble(item.Mablagh);
                            jazbTadil = Math.Round((newroz * jazbTadil) / Days);
                            if (jazbTadil != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(jazbTadil);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(jazbTadil), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------تفاوت ناشی از ضریب تعدیل
                        case 42:
                            var hadaghalTadil = Convert.ToDouble(item.Mablagh);
                            hadaghalTadil = Math.Round((newroz * hadaghalTadil) / Days);
                            if (hadaghalTadil != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(hadaghalTadil);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(hadaghalTadil), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------بند ی
                        case 44:
                            var band_y = Convert.ToDouble(item.Mablagh);
                            band_y = Math.Round((newroz * band_y) / Days);
                            if (band_y != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(band_y);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(band_y), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        case 45://--------------------جزء1
                            var joz1 = Convert.ToDouble(item.Mablagh);
                            joz1 = Math.Round((newroz * joz1) / Days);
                            if (joz1 != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(joz1);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(joz1), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------بند6
                        case 46:
                            var band6 = Convert.ToDouble(item.Mablagh);
                            band6 = Math.Round((newroz * band6) / Days);
                            if (band6 != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(band6);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(band6), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------جذب2
                        case 47:
                            var jazb2 = Convert.ToDouble(item.Mablagh);
                            jazb2 = Math.Round((newroz * jazb2) / Days);
                            if (jazb2 != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(jazb2);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(jazb2), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------جذب3
                        case 48:
                            var jazb3 = Convert.ToDouble(item.Mablagh);
                            jazb3 = Math.Round((newroz * jazb3) / Days);
                            if (jazb3 != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(jazb3);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(jazb3), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------ویژه2
                        case 49:
                            var vije2 = Convert.ToDouble(item.Mablagh);
                            vije2 = Math.Round((newroz * vije2) / Days);
                            if (vije2 != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(vije2);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(vije2), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------ویژه3
                        case 50:
                            var vije3 = Convert.ToDouble(item.Mablagh);
                            vije3 = Math.Round((newroz * vije3) / Days);
                            if (vije3 != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(vije3);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(vije3), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------مخصوص2
                        case 51:
                            var makhsos2 = Convert.ToDouble(item.Mablagh);
                            makhsos2 = Math.Round((newroz * makhsos2) / Days);
                            if (makhsos2 != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(makhsos2);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(makhsos2), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------مخصوص3
                        case 52:
                            var makhsos3 = Convert.ToDouble(item.Mablagh);
                            makhsos3 = Math.Round((newroz * makhsos3) / Days);
                            if (makhsos3 != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(makhsos3);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(makhsos3), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------تطبیق1
                        case 53:
                            var tatbigh1 = Convert.ToDouble(item.Mablagh);
                            tatbigh1 = Math.Round((newroz * tatbigh1) / Days);
                            if (tatbigh1 != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(tatbigh1);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(tatbigh1), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //--------------------خاص
                        case 54:
                            var khas = Convert.ToDouble(item.Mablagh);
                            khas = Math.Round((newroz * khas) / Days);
                            if (khas != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(khas);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(khas), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //ترمیم حقوق
                        case 57:
                            var tarmim_h = Convert.ToDouble(item.Mablagh);
                            tarmim_h = Math.Round((newroz * tarmim_h) / Days);
                            if (tarmim_h != 0)
                            {
                                var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (s != null)
                                    s.fldMablagh += Convert.ToInt32(tarmim_h);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(tarmim_h), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //جذب ناشی از ترمیم حقوق
                        case 60:
                            var tarmim_jazb = Convert.ToDouble(item.Mablagh);
                            tarmim_jazb = Math.Round((newroz * tarmim_jazb) / Days);
                            if (tarmim_jazb != 0)
                            {
                                var tj = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (tj != null)
                                    tj.fldMablagh += Convert.ToInt32(tarmim_jazb);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(tarmim_jazb), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //ویژه ناشی از ترمیم حقوق
                        case 61:
                            var tarmim_v = Convert.ToDouble(item.Mablagh);
                            tarmim_v = Math.Round((newroz * tarmim_v) / Days);
                            if (tarmim_v != 0)
                            {
                                var tv = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (tv != null)
                                    tv.fldMablagh += Convert.ToInt32(tarmim_v);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(tarmim_v), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                        //خاص ناشی از ترمیم حقوق
                        case 62:
                            var tarmim_k = Convert.ToDouble(item.Mablagh);
                            tarmim_k = Math.Round((newroz * tarmim_k) / Days);
                            if (tarmim_k != 0)
                            {
                                var tk = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.ItemHoghughiId).FirstOrDefault();
                                if (tk != null)
                                    tk.fldMablagh += Convert.ToInt32(tarmim_k);
                                else
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(tarmim_k), fldItemEstekhdamId = item.ItemEstekhdamId, fldItemHoghoghiId = item.ItemHoghughiId });
                            }
                            break;
                       
                    }
                }
                
                //--------------------سنوات پایان خدمت                
                if (TypeEstekhdam == 1 && PersonalInfo.fldSanavatPayanKhedmat == true)
                {
                    int karkerd = Convert.ToInt32(rr - rr_ghibat);//Convert.ToInt32(read_mahane["karkerd"]);
                    //maskan = Math.Round((Convert.ToDouble(read_hokm["haghmaskan"]) / 30) * (rr - rr_ghibat), 0);
                    if (karkerd > 0)
                    {
                        var mabna = LastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault();
                        var sanavat2 = LastHokm.Items.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();
                        var paye2 = LastHokm.Items.Where(l => l.ItemHoghughiId == 3).FirstOrDefault();
                        var jebhe2 = LastHokm.Items.Where(l => l.ItemHoghughiId == 30).FirstOrDefault();
                        var janbazi2 = LastHokm.Items.Where(l => l.ItemHoghughiId == 31).FirstOrDefault();
                        int SanavatP = 0;
                        if (mabna != null)
                            SanavatP += mabna.Mablagh;
                        if (sanavat2 != null)
                            SanavatP += sanavat2.Mablagh;
                        if (paye2 != null)
                            SanavatP += paye2.Mablagh;
                        if (jebhe2 != null)
                            SanavatP += jebhe2.Mablagh;
                        if (janbazi2 != null)
                            SanavatP += janbazi2.Mablagh;
                        sanavat_payanKhedmat = Math.Round(((SanavatP) / 30 * (rr - rr_ghibat)) / 12);
                        if (sanavat_payanKhedmat != 0)
                        {
                            var s = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 36).FirstOrDefault();
                            if (s != null)
                                s.fldMablagh += Convert.ToInt32(sanavat_payanKhedmat);
                            else
                                mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(sanavat_payanKhedmat), fldItemEstekhdamId = GetItem_Estekhdam(36, TypeEstekhdam), fldItemHoghoghiId = 36 });
                        }
                        int kasrKarkerd = 0;
                        if (Month <= 6 && karkerd < 31)
                        {
                            kasrKarkerd = 30 - karkerd;
                        }
                        else if (Month >= 6 && Month <= 11 && karkerd < 30)
                        {
                            kasrKarkerd = 30 - karkerd;
                        }
                        else if (Month == 12 && MyLib.Shamsi.Iskabise(Year) == false && karkerd < 29)
                        {
                            kasrKarkerd = 30 - karkerd;
                        }
                        else if (Month == 12 && MyLib.Shamsi.Iskabise(Year) == true && karkerd < 30)
                        {
                            kasrKarkerd = 30 - karkerd;
                        }
                        //if (kasrKarkerd > 0)
                        //sanavat_payanKhedmat = Math.Round(sanavat_payanKhedmat - (sanavat_payanKhedmat / 30 * kasrKarkerd));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Mohasebat motalebat_hog(Hokm LastHokm, MoteghayerhayeHoghoghi moteghayer
                                       , NewFMS.WCF_PayRoll.OBJ_KarKardeMahane karkard, int TypeEstekhdam, int TypeBimeId, int Days, bool Moavaghe,
                                       int Year, int Month, int PayPersonalId, int Jensiyat,int PrsPersonalId)
        {
            try
            {
                double ezafekari = 0, mamoriat = 0,
                motalebe1 = 0, motalebe2 = 0, motalebe3 = 0, motalebe4 = 0,
                tatilkari = 0, t_tatilkari = 0;
                double t_ezafekari = 0, t_mamoriyat = 0, _satroz = 0, _zarib = 0;
                double h_rozane2 = 0, ezafekari160 = 0, e160 = 0, ezafekari111 = 0, e111 = 0;
                double t_ezafekari160 = 0, t_ezafekari111 = 0, jam_motalebe = 0;
                bool h = false, f = false, t = false, Vizhe = false, h_j = false, Tadil = false, Sanavat = false, BarJastegi = false;
                double t_mamoriyat_baranandegi = 0, t_mamoriyat_bedonranandegi = 0;
                double zarib = 0;
                double mashmol_bime = 0/* hog.motalebat_m +hog.m_bime*/;
                zarib = Convert.ToDouble(moteghayer.fldZaribHoghoghiSal);
                //--------------------مشمولی بیمه جهت محاسبه ماموریت به روش جدید                    
                foreach (var item in moteghayer.Items)
                {
                    var Mohasebe_Item = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.fldItemHoghughiId).FirstOrDefault();
                    if (Mohasebe_Item != null)
                        mashmol_bime += Mohasebe_Item.fldMablagh;
                }
                double hoghogh_sanavat = 0;
                double h_rozane3 = 0;
                double _satroz1 = 0, _zarib1 = 0;
                bool ghati = Convert.ToBoolean(karkard.fldGhati);
                string commandText = ""; int KarkarRoz = karkard.fldKarkard;
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                if (Year >= 1402)
                {
                    //مزایای جانبی رفاهی
                    int mazayaRefahi = 25000000;
                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(mazayaRefahi), fldItemEstekhdamId = GetItem_Estekhdam(63, TypeEstekhdam), fldItemHoghoghiId = 63 });

                    //هزینه مهد کودک
                    if (LastHokm.TedadFarzand > 0 && Jensiyat == 0 && KarkarRoz > 0)
                    {

                        commandText = "select fldAge,COUNT(*) over (partition by fldPersonalId) as fldCount from (" +
                           "select DATEDIFF(DAY,dbo.Fn_AssembelyShamsiToMiladiDate(fldBirthDate),getdate())/365 AS fldAge,fldPersonalId from prs.tblAfradTahtePooshesh " +
                          "where fldPersonalId=" + PrsPersonalId + " and  fldNesbat=1)t  where fldAge<6 ";
                        var tblMahd = service.GetData(commandText).Tables[0];
                        if (tblMahd.Rows.Count > 0)
                        {
                            int mahd = ((2500000 * Convert.ToInt32(tblMahd.Rows[0]["fldCount"])) / Days) * KarkarRoz;
                            mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(mahd), fldItemEstekhdamId = GetItem_Estekhdam(64, TypeEstekhdam), fldItemHoghoghiId = 64 });
                        }
                    }
                    //اقلام خوراکی
                    int MablaghKhoraki = 0, hamsar = 0, farzand = 0, SourceId = 0;

                    MablaghKhoraki = 25000000;
                    if ((LastHokm.NoeTaahol != 1 && LastHokm.NoeTaahol != 2 && Jensiyat == 1) || (Jensiyat == 0 && (LastHokm.NoeTaahol == 5 || LastHokm.NoeTaahol == 6)))
                        hamsar = Convert.ToInt32(Math.Round((MablaghKhoraki * 0.2), 0));

                    if ((LastHokm.TedadFarzand > 0 && Jensiyat == 1) || (LastHokm.TedadFarzand > 0 && Jensiyat == 0 && (LastHokm.NoeTaahol == 5 || LastHokm.NoeTaahol == 6)))
                        farzand = Convert.ToInt32(Math.Round((LastHokm.TedadFarzand * (MablaghKhoraki * 0.2)), 0));
                    MablaghKhoraki = MablaghKhoraki + hamsar + farzand;
                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(MablaghKhoraki), fldItemEstekhdamId = GetItem_Estekhdam(65, TypeEstekhdam), fldItemHoghoghiId = 65 });

                    //کالاهای بهداشتی

                    int MablaghKalaBehdashti = 10000000;

                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(MablaghKalaBehdashti), fldItemEstekhdamId = GetItem_Estekhdam(66, TypeEstekhdam), fldItemHoghoghiId = 66 });


                    //-----------------------------------مناسبت
                    if (karkard.fldGheybat < Days)
                    {
                        //string commandText = "select COUNT(*) as fldCount from prs.tblAfradTahtePooshesh " +
                        //     "where fldPersonalId=" + PrsId + " and fldMashmul=1 and fldNesbat=1 ";
                        //var c = service.GetData(commandText).Tables[0];
                        //TedadFarzandan = Convert.ToInt32(c.Rows[0]["fldCount"]);

                        commandText = "SELECT top(1) fldId " +
                         "FROM   [Pay].[tblMonasebatHeader] as m" +
                         " WHERE fldActiveDate<=" + Year * 100 + Month + " and fldActive=1 ORDER BY fldActiveDate desc";
                        var header = service.GetData(commandText).Tables[0];

                        commandText = "SELECT  m.fldId,M.[fldMonasebatId], [fldMablagh], [fldTypeNesbatId] " +
                         "FROM   [Pay].[tblMonasebatMablagh] as m" +
                         " inner join pay.tblMonasebatTarikh as t on t.fldMonasebatId=m.fldMonasebatId" +
                         " WHERE m.fldHeaderId=" + Convert.ToInt32(header.Rows[0]["fldId"]) + " AND t.fldYear=" + Year + " and t.fldMonth=" + Month;
                        var tblMonasebat = service.GetData(commandText).Tables[0];
                        int MablaghMonasebat = 0;
                        Monasebat monasebat = new Monasebat();
                        List<Monasebat> monasebats = new List<Monasebat>();
                        if (tblMonasebat.Rows.Count > 0)
                        {
                            string searchExpression = "fldTypeNesbatId = 1";
                            DataRow[] foundRows = tblMonasebat.Select(searchExpression);
                            for (int i = 0; i < foundRows.Length; i++)
                            {
                                MablaghMonasebat = Convert.ToInt32(foundRows[i]["fldMablagh"]);
                                SourceId = Convert.ToInt32(foundRows[i]["fldId"]);
                                mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(MablaghMonasebat), fldItemEstekhdamId = GetItem_Estekhdam(67, TypeEstekhdam), fldItemHoghoghiId = 67, fldSourceId = SourceId });
                            }
                            if ((LastHokm.NoeTaahol != 1 && LastHokm.NoeTaahol != 2 && Jensiyat == 1) || (Jensiyat == 0 && (LastHokm.NoeTaahol == 5 || LastHokm.NoeTaahol == 6)))
                            {
                                searchExpression = "fldTypeNesbatId = 2 ";
                                DataRow[] foundRows2 = tblMonasebat.Select(searchExpression);
                                for (int i = 0; i < foundRows2.Length; i++)
                                {
                                    MablaghMonasebat = Convert.ToInt32(foundRows2[i]["fldMablagh"]);
                                    SourceId = Convert.ToInt32(foundRows2[i]["fldId"]);
                                    mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(MablaghMonasebat), fldItemEstekhdamId = GetItem_Estekhdam(67, TypeEstekhdam), fldItemHoghoghiId = 67, fldSourceId = SourceId });
                                }

                            }
                            if (((Jensiyat == 1) || (Jensiyat == 0 && (LastHokm.NoeTaahol == 5 || LastHokm.NoeTaahol == 6))) && LastHokm.TedadFarzand > 0)
                            {
                                commandText = "SELECT  m.[fldMonasebatId] " +
                     "FROM   [Pay].[tblMonasebatMablagh] as m" +
                     " inner join pay.tblMonasebatTarikh as t on t.fldMonasebatId=m.fldMonasebatId" +
                     " WHERE m.fldHeaderId=" + Convert.ToInt32(header.Rows[0]["fldId"]) + " AND t.fldYear=" + Year + " and t.fldMonth=" + Month + " group by m.fldMonasebatId";
                                var tblMonasebatFarzand = service.GetData(commandText).Tables[0];
                                for (int j = 0; j < tblMonasebatFarzand.Rows.Count; j++)
                                {
                                    searchExpression = "fldTypeNesbatId >=3 and fldMonasebatId=" + (tblMonasebatFarzand.Rows[0]["fldMonasebatId"]).ToString();
                                    DataRow[] foundRows3 = tblMonasebat.Select(searchExpression);

                                    if (LastHokm.TedadFarzand <= foundRows3.Length)
                                        for (int i = 0; i < LastHokm.TedadFarzand; i++)
                                        {
                                            MablaghMonasebat = Convert.ToInt32(foundRows3[i]["fldMablagh"]);
                                            SourceId = Convert.ToInt32(foundRows3[i]["fldId"]);
                                            mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(MablaghMonasebat), fldItemEstekhdamId = GetItem_Estekhdam(67, TypeEstekhdam), fldItemHoghoghiId = 67, fldSourceId = SourceId });
                                        }
                                    else
                                    {
                                        searchExpression = "fldTypeNesbatId =6 and fldMonasebatId=" + (tblMonasebatFarzand.Rows[0]["fldMonasebatId"]).ToString();
                                        DataRow[] foundRows4 = tblMonasebat.Select(searchExpression);
                                        for (int i = 0; i < LastHokm.TedadFarzand - foundRows3.Length; i++)
                                        {
                                            MablaghMonasebat = Convert.ToInt32(foundRows3[i]["fldMablagh"]);
                                            SourceId = Convert.ToInt32(foundRows3[i]["fldId"]);
                                            mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(MablaghMonasebat), fldItemEstekhdamId = GetItem_Estekhdam(67, TypeEstekhdam), fldItemHoghoghiId = 67, fldSourceId = SourceId });
                                        }
                                    }

                                }

                            }
                        }
                    }
                    ///////////////////////////////////////////////////مناسبت پایان
                    //پاداش جمعیت جوانی

                    commandText = "select f.fldId,fldNesbatShakhs,fldCount from prs.tblAfradTahtePooshesh as f " +
                                    "left join pay.tblMohasebat_Items as i  " +
                                    "inner join com.tblItems_Estekhdam as e on e.fldId=i.fldItemEstekhdamId " +
                                    "on i.fldSourceId=f.fldid and e.fldItemsHoghughiId=68 " +
                                    "outer apply(select count(*)+1 as fldCount from prs.tblAfradTahtePooshesh as f2 where f2.fldPersonalId=f.fldPersonalId and f.fldNesbat=f2.fldNesbat and f.fldBirthDate<f2.fldBirthDate)f2 " +
                                    "where i.fldSourceId is null  and fldNesbat in (1,2)  and f.fldMashmoolPadash =1";
                    var tblJavani = service.GetData(commandText).Tables[0];
                    int mablaghJavani = 0;
                    for (int i = 0; i < tblJavani.Rows.Count; i++)
                    {
                        SourceId = Convert.ToInt32(tblJavani.Rows[i]["fldId"]);
                        if (Convert.ToInt32(tblJavani.Rows[i]["fldNesbatShakhs"]) == 2)
                            mablaghJavani = 20000000;
                        else if (Convert.ToInt32(tblJavani.Rows[i]["fldNesbatShakhs"]) == 1)
                        {
                            if (Convert.ToInt32(tblJavani.Rows[i]["fldCount"]) == 1)
                                mablaghJavani = 15000000;
                            else if (Convert.ToInt32(tblJavani.Rows[i]["fldCount"]) == 2)
                                mablaghJavani = 30000000;
                            else if (Convert.ToInt32(tblJavani.Rows[i]["fldCount"]) == 3)
                                mablaghJavani = 40000000;
                            else if (Convert.ToInt32(tblJavani.Rows[i]["fldCount"]) == 4)
                                mablaghJavani = 50000000;
                            else if (Convert.ToInt32(tblJavani.Rows[i]["fldCount"]) >= 5)
                                mablaghJavani = 60000000;

                        }
                        mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(mablaghJavani), fldItemEstekhdamId = GetItem_Estekhdam(68, TypeEstekhdam), fldItemHoghoghiId = 68, fldSourceId = SourceId });
                    }

                    /// پایان پاداش
                }
                if (TypeEstekhdam == 1 && Days > 0)
                {
                    h_rozane2 = 0;
                    //--------------------اضافه کاری کارگر   
                    if (Convert.ToDouble(karkard.fldKarkard) > 0)
                    {
                        var hoghogh = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 1).FirstOrDefault();
                        if (hoghogh != null)
                            h_rozane2 += hoghogh.fldMablagh;
                        var sanavat = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 2).FirstOrDefault();
                        if (sanavat != null)
                            h_rozane2 += sanavat.fldMablagh;
                        var paye = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 3).FirstOrDefault();
                        if (paye != null)
                            h_rozane2 += paye.fldMablagh;
                        var jebhe = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 30).FirstOrDefault();
                        if (jebhe != null)
                            h_rozane2 += jebhe.fldMablagh;
                        var janbazi = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 31).FirstOrDefault();
                        if (janbazi != null)
                            h_rozane2 += janbazi.fldMablagh;
                        var ashora = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 38).FirstOrDefault();
                        if (ashora != null)
                            h_rozane2 += ashora.fldMablagh;
                        h_rozane2 = h_rozane2 / Convert.ToDouble(karkard.fldKarkard);
                    }
                    else
                    {
                        var hoghogh = LastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault();
                        if (hoghogh != null)
                            h_rozane2 += hoghogh.Mablagh;
                        var sanavat = LastHokm.Items.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();
                        if (sanavat != null)
                            h_rozane2 += sanavat.Mablagh;
                        var paye = LastHokm.Items.Where(l => l.ItemHoghughiId == 3).FirstOrDefault();
                        if (paye != null)
                            h_rozane2 += paye.Mablagh;
                        var jebhe = LastHokm.Items.Where(l => l.ItemHoghughiId == 30).FirstOrDefault();
                        if (jebhe != null)
                            h_rozane2 += jebhe.Mablagh;
                        var janbazi = LastHokm.Items.Where(l => l.ItemHoghughiId == 31).FirstOrDefault();
                        if (janbazi != null)
                            h_rozane2 += janbazi.Mablagh;
                        var ashora = LastHokm.Items.Where(l => l.ItemHoghughiId == 38).FirstOrDefault();
                        if (ashora != null)
                            h_rozane2 += ashora.Mablagh;
                        h_rozane2 = (h_rozane2) / 30;
                    }
                    _satroz = Convert.ToDouble(moteghayer.fldSaatKari);
                    _zarib = Convert.ToDouble(moteghayer.fldZaribEzafeKar);
                    if (ghati == true)
                    {
                        t_ezafekari = Convert.ToDouble(karkard.fldEzafeKari);
                        ezafekari = Math.Round((((Convert.ToDouble(h_rozane2) / Convert.ToDouble(_satroz)) * Convert.ToDouble(_zarib)) / 100) * Convert.ToDouble(t_ezafekari), 0);
                    }
                    else
                    {
                        t_ezafekari = 0;
                        ezafekari = 0;
                    }
                    if (ezafekari > 0)
                    {
                        mohasebe.fldTedadEzafeKar = Convert.ToByte(t_ezafekari);
                        mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(ezafekari), fldItemEstekhdamId = GetItem_Estekhdam(33, TypeEstekhdam), fldItemHoghoghiId = 33 });
                    }
                    h_rozane3 = 0; h_rozane2 = 0;
                    //--------------------تعطیل کاری کارگر  
                    if (Convert.ToDouble(karkard.fldKarkard) > 0)
                    {
                        var hoghogh = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 1).FirstOrDefault();
                        if (hoghogh != null)
                            h_rozane3 += hoghogh.fldMablagh;
                        var sanavat = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 2).FirstOrDefault();
                        if (sanavat != null)
                            h_rozane3 += sanavat.fldMablagh;
                        var paye = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 3).FirstOrDefault();
                        if (paye != null)
                            h_rozane3 += paye.fldMablagh;
                        var jebhe = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 30).FirstOrDefault();
                        if (jebhe != null)
                            h_rozane3 += jebhe.fldMablagh;
                        var janbazi = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 31).FirstOrDefault();
                        if (janbazi != null)
                            h_rozane3 += janbazi.fldMablagh;
                        var ashora = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 38).FirstOrDefault();
                        if (ashora != null)
                            h_rozane3 += ashora.fldMablagh;
                        h_rozane3 = h_rozane3 / Convert.ToDouble(karkard.fldKarkard);
                    }
                    else
                    {
                        var hoghogh = LastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault();
                        if (hoghogh != null)
                            h_rozane3 += hoghogh.Mablagh;
                        var sanavat = LastHokm.Items.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();
                        if (sanavat != null)
                            h_rozane3 += sanavat.Mablagh;
                        var paye = LastHokm.Items.Where(l => l.ItemHoghughiId == 3).FirstOrDefault();
                        if (paye != null)
                            h_rozane3 += paye.Mablagh;
                        var jebhe = LastHokm.Items.Where(l => l.ItemHoghughiId == 30).FirstOrDefault();
                        if (jebhe != null)
                            h_rozane3 += jebhe.Mablagh;
                        var janbazi = LastHokm.Items.Where(l => l.ItemHoghughiId == 31).FirstOrDefault();
                        if (janbazi != null)
                            h_rozane3 += janbazi.Mablagh;
                        var ashora = LastHokm.Items.Where(l => l.ItemHoghughiId == 38).FirstOrDefault();
                        if (ashora != null)
                            h_rozane3 += ashora.Mablagh;
                        h_rozane3 = (h_rozane3) / 30;
                    }
                    _satroz1 = Convert.ToDouble(moteghayer.fldSaatKari);
                    _zarib1 = Convert.ToDouble(moteghayer.fldZaribEzafeKar);
                    //if (tatilkariType == false)
                    t_tatilkari = Convert.ToDouble(karkard.fldTatileKari) * _satroz1;
                    /*else
          t_tatilkari = Convert.ToDouble(read_mahane["tatilkari"]);*/
                    tatilkari = Math.Round((((Convert.ToDouble(h_rozane3) / Convert.ToDouble(_satroz1)) * Convert.ToDouble(_zarib1)) / 100) * Convert.ToDouble(t_tatilkari), 0);
                    if (tatilkari > 0)
                    {
                        mohasebe.fldTedadTatilKar = Convert.ToByte(t_tatilkari);
                        mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(tatilkari), fldItemEstekhdamId = GetItem_Estekhdam(35, TypeEstekhdam), fldItemHoghoghiId = 35 });
                    }
                    //--------------------ماموریت کارگر      
                    if (Convert.ToDouble(karkard.fldKarkard) > 0)
                    {
                        var hoghogh = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 1).FirstOrDefault();
                        if (hoghogh != null)
                            hoghogh_sanavat += hoghogh.fldMablagh;
                        var sanavat = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 2).FirstOrDefault();
                        if (sanavat != null)
                            hoghogh_sanavat += sanavat.fldMablagh;
                        var paye = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 3).FirstOrDefault();
                        if (paye != null)
                            hoghogh_sanavat += paye.fldMablagh;
                        var jebhe = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 30).FirstOrDefault();
                        if (jebhe != null)
                            hoghogh_sanavat += jebhe.fldMablagh;
                        var janbazi = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 31).FirstOrDefault();
                        if (janbazi != null)
                            hoghogh_sanavat += janbazi.fldMablagh;
                        var ashora = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 38).FirstOrDefault();
                        if (ashora != null)
                            hoghogh_sanavat += ashora.fldMablagh;
                        hoghogh_sanavat = (hoghogh_sanavat) / Convert.ToDouble(karkard.fldKarkard);
                    }
                    else
                    {
                        var hoghogh = LastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault();
                        if (hoghogh != null)
                            hoghogh_sanavat += hoghogh.Mablagh;
                        var sanavat = LastHokm.Items.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();
                        if (sanavat != null)
                            hoghogh_sanavat += sanavat.Mablagh;
                        var paye = LastHokm.Items.Where(l => l.ItemHoghughiId == 3).FirstOrDefault();
                        if (paye != null)
                            hoghogh_sanavat += paye.Mablagh;
                        var jebhe = LastHokm.Items.Where(l => l.ItemHoghughiId == 30).FirstOrDefault();
                        if (jebhe != null)
                            hoghogh_sanavat += jebhe.Mablagh;
                        var janbazi = LastHokm.Items.Where(l => l.ItemHoghughiId == 31).FirstOrDefault();
                        if (janbazi != null)
                            hoghogh_sanavat += janbazi.Mablagh;
                        var ashora = LastHokm.Items.Where(l => l.ItemHoghughiId == 38).FirstOrDefault();
                        if (ashora != null)
                            hoghogh_sanavat += ashora.Mablagh;
                        hoghogh_sanavat = hoghogh_sanavat / 30;
                    }
                    t_mamoriyat = Convert.ToDouble(karkard.fldMamoriatBaBeitote);
                    mamoriat = Math.Round(Convert.ToDouble(t_mamoriyat) * hoghogh_sanavat, 0);
                    if (mamoriat > 0)
                    {
                        mohasebe.fldBaBeytute = Convert.ToByte(t_mamoriyat);
                        mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(mamoriat), fldItemEstekhdamId = GetItem_Estekhdam(34, TypeEstekhdam), fldItemHoghoghiId = 34 });
                    }
                    h_rozane2 = 0;
                    //--------------------حق شیفت کارگر   
                    if (Convert.ToDouble(karkard.fldKarkard) > 0)
                    {
                        var hoghogh = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 1).FirstOrDefault();
                        if (hoghogh != null)
                            h_rozane2 += hoghogh.fldMablagh;
                        var sanavat = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 2).FirstOrDefault();
                        if (sanavat != null)
                            h_rozane2 += sanavat.fldMablagh;
                        var paye = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 3).FirstOrDefault();
                        if (paye != null)
                            h_rozane2 += paye.fldMablagh;
                        var jebhe = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 30).FirstOrDefault();
                        if (jebhe != null)
                            h_rozane2 += jebhe.fldMablagh;
                        var janbazi = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 31).FirstOrDefault();
                        if (janbazi != null)
                            h_rozane2 += janbazi.fldMablagh;
                        var ashora = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 38).FirstOrDefault();
                        if (ashora != null)
                            h_rozane2 += ashora.fldMablagh;
                        h_rozane2 = h_rozane2 / Convert.ToDouble(karkard.fldKarkard);
                    }
                    else
                    {
                        var hoghogh = LastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault();
                        if (hoghogh != null)
                            h_rozane2 += hoghogh.Mablagh;
                        var sanavat = LastHokm.Items.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();
                        if (sanavat != null)
                            h_rozane2 += sanavat.Mablagh;
                        var paye = LastHokm.Items.Where(l => l.ItemHoghughiId == 3).FirstOrDefault();
                        if (paye != null)
                            h_rozane2 += paye.Mablagh;
                        var jebhe = LastHokm.Items.Where(l => l.ItemHoghughiId == 30).FirstOrDefault();
                        if (jebhe != null)
                            h_rozane2 += jebhe.Mablagh;
                        var janbazi = LastHokm.Items.Where(l => l.ItemHoghughiId == 31).FirstOrDefault();
                        if (janbazi != null)
                            h_rozane2 += janbazi.Mablagh;
                        var ashora = LastHokm.Items.Where(l => l.ItemHoghughiId == 38).FirstOrDefault();
                        if (ashora != null)
                            h_rozane2 += ashora.Mablagh;
                        h_rozane2 = (h_rozane2) / 30;
                    }
                    _satroz = Convert.ToDouble(moteghayer.fldSaatKari);
                    _zarib = Convert.ToDouble(35);
                    double t_shift = Convert.ToDouble(karkard.fldShift);
                    double shift = Math.Round((((Convert.ToDouble(h_rozane2) / Convert.ToDouble(_satroz)) * Convert.ToDouble(_zarib)) / 100) * Convert.ToDouble(t_shift), 0);
                    if (shift > 0)
                    {
                        mohasebe.fldShift = Convert.ToByte(t_shift);
                        mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(shift), fldItemEstekhdamId = GetItem_Estekhdam(43, TypeEstekhdam), fldItemHoghoghiId = 43 });
                    }
                }
                else if (TypeEstekhdam > 1 && Days > 0)
                {
                    //--------------------اضافه کاری کارمند
                    h = Convert.ToBoolean(moteghayer.fldHoghogh);
                    f = Convert.ToBoolean(moteghayer.fldFoghShoghl);
                    t = Convert.ToBoolean(moteghayer.fldTafavotTatbigh);
                    Vizhe = Convert.ToBoolean(moteghayer.fldFoghVizhe);
                    h_j = Convert.ToBoolean(moteghayer.fldHaghJazb);
                    Tadil = Convert.ToBoolean(moteghayer.fldTadil);
                    Sanavat = Convert.ToBoolean(moteghayer.fldSanavat);
                    BarJastegi = Convert.ToBoolean(moteghayer.fldBarJastegi);
                    var Talash = Convert.ToBoolean(moteghayer.fldFoghTalash);
                    if (h == true)
                    {
                        e160 = e160 + LastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
                        e111 = e111 + LastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
                    }
                    if (f == true)
                    {
                        var fogh = LastHokm.Items.Where(l => l.ItemHoghughiId == 6).FirstOrDefault();//فوق العاده شغل
                        if (fogh != null)
                        {
                            e160 += fogh.Mablagh;
                            e111 += fogh.Mablagh;
                        }
                        var takhsos = LastHokm.Items.Where(l => l.ItemHoghughiId == 7).FirstOrDefault();//تخصصی
                        if (takhsos != null)
                        {
                            e160 += takhsos.Mablagh;
                            e111 += takhsos.Mablagh;
                        }
                        var made = LastHokm.Items.Where(l => l.ItemHoghughiId == 8).FirstOrDefault();//ماده 26
                        if (made != null)
                        {
                            e160 += made.Mablagh;
                            e111 += made.Mablagh;
                        }
                        var modiriyati = LastHokm.Items.Where(l => l.ItemHoghughiId == 9).FirstOrDefault();// مدیریتی
                        if (modiriyati != null)
                        {
                            e160 += modiriyati.Mablagh;
                            e111 += modiriyati.Mablagh;
                        }
                    }
                    if (t == true)
                    {
                        var tatbigh = LastHokm.Items.Where(l => l.ItemHoghughiId == 11).FirstOrDefault();
                        if (tatbigh != null)
                        {
                            e160 += tatbigh.Mablagh;
                            e111 += tatbigh.Mablagh;
                        }
                        var hadaghalTadil = LastHokm.Items.Where(l => l.ItemHoghughiId == 42).FirstOrDefault();
                        if (hadaghalTadil != null)
                        {
                            e160 += hadaghalTadil.Mablagh;
                            e111 += hadaghalTadil.Mablagh;
                        }
                    }
                    if (Vizhe == true)
                    {
                        var foghVizhe = LastHokm.Items.Where(l => l.ItemHoghughiId == 21).FirstOrDefault();
                        if (foghVizhe != null)
                        {
                            e160 = e160 + foghVizhe.Mablagh;
                            e111 = e111 + foghVizhe.Mablagh;
                        }
                        var foghVizhe2 = LastHokm.Items.Where(l => l.ItemHoghughiId == 49).FirstOrDefault();
                        if (foghVizhe2 != null)
                        {
                            e160 = e160 + foghVizhe2.Mablagh;
                            e111 = e111 + foghVizhe2.Mablagh;
                        }
                        var foghVizhe3 = LastHokm.Items.Where(l => l.ItemHoghughiId == 50).FirstOrDefault();
                        if (foghVizhe3 != null)
                        {
                            e160 = e160 + foghVizhe3.Mablagh;
                            e111 = e111 + foghVizhe3.Mablagh;
                        }
                    }
                    if (h_j == true)
                    {
                        var foghJazb = LastHokm.Items.Where(l => l.ItemHoghughiId == 19).FirstOrDefault();
                        if (foghJazb != null)
                        {
                            e160 = e160 + foghJazb.Mablagh;
                            e111 = e111 + foghJazb.Mablagh;
                        }
                        var ghazai = LastHokm.Items.Where(l => l.ItemHoghughiId == 37).FirstOrDefault();
                        if (ghazai != null)
                        {
                            e160 = e160 + ghazai.Mablagh;
                            e111 = e111 + ghazai.Mablagh;
                        }
                        var jazbTakhasosi = LastHokm.Items.Where(l => l.ItemHoghughiId == 40).FirstOrDefault();
                        if (jazbTakhasosi != null)
                        {
                            e160 = e160 + jazbTakhasosi.Mablagh;
                            e111 = e111 + jazbTakhasosi.Mablagh;
                        }
                        var zajbTadil = LastHokm.Items.Where(l => l.ItemHoghughiId == 41).FirstOrDefault();
                        if (zajbTadil != null)
                        {
                            e160 = e160 + zajbTadil.Mablagh;
                            e111 = e111 + zajbTadil.Mablagh;
                        }
                        var jazb2 = LastHokm.Items.Where(l => l.ItemHoghughiId == 47).FirstOrDefault();
                        if (jazb2 != null)
                        {
                            e160 = e160 + jazb2.Mablagh;
                            e111 = e111 + jazb2.Mablagh;
                        }
                        var jazb3 = LastHokm.Items.Where(l => l.ItemHoghughiId == 48).FirstOrDefault();
                        if (jazb3 != null)
                        {
                            e160 = e160 + jazb3.Mablagh;
                            e111 = e111 + jazb3.Mablagh;
                        }
                    }
                    if (Tadil == true)
                    {
                        var foghTadil = LastHokm.Items.Where(l => l.ItemHoghughiId == 16).FirstOrDefault();
                        if (foghTadil != null)
                        {
                            e160 = e160 + foghTadil.Mablagh;
                            e111 = e111 + foghTadil.Mablagh;
                        }
                        var zaribtadil = LastHokm.Items.Where(l => l.ItemHoghughiId == 39).FirstOrDefault();
                        if (zaribtadil != null)
                        {
                            e160 = e160 + zaribtadil.Mablagh;
                            e111 = e111 + zaribtadil.Mablagh;
                        }
                    }
                    if (Sanavat == true)
                    {
                        var sanavat1 = LastHokm.Items.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();//سنوات
                        if (sanavat1 != null)
                        {
                            e160 = e160 + sanavat1.Mablagh;
                            e111 = e111 + sanavat1.Mablagh;
                        }
                        var SanavatEsar = LastHokm.Items.Where(l => l.ItemHoghughiId == 5).FirstOrDefault();//سنوات ایثارگری
                        if (SanavatEsar != null)
                        {
                            e160 = e160 + SanavatEsar.Mablagh;
                            e111 = e111 + SanavatEsar.Mablagh;
                        }
                        var SanavatBasij = LastHokm.Items.Where(l => l.ItemHoghughiId == 4).FirstOrDefault();//سنوات ایثارگری
                        if (SanavatBasij != null)
                        {
                            e160 = e160 + SanavatBasij.Mablagh;
                            e111 = e111 + SanavatBasij.Mablagh;
                        }
                    }
                    if (BarJastegi == true)
                    {
                        var barJestegi = LastHokm.Items.Where(l => l.ItemHoghughiId == 10).FirstOrDefault();
                        if (barJestegi != null)
                        {
                            e160 = e160 + barJestegi.Mablagh;
                            e111 = e111 + barJestegi.Mablagh;
                        }
                    }
                    if (Talash == true)
                    {
                        var foghTalash = LastHokm.Items.Where(l => l.ItemHoghughiId == 29).FirstOrDefault();
                        if (foghTalash != null)
                        {
                            e160 = e160 + foghTalash.Mablagh;
                            e111 = e111 + foghTalash.Mablagh;
                        }
                    }
                    var band_y = LastHokm.Items.Where(l => l.ItemHoghughiId == 44).FirstOrDefault();
                    if (band_y != null)
                    {
                        e160 = e160 + band_y.Mablagh;
                        e111 = e111 + band_y.Mablagh;
                    }
                    var joz1 = LastHokm.Items.Where(l => l.ItemHoghughiId == 45).FirstOrDefault();
                    if (joz1 != null)
                    {
                        e160 = e160 + joz1.Mablagh;
                        e111 = e111 + joz1.Mablagh;
                    }
                    var Tarmim_j = LastHokm.Items.Where(l => l.ItemHoghughiId == 60).FirstOrDefault();/*جذب ناشی از ترمیم حقوق*/
                    if (Tarmim_j != null)
                    {
                        e160 = e160 + Tarmim_j.Mablagh;
                        e111 = e111 + Tarmim_j.Mablagh;
                    }
                    //if (Convert.ToDouble(read_mahane["karkerd"]) > 0)
                    //{
                    //    ezafekari160 = (e160 / Convert.ToDouble(read_mahane["karkerd"]) * 30) / 160;
                    //    ezafekari111 = (e111 / Convert.ToDouble(read_mahane["karkerd"]) * 30) / 111;
                    //}
                    //else
                    //{
                    ezafekari160 = e160 / 160;
                    ezafekari111 = e111 / 111;
                    //}
                    t_ezafekari160 = Convert.ToDouble(karkard.fldEzafeKari);
                    if (Moavaghe && t_ezafekari160 == 0)
                    {
                        t_ezafekari160 = Convert.ToDouble(karkard.fldEzafeKari);
                    }
                    //if (tatilkariType == false)
                    t_ezafekari111 = Convert.ToDouble(karkard.fldTatileKari) * (double)7.33;
                    /*else
          t_ezafekari111 = Convert.ToDouble(read_mahane["ezafekari_1/111"]);*/
                    if (ghati == true)
                    {
                        t_ezafekari = t_ezafekari160;// +t_ezafekari111;
                        ezafekari = Math.Round((Convert.ToDouble(ezafekari160) * Convert.ToDouble(t_ezafekari160)), 0);// + (Convert.ToDouble(ezafekari111) * Convert.ToDouble(t_ezafekari111)), 0);
                    }
                    else
                    {
                        t_ezafekari = 0;
                        ezafekari = 0;
                    }
                    t_tatilkari = t_ezafekari111;
                    tatilkari = Math.Round((Convert.ToDouble(ezafekari111) * Convert.ToDouble(t_ezafekari111)), 0);
                    if (ezafekari > 0)
                    {
                        mohasebe.fldTedadEzafeKar = Convert.ToByte(t_ezafekari);
                        mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(ezafekari), fldItemEstekhdamId = GetItem_Estekhdam(33, TypeEstekhdam), fldItemHoghoghiId = 33 });
                    }
                    if (tatilkari > 0)
                    {
                        mohasebe.fldTedadTatilKar = Convert.ToByte(t_tatilkari);
                        mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(tatilkari), fldItemEstekhdamId = GetItem_Estekhdam(35, TypeEstekhdam), fldItemHoghoghiId = 35 });
                    }
                    //--------------------ماموریت کارمند
                    t_mamoriyat_baranandegi = Convert.ToDouble(karkard.fldMamoriatBaBeitote);
                    t_mamoriyat_bedonranandegi = Convert.ToDouble(karkard.fldMamoriatBedoneBeitote);
                    t_mamoriyat = t_mamoriyat_baranandegi + t_mamoriyat_bedonranandegi;
                    double ba = 0, be = 0, ba10 = 0, ba20 = 0, ba30 = 0, be10 = 0, be20 = 0, be30 = 0;
                    //if (MamoriatType == false)سایر شهرها
                    //{
                    ba = (((400 * Convert.ToDouble(zarib)) / 2) * Convert.ToDouble(t_mamoriyat_baranandegi));
                    be = (((400 * Convert.ToDouble(zarib)) / 4) * Convert.ToDouble(t_mamoriyat_bedonranandegi));
                    if (t_mamoriyat_baranandegi > 0)
                    {
                        ba10 = ((ba / t_mamoriyat_baranandegi) * 10 / 100) * Convert.ToDouble(karkard.fldBa10);
                        ba20 = ((ba / t_mamoriyat_baranandegi) * 20 / 100) * Convert.ToDouble(karkard.fldBa20);
                        ba30 = ((ba / t_mamoriyat_baranandegi) * 30 / 100) * Convert.ToDouble(karkard.fldBa30);
                    }
                    if (t_mamoriyat_bedonranandegi > 0)
                    {
                        be10 = ((be / t_mamoriyat_bedonranandegi) * 10 / 100) * Convert.ToDouble(karkard.fldBe10);
                        be20 = ((be / t_mamoriyat_bedonranandegi) * 20 / 100) * Convert.ToDouble(karkard.fldBe20);
                        be30 = ((be / t_mamoriyat_bedonranandegi) * 30 / 100) * Convert.ToDouble(karkard.fldBe30);
                    }
                    mamoriat = Math.Round(ba + be + ba10 + ba20 + ba30 + be10 + be20 + be30, 0);
                    //else if (MamoriatType == true)دامغان
                    //{
                    /*string commandText = "SELECT        * " +
          "FROM            Prs.tblMoteghayerhaAhkam " +
          "WHERE fldYear=" + Year + "AND fldType=1";
          NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
          var tblMoteghayerhaAhkam = service.GetData(commandText).Tables[0];
          if (tblMoteghayerhaAhkam.Rows.Count != 0)
          {
          double Hadeaghal = Convert.ToDouble(tblMoteghayerhaAhkam.Rows[0]["fldHadaghalDaryafti"]);
          ba = ((Hadeaghal / 20) + ((mashmol_bime - Hadeaghal) / 50)) * t_mamoriyat_baranandegi;
          be = (((Hadeaghal / 20) + ((mashmol_bime - Hadeaghal) / 50)) / 2) * t_mamoriyat_bedonranandegi;
          mamoriat = Math.Round(ba + be);
          }*/
                    //}
                    if (mamoriat > 0)
                    {
                        mohasebe.fldBaBeytute = Convert.ToByte(t_mamoriyat);
                        mohasebe.Items.Add(new Mohasebat_Items { fldMablagh = Convert.ToInt32(mamoriat), fldItemEstekhdamId = GetItem_Estekhdam(34, TypeEstekhdam), fldItemHoghoghiId = 34 });
                    }
                }
                else
                {
                    ezafekari = 0;
                    t_ezafekari = 0;
                    mamoriat = 0;
                    t_mamoriyat = 0;
                }
                //--------------------مطالبات تفکیکی                
                double m_bime = 0, m_maliat = 0;
                double motalebat = 0;
                string jari1 = Year.ToString() + Month.ToString().PadLeft(2, '0');
                int tedad = 0;
                //if (Moavaghe == false)
                //{
                string commandText1 = "select * FROM            Pay.tblMotalebateParametri_Personal " +
                  "  WHERE fldPersonalId= " + PayPersonalId + " and " + jari1 + "<=fldDateDeactive";
                NewFMS.FormulaService.WebServiceFormula service1 = new NewFMS.FormulaService.WebServiceFormula();
                var tblMotalebateParametri_Personal = service1.GetData(commandText1).Tables[0];
                if (tblMotalebateParametri_Personal.Rows.Count > 0)
                {
                    for (int i = 0; i < tblMotalebateParametri_Personal.Rows.Count; i++)
                    {
                        tedad = 0;
                        int t_daryaft1 = Convert.ToInt32(tblMotalebateParametri_Personal.Rows[i]["fldTarikhPardakht"].ToString().Substring(0, 4) + tblMotalebateParametri_Personal.Rows[i]["fldTarikhPardakht"].ToString().Substring(5, 2));
                        if (t_daryaft1 <= Convert.ToInt32(jari1))
                        {
                            commandText1 = "SELECT        count(*)as Count " +
                              "FROM              Pay.[tblMohasebat_kosorat/MotalebatParam] " +
                              "where fldMotalebatId=" + Convert.ToInt32(tblMotalebateParametri_Personal.Rows[i]["fldId"]);
                            var tblMohasebat_kosorat_MotalebatParam = service1.GetData(commandText1).Tables[0];
                            if (tblMohasebat_kosorat_MotalebatParam.Rows.Count != 0)
                                tedad = Convert.ToInt32(tblMohasebat_kosorat_MotalebatParam.Rows[0]["Count"]);
                            Mohasebat Mohasebat_M = new Mohasebat();
                            if (Moavaghe == false)
                            {
                                if (Convert.ToInt32(tblMotalebateParametri_Personal.Rows[i]["fldNoePardakht"]) == 1)
                                {
                                    motalebat = motalebat + Convert.ToDouble(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"]);
                                    if (Convert.ToBoolean(tblMotalebateParametri_Personal.Rows[i]["fldMashmoleBime"]) == true)
                                        m_bime = m_bime + Convert.ToDouble(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"]);
                                    if (Convert.ToBoolean(tblMotalebateParametri_Personal.Rows[i]["fldMashmoleMaliyat"]) == true)
                                        m_maliat = m_maliat + Convert.ToDouble(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"]);
                                    mohasebe.Kosorat_MotalebatParam.Add(new Mohasebat_Kosorat_MotalebatParam
                                    {
                                        fldMotalebatId = Convert.ToInt32(tblMotalebateParametri_Personal.Rows[i]["fldId"]),
                                        fldMablagh = Convert.ToInt32(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"])//,fldKosoratId=null
                                    });
                                }
                                else
                                {
                                    if (tedad < Convert.ToInt32(tblMotalebateParametri_Personal.Rows[i]["fldTedad"]))
                                    {
                                        motalebat = motalebat + Convert.ToDouble(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"]);
                                        if (Convert.ToBoolean(tblMotalebateParametri_Personal.Rows[i]["fldMashmoleBime"]) == true)
                                            m_bime = m_bime + Convert.ToDouble(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"]);
                                        if (Convert.ToBoolean(tblMotalebateParametri_Personal.Rows[i]["fldMashmoleMaliyat"]) == true)
                                            m_maliat = m_maliat + Convert.ToDouble(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"]);
                                        mohasebe.Kosorat_MotalebatParam.Add(new Mohasebat_Kosorat_MotalebatParam
                                        {
                                            fldMotalebatId = Convert.ToInt32(tblMotalebateParametri_Personal.Rows[i]["fldId"]),
                                            fldMablagh = Convert.ToInt32(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"])//,fldKosoratId=null
                                        });
                                    }
                                }
                            }
                            else
                            {
                                motalebat = motalebat + Convert.ToDouble(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"]);
                                if (Convert.ToBoolean(tblMotalebateParametri_Personal.Rows[i]["fldMashmoleBime"]) == true)
                                    m_bime = m_bime + Convert.ToDouble(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"]);
                                if (Convert.ToBoolean(tblMotalebateParametri_Personal.Rows[i]["fldMashmoleMaliyat"]) == true)
                                    m_maliat = m_maliat + Convert.ToDouble(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"]);
                                mohasebe.Kosorat_MotalebatParam.Add(new Mohasebat_Kosorat_MotalebatParam
                                {
                                    fldMotalebatId = Convert.ToInt32(tblMotalebateParametri_Personal.Rows[i]["fldId"]),
                                    fldMablagh = Convert.ToInt32(tblMotalebateParametri_Personal.Rows[i]["fldMablagh"])//,fldKosoratId=null
                                });
                            }
                        }
                        mohasebe.fldMashmolBime = (int)m_bime;
                        mohasebe.fldMashmolMaliyat = (int)m_maliat;
                        /*con_m_motalebat.Close();
            con_m_motalebat1.Close();
            con_m_motalebat2.Close();*/
                    }
                }
                /* hog.motalebat_m = motalebat;
        hog.m_bime = m_bime;
        hog.m_maliat = m_maliat;
        con_motalebat.Close();*/
                //}
                double kolemotalebat = 0;
                //--------------------جمع کل مشمول بیمه و غیر مشمول
                /*  kolemotalebat = hog.hoghogh + hog.ezafekari + hog.mamoriat + hog.moavaghe + hog.sayer_motalebat + hog.sanavat + hog.fogh_shoghl +
        hog.tafavot_tatbigh + hog.hagholad + hog.arzeshyabi + hog.ayelemandi + hog.nobatkari + hog.maskan + hog.sakhtikar +
        hog.kharobar + hog.jazb + hog.tashilatzendegi + hog.badiabohava + hog.tarmim + hog.sayer_fogholadeha + hog.jazb_tabsare +
        hog.talash + hog.motalebat_m + hog.barjastegi + hog.tadil + hog.paye + hog.jebhe + hog.janbazi + hog.riali + hog.sanavat_isar
        + hog.sanavat_basij + hog.tatilkari + hog.sayer + hog.FoghIsar + hog.Takhasosi + hog.Made26 + hog.Modiriati + hog.sanavat_payan_khedmat + hog.Bon;
        hog.kol_motalebe = kolemotalebat;*/
                /*}
        con_mahane.Close();
        con_moteghyer.Close();*/
                return mohasebe;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void kosorat_hog(MoteghayerhayeHoghoghi Moteghayer, NewFMS.Models.Mohasebat.Fiscal Fiscal,
                           Hokm LastHokm, PersonalInfo PersonalInfo, int year, int month, int PayPersonalId, bool Mogharari, int PrsPersonalId,
                           int TypeEstekhdam, int TypeBimeId, bool Moavaghe, int NobatPardakht, int sal, int mah)
        {
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            try
            {
                decimal b_p = 0, b_k = 0, b_b = 0;
                int tedad_afradekhanevade = 0, t_takafol_60 = 0, t_takafol_70 = 0, t_takafol_60_t = 0, t_takafol_70_t = 0;
                switch (LastHokm.NoeTaahol)
                {
                    case 1:
                        tedad_afradekhanevade = 1;
                        break;
                    case 2:
                        tedad_afradekhanevade = 1 + LastHokm.TedadFarzand;
                        break;
                    case 3:
                        tedad_afradekhanevade = 2;
                        break;
                    case 4:
                        tedad_afradekhanevade = 2 + LastHokm.TedadFarzand;
                        break;
                }
                bool omr = false, takmili = false, takmili_60 = false, takmili_70 = false;
                if (PersonalInfo.fldBimeOmr == true)
                    omr = true;
                if (PersonalInfo.fldBimeTakmili == true)
                    takmili = true;
                //if (hokms.Count > 0)
                //{
                //    t_takafol_60 = hokm.a.tedade_t_tkalof_60;
                //    t_takafol_70 = hokm.a.tedade_t_tkalof_70;
                //}
                int bime_asli = 0;
                decimal bimeomr_p = 0, bimeomr_k = 0, bimetakmili_p = 0, bimetakmili_k = 0;
                string commandText = "SELECT * " +
                  "FROM            Pay.tblGHarardadBime " +
                  "WHERE fldTarikhSHoro<='" + year + "/" + month.ToString().PadLeft(2, '0') + "/01'" + " AND fldTarikhPayan>='" + year + "/" + month.ToString().PadLeft(2, '0') + "/01'";
                var tblGharardadbime = service.GetData(commandText).Tables[0];
                if (tblGharardadbime.Rows.Count != 0)
                {
                    int gharardadId = Convert.ToInt32(tblGharardadbime.Rows[0]["fldId"]);
                    commandText = "SELECT * " +
                      "FROM            Pay.tblAfradeTahtePoshesheBimeTakmily " +
                      "WHERE fldPersonalId=" + PayPersonalId + " AND fldGHarardadBimeId=" + gharardadId;
                    service = new NewFMS.FormulaService.WebServiceFormula();
                    var tblTahteposheshTakmili = service.GetData(commandText).Tables[0];
                    if (tblTahteposheshTakmili.Rows.Count != 0)
                    {
                        //بیمه تکمیلی جدید شاهرود
                        commandText = "SELECT * " +
                     "FROM            prs.tblAfradTahtePooshesh " +
                     "WHERE fldPersonalId=" + PrsPersonalId + " and fldNesbatShakhs in (3,4)";
                        service = new NewFMS.FormulaService.WebServiceFormula();
                        var tblAfradTahtePooshesh = service.GetData(commandText).Tables[0];
                        bime_asli = LastHokm.TedadFarzand+1;
                        if (LastHokm.NoeTaahol != 1 && LastHokm.NoeTaahol != 2)
                            bime_asli++;
                        if (tblAfradTahtePooshesh.Rows.Count != 0)
                        {
                            for (int i = 0; i < tblAfradTahtePooshesh.Rows.Count; i++)
                            {
                                if (Convert.ToInt32(tblAfradTahtePooshesh.Rows[i]["fldNesbat"]) == 5)/*پدر و مادر تحت پوشش تامین اجتماعی و 30 درصد با شهرداری*/
                                {
                                    t_takafol_60_t++; //Convert.ToInt32(tblTahteposheshTakmili.Rows[0]["fldTedadTakafol60Sal"]);

                                }
                                else
                                {
                                    t_takafol_70_t++; // Convert.ToInt32(tblTahteposheshTakmili.Rows[0]["fldTedadTakafol70Sal"]);
                                }
                            }
                        }
                    }
                    //--------------------بیمه عمر            
                    decimal darsad_o_p = 0, darsad_o_k = 0, darsad_t_p = 0, darsad_t_k = 0, bime_omr = 0,
                    bime_takmili = 0;
                    darsad_o_p = Convert.ToDecimal(tblGharardadbime.Rows[0]["fldDarsadBimeOmr"]);
                    darsad_o_k = 100 - darsad_o_p;
                    darsad_t_p = 0; //Convert.ToDecimal(tblGharardadbime.Rows[0]["fldDarsadBimeTakmily"]);//بیمه تکمیلی جدید شاهرود
                    darsad_t_k = 100 - darsad_t_p;
                    bime_omr = Convert.ToDecimal(tblGharardadbime.Rows[0]["fldMablagheBimeOmr"]);
                    bime_takmili = Convert.ToDecimal(tblGharardadbime.Rows[0]["fldMablagheBimeSHodeAsli"]);
                    decimal bime_takmili_60 = Convert.ToDecimal(tblGharardadbime.Rows[0]["fldMablaghe60Sal"]),
                    bime_takmili_70 = Convert.ToDecimal(tblGharardadbime.Rows[0]["fldMablaghe70Sal"]);
                    decimal darsad_t_60_p = 70 //Convert.ToDecimal(tblGharardadbime.Rows[0]["fldDarsadBime60Sal"]), //بیمه تکمیلی جدید شاهرود
                        ,darsad_t_60_k = 100 - darsad_t_60_p;
                    decimal darsad_t_70_p = 100//Convert.ToDecimal(tblGharardadbime.Rows[0]["fldDarsadBime70Sal"])//بیمه تکمیلی جدید شاهرود
                    , darsad_t_70_k = 100 - darsad_t_70_p
                    , darsad_t_p_Takafol = 30, darsad_t_k_Takafol = 70
                    , darsad_t_p_BedonTakafol = 100, darsad_t_k_BedonTakafol = 0;
                    int tedad_asli = Convert.ToInt32(tblGharardadbime.Rows[0]["fldMaxBimeAsli"]);
                    //if (hog.kol_motalebe > 0)
                    //{
                    //if (year < 1397)
                    //{
                    if (PersonalInfo.fldEsargariId == 1 || PersonalInfo.fldEsargariId == 4 || PersonalInfo.fldEsargariId == 5)
                    {
                        if (omr == true)
                        {
                            bimeomr_p = Math.Round(bime_omr * darsad_o_p / 100, 0);
                            bimeomr_k = Math.Round(bime_omr * darsad_o_k / 100, 0);
                        }
                        else if (omr == false)
                        {
                            bimeomr_p = 0;
                            bimeomr_k = 0;
                        }
                        if (takmili == true)
                        {
                            //بیمه تکمیلی جدید شاهرود
                            bimetakmili_p = Math.Round(bime_takmili * darsad_t_p / 100, 0) * bime_asli + Math.Round(bime_takmili * darsad_t_60_p / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili * darsad_t_70_p / 100, 0) * t_takafol_70_t;
                            bimetakmili_k = Math.Round(bime_takmili * darsad_t_k / 100, 0) * bime_asli + Math.Round(bime_takmili * darsad_t_60_k / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili * darsad_t_70_k / 100, 0) * t_takafol_70_t;
                           /* if (tedad_asli == 6)
                            {
                                bimetakmili_p = Math.Round(bime_takmili * darsad_t_p / 100, 0) * bime_asli + Math.Round(bime_takmili_60 * darsad_t_60_p / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_p / 100, 0) * t_takafol_70_t;
                                bimetakmili_k = Math.Round(bime_takmili * darsad_t_k / 100, 0) * bime_asli + Math.Round(bime_takmili_60 * darsad_t_60_k / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_k / 100, 0) * t_takafol_70_t;
                            }
                            else
                            {
                                if (bime_asli > tedad_asli)
                                {
                                    int mazad = bime_asli - tedad_asli;
                                    bimetakmili_p = (Math.Round(bime_takmili * darsad_t_p / 100, 0) * tedad_asli + Math.Round(bime_takmili * (darsad_t_p + darsad_t_k) / 100, 0) * mazad) + Math.Round(bime_takmili_60 * darsad_t_60_k / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_k / 100, 0) * t_takafol_70_t;
                                    bimetakmili_k = Math.Round(bime_takmili * darsad_t_k / 100, 0) * tedad_asli + Math.Round(bime_takmili_60 * darsad_t_60_k / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_k / 100, 0) * t_takafol_70_t;
                                }
                                else
                                {
                                    bimetakmili_p = Math.Round(bime_takmili * darsad_t_p / 100, 0) * bime_asli + Math.Round(bime_takmili_60 * darsad_t_60_p / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_p / 100, 0) * t_takafol_70_t;
                                    bimetakmili_k = Math.Round(bime_takmili * darsad_t_k / 100, 0) * bime_asli + Math.Round(bime_takmili_60 * darsad_t_60_k / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_k / 100, 0) * t_takafol_70_t;
                                }
                            }*/
                        }
                        else if (takmili == false)
                        {
                            bimetakmili_p = 0;
                            bimetakmili_k = 0;
                        }
                    }
                    else
                    {
                        if (omr == true)
                        {
                            bimeomr_p = 0;
                            bimeomr_k = Math.Round(bime_omr * (darsad_o_p + darsad_o_k) / 100, 0);
                        }
                        else if (omr == false)
                        {
                            bimeomr_p = 0;
                            bimeomr_k = 0;
                        }
                        if (takmili == true)
                        {
                            if (PersonalInfo.fldJensiyat == 1)
                            {
                                bimetakmili_p = 0;
                                bimetakmili_k = Math.Round(bime_takmili * (darsad_t_p + darsad_t_k) / 100, 0) * bime_asli + Math.Round(bime_takmili * (darsad_t_60_p + darsad_t_60_k) / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili * (darsad_t_70_p + darsad_t_70_k) / 100, 0) * t_takafol_70_t;
                                //bimetakmili_k = Math.Round(bime_takmili * (darsad_t_p + darsad_t_k) / 100, 0) * bime_asli + Math.Round(bime_takmili_60 * (darsad_t_60_p + darsad_t_60_k) / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * (darsad_t_70_p + darsad_t_70_k) / 100, 0) * t_takafol_70_t;
                            }
                            else
                            {
                                bimetakmili_p = Math.Round(bime_takmili * darsad_t_p / 100, 0) * (bime_asli - 1) + Math.Round(bime_takmili * darsad_t_60_p / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili * darsad_t_70_p / 100, 0) * t_takafol_70_t;
                                bimetakmili_k = Math.Round(bime_takmili * (darsad_t_p + darsad_t_k) / 100, 0) * 1 + Math.Round(bime_takmili * darsad_t_k / 100, 0) * (bime_asli - 1) + Math.Round(bime_takmili * darsad_t_60_k / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_k / 100, 0) * t_takafol_70_t;
                                //bimetakmili_p = Math.Round(bime_takmili * darsad_t_p / 100, 0) * (bime_asli - 1) + Math.Round(bime_takmili_60 * darsad_t_60_p / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_p / 100, 0) * t_takafol_70_t;
                                //bimetakmili_k = Math.Round(bime_takmili * (darsad_t_p + darsad_t_k) / 100, 0) * 1 + Math.Round(bime_takmili * darsad_t_k / 100, 0) * (bime_asli - 1) + Math.Round(bime_takmili_60 * darsad_t_60_k / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_k / 100, 0) * t_takafol_70_t;
                            }
                        }
                        else if (takmili == false)
                        {
                            bimetakmili_p = 0;
                            bimetakmili_k = 0;
                        }
                    }
                    /*}
          else
          {
          if (omr == true)
          {
          bimeomr_p = Math.Round(bime_omr * darsad_o_p / 100, 0);
          bimeomr_k = Math.Round(bime_omr * darsad_o_k / 100, 0);
          }
          else if (omr == false)
          {
          bimeomr_p = 0;
          bimeomr_k = 0;
          }
          if (takmili == true)
          {
          if (tedad_asli == 6)
          {
          bimetakmili_p = Math.Round(bime_takmili * darsad_t_p / 100, 0) * bime_asli + Math.Round(bime_takmili_60 * darsad_t_60_p / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_p / 100, 0) * t_takafol_70_t;
          bimetakmili_k = Math.Round(bime_takmili * darsad_t_k / 100, 0) * bime_asli + Math.Round(bime_takmili_60 * darsad_t_60_k / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_k / 100, 0) * t_takafol_70_t;
          }
          else
          {
          if (bime_asli > tedad_asli)
          {
          int mazad = bime_asli - tedad_asli;
          bimetakmili_p = (Math.Round(bime_takmili * darsad_t_p / 100, 0) * tedad_asli + Math.Round(bime_takmili * (darsad_t_p + darsad_t_k) / 100, 0) * mazad) + Math.Round(bime_takmili_60 * darsad_t_60_k / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_k / 100, 0) * t_takafol_70_t;
          bimetakmili_k = Math.Round(bime_takmili * darsad_t_k / 100, 0) * tedad_asli + Math.Round(bime_takmili_60 * darsad_t_60_k / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_k / 100, 0) * t_takafol_70_t;
          }
          else
          {
          bimetakmili_p = Math.Round(bime_takmili * darsad_t_p / 100, 0) * bime_asli + Math.Round(bime_takmili_60 * darsad_t_60_p / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_p / 100, 0) * t_takafol_70_t;
          bimetakmili_k = Math.Round(bime_takmili * darsad_t_k / 100, 0) * bime_asli + Math.Round(bime_takmili_60 * darsad_t_60_k / 100, 0) * t_takafol_60_t + Math.Round(bime_takmili_70 * darsad_t_70_k / 100, 0) * t_takafol_70_t;
          }
          }
          }
          else if (takmili == false)
          {
          bimetakmili_p = 0;
          bimetakmili_k = 0;
          }
          }*/
                    mohasebe.fldBimeOmr = Convert.ToInt32(bimeomr_p + bimeomr_k);
                    mohasebe.fldBimeOmrKarFarma = Convert.ToInt32(bimeomr_k);
                    mohasebe.fldBimeTakmily = Convert.ToInt32(bimetakmili_p + bimetakmili_k);
                    mohasebe.fldBimeTakmilyKarFarma = Convert.ToInt32(bimetakmili_k);
                    mohasebe.fldT_Asli = Convert.ToByte(bime_asli);
                    mohasebe.fldT_60 = Convert.ToByte(t_takafol_60_t);
                    mohasebe.fldT_70 = Convert.ToByte(t_takafol_70_t);
                }
                //--------------------مقرری ماه اول
                decimal mogharari = 0, mogharari_j = 0, mogharari_gh = 0;
                if (Mogharari == true && _Year == year && _Month == month)
                {
                    bool IsBarjastegi = false, IsTakhasosi = false, IsMade26 = false, IsModiriati = false;
                    //SqlConnection con_moteghayer_hokm = new SqlConnection(Program.connectiontext);
                    //SqlCommand Com.moteghayer_hokm = new SqlCommand("select * from moteghayer_kohm where estekhdam=1 and sal=" + hokm.a.t_ejra.Substring(0, 4), con_moteghayer_hokm);
                    //con_moteghayer_hokm.Open();
                    //SqlDataReader read_moteghayer_hokm = Com.moteghayer_hokm.ExecuteReader();
                    //if (read_moteghayer_hokm.Read())
                    //{
                    //    IsBarjastegi = Convert.ToBoolean(read_moteghayer_hokm["barjastegi"]);
                    //    IsTakhasosi = Convert.ToBoolean(read_moteghayer_hokm["takhasosi"]);
                    //    IsMade26 = Convert.ToBoolean(read_moteghayer_hokm["made26"]);
                    //    IsModiriati = Convert.ToBoolean(read_moteghayer_hokm["modiriati"]);
                    //}
                    //con_moteghayer_hokm.Close();
                    var h_paye = LastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault();/*h_paye*/
                    var fogh_shoghl = LastHokm.Items.Where(l => l.ItemHoghughiId == 6).FirstOrDefault();/*fogh_shoghl*/
                    var tatbigh = LastHokm.Items.Where(l => l.ItemHoghughiId == 11).FirstOrDefault();/*tatbigh*/
                    var sanavat = LastHokm.Items.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();/*sanavat*/
                    var sanavat_basij = LastHokm.Items.Where(l => l.ItemHoghughiId == 4).FirstOrDefault();/*sanavat_basij*/
                    var sanavat_isar = LastHokm.Items.Where(l => l.ItemHoghughiId == 5).FirstOrDefault();/*sanavat_isar*/
                    var barjastegi = LastHokm.Items.Where(l => l.ItemHoghughiId == 10).FirstOrDefault();/*barjastegi*/
                    var Takhasosi = LastHokm.Items.Where(l => l.ItemHoghughiId == 7).FirstOrDefault();/*Takhasosi*/
                    var Made26 = LastHokm.Items.Where(l => l.ItemHoghughiId == 8).FirstOrDefault();/*Made26*/
                    var Modiriati = LastHokm.Items.Where(l => l.ItemHoghughiId == 9).FirstOrDefault();/*Modiriati*/
                    if (h_paye != null)
                        mogharari_j = mogharari_j + h_paye.Mablagh;
                    if (fogh_shoghl != null)
                        mogharari_j = mogharari_j + fogh_shoghl.Mablagh;
                    if (tatbigh != null)
                        mogharari_j = mogharari_j + tatbigh.Mablagh;
                    if (sanavat != null)
                        mogharari_j = mogharari_j + sanavat.Mablagh;
                    if (sanavat_basij != null)
                        mogharari_j = mogharari_j + sanavat_basij.Mablagh;
                    if (sanavat_isar != null)
                        mogharari_j = mogharari_j + sanavat_isar.Mablagh;
                    if (barjastegi != null)
                        mogharari_j = mogharari_j + barjastegi.Mablagh;
                    if (Takhasosi != null)
                        mogharari_j = mogharari_j + Takhasosi.Mablagh;
                    if (Made26 != null)
                        mogharari_j = mogharari_j + Made26.Mablagh;
                    if (Modiriati != null)
                        mogharari_j = mogharari_j + Modiriati.Mablagh;
                    //if (IsBarjastegi)
                    //    mogharari_j += hog.barjastegi;
                    //if (IsTakhasosi)
                    //    mogharari_j += hokm.a.Takhasosi;
                    //if (IsMade26)
                    //    mogharari_j += hokm.a.Made26;
                    //if (IsModiriati)
                    //    mogharari_j += hokm.a.Modiriati;
                    commandText = "SELECT  top(1)  fldId, fldTarikhEjra, fldTarikhSodoor, fldAnvaeEstekhdamId, fldStatusTaaholId, fldTedadFarzand,fldTedadAfradTahteTakafol " +
                      "FROM  Prs.tblPersonalHokm " +
                      " WHERE substring(fldTarikhEjra,1,4)< " + year + "and fldPrs_PersonalInfoId=" + PrsPersonalId + " and  fldid<>" + LastHokm.HokmId +
                      " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
                    var tblPersonalHokm2 = service.GetData(commandText).Tables[0];
                    if (tblPersonalHokm2.Rows.Count > 0)
                    {
                        for (int i = 0; i < tblPersonalHokm2.Rows.Count; i++)
                        {
                            commandText = "SELECT fldItems_EstekhdamId,fldMablagh,fldItemsHoghughiId FROM Prs.tblHokm_Item INNER JOIN " +
                              "Com.tblItems_Estekhdam ON Prs.tblHokm_Item.fldItems_EstekhdamId = Com.tblItems_Estekhdam.fldId " +
                              "WHERE fldPersonalHokmId=" + Convert.ToInt32(tblPersonalHokm2.Rows[i]["fldId"]);
                            var Hokm_Item21 = service.GetData(commandText).Tables[0];
                            if (Hokm_Item21.Rows.Count > 0)
                            {
                                for (int j = 0; j < Hokm_Item21.Rows.Count; j++)
                                {
                                    if (Convert.ToInt32(Hokm_Item21.Rows[j]["fldItemsHoghughiId"]) == 1 ||
                                        Convert.ToInt32(Hokm_Item21.Rows[j]["fldItemsHoghughiId"]) == 2 ||
                                        Convert.ToInt32(Hokm_Item21.Rows[j]["fldItemsHoghughiId"]) == 6 ||
                                        Convert.ToInt32(Hokm_Item21.Rows[j]["fldItemsHoghughiId"]) == 11 ||
                                        Convert.ToInt32(Hokm_Item21.Rows[j]["fldItemsHoghughiId"]) == 4 ||
                                        Convert.ToInt32(Hokm_Item21.Rows[j]["fldItemsHoghughiId"]) == 5 ||
                                        Convert.ToInt32(Hokm_Item21.Rows[j]["fldItemsHoghughiId"]) == 7 ||
                                        Convert.ToInt32(Hokm_Item21.Rows[j]["fldItemsHoghughiId"]) == 8 ||
                                        Convert.ToInt32(Hokm_Item21.Rows[j]["fldItemsHoghughiId"]) == 9 ||
                                        Convert.ToInt32(Hokm_Item21.Rows[j]["fldItemsHoghughiId"]) == 10)
                                    {
                                        mogharari_gh += Convert.ToInt32(Hokm_Item21.Rows[j]["fldMablagh"]);
                                    }
                                    //if (IsBarjastegi)
                                    //    mogharari_gh += Convert.ToDecimal(read_hokm["barjastegi"]);
                                    //if (IsTakhasosi)
                                    //    mogharari_gh += Convert.ToDecimal(read_hokm["takhasosi"]);
                                    //if (IsMade26)
                                    //    mogharari_gh += Convert.ToDecimal(read_hokm["Made26"]);
                                    //if (IsModiriati)
                                    //    mogharari_gh += Convert.ToDecimal(read_hokm["Modiriati"]);
                                }
                            }
                        }
                    }
                    else
                        mogharari_gh = 0;
                    bool have_mogharari = true;
                    commandText = "SELECT    isnull(sum(fldMogharari),0) as fldMogharari " +
                      "FROM  Pay.tblMohasebat " +
                      " WHERE fldPersonalId=" + PayPersonalId + " and fldYear=" + year;
                    var mohasebe1 = service.GetData(commandText).Tables[0];
                    int old_mogharari = 0;
                    if (mohasebe1.Rows.Count > 0)
                    {
                        if (Convert.ToDecimal(mohasebe1.Rows[0]["fldMogharari"]) > 0)
                            //have_mogharari = false;
                            old_mogharari = Convert.ToInt32(mohasebe1.Rows[0]["fldMogharari"]);
                    }
                    //if (hog.kol_motalebe > 0)
                    //{
                    if (have_mogharari == true && (TypeEstekhdam > 1) && TypeBimeId == 2)// && mah > 1)
                    {
                        if (mogharari_j > mogharari_gh && mogharari_gh > 0)
                            mogharari = mogharari_j - mogharari_gh;// - old_mogharari;
                        else
                            mogharari = 0;
                    }
                    else
                        mogharari = 0;
                    //}
                    //else
                    //    mogharari = 0;
                    mohasebe.fldMogharari = (int)mogharari;
                }
                else if (Moavaghe == true)
                {
                    commandText = "SELECT    isnull(sum(fldMogharari),0) as fldMogharari " +
                      "FROM  Pay.tblMohasebat " +
                      " WHERE fldPersonalId=" + PayPersonalId + " and fldYear=" + year + " and fldmonth=" + month;
                    var mohasebe2 = service.GetData(commandText).Tables[0];
                    if (mohasebe2.Rows.Count > 0)
                    {
                        if (Convert.ToDecimal(mohasebe2.Rows[0]["fldMogharari"]) > 0)
                            //have_mogharari = false;
                            mogharari = Convert.ToInt32(mohasebe2.Rows[0]["fldMogharari"]);
                        else mogharari = 0;
                    }
                }
                //--------------------مشمولی بیمه
                decimal mashmol_bime = mohasebe.fldMashmolBime; /*=  hog.motalebat_m +hog.m_bime*/
                foreach (var item in Moteghayer.Items)
                {
                    var Mohasebe_Item = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.fldItemHoghughiId).FirstOrDefault();
                    if (Mohasebe_Item != null)
                        mashmol_bime += Mohasebe_Item.fldMablagh;
                }
                //--------------------حق بیمه
                if (mashmol_bime < mogharari)
                {
                    mogharari = 0;
                    mohasebe.fldMogharari = (int)mogharari;//اگر مشمول بیمه کمتر از مقرری باشه مقرری کسر نخواد شد.
                }
                mashmol_bime = Math.Round(mashmol_bime);
                b_p = Convert.ToDecimal(Moteghayer.fldDarsadBimePersonal);
                b_k = Convert.ToDecimal(Moteghayer.fldDarsadbimeKarfarma);
                b_b = Convert.ToDecimal(Moteghayer.fldDarsadBimeBikari);
                decimal b_k_j = Convert.ToDecimal(Moteghayer.fldDarsadBimeJanbazan);
                //decimal saghfebime = Convert.ToDecimal(Moteghayer.),
                decimal bimepersenel = 0, bimekarfarma = 0, bimebikari = 0;
                int mashmol_bimeNakhales = 0;
                //decimal saghfebime_max = Convert.ToDecimal(read_moteghayer["saghfebime_max"]);
                if (mashmol_bime > MaxBime && TypeBimeId == 1)
                {
                    mashmol_bimeNakhales = (int)mashmol_bime;
                    mashmol_bime = MaxBime;
                }
                if (sal == 0 && TypeBimeId == 1)
                    SumBime = (int)mashmol_bime;
                if (Moavaghe)
                {
                    commandText = "SELECT    fldPersonalId,fldYear,fldMonth, Pay.tblMohasebat_PersonalInfo.fldMazad30Sal " +
                      "FROM         Pay.tblMohasebat INNER JOIN " +
                      "Pay.tblMohasebat_PersonalInfo ON Pay.tblMohasebat.fldId = Pay.tblMohasebat_PersonalInfo.fldMohasebatId " +
                      "WHERE fldPersonalId=" + PayPersonalId + " AND fldYear=" + year + " AND fldMonth=" + month;
                    var mohasebe2 = service.GetData(commandText).Tables[0];
                    int old_mogharari = 0;
                    if (mohasebe2.Rows.Count > 0)
                    {
                        PersonalInfo.fldMazad30Sal = Convert.ToBoolean(mohasebe2.Rows[0]["fldMazad30Sal"]);
                    }
                }
                if ((PersonalInfo.fldEsargariId == 1 || PersonalInfo.fldEsargariId == 4) && PersonalInfo.fldMazad30Sal == false)
                {
                    if (TypeBimeId == 2)
                    {
                        //if ((mashmol_bime - mogharari) > saghfebime && (mashmol_bime - mogharari) < saghfebime_max)
                        //{
                        bimepersenel = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_p)) / 100);
                        bimekarfarma = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_k)) / 100);
                        bimebikari = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_b)) / 100);
                        //}
                    }
                    else
                    {
                        //if ((mashmol_bime - mogharari) > saghfebime && (mashmol_bime - mogharari) < saghfebime_max)
                        //{
                        bimepersenel = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_p)) / 100);
                        bimekarfarma = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_k)) / 100);
                        bimebikari = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_b)) / 100);
                        //}
                    }
                }
                else if (PersonalInfo.fldMazad30Sal == true && (PersonalInfo.fldEsargariId == 1 || PersonalInfo.fldEsargariId == 4))
                {
                    if (TypeBimeId == 2)
                    {
                        //if ((mashmol_bime - mogharari) > saghfebime && (mashmol_bime - mogharari) < saghfebime_max)
                        //{
                        bimepersenel = 0;
                        bimekarfarma = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_k)) / 100);
                        bimebikari = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_b)) / 100);
                        //}
                    }
                    else
                    {
                        bimepersenel = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_p)) / 100);
                        bimekarfarma = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_k)) / 100);
                        bimebikari = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_b)) / 100);
                    }
                }
                else if (PersonalInfo.fldEsargariId == 2 || PersonalInfo.fldEsargariId == 3 || PersonalInfo.fldEsargariId == 5)
                {
                    if (PersonalInfo.fldMazad30Sal == false)
                    {
                        if (TypeBimeId == 1)
                        {
                            //if ((mashmol_bime - mogharari) > saghfebime && (mashmol_bime - mogharari) < saghfebime_max)
                            //{
                            bimepersenel = 0;
                            bimekarfarma = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_k_j)) / 100);
                            bimebikari = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_b)) / 100);
                            //}
                        }
                        else
                        {
                            bimepersenel = 0;
                            bimekarfarma = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_k_j)) / 100);
                            bimebikari = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_b)) / 100);
                        }
                    }
                    else
                    {
                        //if ((mashmol_bime - mogharari) > saghfebime && (mashmol_bime - mogharari) < saghfebime_max)
                        //{
                        bimepersenel = 0;
                        bimekarfarma = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_k)) / 100);
                        bimebikari = Math.Round(((mashmol_bime - mogharari) * Convert.ToDecimal(b_b)) / 100);
                        //}
                    }
                }
                //hog.bime_karfarma = bimekarfarma + bimebikari;
                //hog.bime_persenel = bimepersenel;
                //hog.bime_bikari = bimebikari;
                //hog.mashmol_bime = mashmol_bime;
                mohasebe.fldBimeKarFarma = Convert.ToInt32(bimekarfarma);
                mohasebe.fldBimePersonal = Convert.ToInt32(bimepersenel);
                mohasebe.fldBimeBikari = Convert.ToInt32(bimebikari);
                mohasebe.fldMashmolBime = Convert.ToInt32(mashmol_bime);
                mohasebe.fldMashmolBimeNaKhales = Convert.ToInt32(mashmol_bimeNakhales);
                if (PersonalInfo.fldEsargariId == 1 || PersonalInfo.fldEsargariId == 4)
                    mohasebe.fldDarsadBimeKarFarma = Convert.ToDecimal(b_k);
                else
                    mohasebe.fldDarsadBimeKarFarma = Convert.ToDecimal(b_k_j);
                mohasebe.fldDarsadBimePersonal = (decimal)b_p;
                mohasebe.fldDarsadBimeBikari = (decimal)b_b;
                //--------------------حق بیمه بیشتر از 3 فرزند و افراد تحت تکفل
                decimal h_d_mazad = Convert.ToDecimal(Moteghayer.fldHaghDarmanMazad);
                decimal h_d_tahtetaklof = Convert.ToDecimal(Moteghayer.fldHaghDarmanTahteTakaffol);
                decimal h_bime_mazad = 0, h_bime_tahtetakalof = 0;
                int tedad_t_takalof = 0;
                int ezafe_farzand = 0;
                if (PersonalInfo.fldHamsarKarmand == true)
                    tedad_afradekhanevade = tedad_afradekhanevade - 1;
                //if (mohasebe.kol_motalebe > 0)
                //{
                tedad_t_takalof = LastHokm.TedadAfradTakafol;//t_takafol_60 + t_takafol_70;
                if (TypeBimeId == 2)
                {
                    if (LastHokm.TedadFarzand > 3 && PersonalInfo.fldJensiyat == 1)
                    {
                        if (Convert.ToInt32(year.ToString() + month.ToString().PadLeft(2, '0')) >= 139209)
                        {
                            h_bime_mazad = 0;
                        }
                        else if (year > 1392)
                            h_bime_mazad = 0;
                        else
                        {
                            ezafe_farzand = LastHokm.TedadFarzand - 3;
                            h_bime_mazad = h_d_mazad * Convert.ToDecimal(ezafe_farzand);
                        }
                    }
                    else if (PersonalInfo.fldJensiyat == 2)//------تغییرات
                    {//------تغییرات
                        ezafe_farzand = tedad_afradekhanevade - 1;//------تغییرات
                        if (ezafe_farzand > 0)
                            h_bime_mazad = h_d_mazad * Convert.ToDecimal(ezafe_farzand);//------تغییرات
                    }//------تغییرات
                    h_bime_tahtetakalof = h_d_tahtetaklof * Convert.ToDecimal(tedad_t_takalof);
                }
                else
                    h_bime_mazad = 0;
                /* commandText = "SELECT    count(*)as tedad " +
        "FROM         spr_Prs_tblAfradTahtePoosheshSelect " +
        "WHERE fldPersonalId=" + PayPersonalId + " AND fldNesbat=4";*/
                commandText = "SELECT    count(*)as tedad " +
                  "FROM         Prs.tblAfradTahtePooshesh " +
                  "WHERE fldPersonalId=" + PrsPersonalId + " AND fldNesbat=4";
                var tahteposhesh = service.GetData(commandText).Tables[0];
                if (tahteposhesh.Rows.Count > 0)
                {
                    h_bime_mazad += h_d_mazad * Convert.ToDecimal(tahteposhesh.Rows[0]["tedad"]);
                    ezafe_farzand += Convert.ToInt32(tahteposhesh.Rows[0]["tedad"]);
                }
                //}
                //else
                //{
                //    h_bime_mazad = 0;
                //    h_bime_tahtetakalof = 0;
                //}
                //hog.h_bime_mazad = h_bime_mazad;
                //hog.h_bime_tahtetakalof = h_bime_tahtetakalof;
                //--------------------حق بیمه تا 3 فرزند 
                decimal bime3farzand_p = Convert.ToDecimal(Moteghayer.fldHaghDarmanKarmand);
                //bime3farzand_k = Convert.ToDecimal(v_moteghayer["darmanmazad"]) - bime3farzand_p;
                decimal bime3farzand_k = Convert.ToDecimal(Moteghayer.fldHaghDarmanKarfarma);
                decimal bime3farzand_d = Convert.ToDecimal(Moteghayer.fldHaghDarmanDolat);
                decimal h_bime3farzand_p = 0, h_bime3farzand_k = 0, h_bime3farzand_d = 0;
                decimal saghfDarman = Convert.ToDecimal(Moteghayer.fldMaxHaghDarman);
                decimal saghf = mashmol_bime;
                if (saghf > saghfDarman)
                    saghf = saghfDarman;//14248500;//12178200;//9742500;//7794000;//6606000
                //if (hog.kol_motalebe > 0)
                //{
                if (PersonalInfo.fldEsargariId == 1 || PersonalInfo.fldEsargariId == 4)
                {
                    if (TypeBimeId == 2 && PersonalInfo.fldMoafDarman == false && PersonalInfo.fldHamsarKarmand == false)
                    {
                        //h_bime3farzand_p = Convert.ToDecimal(tedad_afradekhanevade - ezafe_farzand) * bime3farzand_p;
                        //h_bime3farzand_k = Convert.ToDecimal(tedad_afradekhanevade - ezafe_farzand) * bime3farzand_k;
                        h_bime3farzand_p = Math.Round((Convert.ToDecimal(saghf) * bime3farzand_p) / 100);
                        h_bime3farzand_k = Math.Round((Convert.ToDecimal(saghf) * bime3farzand_k) / 100);
                        h_bime3farzand_d = Math.Round((Convert.ToDecimal(saghf) * bime3farzand_d) / 100);
                    }
                    else if (TypeBimeId == 2 && PersonalInfo.fldMoafDarman == true && PersonalInfo.fldHamsarKarmand == false)
                    {
                        h_bime3farzand_p = 0;
                        h_bime3farzand_k = 0;
                        h_bime3farzand_d = 0;
                    }
                    else if (TypeBimeId == 2 && PersonalInfo.fldMoafDarman == false && PersonalInfo.fldHamsarKarmand == true)
                    {
                        h_bime3farzand_p = 0;
                        h_bime3farzand_k = Math.Round((Convert.ToDecimal(saghf) * bime3farzand_k) / 100);
                        h_bime3farzand_d = Math.Round((Convert.ToDecimal(saghf) * bime3farzand_d) / 100);
                    }
                }
                else
                {
                    if (TypeBimeId == 2 && PersonalInfo.fldMoafDarman == false)
                    {
                        h_bime3farzand_p = 0;
                        //h_bime3farzand_k = Math.Round((Convert.ToDecimal(tedad_afradekhanevade - ezafe_farzand) * bime3farzand_p) + (Convert.ToDecimal(tedad_afradekhanevade - ezafe_farzand) * bime3farzand_k), 0);
                        h_bime3farzand_k = Math.Round(((Convert.ToDecimal(saghf) * bime3farzand_p) / 100) + ((Convert.ToDecimal(saghf) * bime3farzand_k) / 100), 0);
                        h_bime3farzand_d = Math.Round((Convert.ToDecimal(saghf) * bime3farzand_d) / 100);
                    }
                    else if (TypeBimeId == 2 && PersonalInfo.fldMoafDarman == true)
                    {
                        h_bime3farzand_p = 0;
                        h_bime3farzand_k = 0;
                    }
                }
                //}
                mohasebe.fldHaghDarman = Convert.ToInt32(h_bime3farzand_p + h_bime3farzand_k + h_bime3farzand_d + h_bime_mazad + h_bime_tahtetakalof);
                mohasebe.fldHaghDarmanKarfFarma = Convert.ToInt32(h_bime3farzand_k);
                mohasebe.fldHaghDarmanDolat = Convert.ToInt32(h_bime3farzand_d);
                mohasebe.fldTedadBime1 = tedad_afradekhanevade;
                mohasebe.fldTedadBime2 = ezafe_farzand;
                mohasebe.fldTedadBime3 = tedad_t_takalof;
                //--------------------پس انداز
                decimal pasandaz = 0;
                decimal zarib = Convert.ToDecimal(Moteghayer.fldZaribHoghoghiSal);
                if (PersonalInfo.fldPasAndaz)
                {
                    //if (hog.kol_motalebe > 0)
                    //{
                    if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                    {
                        //pasandaz = Math.Round((400 * Convert.ToDecimal(zarib * 35)) / 100, 0);
                        pasandaz = Math.Round((150 * zarib), 0);
                    }
                    else if (TypeEstekhdam == 4)
                    {
                        pasandaz = Math.Round((Convert.ToDecimal(LastHokm.Items.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh) * 5) / 100, 0);
                    }
                    else
                        pasandaz = 0;
                    //}
                }
                mohasebe.fldPasAndaz = (int)pasandaz * 2;
                //SqlConnection con_mahane = new SqlConnection(Program.connectiontext);
                //SqlCommand com_mahane = new SqlCommand("select * from mahane where code=" + code.ToString()
                //    + " and sal=" + sal.ToString() + " and mah=" + mah.ToString(), con_mahane);
                //con_mahane.Open();
                //SqlDataReader read_mahane = com_mahane.ExecuteReader();
                //decimal jarime = 0, mosaede = 0, jam_kasri = 0;
                //decimal kasri1 = 0, kasri2 = 0, kasri3 = 0, kasri4 = 0;
                decimal ghest = 0;
                bool p_gh = false;
                int? id_vam = 0;
                //if (read_mahane.Read())
                //{
                //--------------------جریمه
                //if (hog.kol_motalebe > 0)
                //{
                //    jarime = Convert.ToDecimal(read_mahane["jarime"]);
                //}
                //else
                //{
                //    jarime = 0;
                //}
                //--------------------سایر کسورات                        
                //kasri1 = Convert.ToDecimal(read_mahane["kasri1"]);
                //kasri2 = Convert.ToDecimal(read_mahane["kasri2"]);
                //kasri3 = Convert.ToDecimal(read_mahane["kasri3"]);
                //kasri4 = Convert.ToDecimal(read_mahane["kasri4"]);
                //if (hog.kol_motalebe > 0)
                //    jam_kasri = kasri1 + kasri2 + kasri3 + kasri4;
                //else
                //    jam_kasri = 0;
                //hog.jarime = jarime;
                //hog.mosaede = mosaede;
                //hog.kasri1 = kasri1;
                //hog.kasri2 = kasri2;
                //hog.kasri3 = kasri3;
                //hog.kasri4 = kasri4;
                //hog.jam_kasri = jam_kasri;
                //--------------------قسط   
                commandText = "SELECT *,(SELECT COUNT(*) FROM Pay.tblMohasebat_PersonalInfo WHERE fldVamId=Pay.tblVam.fldId) AS countPardakhti " +
                  "FROM Pay.tblVam WHERE fldStartDate<='" + year + "/" + month + "/30' AND fldStatus=1 AND fldPersonalId=" + PayPersonalId;
                var tblvam = service.GetData(commandText).Tables[0];
                if (tblvam.Rows.Count > 0)
                {
                    if (Convert.ToInt32(tblvam.Rows[0]["countPardakhti"]) < Convert.ToInt32(tblvam.Rows[0]["fldCount"]))
                    {
                        id_vam = Convert.ToInt32(tblvam.Rows[0]["fldId"]);
                        ghest = Convert.ToDecimal(tblvam.Rows[0]["fldMablagh"]);
                    }
                    else
                    {
                        id_vam = null;
                        ghest = 0;
                    }
                }
                else
                {
                    id_vam = null;
                    ghest = 0;
                }
                mohasebe.fldGhestVam = Convert.ToInt32(ghest);
                mohasebe.fldVamId = id_vam;
                //}
                //--------------------مساعده   
                //int index = 0;
                //SqlConnection con_mosaede = new SqlConnection(Program.connectiontext),
                //    con_p_mosaede = new SqlConnection(Program.connectiontext),
                //    con_p_mosaede1 = new SqlConnection(Program.connectiontext);
                //SqlCommand com_mosaede = new SqlCommand("select * from mosaede where code=" + code.ToString() + " and tasvie=0", con_mosaede);
                //con_mosaede.Open();
                //SqlDataReader read_mosaede = com_mosaede.ExecuteReader();
                //bool p_mo = false;
                //SqlCommand com_p_mosaede = new SqlCommand("select * from p_mosaede where code=" + code.ToString() + " and sal=" +
                //    sal.ToString() + " and mah=" + mah.ToString(), con_p_mosaede);
                //con_p_mosaede.Open();
                //SqlDataReader read_p_mosaede = com_p_mosaede.ExecuteReader();
                //if (read_mosaede.Read())
                //{
                //    index = Convert.ToInt32(read_mosaede["index"]);
                //    SqlCommand com_p_mosaede1 = new SqlCommand("select * from p_mosaede where code=" + code.ToString() + " and sal=" +
                //    sal.ToString() + " and mah=" + mah.ToString() + " and [index]=" + read_mosaede["index"].ToString(), con_p_mosaede1);
                //    con_p_mosaede1.Open();
                //    SqlDataReader read_p_mosaede1 = com_p_mosaede1.ExecuteReader();
                //    if (read_p_mosaede1.Read())
                //    {
                //        p_mo = true;
                //    }
                //    string t_sh = read_mosaede["t_shoro"].ToString();
                //    string today = Program.tarikh;
                //    PersianCalendar _pc1 = new PersianCalendar();
                //    PersianCalendar _pc2 = new PersianCalendar();
                //    string sal1 = "", sal11 = "", mah1 = "", mah11 = "", _r1 = "", r11 = "";
                //    sal1 = t_sh.Substring(0, 4);
                //    mah1 = t_sh.Substring(5, 2);
                //    _r1 = t_sh.Substring(8, 2);
                //    sal11 = today.Substring(0, 4);
                //    mah11 = today.Substring(5, 2);
                //    r11 = today.Substring(8, 2);
                commandText = "SELECT * FROM Pay.tblKarKardeMahane " +
                  "WHERE fldPersonalId=" + PayPersonalId + " AND fldYear=" + year + " AND fldMah=" + month + " AND fldNobatePardakht=" + NobatPardakht;
                int mosaede = 0;
                var tblMahane = service.GetData(commandText).Tables[0];
                if (tblMahane.Rows.Count > 0)
                {
                    mosaede = Convert.ToInt32(tblMahane.Rows[0]["fldMosaedeh"]);
                }
                else
                    mosaede = 0;
                mohasebe.fldMosaede = mosaede;
                //}
                //else if (read_p_mosaede.Read())
                //{
                //    mosaede = Convert.ToDecimal(read_p_mosaede["mablagh"]);
                //}
                //con_p_mosaede.Close();
                //con_p_mosaede1.Close();
                //con_mosaede.Close();
                //-------------------مشمولی مالیات                                                
                if (Fiscal.Items_Title.Count != 0)
                {
                    decimal mashmol_maliyat = mohasebe.fldMashmolMaliyat; /*hog.kol_motalebe - hog.motalebat_m + hog.m_maliat*/;
                    foreach (var item in Fiscal.Items_Title)
                    {
                        var Mohasebe_Item = mohasebe.Items.Where(l => l.fldItemHoghoghiId == item.fldItemHoghughiId).FirstOrDefault();
                        if (Mohasebe_Item != null)
                            mashmol_maliyat += Mohasebe_Item.fldMablagh;
                    }
                    //--------------------مالیات  
                    //mashmol_maliyat = Math.Round(mashmol_maliyat);
                    int sm = Convert.ToInt32(year.ToString() + month.ToString().PadLeft(2, '0'));
                    if (sm >= 139901)
                    {
                        if (TypeBimeId == 2)
                            mashmol_maliyat = Math.Round(mashmol_maliyat) - (bimeomr_p + bimetakmili_p + h_bime3farzand_p + h_bime3farzand_k + h_bime3farzand_d + h_bime_mazad + h_bime_tahtetakalof);//طبق بخش نامه اداره امور مالیاتی حق بیمه سهم کارمند از مشمول کسر گردید
                        else
                            mashmol_maliyat = Math.Round(mashmol_maliyat) - (bimeomr_p + bimetakmili_p + bimepersenel);//طبق بخش نامه اداره امور مالیاتی حق بیمه سهم کارمند از مشمول کسر گردید
                    }
                    else
                        mashmol_maliyat = Math.Round(mashmol_maliyat);
                    decimal darsad_maliyat = 0, d_karmand = 0;
                    decimal zarib_maliyat = 0, az_mablagh_k = 0, az_mablagh = 0;
                    int CountChild = 0, CountChildAll = 0; decimal Percent = 0;
                    CountChildAll = LastHokm.TedadFarzand;
                    if (CountChildAll >= 3)
                    {
                        commandText = "select COUNT(*) as fldCount from prs.tblAfradTahtePooshesh " +
                          "where fldPersonalId=" + PrsPersonalId + " and fldBirthDate>='1400/08/01' and fldNesbat=1 ";
                        var c = service.GetData(commandText).Tables[0];
                        if (c.Rows.Count > 0)
                        {
                            CountChild = Convert.ToInt32(c.Rows[0]["fldCount"]);
                        }
                        Percent = CountChild * Convert.ToDecimal(15 / 100.0);
                    }
                    foreach (var item in Fiscal.Items_Detail)
                    {
                        if (mashmol_maliyat <= Convert.ToDecimal(item.fldAmountTo + item.fldAmountTo * Percent) && mashmol_maliyat >= Convert.ToDecimal(item.fldAmountFrom + item.fldAmountFrom * Percent))
                        {
                            darsad_maliyat = Convert.ToDecimal(item.fldPercentTaxOnWorkers);
                            d_karmand = Convert.ToDecimal(item.fldTaxationOfEmployees);
                            zarib_maliyat = Convert.ToDecimal(item.fldTax);
                            az_mablagh = Convert.ToDecimal(item.fldAmountFrom + item.fldAmountFrom * Percent);
                        }
                    }
                    if (az_mablagh != 0)
                    {
                        foreach (var item in Fiscal.Items_Detail)
                        {
                            if (Convert.ToDecimal(item.fldAmountFrom + item.fldAmountFrom * Percent) < az_mablagh)
                            {
                                az_mablagh_k = Convert.ToDecimal(item.fldAmountTo + item.fldAmountTo * Percent);
                                /*if (TypeEstekhdam > 1)
                {
                zarib_maliyat = 0;
                break;
                }*/
                            }
                        }
                    }
                    if (TypeEstekhdam > 1)
                        darsad_maliyat = d_karmand;
                    decimal maliyat = 0;
                    if (PersonalInfo.fldEsargariId == 1)
                    {
                        maliyat = Math.Round(((mashmol_maliyat - az_mablagh_k) * Convert.ToDecimal(darsad_maliyat)) / 100, 0) + zarib_maliyat;
                    }
                    else
                        maliyat = 0;
                    mohasebe.fldMashmolMaliyat = (int)mashmol_maliyat;
                    mohasebe.fldMaliyat = (int)maliyat;
                }
                else
                {
                    mohasebe.err = true;
                    mohasebe.errMsg = "لطفا عناوین مالیاتی را تعریف کنید";
                    return;
                }
                //--------------------کسورات تفکیکی
                string jari = year.ToString();
                decimal kosurat = 0;
                int komaki = month;
                if (komaki < 10)
                    jari = jari + "0" + komaki;
                else
                    jari = jari + komaki;
                int tedad = 0;
                //if (Moavaghe == false)
                //{
                //if (hog.kol_motalebe > 0)
                //{
                commandText = "SELECT * FROM Pay.tblKosorateParametri_Personal " +
                  "WHERE fldPersonalId=" + PayPersonalId + " AND " + jari + "<=fldDateDeactive";
                var tblkosurat = service.GetData(commandText).Tables[0];
                if (tblkosurat.Rows.Count > 0)
                {
                    for (int i = 0; i < tblkosurat.Rows.Count; i++)
                    {
                        tedad = 0;
                        int t_daryaft = Convert.ToInt32(tblkosurat.Rows[i]["fldTarikhePardakht"].ToString().Substring(0, 4) + tblkosurat.Rows[i]["fldTarikhePardakht"].ToString().Substring(5, 2));
                        if (t_daryaft <= Convert.ToInt32(jari))
                        {
                            commandText = "SELECT        count(*)as Count " +
                              "FROM            Pay.[tblMohasebat_kosorat/MotalebatParam] " +
                              "where fldKosoratId=" + Convert.ToInt32(tblkosurat.Rows[i]["fldId"]);
                            var tblmkosurat = service.GetData(commandText).Tables[0];
                            if (tblmkosurat.Rows.Count != 0)
                            {
                                tedad = Convert.ToInt32(tblmkosurat.Rows[0]["Count"]);
                            }
                            bool tavigh = false;
                            commandText = "SELECT fldId, fldKosuratId, fldYear, fldMonth, fldUserId, fldDesc, fldDate FROM Pay.tblTavighKosurat " +
                              "where fldKosuratId=" + Convert.ToInt32(tblkosurat.Rows[i]["fldId"]) + " and fldYear=" + year + " and fldMonth=" + month;
                            var tblTavigh = service.GetData(commandText).Tables[0];
                            if (tblTavigh.Rows.Count != 0)
                            {
                                tavigh = true;
                            }
                            if (tavigh == false)
                            {
                                if (Moavaghe == false)
                                {
                                    if (Convert.ToInt32(tblkosurat.Rows[i]["fldNoePardakht"]) == 1)
                                    {
                                        kosurat = kosurat + Convert.ToDecimal(tblkosurat.Rows[i]["fldMablagh"]);
                                        mohasebe.Kosorat_MotalebatParam.Add(new Mohasebat_Kosorat_MotalebatParam
                                        {
                                            fldKosoratId = Convert.ToInt32(tblkosurat.Rows[i]["fldId"]),
                                            fldMablagh = Convert.ToInt32(tblkosurat.Rows[i]["fldMablagh"])//,fldMotalebatId=null
                                        });
                                    }
                                    else
                                    {
                                        if (tedad < Convert.ToInt32(tblkosurat.Rows[i]["fldTedad"]))
                                        {
                                            kosurat = kosurat + Convert.ToDecimal(tblkosurat.Rows[i]["fldMablagh"]);
                                            mohasebe.Kosorat_MotalebatParam.Add(new Mohasebat_Kosorat_MotalebatParam
                                            {
                                                fldKosoratId = Convert.ToInt32(tblkosurat.Rows[i]["fldId"]),
                                                fldMablagh = Convert.ToInt32(tblkosurat.Rows[i]["fldMablagh"])//,fldMotalebatId=null
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    kosurat = kosurat + Convert.ToDecimal(tblkosurat.Rows[i]["fldMablagh"]);
                                    mohasebe.Kosorat_MotalebatParam.Add(new Mohasebat_Kosorat_MotalebatParam
                                    {
                                        fldKosoratId = Convert.ToInt32(tblkosurat.Rows[i]["fldId"]),
                                        fldMablagh = Convert.ToInt32(tblkosurat.Rows[i]["fldMablagh"])//,fldMotalebatId=null
                                    });
                                }
                            }
                        }
                    }
                }
                //}
                //--------------------کسورات بانک
                string jari1 = year.ToString();
                decimal kosurat_bank = 0;
                int komaki1 = month;
                if (komaki1 < 10)
                    jari1 = jari1 + "0" + komaki1;
                else
                    jari1 = jari1 + komaki1;
                int tedad1 = 0;
                //if (Moavaghe == false)
                //{
                //if (hog.kol_motalebe > 0)
                //{
                commandText = "SELECT * FROM Pay.tblKosuratBank " +
                  "WHERE fldPersonalId=" + PayPersonalId + " AND " + jari + "<=fldDeactiveDate";
                var tblkosurat1 = service.GetData(commandText).Tables[0];
                if (tblkosurat1.Rows.Count > 0)
                {
                    for (int i = 0; i < tblkosurat1.Rows.Count; i++)
                    {
                        tedad1 = 0;
                        int t_daryaft = Convert.ToInt32(tblkosurat1.Rows[i]["fldTarikhPardakht"].ToString().Substring(0, 4) + tblkosurat1.Rows[i]["fldTarikhPardakht"].ToString().Substring(5, 2));
                        if (t_daryaft <= Convert.ToInt32(jari1))
                        {
                            commandText = "SELECT        count(*)as Count " +
                              "FROM            Pay.tblMohasebat_KosoratBank " +
                              "where fldKosoratBankId=" + Convert.ToInt32(tblkosurat1.Rows[i]["fldId"]);
                            var tblmkosurat = service.GetData(commandText).Tables[0];
                            if (tblmkosurat.Rows.Count != 0)
                            {
                                tedad1 = Convert.ToInt32(tblmkosurat.Rows[0]["Count"]);
                            }
                            if (Moavaghe == false)
                            {
                                if (tedad1 < Convert.ToInt32(tblkosurat1.Rows[i]["fldCount"]))
                                {
                                    kosurat_bank = kosurat_bank + Convert.ToDecimal(tblkosurat1.Rows[i]["fldMablagh"]);
                                    mohasebe.KosoratBank.Add(new Mohasebat_KosoratBank
                                    {
                                        fldKosoratBankId = Convert.ToInt32(tblkosurat1.Rows[i]["fldId"]),
                                        fldMablagh = Convert.ToInt32(tblkosurat1.Rows[i]["fldMablagh"])
                                    });
                                }
                            }
                            else
                            {
                                kosurat_bank = kosurat_bank + Convert.ToDecimal(tblkosurat1.Rows[i]["fldMablagh"]);
                                mohasebe.KosoratBank.Add(new Mohasebat_KosoratBank
                                {
                                    fldKosoratBankId = Convert.ToInt32(tblkosurat1.Rows[i]["fldId"]),
                                    fldMablagh = Convert.ToInt32(tblkosurat1.Rows[i]["fldMablagh"])
                                });
                            }
                        }
                    }
                }
                //}
                ////--------------------جمع کل کسورات
                //if (hog.kol_motalebe < ghest)
                //    ghest = 0;
                ////if (hog.kol_motalebe > 0)
                //hog.kol_kasri = hog.kusorat_m + hog.kosor_bank_m + hog.jarime + hog.mosaede
                //    + hog.jam_kasri + hog.bime_persenel + hog.maliyat + hog.h_bime_mazad
                //    + hog.h_bime_tahtetakalof + hog.h_bime3farzand_p + hog.pasandaz * 2 + hog.h_bime3farzand_k
                //    + hog.bimetakmili_k + hog.bimetakmili_p + hog.bimeomr_k + hog.bimeomr_p + hog.mogharari;
                ////else
                ////  hog.kol_kasri = 0;
                ////--------------------جمع کل مطالبات                        
                //hog.kol_motalebe = hog.kol_motalebe + hog.h_bime3farzand_k + hog.pasandaz + hog.bimeomr_k + hog.bimetakmili_k + hog.jam_motalebe;
                //if (id_vam != 0)
                //{
                //    if (ghest > 0 && p_gh == false)
                //    {
                //        ghest gh = new ghest();
                //        gh.id_vam = id_vam;
                //        gh.m_ghest = ghest;
                //        p_ghest.Add(gh);
                //        hog.ghest = ghest;
                //        hog.kol_kasri = hog.kol_kasri + ghest;
                //    }
                //    else
                //    {
                //        hog.ghest = ghest;
                //        hog.kol_kasri = hog.kol_kasri + ghest;
                //    }
                //}
                //if (mosaede > 0)
                //{
                //    hog.mosaede = mosaede;
                //    hog.kol_kasri = hog.kol_kasri + mosaede;
                //}
                //if (hog.kol_kasri > hog.kol_motalebe && flag == false)
                //{
                //    FMessageBox.Show("جمع مبلغ کسورات " + p.name + " " + p.family + " فرزند " + p.pedar +
                //        " بیشتر از جمع مبلغ مطالبات است\n" + "(جمع حقوق = " + ((double)-(hog.kol_motalebe - hog.kol_kasri)).ToString("#,###") + "-)" + " لطفا اطلاعات مورد نیاز را کنترل کنید", "خطا در محاسبه حقوق",
                //        FMessageBoxButtons.OK, FMessageBoxIcons.Error);
                //}
            }
            catch (Exception x)
            {
                DateTime d1;
                d1 = DateTime.Now;
                string excep = "";
                if (x.InnerException != null)
                    excep = "'kosorat_hog " + x.InnerException.Message + "'";
                else
                    excep = "'kosorat_hog " + x.Message + "'";
                string commandText1 = "exec [Com].[spr_tblErrorInsert] 	0,'admin'," + excep + ",'" + d1.ToString() + "',1,1,''";
                var _Hokm1 = service.GetData(commandText1).Tables[0];
            }
        }
    }
}