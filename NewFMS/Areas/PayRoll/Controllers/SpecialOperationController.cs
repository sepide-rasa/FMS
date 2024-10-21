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
using System.Text;
using System.Net;
using NewFMS.Helper;
using System.Xml;
using System.Reflection;
using System.Web.UI;
using System.Configuration;
using System.Data;
using NewFMS.Models;
using System.IO;
using NewFMS.Models;
using Aspose.Cells;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using ClosedXML.Excel;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class SpecialOperationController : Controller
    {
        //
        // GET: /PayRoll/SpecialOperation/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();

        WCF_Personeli.PersoneliService PersoneliService = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError ErrPersoneli = new WCF_Personeli.ClsError();

        WCF_Common_Pay.Comon_PayService Comon_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError ErrC_P = new WCF_Common_Pay.ClsError();
        public ActionResult Index(string containerId, string State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var setting = PayServic.GetSettingWithFilter("fldOrganId", Session["OrganId"].ToString(),0, 1, IP, out ErrPay).FirstOrDefault();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            result.ViewBag.State = State;
            result.ViewBag.BankFixId = setting.fldH_BankFixId;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult CheckDiskMaliat( short Year,byte Month,byte? NobatPardakht,int type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var FieldName = "CheckMaliyat";
            if (NobatPardakht==null)
            {
                FieldName = "CheckMaliyat_variz";
                NobatPardakht = 0;
            }
            var k = PayServic.GetMohasebatWithFilter(FieldName, Year.ToString(), Month.ToString(), NobatPardakht.ToString(), Convert.ToInt32(Session["OrganId"]),type, IP, out ErrPay);

            return Json(new
            {
                AllowGenerate =k.Count() > 0 ? 0 : 1
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DiskMaliyat(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult DiskPasandaz(string containerId, string State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var setting = PayServic.GetSettingWithFilter("fldOrganId", Session["OrganId"].ToString(),0, 1, IP, out ErrPay).FirstOrDefault();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            var k=PayServic.GetPasAndazBank(Convert.ToInt16(NewFMS.Models.CurrentDate.Year),
                Convert.ToByte(NewFMS.Models.CurrentDate.Month), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
            if (k == null)
            {
                k = 0;
            }
            result.ViewBag.State = State;
            result.ViewBag.BankId = k.ToString();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult HelpDiscKosourat()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpMaliyatDisc()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpBankDisc()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpDiskBazneshastegi()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpDiskBimeKhadamatDarmani()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpDiskPasandaz()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult HelpDiskBime()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult ReloadBank(byte Month, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var k = PayServic.GetPasAndazBank(Year, Month, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
            return Json(new
            {
                BankId=k.ToString()
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBank()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetBankWithFilter("", "", 0,IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldBankName });
            return this.Store(q);
        }

      

        public ActionResult DiskBime()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            return result;
        }

        public ActionResult DiskBimeKhadamatDarmani()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            return result;
        }

        public ActionResult DiskKosouratShahr()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            return result;
        }        
        public FileResult CreateExcel(string Checked, short Year, byte Month, byte Nobat)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
            int index = 0;
            var StatusCheck = Checked.Split(';');
            var Check = "";

            var fldName = ""; var fldFamily = ""; var fldMelli_EconomicCode = ""; var fldKarkard = ""; var tabaee1 = "";
            var tabaee2 = ""; var tabaee3 = ""; var fldMoavaghe = "";
            var fldDateEjra = ""; var fldHagheDarman = ""; var fldMashmool = ""; var fldShenase = "";

            var data = PayServic.GetDisketBimeKhadamatDarmani(Year, Month, Nobat, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();

            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];

            foreach (var item in StatusCheck)
            {
                switch (item)
                {
                    case "fldMelli_EconomicCode":
                        Check = "کد ملی";
                        Cell cell22 = sheet.Cells[alpha[index] + "1"];
                        cell22.PutValue(Check);
                        int k3 = 0;
                        foreach (var _item in data)
                        {
                            fldMelli_EconomicCode = _item.fldCodeMeli;
                            Cell Cell = sheet.Cells[alpha[index] + (k3 + 2)];
                            Cell.PutValue(fldMelli_EconomicCode);
                            k3++;
                        }
                        index++;
                        break;
                    case "fldMashmool":
                        Check = "حقوق مشمول";
                        Cell cell6 = sheet.Cells[alpha[index] + "1"];
                        cell6.PutValue(Check);
                        int x = 0;
                        foreach (var _item in data)
                        {
                            fldMashmool = _item.fldMashmolBime.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
                            Cell.PutValue(fldMashmool);
                            x++;
                        }
                        index++;
                        break;
                    case "fldHagheDarman":
                        Check = "حق بیمه";
                        Cell cell5 = sheet.Cells[alpha[index] + "1"];
                        cell5.PutValue(Check);
                        int z = 0;
                        foreach (var _item in data)
                        {
                            fldHagheDarman = _item.fldHaghDarman.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                            Cell.PutValue(fldHagheDarman);
                            z++;
                        }
                        index++;
                        break;
                    case "tabaee1":
                        Check = "تعداد تبعی 1";
                        Cell cell1 = sheet.Cells[alpha[index] + "1"];
                        cell1.PutValue(Check);
                        int j2 = 0;
                        foreach (var _item in data)
                        {
                            tabaee1 = (_item.fldTedadBime1 - 1).ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (j2 + 2)];
                            Cell.PutValue(tabaee1);
                            j2++;
                        }
                        index++;
                        break;
                    case "tabaee2":
                        Check = "تعداد تبعی 2";
                        Cell cell2 = sheet.Cells[alpha[index] + "1"];
                        cell2.PutValue(Check);
                        int k = 0;
                        foreach (var _item in data)
                        {
                            tabaee2 = _item.fldTedadBime2.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                            Cell.PutValue(tabaee2);
                            k++;
                        }
                        index++;
                        break;
                    case "tabaee3":
                        Check = "تعداد تبعی 3";
                        Cell cell3 = sheet.Cells[alpha[index] + "1"];
                        cell3.PutValue(Check);
                        int q = 0;
                        foreach (var _item in data)
                        {
                            tabaee3 = _item.fldTedadBime3.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                            Cell.PutValue(tabaee3);
                            q++;
                        }
                        index++;
                        break;
                    case "fldDateEjra":
                        Check = "تاریخ شروع بکار";
                        Cell cell4 = sheet.Cells[alpha[index] + "1"];
                        cell4.PutValue(Check);
                        int w = 0;
                        foreach (var _item in data)
                        {
                            fldDateEjra = _item.fldTarikhEjra;
                            Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                            Cell.PutValue(fldDateEjra);
                            w++;
                        }
                        index++;
                        break;
                    case "fldName":
                        Check = "نام";
                        Cell cell = sheet.Cells[alpha[index] + "1"];
                        cell.PutValue(Check);
                        int i = 0;
                        foreach (var _item in data)
                        {
                            fldName = _item.fldName;
                            Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                            Cell.PutValue(fldName);
                            i++;
                        }
                        index++;
                        break;
                    case "fldFamily":
                        Check = "نام خانوادگی";
                        Cell cell12 = sheet.Cells[alpha[index] + "1"];
                        cell12.PutValue(Check);
                        int j = 0;
                        foreach (var _item in data)
                        {
                            fldFamily = _item.fldFamily;
                            Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                            Cell.PutValue(fldFamily);
                            j++;
                        }
                        index++;
                        break;
                    case "fldShenase":
                        Check = "شناسه عادی / ایثارگر";
                        Cell cell7 = sheet.Cells[alpha[index] + "1"];
                        cell7.PutValue(Check);
                        int y = 0;
                        foreach (var _item in data)
                        {
                            fldShenase = "";
                            Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
                            Cell.PutValue(fldShenase);
                            y++;
                        }
                        index++;
                        break;
                    case "fldKarkard":
                        Check = "تاریخ پایان کار";
                        Cell cell23 = sheet.Cells[alpha[index] + "1"];
                        cell23.PutValue(Check);
                        int k2 = 0;
                        foreach (var _item in data)
                        {
                            fldKarkard = "";// _item.fldKarkard.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (k2 + 2)];
                            Cell.PutValue(fldKarkard);
                            k2++;
                        }
                        index++;
                        break;
                    case "fldMoavaghe":
                        Check = "حقوق معوقه مشمول";
                        Cell cell24 = sheet.Cells[alpha[index] + "1"];
                        cell24.PutValue(Check);
                        int k4 = 0;
                        foreach (var _item in data)
                        {
                            fldMoavaghe = _item.fldMoavaghe.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (k4 + 2)];
                            Cell.PutValue(fldMoavaghe);
                            k4++;
                        }
                        index++;
                        break;
                }
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
            var FileName = "حق درمان " + MonthName + " ماه " +"سال " +Year.ToString()+".xls";
            return File(stream.ToArray(), "xls", FileName);
        }
        public FileResult CreateExcelKosouratShahr(string Checked, short Year, byte Month,string BankId)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
            int index = 0;
            var StatusCheck = Checked.Split(';');
            var Check = "";

            var fldName = ""; var fldFamily = ""; var fldMelli_EconomicCode = ""; var fldMonth = ""; var fldMablagh = ""; var fldTozih = ""; var fldTarikhPardakht = "";

            var data = PayServic.GetKosuratBankWithFilter("Disket", Year.ToString(), Month.ToString(),BankId, Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();

            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];
            sheet.Name = "Page 1";

            foreach (var item in StatusCheck)
            {
                switch (item)
                {
                    case "fldName":
                        Check = "نام";
                        Cell cell = sheet.Cells[alpha[index] + "1"];
                        cell.PutValue(Check);
                        int x = 0;
                        foreach (var _item in data)
                        {
                            fldName = _item.fldName;
                            Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
                            Cell.PutValue(fldName);
                            x++;
                        }
                        index++;
                        break;
                    case "fldFamily":
                        Check = "نام خانوادگی";
                        Cell cell1 = sheet.Cells[alpha[index] + "1"];
                        cell1.PutValue(Check);
                        int y = 0;
                        foreach (var _item in data)
                        {
                            fldFamily = _item.fldFamily;
                            Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
                            Cell.PutValue(fldFamily);
                            y++;
                        }
                        index++;
                        break;
                    case "fldMelli_EconomicCode":
                        Check = "کد ملی";
                        Cell cell2 = sheet.Cells[alpha[index] + "1"];
                        cell2.PutValue(Check);
                        int z = 0;
                        foreach (var _item in data)
                        {
                            fldMelli_EconomicCode = _item.fldCodemeli;
                            Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                            Cell.PutValue(fldMelli_EconomicCode);
                            z++;
                        }
                        index++;
                        break;
                    case "fldTarikhPardakht":
                        Check = "تاریخ پرداخت";
                        Cell cell3 = sheet.Cells[alpha[index] + "1"];
                        cell3.PutValue(Check);
                        int w = 0;
                        foreach (var _item in data)
                        {
                            fldTarikhPardakht = _item.fldTarikhPardakht;
                            Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                            Cell.PutValue(fldTarikhPardakht);
                            w++;
                        }
                        index++;
                        break;
                    case "fldMablagh":
                        Check = "مبلغ";
                        Cell cell4 = sheet.Cells[alpha[index] + "1"];
                        cell4.PutValue(Check);
                        int k = 0;
                        foreach (var _item in data)
                        {
                            fldMablagh = _item.fldMablagh.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                            Cell.PutValue(fldMablagh);
                            k++;
                        }
                        index++;
                        break;
                    case "fldTozih":
                        Check = "شرح";
                        Cell cell5 = sheet.Cells[alpha[index] + "1"];
                        cell5.PutValue(Check);
                        int q = 0;
                        foreach (var _item in data)
                        {
                            fldTozih = _item.fldDesc;
                            Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                            Cell.PutValue(fldTozih);
                            q++;
                        }
                        index++;
                        break;
                }
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
            var FileName = "کسورات بانک " + MonthName + " ماه " + "سال " + Year.ToString() + ".xls";
            return File(stream.ToArray(), "xls", FileName);
        }

        public ActionResult GetKargah()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetInsuranceWorkshopWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldWorkShopName, fldNum = c.fldWorkShopNum });
            return this.Store(q);
        }


        public ActionResult DiskBazneshastegi()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            return result;
        }

        public ActionResult Disc(short Year,byte TypePardakht, byte Month, byte NobatePardakht, byte MarhalePardakht,int BankId,string FieldName,int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; byte Er = 0; var FileName = ""; var Passvand = ""; string savePath = "";
            int tedad = 0; var marhale = ""; var organName = ""; string tarikh = ""; var code_shobe = ""; var code_sazeman = "";
            long? jam = 0; var Name_shobe = ""; var DisketId = 0;

            try
            {
                var q = PayServic.GetSelectVariziForBank(FieldName,"", Year, Month, NobatePardakht, MarhalePardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
                organName = servic.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).fldName;
                var setting = PayServic.GetSettingWithFilter("fldOrganId", Session["OrganId"].ToString(),0, 1, IP, out ErrPay).FirstOrDefault();
                code_sazeman = setting.fldH_CodeOrgan;
                code_shobe = setting.fldH_CodeShobe;
                Name_shobe = setting.fldH_NameShobe;
                string shhesab_b = setting.fldSh_HesabCheck;
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);                
                var disket = PayServic.GetDisketWithFilter("", "", 0, IP, out ErrPay).Where(l => l.fldBankFixId == BankId && l.fldType==true).LastOrDefault();
                if (disket != null)
                {
                    marhale = (Convert.ToInt32(disket.fldMarhale) + 1).ToString();
                }
                else
                {
                    if (BankId == 1 || BankId == 5)
                    {
                        marhale = "101";
                    }
                    else if (BankId == 4 || BankId == 15)
                    {
                        marhale = "1";
                    }
                }

                
                string[] a = new string[q.Count + 1];
                if (BankId == 16 || BankId == 35)
                {
                     a = new string[q.Count + 2]; 
                }
                tarikh = servic.GetDate( IP, out Err).fldTarikh;
                var sal = tarikh.Substring(2, 2);
                var mah = tarikh.Substring(5, 2);
                var roz = tarikh.Substring(8, 2);

                string[] alpha = { "A", "B" };
                int index = 0;
                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                int k3 = 0;
                for (int i = 0; i < q.Count; i++)
                {
                    long khales = q[i].fldkhalesPardakhti;
                    jam = jam + khales;
                    tedad++;
                    if (BankId == 1)/*ملی*/
                    {
                        if (System.Configuration.ConfigurationManager.AppSettings["BmiDisketVer"].ToString() == "new")
                        {
                            Cell Cell = sheet.Cells[alpha[0] + (k3 + 1)];
                            Cell.PutValue(q[i].fldShomareHesab.PadLeft(13, '0'));

                            Cell Cell1 = sheet.Cells[alpha[1] + (k3 + 1)];
                            Cell1.PutValue(khales.ToString());
                            k3++;
                        }
                        else
                            a[i + 1] = (i + 1).ToString().PadLeft(5, '0') + q[i].fldShomareHesab.PadLeft(13, '0') + khales.ToString().PadLeft(15, '0') + "000000000000000";
                    }
                    else if (BankId == 2)/*ملت*/
                    {
                        a[i + 1] = q[i].fldShomareHesab.PadLeft(10, '0') + khales.ToString().PadLeft(15, '0');
                    }
                    else if (BankId == 3)/*تجارت*/
                    {
                        a[i + 1] = q[i].fldShomareHesab.PadLeft(10, '0') + khales.ToString().PadLeft(13, '0') + sal + mah + roz;
                    }
                    else if (BankId == 4)/*صادرات*/
                    {
                        a[i + 1] = (i + 1).ToString().PadLeft(5, '0') + q[i].fldShomareHesab.PadLeft(13, '0') + khales.ToString().PadLeft(15, '0') + "000000000000000";
                    }
                    else if (BankId == 5)/*کشاورزی*/
                    {
                        a[i + 1] = (i + 1).ToString().PadLeft(5, '0') + q[i].fldShomareHesab.PadLeft(13, '0') + khales.ToString().PadLeft(15, '0') + "000000000000000";
                    }
                    else if (BankId == 6)/*سپه*/
                    {

                    }
                    else if (BankId == 7)/*مسکن*/
                    {

                    }
                    else if (BankId == 8)/*پارسیان*/
                    {

                    }
                    else if (BankId == 9)/*پاسارگاد*/
                    {

                    }
                    else if (BankId == 10)/*سامان*/
                    {

                    }
                    else if (BankId == 11)/*سرمایه*/
                    {

                    }
                    else if (BankId == 12)/*اقتصاد نوین*/
                    {

                    }
                    else if (BankId == 13)/*پست بانک*/
                    {

                    }
                    else if (BankId == 14)/*رفاه کارگران*/
                    {

                    }
                    else if (BankId == 15)/*کشاورزی مهر*/
                    {
                        a[i] = "00000" + q[i].fldShomareHesab.PadLeft(18, '0') + khales.ToString().PadLeft(12, '0');
                    }
                    else if (BankId == 16 || BankId == 35)/*شهر یا قرض الحسنه مهرایران*/
                    {
                        var str = "";
                        if (FieldName == "Hoghogh")
                        {
                            str = ",C,واریز بابت حقوق ";
                        }
                        else if (FieldName == "BonKart")
                        {
                            str = ",C,واریز بابت بن مزایای جانبی رفاهی ";
                        }
                        else if (FieldName == "Eydi")
                        {
                            str = ",C,واریز بابت عیدی ";
                        }
                        else if (FieldName == "SayerPardakht")
                        {
                            str = ",C,واریز بابت سایر مزایا ";
                        }
                        else if (FieldName == "EzafeKari")
                        {
                            str = ",C,واریز بابت اضافه کاری ";
                        }
                        else if (FieldName == "TatilKari")
                        {
                            str = ",C,واریز بابت تعطیل کاری ";
                        }
                        else if (FieldName == "Morakhasi")
                        {
                            str = ",C,واریز بابت مرخصی ";
                        }
                        else if (FieldName == "Mamuriyat")
                        {
                            str = ",C,واریز بابت مأموریت ";
                        }
                        else if (FieldName == "KomakGheyerNaghdi_Mostamer")
                        {
                            str = ",C,واریز بابت کمک های غیرنقدی مستمر ";
                        }
                        else if (FieldName == "BonKart")
                        {
                            str = ",C,واریز بابت بن مزایای جانبی رفاهی ";
                        }
                        else if (FieldName == "OnAccount")
                        {
                            str = ",C,واریز بابت علی الحساب  ";
                        }
                       // if (q[i].fldShomareHesab!=null)
                        a[i + 2] = q[i].fldShomareHesab.PadLeft(10, '0') + "," + khales.ToString() + str + MonthName + " ماه سال " + Year;
                    }
                    else if (BankId == 17)/*انصار*/
                    {
                        a[i + 1] = q[i].fldShomareHesab.PadLeft(10, '0') + "\t" + khales.ToString();
                    }
                }
                MemoryStream stream = new MemoryStream();
                if (BankId == 1)/*ملی*/
                {
                    if (code_sazeman.Length == 4)
                    {
                        a[0] = code_shobe.PadLeft(4, '0') + code_sazeman + marhale.ToString().PadLeft(4, '0') + sal + mah + roz + jam.ToString().PadLeft(15, '0') + tedad.ToString().PadLeft(5, '0') + "0000000000";
                    }
                    else if (code_sazeman.Length == 6)
                    {
                        a[0] = code_shobe.PadLeft(4, '0') + code_sazeman + marhale.ToString().PadLeft(6, '0') + sal + mah + roz + jam.ToString().PadLeft(15, '0') + tedad.ToString().PadLeft(5, '0') + "000000";
                    }
                    //a[0] = code_shobe.PadLeft(4, '0') + code_sazeman.PadLeft(4, '0') + marhale.ToString().PadLeft(4, '0') + sal + mah + roz + jam.ToString().PadLeft(15, '0') + tedad.ToString().PadLeft(5, '0') + "0000000000";
                    if (System.Configuration.ConfigurationManager.AppSettings["BmiDisketVer"].ToString() == "new")
                    {
                        wb.Save(stream, SaveFormat.Xlsx);
                        FileName = "hog" + sal + mah + roz + ".xlsx";
                        Passvand = ".xlsx";
                    }
                    else
                    {
                        FileName = code_sazeman + code_shobe + "." + mah;
                        Passvand = "." + mah;
                    }                    
                }
                else if (BankId == 2)/*ملت*/
                {
                    a[0] = tedad.ToString().PadLeft(10, '0') + jam.ToString().PadLeft(15, '0');
                    FileName = "FL" + sal + mah + roz + ".PAY";
                    Passvand = ".PAY";
                }
                else if (BankId == 3)/*تجارت*/
                {
                    a[0] = shhesab_b.PadLeft(10, '0') + jam.ToString().PadLeft(13, '0') + sal + mah + roz;
                    FileName = "trans.dat";
                    Passvand = ".dat";
                }
                else if (BankId == 4)/*صادرات*/
                {
                    if (code_sazeman.Length == 4)
                    {
                        a[0] = code_shobe + code_sazeman + marhale.PadLeft(4, '0') + sal + mah + roz + jam.ToString().PadLeft(15, '0') + tedad.ToString().PadLeft(5, '0') + "0000000000";
                    }
                    else if (code_sazeman.Length == 6)
                    {
                        a[0] = code_shobe + code_sazeman + marhale.PadLeft(6, '0') + sal + mah + roz + jam.ToString().PadLeft(15, '0') + tedad.ToString().PadLeft(5, '0') + "000000";
                    }
                    //a[0] = code_shobe + code_sazeman + marhale.PadLeft(4, '0') + sal + mah + roz + jam.ToString().PadLeft(15, '0') + tedad.ToString().PadLeft(5, '0') + "0000000000";
                    FileName = code_shobe + code_sazeman + "." + marhale.PadLeft(3, '0');
                    Passvand = "." + marhale.PadLeft(3, '0');
                }
                else if (BankId == 5)/*کشاورزی*/
                {
                    if (code_sazeman.Length == 4)
                    {
                        a[0] = code_shobe.PadLeft(4, '0') + code_sazeman + marhale.PadLeft(4, '0') + sal + mah + roz + jam.ToString().PadLeft(15, '0') + tedad.ToString().PadLeft(5, '0') + "0000000000";
                    }
                    else if (code_sazeman.Length == 6)
                    {
                        a[0] = code_shobe.PadLeft(4, '0') + code_sazeman + marhale.PadLeft(6, '0') + sal + mah + roz + jam.ToString().PadLeft(15, '0') + tedad.ToString().PadLeft(5, '0') + "000000";
                    }
                    //a[0] = code_shobe.PadLeft(4, '0') + code_sazeman.PadLeft(4, '0') + marhale.PadLeft(4,'0') + sal + mah + roz + jam.ToString().PadLeft(15, '0') + tedad.ToString().PadLeft(5, '0') + "0000000000";
                    FileName = code_sazeman + code_shobe + "." + mah;
                    Passvand = "." + mah;
                }
                else if (BankId == 15)/*کشاورزی مهر*/
                {
                    a[q.Count] = "T00000000" + code_shobe.PadLeft(4, '0') + "0017520035" + jam.ToString().PadLeft(12, '0');
                    FileName = code_shobe + code_sazeman + marhale.PadLeft(2, '0') + ".txt";
                    Passvand = ".txt";
                }
                else if (BankId == 17)/*انصار*/
                {
                    a[0] = "N " + q.Count.ToString() + " " + shhesab_b + " " + jam.ToString();
                    FileName = "trans.txt";
                    Passvand = ".txt";
                }
                else if (BankId == 16 || BankId == 35)/*شهر یا قرض الحسنه مهرایران*/
                {
                    a[0] = "1," + q.Count.ToString() + "," + jam.ToString();
                    a[1] = shhesab_b.PadLeft(10, '0') + "," + jam.ToString() + ",D,برداشت از حساب سپرده جاری " + organName;
                    FileName = "trans.txt";
                    Passvand = ".txt";
                }
                

                savePath = Server.MapPath(@"~\Uploaded\" + FileName);
                if (System.IO.File.Exists(savePath))
                {
                    System.IO.File.Delete(savePath);
                }
                else if (BankId == 1 && System.Configuration.ConfigurationManager.AppSettings["BmiDisketVer"].ToString() == "new")                    
                    System.IO.File.WriteAllBytes(savePath, stream.ToArray());
                else
                    System.IO.File.WriteAllLines(savePath, a);

                MemoryStream disket_byte = new MemoryStream(System.IO.File.ReadAllBytes(savePath));         
                Msg = "تهيه ديسكت بانك با موفقيت به پايان رسيد.";
                MsgTitle = "تراكنش موفق";

                string fieldName = "";
                if (TypePardakht == 1)
                    fieldName = "KarKardMahane";
                else if (TypePardakht == 4)
                    fieldName = "EzafeKari";
                else if (TypePardakht == 5)
                    fieldName = "TatilKari";
                else if (TypePardakht == 2)
                    fieldName = "Eydi";
                else if (TypePardakht == 7)
                    fieldName = "Mamuriyat";
                else if (TypePardakht == 6)
                    fieldName = "Morakhasi";
                else if (TypePardakht == 8)
                    fieldName = "KomakGheyerNaghdi_Mostamer";
                else if (TypePardakht == 9)
                    fieldName = "KomakGheyerNaghdi_GheyerMostamer";
                else if (TypePardakht == 3)
                    fieldName = "SayerPardakht";
                else if (TypePardakht == 12)
                    fieldName = "OnAccount";
                PayServic.UpdateFlag(fieldName, Year, Month, NobatePardakht,MarhalePardakht, Convert.ToInt32(Session["OrganId"]), Convert.ToByte(5), Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrPay);
                if (BankId == 1 || BankId == 4 || BankId == 5 || BankId == 15)/*ملی یا صادرات*/
                {
                    if (Id == 0)
                    {
                        DisketId = PayServic.InsertDisket(tarikh, tedad, true, jam, TypePardakht, code_shobe, code_sazeman, disket_byte.ToArray(), Passvand, FileName, BankId,Name_shobe, "",
                        Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                        if (ErrPay.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = ErrPay.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var diskett = PayServic.GetDisketDetail(Id, IP, out ErrPay);
                        PayServic.UpdateDisket(Id,tarikh, tedad, true, jam, TypePardakht, code_shobe, code_sazeman,diskett.fldFileId, disket_byte.ToArray(), Passvand, FileName, BankId, Name_shobe,"",
                        Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                        if (ErrPay.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = ErrPay.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1
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
                Er = Er,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
                masir = savePath,
                DisketId = DisketId,
                FileName = FileName,
                Passvand = Passvand
            }, JsonRequestBehavior.AllowGet);
        }
        private string estekhdam_new(int? noo)
        {
            string noestekhdam = "";
            switch (noo)
            {
                case 1:
                    noestekhdam = "3";
                    break;
                case 2:
                    noestekhdam = "1";
                    break;
                case 3:
                    noestekhdam = "1";
                    break;
                case 4:
                    noestekhdam = "1";
                    break;
                case 5:
                    noestekhdam = "1";
                    break;
                case 6:
                    noestekhdam = "3";
                    break;
                case 7:
                    noestekhdam = "4";
                    break;
                case 8:
                    noestekhdam = "4";
                    break;
                case 9:
                    noestekhdam = "4";
                    break;
                case 10:
                    noestekhdam = "12";
                    break;
               

            }
            if( noestekhdam=="")
                noestekhdam = "12";
            return noestekhdam;
        }

        private double maliat_(int mashmol_maliyat, int FiscalHeaderId)
        {
            //--------------------مالیات  
            double  az_mablagh_k = 0, az_mablagh = 0;
            var q = PayServic.GetFiscal_DetailWithFilter("fldFiscalHeaderId", FiscalHeaderId.ToString(), 0, IP, out ErrPay).ToList();
            for (int i = 0; i < q.Count; i++)
            {
                if (mashmol_maliyat <= q[i].fldAmountTo && mashmol_maliyat >= q[i].fldAmountFrom)
                {
                    az_mablagh = q[i].fldAmountFrom;
                }
            }

            if (az_mablagh != 0)
            {
                for (int i = 0; i < q.Count; i++)
                {
                    if (q[i].fldAmountFrom <az_mablagh)
                    {
                        az_mablagh_k = q[i].fldAmountTo;
                    }
                }
            }
            if (az_mablagh_k != 0 && mashmol_maliyat > az_mablagh_k)
                return (mashmol_maliyat - az_mablagh_k);
            else
                return 0;
        }

        public ActionResult GenerateDiskBime(short Year, byte Month, byte NobatePardakht, string Sh_List, string FieldName/*, int KargahId, string KargahNum*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; string FileName = ""; /*string Masir_WOR = "";*/ string Masir_KAR = ""; string FileName2=""; 
            byte Er = 0;
            int TedadMard=0;int TedadZan = 0;
            long J_Mazaya = 0; long J_Bime_P = 0; long J_Bime_K = 0; long j_motalebat = 0; long j_bime_bikari = 0;
            var PathBime = ConfigurationManager.AppSettings["PathTamin"].ToString();
            try
            {
                Bime.BimeSRV BimeService = new Bime.BimeSRV();
                BimeService.Timeout = 2000000000;
                var a = BimeService.GetBimeInfo(FieldName, Year, Month, NobatePardakht, Convert.ToInt32(Session["OrganId"]), Sh_List);
                
                System.IO.File.Delete(PathBime + @"\Manufactory2.mdb");
                System.IO.File.Copy(a[0].Masir_KAR, PathBime + @"\Manufactory2.mdb");
                PayServic.UpdateFlag("KarKardMahane", Year, Month, NobatePardakht, 0, Convert.ToInt32(Session["OrganId"]), Convert.ToByte(2), Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrPay);
                Msg = "تهيه دیسکت بیمه با موفقيت به پايان رسيد.";
                MsgTitle = "تهیه دیسکت موفق";
                Session["BimeRes"] = a;
                return Json(new
                {
                    Msg = Msg,
                    MsgTitle = MsgTitle,
                    Er = Er,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                MsgTitle = "خطا";
                Er = 1;
                return Json(new
                {
                    Msg = Msg,
                    MsgTitle = MsgTitle,
                    Er = Er
                }, JsonRequestBehavior.AllowGet);
            }
            
        }
        public ActionResult CheckDataDiskMaliyat( short Year,  byte Month, byte NobatePardakht, byte MarhalePardakht, string FieldName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; byte Er = 0; 

            try
            {
                
                var q = PayServic.GetSelectVariziForBank(FieldName,"", Year, Month, NobatePardakht, MarhalePardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).FirstOrDefault();
                
                if(q==null)
                {
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
       public FileResult GenerateExcelBime(short Year, byte Month, byte NobatePardakht)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
            int index = 0;
            string Checked = "fldCodemeli;fldName;fldFamily;fldJobeCode;fldJobDesc;fldKarkard;fldShomareBime;fldBimePersonal;fldBimeBikari;fldBimeBikari;fldItem;Mahane;Mazaya;Rozane;";
            var StatusCheck = Checked.Split(';');
            var Check = "";

            var fldName = ""; var fldFamily = ""; var fldMelli_EconomicCode = ""; var fldKarkard = ""; 
                 var Kargah = PayServic.SelectInsuranceWorkshop("DisketBime", Year.ToString(), Convert.ToInt32(Session["OrganId"]), Month, IP, out ErrPay).FirstOrDefault();
            var data = PayServic.GetDisketBime(Year, Month,Kargah.fldId, NobatePardakht,  IP, out ErrPay).ToList();
            
           

            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];

            foreach (var item in StatusCheck)
            {
                switch (item)
                {
                    case "fldCodemeli":
                        Check = "کد ملی";
                        Cell cell22 = sheet.Cells[alpha[index] + "1"];
                        cell22.PutValue(Check);
                        int k3 = 0;
                        foreach (var _item in data)
                        {
                            fldMelli_EconomicCode = _item.fldCodemeli;
                            Cell Cell = sheet.Cells[alpha[index] + (k3 + 2)];
                            Cell.PutValue(fldMelli_EconomicCode);
                            k3++;
                        }
                        index++;
                        break;
                    case "fldName":
                        Check = "نام";
                        Cell cell6 = sheet.Cells[alpha[index] + "1"];
                        cell6.PutValue(Check);
                        int x = 0;
                        foreach (var _item in data)
                        {
                            fldName = _item.fldName.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
                            Cell.PutValue(fldName);
                            x++;
                        }
                        index++;
                        break;
                    case "fldFamily":
                        Check = "نام خانوادگی";
                        Cell cell5 = sheet.Cells[alpha[index] + "1"];
                        cell5.PutValue(Check);
                        int z = 0;
                        foreach (var _item in data)
                        {
                            fldFamily = _item.fldFamily.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                            Cell.PutValue(fldFamily);
                            z++;
                        }
                        index++;
                        break;
                    case "fldJobeCode":
                        Check = "کد شغل";
                        Cell cell1 = sheet.Cells[alpha[index] + "1"];
                        cell1.PutValue(Check);
                        int j2 = 0;
                        foreach (var _item in data)
                        {
                            var fldJobeCode = (_item.fldJobeCode).ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (j2 + 2)];
                            Cell.PutValue(fldJobeCode);
                            j2++;
                        }
                        index++;
                        break;
                    case "fldJobDesc":
                        Check = "عنوان شغل";
                        Cell cell2 = sheet.Cells[alpha[index] + "1"];
                        cell2.PutValue(Check);
                        int k = 0;
                        foreach (var _item in data)
                        {
                            var fldJobDesc = _item.fldJobDesc.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                            Cell.PutValue(fldJobDesc);
                            k++;
                        }
                        index++;
                        break;
                    case "fldKarkard":
                        Check = "کارکرد";
                        Cell cell3 = sheet.Cells[alpha[index] + "1"];
                        cell3.PutValue(Check);
                        int q = 0;
                        foreach (var _item in data)
                        {
                            fldKarkard = _item.fldKarkard.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                            Cell.PutValue(fldKarkard);
                            q++;
                        }
                        index++;
                        break;
                    case "fldShomareBime":
                        Check = "شماره بیمه";
                        Cell cell4 = sheet.Cells[alpha[index] + "1"];
                        cell4.PutValue(Check);
                        int w = 0;
                        foreach (var _item in data)
                        {
                            var fldShomareBime = _item.fldShomareBime;
                            Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                            Cell.PutValue(fldShomareBime);
                            w++;
                        }
                        index++;
                        break;
                    case "fldBimePersonal":
                        Check = "بیمه پرسنل";
                        Cell cell = sheet.Cells[alpha[index] + "1"];
                        cell.PutValue(Check);
                        int i = 0;
                        foreach (var _item in data)
                        {
                            var fldBimePersonal = _item.fldBimePersonal;
                            Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                            Cell.PutValue(fldBimePersonal);
                            i++;
                        }
                        index++;
                        break;
                    case "fldBimeKarFarma":
                        Check = "بیمه کارفرما";
                        Cell cell12 = sheet.Cells[alpha[index] + "1"];
                        cell12.PutValue(Check);
                        int j = 0;
                        foreach (var _item in data)
                        {
                            var fldBimeKarFarma = _item.fldBimeKarFarma;
                            Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                            Cell.PutValue(fldBimeKarFarma);
                            j++;
                        }
                        index++;
                        break;
                    case "fldBimeBikari":
                        Check = "بیمه بیکاری";
                        Cell cell7 = sheet.Cells[alpha[index] + "1"];
                        cell7.PutValue(Check);
                        int y = 0;
                        foreach (var _item in data)
                        {
                            var fldBimeBikari = _item.fldBimeBikari;
                            Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
                            Cell.PutValue(fldBimeBikari);
                            y++;
                        }
                        index++;
                        break;
                    case "Mahane":
                        Check = "جمع کل مطالبات مشمول بیمه";
                        Cell cell23 = sheet.Cells[alpha[index] + "1"];
                        cell23.PutValue(Check);
                        int k2 = 0;
                        foreach (var _item in data)
                        {
                            var Mahane = _item.KargariMahane;
                            if(Mahane==0)
                                Mahane = _item.KarmandiMahane;
                            Cell Cell = sheet.Cells[alpha[index] + (k2 + 2)];
                            Cell.PutValue(Mahane);
                            k2++;
                        }
                        index++;
                        break;
                    case "Mazaya":
                        Check = "مزایای ماهانه مشمول";
                        Cell cell24 = sheet.Cells[alpha[index] + "1"];
                        cell24.PutValue(Check);
                        int k4 = 0;
                        foreach (var _item in data)
                        {
                            var Mahane = _item.KargariMahane;
                            if (Mahane == 0)
                                Mahane = _item.KarmandiMahane;
                            var Mazaya = _item.fldMashmolBime - Mahane;
                            Cell Cell = sheet.Cells[alpha[index] + (k4 + 2)];
                            Cell.PutValue(Mazaya);
                            k4++;
                        }
                        index++;
                        break;
                    case "Rozane":
                        Check = "دستمزد روزانه";
                        Cell cell25 = sheet.Cells[alpha[index] + "1"];
                        cell25.PutValue(Check);
                        int k5 = 0;
                        foreach (var _item in data)
                        {
                            var Mahane = _item.KargariMahane;
                            if (Mahane == 0)
                                Mahane = _item.KarmandiMahane;
                            var Mazaya =  Mahane/_item.fldKarkard;
                            Cell Cell = sheet.Cells[alpha[index] + (k5 + 2)];
                            Cell.PutValue(Mazaya);
                            k5++;
                        }
                        index++;
                        break;
                }
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
            var FileName = "اطلاعات بیمه " + MonthName + " ماه " + "سال " + Year.ToString() + ".xls";
            return File(stream.ToArray(), "xls", FileName);
        }
        public ActionResult GenerateDiskMaliyat(string Sh_Check, string DateCheck, byte Bank, string NameShobe, string Sh_Hesab, short Year,
            string DatePay, byte Month, byte NobatePardakht, byte MarhalePardakht, string FieldName, byte TypePardakht,bool SayerMoafiyat,bool IsMotalebat)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; byte Er = 0; var FileName = ""; var Passvand = ""; string savePath = "";
            var FileName2 = ""; var Passvand2 = ""; string savePath2 = ""; var FileName3 = ""; var Passvand3 = ""; string savePath3 = "";
            var TIN = ""; var TFN = "";

            try
            {
                string noestekhdam = "", code_moafi = "", nobime = "", WorkShopName="";
                int hoghogh_mostamar = 0, Bime = 0, sayer_Naghdi = 0, mamoriat = 0, ezafekar = 0, moavaghe = 0, sanavat = 0, jammaliat = 0, eidi = 0
                    , sayer_GhNaghdi = 0, sayer = 0, maliat = 0, maliat_moavaghe = 0, maliat_manfi = 0, raste = 0;
                long bonMostamar = 0, bonGheyrMostamar = 0, bonMostamarMoavagh = 0, bonGheyrMostamarMoavagh = 0, ezafeKhales = 0, TatilKhales = 0,KhalesEydi=0,KhalesSayer=0,KhalesGheyrMostamar=0,KhalesMostamar=0;
                int cc = 0;
                var q = PayServic.GetSelectVariziForBank(FieldName,"", Year, Month, NobatePardakht, MarhalePardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
              
                var setting = PayServic.GetSettingWithFilter("fldOrganId", Session["OrganId"].ToString(), 0,1, IP, out ErrPay).FirstOrDefault();
                TIN = setting.fldCodeEghtesadi;
                TFN = setting.fldCodeParvande;
                string[] Disket = new string[q.Count];
                string[] Personeli = new string[q.Count];

                PayServic.UpdateFlag("KarKardMahane", Year, Month, NobatePardakht, MarhalePardakht, Convert.ToInt32(Session["OrganId"]), Convert.ToByte(4), Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrPay);

                //if (FieldName == "Hoghogh")
                //{
                    var q1 = PayServic.GetSelectDisketMaliyatForHoghogh(Year, Month, NobatePardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
                    Disket = new string[q1.Count];
                    Personeli = new string[q1.Count];
                    for (var j = 0; j < q1.Count; j++)
                    {
                        if (q1[j].eydi == 0 || q1[j].eydi == null)
                        {
                            var e = PayServic.GetSelectVariziForBank("EydiPersonalId", "", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                            if (e != null)
                            {
                                eidi = e.fldMablagh;
                                KhalesEydi = e.fldkhalesPardakhti;
                            }
                            else
                            {
                                eidi = 0;
                                KhalesEydi = 0;
                            }
                        }
                        else
                        {
                            eidi = q1[j].eydi;
                            KhalesEydi = q1[j].eydi;
                        }

                        var b = PayServic.GetSelectVariziForBank("BonKart_MostamarPersonalId","", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                        if (b != null)/*مبلغ قیمت تمام شده سایر مزایای مستمر غیرنقدی*/
                        {
                            bonMostamar = b.fldkhalesPardakhti;
                            bonMostamarMoavagh = b.fldMablagh;/*مبلغ حقوق و مزایای مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی*/
                        }
                        else 
                            {
                            bonMostamar =0;
                            bonMostamarMoavagh = 0;
                        }

                        b = PayServic.GetSelectVariziForBank("BonKart_GheyrMostamarPersonalId","", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                         if (b != null)/*مبلغ قیمت تمام شده مزایای غیر مستمر غیرنقدی ماه جاری- ریالی*/
                         {
                             bonGheyrMostamar = b.fldkhalesPardakhti;
                             bonGheyrMostamarMoavagh = b.fldMablagh;/*مبلغ مزایای غیر مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی*/
                         }
                        else 
                            {
                             bonGheyrMostamar = 0;
                            bonGheyrMostamarMoavagh = 0;
                         }

                         var ezafe = PayServic.GetSelectVariziForBank("EzafeKari_MaliyatPersonalId","", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                         if (ezafe != null)
                         {
                             ezafekar = ezafe.fldMablagh;
                             ezafeKhales = ezafe.fldkhalesPardakhti;
                         }
                         else
                         {
                             ezafekar = 0;
                             ezafeKhales = 0;
                         }

                         var t = PayServic.GetSelectVariziForBank("TatilKari_MaliyatPersonalId","", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                        if (t != null)
                        {
                            ezafekar = ezafekar + t.fldMablagh;
                            TatilKhales = t.fldkhalesPardakhti;
                        }
                        else
                            TatilKhales = 0;

                        if(ezafe==null && t==null) 
                            ezafekar=0;

                        var s = PayServic.GetSelectVariziForBank("SayerPardakht_MaliyatPersonalId", "",Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                        if (s != null)
                        {
                            sayer =  s.fldMablagh;
                            KhalesSayer=s.fldkhalesPardakhti;
                        }
                        else 
                        {
                            sayer=0;
                            KhalesSayer=0;
                        }

                       

                        var m = PayServic.GetSelectVariziForBank("KomakGheyerNaghdi_Mostamer_MaliyatPersonalId","", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                        if (m != null)
                        {
                            sayer_Naghdi = m.fldMablagh;
                            KhalesMostamar=m.fldkhalesPardakhti;
                        }
                        else {
                            sayer_Naghdi=0;
                            KhalesMostamar=0;
                        }

                        var g = PayServic.GetSelectVariziForBank("KomakGheyerNaghdi_GheyreMostamer_MaliyatPersonalId","", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                        if (g != null)
                        {
                            sayer_GhNaghdi = m.fldMablagh;
                            KhalesGheyrMostamar=m.fldkhalesPardakhti;
                        }
                        else 
                        {
                            sayer_GhNaghdi=0;
                            KhalesGheyrMostamar=0;
                        }

                        hoghogh_mostamar = q1[j].ItemEstekhdam;/*مبلغ جمع ناخالص حقوق و مزایای مستمر نقدی ماه جاری - ریالی*/
                        var ezafekarkol = q1[j].ezafekar + q1[j].tatilkari+ezafekar;
                        mamoriat = q1[j].mamoriat;
                        //sayer_Naghdi = q1[j].KolMotalebat + mamoriat;
                        sanavat = q1[j].s_payankhedmat;
                        maliat = q1[j].fldMaliyat;
                        var MaliyatDaraei = q1[j].fldMaliyatDaraei;
                        //double mashmol = maliat_(q1[j].fldMashmolMaliyat, q1[j].fldFiscalHeaderId);
                        moavaghe = q1[j].MashmolMaliyatMoavaghe;/*مبلغ حقوق و مزایای مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است - ریالی*/
                        maliat_moavaghe = q1[j].MaliyatMoavaghe;
                        Bime=q1[j].fldBimePersonal;
                        //maliat_manfi = q1[j].MaliyatManfi;
                        //if (maliat_manfi != 0 )
                        //    maliat = maliat + maliat_manfi;
                        //else
                        //    maliat = maliat + maliat_moavaghe;

                        noestekhdam = estekhdam_new(q1[j].fldNoeEstekhdam);
                        code_moafi = "";
                        code_moafi = q1[j].fldMaliyatEsargari.ToString();
                       /* switch (q1[j].fldEsargariId)
                        {
                            case 1:
                            case 6:
                            case 7:
                                code_moafi = "1";
                                break;
                            case 2:
                            case 4:
                            case 5:
                                code_moafi = "2";
                                break;
                            case 3:
                                code_moafi = "3";
                                break;

                        }*/
                        if (q1[j].fldTypeBimeId == 1)
                        {
                            nobime = "2";
                        }
                        else
                        {
                            nobime = "1";
                        }

                        jammaliat = jammaliat + q1[j].fldMaliyat;
                        int SayerMoafiat = 0,Motalebat=0;
                        if (SayerMoafiyat == true)
                            SayerMoafiat = Convert.ToInt32(q1[j].SayerMoafiyat);
                        //if (IsMotalebat == true)
                        Motalebat = Convert.ToInt32(q1[j].KolMotalebat);/*سایر حقوق و مزایای غیر مستمر نقدی ماه جاری- ریالی*/
                        long khales = 0;
                         khales = q.Where(l => l.fldCodemeli == q1[j].fldCodemeli).FirstOrDefault().fldkhalesPardakhti;
                         khales += ezafeKhales + TatilKhales +/* KhalesEydi +*/ KhalesGheyrMostamar + KhalesMostamar + KhalesSayer + bonGheyrMostamar + bonMostamar + bonGheyrMostamarMoavagh + bonMostamarMoavagh;
                       
                        Disket[cc] = q1[j].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,"+bonMostamar+sayer_Naghdi+ "," +bonMostamarMoavagh+",0,0," + ezafekarkol + ",0," + mamoriat + "," + q1[j].karane
                             + "," +sayer+ ",0,"+eidi+",0,0,0," + sanavat + ",0," + Motalebat+ ",0," +bonGheyrMostamar+sayer_GhNaghdi +"," +bonGheyrMostamarMoavagh+ "," +
                            +Bime +","+ q1[j].fldBimeOmrPersonal +","+ khales;
             

                        //Disket[cc] = q1[j].fldCodemeli + ",1," + Month + ",0,85,0," +
                        //q1[j].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                        //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + Motalebat + ",0," + Bime + ",0," + SayerMoafiat + "," + ezafekar + "," +
                        //sayer_Naghdi + ",0,0," + mamoriat + ",0,0," + sanavat + ",0,0,0," + maliat + "," + maliat;
                        if (q1[j].fldMadrakId == 7)
                            q1[j].fldMadrakId = 1;
                        var a = PersoneliService.GetPersoneliInfoWithFilter("fldCodemeli", q1[j].fldCodemeli, Convert.ToInt32(Session["OrganId"]), 1, IP, out ErrPersoneli).FirstOrDefault();
                        var p = PayServic.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", q1[j].fldPersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out ErrPay).FirstOrDefault();
                        var r = servic.GetRasteWithFilter("fldText", a.fldRasteShoghli, 0, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), out Err).FirstOrDefault();
                         raste = 13;
                         if (p != null)
                             WorkShopName = p.fldWorkShopName;
                         else
                             WorkShopName = "";
                         if (r != null && r.fldMaliyat != null)
                            raste =Convert.ToInt32( r.fldMaliyat);
                        var shBime= q1[j].fldShomareBime.PadLeft(10, '0') ;
                        Personeli[cc] = "1," + q1[j].fldCodemeli + "," + q1[j].fldName + "," + q1[j].fldFamily + "," + q1[j].fldFatherName + "," + a.fldTarikhTavalod.Replace("/", "") + "," + a.fldSh_Shenasname
                            + "," + a.fldNameMahalTavalod + "," + q1[j].fldMadrakId + "," + nobime + "," + shBime + "," + WorkShopName + "," + code_moafi + ",103,103," 
                            + q1[j].fldCodePosti.PadLeft(10, '0') + "," + q1[j].fldAddress + "," + raste + "," + q1[j].Semat + "," + noestekhdam + "," + q1[j].fldTarikhEstekhdam.Replace("/", "") + ",,";
                      
                        cc++;
                        //Personeli[cc] = "1,1," + q1[j].fldCodemeli + "," + q1[j].fldName + "," + q1[j].fldFamily + ",103," + q1[j].fldPersonalId + "," + q1[j].fldMadrakId + "," + q1[j].Semat
                        //    + "," + nobime + ",," + q1[j].fldShomareBime + "," + q1[j].fldCodePosti.PadLeft(10, '0') + "," + q1[j].fldAddress + "," + q1[j].fldTarikhEstekhdam.Replace("/", "") + "," + noestekhdam + "," +
                        //    q1[j].MahalKhedmat + ",1,1,," + code_moafi + ",,";
                        //cc++;
                    }
               // }
              /*  else
                {
                    for (int i = 0; i < q.Count; i++)
                    {
                        if (FieldName == "Eydi")
                        {
                            eidi = q[i].fldMablagh;
                        }
                        else if (FieldName == "BonKart")
                        {
                            bon = q[i].fldMablagh;
                        }
                        else if (FieldName == "EzafeKari" || FieldName == "TatilKari")
                        {
                            ezafekar = q[i].fldMablagh;
                        }
                        else if (FieldName == "SayerPardakht")
                        {
                            sayer = q[i].fldMablagh;
                        }
                        else if (FieldName == "KomakGheyerNaghdi_Mostamer" || FieldName == "KomakGheyerNaghdi_GheyreMostamer")
                        {
                            sayer_GhNaghdi = q[i].fldMablagh;
                        }
                        noestekhdam = estekhdam_new(q[i].fldNoeEstekhdam);
                        code_moafi = "";
                       // switch (q[i].fldEsargariId)
                       // {
                       //     case 1:
                       //     case 6:
                       //     case 7:
                       //         code_moafi = "1";
                       //         break;
                       //     case 2:
                       //     case 4:
                       //     case 5:
                       //         code_moafi = "2";
                       //         break;
                       //     case 3:
                       //         code_moafi = "3";
                       //         break;

                       // }
                        code_moafi = q[i].fldMaliyatEsargari.ToString();
                        if (q[i].fldTypeBimeId == 1)
                        {
                            nobime = "2";
                        }
                        else
                        {
                            nobime = "1";
                        }
                        jammaliat = jammaliat + q[i].fldMaliyat;
                        if (FieldName == "BonKart")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + q[i].fldMablagh + ",0,0,0," + ezafekar + ",0," + mamoriat 
                             + ",0,0,0,0,0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0,0,0," +
                             + Bime + ",0," + q[i].fldkhalesPardakhti;
             
                           
                        }
                        else if (FieldName == "Eydi")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0," + ezafekar + ",0," + mamoriat 
                             + ",0,0,0," + q[i].fldMablagh + ",0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0,0,0," +
                            + Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + ",0,0," + mamoriat + ",0," + q[i].fldMablagh + "," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "EzafeKari")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0," + ezafekar + ",0," + mamoriat
                             + ",0,0,0,0,0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0,0,0," +
                            Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + ",0,0," + mamoriat + ",0,0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "TatilKari")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + sayer_GhNaghdi + ",0,0,0," + ezafekar + ",0," + mamoriat 
                             + ",0,0,0,0,0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0,0,0," +
                             Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + "," + sayer + ",0," + mamoriat + "," + sayer_GhNaghdi + ",0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "KomakGheyerNaghdi_Mostamer")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + sayer_GhNaghdi + ",0,0,0," + ezafekar + ",0," + mamoriat + ",0,"
                             + ",0,0,0,0,0,0," + sanavat + ",0,0,0,0,0," +
                             Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + sayer_GhNaghdi + ",0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + ",0,0," + mamoriat + ",0,0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "KomakGheyerNaghdi_GheyreMostamer")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0," + ezafekar + ",0," + mamoriat + ",0,"
                            + ",0,0,0,0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0," + sayer_GhNaghdi + ",0," +
                           + Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + ",0,0," + mamoriat + "," + sayer_GhNaghdi + ",0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "SayerPardakht")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + sayer_GhNaghdi + ",0,0,0," + ezafekar + ",0," + mamoriat + ",0,"
                          +sayer + ",0,0,0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0," + sayer_Naghdi + ",0," +
                           Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + "," + sayer + ",0," + mamoriat + "," + sayer_GhNaghdi + ",0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        var a = PersoneliService.GetPersoneliInfoWithFilter("fldCodemeli", q[i].fldCodemeli, Convert.ToInt32(Session["OrganId"]), 1, IP, out ErrPersoneli).FirstOrDefault();
                        var p = PayServic.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", q[i].fldPersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out ErrPay).FirstOrDefault();
                        var r = servic.GetRasteWithFilter("fldText", a.fldRasteShoghli, 0, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), out Err).FirstOrDefault();
                        raste = 13;
                        if (p != null)
                            WorkShopName = p.fldWorkShopName;
                        else
                            WorkShopName = "";
                        if (r != null && r.fldMaliyat != null)
                            raste = Convert.ToInt32(r.fldMaliyat);

                        Personeli[cc] = "1," + q[i].fldCodemeli + "," + q[i].fldName + "," + q[i].fldFamily + "," + q[i].fldFatherName + "," + a.fldTarikhTavalod.Replace("/", "") + "," + a.fldSh_Shenasname
                            + "," + a.fldNameMahalTavalod + "," + q[i].fldMadrakId + "," + nobime + "," + q[i].fldShomareBime + "," + WorkShopName + "," + code_moafi + ",103,103,"
                              + q[i].fldCodePosti.PadLeft(10, '0') + "," + q[i].fldAddress + "," + raste + "," + q[i].Semat + "," + noestekhdam + "," + q[i].fldTarikhEstekhdam.Replace("/", "") + ",,";

                        cc++;
                        //Personeli[cc] = "1,1," + q[i].fldCodemeli + "," + q[i].fldName + "," + q[i].fldFamily + ",103," + q[i].fldPersonalId + "," + q[i].fldMadrakId + "," + q[i].Semat
                        //    + "," + nobime + ",," + q[i].fldShomareBime + "," + q[i].fldCodePosti.PadLeft(10, '0') + "," + q[i].fldAddress + "," + q[i].fldTarikhEstekhdam.Replace("/", "") + "," + noestekhdam + "," +
                        //    q[i].MahalKhedmat + ",1,1,," + code_moafi + ",,";
                        cc++;
                    }
                }*/

                string nahve = ",,,,," + jammaliat + ",,";
                if (TypePardakht == 1 || TypePardakht == 5)
                {
                    nahve = Sh_Check + "," + DateCheck.Replace("/", "") + "," + Bank + "," + NameShobe + "," + Sh_Hesab + "," + jammaliat + ",,0";
                }
                string[] wk = new string[1];
                wk[0] = Year + "," + Month +
                   ",0,0," + DatePay.Replace("/", "") +
                   "," + (TypePardakht).ToString() + "," + nahve;
                
                FileName = "WH" + Year.ToString() + Month.ToString().PadLeft(2, '0') + ".TXT";
                FileName2 = "WP" + Year.ToString() + Month.ToString().PadLeft(2, '0') + ".TXT";
                //FileName3 = "WK" + Year.ToString() + Month.ToString().PadLeft(2, '0') + ".TXT";
                Passvand = ".TXT";
                Passvand2 = ".TXT";
                Passvand3 = ".TXT";
                savePath = Server.MapPath(@"~\Uploaded\" + FileName);
                savePath2 = Server.MapPath(@"~\Uploaded\" + FileName2);
                //savePath3 = Server.MapPath(@"~\Uploaded\" + FileName3);
                if (System.IO.File.Exists(savePath))
                {
                    System.IO.File.Delete(savePath);
                }
                if (System.IO.File.Exists(savePath2))
                {
                    System.IO.File.Delete(savePath2);
                }
                //if (System.IO.File.Exists(savePath3))
                //{
                //    System.IO.File.Delete(savePath3);
                //}
                System.IO.File.WriteAllLines(savePath,Disket);
                System.IO.File.WriteAllLines(savePath2, Personeli);
                //System.IO.File.WriteAllLines(savePath3, wk);
                /*MemoryStream disket_byte = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                MemoryStream Personeli_byte = new MemoryStream(System.IO.File.ReadAllBytes(savePath2));
                MemoryStream wk_byte = new MemoryStream(System.IO.File.ReadAllBytes(savePath3));*/
                Msg = "تهيه فایل با موفقيت به پايان رسيد.";
                MsgTitle = "تراكنش موفق";
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
                Er = Er,
                masir = savePath,
                masir2 = savePath2,
                masir3 = savePath3,
                FileName = FileName,
                FileName2 = FileName2,
                FileName3 = FileName3,
                Passvand = Passvand,
                Passvand2 = Passvand2,
                Passvand3 = Passvand3
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadExcel(string Year,string Month,string NobatePardakht)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.AfradTahtePooshesh();
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Year = Year;
            PartialView.ViewBag.Month = Month;
            PartialView.ViewBag.NobatePardakht = NobatePardakht;
            return PartialView;
        }
        public ActionResult CheckUploadFile(short Year, byte Month, byte? NobatPardakht,byte type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
           
            var k = PayServic.GetMohasebatWithFilter("CheckUploadFile", Year.ToString(), Month.ToString(), NobatPardakht.ToString(), Convert.ToInt32(Session["OrganId"]),type, IP, out ErrPay);

            return Json(new
            {
                AllowGenerate = k.Count() > 0 ? 0 : 1
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                if (Session["savePathMaliyat"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathMaliyat"].ToString());
                    Session.Remove("savePathMaliyat");
                    System.IO.File.Delete(physicalPath);
                }

                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                var FileNamee = Path.GetFileNameWithoutExtension(Request.Files[0].FileName) + Session["UserId"] + extension;
                HttpPostedFileBase file = Request.Files[0];
                string savePath = Server.MapPath(@"~\Uploaded\" + FileNamee);

                var IsExcel =  FileInfoExtensions.IsExcel2(file);
                
                if (IsExcel == true)
                {
                    //if (Request.Files[0].ContentLength <= 1048576)
                    //{
                    file.SaveAs(savePath);
                    Session["savePathMaliyat"] = savePath;
                    object r = new
                    {
                        success = true,
                        name = Request.Files[0].FileName,
                        Message = "فایل با موفقیت آپلود شد.",
                        IsValid = true
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    //}
                    //else
                    //{
                    //    object r = new
                    //    {
                    //        success = true,
                    //        name = Request.Files[0].FileName,
                    //        Message = "حجم فایل انتخابی می بایست کمتر از یک مگابایت باشد.",
                    //        IsValid = false
                    //    };
                    //    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    //}
                }
                else
                {
                    object r = new
                    {
                        success = true,
                        name = Request.Files[0].FileName,
                        Message = "فایل انتخاب شده معتبر نمی باشد.",
                        IsValid = false
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                }
            }
            catch (Exception x)
            {
                if (Session["savePathMaliyat"] != null)
                {
                    System.IO.File.Delete(Session["savePathMaliyat"].ToString());
                    Session.Remove("savePathMaliyat");
                }
                return null;
            }
        }

        public ActionResult ImportFile(short Year, byte Month, byte NobatePardakht)
        {
            DataTable dt = new DataTable();
            int UserId = Convert.ToInt32(Session["UserId"]), Er=0;
            string MsgTitle = "";
            string Msg = "";
            try
            {
               
                if (Session["savePathMaliyat"] != null)
                {
                    string path = Session["savePathMaliyat"].ToString();

                    PayServic.DeleteMaliyatDaraei(Year, Month, NobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                    if (ErrPay.ErrorType == true)
                    {
                        Msg = ErrPay.ErrorMsg;
                        MsgTitle = "خطا";
                        Er = 1;
                    }
                    else
                    {
                        dt = ProcessMaliyat(path, UserId, Year, Month, NobatePardakht);
                        string[] FeedBack = ProcessBulkCopyMaliyat(dt).Split(';');

                        if (FeedBack[0] == "ذخیره موفق")
                        {
                            PayServic.UpdateMaliyatDaraei(Year, Month, NobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                            if (ErrPay.ErrorType == true)
                            {
                                Msg = ErrPay.ErrorMsg;
                                MsgTitle = "خطا";
                                Er = 1;
                            }
                            else
                            {
                                Msg = FeedBack[1];
                                MsgTitle = FeedBack[0];
                            }
                        }
                        else
                        {
                            Msg = FeedBack[1];
                            MsgTitle = FeedBack[0];
                        }
                        System.IO.File.Delete(path);
                        Session["savePathMaliyat"] = null;
                       

                    }
                }
                else
                {
                    MsgTitle = "خطا";
                    Msg = "لطفا فایل مورد نظر را جهت آپلود انتخاب کنید";
                }
                
            }
            catch (Exception ex)
            {
                MsgTitle = "خطا";
                Msg = ex.Message;
            }

            dt.Dispose();

            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        private String ProcessBulkCopyMaliyat(DataTable dt)
        {
            string Feedback = string.Empty;
            string connString = ConfigurationManager.ConnectionStrings["RasaFMSPayRollDBConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (var copy = new SqlBulkCopy(connString, SqlBulkCopyOptions.CheckConstraints))
                {
                   
                    copy.DestinationTableName = "pay.tblMaliyatDaraei";
                    copy.ColumnMappings.Add("fldCodeMeli", "fldCodeMeli");
                    copy.ColumnMappings.Add("fldYear", "fldYear");
                    copy.ColumnMappings.Add("fldMonth", "fldMonth");
                    copy.ColumnMappings.Add("fldMaliyat", "fldMaliyat");
                    copy.ColumnMappings.Add("fldOrganId", "fldOrganId");
                    copy.ColumnMappings.Add("fldNobatePardakht", "fldNobatePardakht");
                    copy.ColumnMappings.Add("fldIp", "fldIp");
                    copy.ColumnMappings.Add("fldUserId", "fldUserId");
                    copy.ColumnMappings.Add("fldDate", "fldDate");

                    copy.BatchSize = dt.Rows.Count;
                    try
                    {
                        copy.WriteToServer(dt);

                        Feedback = "ذخیره موفق;ذخیره فایل با موفقیت انجام شد";
                    }
                    catch (Exception ex)
                    {
                        Feedback = "خطا" + ";" + ex.Message;
                    }
                }
            }

            return Feedback;
        }
        private DataTable ProcessMaliyat(string fileName, int UserId, short Year, byte Month, byte NobatePardakht)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("fldCodeMeli");
            dt.Columns.Add("fldYear").DefaultValue = Year;
            dt.Columns.Add("fldMonth").DefaultValue = Month;
            dt.Columns.Add("fldMaliyat");
            dt.Columns.Add("fldOrganId").DefaultValue = Convert.ToInt32(Session["OrganId"]); ;
            dt.Columns.Add("fldNobatePardakht").DefaultValue = NobatePardakht; 
            dt.Columns.Add("fldUserId").DefaultValue = UserId;
            dt.Columns.Add("fldIp").DefaultValue =IP;            
            dt.Columns.Add("fldDate").DefaultValue = DateTime.Now;

            Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(fileName);
            var sheet = wb.Worksheets[0];
            for (int i = sheet.Cells.MinDataRow + 1; i < sheet.Cells.MaxDataRow + 1; i++)
            {
                dt.Rows.Add();
                dt.Rows[dt.Rows.Count - 1][0] = sheet.Cells[i, 0].Value.ToString().PadLeft(10, '0');
                dt.Rows[dt.Rows.Count - 1][3] = sheet.Cells[i, 3].Value;
                //int count = 0;
                //for (int j = sheet.Cells.MinDataColumn; j < sheet.Cells.MinDataColumn + 1; j++)
                //{
                //    var str = sheet.Cells[i, j].StringValue;
                //    dt.Rows[dt.Rows.Count - 1][count] = str;
                //}
            }
            //List<string> ReadCSV = System.IO.File.ReadAllLines(fileName).ToList();
            //foreach (string csvRow in ReadCSV)
            //{
            //    if (!string.IsNullOrEmpty(csvRow))
            //    {
            //        dt.Rows.Add();
            //        int count = 0;
            //        foreach (string FileRec in csvRow.Split(','))
            //        {
            //            var FileRec1 = FileRec.Trim('"');
            //            dt.Rows[dt.Rows.Count - 1][count] = FileRec1;
            //            count++;
            //        }
            //    }

            //}
            return dt;
        }
        
       
        public ActionResult GeneratePDFMaliyat(string Sh_Check, string DateCheck, byte Bank, string NameShobe, string Sh_Hesab, short Year, string DatePay, byte Month, byte NobatePardakht, byte MarhalePardakht, string FieldName, byte TypePardakht,bool SayerMoafiyat,bool IsMotalebat)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; byte Er = 0; var FileName = ""; var Passvand = ""; string savePath = "";
            var FileName2 = ""; var Passvand2 = ""; string savePath2 = ""; var FileName3 = ""; var Passvand3 = ""; string savePath3 = "";
            var TIN = ""; var TFN = "";

            try
            {
                string noestekhdam = "", code_moafi = "", nobime = "", WorkShopName = "";
                int hoghogh_mostamar = 0, Bime = 0, sayer_Naghdi = 0, mamoriat = 0, ezafekar = 0, moavaghe = 0, sanavat = 0, jammaliat = 0, eidi = 0
                    , sayer_GhNaghdi = 0, sayer = 0, maliat = 0, maliat_moavaghe = 0, maliat_manfi = 0, raste = 0;
                long bonMostamar = 0, bonGheyrMostamar = 0, bonMostamarMoavagh = 0, bonGheyrMostamarMoavagh = 0, ezafeKhales = 0, TatilKhales = 0, KhalesEydi = 0, KhalesSayer = 0, KhalesGheyrMostamar = 0, KhalesMostamar = 0;
                int cc = 0;
                var q = PayServic.GetSelectVariziForBank(FieldName, "", Year, Month, NobatePardakht, MarhalePardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();

                var setting = PayServic.GetSettingWithFilter("fldOrganId", Session["OrganId"].ToString(), 0, 1, IP, out ErrPay).FirstOrDefault();
                TIN = setting.fldCodeEghtesadi;
                TFN = setting.fldCodeParvande;
                string[] Disket = new string[q.Count];
                string[] Personeli = new string[q.Count];

               // PayServic.UpdateFlag("KarKardMahane", Year, Month, NobatePardakht, MarhalePardakht, Convert.ToInt32(Session["OrganId"]), Convert.ToByte(4), Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrPay);

                //if (FieldName == "Hoghogh")
                //{
                var q1 = PayServic.GetSelectDisketMaliyatForHoghogh(Year, Month, NobatePardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
                Disket = new string[q1.Count];
                Personeli = new string[q1.Count];
                for (var j = 0; j < q1.Count; j++)
                {
                    if (q1[j].eydi == 0 || q1[j].eydi == null)
                    {
                        var e = PayServic.GetSelectVariziForBank("EydiPersonalId", "", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                        if (e != null)
                        {
                            eidi = e.fldMablagh;
                            KhalesEydi = e.fldkhalesPardakhti;
                        }
                        else
                        {
                            eidi = 0;
                            KhalesEydi = 0;
                        }
                    }
                    else
                    {
                        eidi = q1[j].eydi;
                        KhalesEydi = q1[j].eydi;
                    }

                    var b = PayServic.GetSelectVariziForBank("BonKart_MostamarPersonalId", "", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                    if (b != null)/*مبلغ قیمت تمام شده سایر مزایای مستمر غیرنقدی*/
                    {
                        bonMostamar = b.fldkhalesPardakhti;
                        bonMostamarMoavagh = b.fldMablagh;/*مبلغ حقوق و مزایای مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی*/
                    }
                    else
                    {
                        bonMostamar = 0;
                        bonMostamarMoavagh = 0;
                    }

                    b = PayServic.GetSelectVariziForBank("BonKart_GheyrMostamarPersonalId", "", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                    if (b != null)/*مبلغ قیمت تمام شده مزایای غیر مستمر غیرنقدی ماه جاری- ریالی*/
                    {
                        bonGheyrMostamar = b.fldkhalesPardakhti;
                        bonGheyrMostamarMoavagh = b.fldMablagh;/*مبلغ مزایای غیر مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی*/
                    }
                    else
                    {
                        bonGheyrMostamar = 0;
                        bonGheyrMostamarMoavagh = 0;
                    }

                    var ezafe = PayServic.GetSelectVariziForBank("EzafeKari_MaliyatPersonalId", "", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                    if (ezafe != null)
                    {
                        ezafekar = ezafe.fldMablagh;
                        ezafeKhales = ezafe.fldkhalesPardakhti;
                    }
                    else
                    {
                        ezafekar = 0;
                        ezafeKhales = 0;
                    }

                    var t = PayServic.GetSelectVariziForBank("TatilKari_MaliyatPersonalId", "", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                    if (t != null)
                    {
                        ezafekar = ezafekar + t.fldMablagh;
                        TatilKhales = t.fldkhalesPardakhti;
                    }
                    else
                        TatilKhales = 0;

                    if (ezafe == null && t == null)
                        ezafekar = 0;

                    var s = PayServic.GetSelectVariziForBank("SayerPardakht_MaliyatPersonalId", "", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                    if (s != null)
                    {
                        sayer = s.fldMablagh;
                        KhalesSayer = s.fldkhalesPardakhti;
                    }
                    else
                    {
                        sayer = 0;
                        KhalesSayer = 0;
                    }

                    var m = PayServic.GetSelectVariziForBank("KomakGheyerNaghdi_Mostamer_MaliyatPersonalId", "", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                    if (m != null)
                    {
                        sayer_Naghdi = m.fldMablagh;
                        KhalesMostamar = m.fldkhalesPardakhti;
                    }
                    else
                    {
                        sayer_Naghdi = 0;
                        KhalesMostamar = 0;
                    }

                    var g = PayServic.GetSelectVariziForBank("KomakGheyerNaghdi_GheyreMostamer_MaliyatPersonalId", "", Year, Month, NobatePardakht, MarhalePardakht, q[j].fldPayPersonalId, IP, out ErrPay).FirstOrDefault();
                    if (g != null)
                    {
                        sayer_GhNaghdi = m.fldMablagh;
                        KhalesGheyrMostamar = m.fldkhalesPardakhti;
                    }
                    else
                    {
                        sayer_GhNaghdi = 0;
                        KhalesGheyrMostamar = 0;
                    }

                    hoghogh_mostamar = q1[j].ItemEstekhdam;/*مبلغ جمع ناخالص حقوق و مزایای مستمر نقدی ماه جاری - ریالی*/
                    var ezafekarkol = q1[j].ezafekar + q1[j].tatilkari + ezafekar;
                    mamoriat = q1[j].mamoriat;
                    //sayer_Naghdi = q1[j].KolMotalebat + mamoriat;
                    sanavat = q1[j].s_payankhedmat;
                    maliat = q1[j].fldMaliyat;
                    var MaliyatDaraei = q1[j].fldMaliyatDaraei;
                    //double mashmol = maliat_(q1[j].fldMashmolMaliyat, q1[j].fldFiscalHeaderId);
                    moavaghe = q1[j].MashmolMaliyatMoavaghe;/*مبلغ حقوق و مزایای مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است - ریالی*/
                    maliat_moavaghe = q1[j].MaliyatMoavaghe;
                    Bime = q1[j].fldBimePersonal;
                    //maliat_manfi = q1[j].MaliyatManfi;
                    //if (maliat_manfi != 0 )
                    //    maliat = maliat + maliat_manfi;
                    //else
                    //    maliat = maliat + maliat_moavaghe;

                    noestekhdam = estekhdam_new(q1[j].fldNoeEstekhdam);
                    code_moafi = "";
                    code_moafi = q1[j].fldMaliyatEsargari.ToString();
                    /* switch (q1[j].fldEsargariId)
                     {
                         case 1:
                         case 6:
                         case 7:
                             code_moafi = "1";
                             break;
                         case 2:
                         case 4:
                         case 5:
                             code_moafi = "2";
                             break;
                         case 3:
                             code_moafi = "3";
                             break;

                     }*/
                    if (q1[j].fldTypeBimeId == 1)
                    {
                        nobime = "2";
                    }
                    else
                    {
                        nobime = "1";
                    }

                    jammaliat = jammaliat + q1[j].fldMaliyat;
                    int SayerMoafiat = 0, Motalebat = 0;
                    if (SayerMoafiyat == true)
                        SayerMoafiat = Convert.ToInt32(q1[j].SayerMoafiyat);
                    //if (IsMotalebat == true)
                    Motalebat = Convert.ToInt32(q1[j].KolMotalebat);/*سایر حقوق و مزایای غیر مستمر نقدی ماه جاری- ریالی*/
                    long khales = 0;
                    khales = q.Where(l => l.fldCodemeli == q1[j].fldCodemeli).FirstOrDefault().fldkhalesPardakhti;
                    khales += ezafeKhales + TatilKhales + KhalesEydi + KhalesGheyrMostamar + KhalesMostamar + KhalesSayer + bonGheyrMostamar + bonMostamar + bonGheyrMostamarMoavagh + bonMostamarMoavagh;

                    Disket[cc] = q1[j].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + bonMostamar + sayer_Naghdi + "," + bonMostamarMoavagh + ",0,0," + ezafekarkol + ",0," + mamoriat + "," + q1[j].karane
                         + "," + sayer + ",0," + eidi + ",0,0,0," + sanavat + ",0," + Motalebat + ",0," + bonGheyrMostamar + sayer_GhNaghdi + "," + bonGheyrMostamarMoavagh + "," +
                        +Bime + "," + q1[j].fldBimeOmrPersonal + "," + khales+","+maliat+","+MaliyatDaraei;


                    //Disket[cc] = q1[j].fldCodemeli + ",1," + Month + ",0,85,0," +
                    //q1[j].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                    //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + Motalebat + ",0," + Bime + ",0," + SayerMoafiat + "," + ezafekar + "," +
                    //sayer_Naghdi + ",0,0," + mamoriat + ",0,0," + sanavat + ",0,0,0," + maliat + "," + maliat;
                    if (q1[j].fldMadrakId == 7)
                        q1[j].fldMadrakId = 1;
                    var a = PersoneliService.GetPersoneliInfoWithFilter("fldCodemeli", q1[j].fldCodemeli, Convert.ToInt32(Session["OrganId"]), 1, IP, out ErrPersoneli).FirstOrDefault();
                    var p = PayServic.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", q1[j].fldPersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out ErrPay).FirstOrDefault();
                    var r = servic.GetRasteWithFilter("fldText", a.fldRasteShoghli, 0, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), out Err).FirstOrDefault();
                    raste = 13;
                    if (p != null)
                        WorkShopName = p.fldWorkShopName;
                    else
                        WorkShopName = "";
                    if (r != null && r.fldMaliyat != null)
                        raste = Convert.ToInt32(r.fldMaliyat);
                    Personeli[cc] = "1," + q1[j].fldCodemeli + "," + q1[j].fldName + "," + q1[j].fldFamily + "," + q1[j].fldFatherName + "," + a.fldTarikhTavalod.Replace("/", "") + "," + a.fldSh_Shenasname
                        + "," + a.fldNameMahalTavalod + "," + q1[j].fldMadrakId + "," + nobime + "," + q1[j].fldShomareBime.PadLeft(10, '0') + "," + WorkShopName + "," + code_moafi + ",103,103,"
                        + q1[j].fldCodePosti.PadLeft(10, '0') + "," + q1[j].fldAddress + "," + raste + "," + q1[j].Semat + "," + noestekhdam + "," + q1[j].fldTarikhEstekhdam.Replace("/", "") + ",,";

                    cc++;
                    //Personeli[cc] = "1,1," + q1[j].fldCodemeli + "," + q1[j].fldName + "," + q1[j].fldFamily + ",103," + q1[j].fldPersonalId + "," + q1[j].fldMadrakId + "," + q1[j].Semat
                    //    + "," + nobime + ",," + q1[j].fldShomareBime + "," + q1[j].fldCodePosti.PadLeft(10, '0') + "," + q1[j].fldAddress + "," + q1[j].fldTarikhEstekhdam.Replace("/", "") + "," + noestekhdam + "," +
                    //    q1[j].MahalKhedmat + ",1,1,," + code_moafi + ",,";
                    //cc++;
                }
               // }
                /*else
                {
                    for (int i = 0; i < q.Count; i++)
                    {
                        if (FieldName == "Eydi")
                        {
                            eidi = q[i].fldMablagh;
                        }
                        else if (FieldName == "BonKart")
                        {
                            bon = q[i].fldMablagh;
                        }
                        else if (FieldName == "EzafeKari" || FieldName == "TatilKari")
                        {
                            ezafekar = q[i].fldMablagh;
                        }
                        else if (FieldName == "SayerPardakht")
                        {
                            sayer = q[i].fldMablagh;
                        }
                        else if (FieldName == "KomakGheyerNaghdi_Mostamer" || FieldName == "KomakGheyerNaghdi_GheyreMostamer")
                        {
                            sayer_GhNaghdi = q[i].fldMablagh;
                        }
                        noestekhdam = estekhdam_new(q[i].fldNoeEstekhdam);
                        code_moafi = "";
                        // switch (q[i].fldEsargariId)
                        // {
                        //     case 1:
                        //     case 6:
                        //     case 7:
                        //         code_moafi = "1";
                        //         break;
                        //     case 2:
                        //     case 4:
                        //     case 5:
                        //         code_moafi = "2";
                        //         break;
                        //     case 3:
                        //         code_moafi = "3";
                        //         break;

                        // }
                        code_moafi = q[i].fldMaliyatEsargari.ToString();
                        if (q[i].fldTypeBimeId == 1)
                        {
                            nobime = "2";
                        }
                        else
                        {
                            nobime = "1";
                        }
                        jammaliat = jammaliat + q[i].fldMaliyat;
                        if (FieldName == "BonKart")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + q[i].fldMablagh + ",0,0,0," + ezafekar + ",0," + mamoriat
                             + ",0,0,0,0,0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0,0,0," +
                             +Bime + ",0," + q[i].fldkhalesPardakhti;


                        }
                        else if (FieldName == "Eydi")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0," + ezafekar + ",0," + mamoriat
                             + ",0,0,0," + q[i].fldMablagh + ",0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0,0,0," +
                            +Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + ",0,0," + mamoriat + ",0," + q[i].fldMablagh + "," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "EzafeKari")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0," + ezafekar + ",0," + mamoriat
                             + ",0,0,0,0,0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0,0,0," +
                            Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + ",0,0," + mamoriat + ",0,0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "TatilKari")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + sayer_GhNaghdi + ",0,0,0," + ezafekar + ",0," + mamoriat
                             + ",0,0,0,0,0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0,0,0," +
                             Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + "," + sayer + ",0," + mamoriat + "," + sayer_GhNaghdi + ",0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "KomakGheyerNaghdi_Mostamer")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + sayer_GhNaghdi + ",0,0,0," + ezafekar + ",0," + mamoriat + ",0,"
                             + ",0,0,0,0,0,0," + sanavat + ",0,0,0,0,0," +
                             Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + sayer_GhNaghdi + ",0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + ",0,0," + mamoriat + ",0,0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "KomakGheyerNaghdi_GheyreMostamer")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0," + ezafekar + ",0," + mamoriat + ",0,"
                            + ",0,0,0,0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0," + sayer_GhNaghdi + ",0," +
                           +Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + ",0,0," + mamoriat + "," + sayer_GhNaghdi + ",0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "SayerPardakht")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1,1,1,84,0," + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + sayer_GhNaghdi + ",0,0,0," + ezafekar + ",0," + mamoriat + ",0,"
                          + sayer + ",0,0,0,0,0," + sanavat + ",0," + sayer_Naghdi + ",0," + sayer_Naghdi + ",0," +
                           Bime + ",0," + q[i].fldkhalesPardakhti;

                            //Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            //q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            //+ hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            //sayer_Naghdi + "," + sayer + ",0," + mamoriat + "," + sayer_GhNaghdi + ",0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        var a = PersoneliService.GetPersoneliInfoWithFilter("fldCodemeli", q[i].fldCodemeli, Convert.ToInt32(Session["OrganId"]), 1, IP, out ErrPersoneli).FirstOrDefault();
                        var p = PayServic.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", q[i].fldPersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out ErrPay).FirstOrDefault();
                        var r = servic.GetRasteWithFilter("fldText", a.fldRasteShoghli, 0, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), out Err).FirstOrDefault();
                        raste = 13;
                        if (p != null)
                            WorkShopName = p.fldWorkShopName;
                        else
                            WorkShopName = "";
                        if (r != null && r.fldMaliyat != null)
                            raste = Convert.ToInt32(r.fldMaliyat);

                        Personeli[cc] = "1," + q[i].fldCodemeli + "," + q[i].fldName + "," + q[i].fldFamily + "," + q[i].fldFatherName + "," + a.fldTarikhTavalod.Replace("/", "") + "," + a.fldSh_Shenasname
                            + "," + a.fldNameMahalTavalod + "," + q[i].fldMadrakId + "," + nobime + "," + q[i].fldShomareBime + "," + WorkShopName + "," + code_moafi + ",103,103,"
                              + q[i].fldCodePosti.PadLeft(10, '0') + "," + q[i].fldAddress + "," + raste + "," + q[i].Semat + "," + noestekhdam + "," + q[i].fldTarikhEstekhdam.Replace("/", "") + ",,";

                        cc++;
                        //Personeli[cc] = "1,1," + q[i].fldCodemeli + "," + q[i].fldName + "," + q[i].fldFamily + ",103," + q[i].fldPersonalId + "," + q[i].fldMadrakId + "," + q[i].Semat
                        //    + "," + nobime + ",," + q[i].fldShomareBime + "," + q[i].fldCodePosti.PadLeft(10, '0') + "," + q[i].fldAddress + "," + q[i].fldTarikhEstekhdam.Replace("/", "") + "," + noestekhdam + "," +
                        //    q[i].MahalKhedmat + ",1,1,," + code_moafi + ",,";
                        cc++;
                    }
                }*/

                

                FileName = "WH140202.TXT";// + Year.ToString() + Month.ToString().PadLeft(2, '0') + ".TXT";
                
                Passvand = ".TXT";
              
                savePath = Server.MapPath(@"~\Uploaded\" + FileName);

                if (System.IO.File.Exists(savePath))
                {
                    System.IO.File.Delete(savePath);
                }
                
                System.IO.File.WriteAllLines(savePath, Disket);
               FastReport.Report Report = new FastReport.Report();
               Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptDisketMaliyat.frx");
               //DataSet.DataSet1 dt = new DataSet.DataSet1();
               //DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
               //Report.RegisterData(dt, "rasaFMSPayRoll");
               //Report.RegisterData(dt_Com, "rasaFMSCommon");
               Report.SetParameterValue("FileName", savePath);
                

                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                FastReport.Export.OoXML.Excel2007Export xls = new FastReport.Export.OoXML.Excel2007Export();
                //Report.Export(pdf, stream);
                Report.Export(xls, stream);
                return File(stream.ToArray(), "xls", "لیست مالیات.xls");
                //return File(stream.ToArray(), "application/xls");
                //return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
           
        public ActionResult GenerateExcelMaliyat(string Sh_Check, string DateCheck, byte Bank, string NameShobe, string Sh_Hesab, short Year, string DatePay, byte Month, byte NobatePardakht, byte MarhalePardakht, string FieldName, byte TypePardakht, string NahvePardakht)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
                                 , "AA", "AB", "AC","AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP","AQ",};
            var StatusCheck = alpha; var Check = "";
            int index = 0;
            string Msg = ""; string MsgTitle = ""; byte Er = 0; var FileName = ""; var Passvand = ""; string savePath = "";
            var FileName2 = ""; var Passvand2 = ""; string savePath2 = ""; var FileName3 = ""; var Passvand3 = ""; string savePath3 = "";
            var TIN = ""; var TFN = "";

            try
            {
                string noestekhdam = "", code_moafi = "", nobime = "";
                int hoghogh_mostamar = 0, Bime = 0, sayer_Naghdi = 0, mamoriat = 0, ezafekar = 0, moavaghe = 0, sanavat = 0, jammaliat = 0, eidi = 0,bon=0, sayer_GhNaghdi = 0, sayer = 0, maliat = 0, maliat_moavaghe = 0, maliat_manfi = 0;
                int cc = 0;
                var q = PayServic.GetSelectVariziForBank(FieldName,"", Year, Month, NobatePardakht, MarhalePardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
                var setting = PayServic.GetSettingWithFilter("fldOrganId", Session["OrganId"].ToString(), 0, 1, IP, out ErrPay).FirstOrDefault();
                TIN = setting.fldCodeEghtesadi;
                TFN = setting.fldCodeParvande;
                string[] Disket = new string[q.Count];
                string[] Personeli = new string[q.Count];
                string madrak = "";
                if (FieldName == "Hoghogh")
                {
                    var q1 = PayServic.GetSelectDisketMaliyatForHoghogh(Year, Month, NobatePardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
                    Disket = new string[q1.Count];
                    Personeli = new string[q1.Count];
                  
                    Workbook wb = new Workbook();
                    wb.Worksheets.Clear();

                    Worksheet sheet = wb.Worksheets.Add(("لیست پرسنل"));
                    Check = "آیدی پرسنل";
                    Cell cell = sheet.Cells[alpha[index] + "1"];
                    cell.PutValue(Check);
                    int i = 0;
                    index = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                        Cell.PutValue(_item.fldPersonalId);
                        i++;
                    }
                    index++;
                    Cell cell1 = sheet.Cells[alpha[index] + "1"];
                    Check = "نام خانوادگی_نام";
                    cell1.PutValue(Check);
                    int j1 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (j1 + 2)];
                        Cell.PutValue(_item.fldName+"_"+_item.fldFamily);
                        j1++;
                    }
                    index++;
                    Check = "نام پدر";
                    Cell cell2 = sheet.Cells[alpha[index] + "1"];
                    cell2.PutValue(Check);
                    int k3 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (k3 + 2)];
                        Cell.PutValue(_item.fldFatherName);
                        k3++;
                    }
                    index++;
                    Check = "کدملی";
                    Cell cell3 = sheet.Cells[alpha[index] + "1"];
                    cell3.PutValue(Check);
                    int k2 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (k2 + 2)];
                        Cell.PutValue(_item.fldCodemeli);
                        k2++;
                    }
                    index++;
                    Check = "مدرک";
                    Cell cell4 = sheet.Cells[alpha[index] + "1"];
                    cell4.PutValue(Check);
                    int j2 = 0;
                    foreach (var _item in q1)
                    {
                        switch (_item.fldMadrakId)
                        {
                            case 1:
                                madrak = "زیردیپلم";
                                break;
                            case 2:
                                madrak = "دیپلم";
                                break;
                            case 3:
                                madrak = "فوق دیپلم";
                                break;
                            case 4:
                                madrak = "کارشناسی";
                                break;
                            case 5:
                                madrak = "کارشناسی ارشد";
                                break;
                            case 6:
                                madrak = "دکتری";
                                break;
                            case 7:
                                madrak = "فوق دکتری";
                                break;

                        }
                        Cell Cell = sheet.Cells[alpha[index] + (j2 + 2)];
                        Cell.PutValue(madrak);
                        j2++;
                    }
                    index++;
                    Check = "سمت";
                    Cell cell5 = sheet.Cells[alpha[index] + "1"];
                    cell5.PutValue(Check);
                    int k = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                        Cell.PutValue(_item.Semat);
                        k++;
                    }
                    index++;
                    Check = "نوع بیمه";
                    Cell cell6 = sheet.Cells[alpha[index] + "1"];
                    cell6.PutValue(Check);
                    int q2 = 0;
                    foreach (var _item in q1)
                    {
                        if (_item.fldTypeBimeId == 1)
                        {
                            nobime = "تامین اجتماعی";
                        }
                        else
                        {
                            nobime = "خدمات درمانی";
                        }
                        Cell Cell = sheet.Cells[alpha[index] + (q2 + 2)];
                        Cell.PutValue(nobime);
                        q2++;
                    }
                    index++;
                    Check = "شماره بیمه";
                    Cell cell7 = sheet.Cells[alpha[index] + "1"];
                    cell7.PutValue(Check);
                    int w = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                        Cell.PutValue(_item.fldShomareBime);
                        w++;
                    }
                    index++;
                    Check = "کدپستی";
                    Cell cell8 = sheet.Cells[alpha[index] + "1"];
                    cell8.PutValue(Check);
                    int z = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                        Cell.PutValue(_item.fldCodePosti);
                        z++;
                    }
                    index++;
                    Check = "آدرس";
                    Cell cell9 = sheet.Cells[alpha[index] + "1"];
                    cell9.PutValue(Check);
                    int x = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
                        Cell.PutValue(_item.fldAddress );
                        x++;
                    }
                    index++;
                    Check = "تاریخ استخدام";
                    Cell cell10 = sheet.Cells[alpha[index] + "1"];
                    cell10.PutValue(Check);
                    int y = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
                        Cell.PutValue(_item.fldTarikhEstekhdam);
                        y++;
                    }
                    index++;
                    Check = "نوع استخدام";
                    Cell cell11 = sheet.Cells[alpha[index] + "1"];
                    cell11.PutValue(Check);
                    int y1 = 0;
                    foreach (var _item in q1)
                    {
                         noestekhdam = estekhdam_new(_item.fldNoeEstekhdam);
                        Cell Cell = sheet.Cells[alpha[index] + (y1 + 2)];
                        Cell.PutValue(noestekhdam);
                        y1++;
                    }
                    index++;
                     Check = "محل خدمت";
                    Cell cell12 = sheet.Cells[alpha[index] + "1"];
                    cell12.PutValue(Check);
                    int y2 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y2 + 2)];
                        Cell.PutValue(_item.MahalKhedmat);
                        y2++;
                    }
                    index++;
                     Check = "کد معافی";
                    Cell cell13 = sheet.Cells[alpha[index] + "1"];
                    cell13.PutValue(Check);
                    int y3 = 0;
                    foreach (var _item in q1)
                    {
                         code_moafi = "";
                         switch (_item.fldEsargariId)
                        {
                            case 1:
                            case 6:
                            case 7:
                                code_moafi = "عادی";
                                break;
                            case 2:
                            case 4:
                            case 5:
                                code_moafi = "جانباز";
                                break;
                            case 3:
                                code_moafi = "فرزند شهید";
                                break;

                        }
                        Cell Cell = sheet.Cells[alpha[index] + (y3 + 2)];
                        Cell.PutValue(code_moafi );
                        y3++;
                    }
                    index++;

                     sheet = wb.Worksheets.Add("لیست اقلام حقوق و دستمزد");

                     index = 0;
                     Check = "کدملی";
                     Cell cell17 = sheet.Cells[alpha[index] + "1"];
                     cell17.PutValue(Check);
                     int k22 = 0;
                     foreach (var _item in q1)
                     {
                         Cell Cell = sheet.Cells[alpha[index] + (k22 + 2)];
                         Cell.PutValue(_item.fldCodemeli);
                         k22++;
                     }
                     index++;
                    Check = "نوع پرداخت";
                    Cell cell14 = sheet.Cells[alpha[index] + "1"];
                    cell14.PutValue(Check);
                    int i1 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (i1 + 2)];
                        Cell.PutValue("ریالی");
                        i1++;
                    }
                    index++;
                    Cell cell15 = sheet.Cells[alpha[index] + "1"];
                    Check = "تعداد ماه های کارکرد واقعی از ابتدای سال جاری";
                    cell15.PutValue(Check);
                    int j11 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (j11 + 2)];
                        Cell.PutValue(Month);
                        j11++;
                    }
                    index++;
                    Check = "آخرین ماه فعالیت کاری حقوق بگیر";
                    Cell cell16 = sheet.Cells[alpha[index] + "1"];
                    cell16.PutValue(Check);
                    int k33= 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (k33 + 2)];
                        Cell.PutValue(0);
                        k33++;
                    }
                    index++;
                    
                    Check = "نوع ارز";
                    Cell cell18 = sheet.Cells[alpha[index] + "1"];
                    cell18.PutValue(Check);
                    int j22 = 0;
                    foreach (var _item in q1)
                    {
                        
                        Cell Cell = sheet.Cells[alpha[index] + (j22 + 2)];
                        Cell.PutValue("ریال ایران");
                        j22++;
                    }
                    index++;
                    Check = "نرخ تسعیر ارز";
                    Cell cell19 = sheet.Cells[alpha[index] + "1"];
                    cell19.PutValue(Check);
                    int k222 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (k222 + 2)];
                        Cell.PutValue(0);
                        k222++;
                    }
                    index++;
                    Check = "تاریخ شروع به کار";
                    Cell cell20 = sheet.Cells[alpha[index] + "1"];
                    cell20.PutValue(Check);
                    int q22 = 0;
                    foreach (var _item in q1)
                    {
                        
                        Cell Cell = sheet.Cells[alpha[index] + (q22 + 2)];
                        Cell.PutValue(_item.fldTarikhEstekhdam);
                        q22++;
                    }
                    index++;
                    Check = "تاریخ پایان کار";
                    Cell cell21 = sheet.Cells[alpha[index] + "1"];
                    cell21.PutValue(Check);
                    int w1 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (w1 + 2)];
                        Cell.PutValue("");
                        w1++;
                    }
                    index++;
                    Check = "وضعیت کارمند";
                    Cell cell23 = sheet.Cells[alpha[index] + "1"];
                    cell23.PutValue(Check);
                    int y13 = 0;
                    foreach (var _item in q1)
                    {
                        code_moafi = "";
                        switch (_item.fldEsargariId)
                        {
                            case 1:
                            case 6:
                            case 7:
                                code_moafi = "عادی";
                                break;
                            case 2:
                            case 4:
                            case 5:
                                code_moafi = "جانباز";
                                break;
                            case 3:
                                code_moafi = "فرزند شهید";
                                break;

                        }
                        Cell Cell = sheet.Cells[alpha[index] + (y13 + 2)];
                        Cell.PutValue(code_moafi);
                        y13++;
                    }
                    index++;
                    Check = "وضعیت محل خدمت";
                    Cell cell24 = sheet.Cells[alpha[index] + "1"];
                    cell24.PutValue(Check);
                    int y12 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y12 + 2)];
                        Cell.PutValue("عادی");
                        y12++;
                    }
                    index++;
                    Check = "ناخالص حقوق و دستمزد";
                    Cell cell25 = sheet.Cells[alpha[index] + "1"];
                    cell25.PutValue(Check);
                    int x1 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (x1 + 2)];
                        Cell.PutValue(_item.ItemEstekhdam);
                        x1++;
                    }
                    index++;
                    Check = "معوق مالیات";
                    Cell cell26 = sheet.Cells[alpha[index] + "1"];
                    cell26.PutValue(Check);
                    int y11 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y11 + 2)];
                        Cell.PutValue(_item.MashmolMaliyatMoavaghe);
                        y11++;
                    }
                    index++;
                    Check = "مسکن";
                    Cell cell27 = sheet.Cells[alpha[index] + "1"];
                    cell27.PutValue(Check);
                    int y14 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y14 + 2)];
                        Cell.PutValue("ندارد");
                        y14++;
                    }
                    index++;
                    Check = "مبلغ کسر شده بابت مسکن";
                    Cell cell28 = sheet.Cells[alpha[index] + "1"];
                    cell28.PutValue(Check);
                    int y15 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y15 + 2)];
                        Cell.PutValue(0);
                        y15++;
                    }
                    index++;
                    Check = "وسیله نقلیه";
                    Cell cell29 = sheet.Cells[alpha[index] + "1"];
                    cell29.PutValue(Check);
                    int y16 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y16 + 2)];
                        Cell.PutValue("ندارد");
                        y16++;
                    }
                    index++;
                    Check = "مبلغ کسر شده بابت وسیله نقلیه";
                    Cell cell30 = sheet.Cells[alpha[index] + "1"];
                    cell30.PutValue(Check);
                    int y17 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y17 + 2)];
                        Cell.PutValue(0);
                        y17++;
                    }
                    index++;
                    Check = "پرداخت مزایای مستمر غیر نقدی";
                    Cell cell31 = sheet.Cells[alpha[index] + "1"];
                    cell31.PutValue(Check);
                    int y18 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y18 + 2)];
                        Cell.PutValue(0);
                        y18++;
                    }
                    index++;
                    Check = " هزینه های درمانی";
                    Cell cell32 = sheet.Cells[alpha[index] + "1"];
                    cell32.PutValue(Check);
                    int y19 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y19 + 2)];
                        Cell.PutValue(0);
                        y19++;
                    }
                    index++;
                    Check = " حق بیمه پرداختی";
                    Cell cell33 = sheet.Cells[alpha[index] + "1"];
                    cell33.PutValue(Check);
                    int y20 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y20 + 2)];
                        Cell.PutValue(_item.fldBimePersonal);
                        y20++;
                    }
                    index++;
                    Check = "تسهیلات اعتباری مسکن از بانک ها";
                    Cell cell34 = sheet.Cells[alpha[index] + "1"];
                    cell34.PutValue(Check);
                    int y21 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y21 + 2)];
                        Cell.PutValue(0);
                        y21++;
                    }
                    Check = " سایر معافیت ها";
                    Cell cell35 = sheet.Cells[alpha[index] + "1"];
                    cell35.PutValue(Check);
                    int y22 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y22 + 2)];
                        Cell.PutValue(0);
                        y22++;
                    }
                    index++;
                    Check = "ناخالص اضافه کاری";
                    Cell cell36 = sheet.Cells[alpha[index] + "1"];
                    cell36.PutValue(Check);
                    int y23 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y23 + 2)];
                        Cell.PutValue(_item.ezafekar + _item.tatilkari);
                        y23++;
                    }
                    index++;
                    Check = "سایر پرداخت های غیر مستمر نقدی";
                    Cell cell37 = sheet.Cells[alpha[index] + "1"];
                    cell37.PutValue(Check);
                    int y24 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y24 + 2)];
                        Cell.PutValue(_item.KolMotalebat + _item.mamoriat);//sayer_Naghdi
                        y24++;
                    }
                    index++;
                    Check = "پاداش های موردی";
                    Cell cell38 = sheet.Cells[alpha[index] + "1"];
                    cell38.PutValue(Check);
                    int y25 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y25 + 2)];
                        Cell.PutValue(0);
                        y25++;
                    }
                    index++;

                    Check = " پرداختهای غیر مستمر نقدی معوقه";
                    Cell cell39 = sheet.Cells[alpha[index] + "1"];
                    cell39.PutValue(Check);
                    int y26 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y26 + 2)];
                        Cell.PutValue(0);
                        y26++;
                    }
                    index++;
                    Check = "معافیتهای غیر مستمر نقدی";
                    Cell cell40 = sheet.Cells[alpha[index] + "1"];
                    cell40.PutValue(Check);
                    int y27 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y27 + 2)];
                        Cell.PutValue(_item.mamoriat);
                        y27++;
                    }
                    index++;
                    Check = "پرداخت مزایای غیر مستمر غیر نقدی";
                    Cell cell41 = sheet.Cells[alpha[index] + "1"];
                    cell41.PutValue(Check);
                    int y28 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y28 + 2)];
                        Cell.PutValue(0);
                        y28++;
                    }
                    index++; 
                    Check = " عیدی و مزایای پایان سال";
                    Cell cell42 = sheet.Cells[alpha[index] + "1"];
                    cell42.PutValue(Check);
                    int y29 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y29 + 2)];
                        Cell.PutValue(0);
                        y29++;
                    }
                    index++;
                    Check = "بازخرید مرخصی و بازخرید سنوات";
                    Cell cell43 = sheet.Cells[alpha[index] + "1"];
                    cell43.PutValue(Check);
                    int y30 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y30 + 2)];
                        Cell.PutValue(sanavat);
                        y30++;
                    }
                    index++;
                    Check = "معافیت";
                    Cell cell44 = sheet.Cells[alpha[index] + "1"];
                    cell44.PutValue(Check);
                    int y31 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y31 + 2)];
                        Cell.PutValue(0);
                        y31++;
                    }
                     index++;
                    Check = " معافیت مربوط به مناطق آزاد تجاری";
                    Cell cell45 = sheet.Cells[alpha[index] + "1"];
                    cell45.PutValue(Check);
                    int y32 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y32 + 2)];
                        Cell.PutValue(0);
                        y32++;
                    }
                     index++;
                    Check = "معافیت موضوع قانون اجتناب از اخذ مالیات مضاعف";
                    Cell cell46 = sheet.Cells[alpha[index] + "1"];
                    cell46.PutValue(Check);
                    int y33 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y33 + 2)];
                        Cell.PutValue(0);
                        y31++;
                    }
                     index++;
                    Check = "مالیات";
                    Cell cell47 = sheet.Cells[alpha[index] + "1"];
                    cell47.PutValue(Check);
                    int y34 = 0;
                    foreach (var _item in q1)
                    {
                        jammaliat = jammaliat + _item.fldMaliyat;
                        Cell Cell = sheet.Cells[alpha[index] + (y34 + 2)];
                        Cell.PutValue(_item.fldMaliyat );
                        y34++;
                    }
                     index++;
                    Check = "مالیات";
                    Cell cell48 = sheet.Cells[alpha[index] + "1"];
                    cell48.PutValue(Check);
                    int y35 = 0;
                    foreach (var _item in q1)
                    {
                        Cell Cell = sheet.Cells[alpha[index] + (y35 + 2)];
                        Cell.PutValue(_item.fldMaliyat);
                        y35++;
                    }
                    index++;
                    
                      sheet = wb.Worksheets.Add(("خالصه لیست"));

                      index = 0;
                     Check = "سال";
                     Cell cell49 = sheet.Cells[alpha[index] + "1"];
                     cell49.PutValue(Check);
                     int k23 = 0;
                         Cell Cell1 = sheet.Cells[alpha[index] + (k23 + 2)];
                         Cell1.PutValue(Year);
                         k23++;
                     index++;
                    Check = "ماه";
                    Cell cell50 = sheet.Cells[alpha[index] + "1"];
                    cell50.PutValue(Check);
                    int i11 = 0;
                        Cell Cell2 = sheet.Cells[alpha[index] + (i11 + 2)];
                        Cell2.PutValue(Month);
                        i11++;
                    index++;
                    Cell cell51 = sheet.Cells[alpha[index] + "1"];
                    Check = " بدهی مالیاتی ماه جاری";
                    cell51.PutValue(Check);
                    int j12 = 0;
                        Cell Cell3 = sheet.Cells[alpha[index] + (j12 + 2)];
                        Cell3.PutValue(0);
                        j12++;
                    index++;
                    Check = "بدهی مالیاتی ماه گذشته";
                    Cell cell52 = sheet.Cells[alpha[index] + "1"];
                    cell52.PutValue(Check);
                    int k34= 0;
                        Cell Cell4 = sheet.Cells[alpha[index] + (k34 + 2)];
                        Cell4.PutValue(0);
                        k34++;
                    index++;
                    
                    Check = "تاریخ پرداخت";
                    Cell cell53 = sheet.Cells[alpha[index] + "1"];
                    cell53.PutValue(Check);
                    int j23 = 0;
                        Cell Cell5 = sheet.Cells[alpha[index] + (j23 + 2)];
                        Cell5.PutValue(DatePay);
                        j23++;
                    index++;
                    Check = "نحوه پرداخت";
                    Cell cell54 = sheet.Cells[alpha[index] + "1"];
                    cell54.PutValue(Check);
                    int k223 = 0;   
                    Cell Cell6 = sheet.Cells[alpha[index] + (k223 + 2)];
                        Cell6.PutValue(NahvePardakht);
                        k223++;
                    index++;
                    Check = "شماره سریال چک";
                    Cell cell55 = sheet.Cells[alpha[index] + "1"];
                    cell55.PutValue(Check);
                    int q223 = 0;
                     Cell Cell7 = sheet.Cells[alpha[index] + (q223 + 2)];
                        Cell7.PutValue(Sh_Check);
                        q223++;
                    index++;
                    Check = "تاریخ چک";
                    Cell cell56 = sheet.Cells[alpha[index] + "1"];
                    cell56.PutValue(Check);
                    int w2 = 0;
                        Cell Cell8 = sheet.Cells[alpha[index] + (w2 + 2)];
                        Cell8.PutValue(DateCheck);
                        w2++;
                    index++;
                    Check = "کد نام بانک";
                    Cell cell57 = sheet.Cells[alpha[index] + "1"];
                    cell57.PutValue(Check);
                    int y131 = 0;
                        Cell Cell9 = sheet.Cells[alpha[index] + (y131 + 2)];
                        Cell9.PutValue(Bank);
                        y131++;
                    index++;
                    Check = " نام شعبه";
                    Cell cell58 = sheet.Cells[alpha[index] + "1"];
                    cell58.PutValue(Check);
                    int y132 = 0;
                        Cell Cell01 = sheet.Cells[alpha[index] + (y132 + 2)];
                        Cell01.PutValue(NameShobe);
                        y132++;
                    index++;
                    Check = "شماره حساب";
                    Cell cell59 = sheet.Cells[alpha[index] + "1"];
                    cell59.PutValue(Check);
                    int y133 = 0;
                    Cell Cell02 = sheet.Cells[alpha[index] + (y133 + 2)];
                        Cell02.PutValue(Sh_Hesab);
                        y133++;
                    index++;
                    Check = " مبلغ پرداختی";
                    Cell cell60 = sheet.Cells[alpha[index] + "1"];
                    cell60.PutValue(Check);
                    int y134 = 0;
                        Cell Cell03 = sheet.Cells[alpha[index] + (y134 + 2)];
                        Cell03.PutValue(jammaliat);
                        y134++;
                    index++;
                    Check = "تاریخ پرداخت خزانه";
                    Cell cell61 = sheet.Cells[alpha[index] + "1"];
                    cell61.PutValue(Check);
                    int y135 = 0;
                        Cell Cell04 = sheet.Cells[alpha[index] + (y135 + 2)];
                        Cell04.PutValue("");
                        y135++;
                    index++;
                    Check = " مبلغ پرداخت خزانه";
                    Cell cell62 = sheet.Cells[alpha[index] + "1"];
                    cell62.PutValue(Check);
                    int y136 = 0;
                    Cell Cell05 = sheet.Cells[alpha[index] + (y136 + 2)];
                        Cell05.PutValue(0);
                        y136++;
                        index++;
                    MemoryStream stream = new MemoryStream();
                    wb.Save(stream, SaveFormat.Excel97To2003);
                    return File(stream.ToArray(), "xls", "لیست مالیات.xls");
                }
                else
                {
                    for (int i = 0; i < q.Count; i++)
                    {
                        if (FieldName == "Eydi")
                        {
                            eidi = q[i].fldMablagh;
                        }
                        else if (FieldName == "BonKart")
                        {
                            bon = q[i].fldMablagh;
                        }
                        else if (FieldName == "EzafeKari" || FieldName == "TatilKari")
                        {
                            ezafekar = q[i].fldMablagh;
                        }
                        else if (FieldName == "SayerPardakht")
                        {
                            sayer = q[i].fldMablagh;
                        }
                        else if (FieldName == "KomakGheyerNaghdi_Mostamer" || FieldName == "KomakGheyerNaghdi_GheyreMostamer")
                        {
                            sayer_GhNaghdi = q[i].fldMablagh;
                        }
                        noestekhdam = estekhdam_new(q[i].fldNoeEstekhdam);
                        code_moafi = "";
                        switch (q[i].fldEsargariId)
                        {
                            case 1:
                            case 6:
                            case 7:
                                code_moafi = "1";
                                break;
                            case 2:
                            case 4:
                            case 5:
                                code_moafi = "2";
                                break;
                            case 3:
                                code_moafi = "3";
                                break;

                        }
                        if (q[i].fldTypeBimeId == 1)
                        {
                            nobime = "2";
                        }
                        else
                        {
                            nobime = "1";
                        }
                        jammaliat = jammaliat + q[i].fldMaliyat;
                        if (FieldName == "BonKart")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            sayer_Naghdi + ",0,0," + mamoriat + ",0," + q[i].fldMablagh + "," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "Eydi")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            sayer_Naghdi + ",0,0," + mamoriat + ",0," + q[i].fldMablagh + "," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "EzafeKari")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            sayer_Naghdi + ",0,0," + mamoriat + ",0,0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "TatilKari")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            sayer_Naghdi + "," + sayer + ",0," + mamoriat + "," + sayer_GhNaghdi + ",0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "KomakGheyerNaghdi_Mostamer")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0," + sayer_GhNaghdi + ",0,0,0,0," + ezafekar + "," +
                            sayer_Naghdi + ",0,0," + mamoriat + ",0,0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "KomakGheyerNaghdi_GheyreMostamer")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            sayer_Naghdi + ",0,0," + mamoriat + "," + sayer_GhNaghdi + ",0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        else if (FieldName == "SayerPardakht")
                        {
                            Disket[cc] = q[i].fldCodemeli + ",1," + Month + ",0,85,0," +
                            q[i].fldTarikhEstekhdam.Replace("/", "") + ",," + code_moafi + ",1,"
                            + hoghogh_mostamar + "," + moavaghe + ",1,0,1,0,0,0,0,0,0," + ezafekar + "," +
                            sayer_Naghdi + "," + sayer + ",0," + mamoriat + "," + sayer_GhNaghdi + ",0," + sanavat + ",0,0,0," + q[i].fldMaliyat + "," + q[i].fldMaliyat;
                        }
                        Personeli[cc] = "1,1," + q[i].fldCodemeli + "," + q[i].fldName + "," + q[i].fldFamily + ",103," + q[i].fldPersonalId + "," + q[i].fldMadrakId + "," + q[i].Semat
                            + "," + nobime + ",," + q[i].fldShomareBime + "," + q[i].fldCodePosti.PadLeft(10, '0') + "," + q[i].fldAddress + "," + q[i].fldTarikhEstekhdam.Replace("/", "") + "," + noestekhdam + "," +
                            q[i].MahalKhedmat + ",1,1,," + code_moafi + ",,";
                        cc++;
                    }
                }

                string nahve = ",,,,," + jammaliat + ",,";
                if (TypePardakht == 1 || TypePardakht == 5)
                {
                    nahve = Sh_Check + "," + DateCheck.Replace("/", "") + "," + Bank + "," + NameShobe + "," + Sh_Hesab + "," + jammaliat + ",,0";
                }
                string[] wk = new string[1];
                wk[0] = Year + "," + Month +
                   ",0,0," + DatePay.Replace("/", "") +
                   "," + (TypePardakht).ToString() + "," + nahve;

                FileName = "WH" + Year.ToString() + Month.ToString().PadLeft(2, '0') + ".TXT";
                FileName2 = "WP" + Year.ToString() + Month.ToString().PadLeft(2, '0') + ".TXT";
                FileName3 = "WK" + Year.ToString() + Month.ToString().PadLeft(2, '0') + ".TXT";
                Passvand = ".TXT";
                Passvand2 = ".TXT";
                Passvand3 = ".TXT";
                savePath = Server.MapPath(@"~\Uploaded\" + FileName);
                savePath2 = Server.MapPath(@"~\Uploaded\" + FileName2);
                savePath3 = Server.MapPath(@"~\Uploaded\" + FileName3);
                if (System.IO.File.Exists(savePath))
                {
                    System.IO.File.Delete(savePath);
                }
                if (System.IO.File.Exists(savePath2))
                {
                    System.IO.File.Delete(savePath2);
                }
                if (System.IO.File.Exists(savePath3))
                {
                    System.IO.File.Delete(savePath3);
                }
                System.IO.File.WriteAllLines(savePath, Disket);
                System.IO.File.WriteAllLines(savePath2, Personeli);
                System.IO.File.WriteAllLines(savePath3, wk);
                /*MemoryStream disket_byte = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                MemoryStream Personeli_byte = new MemoryStream(System.IO.File.ReadAllBytes(savePath2));
                MemoryStream wk_byte = new MemoryStream(System.IO.File.ReadAllBytes(savePath3));
                Msg = "تهيه فایل با موفقيت به پايان رسيد.";
                MsgTitle = "تراكنش موفق";*/
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
                Er = Er,
                masir = savePath,
                masir2 = savePath2,
                masir3 = savePath3,
                FileName = FileName,
                FileName2 = FileName2,
                FileName3 = FileName3,
                Passvand = Passvand,
                Passvand2 = Passvand2,
                Passvand3 = Passvand3
            }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadBazneshastegi(short Year, byte Month, byte NobatePardakht)
        {
            var q1 = PayServic.GetDisketBazneshastegi(Year, Month, NobatePardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
            //var wb = new XLWorkbook();
            //var ws = wb.Worksheets.Add("Data_Test_Worksheet");
            var path = Server.MapPath(@"~\Uploaded\Sample.xlsx");
            XLWorkbook wb = new XLWorkbook(path);
            IXLWorksheet ws = wb.Worksheet(1);
            ws.Cell(3, 1).InsertData(q1);
            var FileName = "Retirement" + Session["UserId"].ToString() + ".xlsx";
            var path2 = Server.MapPath(@"~\Uploaded\" + FileName);
            wb.SaveAs(path2);
            MemoryStream st = new MemoryStream(System.IO.File.ReadAllBytes(path2));
            System.IO.File.Delete(path2);
            return File(st.ToArray(), MimeType.Get(".xlsx"),"Retirement.xlsx");
        }

        public ActionResult GenerateDiscBazneshastegi(string Sh_Fish, string DateVariz, short Year, byte Month, byte NobatePardakht)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; byte Er = 0; string name = "";

            try
            {
                var q = PayServic.GetPersonalInfo_MohasebeWithFilter("CheckDisketBazneshastegi", (Year), (Month), (NobatePardakht), Convert.ToByte(0), Convert.ToByte(0), Convert.ToInt32(Session["OrganId"]), "", "", "", IP, out ErrPay).ToList();
                if (q.Count > 0)
                {
                    foreach (var item in q)
                    {
                        name = name + item.fldName_Family + "، ";
                    }
                    return Json(new
                    {
                        Msg = "لطفا ابتدا برای پرسنل ذیل رسته شغلی تعریف کنید و سپس عملیات دیسکت را انجام دهید.  " + "\n" + name + "\n",
                        MsgTitle = "خطا",
                        name = name,
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                //OLD

                //var PathWorker = ConfigurationManager.AppSettings["PathWorker"].ToString();
                //Bime.BimeSRV BimeService = new Bime.BimeSRV();
                //BimeService.Timeout = 100000000;
                //var fullPath = Path.Combine(PathWorker, "workers.mdb");
                //string a = BimeService.GenerateDiscBazneshastegi(fullPath, Convert.ToInt32(Session["OrganId"]), Sh_Fish, DateVariz, Year, Month, NobatePardakht);
                //if (System.IO.File.Exists(fullPath))
                //    System.IO.File.Delete(PathWorker + @"\workers.mdb");
                //System.IO.File.Copy(a, PathWorker + @"\workers.mdb",true);

                //OLDER
                
                //var setting = PayServic.GetSettingWithFilter("fldOrganId", Session["OrganId"].ToString(),0,1, Session["Username"].ToString(), Session["Password"].ToString(), out ErrPay).FirstOrDefault();
                //var q = PayServic.GetDisketBazneshastegi(Year, Month, NobatePardakht, Convert.ToInt32(Session["OrganId"]),IP, out ErrPay).ToList();
                //string[] Disket = new string[q.Count + 1];
                //byte Jensiat = 1;//مرد
                //byte StatusTaahol = 1;//متأهل و...
                //byte MadrakId = 0;//متأهل و...
                //byte NezamVazifeId = 1;
                //string EsargariId = "";
                //string NesbatEsargariId = "";
                //string s_rasmi = "";
                //string s_gheyrerasmi = "";
                //int foghshoghl = 0;
                //int JAZB = 0;
                //int BARJESTEGI = 0;
                //int TADIL = 0;
                //int jame = 0;
                //int jamekol = 0;
                //int CityCode = 0;
                //int RasteCode = 0;
                //Bime.BimeSRV BService = new Bime.BimeSRV();

                //for (int i = 0; i < q.Count; i++)
                //{
                //    var Sanavat = PersoneliService.GetSavabegheSanavateKHedmatWithFilter("fldPersonalId", q[i].fldPersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, Session["Username"].ToString(), Session["Password"].ToString(), out ErrPersoneli).ToList();
                //    var rooz = MyLib.Shamsi.DiffOfShamsiDate(q[i].fldTarikhEstekhdam, q[i].fldTarikhEjra);
                //    s_rasmi = Comon_Pay.GetDiffDayMahSalNumberWithFilter(rooz, Session["Username"].ToString(), Session["Password"].ToString(), out ErrC_P).FirstOrDefault().CountString;
                //    foghshoghl = q[i].foghshoghl + q[i].made26;
                //    JAZB = q[i].jazb + q[i].ghazai + q[i].jazbTakhasosi + q[i].jazbtadil;
                //    BARJESTEGI = q[i].barjastegi + q[i].riali;
                //    TADIL = q[i].tadil + q[i].zaribtadil;
                //    jame = q[i].fldBimePersonal + q[i].fldBimeKarFarma + q[i].fldBimePersonalMoavaghe + q[i].fldBimeKarfarmaMoavaghe + q[i].fldMogharari;
                //    jamekol = jamekol + jame;

                //    int Sanavatgheyrerasmi = 0;
                //    foreach (var item in Sanavat)
                //    {
                //        Sanavatgheyrerasmi += MyLib.Shamsi.DiffOfShamsiDate(item.fldAzTarikh, item.fldTaTarikh);
                //    }
                //    s_gheyrerasmi = Comon_Pay.GetDiffDayMahSalNumberWithFilter(Sanavatgheyrerasmi, Session["Username"].ToString(), Session["Password"].ToString(), out ErrC_P).FirstOrDefault().CountString;
                //    if (q[i].fldJensiyat == false)
                //    {//زن
                //        Jensiat = 2;
                //    }
                //    if (q[i].fldStatusTaaholId == 1)
                //    {//مجرد
                //        StatusTaahol = 2;
                //    }
                //    if (q[i].fldNezamVazifeId > 1)
                //    {
                //        NezamVazifeId = 2;
                //    }
                //    if (q[i].fldMadrakId == 2)
                //    {//دیپلم
                //        MadrakId = 5;
                //    }
                //    else if (q[i].fldMadrakId == 3)
                //    {//فوق دیپلم
                //        MadrakId = 6;
                //    }
                //    else if (q[i].fldMadrakId == 4)
                //    {//لیسانس
                //        MadrakId = 7;
                //    }
                //    else if (q[i].fldMadrakId == 5)
                //    {//فوق لیسانس
                //        MadrakId = 8;
                //    }
                //    else if (q[i].fldMadrakId == 6)
                //    {//دکترا
                //        MadrakId = 9;
                //    }
                //    else if (q[i].fldMadrakId == 1)
                //    {//زیردیپلم
                //        MadrakId = 103;
                //    }
                //    if (q[i].fldEsargariId == 1 || q[i].fldEsargariId == 4)
                //    {
                //        EsargariId = "0";
                //    }
                //    else if (q[i].fldEsargariId == 2)
                //    {
                //        EsargariId = "2";
                //    }
                //    else if (q[i].fldEsargariId == 3)
                //    {
                //        NesbatEsargariId = "7";
                //    }
                //    else if (q[i].fldEsargariId == 5)
                //    {
                //        NesbatEsargariId = "10";
                //    }
                //    CityCode = BService.GetCityCode(q[i].CityName);
                //    RasteCode=BService.GetRasteCode(q[i].fldRasteShoghli);

                //    Disket[i + 1] = "2,1," + q[i].fldSh_Personali + ",'" + q[i].fldCodemeli + "','" + q[i].fldName + "','" +
                //         q[i].fldFamily + "','" + q[i].fldFatherName + "'," + q[i].fldSh_Shenasname + ",'" + q[i].fldTarikhTavalod + "',0," +
                //         CityCode + "," + Jensiat + "," + StatusTaahol + "," + q[i].fldTedadFarzand + "," + MadrakId + ",0,'" + q[i].TitleReshte + "'," +
                //         NezamVazifeId + "," + RasteCode + ",0,'" + q[i].fldReshteShoghli + "',0,'" + q[i].OrganPost + "'," + q[i].fldGroup + "," +
                //         EsargariId + "," + NesbatEsargariId + ",1,0," + q[i].fldTabaghe + ",0,0,0,'" + s_rasmi + "','" + s_gheyrerasmi + "'," + '"' + ",'" +
                //         q[i].fldTarikhEstekhdam + "',0," + '"' + ",0," + '"' + "," + q[i].fldMazad30Sal + "," + '"' + ",'" + q[i].fldTarikhEjra + "'," + q[i].fldYear +
                //         "," + q[i].fldMonth + ",0,0," + q[i].h_paye + "," + q[i].sanavat + "," + foghshoghl + "," + JAZB + "," + q[i].vije + "," + q[i].talash +
                //         ",0," + BARJESTEGI + "," + q[i].modiryati + "," + q[i].takhasosi + "," + q[i].sakhtikar + "," + q[i].nobatkari + "," + q[i].tatbigh + "," +
                //         TADIL + ","+q[i].hadaghaltadil+","+q[i].fogh_isar+"," + q[i].fldMashmolBime + "," + q[i].fldBimePersonal + "," + q[i].fldBimeKarFarma + "," + q[i].fldMashmolBimeMoavaghe + "," +
                //         q[i].fldBimePersonalMoavaghe + "," + q[i].fldBimeKarfarmaMoavaghe + ",0," + q[i].fldMogharari + ",'" + q[i].fldTypeHokm + "',0,0,0," + jame;

                //}
                //Disket[0] = setting.fldCodeDastgah + "," + Year + "," + Month + ",201," + jamekol + "," + jamekol + "$" + Sh_Fish + "," + jamekol + "," + DateVariz;
                //FileName = "wlist"+setting.fldCodeDastgah+"_"+Year+"_"+Month+"_201.csv";
                //Passvand = ".CSV";
                //savePath = Server.MapPath(@"~\Uploaded\" + FileName);
                //if (System.IO.File.Exists(savePath))
                //{
                //    System.IO.File.Delete(savePath);
                //}
                //System.IO.File.WriteAllLines(savePath, Disket, UTF8Encoding.UTF8);
                Msg = "تهيه دیسکت با موفقيت به پايان رسيد.";
                MsgTitle = "عملیات موفق";
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
                name = name,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult rptBime(string FieldName,string sh_List,short Year, byte Month, byte NobatePardakht//,
            /*int KargahId int Mard, int Zan, long J_Mazaya, long J_Bime_P, long J_Bime_K,
            long j_motalebat, long j_bime_bikari*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.FieldName = FieldName;
            result.ViewBag.sh_List = sh_List;
            result.ViewBag.Year = Year;
            result.ViewBag.Month = Month;
            result.ViewBag.NobatePardakht = NobatePardakht;
            //result.ViewBag.KargahId = KargahId;
            /*result.ViewBag.Mard = Mard;
            result.ViewBag.Zan = Zan;
            result.ViewBag.J_Mazaya = J_Mazaya;
            result.ViewBag.J_Bime_P = J_Bime_P;
            result.ViewBag.J_Bime_K = J_Bime_K;
            result.ViewBag.j_motalebat = j_motalebat;
            result.ViewBag.j_bime_bikari = j_bime_bikari;*/
            return result;
        }

        public ActionResult rptPasAndaz(int DisketId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.DisketId = DisketId;
            return result;
        }

        public ActionResult GeneratePDFrptBime(string FieldName,short Year, byte Month, byte NobatePardakht,
            /*int KargahId, int Mard, int Zan, long J_Mazaya, long J_Bime_P, long J_Bime_K,
            long j_motalebat, long j_bime_bikari*/ string sh_List)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                Bime.Bime[] Res = (Bime.Bime[])Session["BimeRes"];
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_tblInsuranceWorkshopSelectTableAdapter Workshop = new DataSet.DataSet1TableAdapters.spr_tblInsuranceWorkshopSelectTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptDisketBimeTableAdapter RptDisketBime = new DataSet.DataSet1TableAdapters.spr_Pay_RptDisketBimeTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_DisketBime_Tatilkar_EzafekarTableAdapter RptDisketBime_Ezafekar = new DataSet.DataSet1TableAdapters.spr_Pay_DisketBime_Tatilkar_EzafekarTableAdapter();
                
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1,IP, out Err).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);                
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                Workshop.Fill(dt.spr_tblInsuranceWorkshopSelect, "OrganId", Convert.ToInt32(Session["OrganId"]).ToString(), Convert.ToInt32(Session["OrganId"]), 1);
                /*if (FieldName == "Hoghough")
                {
                    RptDisketBime.Fill(dt.spr_Pay_RptDisketBime, Year, Month, KargahId, NobatePardakht);
                }
                else
                {
                    RptDisketBime_Ezafekar.Fill(dt.spr_Pay_DisketBime_Tatilkar_Ezafekar, FieldName, Year, Month, NobatePardakht, KargahId);
                }*/

                FastReport.Report Report = new FastReport.Report();
                if (FieldName == "Hoghough" || FieldName == "Hoghough_Moavaghe")
                {
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholase_DisketBime.frx");
                    Report.RegisterData(dt, "rasaFMSPayRoll");
                    Report.RegisterData(dt_Com, "rasaFMSCommon");
                    foreach (var item in Res)
                    {
                        decimal jam = item.J_Bime_P + item.J_Bime_K + item.j_bime_bikari;
                        var t = PayServic.GetInsuranceWorkshopDetail(item.KargahId, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                        DataRow aa = dt.bime_kholase.NewRow();
                        aa["kargahname"] = t.fldWorkShopName + "_" + t.fldPeyman;
                        aa["shkargah"] = t.fldWorkShopNum + "_" + t.fldPeyman;
                        aa["mah"] = item.mah;
                        aa["sal"] = item.sal;
                        aa["mard"] = item.TedadMard;
                        aa["zan"] = item.TedadZan;
                        aa["mashmol_gheir"] = (Convert.ToDecimal(item.j_motalebat).ToString("#,###"));
                        aa["mashmol"] = (Convert.ToDecimal(item.J_Mazaya).ToString("#,###"));
                        aa["persenel"] = (Convert.ToDecimal(item.J_Bime_P).ToString("#,###"));
                        aa["karfarma"] = (Convert.ToDecimal(item.J_Bime_K).ToString("#,###"));
                        aa["bikari"] = (Convert.ToDecimal(item.j_bime_bikari).ToString("#,###"));
                        aa["jam"] = (jam.ToString("#,###"));
                        dt.bime_kholase.Rows.Add(aa);
                    }
                }

                else
                {
                    decimal jam = Res[0].J_Bime_P + Res[0].J_Bime_K + Res[0].j_bime_bikari;
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholase_DisketBime_Ezafekar_TatilKar.frx");
                    Report.RegisterData(dt, "rasaFMSPayRoll");
                    Report.RegisterData(dt_Com, "rasaFMSCommon");
                    Report.SetParameterValue("sal", Year);
                    Report.SetParameterValue("mah", MonthName);
                    Report.SetParameterValue("mard", Res[0].TedadMard);
                    Report.SetParameterValue("zan", Res[0].TedadZan);
                    Report.SetParameterValue("mashmol_gheir", Res[0].j_motalebat);
                    if (Res[0].J_Mazaya == 0)
                    {
                        Report.SetParameterValue("mashmol", Res[0].J_Mazaya);
                    }
                    else
                    {
                        Report.SetParameterValue("mashmol", Res[0].J_Mazaya.ToString("#,#"));
                    }
                    Report.SetParameterValue("persenel", Res[0].J_Bime_P);
                    Report.SetParameterValue("karfarma", Res[0].J_Bime_K);
                    Report.SetParameterValue("bikari", Res[0].j_bime_bikari);
                    Report.SetParameterValue("jam", jam);
                    Report.SetParameterValue("sh_list", sh_List.PadLeft(2, '0'));
                }
                
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult rptBankMeli(int DisketId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.DisketId = DisketId;
            return result;
        }

        public ActionResult rptBankKeshavarzi(int DisketId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.DisketId = DisketId;
            return result;
        }

        public ActionResult rptBankSaderat(int DisketId,short Year, byte Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Year = Year;
            result.ViewBag.Month = Month;
            result.ViewBag.DisketId = DisketId;
            return result;
        }

        public ActionResult rptListPardakht(short Year, byte Month, byte NobatePardakht, byte MarhalePardakht, string FieldName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Year = Year;
            result.ViewBag.Month = Month;
            result.ViewBag.NobatePardakht = NobatePardakht;
            result.ViewBag.MarhalePardakht = MarhalePardakht;
            result.ViewBag.FieldName = FieldName;
            return result;
        }

        public ActionResult GenaratePdfrptBankMeli(int DisketId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter Files = new DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_tblDisketSelectTableAdapter Disket = new DataSet.DataSet1TableAdapters.spr_tblDisketSelectTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1,IP, out Err).FirstOrDefault();
                var FileId = servic.GetBankDetail(1,  IP, out Err).fldFileId;
                var organName = servic.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).fldName;

                dt.EnforceConstraints = false;
                Files.Fill(dt_Com.spr_tblFileSelect, "fldId", FileId.ToString(), 1);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Disket.Fill(dt.spr_tblDisketSelect, "fldId", DisketId.ToString(), 1);
                var Diskett = PayServic.GetDisketDetail(DisketId, IP, out ErrPay);
                var Mablagh_H = MyLib.NumberTool.Num2Str(Convert.ToUInt64(Diskett.fldJam),1);
                var Name_shobe = Diskett.fldNameShobe;
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptBankMeli.frx");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.SetParameterValue("NameOrgan", organName);
                Report.SetParameterValue("NameShobe", Name_shobe);
                Report.SetParameterValue("Varizi_H", Mablagh_H);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GenaratePdfrptPasAndazMeli(int DisketId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter Files = new DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_tblDisketSelectTableAdapter Disket = new DataSet.DataSet1TableAdapters.spr_tblDisketSelectTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1,IP, out Err).FirstOrDefault();
                var FileId = servic.GetBankDetail(1, IP, out Err).fldFileId;
                var organName = servic.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).fldName;

                dt.EnforceConstraints = false;
                Files.Fill(dt_Com.spr_tblFileSelect, "fldId", FileId.ToString(), 1);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Disket.Fill(dt.spr_tblDisketSelect, "fldId", DisketId.ToString(), 1);
                var Diskett = PayServic.GetDisketDetail(DisketId, IP, out ErrPay);

                var Mablagh_H = MyLib.NumberTool.Num2Str(Convert.ToUInt64(Diskett.fldJam), 1);
                var Name_shobe = Diskett.fldNameShobe;
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptBankMeli.frx");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.SetParameterValue("NameOrgan", organName);
                Report.SetParameterValue("NameShobe", Name_shobe);
                Report.SetParameterValue("Varizi_H", Mablagh_H);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GenaratePdfrptBankKeshavarzi(int DisketId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter Files = new DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
                DataSet.DataSet1TableAdapters.spr_tblDisketSelectTableAdapter Disket = new DataSet.DataSet1TableAdapters.spr_tblDisketSelectTableAdapter();
                var FileId = servic.GetBankDetail(5, IP, out Err).fldFileId;
                dt.EnforceConstraints = false;
                
                Files.Fill(dt_Com.spr_tblFileSelect, "fldId", FileId.ToString(), 1);
                Disket.Fill(dt.spr_tblDisketSelect, "fldId", DisketId.ToString(), 0);

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptBankKeshavarzi.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GenaratePdfrptBankSaderat(int DisketId,short Year,byte Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter Files = new DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
                DataSet.DataSet1TableAdapters.spr_tblDisketSelectTableAdapter Disket = new DataSet.DataSet1TableAdapters.spr_tblDisketSelectTableAdapter();
                var FileId = servic.GetBankDetail(4, IP, out Err).fldFileId;
                dt.EnforceConstraints = false;
                var tarikh = servic.GetDate( IP, out Err).fldTarikh;
                var sal = tarikh.Substring(2, 2);
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);

                Files.Fill(dt_Com.spr_tblFileSelect, "fldId", FileId.ToString(), 1);
                Disket.Fill(dt.spr_tblDisketSelect, "fldId", DisketId.ToString(), 0);

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptBankSaderat.frx");
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("sal", Year);
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GeneratePDFListPardakhti(short Year, byte Month, byte NobatePardakht, byte MarhalePardakht, string FieldName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_SelectVariziForBankTableAdapter Listt = new DataSet.DataSet1TableAdapters.spr_Pay_SelectVariziForBankTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(),1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                Listt.Fill(dt.spr_Pay_SelectVariziForBank, FieldName, Year, Month, NobatePardakht, MarhalePardakht, Convert.ToInt32(Session["OrganId"]),"");

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId",Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 26, IP, out Err).ToList();
                if (signers.Count() == 1)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                }
                else if (signers.Count() == 2)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s4 = signers[1].Post;
                    t4 = signers[1].SignerName;
                }
                else if (signers.Count() == 3)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s3 = signers[1].Post;
                    t3 = signers[1].SignerName;
                    s5 = signers[2].Post;
                    t5 = signers[2].SignerName;
                }
                else if (signers.Count() == 4)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s2 = signers[1].Post;
                    t2 = signers[1].SignerName;
                    s3 = signers[2].Post;
                    t3 = signers[2].SignerName;
                    s4 = signers[3].Post;
                    t4 = signers[3].SignerName;
                }
                else if (signers.Count() == 5)
                {
                    s1 = signers[0].Post;
                    t1 = signers[0].SignerName;
                    s2 = signers[1].Post;
                    t2 = signers[1].SignerName;
                    s3 = signers[2].Post;
                    t3 = signers[2].SignerName;
                    s4 = signers[3].Post;
                    t4 = signers[3].SignerName;
                    s5 = signers[4].Post;
                    t5 = signers[4].SignerName;
                }
               
 int flag=1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";
               
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), NobatePardakht.ToString(), Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault() ;
                if (k!= null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakht.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("Sal", Year);
                Report.SetParameterValue("Mah", MonthName);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("flag", flag);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult Download(string Masir, string FileName, string Passvand)
        {
            MemoryStream st = new MemoryStream(System.IO.File.ReadAllBytes(Masir));
            System.IO.File.Delete(Masir);
            return File(st.ToArray(), MimeType.Get(Passvand), FileName);
        }

        public FileResult DownloadFile(int DisketId)
        {
            var Disket=PayServic.GetDisketDetail(DisketId, IP, out ErrPay);
            var Filee=servic.GetFileWithFilter("fldId", Disket.fldFileId.ToString(), 1, IP, out Err).FirstOrDefault();
            return File(Filee.fldImage, MimeType.Get(Filee.fldPasvand), Disket.fldNameFile);
        }

        public ActionResult Read(StoreRequestParameters parameters, int BankFixId, int TypePardakht)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_Disket> data = null;
            data = PayServic.GetDisketWithFilter("fldBankFixId", BankFixId.ToString(), 0, IP, out ErrPay).Where(l => l.fldTypePardakht == TypePardakht && l.fldType==true).ToList();
            return this.Store(data);
        }

        public ActionResult ReadPasandaz(int BankId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_Disket> data = null;
            data = PayServic.GetDisketWithFilter("fldBankFixId", BankId.ToString(), 0, IP, out ErrPay).Where(l => l.fldType == false).ToList();
            return this.Store(data);
        }
        public ActionResult DiscPasandaz(short Year, byte Month, byte NobatePardakht, int BankId, int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; byte Er = 0; var FileName = ""; var Passvand = ""; string savePath = "";
            int tedad = 0; var marhale = ""; var organName = ""; string tarikh = ""; var code_shobe = ""; var code_sazeman = "";
            long? jam = 0; var Name_shobe = ""; var DisketId = 0; int j = 0; int radif = 0;string[] a=new string[1];
            string[] b = new string[1]; int tedad2 = 0; long? jam2 = 0; int z = 0; var FileName2 = ""; var Passvand2 = "";
            string savePath2 = "";

            try
            {
                var q = PayServic.GetDisketPasAndaz(Year, Month, NobatePardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
                organName = servic.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).fldName;
                var setting = PayServic.GetSettingWithFilter("fldOrganId", Session["OrganId"].ToString(), 0, 1, IP, out ErrPay).FirstOrDefault();
                code_sazeman = setting.fldH_CodeOrgan;
                code_shobe = setting.fldH_CodeShobe;
                Name_shobe = setting.fldH_NameShobe;
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var disket = PayServic.GetDisketWithFilter("", "", 0, IP, out ErrPay).Where(l => l.fldBankFixId == BankId && l.fldType == false).LastOrDefault();
                if (disket != null)
                {
                    marhale = (Convert.ToInt32(disket.fldMarhale) + 1).ToString();
                }
                else
                {
                    marhale = "101";
                }
                if (BankId == 1)
                {
                    a = new string[(q.Count * 2) + 1];
                }
                else if (BankId == 16 || BankId == 35)
                {
                    a = new string[q.Count + 2];
                }
                else if (BankId == 2)
                {
                    a = new string[q.Count + 1];
                    b = new string[q.Count + 1];
                }
                
                tarikh = servic.GetDate( IP, out Err).fldTarikh;
                var sal = tarikh.Substring(2, 2);
                var mah = tarikh.Substring(5, 2);
                var roz = tarikh.Substring(8, 2);
                string[] alpha = { "A", "B" };
                int index = 0;
                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                int k3 = 0;
                for (int i = 0; i < q.Count; i++)
                {
                    int khales = Convert.ToInt32(q[i].fldPasAndaz) / 2;
                    if (BankId == 1)
                    {
                        if (System.Configuration.ConfigurationManager.AppSettings["BmiDisketVer"].ToString() == "new")
                        {
                            Cell Cell = sheet.Cells[alpha[0] + (k3 + 1)];
                            Cell.PutValue(q[i].fldShPasAndazPersonal.PadLeft(13, '0'));

                            Cell Cell1 = sheet.Cells[alpha[1] + (k3 + 1)];
                            Cell1.PutValue(khales.ToString());
                            k3++;

                            Cell Cell2 = sheet.Cells[alpha[0] + (k3 + 1)];
                            Cell2.PutValue(q[i].fldShPasAndazKarFarma.PadLeft(13, '0'));

                            Cell Cell3 = sheet.Cells[alpha[1] + (k3 + 1)];
                            Cell3.PutValue(khales.ToString());
                            k3++;
                        }
                        else
                        {
                            jam = jam + (khales * 2);
                            tedad++;
                            radif++;
                            a[j + 1] = radif.ToString().PadLeft(5, '0') + q[i].fldShPasAndazPersonal.PadLeft(13, '0') + khales.ToString().PadLeft(15, '0') + "000000000000000";
                            j++;
                            tedad++;
                            radif++;
                            a[j + 1] = radif.ToString().PadLeft(5, '0') + q[i].fldShPasAndazKarFarma.PadLeft(13, '0') + khales.ToString().PadLeft(15, '0') + "000000000000000";
                            j++;
                        }
                    }
                    else if (BankId == 2)
                    {
                        jam = jam + khales;
                        a[j + 1] = q[i].fldShPasAndazPersonal.PadLeft(10, '0') + khales.ToString().PadLeft(15, '0');
                        j++;
                        tedad++;

                        jam2 = jam2 + khales;
                        b[z + 1] = q[i].fldShPasAndazKarFarma.PadLeft(10, '0') + khales.ToString().PadLeft(15, '0');
                        z++;
                        tedad2++;
                    }
                    else if (BankId == 16 || BankId == 35)
                    {
                        jam = jam + (khales * 2);
                        a[j + 2] = q[i].fldShPasAndazPersonal + "," + (khales * 2).ToString() + ",C,واریز بابت پس انداز " + MonthName + " ماه سال " + Year.ToString();
                        j++;
                    }
                }
                MemoryStream stream = new MemoryStream();
                if (BankId == 1)
                {
                   
                    if (code_sazeman.Length == 4)
                    {
                        a[0] = code_shobe.PadLeft(4, '0') + code_sazeman + marhale.PadLeft(4, '0') + sal + mah + roz + jam.ToString().PadLeft(15, '0') + tedad.ToString().PadLeft(5, '0') + "2940000000";
                    }
                    else if (code_sazeman.Length == 6)
                    {
                        a[0] = code_shobe.PadLeft(4, '0') + code_sazeman + marhale.PadLeft(6, '0') + sal + mah + roz + jam.ToString().PadLeft(15, '0') + tedad.ToString().PadLeft(5, '0') + "294000";
                    }
                    if (System.Configuration.ConfigurationManager.AppSettings["BmiDisketVer"].ToString() == "new")
                    { 
                        wb.Save(stream, SaveFormat.Xlsx);
                        FileName = "hog" + sal + mah + roz + ".xlsx";
                        Passvand = ".xlsx";
                    }
                    else
                    {
                        FileName = code_sazeman + code_shobe + "." + mah;
                        Passvand = "." + mah;
                    }
                }
                else if (BankId == 2)
                {
                    a[0] = tedad.ToString().PadLeft(10, '0') + jam.ToString().PadLeft(15, '0');
                    FileName = "FL" + sal + mah + roz + "P.PAY";
                    Passvand = ".PAY";

                    b[0] = tedad2.ToString().PadLeft(10, '0') + jam2.ToString().PadLeft(15, '0');
                    FileName2 = "FL" + sal + mah + roz + "K.PAY";
                    Passvand2 = ".PAY";
                }
                else if (BankId == 16 || BankId == 35)
                {
                    a[0] = "1," + q.Count.ToString() + "," + jam.ToString();
                    a[1] = setting.fldSh_HesabCheck + "," + jam.ToString() + ",D,برداشت از حساب سپرده جاری " + organName;
                    FileName = "trans.txt";
                    Passvand = ".txt";
                }
                savePath = Server.MapPath(@"~\Uploaded\" + FileName);
                if (System.IO.File.Exists(savePath))
                {
                    System.IO.File.Delete(savePath);
                }
                if (BankId == 1 && System.Configuration.ConfigurationManager.AppSettings["BmiDisketVer"].ToString() == "new")
                    System.IO.File.WriteAllBytes(savePath, stream.ToArray());
                else
                    System.IO.File.WriteAllLines(savePath, a);
              
                MemoryStream disket_byte = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
                if (BankId == 2)
                {
                    savePath2 = Server.MapPath(@"~\Uploaded\" + FileName2);
                    if (System.IO.File.Exists(savePath2))
                    {
                        System.IO.File.Delete(savePath2);
                    }
                    System.IO.File.WriteAllLines(savePath2, b);
                    MemoryStream disket_byte2 = new MemoryStream(System.IO.File.ReadAllBytes(savePath2));
                }
                Msg = "تهيه ديسكت پس انداز با موفقيت به پايان رسيد.";
                MsgTitle = "تهیه دیسکت موفق";
                if (BankId == 1 )/*ملی */
                {
                    if (Id == 0)
                    {
                        DisketId = PayServic.InsertDisket(tarikh, tedad, false, jam, 10, code_shobe, code_sazeman, disket_byte.ToArray(), Passvand, FileName, BankId, Name_shobe, "",
                        Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                        if (ErrPay.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = ErrPay.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var diskett = PayServic.GetDisketDetail(Id, IP, out ErrPay);
                        PayServic.UpdateDisket(Id, tarikh, tedad, false, jam, 10, code_shobe, code_sazeman, diskett.fldFileId, disket_byte.ToArray(), Passvand, FileName, BankId, Name_shobe, "",
                        Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                        if (ErrPay.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = ErrPay.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1
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
                Er = Er,
                masir = savePath,
                masir2 = savePath2,
                DisketId = DisketId,
                FileName = FileName,
                FileName2 = FileName2,
                Passvand = Passvand,
                Passvand2 = Passvand2
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
