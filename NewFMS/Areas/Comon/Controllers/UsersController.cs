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

namespace NewFMS.Areas.Comon.Controllers
{
    public class UsersController : Controller
    {
        //
        // GET: /Comon/Users/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        WCF_PayRoll.PayRolService PayServic = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError ErrPay = new WCF_PayRoll.ClsError();

        WCF_Personeli.PersoneliService PersoneliService = new WCF_Personeli.PersoneliService();
        WCF_Personeli.ClsError ErrPersoneli = new WCF_Personeli.ClsError();

        WCF_Common_Pay.Comon_PayService Common_Payservic = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError ErrC_P = new WCF_Common_Pay.ClsError();
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }

        public ActionResult SaveResetPass(int fldId)
        {
            string Msg = "", MsgTitle = "";byte Er=0;
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            try
            {
                var q = servic.GetUserDetail(fldId,  IP, out Err);
                var pass = servic.HashPass(q.fldUserName);
                servic.UserPassUpdate(q.fldId, pass, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        Msg = Err.ErrorMsg,
                        MsgTitle="خطا",
                        Er=1
                    }, JsonRequestBehavior.AllowGet);
                }
                Msg = "بازنشانی رمز عبور با موفقیت انجام شد.(پیش فرض نام کاربری)";
                MsgTitle = "ویرایش موفق";
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_User> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_User> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;

                        case "fldNameEmployee":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameEmployee";
                            break;

                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;

                        case "fldNameOrgan":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameOrgan";
                            break;

                        case "fldActive_DeactiveName":
                            searchtext =  ConditionValue.Value.ToString()+"%";
                            field = "fldActive_DeactiveName";
                            break;
                        case "fldUserName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldUserName";
                            break;

                    }
                    if (data != null)
                        data1 = servic.SelectUserByUserId(field, searchtext,  100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                    else
                        data = servic.SelectUserByUserId(field, searchtext, 100, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.SelectUserByUserId("", "",100,Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
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

            List<WCF_Common.OBJ_User> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult New(int Id)
        {//باز شدن تب
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.Id = Id;
            return result;
        }

        public ActionResult NodeLoadGroup(string nod,int UserId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "0")
            {
                var q = servic.GetUserGroupTreeWithFilter(UserId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).ToList();
                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    //asyncNode.Text = item.fldTitle;
                    asyncNode.NodeID = item.fldId.ToString();
                    asyncNode.Icon = Ext.Net.Icon.Group;
                    //asyncNode.Leaf = true;
                    //asyncNode.Checked = false;
                    asyncNode.AttributesObject = new { fldTitle = item.fldTitle, fldGrant = item.fldGrant, fldWithGrant = item.fldWithGrant };
                    /*var f = servic.GetUser_GroupWithFilter("fldUserSelectId", UserId.ToString(), 0, Session["Username"].ToString(), Session["Password"].ToString(), out Err).ToList();
                    if (f.Count() != 0)
                    {
                        foreach (var _item in f)
                        {
                            if (_item.fldUserGroupId == item.fldId && _item.fldUserSelectId == UserId)
                            {
                                asyncNode.Checked = true;
                            }
                        }
                    }*/
                    nodes.Add(asyncNode);
                }
            }
            return this.Store(nodes);
        }

        //public ActionResult getImage(int FileId)
        //{//برگرداندن عکس 
        //    byte[] Image = null;
        //    if (FileId == 0)
        //    {
        //        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Server.MapPath("~/Content/Blank.jpg")));
        //        Image = stream.ToArray();
        //    }
        //    else
        //    {
        //        Image = servic.GetFileWithFilter("fldId", FileId.ToString(), 1, Session["Username"].ToString(), Session["Password"].ToString(), out Err).FirstOrDefault().fldImage;
        //    }
        //    return Json(new { image = Convert.ToBase64String(Image) });
        //}

        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var q = servic.GetUserWithFilter("fldId", Id.ToString(), "", 1,IP, out Err).FirstOrDefault();
            var Employee = servic.GetEmployeeDetail(q.fldEmployeeId,  IP, out Err);
            var usergroup = servic.GetUser_GroupWithFilter("fldUserSelectId", Id.ToString(), 0,IP, out Err).ToList();
            List<int> Checked = new List<int>();
            foreach (var item in usergroup)
            {
                Checked.Add(item.fldId);
            }
            var Active_Deactive = "0";
            if (q.fldActive_Deactive == true)
                Active_Deactive = "1";
            return Json(new
            {
                fldId = q.fldId,
                fldEmployeeId = q.fldEmployeeId,
                fldUserName = q.fldUserName,
                fldPassword = q.fldPassword,
                fldFamilyName = q.fldNameEmployee,
                //fldNameOrgan=q.fldNameOrgan,
                //fldNameFather=Employee.fldFatherName,
                //fldOrganId=q.fldOrganId,
                fldActive_Deactive = Active_Deactive,
                fldDesc = q.fldDesc,
                Checked = Checked
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(WCF_Common.OBJ_User user, List<WCF_Common.OBJ_User_Group> UserGroup)
        {
            string Msg = "", MsgTitle = ""; byte Er = 0;
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            try
            {
                if (user.fldDesc == null)
                    user.fldDesc = "";

                if (user.fldId == 0)
                { //ذخیره
                    var pass=CodeDecode.GenerateHash(user.fldUserName);
                    var Id = servic.InsertUser(user.fldEmployeeId, user.fldUserName, pass, user.fldActive_Deactive, user.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ذخیره موفق";
                    Msg = "ذخیره با موفقیت انجام شد.";
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }                    
                    foreach (var item in UserGroup)
                    {
                        servic.InsertUser_Group(item.fldUserGroupId, Id,item.fldGrant,item.fldWithGrant, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                { //ویرایش
                    Msg = servic.UpdateUser(user.fldId, user.fldEmployeeId, user.fldUserName, user.fldActive_Deactive, user.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ویرایش موفق";
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    servic.DeleteUser_Group(user.fldId, Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    foreach (var item in UserGroup)
                    {
                        servic.InsertUser_Group(item.fldUserGroupId,user.fldId,item.fldGrant,item.fldWithGrant,"", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
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
                Er = 1;
            }
            return Json(new
            {
                Er=Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DisableUser(string UserName)
        {
            WCF_Common.OBJ_User obj = new WCF_Common.OBJ_User();
            byte Er = 0; string Msg = "", MsgTitle = ""; 
            try
            {
                var user = servic.GetUserWithFilter("fldUserName", UserName, "", 0, IP, out Err).FirstOrDefault();
                if (user != null)
                {
                    Msg=servic.UpdateActiveUser(user.fldId, false, IP, out Err);
                    MsgTitle = "عملیات موفق";
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception x)
            {
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                else
                    InnerException = x.Message;
                //var error = servic.Error(InnerException, "", IP);
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = InnerException,
                    Er = 1
                });
            }
            return Json(new
            {
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
