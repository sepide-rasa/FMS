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
using System.Text;
using System.Net;
using NewFMS.Helper;
using System.Xml;
using System.Reflection;
using System.Web.UI;
using System.Configuration;
using System.Data;
using NewFMS.Models;
using System.IO;
using NewFMS.Models;
using Aspose.Cells;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using ClosedXML.Excel;


namespace NewFMS.Areas.PayRoll.Controllers
{
    public class OnAccountController : Controller
    {
        //
        // GET: /PayRoll/OnAccount/
        WCF_PayRoll.PayRolService servic = new WCF_PayRoll.PayRolService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_PayRoll.ClsError Err = new WCF_PayRoll.ClsError();

        WCF_Common.CommonService servic_Com = new WCF_Common.CommonService();
        WCF_Common.ClsError Err_Com = new WCF_Common.ClsError();
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
            result.ViewBag.Year = NewFMS.Models.CurrentDate.Year;
            result.ViewBag.Month = NewFMS.Models.CurrentDate.Month;
            result.ViewBag.nobatePardakht = NewFMS.Models.CurrentDate.nobatPardakht;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }


        public ActionResult Upload()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                if (Session["savePathOnAcc"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathOnAcc"].ToString());
                    Session.Remove("savePathOnAcc");
                    System.IO.File.Delete(physicalPath);
                }

                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                var FileNamee = Path.GetFileNameWithoutExtension(Request.Files[0].FileName) + Session["UserId"] + extension;
                HttpPostedFileBase file = Request.Files[0];
                string savePath = Server.MapPath(@"~\Uploaded\" + FileNamee);

                var IsExcel = FileInfoExtensions.IsExcel2(file);

                if (IsExcel == true)
                {
                    //if (Request.Files[0].ContentLength <= 1048576)
                    //{
                    file.SaveAs(savePath);
                    Session["savePathOnAcc"] = savePath;
                    object r = new
                    {
                        success = true,
                        name = Request.Files[0].FileName,
                        Message = "فایل با موفقیت آپلود شد.",
                        IsValid = true
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    //}
                    //else
                    //{
                    //    object r = new
                    //    {
                    //        success = true,
                    //        name = Request.Files[0].FileName,
                    //        Message = "حجم فایل انتخابی می بایست کمتر از یک مگابایت باشد.",
                    //        IsValid = false
                    //    };
                    //    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                    //}
                }
                else
                {
                    object r = new
                    {
                        success = true,
                        name = Request.Files[0].FileName,
                        Message = "فایل انتخاب شده معتبر نمی باشد.",
                        IsValid = false
                    };
                    return Content(string.Format("<textarea>{0}</textarea>", JSON.Serialize(r)));
                }
            }
            catch (Exception x)
            {
                if (Session["savePathOnAcc"] != null)
                {
                    System.IO.File.Delete(Session["savePathOnAcc"].ToString());
                    Session.Remove("savePathOnAcc");
                }
                return null;
            }
        }

        public ActionResult ImportFile( int OrganId,bool ghatei)
        {
            DataTable dt = new DataTable();
            int UserId = Convert.ToInt32(Session["UserId"]), Er = 0;
            string MsgTitle = "";
            string Msg = "";
            try
            {

                if (Session["savePathOnAcc"] != null)
                {
                    string path = Session["savePathOnAcc"].ToString();
                     Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(path);
            var sheet = wb.Worksheets[0];
                    var q = servic.GetOnAccountWithFilter("check", sheet.Cells[1, 0].Value.ToString(), sheet.Cells[1, 3].Value.ToString(), sheet.Cells[1, 4].Value.ToString(), sheet.Cells[1, 2].Value.ToString(), OrganId, 1, IP,out Err).FirstOrDefault();
                    if(q!=null && q.fldFlag==true)
                    {
                        MsgTitle = "خطا";
                        Msg = "اطلاعات فایل بارگذاری شده تکراری است و پرداخت شده است، فایل مورد نظر مجاز نیست.";
                        Er = 1;
                    }
                    //servic.DeleteMaliyatDaraei(Year, Month, NobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                    //if (ErrPay.ErrorType == true)
                    //{
                    //    Msg = ErrPay.ErrorMsg;
                    //    MsgTitle = "خطا";
                    //    Er = 1;
                    //}
                    else
                    {
                        dt = ProcessMaliyat(path, UserId, OrganId, ghatei);
                        
                        string[] FeedBack = ProcessBulkCopyMaliyat(dt).Split(';');

                        if (FeedBack[0] == "ذخیره موفق")
                        {
                            //servic.UpdateMaliyatDaraei(Year, Month, NobatePardakht, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrPay);
                            //if (ErrPay.ErrorType == true)
                            //{
                            //    Msg = ErrPay.ErrorMsg;
                            //    MsgTitle = "خطا";
                            //    Er = 1;
                            //}
                            //else
                            {
                                Msg = FeedBack[1];
                                MsgTitle = FeedBack[0];
                            }
                        }
                        else
                        {
                            Msg = FeedBack[1];
                            MsgTitle = FeedBack[0];
                        }
                        System.IO.File.Delete(path);
                        Session["savePathOnAcc"] = null;


                    }
                }
                else
                {
                    MsgTitle = "خطا";
                    Msg = "لطفا فایل مورد نظر را جهت آپلود انتخاب کنید";
                    Er = 1;
                }

            }
            catch (Exception ex)
            {
                MsgTitle = "خطا";
                Msg = ex.Message;
                Er = 1;
            }

            dt.Dispose();

            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er,
            }, JsonRequestBehavior.AllowGet);
        }
        private String ProcessBulkCopyMaliyat(DataTable dt)
        {
            string Feedback = string.Empty;
            string connString = ConfigurationManager.ConnectionStrings["RasaFMSPayRollDBConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (var copy = new SqlBulkCopy(connString, SqlBulkCopyOptions.CheckConstraints))
                {

                    copy.DestinationTableName = "pay.tblOnAccount";
                    copy.ColumnMappings.Add("fldTitle", "fldTitle");
                    copy.ColumnMappings.Add("fldCodeMeli", "fldCodeMeli");
                    copy.ColumnMappings.Add("fldNobatePardakt", "fldNobatePardakt");
                    copy.ColumnMappings.Add("fldYear", "fldYear");
                    copy.ColumnMappings.Add("fldMonth", "fldMonth");
                    copy.ColumnMappings.Add("fldKhalesPardakhti", "fldKhalesPardakhti");
                    copy.ColumnMappings.Add("fldFlag", "fldFlag");
                    copy.ColumnMappings.Add("fldGhatei", "fldGhatei");
                    copy.ColumnMappings.Add("fldShomareHesab", "fldShomareHesab");
                    copy.ColumnMappings.Add("fldOrganId", "fldOrganId");
                    copy.ColumnMappings.Add("fldUserId", "fldUserId");
                    copy.ColumnMappings.Add("fldIP", "fldIP");
                    copy.ColumnMappings.Add("fldDate", "fldDate");

                    copy.BatchSize = dt.Rows.Count;
                    try
                    {
                        copy.WriteToServer(dt);

                        Feedback = "ذخیره موفق;ذخیره فایل با موفقیت انجام شد";
                    }
                    catch (Exception ex)
                    {
                        Feedback = "خطا" + ";" + ex.Message;
                    }
                }
            }

            return Feedback;
        }
        private DataTable ProcessMaliyat(string fileName, int UserId, int OrganId,bool ghatei)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("fldTitle");
            dt.Columns.Add("fldCodeMeli");
            dt.Columns.Add("fldNobatePardakt");
            dt.Columns.Add("fldYear");
            dt.Columns.Add("fldMonth");            
            dt.Columns.Add("fldKhalesPardakhti");
            dt.Columns.Add("fldFlag").DefaultValue =false;
            dt.Columns.Add("fldGhatei").DefaultValue = ghatei;
            dt.Columns.Add("fldShomareHesab");
            dt.Columns.Add("fldOrganId").DefaultValue = OrganId;            
            dt.Columns.Add("fldUserId").DefaultValue = UserId;
            dt.Columns.Add("fldIP").DefaultValue = IP;
            dt.Columns.Add("fldDate").DefaultValue = DateTime.Now;

            Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(fileName);
            var sheet = wb.Worksheets[0];
            for (int i = sheet.Cells.MinDataRow + 1; i < sheet.Cells.MaxDataRow + 1; i++)
            {
                dt.Rows.Add();
                dt.Rows[dt.Rows.Count - 1][0] = sheet.Cells[i, 0].Value;
                dt.Rows[dt.Rows.Count - 1][1] = sheet.Cells[i, 1].Value.ToString().PadLeft(10, '0');
                dt.Rows[dt.Rows.Count - 1][2] = sheet.Cells[i, 2].Value;
                dt.Rows[dt.Rows.Count - 1][3] = sheet.Cells[i, 3].Value;
                dt.Rows[dt.Rows.Count - 1][4] = sheet.Cells[i, 4].Value;
                dt.Rows[dt.Rows.Count - 1][5] = sheet.Cells[i, 5].Value;
                dt.Rows[dt.Rows.Count - 1][8] = sheet.Cells[i, 6].Value;
                //int count = 0;
                //for (int j = sheet.Cells.MinDataColumn; j < sheet.Cells.MinDataColumn + 1; j++)
                //{
                //    var str = sheet.Cells[i, j].StringValue;
                //    dt.Rows[dt.Rows.Count - 1][count] = str;
                //}
            }
            //List<string> ReadCSV = System.IO.File.ReadAllLines(fileName).ToList();
            //foreach (string csvRow in ReadCSV)
            //{
            //    if (!string.IsNullOrEmpty(csvRow))
            //    {
            //        dt.Rows.Add();
            //        int count = 0;
            //        foreach (string FileRec in csvRow.Split(','))
            //        {
            //            var FileRec1 = FileRec.Trim('"');
            //            dt.Rows[dt.Rows.Count - 1][count] = FileRec1;
            //            count++;
            //        }
            //    }

            //}
            return dt;
        }
        
        public ActionResult GetOrgan()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic_Com.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err_Com).ToList().Select(c => new { fldId = c.fldId, fldTitle = c.fldName });
            return this.Store(q);
        }
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetTypeOfCostCentersDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldTitle = q.fldTitle,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Read(StoreRequestParameters parameters,int OrganId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_PayRoll.OBJ_OnAccount> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_PayRoll.OBJ_OnAccount> data1 = null;
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
                        case "fldKhalesPardakhti":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldKhalesPardakhti";
                            break;
                        case "fldYear":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldYear";
                            break;
                        case "fldMonth":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldMonth";
                            break;
                        case "fldNobatePardakt":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNobatePardakt";
                            break;
                        case "fldCodeMeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodeMeli";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldGhatei":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldGhatei";
                            break;
                        case "fldShomareHesab":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldShomareHesab";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetOnAccountWithFilter(field, searchtext,"","","", OrganId, 100, IP, out Err).ToList();
                    else
                        data = servic.GetOnAccountWithFilter(field, searchtext, "", "", "", OrganId, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetOnAccountWithFilter("", "", "", "", "", OrganId, 100, IP, out Err).ToList();
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

            List<WCF_PayRoll.OBJ_OnAccount> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
