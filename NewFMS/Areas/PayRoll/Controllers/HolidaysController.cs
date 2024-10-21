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
using System.Globalization;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class HolidaysController : Controller
    {
        // GET: PayRoll/Holidays
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService ComServic = new WCF_Common.CommonService();
        WCF_Personeli.PersoneliService PersonalServic = new WCF_Personeli.PersoneliService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError ErrCom = new WCF_Common.ClsError();
        WCF_Personeli.ClsError ErrPersonal = new WCF_Personeli.ClsError();
        public ActionResult Index(string containerId)
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
            result.ViewBag.CurYear = ComServic.GetDate(IP, out ErrCom).fldTarikh.Substring(0, 4);
            return result;
        }
        //public ActionResult Help()
        //{//باز شدن پنجره
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    else
        //    {
        //        Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
        //        return PartialView;
        //    }
        //}
        public ActionResult GetMonasebat()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetMonasebatWithFilter("", "", "", false, 0, IP, out ErrPay).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldNameMonasebat, fldMazaya=c.fldMazaya });
            return this.Store(q);
        }
        public ActionResult setMonasebat(short Year, byte Month, byte Day, string Id, byte MonasebatId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Year = Year;
            result.ViewBag.Month = Month.ToString().PadLeft(2, '0');
            result.ViewBag.Day = Day.ToString().PadLeft(2, '0');
            result.ViewBag.Id = Id;
            result.ViewBag.MonasebatId = MonasebatId;
            return result;
        }
        public ActionResult GetYears()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            short fldsal =Convert.ToInt16(ComServic.GetDate(IP, out ErrCom).fldTarikh.Substring(0, 4));
            List<Models.CurrentDate> Years = new List<Models.CurrentDate>();
            for (int i = fldsal; i < fldsal + 10; i++)
            {
                Models.CurrentDate Sal = new Models.CurrentDate();
                Sal.fldYear = i;
                Years.Add(Sal);
            }
            return Json(Years, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetInfMonth(short Year, byte month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<string> DayMon = new List<string>();
            List<string> MonasebatDesc = new List<string>();
            List<byte> MonasebatIds = new List<byte>();
            List<bool> HasMazaya = new List<bool>();

            var holidays = PayServic.GetMonasebatTarikhWithFilter("DateShamsi", Year.ToString(), month.ToString(), "", "", 0, IP, out ErrPay).ToList();
            if (holidays.Count == 0)
            {
                var q = PayServic.GetMonasebatWithFilter("DateShamsi", Year.ToString(),"",false,0,IP,out ErrPay).Where(l => l.fldTarikh.Split('/')[1].TrimStart('0') == month.ToString()).ToList();
                for (var i = 0; i < q.Count; i++)
                {
                    DayMon.Add(q[i].fldTarikh.Split('/')[2].TrimStart('0'));
                    MonasebatDesc.Add(q[i].fldNameMonasebat);
                    MonasebatIds.Add(q[i].fldId);
                    HasMazaya.Add(q[i].fldMazaya);
                }
            }
            else
            {
                var q = holidays.Where(l => l.fldMonth == month).ToList();
                foreach (var item in holidays)
                {
                    DayMon.Add(item.fldDay.ToString());
                    MonasebatDesc.Add(item.fldNameMonasebat);
                    MonasebatIds.Add(item.fldMonasebatId);
                    HasMazaya.Add(item.fldMazaya);
                }
            }
            //if (holidays.Count > 0)
            //{
            //    foreach (var item in holidays)
            //    {
            //        DayMon.Add(item.fldDay.ToString());
            //        MonasebatDesc.Add(item.fldNameMonasebat);
            //        MonasebatIds.Add(item.fldMonasebatId);
            //    }
            //}
            //if (holidays.Count == 0)
            //{
            //    var q = m.prs_tblPickSelect("DateShamsi", Year.ToString(), 0).Where(l => l.fldTarikh.Split('/')[1].TrimStart('0') == month.ToString()).ToList();
            //    for (var i = 0; i < q.Count; i++)
            //    {
            //        DayMon.Add(q[i].fldTarikh.Split('/')[2].TrimStart('0'));
            //        PikType.Add(Convert.ToByte(q[i].fldTypePick).ToString());
            //        PikDesc.Add(q[i].fldDescPick);
            //    }
            //}
            //else
            //{
            //    var q = holidays.Where(l => l.fldTarikh.Split('/')[1].TrimStart('0') == month.ToString()).ToList();
            //    for (var i = 0; i < q.Count; i++)
            //    {
            //        DayMon.Add(q[i].fldTarikh.Split('/')[2].TrimStart('0'));
            //        PikType.Add(Convert.ToByte(q[i].fldTypePick).ToString());
            //        PikDesc.Add(q[i].fldDescPick);
            //    }
            //}

            PersianCalendar pc = new PersianCalendar();
            var cmonth = pc.GetMonth(DateTime.Now);
            var day = pc.GetDayOfMonth(DateTime.Now);
            var IsKabise = MyLib.Shamsi.Iskabise(Year);
            var tempDate = MyLib.Shamsi.Shamsi2miladiDateTime(Year.ToString() + "/" + month.ToString().PadLeft(2, '0') + "/01");
            var tempweekday = 0;
            if ((int)pc.GetDayOfWeek(tempDate) != 6)
            {
                tempweekday = ((int)pc.GetDayOfWeek(tempDate)) + 1;
            }

            return Json(new
            {
                cmonth = cmonth,
                day = day,
                IsKabise = IsKabise,
                tempweekday = tempweekday,
                MonasebatDesc = MonasebatDesc,
                MonasebatIds=MonasebatIds,
                HasMazaya=HasMazaya,
                DayMon = DayMon
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(List<byte> MonasebatIds, List<byte> Months, List<byte> Days, short year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MsgTitle = "ذخیره موفق";
                //حذف با فیلد نیم سال
                PayServic.DeleteMonasebatTarikh(year, Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                if (ErrPay.ErrorType)
                {
                    return Json(new
                    {
                        Msg = ErrPay.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                //ذخیره
                for (int i = 0; i < Months.Count; i++)
                {
                    
                    Msg=PayServic.InsertMonasebatTarikh(year, Months[i], Days[i], MonasebatIds[i], Session["Username"].ToString(), Session["Password"].ToString(),
                        Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
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
    }
}