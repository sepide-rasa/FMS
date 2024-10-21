using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace NewFMS.Areas.AnbarAmval.Controllers
{
    public class GoodsGroupController : Controller
    {
        //
        // GET: /AnbarAmval/GoodsGroup/

        WCF_AnbarAmval.AnbarAmvalService servic = new WCF_AnbarAmval.AnbarAmvalService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_AnbarAmval.ClsError Err = new WCF_AnbarAmval.ClsError();
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

        public ActionResult TreeGoods(int GroupId, string GroupName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.GroupId = GroupId;
            result.ViewBag.GroupName = GroupName;
            return result;
        }
        public ActionResult Kala_TreeWin(int GroupId, string GroupName)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.GroupId = GroupId;
            result.ViewBag.GroupName = GroupName;
            return result;
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
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetKalaGroupDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(WCF_AnbarAmval.OBJ_KalaGroup KalaGroup)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (KalaGroup.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertKalaGroup(KalaGroup.fldName, KalaGroup.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateKalaGroup(KalaGroup.fldId, KalaGroup.fldName, KalaGroup.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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

        public ActionResult SaveTree(WCF_AnbarAmval.OBJ_KalaTree KalaTree, int? PID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (KalaTree.fldId == 0)
                {//ذخیره
                    if (PID == 0)
                    {
                        PID = null;
                    }
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertKalaTree(PID, KalaTree.fldName, KalaTree.fldGroupId, KalaTree.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateKalaTree(KalaTree.fldId, KalaTree.fldName, KalaTree.fldGroupId, KalaTree.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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

        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "حذف موفق";
                Msg = servic.DeleteKalaGroup(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Msg = servic.DeleteKalaTree(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Loadkala_TarifNashode(string nod, string GroupId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "0")
            {
                var q = servic.Getkala_TarifNashode(Convert.ToInt32(GroupId), IP, out Err).ToList();
                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Cls = item.kalaId.ToString();
                    asyncNode.Text = item.fldName;
                    asyncNode.Icon = Icon.Building;
                    asyncNode.AttributesObject = new
                    {
                        kalaId = item.kalaId,
                        fldName = item.fldName
                    };
                    nodes.Add(asyncNode);
                }
            }
            
            return this.Store(nodes);
        }
        public ActionResult LoadTreeKala(string nod, string GroupId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var q = servic.GetKalaTreeWithFilter("fldPID", nod, GroupId, 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                asyncNode.Cls = item.fldId.ToString();
                asyncNode.Text = item.fldName;
                if (item.fldNodeType == 1)//ساختار درختی
                {
                    asyncNode.Icon = Icon.ApplicationSideTree;
                }
                else//انبار
                {
                    asyncNode.Icon = Icon.Building;
                }
                asyncNode.AttributesObject = new
                {
                    fldNodeType = item.fldNodeType,
                    fldName = item.fldName,
                    fldDesc = item.fldDesc,
                    fldId=item.fldId
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }
        public ActionResult SaveKala_Tree(int KalaId, int KalaTreeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                servic.InsertKala_Tree(KalaId, KalaTreeId,"", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult MoveNodes(int childId, int parentId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                servic.UpdatePID_kala(childId, parentId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<WCF_AnbarAmval.OBJ_KalaGroup> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_AnbarAmval.OBJ_KalaGroup> data1 = null;
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
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetKalaGroupWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetKalaGroupWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetKalaGroupWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_AnbarAmval.OBJ_KalaGroup> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
