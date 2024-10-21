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
using System.Globalization;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class DiscountLicenseController : Controller
    {
        //
        // GET: /Daramad/DiscountLicense/

        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();
        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        public ActionResult Index(string containerId, string FiscalYearId)
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
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<WCF_Daramad.OBJ_Takhfif> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_Takhfif> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldShomareMojavez":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareMojavez";
                            break;
                        case "fldTarikhMojavez":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhMojavez";
                            break;
                        case "fldAzTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAzTarikh";
                            break;
                        case "fldTaTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTaTarikh";
                            break;
                        case "fldTakhfifKoli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTakhfifKoli";
                            break;
                        case "fldTakhfifNaghdi":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTakhfifNaghdi";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servicD.GetTakhfifWithFilter(field, searchtext, 100,IP,out ErrD).ToList();
                    else
                        data = servicD.GetTakhfifWithFilter(field, searchtext, 100, IP, out ErrD).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicD.GetTakhfifWithFilter("", "", 100, IP, out ErrD).ToList();
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
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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

            List<WCF_Daramad.OBJ_Takhfif> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult ReadIncomeCodes(int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Daramad.OBJ_ShomareHesabCodeDaramad> data = null;
            data = servicD.GetShomareHesabCodeDaramadWithFilter("fldOrganId", Session["OrganId"].ToString(),"",FiscalYearId, 0, IP, out ErrD).ToList();
            return this.Store(data);
        }

        public ActionResult New(int Id, string FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Id = Id;
            result.ViewBag.FiscalYearId = FiscalYearId;
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<int> IncomeCode = new List<int>();
            var q = servicD.GetTakhfifDetail(Id, IP, out ErrD);
            var q1 = servicD.GetTakhfifDetailWithFilter("fldTakhfifId", Id.ToString(), 0, IP, out ErrD).ToList();
            foreach (var item in q1)
            {
                IncomeCode.Add(item.fldShCodeDaramad);
            }
            return Json(new
            {
                fldId = q.fldId,
                fldShomareMojavez = q.fldShomareMojavez,
                fldTarikhMojavez = q.fldTarikhMojavez,
                fldAzTarikh = q.fldAzTarikh,
                fldTaTarikh = q.fldTaTarikh,
                fldTakhfifKoli = q.fldTakhfifKoli,
                fldTakhfifNaghdi=q.fldTakhfifNaghdi,
                IncomeCode=IncomeCode,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(WCF_Daramad.OBJ_Takhfif Takhfif,List<int>IncomesCode)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;int Id=0;
            try
            {
                if (Takhfif.fldDesc == null)
                    Takhfif.fldDesc = "";
                if (Takhfif.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = "ذخیره با موفقیت انجام شد.";
                    Id = servicD.InsertTakhfif(Takhfif.fldShomareMojavez, Takhfif.fldTarikhMojavez, Takhfif.fldAzTarikh, Takhfif.fldTaTarikh,
                        Takhfif.fldTakhfifKoli, Takhfif.fldTakhfifNaghdi, Takhfif.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrD.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    foreach (var item in IncomesCode)
                    {
                        servicD.InsertTakhfifDetail(Id, item, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    }
                    if (ErrD.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrD.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }                    
                }
                else
                {
                    //ویرایش 
                    Msg = "ویرایش با موفقیت انجام شد.";
                    MsgTitle = "ویرایش موفق";
                    servicD.UpdateTakhfif(Takhfif.fldId, Takhfif.fldShomareMojavez, Takhfif.fldTarikhMojavez, Takhfif.fldAzTarikh, Takhfif.fldTaTarikh,
                        Takhfif.fldTakhfifKoli, Takhfif.fldTakhfifNaghdi, Takhfif.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrD.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    servicD.DeleteTakhfifDetail(Takhfif.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrD.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    foreach (var item in IncomesCode)
                    {
                        servicD.InsertTakhfifDetail(Takhfif.fldId, item, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    }
                    if (ErrD.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrD.ErrorMsg,
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

        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MsgTitle = "حذف موفق";
                Msg = servicD.DeleteTakhfif(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                if (ErrD.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrD.ErrorMsg;
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
    }
}
