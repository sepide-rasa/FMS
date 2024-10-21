using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using Newtonsoft.Json;

namespace NewFMS.Areas.Deceased.Controllers
{
    public class KartablController : Controller
    {
        //
        // GET: /Deceased/Kartabl/

        WCF_Deceased.DeceasedService servic = new WCF_Deceased.DeceasedService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Deceased.ClsError Err = new WCF_Deceased.ClsError();

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
        public ActionResult New(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }


        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult SelectActions(int KartablId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.KartablId = KartablId;
            return PartialView;
        }
        public ActionResult Save(WCF_Deceased.OBJ_Kartabl Kartabl)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Kartabl.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertKartabl(Kartabl.fldTitleKartabl,Convert.ToBoolean(Kartabl.fldEbtal),Convert.ToBoolean(Kartabl.fldEtmam), Kartabl.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateKartabl(Kartabl.fldId,Kartabl.fldTitleKartabl,Convert.ToBoolean(Kartabl.fldEbtal),Convert.ToBoolean(Kartabl.fldEtmam), Kartabl.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Msg = servic.DeleteKartabl(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetKartablDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldTitleKartabl = q.fldTitleKartabl,
                fldDesc = q.fldDesc,
                fldEbtal = q.fldEbtal,
                fldEtmam = q.fldEtmam
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Deceased.OBJ_Kartabl> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Deceased.OBJ_Kartabl> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTitleKartabl":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitleKartabl";
                            break;
                        case "fldTitleEbtal":
                            searchtext =ConditionValue.Value.ToString();
                            field = "fldTitleEbtal";
                            break;
                        case "fldTitleEtmam":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldTitleEtmam";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetKartablWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetKartablWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetKartablWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Deceased.OBJ_Kartabl> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult LoadSeletedActions(int KartablId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var Data = servic.GetActionsWithFilter("", (Session["OrganId"]).ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            var check = servic.GetAction_KartablWithFilter("fldKartablId", KartablId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
            int[] checkId = new int[check.Count];

            for (int i = 0; i < check.Count; i++)
            {
                checkId[i] = check[i].fldActionId;
            }
            return Json(new
            {
                Data = Data,
                checkId2 = checkId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveActions(List<WCF_Deceased.OBJ_Action_Kartabl> cods, int KartablId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;

            try
            {
                var savedata = servic.GetAction_KartablWithFilter("fldKartablId", KartablId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                foreach (var item in savedata)
                    servic.DeleteAction_Kartabl(item.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Er = 1,
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                }



                foreach (var item in cods)
                {
                    if (item.fldId == 0)
                    {
                        servic.InsertAction_Kartabl(item.fldActionId, KartablId, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }

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
                Er = 1;
            }
            return Json(new
            {
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveOrder(List<WCF_Deceased.OBJ_Kartabl> Order)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {
                string Msg = "", MsgTitle = ""; byte Er = 0;
                try
                {

                   foreach (var item in Order)
                    {
                        servic.UpdateOrderKartabl(item.fldId, Convert.ToInt32(item.fldorderid), Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                        if (Err.ErrorType == true)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                   Msg = "ذخیره ترتیب با موفقیت انجام شد.";
                   MsgTitle = "عملیات موفق";
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
}
