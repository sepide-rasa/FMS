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
using Aspose.Cells;

namespace NewFMS.Areas.PayRoll.Controllers
{
    public class PayPersonalInfoController : Controller
    {
        //
        // GET: /PayRoll/PayPersonalInfo/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common_Pay.Comon_PayService CPservic = new WCF_Common_Pay.Comon_PayService();
        WCF_Personeli.PersoneliService Pservic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
        WCF_Common_Pay.ClsError CPErr = new WCF_Common_Pay.ClsError();
        WCF_Personeli.ClsError PErr = new WCF_Personeli.ClsError();
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
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out CErr).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult New(int Id,int OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Id = Id;
            result.ViewBag.OrganId = OrganId;
            return result;
        }
        public ActionResult ExcelWin(int? OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
             PartialView.ViewBag.OrganId = OrganId != null ? OrganId.ToString() : Session["OrganId"].ToString();
            return PartialView;
        }
        public ActionResult ShomareHesabsWin(int id, int? OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.OrganId = OrganId != null ? OrganId.ToString() : Session["OrganId"].ToString();
            return PartialView;
        }
        public ActionResult ShomareHesabPasandazWin(int id, int? OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q = Cservic.GetAshkhasWithFilter("fldHaghighiId", id.ToString(), "", 0, IP, out CErr).FirstOrDefault();
            PartialView.ViewBag.Id = q.fldId ;
            PartialView.ViewBag.OrganId = OrganId != null ? OrganId.ToString() : Session["OrganId"].ToString();
            return PartialView;
        }
        public ActionResult GetTypeBime()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = CPservic.GetTypeBimeWithFilter("", "", 0, IP, out CPErr).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetCostCenter(int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCostCenterWithFilter("fldOrganId", OrganId.ToString()/*Session["OrganId"].ToString()*/, OrganId.ToString()/*Session["OrganId"].ToString()*/, 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetStatus()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = CPservic.GetStatusWithFilter("", "", 0, IP, out CPErr).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetInsuranceWorkShop(int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetInsuranceWorkshopWithFilter("", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, WorkShopName = c.fldName });
            return this.Store(q);
        }
        public ActionResult GetBank()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetBankWithFilter("", "", 0,IP, out CErr).ToList().Select(c => new { ID = c.fldId, Name = c.fldBankName });
            return this.Store(q);
        }
        public ActionResult GetBankFix()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetBankWithFilter("BankFix", "", 0, IP, out CErr).ToList().Select(c => new { ID = c.fldId, Name = c.fldBankName });
            return this.Store(q);
        }
        public ActionResult GetTypeHesab()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetHesabTypeWithFilter("", "", 0,Session["Username"].ToString(), (Session["Password"].ToString()), IP, out CErr).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetShobe(int ID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Shobe = Cservic.GetSHobeWithFilter("fldBankId", ID.ToString(), 0, IP, out CErr);
            return Json(Shobe.Select(p1 => new { fldID = p1.fldId, fldName = p1.fldName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(WCF_PayRoll.OBJ_PayPersonalInfo Info, WCF_PayRoll.OBJ_ShomareHesabPasAndaz ShHesab,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Info.fldDesc == null)
                    Info.fldDesc = "";
                var d = Cservic.GetDate( IP, out CErr);
                if (Info.fldId == 0)
                {
                    //ذخیره
                   Msg = servic.InsertPayPersonalInfo(Info.fldPrs_PersonalInfoId, Info.fldTypeBimeId,Info.fldShomareBime,Info.fldBimeOmr,Info.fldBimeTakmili,
                        Info.fldMashagheleSakhtVaZianAvar,Info.fldCostCenterId,Info.fldMazad30Sal,Info.fldPasAndaz,Info.fldSanavatPayanKhedmat,Info.fldJobeCode,
                        Info.fldInsuranceWorkShopId, Info.fldHamsarKarmand, Info.fldMoafDarman, 1, d.fldTarikh, Info.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()),
                        OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                    
                    MsgTitle = "ذخیره موفق";
                    //if (Info.fldPasAndaz)
                    //    servic.InsertShomareHesabPasAndaz(Perid,ShHesab.fldBankFixedId,ShHesab.fldShomareHesabPersonal,ShHesab.fldShomareHesabKarfarma,"", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                }
                else
                {
                    //ویرایش
                    Msg = servic.UpdatePayPersonalInfo(Info.fldId, Info.fldPrs_PersonalInfoId, Info.fldTypeBimeId, Info.fldShomareBime, Info.fldBimeOmr, Info.fldBimeTakmili,
                        Info.fldMashagheleSakhtVaZianAvar, Info.fldCostCenterId, Info.fldMazad30Sal, Info.fldPasAndaz, Info.fldSanavatPayanKhedmat, Info.fldJobeCode,
                        Info.fldInsuranceWorkShopId, Info.fldHamsarKarmand, Info.fldMoafDarman, Info.fldDesc, Session["Username"].ToString(),
                        (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                   MsgTitle = "ویرایش موفق";
                   //if (Info.fldPasAndaz)
                   //    servic.InsertShomareHesabPasAndaz(Info.fldId, ShHesab.fldBankFixedId, ShHesab.fldShomareHesabPersonal, ShHesab.fldShomareHesabKarfarma, "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
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
                //var q = servic.GetPersonalInfoSelect("fldCostCenterId", id.ToString(), 1).FirstOrDefault();
                //if (q != null)
                //{
                //    Msg = "آیتم مورد نظر جای دیگری استفاده شده است.";
                //    MsgTitle = "خطا";
                //}
                //else
                //{
                Msg = servic.DeletePayPersonalInfo(id, Session["Username"].ToString(), (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                MsgTitle = "حذف موفق";
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
                //}
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
        
        public ActionResult Details(int Id,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPayPersonalInfoDetail(Id, OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
            //var q2 = servic.GetShomareHesabPasAndazWithFilter("fldPersonalId", Id.ToString(),Convert.ToInt32(Session["OrganId"]),0, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).FirstOrDefault();
            string BimeOmr = "0";
            string BimeTakmili = "0";
            string SanavatPayanKhedmat = "0";
            string MazadCSal = "0";
            string PasAndaz = "0";
            string HamsarKarmand = "0";
            string MashagheleSakhtVaZianAvar = "0";
            if (q.fldBimeOmr)
                BimeOmr = "1";
            if (q.fldBimeTakmili)
                BimeTakmili = "1";
            if (q.fldSanavatPayanKhedmat)
                SanavatPayanKhedmat = "1";
            if (q.fldMashagheleSakhtVaZianAvar)
                MashagheleSakhtVaZianAvar = "1";
            if (q.fldMazad30Sal)
                MazadCSal = "1";
            if (q.fldPasAndaz)
                PasAndaz = "1";
            if (q.fldHamsarKarmand)
                HamsarKarmand = "1";

            //string BankId = "";
            //string ShomareHesabPersonal = "";
            //string ShomareHesabKarfarma = "";
            //if (q2 != null)
            //{
            //    BankId = q2.fldBankFixedId.ToString();
            //    ShomareHesabKarfarma=q2.fldShomareHesabKarfarma;
            //    ShomareHesabPersonal=q2.fldShomareHesabPersonal;
            //}

            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName,
                fldFatherName = q.fldFatherName,
                fldCodemeli = q.fldCodemeli,
                fldSh_Personali = q.fldSh_Personali,
                fldTitleTypeBime = q.fldTitleTypeBime,
                fldWorkShopName = q.fldWorkShopName,
                fldBimeOmr = BimeOmr,
                fldBimeTakmili = BimeTakmili,
                fldJobeCode = q.fldJobeCode,
                fldJobDesc = q.fldJobDesc,
                fldCostCenterId = q.fldCostCenterId.ToString(),
                fldInsuranceWorkShopId = q.fldInsuranceWorkShopId.ToString(),
                fldMashagheleSakhtVaZianAvar = MashagheleSakhtVaZianAvar,
                fldMasuliyatPayanKhedmat = SanavatPayanKhedmat,
                fldMazadCSal = MazadCSal,
                fldPasAndaz = PasAndaz,
                fldHamsarKarmand = HamsarKarmand,
                fldPrs_PersonalInfoId = q.fldPrs_PersonalInfoId,
                fldShomareBime = q.fldShomareBime,
                fldTypeBimeId = q.fldTypeBimeId.ToString(),
                fldDesc = q.fldDesc,
                fldNoeEstekhdamId=q.fldNoeEstekhdamId,
                //fldBankFixedId = BankId,
                //fldShomareHesabKarfarma=ShomareHesabKarfarma,
                //fldShomareHesabPersonal = ShomareHesabPersonal
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadPassAndaz(StoreRequestParameters parameters, int PersonalId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_ShomareHesabPasAndaz> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_ShomareHesabPasAndaz> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankNameP";
                            break;
                        case "fldShomareHesabPersonal":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHesabPersonal";
                            break;
                        case "fldShomareHesabKarfarma":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHesabKarfarma";
                            break;

                    }
                    if (data != null)
                    {
                        data1 = servic.GetShomareHesabPasAndazWithFilter(field, searchtext, PersonalId, 100, IP, out Err).ToList();
                    }
                    else
                    {
                        data = servic.GetShomareHesabPasAndazWithFilter(field, searchtext, PersonalId, 100, IP, out Err).ToList();
                                            }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetShomareHesabPasAndazWithFilter("fldAshkhasId", PersonalId.ToString(), OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_ShomareHesabPasAndaz> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Read(StoreRequestParameters parameters,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_PayPersonalInfo> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_PayPersonalInfo> data1 = null;
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
                            field = "fldName_Father";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNationalCode";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomarePersoneli";
                            break;
                        case "fldShomareBime":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareBime";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                    else
                        data = servic.GetPayPersonalInfoWithFilter(field, searchtext, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPayPersonalInfoWithFilter("", "", OrganId/*Convert.ToInt32(Session["OrganId"])*/, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_PayPersonalInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReloadTypeHesab_Bank(string TypeHesab, string ShobeId, string PersonId, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            if (TypeHesab == "null" && ShobeId == "null")
            { TypeHesab = ""; ShobeId = ""; }
            var data = servic.GetShomareHesabsWithFilter("fldShobeId_TypeHesab", ShobeId, TypeHesab, PersonId, OrganId/*Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).FirstOrDefault();
            var fldShomareHesab = ""; var fldShomareKart = ""; var fldId = 0;
            if (data != null)
            {
                fldShomareHesab = data.fldShomareHesab;
                fldShomareKart = data.fldShomareKart;
                fldId = data.fldId;
            }
            return Json(new { fldShomareHesab = fldShomareHesab, fldShomareKart = fldShomareKart, fldId = fldId }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadBank(string BankId, string PersonId,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var data = servic.GetShomareHesabPasAndazWithFilter("fldAshkhasId", PersonId,OrganId/* Convert.ToInt32(Session["OrganId"])*/, 0, IP, out Err).Where(l => l.fldBankFixId == Convert.ToInt32(BankId)).FirstOrDefault();
            var fldShomareHesabPersonal = ""; var fldShomareHesabKarfarma = ""; var fldId = 0;
            var fldShomareHesabKarfarmaId = 0; var fldShomareHesabPersonalId = 0;
            if (data != null)
            {
                if (data.fldShomareHesabKarfarmaId!=null)
                fldShomareHesabKarfarmaId = Convert.ToInt32(data.fldShomareHesabKarfarmaId);
                fldShomareHesabPersonalId = data.fldShomareHesabPersonalId;
                fldShomareHesabKarfarma = data.fldShomareHesabKarfarma;
                fldShomareHesabPersonal = data.fldShomareHesabPersonal;
                fldId = data.fldId;
            }
            return Json(new { fldShomareHesabKarfarma = fldShomareHesabKarfarma, fldShomareHesabPersonal = fldShomareHesabPersonal
                , fldShomareHesabKarfarmaId = fldShomareHesabKarfarmaId
                , fldShomareHesabPersonalId = fldShomareHesabPersonalId
                , fldId = fldId }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveShomareHesab(WCF_PayRoll.OBJ_ShomareHesabs ShomareHesab,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (ShomareHesab.fldDesc == null)
                    ShomareHesab.fldDesc = "";
                if (ShomareHesab.fldId == 0)
                {
                    //ذخیره
                    Msg = servic.InsertShomareHesabs(ShomareHesab.fldPersonalId, ShomareHesab.fldShobeId, ShomareHesab.fldShomareHesab, ShomareHesab.fldShomareKart,
                        Convert.ToBoolean(ShomareHesab.fldTypeHesab),Convert.ToByte( ShomareHesab.fldHesabTypeId), ShomareHesab.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                    MsgTitle = "ذخیره موفق";
                }
                else
                {
                    Msg = servic.UpdateShomareHesabs(ShomareHesab.fldId, ShomareHesab.fldPersonalId, ShomareHesab.fldShobeId, ShomareHesab.fldShomareHesab, 
                        ShomareHesab.fldShomareKart, Convert.ToBoolean(ShomareHesab.fldTypeHesab), Convert.ToByte(ShomareHesab.fldHesabTypeId), ShomareHesab.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveShomareHesabPasandaz(WCF_PayRoll.OBJ_ShomareHesabPasAndaz ShHesab,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (ShHesab.fldDesc == null)
                    ShHesab.fldDesc = "";
                if (ShHesab.fldId == 0)
                {
                    //ذخیره
                    Msg = servic.InsertShomareHesabPasAndaz(ShHesab.fldShomareHesabPersonalId, ShHesab.fldShomareHesabKarfarmaId, "", Session["Username"].ToString(), 
                        (Session["Password"].ToString()),OrganId/* Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
                    MsgTitle = "ذخیره موفق";
                }
                else
                {
                    Msg = servic.UpdateShomareHesabPasAndaz(ShHesab.fldId, ShHesab.fldShomareHesabPersonalId, ShHesab.fldShomareHesabKarfarmaId, "",
                        Session["Username"].ToString(), (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out Err);
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        public FileResult ExcelBime(int PersonId)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
            int index = 0;var fldTitle = "";
            var Personal = CPservic.GetItemsEstekhdam_FiscalTitleWithFilter("FiscalTitle", PersonId.ToString(), 0, 0, IP, out CPErr).ToList();
            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];
               
            var Check = "عنوان آیتم";
            Cell cell = sheet.Cells[alpha[index] + "1"];
            cell.PutValue(Check);
            int i = 0;
            foreach (var _item in Personal)
            {
                fldTitle = _item.fldTitle;
                Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                Cell.PutValue(fldTitle);
                i++;
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            return File(stream.ToArray(), "xls", "Tax.xls");
        }
        public FileResult CreateExcel(string Checked,string Bime,string Hazine,string Kargah,string Status,int OrganId)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
            int index = 0;
            var StatusCheck = Checked.Split(';');
            var Check = "";
            var Name = ""; var TypeBime = ""; var ShomareBime = ""; var BimeOmr = ""; var BimeTakmili = ""; var FamilyName = ""; var FatherName = "";
            var Codemeli = ""; var HamsarKarmand = "";
            var MashagheleSakhtVaZianAvar = ""; var CostCenter = ""; var MazadCSal = ""; var PasAndaz = ""; var MasuliyatPayanKhedmat = "";
            var JobeCode = ""; var InsuranceWorkShop = "";

            string commandText = "SELECT        Pay.Pay_tblPersonalInfo.fldId, Pay.Pay_tblPersonalInfo.fldTypeBimeId, Pay.Pay_tblPersonalInfo.fldShomareBime, Pay.Pay_tblPersonalInfo.fldCostCenterId, Pay.Pay_tblPersonalInfo.fldPasAndaz, "+
                       "  Pay.Pay_tblPersonalInfo.fldSanavatPayanKhedmat, Pay.Pay_tblPersonalInfo.fldInsuranceWorkShopId, Com.tblEmployee.fldName AS fldNameEmployee, Com.tblEmployee.fldFamily, Com.tblEmployee_Detail.fldFatherName, " +
                       "  Com.tblEmployee.fldCodemeli, Prs.Prs_tblPersonalInfo.fldSh_Personali, Com.tblTypeBime.fldTitle AS fldTitleTypeBime, Pay.tblInsuranceWorkshop.fldWorkShopName, Pay.tblTabJobOfBime.fldJobDesc, "+
                       "  Pay.Pay_tblPersonalInfo.fldJobeCode, CASE WHEN (Pay.Pay_tblPersonalInfo.fldBimeOmr = 1) THEN N'دارد' ELSE N'ندارد' END AS fldBimeOmrName, CASE WHEN (Pay.Pay_tblPersonalInfo.fldBimeTakmili = 1)  "+
                       "  THEN N'دارد' ELSE N'ندارد' END AS fldBimeTakmiliName, CASE WHEN (Pay.Pay_tblPersonalInfo.fldMashagheleSakhtVaZianAvar = 1) THEN N'دارد' ELSE N'ندارد' END AS fldMashagheleSakhtVaZianAvarName,   "+
                      "   CASE WHEN (Pay.Pay_tblPersonalInfo.fldMazad30Sal = 1) THEN N'دارد' ELSE N'ندارد' END AS fldMazadCSalName, CASE WHEN (Pay.Pay_tblPersonalInfo.fldPasAndaz = 1)   "+
                      "   THEN N'دارد' ELSE N'ندارد' END AS fldPasAndazName, CASE WHEN (Pay.Pay_tblPersonalInfo.fldSanavatPayanKhedmat = 1) THEN N'دارد' ELSE N'ندارد' END AS fldSanavatPayanKhedmatName,   "+
                      "   Pay.tblCostCenter.fldTitle AS fldTitleCostCenter, Com.tblEmployee.fldFamily + '_' + Com.tblEmployee.fldName + ' (' + Com.tblEmployee_Detail.fldFatherName + ')' AS fldName_Father,   "+
                      " CASE WHEN (Pay.Pay_tblPersonalInfo.fldHamsarKarmand = 1) THEN N'دارد' ELSE N'ندارد' END AS fldHamsarKarmandName,  "+
                      "   Pay.Pay_tblPersonalInfo.fldHamsarKarmand, Com.tblEmployee.fldName + ' ' + Com.tblEmployee.fldFamily AS UserName, Prs.Prs_tblPersonalInfo.fldEsargariId, "+
                         "    (SELECT        TOP (1) Com.tblStatus.fldId "+
                         "      FROM            Com.tblPersonalStatus AS tblPersonalStatus_1 INNER JOIN "+
                           "                              Com.tblStatus ON tblPersonalStatus_1.fldStatusId = Com.tblStatus.fldId "+
                            "   WHERE        (tblPersonalStatus_1.fldPayPersonalInfoId = Pay.Pay_tblPersonalInfo.fldId) "+
                            "   ORDER BY tblPersonalStatus_1.fldId DESC) AS fldStatusId, "+
                           "  (SELECT        TOP (1) tblStatus_1.fldTitle "+
                            "   FROM            Com.tblPersonalStatus AS tblPersonalStatus_1 INNER JOIN "+
                            "                             Com.tblStatus AS tblStatus_1 ON tblPersonalStatus_1.fldStatusId = tblStatus_1.fldId "+
                            "   WHERE        (tblPersonalStatus_1.fldPayPersonalInfoId = Pay.Pay_tblPersonalInfo.fldId) "+
                            "   ORDER BY tblPersonalStatus_1.fldId DESC) AS fldStatusTitle "+
"FROM            Pay.Pay_tblPersonalInfo INNER JOIN "+
                        " Prs.Prs_tblPersonalInfo ON Pay.Pay_tblPersonalInfo.fldPrs_PersonalInfoId = Prs.Prs_tblPersonalInfo.fldId INNER JOIN "+
                      "   Com.tblTypeBime ON Pay.Pay_tblPersonalInfo.fldTypeBimeId = Com.tblTypeBime.fldId INNER JOIN "+
                       "  Pay.tblInsuranceWorkshop ON Pay.Pay_tblPersonalInfo.fldInsuranceWorkShopId = Pay.tblInsuranceWorkshop.fldId INNER JOIN "+
                       "  Pay.tblTabJobOfBime ON Pay.Pay_tblPersonalInfo.fldJobeCode = Pay.tblTabJobOfBime.fldJobCode INNER JOIN "+
                      "   Pay.tblCostCenter ON Pay.Pay_tblPersonalInfo.fldCostCenterId = Pay.tblCostCenter.fldId INNER JOIN "+
                      "   Com.tblEmployee ON Prs.Prs_tblPersonalInfo.fldEmployeeId = Com.tblEmployee.fldId INNER JOIN "+
                      "   Com.tblEmployee_Detail ON Com.tblEmployee.fldId = Com.tblEmployee_Detail.fldEmployeeId "+
                     " WHERE Com.fn_OrganId(fldPrs_PersonalInfoId) = " + OrganId.ToString() /*(Session["OrganId"]).ToString()*/ + where(Bime, Hazine, Kargah, Status);

            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var per = service.GetData(commandText).Tables[0];


            List<WCF_PayRoll.OBJ_PayPersonalInfo> p = new List<WCF_PayRoll.OBJ_PayPersonalInfo>();
            for (int i = 0; i < per.Rows.Count; i++)
            {
                WCF_PayRoll.OBJ_PayPersonalInfo l = new WCF_PayRoll.OBJ_PayPersonalInfo();
                l.fldId = (int)(per.Rows[i]["fldId"]);
                l.fldNameEmployee = (string)(per.Rows[i]["fldNameEmployee"]);
                l.fldFamily = (string)(per.Rows[i]["fldFamily"]);
                l.fldFatherName = (string)(per.Rows[i]["fldFatherName"]);
                l.fldCodemeli = (string)(per.Rows[i]["fldCodemeli"]);
                l.fldTitleTypeBime = (string)(per.Rows[i]["fldTitleTypeBime"]);
                l.fldShomareBime = (string)(per.Rows[i]["fldShomareBime"]);
                l.fldBimeOmrName = (string)(per.Rows[i]["fldBimeOmrName"]);
                l.fldBimeTakmiliName = (string)(per.Rows[i]["fldBimeTakmiliName"]);
                l.fldMashagheleSakhtVaZianAvarName = (string)(per.Rows[i]["fldMashagheleSakhtVaZianAvarName"]);
                l.fldTitleCostCenter = (string)(per.Rows[i]["fldTitleCostCenter"]);
                l.fldMazadCSalName = (string)(per.Rows[i]["fldMazadCSalName"]);
                l.fldPasAndazName = (string)(per.Rows[i]["fldPasAndazName"]);
                l.fldSanavatPayanKhedmatName = (string)(per.Rows[i]["fldSanavatPayanKhedmatName"]);
                l.fldJobDesc = (string)(per.Rows[i]["fldJobDesc"]);
                l.fldWorkShopName = (string)(per.Rows[i]["fldWorkShopName"]);
                l.fldHamsarKarmandName = (string)(per.Rows[i]["fldHamsarKarmandName"]);

                p.Add(l);
            }
            var Personal = p.ToList();

            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];

            foreach (var item in StatusCheck)
            {
                //var Personal = servic.GetPayPersonalInfoWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 0, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).ToList();
                switch (item)
                {
                    case "Name":
                        Check = "نام";
                        Cell cell = sheet.Cells[alpha[index] + "1"];
                        cell.PutValue(Check);
                        int i = 0;
                        foreach (var _item in Personal)
                        {
                            Name = _item.fldNameEmployee;
                            Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                            Cell.PutValue(Name);
                            i++;
                        }
                        index++;
                        break;
                    case "Family":
                        Check = "نام خانوادگی";
                        Cell cell12 = sheet.Cells[alpha[index] + "1"];
                        cell12.PutValue(Check);
                        int j = 0;
                        foreach (var _item in Personal)
                        {
                            FamilyName = _item.fldFamily;
                            Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                            Cell.PutValue(FamilyName);
                            j++;
                        }
                        index++;
                        break;
                    case "Father":
                        Check = "نام پدر";
                        Cell cell22 = sheet.Cells[alpha[index] + "1"];
                        cell22.PutValue(Check);
                        int k3 = 0;
                        foreach (var _item in Personal)
                        {
                            FatherName = _item.fldFatherName;
                            Cell Cell = sheet.Cells[alpha[index] + (k3 + 2)];
                            Cell.PutValue(FatherName);
                            k3++;
                        }
                        index++;
                        break;
                    case "MeliCode":
                        Check = "کدملی";
                        Cell cell23 = sheet.Cells[alpha[index] + "1"];
                        cell23.PutValue(Check);
                        int k2 = 0;
                        foreach (var _item in Personal)
                        {
                            Codemeli = _item.fldCodemeli;
                            Cell Cell = sheet.Cells[alpha[index] + (k2 + 2)];
                            Cell.PutValue(Codemeli);
                            k2++;
                        }
                        index++;
                        break;
                    case "TypeBime":
                        Check = "نوع بیمه";
                        Cell cell1 = sheet.Cells[alpha[index] + "1"];
                        cell1.PutValue(Check);
                        int j2 = 0;
                        foreach (var _item in Personal)
                        {
                            TypeBime = _item.fldTitleTypeBime;
                            Cell Cell = sheet.Cells[alpha[index] + (j2 + 2)];
                            Cell.PutValue(TypeBime);
                            j2++;
                        }
                        index++;
                        break;
                    case "ShomareBime":
                        Check = "شماره بیمه";
                        Cell cell2 = sheet.Cells[alpha[index] + "1"];
                        cell2.PutValue(Check);
                        int k = 0;
                        foreach (var _item in Personal)
                        {
                            ShomareBime = _item.fldShomareBime;
                            Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                            Cell.PutValue(ShomareBime);
                            k++;
                        }
                        index++;
                        break;
                    case "BimeOmr":
                        Check = "بیمه عمر";
                        Cell cell3 = sheet.Cells[alpha[index] + "1"];
                        cell3.PutValue(Check);
                        int q = 0;
                        foreach (var _item in Personal)
                        {
                            BimeOmr = _item.fldBimeOmrName;
                            Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                            Cell.PutValue(BimeOmr);
                            q++;
                        }
                        index++;
                        break;
                    case "BimeTakmili":
                        Check = "بیمه تکمیلی";
                        Cell cell4 = sheet.Cells[alpha[index] + "1"];
                        cell4.PutValue(Check);
                        int w = 0;
                        foreach (var _item in Personal)
                        {
                            BimeTakmili = _item.fldBimeTakmiliName;
                            Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                            Cell.PutValue(BimeTakmili);
                            w++;
                        }
                        index++;
                        break;
                    case "MashagheleSakhtVaZianAvar":
                        Check = "مشاغل سخت و زیان آور";
                        Cell cell5 = sheet.Cells[alpha[index] + "1"];
                        cell5.PutValue(Check);
                        int z = 0;
                        foreach (var _item in Personal)
                        {
                            MashagheleSakhtVaZianAvar = _item.fldMashagheleSakhtVaZianAvarName;
                            Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                            Cell.PutValue(MashagheleSakhtVaZianAvar);
                            z++;
                        }
                        index++;
                        break;
                    case "CostCenter":
                        Check = "مرکز هزینه";
                        Cell cell6 = sheet.Cells[alpha[index] + "1"];
                        cell6.PutValue(Check);
                        int x = 0;
                        foreach (var _item in Personal)
                        {
                            CostCenter = _item.fldTitleCostCenter;
                            Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
                            Cell.PutValue(CostCenter);
                            x++;
                        }
                        index++;
                        break;
                    case "MazadCSal":
                        Check = "مازاد 30 سال";
                        Cell cell7 = sheet.Cells[alpha[index] + "1"];
                        cell7.PutValue(Check);
                        int y = 0;
                        foreach (var _item in Personal)
                        {
                            MazadCSal = _item.fldMazadCSalName;
                            Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
                            Cell.PutValue(MazadCSal);
                            y++;
                        }
                        index++;
                        break;
                    case "PasAndaz":
                        Check = "پس انداز";
                        Cell cell8 = sheet.Cells[alpha[index] + "1"];
                        cell8.PutValue(Check);
                        int a = 0;
                        foreach (var _item in Personal)
                        {
                            PasAndaz = _item.fldPasAndazName;
                            Cell Cell = sheet.Cells[alpha[index] + (a + 2)];
                            Cell.PutValue(PasAndaz);
                            a++;
                        }
                        index++;
                        break;
                    case "MasuliyatPayanKhedmat":
                        Check = "سنوات پایان خدمت";
                        Cell cell9 = sheet.Cells[alpha[index] + "1"];
                        cell9.PutValue(Check);
                        int b = 0;
                        foreach (var _item in Personal)
                        {
                            MasuliyatPayanKhedmat = _item.fldSanavatPayanKhedmatName;
                            Cell Cell = sheet.Cells[alpha[index] + (b + 2)];
                            Cell.PutValue(MasuliyatPayanKhedmat);
                            b++;
                        }
                        index++;
                        break;
                    case "JobeCode":
                        Check = "کد شغلی بیمه";
                        Cell cell10 = sheet.Cells[alpha[index] + "1"];
                        cell10.PutValue(Check);
                        int s = 0;
                        foreach (var _item in Personal)
                        {
                            JobeCode = _item.fldJobDesc;
                            Cell Cell = sheet.Cells[alpha[index] + (s + 2)];
                            Cell.PutValue(JobeCode);
                            s++;
                        }
                        index++;
                        break;
                    case "InsuranceWorkShop":
                        Check = "کارگاه بیمه";
                        Cell cell11 = sheet.Cells[alpha[index] + "1"];
                        cell11.PutValue(Check);
                        int r = 0;
                        foreach (var _item in Personal)
                        {
                            InsuranceWorkShop = _item.fldWorkShopName;
                            Cell Cell = sheet.Cells[alpha[index] + (r + 2)];
                            Cell.PutValue(InsuranceWorkShop);
                            r++;
                        }
                        index++;
                        break;
                    case "HamsarKarmand":
                        Check = "همسر کارمند";
                        Cell cell13 = sheet.Cells[alpha[index] + "1"];
                        cell13.PutValue(Check);
                        int r1 = 0;
                        foreach (var _item in Personal)
                        {
                            InsuranceWorkShop = _item.fldHamsarKarmandName;
                            Cell Cell = sheet.Cells[alpha[index] + (r1 + 2)];
                            Cell.PutValue(InsuranceWorkShop);
                            r1++;
                        }
                        index++;
                        break;
                }
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            return File(stream.ToArray(), "xls", "PayPersonal.xls");
        }
        private string where(string Bime, string Hazine, string Kargah, string Status)
        {
            try
            {
                string s = "";
                if (Status != "")
                {
                    s = s + " and Com.fn_MaxPersonalStatus(Pay.Pay_tblPersonalInfo.fldId,'hoghoghi')=" + Status;
                }
                if (Bime != "")
                {
                    s = s + " and Pay.Pay_tblPersonalInfo.fldTypeBimeId=" + Bime;
                }
                if (Hazine != "")
                {
                    s = s + " and Pay.Pay_tblPersonalInfo.fldCostCenterId=" + Hazine;
                }
                if (Kargah != "")
                {
                    s = s + " and Pay.Pay_tblPersonalInfo.fldInsuranceWorkShopId=" + Kargah;
                }
                return s;
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    return x.InnerException.Message;
                return "";
            }
        }
        public ActionResult SaveStatus(WCF_Personeli.OBJ_PersonalStatus PersonalStatus,int OrganId)
        {

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (PersonalStatus.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق"; 
                    Msg = Pservic.InsertPersonalStatus(PersonalStatus.fldStatusId, null, PersonalStatus.fldPrsPersonalInfoId, 
                        PersonalStatus.fldDateTaghirVaziyat, PersonalStatus.fldDesc, Session["Username"].ToString(),
                        (Session["Password"].ToString()), OrganId/*Convert.ToInt32(Session["OrganId"])*/, IP, out PErr);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = Pservic.UpdatePersonalStatus(PersonalStatus.fldId,PersonalStatus.fldStatusId, null, PersonalStatus.fldPrsPersonalInfoId,
                        PersonalStatus.fldDateTaghirVaziyat, PersonalStatus.fldDesc, Session["Username"].ToString(),
                        (Session["Password"].ToString()), IP, out PErr);
                }
                if (PErr.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = PErr.ErrorMsg;
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
        public ActionResult StatusDetail(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Pservic.GetPersonalStatusWithFilter("fldStatusChangeEnd_HoghughiId", Id.ToString(), 0, IP, out PErr).FirstOrDefault();
            var fldId = 0; var fldStatusId = ""; var fldDesc = "";
            if (q != null)
            {
                fldId = q.fldId;
                fldStatusId = q.fldStatusId.ToString();
                fldDesc = q.fldDesc;
            }
            return Json(new
            {
                fldId = fldId,
                fldStatusId = fldStatusId,
                fldDesc = fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
