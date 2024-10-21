using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewFMS.Models;
using NewFMS.Models.Mohasebat;


namespace NewFMS.Controllers.TestFormula
{
    public class FormulHokmController : Controller
    {
        //
        // GET: /FormulHokm/
         double Zarib2 = 0.0,z_nobat=0;
    //double zaribIsar=0.0;
        System.Data.DataSet dtFormul = new System.Data.DataSet();
        WCF_Common_Pay.Comon_PayService servic_Com_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_Com_Pay = new WCF_Common_Pay.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        WCF_PayRoll.PayRolService servic_Pay = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError Err_Pay = new WCF_PayRoll.ClsError();
        int Mashmaul_Kosurat = 0;
        public List<ClsHokmItems> Formulator(int PersonalId, int ItemHoghoghi, int TypeEstekhdam, double Zarib,
            int TaaholStatus, string Tarikh_Ejra, int Group, int TopGroup, byte TedadFarzandan
            , int Gender, int? TypeBimeId, int AnvaEstekhdam, List<ClsHokmItems> HokmItem,
            int ZaribHoghAval, int Tatbigh, bool HasZarib, double ZaribTadil, int mashmol)
        {
            //try
            //{
                var PersonalInfo = servic_Pay.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", PersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                 TypeBimeId = PersonalInfo.fldTypeBimeId;
                int oldmashmol = Convert.ToInt32(mashmol);
                 int ZaribHoghNew = ZaribHoghAval;
      List<ClsHokmItems> newdt = null;
      if(AnvaEstekhdam==10){var FirstRow1 = HokmItem[0];
                            FirstRow1.ZaribHoghNew = 0;//FirstRow1.ZaribHoghAval = 0;
                            FirstRow1.MashmulKosurat = 0;return HokmItem;}
      foreach (var item in HokmItem)
      {
        newdt = sum(PersonalId, item.ItemHoghughiId, TypeEstekhdam, item.Zarib, TaaholStatus, Tarikh_Ejra, Group, TopGroup, TedadFarzandan, Gender,Convert.ToInt32(TypeBimeId), 					AnvaEstekhdam, HokmItem,ZaribHoghAval,ZaribHoghNew, HasZarib,Tatbigh,oldmashmol);
      }
      foreach (var item in HokmItem)//محاسبه مجدد
      {
        newdt = sum(PersonalId, item.ItemHoghughiId, TypeEstekhdam, item.Zarib, TaaholStatus, Tarikh_Ejra, Group, TopGroup, TedadFarzandan, Gender, Convert.ToInt32(TypeBimeId), 					AnvaEstekhdam, HokmItem,ZaribHoghAval,ZaribHoghNew, HasZarib,Tatbigh,oldmashmol);
      }
      foreach (var item in HokmItem)//محاسبه مجدد
      {
        newdt = sum(PersonalId, item.ItemHoghughiId, TypeEstekhdam, item.Zarib, TaaholStatus, Tarikh_Ejra, Group, TopGroup, TedadFarzandan, Gender, Convert.ToInt32(TypeBimeId), 					AnvaEstekhdam, HokmItem,ZaribHoghAval,ZaribHoghNew, HasZarib,Tatbigh,oldmashmol);
      }
      var FirstRow = newdt[0];
      FirstRow.ZaribHoghNew = ZaribHoghNew;
      FirstRow.MashmulKosurat = Mashmaul_Kosurat;
      return newdt;
    }
        public List<ClsHokmItems> sum(int PersonalId, int ItemHoghoghi,
                                       int TypeEstekhdam,
                                       double Zarib,
                                       int TaaholStatus,
                                       string Tarikh_Ejra,
                                       int Group,
                                       int TopGroup,
                                       int TedadFarzandan,
                                       int Gender,
                                       int TypeBimeId,
                                       int AnvaEstekhdam,
                                       List<ClsHokmItems> dt,
                                       int ZaribHoghAval,
                                       int ZaribHoghNew,
                                       bool HasZarib,
                                       int _Tatbigh,
                                       int oldmashmol)
        {
            var Mablagh = 0;
            ClsHokmItems _M = new ClsHokmItems();
            switch (ItemHoghoghi)
            {
                case 1: //حقوق مبنا
                    if (TypeEstekhdam != 4)
                    {
                        Mablagh = HoghughMabna(TypeEstekhdam, Tarikh_Ejra, Group, TopGroup, ZaribHoghAval, ZaribHoghNew, HasZarib);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 2: //سنوات
                    if (Zarib > 0)
                    {
                        if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                        {
                            Mablagh = SanavatKarmandi(Zarib, TypeEstekhdam, dt, Tarikh_Ejra, PersonalId);
                        }
                        else if (TypeEstekhdam == 1)
                        {
                            Mablagh = SanavatKargari(Zarib, TypeEstekhdam, dt, Tarikh_Ejra, PersonalId);
                        }
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 3: //پایه سنواتی
                    if (TypeEstekhdam == 1)
                    {
                        Mablagh = PayeSanavat(Tarikh_Ejra, Group, TopGroup);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 4: //سنوات بسیجی
                    if (Zarib > 0)
                    {
                        Mablagh = SanavatBasiji(Zarib, TypeEstekhdam, dt, Tarikh_Ejra, PersonalId);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 5: //سنوات ایثارگری
                    Mablagh = SanavatIsar(Zarib, TypeEstekhdam, dt, Tarikh_Ejra, PersonalId);
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    _M.Mablagh = Mablagh;
                    //_M.Zarib=zaribIsar;
                    //zaribIsar=0;
                    break;
                case 6: //فوق العاده شغل
                    if (Zarib > 0)
                    {
                        Mablagh = Fogh_Shoghl(Zarib, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 7: // تحقیقی و تخصصی
                    if (Zarib > 0)
                    {
                        Mablagh = Fogh_Shoghl(Zarib, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 8: //ماده 26
                    if (Zarib > 0)
                    {
                        Mablagh = Fogh_Shoghl(Zarib, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 9: // مدیریتی و سرپرستی
                    if (Zarib > 0)
                    {
                        Mablagh = Fogh_Shoghl(Zarib, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 10: //برجستگی 
                    if (Zarib > 0)
                    {
                        Mablagh = Fogh_Shoghl(Zarib, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 11: //تفاوت تطبیق 
                    if (TypeEstekhdam != 4)
                    {
                        Mablagh = Tatbigh(TypeEstekhdam, dt, Tarikh_Ejra);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 12: //فوق العاده ایثارگری  
                    if (Zarib > 0)
                    {
                        Mablagh = Fogh_Isar(Zarib, Tarikh_Ejra);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 13: //بدی آب و هوا 
                    if (Zarib > 0)
                    {
                        Mablagh = Fogh_Shoghl(Zarib, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 14: // تسهیلات زندگی 
                    if (Zarib > 0)
                    {
                        Mablagh = Fogh_Shoghl(Zarib, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 15: // سختی کار 
                    if (Zarib > 0)
                    {
                        Mablagh = Fogh_Shoghl(Zarib, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 17: // مزایای ریالی 
                    Mablagh = MazayaRiyali(Group, TopGroup, Tarikh_Ejra, dt, ZaribHoghAval, ZaribHoghNew, HasZarib);
                    //_M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    //_M.Mablagh = Mablagh;
                    break;
                case 19: //جذب
                    if (Zarib > 0)
                    {
                        Mablagh = Jazb(Zarib, TypeEstekhdam, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 40: //جذب مشاغل تخصصی
                    if (Zarib > 0)
                    {
                        Mablagh = JazbTakhasosi(TedadFarzandan, Zarib, Tarikh_Ejra, TaaholStatus, Gender);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 41: //جذب ناشی از ضریب تعدیل
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    if (HasZarib == true)
                    {
                        Mablagh = 0;//JazbZaribTadil(TypeEstekhdam,dt,Tarikh_Ejra,_Tatbigh,Zarib);
                        _M.Mablagh = Mablagh;
                    }
                    else
                        _M.Mablagh = 0;
                    break;
                case 20: // مخصوص 
                    if (Zarib > 0)
                    {
                        Mablagh = Makhsoos(Zarib, TypeEstekhdam, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 21: // فوق العاده ویژه 
                    if (Zarib > 0)
                    {
                        Mablagh = Vizheh(Zarib, TypeEstekhdam, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 22: // حق اولاد
                    if (TypeEstekhdam != 4)
                    {
                        Mablagh = HaghOlad(TedadFarzandan, TypeEstekhdam, Tarikh_Ejra, TaaholStatus, Gender, PersonalId);
                        if (Convert.ToInt32(Tarikh_Ejra.Replace("/", "")) >= 14010701 && Mablagh > 0 && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1401 && TypeEstekhdam != 1)
                        {
                            Mablagh += (1056195 * TedadFarzandan);
                        }
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 23: //عائله مندی
                    if (TypeEstekhdam != 4)
                    {
                        Mablagh = AeleMandi(TypeEstekhdam, Tarikh_Ejra, TaaholStatus, Gender, PersonalId,TedadFarzandan);
                        if (Convert.ToInt32(Tarikh_Ejra.Replace("/", "")) >= 14010701 && Mablagh > 0 && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1401 && TypeEstekhdam != 1)
                        {
                            Mablagh += 2036947;
                        }
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                //         case 22: // حق اولاد
                //           if(TypeEstekhdam!=4){Mablagh = HaghOlad(TedadFarzandan, TypeEstekhdam, Tarikh_Ejra, TaaholStatus, Gender);
                //                                _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                //                                _M.Mablagh = Mablagh;}
                //           break;
                //         case 23: //عائله مندی
                //           if(TypeEstekhdam!=4){Mablagh = AeleMandi(TypeEstekhdam, Tarikh_Ejra, TaaholStatus, Gender);
                //                                _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                //                                _M.Mablagh = Mablagh;}
                //           break;
                case 24: //خوار و بار
                    if (TypeEstekhdam == 1)
                    {
                        Mablagh = KharoBar(TypeEstekhdam, Tarikh_Ejra, TaaholStatus);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 25: // حق مسکن کارمندی
                    Mablagh = HaghMaskan(Zarib, TypeEstekhdam, Tarikh_Ejra,TedadFarzandan,TaaholStatus,Gender,PersonalId, dt);
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    _M.Mablagh = Mablagh;
                    break;
                case 26: // نوبت کاری
                    if (Zarib > 0)
                    {
                        Mablagh = NobateKari(Zarib, TypeEstekhdam, dt, TypeBimeId, Tarikh_Ejra, AnvaEstekhdam, ZaribHoghNew, HasZarib);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                        _M.Zarib = z_nobat;
                    }
                    break;
                case 27: //بن ماهیانه
                    if (TypeEstekhdam == 1)
                    {
                        Mablagh = HaghBon(TypeEstekhdam, Tarikh_Ejra);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 30: //حق جبهه
                    if (Zarib > 0)
                    {
                        Mablagh = HaghJebhe(TypeEstekhdam, Tarikh_Ejra, Group, TopGroup, Zarib, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 31: //حق جانبازییی
                    if (Zarib > 0)
                    {
                        Mablagh = HaghJebhe(TypeEstekhdam, Tarikh_Ejra, Group, TopGroup, Zarib, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 37: //حق جذب قضایی
                    if (Zarib > 0)
                    {
                        Mablagh = Jazb(Zarib, TypeEstekhdam, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 38: //گردان عاشورا
                    if (Zarib > 0)
                    {
                        Mablagh = HaghJebhe(TypeEstekhdam, Tarikh_Ejra, Group, TopGroup, Zarib, dt);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    break;
                case 39: //ضریب تعدیل
                    if (MyLib.Shamsi.Shamsi2miladiDateTime(Tarikh_Ejra) >= MyLib.Shamsi.Shamsi2miladiDateTime("1397/01/01"))
                    {
                        Mablagh = ZaribTaadil(TypeBimeId, Tarikh_Ejra, AnvaEstekhdam, dt, HasZarib, oldmashmol);
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                        _M.Zarib = Zarib2;
                        Zarib2 = 0;
                    }
                    else
                    {
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = 0;
                        _M.Zarib = 0;
                        Zarib2 = 0;
                    }
                    break;
                case 42: //تفاوت ناشی از ضریب تعدیل
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1397)
                    {
                        Mablagh = TafavotZaribTadil(TypeEstekhdam, dt, Tarikh_Ejra, _Tatbigh, Zarib);
                        _M.Mablagh = Mablagh;
                    }
                    else
                        _M.Mablagh = 0;
                    break;
                case 46: //تفاوت بند6
                    /*if(TypeEstekhdam!=4){_M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
          if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4))==1398)
          {  
          //Mablagh = TafavotBand6(TypeEstekhdam,dt,Tarikh_Ejra);
          //_M.Mablagh = Mablagh;
          }
          else
          _M.Mablagh = 0;}*/
                    break;
                case 44: //تفاوت بند ی
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) >= 1399)
                    {
                        var b6 = dt.Where(l => l.ItemHoghughiId == 46).FirstOrDefault();
                        Mablagh = Band_y(Tarikh_Ejra, PersonalId, oldmashmol, AnvaEstekhdam, TypeBimeId, b6.Mablagh);
                        _M.Mablagh = Mablagh;
                    }
                    else
                        _M.Mablagh = 0;
                    break;
                case 45: //تفاوت جز1
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) >= 1399)
                    {
                        Mablagh = joz1(dt, ZaribHoghAval, TypeBimeId, AnvaEstekhdam, Tarikh_Ejra, PersonalId);
                        _M.Mablagh = Mablagh;
                    }
                    else
                        _M.Mablagh = 0;
                    break;
                case 47: //جذب2
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) >= 1399)
                    {
                        Mablagh = jazb2(dt);
                        _M.Mablagh = Mablagh;
                    }
                    else
                        _M.Mablagh = 0;
                    break;
                case 48: //جذب3
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) >= 1399)
                    {
                        Mablagh = jazb3(dt);
                        _M.Mablagh = Mablagh;
                    }
                    else
                        _M.Mablagh = 0;
                    break;
                case 49: //ویژه2
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) >= 1399)
                    {
                        Mablagh = vije2(dt);
                        _M.Mablagh = Mablagh;
                    }
                    else
                        _M.Mablagh = 0;
                    break;
                case 50: //ویژه3
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) >= 1399)
                    {
                        Mablagh = vije3(dt);
                        _M.Mablagh = Mablagh;
                    }
                    else
                        _M.Mablagh = 0;
                    break;
                case 51: //مخصوص2
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) >= 1398)
                    {
                        Mablagh = makhsos2(dt);
                        _M.Mablagh = Mablagh;
                    }
                    else
                        _M.Mablagh = 0;
                    break;
                case 52: //مخصوص3
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) >= 1399)
                    {
                        Mablagh = makhsos3(dt);
                        _M.Mablagh = Mablagh;
                    }
                    else
                        _M.Mablagh = 0;
                    break;
                case 53: //تطبیق1
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    //           if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4))>=1399)
                    //           {
                    //             int sumAll=0;
                    //             for(int y=0;y<dt.Count();y++){
                    //               if(Convert.ToInt32(dt[y].ItemHoghughiId)!=53 && Convert.ToInt32(dt[y].ItemHoghughiId)!=55 && Convert.ToInt32(dt[y].ItemHoghughiId)!=56 && Convert.ToInt32(dt[y].ItemHoghughiId)!=25&& Convert.ToInt32(dt[y].ItemHoghughiId)!=57){                  
                    //                 sumAll+=dt[y].Mablagh;
                    //                 if(Convert.ToInt32(Tarikh_Ejra.Replace("/",""))>=14010701 && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4))==1401 && TypeEstekhdam!=1 && Convert.ToInt32(dt[y].ItemHoghughiId)==22 && dt[y].Mablagh>0){
                    //                   sumAll=sumAll-(1056195*TedadFarzandan);
                    //                 }
                    //                 if(Convert.ToInt32(Tarikh_Ejra.Replace("/",""))>=14010701 && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4))==1401 && TypeEstekhdam!=1 && Convert.ToInt32(dt[y].ItemHoghughiId)==23 && dt[y].Mablagh>0){
                    //                   sumAll=sumAll-2036947;
                    //                 }
                    //               }
                    //             }
                    //             if(Convert.ToInt32(Tarikh_Ejra.Substring(0, 4))==1399){
                    //               if(sumAll<28000000)
                    //                 _M.Mablagh = 28000000-sumAll;
                    //               else 
                    //                 _M.Mablagh =0;
                    //             }
                    //             else if(Convert.ToInt32(Tarikh_Ejra.Substring(0, 4))==1400){
                    //               if(sumAll<35000000)
                    //                 _M.Mablagh = 35000000-sumAll;
                    //               //_M.Mablagh =0;
                    //             }
                    //             else if(Convert.ToInt32(Tarikh_Ejra.Substring(0, 4))==1401){
                    //               if(sumAll<56000000){                  
                    //                 _M.Mablagh = 56000000-sumAll;
                    //               }
                    //               else _M.Mablagh =0;
                    //             }
                    //             else 
                    //               _M.Mablagh = 0;
                    //           }else _M.Mablagh = 0;
                    break;
                //         case 53: //تطبیق1
                //           _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                //           if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4))>=1399)
                //           { 
                //             /*NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                // int Year = Convert.ToInt32(Tarikh_Ejra.Substring(0, 4));
                // string commandText1 = "SELECT   top(1)     fldId FROM  Prs.tblPersonalHokm " +
                // " where fldPrs_PersonalInfoId=" + PersonalId + " and    SUBSTRING(fldTarikhEjra,1,4)=" + Year +
                // " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
                // var _Hokm1 = service.GetData(commandText1).Tables[0];
                // if (_Hokm1.Rows.Count > 0)
                // {
                // commandText1 = "SELECT Prs.tblHokm_Item.fldItems_EstekhdamId, Prs.tblHokm_Item.fldMablagh FROM Prs.tblHokm_Item INNER JOIN Com.tblItems_Estekhdam ON Prs.tblHokm_Item.fldItems_EstekhdamId = Com.tblItems_Estekhdam.fldId WHERE (Prs.tblHokm_Item.fldPersonalHokmId = "+Convert.ToInt32(_Hokm1.Rows[0]["fldId"])+") AND (Com.tblItems_Estekhdam.fldItemsHoghughiId = 53)";
                // var Hokm_Item2 = service.GetData(commandText1).Tables[0];
                // _M.Mablagh = Convert.ToInt32(Hokm_Item2.Rows[0]["fldMablagh"]);
                // }else{*/
                //             int sumAll=0;
                //             for(int y=0;y<dt.Count();y++){
                //               sumAll+=dt[y].Mablagh;
                //             }
                //             if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4))==1399){
                //               if(sumAll<28000000)
                //                 _M.Mablagh = 28000000-sumAll;
                //             }else if (HasZarib == true && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4))==1400){
                //               if(sumAll<28000000)
                //                 _M.Mablagh = 35000000-sumAll;
                //             }
                //             else if(Convert.ToInt32(Tarikh_Ejra.Substring(0, 4))==1401){
                //               if(sumAll<56000000)
                //                 _M.Mablagh = 56000000-sumAll;
                //               else _M.Mablagh =0;
                //             }
                //             //}
                //           }
                //           else
                //             _M.Mablagh = 0;
                //           break;
                case 54: //خاص
                    _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                    if (Convert.ToInt32(Tarikh_Ejra.Replace("/", "")) >= 13990611)
                    {
                        Mablagh = Khas(Zarib, TypeEstekhdam, dt);
                        _M.Mablagh = Mablagh;
                    }
                    else
                        _M.Mablagh = 0;
                    break;
                case 57: //ترمیم حقوق
                    if (Convert.ToInt32(Tarikh_Ejra.Replace("/", "")) >= 14010701 && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1401)
                    {
                        Mablagh = 10059000;
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = Mablagh;
                    }
                    else
                    {
                        _M = dt.Where(l => l.ItemHoghughiId == ItemHoghoghi).FirstOrDefault();
                        _M.Mablagh = 0;
                    }
                    break;
            }
            return dt;
        }
        public int jazb2(List<ClsHokmItems> HokmItem)
        {
            double z_jazb = 0; int Band_y1 = 0;
            var zjazb = HokmItem.Where(l => l.ItemHoghughiId == 19).FirstOrDefault();
            if (zjazb != null)
                z_jazb = zjazb.Zarib;
            var Bandy = HokmItem.Where(l => l.ItemHoghughiId == 44).FirstOrDefault();
            if (Bandy != null)
                Band_y1 = Bandy.Mablagh;
            return Convert.ToInt32((Band_y1) * z_jazb / 100.0);
        }
        public int jazb3(List<ClsHokmItems> HokmItem)
        {
            double z_jazb = 0; int joz1 = 0;
            var zjazb = HokmItem.Where(l => l.ItemHoghughiId == 19).FirstOrDefault();
            if (zjazb != null)
                z_jazb = zjazb.Zarib;
            var joz_1 = HokmItem.Where(l => l.ItemHoghughiId == 45).FirstOrDefault();
            if (joz_1 != null)
                joz1 = joz_1.Mablagh;
            return Convert.ToInt32((joz1) * z_jazb / 100.0);
        }
        public int vije2(List<ClsHokmItems> HokmItem)
        {
            double z_vije = 0; int Band_y1 = 0;
            var zvije = HokmItem.Where(l => l.ItemHoghughiId == 21).FirstOrDefault();
            if (zvije != null)
                z_vije = zvije.Zarib;
            var jazb2 = HokmItem.Where(l => l.ItemHoghughiId == 47).FirstOrDefault();
            //var bandy=HokmItem.Where(l => l.ItemHoghughiId == 44).FirstOrDefault();
            if (jazb2 != null)
                Band_y1 = jazb2.Mablagh;
            //if(bandy!=null)Band_y1+=bandy.Mablagh;
            return Convert.ToInt32((Band_y1) * z_vije / 100.0);
        }
        public int vije3(List<ClsHokmItems> HokmItem)
        {
            double z_vije = 0; int joz1 = 0;
            var zvije = HokmItem.Where(l => l.ItemHoghughiId == 21).FirstOrDefault();
            if (zvije != null)
                z_vije = zvije.Zarib;
            var jazb3 = HokmItem.Where(l => l.ItemHoghughiId == 48).FirstOrDefault();
            //var joz_1=HokmItem.Where(l => l.ItemHoghughiId == 45).FirstOrDefault();
            if (jazb3 != null)
                joz1 = jazb3.Mablagh;
            //if(joz_1!=null)joz1+=joz_1.Mablagh;   
            return Convert.ToInt32((joz1) * z_vije / 100.0);
        }
        public int Khas(double Zarib, int TypeEstekhdam, List<ClsHokmItems> HokmItem)
        {
            int Hoghugh = 0, Sanavat = 0, S_Basiji = 0, S_Isar = 0, Fogh_Shoghl = 0, Takhasosi = 0,
            Made26 = 0, Modiriyati = 0, Barjastegi = 0, Tatbigh = 0, Jazb = 0, tadil = 0, jazb_takhasosi = 0, jazb_tadil = 0;
            int _Jazb = 0;
            Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
            Sanavat = HokmItem.Where(l => l.ItemHoghughiId == 2).FirstOrDefault().Mablagh;
            S_Basiji = HokmItem.Where(l => l.ItemHoghughiId == 4).FirstOrDefault().Mablagh;
            S_Isar = HokmItem.Where(l => l.ItemHoghughiId == 5).FirstOrDefault().Mablagh;
            Fogh_Shoghl = HokmItem.Where(l => l.ItemHoghughiId == 6).FirstOrDefault().Mablagh;
            Takhasosi = HokmItem.Where(l => l.ItemHoghughiId == 7).FirstOrDefault().Mablagh;
            Made26 = HokmItem.Where(l => l.ItemHoghughiId == 8).FirstOrDefault().Mablagh;
            Modiriyati = HokmItem.Where(l => l.ItemHoghughiId == 9).FirstOrDefault().Mablagh;
            Barjastegi = HokmItem.Where(l => l.ItemHoghughiId == 10).FirstOrDefault().Mablagh;
            Tatbigh = HokmItem.Where(l => l.ItemHoghughiId == 11).FirstOrDefault().Mablagh;
            Jazb = HokmItem.Where(l => l.ItemHoghughiId == 19).FirstOrDefault().Mablagh;
            tadil = HokmItem.Where(l => l.ItemHoghughiId == 16).FirstOrDefault().Mablagh;
            jazb_takhasosi = HokmItem.Where(l => l.ItemHoghughiId == 40).FirstOrDefault().Mablagh;
            jazb_tadil = HokmItem.Where(l => l.ItemHoghughiId == 41).FirstOrDefault().Mablagh;
            var jazb_ghazai = HokmItem.Where(l => l.ItemHoghughiId == 37).FirstOrDefault().Mablagh;
            var jazb2 = HokmItem.Where(l => l.ItemHoghughiId == 47).FirstOrDefault().Mablagh;
            var jazb3 = HokmItem.Where(l => l.ItemHoghughiId == 48).FirstOrDefault().Mablagh;
            _Jazb = Convert.ToInt32((Hoghugh + Sanavat + S_Basiji + S_Isar + Fogh_Shoghl +
                                     Takhasosi + Made26 + Modiriyati + Barjastegi + Jazb + jazb_ghazai + Tatbigh + jazb2 + jazb3) * Zarib / 100.0);
            return _Jazb;
        }
        public int makhsos2(List<ClsHokmItems> HokmItem)
        {
            double z_makhsos = 0; int Band_y1 = 0;
            var zmakhsos = HokmItem.Where(l => l.ItemHoghughiId == 20).FirstOrDefault();
            if (zmakhsos != null)
                z_makhsos = zmakhsos.Zarib;
            var jazb2 = HokmItem.Where(l => l.ItemHoghughiId == 47).FirstOrDefault();
            //var bandy=HokmItem.Where(l => l.ItemHoghughiId == 44).FirstOrDefault();
            if (jazb2 != null)
                Band_y1 = jazb2.Mablagh;
            //if(bandy!=null)Band_y1+=bandy.Mablagh;
            return Convert.ToInt32((Band_y1) * z_makhsos / 100.0);
        }
        public int makhsos3(List<ClsHokmItems> HokmItem)
        {
            double z_makhsos = 0; int joz1 = 0;
            var zmakhsos = HokmItem.Where(l => l.ItemHoghughiId == 20).FirstOrDefault();
            if (zmakhsos != null)
                z_makhsos = zmakhsos.Zarib;
            var jazb3 = HokmItem.Where(l => l.ItemHoghughiId == 48).FirstOrDefault();
            //var joz_1=HokmItem.Where(l => l.ItemHoghughiId == 45).FirstOrDefault();
            if (jazb3 != null)
                joz1 = jazb3.Mablagh;
            //if(joz_1!=null)
            //joz1+=joz_1.Mablagh; 
            return Convert.ToInt32((joz1) * z_makhsos / 100.0);
        }
        public int joz1(List<ClsHokmItems> HokmItem, int ZaribHoghAval, int TypeBimeId, int AnvaEstekhdam, string Tarikh_Ejra, int PersonalId)
        {
            int Mablagh = 0;
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            int Year = Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) - 1;
            string commandText1 = "SELECT   top(1)     fldId FROM  Prs.tblPersonalHokm " +
              " where fldPrs_PersonalInfoId=" + PersonalId + " and    SUBSTRING(fldTarikhEjra,1,4)=" + Year +
              " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
            var _Hokm1 = service.GetData(commandText1).Tables[0];
            if (_Hokm1.Rows.Count > 0)
            {
                commandText1 = "SELECT Prs.tblHokm_Item.fldItems_EstekhdamId, Prs.tblHokm_Item.fldMablagh FROM Prs.tblHokm_Item INNER JOIN Com.tblItems_Estekhdam ON Prs.tblHokm_Item.fldItems_EstekhdamId = Com.tblItems_Estekhdam.fldId WHERE (Prs.tblHokm_Item.fldPersonalHokmId = " + Convert.ToInt32(_Hokm1.Rows[0]["fldId"]) + ") AND (Com.tblItems_Estekhdam.fldItemsHoghughiId = 45)";
                var Hokm_Item2 = service.GetData(commandText1).Tables[0];
                if (Hokm_Item2.Rows.Count > 0)
                Mablagh = Convert.ToInt32(Hokm_Item2.Rows[0]["fldMablagh"]);
            }
            if (Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1399)
                Mablagh = Convert.ToInt32(Mablagh * 1.15);
            if (Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1400)
                Mablagh = Convert.ToInt32(Mablagh * 1.25);
            if (Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1401)
                Mablagh = Convert.ToInt32(Mablagh * 1.1);
            return Mablagh;
        }
        /*public int Band_y1(List<ClsHokmItems> HokmItem)
    {   
    double z_jazb=0,z_makhsos=0,z_vije=0;int Band_y1=0;
    var zjazb = HokmItem.Where(l => l.ItemHoghughiId == 19).FirstOrDefault();
    if(zjazb!=null)
    z_jazb=zjazb.Zarib;
    var zmakhsos = HokmItem.Where(l => l.ItemHoghughiId == 20).FirstOrDefault();
    if(zmakhsos!=null)
    z_makhsos=zmakhsos.Zarib;
    var zvije = HokmItem.Where(l => l.ItemHoghughiId == 21).FirstOrDefault();
    if(zvije!=null)
    z_vije=zvije.Zarib;
    var Bandy1=HokmItem.Where(l => l.ItemHoghughiId == 44).FirstOrDefault();
    if(Bandy1!=null)
    Band_y1=Bandy1.Mablagh;
    return Convert.ToInt32((Band_y1*z_jazb/100.0)+(Band_y1*z_makhsos/100.0)+(Band_y1*z_vije/100.0));
    }*/
        public int Band_y(string Tarikh_Ejra, int PersonalId, int oldmashmol, int AnvaEstekhdam, int TypeBime, int band6)
        {
            int Mablagh = 0;
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            int Year = Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) - 1;
            string commandText1 = "SELECT   top(1)     fldId FROM  Prs.tblPersonalHokm " +
              " where fldPrs_PersonalInfoId=" + PersonalId + " and    SUBSTRING(fldTarikhEjra,1,4)=" + Year +
              " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
            var _Hokm1 = service.GetData(commandText1).Tables[0];
            if (_Hokm1.Rows.Count > 0)
            {
                commandText1 = "SELECT Prs.tblHokm_Item.fldItems_EstekhdamId, Prs.tblHokm_Item.fldMablagh FROM Prs.tblHokm_Item INNER JOIN Com.tblItems_Estekhdam ON Prs.tblHokm_Item.fldItems_EstekhdamId = Com.tblItems_Estekhdam.fldId WHERE (Prs.tblHokm_Item.fldPersonalHokmId = " + Convert.ToInt32(_Hokm1.Rows[0]["fldId"]) + ") AND (Com.tblItems_Estekhdam.fldItemsHoghughiId = 44)";
                var Hokm_Item2 = service.GetData(commandText1).Tables[0];
                if (Hokm_Item2.Rows.Count > 0)
                Mablagh = Convert.ToInt32(Hokm_Item2.Rows[0]["fldMablagh"]);
            }
            if (Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1399)
                Mablagh = Convert.ToInt32(Mablagh * 1.15);
            if (Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1400)
                Mablagh = Convert.ToInt32(Mablagh * 1.25);
            if (Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1401)
                Mablagh = Convert.ToInt32(Mablagh * 1.1);
            return Mablagh;
        }
        public int TafavotBand6(int TypeEstekhdam, List<ClsHokmItems> HokmItem, string Tarikh_Ejra)
        {
            int Hoghugh = 0, Sanavat = 0, S_Basiji = 0, S_Isar = 0, Fogh_Shoghl = 0, Takhasosi = 0,
            Made26 = 0, Modiriyati = 0, Barjastegi = 0, Tatbigh = 0, jazb = 0, vije = 0;
            string commandText = "SELECT        fldHadaghalTadil " +
              "FROM            Prs.tblMoteghayerhaAhkam  " +
              " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=1";
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var _Hadeaghal = service.GetData(commandText).Tables[0];
            int Mablagh = 0;
            if (_Hadeaghal.Rows.Count > 0)
            {
                Mablagh = Convert.ToInt32(_Hadeaghal.Rows[0]["fldHadaghalTadil"]);
            }
            int Band6 = 0;
            Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
            Sanavat = HokmItem.Where(l => l.ItemHoghughiId == 2).FirstOrDefault().Mablagh;
            if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
            {
                S_Basiji = HokmItem.Where(l => l.ItemHoghughiId == 4).FirstOrDefault().Mablagh;
                S_Isar = HokmItem.Where(l => l.ItemHoghughiId == 5).FirstOrDefault().Mablagh;
                Fogh_Shoghl = HokmItem.Where(l => l.ItemHoghughiId == 6).FirstOrDefault().Mablagh;
                Takhasosi = HokmItem.Where(l => l.ItemHoghughiId == 7).FirstOrDefault().Mablagh;
                Made26 = HokmItem.Where(l => l.ItemHoghughiId == 8).FirstOrDefault().Mablagh;
                Modiriyati = HokmItem.Where(l => l.ItemHoghughiId == 9).FirstOrDefault().Mablagh;
                Barjastegi = HokmItem.Where(l => l.ItemHoghughiId == 10).FirstOrDefault().Mablagh;
                Tatbigh = HokmItem.Where(l => l.ItemHoghughiId == 11).FirstOrDefault().Mablagh;
                jazb = HokmItem.Where(l => l.ItemHoghughiId == 19).FirstOrDefault().Mablagh;
                vije = HokmItem.Where(l => l.ItemHoghughiId == 21).FirstOrDefault().Mablagh;
                Band6 = Mablagh - (Hoghugh + Sanavat + S_Basiji + S_Isar + Fogh_Shoghl + Takhasosi + Made26 + Modiriyati + Barjastegi + Tatbigh + jazb + vije);
                if (Band6 < 0)
                    Band6 = 0;
            }
            return Band6;
        }
        public int HoghughMabna(int TypeEstekhdam, string Tarikh_Ejra, int Group, int TopGroup, int ZaribHoghAval, int ZaribHoghNew, bool HasZarib)
        {
            string Type = "0"; int _HoghughMabna = 0;
            if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                Type = "1";
            int g = Group + TopGroup;
            var Year = Convert.ToInt32(Tarikh_Ejra.Substring(0, 4));
            string commandText = "SELECT         Prs.tblDetailHoghoghMabna.fldId, Prs.tblDetailHoghoghMabna.fldMablagh " +
              "FROM   Prs.tblDetailHoghoghMabna INNER JOIN " +
              "Prs.tblHoghoghMabna ON Prs.tblDetailHoghoghMabna.fldHoghoghMabnaId = Prs.tblHoghoghMabna.fldId " +
              " WHERE Prs.tblHoghoghMabna.flddesc<='" + Tarikh_Ejra + "' and fldYear=" + Year.ToString() + " AND fldType=" + Type + "AND fldGroh=" + g + " order by Prs.tblHoghoghMabna.flddesc desc";
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var tblHoghugh = service.GetData(commandText).Tables[0];
            if (tblHoghugh.Rows.Count > 0)
            {
                if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                {
                    if (Year < 1397)
                        _HoghughMabna = Convert.ToInt32(tblHoghugh.Rows[0]["fldMablagh"]);
                    else if (Year >= 1397 && HasZarib == false)
                        _HoghughMabna = Convert.ToInt32(tblHoghugh.Rows[0]["fldMablagh"]) * ZaribHoghAval;
                    else if (Year >= 1397 && HasZarib == true)
                        _HoghughMabna = Convert.ToInt32(tblHoghugh.Rows[0]["fldMablagh"]) * ZaribHoghNew;
                }
                else if (TypeEstekhdam == 1)
                    _HoghughMabna = Convert.ToInt32(tblHoghugh.Rows[0]["fldMablagh"]);
            }
            return _HoghughMabna;
        }
        public int HaghJebhe(int TypeEstekhdam, string Tarikh_Ejra, int Group, int TopGroup, double Zarib, List<ClsHokmItems> HokmItem)
        {
            string Type = "0"; int Mablagh1 = 0, Mablagh2 = 0, Mablagh = 0;
            //////////////////////////شاهرود
            //Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
            //Mablagh = Convert.ToInt32(Hoghugh * Zarib / 100.0);
            //return Mablagh;
            ///////////////////////////
            if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                Type = "1";
            int g = Group + TopGroup;
            string commandText = "SELECT         Prs.tblDetailHoghoghMabna.fldId, Prs.tblDetailHoghoghMabna.fldMablagh " +
              "FROM   Prs.tblDetailHoghoghMabna INNER JOIN " +
              "Prs.tblHoghoghMabna ON Prs.tblDetailHoghoghMabna.fldHoghoghMabnaId = Prs.tblHoghoghMabna.fldId " +
              " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + Type + "AND fldGroh=" + g;
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var tblHoghugh = service.GetData(commandText).Tables[0];
            if (tblHoghugh.Rows.Count > 0)
            {
                Mablagh1 = Convert.ToInt32(tblHoghugh.Rows[0]["fldMablagh"]);
            }
            commandText = "SELECT         Prs.tblDetailHoghoghMabna.fldId, Prs.tblDetailHoghoghMabna.fldMablagh " +
              "FROM   Prs.tblDetailHoghoghMabna INNER JOIN " +
              "Prs.tblHoghoghMabna ON Prs.tblDetailHoghoghMabna.fldHoghoghMabnaId = Prs.tblHoghoghMabna.fldId " +
              " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + Type + "AND fldGroh=" + (g + 1);
            tblHoghugh = service.GetData(commandText).Tables[0];
            if (tblHoghugh.Rows.Count > 0)
            {
                Mablagh2 = Convert.ToInt32(tblHoghugh.Rows[0]["fldMablagh"]);
            }
            Mablagh = Mablagh2 - Mablagh1;
            return Mablagh;
        }
        public int PayeSanavat(string Tarikh_Ejra, int Group, int TopGroup)
        {
            int _PayeSanavat = 0;
            int g = Group + TopGroup;
            string commandText = "SELECT         Prs.tblDetailPayeSanavati.fldId, Prs.tblDetailPayeSanavati.fldMablagh " +
              "FROM   Prs.tblDetailPayeSanavati INNER JOIN " +
              "Prs.tblPayeSanavati ON Prs.tblDetailPayeSanavati.fldPayeSanavatiId = Prs.tblPayeSanavati.fldId " +
              " WHERE Prs.tblPayeSanavati.flddesc<='" + Tarikh_Ejra + "' and fldYear=" + Tarikh_Ejra.Substring(0, 4) + "AND fldGroh=" + g + " order by Prs.tblPayeSanavati.flddesc desc";
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var tblHoghugh = service.GetData(commandText).Tables[0];
            if (tblHoghugh.Rows.Count > 0)
            {
                _PayeSanavat = Convert.ToInt32(tblHoghugh.Rows[0]["fldMablagh"]);
            }
            return _PayeSanavat;
        }
        public int SanavatKarmandi(double Zarib, int TypeEstekhdam, List<ClsHokmItems> HokmItem, string Tarikh_Ejra, int PersonalId)
        {
            int Hoghugh = 0, Sanavat = 0, S_Basiji = 0, S_Isar = 0;
            int _Sanavat = 0;
            if (Zarib > 0)
            {
                int Year = Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) - 1;
                string commandText = "SELECT   top(1)     * FROM  Prs.tblPersonalHokm " +
                  " WHERE fldPrs_PersonalInfoId=" + PersonalId + " and  SUBSTRING(fldTarikhEjra,1,4)=" + Year +
                  " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                var _Hokm = service.GetData(commandText).Tables[0];
                int Mablagh = 0;
                if (_Hokm.Rows.Count > 0)
                {
                    string expression = "";
                    DataRow[] foundRows;
                    commandText = "SELECT        fldItems_EstekhdamId,fldMablagh FROM            Prs.tblHokm_Item " +
                      "WHERE fldPersonalHokmId=" + Convert.ToInt32(_Hokm.Rows[0]["fldId"]) + "AND fldItems_EstekhdamId IN (13,14,15,16,36,37,38,39)";
                    var Hokm_Item = service.GetData(commandText).Tables[0];
                    if (Convert.ToInt32(_Hokm.Rows[0]["fldAnvaeEstekhdamId"]) == 6)
                        expression = "fldItems_EstekhdamId=13";
                    else if (Convert.ToInt32(_Hokm.Rows[0]["fldAnvaeEstekhdamId"]) == 7)
                        expression = "fldItems_EstekhdamId=36";
                    foundRows = Hokm_Item.Select(expression);
                    if (foundRows.Count() > 0)
                    {
                        Hoghugh = Convert.ToInt32(foundRows[0]["fldMablagh"]);
                    }
                    if (Convert.ToInt32(_Hokm.Rows[0]["fldAnvaeEstekhdamId"]) == 6)
                        expression = "fldItems_EstekhdamId=14";
                    else if (Convert.ToInt32(_Hokm.Rows[0]["fldAnvaeEstekhdamId"]) == 7)
                        expression = "fldItems_EstekhdamId=37";
                    foundRows = Hokm_Item.Select(expression);
                    if (foundRows.Count() > 0)
                    {
                        Sanavat = Convert.ToInt32(foundRows[0]["fldMablagh"]);
                    }
                    if (Convert.ToInt32(_Hokm.Rows[0]["fldAnvaeEstekhdamId"]) == 6)
                        expression = "fldItems_EstekhdamId=15";
                    else if (Convert.ToInt32(_Hokm.Rows[0]["fldAnvaeEstekhdamId"]) == 7)
                        expression = "fldItems_EstekhdamId=38";
                    foundRows = Hokm_Item.Select(expression);
                    if (foundRows.Count() > 0)
                    {
                        S_Basiji = Convert.ToInt32(foundRows[0]["fldMablagh"]);
                    }
                    if (Convert.ToInt32(_Hokm.Rows[0]["fldAnvaeEstekhdamId"]) == 6)
                        expression = "fldItems_EstekhdamId=16";
                    else if (Convert.ToInt32(_Hokm.Rows[0]["fldAnvaeEstekhdamId"]) == 7)
                        expression = "fldItems_EstekhdamId=39";
                    foundRows = Hokm_Item.Select(expression);
                    if (foundRows.Count() > 0)
                    {
                        S_Isar = Convert.ToInt32(foundRows[0]["fldMablagh"]);
                    }
                    _Sanavat = Sanavat + Convert.ToInt32(((Hoghugh + Sanavat + S_Basiji + S_Isar) * (Zarib / 100.0)));
                }
            }
            return _Sanavat;
        }
        public int SanavatBasiji(double Zarib, int TypeEstekhdam, List<ClsHokmItems> HokmItem, string Tarikh_Ejra, int PersonalId)
        {
            int Hoghugh = 0, S_Basiji = 0;
            int _SanavatBasiji = 0;
            if (Zarib > 0)
            {
                int Year = Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) - 1;
                string commandText = "SELECT   top(1)     fldId FROM  Prs.tblPersonalHokm " +
                  " WHERE fldPrs_PersonalInfoId=" + PersonalId + " and    SUBSTRING(fldTarikhEjra,1,4)=" + Tarikh_Ejra.Substring(0, 4) +
                  " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                var _Hokm = service.GetData(commandText).Tables[0];
                if (_Hokm.Rows.Count > 0)
                {
                    commandText = "SELECT        fldItems_EstekhdamId,fldMablagh FROM            Prs.tblHokm_Item " +
                      "WHERE fldPersonalHokmId=" + Convert.ToInt32(_Hokm.Rows[0]["fldId"]) + "AND fldItems_EstekhdamId =15";
                    var Hokm_Item = service.GetData(commandText).Tables[0];
                    S_Basiji = Convert.ToInt32(Hokm_Item.Rows[0]["fldMablagh"]);
                    _SanavatBasiji = S_Basiji;
                }
                else
                {
                    commandText = "SELECT   top(1)     fldId FROM  Prs.tblPersonalHokm " +
                      " WHERE fldPrs_PersonalInfoId=" + PersonalId + " and    SUBSTRING(fldTarikhEjra,1,4)=" + Year +
                      " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
                    _Hokm = service.GetData(commandText).Tables[0];
                    if (_Hokm.Rows.Count > 0)
                    {
                        string expression = "";
                        DataRow[] foundRows;
                        commandText = "SELECT        fldItems_EstekhdamId,fldMablagh FROM            Prs.tblHokm_Item " +
                          "WHERE fldPersonalHokmId=" + Convert.ToInt32(_Hokm.Rows[0]["fldId"]) + "AND fldItems_EstekhdamId IN (13,15)";
                        var Hokm_Item = service.GetData(commandText).Tables[0];
                        expression = "fldItems_EstekhdamId=13";
                        foundRows = Hokm_Item.Select(expression);
                        if (foundRows.Count() > 0)
                        {
                            Hoghugh = Convert.ToInt32(foundRows[0]["fldMablagh"]);
                        }
                        expression = "fldItems_EstekhdamId=15";
                        foundRows = Hokm_Item.Select(expression);
                        if (foundRows.Count() > 0)
                        {
                            S_Basiji = Convert.ToInt32(foundRows[0]["fldMablagh"]);
                        }
                        _SanavatBasiji = Convert.ToInt32(S_Basiji + (Hoghugh * Zarib / 100.0));
                    }
                }
            }
            return _SanavatBasiji;
        }
        public int SanavatIsar(double Zarib, int TypeEstekhdam, List<ClsHokmItems> HokmItem, string Tarikh_Ejra, int PersonalId)
        {
            /*string commandText = "SELECT   Prs.tblSavabeghJebhe_Personal.fldAzTarikh, Prs.tblSavabeghJebhe_Personal.fldTaTarikh,  Prs.tblSavabeghJebhe_Items.fldDarsad_Sal FROM            Prs.tblSavabeghJebhe_Personal INNER JOIN Prs.tblSavabeghJebhe_Items ON Prs.tblSavabeghJebhe_Personal.fldItemId = Prs.tblSavabeghJebhe_Items.fldId WHERE        (Prs.tblSavabeghJebhe_Personal.fldPrsPersonalId = "+PersonalId.ToString()+")";
      NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
      var _savabegh = service.GetData(commandText).Tables[0];
      for (int i=0;i<_savabegh.Rows.Count;i++)
      {
      int tedad=MyLib.Shamsi.DiffOfShamsiDate(_savabegh.Rows[i]["fldAzTarikh"].ToString(), _savabegh.Rows[0]["fldTaTarikh"].ToString());
      Zarib+= Convert.ToDouble(_savabegh.Rows[i]["fldDarsad_Sal"])/365.0*tedad;
      }
      zaribIsar=Math.Round(Zarib,0);*/
            int _SanavatIsar = 0;
            int Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
            if (Zarib > 0)
                _SanavatIsar = Convert.ToInt32(Hoghugh * Zarib / 100);
            return _SanavatIsar;
        }
        public int SanavatKargari(double Zarib, int TypeEstekhdam, List<ClsHokmItems> HokmItem, string Tarikh_Ejra, int PersonalId)
        {
            int Hoghugh = 0, S_Paye = 0;
            int _Sanavat = 0;
            if (Zarib > 0)
            {
                int Year = Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) - 1;
                string commandText = "SELECT   top(1)     fldId FROM  Prs.tblPersonalHokm " +
                  " WHERE fldStatusHokm=1 and fldPrs_PersonalInfoId=" + PersonalId + " and    SUBSTRING(fldTarikhEjra,1,4)=" + Tarikh_Ejra.Substring(0, 4) +
                  " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                var _Hokm = service.GetData(commandText).Tables[0];
                if (_Hokm.Rows.Count > 0)
                {
                    commandText = "SELECT        fldItems_EstekhdamId,fldMablagh FROM            Prs.tblHokm_Item " +
                      "WHERE fldPersonalHokmId=" + Convert.ToInt32(_Hokm.Rows[0]["fldId"]) + "AND fldItems_EstekhdamId =2";
                    var Hokm_Item = service.GetData(commandText).Tables[0];
                    S_Paye = Convert.ToInt32(Hokm_Item.Rows[0]["fldMablagh"]);
                    _Sanavat = S_Paye;
                }
                else
                {
                    commandText = "SELECT   top(1)     fldId FROM  Prs.tblPersonalHokm " +
                      " WHERE fldPrs_PersonalInfoId=" + PersonalId + " and   SUBSTRING(fldTarikhEjra,1,4)=" + Year +
                      " ORDER BY fldTarikhSodoor DESC,fldTarikhEjra DESC";
                    _Hokm = service.GetData(commandText).Tables[0];
                    if (_Hokm.Rows.Count > 0)
                    {
                        string expression = "";
                        DataRow[] foundRows;
                        commandText = "SELECT        fldItems_EstekhdamId,fldMablagh FROM            Prs.tblHokm_Item " +
                          "WHERE fldPersonalHokmId=" + Convert.ToInt32(_Hokm.Rows[0]["fldId"]) + "AND fldItems_EstekhdamId IN (2,3)";
                        var Hokm_Item = service.GetData(commandText).Tables[0];
                        expression = "fldItems_EstekhdamId=2";
                        foundRows = Hokm_Item.Select(expression);
                        if (foundRows.Count() > 0)
                        {
                            Hoghugh = Convert.ToInt32(foundRows[0]["fldMablagh"]);
                        }
                        expression = "fldItems_EstekhdamId=3";
                        foundRows = Hokm_Item.Select(expression);
                        if (foundRows.Count() > 0)
                        {
                            S_Paye = Convert.ToInt32(foundRows[0]["fldMablagh"]);
                        }
                        _Sanavat = Convert.ToInt32((S_Paye + Hoghugh) * Zarib);
                    }
                }
            }
            return _Sanavat;
        }
        public int Tatbigh(int TypeEstekhdam, List<ClsHokmItems> HokmItem, string Tarikh_Ejra)
        {
            int Hoghugh = 0, Sanavat = 0, S_Basiji = 0, S_Isar = 0, Fogh_Shoghl = 0, Takhasosi = 0,
            Made26 = 0, Modiriyati = 0, Barjastegi = 0;
            int _Tatbigh = 0;
            string commandText = "SELECT        fldHadaghalDaryafti " +
              "FROM            Prs.tblMoteghayerhaAhkam  " +
              " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=1";
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var _Hadeaghal = service.GetData(commandText).Tables[0];
            int Mablagh = 0;
            if (_Hadeaghal.Rows.Count > 0)
            {
                Mablagh = Convert.ToInt32(_Hadeaghal.Rows[0]["fldHadaghalDaryafti"]);
            }
            Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
            Sanavat = HokmItem.Where(l => l.ItemHoghughiId == 2).FirstOrDefault().Mablagh;
            if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
            {
                S_Basiji = HokmItem.Where(l => l.ItemHoghughiId == 4).FirstOrDefault().Mablagh;
                S_Isar = HokmItem.Where(l => l.ItemHoghughiId == 5).FirstOrDefault().Mablagh;
                Fogh_Shoghl = HokmItem.Where(l => l.ItemHoghughiId == 6).FirstOrDefault().Mablagh;
                Takhasosi = HokmItem.Where(l => l.ItemHoghughiId == 7).FirstOrDefault().Mablagh;
                Made26 = HokmItem.Where(l => l.ItemHoghughiId == 8).FirstOrDefault().Mablagh;
                Modiriyati = HokmItem.Where(l => l.ItemHoghughiId == 9).FirstOrDefault().Mablagh;
                Barjastegi = HokmItem.Where(l => l.ItemHoghughiId == 10).FirstOrDefault().Mablagh;
                _Tatbigh = Convert.ToInt32(Mablagh - (Hoghugh + Sanavat + S_Basiji + S_Isar + Fogh_Shoghl + Takhasosi + Made26 + Modiriyati + Barjastegi));
                if (_Tatbigh < 0)
                    _Tatbigh = 0;
            }
            return _Tatbigh;
        }
        public int JazbZaribTadil(int TypeEstekhdam, List<ClsHokmItems> HokmItem, string Tarikh_Ejra, int TafavotTatbigh, double Zarib)
        {
            int _Tatbigh = 0;
            double _Jazb = 0;
            var TatbighItem = HokmItem.Where(l => l.ItemHoghughiId == 11).FirstOrDefault();//تفاوت تطبیق
            var JazbItem = HokmItem.Where(l => l.ItemHoghughiId == 19).FirstOrDefault();//فوق العاده جذب
            if (TatbighItem != null)
                _Tatbigh = TatbighItem.Mablagh;
            if (JazbItem != null)
                _Jazb = JazbItem.Zarib;
            var Tadil = 0;
            Tadil = Convert.ToInt32((TafavotTatbigh - _Tatbigh) * _Jazb / 100);
            return Tadil;
        }
        public int TafavotZaribTadil(int TypeEstekhdam, List<ClsHokmItems> HokmItem, string Tarikh_Ejra, int TafavotTatbigh, double Zarib)
        {
            int EsType = 1;
            if (TypeEstekhdam == 1)
                EsType = 0;
            else if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                EsType = 1;
            int _Tatbigh = 0;
            int _Jazb = 0;
            int HadaghalTadil = 0;
            int Hoghugh = 0, Sanavat = 0, S_Basiji = 0, S_Isar = 0, Fogh_Shoghl = 0, Takhasosi = 0,
            Made26 = 0, Barjastegi = 0, Tatbigh = 0, JazbTadil = 0, FoghVizhe = 0, FoghMakhsos = 0, Modiriyati = 0;
            string commandText = "SELECT        fldId, fldYear, fldType, fldHagheOlad, fldHagheAeleMandi, fldKharoBar, fldMaskan, fldKharoBarMojarad, fldHadaghalDaryafti, fldHaghBon, fldUserId, fldDate, fldDesc, fldHadaghalTadil " +
              " FROM            Prs.tblMoteghayerhaAhkam " +
              " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + EsType;
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var _MoteghayerhaAhkam = service.GetData(commandText).Tables[0];
            if (_MoteghayerhaAhkam.Rows.Count != 0)
            {
                HadaghalTadil = Convert.ToInt32(_MoteghayerhaAhkam.Rows[0]["fldHadaghalTadil"]);
            }
            var HoghughItem = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault();//حقوق مبنا
            if (HoghughItem != null)
            {
                Hoghugh = HoghughItem.Mablagh;
            }
            var SanavatItem = HokmItem.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();//سنوات
            if (SanavatItem != null)
            {
                Sanavat = SanavatItem.Mablagh;
            }
            var S_BasijiItem = HokmItem.Where(l => l.ItemHoghughiId == 4).FirstOrDefault();//سنوات بسیجی
            if (S_BasijiItem != null)
            {
                S_Basiji = S_BasijiItem.Mablagh;
            }
            var S_IsarItem = HokmItem.Where(l => l.ItemHoghughiId == 5).FirstOrDefault();//سنوات ایثارگری
            if (S_IsarItem != null)
            {
                S_Isar = S_IsarItem.Mablagh;
            }
            var Fogh_ShoghlItem = HokmItem.Where(l => l.ItemHoghughiId == 6).FirstOrDefault();//فوق العاده شغل
            if (Fogh_ShoghlItem != null)
            {
                Fogh_Shoghl = Fogh_ShoghlItem.Mablagh;
            }
            var TakhasosiItem = HokmItem.Where(l => l.ItemHoghughiId == 7).FirstOrDefault();//تحقیقی و تخصصی
            if (TakhasosiItem != null)
            {
                Takhasosi = TakhasosiItem.Mablagh;
            }
            var Made26Item = HokmItem.Where(l => l.ItemHoghughiId == 8).FirstOrDefault();//ماده 26
            if (Made26Item != null)
            {
                Made26 = Made26Item.Mablagh;
            }
            var ModiriyatiItem = HokmItem.Where(l => l.ItemHoghughiId == 9).FirstOrDefault();//مدیریتی
            if (ModiriyatiItem != null)
            {
                Modiriyati = ModiriyatiItem.Mablagh;
            }
            var BarjastegiItem = HokmItem.Where(l => l.ItemHoghughiId == 10).FirstOrDefault();//برجستگی
            if (BarjastegiItem != null)
            {
                Barjastegi = BarjastegiItem.Mablagh;
            }
            var TatbighItem = HokmItem.Where(l => l.ItemHoghughiId == 11).FirstOrDefault();//تفاوت تطبیق
            if (TatbighItem != null)
            {
                Tatbigh = TatbighItem.Mablagh;
            }
            var JazbItem = HokmItem.Where(l => l.ItemHoghughiId == 19).FirstOrDefault();//فوق العاده جذب
            if (JazbItem != null)
            {
                _Jazb = JazbItem.Mablagh;
            }
            var FoghMakhsosItem = HokmItem.Where(l => l.ItemHoghughiId == 20).FirstOrDefault();//فوق العاده مخصوص
            if (FoghMakhsosItem != null)
            {
                FoghMakhsos = FoghMakhsosItem.Mablagh;
            }
            var FoghVizheItem = HokmItem.Where(l => l.ItemHoghughiId == 21).FirstOrDefault();//فوق العاده ویژه
            if (FoghVizheItem != null)
            {
                FoghVizhe = FoghVizheItem.Mablagh;
            }
            var JazbTadilItem = HokmItem.Where(l => l.ItemHoghughiId == 41).FirstOrDefault();//جذب ناشی از تعدیل
            if (JazbTadilItem != null)
            {
                JazbTadil = JazbTadilItem.Mablagh;
            }
            var MablaghItem = Hoghugh + Sanavat + S_Basiji + S_Isar + Fogh_Shoghl + Takhasosi + Made26 + Barjastegi + Tatbigh +
              _Jazb + Modiriyati + JazbTadil + FoghVizhe + FoghMakhsos;
            var Tadil = 0;
            if (MablaghItem < HadaghalTadil)
                Tadil = HadaghalTadil - MablaghItem;
            return Tadil;
        }
        public int Jazb(double Zarib, int TypeEstekhdam, List<ClsHokmItems> HokmItem)
        {
            int Hoghugh = 0, Sanavat = 0, S_Basiji = 0, S_Isar = 0, Fogh_Shoghl = 0, Takhasosi = 0,
            Made26 = 0, Modiriyati = 0, Barjastegi = 0, Tatbigh = 0, Riyali = 0;
            int _Jazb = 0;
            Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
            Sanavat = HokmItem.Where(l => l.ItemHoghughiId == 2).FirstOrDefault().Mablagh;
            if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
            {
                S_Basiji = HokmItem.Where(l => l.ItemHoghughiId == 4).FirstOrDefault().Mablagh;
                S_Isar = HokmItem.Where(l => l.ItemHoghughiId == 5).FirstOrDefault().Mablagh;
                Fogh_Shoghl = HokmItem.Where(l => l.ItemHoghughiId == 6).FirstOrDefault().Mablagh;
                Takhasosi = HokmItem.Where(l => l.ItemHoghughiId == 7).FirstOrDefault().Mablagh;
                Made26 = HokmItem.Where(l => l.ItemHoghughiId == 8).FirstOrDefault().Mablagh;
                Modiriyati = HokmItem.Where(l => l.ItemHoghughiId == 9).FirstOrDefault().Mablagh;
                Barjastegi = HokmItem.Where(l => l.ItemHoghughiId == 10).FirstOrDefault().Mablagh;
                Tatbigh = HokmItem.Where(l => l.ItemHoghughiId == 11).FirstOrDefault().Mablagh;
                Riyali = HokmItem.Where(l => l.ItemHoghughiId == 17).FirstOrDefault().Mablagh;
                _Jazb = Convert.ToInt32((Hoghugh + Sanavat + S_Basiji + S_Isar + Fogh_Shoghl + Takhasosi + Made26 + Modiriyati + Barjastegi + Tatbigh + Riyali) * Zarib / 100.0);
            }
            else if (TypeEstekhdam == 1)
                _Jazb = Convert.ToInt32((Hoghugh) * Zarib / 100.0);
            return _Jazb;
        }
        public int Ghazaye(double Zarib, int TypeEstekhdam, List<ClsHokmItems> HokmItem)
        {
            int Hoghugh = 0, Sanavat = 0, S_Basiji = 0, S_Isar = 0, Fogh_Shoghl = 0, Takhasosi = 0,
            Made26 = 0, Modiriyati = 0, Barjastegi = 0, Tatbigh = 0, Riyali = 0;
            int _Ghazaye = 0;
            Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
            Sanavat = HokmItem.Where(l => l.ItemHoghughiId == 2).FirstOrDefault().Mablagh;
            if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
            {
                S_Basiji = HokmItem.Where(l => l.ItemHoghughiId == 4).FirstOrDefault().Mablagh;
                S_Isar = HokmItem.Where(l => l.ItemHoghughiId == 5).FirstOrDefault().Mablagh;
                Fogh_Shoghl = HokmItem.Where(l => l.ItemHoghughiId == 6).FirstOrDefault().Mablagh;
                Takhasosi = HokmItem.Where(l => l.ItemHoghughiId == 7).FirstOrDefault().Mablagh;
                Made26 = HokmItem.Where(l => l.ItemHoghughiId == 8).FirstOrDefault().Mablagh;
                Modiriyati = HokmItem.Where(l => l.ItemHoghughiId == 9).FirstOrDefault().Mablagh;
                Barjastegi = HokmItem.Where(l => l.ItemHoghughiId == 10).FirstOrDefault().Mablagh;
                Tatbigh = HokmItem.Where(l => l.ItemHoghughiId == 11).FirstOrDefault().Mablagh;
                Riyali = HokmItem.Where(l => l.ItemHoghughiId == 17).FirstOrDefault().Mablagh;
                _Ghazaye = Convert.ToInt32((Hoghugh + Sanavat + S_Basiji + S_Isar + Fogh_Shoghl + Takhasosi + Made26 + Modiriyati + Barjastegi + Tatbigh + Riyali) * Zarib / 100.0);
            }
            else if (TypeEstekhdam == 1)
                _Ghazaye = Convert.ToInt32((Hoghugh) * Zarib / 100.0);
            return _Ghazaye;
        }
        public int NobateKari(double Zarib, int TypeEstekhdam, List<ClsHokmItems> HokmItem, int TypeBime, string TarikhEjra, int AnvaEstekhdam, int zarib2, bool haszarib)
        {
            int Hoghugh = 0;/*, Sanavat = 0, Paye_S = 0, Jebhe = 0, Janbazi = 0,ashora=0;*/
            int _NobateKari = 0;
            Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
            if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
            {
                if (Zarib > 35)
                    Zarib = 35;
                z_nobat = Zarib;
                string commandText = "SELECT    TOP(1)    fldId, fldTarikhEjra, fldTarikhSodur, fldAnvaeEstekhdamId, fldTypeBimeId, fldZaribEzafeKar, fldSaatKari, 			fldDarsadBimePersonal, fldDarsadbimeKarfarma, fldDarsadBimeBikari, fldDarsadBimeJanbazan, " +
                  "fldHaghDarmanKarmand, fldHaghDarmanKarfarma, fldHaghDarmanDolat, fldHaghDarmanMazad, fldHaghDarmanTahteTakaffol, fldDarsadBimeMashagheleZiyanAvar, fldMaxHaghDarman, fldZaribHoghoghiSal,  " +
                  "fldHoghogh, fldFoghShoghl, fldTafavotTatbigh, fldFoghVizhe, fldHaghJazb, fldTadil, fldBarJastegi, fldSanavat,fldFoghTalash, fldUserId, fldDate, fldDesc " +
                  "FROM            Pay.tblMoteghayerhayeHoghoghi " +
                  "WHERE fldAnvaeEstekhdamId=" + AnvaEstekhdam + "AND fldTypeBimeId=" + TypeBime + "AND SUBSTRING(fldTarikhEjra,1,4)<='" + TarikhEjra + "'" +
                  "ORDER BY fldTarikhSodur DESC,fldTarikhEjra DESC";
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                var tblMoteghayerhayeHoghoghi = service.GetData(commandText).Tables[0];
                int ZaribHogh = 0;
                if (tblMoteghayerhayeHoghoghi.Rows.Count > 0)
                {
                    ZaribHogh = Convert.ToInt32(Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldZaribHoghoghiSal"]));
                }
                /*if(haszarib==false)
        _NobateKari = Convert.ToInt32((400*ZaribHogh * Zarib) / 100.0);
        else
        _NobateKari = Convert.ToInt32((400*zarib2 * Zarib) / 100.0);
        int nobatkari1= Convert.ToInt32((Hoghugh * 20) / 100.0);*/
                int nobatkari1 = Convert.ToInt32((Hoghugh * Zarib) / 100.0);
                //if(nobatkari1<_NobateKari)
                _NobateKari = nobatkari1;
            }
            else if (TypeEstekhdam == 1)
            {
                if (Zarib > 15)
                    Zarib = 15;
                z_nobat = Zarib;
                var Sanavat = HokmItem.Where(l => l.ItemHoghughiId == 2).FirstOrDefault();
                var Paye_S = HokmItem.Where(l => l.ItemHoghughiId == 3).FirstOrDefault();
                var Jebhe = HokmItem.Where(l => l.ItemHoghughiId == 30).FirstOrDefault();
                var Janbazi = HokmItem.Where(l => l.ItemHoghughiId == 31).FirstOrDefault();
                var ashora = HokmItem.Where(l => l.ItemHoghughiId == 38).FirstOrDefault();
                int jam = Hoghugh;// + Sanavat + Paye_S;
                if (Sanavat != null)
                {
                    jam += Sanavat.Mablagh;
                }
                if (Paye_S != null)
                {
                    jam += Paye_S.Mablagh;
                }
                if (Jebhe != null)
                {
                    jam += Jebhe.Mablagh;
                }
                if (Janbazi != null)
                {
                    jam += Janbazi.Mablagh;
                }
                if (ashora != null)
                {
                    jam += ashora.Mablagh;
                }
                _NobateKari = Convert.ToInt32(jam * Zarib / 100.0);
            }
            return _NobateKari;
        }
        public int Fogh_Shoghl(double Zarib, List<ClsHokmItems> HokmItem)
        {
            int Hoghugh = 0;
            int _Fogh_Shoghl = 0;
            Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
            _Fogh_Shoghl = Convert.ToInt32(Hoghugh * Zarib / 100.0);
            return _Fogh_Shoghl;
        }
        public int MazayaRiyali(int Group, int TopGroup, string Tarikh_Ejra, List<ClsHokmItems> HokmItem, int ZaribHoghAval, int ZaribHoghNew, bool HasZarib)
        {
            string commandText = "SELECT        Prs.tblDetailHoghoghMabna.fldGroh, Prs.tblDetailHoghoghMabna.fldMablagh, Prs.tblHoghoghMabna.fldType " +
              "FROM            Prs.tblHoghoghMabna INNER JOIN " +
              " Prs.tblDetailHoghoghMabna ON Prs.tblHoghoghMabna.fldId = Prs.tblDetailHoghoghMabna.fldHoghoghMabnaId " +
              " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=1";
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var _MazayaRiyali = service.GetData(commandText).Tables[0];
            int Mablagh = 0;
            int M_Groh = 0;
            int M_TopGroh = 0;
            string expression;// = "ItemHoghughiId=6";
            DataRow[] foundRows;
            if (_MazayaRiyali.Rows.Count > 0)
            {
                double z_FoghShoghl = HokmItem.Where(l => l.ItemHoghughiId == 6).FirstOrDefault().Zarib;
                int T_FoghShoghl = 0;
                double Z_Jazb = HokmItem.Where(l => l.ItemHoghughiId == 19).FirstOrDefault().Zarib;
                int S_Basiji = HokmItem.Where(l => l.ItemHoghughiId == 4).FirstOrDefault().Mablagh;
                int S_Isar = HokmItem.Where(l => l.ItemHoghughiId == 5).FirstOrDefault().Mablagh;
                expression = "fldGroh=" + Group.ToString();
                foundRows = _MazayaRiyali.Select(expression);
                if (foundRows.Count() > 0)
                    M_Groh = Convert.ToInt32(foundRows[0]["fldMablagh"]);
                expression = "fldGroh=" + (Group + TopGroup).ToString();
                foundRows = _MazayaRiyali.Select(expression);
                if (foundRows.Count() > 0)
                    M_TopGroh = Convert.ToInt32(foundRows[0]["fldMablagh"]);
                int T_mabna = 0;
                if (Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) < 1397)
                {
                    T_mabna = M_TopGroh - M_Groh;
                }
                else if (HasZarib == false)
                {
                    T_mabna = ((M_TopGroh * ZaribHoghAval) - (M_Groh * ZaribHoghAval));
                }
                else if (HasZarib == true)
                {
                    T_mabna = ((M_TopGroh * ZaribHoghNew) - (M_Groh * ZaribHoghNew));
                }
                T_FoghShoghl = Convert.ToInt32((T_mabna) * z_FoghShoghl / 100.0);
                Mablagh = T_FoghShoghl + (T_mabna);//+ S_Basiji + S_Isar;
                //if (Z_Jazb != 0)
                //{
                //Mablagh = Mablagh + Convert.ToInt32(Mablagh * Z_Jazb / 100.0);
                //}
            }
            return Mablagh;
        }
        public int Makhsoos(double Zarib, int TypeEstekhdam, List<ClsHokmItems> HokmItem)
        {
            int Hoghugh = 0, Sanavat = 0, S_Basiji = 0, S_Isar = 0, Fogh_Shoghl = 0, Takhasosi = 0,
            Made26 = 0, Modiriyati = 0, Barjastegi = 0, Tatbigh = 0, Jazb = 0, tadil = 0, jazb_takhasosi = 0, jazb_tadil = 0;
            int _Jazb = 0;
            Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
            Sanavat = HokmItem.Where(l => l.ItemHoghughiId == 2).FirstOrDefault().Mablagh;
            S_Basiji = HokmItem.Where(l => l.ItemHoghughiId == 4).FirstOrDefault().Mablagh;
            S_Isar = HokmItem.Where(l => l.ItemHoghughiId == 5).FirstOrDefault().Mablagh;
            Fogh_Shoghl = HokmItem.Where(l => l.ItemHoghughiId == 6).FirstOrDefault().Mablagh;
            Takhasosi = HokmItem.Where(l => l.ItemHoghughiId == 7).FirstOrDefault().Mablagh;
            Made26 = HokmItem.Where(l => l.ItemHoghughiId == 8).FirstOrDefault().Mablagh;
            Modiriyati = HokmItem.Where(l => l.ItemHoghughiId == 9).FirstOrDefault().Mablagh;
            Barjastegi = HokmItem.Where(l => l.ItemHoghughiId == 10).FirstOrDefault().Mablagh;
            Tatbigh = HokmItem.Where(l => l.ItemHoghughiId == 11).FirstOrDefault().Mablagh;
            Jazb = HokmItem.Where(l => l.ItemHoghughiId == 19).FirstOrDefault().Mablagh;
            tadil = HokmItem.Where(l => l.ItemHoghughiId == 16).FirstOrDefault().Mablagh;
            jazb_takhasosi = HokmItem.Where(l => l.ItemHoghughiId == 40).FirstOrDefault().Mablagh;
            jazb_tadil = HokmItem.Where(l => l.ItemHoghughiId == 41).FirstOrDefault().Mablagh;
            var jazb_ghazai = HokmItem.Where(l => l.ItemHoghughiId == 37).FirstOrDefault().Mablagh;
            _Jazb = Convert.ToInt32((Hoghugh + Sanavat + S_Basiji + S_Isar + Fogh_Shoghl +
                                     Takhasosi + Made26 + Modiriyati + Barjastegi + Jazb + jazb_ghazai) * Zarib / 100.0);
            return _Jazb;
        }
        public int Fogh_Isar(double Zarib, string Tarikh_Ejra)
        {
            string commandText = "SELECT        fldHadaghalDaryafti " +
              "FROM            Prs.tblMoteghayerhaAhkam  " +
              " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=1";
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var _MazayaRiyali = service.GetData(commandText).Tables[0];
            int Mablagh = 0;
            if (_MazayaRiyali.Rows.Count > 0)
            {
                Mablagh = Convert.ToInt32(_MazayaRiyali.Rows[0]["fldHadaghalDaryafti"]);
                Mablagh = Convert.ToInt32(Mablagh * Zarib / 100.0);
            }
            return Mablagh;
        }
        public int Vizheh(double Zarib, int TypeEstekhdam, List<ClsHokmItems> HokmItem)
        {
            int Hoghugh = 0, Sanavat = 0, S_Basiji = 0, S_Isar = 0, Fogh_Shoghl = 0, Takhasosi = 0,
            Made26 = 0, Modiriyati = 0, Barjastegi = 0, Tatbigh = 0, Jazb = 0, Riyali = 0;
            int _Jazb = 0;
            Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
            Sanavat = HokmItem.Where(l => l.ItemHoghughiId == 2).FirstOrDefault().Mablagh;
            S_Basiji = HokmItem.Where(l => l.ItemHoghughiId == 4).FirstOrDefault().Mablagh;
            S_Isar = HokmItem.Where(l => l.ItemHoghughiId == 5).FirstOrDefault().Mablagh;
            Fogh_Shoghl = HokmItem.Where(l => l.ItemHoghughiId == 6).FirstOrDefault().Mablagh;
            Takhasosi = HokmItem.Where(l => l.ItemHoghughiId == 7).FirstOrDefault().Mablagh;
            Made26 = HokmItem.Where(l => l.ItemHoghughiId == 8).FirstOrDefault().Mablagh;
            Modiriyati = HokmItem.Where(l => l.ItemHoghughiId == 9).FirstOrDefault().Mablagh;
            Barjastegi = HokmItem.Where(l => l.ItemHoghughiId == 10).FirstOrDefault().Mablagh;
            Tatbigh = HokmItem.Where(l => l.ItemHoghughiId == 11).FirstOrDefault().Mablagh;
            Jazb = HokmItem.Where(l => l.ItemHoghughiId == 19).FirstOrDefault().Mablagh;
            Riyali = HokmItem.Where(l => l.ItemHoghughiId == 17).FirstOrDefault().Mablagh;
            int ghazai = 0, jazbTakhasosi = 0, jazbTadil = 0, tadil = 0;
            ghazai = HokmItem.Where(l => l.ItemHoghughiId == 37).FirstOrDefault().Mablagh;
            jazbTakhasosi = HokmItem.Where(l => l.ItemHoghughiId == 40).FirstOrDefault().Mablagh;
            jazbTadil = HokmItem.Where(l => l.ItemHoghughiId == 41).FirstOrDefault().Mablagh;
            tadil = HokmItem.Where(l => l.ItemHoghughiId == 16).FirstOrDefault().Mablagh;
            _Jazb = Convert.ToInt32((Hoghugh + Sanavat + S_Basiji + S_Isar + Fogh_Shoghl + Takhasosi + Made26 + Modiriyati +
                                     Barjastegi + Tatbigh + Jazb + Riyali + ghazai) * Zarib / 100.0);
            return _Jazb;
        }
        public int HaghOlad(int TedadFarzandan, int TypeEstekhdam, string Tarikh_Ejra, int TaaholStatus, int Gender, int PersonalId)
        {
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            try
            {
                string Type = "0"; int _HagheOlad = 0, CountChild = 0, Mablagh = 0; string commandText = "";
                if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                    Type = "1";
                if (TaaholStatus == 1 || TaaholStatus == 3)
                {
                    return 0;
                }
                if ((TypeEstekhdam == 2 || TypeEstekhdam == 3) && Gender == 0)
                    return 0;
                else
                {
                    if (Convert.ToInt32(Tarikh_Ejra.Replace("/", "")) >= 14010701 && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1401 && TypeEstekhdam != 1)
                    {
                        _HagheOlad = 2112390;
                    }
                    else
                    {
                        commandText = "SELECT    top(1)    fldHagheOlad, fldHagheAeleMandi " +
                          "FROM            Prs.tblMoteghayerhaAhkam " +
                          " WHERE flddesc<='" + Tarikh_Ejra + "' and fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + Type + " ORDER BY fldDesc desc";
                        var tblHagheOlad = service.GetData(commandText).Tables[0];
                        if (tblHagheOlad.Rows.Count > 0)
                        {
                            _HagheOlad = Convert.ToInt32(tblHagheOlad.Rows[0]["fldHagheOlad"]);
                        }
                    }
                    if (TypeEstekhdam != 1)
                    {
                        //             _HagheOlad = Convert.ToInt32(tblHagheOlad.Rows[0]["fldHagheOlad"]) * TedadFarzandan;
                        commandText = "select COUNT(*) as fldCount from prs.tblAfradTahtePooshesh " +
                          "where fldPersonalId=" + PersonalId + " and fldBirthDate>='1401/01/01' and fldNesbat=1 ";
                        var c = service.GetData(commandText).Tables[0];
                        if (Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1401 && c.Rows.Count > 0)
                        {
                            CountChild = Convert.ToInt32(c.Rows[0]["fldCount"]);
                            if (TedadFarzandan > CountChild)
                                Mablagh = _HagheOlad * (TedadFarzandan - CountChild);
                            Mablagh = Mablagh + (_HagheOlad * 3 * CountChild);
                        }
                        else if (Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1403)
                        {
                            if (TedadFarzandan < 3)
                                Mablagh = _HagheOlad * (TedadFarzandan);
                            else
                            {
                                Mablagh = _HagheOlad * 2;
                                Mablagh = Mablagh + Convert.ToInt32((_HagheOlad + (_HagheOlad*0.30))* (TedadFarzandan - 2));
                            }

                        }
                        else
                            Mablagh = _HagheOlad * (TedadFarzandan);

                    }
                    else
                        Mablagh = _HagheOlad * (TedadFarzandan);
                    return Mablagh;
                }
            }
            catch (Exception x)
            {
                DateTime d1;
                d1 = DateTime.Now;
                string excep = "";
                if (x.InnerException != null)
                    excep = "'HaghOlad " + x.InnerException.Message + "'";
                else
                    excep = "'HaghOlad " + x.Message + "'";
                string commandText1 = "exec [Com].[spr_tblErrorInsert] 	0,'admin'," + excep + ",'" + d1.ToString() + "',1,1,''";
                var _Hokm1 = service.GetData(commandText1).Tables[0];
                return 0;
            }
        }
        //     public int HaghOlad(int TedadFarzandan, int TypeEstekhdam, string Tarikh_Ejra, int TaaholStatus, int Gender)
        //     {
        //       string Type = "0";
        //       if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
        //         Type = "1";
        //       if (TaaholStatus == 1 || TaaholStatus == 3 )
        //       {
        //         return 0;
        //       }
        //       if((TypeEstekhdam == 2 || TypeEstekhdam == 3)&& Gender == 0)
        //         return 0;
        //       else
        //       {
        //         string commandText = "SELECT        fldHagheOlad, fldHagheAeleMandi " +
        //           "FROM            Prs.tblMoteghayerhaAhkam " +
        //           " WHERE Prs.tblMoteghayerhaAhkam.flddesc<='"+Tarikh_Ejra+"' and fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + Type+" order by Prs.tblMoteghayerhaAhkam.flddesc desc";
        //         NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
        //         var tblHagheOlad = service.GetData(commandText).Tables[0];
        //         int _HagheOlad = 0;
        //         if (tblHagheOlad.Rows.Count > 0)
        //         {
        //           _HagheOlad = Convert.ToInt32(tblHagheOlad.Rows[0]["fldHagheOlad"]) * TedadFarzandan;
        //         }
        //         return _HagheOlad;
        //       }
        //     }
        public int KharoBar(int TypeEstekhdam, string Tarikh_Ejra, int Gender)
        {
            string Type = "0";
            if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                Type = "1";
            string commandText = "SELECT         fldKharoBar,fldKharoBarMojarad " +
              "FROM            Prs.tblMoteghayerhaAhkam " +
              " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + Type;
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var Moteghayer = service.GetData(commandText).Tables[0];
            int _KharoBar = 0;
            if (Moteghayer.Rows.Count > 0)
            {
                //اگر مجرد یا مجرد با فرزند باشه
                if (Gender == 1 || Gender == 2)
                    _KharoBar = Convert.ToInt32(Moteghayer.Rows[0]["fldKharoBarMojarad"]);
                else
                    _KharoBar = Convert.ToInt32(Moteghayer.Rows[0]["fldKharoBar"]);
            }
            return _KharoBar;
        }
        public int HaghBon(int TypeEstekhdam, string Tarikh_Ejra)
        {
            string Type = "0";
            if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                Type = "1";
            string commandText = "SELECT         fldHaghBon " +
              "FROM            Prs.tblMoteghayerhaAhkam " +
              " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + Type;
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var Moteghayer = service.GetData(commandText).Tables[0];
            int _HaghBon = 0;
            if (Moteghayer.Rows.Count > 0)
            {
                _HaghBon = Convert.ToInt32(Moteghayer.Rows[0]["fldHaghBon"]);
            }
            return _HaghBon;
        }
        public int AeleMandi(int TypeEstekhdam, string Tarikh_Ejra, int TaaholStatus, int Gender, int PersonalId, int TedadFarzandan)
        {
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            try
            {
                string Type = "0"; int _HagheOlad = 0; string commandText = "";
                if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                    Type = "1";
                // اگر آقا و مجرد باشه=0، اگر خانم و متاهل باشه=0
                if (Gender == 0)
                {
                    return 0;
                }
                if (TaaholStatus == 1 /*&& Gender == 1*/)
                    return 0;
                if (TaaholStatus == 2)// || TaaholStatus == 4)
                    return 0;
                else
                {
                    if (Convert.ToInt32(Tarikh_Ejra.Replace("/", "")) >= 14010701 && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1401 && TypeEstekhdam != 1)
                    {
                        _HagheOlad = 6110842;
                    }
                    else
                    {
                        commandText = "SELECT        fldHagheOlad, fldHagheAeleMandi " +
                          "FROM            Prs.tblMoteghayerhaAhkam " +
                          " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + Type;
                        var Moteghayer = service.GetData(commandText).Tables[0];
                        if (Moteghayer.Rows.Count > 0)
                        {
                            _HagheOlad = Convert.ToInt32(Moteghayer.Rows[0]["fldHagheAeleMandi"]);
                        }
                    }
                    if (Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) < 1403 && TypeEstekhdam!=1)
                    {
                        commandText = "select * from prs.tblAfradTahtePooshesh " +
                          "where fldPersonalId=" + PersonalId + " and fldTarikhEzdevaj>='1401/01/01' and fldNesbat=2 ";
                        var c = service.GetData(commandText).Tables[0];
                        if (c.Rows.Count > 0 && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) < 1403)
                        {
                            if (Convert.ToInt32(Tarikh_Ejra.Replace("/", "")) >= 14010701 && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1401 && TypeEstekhdam != 1)
                                _HagheOlad = 12221685;
                            else
                                _HagheOlad = _HagheOlad * 2;
                        }
                    }
                    else if (TedadFarzandan >= 3 && Convert.ToInt32(Tarikh_Ejra.Substring(0, 4)) == 1403 && TypeEstekhdam != 1)
                    {
                        _HagheOlad = _HagheOlad + Convert.ToInt32(_HagheOlad * 0.30);
                    }
                    return _HagheOlad;
                }
            }
            catch (Exception x)
            {
                DateTime d1;
                d1 = DateTime.Now;
                string excep = "";
                if (x.InnerException != null)
                    excep = "'AeleMandi " + x.InnerException.Message + "'";
                else
                    excep = "'AeleMandi " + x.Message + "'";
                string commandText1 = "exec [Com].[spr_tblErrorInsert] 	0,'admin'," + excep + ",'" + d1.ToString() + "',1,1,''";
                var _Hokm1 = service.GetData(commandText1).Tables[0];
                return 0;
            }
        }
        //     public int AeleMandi(int TypeEstekhdam, string Tarikh_Ejra, int TaaholStatus, int Gender)
        //     {
        //       string Type = "0";
        //       if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
        //         Type = "1";
        //       // اگر آقا و مجرد باشه=0، اگر خانم و متاهل باشه=0
        //       /*if (Gender == 0)
        // {
        // return 0;
        // }*/
        //       if((TaaholStatus == 3 || TaaholStatus == 4) && Gender ==0)
        //         return 0;
        //       else if(TaaholStatus == 1 && Gender ==1)
        //         return 0;
        //       else
        //       {
        //         string commandText = "SELECT        fldHagheOlad, fldHagheAeleMandi " +
        //           "FROM            Prs.tblMoteghayerhaAhkam " +
        //           " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + Type;
        //         NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
        //         var Moteghayer = service.GetData(commandText).Tables[0];
        //         int _HagheOlad = 0;
        //         if (Moteghayer.Rows.Count > 0)
        //         {
        //           _HagheOlad = Convert.ToInt32(Moteghayer.Rows[0]["fldHagheAeleMandi"]);
        //         }
        //         return _HagheOlad;
        //       }
        //     }
        public int HaghMaskan(double Zarib, int TypeEstekhdam, string Tarikh_Ejra, int TedadFarzandan, int TaaholStatus, int Gender, int PersonalId, List<ClsHokmItems> HokmItem)
        {
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            try
            {
                int Hoghugh = 0, Sanavat = 0, S_Basiji = 0, S_Isar = 0, Fogh_Shoghl = 0, Takhasosi = 0,
                Made26 = 0, Modiriyati = 0, Barjastegi = 0, Tatbigh = 0, Jazb = 0, Riyali = 0;
                int _HaghMaskan = 0, Vizhe = 0, NobatKari = 0, Fogh_Isar = 0;
                string Type = "0";
                short Year = Convert.ToInt16(Tarikh_Ejra.Substring(0, 4));
                if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                    Type = "1";
                string commandText = "SELECT         fldMaskan " +
                   "FROM            Prs.tblMoteghayerhaAhkam " +
                   " WHERE Prs.tblMoteghayerhaAhkam.flddesc<='" + Tarikh_Ejra + "' and  fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + Type + " order by flddesc desc";
                var Moteghayer = service.GetData(commandText).Tables[0];
                if (Moteghayer.Rows.Count > 0)
                {
                    _HaghMaskan = Convert.ToInt32(Moteghayer.Rows[0]["fldMaskan"]);
                }
                if (TypeEstekhdam == 2 || TypeEstekhdam == 3)
                {
                    /*Hoghugh = HokmItem.Where(l => l.ItemHoghughiId == 1).FirstOrDefault().Mablagh;
          Sanavat = HokmItem.Where(l => l.ItemHoghughiId == 2).FirstOrDefault().Mablagh;
          S_Basiji = HokmItem.Where(l => l.ItemHoghughiId == 4).FirstOrDefault().Mablagh;
          S_Isar = HokmItem.Where(l => l.ItemHoghughiId == 5).FirstOrDefault().Mablagh;
          Fogh_Shoghl = HokmItem.Where(l => l.ItemHoghughiId == 6).FirstOrDefault().Mablagh;
          Takhasosi = HokmItem.Where(l => l.ItemHoghughiId == 7).FirstOrDefault().Mablagh;
          Made26 = HokmItem.Where(l => l.ItemHoghughiId == 8).FirstOrDefault().Mablagh;
          Modiriyati = HokmItem.Where(l => l.ItemHoghughiId == 9).FirstOrDefault().Mablagh;
          Barjastegi = HokmItem.Where(l => l.ItemHoghughiId == 10).FirstOrDefault().Mablagh;
          Tatbigh = HokmItem.Where(l => l.ItemHoghughiId == 11).FirstOrDefault().Mablagh;
          Jazb = HokmItem.Where(l => l.ItemHoghughiId == 19).FirstOrDefault().Mablagh;
          Riyali = HokmItem.Where(l => l.ItemHoghughiId == 17).FirstOrDefault().Mablagh;
          Vizhe = HokmItem.Where(l => l.ItemHoghughiId == 21).FirstOrDefault().Mablagh;
          NobatKari = HokmItem.Where(l => l.ItemHoghughiId == 26).FirstOrDefault().Mablagh;
          Fogh_Isar = HokmItem.Where(l => l.ItemHoghughiId == 12).FirstOrDefault().Mablagh;
          _HaghMaskan = Convert.ToInt32((Hoghugh + Sanavat + S_Basiji + S_Isar + Fogh_Shoghl + Takhasosi + Made26 + Modiriyati + Barjastegi + Tatbigh + Jazb + Riyali + Vizhe + NobatKari + Fogh_Isar) * Zarib / 100.0);*/

                    if (Year < 1403)
                    {
                        int sumAll = 0;
                        for (int y = 0; y < HokmItem.Count(); y++)
                        {
                            if (HokmItem[y].ItemHoghughiId != 25 && HokmItem[y].ItemHoghughiId != 55 && HokmItem[y].ItemHoghughiId != 56)
                                sumAll += HokmItem[y].Mablagh;
                        }
                        _HaghMaskan = Convert.ToInt32(sumAll * Zarib / 100.0);
                    }
                    else if (Year == 1403)
                    {

                        if ((TaaholStatus != 1 && TaaholStatus != 2 && Gender == 1) || (Gender == 0 && (TaaholStatus == 5 || TaaholStatus == 6)))
                            _HaghMaskan += 5000000;
                        if ((TedadFarzandan > 0 && Gender == 1) || (TedadFarzandan > 0 && Gender == 0 && TaaholStatus == 6))

                            _HaghMaskan = _HaghMaskan + (TedadFarzandan * 4000000);
                    }
                }
                /*  else if (TypeEstekhdam == 1)
                  {
                    string commandText = "SELECT         fldMaskan " +
                      "FROM            Prs.tblMoteghayerhaAhkam " +
                      " WHERE Prs.tblMoteghayerhaAhkam.flddesc<='"+Tarikh_Ejra+"' and  fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + Type+" order by flddesc desc";
                    var Moteghayer = service.GetData(commandText).Tables[0];
                    if (Moteghayer.Rows.Count > 0)
                    {
                      _HaghMaskan = Convert.ToInt32(Moteghayer.Rows[0]["fldMaskan"]);
                    }
                  }*/
                return _HaghMaskan;
            }
            catch (Exception x)
            {
                DateTime d1;
                d1 = DateTime.Now;
                string excep = "";
                if (x.InnerException != null)
                    excep = "'HaghMaskan " + x.InnerException.Message + "'";
                else
                    excep = "'HaghMaskan " + x.Message + "'";
                string commandText1 = "exec [Com].[spr_tblErrorInsert] 	0,'admin'," + excep + ",'" + d1.ToString() + "',1,1,''";
                var _Hokm1 = service.GetData(commandText1).Tables[0];
                return 0;
            }
        }
        public int ZaribTaadil(int TypeBime, string TarikhEjra, int AnvaEstekhdam, List<ClsHokmItems> HokmItems, bool HasZarib, int mashmol)
        {
            try
            {
                List<MoteghayerhayeHoghoghi> M_hoghugh = new List<MoteghayerhayeHoghoghi>();
                string commandText = "SELECT    TOP(1)    fldId, fldTarikhEjra, fldTarikhSodur, fldAnvaeEstekhdamId, fldTypeBimeId, fldZaribEzafeKar, fldSaatKari, fldDarsadBimePersonal, fldDarsadbimeKarfarma, fldDarsadBimeBikari, fldDarsadBimeJanbazan, " +
                  "fldHaghDarmanKarmand, fldHaghDarmanKarfarma, fldHaghDarmanDolat, fldHaghDarmanMazad, fldHaghDarmanTahteTakaffol, fldDarsadBimeMashagheleZiyanAvar, fldMaxHaghDarman, fldZaribHoghoghiSal,  " +
                  "fldHoghogh, fldFoghShoghl, fldTafavotTatbigh, fldFoghVizhe, fldHaghJazb, fldTadil, fldBarJastegi, fldSanavat,fldFoghTalash, fldUserId, fldDate, fldDesc " +
                  "FROM            Pay.tblMoteghayerhayeHoghoghi " +
                  "WHERE fldAnvaeEstekhdamId=" + AnvaEstekhdam + "AND fldTypeBimeId=" + TypeBime + "AND SUBSTRING(fldTarikhEjra,1,4)<='" + Convert.ToInt32(TarikhEjra.Substring(0, 4)) + "'" +
                  "ORDER BY fldTarikhSodur DESC,fldTarikhEjra DESC";
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                var tblMoteghayerhayeHoghoghi = service.GetData(commandText).Tables[0];
                MoteghayerhayeHoghoghi Moteghayer_H = new MoteghayerhayeHoghoghi();
                if (tblMoteghayerhayeHoghoghi.Rows.Count > 0)
                {
                    commandText = "SELECT  fldId, fldMoteghayerhayeHoghoghiId, fldItemEstekhdamId, fldUserId, fldDate, fldDesc " +
                      "from Pay.tblMoteghayerhayeHoghoghi_Detail " +
                      "WHERE fldMoteghayerhayeHoghoghiId=" + Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldId"]);
                    var MoteghayerhayeHoghoghi_Detail = service.GetData(commandText).Tables[0];
                    if (MoteghayerhayeHoghoghi_Detail.Rows.Count > 0)
                    {
                        for (int i = 0; i < MoteghayerhayeHoghoghi_Detail.Rows.Count; i++)
                        {
                            Moteghayer_H.Items.Add(new MoteghayerhayeHoghoghi_Detail
                            {
                                fldId = Convert.ToInt32(MoteghayerhayeHoghoghi_Detail.Rows[i]["fldId"]),
                                fldMoteghayerhayeHoghoghiId = Convert.ToInt32(MoteghayerhayeHoghoghi_Detail.Rows[i]["fldMoteghayerhayeHoghoghiId"])
         ,
                                fldItemEstekhdamId = Convert.ToInt32(MoteghayerhayeHoghoghi_Detail.Rows[i]["fldItemEstekhdamId"])
                            });
                        }
                    }
                    M_hoghugh.Add(Moteghayer_H);
                }
                //return M_hoghugh.OrderBy(l => l.fldTarikhEjra).FirstOrDefault();
                double Mablagh = 0.0, Mablagh1 = 0.0; int ZaribTadil = 0;
                double mashmol_bime = 0; /*=  hog.motalebat_m +hog.m_bime*/
                //decimal mashmol_bime = mohasebe.fldMashmolBime;
                foreach (var item in Moteghayer_H.Items)
                {
                    var Mohasebe_Item = HokmItems.Where(l => l.ItemEstekhdamId == item.fldItemEstekhdamId
                                                        && l.ItemEstekhdamId != 114 && l.ItemEstekhdamId != 115 && l.ItemEstekhdamId != 109 && l.ItemEstekhdamId != 110).FirstOrDefault();
                    if (Mohasebe_Item != null)
                        Mablagh += Mohasebe_Item.Mablagh;
                }
                if (HasZarib == false)
                {
                    mashmol_bime = Math.Round((Mablagh / 1000000), 2);
                    Mashmaul_Kosurat = Convert.ToInt32(Mablagh);
                    Mablagh1 = Math.Round((-0.22 * mashmol_bime + 11), 2);
                    if (Convert.ToInt32(TarikhEjra.Substring(0, 4)) == 1397)
                        ZaribTadil = Convert.ToInt32(Mablagh * Mablagh1 / 100);
                    Zarib2 = Mablagh1;
                }
                else
                {
                    Mashmaul_Kosurat = Convert.ToInt32(mashmol);
                    mashmol_bime = Math.Round((Convert.ToDouble(mashmol) / 1000000), 2);
                    Mablagh1 = Math.Round((-0.22 * mashmol_bime + 11), 2);
                    if (Convert.ToInt32(TarikhEjra.Substring(0, 4)) == 1397)
                        ZaribTadil = Convert.ToInt32(mashmol * Mablagh1 / 100);
                    Zarib2 = Mablagh1;
                }
                return ZaribTadil;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int ZaribHoghAvaliye(int TypeBime, string TarikhEjra, int AnvaEstekhdam, List<ClsHokmItems> HokmItems)
        {
            try
            {
                List<MoteghayerhayeHoghoghi> M_hoghugh = new List<MoteghayerhayeHoghoghi>();
                string commandText = "SELECT    TOP(1)    fldId, fldTarikhEjra, fldTarikhSodur, fldAnvaeEstekhdamId, fldTypeBimeId, fldZaribEzafeKar, fldSaatKari, fldDarsadBimePersonal, fldDarsadbimeKarfarma, fldDarsadBimeBikari, fldDarsadBimeJanbazan, " +
                  "fldHaghDarmanKarmand, fldHaghDarmanKarfarma, fldHaghDarmanDolat, fldHaghDarmanMazad, fldHaghDarmanTahteTakaffol, fldDarsadBimeMashagheleZiyanAvar, fldMaxHaghDarman, fldZaribHoghoghiSal,  " +
                  "fldHoghogh, fldFoghShoghl, fldTafavotTatbigh, fldFoghVizhe, fldHaghJazb, fldTadil, fldBarJastegi, fldSanavat,fldFoghTalash, fldUserId, fldDate, fldDesc " +
                  "FROM            Pay.tblMoteghayerhayeHoghoghi " +
                  "WHERE fldAnvaeEstekhdamId=" + AnvaEstekhdam + "AND fldTypeBimeId=" + TypeBime + "AND SUBSTRING(fldTarikhEjra,1,4)<='" + TarikhEjra + "'" +
                  "ORDER BY fldTarikhSodur DESC,fldTarikhEjra DESC";
                NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                var tblMoteghayerhayeHoghoghi = service.GetData(commandText).Tables[0];
                int ZaribHogh = 0;
                if (tblMoteghayerhayeHoghoghi.Rows.Count > 0)
                {
                    ZaribHogh = Convert.ToInt32(Convert.ToInt32(tblMoteghayerhayeHoghoghi.Rows[0]["fldZaribHoghoghiSal"]));
                }
                return ZaribHogh;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int JazbTakhasosi(int TedadFarzandan, double Zarib, string Tarikh_Ejra, int TaaholStatus, int Gender)
        {
            string commandText = "SELECT         fldHadaghalDaryafti " +
              "FROM            Prs.tblMoteghayerhaAhkam " +
              " WHERE fldYear=" + Tarikh_Ejra.Substring(0, 4) + " AND fldType=" + 1;
            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var tblJazbTakhasosi = service.GetData(commandText).Tables[0];
            int _JazbTakhasosi = 0;
            if (tblJazbTakhasosi.Rows.Count > 0)
            {
                _JazbTakhasosi = Convert.ToInt32(Convert.ToInt32(tblJazbTakhasosi.Rows[0]["fldHadaghalDaryafti"]) * Zarib / 100.0);
            }
            return _JazbTakhasosi;
        }
    }
}