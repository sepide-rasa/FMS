using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;

namespace NewFMS.Areas.Comon.Controllers
{
    public class PermissionController : Controller
    {
        //
        // GET: /Comon/Permission/
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
        public ActionResult UserforPermission(string containerId)
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
        public ActionResult ShowUsers(int ApplicationPartId, int ModuleOrg)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.ApplicationPartId = ApplicationPartId;
            result.ViewBag.ModuleOrg = ModuleOrg;
            return result;
        }
        public ActionResult ReadUsers(StoreRequestParameters parameters, int ApplicationPartId, int ModuleOrg)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            List<WCF_Common.OBJ_RptDastresiKarbaran> data = null;
            data = servic.GetrptDastresiKarbaranWithFilter(ApplicationPartId, ModuleOrg, IP, out Err).ToList();
            return this.Store(data);
        }
        public ActionResult GetUserGroup()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetUserGroupWithFilter("ByUserId", "", 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetModule(string userGroup)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
           // var q = servic.GetModuleWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldTitle });
            var q = servic.GetUserGroup_ModuleOrganWithFilter("fldUserGroupId", userGroup, 0, IP, out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldModuleOrganName });
            return this.Store(q);
        }
        public ActionResult GetModuleForUser()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            // var q = servic.GetModuleWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldTitle });
            var q = servic.GetModule_OrganWithFilter("ByUserId", Session["UserId"].ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList().Select(n => new { ID = n.fldId, Name = n.fldNameModule_Organ });
            return this.Store(q);
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public JsonResult ReloadDrp()
        {//نمایش اطلاعات جهت رویت کاربر

            var q = servic.GetUserGroupWithFilter("ByUserId", "", 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).FirstOrDefault();

            return Json(new
            {
                fldUserGroupId = q.fldId.ToString(),

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SavePermission(List<WCF_Common.OBJ_Permission> Permission, int UserGroup_ModuleOrganID, int ModuleID, string UnChecked)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";
            try
            {
                /*if (c.Permission(22, Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                {*/
                var q = servic.GetPermissionWithFilter("fldUserGroup_ModuleOrganID", UserGroup_ModuleOrganID.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]),IP, out Err).ToList();
                if (Err.ErrorType == true)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                }
                if (q.Count() != 0)
                {
                    servic.DeletePermission(UserGroup_ModuleOrganID, ModuleID, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                    if (Err.ErrorType == true)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                    }

                    //حذف این دسترسی از بقیه زیرمجموعه ها
                    servic.DeleteChildUserGroupPermission(UserGroup_ModuleOrganID, UnChecked, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                    if (Err.ErrorType == true)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                    }

                }

                if (Permission != null)
                {
                    foreach (var item in Permission)
                    {
                        Msg = servic.InsertPermission(item.fldUserGroup_ModuleOrganID, item.fldApplicationPartId, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                        if (Err.ErrorType == true)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                MsgTitle = "ذخیره موفق";
                Msg = "ذخیره با موفقیت انجام شد.";
                if (Err.ErrorType == true)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                }
                //}
                //else
                //{
                //    return null;
                //}
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
        public ActionResult NodeLoad(string node, string GrohKarbari, string UserGroup_ModuleOrganID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var child = servic.GetApplictionPartWithFilter("fldPId", node, UserGroup_ModuleOrganID, 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();

            foreach (var ch in child)
            {
                Node childNode = new Node();
                childNode.Text = ch.fldTitle;
                childNode.NodeID = ch.fldId.ToString();
                childNode.Icon = Ext.Net.Icon.Building;
                childNode.Checked = false;
                var f = servic.GetPermissionWithFilter("fldUserGroup_ModuleOrganID", UserGroup_ModuleOrganID.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]),IP, out Err).ToList();
                if (f.Count() != 0)
                {
                    foreach (var _item in f)
                    {
                        if (_item.fldApplicationPartId == ch.fldId)
                        {
                            childNode.Checked = true;
                        }
                    }
                }
                nodes.Add(childNode);
            }
            return this.Store(nodes);
        }
        public ActionResult NodeLoadGroupTreePermissionforUser(string node, string UserGroup_ModuleOrganID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var child = servic.GetApplictionPartWithFilter("fldPId", node, UserGroup_ModuleOrganID, 0, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();

            foreach (var ch in child)
            {
                Node childNode = new Node();
                childNode.Text = ch.fldTitle;
                childNode.NodeID = ch.fldId.ToString();
                childNode.Icon = Ext.Net.Icon.UserMagnify;
                nodes.Add(childNode);
            }
            return this.Store(nodes);
        }
    }
}
