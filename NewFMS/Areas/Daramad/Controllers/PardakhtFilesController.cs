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

namespace NewFMS.Areas.Daramad.Controllers
{
    public class PardakhtFilesController : Controller
    {
        //
        // GET: /Daramad/PardakhtFiles/
        // GET: /Daramad/instalments/
        WCF_Daramad.DaramadService servicD = new WCF_Daramad.DaramadService();
        WCF_Common.CommonService servic = new WCF_Common.CommonService();

        WCF_Daramad.ClsError ErrD = new WCF_Daramad.ClsError();
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        public ActionResult Index(string containerId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var currentDate = servic.GetDate( IP, out Err).fldDateTime.ToString("yyyy-MM-dd");

            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            result.ViewBag.currentDate = currentDate;
            result.ViewBag.dateShamsi = servic.GetDate( IP, out Err).fldTarikh;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }

        public ActionResult Read(StoreRequestParameters parameters, string AzTarikh, string TaTarikh)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);
            List<WCF_Daramad.OBJ_PardakhtFiles_Detail> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Daramad.OBJ_PardakhtFiles_Detail> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldFishId":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFishId";
                            break;
                        case "fldShenaseGhabz":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenaseGhabz";
                            break;
                        case "fldShenasePardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShenasePardakht";
                            break;
                        case "fldTitleNahvePardakht":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitleNahvePardakht";
                            break;
                        case "fldCodeRahgiry":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodeRahgiry";
                            break;
                        case "fldBankName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBankName";
                            break;
                        case "fldNahvePardakhtName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNahvePardakhtName";
                            break;
                    }
                    if (data != null)

                        data1 = servicD.GetPardakhtFiles_DetailWithFilter(field, searchtext,Session["OrganId"].ToString(), AzTarikh, TaTarikh, 0,IP,out ErrD).ToList();
                    else
                        data = servicD.GetPardakhtFiles_DetailWithFilter(field, searchtext, Session["OrganId"].ToString(), AzTarikh, TaTarikh, 0, IP, out ErrD).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                if (AzTarikh == "" || TaTarikh == "")
                {
                    data = servicD.GetPardakhtFiles_DetailWithFilter("", "", Session["OrganId"].ToString(), AzTarikh, TaTarikh, 100,IP,out ErrD).ToList();
                }
                else
                {
                    data = servicD.GetPardakhtFiles_DetailWithFilter("", "", Session["OrganId"].ToString(), AzTarikh, TaTarikh, 0, IP, out ErrD).ToList();
                }
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

            List<WCF_Daramad.OBJ_PardakhtFiles_Detail> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
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
        public ActionResult New()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var fldCodAnformatic = servic.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).fldCodAnformatic;
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.fldCodAnformatic = fldCodAnformatic;
            return PartialView;
        }

        public ActionResult UploadFile()
        {
            string Msg = "";
            try
            {
                if (Session["Username"] == null)
                    return RedirectToAction("logon", "Account", new { area = "" });

                if (Session["savePathPardakhtFile"] != null)
                {
                    System.IO.File.Delete(Session["savePathPardakhtFile"].ToString());
                    Session.Remove("savePathPardakhtFile");
                }
                var fldCodAnformatic = servic.GetOrganizationDetail(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err).fldCodAnformatic;
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == "."+fldCodAnformatic)
                {
                    if (Request.Files[0].ContentLength <= 1048576)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                        file.SaveAs(savePath);
                        Session["savePathPardakhtFile"] = savePath;
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
                        Message = "فایل انتخاب شده غیرمعتبر است."
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

        public JsonResult GetBankName()
        {
            string BankName = "";
            if (Session["savePathPardakhtFile"] != null)
            {
                string type = System.IO.Path.GetFileName(Session["savePathPardakhtFile"].ToString()).Substring(0, 3);
                var bank = servic.GetBankWithFilter("fldInfinitiveBank", type, 0,IP, out Err).FirstOrDefault();
                if (bank != null)
                    BankName = bank.fldBankName;
            }
            return Json(BankName, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                if (Session["savePathPardakhtFile"] == null)
                {
                    return Json(new
                    {
                        Msg = "لطفا فایل را انتخاب نمایید.",
                        MsgTitle = "خطا",
                        Er = 1,
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int y = 0;
                    int PardakhtFileId = 0;
                    var k = System.IO.File.ReadAllLines(Session["savePathPardakhtFile"].ToString());
                    string FileName = System.IO.Path.GetFileName(Session["savePathPardakhtFile"].ToString());
                    var Organ = servic.GetOrganizationWithFilter("fldCodAnformatic", FileName.Split('.')[1], 1, Session["Username"].ToString(), Session["Password"].ToString(),IP, out Err).FirstOrDefault();
                    string type = System.IO.Path.GetFileName(Session["savePathPardakhtFile"].ToString()).Substring(0, 3);
                    var bank = servic.GetBankWithFilter("fldInfinitiveBank", type, 1, IP, out Err).FirstOrDefault();

                    if (bank != null)
                    {
                        string date = FileName.Substring(3, 8);
                        var q=servicD.GetPardakhtFileWithFilter("fldFileName", FileName, 1, IP, out ErrD).FirstOrDefault();

                        if (q != null)
                        {
                            PardakhtFileId=q.fldId;
                        }
                        else
                        {
                            PardakhtFileId = servicD.InsertPardakhtFile(bank.fldId, FileName,
                                date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2),
                               "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                            if (ErrD.ErrorType)
                            {
                                if (System.IO.File.Exists(Session["savePathPardakhtFile"].ToString()))
                                    System.IO.File.Delete(Session["savePathPardakhtFile"].ToString());
                                if (Session["savePathPardakhtFile"] != null)
                                {
                                    Session.Remove("savePathPardakhtFile");
                                }
                                return Json(new
                                {
                                    Msg = ErrD.ErrorMsg,
                                    MsgTitle = "خطا",
                                    Er = 1,
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        for (int i = 1; i < k.Count(); i++)
                        {
                            string shGhabz = k[i].Substring(16, 13).TrimStart('0');
                            string shPardakht = k[i].Substring(29, 13).TrimStart('0');
                            var q1 = servicD.GetInfoPardakhtFTP(Organ.fldId, shGhabz, shPardakht,IP,out ErrD).ToList();
                            var isValid = q1.Where(kk => kk.fldShorooShenaseGhabz == Convert.ToInt16(shGhabz.Substring(0, 2))).FirstOrDefault();
                            if (isValid == null)
                                continue;

                            if (shGhabz.Substring(shGhabz.Length - 2, 1) == isValid.fldCodKhedmat.ToString() &&
                                shGhabz.Substring(shGhabz.Length - 5, 3) == isValid.fldCodAnformatic)
                            {
                                string date1 = k[i].Substring(8, 8);
                                int type1 = Convert.ToInt32(k[i].Substring(6, 2));/*نحوه پرداخت*/
                                servicD.InsertPardakhtFiles_Detail(shGhabz, shPardakht,
                                     date1.Substring(0,4) + "/" + date1.Substring(4, 2) + "/" + date1.Substring(6, 2),
                                    k[i].Substring(k[i].Length - 6), PardakhtFileId, Organ.fldId, type1.ToString(), "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out ErrD);
                                if (ErrD.ErrorType)
                                {
                                    if (System.IO.File.Exists(Session["savePathPardakhtFile"].ToString()))
                                        System.IO.File.Delete(Session["savePathPardakhtFile"].ToString());
                                    if (Session["savePathPardakhtFile"] != null)
                                    {
                                        Session.Remove("savePathPardakhtFile");
                                    }
                                    return Json(new
                                    {
                                        Msg = ErrD.ErrorMsg,
                                        MsgTitle = "خطا",
                                        Er = 1,
                                    }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    y++;
                                }
                            }
                        }
                        if (System.IO.File.Exists(Session["savePathPardakhtFile"].ToString()))
                            System.IO.File.Delete(Session["savePathPardakhtFile"].ToString());
                        if (Session["savePathPardakhtFile"] != null)
                        {
                            Session.Remove("savePathPardakhtFile");
                        }
                    }
                    
                    return Json(new
                    {
                        Msg = "ذخیره با موفقیت انجام شد. تعداد رکورد قابل قبول: " + y,
                        MsgTitle = "ذخیره موفق",
                        Er = 0,
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                string Msg = "";
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                return Json(new
                {
                    Msg = Msg,
                    MsgTitle = "خطا",
                    Er = 1,
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
