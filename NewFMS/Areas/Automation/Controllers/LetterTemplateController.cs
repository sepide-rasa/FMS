using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;
using NewFMS.Models;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
//using TXTextControl;
//using TXTextControl.Web;
//using TXTextControl.Web.MVC;
//using TXTextControl.DocumentServer;

namespace NewFMS.Areas.Automation.Controllers
{
    public class LetterTemplateController : Controller
    {
        //
        // GET: /Automation/LetterTemplate/

        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();

        WCF_Common.CommonService servic_C = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_C = new WCF_Common.ClsError();
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

       /* public ActionResult LoadDoc(int LetterTemplateId, string ConnectionID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var data = servic.GetLetterTemplateDetail(LetterTemplateId, Convert.ToInt32(Session["OrganId"]), IP, out Err).fldFormat;
            WebSocketHandler wsHandler = WebSocketHandler.GetInstance(ConnectionID);
            TXTextControl.ServerTextControl tx = new ServerTextControl();


            tx.Create();
            tx.Text = "این یک تست است.";

            tx.SelectAll();
            tx.Selection.FontName = "B Homa";
            //tx.Selection.FontSize = 400;
            //var bbb = tx.Font.SystemFontName;
            //tx.Font = System.Drawing.SystemFonts.GetFontByName("Arial");
            tx.FontSettings.ScalableFontsOnly = false;
            tx.FontSettings.TrueTypeFontsOnly = false;
            tx.FontSettings.EmbeddableFontsOnly = false;
            var aa = tx.Font.FontFamily.Name;
            var bb = tx.GetSupportedFonts();
            tx.FontSettings.AdaptFontEvent = true;
            //tx.ResetFontSettings();
            //tx.BackColor = System.Drawing.Color.Aqua;
            tx.ParagraphFormat.Direction = TXTextControl.Direction.RightToLeft;
            tx.ParagraphFormat.Alignment = HorizontalAlignment.Right;
            //tx.ResetParagraphFormat();
            byte[] data1;
            tx.Save(out data1, TXTextControl.BinaryStreamType.InternalUnicodeFormat);





            // the document directly server-side    
            //wsHandler.LoadText(data, TXTextControl.Web.StringStreamType.HTMLFormat);
            return Json(new
            {
                data = Convert.ToBase64String(data1)
            }, JsonRequestBehavior.AllowGet);
        }*/
        public ActionResult SaveDoc(int LetterTemplateId, string ConnectionID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            /*TXTextControl.ServerTextControl s = new ServerTextControl();
            WebSocketHandler wsHandler = WebSocketHandler.GetInstance(ConnectionID);
            TXTextControl.Selection a = new TXTextControl.Selection();*/

            string data = "";
           /* wsHandler.SaveText(out data, TXTextControl.Web.StringStreamType.HTMLFormat);*/
            servic.UpdateLetterTemplate_Format(LetterTemplateId, data, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                Msg = "الگوی نامه با موفقیت ذخیره شد"
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TextEditor(int LetterTemplateId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            var MergeFieldList = servic.GetLetterTemplateDetail(LetterTemplateId, Convert.ToInt32(Session["OrganId"]), IP, out Err)
                .fldEnNameMergeField.Split(',').Where(l => l != "").ToList();
            dynamic exo = new System.Dynamic.ExpandoObject();
            var expandoDic = (IDictionary<string, object>)exo;
            foreach (string field in MergeFieldList)
            {
                expandoDic.Add(field, field + "_data");
            }

            var myJson = JsonConvert.SerializeObject(expandoDic);
            var myXml = JsonConvert.DeserializeXNode(myJson.ToString(), "letterTemplate");
            var xmlDoc = new XmlDocument();
            using (var xmlReader = myXml.CreateReader())
            {
                xmlDoc.Load(xmlReader);
            }
            ViewBag.xmlDataSource = xmlDoc;
            ViewBag.LetterTemplateId = LetterTemplateId;
            return View();
        }

        [HttpPost]
        public FileContentResult Download(int fldFileId)
        {
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {

                var q = servic_C.GetFileWithFilter("fldId", fldFileId.ToString(), 1, IP, out Err_C).FirstOrDefault();
                if (q != null && q.fldImage != null)
                {
                    MemoryStream st = new MemoryStream(q.fldImage);
                    return File(st.ToArray(), MimeType.Get(q.fldPasvand), "DownloadFile" + q.fldPasvand);
                }
                return null;
            }
        }
        public ActionResult GetMergeField()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            if (Request.IsAjaxRequest() == false)
            {
                return null;
            }
            else
            {

                var q = servic.GetMergeFieldTypesWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldFaName });
                return this.Store(q);
            }
        }
        public ActionResult Save(WCF_Automation.OBJ_LetterTemplate LetterTemplate, bool DeleteFileHu)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; byte[] Image = null; string Pasvand = ""; byte[] LetFile = null; string LetPasvand = "";
            try
            {

                LetterTemplate.fldIdMergeField = LetterTemplate.fldIdMergeField.Remove(LetterTemplate.fldIdMergeField.Length - 1);
                if (LetterTemplate.fldId == 0)
                {
                    //ذخیره
                    if (Session["savePathBackGround"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathBackGround"].ToString()));
                        Image = stream.ToArray();
                        Pasvand = Path.GetExtension(Session["savePathBackGround"].ToString());
                    }
                    if (Session["savePathLet"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathLet"].ToString()));
                        LetFile = stream.ToArray();
                        LetPasvand = Path.GetExtension(Session["savePathLet"].ToString());
                    }

                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertLetterTemplate(LetterTemplate.fldName, LetterTemplate.fldIsBackGround, Image, Pasvand, LetterTemplate.fldIdMergeField,LetFile,LetPasvand, LetterTemplate.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Session["savePathBackGround"] != null)
                    {
                        string physicalPath = System.IO.Path.Combine(Session["savePathBackGround"].ToString());
                        Session.Remove("savePathBackGround");
                        System.IO.File.Delete(physicalPath);
                    }
                    if (Session["savePathLet"] != null)
                    {
                        string physicalPath = System.IO.Path.Combine(Session["savePathLet"].ToString());
                        Session.Remove("savePathLet");
                        System.IO.File.Delete(physicalPath);
                    }
                }
                else
                {
                    //ویرایش
                    if (Session["savePathBackGround"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathBackGround"].ToString()));
                        Pasvand = Path.GetExtension(Session["savePathBackGround"].ToString());
                        Image = stream.ToArray();
                    }
                    else
                    {
                        if (DeleteFileHu == true)
                        {
                            Image = null;
                            Pasvand = "";
                        }
                        else
                        {

                            var pic = servic_C.GetFileWithFilter("fldId", LetterTemplate.fldFileId.ToString(), 1, IP, out Err_C).FirstOrDefault();
                            if (pic != null && pic.fldImage != null)
                            {
                                Image = pic.fldImage;
                                Pasvand = pic.fldPasvand;
                            }
                        }
                    }
                    if (Session["savePathLet"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathLet"].ToString()));
                        LetFile = stream.ToArray();
                        LetPasvand = Path.GetExtension(Session["savePathLet"].ToString());
                    }
                    else
                    {

                        var pic = servic_C.GetFileWithFilter("fldId", LetterTemplate.fldLetterFileId.ToString(), 1, IP, out Err_C).FirstOrDefault();
                        if (pic != null && pic.fldImage != null)
                        {
                            LetFile = pic.fldImage;
                            LetPasvand = pic.fldPasvand;
                        }
                    }

                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateLetterTemplate(LetterTemplate.fldId, LetterTemplate.fldName, LetterTemplate.fldIsBackGround, LetterTemplate.fldFileId, Image, Pasvand, LetterTemplate.fldIdMergeField, LetFile, LetPasvand, LetterTemplate.fldLetterFileId,LetterTemplate.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Session["savePathBackGround"] != null)
                    {
                        string physicalPath = System.IO.Path.Combine(Session["savePathBackGround"].ToString());
                        Session.Remove("savePathBackGround");
                        System.IO.File.Delete(physicalPath);
                    }
                    if (Session["savePathLet"] != null)
                    {
                        string physicalPath = System.IO.Path.Combine(Session["savePathLet"].ToString());
                        Session.Remove("savePathLet");
                        System.IO.File.Delete(physicalPath);
                    }
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

                if (Session["savePathBackGround"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathBackGround"].ToString());
                    Session.Remove("savePathBackGround");
                    System.IO.File.Delete(physicalPath);
                }
                if (Session["savePathLet"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathLet"].ToString());
                    Session.Remove("savePathLet");
                    System.IO.File.Delete(physicalPath);
                }

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
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                servic.DeleteLetterTemplate(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                Msg = "حذف با موفقیت انجام شد.";

                MsgTitle = "حذف موفق";
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
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetLetterTemplateDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            string pic = ""; string FileNAme = "";
            if (q.fldFileId != null)
            {
                var file = servic_C.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1, IP, out Err_C).FirstOrDefault();
                if (file != null && file.fldImage != null)
                {
                    pic = Convert.ToBase64String(file.fldImage);
                    FileNAme = "File" + file.fldPasvand;
                }
            }
            return Json(new
            {
                fldID = q.fldId,
                fldName = q.fldName,
                fldIsBackGround = q.fldIsBackGround,
                fldDesc = q.fldDesc,
                FileName = FileNAme,
                fldFileId = q.fldFileId,
                fldIdMergeField = q.fldIdMergeField,
                fldLetterFileId = q.fldLetterFileId
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Automation.OBJ_LetterTemplate> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Automation.OBJ_LetterTemplate> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldID":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldID";
                            break;
                        case "fldType":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldType";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetLetterTemplateWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetLetterTemplateWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetLetterTemplateWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Automation.OBJ_LetterTemplate> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult DeleteSessionFile()
        {
            string Msg = "";

            if (Session["savePathBackGround"] != null)
            {
                string physicalPath = System.IO.Path.Combine(Session["savePathBackGround"].ToString());
                Session.Remove("savePathBackGround");
                Session.Remove("FileName");
                System.IO.File.Delete(physicalPath);
                Msg = "حذف فایل با موفقیت انجام شد";
            }
            return Json(new
            {
                Msg = Msg
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ShowPic(string dc)
        {//برگرداندن عکس 
            byte[] file = null;
            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathBackGround"].ToString()));
            file = stream.ToArray();
            var image = Convert.ToBase64String(file);
            return Json(new { image = image });
        }

        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; int Er = 0;
            try
            {
                if (Session["savePathBackGround"] != null)
                {
                    System.IO.File.Delete(Session["savePathBackGround"].ToString());
                    Session.Remove("FileName");
                    Session.Remove("savePathBackGround");
                }

                var IsImage = FileInfoExtensions.IsImage(Request.Files[1]);
                var extension = Path.GetExtension(Request.Files[1].FileName).ToLower();
                if (IsImage == true)
                {
                    if (Request.Files[1].ContentLength <= 10485760)
                    {
                        HttpPostedFileBase file = Request.Files[1];
                        var Name = Guid.NewGuid();
                        string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                        file.SaveAs(savePath);
                        Session["savePathBackGround"] = savePath;
                        Session["FileName"] = Path.GetFileNameWithoutExtension(Request.Files[1].FileName);
                        object r = new
                        {
                            success = true,
                            name = Request.Files[1].FileName,
                            Message = "فایل با موفقیت آپلود شد.",
                            IsValid = true
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        object r = new
                        {
                            success = true,
                            name = Request.Files[1].FileName,
                            Message = "حجم فایل انتخابی می بایست کمتر از 10 مگابایت باشد.",
                            IsValid = false
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        //X.Msg.Show(new MessageBoxConfig
                        //{
                        //    Buttons = MessageBox.Button.OK,
                        //    Icon = MessageBox.Icon.ERROR,
                        //    Title = "خطا",
                        //    Message = "حجم فایل انتخابی می بایست کمتر از یک مگابایت باشد."
                        //});
                        //DirectResult result = new DirectResult();
                        //return result;
                    }
                }
                else
                {
                    object r = new
                    {
                        success = true,
                        name = Request.Files[1].FileName,
                        Message = "فایل انتخاب شده غیر مجاز است.",
                        IsValid = false
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    //X.Msg.Show(new MessageBoxConfig
                    //{
                    //    Buttons = MessageBox.Button.OK,
                    //    Icon = MessageBox.Icon.ERROR,
                    //    Title = "خطا",
                    //    Message = "فایل انتخاب شده غیر مجاز است."
                    //});
                    //DirectResult result = new DirectResult();
                    //return result;
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
                return Json(new
                {
                    MsgTitle = MsgTitle,
                    Msg = Msg,
                    Er = 1
                });
            }
        }
        public ActionResult UploadLet()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; int Er = 0;
            try
            {
                if (Session["savePathLet"] != null)
                {
                    System.IO.File.Delete(Session["savePathLet"].ToString());
                    Session.Remove("FileName");
                    Session.Remove("savePathLet");
                }

                var IsWord = FileInfoExtensions.IsWord(Request.Files[0]);
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (IsWord == true)
                {
                    if (Request.Files[0].ContentLength <= 10485760)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        var Name = Guid.NewGuid();
                        string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                        file.SaveAs(savePath);
                        Session["savePathLet"] = savePath;
                        Session["FileName"] = Path.GetFileNameWithoutExtension(Request.Files[0].FileName);
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName,
                            Message = "فایل با موفقیت آپلود شد.",
                            IsValid = true
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                    else
                    {
                        object r = new
                        {
                            success = true,
                            name = Request.Files[0].FileName,
                            Message = "حجم فایل انتخابی می بایست کمتر از 10 مگابایت باشد.",
                            IsValid = false
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                        //X.Msg.Show(new MessageBoxConfig
                        //{
                        //    Buttons = MessageBox.Button.OK,
                        //    Icon = MessageBox.Icon.ERROR,
                        //    Title = "خطا",
                        //    Message = "حجم فایل انتخابی می بایست کمتر از یک مگابایت باشد."
                        //});
                        //DirectResult result = new DirectResult();
                        //return result;
                    }
                }
                else
                {
                    object r = new
                    {
                        success = true,
                        name = Request.Files[0].FileName,
                        Message = "فایل انتخاب شده غیر مجاز است.",
                        IsValid = false
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    //X.Msg.Show(new MessageBoxConfig
                    //{
                    //    Buttons = MessageBox.Button.OK,
                    //    Icon = MessageBox.Icon.ERROR,
                    //    Title = "خطا",
                    //    Message = "فایل انتخاب شده غیر مجاز است."
                    //});
                    //DirectResult result = new DirectResult();
                    //return result;
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
                return Json(new
                {
                    MsgTitle = MsgTitle,
                    Msg = Msg,
                    Er = 1
                });
            }
        }
    }
}
