using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class CodeingHesabdariController : Controller
    {
        //
        // GET: /Daramad/CodeingHesabdari/
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        public ActionResult IndexWin(int id, int FiscalYearId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q = servic.GetShomareHesabCodeDaramadWithFilter("fldId", id.ToString(), "",FiscalYearId, 0, IP, out Err).FirstOrDefault();
            PartialView.ViewBag.ShomareHesabCodeDaramadId = id;
            PartialView.ViewBag.DaramadTitle = q.fldDaramadTitle;// +"(" + q.fldDaramadCode + ")";
            return PartialView;
        }
        public ActionResult GetComboBox()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCompanyWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        
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
                Msg = servic.DeleteParametrHesabdari(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
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
            var q = servic.GetParametrHesabdariDetail(Id, IP, out Err);
           
            
            return Json(new
            {
                fldId = q.fldId,
                fldShomareHesabCodeDaramadId = q.fldShomareHesabCodeDaramadId.ToString(),
                fldCodeHesab = q.fldCodeHesab,
                fldCompanyId = q.fldCompanyId,
                fldHesabId = q.fldHesabId,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(WCF_Daramad.OBJ_ParametrHesabdari Param)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Param.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertParametrHesabdari(Param.fldShomareHesabCodeDaramadId,Param.fldCodeHesab,Param.fldHesabId,Param.fldCompanyId,Param.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateParametrHesabdari(Param.fldId, Param.fldShomareHesabCodeDaramadId, Param.fldCodeHesab, Param.fldHesabId, Param.fldCompanyId, Param.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
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
        public ActionResult Read(StoreRequestParameters parameters, int ShomareHesabCodeDaramadId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_ParametrHesabdari> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_ParametrHesabdari> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldDaramadTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDaramadTitle";
                            break;
                        case "fldNameParametreFa":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameParametreFa";
                            break;
                        case "NoeName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "NoeName";
                            break;
                        case "NoeFieldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "NoeFieldName";
                            break;
                        case "VaziyatName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "VaziyatName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetParametrHesabdariWithFilter(field, searchtext, 0, IP, out Err).ToList();
                    else
                        data = servic.GetParametrHesabdariWithFilter(field, searchtext, 0, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetParametrHesabdariWithFilter("fldShomareHesabCodeDaramadId", ShomareHesabCodeDaramadId.ToString(), 0, IP, out Err).ToList();
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

            List<WCF_Daramad.OBJ_ParametrHesabdari> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
