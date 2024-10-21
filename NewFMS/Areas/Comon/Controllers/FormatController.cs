using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using System.IO;

namespace NewFMS.Areas.Comon.Controllers
{
    public class FormatController : Controller
    {
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService Cservic = new WCF_Common.CommonService();
        WCF_Common.ClsError CErr = new WCF_Common.ClsError();
        //
        // GET: /Archive/Format/

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
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Save(WCF_Common.OBJ_FormatFile Format)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; byte[] file = null;
            try
            {
                if (Format.fldId == 0)
                {
                    //ذخیره
                    if (Session["savePathFormat"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathFormat"].ToString()));
                        file = stream.ToArray();
                    }
                    else
                    {
                        var Image = Server.MapPath("~/Content/Blank.jpg");
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
                        file = stream.ToArray();
                    }
                    MsgTitle = "ذخیره موفق";
                    Msg = Cservic.InsertFormatFile(Format.fldFormatName, file, Format.fldPassvand,Format.fldSize, Format.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out CErr);
                }
                else
                {
                    //ویرایش
                    if (Session["savePathFormat"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathFormat"].ToString()));
                        file = stream.ToArray();
                    }
                    else
                    {
                        var FormatFile = Cservic.GetFormatFileDetail(Format.fldId, Convert.ToInt32(Session["OrganId"]), IP, out CErr);
                        if (CErr.ErrorType)
                        {
                            return Json(new
                            {
                                MsgTitle = "خطا",
                                Msg = CErr.ErrorMsg,
                                Er = 1
                            });
                        }
                        else
                        {
                            file = FormatFile.fldIcon;
                        }
                    }
                    MsgTitle = "ویرایش موفق";
                    Msg = Cservic.UpdateFormatFile(Format.fldId, Format.fldFormatName, file, Format.fldPassvand,Format.fldSize, Format.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out CErr);
                }
                if (CErr.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = CErr.ErrorMsg;
                    Er = 1;
                }
                if (Session["savePathFormat"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathFormat"].ToString());
                    Session.Remove("savePathFormat");
                    System.IO.File.Delete(physicalPath);
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
                if (Session["savePathFormat"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathFormat"].ToString());
                    Session.Remove("savePathFormat");
                    System.IO.File.Delete(physicalPath);
                }
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
                //حذف
                MsgTitle = "حذف موفق";
                Msg = Cservic.DeleteFormatFile(id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out CErr);
                if (CErr.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = CErr.ErrorMsg;
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

        public ActionResult Upload()
        {
            string Msg = "";
            try
            {
                if (Session["Username"] == null)
                    return RedirectToAction("logon", "Account", new { area = "" });

                if (Session["savePathFormat"] != null)
                {
                    System.IO.File.Delete(Session["savePathFormat"].ToString());
                    Session.Remove("savePathFormat");
                }
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".tif"
                || extension == ".tiff")
                {
                    if (Request.Files[0].ContentLength <= 1024000)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        var Name = Guid.NewGuid();
                        string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                        file.SaveAs(savePath);
                        Session["savePathFormat"] = savePath;
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        X.Msg.Show(new MessageBoxConfig
                        {
                            Buttons = MessageBox.Button.OK,
                            Icon = MessageBox.Icon.ERROR,
                            Title = "خطا",
                            Message = "حجم فایل انتخابی می بایست کمتر از یک مگابایت باشد."
                        });
                        DirectResult result = new DirectResult();
                        return result;
                    }
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR,
                        Title = "خطا",
                        Message = "فایل انتخاب شده غیر مجاز است."
                    });
                    DirectResult result = new DirectResult();
                    return result;
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "خطا",
                    Message = Msg
                });
                DirectResult result = new DirectResult();
                return result;
            }
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Cservic.GetFormatFileDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out CErr);
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldFormatName,
                fldPassvand = q.fldPassvand,
                fldDesc = q.fldDesc,
                fldSize = q.fldSize
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_FormatFile> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_FormatFile> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldFormatName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFormatName";
                            break;
                        case "fldPassvand":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPassvand";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = Cservic.GetFormatFileWithFilter(field, searchtext, 100, Convert.ToInt32(Session["OrganId"]), IP, out CErr).ToList();
                    else
                        data = Cservic.GetFormatFileWithFilter(field, searchtext, 100, Convert.ToInt32(Session["OrganId"]), IP, out CErr).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = Cservic.GetFormatFileWithFilter("", "", 100, Convert.ToInt32(Session["OrganId"]), IP, out CErr).ToList();
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

            List<WCF_Common.OBJ_FormatFile> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
