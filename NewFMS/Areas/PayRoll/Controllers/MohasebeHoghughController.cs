using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Ext.Net.MVC;
using System.IO;
using System.Data;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Collections;
using System.Threading;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class MohasebeHoghughController : Controller
    {
        //
        // GET: /PayRoll/MohasebeHoghugh/

        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();

        WCF_Common_Pay.Comon_PayService servic_Com_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_Com_Pay = new WCF_Common_Pay.ClsError();

        WCF_PayRoll.PayRolService service_Pay = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError Err_Pay = new WCF_PayRoll.ClsError();
        // GET: PersonalHokm
        public ActionResult Index(string containerId)
        {//باز شدن تب جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            return new Ext.Net.MVC.PartialViewResult();
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult IndexNew(string containerId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.PersonalHokm();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }

        [DirectMethod]
        public ActionResult New( string containerId)
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            var year = Convert.ToInt32(NewFMS.Models.CurrentDate.Year);
            var month = Convert.ToInt32(NewFMS.Models.CurrentDate.Month);
            var nobatpardakht = Convert.ToInt32(NewFMS.Models.CurrentDate.nobatPardakht);            
            result.ViewBag.Year = year;
            result.ViewBag.Month = month;
            result.ViewBag.nobatpardakht = nobatpardakht;
            return result;
        }
        public ActionResult GetTypeHesab()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetHesabTypeWithFilter("hoghoogh", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult NewDel(string containerId)
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            var year = Convert.ToInt32(NewFMS.Models.CurrentDate.Year);
            var month = Convert.ToInt32(NewFMS.Models.CurrentDate.Month);
            var nobatpardakht = Convert.ToInt32(NewFMS.Models.CurrentDate.nobatPardakht);
            result.ViewBag.Year = year;
            result.ViewBag.Month = month;
            result.ViewBag.nobatpardakht = nobatpardakht;
            return result;
        }
        public ActionResult GetCostCenter(string OrganId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = service_Pay.GetCostCenterWithFilter("fldOrganId", OrganId, OrganId
              , 0, IP, out Err_Pay).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }

        public ActionResult GetTypeEstkhdam()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com_Pay.GetAnvaEstekhdamWithFilter("", "" , 0, IP, out Err_Com_Pay).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        
        public ActionResult CheckShomareHesabPasAndaz(short Year, byte Month, byte NobatPardakht, byte fldNoePardakht, int? fldPayPersonalId,byte? HesabTypeId,int OrganId)
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            byte Flag = 1;//اجازه محاسبه            
            string Msg = ""; string MsgTitle = ""; byte Er = 0;
            string FlagMsg = ""; string name = ""; string name2 = "",NameSh="";
            try
            {
                var p = service_Pay.CheckShomareHesabForMohasebat(Year.ToString(), Month.ToString(), NobatPardakht, OrganId, Convert.ToInt32(fldPayPersonalId), Convert.ToByte(HesabTypeId), IP, out Err_Pay).ToList();
                var q = service_Pay.CheckShomareHesabPasAndazForMohasebat(Year.ToString(), Month.ToString(), NobatPardakht, OrganId, Convert.ToInt32(fldPayPersonalId), IP, out Err_Pay).ToList();
                var pay = service_Pay.GetPayPersonalInfoWithFilter("CheckMohasebatShomareBime", Year.ToString(), OrganId, Month, IP, out Err_Pay).ToList();

                    foreach (var item in q)
                    {
                        name = name + item.fldName_Family + "، ";
                    }
                    foreach (var item in p)
                    {
                        name2 = name2 + item.fldName_Family + "، ";
                    }
                    foreach (var item in pay)
                    {
                        NameSh = NameSh + item.fldName + "، ";
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
                
                Flag = Flag,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                name = name,
                name2 = name2,
                NameSh = NameSh
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckStatusFlag(short Year, byte Month, byte NobatPardakht, byte fldNoePardakht, int? fldPayPersonalId,string MonthName,int OrganId)
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            byte Flag = 1;//اجازه محاسبه            
            string Msg = ""; string MsgTitle = ""; byte Er = 0;
            string FlagMsg = "";
            try
            {

                var personal = service_Pay.GetPayPersonalInfoDetail(Convert.ToInt32(fldPayPersonalId), OrganId, IP, out Err_Pay);
                //if (fldPayPersonalId==null)
                //{
                var karkard = service_Pay.GetKarKardeMahaneWithFilter("fldNobatePardakht_YM", NobatPardakht.ToString(), Year.ToString(), Month.ToString(), 0, 0, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                var EtelaatEzafekar = service_Pay.GetEzafeKari_TatilKariWithFilter("Mohasebe", "", 0, Year, Month, NobatPardakht, 1, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                var EtelaatTatilkar = service_Pay.GetEzafeKari_TatilKariWithFilter("Mohasebe", "", 0, Year, Month, NobatPardakht, 2, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                var EtelaatMorakhasi = service_Pay.GetMorakhasiWithFilter("Mohasebe", "", 0, Year, Month, NobatPardakht, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                var EtelaatMamooriat = service_Pay.GetMamuriyatWithFilter("Mohasebe", "", 0, Year, Month, NobatPardakht, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                var EtelaatEydi = service_Pay.GetEtelaatEydiWithFilter("Mohasebe", "", 0, Year, NobatPardakht, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                /*گزارش قبل از آپلود فایل مالیات*/
                var FieldName2 = "CheckMohasebat_Maliyat";
                if (fldPayPersonalId != 0 && fldPayPersonalId != null)
                {
                    FieldName2 = "CheckMohasebat_Maliyat_PersonalId";
                }
                var k = service_Pay.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), NobatPardakht.ToString(), OrganId, Convert.ToInt32(fldPayPersonalId), IP, out Err_Pay).FirstOrDefault();
                if (k != null)
                {
                    Flag = 0;

                }
                if (Flag != 0)
                {
                    if (karkard != null && karkard.fldFlag == true)
                    {
                        Flag = 3;//بسته شدن حقوق
                    }
                    else
                    {
                        if (fldNoePardakht == 1)
                        {
                            if (karkard == null)
                            {
                                Flag = 2;//عدم وجود اطلاعات
                                FlagMsg = "اطلاعات ماهانه ای برای محاسبه حقوق " + MyLib.Shamsi.ShamsiMonthname(Month) + " ماه وجود ندارد.";
                            }
                            else if (karkard != null && karkard.fldFlag == true)
                            {
                                Flag = 3;//بسته شدن حقوق
                            }

                        }
                        else if (fldNoePardakht == 2)
                        {
                            if (EtelaatEzafekar == null)
                            {
                                Flag = 2;//عدم وجود اطلاعات
                                FlagMsg = "اطلاعات ماهانه ای برای محاسبه اضافه کار " + MyLib.Shamsi.ShamsiMonthname(Month) + " ماه وجود ندارد.";
                            }
                            else if (EtelaatEzafekar != null && EtelaatEzafekar.fldFlag == true)
                            {
                                Flag = 3;//بسته شدن حقوق
                            }

                        }
                        else if (fldNoePardakht == 3)
                        {
                            if (EtelaatTatilkar == null)
                            {
                                Flag = 2;//عدم وجود اطلاعات
                                FlagMsg = "اطلاعات ماهانه ای برای محاسبه تعطیل کار " + MyLib.Shamsi.ShamsiMonthname(Month) + " ماه وجود ندارد.";
                            }
                            else if (EtelaatTatilkar != null && EtelaatTatilkar.fldFlag == true)
                            {
                                Flag = 3;//بسته شدن حقوق
                            }

                        }
                        else if (fldNoePardakht == 6)
                        {
                            if (EtelaatMorakhasi == null)
                            {
                                Flag = 2;//عدم وجود اطلاعات
                                FlagMsg = "اطلاعات ماهانه ای برای محاسبه مرخصی " + MyLib.Shamsi.ShamsiMonthname(Month) + " ماه وجود ندارد.";
                            }
                            else if (EtelaatMorakhasi != null && EtelaatMorakhasi.fldFlag == true)
                            {
                                Flag = 3;//بسته شدن حقوق
                            }

                        }
                        else if (fldNoePardakht == 5)
                        {
                            if (EtelaatMamooriat == null)
                            {
                                Flag = 2;//عدم وجود اطلاعات
                                FlagMsg = "اطلاعات ماهانه ای برای محاسبه ماموریت " + MyLib.Shamsi.ShamsiMonthname(Month) + " ماه وجود ندارد.";
                            }
                            else if (EtelaatMamooriat != null && EtelaatMamooriat.fldFlag == true)
                            {
                                Flag = 3;//بسته شدن حقوق
                            }

                        }
                        else if (fldNoePardakht == 4)
                        {
                            if (EtelaatEydi == null)
                            {
                                Flag = 2;//عدم وجود اطلاعات
                                FlagMsg = "اطلاعاتی برای محاسبه عیدی سال " + Year + "  وجود ندارد.";
                            }
                            else if (EtelaatEydi != null && EtelaatEydi.fldFlag == true)
                            {
                                Flag = 3;//بسته شدن حقوق
                            }

                        }
                        //}

                        //else if (fldPayPersonalId != null)
                        //{
                        //    var karkard = service_Pay.GetKarKardeMahaneWithFilter("fldNobatePardakht_YM_Id", NobatPardakht.ToString(), Year.ToString(), Month.ToString(), 0,Convert.ToInt32(fldPayPersonalId), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                        //    var EtelaatEzafekar = service_Pay.GetEzafeKari_TatilKariWithFilter("Mohasebe_Id", "", Convert.ToInt32(fldPayPersonalId), Year, Month, NobatPardakht, 1, Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                        //    var EtelaatTatilkar = service_Pay.GetEzafeKari_TatilKariWithFilter("Mohasebe_Id", "", Convert.ToInt32(fldPayPersonalId), Year, Month, NobatPardakht, 2, Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                        //    var EtelaatMorakhasi = service_Pay.GetMorakhasiWithFilter("Mohasebe_Id", "", Convert.ToInt32(fldPayPersonalId), Year, Month, NobatPardakht, Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                        //    var EtelaatMamooriat = service_Pay.GetMamuriyatWithFilter("Mohasebe_Id", "", Convert.ToInt32(fldPayPersonalId), Year, Month, NobatPardakht, Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                        //    var EtelaatEydi = service_Pay.GetEtelaatEydiWithFilter("Mohasebe_Id", "", Convert.ToInt32(fldPayPersonalId), Year, NobatPardakht, Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                        //    if (fldNoePardakht == 1)
                        //    {
                        //        if (karkard == null)
                        //        {
                        //            Flag = 2;//عدم وجود اطلاعات
                        //            FlagMsg = "اطلاعات ماهانه ای برای محاسبه حقوق " + MyLib.Shamsi.ShamsiMonthname(Month) + " ماه وجود ندارد.";
                        //        }
                        //        else if (karkard != null && karkard.fldFlag == true)
                        //        {
                        //            Flag = 3;//بسته شدن حقوق
                        //        }
                        //    }
                        //    else if (fldNoePardakht == 2)
                        //    {
                        //        if (EtelaatEzafekar == null)
                        //        {
                        //            Flag = 2;//عدم وجود اطلاعات
                        //            FlagMsg = "اطلاعات ماهانه ای برای محاسبه اضافه کار " + MyLib.Shamsi.ShamsiMonthname(Month) + " ماه وجود ندارد.";
                        //        }
                        //    }
                        //    else if (fldNoePardakht == 3)
                        //    {
                        //        if (EtelaatTatilkar == null)
                        //        {
                        //            Flag = 2;//عدم وجود اطلاعات
                        //            FlagMsg = "اطلاعات ماهانه ای برای محاسبه تعطیل کار " + MyLib.Shamsi.ShamsiMonthname(Month) + " ماه وجود ندارد.";
                        //        }
                        //    }
                        //    else if (fldNoePardakht == 6)
                        //    {
                        //        if (EtelaatMorakhasi == null)
                        //        {
                        //            Flag = 2;//عدم وجود اطلاعات
                        //            FlagMsg = "اطلاعات ماهانه ای برای محاسبه مرخصی " + MyLib.Shamsi.ShamsiMonthname(Month) + " ماه وجود ندارد.";
                        //        }
                        //    }
                        //    else if (fldNoePardakht == 5)
                        //    {
                        //        if (EtelaatMamooriat == null)
                        //        {
                        //            Flag = 2;//عدم وجود اطلاعات
                        //            FlagMsg = "اطلاعات ماهانه ای برای محاسبه ماموریت " + MyLib.Shamsi.ShamsiMonthname(Month) + " ماه وجود ندارد.";
                        //        }
                        //    }
                        //    else if (fldNoePardakht == 4)
                        //    {
                        //        if (EtelaatEydi == null)
                        //        {
                        //            Flag = 2;//عدم وجود اطلاعات
                        //            FlagMsg = "اطلاعاتی برای محاسبه عیدی سال " + Year + "  وجود ندارد.";
                        //        }
                        //    }
                        //}

                        if (Flag == 1)
                        {
                            var FieldName = "";
                            if (fldNoePardakht == 1 && fldPayPersonalId == null)
                            {
                                FieldName = "Hoghogh"; ;
                                Msg = "حقوق " + MonthName + " ماه سال " + Year + " پرسنل محاسبه شده است. آیا می خواهید حقوق دوباره محاسبه شود؟";
                            }
                            else if (fldNoePardakht == 2 && fldPayPersonalId == null)
                            {
                                FieldName = "EzafeKar";
                                Msg = "اضافه کاری " + MonthName + " ماه سال " + Year + " پرسنل محاسبه شده است. آیا می خواهید اضافه کار دوباره محاسبه شود؟";
                            }
                            else if (fldNoePardakht == 3 && fldPayPersonalId == null)
                            {
                                FieldName = "TatilKar";
                                Msg = "تعطیل کاری " + MonthName + " ماه سال " + Year + " پرسنل محاسبه شده است. آیا می خواهید تعطیل کاری دوباره محاسبه شود؟";
                            }
                            else if (fldNoePardakht == 6 && fldPayPersonalId == null)
                            {
                                FieldName = "Morakhasi";
                                Msg = "مرخصی " + MonthName + " ماه سال " + Year + " پرسنل محاسبه شده است. آیا می خواهید مرخصی دوباره محاسبه شود؟";
                            }
                            else if (fldNoePardakht == 5 && fldPayPersonalId == null)
                            {
                                FieldName = "Mamooriyat";
                                Msg = "ماموریت " + MonthName + " ماه سال " + Year + " پرسنل محاسبه شده است. آیا می خواهید ماموریت دوباره محاسبه شود؟";
                            }
                            else if (fldNoePardakht == 4 && fldPayPersonalId == null)
                            {
                                FieldName = "Eydi";
                                Msg = "عیدی " + MonthName + " سال " + " پرسنل محاسبه شده است. آیا می خواهید عیدی دوباره محاسبه شود؟";
                            }
                            if (fldNoePardakht == 1 && fldPayPersonalId != null)
                            {
                                FieldName = "Hoghogh_Personal";
                                Msg = "حقوق " + MonthName + " ماه سال " + Year + personal.fldName + " محاسبه شده است. آیا می خواهید حقوق دوباره محاسبه شود؟";
                            }
                            else if (fldNoePardakht == 2 && fldPayPersonalId != null)
                            {
                                FieldName = "EzafeKar_Personal";
                                Msg = "اضافه کاری " + MonthName + " ماه سال " + Year + personal.fldName + " محاسبه شده است. آیا می خواهید اضافه کاری دوباره محاسبه شود؟";
                            }
                            else if (fldNoePardakht == 3 && fldPayPersonalId != null)
                            {
                                FieldName = "TatilKar_Personal";
                                Msg = "تعطیل کاری " + MonthName + " ماه سال " + Year + personal.fldName + " محاسبه شده است. آیا می خواهید تعطیل کاری دوباره محاسبه شود؟";
                            }
                            else if (fldNoePardakht == 6 && fldPayPersonalId != null)
                            {
                                FieldName = "Morakhasi_Personal";
                                Msg = "مرخصی " + MonthName + " ماه سال " + Year + personal.fldName + " محاسبه شده است. آیا می خواهید مرخصی دوباره محاسبه شود؟";
                            }
                            else if (fldNoePardakht == 5 && fldPayPersonalId != null)
                            {
                                FieldName = "Mamooriyat_Personal";
                                Msg = "ماموریت " + MonthName + " ماه سال " + Year + personal.fldName + " محاسبه شده است. آیا می خواهید ماموریت دوباره محاسبه شود؟";
                            }
                            else if (fldNoePardakht == 4 && fldPayPersonalId != null)
                            {
                                FieldName = "Eydi_Personal";
                                Msg = "عیدی " + " سال " + Year + personal.fldName + " محاسبه شده است. آیا می خواهید عیدی دوباره محاسبه شود؟";
                            }
                            var q = service_Pay.CheckMohasebat(FieldName, Convert.ToInt32(fldPayPersonalId), Month, Year, NobatPardakht, OrganId, IP, out Err_Pay).FirstOrDefault();
                            if (q != null)
                            {
                                Flag = 4;
                            }
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
                MonthName = MyLib.Shamsi.ShamsiMonthname(Month),
                Flag=Flag,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                FlagMsg = FlagMsg
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckStatusFlagDel(short Year, byte Month, byte NobatPardakht, byte fldNoePardakht, int? fldPayPersonalId, string MonthName, int OrganId)
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            byte Flag = 1;//اجازه محاسبه            
            string Msg = ""; string MsgTitle = ""; byte Er = 0;
            string FlagMsg = "";
            try
            {

                var karkard = service_Pay.GetKarKardeMahaneWithFilter("fldNobatePardakht_YM", NobatPardakht.ToString(), Year.ToString(), Month.ToString(), 0, 0, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                var personal = service_Pay.GetPayPersonalInfoDetail(Convert.ToInt32(fldPayPersonalId), OrganId, IP, out Err_Pay);
                var EtelaatEzafekar = service_Pay.GetEzafeKari_TatilKariWithFilter("Mohasebe", "", 0, Year, Month, NobatPardakht, 1, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                var EtelaatTatilkar = service_Pay.GetEzafeKari_TatilKariWithFilter("Mohasebe", "", 0, Year, Month, NobatPardakht, 2, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                var EtelaatMorakhasi = service_Pay.GetMorakhasiWithFilter("Mohasebe", "", 0, Year, Month, NobatPardakht, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                var EtelaatMamooriat = service_Pay.GetMamuriyatWithFilter("Mohasebe", "", 0, Year, Month, NobatPardakht, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                var EtelaatEydi = service_Pay.GetEtelaatEydiWithFilter("Mohasebe", "", 0, Year, NobatPardakht, OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                /*گزارش قبل از آپلود فایل مالیات*/
                var FieldName2 = "CheckMohasebat_Maliyat";
                if (fldPayPersonalId != 0 && fldPayPersonalId != null)
                {
                    FieldName2 = "CheckMohasebat_Maliyat_PersonalId";
                }
                var k = service_Pay.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), NobatPardakht.ToString(), OrganId, Convert.ToInt32(fldPayPersonalId), IP, out Err_Pay).FirstOrDefault();
                if (k != null)
                {
                    Flag = 0;

                }
                if (Flag != 0)
                {
                    var FieldName = "";

                    if (karkard != null && karkard.fldFlag == true)
                    {
                        Flag = 3;//بسته شدن حقوق
                    }

                    else
                    {

                        if (fldNoePardakht == 1 && fldPayPersonalId == null && Flag != 3)
                        {
                            FieldName = "Hoghogh";
                            Msg = " آیا برای حذف محاسبات حقوق " + MonthName + " ماه سال " + Year + " مطمئن هسستید؟";
                        }

                        else if (fldNoePardakht == 2 && EtelaatEzafekar != null && EtelaatEzafekar.fldFlag == true)
                        {
                            Flag = 3;//بسته شدن حقوق
                        }
                        else if (fldNoePardakht == 2 && fldPayPersonalId == null)
                        {
                            FieldName = "EzafeKar";
                            Msg = " آیا برای حذف محاسبات اضافه کاری " + MonthName + " ماه سال " + Year + " مطمئن هسستید؟";
                        }

                        else if (fldNoePardakht == 3 && EtelaatTatilkar != null && EtelaatTatilkar.fldFlag == true)
                        {
                            Flag = 3;//بسته شدن حقوق
                        }
                        else if (fldNoePardakht == 3 && fldPayPersonalId == null)
                        {
                            FieldName = "TatilKar";
                            Msg = " آیا برای حذف محاسبات تعطیل کاری " + MonthName + " ماه سال " + Year + " مطمئن هسستید؟";
                        }
                        else if (fldNoePardakht == 6 && EtelaatMorakhasi != null && EtelaatMorakhasi.fldFlag == true)
                        {
                            Flag = 3;//بسته شدن حقوق
                        }
                        else if (fldNoePardakht == 6 && fldPayPersonalId == null)
                        {
                            FieldName = "Morakhasi";
                            Msg = " آیا برای حذف محاسبات مرخصی " + MonthName + " ماه سال " + Year + " مطمئن هسستید؟";
                        }
                        else if (fldNoePardakht == 5 && EtelaatMamooriat != null && EtelaatMamooriat.fldFlag == true)
                        {
                            Flag = 3;//بسته شدن حقوق
                        }
                        else if (fldNoePardakht == 5 && fldPayPersonalId == null)
                        {
                            FieldName = "Mamooriyat";
                            Msg = " آیا برای حذف محاسبات ماموریت " + MonthName + " ماه سال " + Year + " مطمئن هسستید؟";
                        }
                        else if (fldNoePardakht == 4 && EtelaatEydi != null && EtelaatEydi.fldFlag == true)
                        {
                            Flag = 3;//بسته شدن حقوق
                        }
                        else if (fldNoePardakht == 4 && fldPayPersonalId == null)
                        {
                            FieldName = "Eydi";
                            Msg = " آیا برای حذف محاسبات عیدی " + MonthName + " ماه سال " + Year + " مطمئن هسستید؟";
                        }
                        else if (fldNoePardakht == 1 && fldPayPersonalId != null && Flag != 3)
                        {
                            FieldName = "Hoghogh_Personal";
                            Msg = " آیا برای حذف محاسبات حقوق " + MonthName + " ماه سال " + Year + personal.fldName + " مطمئن هسستید؟";
                        }
                        else if (fldNoePardakht == 2 && fldPayPersonalId != null)
                        {
                            FieldName = "EzafeKar_Personal";
                            Msg = " آیا برای حذف محاسبات اضافه کاری " + MonthName + " ماه سال " + Year + personal.fldName + " مطمئن هسستید؟";
                        }
                        else if (fldNoePardakht == 3 && fldPayPersonalId != null)
                        {
                            FieldName = "TatilKar_Personal";
                            Msg = " آیا برای حذف محاسبات تعطیل کاری " + MonthName + " ماه سال " + Year + personal.fldName + " مطمئن هسستید؟";
                        }
                        else if (fldNoePardakht == 6 && fldPayPersonalId != null)
                        {
                            FieldName = "Morakhasi_Personal";
                            Msg = " آیا برای حذف محاسبات مرخصی " + MonthName + " ماه سال " + Year + personal.fldName + " مطمئن هسستید؟";
                        }
                        else if (fldNoePardakht == 5 && fldPayPersonalId != null)
                        {
                            FieldName = "Mamooriyat_Personal";
                            Msg = " آیا برای حذف محاسبات ماموریت " + MonthName + " ماه سال " + Year + personal.fldName + " مطمئن هسستید؟";
                        }
                        else if (fldNoePardakht == 4 && fldPayPersonalId != null)
                        {
                            FieldName = "Eydi_Personal";
                            Msg = " آیا برای حذف محاسبات عیدی " + MonthName + " ماه سال " + Year + personal.fldName + " مطمئن هسستید؟";
                        }
                        if (Flag != 0)
                        {
                            var q = service_Pay.CheckMohasebat(FieldName, Convert.ToInt32(fldPayPersonalId), Month, Year, NobatPardakht, OrganId, IP, out Err_Pay).FirstOrDefault();
                            if (q != null)
                            {
                                Flag = 4;
                            }
                            else
                            {
                                Flag = 5;
                            }
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
                MonthName = MyLib.Shamsi.ShamsiMonthname(Month),
                Flag = Flag,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                FlagMsg = FlagMsg
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCalc(short Year, byte Month, byte NobatPardakht, byte fldNoePardakht, int? fldPayPersonalId, string MonthName, string CostCenter, string AnvaeEstekhdam, int OrganId)
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            byte Flag = 1;//اجازه محاسبه            
            string Msg = "عملیات حذف با موفقیت انجام شد."; string MsgTitle = "حذف موفق"; byte Er = 0;
            string FlagMsg = "";
            try
            {
                var personal = service_Pay.GetPayPersonalInfoDetail(Convert.ToInt32(fldPayPersonalId), OrganId, IP, out Err_Pay);
                var FieldName = "";
               
                if (fldNoePardakht == 1 && fldPayPersonalId == null )
                {
                    FieldName = "Hoghogh"; 
                }
                if (fldNoePardakht == 2 && fldPayPersonalId == null)
                {
                    FieldName = "EzafeKar";
                }
                else if (fldNoePardakht == 3 && fldPayPersonalId == null)
                {
                    FieldName = "TatilKar";
                }
                else if (fldNoePardakht == 6 && fldPayPersonalId == null)
                {
                    FieldName = "Morakhasi";
                }
                else if (fldNoePardakht == 5 && fldPayPersonalId == null)
                {
                    FieldName = "Mamooriyat";
                }
                else if (fldNoePardakht == 4 && fldPayPersonalId == null)
                {
                    FieldName = "Eydi";
                }
                if (fldNoePardakht == 1 && fldPayPersonalId != null && Flag != 3)
                {
                    FieldName = "Hoghogh_Personal";
                }
                else if (fldNoePardakht == 2 && fldPayPersonalId != null)
                {
                    FieldName = "EzafeKar_Personal";
                }
                else if (fldNoePardakht == 3 && fldPayPersonalId != null)
                {
                    FieldName = "TatilKar_Personal";
                }
                else if (fldNoePardakht == 6 && fldPayPersonalId != null)
                {
                    FieldName = "Morakhasi_Personal";
                }
                else if (fldNoePardakht == 5 && fldPayPersonalId != null)
                {
                    FieldName = "Mamooriyat_Personal";
                }
                else if (fldNoePardakht == 4 && fldPayPersonalId != null)
                {
                    FieldName = "Eydi_Personal";
                }
                var q = service_Pay.MohasebatDelete(FieldName, Convert.ToInt32(fldPayPersonalId), Month, Year, NobatPardakht, CostCenter, AnvaeEstekhdam, Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId, out Err_Pay);
                if (Err_Pay.ErrorType)
                {
                    MsgTitle = "خطا";
                    Er = 1;
                    Msg = Err_Pay.ErrorMsg;
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
                MonthName = MyLib.Shamsi.ShamsiMonthname(Month),
                Flag = Flag,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                FlagMsg = FlagMsg
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSalVorod()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            //var q = m.sp_GetDate().FirstOrDefault();
            int fldsal = Convert.ToInt32(NewFMS.Models.CurrentDate.Year) - 7;
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
            var q = servic_Com.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }

        public JsonResult Formul(int? fldPayPersonalId, int OrganId, int fldYear, int fldMonth, byte fldNoePardakht
        , byte fldNobatePardakht, string fldTarikh, string fldMoavaghe, string fldMaliyat, string fldMagharari, string Type, string CostCenter, string AnvaeEstekhdam,byte HesabTypeId,byte fldMaliyatType)
        {
            string Msg = "", MsgTitle = ""; int Er = 0, id = 0; var MoavaghatId = 0;
            try
            {
                string BimeName = "";
                var bime = service_Pay.GetMaxBimeWithFilter("fldYear", fldYear.ToString(), 0, IP, out Err_Pay).FirstOrDefault();
                string Moavaghe = "0";
               
                if (fldMoavaghe == "1")
                    Moavaghe = "1";
                if (fldMaliyat == "1")
                    Moavaghe = "1";

                int j = 0;
                var days=30;
                byte Ezafe_Tatil = 1;
                if (fldNoePardakht == 3)
                    Ezafe_Tatil = 2;
                List<Models.ClsHokmItems> AllData = new List<Models.ClsHokmItems>();
                var isKabise=MyLib.Shamsi.Iskabise(fldYear);
            if (fldMonth<=6)
            {
                days=31;
            }
            else  if ( fldMonth>6 && fldMonth<12)
            {
                days=30;
            }
            else if (fldMonth == 12 && isKabise == true)
            {
                days = 30;
            }
            else if (fldMonth == 12 && isKabise == false)
            {
                days = 29;
            }
                          var Tarikh = fldYear + "/" + fldMonth.ToString().PadLeft(2, '0') + "/"+days;
                             var fldPrs_PersonalInfoId = 0;

                             var q = servic_Com_Pay.GetComputationFormulaWithFilter("fldAzTarikh", OrganId.ToString(), Type, Tarikh, OrganId, 0, IP, out Err_Com_Pay).FirstOrDefault();


                if (fldPayPersonalId != null)
                {
                    var PayPersonalInfo = service_Pay.GetPayPersonalInfoDetail(Convert.ToInt32(fldPayPersonalId), OrganId, IP, out Err_Pay);
                   fldPrs_PersonalInfoId = PayPersonalInfo.fldPrs_PersonalInfoId;
                   //var f = servic_Com_Pay.GetMaxPersonalEstekhdamTypeWithFilter("", fldPrs_PersonalInfoId, "", IP, out Err_Com_Pay).FirstOrDefault();
                   var f = servic_Com_Pay.GetMaxPersonalEstekhdamTypeWithFilter("Tarikh", fldPrs_PersonalInfoId, Tarikh, IP, out Err_Com_Pay).FirstOrDefault();
                   var AnvaeEstekhdamId = f.fldAnvaEstekhdamId; //10 نوع استخدام
                   

                   var z = servic_Com_Pay.GetAnvaEstekhdamDetail(AnvaeEstekhdamId, IP, out Err_Com_Pay);

                   var TypeEstekhdam = z.fldNoeEstekhdamId;//نوع 1=کارگر نوع 2=کارمند رسمی 3=کارمند پیمانی 4= شهردار
                   var Typebime=PayPersonalInfo.fldTypeBimeId;
                   string Formula = q.fldFormule;
                     string[] ReferenceSplit = q.fldLibrary.Split(';').Reverse().Skip(1).Reverse().ToArray();
                     var karkard = service_Pay.GetKarKardeMahaneWithFilter("fldNobatePardakht_YM_Id", fldNobatePardakht.ToString(), fldYear.ToString(), fldMonth.ToString(), fldNoePardakht, Convert.ToInt32(fldPayPersonalId), OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                     bool Moavagh = false; string azTarikh = "",TaTarikh="";
                     if (karkard!= null && karkard.fldMoavaghe == true)
                     {
                         Moavagh = Convert.ToBoolean(karkard.fldMoavaghe);
                         azTarikh = karkard.fldAzTarikhMoavaghe.ToString().Substring(0, 4) + "/" + karkard.fldAzTarikhMoavaghe.ToString().Substring(4);
                         TaTarikh = karkard.fldTaTarikhMoavaghe.ToString().Substring(0, 4) + "/" + karkard.fldTaTarikhMoavaghe.ToString().Substring(4);
                         
                     }
                     else
                     {
                         if(Moavaghe=="1")
                         Moavagh =true;
                         azTarikh = fldTarikh;
                         TaTarikh = fldYear.ToString() + "/" + fldMonth.ToString().PadLeft(2, '0'); ;
                     }

                     
                    ICodeCompiler loCompiler = new CSharpCodeProvider().CreateCompiler();
                     CompilerParameters loParameters = new CompilerParameters();

                     for (int i = 0; i < ReferenceSplit.Length; i++)
                     {
                         if (ReferenceSplit[i] == "System.dll" || ReferenceSplit[i] == "System.Data.dll" || ReferenceSplit[i] == "System.Xml.dll" || ReferenceSplit[i] == "System.Core.dll")
                         {
                             loParameters.ReferencedAssemblies.Add(ReferenceSplit[i]);
                         }
                         else
                         {
                             var Path = Server.MapPath(@"~\Reference\" + ReferenceSplit[i]);
                             loParameters.ReferencedAssemblies.Add(Path);
                         }
                     }

                     loParameters.GenerateInMemory = true;
                     CompilerResults loCompiled = loCompiler.CompileAssemblyFromSource(loParameters, Formula);

                     Assembly loAssembly = loCompiled.CompiledAssembly;
                     object loObject = loAssembly.CreateInstance("MyNamespace.MyClass");

                     object[] loCodeParms = new object[14];
                     loCodeParms[0] = fldPayPersonalId;
                     loCodeParms[1] = fldPrs_PersonalInfoId;
                     loCodeParms[2] = fldYear;
                     loCodeParms[3] = fldMonth;
                     //loCodeParms[4] = Moavaghe;
                     loCodeParms[4] = Moavagh;
                     loCodeParms[5] = fldMagharari;
                     loCodeParms[6] = fldNobatePardakht;
                     loCodeParms[7] = TypeEstekhdam;
                     loCodeParms[8] = Typebime;
                     loCodeParms[9] = AnvaeEstekhdamId;
                     //loCodeParms[10] = fldTarikh;
                     loCodeParms[10] = azTarikh;
                     loCodeParms[11] = fldNoePardakht;
                     loCodeParms[12] = TaTarikh;
                     loCodeParms[13] = bime.fldMablaghBime;
                     Models.Mohasebat.Mohasebat mohasebe = new Models.Mohasebat.Mohasebat();
                     object loResult;
                     if (fldNoePardakht != 1)
                     {
                         loResult = loObject.GetType().InvokeMember("DynamicCode", BindingFlags.InvokeMethod, null, loObject, loCodeParms);
                         mohasebe = (Models.Mohasebat.Mohasebat)loResult;
                     }

                     
                    if (fldNoePardakht == 1)
                     {
                         NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                         string commandText = "SELECT Pay.tblMaliyatManfi.fldId FROM Pay.tblMaliyatManfi INNER " +
                             "JOIN Pay.tblMohasebat ON Pay.tblMaliyatManfi.fldMohasebeId = " +
                             "Pay.tblMohasebat.fldId where Pay.tblMohasebat.fldPersonalId=" + fldPayPersonalId +
                             " and Pay.tblMohasebat.fldYear=" + fldYear + " and Pay.tblMohasebat.fldMonth=" + fldMonth;
                         var tblmaliatManfi = service.GetData(commandText).Tables[0];
                         if (tblmaliatManfi.Rows.Count > 0)
                         {
                             service_Pay.DeleteMaliyatManfi(Convert.ToInt32(tblmaliatManfi.Rows[0]["fldId"]),
                                 Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Pay);
                         }
                         commandText = "SELECT Pay.tblP_MaliyatManfi.fldId FROM Pay.tblP_MaliyatManfi INNER JOIN " +
                             "Pay.tblMohasebat ON Pay.tblP_MaliyatManfi.fldMohasebeId = Pay.tblMohasebat.fldId " +
                             "where Pay.tblMohasebat.fldPersonalId=" + fldPayPersonalId +
                              " and Pay.tblMohasebat.fldYear=" + fldYear + " and Pay.tblMohasebat.fldMonth=" + fldMonth;
                         var tblpmaliatManfi = service.GetData(commandText).Tables[0];
                         if (tblpmaliatManfi.Rows.Count > 0)
                         {
                             service_Pay.DeleteP_MaliyatManfi(Convert.ToInt32(tblpmaliatManfi.Rows[0]["fldId"]),
                                 Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Pay);
                         }                         

                         //service_Pay.MohasebatDelete("Hoghogh_Personal",Convert.ToInt32(fldPayPersonalId),Convert.ToByte(fldMonth),Convert.ToInt16(fldYear), fldNobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err_Pay);
                         DataSet.DataSet1TableAdapters.QueriesTableAdapter qd = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                         qd.spr_DeleteMohasebat("Hoghogh_Personal", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth), Convert.ToInt16(fldYear), fldNobatePardakht, OrganId, CostCenter, AnvaeEstekhdam);
                         loResult = loObject.GetType().InvokeMember("DynamicCode", BindingFlags.InvokeMethod, null, loObject, loCodeParms);
                         
                        mohasebe = (Models.Mohasebat.Mohasebat)loResult;

                        //NewFMS.Controllers.TestFormula.FormulHoghughController k = new NewFMS.Controllers.TestFormula.FormulHoghughController();
                        //mohasebe = k.Formulator((int)fldPayPersonalId, fldPrs_PersonalInfoId, fldYear, fldMonth, Convert.ToBoolean(Convert.ToInt32(fldMoavaghe)),
                        //   Convert.ToBoolean(Convert.ToInt32(fldMagharari)), fldNobatePardakht, TypeEstekhdam, Typebime, AnvaeEstekhdamId, fldTarikh, fldNoePardakht);


                         int maliat_moavaghe = 0, maliat_hoghugh = mohasebe.fldMaliyat;
                         var Id= service_Pay.InsertMohasebat(Convert.ToInt32(fldPayPersonalId), mohasebe.fldYear, mohasebe.fldMonth, mohasebe.fldKarkard, mohasebe.fldGheybat,
                         mohasebe.fldTedadEzafeKar, mohasebe.fldTedadTatilKar, mohasebe.fldBaBeytute, mohasebe.fldBedunBeytute, mohasebe.fldBimeOmrKarFarma,
                         mohasebe.fldBimeOmr, mohasebe.fldBimeTakmilyKarFarma, mohasebe.fldBimeTakmily, mohasebe.fldHaghDarmanKarfFarma,
                         mohasebe.fldHaghDarmanDolat, mohasebe.fldHaghDarman, mohasebe.fldBimePersonal, mohasebe.fldBimeKarFarma, mohasebe.fldBimeBikari,
                         mohasebe.fldBimeMashaghel, mohasebe.fldDarsadBimePersonal, mohasebe.fldDarsadBimeKarFarma, mohasebe.fldDarsadBimeBikari,
                         mohasebe.fldDarsadBimeSakht, mohasebe.fldTedadNobatKari, mohasebe.fldMosaede, mohasebe.fldNobatPardakht, mohasebe.fldGhestVam, mohasebe.fldPasAndaz,
                         mohasebe.fldMashmolBime,mohasebe.fldMashmolBimeNaKhales, mohasebe.fldMashmolMaliyat, mohasebe.fldFlag, mohasebe.fldkhalesPardakhti, mohasebe.fldMogharari, mohasebe.fldMaliyat,
                         mohasebe.fldFiscalHeaderId, mohasebe.fldMoteghayerHoghoghiId, mohasebe.fldHokmId, mohasebe.fldT_Asli, mohasebe.fldT_70, mohasebe.fldT_60, mohasebe.fldHamsareKarmand,
                         mohasebe.fldMazad30Sal, mohasebe.fldVamId, mohasebe.fldTedadBime1, mohasebe.fldTedadBime2, mohasebe.fldTedadBime3, HesabTypeId, fldMaliyatType, OrganId, mohasebe.fldShift, "",
                         Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                         if (Err_Pay.ErrorType)
                         {
                             MsgTitle = "خطا";
                             Msg = Err_Pay.ErrorMsg + "InsertMohasebat" + mohasebe.fldYear + mohasebe.fldMonth;
                             Er = 1;
                             return Json(new
                             {
                                 Er = Er,
                                 Msg = Msg,
                                 MsgTitle = MsgTitle
                             }, JsonRequestBehavior.AllowGet);
                         }
                         foreach (var item in mohasebe.Items)
                         {
                             service_Pay.InsertMohasebat_Items(Convert.ToInt32(Id), item.fldItemEstekhdamId, item.fldMablagh, "", Tarikh, AnvaeEstekhdamId, Typebime, item.fldSourceId, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                             if (Err_Pay.ErrorType)
                             {
                                 MsgTitle = "خطا";
                                 Msg = Err_Pay.ErrorMsg+"InsertMohasebat_Items";
                                 Er = 1;
                                 return Json(new
                                 {
                                     Er = Er,
                                     Msg = Msg,
                                     MsgTitle = MsgTitle
                                 }, JsonRequestBehavior.AllowGet);
                             }
                         }
                         var c = mohasebe.Kosorat_MotalebatParam.Count;
                         foreach (var item in mohasebe.Kosorat_MotalebatParam)
                         {
                             service_Pay.InsertMohasebat_kosorat_MotalebatParam(Convert.ToInt32(Id), item.fldKosoratId, item.fldMotalebatId, item.fldMablagh, "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                             if (Err_Pay.ErrorType)
                             {
                                 MsgTitle = "خطا";
                                 Msg = Err_Pay.ErrorMsg+"InsertMohasebat_Items";
                                 Er = 1;
                                 return Json(new
                                 {
                                     Er = Er,
                                     Msg = Msg,
                                     MsgTitle = MsgTitle
                                 }, JsonRequestBehavior.AllowGet);
                             }
                         }
                         var d = mohasebe.KosoratBank.Count;
                         foreach (var item in mohasebe.KosoratBank)
                         {
                             service_Pay.InsertMohasebat_KosoratBank(Convert.ToInt32(Id), item.fldKosoratBankId, item.fldMablagh, "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                             if (Err_Pay.ErrorType)
                             {
                                 MsgTitle = "خطا";
                                 Msg = Err_Pay.ErrorMsg+"InsertMohasebat_KosoratBank";
                                 Er = 1;
                                 return Json(new
                                 {
                                     Er = Er,
                                     Msg = Msg,
                                     MsgTitle = MsgTitle
                                 }, JsonRequestBehavior.AllowGet);
                             }
                         }
                         foreach (var item in mohasebe.Movaghat)
                         {
                             MoavaghatId=service_Pay.InsertMoavaghat(Convert.ToInt32(Id),Convert.ToInt16(item.fldYear),Convert.ToByte(item.fldMonth),item.fldHaghDarmanKarfFarma,item.fldHaghDarmanDolat,
                                 item.fldHaghDarman,item.fldBimePersonal,item.fldBimeKarFarma,item.fldBimeBikari,item.fldBimeMashaghel,item.fldPasAndaz,
                                 item.fldMashmolBime, item.fldMashmolBimeNaKhales, item.fldMashmolMaliyat, item.fldMaliyat, item.fldHokmId, "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                             maliat_moavaghe += item.fldMaliyat;
                             if (Err_Pay.ErrorType)
                             {
                                 MsgTitle = "خطا";
                                 Msg = Err_Pay.ErrorMsg + "InsertMoavaghat";
                                 Er = 1;
                                 return Json(new
                                 {
                                     Er = Er,
                                     Msg = Msg,
                                     MsgTitle = MsgTitle
                                 }, JsonRequestBehavior.AllowGet);
                             }
                             foreach (var _item in item.Items)
                             {
                                 service_Pay.InsertMoavaghat_Items(MoavaghatId, _item.fldItemEstekhdamId, _item.fldMablagh, "", Tarikh, AnvaeEstekhdamId, Typebime, _item.fldSourceId, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                                 if (Err_Pay.ErrorType)
                                 {
                                     MsgTitle = "خطا";
                                     Msg = Err_Pay.ErrorMsg + "InsertMoavaghat_Items";
                                     Er = 1;
                                     return Json(new
                                     {
                                         Er = Er,
                                         Msg = Msg,
                                         MsgTitle = MsgTitle
                                     }, JsonRequestBehavior.AllowGet);
                                 }
                             }
                         }
                         //if (maliat_moavaghe < 0)
                         //    service_Pay.InsertMaliyatManfi(maliat_moavaghe, Convert.ToInt32(Id), (short)fldYear,
                         //        (byte)fldMonth, "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Pay);
                         int maliatmanfi = 0, p_maliatmanfi = 0, tafazol_manfi = 0;

                         commandText = "SELECT ISNULL(SUM(Pay.tblMaliyatManfi.fldMablagh), 0) AS fldMablagh FROM Pay.tblMaliyatManfi INNER JOIN " +
                             "Pay.tblMohasebat ON Pay.tblMaliyatManfi.fldMohasebeId = Pay.tblMohasebat.fldId " +
                             "where Pay.tblMohasebat.fldPersonalId=" + fldPayPersonalId;
                         var maliat_Manfi = service.GetData(commandText).Tables[0];
                         if (maliat_Manfi.Rows.Count > 0)
                         {
                             maliatmanfi = Convert.ToInt32(maliat_Manfi.Rows[0]["fldMablagh"]);
                         }
                         commandText = "SELECT ISNULL(SUM(Pay.tblP_MaliyatManfi.fldMablagh), 0) AS fldMablagh FROM Pay.tblP_MaliyatManfi INNER JOIN " +
                             "Pay.tblMohasebat ON Pay.tblP_MaliyatManfi.fldMohasebeId = Pay.tblMohasebat.fldId " +
                             "where Pay.tblMohasebat.fldPersonalId=" + fldPayPersonalId;
                         var p_maliatManfi = service.GetData(commandText).Tables[0];
                         if (p_maliatManfi.Rows.Count > 0)
                         {
                             p_maliatmanfi = Convert.ToInt32(p_maliatManfi.Rows[0]["fldMablagh"]);
                         }
                         if (-(p_maliatmanfi) < -(maliatmanfi))
                         {
                             tafazol_manfi = ((-(maliatmanfi)) - (-(p_maliatmanfi)));
                             if (maliat_hoghugh < tafazol_manfi)
                                 tafazol_manfi = maliat_hoghugh;

                             //if (tafazol_manfi != 0)
                             //    service_Pay.InsertP_MaliyatManfi(Convert.ToInt32(Id), (-tafazol_manfi), (short)fldYear, (byte)fldMonth, ""
                             //        , Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Pay);

                         }
                         Msg = "محاسبه حقوق با موفقیت انجام شد.";
                         MsgTitle = "عملیات موفق";
                    
                         var g = mohasebe.Movaghat.Where(l => l.fldMashmolBimeNaKhales > 0).FirstOrDefault(); 
                        if ( mohasebe.fldMashmolBimeNaKhales >0 ||g!=null)
                         {
                             Msg = "مشمول بیمه شخص مورد نظر بیشتر از حداکثر مشمول کسر حق بیمه می باشد. لذا مازاد حق بیمه کسر نخواد شد.";
                             MsgTitle = "هشدار";
                             Er = 5;
                         }
                         
                     }
                     else if (fldNoePardakht == 2 || fldNoePardakht==3)
                     {
                         
                         int Mablagh=0,tedad=0;
                         if (fldNoePardakht == 2)
                         {
                             service_Pay.MohasebatDelete("EzafeKar_Personal", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth),
                                 Convert.ToInt16(fldYear), fldNobatePardakht, CostCenter, AnvaeEstekhdam, Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId, out Err_Pay);
                             tedad = mohasebe.fldTedadEzafeKar;
                             var EzafeKar = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 33).FirstOrDefault();
                             if (EzafeKar != null)
                                 Mablagh = EzafeKar.fldMablagh;
                         }
                         else if (fldNoePardakht == 3)
                         {
                             service_Pay.MohasebatDelete("TatilKar_Personal", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth),
                                 Convert.ToInt16(fldYear), fldNobatePardakht, CostCenter, AnvaeEstekhdam, Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId, out Err_Pay);
                             var TatilKari = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 35).FirstOrDefault();
                             if (TatilKari != null)
                                 Mablagh = TatilKari.fldMablagh;
                             tedad = mohasebe.fldTedadTatilKar;
                         }

                         service_Pay.InsertMohasebatEzafeKari_TatilKari(Convert.ToInt32(fldPayPersonalId), mohasebe.fldYear, mohasebe.fldMonth, tedad, Mablagh,
                         mohasebe.fldBimePersonal, mohasebe.fldBimeKarFarma, mohasebe.fldBimeBikari, mohasebe.fldBimeSakht, mohasebe.fldDarsadBimePersonal,
                         mohasebe.fldDarsadBimeKarFarma, mohasebe.fldDarsadBimeBikari, mohasebe.fldDarsadBimeSakht, mohasebe.fldMashmolBime,mohasebe.fldMashmolBimeNaKhales,
                         mohasebe.fldMashmolMaliyat, mohasebe.fldNobatPardakht, Ezafe_Tatil, mohasebe.fldkhalesPardakhti, mohasebe.fldMaliyat,
                         mohasebe.fldFiscalHeaderId, mohasebe.fldMoteghayerHoghoghiId, HesabTypeId, OrganId, "",
                         Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                         if (Err_Pay.ErrorType)
                         {
                             MsgTitle = "خطا";
                             Msg = Err_Pay.ErrorMsg + "InsertMohasebatEzafeKari_TatilKari";
                             Er = 1;
                             return Json(new
                             {
                                 Er = Er,
                                 Msg = Msg,
                                 MsgTitle = MsgTitle
                             }, JsonRequestBehavior.AllowGet);
                         }
                         if (fldNoePardakht == 2)
                             Msg = "محاسبه اضافه کاری با موفقیت انجام شد.";
                         else
                             Msg = "محاسبه تعطیل کاری با موفقیت انجام شد.";
                         MsgTitle = "عملیات موفق";
                         
                         if (mohasebe.fldMashmolBimeNaKhales > 0)
                         {
                             Msg = "مشمول بیمه شخص مورد نظر بیشتر از حداکثر مشمول کسر حق بیمه می باشد. لذا مازاد حق بیمه کسر نخواد شد.";
                             MsgTitle = "هشدار";
                             Er = 5;
                         }
                     }
                     else if (fldNoePardakht == 4 )
                     {
                         service_Pay.MohasebatDelete("Eydi_Personal", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth),
                             Convert.ToInt16(fldYear), fldNobatePardakht, CostCenter, AnvaeEstekhdam, Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId, out Err_Pay);
                         service_Pay.InsertMohasebat_Eydi(Convert.ToInt32(fldPayPersonalId), mohasebe.fldYear, mohasebe.fldMonth, mohasebe.fldDayCount, mohasebe.fldMablagh,
                         mohasebe.fldMaliyat, mohasebe.fldMablaghKosorat, mohasebe.fldkhalesPardakhti, mohasebe.fldNobatPardakht, HesabTypeId, OrganId, "",
                         Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                         if (Err_Pay.ErrorType)
                         {
                             MsgTitle = "خطا";
                             Msg = Err_Pay.ErrorMsg + "InsertMohasebat_Eydi";
                             Er = 1;
                             return Json(new
                             {
                                 Er = Er,
                                 Msg = Msg,
                                 MsgTitle = MsgTitle
                             }, JsonRequestBehavior.AllowGet);
                         }

                         Msg = "محاسبه عیدی با موفقیت انجام شد.";
                         MsgTitle = "عملیات موفق";
                     }
                     else if (fldNoePardakht == 5)
                     {
                         service_Pay.MohasebatDelete("Mamooriyat_Personal", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth),
                             Convert.ToInt16(fldYear), fldNobatePardakht, CostCenter, AnvaeEstekhdam, Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId, out Err_Pay);
                         int Mablagh = 0;

                         var Mamuriyat = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 34).FirstOrDefault();
                         if (Mamuriyat != null)
                             Mablagh = Mamuriyat.fldMablagh;

                         service_Pay.InsertMohasebat_Mamuriyat(Convert.ToInt32(fldPayPersonalId), mohasebe.fldYear, mohasebe.fldMonth, mohasebe.fldBaBeytute, mohasebe.fldBedunBeytute,
                         Mablagh, mohasebe.fldBimePersonal, mohasebe.fldBimeKarFarma, mohasebe.fldBimeBikari,mohasebe.fldBimeSakht,mohasebe.fldDarsadBimePersonal,
                         mohasebe.fldDarsadBimeKarFarma, mohasebe.fldDarsadBimeBikari, mohasebe.fldDarsadBimeSakht, mohasebe.fldMashmolBime, mohasebe.fldMashmolBimeNaKhales, mohasebe.fldMashmolMaliyat,
                         mohasebe.fldkhalesPardakhti, mohasebe.fldMaliyat, mohasebe.fldTashilat, Convert.ToByte(mohasebe.fldNobatPardakht), mohasebe.fldFiscalHeaderId, mohasebe.fldMoteghayerHoghoghiId, HesabTypeId,
                         OrganId, "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                         if (Err_Pay.ErrorType)
                         {
                             MsgTitle = "خطا";
                             Msg = Err_Pay.ErrorMsg + "InsertMohasebat_Mamuriyat";
                             Er = 1;
                             return Json(new
                             {
                                 Er = Er,
                                 Msg = Msg,
                                 MsgTitle = MsgTitle
                             }, JsonRequestBehavior.AllowGet);
                         }

                         Msg = "محاسبه ماموریت با موفقیت انجام شد.";
                         MsgTitle = "عملیات موفق";
                         if (mohasebe.fldMashmolBimeNaKhales > 0)
                         {
                             Msg = "مشمول بیمه شخص مورد نظر بیشتر از حداکثر مشمول کسر حق بیمه می باشد. لذا مازاد حق بیمه کسر نخواد شد.";
                             MsgTitle = "هشدار";
                             Er = 5;
                         }
                     }
                     else if (fldNoePardakht == 6)
                     {
                         service_Pay.MohasebatDelete("Morakhasi_Personal", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth),
                             Convert.ToInt16(fldYear), fldNobatePardakht, CostCenter, AnvaeEstekhdam, Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId, out Err_Pay);
                         service_Pay.InsertMohasebat_Morakhasi(Convert.ToInt32(fldPayPersonalId),Convert.ToByte(mohasebe.fldTedad), mohasebe.fldMablagh, mohasebe.fldMonth, mohasebe.fldYear
                            , Convert.ToByte(mohasebe.fldNobatPardakht), mohasebe.fldSalHokm, mohasebe.fldHokmId, HesabTypeId, OrganId, "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                         if (Err_Pay.ErrorType)
                         {
                             MsgTitle = "خطا" + "MohasebatDelete";
                             Msg = Err_Pay.ErrorMsg;
                             Er = 1;
                             return Json(new
                             {
                                 Er = Er,
                                 Msg = Msg,
                                 MsgTitle = MsgTitle
                             }, JsonRequestBehavior.AllowGet);
                         }
                        
                         Msg = "محاسبه مرخصی با موفقیت انجام شد.";
                         MsgTitle = "عملیات موفق";
                     }
                    Functions.SendProgress("محاسبه شده: ", 1, 1);
                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
                        }, JsonRequestBehavior.AllowGet);


                       
                    
                }
                else
                {
                    if (fldNoePardakht == 1)
                    {
                        NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                        string commandText = "SELECT Pay.tblMaliyatManfi.fldId FROM Pay.tblMaliyatManfi INNER " +
                            "JOIN Pay.tblMohasebat ON Pay.tblMaliyatManfi.fldMohasebeId = " +
                            "Pay.tblMohasebat.fldId where com.fn_organIdWithPayPersonal(Pay.tblMohasebat.fldPersonalId)=" +
                            OrganId.ToString() + " and Pay.tblMohasebat.fldYear=" + fldYear + 
                            " and Pay.tblMohasebat.fldMonth=" + fldMonth;
                        var tblmaliatManfi = service.GetData(commandText).Tables[0];
                        for (int i = 0; i < tblmaliatManfi.Rows.Count; i++)
                        {
                            service_Pay.DeleteMaliyatManfi(Convert.ToInt32(tblmaliatManfi.Rows[i]["fldId"]),
                                Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Pay);
                        }
                        commandText = "SELECT Pay.tblP_MaliyatManfi.fldId FROM Pay.tblP_MaliyatManfi INNER JOIN " +
                            "Pay.tblMohasebat ON Pay.tblP_MaliyatManfi.fldMohasebeId = Pay.tblMohasebat.fldId " +
                            "where com.fn_organIdWithPayPersonal(Pay.tblMohasebat.fldPersonalId)=" +
                            OrganId.ToString() + " and Pay.tblMohasebat.fldYear=" + fldYear + " and Pay.tblMohasebat.fldMonth=" + fldMonth;
                        var tblpmaliatManfi = service.GetData(commandText).Tables[0];
                        for (int i = 0; i < tblpmaliatManfi.Rows.Count; i++)
                        {
                            service_Pay.DeleteP_MaliyatManfi(Convert.ToInt32(tblpmaliatManfi.Rows[i]["fldId"]),
                                Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Pay);
                        }
                        //service_Pay.MohasebatDelete("Hoghogh", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth), Convert.ToInt16(fldYear), fldNobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Pay);
                        DataSet.DataSet1TableAdapters.QueriesTableAdapter qd = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                        qd.spr_DeleteMohasebat("Hoghogh", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth), Convert.ToInt16(fldYear), fldNobatePardakht, OrganId, CostCenter, AnvaeEstekhdam);
                        if (Err_Pay.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err_Pay.ErrorMsg + "spr_DeleteMohasebat";
                            Er = 1;
                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }

                       
                    }
                    else if (fldNoePardakht == 2)
                    {
                        //service_Pay.MohasebatDelete("EzafeKar", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth),
                        //    Convert.ToInt16(fldYear), fldNobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()), IP, Convert.ToInt32(Session["OrganId"]), out Err_Pay);
                        DataSet.DataSet1TableAdapters.QueriesTableAdapter qd = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                        qd.spr_DeleteMohasebat("EzafeKar", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth), Convert.ToInt16(fldYear), fldNobatePardakht, OrganId, CostCenter, AnvaeEstekhdam);
                        if (Err_Pay.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err_Pay.ErrorMsg + "spr_DeleteMohasebat";
                            Er = 1;
                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (fldNoePardakht == 3)
                    {
                        //service_Pay.MohasebatDelete("TatilKar", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth),
                        //    Convert.ToInt16(fldYear), fldNobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()), IP, Convert.ToInt32(Session["OrganId"]), out Err_Pay);
                        DataSet.DataSet1TableAdapters.QueriesTableAdapter qd = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                        qd.spr_DeleteMohasebat("TatilKar", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth), Convert.ToInt16(fldYear), fldNobatePardakht, OrganId, CostCenter, AnvaeEstekhdam);
                        if (Err_Pay.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err_Pay.ErrorMsg;
                            Er = 1;
                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else if (fldNoePardakht == 4)
                    {
                        //service_Pay.MohasebatDelete("Eydi", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth),
                        //    Convert.ToInt16(fldYear), fldNobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()), IP, Convert.ToInt32(Session["OrganId"]), out Err_Pay);
                        DataSet.DataSet1TableAdapters.QueriesTableAdapter qd = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                        qd.spr_DeleteMohasebat("Eydi", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth), Convert.ToInt16(fldYear), fldNobatePardakht, OrganId, CostCenter, AnvaeEstekhdam);
                        if (Err_Pay.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err_Pay.ErrorMsg;
                            Er = 1;
                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else if (fldNoePardakht == 5)
                    {
                        //service_Pay.MohasebatDelete("Mamooriyat", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth),
                        //    Convert.ToInt16(fldYear), fldNobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()), IP, Convert.ToInt32(Session["OrganId"]), out Err_Pay);
                        DataSet.DataSet1TableAdapters.QueriesTableAdapter qd = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                        qd.spr_DeleteMohasebat("Mamooriyat", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth), Convert.ToInt16(fldYear), fldNobatePardakht, OrganId, CostCenter, AnvaeEstekhdam);
                        if (Err_Pay.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err_Pay.ErrorMsg;
                            Er = 1;
                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }


                    }
                    else if (fldNoePardakht == 6)
                    {
                        //service_Pay.MohasebatDelete("Morakhasi", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth),
                        //    Convert.ToInt16(fldYear), fldNobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()), IP, Convert.ToInt32(Session["OrganId"]), out Err_Pay);
                        DataSet.DataSet1TableAdapters.QueriesTableAdapter qd = new DataSet.DataSet1TableAdapters.QueriesTableAdapter();
                        qd.spr_DeleteMohasebat("Morakhasi", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(fldMonth), Convert.ToInt16(fldYear), fldNobatePardakht, OrganId, CostCenter, AnvaeEstekhdam);
                        if (Err_Pay.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err_Pay.ErrorMsg;
                            Er = 1;
                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }


                    }
                    var Personal = service_Pay.GetPersonalInfo_MohasebeWithFilter("Calc", Convert.ToInt16(fldYear), Convert.ToByte(fldMonth), Convert.ToByte(fldNobatePardakht), Convert.ToByte(fldNoePardakht), Ezafe_Tatil, OrganId, CostCenter, AnvaeEstekhdam, Tarikh, IP, out Err_Pay).ToList();

                    string Formula = q.fldFormule;
                    string[] ReferenceSplit = q.fldLibrary.Split(';').Reverse().Skip(1).Reverse().ToArray();

                    ICodeCompiler loCompiler = new CSharpCodeProvider().CreateCompiler();
                    CompilerParameters loParameters = new CompilerParameters();

                    for (int i = 0; i < ReferenceSplit.Length; i++)
                    {
                        if (ReferenceSplit[i] == "System.dll" || ReferenceSplit[i] == "System.Data.dll" || ReferenceSplit[i] == "System.Xml.dll" || ReferenceSplit[i] == "System.Core.dll")
                        {
                            loParameters.ReferencedAssemblies.Add(ReferenceSplit[i]);
                        }
                        else
                        {
                            var Path = Server.MapPath(@"~\Reference\" + ReferenceSplit[i]);
                            loParameters.ReferencedAssemblies.Add(Path);
                        }
                    }

                    loParameters.GenerateInMemory = true;
                    CompilerResults loCompiled = loCompiler.CompileAssemblyFromSource(loParameters, Formula);

                    Assembly loAssembly = loCompiled.CompiledAssembly;
                    object loObject = loAssembly.CreateInstance("MyNamespace.MyClass");
                    int itemsCount = Personal.Count();
                    int ii = 1;
                    string manfi = "";
                    foreach (var item in Personal)
                    {          

                        id = item.PayId;
                        //var f = servic_Com_Pay.GetMaxPersonalEstekhdamTypeWithFilter("", item.PrsId, "", IP, out Err_Com_Pay).FirstOrDefault();
                        var f = servic_Com_Pay.GetMaxPersonalEstekhdamTypeWithFilter("Tarikh", item.PrsId, Tarikh, IP, out Err_Com_Pay).FirstOrDefault();
                        var AnvaeEstekhdamId = item.fldAnvaeEstekhdamId; /*f.fldAnvaEstekhdamId; *///10 نوع استخدام


                        var z = servic_Com_Pay.GetAnvaEstekhdamDetail(AnvaeEstekhdamId, IP, out Err_Com_Pay);

                        var TypeEstekhdam = z.fldNoeEstekhdamId;//نوع 1=کارگر نوع 2=کارمند رسمی 3=کارمند پیمانی 4= شهردار
                        var PayPersonalInfo = service_Pay.GetPayPersonalInfoDetail(Convert.ToInt32(item.PayId), OrganId, IP, out Err_Pay);
                        fldPrs_PersonalInfoId = PayPersonalInfo.fldPrs_PersonalInfoId;
                        var Typebime = PayPersonalInfo.fldTypeBimeId;
                        var karkard = service_Pay.GetKarKardeMahaneWithFilter("fldNobatePardakht_YM_Id", fldNobatePardakht.ToString(), fldYear.ToString(), fldMonth.ToString(), fldNoePardakht, Convert.ToInt32(item.PayId), OrganId, 1, IP, out Err_Pay).FirstOrDefault();
                        bool Moavagh = false; string azTarikh = "", TaTarikh = "";
                        if (karkard != null && karkard.fldMoavaghe == true)
                        {
                            Moavagh = Convert.ToBoolean(karkard.fldMoavaghe);
                            azTarikh = karkard.fldAzTarikhMoavaghe.ToString().Substring(0, 4) + "/" + karkard.fldAzTarikhMoavaghe.ToString().Substring(4);
                            TaTarikh = karkard.fldTaTarikhMoavaghe.ToString().Substring(0, 4) + "/" + karkard.fldTaTarikhMoavaghe.ToString().Substring(4);

                        }
                        else
                        {
                            if (Moavaghe == "1")
                                Moavagh = true;
                            azTarikh = fldTarikh;
                            TaTarikh = fldYear.ToString() + "/" + fldMonth.ToString().PadLeft(2, '0');
                        }
                        
                        object[] loCodeParms = new object[14];

                        loCodeParms[0] = item.PayId;
                        loCodeParms[1] = item.PrsId;
                        loCodeParms[2] = fldYear;
                        loCodeParms[3] = fldMonth;
                        //loCodeParms[4] = Moavaghe;
                        loCodeParms[4] = Moavagh;
                        loCodeParms[5] = fldMagharari;
                        loCodeParms[6] = fldNobatePardakht;
                        loCodeParms[7] = TypeEstekhdam;
                        loCodeParms[8] = Typebime;
                        loCodeParms[9] = AnvaeEstekhdamId;
                        //loCodeParms[10] = fldTarikh;
                        loCodeParms[10] = azTarikh;
                        loCodeParms[11] = fldNoePardakht;
                        loCodeParms[12] = TaTarikh;
                        loCodeParms[13] = bime.fldMablaghBime;
                        object loResult = loObject.GetType().InvokeMember("DynamicCode", BindingFlags.InvokeMethod, null, loObject, loCodeParms);
                        Models.Mohasebat.Mohasebat mohasebe = (Models.Mohasebat.Mohasebat)loResult;
                        if (fldNoePardakht == 1)
                        {
                            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
                            
                            int maliat_moavaghe = 0, maliat_hoghugh = mohasebe.fldMaliyat;
                            var Id=service_Pay.InsertMohasebat(PayPersonalInfo.fldId, mohasebe.fldYear, mohasebe.fldMonth, mohasebe.fldKarkard, mohasebe.fldGheybat,
                            mohasebe.fldTedadEzafeKar, mohasebe.fldTedadTatilKar, mohasebe.fldBaBeytute, mohasebe.fldBedunBeytute, mohasebe.fldBimeOmrKarFarma,
                            mohasebe.fldBimeOmr, mohasebe.fldBimeTakmilyKarFarma, mohasebe.fldBimeTakmily, mohasebe.fldHaghDarmanKarfFarma,
                            mohasebe.fldHaghDarmanDolat, mohasebe.fldHaghDarman, mohasebe.fldBimePersonal, mohasebe.fldBimeKarFarma, mohasebe.fldBimeBikari,
                            mohasebe.fldBimeMashaghel, mohasebe.fldDarsadBimePersonal, mohasebe.fldDarsadBimeKarFarma, mohasebe.fldDarsadBimeBikari,
                            mohasebe.fldDarsadBimeSakht, mohasebe.fldTedadNobatKari, mohasebe.fldMosaede, mohasebe.fldNobatPardakht, mohasebe.fldGhestVam, mohasebe.fldPasAndaz,
                            mohasebe.fldMashmolBime, mohasebe.fldMashmolBimeNaKhales, mohasebe.fldMashmolMaliyat, mohasebe.fldFlag, mohasebe.fldkhalesPardakhti, mohasebe.fldMogharari, mohasebe.fldMaliyat,
                            mohasebe.fldFiscalHeaderId, mohasebe.fldMoteghayerHoghoghiId, mohasebe.fldHokmId, mohasebe.fldT_Asli, mohasebe.fldT_70, mohasebe.fldT_60, mohasebe.fldHamsareKarmand,
                            mohasebe.fldMazad30Sal, mohasebe.fldVamId, mohasebe.fldTedadBime1, mohasebe.fldTedadBime2, mohasebe.fldTedadBime3, HesabTypeId,fldMaliyatType, OrganId, mohasebe.fldShift, "",
                            Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                            if (Err_Pay.ErrorType)
                            {
                                MsgTitle = "خطا";
                                Msg = Err_Pay.ErrorMsg + "InsertMohasebat";
                                Er = 1;
                                return Json(new
                                {
                                    Er = Er,
                                    Msg = Msg,
                                    MsgTitle = MsgTitle
                                }, JsonRequestBehavior.AllowGet);
                            }
                            foreach (var _item in mohasebe.Items)
                            {
                                service_Pay.InsertMohasebat_Items(Convert.ToInt32(Id), _item.fldItemEstekhdamId, _item.fldMablagh, "", Tarikh, AnvaeEstekhdamId, Typebime, _item.fldSourceId, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                                if (Err_Pay.ErrorType)
                                {
                                    MsgTitle = "خطا";
                                    Msg = Err_Pay.ErrorMsg + "InsertMohasebat_Items";
                                    Er = 1;
                                    return Json(new
                                    {
                                        Er = Er,
                                        Msg = Msg,
                                        MsgTitle = MsgTitle
                                    }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            foreach (var _item in mohasebe.Kosorat_MotalebatParam)
                            {
                                service_Pay.InsertMohasebat_kosorat_MotalebatParam(Convert.ToInt32(Id), _item.fldKosoratId, _item.fldMotalebatId, _item.fldMablagh, "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                                if (Err_Pay.ErrorType)
                                {
                                    MsgTitle = "خطا" + "InsertMohasebat_kosorat_MotalebatParam";
                                    Msg = Err_Pay.ErrorMsg;
                                    Er = 1;
                                    return Json(new
                                    {
                                        Er = Er,
                                        Msg = Msg,
                                        MsgTitle = MsgTitle
                                    }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            foreach (var _item in mohasebe.KosoratBank)
                            {
                                service_Pay.InsertMohasebat_KosoratBank(Convert.ToInt32(Id), _item.fldKosoratBankId, _item.fldMablagh, "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                                if (Err_Pay.ErrorType)
                                {
                                    MsgTitle = "خطا";
                                    Msg = Err_Pay.ErrorMsg + "InsertMohasebat_KosoratBank";
                                    Er = 1;
                                    return Json(new
                                    {
                                        Er = Er,
                                        Msg = Msg,
                                        MsgTitle = MsgTitle
                                    }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            foreach (var _item in mohasebe.Movaghat)
                            {
                                MoavaghatId = service_Pay.InsertMoavaghat(Convert.ToInt32(Id), Convert.ToInt16(_item.fldYear), Convert.ToByte(_item.fldMonth), _item.fldHaghDarmanKarfFarma, _item.fldHaghDarmanDolat,
                                    _item.fldHaghDarman, _item.fldBimePersonal, _item.fldBimeKarFarma, _item.fldBimeBikari, _item.fldBimeMashaghel, _item.fldPasAndaz,
                                    _item.fldMashmolBime, _item.fldMashmolBimeNaKhales, _item.fldMashmolMaliyat, _item.fldMaliyat, _item.fldHokmId, "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                                maliat_moavaghe += _item.fldMaliyat;
                                if (Err_Pay.ErrorType)
                                {
                                    MsgTitle = "خطا";
                                    Msg = Err_Pay.ErrorMsg + "InsertMoavaghat";
                                    Er = 1;
                                    return Json(new
                                    {
                                        Er = Er,
                                        Msg = Msg,
                                        MsgTitle = MsgTitle
                                    }, JsonRequestBehavior.AllowGet);
                                }
                                foreach (var itemMoavagh in _item.Items)
                                {
                                    service_Pay.InsertMoavaghat_Items(MoavaghatId, itemMoavagh.fldItemEstekhdamId, itemMoavagh.fldMablagh, "", Tarikh, AnvaeEstekhdamId, Typebime, itemMoavagh.fldSourceId, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                                    if (Err_Pay.ErrorType)
                                    {
                                        MsgTitle = "خطا";
                                        Msg = Err_Pay.ErrorMsg + "InsertMoavaghat_Items";
                                        Er = 1;
                                        return Json(new
                                        {
                                            Er = Er,
                                            Msg = Msg,
                                            MsgTitle = MsgTitle
                                        }, JsonRequestBehavior.AllowGet);
                                    }
                                }                               
                            }
                            Msg = "محاسبه حقوق با موفقیت انجام شد.";
                            MsgTitle = "عملیات موفق";
                            //if (maliat_moavaghe < 0)
                            //    service_Pay.InsertMaliyatManfi(maliat_moavaghe, Convert.ToInt32(Id), (short)fldYear,
                            //        (byte)fldMonth, "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Pay);
                            int maliatmanfi = 0, p_maliatmanfi = 0, tafazol_manfi = 0;

                            string commandText = "SELECT ISNULL(SUM(Pay.tblMaliyatManfi.fldMablagh), 0) AS fldMablagh FROM Pay.tblMaliyatManfi INNER JOIN " +
                                "Pay.tblMohasebat ON Pay.tblMaliyatManfi.fldMohasebeId = Pay.tblMohasebat.fldId " +
                                "where Pay.tblMohasebat.fldPersonalId=" + item.PayId;
                            var maliat_Manfi = service.GetData(commandText).Tables[0];
                            if (maliat_Manfi.Rows.Count > 0)
                            {
                                maliatmanfi = Convert.ToInt32(maliat_Manfi.Rows[0]["fldMablagh"]);
                            }
                            commandText = "SELECT ISNULL(SUM(Pay.tblP_MaliyatManfi.fldMablagh), 0) AS fldMablagh FROM Pay.tblP_MaliyatManfi INNER JOIN " +
                                "Pay.tblMohasebat ON Pay.tblP_MaliyatManfi.fldMohasebeId = Pay.tblMohasebat.fldId " +
                                "where Pay.tblMohasebat.fldPersonalId=" + item.PayId;
                            var p_maliatManfi = service.GetData(commandText).Tables[0];
                            if (p_maliatManfi.Rows.Count > 0)
                            {
                                p_maliatmanfi = Convert.ToInt32(p_maliatManfi.Rows[0]["fldMablagh"]);
                            }
                            if (-(p_maliatmanfi) < -(maliatmanfi))
                            {
                                tafazol_manfi = ((-(maliatmanfi)) - (-(p_maliatmanfi)));
                                if (maliat_hoghugh < tafazol_manfi)
                                    tafazol_manfi = maliat_hoghugh;
                                //if (tafazol_manfi!=0)
                                //service_Pay.InsertP_MaliyatManfi(Convert.ToInt32(Id), (-tafazol_manfi), (short)fldYear, (byte)fldMonth, ""
                                //    , Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Pay);

                            }
                            var qq = service_Pay.GetMohasebatWithFilter("fldid", Id.ToString(), "", "", OrganId, 0, IP, out Err_Pay).Where(o => o.fldkhalesPardakhti < 0).FirstOrDefault();
                            if (qq != null)
                            {
                                var prs = service_Pay.GetPayPersonalInfoWithFilter("fldid", qq.fldPersonalId.ToString(), OrganId, 0, IP, out Err_Pay).FirstOrDefault();
                                if (prs != null)
                                {
                                    manfi += "\n حقوق " + prs.fldName + "(" + prs.fldFatherName + ") منفی شده است.";
                                }
                            }
                            
                                 var g = mohasebe.Movaghat.Where(l => l.fldMashmolBimeNaKhales > 0).FirstOrDefault(); 
                        if ( mohasebe.fldMashmolBimeNaKhales >0 ||g!=null)
                         {
                             var prs = service_Pay.GetPayPersonalInfoWithFilter("fldid", PayPersonalInfo.fldId.ToString(), OrganId, 0, IP, out Err_Pay).FirstOrDefault();
                             if (prs != null)
                             {
                                 BimeName +=  prs.fldName + "(" + prs.fldFatherName +")";
                             }
                             Msg = "مشمول بیمه " + BimeName + "  بیشتر از حداکثر مشمول کسر حق بیمه می باشد. لذا مازاد حق بیمه کسر نخواد شد.";
                             MsgTitle = "هشدار";
                             Er = 5;
                         }
                           // nameBime=nameBime+","+ prs.fldName + "(" + prs.fldFatherName + ")"+;
                            
                        }
                        else if (fldNoePardakht == 2 || fldNoePardakht == 3)
                        {

                            int Mablagh = 0, tedad = 0;
                            if (fldNoePardakht == 2)
                            {
                                tedad = mohasebe.fldTedadEzafeKar;
                                var EzafeKar = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 33).FirstOrDefault();
                                if (EzafeKar != null)
                                    Mablagh = EzafeKar.fldMablagh;
                            }
                            else if (fldNoePardakht == 3)
                            {
                                var TatilKari = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 35).FirstOrDefault();
                                if (TatilKari != null)
                                    Mablagh = TatilKari.fldMablagh;
                                tedad = mohasebe.fldTedadTatilKar;
                            }

                            service_Pay.InsertMohasebatEzafeKari_TatilKari(PayPersonalInfo.fldId, mohasebe.fldYear, mohasebe.fldMonth, tedad, Mablagh,
                            mohasebe.fldBimePersonal, mohasebe.fldBimeKarFarma, mohasebe.fldBimeBikari, mohasebe.fldBimeSakht, mohasebe.fldDarsadBimePersonal,
                            mohasebe.fldDarsadBimeKarFarma, mohasebe.fldDarsadBimeBikari, mohasebe.fldDarsadBimeSakht, mohasebe.fldMashmolBime, mohasebe.fldMashmolBimeNaKhales,
                            mohasebe.fldMashmolMaliyat, mohasebe.fldNobatPardakht, Ezafe_Tatil, mohasebe.fldkhalesPardakhti, mohasebe.fldMaliyat,
                            mohasebe.fldFiscalHeaderId, mohasebe.fldMoteghayerHoghoghiId, HesabTypeId, OrganId, "",
                            Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                            if (Err_Pay.ErrorType)
                            {
                                MsgTitle = "خطا";
                                Msg = Err_Pay.ErrorMsg + "InsertMohasebatEzafeKari_TatilKari";
                                Er = 1;
                                return Json(new
                                {
                                    Er = Er,
                                    Msg = Msg,
                                    MsgTitle = MsgTitle
                                }, JsonRequestBehavior.AllowGet);
                            }
                            if (fldNoePardakht == 2)
                                Msg = "محاسبه اضافه کاری با موفقیت انجام شد.";
                            else
                                Msg = "محاسبه تعطیل کاری با موفقیت انجام شد.";
                            MsgTitle = "عملیات موفق";
                            if (mohasebe.fldMashmolBimeNaKhales > 0)
                            {
                                var prs = service_Pay.GetPayPersonalInfoWithFilter("fldid", PayPersonalInfo.fldId.ToString(), OrganId, 0, IP, out Err_Pay).FirstOrDefault();
                                if (prs != null)
                                {
                                    BimeName += prs.fldName + "(" + prs.fldFatherName + ")";
                                }
                                Msg = "مشمول بیمه " + BimeName + "  بیشتر از حداکثر مشمول کسر حق بیمه می باشد. لذا مازاد حق بیمه کسر نخواد شد.";
                                MsgTitle = "هشدار";
                                Er = 5;
                            }
                        }
                        else if (fldNoePardakht == 4)
                        {
                            service_Pay.InsertMohasebat_Eydi(PayPersonalInfo.fldId, mohasebe.fldYear, mohasebe.fldMonth, mohasebe.fldDayCount, mohasebe.fldMablagh,
                            mohasebe.fldMaliyat, mohasebe.fldMablaghKosorat, mohasebe.fldkhalesPardakhti, mohasebe.fldNobatPardakht, HesabTypeId, OrganId, "",
                            Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                            if (Err_Pay.ErrorType)
                            {
                                MsgTitle = "خطا";
                                Msg = Err_Pay.ErrorMsg;
                                Er = 1;
                                return Json(new
                                {
                                    Er = Er,
                                    Msg = Msg,
                                    MsgTitle = MsgTitle
                                }, JsonRequestBehavior.AllowGet);
                            }

                            Msg = "محاسبه عیدی با موفقیت انجام شد.";
                            MsgTitle = "عملیات موفق";
                        }
                        else if (fldNoePardakht == 5)
                        {
                            int Mablagh = 0;

                            var Mamuriyat = mohasebe.Items.Where(l => l.fldItemHoghoghiId == 34).FirstOrDefault();
                            if (Mamuriyat != null)
                                Mablagh = Mamuriyat.fldMablagh;

                            service_Pay.InsertMohasebat_Mamuriyat(PayPersonalInfo.fldId, mohasebe.fldYear, mohasebe.fldMonth, mohasebe.fldBaBeytute, mohasebe.fldBedunBeytute,
                            Mablagh, mohasebe.fldBimePersonal, mohasebe.fldBimeKarFarma, mohasebe.fldBimeBikari, mohasebe.fldBimeSakht, mohasebe.fldDarsadBimePersonal,
                            mohasebe.fldDarsadBimeKarFarma, mohasebe.fldDarsadBimeBikari, mohasebe.fldDarsadBimeSakht, mohasebe.fldMashmolBime, mohasebe.fldMashmolBimeNaKhales, mohasebe.fldMashmolMaliyat,
                            mohasebe.fldkhalesPardakhti, mohasebe.fldMaliyat, mohasebe.fldTashilat, Convert.ToByte(mohasebe.fldNobatPardakht), mohasebe.fldFiscalHeaderId, mohasebe.fldMoteghayerHoghoghiId, HesabTypeId,
                            OrganId, "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                            if (Err_Pay.ErrorType)
                            {
                                MsgTitle = "خطا";
                                Msg = Err_Pay.ErrorMsg;
                                Er = 1;
                                return Json(new
                                {
                                    Er = Er,
                                    Msg = Msg,
                                    MsgTitle = MsgTitle
                                }, JsonRequestBehavior.AllowGet);
                            }

                            Msg = "محاسبه ماموریت با موفقیت انجام شد.";
                            MsgTitle = "عملیات موفق";
                            if (mohasebe.fldMashmolBimeNaKhales > 0 )
                            {
                                var prs = service_Pay.GetPayPersonalInfoWithFilter("fldid", PayPersonalInfo.fldId.ToString(), OrganId, 0, IP, out Err_Pay).FirstOrDefault();
                                if (prs != null)
                                {
                                    BimeName += prs.fldName + "(" + prs.fldFatherName + ")";
                                }
                                Msg = "مشمول بیمه " + BimeName + "  بیشتر از حداکثر مشمول کسر حق بیمه می باشد. لذا مازاد حق بیمه کسر نخواد شد.";
                                MsgTitle = "هشدار";
                                Er = 5;
                            }
                        }
                        else if (fldNoePardakht == 6)
                        {
                            service_Pay.InsertMohasebat_Morakhasi(PayPersonalInfo.fldId, Convert.ToByte(mohasebe.fldTedad), mohasebe.fldMablagh, mohasebe.fldMonth, mohasebe.fldYear
                            , Convert.ToByte(mohasebe.fldNobatPardakht), mohasebe.fldSalHokm, mohasebe.fldHokmId, HesabTypeId, OrganId,
                            "", Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out Err_Pay);
                            if (Err_Pay.ErrorType)
                            {
                                MsgTitle = "خطا";
                                Msg = Err_Pay.ErrorMsg;
                                Er = 1;
                                return Json(new
                                {
                                    Er = Er,
                                    Msg = Msg,
                                    MsgTitle = MsgTitle
                                }, JsonRequestBehavior.AllowGet);
                            }

                            Msg = "محاسبه مرخصی با موفقیت انجام شد.";
                            MsgTitle = "عملیات موفق";
                        }
                       
                        Functions.SendProgress("محاسبه شده: ", ii++, itemsCount);
                    }
                    
                    return Json(new
                    {
                        Er = Er,
                        Msg = Msg + manfi,
                        MsgTitle = MsgTitle
                    }, JsonRequestBehavior.AllowGet);
                }
                //return Convert.ToInt32(loResult);
                //   return RedirectToAction("Formulator", "FormulHoghugh", new
                //{
                //    area = "",
                //    PayPersonalId = fldPayPersonalId,
                //    PrsPersonalId = fldPrs_PersonalInfoId,
                //    Year = fldYear,
                //    Month = fldMonth,
                //    Moavaghe = Convert.ToBoolean(Convert.ToInt32(fldMoavaghe)),
                //    Mogharari = Convert.ToBoolean(Convert.ToInt32(fldMagharari)),
                //    NobatPardakht = fldNobatePardakht,
                //    TypeEstekhdam = TypeEstekhdam,
                //    TypebimeId = Typebime,
                //    AnvaeEstekhdamId = AnvaeEstekhdamId
                //});
           
            }
            catch (Exception x)
            {
                if (Convert.ToInt32(Session["UserId"]) == 1)
                {
                    var m = x.Message.ToString();
                    if (x.InnerException != null)
                        m = x.InnerException.Message;
                    //X.Msg.Show(new MessageBoxConfig
                    //{
                    //    Buttons = MessageBox.Button.OK,
                    //    Icon = MessageBox.Icon.ERROR,
                    //    Title = "خطا",
                    //    Message = m + "Exception"+fldYear
                    //});
                    return Json(new
                    {
                        Er = Er,
                        Msg = m + "Exception" + fldYear,
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                   // DirectResult result = new DirectResult();
                    //return Json(new
                    //{
                    //    HoghughMabna = 1,
                    //    HavHMabna = 1
                    //}, JsonRequestBehavior.AllowGet);
                    //return null;
                }
                //return Json(new
                //{
                //    HoghughMabna = 1,
                //    HavHMabna = 1
                //}, JsonRequestBehavior.AllowGet);
                return null;
            }
        }

        
    }
}
