using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace NewFMS.Areas.AnbarAmval.Controllers
{
    public class StoreController : Controller
    {
        //
        // GET: /AnbarAmval/Store/
        WCF_AnbarAmval.AnbarAmvalService servic = new WCF_AnbarAmval.AnbarAmvalService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_AnbarAmval.ClsError Err = new WCF_AnbarAmval.ClsError();
        WCF_Common.CommonService servicCom = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCom = new WCF_Common.ClsError();
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

        public ActionResult New(int id, string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult()
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo,
                ViewData = this.ViewData

            };
            result.ViewBag.Id = id;
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult WinTree(string GroupId, string GroupName)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.GroupId = GroupId;
                PartialView.ViewBag.GroupName = GroupName;
                return PartialView;
            }
        }
        public ActionResult LoadTreeAnbarGroup(string nod, string GroupId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var q = servic.GetAnbarTreeWithFilter("fldPID", nod, GroupId, 0, IP, out Err).ToList();
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
                    fldAnbar_treeId=item.fldAnbar_treeId,
                    fldName = item.fldName,
                    fldDesc = item.fldDesc
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }

        public ActionResult MoveNodes( int parentId,int AnbarTreeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                servic.UpdatePID_Anbar_Tree(AnbarTreeId,parentId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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

        public ActionResult DeleteNode(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "حذف موفق";
                Msg = servic.DeleteAnbar_Tree(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        
        public ActionResult Save(WCF_AnbarAmval.OBJ_Anbar Anbar, int? fldPID,string AnbarTree )
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Anbar.fldId == 0)
                {//ذخیره

                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertAnbar(Anbar.fldName, Anbar.fldAddress, Anbar.fldPhone,AnbarTree, Anbar.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {//ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateAnbar(Anbar.fldId, Anbar.fldName, Anbar.fldAddress, Anbar.fldPhone,AnbarTree, Anbar.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Msg = servic.DeleteAnbar(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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

        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAnbarDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName,
                fldAddress = q.fldAddress,
                fldPhone = q.fldPhone,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadTreeAnbar(string nod)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var q = servic.GetAnbarWithFilter("AllTree", nod, 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text = item.fldName;
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }

        public ActionResult ReadTree(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<WCF_AnbarAmval.OBJ_AnbarGroup> data = null;
            data = servic.GetAnbarGroupWithFilter("", "", 0, IP, out Err).ToList();
              return this.Store(data);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<WCF_AnbarAmval.OBJ_Anbar> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_AnbarAmval.OBJ_Anbar> data1 = null;
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
                        case "fldAddress":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAddress";
                            break;
                        case "fldPhone":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPhone";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetAnbarWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    else
                        data = servic.GetAnbarWithFilter(field, searchtext, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetAnbarWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_AnbarAmval.OBJ_Anbar> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

    }
}
