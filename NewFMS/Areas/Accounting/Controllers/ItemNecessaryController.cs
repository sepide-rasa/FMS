using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;

namespace NewFMS.Areas.Accounting.Controllers
{
    public class ItemNecessaryController : Controller
    {
        //
        // GET: /Accounting/ItemNecessary/

        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"]; 
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
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
        public ActionResult GetMahiyat()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMahiyatWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetTypeHesab()
        {
            var q = servic.GetTypeHesabWithFilter("", "", 0, IP, out Err).ToList().Select(l => new { fldName = l.fldName, fldId = l.fldId });
            return this.Store(q);
        }
        public ActionResult LoadTreeItemNecessary(string nod)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var q = servic.GetItemNecessaryWithFilter("fldPID", nod,"",  0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                asyncNode.Cls = item.fldId.ToString();
                asyncNode.Text = item.fldNameItem;
                asyncNode.AttributesObject = new
                {
                    fldNameItem = item.fldNameItem,
                    fldMahiyatId = item.fldMahiyatId,
                    fldMahiyat_GardeshId = item.fldMahiyat_GardeshId,
                    fldDesc = item.fldDesc,
                    fldId = item.fldId,
                    fldTypeHesabId=item.fldTypeHesabId
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }
        public ActionResult Save(WCF_Accounting.OBJ_ItemNecessary Necessary, int? PID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Necessary.fldId == 0)
                {//ذخیره
                    if (PID == 0)
                    {
                        PID = null;
                    }
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertItemNecessary(PID, Necessary.fldNameItem, Necessary.fldMahiyatId, Necessary.fldMahiyat_GardeshId, Necessary.fldTypeHesabId, Necessary.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateItemNecessary(Necessary.fldId, Necessary.fldNameItem, Necessary.fldMahiyatId, Necessary.fldMahiyat_GardeshId, Necessary.fldTypeHesabId, Necessary.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        public ActionResult DeleteNode(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "حذف موفق";
                Msg = servic.DeleteItemNecessary(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult MoveNodes(int childId, int parentId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                servic.UpdatePID_ItemNecessary(childId, parentId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Er = 1;
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
