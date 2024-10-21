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
    public class PcPosInfoController : Controller
    {
        //
        // GET: /Daramad/PcPosInfo/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];

        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null || Session["OrganId"]==null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var organ = servic.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.OrganId = organ.fldId;
            result.ViewBag.OrganName = organ.fldName;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        public ActionResult GetPSPModel()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicD.GetPspModelWithFilter("", "", 0, IP, out ErrD).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitleModel });
            return this.Store(q);
        }

        //public ActionResult NodeLoadGroup(string nod)
        //{
        //    NodeCollection nodes = new Ext.Net.NodeCollection();
        //    if (nod == "0")
        //    {
        //        var q = servic.GetOrganizationWithFilter("fldPIdOrgan", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();

        //        foreach (var item in q)
        //        {
        //            Node asyncNode = new Node();
        //            asyncNode.Text = item.fldName;
        //            asyncNode.NodeID = item.fldId.ToString();
        //            asyncNode.Expanded = true;

        //            var child = servic.GetOrganizationWithFilter("fldPId", item.fldId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
        //            foreach (var ch in child)
        //            {
        //                Node childNode = new Node();
        //                childNode.Text = ch.fldName;
        //                childNode.NodeID = ch.fldId.ToString();
        //                childNode.Icon = Ext.Net.Icon.Building;
        //                childNode.Expanded = true;
        //                asyncNode.Children.Add(childNode);
        //            }
        //            nodes.Add(asyncNode);
        //        }
        //    }
        //    else
        //    {
        //        var child = servic.GetOrganizationWithFilter("fldPId", nod, 0, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).ToList();

        //        foreach (var ch in child)
        //        {
        //            Node childNode = new Node();
        //            childNode.Text = ch.fldName;
        //            childNode.NodeID = ch.fldId.ToString();
        //            childNode.Icon = Ext.Net.Icon.Building;
        //            childNode.Expanded = true;
        //            nodes.Add(childNode);
        //        }
        //    }
        //    return this.Store(nodes);
        //}
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null || Session["OrganId"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Daramad.OBJ_PcPosInfo> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_PcPosInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldPspId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldPspId";
                            break;
                        case "fldOrganId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldOrganId";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
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
                        data1 = servicD.GetPcPosInfoWithFilter(field, searchtext,Session["OrganId"].ToString(), 100, IP, out ErrD).ToList();
                    else
                        data = servicD.GetPcPosInfoWithFilter(field, searchtext,Session["OrganId"].ToString(), 100, IP, out ErrD).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicD.GetPcPosInfoWithFilter("", "",Session["OrganId"].ToString(),100, IP, out ErrD).ToList();
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

            List<WCF_Daramad.OBJ_PcPosInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult Save(WCF_Daramad.OBJ_PcPosInfo PcPosInfo)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (PcPosInfo.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    Msg = servicD.InsertPcPosInfo(PcPosInfo.fldPspId, PcPosInfo.fldOrganId, PcPosInfo.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = servicD.UpdatePcPosInfo(PcPosInfo.fldId, PcPosInfo.fldPspId, PcPosInfo.fldOrganId, PcPosInfo.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);

                }
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

        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null || Session["OrganId"]==null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicD.GetPcPosInfoDetail(Id,Session["OrganId"].ToString(), IP, out ErrD);
            return Json(new
            {
                fldId = q.fldId,
                fldPspId = q.fldPspId.ToString(),
                fldOrganId = q.fldOrganId,
                fldDesc = q.fldDesc
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
                Msg = servicD.DeletePcPosInfo(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
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
