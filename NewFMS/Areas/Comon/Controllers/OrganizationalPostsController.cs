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
    public class OrganizationalPostsController : Controller
    {
        //
        // GET: /Comon/OrganizationalPosts/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_Personeli.PersoneliService servic_P = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError Err_P = new WCF_Personeli.ClsError();
        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            return new Ext.Net.MVC.PartialViewResult();

        }
        public ActionResult IndexNew(string containerId)
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
        public ActionResult IndexNew2(string containerId)
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
        public ActionResult ShowPersonal(int OrgPostId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.OrgPostId = OrgPostId;
            return PartialView;
        }
        public ActionResult Save(WCF_Common.OBJ_OrganizationalPosts p)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            if (p.fldPID == 0)
                p.fldPID = null;
            try
            {
                if (p.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertOrganizationalPosts(p.fldTitle, p.fldOrgPostCode, p.fldChartOrganId,/*p.fldPID*/null, p.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateOrganizationalPosts(p.fldId, p.fldTitle, p.fldOrgPostCode, p.fldChartOrganId,/*p.fldPID*/null, p.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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

        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteOrganizationalPosts(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetOrganizationalPostsDetail(Id, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldChartOrganId=q.fldChartOrganId,
                fldOrgPostCode=q.fldOrgPostCode,
                fldPID=q.fldPID,
                fldNameChart=q.fldNameChart,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult NodeLoad(string nod)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();

            if (nod == "0")
            {
                var q = servic.GetChartOrganWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldTitle;
                    asyncNode.NodeID = item.fldId.ToString();

                    var child = servic.GetChartOrganWithFilter("fldPID", item.fldId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldTitle;
                        childNode.NodeID = ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        asyncNode.Children.Add(childNode);
                    }

                    nodes.Add(asyncNode);
                }

            }
            else
            {
                var child = servic.GetChartOrganWithFilter("fldPID", nod, 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldTitle;
                    childNode.NodeID = ch.fldId.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    nodes.Add(childNode);

                }

            }
            return this.Direct(nodes);
        }
        public ActionResult NodeLoadPost(string nod, int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();

            if (nod == "0")
            {
                var q = servic.GetOrganizationalPostsWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();
                if (OrganId!=0)
                    q = servic.GetOrganizationalPostsWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).Where(l => l.fldChartOrganId == OrganId).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldTitle;
                    asyncNode.NodeID = item.fldId.ToString();

                    var child = servic.GetOrganizationalPostsWithFilter("fldPID", item.fldId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();
                    if (OrganId != 0)
                        child = servic.GetOrganizationalPostsWithFilter("fldPID", item.fldId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).Where(l => l.fldChartOrganId == OrganId).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldTitle;
                        childNode.NodeID = ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        asyncNode.Children.Add(childNode);
                    }

                    nodes.Add(asyncNode);
                }

            }
            else
            {
                var child = servic.GetOrganizationalPostsWithFilter("fldPID", nod, 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();
                //if (OrganId != 0)
                //    child = servic.GetOrganizationalPostsWithFilter("fldPID", nod, 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).Where(l => l.fldChartOrganId == OrganId).ToList();
                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldTitle;
                    childNode.NodeID = ch.fldId.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    nodes.Add(childNode);

                }

            }
            return this.Direct(nodes);
        }
        public ActionResult ReadShowPersonal(StoreRequestParameters parameters, int OrgPostId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_PersonalInfo> data = null;

            data = servic_P.GetPersoneliInfoWithFilter("fldOrganPostId", OrgPostId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err_P).ToList();

            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Personeli.OBJ_PersonalInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            return this.Store(rangeData, data.Count);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_OrganizationalPosts> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_OrganizationalPosts> data1 = null;
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
                        case "fldOrgPostCode":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldOrgPostCode";
                            break;
                        case "fldNameChart":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameChart";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetOrganizationalPostsWithFilter(field, searchtext, 100, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();
                    else
                        data = servic.GetOrganizationalPostsWithFilter(field, searchtext, 100, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetOrganizationalPostsWithFilter("All", "", 100, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();
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

            List<WCF_Common.OBJ_OrganizationalPosts> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
