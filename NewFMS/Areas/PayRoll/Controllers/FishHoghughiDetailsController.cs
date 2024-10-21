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
using System.Data;
using System.Xml;
using System.Security.Cryptography;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class FishHoghughiDetailsController : Controller
    {
        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService ComServic = new WCF_Common.CommonService();
        WCF_Personeli.PersoneliService PersonalServic = new WCF_Personeli.PersoneliService();
        WCF_Common_Pay.Comon_PayService Com_PayServic = new WCF_Common_Pay.Comon_PayService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError ErrCom = new WCF_Common.ClsError();
        WCF_Personeli.ClsError ErrPersonal= new WCF_Personeli.ClsError();
        WCF_Common_Pay.ClsError ErrCom_Pay = new WCF_Common_Pay.ClsError();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        // GET: PayRoll/FishHoghughiDetails
        public ActionResult Index(string containerId)
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
        public ActionResult Hokm(int HokmId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.HokmDetails();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                ViewData = ViewData
            };
            result.ViewBag.HokmId = HokmId;
            return result;
        }
        public ActionResult Moavaghe(int PersonalId, int Nobat, short Year, byte Month, short YearPardakht, byte MonthPardakht, byte TypeHesab)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.PersonalId = PersonalId;
            result.ViewBag.Nobat = Nobat;
            result.ViewBag.Year = Year;
            result.ViewBag.Month = Month;
            result.ViewBag.YearPardakht = YearPardakht;
            result.ViewBag.MonthPardakht = MonthPardakht;
            result.ViewBag.MonthPardakhtName =MyLib.Shamsi.ShamsiMonthname(MonthPardakht);
            result.ViewBag.TypeHesab = TypeHesab;
            result.ViewBag.MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
            return result;
        }
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = ComServic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out ErrCom).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetTypeHesab()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = ComServic.GetHesabTypeWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrCom).ToList()
                .Select(c => new { fldId = c.fldId, fldTitle = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetYear(int? Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            var q = ComServic.GetDate(IP, out ErrCom);
            int fldsal = Convert.ToInt32(q.fldTarikh.Substring(0, 4));
            Year=Year == null ? 1387 : Year;
            for (int i = Convert.ToInt32(Year); i < fldsal + 1; i++)
            {
                Models.CurrentDate CboSal = new Models.CurrentDate();
                CboSal.fldYear = i;
                sh.Add(CboSal);
            }
            return Json(sh.OrderByDescending(l => l.fldYear), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMonth(int? Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            var q = ComServic.GetDate(IP, out ErrCom);
            int fldsal = Convert.ToInt32(q.fldTarikh.Substring(0, 4));

            Month = Month == null ? 1 : Month;
            for (int i = Convert.ToByte(Month); i < 13; i++)
            {
                Models.CurrentDate ObjMonth = new Models.CurrentDate();
                ObjMonth.fldMonth = i.ToString().PadLeft('0',2);
                ObjMonth.fldMonthName = MyLib.Shamsi.ShamsiMonthname(i);
                sh.Add(ObjMonth);
            }
            return Json(sh.OrderBy(l => l.fldMonth), JsonRequestBehavior.AllowGet);
        }
        public FileResult CreateExcelFishDetails(int PersonalId, byte Nobat, short Year, string Month, short TaYear, string TaMonth, byte FilterType, byte TypeReport,byte TypeHesab)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
            int index = 0; var fldBed = ""; var fldBes = ""; var fldHokmId = ""; var fldKarkard = ""; var fldMashmolBime = " "; var fldMashmolMaliyat = "";
            var fldMonth = " "; var fldMonthPardakht = ""; var fldTitle = ""; var fldYear = ""; var fldYearPardakht = ""; var fldMablaghHokm = "";
            var fldBimeMashmool = " "; var fldMaliyatMashmool = ""; var Check = "";

            var StatusCheck = "fldTitle;fldBed;fldBes;fldMablaghHokm;fldHokmId;fldKarkard;fldBimeMashmool;fldMaliyatMashmool;fldMashmolBime;fldMashmolMaliyat;fldYear;fldMonth;fldYearPardakht;fldMonthPardakht".Split(';');
            List<Models.FishDetails> Details = null;
            List<WCF_PayRoll.OBJ_MotalebatDetails> data = null;
            List<WCF_PayRoll.OBJ_MotalebatDetails> data2 = null;
            var Az = Convert.ToInt32(Year + Month);
            var Ta = Convert.ToInt32(TaYear + TaMonth);

            if (FilterType == 0)
            {
                if (TypeReport == 1)
                {
                    data = PayServic.SelectMotalebat_Details(PersonalId, Nobat, Az, Ta,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();

                    data2 = PayServic.SelectKosourat_Details(PersonalId, Nobat, Az, Ta,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();
                }
                else
                {
                    data = PayServic.SelectMotalebat_Moavaghe_Details(PersonalId, Nobat, Year, Convert.ToByte(Month),TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();

                    data2 = PayServic.SelectKosourat_Moavaghe_Details(PersonalId, Nobat, Year, Convert.ToByte(Month),TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();
                }
            }
            else
            {
                if (TypeReport == 1)
                {
                    data = PayServic.SelectMotalebat_Details(PersonalId, Nobat, Az, Ta,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).Where(l => l.fldCalcType == FilterType).ToList();

                    data2 = PayServic.SelectKosourat_Details(PersonalId, Nobat, Az, Ta,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).Where(l => l.fldCalcType == FilterType).ToList();
                }
                else
                {
                    data = PayServic.SelectMotalebat_Moavaghe_Details(PersonalId, Nobat, Year, Convert.ToByte(Month),TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).Where(l => l.fldCalcType == FilterType).ToList();

                    data2 = PayServic.SelectKosourat_Moavaghe_Details(PersonalId, Nobat, Year, Convert.ToByte(Month),TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).Where(l => l.fldCalcType == FilterType).ToList();
                }
            }
            Details = data.Select(l => new Models.FishDetails()//مطالبات
            {
                fldBed = l.fldMablagh,
                fldBes = 0,
                fldHokmId = l.fldHokmId,
                fldKarkard = l.fldKarkard,
                fldMashmolBime = l.fldMashmolBime,
                fldMashmolMaliyat = l.fldMashmolMaliyat,
                fldMonthName = MyLib.Shamsi.ShamsiMonthname(l.fldMonth),
                fldMonthPardakhtName = MyLib.Shamsi.ShamsiMonthname(l.fldMonthPardakht),
                fldTitle = l.fldTitle,
                fldYear = l.fldYear,
                fldYearPardakht = l.fldYearPardakht,
                fldtype = 1,
                fldCalcType = l.fldCalcType,
                fldMablaghHokm = l.fldMablaghHokm,
                fldBimeMashmool = l.fldBimeMashmool,
                fldMaliyatMashmool = l.fldMaliyatMashmool
            }).ToList();
            Details.AddRange(data2.Select(l => new Models.FishDetails()//کسورات
            {
                fldBed = 0,
                fldBes = l.fldMablagh,
                fldHokmId = l.fldHokmId,
                fldKarkard = l.fldKarkard,
                fldMashmolBime = l.fldMashmolBime,
                fldMashmolMaliyat = l.fldMashmolMaliyat,
                fldMonthName = MyLib.Shamsi.ShamsiMonthname(l.fldMonth),
                fldMonthPardakhtName = MyLib.Shamsi.ShamsiMonthname(l.fldMonthPardakht),
                fldTitle = l.fldTitle,
                fldYear = l.fldYear,
                fldYearPardakht = l.fldYearPardakht,
                fldtype = 2,
                fldCalcType = l.fldCalcType,
                fldMablaghHokm = l.fldMablaghHokm,
                fldBimeMashmool = l.fldBimeMashmool,
                fldMaliyatMashmool = l.fldMaliyatMashmool
            }).ToList());
            Details.OrderBy(l => l.fldtype).ThenByDescending(l => l.fldYear).ThenBy(l => l.fldMonth).ThenByDescending(l => l.fldHokmId);

            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];

            foreach (var item in StatusCheck)
            {
                switch (item)
                {

                    case "fldTitle":
                        Check = "عنوان";
                        Cell cell = sheet.Cells[alpha[index] + "1"];
                        cell.PutValue(Check);
                        int i = 0;
                        foreach (var _item in Details)
                        {
                            fldTitle = _item.fldTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                            Cell.PutValue(fldTitle);
                            i++;
                        }
                        index++;
                        break;
                    case "fldBed":
                        Check = "بدهکار";
                        Cell cell1 = sheet.Cells[alpha[index] + "1"];
                        cell1.PutValue(Check);
                        int i1 = 0;
                        foreach (var _item in Details)
                        {
                            fldBed = _item.fldBed.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i1 + 2)];
                            Cell.PutValue(fldBed);
                            i1++;
                        }
                        index++;
                        break;
                    case "fldBes":
                        Check = "بستانکار";
                        Cell cell2 = sheet.Cells[alpha[index] + "1"];
                        cell2.PutValue(Check);
                        int i2 = 0;
                        foreach (var _item in Details)
                        {
                            fldBes = _item.fldBes.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i2 + 2)];
                            Cell.PutValue(fldBes);
                            i2++;
                        }
                        index++;
                        break;
                    case "fldMablaghHokm":
                        Check = "مبلغ حکم";
                        Cell cell3 = sheet.Cells[alpha[index] + "1"];
                        cell3.PutValue(Check);
                        int i3 = 0;
                        foreach (var _item in Details)
                        {
                            fldMablaghHokm = _item.fldMablaghHokm.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i3 + 2)];
                            Cell.PutValue(fldMablaghHokm);
                            i3++;
                        }
                        index++;
                        break;
                    case "fldHokmId":
                        Check = "شماره حکم";
                        Cell cell4 = sheet.Cells[alpha[index] + "1"];
                        cell4.PutValue(Check);
                        int i4 = 0;
                        foreach (var _item in Details)
                        {
                            fldHokmId = _item.fldHokmId.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i4 + 2)];
                            Cell.PutValue(fldHokmId);
                            i4++;
                        }
                        index++;
                        break;
                    case "fldKarkard":
                        Check = "کارکرد";
                        Cell cell5 = sheet.Cells[alpha[index] + "1"];
                        cell5.PutValue(Check);
                        int i5 = 0;
                        foreach (var _item in Details)
                        {
                            fldKarkard = _item.fldKarkard.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i5 + 2)];
                            Cell.PutValue(fldKarkard);
                            i5++;
                        }
                        index++;
                        break;
                    case "fldBimeMashmool":
                        Check = "مشمول بیمه";
                        Cell cell6 = sheet.Cells[alpha[index] + "1"];
                        cell6.PutValue(Check);
                        int i6 = 0;
                        foreach (var _item in Details)
                        {
                            fldBimeMashmool = _item.fldBimeMashmool;
                            Cell Cell = sheet.Cells[alpha[index] + (i6 + 2)];
                            Cell.PutValue(fldBimeMashmool);
                            i6++;
                        }
                        index++;
                        break;
                    case "fldMaliyatMashmool":
                        Check = "مشمول مالیات";
                        Cell cell7 = sheet.Cells[alpha[index] + "1"];
                        cell7.PutValue(Check);
                        int i7 = 0;
                        foreach (var _item in Details)
                        {
                            fldMaliyatMashmool = _item.fldMaliyatMashmool.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i7 + 2)];
                            Cell.PutValue(fldMaliyatMashmool);
                            i7++;
                        }
                        index++;
                        break;
                    case "fldMashmolBime":
                        Check = "مبلغ مشمول بیمه";
                        Cell cell8 = sheet.Cells[alpha[index] + "1"];
                        cell8.PutValue(Check);
                        int i8 = 0;
                        foreach (var _item in Details)
                        {
                            fldMashmolBime = _item.fldMashmolBime.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i8 + 2)];
                            Cell.PutValue(fldMashmolBime);
                            i8++;
                        }
                        index++;
                        break;
                    case "fldMashmolMaliyat":
                        Check = "مبلغ مشمول مالیات";
                        Cell cell9 = sheet.Cells[alpha[index] + "1"];
                        cell9.PutValue(Check);
                        int i9 = 0;
                        foreach (var _item in Details)
                        {
                            fldMashmolMaliyat = _item.fldMashmolMaliyat.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i9 + 2)];
                            Cell.PutValue(fldMashmolMaliyat);
                            i9++;
                        }
                        index++;
                        break;
                    case "fldYear":
                        Check = "سال محاسبه";
                        Cell cell10 = sheet.Cells[alpha[index] + "1"];
                        cell10.PutValue(Check);
                        int i10 = 0;
                        foreach (var _item in Details)
                        {
                            fldYear = _item.fldYear.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i10 + 2)];
                            Cell.PutValue(fldYear);
                            i10++;
                        }
                        index++;
                        break;
                    case "fldMonth":
                        Check = "ماه محاسبه";
                        Cell cell11 = sheet.Cells[alpha[index] + "1"];
                        cell11.PutValue(Check);
                        int i11 = 0;
                        foreach (var _item in Details)
                        {
                            fldMonth = _item.fldMonthName;
                            Cell Cell = sheet.Cells[alpha[index] + (i11 + 2)];
                            Cell.PutValue(fldMonth);
                            i11++;
                        }
                        index++;
                        break;
                    case "fldYearPardakht":
                        Check = "سال پرداخت";
                        Cell cell12 = sheet.Cells[alpha[index] + "1"];
                        cell12.PutValue(Check);
                        int i12 = 0;
                        foreach (var _item in Details)
                        {
                            fldYearPardakht = _item.fldYearPardakht.ToString();
                            Cell Cell = sheet.Cells[alpha[index] + (i12 + 2)];
                            Cell.PutValue(fldYearPardakht);
                            i12++;
                        }
                        index++;
                        break;
                    case "fldMonthPardakht":
                        Check = "ماه پرداخت";
                        Cell cell13 = sheet.Cells[alpha[index] + "1"];
                        cell13.PutValue(Check);
                        int i13 = 0;
                        foreach (var _item in Details)
                        {
                            fldMonthPardakht = _item.fldMonthPardakhtName;
                            Cell Cell = sheet.Cells[alpha[index] + (i13 + 2)];
                            Cell.PutValue(fldMonthPardakht);
                            i13++;
                        }
                        index++;
                        break;
                }
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            return File(stream.ToArray(), "xls", "FishHoghoughiDetails.xls");
        }
        public ActionResult ReadMoavagheDetails(StoreRequestParameters parameters, int PersonalId, byte Nobat, short Year, byte Month, byte FilterType, byte TypeHesab)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<Models.FishDetails> Details = null;
            List<WCF_PayRoll.OBJ_MotalebatDetails> data = null;
            List<WCF_PayRoll.OBJ_MotalebatDetails> data2 = null;

            if (FilterType == 0)
            {
                data = PayServic.SelectMotalebat_Moavaghe_Details(PersonalId, Nobat, Year, Month,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                    Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();

                data2 = PayServic.SelectKosourat_Moavaghe_Details(PersonalId, Nobat, Year, Month,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                    Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();
            }
            else
            {
                data = PayServic.SelectMotalebat_Moavaghe_Details(PersonalId, Nobat, Year, Month,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                    Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).Where(l => l.fldCalcType == FilterType).ToList();

                data2 = PayServic.SelectKosourat_Moavaghe_Details(PersonalId, Nobat, Year, Month,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                    Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).Where(l => l.fldCalcType == FilterType).ToList();
            }

            Details = data.Select(l => new Models.FishDetails()//مطالبات
            {
                fldBed = l.fldMablagh,
                fldBes = 0,
                fldHokmId = l.fldHokmId,
                fldKarkard = l.fldKarkard,
                fldMashmolBime = l.fldMashmolBime,
                fldMashmolMaliyat = l.fldMashmolMaliyat,
                fldMonthName = MyLib.Shamsi.ShamsiMonthname(l.fldMonth),
                fldMonthPardakhtName = MyLib.Shamsi.ShamsiMonthname(l.fldMonthPardakht),
                fldTitle = l.fldTitle,
                fldGroupedTitle = l.fldTitle,
                fldYear = l.fldYear,
                fldGrouped = (l.fldItemsHoghughiId == 1 || l.fldItemsHoghughiId == 2 || l.fldItemsHoghughiId == 3 || l.fldItemsHoghughiId == 4
                || l.fldItemsHoghughiId == 5 || l.fldItemsHoghughiId == 30 || l.fldItemsHoghughiId == 31 || l.fldItemsHoghughiId == 38) ? 1 :
                (l.fldItemsHoghughiId == 19 || l.fldItemsHoghughiId == 47 || l.fldItemsHoghughiId == 48 || l.fldItemsHoghughiId == 60) ? 2 :
                (l.fldItemsHoghughiId == 21 || l.fldItemsHoghughiId == 49 || l.fldItemsHoghughiId == 50 || l.fldItemsHoghughiId == 61) ? 3 :
                (l.fldItemsHoghughiId == 20 || l.fldItemsHoghughiId == 51 || l.fldItemsHoghughiId == 52) ? 4 :
                (l.fldItemsHoghughiId == 44 || l.fldItemsHoghughiId == 45) ? 6 :
                (l.fldItemsHoghughiId == 54 || l.fldItemsHoghughiId == 62) ? 5 : 0,
                fldYearPardakht = l.fldYearPardakht,
                fldtype = 1,
                fldCalcType = l.fldCalcType,
                fldMonth = l.fldMonth,
                fldMonthPardakht = l.fldMonthPardakht,
                fldOrder = Convert.ToInt32((l.fldCalcType.ToString() + l.fldYear.ToString() + l.fldMonth.ToString().PadLeft(2, '0'))),
                fldMablaghHokm = l.fldMablaghHokm,
                fldBimeMashmool = l.fldBimeMashmool,
                fldMaliyatMashmool = l.fldMaliyatMashmool,
                fldCalcTypeName = l.fldCalcType == 1 ?
                "جاری (سال " + l.fldYear + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonth) + ")"
                    //")</br></br>مشمول بیمه: " + l.fldMashmolBime.ToString("N0") + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; مشمول مالیات: " + l.fldMashmolMaliyat.ToString("N0")
                    //: FilterType != 2 ? "معوقه (سال " + l.fldYearPardakht + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonthPardakht) + ")"
                : "معوقه (سال " + l.fldYear + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonth) + ")"//+
                //")</br></br>مشمول بیمه: " + l.fldMashmolBime.ToString("N0") + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; مشمول مالیات: " + l.fldMashmolMaliyat.ToString("N0")
            }).ToList();

            Details.AddRange(data2.Select(l => new Models.FishDetails()//کسورات
            {
                fldBed = 0,
                fldBes = l.fldMablagh,
                fldHokmId = l.fldHokmId,
                fldKarkard = l.fldKarkard,
                fldMashmolBime = l.fldMashmolBime,
                fldMashmolMaliyat = l.fldMashmolMaliyat,
                fldMonthName = MyLib.Shamsi.ShamsiMonthname(l.fldMonth),
                fldMonthPardakhtName = MyLib.Shamsi.ShamsiMonthname(l.fldMonthPardakht),
                fldGrouped = (l.fldItemsHoghughiId == 1 || l.fldItemsHoghughiId == 2 || l.fldItemsHoghughiId == 3 || l.fldItemsHoghughiId == 4
                || l.fldItemsHoghughiId == 5 || l.fldItemsHoghughiId == 30 || l.fldItemsHoghughiId == 31 || l.fldItemsHoghughiId == 38) ? 1 :
                (l.fldItemsHoghughiId == 19 || l.fldItemsHoghughiId == 47 || l.fldItemsHoghughiId == 48 || l.fldItemsHoghughiId == 60) ? 2 :
                (l.fldItemsHoghughiId == 21 || l.fldItemsHoghughiId == 49 || l.fldItemsHoghughiId == 50 || l.fldItemsHoghughiId == 61) ? 3 :
                (l.fldItemsHoghughiId == 20 || l.fldItemsHoghughiId == 51 || l.fldItemsHoghughiId == 52) ? 4 :
                (l.fldItemsHoghughiId == 44 || l.fldItemsHoghughiId == 45) ? 6 :
                (l.fldItemsHoghughiId == 54 || l.fldItemsHoghughiId == 62) ? 5 : 0,
                fldTitle = l.fldTitle,
                fldGroupedTitle = l.fldTitle,
                fldYear = l.fldYear,
                fldYearPardakht = l.fldYearPardakht,
                fldtype = 2,
                fldCalcType = l.fldCalcType,
                fldMonth = l.fldMonth,
                fldMonthPardakht = l.fldMonthPardakht,
                fldOrder = Convert.ToInt32((l.fldCalcType.ToString() + l.fldYear.ToString() + l.fldMonth.ToString().PadLeft(2, '0'))),
                fldMablaghHokm = l.fldMablaghHokm,
                fldBimeMashmool = l.fldBimeMashmool,
                fldMaliyatMashmool = l.fldMaliyatMashmool,
                fldCalcTypeName = l.fldCalcType == 1 ?
                "جاری (سال " + l.fldYear + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonth) + ")"
                    //")</br></br>مشمول بیمه: " + l.fldMashmolBime.ToString("N0") + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; مشمول مالیات: " + l.fldMashmolMaliyat.ToString("N0")
                    //: FilterType != 2 ? "معوقه (سال " + l.fldYearPardakht + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonthPardakht) + ")"
                : "معوقه (سال " + l.fldYear + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonth) + ")"
                // ")</br></br>مشمول بیمه: " + l.fldMashmolBime.ToString("N0") + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; مشمول مالیات: " + l.fldMashmolMaliyat.ToString("N0")
            }).ToList());
            return this.Store(Details.OrderBy(l => l.fldtype)
                .ThenByDescending(l => l.fldYear).ThenBy(l => l.fldMonth).ThenByDescending(l => l.fldHokmId).ThenByDescending(l => l.fldGrouped), Details.Count);
        }
        public ActionResult Read(StoreRequestParameters parameters, int PersonalId, byte Nobat, short Year, string Month, short TaYear, string TaMonth, byte FilterType, byte TypeHesab)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<Models.FishDetails> Details = null;
            List<WCF_PayRoll.OBJ_MotalebatDetails> data = null;
            List<WCF_PayRoll.OBJ_MotalebatDetails> data2 = null;
            var Az =Convert.ToInt32(Year + Month);
            var Ta = Convert.ToInt32(TaYear + TaMonth);

            if (FilterType == 0)
            {
                data = PayServic.SelectMotalebat_Details(PersonalId, Nobat, Az, Ta,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                    Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();

                data2 = PayServic.SelectKosourat_Details(PersonalId, Nobat, Az, Ta,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                    Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();
            }
            else
            {
                data = PayServic.SelectMotalebat_Details(PersonalId, Nobat, Az, Ta,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                    Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).Where(l=>l.fldCalcType==FilterType).ToList();

                data2 = PayServic.SelectKosourat_Details(PersonalId, Nobat, Az, Ta,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                    Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).Where(l => l.fldCalcType == FilterType).ToList();
            }

            Details = data.Select(l => new Models.FishDetails()//مطالبات
            {
                fldBed=l.fldMablagh,
                fldBes=0,
                fldHokmId=l.fldHokmId,
                fldKarkard=l.fldKarkard,
                fldMashmolBime=l.fldMashmolBime,
                fldMashmolMaliyat=l.fldMashmolMaliyat,
                fldMonthName=MyLib.Shamsi.ShamsiMonthname(l.fldMonth),
                fldMonthPardakhtName=MyLib.Shamsi.ShamsiMonthname(l.fldMonthPardakht),
                fldTitle=l.fldTitle,
                fldGroupedTitle = l.fldTitle,
                fldYear=l.fldYear,
                fldGrouped = (l.fldItemsHoghughiId == 1 || l.fldItemsHoghughiId == 2 || l.fldItemsHoghughiId == 3 || l.fldItemsHoghughiId == 4
                || l.fldItemsHoghughiId == 5 || l.fldItemsHoghughiId == 30 || l.fldItemsHoghughiId == 31 || l.fldItemsHoghughiId == 38) ? 1 :
                (l.fldItemsHoghughiId == 19 || l.fldItemsHoghughiId == 47 || l.fldItemsHoghughiId == 48 || l.fldItemsHoghughiId == 60) ? 2 :
                (l.fldItemsHoghughiId == 21 || l.fldItemsHoghughiId == 49 || l.fldItemsHoghughiId == 50 || l.fldItemsHoghughiId == 61) ? 3 :
                (l.fldItemsHoghughiId == 20 || l.fldItemsHoghughiId == 51 || l.fldItemsHoghughiId == 52) ? 4 :
                (l.fldItemsHoghughiId == 44 || l.fldItemsHoghughiId == 45) ? 6 :
                (l.fldItemsHoghughiId == 54 || l.fldItemsHoghughiId == 62) ? 5 : 0,
                fldYearPardakht=l.fldYearPardakht,
                fldtype=1,
                fldCalcType=l.fldCalcType,
                fldMonth = l.fldMonth,
                fldMonthPardakht = l.fldMonthPardakht,
                fldOrder = Convert.ToInt32((l.fldCalcType.ToString()+l.fldYear.ToString()+l.fldMonth.ToString().PadLeft(2,'0'))),
                fldMablaghHokm=l.fldMablaghHokm,
                fldBimeMashmool=l.fldBimeMashmool,
                fldMaliyatMashmool=l.fldMaliyatMashmool,
                fldCalcTypeName = l.fldCalcType == 1 ?
                "جاری (سال " + l.fldYear + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonth) + ")"
                //")</br></br>مشمول بیمه: " + l.fldMashmolBime.ToString("N0") + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; مشمول مالیات: " + l.fldMashmolMaliyat.ToString("N0")
                //: FilterType != 2 ? "معوقه (سال " + l.fldYearPardakht + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonthPardakht) + ")"
                : "معوقه (سال " + l.fldYear + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonth) + ")"//+
                //")</br></br>مشمول بیمه: " + l.fldMashmolBime.ToString("N0") + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; مشمول مالیات: " + l.fldMashmolMaliyat.ToString("N0")
            }).ToList();  
          
            Details.AddRange(data2.Select(l => new Models.FishDetails()//کسورات
            {
                fldBed = 0,
                fldBes = l.fldMablagh,
                fldHokmId=l.fldHokmId,
                fldKarkard=l.fldKarkard,
                fldMashmolBime=l.fldMashmolBime,
                fldMashmolMaliyat=l.fldMashmolMaliyat,
                fldMonthName=MyLib.Shamsi.ShamsiMonthname(l.fldMonth),
                fldMonthPardakhtName=MyLib.Shamsi.ShamsiMonthname(l.fldMonthPardakht),
                fldGrouped = (l.fldItemsHoghughiId == 1 || l.fldItemsHoghughiId == 2 || l.fldItemsHoghughiId == 3 || l.fldItemsHoghughiId == 4
                || l.fldItemsHoghughiId == 5 || l.fldItemsHoghughiId == 30 || l.fldItemsHoghughiId == 31 || l.fldItemsHoghughiId == 38) ? 1 :
                (l.fldItemsHoghughiId == 19 || l.fldItemsHoghughiId == 47 || l.fldItemsHoghughiId == 48 || l.fldItemsHoghughiId == 60) ? 2 :
                (l.fldItemsHoghughiId == 21 || l.fldItemsHoghughiId == 49 || l.fldItemsHoghughiId == 50 || l.fldItemsHoghughiId == 61) ? 3 :
                (l.fldItemsHoghughiId == 20 || l.fldItemsHoghughiId == 51 || l.fldItemsHoghughiId == 52) ? 4 :
                (l.fldItemsHoghughiId == 44 || l.fldItemsHoghughiId == 45) ? 6 :
                (l.fldItemsHoghughiId == 54 || l.fldItemsHoghughiId == 62) ? 5 : 0,
                fldTitle=l.fldTitle,
                fldGroupedTitle = l.fldTitle,
                fldYear=l.fldYear,
                fldYearPardakht=l.fldYearPardakht,               
                fldtype=2,
                fldCalcType = l.fldCalcType,
                fldMonth=l.fldMonth,
                fldMonthPardakht=l.fldMonthPardakht,
                fldOrder = Convert.ToInt32((l.fldCalcType.ToString() + l.fldYear.ToString() + l.fldMonth.ToString().PadLeft(2, '0'))),
                fldMablaghHokm=l.fldMablaghHokm,
                fldBimeMashmool=l.fldBimeMashmool,
                fldMaliyatMashmool=l.fldMaliyatMashmool,
                fldCalcTypeName = l.fldCalcType == 1 ?
                "جاری (سال " + l.fldYear + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonth) + ")"
                //")</br></br>مشمول بیمه: " + l.fldMashmolBime.ToString("N0") + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; مشمول مالیات: " + l.fldMashmolMaliyat.ToString("N0")
                //: FilterType != 2 ? "معوقه (سال " + l.fldYearPardakht + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonthPardakht) + ")"
                : "معوقه (سال " + l.fldYear + " ماه " + MyLib.Shamsi.ShamsiMonthname(l.fldMonth) + ")"
               // ")</br></br>مشمول بیمه: " + l.fldMashmolBime.ToString("N0") + "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; مشمول مالیات: " + l.fldMashmolMaliyat.ToString("N0")
            }).ToList());
            return this.Store(Details.OrderBy(l => l.fldtype)
                .ThenByDescending(l => l.fldYear).ThenBy(l => l.fldMonth).ThenByDescending(l => l.fldHokmId).ThenByDescending(l => l.fldGrouped), Details.Count);
        }
        public ActionResult ReadMoavaghe(StoreRequestParameters parameters, int PersonalId, int Nobat, short Year, byte Month, short YearPardakht, byte MonthPardakht, byte TypeHesab)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_FishHoghoghi_Moavaghe> data = null;
            data = PayServic.SelectMoavaghe_Details(PersonalId,Nobat,Year,Month,YearPardakht,MonthPardakht,TypeHesab, Session["Username"].ToString(), Session["Password"].ToString(),
                Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrPay).ToList();
            return this.Store(data);
        }
        public ActionResult ReadHokm(StoreRequestParameters parameters, int HokmId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            WCF_Personeli.OBJ_PersonalHokm data = null;
            data = PersonalServic.GetPersonalHokmDetail(HokmId, Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), IP, out ErrPersonal);
            return this.Store(data);
        }
        public ActionResult ReadHokm_Items(StoreRequestParameters parameters, int HokmId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Personeli.OBJ_Hokm_Item> data = null;
            data = PersonalServic.GetHokm_ItemFilter("fldPersonalHokmId",HokmId.ToString(),0, IP, out ErrPersonal).ToList();
            return this.Store(data);
        }
    }
}