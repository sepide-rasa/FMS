using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using Aspose.Cells;
using System.Data.SqlClient;
using System.Data;
using NewFMS.DataSet;
using System.Collections;
namespace NewFMS.Areas.Daramad.Controllers
{
    public class PcPosIPController : Controller
    {
        //
        // GET: /Daramad/PcPosIP/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();
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
        public ActionResult GetPcPosInfo()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicD.GetPcPosInfoWithFilter("", "",Session["OrganId"].ToString(), 0, IP, out ErrD).ToList().Select(c => new { fldID = c.fldId, fldOrganId = c.fldOrganId, fldName = c.fldPspName + "(" + c.fldOrganName + ")" }).OrderBy(x => x.fldName);
            return this.Store(q);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_PcPosIP> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_PcPosIP> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldPcPosInfoId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldPcPosInfoId";
                            break;
                        case "fldIp":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldIp";
                            break;
                        case "fldSerial":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSerial";
                            break;
                        case "fldPspName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPspName";
                            break;
                        case "fldOrganName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOrganName";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servicD.GetPcPosIPWithFilter(field, searchtext, (Session["OrganId"]).ToString(), 100, IP, out ErrD).ToList();
                    else
                        data = servicD.GetPcPosIPWithFilter(field, searchtext, (Session["OrganId"]).ToString(), 100, IP, out ErrD).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicD.GetPcPosIPWithFilter("", "",(Session["OrganId"]).ToString(), 100, IP, out ErrD).ToList();
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

            List<WCF_Daramad.OBJ_PcPosIP> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(WCF_Daramad.OBJ_PcPosIP PcPosIP, string fldIdUsers)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; int Id = 0;
            try
            {
                List<string> UserIdd = new List<string>();
                UserIdd = fldIdUsers.Split(';').Reverse().Skip(1).Reverse().ToList();
                if (PcPosIP.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    Msg="ذخیره با موفقیت انجام شد";
                    Id = servicD.InsertPcPosIP(PcPosIP.fldPcPosInfoId, PcPosIP.fldSerial, PcPosIP.fldIp, PcPosIP.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrD.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    foreach (var item in UserIdd)
                    {
                        Msg = servicD.InsertPcPosUser(Id, Convert.ToInt32(item), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                        if (ErrD.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = ErrD.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = servicD.UpdatePcPosIP(PcPosIP.fldId, PcPosIP.fldPcPosInfoId, PcPosIP.fldSerial, PcPosIP.fldIp, PcPosIP.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    if (ErrD.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = ErrD.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    Msg = servicD.DeletePcPosUser("fldPosIpId", PcPosIP.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                    foreach (var item in UserIdd)
                    {
                        Msg = servicD.InsertPcPosUser(PcPosIP.fldId, Convert.ToInt32(item), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                        if (ErrD.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = ErrD.ErrorMsg,
                                MsgTitle = "خطا",
                                Er = 1
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

        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<int> UserIds = new List<int>();
            List<string> NameUser = new List<string>();
            string UserPcPosSelectedIds = "";
            var q = servicD.GetPcPosIPDetail(Id, (Session["OrganId"]).ToString(), IP, out ErrD);
            var q1 = servicD.GetPcPosUserWithFilter("fldPosIpId",Id.ToString(),0,IP, out ErrD).ToList();
            foreach (var item in q1)
            {
                UserIds.Add(item.fldId);
                NameUser.Add(item.fldUserName);
                UserPcPosSelectedIds = UserPcPosSelectedIds + item.fldIdUser + ";";
            }
            return Json(new
            {
                fldId = q.fldId,
                fldPcPosInfoId = q.fldPcPosInfoId.ToString(),
                fldSerial = q.fldSerial,
                fldIp = q.fldIp,
                fldDesc = q.fldDesc,
                UserIds=UserIds,
                NameUser=NameUser,
                UserPcPosSelectedIds = UserPcPosSelectedIds
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MsgTitle = "حذف موفق";
                Msg = servicD.DeletePcPosIP(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                if (ErrD.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrD.ErrorMsg;
                    Er = 1;
                }
                Msg = servicD.DeletePcPosUser("fldPosIpId", Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
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

    }
}
