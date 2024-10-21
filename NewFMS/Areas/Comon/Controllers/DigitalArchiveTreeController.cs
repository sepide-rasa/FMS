using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;

namespace NewFMS.Areas.Comon.Controllers
{
    public class DigitalArchiveTreeController : Controller
    {
        //
        // GET: /Comon/DigitalArchiveTree/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        
        public ActionResult Index()
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(WCF_Common.OBJ_DigitalArchiveTreeStructure DigitalArchive)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";
            try
            {
               

                if (DigitalArchive.fldPID == 0)
                    DigitalArchive.fldPID = null;
                if (DigitalArchive.fldModuleId == 0)
                    DigitalArchive.fldModuleId = null;

                if (DigitalArchive.fldId == 0)
                { //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertDigitalArchiveTreeStructure(DigitalArchive.fldPID, DigitalArchive.fldModuleId, DigitalArchive.fldTitle, DigitalArchive.fldAttachFile, DigitalArchive.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                }
                else
                { //ویرایش

                    MsgTitle = "ذخیره موفق";
                    Msg = servic.UpdateDigitalArchiveTreeStructure(DigitalArchive.fldId, DigitalArchive.fldPID, DigitalArchive.fldModuleId, DigitalArchive.fldTitle, DigitalArchive.fldAttachFile, DigitalArchive.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
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

        public ActionResult NodeLoad(string node, string ModuleId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (node == "0")
            {
                var q = servic.GetDigitalArchiveTreeStructureWithFilter("", "", 0,IP, out Err).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldTitle;
                    asyncNode.NodeID = item.fldId.ToString();

                    var child = servic.GetDigitalArchiveTreeStructureWithFilter("fldPID_ModuleId", item.fldModuleId.ToString(), 0,IP, out Err).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldTitle;
                        childNode.NodeID = ch.fldId.ToString();
                        asyncNode.Children.Add(childNode);
                    }
                    nodes.Add(asyncNode);
                }

            }
            else
            {
                var child = servic.GetDigitalArchiveTreeStructureWithFilter("fldPID", node, 0,IP, out Err).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldTitle;
                    childNode.NodeID = ch.fldId.ToString();
                    nodes.Add(childNode);
                }

            }
            return this.Direct(nodes);
        }



        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDigitalArchiveTreeStructureDetail(Id, IP, out Err);

            return Json(new
            {
                fldId = q.fldId,
                fldNodeName = q.fldTitle,
                fldUplodFile = q.fldAttachFile,
                fldModuleId = q.fldModuleId,
                fldModuleName = q.fldModuleName,
                fldPID = q.fldPID,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckPID(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var flag = false;
            var q = servic.GetDigitalArchiveTreeStructureWithFilter("fldid", id.ToString(), 0,IP, out Err).FirstOrDefault();
            if (q != null)
            {
                if (q.fldPID == null)
                    flag = true;
            }
            return Json(new
            {
                flag = flag
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";

            try
            {

                //حذف
                var p = servic.GetDigitalArchiveTreeStructureWithFilter("fldPID", id.ToString(), 0,IP, out Err).FirstOrDefault();
                if (p != null)
                {
                    Msg = "ابتدا زیر شاخه ها را پاک کنید.";
                    MsgTitle = "خطا";
                }
                else
                {

                    MsgTitle = "حذف موفق";
                    Msg = servic.DeleteDigitalArchiveTreeStructure(id, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
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
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
