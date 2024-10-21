using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;


namespace NewFMS.Areas.Archive.Controllers
{
    public class ArchiveController : Controller
    {
        //
        // GET: /Archive/Archive/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Archive.ArchiveService ArchiveService = new WCF_Archive.ArchiveService();
        WCF_Archive.ClsError ErrArchive = new WCF_Archive.ClsError();
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
        public ActionResult GetModule_Organ()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetModule_OrganWithFilter("fldOrganId", Session["OrganId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList().Select(c => new { fldId = c.fldModuleId, fldName = c.fldNameModule });
            return this.Store(q);
        }

        public ActionResult NodeLoadArchive(string nod, int ModuleId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var child = ArchiveService.GetArchiveTreeWithFilter("fldPID_Module", nod,ModuleId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out ErrArchive).ToList();
            foreach (var ch in child)
            {
                Node childNode = new Node();
                childNode.Text = ch.fldTitle;
                childNode.NodeID = ch.fldId.ToString();
                nodes.Add(childNode);
            }
            return this.Store(nodes);
        }

        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = ArchiveService.GetArchiveTreeDetail(Id,Convert.ToInt32(Session["OrganId"]),IP, out ErrArchive);
            return Json(new
            {
                fldId = q.fldId,
                fldPID=q.fldPID,
                fldTitle=q.fldTitle,
                fldFileUpload=q.fldFileUpload,
                fldDesc = q.fldDesc,
                fldModuleId=q.fldModuleId.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }

        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                MsgTitle = "حذف موفق";
                Msg = ArchiveService.DeleteArchiveTree(id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrArchive);
                if (ErrArchive.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = ErrArchive.ErrorMsg;
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
        public ActionResult Save(WCF_Archive.OBJ_ArchiveTree ArchiveTree)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; int ArchiveTreeId = 0;
            try
            {
                if (ArchiveTree.fldId == 0)
                {
                    MsgTitle = "ذخیره موفق";
                    Msg = "ذخیره با موفقیت انجام شد.";
                    ArchiveTreeId = ArchiveService.InsertArchiveTree(ArchiveTree.fldPID, ArchiveTree.fldTitle, ArchiveTree.fldFileUpload,Convert.ToByte(ArchiveTree.fldModuleId), ArchiveTree.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrArchive);   
                }
                else
                {
                    MsgTitle = "ویرایش موفق";
                    Msg = ArchiveService.UpdateArchiveTree(ArchiveTree.fldId, ArchiveTree.fldPID, ArchiveTree.fldTitle, ArchiveTree.fldFileUpload,Convert.ToByte(ArchiveTree.fldModuleId), ArchiveTree.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrArchive);
                }
                if (ErrArchive.ErrorType)
                {
                    Msg = ErrArchive.ErrorMsg;
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
                Er = Er,
                ArchiveTreeId = ArchiveTreeId
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
