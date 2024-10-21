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
using FastReport;
using System.Text;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using ClosedXML.Excel;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class ReportsController : Controller
    {
        //
        // GET: /PayRoll/Reports/
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

            //var Access = Models.Permission.haveAccess(1270, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]));
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            result.ViewBag.CostCenter = NewFMS.Models.CurrentDate.CostCenter;
            result.ViewBag.State = State;
            //result.ViewBag.Access = Access.ToString();
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        //public ActionResult GetTypeHesab()
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    var q = servic.GetHesabTypeWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList()
        //        .Select(c => new { fldId = c.fldId, fldTitle = c.fldTitle });
        //    return this.Store(q);
        //}
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }

        public ActionResult monthoutlet()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.CostCenter = NewFMS.Models.CurrentDate.CostCenter;
            return result;
        }
        public FileResult Excel12(short Year, byte NobatPardakht)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
                                 , "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV"
                                 , "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV","BW","BX","BY","BZ","CA","CB","CC","CD"};
            var StatusCheck = alpha; var Check = "";
            int index = 0;
            //   var re1 = PayServic.Rpt12Mahe(Year, Month, NobatPardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
            var re = PayServic.Rpt12Mahe(Year, NobatPardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).GroupBy(l => l.ماه).Select(l => l).ToList();
            Workbook wb = new Workbook();
            wb.Worksheets.Clear();

            for (int kkk = 0; kkk < re.Count; kkk++)
            {
                //Worksheet sheet = wb.Worksheets[kkk];
                Worksheet sheet = wb.Worksheets.Add((kkk + 1).ToString());
                Check = "آیدی پرسنل";
                Cell cell = sheet.Cells[alpha[index] + "1"];
                cell.PutValue(Check);
                int i = 0;
                index = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                    Cell.PutValue(_item.آیدی_پرسنل);
                    i++;
                }
                index++;
                Cell cell1 = sheet.Cells[alpha[index] + "1"];
                Check = "نام خانوادگی_نام";
                cell1.PutValue(Check);
                int j = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                    Cell.PutValue(_item.نام_خانوادگی_نام);
                    j++;
                }
                index++;
                Check = "نام پدر";
                Cell cell2 = sheet.Cells[alpha[index] + "1"];
                cell2.PutValue(Check);
                int k3 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (k3 + 2)];
                    Cell.PutValue(_item.نام_پدر);
                    k3++;
                }
                index++;
                Check = "کد ملی";
                Cell cell3 = sheet.Cells[alpha[index] + "1"];
                cell3.PutValue(Check);
                int k2 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (k2 + 2)];
                    Cell.PutValue(_item.کدملی);
                    k2++;
                }
                index++;
                Check = "ماه";
                Cell cell4 = sheet.Cells[alpha[index] + "1"];
                cell4.PutValue(Check);
                int j2 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (j2 + 2)];
                    Cell.PutValue(_item.ماه);
                    j2++;
                }
                index++;
                Check = "مالیات";
                Cell cell5 = sheet.Cells[alpha[index] + "1"];
                cell5.PutValue(Check);
                int k = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                    Cell.PutValue(_item.مالیات);
                    k++;
                }
                index++;
                Check = "مالیات معوقه";
                Cell cell6 = sheet.Cells[alpha[index] + "1"];
                cell6.PutValue(Check);
                int q = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                    Cell.PutValue(_item.مالیات_معوقه);
                    q++;
                }
                index++;
                Check = "معوقه";
                Cell cell7 = sheet.Cells[alpha[index] + "1"];
                cell7.PutValue(Check);
                int w = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                    Cell.PutValue(_item.معوقه);
                    w++;
                }
                index++;
                Check = "مشمول مالیات";
                Cell cell8 = sheet.Cells[alpha[index] + "1"];
                cell8.PutValue(Check);
                int z = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                    Cell.PutValue(_item.مشمول_مالیات);
                    z++;
                }
                index++;
                Check = "مشمول مالیات معوقه";
                Cell cell9 = sheet.Cells[alpha[index] + "1"];
                cell9.PutValue(Check);
                int x = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
                    Cell.PutValue(_item.مشمول_مالیات_معوقه);
                    x++;
                }
                index++;
                Check = "مطالبات پارامتری";
                Cell cell10 = sheet.Cells[alpha[index] + "1"];
                cell10.PutValue(Check);
                int y = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
                    Cell.PutValue(_item.مطالبات);
                    y++;
                }
                index++;
                Check = "کل مطالبات";
                Cell cell11 = sheet.Cells[alpha[index] + "1"];
                cell11.PutValue(Check);
                int a = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (a + 2)];
                    Cell.PutValue(_item.کل_مطالبات);
                    a++;
                }
                index++;
                Check = "حق اولاد";
                Cell cell12 = sheet.Cells[alpha[index] + "1"];
                cell12.PutValue(Check);
                int b = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (b + 2)];
                    Cell.PutValue(_item.حق_اولاد);
                    b++;
                }
                index++;
                Check = "عائله مندی";
                Cell cell13 = sheet.Cells[alpha[index] + "1"];
                cell13.PutValue(Check);
                int s = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (s + 2)];
                    Cell.PutValue(_item.عائله_مندی);
                    s++;
                }
                index++;
                Check = "حق مسکن";
                Cell cell14 = sheet.Cells[alpha[index] + "1"];
                cell14.PutValue(Check);
                int r = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r + 2)];
                    Cell.PutValue(_item.حق_مسکن);
                    r++;
                }
                index++;
                Check = "تعطیل_کاری اضافه کاری";
                Cell cell15 = sheet.Cells[alpha[index] + "1"];
                cell15.PutValue(Check);
                int r1 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r1 + 2)];
                    Cell.PutValue(_item.تعطیل_کاری_اضافه_کاری);
                    r1++;
                }
                index++;
                Check = "ماموریت";
                Cell cell16 = sheet.Cells[alpha[index] + "1"];
                cell16.PutValue(Check);
                int r2 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r2 + 2)];
                    Cell.PutValue(_item.ماموریت);
                    r2++;
                }
                index++;
                Check = "سنوات پایان خدمت";
                Cell cell17 = sheet.Cells[alpha[index] + "1"];
                cell17.PutValue(Check);
                int r3 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r3 + 2)];
                    Cell.PutValue(_item.سنوات_پایان_خدمت);
                    r3++;
                }
                index++;
                Check = "تسهیلات رفاهی";
                Cell cell18 = sheet.Cells[alpha[index] + "1"];
                cell18.PutValue(Check);
                int r4 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r4 + 2)];
                    Cell.PutValue(_item.تسهیلات_رفاهی);
                    r4++;
                }
                index++;
                Check = "حقوق";
                Cell cell19 = sheet.Cells[alpha[index] + "1"];
                cell19.PutValue(Check);
                int r5 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r5 + 2)];
                    Cell.PutValue(_item.حقوق);
                    r5++;
                }
                index++;
                Check = "فوق_العاده_شغل";
                Cell cell20 = sheet.Cells[alpha[index] + "1"];
                cell20.PutValue(Check);
                int r6 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r6 + 2)];
                    Cell.PutValue(_item.فوق_العاده_شغل);
                    r6++;
                }
                index++;
                Check = "تحقیقی و تخصصی";
                Cell cell21 = sheet.Cells[alpha[index] + "1"];
                cell21.PutValue(Check);
                int r7 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r7 + 2)];
                    Cell.PutValue(_item.تحقیقی_و_تخصصی);
                    r7++;
                }
                index++;
                Check = "فوق العاده ماده26";
                Cell cell22 = sheet.Cells[alpha[index] + "1"];
                cell22.PutValue(Check);
                int r8 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r8 + 2)];
                    Cell.PutValue(_item.فوق_العاده_ماده26);
                    r8++;
                }
                index++;
                Check = "مدیریتی و سرپرستی";
                Cell cell23 = sheet.Cells[alpha[index] + "1"];
                cell23.PutValue(Check);
                int r9 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r9 + 2)];
                    Cell.PutValue(_item.مدیریتی_و_سرپرستی);
                    r9++;
                }
                index++;
                Check = "برجستگی";
                Cell cell24 = sheet.Cells[alpha[index] + "1"];
                cell24.PutValue(Check);
                int r10 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r10 + 2)];
                    Cell.PutValue(_item.برجستگی);
                    r10++;
                }
                index++;
                Check = "تفاوت تطبیق";
                Cell cell25 = sheet.Cells[alpha[index] + "1"];
                cell25.PutValue(Check);
                int r11 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r11 + 2)];
                    Cell.PutValue(_item.تفاوت_تطبیق);
                    r11++;
                }
                index++;
                Check = "فوق العاده ایثارگری";
                Cell cell26 = sheet.Cells[alpha[index] + "1"];
                cell26.PutValue(Check);
                int r12 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r12 + 2)];
                    Cell.PutValue(_item.فوق_العاده_ایثارگری);
                    r12++;
                }
                index++;
                Check = "بدی آب و هوا";
                Cell cell27 = sheet.Cells[alpha[index] + "1"];
                cell27.PutValue(Check);
                int r13 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r13 + 2)];
                    Cell.PutValue(_item.بدی_آب_و_هوا);
                    r13++;
                }
                index++;
                Check = "تسهیلات زندگی";
                Cell cell28 = sheet.Cells[alpha[index] + "1"];
                cell28.PutValue(Check);
                int r14 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r14 + 2)];
                    Cell.PutValue(_item.تسهیلات_زندگی);
                    r14++;
                }
                index++;
                Check = "سختی کار";
                Cell cell29 = sheet.Cells[alpha[index] + "1"];
                cell29.PutValue(Check);
                int r15 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r15 + 2)];
                    Cell.PutValue(_item.سختی_کار);
                    r15++;
                }
                index++;
                Check = "فوق العاده تعدیل";
                Cell cell30 = sheet.Cells[alpha[index] + "1"];
                cell30.PutValue(Check);
                int r16 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r16 + 2)];
                    Cell.PutValue(_item.فوق_العاده_تعدیل);
                    r16++;
                }
                index++;
                Check = "مزایای ریالی گروه تشویقی";
                Cell cell31 = sheet.Cells[alpha[index] + "1"];
                cell31.PutValue(Check);
                int r17 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r17 + 2)];
                    Cell.PutValue(_item.مزایای_ریالی_گروه_تشویقی);
                    r17++;
                }
                index++;
                Check = "حق جذب بند9";
                Cell cell32 = sheet.Cells[alpha[index] + "1"];
                cell32.PutValue(Check);
                int r18 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r18 + 2)];
                    Cell.PutValue(_item.حق_جذب_بند9);
                    r18++;
                }
                index++;
                Check = "خوار و بار";
                Cell cell33 = sheet.Cells[alpha[index] + "1"];
                cell33.PutValue(Check);
                int r19 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r19 + 2)];
                    Cell.PutValue(_item.خوار_و_بار);
                    r19++;
                }
                index++;
                Check = "نوبت کاری";
                Cell cell34 = sheet.Cells[alpha[index] + "1"];
                cell34.PutValue(Check);
                int r20 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r20 + 2)];
                    Cell.PutValue(_item.نوبت_کاری);
                    r20++;
                }
                index++;
                Check = "بن ماهیانه";
                Cell cell35 = sheet.Cells[alpha[index] + "1"];
                cell35.PutValue(Check);
                int r21 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r21 + 2)];
                    Cell.PutValue(_item.بن_ماهیانه);
                    r21++;
                }
                index++;
                Check = "حق جذب تبصره 7 ماده یک";
                Cell cell36 = sheet.Cells[alpha[index] + "1"];
                cell36.PutValue(Check);
                int r22 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r22 + 2)];
                    Cell.PutValue(_item.حق_جذب_تبصره_7_ماده_یک);
                    r22++;
                }
                index++;
                Check = "فوق العاده تلاش";
                Cell cell37 = sheet.Cells[alpha[index] + "1"];
                cell37.PutValue(Check);
                int r23 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r23 + 2)];
                    Cell.PutValue(_item.فوق_العاده_تلاش);
                    r23++;
                }
                index++;
                Check = "سایر مزایا";
                Cell cell38 = sheet.Cells[alpha[index] + "1"];
                cell38.PutValue(Check);
                int r24 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r24 + 2)];
                    Cell.PutValue(_item.سایر_مزایا);
                    r24++;
                }
                index++;
                Check = "اضافه کاری";
                Cell cell39 = sheet.Cells[alpha[index] + "1"];
                cell39.PutValue(Check);
                int r25 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r25 + 2)];
                    Cell.PutValue(_item.اضافه_کاری);
                    r25++;
                }
                index++;
                Check = "تعطیل کاری";
                Cell cell40 = sheet.Cells[alpha[index] + "1"];
                cell40.PutValue(Check);
                int r26 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r26 + 2)];
                    Cell.PutValue(_item.تعطیل_کاری);
                    r26++;
                }
                index++;
                Check = "حق جذب قضایی";
                Cell cell41 = sheet.Cells[alpha[index] + "1"];
                cell41.PutValue(Check);
                int r27 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r27 + 2)];
                    Cell.PutValue(_item.حق_جذب_قضایی);
                    r27++;
                }
                index++;
                Check = "ضریب تعدیل";
                Cell cell42 = sheet.Cells[alpha[index] + "1"];
                cell42.PutValue(Check);
                int r28 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r28 + 2)];
                    Cell.PutValue(_item.ضریب_تعدیل);
                    r28++;
                }
                index++;
                Check = "جذب مشاغل تخصصی";
                Cell cell43 = sheet.Cells[alpha[index] + "1"];
                cell43.PutValue(Check);
                int r29 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r29 + 2)];
                    Cell.PutValue(_item.جذب_مشاغل_تخصصی);
                    r29++;
                }
                index++;
                Check = "جذب تعدیل";
                Cell cell44 = sheet.Cells[alpha[index] + "1"];
                cell44.PutValue(Check);
                int r30 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r30 + 2)];
                    Cell.PutValue(_item.جذب_تعدیل);
                    r30++;
                }
                index++;
                Check = "تفاوت_ناشی از ضریب تعدیل";
                Cell cell45 = sheet.Cells[alpha[index] + "1"];
                cell45.PutValue(Check);
                int r31 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r31 + 2)];
                    Cell.PutValue(_item.تفاوت_ناشی_از_ضریب_تعدیل);
                    r31++;
                }
                index++;
                Check = "حق_شیفت";
                Cell cell46 = sheet.Cells[alpha[index] + "1"];
                cell46.PutValue(Check);
                int r32 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r32 + 2)];
                    Cell.PutValue(_item.حق_شیفت);
                    r32++;
                }
                index++;
                Check = "تفاوت بند ی ";
                Cell cell47 = sheet.Cells[alpha[index] + "1"];
                cell47.PutValue(Check);
                int r33 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r33 + 2)];
                    Cell.PutValue(_item.تفاوت_بند_ی_);
                    r33++;
                }
                index++;
                Check = "تفاوت جزء 1 ";
                Cell cell48 = sheet.Cells[alpha[index] + "1"];
                cell48.PutValue(Check);
                int r34 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r34 + 2)];
                    Cell.PutValue(_item.تفاوت_جزء_1_);
                    r34++;
                }
                index++;
                Check = "تفاوت ناشی از بند6";
                Cell cell49 = sheet.Cells[alpha[index] + "1"];
                cell49.PutValue(Check);
                int r35 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r35 + 2)];
                    Cell.PutValue(_item.تفاوت_ناشی_از_بند6);
                    r35++;
                }
                index++;
                Check = "فوق العاده جذب";
                Cell cell50 = sheet.Cells[alpha[index] + "1"];
                cell50.PutValue(Check);
                int r36 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r36 + 2)];
                    Cell.PutValue(_item.فوق_العاده_جذب);
                    r36++;
                }
                index++;
                Check = "فوق العاده ویژه";
                Cell cell51 = sheet.Cells[alpha[index] + "1"];
                cell51.PutValue(Check);
                int r37 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r37 + 2)];
                    Cell.PutValue(_item.فوق_العاده_ویژه);
                    r37++;
                }
                index++;
                Check = "فوق العاده مخصوص";
                Cell cell52 = sheet.Cells[alpha[index] + "1"];
                cell52.PutValue(Check);
                int r38 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r38 + 2)];
                    Cell.PutValue(_item.فوق_العاده_مخصوص);
                    r38++;
                }
                index++;
                Check = "تفاوت تطبیق موضوع جزء1";
                Cell cell53 = sheet.Cells[alpha[index] + "1"];
                cell53.PutValue(Check);
                int r39 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r39 + 2)];
                    Cell.PutValue(_item.تفاوت_تطبیق_موضوع_جزء1);
                    r39++;
                }
                index++;
                Check = "فوق العاده خاص";
                Cell cell54 = sheet.Cells[alpha[index] + "1"];
                cell54.PutValue(Check);
                int r40 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r40 + 2)];
                    Cell.PutValue(_item.فوق_العاده_خاص);
                    r40++;
                }
                index++;
                Check = "کارانه";
                Cell cell55 = sheet.Cells[alpha[index] + "1"];
                cell55.PutValue(Check);
                int r41 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r41 + 2)];
                    Cell.PutValue(_item.کارانه);
                    r41++;
                }
                index++;
                Check = "ترمیم حقوق";
                Cell cell56 = sheet.Cells[alpha[index] + "1"];
                cell56.PutValue(Check);
                int r42 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r42 + 2)];
                    Cell.PutValue(_item.ترمیم_حقوق);
                    r42++;
                }
                index++;
                Check = "بیمه معاف";
                Cell cell57 = sheet.Cells[alpha[index] + "1"];
                cell57.PutValue(Check);
                int r43 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r43 + 2)];
                    Cell.PutValue(_item.بیمه_معاف);
                    r43++;
                }
                index++;
                Check = "پس انداز+پس انداز معوق";
                Cell cell58 = sheet.Cells[alpha[index] + "1"];
                cell58.PutValue(Check);
                int r44 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r44 + 2)];
                    Cell.PutValue(_item.پس_انداز_پس_انداز_معوق);
                    r44++;
                }
                index++;
                Check = "بیمه تکمیلی کارفرما";
                Cell cell59 = sheet.Cells[alpha[index] + "1"];
                cell59.PutValue(Check);
                int r45 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r45 + 2)];
                    Cell.PutValue(_item.بیمه_تکمیلی_کارفرما);
                    r45++;
                }
                index++;




                Check = "جمع مطالبات";
                Cell cell60 = sheet.Cells[alpha[index] + "1"];
                cell60.PutValue(Check);
                int r46 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r46 + 2)];
                    Cell.PutValue(_item.جمع_مطالبات);
                    r46++;
                }
                index++;
                Check = "حق درمان سهم کارفرما";
                Cell cell61 = sheet.Cells[alpha[index] + "1"];
                cell61.PutValue(Check);
                int r47 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r47 + 2)];
                    Cell.PutValue(_item.حق_درمان_سهم_کارفرما);
                    r47++;
                }
                index++;
                Check = "حق درمان سهم کارفرما(معوقه)";
                Cell cell62 = sheet.Cells[alpha[index] + "1"];
                cell62.PutValue(Check);
                int r48 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r48 + 2)];
                    Cell.PutValue(_item.حق_درمان_سهم_کارفرما_معوق);
                    r48++;
                }
                index++;
                Check = "حق درمان سهم دولت";
                Cell cell63 = sheet.Cells[alpha[index] + "1"];
                cell63.PutValue(Check);
                int r49 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r49 + 2)];
                    Cell.PutValue(_item.حق_درمان_سهم_دولت);
                    r49++;
                }
                index++;
                Check = "حق درمان سهم دولت(معوقه)";
                Cell cell64 = sheet.Cells[alpha[index] + "1"];
                cell64.PutValue(Check);
                int r50 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r50 + 2)];
                    Cell.PutValue(_item.حق_درمان_سهم_دولت_معوق);
                    r50++;
                }
                index++;
                Check = "پس انداز";
                Cell cell65 = sheet.Cells[alpha[index] + "1"];
                cell65.PutValue(Check);
                int r51 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r51 + 2)];
                    Cell.PutValue(_item.پس_انداز);
                    r51++;
                }
                index++;
                Check = "پس انداز معوق";
                Cell cell66 = sheet.Cells[alpha[index] + "1"];
                cell66.PutValue(Check);
                int r52 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r52 + 2)];
                    Cell.PutValue(_item.پس_انداز_معوق);
                    r52++;
                }
                index++;
                Check = "بیمه تکمیلی سهم کارفرما";
                Cell cell67 = sheet.Cells[alpha[index] + "1"];
                cell67.PutValue(Check);
                int r53 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r53 + 2)];
                    Cell.PutValue(_item.بیمه_تکمیلی_سهم_کارفرما);
                    r53++;
                }
                index++;
                Check = "بیمه عمر سهم کارفرما";
                Cell cell68 = sheet.Cells[alpha[index] + "1"];
                cell68.PutValue(Check);
                int r54 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r54 + 2)];
                    Cell.PutValue(_item.بیمه_عمر_سهم_کارفرما);
                    r54++;
                }
                index++;
                Check = "بیمه سهم کارفرما";
                Cell cell69 = sheet.Cells[alpha[index] + "1"];
                cell69.PutValue(Check);
                int r55 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r55 + 2)];
                    Cell.PutValue(_item.بیمه_سهم_کارفرما);
                    r55++;
                }
                index++;
                Check = "بیمه سهم کارفرما(معوقه)";
                Cell cell70 = sheet.Cells[alpha[index] + "1"];
                cell70.PutValue(Check);
                int r56 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r56 + 2)];
                    Cell.PutValue(_item.بیمه_سهم_کارفرما_معوق);
                    r56++;
                }
                index++;
                Check = "بیمه بیکاری";
                Cell cell71 = sheet.Cells[alpha[index] + "1"];
                cell71.PutValue(Check);
                int r57 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r57 + 2)];
                    Cell.PutValue(_item.بیمه_بیکاری);
                    r57++;
                }
                index++;
                Check = "بیمه بیکاری(معوقه)";
                Cell cell72 = sheet.Cells[alpha[index] + "1"];
                cell72.PutValue(Check);
                int r58 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r58 + 2)];
                    Cell.PutValue(_item.بیمه_بیکاری_معوق);
                    r58++;
                }
                index++;
                ////////////////
                Check = "مزایای_جانبی_رفاهی";
                Cell cell73 = sheet.Cells[alpha[index] + "1"];
                cell73.PutValue(Check);
                int r59 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r59 + 2)];
                    Cell.PutValue(_item.مزایای_جانبی_رفاهی);
                    r59++;
                }
                index++;
                Check = "هزینه_مهد_کودک";
                Cell cell74 = sheet.Cells[alpha[index] + "1"];
                cell74.PutValue(Check);
                int r60 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r60 + 2)];
                    Cell.PutValue(_item.هزینه_مهد_کودک);
                    r60++;
                }
                index++;
                Check = "اقلام_خوراکی";
                Cell cell75 = sheet.Cells[alpha[index] + "1"];
                cell75.PutValue(Check);
                int r61 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r61 + 2)];
                    Cell.PutValue(_item.اقلام_خوراکی);
                    r61++;
                }
                index++;
                Check = "کالاهای_بهداشتی";
                Cell cell76 = sheet.Cells[alpha[index] + "1"];
                cell76.PutValue(Check);
                int r62 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r62 + 2)];
                    Cell.PutValue(_item.کالاهای_بهداشتی);
                    r62++;
                }
                index++;
                Check = "مناسبت";
                Cell cell77 = sheet.Cells[alpha[index] + "1"];
                cell77.PutValue(Check);
                int r63 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r63 + 2)];
                    Cell.PutValue(_item.مناسبت);
                    r63++;
                }
                index++;
                Check = "مزایای_جوانی_جمعیت";
                Cell cell78 = sheet.Cells[alpha[index] + "1"];
                cell78.PutValue(Check);
                int r64 = 0;
                foreach (var _item in re[kkk])
                {
                    Cell Cell = sheet.Cells[alpha[index] + (r64 + 2)];
                    Cell.PutValue(_item.مزایای_جوانی_جمعیت);
                    r64++;
                }
                index++;
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            return File(stream.ToArray(), "xls", "گزارش 12 ماهه.xls");

        }
        public ActionResult RptListPardakhti(string containerId, string State)
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
            result.ViewBag.CostCenter = NewFMS.Models.CurrentDate.CostCenter;
            result.ViewBag.State = State;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }

        public ActionResult Vam(string containerId, string State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.State = State;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }


        public ActionResult GetCostCenter(int? OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var IdOrgan = Session["OrganId"].ToString();
            if (OrganId != null && OrganId != 0)
            {
                IdOrgan = OrganId.ToString();
            }
            var q = PayServic.GetCostCenterWithFilter("fldOrganId", IdOrgan, IdOrgan
                , 0, IP, out ErrPay).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }

        public ActionResult GetBime()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Comon_Pay.GetTypeBimeWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }

        public ActionResult GetTypeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Comon_Pay.GetTypeEstekhdamWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult RptKosorBazneshastegi(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            return result;
        }
        public ActionResult GeneratePDFRptKosorBazneshastegi(short Year, byte Month, byte nobatPardakht, string Sh_Fish, string DateVariz)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_PayRptSettingTableAdapter Setting = new DataSet.DataSet1TableAdapters.spr_PayRptSettingTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKosurBazneshastegiTableAdapter KosurBazneshastegi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKosurBazneshastegiTableAdapter();

                var Kosur = PayServic.GetRptKosurBazneshastegi(Year, Month, nobatPardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).FirstOrDefault();
                dt.EnforceConstraints = false;
                Setting.Fill(dt.spr_PayRptSetting, Convert.ToInt32(Session["OrganId"]));
                KosurBazneshastegi.Fill(dt.spr_Pay_RptKosurBazneshastegi, Year, Month, nobatPardakht, Convert.ToInt32(Session["OrganId"]));

                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);

                decimal jamkol = Kosur.MogharariMahAval + Kosur.Mogharari + Kosur.SahamKarmand + Kosur.SahmKarfarma + Kosur.SahamKarmandMoavaghe + Kosur.SahmKarfarmaMoavaghe;
                string horof = MyLib.NumberTool.Num2Str(Convert.ToUInt64(jamkol), 1);
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\KosurBazneshastegi.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("sh", Sh_Fish);
                Report.SetParameterValue("tarikh", DateVariz);
                Report.SetParameterValue("horof", horof);
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
        public ActionResult GenerateExcelRptKosorBazneshastegi(short Year, byte Month, byte nobatPardakht, string Sh_Fish, string DateVariz)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_PayRptSettingTableAdapter Setting = new DataSet.DataSet1TableAdapters.spr_PayRptSettingTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKosurBazneshastegiTableAdapter KosurBazneshastegi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKosurBazneshastegiTableAdapter();

                var Kosur = PayServic.GetRptKosurBazneshastegi(Year, Month, nobatPardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).FirstOrDefault();
                dt.EnforceConstraints = false;
                Setting.Fill(dt.spr_PayRptSetting, Convert.ToInt32(Session["OrganId"]));
                KosurBazneshastegi.Fill(dt.spr_Pay_RptKosurBazneshastegi, Year, Month, nobatPardakht, Convert.ToInt32(Session["OrganId"]));

                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);

                decimal jamkol = Kosur.MogharariMahAval + Kosur.Mogharari + Kosur.SahamKarmand + Kosur.SahmKarfarma + Kosur.SahamKarmandMoavaghe + Kosur.SahmKarfarmaMoavaghe;
                string horof = MyLib.NumberTool.Num2Str(Convert.ToUInt64(jamkol), 1);
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\KosurBazneshastegi.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("sh", Sh_Fish);
                Report.SetParameterValue("tarikh", DateVariz);
                Report.SetParameterValue("horof", horof);
                FastReport.Export.Xml.XMLExport excel = new FastReport.Export.Xml.XMLExport();
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(excel, stream);
                return File(stream.ToArray(), "application/vnd.ms-excel", "form5-1.xls");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CheckListKhadamatDarmani(int OrganId,short Year, byte Month, byte nobatPardakht)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKhadamatDarmaniTableAdapter RptKhadamatDarmani = new DataSet.DataSet1TableAdapters.spr_Pay_RptKhadamatDarmaniTableAdapter();
            RptKhadamatDarmani.Fill(dt.spr_Pay_RptKhadamatDarmani, Year, Month, nobatPardakht, OrganId);
            var AllowPrint = true;
            var q = dt.spr_Pay_RptKhadamatDarmani.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFListKhadamatDarmani(int OrganId,short Year, byte Month, byte nobatPardakht)
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
                DataSet.DataSet1TableAdapters.spr_Pay_RptKhadamatDarmaniTableAdapter RptKhadamatDarmani = new DataSet.DataSet1TableAdapters.spr_Pay_RptKhadamatDarmaniTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                RptKhadamatDarmani.Fill(dt.spr_Pay_RptKhadamatDarmani, Year, Month, nobatPardakht, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 3, IP, out Err).ToList();
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

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListBimeKhadamatDarmani.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
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

        public ActionResult GeneratePDFListPardakhtEydi(short Year, byte Month, int costCenterId, string costCenterName, byte nobatPardakht, string nobatPardakhtName,int OrganId)
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
                DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtEydiTableAdapter ListPardakhtEydi = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtEydiTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                ListPardakhtEydi.Fill(dt.spr_Pay_RptListPardakhtEydi, costCenterId, Year, Month, nobatPardakht, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 21, IP, out Err).ToList();
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtEydi.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
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
                Report.SetParameterValue("Sal", Year);
                Report.SetParameterValue("NobatPardakht", nobatPardakhtName);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("MarkazHazine", costCenterName);
                Report.SetParameterValue("flag", flag);
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
        public ActionResult GenerateExcelListPardakhtEydi(short Year, byte Month, int costCenterId, string costCenterName, byte nobatPardakht, string nobatPardakhtName, int OrganId)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
                                 , "AA", "AB", "AC"};
            var StatusCheck = alpha; var Check = "";
            int index = 0;
            var eydi = PayServic.RptListPardakhtEydi(costCenterId, Year, Month, nobatPardakht, OrganId, IP, out ErrPay).ToList();
            Workbook wb = new Workbook();
            wb.Worksheets.Clear();

            Worksheet sheet = wb.Worksheets.Add(("لیست پرداخت عیدی"));
            Check = "آیدی پرسنل";
            Cell cell = sheet.Cells[alpha[index] + "1"];
            cell.PutValue(Check);
            int i = 0;
            index = 0;
            foreach (var _item in eydi)
            {
                Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                Cell.PutValue(_item.fldPersonalId);
                i++;
            }
            index++;
            Cell cell1 = sheet.Cells[alpha[index] + "1"];
            Check = "نام خانوادگی_نام";
            cell1.PutValue(Check);
            int j = 0;
            foreach (var _item in eydi)
            {
                Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                Cell.PutValue(_item.fldName_Family);
                j++;
            }
            index++;
            Check = "نام پدر";
            Cell cell2 = sheet.Cells[alpha[index] + "1"];
            cell2.PutValue(Check);
            int k3 = 0;
            foreach (var _item in eydi)
            {
                Cell Cell = sheet.Cells[alpha[index] + (k3 + 2)];
                Cell.PutValue(_item.fldFatherName);
                k3++;
            }
            index++;
            Check = "شماره حساب";
            Cell cell3 = sheet.Cells[alpha[index] + "1"];
            cell3.PutValue(Check);
            int k2 = 0;
            foreach (var _item in eydi)
            {
                Cell Cell = sheet.Cells[alpha[index] + (k2 + 2)];
                Cell.PutValue(_item.fldShomareHesab);
                k2++;
            }
            index++;
            Check = "سایر کسورات";
            Cell cell4 = sheet.Cells[alpha[index] + "1"];
            cell4.PutValue(Check);
            int j2 = 0;
            foreach (var _item in eydi)
            {
                Cell Cell = sheet.Cells[alpha[index] + (j2 + 2)];
                Cell.PutValue(_item.fldKosurat);
                j2++;
            }
            index++;
            Check = "مبلغ عیدی";
            Cell cell5 = sheet.Cells[alpha[index] + "1"];
            cell5.PutValue(Check);
            int k = 0;
            foreach (var _item in eydi)
            {
                Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                Cell.PutValue(_item.fldMablagh);
                k++;
            }
            index++;
            Check = "مالیات";
            Cell cell6 = sheet.Cells[alpha[index] + "1"];
            cell6.PutValue(Check);
            int q = 0;
            foreach (var _item in eydi)
            {
                Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                Cell.PutValue(_item.fldMaliyat);
                q++;
            }
            index++;
            Check = "تعداد روز";
            Cell cell7 = sheet.Cells[alpha[index] + "1"];
            cell7.PutValue(Check);
            int w = 0;
            foreach (var _item in eydi)
            {
                Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                Cell.PutValue(_item.fldDayCount);
                w++;
            }
            index++;
            Check = "خالص پرداختی";
            Cell cell8 = sheet.Cells[alpha[index] + "1"];
            cell8.PutValue(Check);
            int z = 0;
            foreach (var _item in eydi)
            {
                Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                Cell.PutValue(_item.fldKhalesPardakhti);
                z++;
            }
            index++;
            Check = "نوبت";
            Cell cell9 = sheet.Cells[alpha[index] + "1"];
            cell9.PutValue(Check);
            int x = 0;
            foreach (var _item in eydi)
            {
                Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
                Cell.PutValue(_item.NameNobat);
                x++;
            }
            index++;
            Check = "سال";
            Cell cell10 = sheet.Cells[alpha[index] + "1"];
            cell10.PutValue(Check);
            int y = 0;
            foreach (var _item in eydi)
            {
                Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
                Cell.PutValue(_item.Sal);
                y++;
            }
            index++;


            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            return File(stream.ToArray(), "xls", "لیست پرداخت عیدی.xls");

        }
        public ActionResult GeneratePDFListPardakhtMorakhasi(short Year, byte Month, int costCenterId, string costCenterName, int OrganId)
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
                DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtMorakhasiTableAdapter ListPardakhtMorakhasi = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtMorakhasiTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                ListPardakhtMorakhasi.Fill(dt.spr_Pay_RptListPardakhtMorakhasi, Year, Month, costCenterId, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 32, IP, out Err).ToList();
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat_Variz";
                
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), "", OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtMorakhasi.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
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
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("flag", flag);
                Report.SetParameterValue("Markaz", costCenterName);

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

        public ActionResult GeneratePDFListPardakhtSayer(short Year, byte Month, byte TypeHesab, byte MarhalePardakht, byte nobatPardakht, string MarhalePardakhtName, string nobatPardakhtName, string Title, int OrganId)
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
                DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtSayerPardakhtTableAdapter ListPardakhtSayer = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtSayerPardakhtTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                if (TypeHesab == 0)
                {
                    ListPardakhtSayer.Fill(dt.spr_Pay_RptListPardakhtSayerPardakht, "Hoghogh", Year, Month, nobatPardakht, MarhalePardakht, OrganId);
                }
                else
                {
                    ListPardakhtSayer.Fill(dt.spr_Pay_RptListPardakhtSayerPardakht, "bon", Year, Month, nobatPardakht, MarhalePardakht, OrganId);
                }

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 30, IP, out Err).ToList();
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtSayerPardakht.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
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
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("nobat", nobatPardakhtName);
                Report.SetParameterValue("Marhale", MarhalePardakhtName);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("sharh", Title);
                Report.SetParameterValue("flag", flag);

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

        public ActionResult GeneratePDFListPardakhtHoghough(short Year, byte Month, int costCenterId, string costCenterName, byte nobatPardakht, string nobatPardakhtName, byte TypeBime, string TypeList, byte moavaghe, bool type, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                int TypeEstekhdam = 1;
                string Path = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtHoghoghTableAdapter ListPardakhtHoghogh = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtHoghoghTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptNameItemEstekhdamTableAdapter NameItemEstekhdam = new DataSet.DataSet1TableAdapters.spr_RptNameItemEstekhdamTableAdapter();
                System.Data.DataSet dt_K_M = new System.Data.DataSet();
                dt_K_M.DataSetName = "DataSetKM";
                System.Data.DataTable Kosourat = new System.Data.DataTable();
                Kosourat.TableName = "Kosourat";
                System.Data.DataTable Motalebat = new System.Data.DataTable();
                Motalebat.TableName = "Motalebat";

                //ساختن ستونهای DataTable
                Kosourat.Columns.Add("Title", typeof(string));
                Kosourat.Columns.Add("Amount", typeof(int));
                Motalebat.Columns.Add("Title", typeof(string));
                Motalebat.Columns.Add("Amount", typeof(int));
                for (int i = 1; i < 10; i++)
                {
                    Kosourat.Columns.Add("Title" + i.ToString(), typeof(string));
                    Kosourat.Columns.Add("Amount" + i.ToString(), typeof(int));
                    Motalebat.Columns.Add("Title" + i.ToString(), typeof(string));
                    Motalebat.Columns.Add("Amount" + i.ToString(), typeof(int));
                }
                dt_K_M.Tables.Add(Kosourat);
                dt_K_M.Tables.Add(Motalebat);

                //پر کردن DataTable
                var k = PayServic.RptSumMotalebat_Kosurat("Kosurat", Year, Month, costCenterId, TypeBime, nobatPardakht, OrganId, IP, out ErrPay).ToList();
                var M = PayServic.RptSumMotalebat_Kosurat("Motalebat", Year, Month, costCenterId, TypeBime, nobatPardakht, OrganId, IP, out ErrPay).ToList();
                DataRow K_Row = Kosourat.NewRow();
                for (int i = 0; i < k.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            K_Row["Title"] = k[i].fldTitle;
                            K_Row["Amount"] = k[i].fldMablagh;
                            break;
                        case 1:
                            K_Row["Title1"] = k[i].fldTitle;
                            K_Row["Amount1"] = k[i].fldMablagh;
                            break;
                        case 2:
                            K_Row["Title2"] = k[i].fldTitle;
                            K_Row["Amount2"] = k[i].fldMablagh;
                            break;
                        case 3:
                            K_Row["Title3"] = k[i].fldTitle;
                            K_Row["Amount3"] = k[i].fldMablagh;
                            break;
                        case 4:
                            K_Row["Title4"] = k[i].fldTitle;
                            K_Row["Amount4"] = k[i].fldMablagh;
                            break;
                        case 5:
                            K_Row["Title5"] = k[i].fldTitle;
                            K_Row["Amount5"] = k[i].fldMablagh;
                            break;
                        case 6:
                            K_Row["Title6"] = k[i].fldTitle;
                            K_Row["Amount6"] = k[i].fldMablagh;
                            break;
                        case 7:
                            K_Row["Title7"] = k[i].fldTitle;
                            K_Row["Amount7"] = k[i].fldMablagh;
                            break;
                        case 8:
                            K_Row["Title8"] = k[i].fldTitle;
                            K_Row["Amount8"] = k[i].fldMablagh;
                            break;
                        case 9:
                            K_Row["Title9"] = k[i].fldTitle;
                            K_Row["Amount9"] = k[i].fldMablagh;
                            break;
                    }
                }
                if (k.Count > 0)
                    Kosourat.Rows.Add(K_Row);
                DataRow M_Row = Motalebat.NewRow();
                for (int i = 0; i < M.Count; i++)
                {

                    switch (i)
                    {
                        case 0:
                            M_Row["Title"] = M[i].fldTitle;
                            M_Row["Amount"] = M[i].fldMablagh;
                            break;
                        case 1:
                            M_Row["Title1"] = M[i].fldTitle;
                            M_Row["Amount1"] = M[i].fldMablagh;
                            break;
                        case 2:
                            M_Row["Title2"] = M[i].fldTitle;
                            M_Row["Amount2"] = M[i].fldMablagh;
                            break;
                        case 3:
                            M_Row["Title3"] = M[i].fldTitle;
                            M_Row["Amount3"] = M[i].fldMablagh;
                            break;
                        case 4:
                            M_Row["Title4"] = M[i].fldTitle;
                            M_Row["Amount4"] = M[i].fldMablagh;
                            break;
                        case 5:
                            M_Row["Title5"] = M[i].fldTitle;
                            M_Row["Amount5"] = M[i].fldMablagh;
                            break;
                        case 6:
                            M_Row["Title6"] = M[i].fldTitle;
                            M_Row["Amount6"] = M[i].fldMablagh;
                            break;
                        case 7:
                            M_Row["Title7"] = M[i].fldTitle;
                            M_Row["Amount7"] = M[i].fldMablagh;
                            break;
                        case 8:
                            M_Row["Title8"] = M[i].fldTitle;
                            M_Row["Amount8"] = M[i].fldMablagh;
                            break;
                        case 9:
                            M_Row["Title9"] = M[i].fldTitle;
                            M_Row["Amount9"] = M[i].fldMablagh;
                            break;
                    }

                }
                if (M.Count > 0)
                    Motalebat.Rows.Add(M_Row);

                var TypeReport = PayServic.GetCostCenterDetail(costCenterId, OrganId, IP, out ErrPay).fldReportTypeId;
                if (TypeReport == 1 || TypeReport == 2)//کارمند رسمی
                {
                    TypeEstekhdam = 2;
                    if (TypeReport == 1)//کارمند رسمی
                    {
                        if (TypeList == "0")
                        {
                            Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtrasmi1.frx";
                        }
                        else
                        {
                            Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtrasmi.frx";
                        }
                    }
                    else //کارمند رسمی(تأمین اجتماعی)
                    {
                        if (TypeList == "0")
                        {
                            Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtrasmi1_t.frx";
                        }
                        else
                        {
                            Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtrasmi_t.frx";
                        }
                    }

                }
                else if (TypeReport == 3)
                {
                    TypeEstekhdam = 3;
                    if (TypeList == "0")
                    {
                        Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtKarkonanPeymani1.frx";
                    }
                    else
                    {
                        Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtKarkonanPeymani.frx";
                    }
                }
                else if (TypeReport == 4)
                {
                    if (TypeList == "0")
                    {
                        Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtKargar1.frx";
                    }
                    else
                    {
                        Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtKargar.frx";
                    }
                }
                else if (TypeReport == 5 || TypeReport == 6)
                {
                    TypeEstekhdam = 4;
                    if (TypeReport == 5)//شهردار رسمی
                    {
                        if (TypeList == "0")
                        {
                            Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtShahrdar1.frx";
                        }
                        else
                        {
                            Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtShahrdar.frx";
                        }
                    }
                    else//شهردار تأمین اجتماعی
                    {
                        if (TypeList == "0")
                        {
                            Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtShahrdar1_t.frx";
                        }
                        else
                        {
                            Path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtShahrdar_t.frx";
                        }
                    }
                }
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 28, IP, out Err).ToList();
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

                //پر کردن DataSet
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                byte t = 0;
                if (moavaghe != 0)
                    t = 2;
                else if (type)
                    t = 1;
                ListPardakhtHoghogh.Fill(dt.spr_Pay_RptListPardakhtHoghogh, costCenterId, TypeBime, Year, Month, nobatPardakht, t, OrganId);
                NameItemEstekhdam.Fill(dt.spr_RptNameItemEstekhdam, TypeEstekhdam);
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var d = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (d != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/

                FastReport.Report Report = new FastReport.Report();
                Report.Load(Path);
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt_K_M, "rasaFMSPayRoll");
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
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("sal", Year.ToString());
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("markaz", costCenterName);
                Report.SetParameterValue("flag", flag);
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

        public ActionResult GeneratePDFListPardakhtMoavaghe(short Year, byte Month, int PersonId, string Name, int OrganId)
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
                DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtMoavagheTableAdapter ListPardakhtMoavaghe = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtMoavagheTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                ListPardakhtMoavaghe.Fill(dt.spr_Pay_RptListPardakhtMoavaghe, Year, Month, PersonId, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 29, IP, out Err).ToList();
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat_Variz";
                if (PersonId != 0)
                {
                    FieldName2 = "CheckMaliyat_Variz_PersonalId";
                }
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), "", OrganId, Convert.ToInt32(PersonId), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtMoavaghe.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
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
                Report.SetParameterValue("Sal", Year);
                Report.SetParameterValue("name", Name);
                Report.SetParameterValue("Mah", MonthName);
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

        public ActionResult GeneratePDFListPardakhtMamooriat(short Year, byte Month, int OrganId)
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
                DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtMamoriyatTableAdapter ListPardakhtMamoriyat = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtMamoriyatTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                ListPardakhtMamoriyat.Fill(dt.spr_Pay_RptListPardakhtMamoriyat, Year, Month, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 31, IP, out Err).ToList();
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
 var FieldName2 = "CheckMaliyat_Variz";

 var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), "", OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k!= null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtMamoriyat.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
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
                Report.SetParameterValue("Sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("flag", flag);
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

        public ActionResult GeneratePDFListPardakhtKomak(short Year, byte Month, byte TypeKomak, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                var onvan = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtGheyreNaghdiTableAdapter ListPardakhtGheyreNaghdi = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtGheyreNaghdiTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                byte CalcType = 7;
                if (TypeKomak == 1)
                {
                    ListPardakhtGheyreNaghdi.Fill(dt.spr_Pay_RptListPardakhtGheyreNaghdi, "Mostamar", Year, Month, OrganId);
                    onvan = "مستمر";
                }
                else
                {
                    ListPardakhtGheyreNaghdi.Fill(dt.spr_Pay_RptListPardakhtGheyreNaghdi, "GheyreMostamar", Year, Month, OrganId);
                    onvan = "غیر مستمر";
                    CalcType = 8;
                }

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 33, IP, out Err).ToList();
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat_Variz";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), "", OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtKomakGhirNaghdi.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
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
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("onvan", onvan);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("flag", flag);
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
        public ActionResult GeneratePDFListPardakhtBonKart(short Year, byte Month, byte Nobat,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var Module_OrganId = servic.GetModule_OrganWithFilter("CheckOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP,
                    out Err).Where(l => l.fldModuleId == 3).FirstOrDefault().fldId;
                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\ListPardakhtBonKart.frx";
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), Nobat.ToString(), OrganId, 0, IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                rep.Load(path);
                rep.SetParameterValue("Module_OrganId", Module_OrganId);
                rep.SetParameterValue("NobatPardakht", Nobat);
                rep.SetParameterValue("sal", Year);
                rep.SetParameterValue("Month", Month);
                rep.SetParameterValue("organId", OrganId);
                rep.SetParameterValue("userId", Convert.ToInt32(Session["UserId"]));
                rep.SetParameterValue("ReportId", 56);
                rep.SetParameterValue("flag", flag);
                rep.SetParameterValue("connectionstr", System.Configuration.ConfigurationManager.ConnectionStrings["RasaFMSCommonDBConnectionString"].ConnectionString);
                if (rep.Report.Prepare())
                {
                    // Set PDF export props
                    FastReport.Export.Pdf.PDFExport pdfExport = new FastReport.Export.Pdf.PDFExport();
                    pdfExport.ShowProgress = false;
                    pdfExport.Compressed = true;
                    pdfExport.AllowPrint = true;
                    pdfExport.EmbeddingFonts = true;
                    MemoryStream strm = new MemoryStream();
                    rep.Report.Export(pdfExport, strm);
                    rep.Dispose();
                    pdfExport.Dispose();
                    strm.Position = 0;
                    // return stream in browser
                    return File(strm.ToArray(), "application/pdf");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CheckListBimeOmr(int OrganId, short Year, byte Month)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeOmrTableAdapter ListBimeOmr = new DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeOmrTableAdapter();
            ListBimeOmr.Fill(dt.spr_Pay_RptListBimeOmr, Year, Month, OrganId);
            var AllowPrint = true;
            var q = dt.spr_Pay_RptListBimeOmr.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFListBimeOmr(int OrganId,short Year, byte Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeOmrTableAdapter ListBimeOmr = new DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeOmrTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                ListBimeOmr.Fill(dt.spr_Pay_RptListBimeOmr, Year, Month, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), 
                    Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 7, IP, out Err).ToList();
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

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Bimeomr.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah1", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
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
        public ActionResult CheckMaliyatSaliyane(int OrganId,short Year, int MashmoolSal)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeOmrTableAdapter ListBimeOmr = new DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeOmrTableAdapter();
            ListBimeOmr.Fill(dt.spr_Pay_RptListBimeOmr, Year, 12, OrganId);
            var AllowPrint = true;
            var q = dt.spr_Pay_RptListBimeOmr.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFMaliyatSaliyane(int OrganId,short Year, int MashmoolSal)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeOmrTableAdapter ListBimeOmr = new DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeOmrTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                ListBimeOmr.Fill(dt.spr_Pay_RptListBimeOmr, Year, 12, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(12);
                var Tarikh = Year + "/" + "12" + "/" + Helper.LastDayofMonth.GetLastDay(Year, 12);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 40, IP, out Err).ToList();
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

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Bimeomr.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah1", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
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
        public ActionResult CheckhoghoughMazaya(int OrganId,short Year, byte Month, byte nobatPardakht, byte marhalePardakht, byte TypeReport, string GrouhBandi)
        {

            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghoghTableAdapter KholaseHoghogh = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghoghTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEydiTableAdapter KholaseEydi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEydiTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMorakhasiTableAdapter KholaseMorakhasi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMorakhasiTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEzafekar_TatilKarTableAdapter Ezafekar_TatilKar = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEzafekar_TatilKarTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseSayerPardakhtTableAdapter SayerPardakht = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseSayerPardakhtTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseKomakTableAdapter KholaseKomak = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseKomakTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMamooriyatTableAdapter Mamooriyat = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMamooriyatTableAdapter();
            short TaSal = 0; byte TaMah = 0;
            var FieldName = "CostCenter";
            if (GrouhBandi == "2")
                FieldName = "ChartOrgan";
            var AllowPrint = true;
            if (TypeReport == 1)
            {
                KholaseHoghogh.Fill(dt.spr_Pay_RptKholaseHoghogh, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                var q = dt.spr_Pay_RptKholaseHoghogh.Count();
                if (q == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 2)
            {
                KholaseEydi.Fill(dt.spr_Pay_RptKholaseEydi, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                var q1 = dt.spr_Pay_RptKholaseEydi.Count();
                if (q1 == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 3)
            {
                KholaseMorakhasi.Fill(dt.spr_Pay_RptKholaseMorakhasi, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                var q2 = dt.spr_Pay_RptKholaseMorakhasi.Count();
                if (q2 == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 4)
            {
                FieldName = "EzafeKar_CostCenter";
                if (GrouhBandi == "2")
                    FieldName = "EzafeKar_ChartOrgan";

                Ezafekar_TatilKar.Fill(dt.spr_Pay_RptKholaseEzafekar_TatilKar, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                var q3 = dt.spr_Pay_RptKholaseEzafekar_TatilKar.Count();
                if (q3 == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 5)
            {
                FieldName = "TatilKar_CostCenter";
                if (GrouhBandi == "2")
                    FieldName = "TatilKar_ChartOrgan";

                Ezafekar_TatilKar.Fill(dt.spr_Pay_RptKholaseEzafekar_TatilKar, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                var q4 = dt.spr_Pay_RptKholaseEzafekar_TatilKar.Count();
                if (q4 == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 6)
            {
                SayerPardakht.Fill(dt.spr_Pay_RptKholaseSayerPardakht, FieldName, Year, Month, nobatPardakht, marhalePardakht, OrganId, TaSal, TaMah);
                var q5 = dt.spr_Pay_RptKholaseSayerPardakht.Count();
                if (q5 == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 7)
            {
                FieldName = "Mostamar_CostCenter";
                if (GrouhBandi == "2")
                    FieldName = "Mostamar_ChartOrgan";

                KholaseKomak.Fill(dt.spr_Pay_RptKholaseKomak, FieldName, Year, Month, OrganId, TaSal, TaMah);
                var q6 = dt.spr_Pay_RptKholaseKomak.Count();
                if (q6 == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 8)
            {
                FieldName = "GheyrMostamar_CostCenter";
                if (GrouhBandi == "2")
                    FieldName = "GheyrMostamar_ChartOrgan";

                KholaseKomak.Fill(dt.spr_Pay_RptKholaseKomak, FieldName, Year, Month, OrganId, TaSal, TaMah);
                var q7 = dt.spr_Pay_RptKholaseKomak.Count();
                if (q7 == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 9)
            {
                Mamooriyat.Fill(dt.spr_Pay_RptKholaseMamooriyat, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                var q8 = dt.spr_Pay_RptKholaseMamooriyat.Count();
                if (q8 == 0)
                    AllowPrint = false;
            }

            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GeneratePDFhoghoughMazaya(int OrganId,short Year, byte Month, byte nobatPardakht, byte marhalePardakht, byte TypeReport, string GrouhBandi, byte moavaghe)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {

                short TaSal = 0; byte TaMah = 0;
                var GrouhBandiName = "مرکز هزینه";
                if (GrouhBandi == "2")
                    GrouhBandiName = "محل خدمت";

                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghoghTableAdapter KholaseHoghogh = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghoghTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghogh_BedoneMoavagheTableAdapter KholaseHoghogh_Bedonemovaghe = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghogh_BedoneMoavagheTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEydiTableAdapter KholaseEydi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEydiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMorakhasiTableAdapter KholaseMorakhasi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMorakhasiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEzafekar_TatilKarTableAdapter Ezafekar_TatilKar = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEzafekar_TatilKarTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseSayerPardakhtTableAdapter SayerPardakht = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseSayerPardakhtTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseKomakTableAdapter KholaseKomak = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseKomakTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMamooriyatTableAdapter Mamooriyat = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMamooriyatTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter getdate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                getdate.Fill(dt_Com.spr_GetDate);
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),
                    IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 34, IP, out Err).ToList();
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

                FastReport.Report Report = new FastReport.Report();
                var FieldName = "CostCenter";
                if (GrouhBandi == "2")
                    FieldName = "ChartOrgan";
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";
               
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                if (TypeReport == 1)
                {
                    if (moavaghe != 1)
                        FieldName = FieldName + "_BedoneMoavaghe";
                    KholaseHoghogh.Fill(dt.spr_Pay_RptKholaseHoghogh, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    //else
                    //    KholaseHoghogh_Bedonemovaghe.Fill(dt.spr_Pay_RptKholaseHoghogh_BedoneMoavaghe, FieldName, Year, Month, nobatPardakht, Convert.ToInt32(Session["OrganId"]), TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholasehoghogh.frx");
                }
                else if (TypeReport == 2)
                {
                    KholaseEydi.Fill(dt.spr_Pay_RptKholaseEydi, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseEydi.frx");
                }
                else if (TypeReport == 3)
                {
                    KholaseMorakhasi.Fill(dt.spr_Pay_RptKholaseMorakhasi, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseMorakhasi.frx");
                }
                else if (TypeReport == 4)
                {
                    FieldName = "EzafeKar_CostCenter";
                    if (GrouhBandi == "2")
                        FieldName = "EzafeKar_ChartOrgan";

                    Ezafekar_TatilKar.Fill(dt.spr_Pay_RptKholaseEzafekar_TatilKar, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseEzafekari.frx");
                    Report.SetParameterValue("Title", "اضافه کاری");
                }
                else if (TypeReport == 5)
                {
                    FieldName = "TatilKar_CostCenter";
                    if (GrouhBandi == "2")
                        FieldName = "TatilKar_ChartOrgan";

                    Ezafekar_TatilKar.Fill(dt.spr_Pay_RptKholaseEzafekar_TatilKar, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseEzafekari.frx");
                    Report.SetParameterValue("Title", "تعطیل کاری");
                }
                else if (TypeReport == 6)
                {
                    SayerPardakht.Fill(dt.spr_Pay_RptKholaseSayerPardakht, FieldName, Year, Month, nobatPardakht, marhalePardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseSayerPardakht.frx");
                }
                else if (TypeReport == 7)
                {
                    FieldName = "Mostamar_CostCenter";
                    if (GrouhBandi == "2")
                        FieldName = "Mostamar_ChartOrgan";

                    KholaseKomak.Fill(dt.spr_Pay_RptKholaseKomak, FieldName, Year, Month, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseKomakGheyrNaghdi.frx");
                    Report.SetParameterValue("Title", "پرداخت کمک های غیرنقدی مستمر");
                }
                else if (TypeReport == 8)
                {
                    FieldName = "GheyrMostamar_CostCenter";
                    if (GrouhBandi == "2")
                        FieldName = "GheyrMostamar_ChartOrgan";

                    KholaseKomak.Fill(dt.spr_Pay_RptKholaseKomak, FieldName, Year, Month, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseKomakGheyrNaghdi.frx");
                    Report.SetParameterValue("Title", "پرداخت کمک های غیرنقدی غیرمستمر");
                }
                else if (TypeReport == 9)
                {
                    Mamooriyat.Fill(dt.spr_Pay_RptKholaseMamooriyat, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseMamooriyat.frx");
                }

                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("GrouhBandi", GrouhBandiName);
                Report.SetParameterValue("flag", flag);
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
        public ActionResult GenerateExcelhoghoughMazaya(int OrganId,short Year, byte Month, byte nobatPardakht, byte marhalePardakht, byte TypeReport, string GrouhBandi)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                short TaSal = 0; byte TaMah = 0;
                var GrouhBandiName = "مرکز هزینه";
                if (GrouhBandi == "2")
                    GrouhBandiName = "محل خدمت";

                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghoghTableAdapter KholaseHoghogh = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghoghTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEydiTableAdapter KholaseEydi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEydiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMorakhasiTableAdapter KholaseMorakhasi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMorakhasiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEzafekar_TatilKarTableAdapter Ezafekar_TatilKar = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEzafekar_TatilKarTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseSayerPardakhtTableAdapter SayerPardakht = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseSayerPardakhtTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseKomakTableAdapter KholaseKomak = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseKomakTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMamooriyatTableAdapter Mamooriyat = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMamooriyatTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),
                    IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 34, IP, out Err).ToList();
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/

                FastReport.Report Report = new FastReport.Report();
                var FieldName = "CostCenter";
                if (GrouhBandi == "2")
                    FieldName = "ChartOrgan";

                if (TypeReport == 1)
                {
                    KholaseHoghogh.Fill(dt.spr_Pay_RptKholaseHoghogh, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholasehoghogh.frx");
                }
                else if (TypeReport == 2)
                {
                    KholaseEydi.Fill(dt.spr_Pay_RptKholaseEydi, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseEydi.frx");
                }
                else if (TypeReport == 3)
                {
                    KholaseMorakhasi.Fill(dt.spr_Pay_RptKholaseMorakhasi, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseMorakhasi.frx");
                }
                else if (TypeReport == 4)
                {
                    FieldName = "EzafeKar_CostCenter";
                    if (GrouhBandi == "2")
                        FieldName = "EzafeKar_ChartOrgan";

                    Ezafekar_TatilKar.Fill(dt.spr_Pay_RptKholaseEzafekar_TatilKar, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseEzafekari.frx");
                    Report.SetParameterValue("Title", "اضافه کاری");
                }
                else if (TypeReport == 5)
                {
                    FieldName = "TatilKar_CostCenter";
                    if (GrouhBandi == "2")
                        FieldName = "TatilKar_ChartOrgan";

                    Ezafekar_TatilKar.Fill(dt.spr_Pay_RptKholaseEzafekar_TatilKar, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseEzafekari.frx");
                    Report.SetParameterValue("Title", "تعطیل کاری");
                }
                else if (TypeReport == 6)
                {
                    SayerPardakht.Fill(dt.spr_Pay_RptKholaseSayerPardakht, FieldName, Year, Month, nobatPardakht, marhalePardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseSayerPardakht.frx");
                }
                else if (TypeReport == 7)
                {
                    FieldName = "Mostamar_CostCenter";
                    if (GrouhBandi == "2")
                        FieldName = "Mostamar_ChartOrgan";

                    KholaseKomak.Fill(dt.spr_Pay_RptKholaseKomak, FieldName, Year, Month, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseKomakGheyrNaghdi.frx");
                    Report.SetParameterValue("Title", "پرداخت کمک های غیرنقدی مستمر");
                }
                else if (TypeReport == 8)
                {
                    FieldName = "GheyrMostamar_CostCenter";
                    if (GrouhBandi == "2")
                        FieldName = "GheyrMostamar_ChartOrgan";

                    KholaseKomak.Fill(dt.spr_Pay_RptKholaseKomak, FieldName, Year, Month, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseKomakGheyrNaghdi.frx");
                    Report.SetParameterValue("Title", "پرداخت کمک های غیرنقدی غیرمستمر");
                }
                else if (TypeReport == 9)
                {
                    Mamooriyat.Fill(dt.spr_Pay_RptKholaseMamooriyat, FieldName, Year, Month, nobatPardakht, OrganId, TaSal, TaMah);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseMamooriyat.frx");
                }

                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("GrouhBandi", GrouhBandiName);
                Report.SetParameterValue("flag", flag);
                FastReport.Export.Xml.XMLExport excel = new FastReport.Export.Xml.XMLExport();
                MemoryStream stream = new MemoryStream();
                Report.Prepare();
                Report.Export(excel, stream);
                return File(stream.ToArray(), "application/vnd.ms-excel", "TarazKoli.xls");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckhoghoughMazayaBazei(int OrganId,short Year, byte Month, byte nobatPardakht, byte marhalePardakht, byte TypeReport, string GrouhBandi, short TaYear, byte TaMonth)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var GrouhBandiName = "مرکز هزینه";
            if (GrouhBandi == "2")
                GrouhBandiName = "محل خدمت";
            var AllowPrint = true;
            DataSet.DataSet1 dt = new DataSet.DataSet1();

            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghoghTableAdapter KholaseHoghogh = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghoghTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEydiTableAdapter KholaseEydi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEydiTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMorakhasiTableAdapter KholaseMorakhasi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMorakhasiTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEzafekar_TatilKarTableAdapter Ezafekar_TatilKar = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEzafekar_TatilKarTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseSayerPardakhtTableAdapter SayerPardakht = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseSayerPardakhtTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseKomakTableAdapter KholaseKomak = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseKomakTableAdapter();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMamooriyatTableAdapter Mamooriyat = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMamooriyatTableAdapter();

            var FieldName = "CostCenter_Sal";
            if (GrouhBandi == "2")
                FieldName = "ChartOrgan_Sal";

            if (TypeReport == 1)
            {
                KholaseHoghogh.Fill(dt.spr_Pay_RptKholaseHoghogh, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                var q = dt.spr_Pay_RptKholaseHoghogh.Count();
                if (q == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 2)
            {
                KholaseEydi.Fill(dt.spr_Pay_RptKholaseEydi, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                var q1 = dt.spr_Pay_RptKholaseEydi.Count();
                if (q1 == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 3)
            {
                KholaseMorakhasi.Fill(dt.spr_Pay_RptKholaseMorakhasi, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                var qq = dt.spr_Pay_RptKholaseMorakhasi.Count();
                if (qq == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 4)
            {
                FieldName = "EzafeKar_CostCenter_Sal";
                if (GrouhBandi == "2")
                    FieldName = "EzafeKar_ChartOrgan_Sal";

                Ezafekar_TatilKar.Fill(dt.spr_Pay_RptKholaseEzafekar_TatilKar, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                var q2 = dt.spr_Pay_RptKholaseEzafekar_TatilKar.Count();
                if (q2 == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 5)
            {
                FieldName = "TatilKar_CostCenter_Sal";
                if (GrouhBandi == "2")
                    FieldName = "TatilKar_ChartOrgan_Sal";

                Ezafekar_TatilKar.Fill(dt.spr_Pay_RptKholaseEzafekar_TatilKar, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                var qqq = dt.spr_Pay_RptKholaseEzafekar_TatilKar.Count();
                if (qqq == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 6)
            {
                SayerPardakht.Fill(dt.spr_Pay_RptKholaseSayerPardakht, FieldName, Year, Month, nobatPardakht, marhalePardakht, OrganId, TaYear, TaMonth);
                var q3 = dt.spr_Pay_RptKholaseSayerPardakht.Count();
                if (q3 == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 7)
            {
                FieldName = "Mostamar_CostCenter_Sal";
                if (GrouhBandi == "2")
                    FieldName = "Mostamar_ChartOrgan_Sal";

                KholaseKomak.Fill(dt.spr_Pay_RptKholaseKomak, FieldName, Year, Month, OrganId, TaYear, TaMonth);
                var q11 = dt.spr_Pay_RptKholaseKomak.Count();
                if (q11 == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 8)
            {
                FieldName = "GheyrMostamar_CostCenter_Sal";
                if (GrouhBandi == "2")
                    FieldName = "GheyrMostamar_ChartOrgan_Sal";

                KholaseKomak.Fill(dt.spr_Pay_RptKholaseKomak, FieldName, Year, Month, OrganId, TaYear, TaMonth);
                var mm = dt.spr_Pay_RptKholaseKomak.Count();
                if (mm == 0)
                    AllowPrint = false;
            }
            else if (TypeReport == 9)
            {
                Mamooriyat.Fill(dt.spr_Pay_RptKholaseMamooriyat, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                var ff = dt.spr_Pay_RptKholaseMamooriyat.Count();
                if (ff == 0)
                    AllowPrint = false;
            }
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFKholaseBonKart(int OrganId, short Year, byte Month, byte nobatPardakht, byte GrouhBandi,
            short? TaYear, byte? TaMonth,byte State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var Module_OrganId = servic.GetModule_OrganWithFilter("CheckOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP,
                    out Err).Where(l => l.fldModuleId == 3).FirstOrDefault().fldId;
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseBonKart.frx";
                rep.Load(path);

                var FieldName = "CostCenter";
                if (GrouhBandi == 2 && State==1)
                {
                    FieldName = "ChartOrgan";
                }
                else if (GrouhBandi == 1 && State == 2)
                {
                    FieldName = "CostCenter_Sal";
                }
                else if (GrouhBandi == 2 && State == 2)
                {
                    FieldName = "ChartOrgan_Sal";
                }
                rep.SetParameterValue("NobatPardakht", nobatPardakht);
                rep.SetParameterValue("sal", Year);
                rep.SetParameterValue("Tasal", TaYear!=null?TaYear:0);
                rep.SetParameterValue("mah", Month);
                rep.SetParameterValue("Tamah", TaMonth != null ? TaMonth : 0);
                rep.SetParameterValue("organId", OrganId);
                rep.SetParameterValue("ReportId", 34);
                rep.SetParameterValue("Module_OrganId", Module_OrganId);
                rep.SetParameterValue("userId", Convert.ToInt32(Session["UserId"]));
                rep.SetParameterValue("fieldName", FieldName);
                rep.SetParameterValue("group", GrouhBandi);
                rep.SetParameterValue("flag", flag);
                rep.SetParameterValue("connectionstr", System.Configuration.ConfigurationManager.ConnectionStrings["RasaFMSCommonDBConnectionString"].ConnectionString);

                if (rep.Report.Prepare())
                {
                    // Set PDF export props
                    FastReport.Export.Pdf.PDFExport pdfExport = new FastReport.Export.Pdf.PDFExport();
                    pdfExport.ShowProgress = false;
                    pdfExport.Compressed = true;
                    pdfExport.AllowPrint = true;
                    pdfExport.EmbeddingFonts = true;
                    MemoryStream strm = new MemoryStream();
                    rep.Report.Export(pdfExport, strm);
                    rep.Dispose();
                    pdfExport.Dispose();
                    strm.Position = 0;
                    // return stream in browser
                    return File(strm.ToArray(), "application/pdf");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GenerateExcelKholaseBonKart(int OrganId, short Year, byte Month, byte nobatPardakht, byte GrouhBandi,short? TaYear, byte? TaMonth)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                var Module_OrganId = servic.GetModule_OrganWithFilter("CheckOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP,
                    out Err).Where(l => l.fldModuleId == 3).FirstOrDefault().fldId;
                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseBonKart.frx";
                rep.Load(path);

                var FieldName = "CostCenter";
                if (GrouhBandi == 2 )
                {
                    FieldName = "ChartOrgan";
                }
                rep.SetParameterValue("NobatPardakht", nobatPardakht);
                rep.SetParameterValue("sal", Year);
                rep.SetParameterValue("Tasal", TaYear!=null?TaYear:0);
                rep.SetParameterValue("mah", Month);
                rep.SetParameterValue("Tamah", TaMonth != null ? TaMonth : 0);
                rep.SetParameterValue("organId", OrganId);
                rep.SetParameterValue("ReportId", 34);
                rep.SetParameterValue("Module_OrganId", Module_OrganId);
                rep.SetParameterValue("userId", Convert.ToInt32(Session["UserId"]));
                rep.SetParameterValue("fieldName", FieldName);
                rep.SetParameterValue("group", GrouhBandi);
                rep.SetParameterValue("connectionstr", System.Configuration.ConfigurationManager.ConnectionStrings["RasaFMSCommonDBConnectionString"].ConnectionString);

                if (rep.Report.Prepare())
                {
                    // Set PDF export props
                    FastReport.Export.Xml.XMLExport pdfExport = new FastReport.Export.Xml.XMLExport();
                    pdfExport.ShowProgress = false;
                    MemoryStream strm = new MemoryStream();
                    rep.Report.Export(pdfExport, strm);
                    rep.Dispose();
                    pdfExport.Dispose();
                    strm.Position = 0;
                    // return stream in browser
                    return File(strm.ToArray(), "application/vnd.ms-excel", "KholaseBonKart.xls");
                    //return File(strm.ToArray(), "application/pdf");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GeneratePDFhoghoughMazayaBazei(int OrganId,short Year, byte Month, byte nobatPardakht, byte marhalePardakht, byte TypeReport, string GrouhBandi, short TaYear, byte TaMonth)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {

                var GrouhBandiName = "مرکز هزینه";
                if (GrouhBandi == "2")
                    GrouhBandiName = "محل خدمت";

                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghoghTableAdapter KholaseHoghogh = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseHoghoghTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEydiTableAdapter KholaseEydi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEydiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMorakhasiTableAdapter KholaseMorakhasi = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMorakhasiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEzafekar_TatilKarTableAdapter Ezafekar_TatilKar = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseEzafekar_TatilKarTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseSayerPardakhtTableAdapter SayerPardakht = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseSayerPardakhtTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseKomakTableAdapter KholaseKomak = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseKomakTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMamooriyatTableAdapter Mamooriyat = new DataSet.DataSet1TableAdapters.spr_Pay_RptKholaseMamooriyatTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),
                    IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var TaMonthName = MyLib.Shamsi.ShamsiMonthname(TaMonth);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 34, IP, out Err).ToList();
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                var FieldName = "CostCenter_Sal";
                if (GrouhBandi == "2")
                    FieldName = "ChartOrgan_Sal";

                if (TypeReport == 1)
                {
                    KholaseHoghogh.Fill(dt.spr_Pay_RptKholaseHoghogh, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholasehoghogh_Baze.frx");
                }
                else if (TypeReport == 2)
                {
                    KholaseEydi.Fill(dt.spr_Pay_RptKholaseEydi, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseEydi_Baze.frx");
                }
                else if (TypeReport == 3)
                {
                    KholaseMorakhasi.Fill(dt.spr_Pay_RptKholaseMorakhasi, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseMorakhasi_Baze.frx");
                }
                else if (TypeReport == 4)
                {
                    FieldName = "EzafeKar_CostCenter_Sal";
                    if (GrouhBandi == "2")
                        FieldName = "EzafeKar_ChartOrgan_Sal";

                    Ezafekar_TatilKar.Fill(dt.spr_Pay_RptKholaseEzafekar_TatilKar, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseEzafekari_Baze.frx");
                    Report.SetParameterValue("Title", "اضافه کاری");
                }
                else if (TypeReport == 5)
                {
                    FieldName = "TatilKar_CostCenter_Sal";
                    if (GrouhBandi == "2")
                        FieldName = "TatilKar_ChartOrgan_Sal";

                    Ezafekar_TatilKar.Fill(dt.spr_Pay_RptKholaseEzafekar_TatilKar, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseEzafekari_Baze.frx");
                    Report.SetParameterValue("Title", "تعطیل کاری");
                }
                else if (TypeReport == 6)
                {
                    SayerPardakht.Fill(dt.spr_Pay_RptKholaseSayerPardakht, FieldName, Year, Month, nobatPardakht, marhalePardakht, OrganId, TaYear, TaMonth);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseSayerPardakht_Baze.frx");
                }
                else if (TypeReport == 7)
                {
                    FieldName = "Mostamar_CostCenter_Sal";
                    if (GrouhBandi == "2")
                        FieldName = "Mostamar_ChartOrgan_Sal";

                    KholaseKomak.Fill(dt.spr_Pay_RptKholaseKomak, FieldName, Year, Month, OrganId, TaYear, TaMonth);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseKomakGheyrNaghdi_Baze.frx");
                    Report.SetParameterValue("Title", "پرداخت کمک های غیرنقدی مستمر");
                }
                else if (TypeReport == 8)
                {
                    FieldName = "GheyrMostamar_CostCenter_Sal";
                    if (GrouhBandi == "2")
                        FieldName = "GheyrMostamar_ChartOrgan_Sal";

                    KholaseKomak.Fill(dt.spr_Pay_RptKholaseKomak, FieldName, Year, Month, OrganId, TaYear, TaMonth);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseKomakGheyrNaghdi_Baze.frx");
                    Report.SetParameterValue("Title", "پرداخت کمک های غیرنقدی غیرمستمر");
                }
                else if (TypeReport == 9)
                {
                    Mamooriyat.Fill(dt.spr_Pay_RptKholaseMamooriyat, FieldName, Year, Month, nobatPardakht, OrganId, TaYear, TaMonth);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKholaseMamooriyat_Baze.frx");
                }

                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("Tasal", TaYear);
                Report.SetParameterValue("Tamah", TaMonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("GrouhBandi", GrouhBandiName);
                Report.SetParameterValue("flag", flag);
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
        public ActionResult GeneratePDFListPardakhtEzafekar_Tatilkar(short Year, byte Month, byte nobatPardakht, string nobatPardakhtName, int PersonId, int costCenterId, string costCenterName, string FieldName,int OrganId)
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
                DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtEzafeKariTableAdapter ListPardakhtEzafeKari_TatilKari = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPardakhtEzafeKariTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                ListPardakhtEzafeKari_TatilKari.Fill(dt.spr_Pay_RptListPardakhtEzafeKari, FieldName, costCenterId, Year, Month, PersonId, nobatPardakht, OrganId);
                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                List<WCF_Common.OBJ_Signers> signers = null;
                if (FieldName == "EzafeKar")
                {
                    signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 22, IP, out Err).ToList();
                }
                else
                {
                    signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 27, IP, out Err).ToList();
                }
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptListPardakhtEzafekar_TatilKar.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("markaz", costCenterName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("Sal", Year);
                Report.SetParameterValue("flag", flag);
                if (FieldName == "EzafeKar")
                {
                    Report.SetParameterValue("FieldName", "اضافه کاری");
                }
                else
                {
                    Report.SetParameterValue("FieldName", "تعطیل کاری");
                }
                Report.SetParameterValue("Nobat", "نوبت پرداخت " + nobatPardakhtName);
                Report.SetParameterValue("Mah", MonthName);
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

        public ActionResult GenerateExcelListAghsateVam(int OrganId,short Year, byte Month)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            try
            {
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA" };
                int index = 0;

                var q = PayServic.RptListAghsatVam_Excel(Year, Month, OrganId, IP, out ErrPay).ToList();
                string Checked = "Name";
                var ListExcel = Checked.Split(';');

                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                var Check = "";
                var Name = "";
                foreach (var item in ListExcel)
                {

                    switch (item)
                    {
                        case "Name":
                            Check = "نام";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int ii = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldName;
                                Cell Cell = sheet.Cells[alpha[index] + (ii + 2)];
                                Cell.PutValue(Name);
                                ii++;
                            }
                            index++;
                            break;


                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "csv", "گزارش اقساط وام.csv");

            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GenerateExcelKosourat_Motalebat_P(int OrganId,short Year, byte Month, int ParamId, string ParamName)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            try
            {
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA" };
                int index = 0;

                var q = PayServic.RptKosourat_MotalebatParam_Excel(Year, Month, ParamId, OrganId, IP, out ErrPay).ToList();
                string Checked = "Name";
                var ListExcel = Checked.Split(';');

                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                var Check = "";
                var Name = "";
                foreach (var item in ListExcel)
                {

                    switch (item)
                    {
                        case "Name":
                            Check = "نام";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int ii = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldName;
                                Cell Cell = sheet.Cells[alpha[index] + (ii + 2)];
                                Cell.PutValue(Name);
                                ii++;
                            }
                            index++;
                            break;


                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "csv", ParamName + "گزارش کسورات.csv");

            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckKarkarMahane(int OrganId,string Year, string Month, byte NobatPardakht)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetKarKardeMahaneWithFilter("Mohasebe", "", Year, Month, NobatPardakht, 0, OrganId, 1, IP, out ErrPay).FirstOrDefault();

            var AllowPrint = true;

            if (q == null)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerateExcelKarkarMahane(int OrganId,string Year, string Month, byte NobatPardakht)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            try
            {
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA" };
                int index = 0;

                var q = PayServic.GetKarKardeMahaneWithFilter("Mohasebe", "", Year, Month, NobatPardakht, 0, OrganId, 0, IP, out ErrPay).ToList();
                string Checked = "Name;Codemeli;Karkard;Gheybat;EzafeKari;TatileKari;";
                var ListExcel = Checked.Split(';');

                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                var Check = "";
                var Name = "";
                foreach (var item in ListExcel)
                {

                    switch (item)
                    {
                        case "Name":
                            Check = "نام_نام خانوادگی (نام پدر)";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int ii = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldName_Father;
                                Cell Cell = sheet.Cells[alpha[index] + (ii + 2)];
                                Cell.PutValue(Name);
                                ii++;
                            }
                            index++;
                            break;
                        case "Codemeli":
                            Check = "کدملی";
                            Cell cell1 = sheet.Cells[alpha[index] + "1"];
                            cell1.PutValue(Check);
                            int ii1 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldCodemeli;
                                Cell Cell = sheet.Cells[alpha[index] + (ii1 + 2)];
                                Cell.PutValue(Name);
                                ii1++;
                            }
                            index++;
                            break;
                        case "Karkard":
                            Check = "کارکرد";
                            Cell cell2 = sheet.Cells[alpha[index] + "1"];
                            cell2.PutValue(Check);
                            int ii2 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldKarkard.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii2 + 2)];
                                Cell.PutValue(Name);
                                ii2++;
                            }
                            index++;
                            break;
                        case "Gheybat":
                            Check = "غیبت";
                            Cell cell3 = sheet.Cells[alpha[index] + "1"];
                            cell3.PutValue(Check);
                            int ii3 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldGheybat.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii3 + 2)];
                                Cell.PutValue(Name);
                                ii3++;
                            }
                            index++;
                            break;
                        case "EzafeKari":
                            Check = "اضافه کاری";
                            Cell cell4 = sheet.Cells[alpha[index] + "1"];
                            cell4.PutValue(Check);
                            int ii4 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldEzafeKari.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii4 + 2)];
                                Cell.PutValue(Name);
                                ii4++;
                            }
                            index++;
                            break;
                        case "TatileKari":
                            Check = "تعطیل کاری";
                            Cell cell5 = sheet.Cells[alpha[index] + "1"];
                            cell5.PutValue(Check);
                            int ii5 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldTatileKari.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii5 + 2)];
                                Cell.PutValue(Name);
                                ii5++;
                            }
                            index++;
                            break;

                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "xls", "گزارش کارکرد ماهانه " + Year + Month + ".xls");

            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CheckListAghsateVam(int OrganId,short Year, byte Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptListAghsatVamTableAdapter ListAghsatVam = new DataSet.DataSet1TableAdapters.spr_Pay_RptListAghsatVamTableAdapter();
            ListAghsatVam.Fill(dt.spr_Pay_RptListAghsatVam, Year, Month, OrganId);
            var AllowPrint = true;
            var q = dt.spr_Pay_RptListAghsatVam.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            });
        }
        public ActionResult GeneratePDFListAghsateVam(int OrganId, short Year, byte Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptListAghsatVamTableAdapter ListAghsatVam = new DataSet.DataSet1TableAdapters.spr_Pay_RptListAghsatVamTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                dt.EnforceConstraints = false;
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Date.Fill(dt_Com.spr_GetDate);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                ListAghsatVam.Fill(dt.spr_Pay_RptListAghsatVam, Year, Month, OrganId);
                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(),
                    Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 8, IP, out Err).ToList();
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
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\aghsatVam.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("Sal", Year);
                Report.SetParameterValue("Mah1", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
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
        public ActionResult CheckListPasAndaz(int OrganId,short Year, byte Month, byte nobatPardakht)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptListPasAndazKarmandanTableAdapter ListPasAndaz = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPasAndazKarmandanTableAdapter();
            ListPasAndaz.Fill(dt.spr_Pay_RptListPasAndazKarmandan, Year, Month, nobatPardakht, OrganId);
            var AllowPrint = true;
            var q = dt.spr_Pay_RptListPasAndazKarmandan.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFListPasAndaz(int OrganId,short Year, byte Month, byte nobatPardakht)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptListPasAndazKarmandanTableAdapter ListPasAndaz = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPasAndazKarmandanTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                dt.EnforceConstraints = false;
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                ListPasAndaz.Fill(dt.spr_Pay_RptListPasAndazKarmandan, Year, Month, nobatPardakht, OrganId);
                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),
                    IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 2, IP, out Err).ToList();
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

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\PasAndaz.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("NameUser", User.fldNameEmployee);
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

        public ActionResult GeneratePDFListMandePasAndaz(int OrganId,short Year, byte Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptListMandePasAndazTableAdapter ListMandePas = new DataSet.DataSet1TableAdapters.spr_Pay_RptListMandePasAndazTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                ListMandePas.Fill(dt.spr_Pay_RptListMandePasAndaz, Year, Month, OrganId);
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(),
                    Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 17, IP, out Err).ToList();
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

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\MandePasAndaz.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("Year", Year);
                Report.SetParameterValue("Mah", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("NameUser", User.fldNameEmployee);
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
        public ActionResult CheckListMaliyat(int OrganId,short Year, byte Month, int CostCenterId, byte nobatPardakht, string CostCenterName)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_RptFiscalTableAdapter Fiscal = new DataSet.DataSet1TableAdapters.spr_RptFiscalTableAdapter();
            Fiscal.Fill(dt.spr_RptFiscal, Year, Month, CostCenterId, nobatPardakht, OrganId);
            var AllowPrint = true;
            var q = dt.spr_RptFiscal.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFListMaliyat(int OrganId,short Year, byte Month, int CostCenterId, byte nobatPardakht, string CostCenterName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_RptFiscalTableAdapter Fiscal = new DataSet.DataSet1TableAdapters.spr_RptFiscalTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptNameItemEstekhdamTableAdapter NameItemEstekhdam = new DataSet.DataSet1TableAdapters.spr_RptNameItemEstekhdamTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Fiscal.Fill(dt.spr_RptFiscal, Year, Month, CostCenterId, nobatPardakht, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),
                    IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);

                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 19, IP, out Err).ToList();
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

                FastReport.Report Report = new FastReport.Report();
                var Cost = PayServic.GetCostCenterWithFilter("fldId", CostCenterId.ToString(), OrganId.ToString(), 1, IP, out ErrPay).FirstOrDefault();
                if (Cost.fldReportTypeId == 4)
                {
                    NameItemEstekhdam.Fill(dt.spr_RptNameItemEstekhdam, 1);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Rptkargari.frx");
                }
                else
                {
                    NameItemEstekhdam.Fill(dt.spr_RptNameItemEstekhdam, 2);
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKarmandi.frx");
                }
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah1", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("NameUser", User.fldNameEmployee);
                Report.SetParameterValue("markaz1", CostCenterName);
                Report.SetParameterValue("markaz", CostCenterId);
                Report.SetParameterValue("flag", flag);
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
        public ActionResult CheckKosouratBank(int OrganId,short Year, byte Month, int CostCenterId, byte nobatPardakht, string CostCenterName, string nobatPardakhtName)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKosuratBankTableAdapter KosuratBank = new DataSet.DataSet1TableAdapters.spr_Pay_RptKosuratBankTableAdapter();
            KosuratBank.Fill(dt.spr_Pay_RptKosuratBank, nobatPardakht, Year, Month, CostCenterId, OrganId);
            var AllowPrint = true;
            var q = dt.spr_Pay_RptKosuratBank.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDKosouratBank(int OrganId,short Year, byte Month, int CostCenterId, byte nobatPardakht, string CostCenterName, string nobatPardakhtName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKosuratBankTableAdapter KosuratBank = new DataSet.DataSet1TableAdapters.spr_Pay_RptKosuratBankTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();

                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                KosuratBank.Fill(dt.spr_Pay_RptKosuratBank, nobatPardakht, Year, Month, CostCenterId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);

                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 39, IP, out Err).ToList();
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
              
 int flag=1;/*گزارش قبل از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k!= null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKosuratBank.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("flag", flag);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("markaz", CostCenterName);

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
        public ActionResult CheckListBimeTakmili(int OrganId,short Year, byte Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeTakmiliTableAdapter ListBimeTakmili = new DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeTakmiliTableAdapter();
            ListBimeTakmili.Fill(dt.spr_Pay_RptListBimeTakmili, Year, Month, OrganId);
            var AllowPrint = true;
            var q = dt.spr_Pay_RptListBimeTakmili.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            });
        }
        public ActionResult GeneratePDFListBimeTakmili(int OrganId,short Year, byte Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

                DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeTakmiliTableAdapter ListBimeTakmili = new DataSet.DataSet1TableAdapters.spr_Pay_RptListBimeTakmiliTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 6, IP, out Err).ToList();
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

                dt.EnforceConstraints = false;
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                ListBimeTakmili.Fill(dt.spr_Pay_RptListBimeTakmili, Year, Month, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Takmili.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("mah1", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("NameUser", User.fldNameEmployee);
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
        public ActionResult CheckKosuratParam_Bank(int OrganId,short Year, byte Month, byte nobatPardakht, int CostCenterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKosuratParam_BankTableAdapter KosuratParam_Bank = new DataSet.DataSet1TableAdapters.spr_Pay_RptKosuratParam_BankTableAdapter();
            KosuratParam_Bank.Fill(dt.spr_Pay_RptKosuratParam_Bank, Year, Month, nobatPardakht, OrganId, CostCenterId);
            var AllowPrint = true;
            var q = dt.spr_Pay_RptKosuratParam_Bank.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFKosuratParam_Bank(int OrganId,short Year, byte Month, byte nobatPardakht, int CostCenterId, string CostCenterName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKosuratParam_BankTableAdapter KosuratParam_Bank = new DataSet.DataSet1TableAdapters.spr_Pay_RptKosuratParam_BankTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                KosuratParam_Bank.Fill(dt.spr_Pay_RptKosuratParam_Bank, Year, Month, nobatPardakht, OrganId, CostCenterId);
                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),
                    IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 18, IP, out Err).ToList();
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

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\KosuratParam_Bank.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("Year", Year);
                Report.SetParameterValue("Month", MonthName);
                Report.SetParameterValue("CostCenterName", CostCenterName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Title", "کسورات (کلی) به همراه کسورات بانک");
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
        public ActionResult CheckMotalebatekoli(int OrganId,short Year, byte Month, byte nobatPardakht)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptMotalebatKoliTableAdapter MotalebatKoli = new DataSet.DataSet1TableAdapters.spr_Pay_RptMotalebatKoliTableAdapter();
            MotalebatKoli.Fill(dt.spr_Pay_RptMotalebatKoli, nobatPardakht, Year, Month, OrganId);
            var AllowPrint = true;
            var q = dt.spr_Pay_RptMotalebatKoli.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFMotalebatekoli(int OrganId,short Year, byte Month, byte nobatPardakht)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptMotalebatKoliTableAdapter MotalebatKoli = new DataSet.DataSet1TableAdapters.spr_Pay_RptMotalebatKoliTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                MotalebatKoli.Fill(dt.spr_Pay_RptMotalebatKoli, nobatPardakht, Year, Month, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),
                    IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 23, IP, out Err).ToList();
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

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMotalebatKoli.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("Sal", Year);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
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
        public ActionResult CheckSanadHesabdari1(int OrganId,short Year, byte Month, byte nobatPardakht, bool Takhfif)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_pay_rptSanad1TableAdapter Sanad1 = new DataSet.DataSet1TableAdapters.spr_pay_rptSanad1TableAdapter();
            Sanad1.Fill(dt.spr_pay_rptSanad1, Year, Month, nobatPardakht, Takhfif, OrganId);
            var AllowPrint = true;
            var q = dt.spr_pay_rptSanad1.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFSanadHesabdari1(int OrganId,short Year, byte Month, byte nobatPardakht, bool Takhfif)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_pay_rptSanad1TableAdapter Sanad1 = new DataSet.DataSet1TableAdapters.spr_pay_rptSanad1TableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                Sanad1.Fill(dt.spr_pay_rptSanad1, Year, Month, nobatPardakht, Takhfif, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),
                    IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 24, IP, out Err).ToList();
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\SanadHesabDari1.frx");
                //Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.SetParameterValue("Sal", Year);
                Report.SetParameterValue("Mah", MonthName);
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("flag", flag);
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
        public ActionResult CheckSanadHesabdari2(int OrganId,short Year, byte Month, byte nobatPardakht)
        {
            var q = PayServic.GetRptSanad2(Year, Month, nobatPardakht, OrganId, IP, out ErrPay).ToList();
            var AllowPrint = true;
            if (q.Count() == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckSanadHesabdari3(short Year, byte Month, byte nobatPardakht)
        {
            var q = PayServic.GetRptSanad3(Year, Month, nobatPardakht, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
            var AllowPrint = true;
            if (q.Count() == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFSanadHesabdari2(int OrganId,short Year, byte Month, byte nobatPardakht)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                System.Data.DataSet dt_K_M = new System.Data.DataSet();
                dt_K_M.DataSetName = "DataSetKM";
                System.Data.DataTable sanad_2_1dt = new System.Data.DataTable();
                sanad_2_1dt.TableName = "sanad_2_1";
                System.Data.DataTable sanad_2_2dt = new System.Data.DataTable();
                sanad_2_2dt.TableName = "sanad_2_2";

                //ساختن ستونهای DataTable
                sanad_2_1dt.Columns.Add("onvan", typeof(string));
                sanad_2_1dt.Columns.Add("edari", typeof(long));
                sanad_2_1dt.Columns.Add("shahri", typeof(long));
                sanad_2_1dt.Columns.Add("omrani", typeof(long));
                sanad_2_1dt.Columns.Add("behsasi", typeof(long));

                sanad_2_2dt.Columns.Add("onvan", typeof(string));
                sanad_2_2dt.Columns.Add("edari", typeof(long));
                sanad_2_2dt.Columns.Add("shahri", typeof(long));
                sanad_2_2dt.Columns.Add("omrani", typeof(long));
                sanad_2_2dt.Columns.Add("behsasi", typeof(long));

                dt_K_M.Tables.Add(sanad_2_1dt);
                dt_K_M.Tables.Add(sanad_2_2dt);

                //پر کردن DataTable
                string[] onvan_hoghugh = { "حقوق شهردار:", "حقوق کارمندان رسمی:", "حقوق کارمندان ثابت:",
                                     "دستمزد کارگران رسمی:", "دستمزد کارگران روزمزد:",
                                     "حقوق کارکنان قراردادی:", "دستمزد کارگران ثابت:" };
                long? hoghogh_edari = 0, hoghogh_shahri = 0, hoghogh_omrani = 0, hoghogh_nosazi = 0;
                long? khales_edari = 0, khales_shahri = 0, khales_omrani = 0, khales_nosazi = 0;
                var q = PayServic.GetRptSanad2(Year, Month, nobatPardakht, OrganId, IP, out ErrPay).ToList();
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < q.Count; j++)
                    {
                        if (q[j].fldEmploymentCenterId == i + 1 && q[j].fldTypeOfCostCenterId == 1)
                        {
                            hoghogh_edari += q[j].hoghogh;
                        }
                        if (q[j].fldEmploymentCenterId == i + 1 && q[j].fldTypeOfCostCenterId == 2)
                        {
                            hoghogh_shahri += q[j].hoghogh;
                        }
                        if (q[j].fldEmploymentCenterId == i + 1 && q[j].fldTypeOfCostCenterId == 3)
                        {
                            hoghogh_omrani += q[j].hoghogh;
                        }
                        if (q[j].fldEmploymentCenterId == i + 1 && q[j].fldTypeOfCostCenterId == 4)
                        {
                            hoghogh_nosazi += q[j].hoghogh;
                        }
                    }

                    khales_edari += hoghogh_edari;
                    khales_shahri += hoghogh_shahri;
                    khales_omrani += hoghogh_omrani;
                    khales_nosazi += hoghogh_nosazi;
                    DataRow _Row = sanad_2_1dt.NewRow();
                    _Row["onvan"] = onvan_hoghugh[i];
                    _Row["edari"] = hoghogh_edari;
                    _Row["shahri"] = hoghogh_shahri;
                    _Row["omrani"] = hoghogh_omrani;
                    _Row["behsasi"] = hoghogh_nosazi;
                    sanad_2_1dt.Rows.Add(_Row);
                    hoghogh_edari = hoghogh_shahri = hoghogh_omrani = hoghogh_nosazi = 0;
                }

                string[] onvan_mazaya = { "مزایای شهردار:", "مزایای کارمندان رسمی:", "مزایای کارمندان ثابت:",
                                     "مزایای کارگران رسمی:", "مزایای کارگران روزمزد:",
                                     "مزایای کارکنان قراردادی:", "مزایای کارگران ثابت:" };
                long? mazaya_edari = 0, mazaya_shahri = 0, mazaya_omrani = 0, mazaya_nosazi = 0;
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < q.Count; j++)
                    {
                        if (q[j].fldEmploymentCenterId == i + 1 && q[j].fldTypeOfCostCenterId == 1)
                        {
                            mazaya_edari += q[j].KolMotalebat;
                        }
                        if (q[j].fldEmploymentCenterId == i + 1 && q[j].fldTypeOfCostCenterId == 2)
                        {
                            mazaya_shahri += q[j].KolMotalebat;
                        }
                        if (q[j].fldEmploymentCenterId == i + 1 && q[j].fldTypeOfCostCenterId == 3)
                        {
                            mazaya_omrani += q[j].KolMotalebat;
                        }
                        if (q[j].fldEmploymentCenterId == i + 1 && q[j].fldTypeOfCostCenterId == 4)
                        {
                            mazaya_nosazi += q[j].KolMotalebat;
                        }
                    }
                    khales_edari += mazaya_edari;
                    khales_shahri += mazaya_shahri;
                    khales_omrani += mazaya_omrani;
                    khales_nosazi += mazaya_nosazi;
                    DataRow _Row2 = sanad_2_1dt.NewRow();
                    _Row2["onvan"] = onvan_mazaya[i];
                    _Row2["edari"] = mazaya_edari;
                    _Row2["shahri"] = mazaya_shahri;
                    _Row2["omrani"] = mazaya_omrani;
                    _Row2["behsasi"] = mazaya_nosazi;
                    sanad_2_1dt.Rows.Add(_Row2);
                    mazaya_edari = mazaya_shahri = mazaya_omrani = mazaya_nosazi = 0;
                }

                string[] onvan_mazaya1 = { "اضافه کار و تعطیل کاری و نوبت کاری:", "عائله مندی و حق اولاد:", 
                                     "حق بیمه و پس انداز سهم شهرداری:"};

                //اضافه کار
                long? ezafekari_edari = 0, ezafekari_shahri = 0, ezafekari_omrani = 0, ezafekari_nosazi = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1)
                    {
                        ezafekari_edari += q[j].ezafekar;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2)
                    {
                        ezafekari_shahri += q[j].ezafekar;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3)
                    {
                        ezafekari_omrani += q[j].ezafekar;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4)
                    {
                        ezafekari_nosazi += q[j].ezafekar;
                    }
                }
                khales_edari += ezafekari_edari;
                khales_shahri += ezafekari_shahri;
                khales_omrani += ezafekari_omrani;
                khales_nosazi += ezafekari_nosazi;

                DataRow _Row3 = sanad_2_1dt.NewRow();
                _Row3["onvan"] = onvan_mazaya1[0];
                _Row3["edari"] = ezafekari_edari;
                _Row3["shahri"] = ezafekari_shahri;
                _Row3["omrani"] = ezafekari_omrani;
                _Row3["behsasi"] = ezafekari_nosazi;
                sanad_2_1dt.Rows.Add(_Row3);

                //عائله مندی
                long? olad_edari = 0, olad_shahri = 0, olad_omrani = 0, olad_nosazi = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1)
                    {
                        olad_edari += q[j].ayelemandi;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2)
                    {
                        olad_shahri += q[j].ayelemandi;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3)
                    {
                        olad_omrani += q[j].ayelemandi;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4)
                    {
                        olad_nosazi += q[j].ayelemandi;
                    }
                }
                khales_edari += olad_edari;
                khales_shahri += olad_shahri;
                khales_omrani += olad_omrani;
                khales_nosazi += olad_nosazi;
                DataRow _Row4 = sanad_2_1dt.NewRow();
                _Row4["onvan"] = onvan_mazaya1[1];
                _Row4["edari"] = olad_edari;
                _Row4["shahri"] = olad_shahri;
                _Row4["omrani"] = olad_omrani;
                _Row4["behsasi"] = olad_nosazi;
                sanad_2_1dt.Rows.Add(_Row4);

                //حق بیمه
                long? bime_edari = 0, bime_shahri = 0, bime_omrani = 0, bime_nosazi = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1)
                    {
                        bime_edari += q[j].haghbime;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2)
                    {
                        bime_shahri += q[j].haghbime;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3)
                    {
                        bime_omrani += q[j].haghbime;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4)
                    {
                        bime_nosazi += q[j].haghbime;
                    }
                }
                khales_edari += bime_edari;
                khales_shahri += bime_shahri;
                khales_omrani += bime_omrani;
                khales_nosazi += bime_nosazi;
                DataRow _Row5 = sanad_2_1dt.NewRow();
                _Row5["onvan"] = onvan_mazaya1[2];
                _Row5["edari"] = bime_edari;
                _Row5["shahri"] = bime_shahri;
                _Row5["omrani"] = bime_omrani;
                _Row5["behsasi"] = bime_nosazi;
                sanad_2_1dt.Rows.Add(_Row5);

                //مأموریت
                long? mamoriat_edari = 0, mamoriat_shahri = 0, mamoriat_omrani = 0, mamoriat_nosazi = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1)
                    {
                        mamoriat_edari += q[j].mamoriat;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2)
                    {
                        mamoriat_shahri += q[j].mamoriat;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3)
                    {
                        mamoriat_omrani += q[j].mamoriat;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4)
                    {
                        mamoriat_nosazi += q[j].mamoriat;
                    }
                }
                khales_edari += mamoriat_edari;
                khales_shahri += mamoriat_shahri;
                khales_omrani += mamoriat_omrani;
                khales_nosazi += mamoriat_nosazi;
                DataRow _Row6 = sanad_2_1dt.NewRow();
                _Row6["onvan"] = "حق ماموریت";
                _Row6["edari"] = mamoriat_edari;
                _Row6["shahri"] = mamoriat_shahri;
                _Row6["omrani"] = mamoriat_omrani;
                _Row6["behsasi"] = mamoriat_nosazi;
                sanad_2_1dt.Rows.Add(_Row6);

                string[] onvan_kosorat = { "مالیات پرداختی:", "بیمه خدمات درمانی:", 
                                     "بیمه تامین اجتماعی:","بازنشستگی:","پس انداز:"
                                     ,"بیمه عمر:","بیمه تکمیلی:","سایر کسورات:","خالص پرداختی:"};

                long? kosorat_edari = 0, kosorat_shahri = 0, kosorat_omrani = 0, kosorat_nosazi = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1)
                    {
                        kosorat_edari += q[j].maliat;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2)
                    {
                        kosorat_shahri += q[j].maliat;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3)
                    {
                        kosorat_omrani += q[j].maliat;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4)
                    {
                        kosorat_nosazi += q[j].maliat;
                    }
                }
                khales_edari -= kosorat_edari;
                khales_shahri -= kosorat_shahri;
                khales_omrani -= kosorat_omrani;
                khales_nosazi -= kosorat_nosazi;
                DataRow _Row7 = sanad_2_2dt.NewRow();
                _Row7["onvan"] = onvan_kosorat[0];
                _Row7["edari"] = kosorat_edari;
                _Row7["shahri"] = kosorat_shahri;
                _Row7["omrani"] = kosorat_omrani;
                _Row7["behsasi"] = kosorat_nosazi;
                sanad_2_2dt.Rows.Add(_Row7);
                kosorat_edari = kosorat_shahri = kosorat_omrani = kosorat_nosazi = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1)
                    {
                        kosorat_edari += q[j].darman;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2)
                    {
                        kosorat_shahri += q[j].darman;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3)
                    {
                        kosorat_omrani += q[j].darman;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4)
                    {
                        kosorat_nosazi += q[j].darman;
                    }
                }
                khales_edari -= kosorat_edari;
                khales_shahri -= kosorat_shahri;
                khales_omrani -= kosorat_omrani;
                khales_nosazi -= kosorat_nosazi;
                DataRow _Row8 = sanad_2_2dt.NewRow();
                _Row8["onvan"] = onvan_kosorat[1];
                _Row8["edari"] = kosorat_edari;
                _Row8["shahri"] = kosorat_shahri;
                _Row8["omrani"] = kosorat_omrani;
                _Row8["behsasi"] = kosorat_nosazi;
                sanad_2_2dt.Rows.Add(_Row8);
                kosorat_edari = kosorat_shahri = kosorat_omrani = kosorat_nosazi = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1 && q[j].fldTypeBimeId == 1)
                    {
                        kosorat_edari += q[j].bime;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2 && q[j].fldTypeBimeId == 1)
                    {
                        kosorat_shahri += q[j].bime;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3 && q[j].fldTypeBimeId == 1)
                    {
                        kosorat_omrani += q[j].bime;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4 && q[j].fldTypeBimeId == 1)
                    {
                        kosorat_nosazi += q[j].bime;
                    }
                }
                khales_edari -= kosorat_edari;
                khales_shahri -= kosorat_shahri;
                khales_omrani -= kosorat_omrani;
                khales_nosazi -= kosorat_nosazi;
                DataRow _Row9 = sanad_2_2dt.NewRow();
                _Row9["onvan"] = onvan_kosorat[2];
                _Row9["edari"] = kosorat_edari;
                _Row9["shahri"] = kosorat_shahri;
                _Row9["omrani"] = kosorat_omrani;
                _Row9["behsasi"] = kosorat_nosazi;
                sanad_2_2dt.Rows.Add(_Row9);
                kosorat_edari = kosorat_shahri = kosorat_omrani = kosorat_nosazi = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1 && q[j].fldTypeBimeId == 2)
                    {
                        kosorat_edari += q[j].bime;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2 && q[j].fldTypeBimeId == 2)
                    {
                        kosorat_shahri += q[j].bime;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3 && q[j].fldTypeBimeId == 2)
                    {
                        kosorat_omrani += q[j].bime;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4 && q[j].fldTypeBimeId == 2)
                    {
                        kosorat_nosazi += q[j].bime;
                    }
                }
                khales_edari -= kosorat_edari;
                khales_shahri -= kosorat_shahri;
                khales_omrani -= kosorat_omrani;
                khales_nosazi -= kosorat_nosazi;
                DataRow _Row10 = sanad_2_2dt.NewRow();
                _Row10["onvan"] = onvan_kosorat[3];
                _Row10["edari"] = kosorat_edari;
                _Row10["shahri"] = kosorat_shahri;
                _Row10["omrani"] = kosorat_omrani;
                _Row10["behsasi"] = kosorat_nosazi;
                sanad_2_2dt.Rows.Add(_Row10);
                kosorat_edari = kosorat_shahri = kosorat_omrani = kosorat_nosazi = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1)
                    {
                        kosorat_edari += q[j].pasandaz;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2)
                    {
                        kosorat_shahri += q[j].pasandaz;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3)
                    {
                        kosorat_omrani += q[j].pasandaz;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4)
                    {
                        kosorat_nosazi += q[j].pasandaz;
                    }
                }
                khales_edari -= kosorat_edari;
                khales_shahri -= kosorat_shahri;
                khales_omrani -= kosorat_omrani;
                khales_nosazi -= kosorat_nosazi;
                DataRow _Row11 = sanad_2_2dt.NewRow();
                _Row11["onvan"] = onvan_kosorat[4];
                _Row11["edari"] = kosorat_edari;
                _Row11["shahri"] = kosorat_shahri;
                _Row11["omrani"] = kosorat_omrani;
                _Row11["behsasi"] = kosorat_nosazi;
                sanad_2_2dt.Rows.Add(_Row11);
                kosorat_edari = kosorat_shahri = kosorat_omrani = kosorat_nosazi = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1)
                    {
                        kosorat_edari += q[j].omr;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2)
                    {
                        kosorat_shahri += q[j].omr;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3)
                    {
                        kosorat_omrani += q[j].omr;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4)
                    {
                        kosorat_nosazi += q[j].omr;
                    }
                }
                khales_edari -= kosorat_edari;
                khales_shahri -= kosorat_shahri;
                khales_omrani -= kosorat_omrani;
                khales_nosazi -= kosorat_nosazi;
                DataRow _Row12 = sanad_2_2dt.NewRow();
                _Row12["onvan"] = onvan_kosorat[5];
                _Row12["edari"] = kosorat_edari;
                _Row12["shahri"] = kosorat_shahri;
                _Row12["omrani"] = kosorat_omrani;
                _Row12["behsasi"] = kosorat_nosazi;
                sanad_2_2dt.Rows.Add(_Row12);
                kosorat_edari = kosorat_shahri = kosorat_omrani = kosorat_nosazi = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1)
                    {
                        kosorat_edari += q[j].takmili;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2)
                    {
                        kosorat_shahri += q[j].takmili;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3)
                    {
                        kosorat_omrani += q[j].takmili;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4)
                    {
                        kosorat_nosazi += q[j].takmili;
                    }
                }
                khales_edari -= kosorat_edari;
                khales_shahri -= kosorat_shahri;
                khales_omrani -= kosorat_omrani;
                khales_nosazi -= kosorat_nosazi;
                DataRow _Row13 = sanad_2_2dt.NewRow();
                _Row13["onvan"] = onvan_kosorat[6];
                _Row13["edari"] = kosorat_edari;
                _Row13["shahri"] = kosorat_shahri;
                _Row13["omrani"] = kosorat_omrani;
                _Row13["behsasi"] = kosorat_nosazi;
                sanad_2_2dt.Rows.Add(_Row13);
                kosorat_edari = kosorat_shahri = kosorat_omrani = kosorat_nosazi = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeOfCostCenterId == 1)
                    {
                        kosorat_edari += q[j].kolkosurat;
                    }
                    if (q[j].fldTypeOfCostCenterId == 2)
                    {
                        kosorat_shahri += q[j].kolkosurat;
                    }
                    if (q[j].fldTypeOfCostCenterId == 3)
                    {
                        kosorat_omrani += q[j].kolkosurat;
                    }
                    if (q[j].fldTypeOfCostCenterId == 4)
                    {
                        kosorat_nosazi += q[j].kolkosurat;
                    }
                }
                khales_edari -= kosorat_edari;
                khales_shahri -= kosorat_shahri;
                khales_omrani -= kosorat_omrani;
                khales_nosazi -= kosorat_nosazi;
                DataRow _Row14 = sanad_2_2dt.NewRow();
                _Row14["onvan"] = onvan_kosorat[7];
                _Row14["edari"] = kosorat_edari;
                _Row14["shahri"] = kosorat_shahri;
                _Row14["omrani"] = kosorat_omrani;
                _Row14["behsasi"] = kosorat_nosazi;
                sanad_2_2dt.Rows.Add(_Row14);
                kosorat_edari = kosorat_shahri = kosorat_omrani = kosorat_nosazi = 0;

                DataRow _Row15 = sanad_2_2dt.NewRow();
                _Row15["onvan"] = onvan_kosorat[8];
                _Row15["edari"] = khales_edari;
                _Row15["shahri"] = khales_shahri;
                _Row15["omrani"] = khales_omrani;
                _Row15["behsasi"] = khales_nosazi;
                sanad_2_2dt.Rows.Add(_Row15);

                for (int i = 0; i < 9; i++)
                {
                    DataRow _Roww = sanad_2_2dt.NewRow();
                    _Roww["onvan"] = "";
                    _Roww["edari"] = "0";
                    _Roww["shahri"] = "0";
                    _Roww["omrani"] = "0";
                    _Roww["behsasi"] = "0";
                    sanad_2_2dt.Rows.Add(_Roww);
                }

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 25, IP, out Err).ToList();
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\SanadHesabDari2.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt_K_M, "rasaFMSPayRoll");
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
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("flag", flag);
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
        public ActionResult GeneratePDFSanadHesabdari3_BonKart(int OrganId, short Year, byte Month, byte nobatPardakht)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                System.Data.DataSet dt_K_M = new System.Data.DataSet();
                dt_K_M.DataSetName = "DataSetKM";
                System.Data.DataTable sanad_2_1dt = new System.Data.DataTable();
                sanad_2_1dt.TableName = "sanad_2_1";
                System.Data.DataTable sanad_2_2dt = new System.Data.DataTable();
                sanad_2_2dt.TableName = "sanad_2_2";

                //ساختن ستونهای DataTable
                sanad_2_1dt.Columns.Add("onvan", typeof(string));
                sanad_2_1dt.Columns.Add("edari", typeof(long));
                sanad_2_1dt.Columns.Add("shahri", typeof(long));
                sanad_2_1dt.Columns.Add("omrani", typeof(long));
                sanad_2_1dt.Columns.Add("behsasi", typeof(long));

                sanad_2_2dt.Columns.Add("onvan", typeof(string));
                sanad_2_2dt.Columns.Add("edari", typeof(long));
                sanad_2_2dt.Columns.Add("shahri", typeof(long));
                sanad_2_2dt.Columns.Add("omrani", typeof(long));
                sanad_2_2dt.Columns.Add("behsasi", typeof(long));

                dt_K_M.Tables.Add(sanad_2_1dt);
                dt_K_M.Tables.Add(sanad_2_2dt);

                //پر کردن DataTable
                long? khales_edari = 0;
                var q = PayServic.GetRptSanad3(Year, Month, nobatPardakht, OrganId, IP, out ErrPay).ToList();
                
                string[] onvan_mazaya1 = {"اقلام خوراکی(160203):", "کالاهای بهداشتی(160290):" ,"مناسبت ها(160209):"};
                //اقلام خوراکی
                long? khoraki = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    khoraki += q[j].khoraki;
                }
                khales_edari += khoraki;
                DataRow _Row1 = sanad_2_1dt.NewRow();
                _Row1["onvan"] = onvan_mazaya1[0];
                _Row1["edari"] = khoraki;
                sanad_2_1dt.Rows.Add(_Row1);

                //کالاهای بهداشتی
                long? kalaBehdashti = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    kalaBehdashti += q[j].kalaBehdashti;
                }
                khales_edari += kalaBehdashti;
                DataRow _Row2 = sanad_2_1dt.NewRow();
                _Row2["onvan"] = onvan_mazaya1[1];
                _Row2["edari"] = kalaBehdashti;
                sanad_2_1dt.Rows.Add(_Row2);

                //مناسبت
                long? Monasebat = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    Monasebat += q[j].Monasebat;
                }
                khales_edari += Monasebat;
                DataRow _Row3 = sanad_2_1dt.NewRow();
                _Row3["onvan"] = onvan_mazaya1[2];
                _Row3["edari"] = Monasebat;
                sanad_2_1dt.Rows.Add(_Row3);

                
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 25, IP, out Err).ToList();
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\SanadHesabDari3.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt_K_M, "rasaFMSPayRoll");
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
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("flag", flag);
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
        public ActionResult GeneratePDFSanadHesabdari3(int OrganId,short Year, byte Month, byte nobatPardakht, byte? ReportTypeSanad3)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                System.Data.DataSet dt_K_M = new System.Data.DataSet();
                dt_K_M.DataSetName = "DataSetKM";
                System.Data.DataTable sanad_2_1dt = new System.Data.DataTable();
                sanad_2_1dt.TableName = "sanad_2_1";
                System.Data.DataTable sanad_2_2dt = new System.Data.DataTable();
                sanad_2_2dt.TableName = "sanad_2_2";

                //ساختن ستونهای DataTable
                sanad_2_1dt.Columns.Add("onvan", typeof(string));
                sanad_2_1dt.Columns.Add("edari", typeof(long));
                sanad_2_1dt.Columns.Add("shahri", typeof(long));
                sanad_2_1dt.Columns.Add("omrani", typeof(long));
                sanad_2_1dt.Columns.Add("behsasi", typeof(long));

                sanad_2_2dt.Columns.Add("onvan", typeof(string));
                sanad_2_2dt.Columns.Add("edari", typeof(long));
                sanad_2_2dt.Columns.Add("shahri", typeof(long));
                sanad_2_2dt.Columns.Add("omrani", typeof(long));
                sanad_2_2dt.Columns.Add("behsasi", typeof(long));

                dt_K_M.Tables.Add(sanad_2_1dt);
                dt_K_M.Tables.Add(sanad_2_2dt);

                //پر کردن DataTable
                string[] onvan_hoghugh = { "حقوق شهردار(110101):", "حقوق ثابت/مبنا کارکنان رسمی و پیمانی(110102):", "حقوق و دستمزد کارگران رسمی مشمول قانون کار(110103):",
                                     "حقوق کارمندان قراردادی(110104):", "حقوق کارگران قراردادی(110105):" };
                long? hoghogh_sh = 0;
                long? khales_edari = 0;
                var q = PayServic.GetRptSanad3(Year, Month, nobatPardakht, OrganId, IP, out ErrPay).ToList();
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < q.Count; j++)
                    {
                        if (q[j].fldEmploymentCenterId == i + 1)
                            hoghogh_sh += q[j].hoghogh;
                    }

                    khales_edari += hoghogh_sh;

                    DataRow _Row = sanad_2_1dt.NewRow();
                    _Row["onvan"] = onvan_hoghugh[i];
                    _Row["edari"] = hoghogh_sh;

                    sanad_2_1dt.Rows.Add(_Row);
                    hoghogh_sh = 0;
                }

                
                long? mazaya_edari = 0;
                if (ReportTypeSanad3 == null || ReportTypeSanad3 == 0)
                    ReportTypeSanad3 =1;
                if (ReportTypeSanad3 == 1)
                {
                    string[] onvan_mazaya = { "مزایای شهردار(110201):", "مزایای کارکنان رسمی و پیمانی(110202):", "مزایای کارگران رسمی مشمول قانون کار(110203):",
                                     "مزایای کارمندان قراردادی(110204):", "مزایای کارگران قراردادی(110205):"};
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < q.Count; j++)
                    {
                        if (q[j].fldEmploymentCenterId == i + 1 )
                            mazaya_edari += q[j].KolMotalebat + q[j].fldMotalebat;
                        
                    }
                    khales_edari += mazaya_edari;

                    DataRow _Row2 = sanad_2_1dt.NewRow();
                    _Row2["onvan"] = onvan_mazaya[i];
                  
                        _Row2["edari"] = mazaya_edari;
                   
                    sanad_2_1dt.Rows.Add(_Row2);
                    mazaya_edari = 0;
                }
            }
            else
                {
                    string[] onvan_mazaya = { "مزایای شهردار(110201):", "مزایای کارکنان رسمی و پیمانی(110202):", "مزایای کارگران رسمی مشمول قانون کار(110203):",
                                     "مزایای کارمندان قراردادی(110204):", "مزایای کارگران قراردادی(110205):","کمک های غیر نقدی بن کارگران رسمی:" ,"کمک های غیر نقدی بن کارگران قرار دادی:"};

                    for (int i = 0; i < 7; i++)
                    {
                        for (int j = 0; j < q.Count; j++)
                        {
                            if (q[j].fldEmploymentCenterId == i + 1 && (i != 2 && i != 4 && i != 5))
                                mazaya_edari += q[j].KolMotalebat + q[j].fldMotalebat;
                            else if (q[j].fldEmploymentCenterId == i + 1 && (i == 2 || i == 4))
                                mazaya_edari += q[j].KolMotalebat;
                            else if (q[j].fldEmploymentCenterId == 3 && i == 5)
                            {
                                mazaya_edari += q[j].fldMotalebat;

                            }
                            else if (q[j].fldEmploymentCenterId == 5 && i == 6)
                            {
                                mazaya_edari += q[j].fldMotalebat;

                            }
                        }
                        khales_edari += mazaya_edari;

                        DataRow _Row2 = sanad_2_1dt.NewRow();
                        _Row2["onvan"] = onvan_mazaya[i];

                        _Row2["edari"] = mazaya_edari;

                        sanad_2_1dt.Rows.Add(_Row2);
                        mazaya_edari = 0;
                    }
                }
                string[] onvan_mazaya1 = { "اضافه کار(110206):","تعطیل کاری و نوبت کاری(110207):", "عائله مندی و حق اولاد(160202):", 
                                     "حق بیمه تامین اجتماعی سهم شهرداری(160102):","بازنشستگی سهم شهرداری(160101):","حق بیمه خدمات درمانی سهم شهرداری(160103):",
                                     "بیمه تکمیلی سهم شهرداری(160207):","بیمه عمر سهم شهرداری:","پس انداز سهم کارفرما(160213):","رفاهی مسکن و کارانه(160214):","تسهیلات رفاهی(160290):","سنوات پایان خدمت(170201):"};

                //کارانه
                long? karane = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    karane += q[j].karane;
                }
                khales_edari += karane;

                DataRow _Rowk = sanad_2_1dt.NewRow();
                _Rowk["onvan"] = onvan_mazaya1[9];
                _Rowk["edari"] = karane;
                sanad_2_1dt.Rows.Add(_Rowk);

                //تسهیلات
                long? tashilat = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    tashilat += q[j].tashilatRefahi;
                }
                khales_edari += tashilat;

                DataRow _Rowt = sanad_2_1dt.NewRow();
                _Rowt["onvan"] = onvan_mazaya1[10];
                _Rowt["edari"] = tashilat;
                sanad_2_1dt.Rows.Add(_Rowt);

                //سنوات پایان خدمت
                long? sanavatP = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    sanavatP += q[j].sanavatPayanKhedmat;
                }
                khales_edari += sanavatP;

                DataRow _Rows = sanad_2_1dt.NewRow();
                _Rows["onvan"] = onvan_mazaya1[11];
                _Rows["edari"] = sanavatP;
                sanad_2_1dt.Rows.Add(_Rows);

                //اضافه کار
                long? ezafekari_edari = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    ezafekari_edari += q[j].ezafekar;
                }
                khales_edari += ezafekari_edari;

                DataRow _Row3 = sanad_2_1dt.NewRow();
                _Row3["onvan"] = onvan_mazaya1[0];
                _Row3["edari"] = ezafekari_edari;
                sanad_2_1dt.Rows.Add(_Row3);

                //تعطیل کار
                long? tatilkar_edari = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    tatilkar_edari += q[j].tatilkari;
                }
                khales_edari += tatilkar_edari;

                DataRow _Row33 = sanad_2_1dt.NewRow();
                _Row33["onvan"] = onvan_mazaya1[1];
                _Row33["edari"] = tatilkar_edari;
                sanad_2_1dt.Rows.Add(_Row33);

                //بازنشستگی کارفرما
                long? bime1_edari = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeBimeId == 2)
                        bime1_edari += q[j].bimeKarfarma;
                }
                khales_edari += bime1_edari;
                DataRow _Row55 = sanad_2_1dt.NewRow();
                _Row55["onvan"] = onvan_mazaya1[4];
                _Row55["edari"] = bime1_edari;

                sanad_2_1dt.Rows.Add(_Row55);

                //حق بیمه کارفرما
                long? bime_edari = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeBimeId == 1)
                        bime_edari += q[j].bimeKarfarma;
                }
                khales_edari += bime_edari;
                DataRow _Row5 = sanad_2_1dt.NewRow();
                _Row5["onvan"] = onvan_mazaya1[3];
                _Row5["edari"] = bime_edari;

                sanad_2_1dt.Rows.Add(_Row5);

                //درمان کارفرما
                long? bime2_edari = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    bime2_edari += q[j].darmanKarfarma;
                }
                khales_edari += bime2_edari;
                DataRow _Row56 = sanad_2_1dt.NewRow();
                _Row56["onvan"] = onvan_mazaya1[5];
                _Row56["edari"] = bime2_edari;
                sanad_2_1dt.Rows.Add(_Row56);

                //عائله مندی
                long? olad_edari = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    olad_edari += q[j].ayelemandi;
                }
                khales_edari += olad_edari;
                DataRow _Row4 = sanad_2_1dt.NewRow();
                _Row4["onvan"] = onvan_mazaya1[2];
                _Row4["edari"] = olad_edari;
                sanad_2_1dt.Rows.Add(_Row4);


                //تکمیلی کارفرما
                long? bime3_edari = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    bime3_edari += q[j].TakmiliKarfarma;
                }
                khales_edari += bime3_edari;
                DataRow _Row57 = sanad_2_1dt.NewRow();
                _Row57["onvan"] = onvan_mazaya1[6];
                _Row57["edari"] = bime3_edari;
                sanad_2_1dt.Rows.Add(_Row57);

                //عمر کارفرما
                long? bime4_edari = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    bime4_edari += q[j].omrKarfarma;
                }
                khales_edari += bime4_edari;
                DataRow _Row58 = sanad_2_1dt.NewRow();
                _Row58["onvan"] = onvan_mazaya1[7];
                _Row58["edari"] = bime4_edari;
                sanad_2_1dt.Rows.Add(_Row58);

                //پس انداز کارفرما
                long? bime5_edari = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    bime5_edari += q[j].pasandazKarfarma;
                }
                khales_edari += bime5_edari;
                DataRow _Row59 = sanad_2_1dt.NewRow();
                _Row59["onvan"] = onvan_mazaya1[8];
                _Row59["edari"] = bime5_edari;
                sanad_2_1dt.Rows.Add(_Row59);

                //مأموریت
                long? mamoriat_edari = 0, mamoriat_shahri = 0, mamoriat_omrani = 0, mamoriat_nosazi = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    mamoriat_edari += q[j].mamoriat;
                }
                khales_edari += mamoriat_edari;

                DataRow _Row6 = sanad_2_1dt.NewRow();
                _Row6["onvan"] = "حق ماموریت";
                _Row6["edari"] = mamoriat_edari;
                sanad_2_1dt.Rows.Add(_Row6);

                string[] onvan_kosorat = { "مالیات پرداختی:", "بیمه خدمات درمانی:", 
                                     "بیمه تامین اجتماعی:","بازنشستگی:","پس انداز:"
                                     ,"بیمه عمر:","بیمه تکمیلی:","سایر کسورات:","خالص پرداختی:"};

                long? kosorat_edari = 0;
                for (int j = 0; j < q.Count; j++)
                {
                    kosorat_edari += q[j].maliat;
                }
                khales_edari -= kosorat_edari;
                DataRow _Row7 = sanad_2_2dt.NewRow();
                _Row7["onvan"] = onvan_kosorat[0];
                _Row7["edari"] = kosorat_edari;

                sanad_2_2dt.Rows.Add(_Row7);
                kosorat_edari = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    kosorat_edari += q[j].darman;
                }
                khales_edari -= kosorat_edari;

                DataRow _Row8 = sanad_2_2dt.NewRow();
                _Row8["onvan"] = onvan_kosorat[1];
                _Row8["edari"] = kosorat_edari;

                sanad_2_2dt.Rows.Add(_Row8);
                kosorat_edari = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeBimeId == 1)
                        kosorat_edari += q[j].bime;
                }
                khales_edari -= kosorat_edari;

                DataRow _Row9 = sanad_2_2dt.NewRow();
                _Row9["onvan"] = onvan_kosorat[2];
                _Row9["edari"] = kosorat_edari;

                sanad_2_2dt.Rows.Add(_Row9);
                kosorat_edari = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    if (q[j].fldTypeBimeId == 2)
                        kosorat_edari += q[j].bime;
                }
                khales_edari -= kosorat_edari;

                DataRow _Row10 = sanad_2_2dt.NewRow();
                _Row10["onvan"] = onvan_kosorat[3];
                _Row10["edari"] = kosorat_edari;

                sanad_2_2dt.Rows.Add(_Row10);
                kosorat_edari = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    kosorat_edari += q[j].pasandaz;
                }
                khales_edari -= kosorat_edari;

                DataRow _Row11 = sanad_2_2dt.NewRow();
                _Row11["onvan"] = onvan_kosorat[4];
                _Row11["edari"] = kosorat_edari;

                sanad_2_2dt.Rows.Add(_Row11);
                kosorat_edari = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    kosorat_edari += q[j].omr;
                }
                khales_edari -= kosorat_edari;

                DataRow _Row12 = sanad_2_2dt.NewRow();
                _Row12["onvan"] = onvan_kosorat[5];
                _Row12["edari"] = kosorat_edari;

                sanad_2_2dt.Rows.Add(_Row12);
                kosorat_edari = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    kosorat_edari += q[j].takmili;
                }
                khales_edari -= kosorat_edari;

                DataRow _Row13 = sanad_2_2dt.NewRow();
                _Row13["onvan"] = onvan_kosorat[6];
                _Row13["edari"] = kosorat_edari;

                sanad_2_2dt.Rows.Add(_Row13);
                kosorat_edari = 0;

                for (int j = 0; j < q.Count; j++)
                {
                    kosorat_edari += q[j].kolkosurat;
                }
                khales_edari -= kosorat_edari;

                DataRow _Row14 = sanad_2_2dt.NewRow();
                _Row14["onvan"] = onvan_kosorat[7];
                _Row14["edari"] = kosorat_edari;

                sanad_2_2dt.Rows.Add(_Row14);
                kosorat_edari = 0;

                DataRow _Row15 = sanad_2_2dt.NewRow();
                _Row15["onvan"] = onvan_kosorat[8];
                _Row15["edari"] = khales_edari;

                sanad_2_2dt.Rows.Add(_Row15);

                for (int i = 0; i < 13; i++)
                {
                    DataRow _Roww = sanad_2_2dt.NewRow();
                    _Roww["onvan"] = "";
                    _Roww["edari"] = "0";

                    sanad_2_2dt.Rows.Add(_Roww);
                }

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 25, IP, out Err).ToList();
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\SanadHesabDari3.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.RegisterData(dt_K_M, "rasaFMSPayRoll");
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
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("flag", flag);
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
        public ActionResult CheckPayPersonalInfo(byte value)
        {
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptListPersonalTableAdapter RptListPersonal = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPersonalTableAdapter();
            RptListPersonal.Fill(dt.spr_Pay_RptListPersonal, value, 0, Convert.ToInt32(Session["OrganId"]));
            var AllowPrint = true;
            var q = dt.spr_Pay_RptListPersonal.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFPayPersonalInfo(int OrganId,byte value)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptListPersonalTableAdapter RptListPersonal = new DataSet.DataSet1TableAdapters.spr_Pay_RptListPersonalTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                dt.EnforceConstraints = false;
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                RptListPersonal.Fill(dt.spr_Pay_RptListPersonal, value, 0, OrganId);
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\PersonalList.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("NameUser", User.fldNameEmployee);
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
        public ActionResult CheckRptVam(string StartDate, string EndDate)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptVamTableAdapter RptVam = new DataSet.DataSet1TableAdapters.spr_Pay_RptVamTableAdapter();
            RptVam.Fill(dt.spr_Pay_RptVam, StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
            var AllowPrint = true;
            var q = dt.spr_Pay_RptVam.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFVam(string StartDate, string EndDate)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptVamTableAdapter RptVam = new DataSet.DataSet1TableAdapters.spr_Pay_RptVamTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                RptVam.Fill(dt.spr_Pay_RptVam, StartDate, EndDate, Convert.ToInt32(Session["OrganId"]));
                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", (Session["OrganId"]).ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, StartDate, 20, IP, out Err).ToList();
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

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Vam.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("AzTarikh", StartDate);
                Report.SetParameterValue("TaTarikh", EndDate);
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
        public ActionResult CheckStatusPersonal(byte type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptStatusPersonalTableAdapter StatusPersonal = new DataSet.DataSet1TableAdapters.spr_Pay_RptStatusPersonalTableAdapter();
            StatusPersonal.Fill(dt.spr_Pay_RptStatusPersonal, type, Convert.ToInt32(Session["OrganId"]));
            var AllowPrint = true;
            var q = dt.spr_Pay_RptStatusPersonal.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GeneratePDFStatusPersonal(byte type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptStatusPersonalTableAdapter StatusPersonal = new DataSet.DataSet1TableAdapters.spr_Pay_RptStatusPersonalTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                string Title = "فعال";
                if (type == 2)
                {
                    Title = "غیرفعال";
                }
                else if (type == 3)
                {
                    Title = "بازنشسته";
                }
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                StatusPersonal.Fill(dt.spr_Pay_RptStatusPersonal, type, Convert.ToInt32(Session["OrganId"]));
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\StatusPersonal.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("NameUser", User.fldNameEmployee);
                Report.SetParameterValue("Title", Title);
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

        public ActionResult GeneratePDFUser()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_RptUserTableAdapter RptUser = new DataSet.DataSet_ComTableAdapters.spr_RptUserTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                RptUser.Fill(dt_Com.spr_RptUser, Convert.ToInt32(Session["OrganId"]));
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\User.frx");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("NameUser", User.fldNameEmployee);
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

        public ActionResult GetK_MParametri(string type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var fieldName = "Kosurat";
            if (type == "5" || type == "18")
                fieldName = "Motalebat";
            var q = PayServic.GetParametrsWithFilter(fieldName, "", (Session["OrganId"]).ToString(), 0, IP, out ErrPay).ToList().Select(c => new { fldId = c.fldId, fldParametrName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult CheckKosorat_MotalebatParametri(string StartDate, string EndDate, string FieldName, int ParamId, string ParamName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            dt.EnforceConstraints = false;
            DataSet.DataSet1TableAdapters.spr_Pay_RptKosorat_MotalebatParametriTableAdapter Kosorat_MotalebatParametri = new DataSet.DataSet1TableAdapters.spr_Pay_RptKosorat_MotalebatParametriTableAdapter();
            Kosorat_MotalebatParametri.Fill(dt.spr_Pay_RptKosorat_MotalebatParametri, StartDate, EndDate, ParamId, FieldName, Convert.ToInt32(Session["OrganId"]));
            var AllowPrint = true;
            var q = dt.spr_Pay_RptKosorat_MotalebatParametri.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            });
        }
        public ActionResult GeneratePDFKosorat_MotalebatParametri(string StartDate, string EndDate, string FieldName, int ParamId, string ParamName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                string title = "";
                if (FieldName == "Kosorat")
                {
                    title = "کسورات " + ParamName;
                }
                else
                {
                    title = "مطالبات " + ParamName;
                }
                DataSet.DataSet1 dt = new DataSet.DataSet1(); dt.EnforceConstraints = false;
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKosorat_MotalebatParametriTableAdapter Kosorat_MotalebatParametri = new DataSet.DataSet1TableAdapters.spr_Pay_RptKosorat_MotalebatParametriTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Date.Fill(dt_Com.spr_GetDate);
                Kosorat_MotalebatParametri.Fill(dt.spr_Pay_RptKosorat_MotalebatParametri, StartDate, EndDate, ParamId, FieldName, Convert.ToInt32(Session["OrganId"]));
                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", (Session["OrganId"]).ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var signers = servic.GetSignersWithFilter(Module_Organ.fldId, StartDate, 10, IP, out Err).ToList();
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


                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Kosurat-MotalebatParametriPerasonal.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("FromDate", StartDate);
                Report.SetParameterValue("ToDate", EndDate);
                Report.SetParameterValue("Title", title);
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
        public ActionResult CheckKosourat_Motalebat_P(int OrganId,short Year, byte Month, byte nobatPardakht, string nobatPardakhtName, int TypeParametrhaId, string ParamName, int CostCenterId, string CostCenterName, string FieldName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptKosurat_MotalebatPardakhtShodeTableAdapter Kosurat_MotalebatPardakhtShode = new DataSet.DataSet1TableAdapters.spr_Pay_RptKosurat_MotalebatPardakhtShodeTableAdapter();
            Kosurat_MotalebatPardakhtShode.Fill(dt.spr_Pay_RptKosurat_MotalebatPardakhtShode, FieldName, Year, Month, TypeParametrhaId, CostCenterId,
                nobatPardakht, OrganId);
            var AllowPrint = true;
            var q = dt.spr_Pay_RptKosurat_MotalebatPardakhtShode.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GeneratePDKosourat_Motalebat_P(int OrganId,short Year, byte Month, byte nobatPardakht, string nobatPardakhtName, int TypeParametrhaId, string ParamName, int CostCenterId, string CostCenterName, string FieldName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                string Title = "";
                List<WCF_Common.OBJ_Signers> signers = null;
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                dt.EnforceConstraints = false;
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptKosurat_MotalebatPardakhtShodeTableAdapter Kosurat_MotalebatPardakhtShode = new DataSet.DataSet1TableAdapters.spr_Pay_RptKosurat_MotalebatPardakhtShodeTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                Kosurat_MotalebatPardakhtShode.Fill(dt.spr_Pay_RptKosurat_MotalebatPardakhtShode, FieldName, Year, Month, TypeParametrhaId, CostCenterId, nobatPardakht,
                    OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),
                    IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                if (FieldName == "Kosurat")
                {
                    signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 35, IP, out Err).ToList();
                    Title = "کسورات";
                }
                else
                {
                    signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 36, IP, out Err).ToList();
                    Title = "مطالبات";
                }
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
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                    flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptKosurst_MotalebatPardakhtshode.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("mah", MonthName);
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("onvan", CostCenterName);
                Report.SetParameterValue("flag", flag);
                Report.SetParameterValue("ParamName", Title + " " + ParamName);

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
        public ActionResult CheckMande_SumKosur(int OrganId,short Year, byte Month, byte nobatPardakht, string nobatPardakhtName, int TypeParametrhaId, string ParamName, int CostCenterId, string CostCenterName, string FieldName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            DataSet.DataSet1 dt = new DataSet.DataSet1();
            DataSet.DataSet1TableAdapters.spr_Pay_RptMande_SumKosuratTableAdapter Mande_SumKosurat = new DataSet.DataSet1TableAdapters.spr_Pay_RptMande_SumKosuratTableAdapter();
            Mande_SumKosurat.Fill(dt.spr_Pay_RptMande_SumKosurat, FieldName, Year.ToString(), Month.ToString(), nobatPardakht, TypeParametrhaId, CostCenterId,
                OrganId);
            var AllowPrint = true;
            var q = dt.spr_Pay_RptMande_SumKosurat.Count();
            if (q == 0)
                AllowPrint = false;
            return Json(new
            {
                AllowPrint = AllowPrint
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerateMande_SumKosur(int OrganId,short Year, byte Month, byte nobatPardakht, string nobatPardakhtName, int TypeParametrhaId, string ParamName, int CostCenterId, string CostCenterName, string FieldName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                string Title = "";
                List<WCF_Common.OBJ_Signers> signers = null;
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter Date = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptMande_SumKosuratTableAdapter Mande_SumKosurat = new DataSet.DataSet1TableAdapters.spr_Pay_RptMande_SumKosuratTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Date.Fill(dt_Com.spr_GetDate);
                Mande_SumKosurat.Fill(dt.spr_Pay_RptMande_SumKosurat, FieldName, Year.ToString(), Month.ToString(), nobatPardakht, TypeParametrhaId, CostCenterId, OrganId);

                var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", OrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                if (FieldName == "Jam")
                {
                    signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 42, IP, out Err).ToList();
                    Title = "جمع";
                }
                else
                {
                    signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 43, IP, out Err).ToList();
                    Title = "مانده";
                }
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

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\RptMande_SumKosur.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("s5", s5);
                Report.SetParameterValue("t1", t1);
                Report.SetParameterValue("t2", t2);
                Report.SetParameterValue("t3", t3);
                Report.SetParameterValue("t4", t4);
                Report.SetParameterValue("t5", t5);
                Report.SetParameterValue("s1", s1);
                Report.SetParameterValue("s2", s2);
                Report.SetParameterValue("s3", s3);
                Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Mah", MonthName);
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("Title", Title);
                Report.SetParameterValue("markaz", CostCenterName);

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
        public ActionResult GeneratePDFFishEydi(int OrganId, short Year, byte Month, byte nobatPardakht, int PersonId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                //string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                //string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                //List<WCF_Common.OBJ_Signers> signers = null;
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishEydiTableAdapter FishEydi = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishEydiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishEydi1TableAdapter FishEydi1 = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishEydi1TableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishEydi2TableAdapter FishEydi2 = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishEydi2TableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                FishEydi.Fill(dt.spr_Pay_RptFishEydi, Year, Month, nobatPardakht, PersonId, 0, OrganId);
                FishEydi1.Fill(dt.spr_Pay_RptFishEydi1, Year, Month, nobatPardakht, PersonId, 1, OrganId);
                FishEydi2.Fill(dt.spr_Pay_RptFishEydi2, Year, Month, nobatPardakht, PersonId, 2, OrganId);
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";
                if (PersonId != 0)
                {
                    FieldName2 = "CheckMaliyat_PersonalId";
                }
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(PersonId), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                //var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", User.fldOrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                //var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                //signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 44, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();

                //if (signers.Count() == 1)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //}
                //else if (signers.Count() == 2)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s4 = signers[1].Post;
                //    t4 = signers[1].SignerName;
                //}
                //else if (signers.Count() == 3)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s3 = signers[1].Post;
                //    t3 = signers[1].SignerName;
                //    s5 = signers[2].Post;
                //    t5 = signers[2].SignerName;
                //}
                //else if (signers.Count() == 4)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //}
                //else if (signers.Count() == 5)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //    s5 = signers[4].Post;
                //    t5 = signers[4].SignerName;
                //}

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\FishEydi.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                //Report.SetParameterValue("s5", s5);
                //Report.SetParameterValue("t1", t1);
                //Report.SetParameterValue("t2", t2);
                //Report.SetParameterValue("t3", t3);
                //Report.SetParameterValue("t4", t4);
                //Report.SetParameterValue("t5", t5);
                //Report.SetParameterValue("s1", s1);
                //Report.SetParameterValue("s2", s2);
                //Report.SetParameterValue("s3", s3);
                //Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Month", MonthName);
                Report.SetParameterValue("flag", flag);
                Report.SetParameterValue("Year", Year.ToString());
                string serial = ""; byte Type = 0;
                if (PersonId != 0)
                    Type = 1;
                serial = PayServic.InsertFishLog(Convert.ToByte(Type), PersonId, OrganId, Year, Month, nobatPardakht, null, null, null, null, 2, null,
                    Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out ErrPay);
                Report.SetParameterValue("serial", serial);
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

        public ActionResult GeneratePDFFishMamooriat(int OrganId,short Year, byte Month, byte nobatPardakht, int PersonId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                //string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                //string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                // List<WCF_Common.OBJ_Signers> signers = null;
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishMamooriyatTableAdapter FishMamooriyat = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishMamooriyatTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishMamooriyat1TableAdapter FishMamooriyat1 = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishMamooriyat1TableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishMamooriyat2TableAdapter FishMamooriyat2 = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishMamooriyat2TableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                FishMamooriyat.Fill(dt.spr_Pay_RptFishMamooriyat, Year, Month, PersonId, nobatPardakht, 0, OrganId);
                FishMamooriyat1.Fill(dt.spr_Pay_RptFishMamooriyat1, Year, Month, PersonId, nobatPardakht, 1, OrganId);
                FishMamooriyat2.Fill(dt.spr_Pay_RptFishMamooriyat2, Year, Month, PersonId, nobatPardakht, 2, OrganId);
                int flag = 1;
                var FieldName2 = "CheckMaliyat";
                if (PersonId != 0)
                {
                    FieldName2 = "CheckMaliyat_PersonalId";
                }
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(0), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                //var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", User.fldOrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                //var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                //signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 48, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();

                //if (signers.Count() == 1)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //}
                //else if (signers.Count() == 2)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s4 = signers[1].Post;
                //    t4 = signers[1].SignerName;
                //}
                //else if (signers.Count() == 3)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s3 = signers[1].Post;
                //    t3 = signers[1].SignerName;
                //    s5 = signers[2].Post;
                //    t5 = signers[2].SignerName;
                //}
                //else if (signers.Count() == 4)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //}
                //else if (signers.Count() == 5)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //    s5 = signers[4].Post;
                //    t5 = signers[4].SignerName;
                //}

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\FishMamooriyat.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                //Report.SetParameterValue("s5", s5);
                //Report.SetParameterValue("t1", t1);
                //Report.SetParameterValue("t2", t2);
                //Report.SetParameterValue("t3", t3);
                //Report.SetParameterValue("t4", t4);
                //Report.SetParameterValue("t5", t5);
                //Report.SetParameterValue("s1", s1);
                //Report.SetParameterValue("s2", s2);
                //Report.SetParameterValue("s3", s3);
                //Report.SetParameterValue("s4", s4);
                string serial = ""; byte Type = 0;
                if (PersonId != 0)
                    Type = 1;
                serial = PayServic.InsertFishLog(Convert.ToByte(Type), PersonId, OrganId, Year, Month, nobatPardakht, null, null, null, null, 2, null,
                    Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out ErrPay);
                Report.SetParameterValue("serial", serial);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Month", MonthName);
                Report.SetParameterValue("Year", Year.ToString());
                Report.SetParameterValue("flag", flag);
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

        public ActionResult GeneratePDFFishMorakhasi(int OrganId, short Year, byte Month, byte nobatPardakht, int PersonId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                //string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                //string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                //List<WCF_Common.OBJ_Signers> signers = null;
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishMorakhasiTableAdapter FishMorakhasi = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishMorakhasiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishMorakhasi1TableAdapter FishMorakhasi1 = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishMorakhasi1TableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                FishMorakhasi.Fill(dt.spr_Pay_RptFishMorakhasi, Year, Month, nobatPardakht, PersonId, OrganId, 0);
                FishMorakhasi1.Fill(dt.spr_Pay_RptFishMorakhasi1, Year, Month, nobatPardakht, PersonId, OrganId, 1);
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";
                if (PersonId != 0)
                {
                    FieldName2 = "CheckMaliyat_PersonalId";
                }
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(PersonId), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                //var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", User.fldOrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                //var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                //signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 49, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();

                //if (signers.Count() == 1)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //}
                //else if (signers.Count() == 2)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s4 = signers[1].Post;
                //    t4 = signers[1].SignerName;
                //}
                //else if (signers.Count() == 3)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s3 = signers[1].Post;
                //    t3 = signers[1].SignerName;
                //    s5 = signers[2].Post;
                //    t5 = signers[2].SignerName;
                //}
                //else if (signers.Count() == 4)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //}
                //else if (signers.Count() == 5)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //    s5 = signers[4].Post;
                //    t5 = signers[4].SignerName;
                //}

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\FishMorakhasi.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                //Report.SetParameterValue("s5", s5);
                //Report.SetParameterValue("t1", t1);
                //Report.SetParameterValue("t2", t2);
                //Report.SetParameterValue("t3", t3);
                //Report.SetParameterValue("t4", t4);
                //Report.SetParameterValue("t5", t5);
                //Report.SetParameterValue("s1", s1);
                //Report.SetParameterValue("s2", s2);
                //Report.SetParameterValue("s3", s3);
                //Report.SetParameterValue("s4", s4);
                string serial = ""; byte Type = 0;
                if (PersonId != 0)
                    Type = 1;
                serial = PayServic.InsertFishLog(Convert.ToByte(Type), PersonId, OrganId, Year, Month, nobatPardakht, null, null, null, null, 3, null,
                    Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out ErrPay);
                Report.SetParameterValue("serial", serial);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Month", MonthName);
                Report.SetParameterValue("Year", Year.ToString());
                Report.SetParameterValue("flag", flag);
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
        public ActionResult GeneratePDFFishBonKart(int OrganId, short Year, byte Month, byte nobatPardakht, byte Type, int? PersonalId, string FieldName, int? CostCenter_ChartId,
             byte? CalcType, byte? FilterType, int? CostCenterId){
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                Report rep = new Report();
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\Reports\bonkart.frx";
                rep.Load(path);
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";
                if (PersonalId != 0)
                {
                    FieldName2 = "CheckMaliyat_PersonalId";
                }
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(PersonalId), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                rep.SetParameterValue("NobatPardakht", nobatPardakht);
                rep.SetParameterValue("Year", Year);
                rep.SetParameterValue("Month", Month);
                rep.SetParameterValue("organID", OrganId);
                rep.SetParameterValue("userId", Convert.ToInt32(Session["UserId"]));
                rep.SetParameterValue("flag", flag);
                rep.SetParameterValue("PersonalId", PersonalId);                
                if (Type == 0)
                {
                    rep.SetParameterValue("fieldName", FieldName);
                    rep.SetParameterValue("value", CostCenter_ChartId);
                }
                else
                {
                    var serial = PayServic.InsertFishLog(Convert.ToByte(Type), PersonalId, OrganId, Year, Month, nobatPardakht, Convert.ToByte(FilterType), 0,
                       CostCenterId, CostCenter_ChartId, Convert.ToByte(CalcType), null, Session["Username"].ToString(), (Session["Password"].ToString()),
                       OrganId, IP, out ErrPay);
                    rep.SetParameterValue("serial", serial);
                    rep.SetParameterValue("fieldName", "fldPersonalId");
                    rep.SetParameterValue("value", PersonalId);
                }
                rep.SetParameterValue("connectionstr", System.Configuration.ConfigurationManager.ConnectionStrings["RasaFMSCommonDBConnectionString"].ConnectionString);

                if (rep.Report.Prepare())
                {
                    // Set PDF export props
                    FastReport.Export.Pdf.PDFExport pdfExport = new FastReport.Export.Pdf.PDFExport();
                    pdfExport.ShowProgress = false;
                    pdfExport.Compressed = true;
                    pdfExport.AllowPrint = true;
                    pdfExport.EmbeddingFonts = true;
                    MemoryStream strm = new MemoryStream();
                    rep.Report.Export(pdfExport, strm);
                    rep.Dispose();
                    pdfExport.Dispose();
                    strm.Position = 0;
                    // return stream in browser
                    return File(strm.ToArray(), "application/pdf");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GeneratePDFFishSayer(int OrganId,short Year, byte Month, byte nobatPardakht, byte marhalePardakht, int PersonId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                //string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                //string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                //List<WCF_Common.OBJ_Signers> signers = null;
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishSayerPardakhTableAdapter FishSayerPardakht = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishSayerPardakhTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishSayerPardakh1TableAdapter FishSayerPardakht1 = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishSayerPardakh1TableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishSayerPardakh2TableAdapter FishSayerPardakht2 = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishSayerPardakh2TableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                FishSayerPardakht.Fill(dt.spr_Pay_RptFishSayerPardakh, Year, Month, PersonId, nobatPardakht, marhalePardakht, 0, OrganId);
                FishSayerPardakht1.Fill(dt.spr_Pay_RptFishSayerPardakh1, Year, Month, PersonId, nobatPardakht, marhalePardakht, 1, OrganId);
                FishSayerPardakht2.Fill(dt.spr_Pay_RptFishSayerPardakh2, Year, Month, PersonId, nobatPardakht, marhalePardakht, 2, OrganId);
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";
                if (PersonId != 0)
                {
                    FieldName2 = "CheckMaliyat_PersonalId";
                }
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(PersonId), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                //var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", User.fldOrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                //var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                //signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 47, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();

                //if (signers.Count() == 1)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //}
                //else if (signers.Count() == 2)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s4 = signers[1].Post;
                //    t4 = signers[1].SignerName;
                //}
                //else if (signers.Count() == 3)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s3 = signers[1].Post;
                //    t3 = signers[1].SignerName;
                //    s5 = signers[2].Post;
                //    t5 = signers[2].SignerName;
                //}
                //else if (signers.Count() == 4)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //}
                //else if (signers.Count() == 5)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //    s5 = signers[4].Post;
                //    t5 = signers[4].SignerName;
                //}

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\FishSayerPardakht.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                //Report.SetParameterValue("s5", s5);
                //Report.SetParameterValue("t1", t1);
                //Report.SetParameterValue("t2", t2);
                //Report.SetParameterValue("t3", t3);
                //Report.SetParameterValue("t4", t4);
                //Report.SetParameterValue("t5", t5);
                //Report.SetParameterValue("s1", s1);
                //Report.SetParameterValue("s2", s2);
                //Report.SetParameterValue("s3", s3);
                //Report.SetParameterValue("s4", s4);
                string serial = ""; byte Type = 0;
                if (PersonId != 0)
                    Type = 1;
                serial = PayServic.InsertFishLog(Convert.ToByte(Type), PersonId, OrganId, Year, Month, nobatPardakht, null, marhalePardakht, null, null, 6, null,
                    Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out ErrPay);
                Report.SetParameterValue("serial", serial);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Month", MonthName);
                Report.SetParameterValue("Year", Year.ToString());
                Report.SetParameterValue("flag", flag);
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

        public ActionResult GeneratePDFFishKomak(int OrganId,short Year, byte Month, int PersonId, bool type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                //string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                //string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                string Title = "";
                //List<WCF_Common.OBJ_Signers> signers = null;
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishKomakGHeyreNaghdiTableAdapter FishKomakGHeyreNaghdi = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishKomakGHeyreNaghdiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishKomakGHeyreNaghdi1TableAdapter FishKomakGHeyreNaghdi1 = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishKomakGHeyreNaghdi1TableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishKomakGHeyreNaghdi2TableAdapter FishKomakGHeyreNaghdi2 = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishKomakGHeyreNaghdi2TableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();

                if (type == true)
                {
                    Title = "کمک های غیرنقدی مستمر";
                }
                else
                {
                    Title = "کمک های غیرنقدی غیر مستمر";
                }

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                FishKomakGHeyreNaghdi.Fill(dt.spr_Pay_RptFishKomakGHeyreNaghdi, Year, Month, PersonId, type, 0, OrganId);
                FishKomakGHeyreNaghdi1.Fill(dt.spr_Pay_RptFishKomakGHeyreNaghdi1, Year, Month, PersonId, type, 1, OrganId);
                FishKomakGHeyreNaghdi2.Fill(dt.spr_Pay_RptFishKomakGHeyreNaghdi2, Year, Month, PersonId, type, 2, OrganId);
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat_Variz";

                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), "", OrganId, Convert.ToInt32(PersonId), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                //var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", User.fldOrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                //var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                //signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 46, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();               

                //if (signers.Count() == 1)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //}
                //else if (signers.Count() == 2)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s4 = signers[1].Post;
                //    t4 = signers[1].SignerName;
                //}
                //else if (signers.Count() == 3)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s3 = signers[1].Post;
                //    t3 = signers[1].SignerName;
                //    s5 = signers[2].Post;
                //    t5 = signers[2].SignerName;
                //}
                //else if (signers.Count() == 4)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //}
                //else if (signers.Count() == 5)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //    s5 = signers[4].Post;
                //    t5 = signers[4].SignerName;
                //}

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\FishKomakGheyreNaghdi.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                //Report.SetParameterValue("s5", s5);
                //Report.SetParameterValue("t1", t1);
                //Report.SetParameterValue("t2", t2);
                //Report.SetParameterValue("t3", t3);
                //Report.SetParameterValue("t4", t4);
                //Report.SetParameterValue("t5", t5);
                //Report.SetParameterValue("s1", s1);
                //Report.SetParameterValue("s2", s2);
                //Report.SetParameterValue("s3", s3);
                //Report.SetParameterValue("s4", s4);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Month", MonthName);
                Report.SetParameterValue("Year", Year.ToString());
                Report.SetParameterValue("flag", flag);
                Report.SetParameterValue("Title", Title);

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

        public ActionResult GeneratePDFFishEzafekari(int OrganId,short Year, byte Month, byte nobatPardakht, string nobatPardakhtName, int PersonId, string FieldName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string Title = "";
                //string t1 = "", t2 = "", t3 = "", t4 = "", t5 = "";
                //string s1 = "", s2 = "", s3 = "", s4 = "", s5 = "";
                //List<WCF_Common.OBJ_Signers> signers = null;
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishEzafekari_TatilKariTableAdapter Ezafekari_TatilKari = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishEzafekari_TatilKariTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishEzafekari_TatilKari1TableAdapter Ezafekari_TatilKari1 = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishEzafekari_TatilKari1TableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishEzafekari_TatilKari2TableAdapter Ezafekari_TatilKari2 = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishEzafekari_TatilKari2TableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                var MonthName = MyLib.Shamsi.ShamsiMonthname(Month);
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                Ezafekari_TatilKari.Fill(dt.spr_Pay_RptFishEzafekari_TatilKari, FieldName, Year, Month, PersonId, nobatPardakht, 0, OrganId);
                Ezafekari_TatilKari1.Fill(dt.spr_Pay_RptFishEzafekari_TatilKari1, FieldName, Year, Month, PersonId, nobatPardakht, 1, OrganId);
                Ezafekari_TatilKari2.Fill(dt.spr_Pay_RptFishEzafekari_TatilKari2, FieldName, Year, Month, PersonId, nobatPardakht, 2, OrganId);
                int flag = 1;/*گزارش بعد از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";
                if (PersonId != 0)
                {
                    FieldName2 = "CheckMaliyat_PersonalId";
                }
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(PersonId), IP, out ErrPay).FirstOrDefault();
                if (k != null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/
                //var Module_Organ = servic.GetModule_OrganWithFilter("fldOrganId", User.fldOrganId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).Where(l => l.fldModuleId == 3).FirstOrDefault();
                //var Tarikh = Year + "/" + Month.ToString().PadLeft(2, '0') + "/" + Helper.LastDayofMonth.GetLastDay(Year, Month);
                byte CalcType = 4;
                if (FieldName == "EzafeKari")
                {
                    // signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 41, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                    Title = "اضافه کاری";
                }
                else
                {
                    // signers = servic.GetSignersWithFilter(Module_Organ.fldId, Tarikh, 45, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                    Title = "تعطیل کاری";
                    CalcType = 5;
                }

                //if (signers.Count() == 1)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //}
                //else if (signers.Count() == 2)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s4 = signers[1].Post;
                //    t4 = signers[1].SignerName;
                //}
                //else if (signers.Count() == 3)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s3 = signers[1].Post;
                //    t3 = signers[1].SignerName;
                //    s5 = signers[2].Post;
                //    t5 = signers[2].SignerName;
                //}
                //else if (signers.Count() == 4)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //}
                //else if (signers.Count() == 5)
                //{
                //    s1 = signers[0].Post;
                //    t1 = signers[0].SignerName;
                //    s2 = signers[1].Post;
                //    t2 = signers[1].SignerName;
                //    s3 = signers[2].Post;
                //    t3 = signers[2].SignerName;
                //    s4 = signers[3].Post;
                //    t4 = signers[3].SignerName;
                //    s5 = signers[4].Post;
                //    t5 = signers[4].SignerName;
                //}                

                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\FishEzafekar_TatilKar.frx");
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                //Report.SetParameterValue("s5", s5);
                //Report.SetParameterValue("t1", t1);
                //Report.SetParameterValue("t2", t2);
                //Report.SetParameterValue("t3", t3);
                //Report.SetParameterValue("t4", t4);
                //Report.SetParameterValue("t5", t5);
                //Report.SetParameterValue("s1", s1);
                //Report.SetParameterValue("s2", s2);
                //Report.SetParameterValue("s3", s3);
                //Report.SetParameterValue("s4", s4);
                string serial = ""; byte Type = 0;
                if (PersonId != 0)
                    Type = 1;
                serial = PayServic.InsertFishLog(Convert.ToByte(Type), PersonId, OrganId, Year, Month, nobatPardakht, null, null, null, null, CalcType,
                    null, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId, IP, out ErrPay);
                Report.SetParameterValue("serial", serial);
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                Report.SetParameterValue("Month", MonthName);
                Report.SetParameterValue("Year", Year.ToString());
                Report.SetParameterValue("Title", Title);
                Report.SetParameterValue("flag", flag);
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

        public ActionResult GeneratePDFFishHoghughi(int OrganId, short Year, byte Month, byte nobatPardakht, byte Type, int? PersonalId, byte TypeFish, string FieldName, int? CostCenter_ChartId,
             byte? CalcType, byte? FilterType, int? CostCenterId/*, byte? TypeHesab*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                string serial = "";
                //var d = servic.GetDate(IP, out Err).fldDateTime;
                //var userid = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault().fldId;
                //serial = userid.ToString() + d.Year.ToString() + d.Month.ToString()+d.Day.ToString()+d.Hour.ToString()+d.Minute.ToString() + d.Second.ToString() + d.Millisecond.ToString() + IP.Replace(":","").Replace(".","");


                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

                DataSet.DataSet1TableAdapters.spr_Pay_rptFishHoghoghiTableAdapter FishHoghoghi = new DataSet.DataSet1TableAdapters.spr_Pay_rptFishHoghoghiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishHoghoghi_MotalebatTableAdapter FishHoghoghi_Motalebat = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishHoghoghi_MotalebatTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishHoghoghi_KosuratParam_BankTableAdapter FishHoghoghi_KosuratParam_Bank = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishHoghoghi_KosuratParam_BankTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptTasvirShakhTableAdapter Tasvir = new DataSet.DataSet1TableAdapters.spr_Pay_RptTasvirShakhTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter FileS = new DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                int flag=1;/*گزارش قبل از آپلود فایل مالیات*/
                var FieldName2 = "CheckMaliyat";
                if (PersonalId != 0)
                {
                    FieldName2 = "CheckMaliyat_PersonalId";
                }
                var k = PayServic.GetMohasebatWithFilter(FieldName2, Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, Convert.ToInt32(PersonalId), IP, out ErrPay).FirstOrDefault();
                if (k!= null)
                                       flag = 0;/*گزارش قبل از آپلود فایل مالیات*/

                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                var Setting = PayServic.GetSettingWithFilter("fldOrganId", OrganId.ToString(), 0, 0, IP, out ErrPay).FirstOrDefault();
                var sh = PayServic.GetMohasebatWithFilter("Fish", Year.ToString(), Month.ToString(), nobatPardakht.ToString(), OrganId, 1, IP, out ErrPay).FirstOrDefault();
                int FileId = 0;
                if(sh !=null)
                    FileId=servic.GetBankDetail(sh.fldBankId, IP, out Err).fldFileId;
               // int FileId = servic.GetBankDetail(Convert.ToInt32(Setting.fldH_BankFixId), IP, out Err).fldFileId;
                if (Setting.fldShowBankLogo == true)
                    FileS.Fill(dt_Com.spr_tblFileSelect, "fldId", FileId.ToString(), 1);
                if (Type == 0)
                {
                    FishHoghoghi.Fill(dt.spr_Pay_rptFishHoghoghi, FieldName, CostCenter_ChartId, nobatPardakht, Year, Month, OrganId, Convert.ToInt32(Session["UserId"]));
                    //FishHoghoghi_Motalebat.Fill(dt.spr_Pay_RptFishHoghoghi_Motalebat, 0, nobatPardakht, Year, Month, Convert.ToInt32(Session["UserId"]));
                    FishHoghoghi_KosuratParam_Bank.Fill(dt.spr_Pay_RptFishHoghoghi_KosuratParam_Bank, Year, Month, nobatPardakht, 0,OrganId, Convert.ToInt32(Session["UserId"]));
                    Tasvir.Fill(dt.spr_Pay_RptTasvirShakh, 0, OrganId);

                    for (int j = 0; j < dt.spr_Pay_rptFishHoghoghi.Count; j++)
                    {
                        FishHoghoghi_Motalebat.Fill(dt.spr_Pay_RptFishHoghoghi_Motalebat, dt.spr_Pay_rptFishHoghoghi[j].fldPersonalId, nobatPardakht, Year, Month, Convert.ToInt32(Session["UserId"]));
                        var motalebat = dt.spr_Pay_RptFishHoghoghi_Motalebat.ToList();
                            //dt.spr_Pay_RptFishHoghoghi_Motalebat.Where(l => l.fldPersonalId == dt.spr_Pay_rptFishHoghoghi[j].fldPersonalId).ToList();
                        var count = motalebat.Count;
                        if (count > 19)
                            count = 19;
                        //dt.rptFishHoghoghi_Motalebat1.Clear();
                        for (int i = 0; i < count; i++)
                        {
                            if (motalebat[i].fldMablagh != 0)
                            {
                                DataRow a = dt.rptFishHoghoghi_Motalebat1.NewRow();
                                a["fldItemsHoghughiTitle"] = motalebat[i].fldTitle;
                                a["fldMablagh"] = motalebat[i].fldMablagh;
                                a["fldPersonalId"] = motalebat[i].fldPersonalId;
                                a["radif"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldId;
                                dt.rptFishHoghoghi_Motalebat1.Rows.Add(a);
                            }
                        }
                        //if (motalebat.Count >= 19)
                        // dt.rptFishHoghoghi_Motalebat2.Clear();
                        for (int i = 19; i < motalebat.Count; i++)
                        {
                            if (motalebat[i].fldMablagh != 0)
                            {
                                DataRow a = dt.rptFishHoghoghi_Motalebat2.NewRow();
                                a["fldItemsHoghughiTitle"] = motalebat[i].fldTitle;
                                a["fldMablagh"] = motalebat[i].fldMablagh;
                                a["fldPersonalId"] = motalebat[i].fldPersonalId;
                                a["radif"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldId;
                                dt.rptFishHoghoghi_Motalebat2.Rows.Add(a);
                            }
                        }
                    }
                }
                else
                {
                    FishHoghoghi.Fill(dt.spr_Pay_rptFishHoghoghi, "fldPersonalId", PersonalId, nobatPardakht, Year, Month, OrganId, Convert.ToInt32(Session["UserId"]));
                    FishHoghoghi_Motalebat.Fill(dt.spr_Pay_RptFishHoghoghi_Motalebat, PersonalId, nobatPardakht, Year, Month, Convert.ToInt32(Session["UserId"]));
                    FishHoghoghi_KosuratParam_Bank.Fill(dt.spr_Pay_RptFishHoghoghi_KosuratParam_Bank, Year, Month, nobatPardakht, PersonalId,OrganId, Convert.ToInt32(Session["UserId"]));
                    Tasvir.Fill(dt.spr_Pay_RptTasvirShakh, PersonalId, OrganId);

                    var count = dt.spr_Pay_RptFishHoghoghi_Motalebat.Count;
                    if (count > 19)
                        count = 19;
                    //dt.rptFishHoghoghi_Motalebat1.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        if (dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh != 0)
                        {
                            DataRow a = dt.rptFishHoghoghi_Motalebat1.NewRow();
                            a["fldItemsHoghughiTitle"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldTitle;
                            a["fldMablagh"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh;
                            a["fldPersonalId"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldPersonalId;
                            a["radif"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldId;
                            dt.rptFishHoghoghi_Motalebat1.Rows.Add(a);
                        }
                    }
                    //if (dt.spr_Pay_RptFishHoghoghi_Motalebat.Count >= 19)
                    //dt.rptFishHoghoghi_Motalebat2.Clear();
                    for (int i = 19; i < dt.spr_Pay_RptFishHoghoghi_Motalebat.Count; i++)
                    {
                        if (dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh != 0)
                        {
                            DataRow a = dt.rptFishHoghoghi_Motalebat2.NewRow();
                            a["fldItemsHoghughiTitle"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldTitle;
                            a["fldMablagh"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh;
                            a["fldPersonalId"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldPersonalId;
                            a["radif"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldId;
                            dt.rptFishHoghoghi_Motalebat2.Rows.Add(a);
                        }
                    }
                }

                EnablePartialRendering = "false";
                FastReport.Report Report = new FastReport.Report();
                if (TypeFish == 1)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Fish.frx");
                else if (TypeFish == 2)
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Fish1.frx");
                else
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Fish2.frx");
                serial = PayServic.InsertFishLog(Convert.ToByte(Type), PersonalId, OrganId, Year, Month, nobatPardakht, Convert.ToByte(FilterType), Convert.ToByte(TypeFish),
                    CostCenterId, CostCenter_ChartId, Convert.ToByte(CalcType), null, Session["Username"].ToString(), (Session["Password"].ToString()), 
                    OrganId, IP, out ErrPay);
                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("Message", "");
                Report.SetParameterValue("serial", serial);
                Report.SetParameterValue("flag", flag);
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



        public ActionResult GeneratePDFFishHoghughiDeatils(int OrganId,short? Year, byte? Month, byte? nobatPardakht, int? PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {


                EnablePartialRendering = "false";
                FastReport.Report Report = new FastReport.Report();

                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\FishHoghoghi_Info.frx");

                Report.SetParameterValue("PersonalId", PersonalId);
                Report.SetParameterValue("Year", Year);
                Report.SetParameterValue("Month", Month);
                Report.SetParameterValue("nobatPardakht", nobatPardakht);
                Report.SetParameterValue("OrganId", OrganId);
                Report.SetParameterValue("UserId", Convert.ToInt32(Session["UserId"]));
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

        public void GeneratePDFFishHoghughiXML(int OrganId, short? Year, byte? Month, byte? nobatPardakht, byte? Type, int? PersonalId, byte? TypeFish/*, byte? TypeHesab*/)
        {
            if (Session["Username"] == null)
                return;

            try
            {
                MemoryStream outstream = new MemoryStream();
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet1TableAdapters.spr_Pay_rptFishHoghoghiTableAdapter FishHoghoghi = new DataSet.DataSet1TableAdapters.spr_Pay_rptFishHoghoghiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_rptFishHoghoghiTableAdapter FishHoghoghi1 = new DataSet.DataSet1TableAdapters.spr_Pay_rptFishHoghoghiTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishHoghoghi_MotalebatTableAdapter FishHoghoghi_Motalebat = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishHoghoghi_MotalebatTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptFishHoghoghi_KosuratParam_BankTableAdapter FishHoghoghi_KosuratParam_Bank = new DataSet.DataSet1TableAdapters.spr_Pay_RptFishHoghoghi_KosuratParam_BankTableAdapter();
                DataSet.DataSet1TableAdapters.spr_Pay_RptTasvirShakhTableAdapter Tasvir = new DataSet.DataSet1TableAdapters.spr_Pay_RptTasvirShakhTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter FileS = new DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
                dt.EnforceConstraints = false;
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                var Setting = PayServic.GetSettingWithFilter("fldOrganId", OrganId.ToString(), 0, 0, IP, out ErrPay).FirstOrDefault();
                int FileId = servic.GetBankDetail(Convert.ToInt32(Setting.fldH_BankFixId), IP, out Err).fldFileId;
                if (Setting.fldShowBankLogo == true)
                    FileS.Fill(dt_Com.spr_tblFileSelect, "fldId", FileId.ToString(), 1);
                if (Type == 0)
                {
                    FishHoghoghi.Fill(dt.spr_Pay_rptFishHoghoghi, "", PersonalId, nobatPardakht, Year, Month, OrganId, Convert.ToInt32(Session["UserId"]));
                    //FishHoghoghi_Motalebat.Fill(dt.spr_Pay_RptFishHoghoghi_Motalebat, 0, nobatPardakht, Year, Month);
                    //FishHoghoghi_KosuratParam_Bank.Fill(dt.spr_Pay_RptFishHoghoghi_KosuratParam_Bank, Year, Month, nobatPardakht, 0);
                    //Tasvir.Fill(dt.spr_Pay_RptTasvirShakh, 0, Convert.ToInt32(Session["OrganId"]));
                    //تولید xml
                    XmlDocument XDoc = new XmlDocument();
                    // Create root node.
                    XmlElement XElemRoot = XDoc.CreateElement("FishContent");
                    XElemRoot.SetAttribute("count", dt.spr_Pay_rptFishHoghoghi.Count.ToString());
                    XElemRoot.SetAttribute("type", "0");
                    int[] ar = new int[dt.spr_Pay_rptFishHoghoghi.Count];
                    for (int i = 0; i < ar.Count(); i++)
                    {
                        ar[i] = Convert.ToInt32(dt.spr_Pay_rptFishHoghoghi.Rows[i]["fldPersonalId"]);
                    }

                    for (int j = 0; j < ar.Count(); j++)
                    {
                        dt.rptFishHoghoghi_Motalebat1.Clear();
                        dt.rptFishHoghoghi_Motalebat2.Clear();
                        PersonalId = ar[j];
                        FishHoghoghi1.Fill(dt.spr_Pay_rptFishHoghoghi, "fldPersonalId", PersonalId, nobatPardakht, Year, Month, OrganId, Convert.ToInt32(Session["UserId"]));
                        FishHoghoghi_Motalebat.Fill(dt.spr_Pay_RptFishHoghoghi_Motalebat, PersonalId, nobatPardakht, Year, Month, Convert.ToInt32(Session["UserId"]));
                        FishHoghoghi_KosuratParam_Bank.Fill(dt.spr_Pay_RptFishHoghoghi_KosuratParam_Bank, Year, Month, nobatPardakht, PersonalId,OrganId, Convert.ToInt32(Session["UserId"]));
                        Tasvir.Fill(dt.spr_Pay_RptTasvirShakh, PersonalId, OrganId);

                        var count = dt.spr_Pay_RptFishHoghoghi_Motalebat.Count;
                        if (count > 19)
                            count = 19;
                        //dt.rptFishHoghoghi_Motalebat1.Clear();
                        for (int i = 0; i < count; i++)
                        {
                            if (dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh != 0)
                            {
                                DataRow a = dt.rptFishHoghoghi_Motalebat1.NewRow();
                                a["fldItemsHoghughiTitle"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldTitle;
                                a["fldMablagh"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh;
                                a["fldPersonalId"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldPersonalId;
                                a["radif"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldId;
                                dt.rptFishHoghoghi_Motalebat1.Rows.Add(a);
                            }
                        }
                        //if (dt.spr_Pay_RptFishHoghoghi_Motalebat.Count >= 19)
                        //dt.rptFishHoghoghi_Motalebat2.Clear();
                        for (int i = 19; i < dt.spr_Pay_RptFishHoghoghi_Motalebat.Count; i++)
                        {
                            if (dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh != 0)
                            {
                                DataRow a = dt.rptFishHoghoghi_Motalebat2.NewRow();
                                a["fldItemsHoghughiTitle"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldTitle;
                                a["fldMablagh"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh;
                                a["fldPersonalId"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldPersonalId;
                                a["radif"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldId;
                                dt.rptFishHoghoghi_Motalebat2.Rows.Add(a);
                            }
                        }
                        EnablePartialRendering = "false";
                        FastReport.Report Report = new FastReport.Report();
                        if (TypeFish == 1)
                            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Fish.frx");
                        else if (TypeFish == 2)
                            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Fish1.frx");
                        else
                            Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Fish2.frx");
                        Report.RegisterData(dt, "rasaFMSPayRoll");
                        Report.RegisterData(dt_Com, "rasaFMSCommon");
                        Report.SetParameterValue("Message", "");
                        Report.Prepare();
                        FastReport.Export.Image.ImageExport export = new FastReport.Export.Image.ImageExport();
                        export.ImageFormat = FastReport.Export.Image.ImageExportFormat.Jpeg;
                        export.Resolution = 96;
                        export.JpegQuality = 100;
                        export.SeparateFiles = true;
                        MemoryStream stream = new MemoryStream();
                        Report.Export(export, stream);


                        //Add the node to the document.
                        XDoc.AppendChild(XElemRoot);
                        var pers = PayServic.GetPayPersonalInfoDetail((int)PersonalId, OrganId, IP, out ErrPay);
                        var ss = PersoneliService.GetPersoneliInfoDetail(pers.fldPrs_PersonalInfoId, OrganId, IP, out ErrPersoneli);

                        XmlElement Xsource = XDoc.CreateElement("Fish");
                        Xsource.SetAttribute("Code", pers.fldId.ToString());
                        Xsource.SetAttribute("Name", pers.fldNameEmployee);
                        Xsource.SetAttribute("Family", pers.fldFamily);
                        Xsource.SetAttribute("NaFather", pers.fldFatherName);
                        Xsource.SetAttribute("CodeMeli", pers.fldCodemeli);
                        Xsource.SetAttribute("Pass", pers.fldCodemeli);
                        Xsource.SetAttribute("Mobile", ss.fldMobile);
                        Xsource.SetAttribute("Mah", Month.ToString());
                        Xsource.SetAttribute("Sal", Year.ToString());
                        Xsource.SetAttribute("Pic", ImageToBase64(stream));
                        XElemRoot.AppendChild(Xsource);
                    }
                    XDoc.Save(outstream);
                    var filename = "GroupFishHoghoughi" + Year.ToString().Substring(2, 2) + Month.ToString().PadLeft(2, '0') + ".xml";
                    XmlTextWriter writer = new XmlTextWriter(outstream, System.Text.Encoding.UTF8);
                    //XDoc.WriteTo(writer);
                    writer.Flush();
                    Response.Clear();
                    byte[] byteArray = outstream.ToArray();
                    Response.AppendHeader("Content-Disposition", "filename=" + filename);
                    Response.AppendHeader("Content-Length", byteArray.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.BinaryWrite(byteArray);
                    writer.Close();
                }
                else
                {
                    FishHoghoghi.Fill(dt.spr_Pay_rptFishHoghoghi, "fldPersonalId", PersonalId, nobatPardakht, Year, Month, OrganId, Convert.ToInt32(Session["UserId"]));
                    FishHoghoghi_Motalebat.Fill(dt.spr_Pay_RptFishHoghoghi_Motalebat, PersonalId, nobatPardakht, Year, Month, Convert.ToInt32(Session["UserId"]));
                    FishHoghoghi_KosuratParam_Bank.Fill(dt.spr_Pay_RptFishHoghoghi_KosuratParam_Bank, Year, Month, nobatPardakht, PersonalId, OrganId, Convert.ToInt32(Session["UserId"]));
                    Tasvir.Fill(dt.spr_Pay_RptTasvirShakh, PersonalId, OrganId);

                    var count = dt.spr_Pay_RptFishHoghoghi_Motalebat.Count;
                    if (count > 19)
                        count = 19;
                    //dt.rptFishHoghoghi_Motalebat1.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        if (dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh != 0)
                        {
                            DataRow a = dt.rptFishHoghoghi_Motalebat1.NewRow();
                            a["fldItemsHoghughiTitle"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldTitle;
                            a["fldMablagh"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh;
                            a["fldPersonalId"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldPersonalId;
                            a["radif"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldId;
                            dt.rptFishHoghoghi_Motalebat1.Rows.Add(a);
                        }
                    }
                    //if (dt.spr_Pay_RptFishHoghoghi_Motalebat.Count >= 19)
                    //dt.rptFishHoghoghi_Motalebat2.Clear();
                    for (int i = 19; i < dt.spr_Pay_RptFishHoghoghi_Motalebat.Count; i++)
                    {
                        if (dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh != 0)
                        {
                            DataRow a = dt.rptFishHoghoghi_Motalebat2.NewRow();
                            a["fldItemsHoghughiTitle"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldTitle;
                            a["fldMablagh"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldMablagh;
                            a["fldPersonalId"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldPersonalId;
                            a["radif"] = dt.spr_Pay_RptFishHoghoghi_Motalebat[i].fldId;
                            dt.rptFishHoghoghi_Motalebat2.Rows.Add(a);
                        }
                    }
                    EnablePartialRendering = "false";
                    FastReport.Report Report = new FastReport.Report();
                    if (TypeFish == 1)
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Fish.frx");
                    else if (TypeFish == 2)
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Fish1.frx");
                    else
                        Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Fish2.frx");
                    Report.RegisterData(dt, "rasaFMSPayRoll");
                    Report.RegisterData(dt_Com, "rasaFMSCommon");
                    Report.SetParameterValue("Message", "");
                    Report.Prepare();
                    FastReport.Export.Image.ImageExport export = new FastReport.Export.Image.ImageExport();
                    export.ImageFormat = FastReport.Export.Image.ImageExportFormat.Jpeg;
                    export.Resolution = 96;
                    export.JpegQuality = 100;
                    export.SeparateFiles = true;
                    MemoryStream stream = new MemoryStream();
                    Report.Export(export, stream);

                    //تولید xml
                    XmlDocument XDoc = new XmlDocument();
                    // Create root node.
                    XmlElement XElemRoot = XDoc.CreateElement("FishContent");
                    XElemRoot.SetAttribute("count", "1");
                    XElemRoot.SetAttribute("type", "0");


                    //Add the node to the document.
                    XDoc.AppendChild(XElemRoot);
                    var pers = PayServic.GetPayPersonalInfoDetail((int)PersonalId, OrganId, IP, out ErrPay);
                    var ss = PersoneliService.GetPersoneliInfoDetail(pers.fldPrs_PersonalInfoId, OrganId, IP, out ErrPersoneli);

                    XmlElement Xsource = XDoc.CreateElement("Fish");
                    Xsource.SetAttribute("Code", pers.fldId.ToString());
                    Xsource.SetAttribute("Name", pers.fldNameEmployee);
                    Xsource.SetAttribute("Family", pers.fldFamily);
                    Xsource.SetAttribute("NaFather", pers.fldFatherName);
                    Xsource.SetAttribute("CodeMeli", pers.fldCodemeli);
                    Xsource.SetAttribute("Mobile", ss.fldMobile);
                    Xsource.SetAttribute("Pass", pers.fldCodemeli);
                    Xsource.SetAttribute("Mah", Month.ToString());
                    Xsource.SetAttribute("Sal", Year.ToString());
                    Xsource.SetAttribute("Pic", ImageToBase64(stream));
                    XElemRoot.AppendChild(Xsource);

                    XDoc.Save(outstream);
                    var filename = "FishHoghoughi" + Year.ToString().Substring(2, 2) + Month.ToString().PadLeft(2, '0') + ".xml";
                    XmlTextWriter writer = new XmlTextWriter(outstream, System.Text.Encoding.UTF8);
                    //XDoc.WriteTo(writer);
                    writer.Flush();
                    Response.Clear();
                    byte[] byteArray = outstream.ToArray();
                    Response.AppendHeader("Content-Disposition", "filename=" + filename);
                    Response.AppendHeader("Content-Length", byteArray.Length.ToString());
                    Response.ContentType = "application/octet-stream";
                    Response.BinaryWrite(byteArray);
                    writer.Close();
                }
                //return File(outstream.ToArray(), "application/xml");
            }
            catch (Exception x)
            {
                return;
            }
        }
        public ActionResult GenerateExcelFishHoghughi(int OrganId, short Year, byte Month, byte nobatPardakht,  int? PersonalId,  string FieldName
            )
        {
           
                
              
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            try
            {
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA" };
                int index = 0;

                var q = PayServic.GetSelectVariziForBank(FieldName,"", Year, Month, nobatPardakht, 0, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
                string Checked = "Title;CodeMeli;NobatPardakht;Year;Month;KhalesPardakhti;ShomareHesab;";
                var ListExcel = Checked.Split(';');

                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                var Check = "";
                var Name = "";
                string title = "پرداخت حقوق بصورت علی الحساب";
                foreach (var item in ListExcel)
                {

                    switch (item)
                    {
                        case "Title":
                            Check = "عنوان";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int ii = 0;
                            foreach (var _item in q)
                            {
                                Name =title;
                                Cell Cell = sheet.Cells[alpha[index] + (ii + 2)];
                                Cell.PutValue(Name);
                                ii++;
                            }
                            index++;
                            break;
                        case "CodeMeli":
                            Check = "کدملی";
                            Cell cell1 = sheet.Cells[alpha[index] + "1"];
                            cell1.PutValue(Check);
                            int ii1 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldCodemeli;
                                Cell Cell = sheet.Cells[alpha[index] + (ii1 + 2)];
                                Cell.PutValue(Name);
                                ii1++;
                            }
                            index++;
                            break;
                        case "KhalesPardakhti":
                            Check = "مبلغ پرداختی";
                            Cell cell2 = sheet.Cells[alpha[index] + "1"];
                            cell2.PutValue(Check);
                            int ii2 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldkhalesPardakhti.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii2 + 2)];
                                Cell.PutValue(Name);
                                ii2++;
                            }
                            index++;
                            break;
                        case "Year":
                            Check = "سال";
                            Cell cell3 = sheet.Cells[alpha[index] + "1"];
                            cell3.PutValue(Check);
                            int ii3 = 0;
                            foreach (var _item in q)
                            {
                                Name = Year.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii3 + 2)];
                                Cell.PutValue(Name);
                                ii3++;
                            }
                            index++;
                            break;
                        case "Month":
                            Check = "ماه";
                            Cell cell4 = sheet.Cells[alpha[index] + "1"];
                            cell4.PutValue(Check);
                            int ii4 = 0;
                            foreach (var _item in q)
                            {
                                Name = Month.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii4 + 2)];
                                Cell.PutValue(Name);
                                ii4++;
                            }
                            index++;
                            break;
                        case "NobatPardakht":
                            Check = "نوبت پرداخت";
                            Cell cell5 = sheet.Cells[alpha[index] + "1"];
                            cell5.PutValue(Check);
                            int ii5 = 0;
                            foreach (var _item in q)
                            {
                                Name = nobatPardakht.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii5 + 2)];
                                Cell.PutValue(Name);
                                ii5++;
                            }
                            index++;
                            break;
                        case "ShomareHesab":
                            Check = "شماره حساب";
                            Cell cell7 = sheet.Cells[alpha[index] + "1"];
                            cell7.PutValue(Check);
                            int ii7 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldShomareHesab.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii7 + 2)];
                                Cell.PutValue(Name);
                                ii7++;
                            }
                            index++;
                            break;

                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "xls", "فایل حقوق بصورت علی الحساب " + Year + Month + ".xls");

            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GenerateExcelHoghughOnAccount(int OrganId, short Year, byte Month, byte nobatPardakht
            )
        {



            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            try
            {
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA" };
                int index = 0;

                var q = PayServic.GetSelectHoghogh_OnAccount (Year, Month, nobatPardakht,  OrganId, IP, out ErrPay).ToList();
                string Checked = "Name;CodeMeli;NobatPardakht;Year;Month;KhalesPardakhti;OnAccount;Type;";
                var ListExcel = Checked.Split(';');

                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                var Check = "";
                var Name = "";
               
                foreach (var item in ListExcel)
                {

                    switch (item)
                    {
                        case "Name":
                            Check = "نام و نام خانوادگی";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int ii = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldName+"_"+_item.fldFamily+"("+_item.fldFatherName+")";
                                Cell Cell = sheet.Cells[alpha[index] + (ii + 2)];
                                Cell.PutValue(Name);
                                ii++;
                            }
                            index++;
                            break;
                        case "CodeMeli":
                            Check = "کدملی";
                            Cell cell1 = sheet.Cells[alpha[index] + "1"];
                            cell1.PutValue(Check);
                            int ii1 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldCodemeli;
                                Cell Cell = sheet.Cells[alpha[index] + (ii1 + 2)];
                                Cell.PutValue(Name);
                                ii1++;
                            }
                            index++;
                            break;
                        case "KhalesPardakhti":
                            Check = "خالص پرداختی";
                            Cell cell2 = sheet.Cells[alpha[index] + "1"];
                            cell2.PutValue(Check);
                            int ii2 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldkhalesPardakhti.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii2 + 2)];
                                Cell.PutValue(Name);
                                ii2++;
                            }
                            index++;
                            break;
                        case "OnAccount":
                            Check = "پرداخت علی الحساب";
                            Cell cell5 = sheet.Cells[alpha[index] + "1"];
                            cell5.PutValue(Check);
                            int ii5 = 0;
                            foreach (var _item in q)
                            {
                                var onAc = _item.fldOnAccount;
                                Cell Cell = sheet.Cells[alpha[index] + (ii5 + 2)];
                                Cell.PutValue(onAc);
                                ii5++;
                            }
                            index++;
                            break;
                        case "Year":
                            Check = "سال";
                            Cell cell3 = sheet.Cells[alpha[index] + "1"];
                            cell3.PutValue(Check);
                            int ii3 = 0;
                            foreach (var _item in q)
                            {
                                
                                Cell Cell = sheet.Cells[alpha[index] + (ii3 + 2)];
                                Cell.PutValue(Year);
                                ii3++;
                            }
                            index++;
                            break;
                        case "Month":
                            Check = "ماه";
                            Cell cell6 = sheet.Cells[alpha[index] + "1"];
                            cell6.PutValue(Check);
                            int ii6 = 0;
                            foreach (var _item in q)
                            {
                                
                                Cell Cell = sheet.Cells[alpha[index] + (ii6 + 2)];
                                Cell.PutValue(Month);
                                ii6++;
                            }
                            index++;
                            break;
                        case "Type":
                            Check = "نوع";
                            Cell cell4 = sheet.Cells[alpha[index] + "1"];
                            cell4.PutValue(Check);
                            int ii4 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldType;
                                Cell Cell = sheet.Cells[alpha[index] + (ii4 + 2)];
                                Cell.PutValue(Name);
                                ii4++;
                            }
                            index++;
                            break;
                        
                       

                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "xls", "فایل مابه التفاوت علی الحساب و حقوق " + Year + Month + ".xls");

            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GenerateExcelBonKartFishHoghughi(int OrganId, short Year, byte Month, byte nobatPardakht, int? PersonalId, string FieldName
            )
        {



            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            try
            {
                string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", "CA" };
                int index = 0;

                var q = PayServic.GetSelectVariziForBank(FieldName,"", Year, Month, nobatPardakht, 0, Convert.ToInt32(Session["OrganId"]), IP, out ErrPay).ToList();
                string Checked = "Title;CodeMeli;NobatPardakht;Year;Month;KhalesPardakhti;ShomareHesab;";
                var ListExcel = Checked.Split(';');

                Workbook wb = new Workbook();
                Worksheet sheet = wb.Worksheets[0];
                var Check = "";
                var Name = "";
                string title = "پرداخت بن کارت بصورت علی الحساب";
                foreach (var item in ListExcel)
                {

                    switch (item)
                    {
                        case "Title":
                            Check = "عنوان";
                            Cell cell = sheet.Cells[alpha[index] + "1"];
                            cell.PutValue(Check);
                            int ii = 0;
                            foreach (var _item in q)
                            {
                                Name = title;
                                Cell Cell = sheet.Cells[alpha[index] + (ii + 2)];
                                Cell.PutValue(Name);
                                ii++;
                            }
                            index++;
                            break;
                        case "CodeMeli":
                            Check = "کدملی";
                            Cell cell1 = sheet.Cells[alpha[index] + "1"];
                            cell1.PutValue(Check);
                            int ii1 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldCodemeli;
                                Cell Cell = sheet.Cells[alpha[index] + (ii1 + 2)];
                                Cell.PutValue(Name);
                                ii1++;
                            }
                            index++;
                            break;
                        case "KhalesPardakhti":
                            Check = "مبلغ پرداختی";
                            Cell cell2 = sheet.Cells[alpha[index] + "1"];
                            cell2.PutValue(Check);
                            int ii2 = 0;
                            foreach (var _item in q)
                            {
                                Name = _item.fldkhalesPardakhti.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii2 + 2)];
                                Cell.PutValue(Name);
                                ii2++;
                            }
                            index++;
                            break;
                        case "Year":
                            Check = "سال";
                            Cell cell3 = sheet.Cells[alpha[index] + "1"];
                            cell3.PutValue(Check);
                            int ii3 = 0;
                            foreach (var _item in q)
                            {
                                Name = Year.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii3 + 2)];
                                Cell.PutValue(Name);
                                ii3++;
                            }
                            index++;
                            break;
                        case "Month":
                            Check = "ماه";
                            Cell cell4 = sheet.Cells[alpha[index] + "1"];
                            cell4.PutValue(Check);
                            int ii4 = 0;
                            foreach (var _item in q)
                            {
                                Name = Month.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii4 + 2)];
                                Cell.PutValue(Name);
                                ii4++;
                            }
                            index++;
                            break;
                        case "NobatPardakht":
                            Check = "نوبت پرداخت";
                            Cell cell5 = sheet.Cells[alpha[index] + "1"];
                            cell5.PutValue(Check);
                            int ii5 = 0;
                            foreach (var _item in q)
                            {
                                Name = nobatPardakht.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii5 + 2)];
                                Cell.PutValue(Name);
                                ii5++;
                            }
                            index++;
                            break;
                        case "ShomareHesab":
                            Check = "شماره حساب";
                            Cell cell7 = sheet.Cells[alpha[index] + "1"];
                            cell7.PutValue(Check);
                            int ii7 = 0;
                            foreach (var _item in q)
                            {
                                Name = nobatPardakht.ToString();
                                Cell Cell = sheet.Cells[alpha[index] + (ii7 + 2)];
                                Cell.PutValue(Name);
                                ii7++;
                            }
                            index++;
                            break;

                    }
                }
                MemoryStream stream = new MemoryStream();
                wb.Save(stream, SaveFormat.Excel97To2003);
                return File(stream.ToArray(), "xls", "فایل حقوق بصورت علی الحساب " + Year + Month + ".xls");

            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        public string ImageToBase64(MemoryStream FileStream)
        {
            // Convert FileStream to byte[]
            byte[] imageBytes = FileStream.ToArray();

            // Convert byte[] to Base64 String
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;

        }
        public string EnablePartialRendering { get; set; }


        public ActionResult GeneratePDFPusheshTakmili(int OrganId, short Year, int ChartId, int CostCenterId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                //var d = servic.GetDate(IP, out Err).fldDateTime;
                //var userid = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault().fldId;
                //serial = userid.ToString() + d.Year.ToString() + d.Month.ToString()+d.Day.ToString()+d.Hour.ToString()+d.Minute.ToString() + d.Second.ToString() + d.Millisecond.ToString() + IP.Replace(":","").Replace(".","");


                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

                DataSet.DataSet1TableAdapters.spr_RptAfradTahtePoosheshForBimeTakmiliTableAdapter bime = new DataSet.DataSet1TableAdapters.spr_RptAfradTahtePoosheshForBimeTakmiliTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter Logo = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter FileS = new DataSet.DataSet_ComTableAdapters.spr_tblFileSelectTableAdapter();
                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                var User = servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
                Logo.Fill(dt_Com.spr_LogoReportWithUserId, OrganId);
                var Setting = PayServic.GetSettingWithFilter("fldOrganId", OrganId.ToString(), 0, 0, IP, out ErrPay).FirstOrDefault();
                int FileId = servic.GetBankDetail(Convert.ToInt32(Setting.fldH_BankFixId), IP, out Err).fldFileId;
                if (Setting.fldShowBankLogo == true)
                    FileS.Fill(dt_Com.spr_tblFileSelect, "fldId", FileId.ToString(), 1);

                bime.Fill(dt.spr_RptAfradTahtePoosheshForBimeTakmili, Year, CostCenterId, ChartId, OrganId);

               

                EnablePartialRendering = "false";
                FastReport.Report Report = new FastReport.Report();
                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\PusheshBimeTakmili.frx");

                Report.RegisterData(dt, "rasaFMSPayRoll");
                Report.RegisterData(dt_Com, "rasaFMSCommon");
                Report.SetParameterValue("Message", "");
                Report.SetParameterValue("sal", Year);
                Report.SetParameterValue("NameUser", User.fldNameEmployee);
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
    }
}
