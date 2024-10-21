using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Deceased.Controllers
{
    public class MotevaffaController : Controller
    {
        //
        // GET: /Deceased/Motevaffa/

        WCF_Deceased.DeceasedService servic = new WCF_Deceased.DeceasedService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Deceased.ClsError Err = new WCF_Deceased.ClsError();

        WCF_Common.CommonService servic_C = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_C = new WCF_Common.ClsError();

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
        public ActionResult GetVadiSalam()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetVadiSalamWithFilter("fldOrganId", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldName });
            return this.Store(q);
        }
        public ActionResult GetGhete(string VadiSalamId, string MotevafaId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var IdMotevafa = 0;
            if (MotevafaId != "")
                IdMotevafa = Convert.ToInt32(MotevafaId);
            var q = servic.GetGheteWithFilter("GheteKhali_Motevafa", VadiSalamId, IdMotevafa, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldNameGhete });
            return this.Store(q);
        }
        public ActionResult GetRadif(string GheteId, string MotevafaId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var IdMotevafa = 0;
            if (MotevafaId != "")
                IdMotevafa = Convert.ToInt32(MotevafaId);
            var q = servic.GetRadifWithFilter("RadifKhali_Motevafa", GheteId, IdMotevafa, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldNameRadif });
            return this.Store(q);
        }
        public ActionResult GetShomare(string RadifId, string MotevafaId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var IdMotevafa = 0;
            if (MotevafaId != "")
                IdMotevafa = Convert.ToInt32(MotevafaId);
            var q = servic.GetShomareWithFilter("ShomareKhali_Motevafa", RadifId, IdMotevafa, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldShomare });
            return this.Store(q);
        }
        public ActionResult GetMahalFot()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMahalFotWithFilter("", "",  0, IP, out Err).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldNameMahal });
            return this.Store(q);
        }
        public ActionResult GetElatFot()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCauseOfDeathWithFilter("", "", 0, IP, out Err).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldReason });
            return this.Store(q);
        }
        public int checkCodeMeli(string codec)
        {
            char[] chArray = codec.ToCharArray();
            int[] numArray = new int[chArray.Length];
            string x = codec;
            switch (x)
            {
                case "0000000000":
                case "1111111111":
                case "2222222222":
                case "3333333333":
                case "4444444444":
                case "5555555555":
                case "6666666666":
                case "7777777777":
                case "8888888888":
                case "9999999999":
                case "0123456789":
                case "9876543210":

                    return 0;
                    break;
            }
            for (int i = 0; i < chArray.Length; i++)
            {
                numArray[i] = (int)char.GetNumericValue(chArray[i]);
            }
            int num2 = numArray[9];

            int num3 = ((((((((numArray[0] * 10) + (numArray[1] * 9)) + (numArray[2] * 8)) + (numArray[3] * 7)) + (numArray[4] * 6)) + (numArray[5] * 5)) + (numArray[6] * 4)) + (numArray[7] * 3)) + (numArray[8] * 2);
            int num4 = num3 - ((num3 / 11) * 11);
            if ((((num4 == 0) && (num2 == num4)) || ((num4 == 1) && (num2 == 1))) || ((num4 > 1) && (num2 == Math.Abs((int)(num4 - 11)))))
            {
                return 1;
            }
            else
            {
                return 0;

            }
        }
        public ActionResult Save(WCF_Deceased.OBJ_Motevaffa Motevaffa, int ShomareId, string CodeMoshakhase, string Sh_Shenasnameh, WCF_Common.OBJ_Employee Employee)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Motevaffa.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    var IDEmployee=servic.InsertEmploye(Employee.fldCodemeli, CodeMoshakhase, Employee.fldName, Employee.fldFamily, Sh_Shenasnameh, Employee.fldFatherName, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Err = 1,
                            Msg = Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }  
                    Msg = servic.InsertMotevaffa(ShomareId, IDEmployee, Motevaffa.fldCauseOfDeathId, Motevaffa.fldTarikhFot, Motevaffa.fldTarikhDafn, Motevaffa.fldMahalFotId, Motevaffa.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateMotevaffa(Motevaffa.fldId, Motevaffa.fldCauseOfDeathId, Motevaffa.fldGhabreAmanatId, Motevaffa.fldTarikhFot, Motevaffa.fldTarikhDafn, Motevaffa.fldMahalFotId, Motevaffa.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(byte id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteMotevaffa(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMotevaffaDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);

            var Code_Meli = q.fldCodemeli;
            if (q.fldCodemeli == null || q.fldCodemeli == "")
            {
                Code_Meli = q.fldCodeMoshakhase;
            }
            return Json(new
            {
                fldId = q.fldId,
                fldDesc = q.fldDesc,
                fldCauseOfDeathId = q.fldCauseOfDeathId.ToString(),
                fldCodemeli = Code_Meli,
                fldFamily = q.fldFamily,
                fldFatherName = q.fldFatherName,
                fldGhabreAmanatId = q.fldGhabreAmanatId,
                fldGheteId = q.fldGheteId.ToString(),
                fldMahalFotId = q.fldMahalFotId.ToString(),
                fldname = q.fldname,
                fldRadifId = q.fldRadifId.ToString(),
                fldSh_Shenasname = q.fldSh_Shenasname,
                fldTarikhDafn = q.fldTarikhDafn,
                fldTarikhFot = q.fldTarikhFot,
                fldVadiSalamId = q.fldVadiSalamId.ToString(),
                fldShomareId = q.fldShomareId.ToString(),
                fldTypeMotevafa = q.fldTypeMotevafa.ToString(),
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckShaks(string Code_ShenaseMelli, string Type)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; string fldMeli_Moshakhase = ""; var fldId = 0;
            string fldName = ""; string fldFamily = ""; string fldFatherName = ""; string fldCodemeli = ""; string fldSh_Shenasname = "";
            string VadiSalamId = "0"; string GheteId = "0"; string RadifId = "0"; string ShomareId = "0";
            int chck = 1;
            try
            {
                if (Type == "0")
                {
                    chck = checkCodeMeli(Code_ShenaseMelli);
                    if (chck != 1)
                    {
                        return Json(new
                        {
                            Msg = "کد ملی وارد شده معتبر نمی باشد.",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }



                var k = servic_C.GetEmployeeWithFilter("fldMeli_Moshakhase", Code_ShenaseMelli, 0, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), out Err_C).FirstOrDefault();

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
                var amant = servic.GetGhabreAmanatWithFilter("checkCodemeli", Code_ShenaseMelli, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                
                if (amant != null)
                {
                    VadiSalamId = amant.fldVadiSalamId.ToString();
                    GheteId = amant.fldGheteId.ToString();
                    RadifId = amant.fldRadifId.ToString();
                    ShomareId = amant.fldShomareId.ToString();
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
                Er = Er,
                fldId = fldId,
                fldName = fldName,
                fldFamily = fldFamily,
                fldFatherName = fldFatherName,
                fldSh_Shenasname = fldSh_Shenasname,
                VadiSalamId = VadiSalamId,
                GheteId = GheteId,
                RadifId = RadifId,
                ShomareId = ShomareId,
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Deceased.OBJ_Motevaffa> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Deceased.OBJ_Motevaffa> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTypeNameMotevafa":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeNameMotevafa";
                            break;
                        case "fldname":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldname";
                            break;
                        case "fldFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamily";
                            break;
                        case "fldCodeMotevafa":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodeMotevafa";
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
                        case "fldTarikhFot":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhFot";
                            break;
                        case "fldTarikhDafn":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhDafn";
                            break;
                        case "fldShomareTabaghe":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareTabaghe";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetMotevaffaWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetMotevaffaWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMotevaffaWithFilter("fldOrganId", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Deceased.OBJ_Motevaffa> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
