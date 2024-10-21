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

namespace NewFMS.Areas.Daramad
{
    public class PSPModelController : Controller
    {
        //
        // GET: /Daramad/PSPModel/

        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();
        WCF_Common.CommonService servic = new WCF_Common.CommonService();

        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        public ActionResult IndexNew(string containerId)
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
        public ActionResult ShomareHesab(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.PSPModelId = id;
                return PartialView;
            }
        }
        public ActionResult GetPSP()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicD.GetPspWithFilter("", "", 0,IP, out ErrD).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult Save(WCF_Daramad.OBJ_PspModel psp)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                if (psp.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    Msg = servicD.InsertPspModel(psp.fldPspId, psp.fldModel, psp.fldMultiHesab, psp.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                }
                else
                {//ویرایش
                    MsgTitle = "ویرایش موفق";
                    if (psp.fldMultiHesab == false)
                    {
                        servicD.DeletePSPModel_ShomareHesab(psp.fldId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                        if (ErrD.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = ErrD.ErrorMsg;
                            Er = 1;
                        }
                    }
                    Msg = servicD.UpdatePspModel(psp.fldId, psp.fldPspId, psp.fldModel, psp.fldMultiHesab, psp.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                }
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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
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
                Msg = servicD.DeletePspModel(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
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
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicD.GetPspModelDetail(Id, IP, out ErrD);
            string MultiHesab = "0";
            if (q.fldMultiHesab)
                MultiHesab = "1";
            return Json(new
            {
                fldId = q.fldId,
                fldModel = q.fldModel,
                fldDesc = q.fldDesc,
                fldPspId = q.fldPspId.ToString(),
                fldMultiHesab = MultiHesab
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_PspModel> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_PspModel> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                        case "fldModel":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldModel";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                        case "fldMultiHesabName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMultiHesabName";
                            break;
                    }
                    if (data != null)
                        data1 = servicD.GetPspModelWithFilter(field, searchtext, 100, IP, out ErrD).ToList();
                    else
                        data = servicD.GetPspModelWithFilter(field, searchtext, 100, IP, out ErrD).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicD.GetPspModelWithFilter("", "", 100, IP, out ErrD).ToList();
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

            List<WCF_Daramad.OBJ_PspModel> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadShomareHesabPSP(StoreRequestParameters parameters, string PSPModelId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_PSPModel_ShomareHesab> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_PSPModel_ShomareHesab> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                    }
                    if (data != null)
                        data1 = servicD.GetPSPModel_ShomareHesabWithFilter(field, searchtext, 100, IP, out ErrD).ToList();
                    else
                        data = servicD.GetPSPModel_ShomareHesabWithFilter(field, searchtext, 100, IP, out ErrD).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicD.GetPSPModel_ShomareHesabWithFilter("fldPSPModelId", PSPModelId, 100, IP, out ErrD).ToList();
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

            List<WCF_Daramad.OBJ_PSPModel_ShomareHesab> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveShomareHesabPSP(List<WCF_Daramad.OBJ_PSPModel_ShomareHesab> ShHesabs, int fldPSPModelId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                servicD.DeletePSPModel_ShomareHesab(fldPSPModelId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                if (ErrD.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrD.ErrorMsg;
                    Er = 1;
                }
                else
                {
                    foreach (var item in ShHesabs)
                    {
                        MsgTitle = "ذخیره موفق";
                        Msg = servicD.InsertPSPModel_ShomareHesab(item.fldPSPModelId, item.fldShHesabId, item.fldOrder, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                     
                        if (ErrD.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = ErrD.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = Er
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}