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

namespace NewFMS.Areas.Archive.Controllers
{
    public class AssignPropertiesController : Controller
    {
        //
        // GET: /Archive/AssignProperties/
        WCF_Archive.ArchiveService ArchiveService = new WCF_Archive.ArchiveService();
        WCF_Archive.ClsError ArchiveErr = new WCF_Archive.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
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

        public ActionResult NodeLoadArchive(string nod)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var child = ArchiveService.GetArchiveTreeWithFilter("fldPID", nod,"", Convert.ToInt32(Session["OrganId"]), 0, IP, out ArchiveErr).ToList();
            foreach (var ch in child)
            {
                Node childNode = new Node();
                childNode.Text = ch.fldTitle;
                childNode.NodeID = ch.fldId.ToString();
                childNode.Cls = ch.fldFileUpload.ToString();
                nodes.Add(childNode);
            }
            return this.Store(nodes);
        }

        public ActionResult GetProperties()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = ArchiveService.GetPropertiesWithFilter("", "", 0, IP, out ArchiveErr).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldNameFn });
            return this.Store(q);
        }

        public ActionResult Save(WCF_Archive.OBJ_ParticularProperties ParticularProperties)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (ParticularProperties.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = ArchiveService.InsertParticularProperties(ParticularProperties.fldArchiveTreeId, ParticularProperties.fldPropertiesId, ParticularProperties.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ArchiveErr);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = ArchiveService.UpdateParticularProperties(ParticularProperties.fldId, ParticularProperties.fldArchiveTreeId, ParticularProperties.fldPropertiesId, ParticularProperties.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ArchiveErr);
                }
                if (ArchiveErr.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ArchiveErr.ErrorMsg;
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
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = ArchiveService.DeleteParticularProperties(id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ArchiveErr);
                if (ArchiveErr.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ArchiveErr.ErrorMsg;
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
            var q = ArchiveService.GetParticularPropertiesDetail(Id, IP, out ArchiveErr);
            return Json(new
            {
                fldId = q.fldId,
                fldArchiveTreeId = q.fldArchiveTreeId,
                fldTitle = q.fldTitle,
                fldPropertiesId = q.fldPropertiesId,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Archive.OBJ_ParticularProperties> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Archive.OBJ_ParticularProperties> data1 = null;
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
                        case "fldNameFn":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameFn";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = ArchiveService.GetParticularPropertiesWithFilter(field, searchtext, 100, IP, out ArchiveErr).ToList();
                    else
                        data = ArchiveService.GetParticularPropertiesWithFilter(field, searchtext, 100, IP, out ArchiveErr).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = ArchiveService.GetParticularPropertiesWithFilter("", "", 100, IP, out ArchiveErr).ToList();
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
                            return !(oValue.ToString().IndexOf(value.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
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

            List<WCF_Archive.OBJ_ParticularProperties> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
