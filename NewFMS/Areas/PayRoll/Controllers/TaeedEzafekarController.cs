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

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class TaeedEzafekarController : Controller
    {
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();

        WCF_Personeli.PersoneliService PersoneliService = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError ErrPersoneli = new WCF_Personeli.ClsError();

        WCF_Common_Pay.Comon_PayService Comon_Pay = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError ErrC_P = new WCF_Common_Pay.ClsError();
        //
        // GET: /PayRoll/TaeedEzafekar/
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
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
            result.ViewBag.CostCenter = NewFMS.Models.CurrentDate.CostCenter;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult IndexShow(string containerId)
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
        public ActionResult HelpShowEzafekari()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetCostCenter()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetCostCenterWithFilter("", "", (Session["OrganId"]).ToString(), 0, IP, out ErrPay).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetChartOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = PayServic.GetSpecialPermissionWithFilter("fldUserSelectId", Session["UserId"].ToString(), 0, IP, out ErrPay).Where(l=>l.fldOpertionId==2).ToList().Select(c => new { fldId = c.fldChartOrganId, fldName = c.fldTitleChart });
            return this.Store(q);
        }

        public ActionResult Read(int CostCenterId, short Yearr, byte Monthh,byte NobatPardakht, StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_SelectPayPersonalInfo_Ezafekar> data = null;
            data = PayServic.SelectPayPersonalInfo_Ezafekar("CostCenter",CostCenterId, Convert.ToInt32(Session["OrganId"]), Yearr, Monthh, 0,IP, out ErrPay).ToList();

            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_PayRoll.OBJ_SelectPayPersonalInfo_Ezafekar> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadShow(int ChartOrganId, short Yearr, byte Monthh, byte NobatPardakht, StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_SelectPayPersonalInfo_Ezafekar> data = null;
            data = PayServic.SelectPayPersonalInfo_Ezafekar("ChartOrgan", ChartOrganId, Convert.ToInt32(Session["OrganId"]), Yearr, Monthh, 0,IP, out ErrPay).ToList();

            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_PayRoll.OBJ_SelectPayPersonalInfo_Ezafekar> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            return this.Store(rangeData, data.Count);
        }

        public ActionResult Save(short Year, byte Month,byte NobatePardakht, List<WCF_PayRoll.OBJ_KarKardeMahane> KarKardeMahane)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; byte Er = 0;
            try
            {
                var q = PayServic.GetKarKardeMahaneWithFilter("fldNobatePardakht_YM", NobatePardakht.ToString(), Year.ToString(), Month.ToString(), 0, 0, Convert.ToInt32(Session["OrganId"]), 0,IP, out ErrPay).FirstOrDefault();
                if (q.fldFlag == true)
                {
                    return Json(new
                    {
                        Msg = " حقوق ماه جاری بسته شده است و شما مجاز به تغییر اطلاعات نمی باشید.",
                        MsgTitle = "اخطار"
                    }, JsonRequestBehavior.AllowGet);
                }
                foreach (var item in KarKardeMahane)
                {
                    Msg = PayServic.UpdateEzafekarKarkardMahane(item.fldEzafeKari, item.fldGhati, item.fldPrs_PersonalInfoId,Month,Year, Session["Username"].ToString(), Session["Password"].ToString(), IP, out ErrPay);
                    MsgTitle="عملیات موفق";
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
