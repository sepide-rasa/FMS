using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using NewFMS.Areas.Contracts.Models;

namespace NewFMS.Areas.Contracts.Controllers
{
    public class TankhahGardanController : Controller
    {
        // GET: Contracts/TankhahGardan
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService servicCommon = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
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
        public ActionResult ChangeStatus(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            ContractEntities m = new ContractEntities();
            var result = new Ext.Net.MVC.PartialViewResult();
            var q=m.spr_tblTanKhahGardanSelect("fldId", Id.ToString(), 0, 1).FirstOrDefault();
            result.ViewBag.Id = Id.ToString();
            result.ViewBag.Status = Convert.ToByte(!(q.fldStatus)).ToString();
            return result;
        }
        //public ActionResult GetOrgan()
        //{
        //    if (Session["Username"] == null)
        //        return RedirectToAction("logon", "Account", new { area = "" });
        //    var q = servicCommon.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out ErrCommon).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
        //    return this.Store(q);
        //}
        public ActionResult UpdateStatus(int Id,byte Status)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                ContractEntities m = new ContractEntities();
                if (NewFMS.Models.Permission.haveAccess(1294, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                {
                    MsgTitle = "عملیات موفق";
                    Msg = "تغییر وضعیت با موفقیت انجام شد.";
                    m.spr_tblTanKhahGardanUpdate_Status(Id, Convert.ToBoolean(Status), Convert.ToInt32(Session["OrganId"]), Convert.ToInt32(Session["UserId"]), IP);
                }
                else
                {
                    Er = 1;
                    MsgTitle = "خطا";
                    Msg = "شما مجاز به دسترسی نمی باشید.";
                }
            }
            catch (Exception x)
            {
                NewFMS.Models.AutomationEntities m = new NewFMS.Models.AutomationEntities();
                System.Data.Entity.Core.Objects.ObjectParameter ErrorId = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                else
                    InnerException = x.Message;
                m.spr_tblErrorInsert(ErrorId, Session["Username"].ToString(), InnerException, DateTime.Now, IP, Convert.ToInt32(Session["UserId"]), "");
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + ErrorId.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(Models.spr_tblTanKhahGardanSelect Tankhah)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                ContractEntities m = new ContractEntities();
                if (Tankhah.fldDesc == null)
                    Tankhah.fldDesc = "";
                if (Tankhah.fldId == 0)
                {
                    if (NewFMS.Models.Permission.haveAccess(1293, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ذخیره
                        MsgTitle = "ذخیره موفق";
                        Msg = "ذخیره با موفقیت انجام شد.";
                        var q = m.spr_tblTanKhahGardanSelect("fldEmployeeId", Tankhah.fldEmployeeId.ToString(), Convert.ToInt32(Session["OrganId"]), 1).FirstOrDefault();
                        if (q != null)
                        {
                            return Json(new
                            {
                                Msg = "این شخص قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblTanKhahGardanInsert(Tankhah.fldEmployeeId,Convert.ToInt32(Session["OrganId"]),Convert.ToInt32(Session["UserId"]), Tankhah.fldDesc, IP);
                    }
                    else
                    {
                        Er = 1;
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به دسترسی نمی باشید.";
                    }
                }
                else
                {
                    if (NewFMS.Models.Permission.haveAccess(1294, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                    {
                        //ویرایش
                        MsgTitle = "ویرایش موفق";
                        Msg = "ویرایش با موفقیت انجام شد.";
                        var q = m.spr_tblTanKhahGardanSelect("fldEmployeeId", Tankhah.fldEmployeeId.ToString(), Convert.ToInt32(Session["OrganId"]), 1).FirstOrDefault();
                        if (q != null && q.fldId != Tankhah.fldId)
                        {
                            return Json(new
                            {
                                Msg = "این شخص قبلا در سیستم ثبت شده است.",
                                MsgTitle = "خطا",
                                Er = 1
                            }, JsonRequestBehavior.AllowGet);
                        }
                        m.spr_tblTanKhahGardanUpdate(Tankhah.fldId, Tankhah.fldEmployeeId,Convert.ToInt32(Session["OrganId"]),Convert.ToInt32(Session["UserId"]),Tankhah.fldDesc , IP);
                    }
                    else
                    {
                        Er = 1;
                        MsgTitle = "خطا";
                        Msg = "شما مجاز به دسترسی نمی باشید.";
                    }
                }

            }
            catch (Exception x)
            {
                NewFMS.Models.AutomationEntities m = new NewFMS.Models.AutomationEntities();
                System.Data.Entity.Core.Objects.ObjectParameter ErrorId = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                else
                    InnerException = x.Message;
                m.spr_tblErrorInsert(ErrorId, Session["Username"].ToString(), InnerException, DateTime.Now, IP, Convert.ToInt32(Session["UserId"]), "");
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + ErrorId.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
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
                ContractEntities m = new ContractEntities();
                if (NewFMS.Models.Permission.haveAccess(1295, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
                {
                    //حذف
                    MsgTitle = "حذف موفق";
                    Msg = "حذف با موفقیت انجام شد.";
                    m.spr_tblTanKhahGardanDelete(id, Convert.ToInt32(Session["UserId"]));
                }
                else
                {
                    Er = 1;
                    MsgTitle = "خطا";
                    Msg = "شما مجاز به دسترسی نمی باشید.";
                }

            }
            catch (Exception x)
            {
                NewFMS.Models.AutomationEntities m = new NewFMS.Models.AutomationEntities();
                System.Data.Entity.Core.Objects.ObjectParameter ErrorId = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                else
                    InnerException = x.Message;
                m.spr_tblErrorInsert(ErrorId, Session["Username"].ToString(), InnerException, DateTime.Now, IP, Convert.ToInt32(Session["UserId"]), "");
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + ErrorId.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
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
            ContractEntities m = new ContractEntities();
            var q = m.spr_tblTanKhahGardanSelect("fldId", Id.ToString(), 0, 1).FirstOrDefault();
            return Json(new
            {
                fldId = q.fldId,
                fldName_Family=q.fldName_Family,
                fldEmployeeId=q.fldEmployeeId,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (NewFMS.Models.Permission.haveAccess(1292, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"])) == true)
            {
                ContractEntities m = new ContractEntities();
                List<Models.spr_tblTanKhahGardanSelect> data = null;
                data = m.spr_tblTanKhahGardanSelect("", "", Convert.ToInt32(Session["OrganId"]), 0).ToList();

                //-- start paging ------------------------------------------------------------
                int limit = parameters.Limit;

                if ((parameters.Start + parameters.Limit) > data.Count)
                {
                    limit = data.Count - parameters.Start;
                }

                List<Models.spr_tblTanKhahGardanSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
                //-- end paging ------------------------------------------------------------

                return this.Store(rangeData, data.Count);
            }
            else return null;
        }
    }
}