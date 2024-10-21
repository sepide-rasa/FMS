using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;


namespace NewFMS.Areas.Comon.Controllers
{
    public class AshkhaseHoghoghiController : Controller
    {
        //
        // GET: /Comon/AshkhaseHoghoghi/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
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
        public ActionResult AshkhasHoghoghiDetails(int id, byte flag, int FishId, int ElamAvarezId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.Id = id;
                var Date = servic.GetDate(IP.ToString(), out Err);
                var q = servic.GetAshkhaseHoghoghiDetail(id, IP, out Err);
                PartialView.ViewBag.TypeShakhs = q.fldTypeShakhs;

                PartialView.ViewBag.Date = Date.fldDateTime;
                PartialView.ViewBag.flag = flag;
                PartialView.ViewBag.FishId = FishId;

                PartialView.ViewBag.ElamAvarezId = ElamAvarezId;
                return PartialView;
            }
        }
        public ActionResult AshkhasHoghoghiTitle(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.Id = id;
                return PartialView;
            }
        }
        public ActionResult Save(WCF_Common.OBJ_AshkhaseHoghoghi Ashkhas)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "ذخیره با موفقیت انجام شد.", MsgTitle = ""; var Er = 0; var AshkhasId = 0;
            try
            {
                if (Ashkhas.fldShenaseMelli == null)
                    Ashkhas.fldShenaseMelli = "0";
                if (Ashkhas.fldShenaseMelli == "")
                    Ashkhas.fldShenaseMelli = "0";
                if (Ashkhas.fldShomareSabt == "")
                    Ashkhas.fldShomareSabt = null;
                if (Ashkhas.fldSayer == 2)
                {
                    if (Ashkhas.fldShenaseMelli == null)
                        Ashkhas.fldShenaseMelli = "";
                }
                if (Ashkhas.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    AshkhasId = servic.InsertAshkhaseHoghoghi(Ashkhas.fldShenaseMelli, Ashkhas.fldName, Ashkhas.fldShomareSabt, Ashkhas.fldTypeShakhs, Ashkhas.fldSayer
                        , Ashkhas.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateAshkhaseHoghoghi(Ashkhas.fldId, Ashkhas.fldShenaseMelli, Ashkhas.fldName, Ashkhas.fldShomareSabt, Ashkhas.fldTypeShakhs, Ashkhas.fldSayer
                    , Ashkhas.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Er = Er,
                AshkhasId = AshkhasId
            }, JsonRequestBehavior.AllowGet);
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
                Msg = servic.DeleteAshkhaseHoghoghi(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Details(int Id)
        {
            var q = servic.GetAshkhaseHoghoghiDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldShenaseMelli = q.fldShenaseMelli,
                fldName = q.fldName,
                fldShomareSabt = q.fldShomareSabt,
                fldDesc = q.fldDesc,
                fldTypeShakhs = q.fldTypeShakhs.ToString(),
                fldSayer = q.fldSayer.ToString(),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_AshkhaseHoghoghi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_AshkhaseHoghoghi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldShenaseMelli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenaseMelli";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldShomareSabt":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareSabt";
                            break;
                        case "fldTypeShakhsName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeShakhsName";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetAshkhaseHoghoghiWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetAshkhaseHoghoghiWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetAshkhaseHoghoghiWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_AshkhaseHoghoghi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult DetailsAshkhaseHoghoghiDetail(int HoghoghiId)
        {
            var q = servic.GetAshkhaseHoghoghi_DetailWithFilter("fldAshkhaseHoghoghiId", HoghoghiId.ToString(), 1, IP, out Err).FirstOrDefault();
            var q2 = servic.GetAshkhaseHoghoghiDetail(HoghoghiId, IP, out Err);
            if (q == null)
            {

                return Json(new
                {
                    fldAshkhaseHoghoghiId = q2.fldId,
                    fldShenaseMelli = q2.fldShenaseMelli,
                    fldShomareSabt = q2.fldShomareSabt,
                    fldName = q2.fldName,
                    fldTypeShakhs = q2.fldTypeShakhs.ToString()
                }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new
                {
                    fldId = q.fldId,
                    fldShenaseMelli = q2.fldShenaseMelli,
                    fldShomareSabt = q2.fldShomareSabt,
                    fldName = q2.fldName,
                    fldDesc = q.fldDesc,
                    fldAddress = q.fldAddress,
                    fldCodePosti = q.fldCodePosti,
                    fldAshkhaseHoghoghiId = q.fldAshkhaseHoghoghiId,
                    fldCodEghtesadi = q.fldCodEghtesadi,
                    fldShomareTelephone = q.fldShomareTelephone,
                    fldTypeShakhs = q.fldTypeShakhs.ToString()

                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SaveAshkhaseHoghoghiDetail(WCF_Common.OBJ_AshkhaseHoghoghi_Detail Hoghoghi)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {


                if (Hoghoghi.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertAshkhaseHoghoghi_Detail(Hoghoghi.fldAshkhaseHoghoghiId, Hoghoghi.fldCodEghtesadi, Hoghoghi.fldAddress, Hoghoghi.fldCodePosti,
                        Hoghoghi.fldShomareTelephone, Hoghoghi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                }
                else
                {
                    //ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateAshkhaseHoghoghi_Detail(Hoghoghi.fldId, Hoghoghi.fldAshkhaseHoghoghiId, Hoghoghi.fldCodEghtesadi, Hoghoghi.fldAddress, Hoghoghi.fldCodePosti,
                    Hoghoghi.fldShomareTelephone, Hoghoghi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);


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
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadTitles(StoreRequestParameters parameters, int fldAshkhasHoghoghiId)
        {
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            Models.AutomationEntities p = new Models.AutomationEntities();
            List<Models.spr_tblAshkhaseHoghoghiTitlesSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List < Models.spr_tblAshkhaseHoghoghiTitlesSelect> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                    }
                    if (data != null)
                        data1 = p.spr_tblAshkhaseHoghoghiTitlesSelect(field, searchtext, Convert.ToInt32(Session["OrganId"]),100).Where(l => l.fldAshkhasHoghoghiId == fldAshkhasHoghoghiId).ToList();
                    else
                        data = p.spr_tblAshkhaseHoghoghiTitlesSelect(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100).Where(l => l.fldAshkhasHoghoghiId == fldAshkhasHoghoghiId).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.spr_tblAshkhaseHoghoghiTitlesSelect("", "",Convert.ToInt32(Session["OrganId"]), 100).Where(l => l.fldAshkhasHoghoghiId == fldAshkhasHoghoghiId).ToList();
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

            List<Models.spr_tblAshkhaseHoghoghiTitlesSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveTitle(Models.spr_tblAshkhaseHoghoghiTitlesSelect Hoghoghi)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                Models.AutomationEntities p = new Models.AutomationEntities();

                if (Hoghoghi.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = "ذخیره با موفقیت انجام شد.";
                    p.spr_tblAshkhaseHoghoghiTitlesInsert(Hoghoghi.fldName,Hoghoghi.fldAshkhasHoghoghiId, IP,"",Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]));

                }
                else
                {
                    //ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = "ویرایش با موفقیت انجام شد.";
                    p.spr_tblAshkhaseHoghoghiTitlesUpdate(Hoghoghi.fldId, Hoghoghi.fldName, Hoghoghi.fldAshkhasHoghoghiId, IP, "", Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["OrganId"]));


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
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailsTitle(int Id)
        {
            Models.AutomationEntities p = new Models.AutomationEntities();
            var q = p.spr_tblAshkhaseHoghoghiTitlesSelect("fldId", Id.ToString(), Convert.ToInt32(Session["OrganId"]), 0).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldAshkhasHoghoghiId = q.fldAshkhasHoghoghiId,
                fldName = q.fldName
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteTitle(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Models.AutomationEntities p = new Models.AutomationEntities();
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg ="حذف با موفقیت انجام شد." ;
                    p.spr_tblAshkhaseHoghoghiTitlesDelete(id, Convert.ToInt32(Session["UserId"]));
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
    }
}
