using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using System.IO;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class KarkardMahane_DetailController : Controller
    {
        //
        // GET: /PayRoll/KarkardMahane_Detail/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();

        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.KarkardMahane();
            var q = Cservic.GetDate(IP, out CErr);
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();

            result.ViewBag.Sal = q.fldTarikh.Substring(0, 4);
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult New(int id, int KarkardMahaneId,int OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.KarkardMahaneId = KarkardMahaneId;
            var d = 0;
            var data = servic.GetKarkardMahane_DetailWithFilter("fldKarkardMahaneId", KarkardMahaneId.ToString(), 100, IP, out Err).ToList();
            var data1 = servic.GetKarKardeMahaneWithFilter("fldId", KarkardMahaneId.ToString(), "", "", 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).FirstOrDefault();
            foreach (var item in data)
                d = d + item.fldKarkard;
            var KarkardMande = data1.fldKarkard - d;

            if (id != 0)
            {
                var k = servic.GetKarkardMahane_DetailWithFilter("fldId", id.ToString(), 100, IP, out Err).FirstOrDefault();
                KarkardMande = KarkardMande + k.fldKarkard;
            }

            PartialView.ViewBag.KarkardMande = KarkardMande;
            PartialView.ViewBag.OrganId = OrganId.ToString();

            return PartialView;
        }
        public ActionResult GetSalVorod()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            var q = Cservic.GetDate(IP, out CErr);
            int fldsal = Convert.ToInt32(q.fldTarikh.Substring(0,4)) - 7;
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
            var q = Cservic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out CErr).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult Save(WCF_PayRoll.OBJ_KarkardMahane_Detail Detail,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var KarkardMande = 0; var Er = 0;
            try
            {
                if (Detail.fldDesc == null)
                    Detail.fldDesc = "";
                if (Detail.fldId == 0)
                {
                    //ذخیره
                    Msg = servic.InsertKarkardMahane_Detail(Detail.fldKarkardMahaneId, Detail.fldKarkard, Detail.fldKargahBimeId, Detail.fldDesc,
                        Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                    MsgTitle = "ذخیره موفق";
                }
                else
                {
                    //ویرایش
                    Msg = servic.UpdateKarkardMahane_Detail(Detail.fldId, Detail.fldKarkardMahaneId, Detail.fldKarkard, Detail.fldKargahBimeId, Detail.fldDesc,
                        Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                    MsgTitle = "ویرایش موفق";
                }
                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
                    Er = 1;
                }
                else
                {
                    var d = 0; 
                    var data = servic.GetKarkardMahane_DetailWithFilter("fldKarkardMahaneId", Detail.fldKarkardMahaneId.ToString(), 100, IP, out Err).ToList();
                    var data1 = servic.GetKarKardeMahaneWithFilter("fldId", Detail.fldKarkardMahaneId.ToString(), "", "", 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).FirstOrDefault();
                    foreach (var item in data)
                        d = d + item.fldKarkard;
                    KarkardMande=data1.fldKarkard - d;
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
                KarkardMande = KarkardMande,
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                servic.DeleteKarkardMahane_Detail(id, Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                Msg = "حذف با موفقیت انجام شد.";
                MsgTitle = "حذف موفق";
                if (Err.ErrorType == true)
                {
                    Msg = Err.ErrorMsg;
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadKarkard(string YearId, string MonthId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_PayRoll.OBJ_KarKardeMahane> data = null;
            if (YearId != "null" && MonthId != "null")
            {
                data = servic.GetKarKardeMahaneWithFilter("fldMah_Year", "", YearId, MonthId, 0, 0, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters, string sal, string mah,int OrganId)
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
                            field = "fldId";
                            break;
                        case "fldName_Father":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName_Father_YM";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli_YM";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_Personali_YM";
                            break;
                        case "fldYear":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldYear_YM";
                            break;
                        case "fldMonth":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMonth_YM";
                            break;
                        case "fldNobatePardakhtS":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNobatePardakhtS_YM";
                            break;
                        case "fldKarkard":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldKarkard_YM";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetKarKardeMahaneWithFilter(field, searchtext, sal, mah, 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                    else
                        data = servic.GetKarKardeMahaneWithFilter(field, searchtext, sal, mah, 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetKarKardeMahaneWithFilter("fldMah_Year", "", sal, mah, 0, 0, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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
        public ActionResult Rload(int KarkardMahaneId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_KarkardMahane_Detail> data = null;

            data = servic.GetKarkardMahane_DetailWithFilter("fldKarkardMahaneId", KarkardMahaneId.ToString(), 100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetKarkardMahane_DetailDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldKargahBimeId = q.fldKargahBimeId,
                fldKarkard = q.fldKarkard,
                fldKarkardMahaneId = q.fldKarkardMahaneId,
                fldWorkShopName = q.fldWorkShopName,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
