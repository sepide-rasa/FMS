using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class FarzandanController : Controller
    {
        //
        // GET: /Personeli/Farzandan/
        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        WCF_PayRoll.PayRolService PayService = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
        WCF_PayRoll.ClsError PErr = new WCF_PayRoll.ClsError();

        public ActionResult IndexNew(string containerId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.AfradTahtePooshesh(); 
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult New(int id, int PersonalId, string Family, string FatherName, string Name, string Jensiyat)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            string TarikhShamsi;
            var m = Cservic.GetDate(IP, out CErr);            
            TarikhShamsi = m.fldTarikh;
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.Family = Family;
            PartialView.ViewBag.FatherName = FatherName;
            PartialView.ViewBag.Name = Name;
            PartialView.ViewBag.Jensiyat = Jensiyat;
            PartialView.ViewBag.PersonalId = PersonalId;
            PartialView.ViewBag.fldTarikh = m.fldDateTime.ToString("yyyy-MM-dd");
            PartialView.ViewBag.TarikhShamsi = m.fldTarikh;
            return PartialView;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetNesbatShakhs(int IdPersonal)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetNesbatWithPersonalInfoId("Nesbat", IdPersonal, IP, out Err).ToList().Select(c => new { fldId = c.Value, fldName = c.Title });
            return this.Store(q);
        }
        public ActionResult GetNesbat(int IdPersonal, int Nesbat)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var person = PayService.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", IdPersonal.ToString(),
                Convert.ToInt32(Session["OrganId"]), 1, IP, out PErr).FirstOrDefault();
            if (person!=null && person.fldTypeBimeId == 1 && (Nesbat == 3 || Nesbat == 4))/*تامین اجتماعی*/
            {
                var q1 = servic.GetNesbatWithPersonalInfoId("Takafol", IdPersonal, IP, out Err).ToList().Where(l=>l.Value==3 || l.Value==5)
                    .Select(c => new { fldId = c.Value, fldName = c.Title });
                return this.Store(q1);
            }
            var q = servic.GetNesbatWithPersonalInfoId("Takafol", IdPersonal, IP, out Err).ToList().Where(l => l.Value != 5)
                .Select(c => new { fldId = c.Value, fldName = c.Title });
            return this.Store(q);
        }
        public ActionResult Save(WCF_Personeli.OBJ_AfradTahtePooshesh Farzandan)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;
            try
            {
                if (Farzandan.fldDesc == null)
                    Farzandan.fldDesc = "";
                if (Farzandan.fldId == 0)
                { //ذخیره
                    servic.InsertAfradTahtePooshesh(Farzandan.fldPersonalId, Farzandan.fldName,Farzandan.fldFamily,Farzandan.fldBirthDate, Farzandan.fldStatus, Farzandan.fldMashmul,
                        Farzandan.fldNesbat, Farzandan.fldCodeMeli, Farzandan.fldSh_Shenasname, Farzandan.fldFatherName,Farzandan.fldTarikhEzdevaj,Convert.ToByte(Farzandan.fldNesbatShakhs), Farzandan.fldDesc,Convert.ToBoolean(Farzandan.fldMashmoolPadash),Farzandan.fldTarikhTalagh, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                    Msg = "ذخیره با موفقیت انجام شد.";
                    MsgTitle = "ذخیره موفق";
                }
                else
                { //ویرایش

                    servic.UpdateAfradTahtePooshesh(Farzandan.fldId, Farzandan.fldPersonalId, Farzandan.fldName, Farzandan.fldFamily, Farzandan.fldBirthDate, Farzandan.fldStatus, Farzandan.fldMashmul,
                        Farzandan.fldNesbat, Farzandan.fldCodeMeli, Farzandan.fldSh_Shenasname, Farzandan.fldFatherName, Farzandan.fldTarikhEzdevaj, Convert.ToByte(Farzandan.fldNesbatShakhs), Farzandan.fldDesc, Convert.ToBoolean(Farzandan.fldMashmoolPadash),Farzandan.fldTarikhTalagh, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                    Msg = "ویرایش با موفقیت انجام شد.";
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
                servic.DeleteAfradTahtePooshesh(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                Msg = "حذف با موفقیت انجام شد.";
                MsgTitle = "حذف موفق";
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAfradTahtePoosheshDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            var p = servic.GetPersoneliInfoDetail(q.fldPersonalId, Convert.ToInt32(Session["OrganId"]), IP, out Err);

            return Json(new
            {
                fldNameEmployee = p.fldNameEmployee,
                fldNationalCode = p.fldCodemeli,
                fldShomarePersoneli = p.fldSh_Personali,
                fldId = q.fldId,
                fldBirthDate = q.fldBirthDate,
                fldMashmul = q.fldMashmul,
                fldName = q.fldName,
                fldFamily=q.fldFamily,
                fldPersonalId = q.fldPersonalId,
                fldStatus = q.fldStatus.ToString(),
                fldCodeMeli=q.fldCodeMeli,
                fldFatherName=q.fldFatherName,
                fldNesbat=q.fldNesbat.ToString(),
                fldSh_Shenasname=q.fldSh_Shenasname,
                fldDesc = q.fldDesc,
                fldTarikhEzdevaj=q.fldTarikhEzdevaj,
                fldTarikhTalagh = q.fldTarikhTalagh,
                fldNesbatShakhs = q.fldNesbatShakhs.ToString(),
                fldMashmoolPadash=q.fldMashmoolPadash
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailsHeader(int Id)
        {

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPersoneliInfoDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
 
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldNameEmployee,
                fldNationalCode = q.fldCodemeli,
                fldShomarePersoneli = q.fldSh_Personali,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_PersonalInfo> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_PersonalInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName_FamilyEmployee":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName_FamilyEmployee";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_Personali";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetPersoneliInfoWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetPersoneliInfoWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPersoneliInfoWithFilter("", "",Convert.ToInt32(Session["OrganId"]),100, IP,out Err).ToList();
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

            List<WCF_Personeli.OBJ_PersonalInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Rload(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_AfradTahtePooshesh> data = null;

            data = servic.GetAfradTahtePoosheshWithFilter("fldPersonalId", PersonalId.ToString(),Convert.ToInt32(Session["OrganId"]),100, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
