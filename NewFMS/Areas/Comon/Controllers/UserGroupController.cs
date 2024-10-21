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
    public class UserGroupController : Controller
    {
        //
        // GET: /Comon/UserGroup/
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
        public ActionResult CopyPermi(int id)
        {//باز شدن پنجره
            var k=servic.GetPermissionWithFilter("UserGroupId",id.ToString(),0,Session["Username"].ToString(), Session["Password"].ToString(),Convert.ToInt32(Session["OrganId"]),IP, out Err).Any();
            if (k == true)
            {
                return Json(new
                {
                    Msg = "برای گروه کاربری موردنظر قبلا دسترسی ثبت شده است.",
                    MsgTitle = "خطا",
                    Err=1
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.KhaliId = id;
                return PartialView;
            }
        }
        public ActionResult ShowUser(int UserGroupId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.UserGroupId = UserGroupId;
            return PartialView;
        }
        public ActionResult ReadShowUser(StoreRequestParameters parameters, int UserGroupId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_User_Group> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_User_Group> data1 = null;
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
                            field = "fldName_Active";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli_Active";
                            break;
                        case "fldFatherName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFatherName_Active";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetUser_GroupWithFilter(field, searchtext, 100, IP, out Err).Where(l => l.fldUserGroupId == UserGroupId).ToList();
                    else
                        data = servic.GetUser_GroupWithFilter(field, searchtext, 100, IP, out Err).Where(l => l.fldUserGroupId == UserGroupId).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetUser_GroupWithFilter("fldUserGroupId_Active", UserGroupId.ToString(), 100, IP, out Err).ToList();
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

            List<WCF_Common.OBJ_User_Group> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
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
        public ActionResult GetOrgan_Module()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            NodeCollection nodes = new Ext.Net.NodeCollection();
            var q = servic.GetModule_OrganWithFilter("", "", 0, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).ToList();//.Select(c => new { fldId = c.fldId, fldName = c.fldNameModule_Organ });
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text = item.fldNameModule_Organ;
                asyncNode.Checked = false;
                asyncNode.Leaf = true;
                asyncNode.Icon = Icon.Building;
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }
        public ActionResult Save(WCF_Common.OBJ_UserGroup UserGroup, List<int> Module_Organ)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; int Id = 0;
            try
            {
                if (UserGroup.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Id = servic.InsertUserGroup(UserGroup.fldTitle, UserGroup.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Err = 1,
                            Id = Id
                        }, JsonRequestBehavior.AllowGet);
                    }
                    foreach (var item in Module_Organ)
	                {
                        Msg = servic.InsertUserGroup_ModuleOrgan(Id, item, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا",
                                Err = 1,
                                Id = Id
                            }, JsonRequestBehavior.AllowGet);
                        }
	                }
                }
                else
                {
                    MsgTitle = "ویرایش موفق";
                    var count = 0;
                    var SaveData = servic.GetUserGroup_ModuleOrganWithFilter("fldUserGroupId", UserGroup.fldId.ToString(), 0, IP, out Err).ToList();
                    int[] SaveDataId = new int[SaveData.Count];

                    for (int i = 0; i < SaveData.Count; i++)
                    {
                        SaveDataId[i] = SaveData[i].fldModuleOrganId;
                    }

                    var DeleteRow = SaveDataId.Except(Module_Organ).ToArray();
                    var InsertRow = Module_Organ.Except(SaveDataId).ToArray();

                    if (DeleteRow.Length != 0)
                    {
                        for (int i = 0; i < DeleteRow.Length; i++)
                        {
                            var qq = servic.GetUserGroup_ModuleOrganWithFilter("fldModuleOrganId", DeleteRow[i].ToString(), 0, IP, out Err).Where(l => l.fldUserGroupId == UserGroup.fldId).FirstOrDefault();
                            var permission = servic.GetPermissionWithFilter("fldUserGroup_ModuleOrganID", qq.fldId.ToString(), 1, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]),IP, out Err).FirstOrDefault();
                            if (permission != null)
                            {
                                count = count + 1;
                            }
                        }
                    }
                    if (count == 0)
                    {
                        Msg = servic.UpdateUserGroup(UserGroup.fldId, UserGroup.fldTitle, UserGroup.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا",
                                Err = 1,
                                Id = UserGroup.fldId
                            }, JsonRequestBehavior.AllowGet);
                        }
                        foreach (var item in DeleteRow)
                        {
                            var qq=servic.GetUserGroup_ModuleOrganWithFilter("fldModuleOrganId", item.ToString(), 0, IP, out Err).Where(l=>l.fldUserGroupId==UserGroup.fldId).FirstOrDefault();
                            servic.DeleteUserGroup_ModuleOrgan(qq.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Err = 1,
                                    Id = UserGroup.fldId
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        foreach (var item in InsertRow)
                        {
                            servic.InsertUserGroup_ModuleOrgan(UserGroup.fldId, item, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                            if (Err.ErrorType)
                            {
                                return Json(new
                                {
                                    Msg = Err.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Err = 1,
                                    Id = UserGroup.fldId
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Msg = "کاربر گرامی؛ برای سازمان_ماژولهای حذف شده، دسترسی ثبت شده است و شما قادر به حذف آن نمی باشید. ",
                            MsgTitle = "خطا",
                            Err = 1,
                            Id = UserGroup.fldId,
                            allowEdit = false
                        }, JsonRequestBehavior.AllowGet);
                    }

                    //var userGroupModule_Organ = servic.GetUserGroup_ModuleOrganWithFilter("fldUserGroupId", UserGroup.fldId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                    //foreach (var item in userGroupModule_Organ)
                    //{
                    //    var permission = servic.GetPermissionWithFilter("fldUserGroup_ModuleOrganID", item.fldId.ToString(), 1, Session["Username"].ToString(), Session["Password"].ToString(), out Err).FirstOrDefault();
                    //    if (permission == null)
                    //    {
                    //        count = count + 1;
                    //    }
                    //}
                    //if (count == userGroupModule_Organ.Count)
                    //{
                    //    Msg = servic.UpdateUserGroup(UserGroup.fldId, UserGroup.fldTitle, UserGroup.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                    //    if (Err.ErrorType)
                    //    {
                    //        return Json(new
                    //        {
                    //            Msg = Err.ErrorMsg,
                    //            MsgTitle = "خطا",
                    //            Err = 1,
                    //            Id = UserGroup.fldId
                    //        }, JsonRequestBehavior.AllowGet);
                    //    }
                    //    foreach (var item in userGroupModule_Organ)
                    //    {
                    //        Msg=servic.DeleteUserGroup_ModuleOrgan(item.fldId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                    //        if (Err.ErrorType)
                    //        {
                    //            return Json(new
                    //            {
                    //                Msg = Err.ErrorMsg,
                    //                MsgTitle = "خطا",
                    //                Err = 1,
                    //                Id = UserGroup.fldId
                    //            }, JsonRequestBehavior.AllowGet);
                    //        }
                    //    }
                    //    foreach (var item in Module_Organ)
                    //    {
                    //        Msg=servic.InsertUserGroup_ModuleOrgan(UserGroup.fldId,item,"", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                    //        if (Err.ErrorType)
                    //        {
                    //            return Json(new
                    //            {
                    //                Msg = Err.ErrorMsg,
                    //                MsgTitle = "خطا",
                    //                Err = 1,
                    //                Id = UserGroup.fldId
                    //            }, JsonRequestBehavior.AllowGet);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    return Json(new
                    //    {
                    //        Msg = "کاربر گرامی؛ برای سازمان_ماژولهای حذف شده، دسترسی ثبت شده است و شما قادر به حذف آن نمی باشید. ",
                    //        MsgTitle = "خطا",
                    //        Err = 1,
                    //        Id = UserGroup.fldId,
                    //        allowEdit=false
                    //    }, JsonRequestBehavior.AllowGet);
                    //}
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
                Err = Er,
                Id = Id
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
                Msg = servic.DeleteUserGroup(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            List<int> Organ_Module = new List<int>();
            var q = servic.GetUserGroupDetail(Id,  IP, out Err);
            var q1 = servic.GetUserGroup_ModuleOrganWithFilter("fldUserGroupId", Id.ToString(), 0, IP, out Err).ToList();
            foreach (var item in q1)
            {
                Organ_Module.Add(item.fldModuleOrganId);
            }
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldDesc = q.fldDesc,
                Organ_Module = Organ_Module
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_UserGroup> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_UserGroup> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId_ByUserId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle_ByUserId";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc_ByUserId";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetUserGroupWithFilter(field, searchtext, 100, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();
                    else
                        data = servic.GetUserGroupWithFilter(field, searchtext, 100, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetUserGroupWithFilter("ByUserId", "", 100, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();
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

            List<WCF_Common.OBJ_UserGroup> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult ReadUserGps(StoreRequestParameters parameters,int UserGp)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_UserGroup> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_UserGroup> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId_ByUserId";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle_ByUserId";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc_ByUserId";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetUserGroupWithFilter(field, searchtext, 100, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err).Where(l => l.fldId != UserGp).ToList();
                    else
                        data = servic.GetUserGroupWithFilter(field, searchtext, 100, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err).Where(l => l.fldId != UserGp).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetUserGroupWithFilter("ByUserId", "", 100, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err).Where(l => l.fldId != UserGp).ToList();
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

            List<WCF_Common.OBJ_UserGroup> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SaveCopyPermi(string UserGroups,int Khali)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                MsgTitle = "ذخیره موفق";
                var GroupHa = UserGroups.Split(';');
                for (int i = 0; i < GroupHa.Length-1; i++)
                {
                    Msg = servic.CopyPermission(Convert.ToInt32(GroupHa[i]), Khali, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                }
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
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
    }
}
