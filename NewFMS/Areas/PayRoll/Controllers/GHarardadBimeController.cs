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
    public class GHarardadBimeController : Controller
    {
        //
        // GET: /PayRoll/GHarardadBime/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();

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
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(WCF_PayRoll.OBJ_GHarardadBime GHarardadBime)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (GHarardadBime.fldDesc == null)
                    GHarardadBime.fldDesc = "";

                if (GHarardadBime.fldId == 0)
                { //ذخیره
                    Msg = servic.InsertGHarardadBime(GHarardadBime.fldNameBime, GHarardadBime.fldTarikhSHoro,GHarardadBime.fldTarikhPayan, GHarardadBime.fldMablagheBimeSHodeAsli, 
                        GHarardadBime.fldMablaghe60Sal,GHarardadBime.fldMablaghe70Sal, GHarardadBime.fldMablagheBimeOmr, GHarardadBime.fldMaxBimeAsli, GHarardadBime.fldDarsadBimeOmr,
                        GHarardadBime.fldDarsadBimeTakmily, GHarardadBime.fldDarsadBime60Sal, GHarardadBime.fldDarsadBime70Sal, GHarardadBime.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ذخیره موفق";
                }
                else
                { //ویرایش
                    Msg = servic.UpdateGHarardadBime(GHarardadBime.fldId, GHarardadBime.fldNameBime, GHarardadBime.fldTarikhSHoro,GHarardadBime.fldTarikhPayan, GHarardadBime.fldMablagheBimeSHodeAsli,
                        GHarardadBime.fldMablaghe60Sal,GHarardadBime.fldMablaghe70Sal, GHarardadBime.fldMablagheBimeOmr, GHarardadBime.fldMaxBimeAsli, GHarardadBime.fldDarsadBimeOmr,
                       GHarardadBime.fldDarsadBimeTakmily, GHarardadBime.fldDarsadBime60Sal, GHarardadBime.fldDarsadBime70Sal, GHarardadBime.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ویرایش موفق";

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
                Err=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                Msg = servic.DeleteGHarardadBime(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Err=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetGHarardadBimeDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldDarsadBime60Sal = q.fldDarsadBime60Sal,
                fldDarsadBime70Sal = q.fldDarsadBime70Sal,
                fldDarsadBimeOmr = q.fldDarsadBimeOmr,
                fldDarsadBimeTakmily = q.fldDarsadBimeTakmily,
                fldMablaghe60Sal = q.fldMablaghe60Sal,
                fldMablaghe70Sal = q.fldMablaghe70Sal,
                fldMablagheBimeOmr = q.fldMablagheBimeOmr,
                fldMablagheBimeSHodeAsli = q.fldMablagheBimeSHodeAsli,
                fldMaxBimeAsli = q.fldMaxBimeAsli.ToString(),
                fldNameBime = q.fldNameBime,
                fldTarikhPayan = q.fldTarikhPayan,
                fldTarikhSHoro = q.fldTarikhSHoro,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_GHarardadBime> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_GHarardadBime> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldNameBime":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameBime";
                            break;
                        case "fldTarikhSHoro":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhSHoro";
                            break;
                        case "fldTarikhPayan":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhPayan";
                            break;
                        case "fldMaxBimeAsli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMaxBimeAsli";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetGHarardadBimeWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetGHarardadBimeWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetGHarardadBimeWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_GHarardadBime> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
