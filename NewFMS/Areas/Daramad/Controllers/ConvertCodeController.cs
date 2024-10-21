using Ext.Net;
using Ext.Net.MVC;
using FastMember;
using Microsoft.CSharp.RuntimeBinder;
using MyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace NewFMS.Areas.Daramad.Controllers
{
    public class ConvertCodeController : Controller
    {
        // GET: Daramad/ConvertCode
        WCF_Daramad.DaramadService servic = new WCF_Daramad.DaramadService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Daramad.ClsError Err = new WCF_Daramad.ClsError();
        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
        public ActionResult Index(int FiscalYearId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.FiscalYearId = FiscalYearId;

            return result;
        }
        public ActionResult Upload()
        {
            string Msg = "";
            try
            {
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    Session.Remove("FileName");
                    System.IO.File.Delete(physicalPath);
                }
                HttpPostedFileBase file = Request.Files[0];
                string e = Path.GetExtension(file.FileName);

                if (e.ToLower() == ".xls" || e.ToLower() == ".xlsx")
                {
                    string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                    file.SaveAs(savePath);
                    Session["FileName"] = file.FileName;
                    Session["savePath"] = savePath;
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
        public ActionResult ProcessXlsRecords()
        {
            try
            {

                Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(Session["savePath"].ToString());
                List<Models.OBJ_MapCodingDaramad> Detail = new List<Models.OBJ_MapCodingDaramad>();
                for (int i = 1; i < wb.Worksheets[0].Cells.MaxDataRow + 1; i++)
                {
                    Models.OBJ_MapCodingDaramad mm = new Models.OBJ_MapCodingDaramad();
                    int count = 0;
                    for (int j = wb.Worksheets[0].Cells.MinColumn; j < wb.Worksheets[0].Cells.MaxDataColumn + 1; j++)
                    {
                        var str = wb.Worksheets[0].Cells[i, j].StringValue;
                        switch (count)
                        {
                            case 0:
                                mm.oldCode = str;
                                break;
                            case 1:
                                mm.newCode = str;
                                break;
                            case 2:
                                mm.title = str;
                                break;
                            case 3:
                                mm.pcode = str;
                                break;
                        }
                        count++;

                    }

                        Detail.Add(mm);
                }

                foreach (var item in Detail)
                {
                    servic.MapCodingDaramad(item.oldCode, item.newCode, item.title, item.pcode, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new
                {
                    Msg = "عملیات با موفقیت انجام شد.",
                    MsgTitle = "موفق",
                    Er = 0
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                var Msg = "";
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;
                servic_Com.InsertError(Msg, servic_Com.GetDate(IP, out Err_Com).fldDateTime, IP, "", Session["Username"].ToString(), (Session["Password"].ToString()), out Err_Com);
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}