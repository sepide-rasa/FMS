using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.Globalization;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class OccasionsController : Controller
    {
        // GET: PayRoll/Occasions
        WCF_PayRoll.PayRolService PayService = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService ComService = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError PayErr = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError ComErr = new WCF_Common.ClsError();
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
            var DefaultYear=ComService.GetDate(IP,out ComErr).fldTarikh.Substring(0, 4);
            var DefaultMonth = ComService.GetDate(IP, out ComErr).fldTarikh.Substring(5, 2);
            result.ViewBag.DefaultYear = DefaultYear;
            result.ViewBag.DefaultMonth = DefaultMonth;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult occasions(string containerId)
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
            return result;
        }
        public ActionResult GetGroupingType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayService.GetMonasebatTypeWithFilter("", "", 0, IP, out PayErr).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayService.GetMonasebatHeaderDetail(Id,IP,out PayErr);
            return Json(new
            {
                fldId = q.fldId,
                fldYear = q.fldActiveDate.ToString().Substring(0,4),
                fldMonth = q.fldActiveDate.ToString().Substring(4,2),
                fldStatus=Convert.ToByte(q.fldActive).ToString(),
                fldYearD = q.fldDeactiveDate==null?"0":q.fldDeactiveDate.ToString().Substring(0, 4),
                fldMonthD = q.fldDeactiveDate==null?"0":q.fldDeactiveDate.ToString().Substring(4, 2)
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailsOccasions(byte Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayService.GetMonasebatDetail(Id, IP, out PayErr);
            return Json(new
            {
                fldId = q.fldId,
                fldMazaya=Convert.ToByte(q.fldMazaya).ToString(),
                fldDateType=Convert.ToByte(q.fldDateType).ToString(),
                fldHoliday=Convert.ToByte(q.fldHoliday).ToString(),
                fldMonasebatTypeId=q.fldMonasebatTypeId,
                fldNameMonasebat=q.fldNameMonasebat,
                fldMonth=q.fldMonth,
                fldDay=q.fldDay
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = PayService.DeleteMonasebatHeader(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out PayErr);
                if (PayErr.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = PayErr.ErrorMsg;
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
        public ActionResult DeleteOccasions(byte id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = PayService.DeleteMonasebat(id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out PayErr);
                if (PayErr.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = PayErr.ErrorMsg;
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
        public ActionResult Monasebat_Mablagh(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.Occasions();
            var result = new Ext.Net.MVC.PartialViewResult
            {
                ViewData = ViewData
            };
            result.ViewBag.HeaderId = HeaderId;
            return result;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var data = PayService.GetMonasebatHeaderWithFilter("", "", 0, IP, out PayErr).ToList();
            return this.Store(data, data.Count);
        }
        public ActionResult Readoccasions(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var data = PayService.GetMonasebatWithFilter("", "","",false, 0, IP, out PayErr).ToList();
            return this.Store(data, data.Count);
        }
        public ActionResult ReadMonasebat(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var data = PayService.GetMonasebatWithFilter("fldMazaya", "1","",false, 0, IP, out PayErr).ToList();
            return this.Store(data, data.Count);
        }
        public ActionResult ReadMablagh(StoreRequestParameters parameters, int HeaderId, byte MonasebatId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var data = PayService.GetMonasebatMablaghWithFilter("fldHeaderId_MonasebatId", HeaderId.ToString(), MonasebatId.ToString(), "", 0, IP, out PayErr).ToList();
            return this.Store(data, data.Count);
        }
        public ActionResult GetYear(short? Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            var q = ComService.GetDate(IP, out ComErr);
            int fldsal = Year != null ? Convert.ToInt16(Year):Convert.ToInt16(q.fldTarikh.Substring(0, 4));
            for (int i = fldsal; i < fldsal + 2; i++)
            {
                Models.CurrentDate CboSal = new Models.CurrentDate();
                CboSal.fldYear = i;
                sh.Add(CboSal);
            }
            return Json(sh.OrderByDescending(l => l.fldYear), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetYearD(short Year,byte Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();

            PersianCalendar pc = new PersianCalendar();
            var Datee =MyLib.Shamsi.Shamsi2miladiDateTime(Year + "/" + Month.ToString().PadLeft('0', 2) + "/01");
            var ShamsiDate = MyLib.Shamsi.Miladi2ShamsiString(Datee.AddMonths(1));
         
            int fldsal = Convert.ToInt32(ShamsiDate.Substring(0,4));
            for (int i = fldsal; i < fldsal + 2; i++)
            {
                Models.CurrentDate CboSal = new Models.CurrentDate();
                CboSal.fldYear = i;
                sh.Add(CboSal);
            }
            return Json(sh.OrderByDescending(l => l.fldYear), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMonthD(short Year,short YearD, byte Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            PersianCalendar pc = new PersianCalendar();
            var Mnth = "1";
            if (YearD == Year)
            {
                var Datee = MyLib.Shamsi.Shamsi2miladiDateTime(Year + "/" + Month.ToString().PadLeft('0', 2) + "/01");
                Mnth = MyLib.Shamsi.Miladi2ShamsiString(Datee.AddMonths(1)).Substring(5, 2);
            }

            for (int i = Convert.ToByte(Mnth); i < 13; i++)
            {
                Models.CurrentDate ObjMonth = new Models.CurrentDate();
                ObjMonth.fldMonth = i.ToString().PadLeft('0', 2);
                ObjMonth.fldMonthName = MyLib.Shamsi.ShamsiMonthname(i);
                sh.Add(ObjMonth);
            }
            return Json(sh.OrderBy(l => l.fldMonth), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMonth(int? Month)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            var q = ComService.GetDate(IP, out ComErr);
            int fldsal = Convert.ToInt32(q.fldTarikh.Substring(0, 4));
            Month = Month == null ? 1 : Month;
            for (int i = Convert.ToByte(Month); i < 13; i++)
            {
                Models.CurrentDate ObjMonth = new Models.CurrentDate();
                ObjMonth.fldMonth = i.ToString().PadLeft('0', 2);
                ObjMonth.fldMonthName = MyLib.Shamsi.ShamsiMonthname(i);
                sh.Add(ObjMonth);
            }
            return Json(sh.OrderBy(l => l.fldMonth), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(WCF_PayRoll.OBJ_MonasebatHeader Header)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Header.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    Msg = PayService.InsertMonasebatHeader(Header.fldActiveDate, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out PayErr);
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = PayService.UpdateMonasebatHeader(Header.fldId,Header.fldDeactiveDate ,Header.fldActive, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out PayErr);
                }
                if (PayErr.ErrorType)
                {
                    return Json(new
                    {
                        Msg = PayErr.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
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
        public ActionResult SaveOccasions(WCF_PayRoll.OBJ_Monasebat Monasebat)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Monasebat.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    Msg = PayService.InsertMonasebat(Monasebat.fldNameMonasebat,Monasebat.fldMonasebatTypeId,Monasebat.fldMonth,Monasebat.fldDay,Monasebat.fldDateType,
                        Monasebat.fldHoliday,Monasebat.fldMazaya, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out PayErr);
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = PayService.UpdateMonasebat(Monasebat.fldId, Monasebat.fldNameMonasebat, Monasebat.fldMonasebatTypeId, Monasebat.fldMonth, Monasebat.fldDay, Monasebat.fldDateType,
                        Monasebat.fldHoliday, Monasebat.fldMazaya, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out PayErr);
                }
                if (PayErr.ErrorType)
                {
                    return Json(new
                    {
                        Msg = PayErr.ErrorMsg,
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
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
        public ActionResult SaveMablagh(List<WCF_PayRoll.OBJ_MonasebatMablagh> MablaghArray,int HeaderId,byte MonasebatId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                foreach (var item in MablaghArray)
                {
                    if (item.fldId == 0)
                    {//ذخیره

                        MsgTitle = "ذخیره موفق";
                        Msg = PayService.InsertMonasebatMablagh(HeaderId,MonasebatId,item.fldMablagh,item.fldTypeNesbatId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out PayErr);
                    }
                    else
                    {//ویرایش

                        MsgTitle = "ویرایش موفق";
                        Msg = "ویرایش با موفقیت انجام شد.";

                        if (item.fldMablagh == 0)
                        {
                            PayService.DeleteMonasebatMablagh(item.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out PayErr);
                        }
                        else
                        {
                            PayService.UpdateMonasebatMablagh(item.fldId, HeaderId, MonasebatId, item.fldMablagh, item.fldTypeNesbatId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out PayErr);
                        }
                    }
                    if (PayErr.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = PayErr.ErrorMsg,
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