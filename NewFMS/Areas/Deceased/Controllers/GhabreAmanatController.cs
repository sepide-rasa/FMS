using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Deceased
{
    public class GhabreAmanatController : Controller
    {
        //
        // GET: /Deceased/GhabreAmanat/
        WCF_Deceased.DeceasedService servic = new WCF_Deceased.DeceasedService();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Deceased.ClsError Err = new WCF_Deceased.ClsError();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        

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
            var k=servic_Com.GetDate(IP, out Err_Com);
            result.ViewBag.TarikhRooz = k.fldTarikh;
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
        public ActionResult GetVadiSalam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetVadiSalamWithFilter("fldOrganId", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Title = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetGhete(int VadiSalamId, string AmanatId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var IdAmanat=0;
            if (AmanatId != "")
                IdAmanat = Convert.ToInt32(AmanatId);
            var q = servic.GetGheteWithFilter("GheteKhali", VadiSalamId.ToString(), IdAmanat, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Title = c.fldNameGhete });
            return this.Store(q);
        }
        public ActionResult GetRadif(int GheteId, string AmanatId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var IdAmanat = 0;
            if (AmanatId != "")
                IdAmanat = Convert.ToInt32(AmanatId);
            var q = servic.GetRadifWithFilter("RadifKhali", GheteId.ToString(), IdAmanat, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Title = c.fldNameRadif });
            return this.Store(q);
        }
        public ActionResult GetShomare(int RadifId, string AmanatId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var IdAmanat = 0;
            if (AmanatId != "")
                IdAmanat = Convert.ToInt32(AmanatId);
            var q = servic.GetShomareWithFilter("ShomareKhali", RadifId.ToString(), IdAmanat, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Title = c.fldShomare });
            return this.Store(q);
        }

        public ActionResult Save(WCF_Deceased.OBJ_GhabreAmanat amanat, string CodeMeli, string CodeMoshakhase, string Name, string Family, string Sh_Sh, string NameFather)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (amanat.fldDesc == null)
                    amanat.fldDesc = "";

                if (CodeMeli == "")
                    CodeMeli = null;
                if (CodeMoshakhase == "")
                    CodeMoshakhase = null;
               
                int empId = servic.InsertEmploye(CodeMeli, CodeMoshakhase, Name, Family, Sh_Sh, NameFather, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                if (amanat.fldId == 0)
                { //ذخیره
                    var q = servic.GetGhabreAmanatWithFilter("fldEmployeeId", empId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
                    if (q != null)
                    {
                        Msg = "برای یک شخص نمی توان بیش از یه قبر به امانت گرفت.";
                        MsgTitle = "اخطار";
                        Er = 1;
                    }
                    else
                    {
                       
                        Msg = servic.InsertGhabreAmanat(amanat.fldShomareId, empId, amanat.fldTarikhRezerv, amanat.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        MsgTitle = "ذخیره موفق";
                    }
                }
                else
                { //ویرایش
                    Msg = servic.UpdateGhabreAmanat(amanat.fldId, amanat.fldShomareId, amanat.fldEmployeeId, amanat.fldTarikhRezerv, amanat.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ویرایش موفق";
                }
                if (Err.ErrorType)
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
                Er = Er
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
                Msg = servic.DeleteGhabreAmanat(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetGhabreAmanatDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            var typeCode = "0";
            var Code_Meli = q.fldCodemeli;
            if (q.fldCodemeli == null || q.fldCodemeli == "")
            {
                typeCode = "1";
                Code_Meli=q.fldCodeMoshakhase;
            }
            return Json(new
            {
                fldId = q.fldId,
                fldShomareId = q.fldShomareId.ToString(),
                fldCodemeli = q.fldCodemeli,
                fldFatherName = q.fldFatherName,
                fldname = q.fldname,
                fldFamily=q.fldFamily,
                fldVadiSalamId = q.fldVadiSalamId.ToString(),
                fldGheteId = q.fldGheteId.ToString(),
                fldRadifId = q.fldRadifId.ToString(),
                fldDesc = q.fldDesc,
                typeCode=typeCode,
                Code_Meli = Code_Meli,
                fldSh_Shenasname=q.fldSh_Shenasname,
                fldTarikhRezerv=q.fldTarikhRezerv,
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Deceased.OBJ_GhabreAmanat> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Deceased.OBJ_GhabreAmanat> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldNameVadiSalam":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameVadiSalam";
                            break;
                        case "fldNameGhete":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameGhete";
                            break;
                        case "fldNameRadif":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameRadif";
                            break;
                        case "fldShomare":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomare";
                            break;
                        case "fldMeli_Moshakhase":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMeli_Moshakhase";
                            break;
                        case "fldname":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldname";
                            break;
                        case "fldFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamily";
                            break;
                        case "fldFatherName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFatherName";
                            break;
                        case "fldSh_Shenasname":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_Shenasname";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetGhabreAmanatWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]),100, IP, out Err).ToList();
                    else
                        data = servic.GetGhabreAmanatWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetGhabreAmanatWithFilter("", "", Convert.ToInt32(Session["OrganId"]),100, IP, out Err).ToList();
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

            List<WCF_Deceased.OBJ_GhabreAmanat> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult CheckCode_ShenaseMelli(string Code_ShenaseMelli, string Type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //Type=0 حقیقی
            string Msg = "", MsgTitle = ""; var Er = 0; string fldMeli_Moshakhase = ""; var fldId = 0;
            string fldName = ""; string fldFamily = ""; string fldFatherName = ""; string fldCodemeli = ""; string fldSh_Shenasname = "";
            try
            {

                var k = servic_Com.GetEmployeeWithFilter("fldMeli_Moshakhase", Code_ShenaseMelli, 0, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), out Err_Com).FirstOrDefault();
                if (k != null)
                {
                    fldMeli_Moshakhase = k.fldMeli_Moshakhase;
                    fldId = k.fldId;
                    if (k.fldName != null)
                        fldName = k.fldName;
                    if (k.fldFamily != null)
                        fldFamily = k.fldFamily;
                    if (k.fldFatherName != null)
                    fldFatherName = k.fldFatherName;
                    if (k.fldSh_Shenasname != null)
                        fldSh_Shenasname = k.fldSh_Shenasname;
                    fldCodemeli = k.fldCodemeli;
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
                Err = Er,
                 fldShenase_CodeMeli = fldMeli_Moshakhase,
                    fldId = fldId,
                    fldName = fldName,
                    fldFamily = fldFamily,
                    fldFatherName = fldFatherName,
                    fldCodemeli=fldCodemeli,
                    fldSh_Shenasname=fldSh_Shenasname
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
