using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using System.Web.Script.Serialization;

namespace NewFMS.Areas.Comon.Controllers
{
    public class MasoulinController : Controller
    {
        //
        // GET: /Comon/Masoulin/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        public ActionResult Index(string containerId)
        {//باز شدن تب جدید

            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            return new Ext.Net.MVC.PartialViewResult();
        }
        public ActionResult IndexNew(string containerId)
        {
            if (Session["UserName"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ViewData.Model = new NewFMS.Models.Masoulin(); ;
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
        public ActionResult New(int Id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id =Id;
            return PartialView;
        }

        public ActionResult NewZirList(int id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }

        public ActionResult GetModule_Org()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetModule_OrganWithFilter("fldUserId", "", 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldNameModule_Organ });
            return this.Store(q);
        }

        public ActionResult CheckHaveMasoulin(int Module_OrganId, string TarikhEjra)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            int HeaderId=0;
            string Mode="Save";
            var q = servic.GetMasoulinWithFilter("fldTarikhEjra", TarikhEjra,Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l => l.fldModule_OrganId == Module_OrganId).FirstOrDefault();
            if (q != null)
            {
                HeaderId = q.fldId;
                Mode = "Edit";
            }
            List<WCF_Common.OBJ_Masoulin_DetailList> ListMasoulin = null;
            ListMasoulin = servic.GetMasoulin_DetailList(HeaderId,IP, out Err).ToList();
            return Json(new
            {
                ListMasoulin = ListMasoulin,
                Mode = Mode
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(WCF_Common.OBJ_Masoulin Masoulin)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
      
            try
            {
                if (Masoulin.fldId == 0/* && Mode == "Save"*/)
                { //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertMasoulin(Masoulin.fldModule_OrganId, Masoulin.fldTarikhEjra, /*Detail,*/ Masoulin.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                { //ویرایش
                    MsgTitle = "ویرایش موفق";
                    var q = servic.GetMasoulinWithFilter("fldTarikhEjra", Masoulin.fldTarikhEjra,Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).Where(l => l.fldModule_OrganId == Masoulin.fldModule_OrganId).FirstOrDefault();
                    Msg = servic.UpdateMasoulin(Masoulin.fldId, Masoulin.fldTarikhEjra, Masoulin.fldModule_OrganId, /*Detail, */Masoulin.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    
                }
                if (Err.ErrorType)
                {
                    Er = 1;
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
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
        public ActionResult DeleteDetail(string MasuolinId, string OrderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
               var q= servic.GetMasoulin_DetailWithFilter("fldMasuolinId", MasuolinId, 0, IP, out Err).Where(l => l.fldOrderId ==Convert.ToInt32(OrderId)).FirstOrDefault();
               if (q!=null&& q.fldEmployId != null)
               {
                   MsgTitle = "حذف موفق";
                   servic.UpdateMasoulin_Detail(q.fldId, null, null, Convert.ToInt32(MasuolinId), Convert.ToInt32(OrderId), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                  Msg = "حذف با موفقیت انجام شد.";
               }
               else
               {
                   Msg = "";
                   MsgTitle = "";
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
        public ActionResult SaveDetail(List<WCF_Common.OBJ_Masoulin_Detail> Masoulin_DetailList)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";

            try
            {
                MsgTitle = "ویرایش موفق";
                Msg = "ویرایش با موفقیت انجام شد.";
                var q = servic.DeleteMasoulin_Detail(Masoulin_DetailList[0].fldMasuolinId, "Header", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                }
                foreach (var item in Masoulin_DetailList)
                {
                    if (item.fldEmployId == 0)
                        item.fldEmployId = null;
                    if (item.fldOrganPostId == 0)
                        item.fldOrganPostId = null;
                    servic.InsertMasoulin_Detail(item.fldEmployId, item.fldOrganPostId, item.fldMasuolinId, item.fldOrderId, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
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
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMasoulinDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldTarikhEjra = q.fldTarikhEjra,
                fldModule_OrganId = q.fldModule_OrganId,
                fldTitleModule = q.fldTitleModule
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_Masoulin_Detail> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_Masoulin_Detail> data1 = null;
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
                        case "fldGirandeAlarmName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldGirandeAlarmName";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetMasoulin_DetailWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetMasoulin_DetailWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMasoulin_DetailWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_Masoulin_Detail> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Rload(int MasoliyatHeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_Masoulin_Detail> data = null;

            data = servic.GetMasoulin_DetailWithFilter("", "", 100,IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadDetail(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<WCF_Common.OBJ_Masoulin_Detail> data = null;
            //data = servic.GetMasoulin_DetailList(HeaderId, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).ToList();
            data = servic.GetMasoulin_DetailWithFilter("fldMasuolinId", HeaderId.ToString(), 100,IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadHeader(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_Masoulin> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_Masoulin> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTarikhEjra":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhEjra";
                            break;
                        case "fldModule_OrganName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldModule_OrganName";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetMasoulinWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100,IP, out Err).ToList();
                    else
                        data = servic.GetMasoulinWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetMasoulinWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_Masoulin> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

      public ActionResult RloadZirListItem(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_Masuolin_ZirList> data = null;

            data = servic.GetMasuolin_ZirList(Id,IP, out Err).ToList();

            string[] HeaderText = new string[5];
            var q = servic.GetMasoulin_DetailWithFilter("fldMasuolinId", Id.ToString(), 100,IP, out Err).ToList();
            foreach (var Item in q)
                HeaderText[Item.fldOrderId - 1] = Item.NamePostOrgan;

            return Json(new
            {
                GridVal = data,
                HeaderText = HeaderText
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveZirList(List<Models.ZirList> ItemVal, int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";
            try
            {
                //ذخیره
                var q = servic.GetMasoulin_DetailWithFilter("fldMasuolinId", HeaderId.ToString(), 0,IP, out Err).ToList();

                foreach (var I in q)
                    servic.DeleteZirListHa("fldMasuolinId", I.fldId, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                foreach (var item in ItemVal)
                {
                    var L = servic.GetMasoulin_DetailWithFilter("fldMasuolinId", HeaderId.ToString(), 0,IP,  out Err).Where(k => k.fldOrderId == item.OrderId).FirstOrDefault();
                    if (L.NamePostOrgan != " "&&item.ShowTitle==true)
                        servic.InsertZirListHa(item.ReportId, L.fldId, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                Msg = "ذخیره با موفقیت انجام شد.";
                MsgTitle = "ذخیره موفق";


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
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
