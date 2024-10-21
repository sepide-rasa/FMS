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

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class KarkardMahaneController : Controller
    {
        //
        // GET: /PayRoll/KarkardMahane/
        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();

        WCF_Common_Pay.Comon_PayService servic_Com_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError Err_Com_Pay = new WCF_Common_Pay.ClsError();

        WCF_PayRoll.PayRolService service_Pay = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError Err_Pay = new WCF_PayRoll.ClsError();
        public ActionResult Index(string containerId)
        {//باز شدن تب جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }

        public ActionResult IndexNew(string containerId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.KarkardMahane(); ;
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
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
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
        public ActionResult New(int PersonalId, int id, string containerId, byte DelCalc,int OrganId)
        {//باز شدن تب
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

           /* var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            //var q = servic_Com.GetDate(IP, out Err_Com);
            //var y = MyLib.Shamsi.Miladi2ShamsiString(q.fldDateTime);
            //var year = y.Substring(0, 4);
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();*/
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();

                var year = Convert.ToInt32(NewFMS.Models.CurrentDate.Year);
                var month = Convert.ToInt32(NewFMS.Models.CurrentDate.Month);
            
            if (id != 0)
            {
                var q = service_Pay.GetKarKardeMahaneDetail(id,OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                year = Convert.ToInt32(q.fldYear);
                month = Convert.ToInt32(q.fldMah);
            }
            var k = service_Pay.GetPayPersonalInfoDetail(PersonalId,OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
            var f = servic_Com_Pay.GetMaxPersonalEstekhdamTypeWithFilter("", k.fldPrs_PersonalInfoId, "", IP, out Err_Com_Pay).FirstOrDefault();
           var AnvaeEstekhdamId=f.fldAnvaEstekhdamId;


            var z = servic_Com_Pay.GetAnvaEstekhdamDetail(AnvaeEstekhdamId, IP, out Err_Com_Pay);
           
              var  TypeEstekhdam = z.fldNoeEstekhdamId;//نوع 1=کارگر نوع 2=کارمند رسمی 3=کارمند پیمانی 4= شهردار
          
            var isKabise=MyLib.Shamsi.Iskabise(year);
            var days=30;
            if (TypeEstekhdam==1 && month<=6)
            {
                days=31;
            }
            else  if (TypeEstekhdam==1 && month>6 && month<12)
            {
                days=30;
            }
            else if (TypeEstekhdam == 1 && month == 12 && isKabise == true)
            {
                days = 30;
            }
            else if (TypeEstekhdam == 1 && month == 12 && isKabise == false)
            {
                days = 29;
            }
            

            result.ViewBag.Id = id;
            result.ViewBag.PersonalId = PersonalId;
            result.ViewBag.Year = year;
            result.ViewBag.Month = month;
            result.ViewBag.days = days;
            result.ViewBag.TypeEstekhdam = TypeEstekhdam;
            result.ViewBag.DelCalc = DelCalc;
            result.ViewBag.OrganId = OrganId.ToString();

            return result;
        }

        public ActionResult CheckCloseHoghough(short Year, byte Month,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; byte Er = 0; bool FlagC = true;//حقوق بسته شده است
            try
            {
                var q = service_Pay.SelectFlagKarKard_Mohasebe(Year, Month, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay)
                    .Where(l => l.fldFlag == true).ToList();
                if (q.Count > 0)
                {

                    FlagC = false;
                }
            }
            catch (Exception x)
            {
               if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                FlagC = FlagC
            }, JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult CheckStatusFlagEdit(short Year, byte Month, byte NobatPardakht, byte fldNoePardakht, int? fldPayPersonalId, string MonthName,int OrganId)
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            byte Flag = 1;//اجازه محاسبه            
            string Msg = ""; string MsgTitle = ""; byte Er = 0;
            string FlagMsg = "";
            try
            {

                var karkard = service_Pay.GetKarKardeMahaneWithFilter("fldNobatePardakht_YM", NobatPardakht.ToString(), Year.ToString(), Month.ToString(), 0, 0,OrganId/* Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err_Pay).FirstOrDefault();
                var personal = service_Pay.GetPayPersonalInfoDetail(Convert.ToInt32(fldPayPersonalId),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                var FieldName = "";
                
                    if (karkard != null && karkard.fldFlag == true)
                    {
                        Flag = 3;//بسته شدن حقوق
                    }
                //}
                else
                {
                    if (fldNoePardakht == 1 && fldPayPersonalId == null && Flag != 3)
                    {
                        FieldName = "Hoghogh"; ;
                        Msg = " حقوق " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات حقوق حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 2 && fldPayPersonalId == null)
                    {
                        FieldName = "EzafeKar";
                        Msg = " اضافه کاری " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات اضافه کاری حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 3 && fldPayPersonalId == null)
                    {
                        FieldName = "TatilKar";
                        Msg = " تعطیل کاری " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات ، محاسبات تعطیل کاری حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 6 && fldPayPersonalId == null)
                    {
                        FieldName = "Morakhasi";
                        Msg = " مرخصی " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات مرخصی حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 5 && fldPayPersonalId == null)
                    {
                        FieldName = "Mamooriyat";
                        Msg = " ماموریت " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات ماموریت حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 4 && fldPayPersonalId == null)
                    {
                        FieldName = "Eydi";
                        Msg = " عیدی " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات عیدی حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 1 && fldPayPersonalId != null && Flag != 3)
                    {
                        FieldName = "Hoghogh_Personal";
                        Msg = " حقوق " + MonthName + " ماه سال " + Year + personal.fldName + "  محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات حقوق حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 2 && fldPayPersonalId != null)
                    {
                        FieldName = "EzafeKar_Personal";
                        Msg = " اضافه کاری " + MonthName + " ماه سال " + Year + personal.fldName + " محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات اضافه کاری حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 3 && fldPayPersonalId != null)
                    {
                        FieldName = "TatilKar_Personal";
                        Msg = " تعطیل کاری " + MonthName + " ماه سال " + Year + personal.fldName + "محاسبه شده است. در صورت ویرایش اطلاعات کارکرد ماهانه، محاسبات تعطیل کاری حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 6 && fldPayPersonalId != null)
                    {
                        FieldName = "Morakhasi_Personal";
                        Msg = " مرخصی " + MonthName + " ماه سال " + Year + personal.fldName + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات مرخصی حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 5 && fldPayPersonalId != null)
                    {
                        FieldName = "Mamooriyat_Personal";
                        Msg = " ماموریت " + MonthName + " ماه سال " + Year + personal.fldName + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات ماموریت حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 4 && fldPayPersonalId != null)
                    {
                        FieldName = "Eydi_Personal";
                        Msg = " عیدی " + MonthName + " ماه سال " + Year + personal.fldName + "محاسبه شده است. در صورت ویرایش اطلاعات، محاسبات عیدی حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    var q = service_Pay.CheckMohasebat(FieldName, Convert.ToInt32(fldPayPersonalId), Convert.ToByte(Month), Convert.ToInt16(Year), Convert.ToByte(NobatPardakht), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay).FirstOrDefault();
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
                MonthName = MonthName,//MyLib.Shamsi.ShamsiMonthname(Month),
                Flag = Flag,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                FlagMsg = FlagMsg
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckStatusFlagDel(short Year, byte Month, byte NobatPardakht, byte fldNoePardakht, int? fldPayPersonalId, string MonthName,int OrganId)
        {//باز شدن تب
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            byte Flag = 1;//اجازه محاسبه            
            string Msg = ""; string MsgTitle = ""; byte Er = 0;
            string FlagMsg = "";
            try
            {

                var karkard = service_Pay.GetKarKardeMahaneWithFilter("fldNobatePardakht_YM", NobatPardakht.ToString(), Year.ToString(), Month.ToString(), 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err_Pay).FirstOrDefault();
                var personal = service_Pay.GetPayPersonalInfoDetail(Convert.ToInt32(fldPayPersonalId),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                var FieldName = "";
                //if (fldNoePardakht == 1)
                //{
                if (karkard != null && karkard.fldFlag == true)
                {
                    Flag = 3;//بسته شدن حقوق
                }
                else
                {
                    //}
                    if (fldNoePardakht == 1 && fldPayPersonalId == null && Flag != 3)
                    {
                        FieldName = "Hoghogh"; ;
                        Msg = " حقوق " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت حذف اطلاعات، محاسبات حقوق حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 2 && fldPayPersonalId == null)
                    {
                        FieldName = "EzafeKar";
                        Msg = " اضافه کاری " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت حذف اطلاعات، محاسبات اضافه کاری حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 3 && fldPayPersonalId == null)
                    {
                        FieldName = "TatilKar";
                        Msg = " تعطیل کاری " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت حذف اطلاعات، محاسبات تعطیل کاری حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 6 && fldPayPersonalId == null)
                    {
                        FieldName = "Morakhasi";
                        Msg = " مرخصی " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت حذف اطلاعات، محاسبات مرخصی حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 5 && fldPayPersonalId == null)
                    {
                        FieldName = "Mamooriyat";
                        Msg = " ماموریت " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت حذف اطلاعات، محاسبات ماموریت حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 4 && fldPayPersonalId == null)
                    {
                        FieldName = "Eydi";
                        Msg = " عیدی " + MonthName + " ماه سال " + Year + "محاسبه شده است. در صورت حذف اطلاعات، محاسبات عیدی حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 1 && fldPayPersonalId != null && Flag != 3)
                    {
                        FieldName = "Hoghogh_Personal";
                        Msg = " حقوق " + MonthName + " ماه سال " + Year + personal.fldName + "  محاسبه شده است. در صورت حذف اطلاعات، محاسبات حقوق حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 2 && fldPayPersonalId != null)
                    {
                        FieldName = "EzafeKar_Personal";
                        Msg = " اضافه کاری " + MonthName + " ماه سال " + Year + personal.fldName + " محاسبه شده است. در صورت حذف اطلاعات، محاسبات اضافه کاری حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 3 && fldPayPersonalId != null)
                    {
                        FieldName = "TatilKar_Personal";
                        Msg = " تعطیل کاری " + MonthName + " ماه سال " + Year + personal.fldName + "محاسبه شده است. در صورت حذف اطلاعات، محاسبات تعطیل کاری حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 6 && fldPayPersonalId != null)
                    {
                        FieldName = "Morakhasi_Personal";
                        Msg = " مرخصی " + MonthName + " ماه سال " + Year + personal.fldName + "محاسبه شده است. در صورت حذف اطلاعات، محاسبات مرخصی حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 5 && fldPayPersonalId != null)
                    {
                        FieldName = "Mamooriyat_Personal";
                        Msg = " ماموریت " + MonthName + " ماه سال " + Year + personal.fldName + "محاسبه شده است. در صورت حذف اطلاعات، محاسبات ماموریت حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    else if (fldNoePardakht == 4 && fldPayPersonalId != null)
                    {
                        FieldName = "Eydi_Personal";
                        Msg = " عیدی " + MonthName + " ماه سال " + Year + personal.fldName + "محاسبه شده است. در صورت حذف اطلاعات، محاسبات عیدی حذف خواهد گردید. " + " آیا مایل به ادامه عملیات هسستید؟";
                    }
                    var q = service_Pay.CheckMohasebat(FieldName, Convert.ToInt32(fldPayPersonalId), Convert.ToByte(Month), Convert.ToInt16(Year), Convert.ToByte(NobatPardakht), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay).FirstOrDefault();
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
                MonthName = MonthName,//MyLib.Shamsi.ShamsiMonthname(Month),
                Flag = Flag,
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
                FlagMsg = FlagMsg
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(WCF_PayRoll.OBJ_KarKardeMahane karkard, bool Flag, byte? DelCalc,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                if (karkard.fldDesc == null)
                    karkard.fldDesc = "";
                if (karkard.fldId == 0)
                { //ذخیره
                    var q = service_Pay.GetKarKardeMahaneWithFilter("CheckSave", karkard.fldPersonalId.ToString(), karkard.fldYear.ToString(), karkard.fldMah.ToString(),
                        karkard.fldNobatePardakht, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err_Pay).FirstOrDefault();
                    if (Flag == true)
                    {
                        Msg = service_Pay.UpdateFlag("Mohasebat_Sal", karkard.fldYear, karkard.fldMah, karkard.fldNobatePardakht,0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, Convert.ToByte(3), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_Pay);
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
                    if (q != null)
                    {
                        Msg = "کارکرد ماهانه شخص مورد نظر قبلا ثبت شده است.";
                        MsgTitle = "خطا";
                    }
                    else
                    {
                        service_Pay.InsertKarKardeMahane(karkard.fldPersonalId, karkard.fldYear, karkard.fldMah, karkard.fldKarkard, karkard.fldGheybat, karkard.fldNobateKari,
                        karkard.fldEzafeKari, karkard.fldTatileKari, karkard.fldMamoriatBaBeitote, karkard.fldMamoriatBedoneBeitote, karkard.fldMosaedeh, karkard.fldNobatePardakht, karkard.fldFlag, karkard.fldGhati,
                        karkard.fldBa10, karkard.fldBa20, karkard.fldBa30, karkard.fldBe10, karkard.fldBe20, karkard.fldBe30, karkard.fldShift, Convert.ToBoolean(karkard.fldMoavaghe), karkard.fldAzTarikhMoavaghe, karkard.fldTaTarikhMoavaghe, karkard.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
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
                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";
                    }
                }
                else
                { //ویرایش
                    var q = service_Pay.GetKarKardeMahaneDetail(karkard.fldId, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                    if (q.fldFlag == true)
                    {
                        Msg = " حقوق ماه جاری بسته شده است و شما مجاز به تغییر اطلاعات نمی باشید.";
                        MsgTitle = "اخطار";
                    }
                    else
                    {
                        var p = service_Pay.GetKarKardeMahaneWithFilter("CheckEdit", karkard.fldPersonalId.ToString(), karkard.fldYear.ToString(), karkard.fldMah.ToString(), karkard.fldNobatePardakht, karkard.fldId, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 1, IP, out Err_Pay).FirstOrDefault();
                        if (p != null)
                        {
                            Msg = " اطلاعات کارکرد ماهانه شخص مورد نظر تکراری است.";
                            MsgTitle = "اخطار";
                        }
                        else
                        {
                            if (DelCalc == 1)
                            {
                                service_Pay.MohasebatDelete("AllMohasebat_Personal_sal", Convert.ToInt32(karkard.fldPersonalId), Convert.ToByte(karkard.fldMah), Convert.ToInt16(karkard.fldYear), Convert.ToByte(karkard.fldNobateKari), "", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId/*Convert.ToInt32(Session["OrganId"])*/, out Err_Pay);
                                if (Err_Pay.ErrorType == true)
                                {
                                    Msg = Err_Pay.ErrorMsg;
                                    MsgTitle = "خطا";
                                    Er = 1;

                                    return Json(new
                                    {
                                        Er = Er,
                                        Msg = Msg,
                                        MsgTitle = MsgTitle
                                    }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            service_Pay.UpdateKarKardeMahane(karkard.fldId, karkard.fldPersonalId, karkard.fldYear, karkard.fldMah, karkard.fldKarkard, karkard.fldGheybat, karkard.fldNobateKari,
                                karkard.fldEzafeKari, karkard.fldTatileKari, karkard.fldMamoriatBaBeitote, karkard.fldMamoriatBedoneBeitote, karkard.fldMosaedeh, karkard.fldNobatePardakht, q.fldFlag, q.fldGhati,
                                karkard.fldBa10, karkard.fldBa20, karkard.fldBa30, karkard.fldBe10, karkard.fldBe20, karkard.fldBe30, karkard.fldShift, Convert.ToBoolean(karkard.fldMoavaghe), karkard.fldAzTarikhMoavaghe, karkard.fldTaTarikhMoavaghe, karkard.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
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
                            Msg = "ویرایش با موفقیت انجام شد.";
                            MsgTitle = "ویرایش موفق";
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Delete(int id, short? Year, byte? Month, byte? NobatPardakht, byte? fldNoePardakht, int? fldPayPersonalId, byte? DelCalc,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                var q = service_Pay.GetKarKardeMahaneDetail(id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
                if (q.fldFlag == true)
                {
                    Msg = "شما مجاز به حذف نیستید..";
                    MsgTitle = "اخطار";
                }
                else
                {
                    if (DelCalc == 1)
                    {
                        service_Pay.MohasebatDelete("AllMohasebat_Personal_sal", Convert.ToInt32(fldPayPersonalId), Convert.ToByte(Month), Convert.ToInt16(Year), Convert.ToByte(NobatPardakht), "", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, OrganId/*Convert.ToInt32(Session["OrganId"])*/, out Err_Pay);
                        if (Err_Pay.ErrorType == true)
                        {
                            Msg = Err_Pay.ErrorMsg;
                            MsgTitle = "خطا";
                            Er = 1;

                            return Json(new
                            {
                                Er = Er,
                                Msg = Msg,
                                MsgTitle = MsgTitle
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    service_Pay.DeleteKarKardeMahane(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
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
                    Msg = "حذف با موفقیت انجام شد.";
                    MsgTitle = "حذف موفق";
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
          
            var IsKargar = 0;
            var q = service_Pay.GetKarKardeMahaneDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
            var q2 = service_Pay.GetPayPersonalInfoDetail(q.fldPersonalId, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
            //var H = p.sp_Com_MaxPersonalEstekhdamType(q2.fldPersonalInfoKargoziniId).FirstOrDefault();
            //if (H.fldTypeEstekhdamId == 1)
            //    IsKargar = 1;
            string Sal = "",Month="",TaSal="",TaMonth="";
       
        if (q.fldMoavaghe == true)
        {
             Sal = q.fldAzTarikhMoavaghe.ToString().Substring(0, 4);
                Month = q.fldAzTarikhMoavaghe.ToString().Substring(4);
                TaSal = q.fldTaTarikhMoavaghe.ToString().Substring(0, 4);
                TaMonth = q.fldTaTarikhMoavaghe.ToString().Substring(4);
        }
            return Json(new
            {
                fldId = q.fldId,
                fldPersonalId = q.fldPersonalId,
                fldName = q.fldName_Father,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                fldYear = q.fldYear.ToString(),
                fldMah = q.fldMah.ToString(),
                fldKarkard = q.fldKarkard,
                fldGheybat = q.fldGheybat,
                fldNobateKari = q.fldNobateKari,
                fldEzafeKari = q.fldEzafeKari,
                fldTatileKari = q.fldTatileKari,
                fldMamoriatBaBeitote = q.fldMamoriatBaBeitote,
                fldMamoriatBedoneBeitote = q.fldMamoriatBedoneBeitote,
                fldMosaedeh = q.fldMosaedeh,
                fldNobatePardakht = q.fldNobatePardakht.ToString(),
                fldFlag = q.fldFlag,
                fldGhati = q.fldGhati,
                fldBa10 = q.fldBa10,
                fldBa20 = q.fldBa20,
                fldBa30 = q.fldBa30,
                fldBe10 = q.fldBe10,
                fldBe20 = q.fldBe20,
                fldBe30 = q.fldBe30,
                IsKargar = IsKargar,
                fldDesc = q.fldDesc,
                fldShift=q.fldShift,
                fldMoavaghe = q.fldMoavaghe,
                Sal = Sal,
                Month = Month,
                TaSal = TaSal,
                TaMonth = TaMonth,
                fldMoavagheName = q.fldMoavagheName
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailsHeader(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var IsKargar = 0;
            var q = service_Pay.GetPayPersonalInfoDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err_Pay);
            var H = servic_Com_Pay.GetMaxPersonalEstekhdamTypeWithFilter("", q.fldPrs_PersonalInfoId, "", IP, out Err_Com_Pay).FirstOrDefault();
            if (H.fldNoeEstekhdamId == 1)
                IsKargar = 1;
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName_Father,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                IsKargar = IsKargar,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ReadHeader(StoreRequestParameters parameters,int OrganId)
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
                        data1 = service_Pay.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
                    else
                        data = service_Pay.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = service_Pay.GetPayPersonalInfoWithFilter("", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
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
        public ActionResult ReadKarKardeMahane(StoreRequestParameters parameters, int PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_KarKardeMahane> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_KarKardeMahane> data1 = null;
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
                        case "fldMonth":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Month";
                            break;
                        case "fldKarkard":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Karkard";
                            break;
                        case "fldGheybat":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Gheybat";
                            break;
                        case "fldNobateKari":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_NobateKari";
                            break;
                        case "fldEzafeKari":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_EzafeKari";
                            break;
                        case "fldTatileKari":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_TatileKari";
                            break;
                        case "fldNobatePardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_NobatePardakht";
                            break;
                        case "fldMosaedeh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_Mosaedeh";
                            break;
                        case "fldMamoriatBaBeitote":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_MamoriatBaBeitote";
                            break;
                        case "fldMamoriatBedoneBeitote":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPersonalId_MamoriatBedoneBeitote";
                            break;
                    }
                    if (data != null)
                        data1 = service_Pay.GetKarKardeMahaneWithFilter(field, searchtext, "", "", 0, PersonalId, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
                    else
                        data = service_Pay.GetKarKardeMahaneWithFilter(field, searchtext, "", "", 0, PersonalId, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = service_Pay.GetKarKardeMahaneWithFilter("fldPersonalId", PersonalId.ToString(), "", "", 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err_Pay).ToList();
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

            List<WCF_PayRoll.OBJ_KarKardeMahane> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Reload(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_KarKardeMahane> data = null;

            data = service_Pay.GetKarKardeMahaneWithFilter("fldPersonalId", PersonalId.ToString(), "", "", 0, 0, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err_Pay).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
