using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.Comon
{
    public class EmployeeController : Controller
    {
        //
        // GET: /Comon/Employee/
        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();
        /// <summary>
        /// فرم اصلی مربوط به کارمندان
        /// </summary>
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
            int Permission_TypeSh = 0;
            if (servic.Permission(11, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err) &&
            servic.Permission(413, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_TypeSh = 1;
            else if (servic.Permission(11, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_TypeSh = 2;
            else if (servic.Permission(413, Convert.ToInt32(Session["OrganId"]), Session["Username"].ToString(), Session["Password"].ToString(), out Err))
                Permission_TypeSh = 3;
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.Permission_TypeSh = Permission_TypeSh;
            return result;
        }
        public ActionResult EmployeeDetails(int id, byte flag, int FishId,int ElamAvarezId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.Id = id;
                var Date = servic.GetDate( IP.ToString(), out Err);
                PartialView.ViewBag.Date = Date.fldDateTime;
                PartialView.ViewBag.flag = flag;
                PartialView.ViewBag.FishId = FishId;
                PartialView.ViewBag.ElamAvarezId = ElamAvarezId;
                return PartialView;
            }
        }
        /// <summary>
        /// باز شدن فرم جهت ذخیره اطلاعات جدید و ویرایش اطلاعات موجود
        /// </summary>
        /// <param name="id">کد جدول کارمندان</param>
        
        public ActionResult New(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            else
            {
                Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
                PartialView.ViewBag.Id = id;
                return PartialView;
            }
        }
        /// <summary>
        /// این متد برای آپلود عکس کارمندان در فرم استفاده میشود
        /// </summary>
        public ActionResult GetStatusTaahol()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetStatusTaaholWithFilter("", "", 0,IP, out Err).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetMahlSodoor()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCityWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);

        }
        public ActionResult GetNezamVazife()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetNezamVazifeWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);

        }
        public ActionResult GetReshteh()
        {
            var q = servic.GetReshteTahsiliWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);

        }
        public ActionResult GetMadrakTahsili()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetMadrakTahsiliWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);

        }
        public ActionResult GetMahalTavalod()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCityWithFilter("", "", 0,IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);

        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult Upload()
        {
            if (Request.ContentLength <= 102400)
            {
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
                    Session.Remove("FileName");
                    System.IO.File.Delete(physicalPath);
                }
                HttpPostedFileBase file = Request.Files[0];
                if ((file.ContentType).Substring(0, 5) == "image")
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
                        Message = "فایل غیرمجاز"
                    });
                    DirectResult result = new DirectResult();
                    result.IsUpload = true;
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
                    Message = "حجم فایل بیشتر از حد مجاز است."
                });
                DirectResult result = new DirectResult();
                result.IsUpload = true;
                return result;
                //return null;
            }
        }
        /// <summary>
        /// .این متد برای نمایش عکس استفاده میشود
        /// </summary>
        /// <param name="FileId">کد جدول فایل</param>
        /// <returns>.فیلد عکس ذخیره شده در جدول فایل را برمیگرداند</returns>
        public FileContentResult ShowPic(int FileId, string dc)
        {//برگرداندن عکس 
            var q = servic.GetFileWithFilter("fldId", FileId.ToString(), 1,IP, out Err).FirstOrDefault();
            if (q != null)
            {
                if (q.fldImage != null)
                {
                    //return File(PDF(q.fldPic),".pdf");
                    return File((q.fldImage), ".jpg");
                }

            }
            return null;

        }
        public ActionResult ShowPicUpload(string dc)
        {//برگرداندن عکس 
            byte[] file = null;
            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
            file = stream.ToArray();
            var image = Convert.ToBase64String(file);
            return Json(new { image = image });
        }
        //public ActionResult HaveFile(int? UsersId)
        //{
        //    bool HaveFile = false;
        //    if ((UsersId != null && UsersId != 0) || Session["savePath"] != null)
        //        HaveFile = true;
        //    return Json(new
        //    {
        //        HaveFile = HaveFile
        //    }, JsonRequestBehavior.AllowGet);
        //}
        
        //public FileContentResult Download(int UserId)
        //{
        //    var q =servic.GetEmployeeWithFilter("fldId", UserId.ToString(), 1, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).FirstOrDefault();
        //    if (Session["savePath"] != null)
        //    {
        //        string pdfPath = Session["savePath"].ToString();
        //        string FileName = Session["FileName"].ToString();
        //        string e = FileName.Substring(FileName.IndexOf('.') + 1);
        //        MemoryStream st = new MemoryStream(System.IO.File.ReadAllBytes(pdfPath));
        //        return File(st.ToArray(), MimeType.Get(e), "fileUpload." + e);
        //    }
        //    else if (q != null)
        //    {
        //        var qq = servic.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).FirstOrDefault();
        //        if (qq.fldImage != null)
        //        {
        //            string e1 ="jpg";
        //                // qq.fldName.Substring(qq.fldName.IndexOf('.') + 1);
                    
                        
        //            MemoryStream st = new MemoryStream(qq.fldImage);
        //            return File(st.ToArray(), MimeType.Get(e1), "fileUpload." + e1);
        //        }

        //    }
        //    return null;
        //}


        /// <summary>
        ///. از این متد برای  ذخیره اطلاعات جدید یا اطلاعات ویرایش شده استفاده می شود 
        /// </summary>
        ///<remarks><para> این قسمت به دو قسمت تقسیم میشودUpdate کد</para><para> .میکند Update اگر عکس کارمندان نیاز به ویرایش داشته باشد آنرا هم</para>
        ///              <para>پر میشود NULL در غیراینصورت در فیلد مربوط به عکس با مقدار</para> </remarks>
        ///     <example>     
        ///     
        ///       <code>
        ///       if(Employee.Id==0)
        ///          {
        ///              //Insert
        ///          }
        ///       else
        ///          {
        ///              //Update
        ///              
        ///          }
        ///      </code>
        ///     </example>
        /// <param name="Employee">. که شامل تمامی فیلد های جدول کارمندان به همراه مقادیر آنها میشود object</param>
        ///<returns> . موفقیت آمیز باشد پیغام ذخیره موفق یا ویرایش موفق را نمایش میدهد Update و Insert در صورتی که</returns>

        public ActionResult Save(WCF_Common.OBJ_Employee Employee)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "ذخیره با موفقیت انجام شد", MsgTitle = ""; var Er = 0; var AshkhasId = 0;
            try
            {
               /* byte[] report_file = null; string FileName = "",e="";
                if (Session["savePath"] != null)
                {
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                    report_file = stream.ToArray();
                    FileName = Session["FileName"].ToString();
                    e = Path.GetExtension(Session["savePath"].ToString());
                    var lenght = stream.Length;
                    if (lenght > 102400)
                    {
                        Msg = "حجم عکس انتخاب شده نباید بیشتر از 100 کیلو بایت  باشد.";
                        MsgTitle = "اخطار";

                        return Json(new
                        {
                            Msg = Msg,
                            MsgTitle = MsgTitle,
                            Er=1
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var Image = Server.MapPath("~/Content/Blank.jpg");
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
                    report_file = stream.ToArray();
                    FileName = "Blank.jpg";
                }*/
                if (Employee.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    AshkhasId = servic.InsertEmployee(Employee.fldName, Employee.fldFamily, Employee.fldCodemeli, Employee.fldStatus, Employee.fldTypeShakhs
                        , Employee.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    //var m = servic.GetEmployeeWithFilter("fldId", Employee.fldId.ToString(), 1, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).FirstOrDefault();
                    //if (Session["savePath"] == null)
                    //{
                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdateEmployee(Employee.fldId, Employee.fldName, Employee.fldFamily, Employee.fldCodemeli, Employee.fldStatus, Employee.fldTypeShakhs
                        , Employee.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    //}
                    //else
                    //{
                    //    MsgTitle = "ویرایش موفق";
                    //    Msg = servic.UpdateEmployee(Employee.fldId, Employee.fldName, Employee.fldFamily, Employee.fldCodemeli, Employee.fldStatus
                    //        , Employee.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                    //}
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
                Er = Er,
                AshkhasId = AshkhasId
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// .اطلاعات مربوط به یک سطر از جدول کارمندان را براساس کد داده شده پاک میکند
        /// </summary>
        /// <param name="id">کد جدول کارمندان</param>
        /// <returns>. درصورتیکه حذف با موفقیت انجام شود پیغام حذف موفق نمایش داده میشود</returns>
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteEmployee(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
                Er=Er
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///. از این متد برای قسمت ویرایش فرم استفاده میشود.زمانی که فرم در حالت ویرایش باشد اطلاعات مربوط به آن قسمت را نشان میدهد
        /// </summary>
        /// <param name="Id">کد مربوط به سطر در حال ویرایش</param>
        /// <returns>اطلاعات مربوط به یک سطر از جدول کارمندان را بر میگرداند</returns>
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetEmployeeDetail(Id,  IP, out Err);
            string Status = "0";
            if (q.fldStatus)
                Status = "1";
            //string Jensiat = "0";
            //if (q.fldJensiyat)
            //    Jensiat = "1";
            //var pic = servic.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1, Session["Username"].ToString(), Session["Password"].ToString(), out Err).FirstOrDefault();
            //var Img = Convert.ToBase64String(pic.fldImage);
            return Json(new
            {
                fldId = q.fldId,
                fldCodemeli = q.fldCodemeli,
                fldFamily = q.fldFamily,
                fldStatus=Status,
                fldName = q.fldName,
                fldDesc = q.fldDesc,
                fldTypeShakhs=q.fldTypeShakhs.ToString()
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///. فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول کارمندان و امکان سرچ کردن بر اساس   
        /// </summary>
  
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_Employee> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_Employee> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamily";
                            break;
                        case "fldStatusName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldStatusName";
                            break;
                        case "fldTypeShakhsName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTypeShakhsName";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetEmployeeWithFilter(field, searchtext, 100, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), out Err).ToList();
                    else
                        data = servic.GetEmployeeWithFilter(field, searchtext, 100, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetEmployeeWithFilter("", "", 100, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), out Err).ToList();
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

            List<WCF_Common.OBJ_Employee> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult DetailsEmployeeDetail(int EmployeeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetEmployee_DetailWithFilter("fldEmployeeId", EmployeeId.ToString(), 0,IP, out Err).FirstOrDefault();
            string Meliyat = "0";
            string Jensiat = "0";
            var haveDetail = "1";
            if (q == null)
            {
                var q2 = servic.GetEmployeeDetail(EmployeeId,  IP, out Err);
                return Json(new
                {
                    haveDetail = "0",
                    fldEmployeeId = q2.fldId,
                    fldCodemeli = q2.fldCodemeli,
                    fldFamily = q2.fldFamily,
                    fldName = q2.fldName,
                    fldDesc = q2.fldDesc
                }, JsonRequestBehavior.AllowGet);
            }
            if (q.fldMeliyat == true)
                Meliyat = "1";
            if (q.fldJensiyat == true)
                Jensiat = "1";
            //var pic = servic.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1, Session["Username"].ToString(), Session["Password"].ToString(), out Err).FirstOrDefault();
            //var Img = Convert.ToBase64String(pic.fldImage);

            var Image = Server.MapPath("~/Content/Blank.jpg");
            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
            var Img = Convert.ToBase64String(stream.ToArray());
            int? FileId = 0;
            if (q.fldFileId != null)
            {
                var pic = servic.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1,IP, out Err).FirstOrDefault();
                if (pic.fldImage != null)
                    Img = Convert.ToBase64String(pic.fldImage);
                FileId = q.fldFileId;
            }

            return Json(new
            {
                haveDetail = haveDetail,
                fldId = q.fldId,
                fldCodemeli = q.fldCodemeli,
                fldFamily = q.fldFamily,
                fldName = q.fldName,
                fldDesc = q.fldDesc,
                ImageString = Img,
                fldMeliyat = Meliyat,
                fldAddress = q.fldAddress,
                fldCodePosti = q.fldCodePosti,
                fldEmployeeId = q.fldEmployeeId,
                fldFatherName = q.fldFatherName,
                fldFileId = FileId,
                fldMadrakId = q.fldMadrakId.ToString(),
                fldMahalSodoorId = q.fldMahalSodoorId.ToString(),
                fldMahalTavalodId = q.fldMahalTavalodId.ToString(),
                fldNezamVazifeId = q.fldNezamVazifeId.ToString(),
                fldReshteId = q.fldReshteId.ToString(),
                fldSh_Shenasname = q.fldSh_Shenasname,
                fldTaaholId = q.fldTaaholId.ToString(),
                fldTarikhSodoor = q.fldTarikhSodoor,
                fldTarikhTavalod = q.fldTarikhTavalod,
                fldJensiyat = Jensiat,
                fldMobile = q.fldMobile
                //,fldTypeShakhs = q.fldTypeShakhs.ToString()
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveEmployeeDetail(WCF_Common.OBJ_Employee_Detail Employee)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                 byte[] report_file = null; string FileName = "",e="";
                 if (Session["savePath"] != null)
                 {
                     MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                     report_file = stream.ToArray();
                     FileName = Session["FileName"].ToString();
                     e = Path.GetExtension(Session["savePath"].ToString());
                     var lenght = stream.Length;
                     if (lenght > 102400)
                     {
                         Msg = "حجم عکس انتخاب شده نباید بیشتر از 100 کیلو بایت  باشد.";
                         MsgTitle = "اخطار";

                         return Json(new
                         {
                             Msg = Msg,
                             MsgTitle = MsgTitle,
                             Er=1
                         }, JsonRequestBehavior.AllowGet);
                     }
                 }
                 else
                 {
                     var Image = Server.MapPath("~/Content/Blank.jpg");
                     MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
                     report_file = stream.ToArray();
                     FileName = "Blank.jpg";
                     e = "jpg";
                 }
                 if (Employee.fldFatherName == null)
                     Employee.fldFatherName = "";
                if (Employee.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertEmployee_Detail(Employee.fldEmployeeId, Employee.fldFatherName, Employee.fldJensiyat, Employee.fldTarikhTavalod
                        , Employee.fldMadrakId, Employee.fldNezamVazifeId, Employee.fldTaaholId, Employee.fldReshteId, report_file, e, Employee.fldSh_Shenasname,
                        Employee.fldMahalTavalodId, Employee.fldMahalSodoorId, Employee.fldTarikhSodoor, Employee.fldAddress, Employee.fldCodePosti, Employee.fldMeliyat
                        , Employee.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, Employee.fldTel, Employee.fldMobile, out Err);
                }
                else
                {
                    //ویرایش
                    var m = servic.GetEmployee_DetailWithFilter("fldId", Employee.fldId.ToString(), 1,IP, out Err).FirstOrDefault();
                   
                    if (Session["savePath"] == null)
                    {
                        byte[] picFile = null;
                        if (Employee.fldFileId == 0) 
                            picFile = report_file;
                        else
                        {
                            var pic = servic.GetFileWithFilter("fldId", Employee.fldFileId.ToString(), 1,IP, out Err).FirstOrDefault();
                            if (pic.fldImage == null)
                                picFile = report_file;
                        }
                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdateEmployee_Detail(Employee.fldId, Employee.fldEmployeeId, Employee.fldFatherName, Employee.fldJensiyat, Employee.fldTarikhTavalod
                        , Employee.fldMadrakId, Employee.fldNezamVazifeId, Employee.fldTaaholId, Employee.fldReshteId, Employee.fldFileId, picFile, e, Employee.fldSh_Shenasname,
                        Employee.fldMahalTavalodId, Employee.fldMahalSodoorId, Employee.fldTarikhSodoor, Employee.fldAddress, Employee.fldCodePosti, Employee.fldMeliyat
                        , Employee.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, Employee.fldTel, Employee.fldMobile, out Err);
                    }
                    else
                    {
                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdateEmployee_Detail(Employee.fldId, Employee.fldEmployeeId, Employee.fldFatherName, Employee.fldJensiyat, Employee.fldTarikhTavalod
                        , Employee.fldMadrakId, Employee.fldNezamVazifeId, Employee.fldTaaholId, Employee.fldReshteId, Employee.fldFileId, report_file, e, Employee.fldSh_Shenasname,
                        Employee.fldMahalTavalodId, Employee.fldMahalSodoorId, Employee.fldTarikhSodoor, Employee.fldAddress, Employee.fldCodePosti, Employee.fldMeliyat
                            , Employee.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, Employee.fldTel, Employee.fldMobile, out Err);
                    }
                }
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    System.IO.File.Delete(physicalPath);
                    Session.Remove("savePath");
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
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
