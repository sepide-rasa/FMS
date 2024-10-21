using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using FastMember;
using System.IO;
using NewFMS.Models;


namespace NewFMS.Areas.Weigh.Controllers
{
    public class RemittanceController : Controller
    {
        //
        // GET: /Weigh/Remittance/

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Weigh.WeighService servic = new WCF_Weigh.WeighService();
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService();
        WCF_Weigh.ClsError Err = new WCF_Weigh.ClsError();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
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
        public ActionResult GetChart()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q =servic_com.GetChartEjraee_LastNode(Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err_com).OrderBy(l => l.fldTitle).ToList().Select(n => new { fldId = n.fldId, fldName = n.fldTitle });
            return this.Store(q);
        }
        public ActionResult New(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            var CurrentDate = servic_com.GetDate(IP, out Err_com).fldTarikh;
            result.ViewBag.HeaderId = HeaderId.ToString();
            result.ViewBag.CurrentYear = CurrentDate.Substring(0, 4);
            result.ViewBag.CurrentDate = CurrentDate;
            return result;
        }

        public ActionResult InfoRemittance(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.HeaderId = HeaderId.ToString();
            return result;
        }

        public ActionResult ReadInfoRemittance(StoreRequestParameters parameters, int HeaderId)
        {
           
            
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Weigh.OBJ_KalaHavale> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Weigh.OBJ_KalaHavale> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldNameHeader":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameHeader";
                            break;
                        case "fldNameKala":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameKala";
                            break;
                        case "fldMaxTon":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMaxTon";
                            break;
                        case "fldSumKala":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSumKala";
                            break;
                        case "fldBaghimande":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldBaghimande";
                            break;
                       
                    }
                    if (data != null)
                        data1 = servic.SelectSumKalaHavale(field, searchtext, HeaderId, IP, out Err).ToList();
                    else
                        data = servic.SelectSumKalaHavale(field, searchtext, HeaderId, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.SelectSumKalaHavale("","",HeaderId, IP, out Err).ToList();
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

            List<WCF_Weigh.OBJ_KalaHavale> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult InfoKala(int HeaderId, int KalaId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.HeaderId = HeaderId.ToString();
            result.ViewBag.KalaId = KalaId.ToString();
            return result;
        }
        public ActionResult ReadInfoKala(StoreRequestParameters parameters, int HeaderId, int KalaId)
        {
            
           
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Weigh.OBJ_SumKalaHavale_Detail> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Weigh.OBJ_SumKalaHavale_Detail> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldNameHavale":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameHavale";
                            break;
                        case "fldNameKala":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameKala";
                            break;
                        case "fldNameBaskool":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameBaskool";
                            break;
                        case "fldNameKhodro":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameKhodro";
                            break;
                        case "fldPlaque":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldPlaque";
                            break;
                        case "fldNameRanande":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameRanande";
                            break;
                        case "fldNamePlace":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNamePlace";
                            break;
                        case "fldVaznKhali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldVaznKhali";
                            break;
                        case "fldTarikhVaznKhali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhVaznKhali";
                            break;
                        case "fldVaznKol":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldVaznKol";
                            break;
                        case "fldTarikh_TimeTozin":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikh_TimeTozin";
                            break;
                        case "fldVaznKhals":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldVaznKhals";
                            break;
                    }
                    if (data != null)
                        data1 = servic.SelectSumKalaHavale_Detail(field, searchtext, HeaderId, KalaId, IP, out Err).ToList();
                    else
                        data = servic.SelectSumKalaHavale_Detail(field, searchtext, HeaderId, KalaId, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.SelectSumKalaHavale_Detail("","",HeaderId, KalaId, IP, out Err).ToList();
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

            List<WCF_Weigh.OBJ_SumKalaHavale_Detail> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadRemittance_Details(StoreRequestParameters parameters, int HeaderId)
        {
            List<WCF_Weigh.OBJ_Remittance_Details> data = null;
            data = servic.GetRemittance_DetailsWithFilter("fldRemittanceId", HeaderId.ToString(), 0,0, IP, out Err).ToList();
            return this.Store(data);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Weigh.OBJ_Remittance_Header> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Weigh.OBJ_Remittance_Header> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldStatusName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStatusName";
                            break;
                        case "fldKalaName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldKalaName";
                            break;
                        case "fldFamilyName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamilyName";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;
                        case "fldStartDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStartDate";
                            break;
                        case "fldEndDate":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldEndDate";
                            break;
                        case "fldTypeShakhs":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeShakhs";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetRemittance_HeaderWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetRemittance_HeaderWithFilter(field, searchtext, Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetRemittance_HeaderWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Weigh.OBJ_Remittance_Header> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                if (Session["RemittanceDoc"] != null)
                {
                    System.IO.File.Delete(Session["RemittanceDoc"].ToString());
                    Session.Remove("RemittanceDoc");
                }
                var IsImage = FileInfoExtensions.IsImage(Request.Files[0]);
                var IsWord = FileInfoExtensions.IsWord(Request.Files[0]);
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (IsImage == true || IsWord==true)
                {
                    if (Request.Files[0].ContentLength <= 5242880)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        var Name = Guid.NewGuid();
                        string path = Server.MapPath(@"~\Uploaded\" + Name + extension);
                        file.SaveAs(path);
                        Session["RemittanceDoc"] = path;
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
                            Message = "حجم فایل انتخابی می بایست کمتر از 5 مگابایت باشد.",
                            IsValid = false
                        };
                        return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    }
                }
                else
                {
                    object r = new
                    {
                        success = true,
                        FileName = Request.Files[0].FileName,
                        Message = "فایل انتخاب شده غیر مجاز است.",
                        IsValid = false
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                }
            }
            catch (Exception x)
            {
                var Msg = x.Message;
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;

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
        public ActionResult Save(WCF_Weigh.OBJ_Remittance_Header Header, List<NewFMS.Models.RemittanceDetails> Remittance_DetailsArray, bool IsDeleted)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;byte[] Filee=null;string Passvand="";int? FileId=null;
            try
            {
                if (Remittance_DetailsArray.Where(l => l.fldDesc == null).Count() > 0)
                {
                    var a = Remittance_DetailsArray.Where(l => l.fldDesc == null).ToList();
                    foreach (var item in a)
                    {
                        item.fldDesc = "";
                    }
                }
                System.Data.DataTable dt = new System.Data.DataTable { TableName = "Weigh.tblRemittance_Details" };
                using (var reader = FastMember.ObjectReader.Create(Remittance_DetailsArray))
                {
                    dt.Load(reader);
                }
                
                if (Header.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    if (IsDeleted == false && Session["RemittanceDoc"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["RemittanceDoc"].ToString()));
                        Filee = stream.ToArray();
                        Passvand = Path.GetExtension(Session["RemittanceDoc"].ToString());
                    }
                    Msg = servic.InsertRemittance_Header(Header.fldTitle, Header.fldAshkhasiId, Header.fldStatus,Filee,Passvand, Header.fldStartDate, Header.fldEndDate, dt, Header.fldDesc,Header.fldChartOrganEjraeeId,Convert.ToInt32(Header.fldEmployId), Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Session["RemittanceDoc"] != null)
                    {
                        System.IO.File.Delete(Session["RemittanceDoc"].ToString());
                        Session.Remove("RemittanceDoc");
                    }
                    //int HeaderId = servic.InsertRemittance_Header(Header.fldDocumentNum,Header.fldAtfNum,Header.fldArchiveNum,Header.fldDescriptionDocu,
                    //    Header.fldTarikhDocument,0,Header.fldType ,"",Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    //foreach (var item in Remittance_DetailsArray)
                    //{
                    //    Msg=servic.InsertRemittance_Details(HeaderId, item.fldCodingId, item.fldDescription, item.fldBedehkar, item.fldBestankar, item.fldCenterCoId,
                    //        item.fldCaseTypeId, item.fldSourceId, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    //    if (Err.ErrorType)
                    //    {
                    //        var MsgError = Err.ErrorMsg;
                    //        servic.DeleteRemittance_Header(HeaderId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    //        return Json(new
                    //        {
                    //            Msg = MsgError,
                    //            MsgTitle = "خطا",
                    //            Er = 1
                    //        }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = "ویرایش با موفقیت انجام شد.";
                    if (IsDeleted == false && Session["RemittanceDoc"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["RemittanceDoc"].ToString()));
                        Filee = stream.ToArray();
                        Passvand = Path.GetExtension(Session["RemittanceDoc"].ToString());                        
                    }
                    FileId = Header.fldFileId;
                    servic.UpdateRemittance_Header(Header.fldId,Header.fldTitle, Header.fldAshkhasiId,Header.fldStatus, Header.fldStartDate, Header.fldEndDate,Filee,Passvand,FileId, dt, Header.fldDesc
                        ,Header.fldChartOrganEjraeeId,Convert.ToInt32(Header.fldEmployId), Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Session["RemittanceDoc"] != null)
                    {
                        System.IO.File.Delete(Session["RemittanceDoc"].ToString());
                        Session.Remove("RemittanceDoc");
                    }
                    //servic.UpdateRemittance_Header(Header.fldId, Header.fldArchiveNum, Header.fldDescriptionDocu,
                    //    Header.fldTarikhDocument, 0,Header.fldType, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    ////delete
                    //servic.DeleteRemittance_Details("fldDocument_HedearId", Header.fldId, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    //if (Err.ErrorType)
                    //{
                    //    return Json(new
                    //    {
                    //        Msg = Err.ErrorMsg,
                    //        MsgTitle = "خطا",
                    //        Er = 1
                    //    }, JsonRequestBehavior.AllowGet);
                    //}
                    //foreach (var item in Remittance_DetailsArray)
                    //{
                    //    servic.InsertRemittance_Details(Header.fldId, item.fldCodingId, item.fldDescription, item.fldBedehkar, item.fldBestankar, item.fldCenterCoId,
                    //        item.fldCaseTypeId, item.fldSourceId, "", Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    //    if (Err.ErrorType)
                    //    {
                    //        return Json(new
                    //        {
                    //            Msg = Err.ErrorMsg,
                    //            MsgTitle = "خطا",
                    //            Er = 1
                    //        }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                }
            }
            catch (Exception x)
            {
                if (Session["RemittanceDoc"] != null)
                {
                    System.IO.File.Delete(Session["RemittanceDoc"].ToString());
                    Session.Remove("RemittanceDoc");
                }
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

        public ActionResult Details(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetRemittance_HeaderDetail(HeaderId, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            List<int> CheckID = new List<int>();
            var Checked = servic.GetRemittance_DetailsWithFilter("HeaderId", HeaderId.ToString(), 0, 0, IP, out Err).ToList();
            foreach (var item in Checked)
            {
                CheckID.Add(item.fldId);
            }
            string status = "0";
            if (q.fldStatus == true)
                status = "1";
            return Json(new
            {
                fldId = q.fldId,
                fldAshkhasiId = q.fldAshkhasiId,
                fldCodemeli = q.fldCodemeli,
                fldDesc = q.fldDesc,
                fldStartDate = q.fldStartDate,
                fldEndDate = q.fldEndDate,
                fldFamilyName = q.fldFamilyName,
                fldTitle = q.fldTitle,
                fldStatus=status,
                fldEmployId=q.fldEmployId,
                fldChartOrganEjraeeId=q.fldChartOrganEjraeeId.ToString(),
                fldKalaName=q.fldKalaName,
                fldNameChart=q.fldNameChart,
                fldNameTahvilGirande=q.fldNameTahvilGirande,
                CheckID = CheckID,
                FileId=q.fldFileId
            }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadFileHavale(int fldFileId)
        {       
            try
            {
                var q = servic_com.GetFileWithFilter("fldId", fldFileId.ToString(), 1, IP, out Err_com).FirstOrDefault();
                if (q != null && q.fldImage != null)
                {
                    MemoryStream st = new MemoryStream(q.fldImage);
                    return File(st.ToArray(), MimeType.Get(q.fldPasvand), "File" + q.fldPasvand);
                }
                return null;
            }
            catch (Exception x)
            {
                return null;
            }
        }
        public ActionResult DeleteFileHavale(int fldId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                NewFMS.Models.WeightEntities m = new Models.WeightEntities();
                MsgTitle = "حذف موفق";
                Msg = "حذف فایل با موفقیت انجام شد.";
                m.spr_DeleteFileHavale(fldId, Convert.ToInt32(Session["UserId"]));
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
        
        public ActionResult DeleteHeader(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق";
               
                Msg = servic.DeleteRemittance_Header(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult PrintHavale(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.HeaderId = HeaderId;
            return result;
        }

        public ActionResult GeneratePdfRptHavale(int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptHavaleTableAdapter Havale = new DataSet.DataSet1TableAdapters.spr_RptHavaleTableAdapter();
                dt.EnforceConstraints = false;

               
                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                Havale.Fill(dt.spr_RptHavale, HeaderId);
                GetDate.Fill(dt_Com.spr_GetDate);
                var User = servic_com.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err_com).FirstOrDefault();
                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "rasaFMSDBDataSet");
                Report.RegisterData(dt, "rasaFMSDBDataSet");

                Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Baskool\RptHavale.frx");
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
                Report.SetParameterValue("UserName", User.fldNameEmployee);
                pdf.EmbeddingFonts = true;
                MemoryStream stream = new MemoryStream();
                Report.Prepare();

                Report.Export(pdf, stream);
                return File(stream.ToArray(), "application/pdf");
            }
            catch (Exception x)
            {
                return Json(x.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}

