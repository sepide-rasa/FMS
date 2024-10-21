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
using System.Data.SqlClient;
using System.Data;
using NewFMS.DataSet;
using System.Collections;
using NewFMS.Models;

namespace NewFMS.Areas.Personeli.Controllers
{
    public class PersonalInfoController : Controller
    {
        //
        // GET: /Personeli/PersonalInfo/ 

        WCF_Personeli.PersoneliService servic = new WCF_Personeli.PersoneliService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Personeli.ClsError Err = new WCF_Personeli.ClsError();

        WCF_Common.CommonService servicCommon = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();

        WCF_Common_Pay.Comon_PayService Common_Payservic = new WCF_Common_Pay.Comon_PayService();
        WCF_Common_Pay.ClsError ErrC_P = new WCF_Common_Pay.ClsError();

        WCF_PayRoll.PayRolService servic_Pay = new WCF_PayRoll.PayRolService();
        WCF_PayRoll.ClsError Err_Pay = new WCF_PayRoll.ClsError();
       /// <summary>
       /// صفحه اصلی فرم تعریف پرسنل
       /// </summary>
       /// <param name="containerId">.جهت باز شدن فرم در تب اطلاعات پرسنل، از این پارامتر برای مشخص کردن اسم تب استفاده میشود</param>
       /// <returns></returns>
        public ActionResult Index(string containerId)
        {//باز شدن تب جدید
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area=""});
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        /// <summary>
        /// باز شدن فرم جهت ذخیره اطلاعات جدید و ویرایش اطلاعات موجود
        /// </summary>
        /// <param name="id">کد جدول پرسنل</param>
        /// <param name="containerId"></param>
        /// <returns></returns>
        public ActionResult New(int Id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account" ,new { area=""});
            var result = new Ext.Net.MVC.PartialViewResult();
            var Date = servicCommon.GetDate( IP.ToString(), out ErrCommon);
            result.ViewBag.Date = Date.fldDateTime;
            result.ViewBag.Id = Id;
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
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
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        /// <summary>
        /// صفحه اصلی فرم گروه تشویقی
        /// </summary>
        /// <param name="id">کد جدول پرسنل</param>
        /// <param name="containerId">شناسه تب گروه تشویقی در فرم پرسنل</param>
        /// <returns></returns>
        public ActionResult GoruhTashvighi(int id, string containerId)
        {//باز شدن تب
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.PersonelInfo = id;
            Session["PersonelInfo"] = id;
            return result;
        }
        public ActionResult GoruhTashvighiWin(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.PrsPersonalInfoId = id;
            return PartialView;
        }
        /// <summary>
        /// باز شدن فرم جهت ذخیره اطلاعات جدید و ویرایش اطلاعات موجود
        /// </summary>
        /// <param name="id">کد جدول سوابق گروه تشویقی</param>
        /// <returns></returns>
        public ActionResult newGrohTashvighi(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.PersonelInfo = Session["PersonelInfo"];
            return PartialView;
        }
        /// <summary>
        /// باز شدن فرم تغییر وضعیت جهت ذخیره اطلاعات جدید
        /// </summary>
        /// <param name="id">کد جدول پرسنل</param>
        /// <param name="State">جهت استفاده از یک فرم در حالت های مختلف از این متغیر برای مشخص کردن حالت استفاده میشود</param>
        /// <returns></returns>
        public ActionResult ChangeStatus(int id, int State,int? OrganId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.PrsPersonalInfoId = id;
            PartialView.ViewBag.State = State;
            PartialView.ViewBag.OrganId = OrganId != null ? OrganId.ToString() : Session["OrganId"].ToString();
            return PartialView;
        }
        /// <summary>
        /// باز شدن فرم تغییر وضعیت استخدام جهت ذخیره اطلاعات جدید
        /// </summary>
        /// <param name="id">کد جدول پرسنل</param>
        /// <returns></returns>
        public ActionResult ChangeEstekhdamStatus(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.PrsPersonalInfoId = id;
            return PartialView;
        }

        public ActionResult ChangeTahsilat(int EmployeeId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.EmployeeId = EmployeeId;
            return PartialView;
        }
        public ActionResult IndexTasvieHesab(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.PrsPersonalInfoId = id;
            return PartialView;
        }
        /// <summary>
        /// صفحه اصلی فرم سوابق سنوات خدمت
        /// </summary>
        /// <param name="id">کد جدول پرسنل</param>
        /// <param name="containerId">شناسه تب سوابق سنوات خدمت در فرم پرسنل</param>
        /// <returns></returns>
        public ActionResult SavabeghSanavatKhedmat(int id, string containerId)
        {//باز شدن تب
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult
            {
                WrapByScriptTag = true,
                ContainerId = containerId,
                RenderMode = RenderMode.AddTo
            };
            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            result.ViewBag.PersonId = id;
            Session["PersonalId"] = id;
            return result;
        }
        public ActionResult SavabeghSanavatKhedmatWin(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.PrsPersonalInfoId = id;
            return PartialView;
        }
        /// <summary>
        /// باز شدن فرم سوابق سنوات خدمت جهت ذخیره اطلاعات جدید
        /// </summary>
        /// <param name="id">کد جدول سوابق سنوات خدمت</param>
        /// <param name="PersonId">کد جدول پرسنل</param>
        /// <returns></returns>
        public ActionResult newSavabeghSanavatKhedmat(int id, int PersonId)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            PartialView.ViewBag.PersonId = PersonId;
            return PartialView;
        }
        /// <summary>
        /// .تمامی اطلاعات جدول شهرها را در لیست بازشو نمایش میدهد
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMahalTavalod()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCommon.GetCityWithFilter("", "", 0, IP, out ErrCommon).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);

        }
        /// <summary>
        /// .تمامی اطلاعات جدول شهرها را در لیست بازشو نمایش میدهد
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMahlSodoor()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCommon.GetCityWithFilter("", "", 0,IP, out ErrCommon).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);

        }
        /// <summary>
        /// .تمامی اطلاعات وضعیت ایثارگری را در لیست بازشو نمایش میدهد
        /// </summary>
        /// <returns></returns>
        public ActionResult GetEsargari()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetVaziyatEsargariWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);

        }
        public ActionResult GetRaste()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCommon.GetRasteWithFilter("", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), out ErrCommon).ToList().Select(c => new { fldId = c.fldText, fldName = c.fldText });
            return this.Store(q);

        }
        /// <summary>
        /// .تمامی اطلاعات جدول مدرک تحصیلی را در لیست بازشو نمایش میدهد
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMadrakTahsili()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCommon.GetMadrakTahsiliWithFilter("", "", 0,IP, out ErrCommon).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);

        }
        /// <summary>
        /// .تمامی اطلاعات نظام وظیفه را در لیست بازشو نمایش میدهد
        /// </summary>
        /// <returns></returns>
        public ActionResult GetNezamVazife()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCommon.GetNezamVazifeWithFilter("", "", 0,IP, out ErrCommon).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);

        }
        public ActionResult GetNoeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetAnvaEstekhdamWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        /// <summary>
        /// .تمامی اطلاعات جدول وضعیت را در لیست بازشو نمایش میدهد
        /// </summary>
        /// <returns></returns>
        
        public ActionResult GetStatus()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetStatusWithFilter("", "", 0, IP,out ErrC_P).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult GetStatusTaahol()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCommon.GetStatusTaaholWithFilter("", "", 0,IP, out ErrCommon).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        /// <summary>
        /// .تمامی اطلاعات جدول انواع گروه تشویقی را در لیست بازشو نمایش میدهد
        /// </summary>
        /// <returns></returns>
        public ActionResult GetNoeGroh()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetAnvaGroupTashvighiWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
        
        /// <summary>
        /// .تمامی اطلاعات جدول انواع استخدام را در لیست بازشو نمایش میدهد
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAnvaeEstekhdam()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = Common_Payservic.GetAnvaEstekhdamWithFilter("", "", 0, IP, out ErrC_P).ToList().Select(c => new { ID = c.fldId, Name = c.fldTitle });
            return this.Store(q);
        }
       
        /// <summary>
        /// این متد برای آپلود عکس پرسنل در فرم استفاده میشود
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload()
        {
            if (Request.ContentLength <= 102400)
            {
                if (Session["savePathUser"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathUser"].ToString());
                    Session.Remove("savePathUser");
                    Session.Remove("FileName");
                    System.IO.File.Delete(physicalPath);
                }
                HttpPostedFileBase file = Request.Files[0];
                if ((file.ContentType).Substring(0, 5) == "image")
                {
                    string savePath = Server.MapPath(@"~\Uploaded\" + file.FileName);
                    file.SaveAs(savePath);
                    Session["FileName"] = file.FileName;
                    Session["savePathUser"] = savePath;
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
                        Icon = MessageBox.Icon.INFO,
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
        ///.این متد برای نمایش عکس استفاده میشود
        /// </summary>
        /// <param name="FileId">کد جدول فایل</param>
        /// <returns>.فیلد عکس ذخیره شده در جدول فایل را برمیگرداند</returns>
        public FileContentResult ShowPic(int FileId)
        {//برگرداندن عکس 
            var q = servicCommon.GetFileWithFilter("fldId",FileId.ToString(),1,IP, out ErrCommon).FirstOrDefault();
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

        public ActionResult ShowImage()
        {//برگرداندن عکس 
            byte[] file = null;
            MemoryStream stream = null;
            stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathUser"].ToString()));
            var image = Convert.ToBase64String(stream.ToArray());
            return Json(new { image = image });
        }
       
        //public ActionResult HaveFile(int? EmployeeId)
        //{
        //    bool HaveFile = false;
        //    if ((EmployeeId != null && EmployeeId != 0) || Session["savePath"] != null)
        //        HaveFile = true;
        //    return Json(new
        //    {
        //        HaveFile = HaveFile
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //public FileContentResult Download(int UserId)
        //{
        //    var q = servicCommon.GetEmployeeWithFilter("fldId", UserId.ToString(), 1, Session["Username"].ToString(), (Session["Password"].ToString()), out ErrCommon).FirstOrDefault();
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
        //        var qq = servicCommon.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1, Session["Username"].ToString(), (Session["Password"].ToString()), out ErrCommon).FirstOrDefault();
        //        if (qq.fldImage != null)
        //        {
        //            string e1 = "jpg";
        //            // qq.fldName.Substring(qq.fldName.IndexOf('.') + 1);


        //            MemoryStream st = new MemoryStream(qq.fldImage);
        //            return File(st.ToArray(), MimeType.Get(e1), "fileUpload." + e1);
        //        }

        //    }
        //    return null;
        //}

        public ActionResult CheckMohasebat(int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0; bool flag=false; int data = 0; ;
            try
            {
             
              
                    var q = servic_Pay.CheckAllMohasebat(PersonalId, Convert.ToInt32(Session["OrganId"]), IP, out Err_Pay).ToList();
                    var a = q.Where(l => l.fldFlag == true).FirstOrDefault();
                    if(a!= null )
                    {
                        MsgTitle = "اخطار";
                        Msg = "حقوق ماه جاری بسته شده است،  تغییر محل خدمت مجاز نیست. ";
                        flag = a.fldFlag;
                        data = 1;
                        Er = 1;
                    }
                    else if(q.Count!=0)
                    {
                        MsgTitle = "هشدار";
                        Msg = "برای شخص مورد نظر در ماه جاری محاسبات حقوق انجام شده است، در صورت تغییر محل خدمت محاسبات وی حذف خواهد شد. آیا مایل به ادامه عملیات هسستید؟. ";
                        flag = q[0].fldFlag;
                        data = 1;
                    }
             

            }
            catch (Exception x)
            {
                if (Session["savePathUser"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathUser"].ToString());
                    Session.Remove("savePathUser");
                    System.IO.File.Delete(physicalPath);
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
                Er = Er,
                Msg = Msg,
                MsgTitle = MsgTitle,
                flag=flag,
                data=data
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// از این متد برای ذخیره اطلاعات جدید یا اطلاعات ویرایش شده استفاده می شود 
        /// </summary>
        ///  <remarks><para> این قسمت به دو قسمت تقسیم میشودUpdate کد</para>
        ///  <para> .میکند Update اگر عکس کارمند نیاز به ویرایش داشته باشد آنرا هم</para>
        ///  <para>پر میشود NULL در غیراینصورت فیلد مربوط به عکس با مقدار</para> </remarks>
        /// <example>
        /// <code>
        /// if(Personal.Id==0)
        ///          {
        ///              //Insert
        ///          }
        ///       else
        ///          {
        ///              //Update
        ///          }
        /// </code>
        /// </example>
        /// <param name="Personal">که شامل فیلدهای جدول پرسنل و جدول کارمند به همراه مقادیر آن ها Object</param>
        ///  <returns>موفقیت آمیز باشد پیغام ذخیره موفق یا ویرایش موفق را نمایش میدهد Update و Insert در صورتی که</returns>
        public ActionResult Save(WCF_Personeli.OBJ_PersonalInfo Personal, WCF_Personeli.OBJ_Employee Employee, WCF_Personeli.OBJ_Employee_Detail EmployeeDetail, bool fldPerStatus, int fldNoeEstekhdam, int FileId, byte? DelCalc)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;
            try
            {
                byte[] report_file = null; string FileName = "",e="";
                var Date = servicCommon.GetDate( IP.ToString(), out ErrCommon);
                if (DelCalc == 1)
                {
                    var p = servic_Pay.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", Personal.fldId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                    servic_Pay.MohasebatDelete("AllMohasebat_Personal", Convert.ToInt32(p.fldId),Convert.ToByte(0), Convert.ToInt16(0), Convert.ToByte(0), "", "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, Convert.ToInt32(Session["OrganId"]), out Err_Pay);
                    if (Err_Pay.ErrorType==true )
                    {
                        Msg = Err_Pay.ErrorMsg;
                        MsgTitle ="خطا";
                        Er = 1;

                        return Json(new
                        {
                            Er = Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (Session["savePathUser"] != null)
                {
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathUser"].ToString()));
                    report_file = stream.ToArray();
                    FileName = Session["FileName"].ToString();
                    e = Path.GetExtension(Session["savePathUser"].ToString());
                    var lenght = stream.Length;
                    if (lenght > 102400)
                    {
                        Msg = "حجم عکس انتخاب شده نباید بیشتر از 100 کیلو بایت  باشد.";
                        MsgTitle = "اخطار";
                        Er=1;

                        return Json(new
                        {
                            Er=Er,
                            Msg = Msg,
                            MsgTitle = MsgTitle
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
                Employee.fldStatus = fldPerStatus;
                Employee.fldId = Personal.fldEmployeeId;
                if(Personal.fldJensiyat==false)
                {
                    Personal.fldNezamVazifeId = null;
                }
                if (EmployeeDetail.fldCodePosti == null)
                    EmployeeDetail.fldCodePosti = "";
                if (EmployeeDetail.fldAddress == null)
                    EmployeeDetail.fldAddress = "";
                if (Personal.fldId == 0)
                {
                    //ذخیره
                    //byte[] picFile = null;
                    //if (FileId == 0)
                    //    picFile = report_file;
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertPersoneliInfo(Employee,EmployeeDetail,Personal,report_file,e,MyLib.Shamsi.Miladi2ShamsiString(Date.fldDateTime),
                        fldNoeEstekhdam, MyLib.Shamsi.Miladi2ShamsiString(Date.fldDateTime), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    if (Err.ErrorType)
                    {
                        return Json(new
                        {
                            Er = 1,
                            Msg = Err.ErrorMsg,
                            MsgTitle = "خطا"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (Personal.fldChartOrganId != 0)
                    {
                        var post = servicCommon.GetOrganizationalPostsWithFilter("fldId", Personal.fldOrganPostId.ToString(), 0,
                            Session["Username"].ToString(), (Session["Password"].ToString()), IP, out ErrCommon).FirstOrDefault();
                        if (post!=null&&post.fldChartOrganId != Personal.fldChartOrganId)
                        {
                            var post1 = servicCommon.GetOrganizationalPostsWithFilter("fldTitle", post.fldTitle, 0, Session["Username"].ToString(),
                                (Session["Password"].ToString()), IP, out ErrCommon).Where(k => k.fldChartOrganId == Personal.fldChartOrganId
                                && k.fldOrgPostCode == post.fldOrgPostCode).FirstOrDefault();
                            if (post1 == null)
                            {
                              
                                servicCommon.InsertOrganizationalPosts(post.fldTitle, post.fldOrgPostCode, Personal.fldChartOrganId, post.fldPID, post.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrCommon);
                                if (ErrCommon.ErrorType)
                                {
                                    return Json(new
                                    {
                                        Er = 1,
                                        Msg = ErrCommon.ErrorMsg,
                                        MsgTitle = "خطا"
                                    }, JsonRequestBehavior.AllowGet);
                                }
                                post1 = servicCommon.GetOrganizationalPostsWithFilter("fldTitle", post.fldTitle, 0, Session["Username"].ToString(),
                                   (Session["Password"].ToString()), IP, out ErrCommon).Where(k => k.fldChartOrganId == Personal.fldChartOrganId
                                   && k.fldOrgPostCode == post.fldOrgPostCode).FirstOrDefault();
                            }
                            if (post1 != null)
                                Personal.fldOrganPostId = post1.fldId;
                        }
                    }
                    //ویرایش
                    if (Session["savePathUser"] == null)
                    {
                        byte[] picFile = null;
                        if (FileId == 0)
                            picFile = report_file;
                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdatePersonelilInfo(Employee, EmployeeDetail, Personal, FileId, picFile, e, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdatePersonelilInfo(Employee, EmployeeDetail, Personal, FileId, report_file, e, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            return Json(new
                            {
                                Er = 1,
                                Msg = Err.ErrorMsg,
                                MsgTitle = "خطا"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                if (Session["savePathUser"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathUser"].ToString());
                    Session.Remove("savePathUser");
                    System.IO.File.Delete(physicalPath);
                }
            }
            catch (Exception x)
            {
                if (Session["savePathUser"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathUser"].ToString());
                    Session.Remove("savePathUser");
                    System.IO.File.Delete(physicalPath);
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
                Er=Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// از این متد برای ذخیره اطلاعات جدید یا اطلاعات ویرایش شده استفاده می شود
        /// </summary>
        /// <example>
        /// <code>
        /// if(SavabeghTashvighi.Id==0)
        ///          {
        ///              //Insert
        ///          }
        ///       else
        ///          {
        ///              //Update
        ///          }
        /// </code>
        /// </example>
        /// <param name="SavabeghTashvighi">که شامل فیلدهای جدول سوابق گروه های تشویقی به همراه مقادیر آن ها Object</</param>
        /// <returns>موفقیت آمیز باشد پیغام ذخیره موفق یا ویرایش موفق را نمایش میدهد Update و Insert در صورتی که</returns>
        public ActionResult SaveSavabeghTashvighi(WCF_Personeli.OBJ_SavabeghGroupTashvighi SavabeghTashvighi)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (SavabeghTashvighi.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertSavabeghGroupTashvighi(SavabeghTashvighi.fldPersonalId, SavabeghTashvighi.fldAnvaGroupId, SavabeghTashvighi.fldTedadGroup, SavabeghTashvighi.fldTarikh, SavabeghTashvighi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateSavabeghGroupTashvighi(SavabeghTashvighi.fldId, SavabeghTashvighi.fldPersonalId, SavabeghTashvighi.fldAnvaGroupId, SavabeghTashvighi.fldTedadGroup, SavabeghTashvighi.fldTarikh, SavabeghTashvighi.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteStatus(int id, int fldPrsPersonalInfoId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                //var q = servic.GetPersonalInfoSelect("fldCostCenterId", id.ToString(), 1).FirstOrDefault();
                //if (q != null)
                //{
                //    Msg = "آیتم مورد نظر جای دیگری استفاده شده است.";
                //    MsgTitle = "خطا";
                //}
                //else
                //{
                var q = servic.GetPersonalStatusWithFilter("fldPrsPersonalInfoId", fldPrsPersonalInfoId.ToString(),0, IP, out Err).ToList();
                if (q.Count > 1)
                {
                    Msg = servic.DeletePersonalStatus(id, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                    MsgTitle = "حذف موفق";
                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                    }
                }
                else
                {
                    MsgTitle = "خطا";
                    Msg = "امکان حذف برای تنها وضعیت مشخص شده وجود ندارد. ";
                    Er = 1;
                }
                //}
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
        /// <summary>
        /// از این متد برای ذخیره اطلاعات جدید استفاده می شود
        /// </summary>
        /// <param name="PersonalStatus">که شامل فیلدهای جدول تغییر وضعیت به همراه مقادیر آن ها Object</param>
        /// <returns>موفقیت آمیز باشد پیغام ذخیره موفق را نمایش میدهد Insert در صورتی که</returns>
        public ActionResult SaveStatus(WCF_Personeli.OBJ_PersonalStatus PersonalStatus,int? pay  )
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (PersonalStatus.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertPersonalStatus(PersonalStatus.fldStatusId, PersonalStatus.fldPrsPersonalInfoId, null,
                        PersonalStatus.fldDateTaghirVaziyat, PersonalStatus.fldDesc, Session["Username"].ToString(),
                        (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdatePersonalStatus(PersonalStatus.fldId, PersonalStatus.fldStatusId, PersonalStatus.fldPrsPersonalInfoId,
                        null, PersonalStatus.fldDateTaghirVaziyat, PersonalStatus.fldDesc, Session["Username"].ToString(),
                        (Session["Password"].ToString()), IP, out Err);
                }
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
                else if(pay==1)
                {
                    var q = servic_Pay.GetPayPersonalInfoWithFilter("fldPrs_PersonalInfoId", PersonalStatus.fldPrsPersonalInfoId.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err_Pay).FirstOrDefault();
                    if(q!=null)
                    {
                        //ذخیره
                        MsgTitle = "ذخیره موفق";
                        Msg = servic.InsertPersonalStatus(PersonalStatus.fldStatusId, null, q.fldId,
                            PersonalStatus.fldDateTaghirVaziyat, PersonalStatus.fldDesc, Session["Username"].ToString(),
                            (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                        if (Err.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err.ErrorMsg;
                            Er = 1;
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
                Msg = Msg,
                MsgTitle = MsgTitle,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// از این متد برای ذخیره اطلاعات جدید استفاده می شود
        /// </summary>
        /// <param name="HistoryNoeEstekhdam">که شامل فیلدهای جدول تاریخچه نوع استخدام به همراه مقادیر آن ها Object</param>
        /// <returns>موفقیت آمیز باشد پیغام ذخیره موفق را نمایش میدهد Insert در صورتی که</returns>
        public ActionResult SaveEstekhdamStatus(WCF_Personeli.OBJ_HistoryNoeEstekhdam HistoryNoeEstekhdam)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (HistoryNoeEstekhdam.fldId == 0)
                {
                    Msg = servic.InsertHistoryNoeEstekhdam(HistoryNoeEstekhdam.fldPrsPersonalInfoId, HistoryNoeEstekhdam.fldNoeEstekhdamId, HistoryNoeEstekhdam.fldTarikh, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ذخیره موفق";

                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                    }
                }
                else
                {
                    Msg = servic.UpdateHistoryNoeEstekhdam(HistoryNoeEstekhdam.fldId, HistoryNoeEstekhdam.fldPrsPersonalInfoId, HistoryNoeEstekhdam.fldNoeEstekhdamId, HistoryNoeEstekhdam.fldTarikh, "", Session["Username"].ToString(), (Session["Password"].ToString()),  IP, out Err);
                    MsgTitle = "ویرایش موفق";

                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
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
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveTahsilatStatus(WCF_Common.OBJ_HistoryTahsilat HistoryTahsilat)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (HistoryTahsilat.fldId == 0)
                {
                    Msg = servicCommon.InsertHistoryTahsilat(HistoryTahsilat.fldEmployeeId, HistoryTahsilat.fldMadrakId,HistoryTahsilat.fldReshteId, HistoryTahsilat.fldTarikh,"", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrCommon);
                    MsgTitle = "ذخیره موفق";

                    if (ErrCommon.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = ErrCommon.ErrorMsg;
                        Er = 1;
                    }
                }
                else
                {
                    var q = servicCommon.GetHistoryTahsilatDetail(HistoryTahsilat.fldId, IP, out ErrCommon);
                    var c = servic.GetPersonalHokmWithFilter("CheckEditTashilat", q.fldTarikh, q.fldTitleReshte, q.fldEmployeeId, 1, q.fldMadrakId, IP, out Err).FirstOrDefault();
                    if (c != null)
                    {
                        return Json(new
                        {
                            Msg = "اطلاعات شخص مورد نظر در حکم شخص استفاده شده است لذا شما اجازه حذف ندارید.",
                            MsgTitle = "خطا",
                            Er = 1
                        }, JsonRequestBehavior.AllowGet);
                    }
                    Msg = servicCommon.UpdateHistoryTahsilat(HistoryTahsilat.fldId, HistoryTahsilat.fldEmployeeId, HistoryTahsilat.fldMadrakId, HistoryTahsilat.fldReshteId, HistoryTahsilat.fldTarikh, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrCommon);
                    MsgTitle = "ویرایش موفق";

                    if (ErrCommon.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = ErrCommon.ErrorMsg;
                        Er = 1;
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
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveTasviehHesab(WCF_Personeli.OBJ_TasviehHesab TasviehHesab)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (TasviehHesab.fldId == 0)
                {
                    Msg = servic.InsertTasviehHesab(TasviehHesab.fldPrsPersonalInfoId, TasviehHesab.fldTarikh, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    MsgTitle = "ذخیره موفق";

                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
                    }
                }
                else
                {
                    Msg = servic.UpdateTasviehHesab(TasviehHesab.fldId, TasviehHesab.fldPrsPersonalInfoId, TasviehHesab.fldTarikh, "", Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
                    MsgTitle = "ویرایش موفق";

                    if (Err.ErrorType)
                    {
                        MsgTitle = "خطا";
                        Msg = Err.ErrorMsg;
                        Er = 1;
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
                Msg = Msg,
                MsgTitle = MsgTitle,
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteEstekhdamStatus(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                var q=servic.GetHistoryNoeEstekhdamDetail(id,Convert.ToInt32(Session["OrganId"]),  IP, out Err);
                var c = servic.GetPersonalHokmWithFilter("CheckEditEstkhedam", q.fldTarikh, "",q.fldPrsPersonalInfoId, 1,0, IP, out Err).FirstOrDefault();
                if (c != null)
                {
                    return Json(new
                    {
                        Msg = "اطلاعات شخص مورد نظر در حکم شخص استفاده شده است لذا شما اجازه حذف ندارید.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
               
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteHistoryNoeEstekhdam((id), Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
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

        public ActionResult DeleteTahsilatStatus(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                var q = servicCommon.GetHistoryTahsilatDetail(id,  IP, out ErrCommon);
                var c = servic.GetPersonalHokmWithFilter("CheckEditTashilat", q.fldTarikh, q.fldTitleReshte, q.fldEmployeeId, 1,q.fldMadrakId, IP, out Err).FirstOrDefault();
                if (c != null)
                {
                    return Json(new
                    {
                        Msg = "اطلاعات شخص مورد نظر در حکم شخص استفاده شده است لذا شما اجازه حذف ندارید.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }

                //حذف
                MsgTitle = "حذف موفق";
                Msg = servicCommon.DeleteHistoryTahsilat((id), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out ErrCommon);
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

        public ActionResult DeleteTasviehHesab(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                var q = servic.GetTasviehHesabDetail(id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
                var c = servic.GetHistoryNoeEstekhdamWithFilter("CheckTarikhTasvie", q.fldTarikh,  q.fldPrsPersonalInfoId, 1, IP, out Err).FirstOrDefault();
                if (c != null)
                {
                    return Json(new
                    {
                        Msg = "اطلاعات شخص مورد نظر در تاریخچه استخدام استفاده شده است لذا شما اجازه حذف ندارید.",
                        MsgTitle = "خطا",
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }

                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteTasviehHesab((id), Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
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

        /// <summary>
        /// از این متد برای ذخیره اطلاعات جدید یا اطلاعات ویرایش شده استفاده می شود 
        /// </summary>
        /// <example>
        /// <code>
        /// if(SanavateKHedmat.Id==0)
        ///          {
        ///              //Insert
        ///          }
        ///       else
        ///          {
        ///              //Update
        ///          }
        /// </code>
        /// </example>
        /// <param name="SanavateKHedmat">که شامل فیلدهای جدول سوابق سنوات خدمت به همراه مقادیر آن ها Object</param>
        /// <returns>موفقیت آمیز باشد پیغام ذخیره موفق یا ویرایش موفق را نمایش میدهد Update و Insert در صورتی که</returns>
        public ActionResult SaveSavabeghSanavatKhedmat(WCF_Personeli.OBJ_SavabegheSanavateKHedmat SanavateKHedmat)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (SanavateKHedmat.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertSavabegheSanavateKHedmat(SanavateKHedmat.fldPersonalId, SanavateKHedmat.fldNoeSabeghe, SanavateKHedmat.fldAzTarikh, SanavateKHedmat.fldTaTarikh, SanavateKHedmat.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateSavabegheSanavateKHedmat(SanavateKHedmat.fldId, SanavateKHedmat.fldPersonalId, SanavateKHedmat.fldNoeSabeghe, SanavateKHedmat.fldAzTarikh, SanavateKHedmat.fldTaTarikh, SanavateKHedmat.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// جهت تشخیص نوع استخدام شخص مورد نظر که کارمند است یا خیر استفاده میشود.
        /// </summary>
        /// <param name="id">کد جدول پرسنل</param>
        /// <returns></returns>
        public ActionResult CheckNoeEstekhdam(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Msg = "";
            var MsgTitle = "";
            var q = servic.GetMaxPersonalEstekhdamType("",(id),"", IP, out Err).FirstOrDefault();
            if (q != null)
            {
                var p = Common_Payservic.GetAnvaEstekhdamWithFilter("fldId", q.fldAnvaEstekhdamId.ToString(), 0, IP, out ErrC_P).FirstOrDefault();
                if (p.fldNoeEstekhdamId > 1)
                {
                    Msg = "";
                    MsgTitle = "";
                }
                else
                {
                    Msg = "نوع استخدام شخص انتخاب شده جهت ثبت سوابق می بایست کارمندی انتخاب شود.";
                    MsgTitle = "خطا";
                }
            }
            return Json(new { Msg = Msg, MsgTitle = MsgTitle});
        }
        /// <summary>
        ///  جستجو اطلاعات پرسنل بر اساس کد ملی جدول کارمند
        /// </summary>
        /// <param name="NationalCode">کد ملی کارمند</param>
        /// <param name="updateCheck"></param>
        /// <returns>.در صورتی پرسنلی با کد ملی فرستاده شده وجود داشته باشد اطلاعات آن را بر میگرداند در غیر این صورت مقدار خالی بر می گرداند</returns>
        public ActionResult ChekMeliCode(string NationalCode/*, int updateCheck*/)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCommon.GetEmployeeWithFilter("fldCodemeli", NationalCode, 0, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), out ErrCommon).FirstOrDefault();
            string Gender = "0";
            string Meliyat = "0";
                string Status = "0";
            if (q != null)
            {
            if (q.fldStatus)
                Status = "1";
                var z = servic.GetPersoneliInfoWithFilter("fldEmployeeId", q.fldId.ToString(),Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                if (z == null /*|| updateCheck == 1*/)
                {
                    var Image = Server.MapPath("~/Content/Blank.jpg");
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
                    var Img = Convert.ToBase64String(stream.ToArray());
                    int? fileId = 0;

                    var k = servicCommon.GetEmployee_DetailWithFilter("fldEmployeeId", q.fldId.ToString(), 0, IP, out ErrCommon).FirstOrDefault();
                    if (k == null)
                    {
                        return Json(new
                        {
                            havePerson = 0,
                            haveDetail=0,
                            fldId = q.fldId,
                            fldName = q.fldName,
                            fldFamilyName = q.fldFamily,
                            fldStatus=Status,
                            fldDesc = q.fldDesc,
                            ImageString = Img,
                            fldFileId = fileId
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (k.fldFileId != null)
                        {
                            var pic = servicCommon.GetFileWithFilter("fldId", k.fldFileId.ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                            Img = Convert.ToBase64String(pic.fldImage);
                            fileId = k.fldFileId;
                        }
                        if (k.fldJensiyat == true)
                            Gender = "1";
                        if (k.fldMeliyat == true)
                            Meliyat = "1";
                        return Json(new
                        {
                            havePerson = 0,
                            haveDetail = 1,
                            fldId = q.fldId,
                            fldName = q.fldName,
                            fldFamilyName = q.fldFamily,
                            fldStatus = Status,
                            fldJensiyat = Gender,
                            fldFatherName = k.fldFatherName,
                            fldDesc = q.fldDesc,
                            ImageString = Img,
                            fldMeliyat = Meliyat,
                            fldAddress = k.fldAddress,
                            fldCodePosti = k.fldCodePosti,
                            fldEmployeeId = k.fldEmployeeId,
                            fldFileId = fileId,
                            fldMadrakId = k.fldMadrakId.ToString(),
                            fldMahalSodoorId = k.fldMahalSodoorId.ToString(),
                            fldMahalTavalodId = k.fldMahalTavalodId.ToString(),
                            fldNezamVazifeId = k.fldNezamVazifeId.ToString(),
                            fldReshteId = k.fldReshteId.ToString(),
                            fldSh_Shenasname = k.fldSh_Shenasname,
                            fldTaaholId = k.fldTaaholId.ToString(),
                            fldTarikhSodoor = k.fldTarikhSodoor,
                            fldTarikhTavalod = k.fldTarikhTavalod,
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        havePerson = 1,
                        Msg = "پرسنل قبلا در سیستم ثبت شده است",
                        MsgTitle = "خطا"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    havePerson = 0,
                    fldFamilyName = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckSh_Personeli(string Sh_Personeli,int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string MsgTitle = ""; string Msg = ""; byte Er = 0;
            try
            {
                var q=servic.GetPersoneliInfoWithFilter("CheckSh_Personali", Sh_Personeli,Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).Where(l=>l.fldId!=Id).FirstOrDefault();
                if (Err.ErrorType)
                {
                    return Json(new
                    {
                        MsgTitle = "خطا",
                        Msg = Err.ErrorMsg,
                        Er = 1
                    }, JsonRequestBehavior.AllowGet);
                }
                if (q != null)
                {
                    MsgTitle = "خطا";
                    Msg = "شماره پرسنلی وارد شده تکراری است.";
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
                MsgTitle = MsgTitle,
                Msg = Msg,
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// .برای حذف یک سطر از جدول پرسنل بر اساس کد فرستاده شده استفاده میشود
        /// </summary>
        /// <param name="id">کد جدول پرسنلی</param>
        /// <returns></returns>
        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; byte Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeletePersoneliInfo(Id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
                    Er = 1;
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
                Er = 1;
            }
            return Json(new
            {
                Er=Er,
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// .برای حذف یک سطر از سوابق گروه های تشویقی بر اساس کد فرستاده شده استفاده میشود
        /// </summary>
        /// <param name="id">کد جدول سوابق گروه های تشویقی</param>
        /// <returns></returns>
        public ActionResult DeleteSavabeghTashvighi(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteSavabeghGroupTashvighi(Convert.ToInt32(id), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        /// .برای حذف یک سطر از جدول سنوات سوابق خدمت بر اساس کد فرستاده شده استفاده میشود
        /// </summary>
        /// <param name="id">کد جدول سنوات سوابق خدمت</param>
        /// <returns></returns>
        public ActionResult DeleteSavabeghSanavatKhedmat(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteSavabegheSanavateKHedmat(Convert.ToByte(id), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        /// .برای دسترسی به کد انواع استخدام بر اساس کد فرستاده شده استفاده می شود
        /// </summary>
        /// <param name="Id">کد جدول پرسنل</param>
        /// <returns>.کد انواع استخدام مربوط به کد پرسنل فرستاده شده را بر میگرداند</returns>
        public ActionResult EstekhdamDetail(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //var q = servic.GetMaxPersonalEstekhdamType("", Id, "",IP, out Err).FirstOrDefault();
            var q = servic.GetHistoryNoeEstekhdamDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
            var c = servic.GetPersonalHokmWithFilter("CheckEditEstkhedam", q.fldTarikh, "", q.fldPrsPersonalInfoId, 1,0, IP, out Err).FirstOrDefault();
            var Er = 0;
            if (c != null)
                Er = 1;
            var fldAnvaEstekhdamId = "";
                fldAnvaEstekhdamId = q.fldNoeEstekhdamId.ToString();

            return Json(new
            {
                fldAnvaeEstekhdamId = q.fldNoeEstekhdamId.ToString(),
                fldTarikh=q.fldTarikh,
                fldId=q.fldId,
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TahsilatDetail(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //var q = servic.GetMaxPersonalEstekhdamType("", Id, "",IP, out Err).FirstOrDefault();
            var q = servicCommon.GetHistoryTahsilatDetail(Id, IP, out ErrCommon);
            var c = servic.GetPersonalHokmWithFilter("CheckEditTashilat", q.fldTarikh, q.fldReshteId.ToString(), q.fldEmployeeId,1,q.fldMadrakId, IP, out Err).FirstOrDefault();
            var Er = 0;
            if (c != null)
                Er = 1;

            return Json(new
            {
                fldMadrakId = q.fldMadrakId.ToString(),
                fldTarikh = q.fldTarikh,
                fldReshteId = q.fldReshteId.ToString(),
                fldId = q.fldId,
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TasviehHesabDetail(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            //var q = servic.GetMaxPersonalEstekhdamType("", Id, "",IP, out Err).FirstOrDefault();
            var q = servic.GetTasviehHesabDetail(Id, Convert.ToInt32(Session["OrganId"]), IP, out Err);
          
            return Json(new
            {
                fldTarikh = q.fldTarikh,
                fldId = q.fldId
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// .برای دسترسی به اطلاعات جدول تغییر وضعیت بر اساس کد فرستاده شده استفاده می شود
        /// </summary>
        /// <param name="Id">کد جدول پرسنل</param>
        /// <returns>.اطلاعات جدول تغییر وضعیت را بر اساس کد جدول پرسنل بر میگرداند</returns>
        public ActionResult StatusDetail(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPersonalStatusWithFilter("fldStatusChangeEnd_KargoziniId", Id.ToString(), 1, IP, out Err).FirstOrDefault();
            var fldId = 0; var fldStatusId = ""; var fldDesc = "";
            if (q != null)
            {
                fldId = q.fldId;
                fldStatusId = q.fldStatusId.ToString();
                fldDesc = q.fldDesc;
            }
            return Json(new
            {
                fldId = fldId,
                fldStatusId = fldStatusId,
                fldDesc = fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// .برای دسترسی به اطلاعات جدول سوابق گروه تشویقی بر اساس کد فرستاده شده استفاده می شود
        /// </summary>
        /// <param name="Id">کد جدول سوابق گروه تشویقی</param>
        /// <returns>.اطلاعات مربوط به یک سطر از جدول سوابق گروه تشویقی را بر میگرداند</returns>
        public ActionResult DetailsSavabeghTashvighi(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSavabeghGroupTashvighiDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldAnvaGroupId = q.fldAnvaGroupId.ToString(),
                fldPersonalId = q.fldPersonalId,
                fldTarikh = q.fldTarikh,
                fldTedadGroup = q.fldTedadGroup,
                fldAnvaGroupTashvighiTitle = q.fldAnvaGroupTashvighiTitle,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// .برای دسترسی به اطلاعات جدول سوابق سنوات خدمت بر اساس کد فرستاده شده استفاده می شود
        /// </summary>
        /// <param name="Id">کد جدول سوابق سنوات خدمت</param>
        /// <returns>.اطلاعات مربوط به یک سطر از جدول سوابق سنوات خدمت را بر میگرداند</returns>
        public ActionResult SavabeghSanavatKhedmatDetail(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSavabegheSanavateKHedmatDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            string NoeSabeghe = "0";
            if (q.fldNoeSabeghe)
                NoeSabeghe = "1";
            return Json(new
            {
                fldId = q.fldId,
                fldAzTarikh = q.fldAzTarikh,
                fldNoeSabegheName = q.fldNoeSabegheName,
                fldPersonalId = q.fldPersonalId,
                fldTaTarikh = q.fldTaTarikh,
                fldNoeSabeghe = NoeSabeghe,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailsStatus(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPersonalStatusDetail(id, IP, out Err);
            if (Err.ErrorType)
            {
                return Json(new
                {
                    ErrorMsg = ErrCommon.ErrorMsg,
                    Error = 1
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Error = 0,
                fldId = q.fldId,
                fldDateTaghirVaziyat = q.fldDateTaghirVaziyat,
                fldStatusId = q.fldStatusId
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// .برای دسترسی به اطلاعات جدول پرسنل بر اساس کد فرستاده شده استفاده می شود
        /// </summary>
        /// <param name="Id">کد جدول پرسنل</param>
        /// <returns>.اطلاعات مربوط به یک سطر از جدول پرسنل را بر میگرداند</returns>
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetPersoneliInfoDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            var q2 = Common_Payservic.GetPersonalStatusWithFilter("fldId", q.fldIdStatus.ToString(), 1, IP, out ErrC_P).FirstOrDefault();
            var h = servicCommon.GetEmployeeWithFilter("fldId", q.fldEmployeeId.ToString(), 0, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), out ErrCommon).FirstOrDefault();
            if (ErrCommon.ErrorType)
            {
                return Json(new
            {
                ErrorMsg = ErrCommon.ErrorMsg,
                Error = 1
            }, JsonRequestBehavior.AllowGet);
            }
            var k = servicCommon.GetEmployee_DetailWithFilter("fldEmployeeId", q.fldEmployeeId.ToString(), 0, IP, out ErrCommon).FirstOrDefault();
            string PerStatus = "0";
            string Meliyat = "0";
            string Jensiat = "0";
            if (h.fldStatus)
                PerStatus = "1";
            if (k.fldMeliyat == true)
                Meliyat = "1";
            if (k.fldJensiyat == true)
                Jensiat = "1";

            var Image = Server.MapPath("~/Content/Blank.jpg");
                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
                var Img = Convert.ToBase64String(stream.ToArray());
                int? FileId = 0;
            if (k.fldFileId != null)
            {
                var pic = servicCommon.GetFileWithFilter("fldId", k.fldFileId.ToString(), 1, IP, out ErrCommon).FirstOrDefault();
                if (pic.fldImage != null)
                    Img = Convert.ToBase64String(pic.fldImage);
                FileId = k.fldFileId;
            }
            return Json(new
            {
                Error=0,
                fldId = q.fldId,
                fldChartOrganId = q.fldChartOrganId,
                fldCodemeli = h.fldCodemeli,
                fldFamily = h.fldFamily,
                fldName = h.fldName,
                fldPerStatus=PerStatus,
                fldTitleChartOrgan=q.fldTitleChartOrgan,
                ImageString = Img,
                fldMeliyat = Meliyat,
                fldAddress = k.fldAddress,
                fldCodePosti = k.fldCodePosti,
                fldEmployeeId = k.fldEmployeeId,
                fldFatherName = k.fldFatherName,
                fldFileId = FileId,
                fldMadrakId = k.fldMadrakId.ToString(),
                fldMahalSodoorId = k.fldMahalSodoorId.ToString(),
                fldMahalTavalodId = k.fldMahalTavalodId.ToString(),
                fldNezamVazifeId = k.fldNezamVazifeId.ToString(),
                fldReshteId = k.fldReshteId.ToString(),
                fldSh_Shenasname = k.fldSh_Shenasname,
                fldTaaholId = k.fldTaaholId.ToString(),
                fldTarikhSodoor = k.fldTarikhSodoor,
                fldTarikhTavalod = k.fldTarikhTavalod,
                fldJensiyat = Jensiat,
                fldMoble=k.fldMobile,
                fldTel=k.fldTel,
                fldEsargariId = q.fldEsargariId.ToString(),
                fldMadrakTahsiliTitle = q.fldMadrakTahsiliTitle,
                fldMeliyatName = q.fldMeliyatName,
                fldNameEmployee = q.fldNameEmployee,
                fldNameMahalTavalod = q.fldNameMahalTavalod,
                fldNameMahlSodoor = q.fldNameMahlSodoor,
                fldNamePostOran = q.NamePostOran,
                TitleOrganPostEjraee=q.TitleOrganPostEjraee,
                fldOrganPostEjraeeId=q.fldOrganPostEjraeeId,
                fldNezamVazifeTitle = q.fldNezamVazifeTitle,
                fldOrganPostId = q.fldOrganPostId,
                fldRasteShoghli = q.fldRasteShoghli,
                fldReshteShoghli = q.fldReshteShoghli,
                fldReshteTahsiliId = q.fldReshteId.ToString(),
                fldSh_MojavezEstekhdam = q.fldSh_MojavezEstekhdam,
                fldSh_Personali = q.fldSh_Personali,
                fldSharhEsargari = q.fldSharhEsargari,
                fldTabaghe = q.fldTabaghe,
                fldTarikhEstekhdam = q.fldTarikhEstekhdam,
                fldTarikhMajavezEstekhdam = q.fldTarikhMajavezEstekhdam,
                fldVaziyatEsargariTitle = q.fldVaziyatEsargariTitle,
                fldDesc = q.fldDesc,
                fldStatus=q2.fldStatusId,
                fldNoeEstekhdam=q.fldNoeEstekhdam,
                fldOrganId=q.fldOrganId
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadHistoryEstekhdam(StoreRequestParameters parameters, int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_HistoryNoeEstekhdam> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_HistoryNoeEstekhdam> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhP";
                            break;
                        case "fldNoeEstekhdamTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeEstekhdamTitleP";
                            break;

                    }
                    if (data != null)
                    {
                        data1 = servic.GetHistoryNoeEstekhdamWithFilter(field, searchtext, PersonalId, 100, IP, out Err).ToList();
                    }
                    else
                    {
                        data = servic.GetHistoryNoeEstekhdamWithFilter(field, searchtext, PersonalId, 100, IP, out Err).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetHistoryNoeEstekhdamWithFilter("fldPrsPersonalInfoId", PersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_HistoryNoeEstekhdam> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult ReadHistoryTahsilat(StoreRequestParameters parameters, int EmployeeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Common.OBJ_HistoryTahsilat> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Common.OBJ_HistoryTahsilat> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikh";
                            break;
                        case "fldTitleMadrak":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitleMadrak";
                            break;
                        case "fldTitleReshte":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitleReshte";
                            break;

                    }
                    if (data != null)
                    {
                        data1 = servicCommon.GetHistoryTahsilatWithFilter(field, EmployeeId.ToString(), searchtext, 100, IP, out ErrCommon).ToList();
                    }
                    else
                    {
                        data = servicCommon.GetHistoryTahsilatWithFilter(field, EmployeeId.ToString(), searchtext, 100, IP, out ErrCommon).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servicCommon.GetHistoryTahsilatWithFilter("fldEmployeeId", EmployeeId.ToString(),"", 100, IP, out ErrCommon).ToList();
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

            List<WCF_Common.OBJ_HistoryTahsilat> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }


        public ActionResult ReadTasviehHesab(StoreRequestParameters parameters, int PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_TasviehHesab> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_TasviehHesab> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikhP";
                            break;
                        case "fldNoeEstekhdamTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeEstekhdamTitleP";
                            break;

                    }
                    if (data != null)
                    {
                        data1 = servic.GetTasviehHesabWithFilter(field, searchtext, PersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    }
                    else
                    {
                        data = servic.GetTasviehHesabWithFilter(field, searchtext, PersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetTasviehHesabWithFilter("fldPrsPersonalInfoId","", PersonalId.ToString(), Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_TasviehHesab> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        /// <summary>
        ///فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول پرسنل و امکان سرچ کردن بر اساس     
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_PersonalInfo> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_PersonalInfo> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldSh_Personali":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_Personali";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNameEmployee";
                            break;
                        case "fldFamily":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFamilyEmployee";
                            break;
                        case "fldCodemeli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldCodemeli";
                            break;
                        case "fldSh_Shenasname":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_Shenasname";
                            break;
                        case "fldReshteShoghli":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldReshteShoghli";
                            break;
                        case "fldTitleStatus":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitleStatus";
                            break;
                        case "fldSh_MojavezEstekhdam":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldSh_MojavezEstekhdam";
                            break;
                    }
                    if (data != null)
                        data1 = servic.GetPersoneliInfoWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                    else
                        data = servic.GetPersoneliInfoWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetPersoneliInfoWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_PersonalInfo> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult ReadStatus(StoreRequestParameters parameters, int PersonalId,int State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_PersonalStatus> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_PersonalStatus> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                    }
                    if (data != null)
                    {
                        if (State == 2)
                            data1 = servic.GetPersonalStatusWithFilter(field, searchtext, 100, IP, out Err).Where(o => o.fldPayPersonalInfoId == PersonalId).ToList();
                        else
                            data1 = servic.GetPersonalStatusWithFilter(field, searchtext, 100, IP, out Err).Where(o => o.fldPrsPersonalInfoId == PersonalId).ToList();
                    }
                    else
                    {
                        if (State == 2)
                            data = servic.GetPersonalStatusWithFilter(field, searchtext, 100, IP, out Err).Where(o => o.fldPayPersonalInfoId == PersonalId).ToList();
                        else
                            data = servic.GetPersonalStatusWithFilter(field, searchtext, 100, IP, out Err).Where(o => o.fldPrsPersonalInfoId == PersonalId).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                if (State == 2)
                    data = servic.GetPersonalStatusWithFilter("fldPayPersonalInfoId", PersonalId.ToString(), 100, IP, out Err).ToList();
                else
                    data = servic.GetPersonalStatusWithFilter("fldPrsPersonalInfoId", PersonalId.ToString(), 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_PersonalStatus> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        /// <summary>
        /// ///فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول سوابق گروه تشویقی و امکان سرچ کردن بر اساس     
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ActionResult ReadSaVabeghTashvighi(StoreRequestParameters parameters, string PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_SavabeghGroupTashvighi> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_SavabeghGroupTashvighi> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldTedadGroup":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTedadGroup";
                            break;
                        case "fldTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTarikh";
                            break;
                        case "fldAnvaGroupTashvighiTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAnvaGroupTashvighiTitle";
                            break;
                       
                    }
                    if (data != null)
                        data1 = servic.GetSavabeghGroupTashvighiWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(l => l.fldPersonalId == Convert.ToInt32(PersonalId)).ToList();
                    else
                        data = servic.GetSavabeghGroupTashvighiWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(l => l.fldPersonalId == Convert.ToInt32(PersonalId)).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetSavabeghGroupTashvighiWithFilter("fldPersonalId", PersonalId,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_SavabeghGroupTashvighi> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }

        public ActionResult GetReshteh()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servicCommon.GetReshteTahsiliWithFilter("", "", 0, IP, out ErrCommon).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);

        }
        /// <summary>
        /// ///فرم Grid خاص در fieldname نمایش لیست اطلاعات ذخیره شده جدول سوابق سنوات خدمت و امکان سرچ کردن بر اساس     
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ActionResult ReadSanvatKhedmat(StoreRequestParameters parameters, string PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_SavabegheSanavateKHedmat> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_SavabegheSanavateKHedmat> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldAzTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAzTarikh";
                            break;
                        case "fldTaTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTaTarikh";
                            break;
                        case "fldNoeSabegheName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldNoeSabegheName";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetSavabegheSanavateKHedmatWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(l => l.fldPersonalId == Convert.ToInt32(PersonalId)).ToList();
                    else
                        data = servic.GetSavabegheSanavateKHedmatWithFilter(field, searchtext,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).Where(l => l.fldPersonalId == Convert.ToInt32(PersonalId)).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetSavabegheSanavateKHedmatWithFilter("fldPersonalId", PersonalId,Convert.ToInt32(Session["OrganId"]), 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_SavabegheSanavateKHedmat> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        /// <summary>
        /// صفحه اصلی فرم خروجی اکسل
        /// </summary>
        /// <param name="containerId">شناسه تب خروجی اکسل در فرم پرسنل</param>
        /// <returns></returns>
        public ActionResult Excel(string containerId)
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
        public ActionResult ExcelWin()
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        /// <summary>
        ///. اطلاعات جدول پرسنل را در فایل اکسل نمایش میدهد
        /// </summary>
        /// <param name="Checked">آرایه ای از لیست اطلاعات مورد نظر پرسنل</param>
        /// <returns>ساختن فایل اکسل</returns>
        private string where(string Status, string Gender, string Esargari, string Madrak, string NoeEstekhdam)
        {
            try
            {
                string s = "";
                if (Status != "")
                {
                    s = s + " and Com.fn_MaxPersonalStatus(Prs.Prs_tblPersonalInfo.fldId,'kargozini')=" + Status;
                }
                if (Gender != "")
                {
                    s = s + " and Com.tblEmployee_Detail.fldJensiyat=" + Gender ;
                }
                if (Esargari != "")
                {
                    s = s + " and Prs.Prs_tblPersonalInfo.fldEsargariId=" + Esargari;
                }
                if (Madrak != "")
                {
                    Madrak=Madrak.TrimEnd(',');
                    s = s + " and Com.tblEmployee_Detail.fldMadrakId in (" + Madrak+" )";
                }
                if (NoeEstekhdam != "")
                {
                    NoeEstekhdam = NoeEstekhdam.TrimEnd(',');
                    s = s + " and Com.fn_MaxPersonalTypeEstekhdam(Prs.Prs_tblPersonalInfo.fldId) in (" + NoeEstekhdam + " )";
                }
                return s;
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    return x.InnerException.Message;
                return "";
            }
        }
        public FileResult CreateExcel(string Checked, string Status, string Gender, string Esargari, string Madrak, string NoeEstekhdam)
        {
            string[] alpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC" };
            int index = 0;
            var StatusCheck = Checked.Split(';');
            var Check = "";
            var Name = ""; var FamilyName = ""; var FatherName = ""; var GenderChar = ""; var ShomareShenasname = ""; var Codemeli = "";
            var TarikhTavalod = ""; var CityName = ""; var Address = ""; var CodePosti = ""; var StatusEsargari = "";
            var ShomarePersoneli = ""; var ReshteTahsili = ""; var TitleStatusJesmi = ""; var StatusNezamVazife = ""; var Post = "";
            var RasteShoghli = ""; var ReshteShoghli = ""; var TarikhEstekhdam = ""; var ServiceLocation = ""; var Tabaghe = ""; var MeliyatChar = ""; var ShMojavezEstekhdam = ""; var TMojavezEstekhdam = "";

            var TarikhSodoor = ""; var CityTavalod = ""; var MadrakTitle = ""; var SharhEsargari = ""; var NoeEstekhdamTitle = ""; var StatusTitle = ""; var SanavatKh = "";

            string commandText = "SELECT        Prs.Prs_tblPersonalInfo.fldId, Prs.Prs_tblPersonalInfo.fldEmployeeId, Com.tblEmployee_Detail.fldSh_Shenasname, Com.tblEmployee_Detail.fldTarikhTavalod, Com.tblEmployee_Detail.fldMahalTavalodId, " +
                  " Com.tblEmployee_Detail.fldMahalSodoorId, ISNULL(Com.tblEmployee_Detail.fldAddress, '') AS fldAddress, ISNULL(Com.tblEmployee_Detail.fldCodePosti, '') AS fldCodePosti, " +
                      " Prs.Prs_tblPersonalInfo.fldEsargariId, ISNULL(Prs.Prs_tblPersonalInfo.fldSharhEsargari, '') AS fldSharhEsargari, Prs.Prs_tblPersonalInfo.fldSh_Personali, Com.tblEmployee_Detail.fldMadrakId, Com.tblEmployee_Detail.fldReshteId, " +
                         " Com.tblEmployee_Detail.fldNezamVazifeId, Prs.Prs_tblPersonalInfo.fldOrganPostId, ISNULL(Prs.Prs_tblPersonalInfo.fldRasteShoghli, '') AS fldRasteShoghli, ISNULL(Prs.Prs_tblPersonalInfo.fldReshteShoghli, '') AS fldReshteShoghli, " +
                         " Prs.Prs_tblPersonalInfo.fldTarikhEstekhdam, Com.tblOrganizationalPosts.fldChartOrganId, Prs.Prs_tblPersonalInfo.fldTabaghe, Com.tblEmployee_Detail.fldMeliyat, " +
                         " ISNULL(Prs.Prs_tblPersonalInfo.fldSh_MojavezEstekhdam, '') AS fldSh_MojavezEstekhdam, ISNULL(Prs.Prs_tblPersonalInfo.fldTarikhMajavezEstekhdam, '') AS fldTarikhMajavezEstekhdam, Prs.Prs_tblPersonalInfo.fldUserId, Prs.Prs_tblPersonalInfo.fldDesc, Prs.Prs_tblPersonalInfo.fldDate, " +
                         " CASE WHEN fldMeliyat = 0 THEN N'غیر ایرانی' ELSE N'ایرانی' END AS fldMeliyatName, Com.tblMadrakTahsili.fldTitle AS fldMadrakTahsiliTitle, Prs.tblVaziyatEsargari.fldTitle AS fldVaziyatEsargariTitle, " +
                         " ISNULL(Com.tblNezamVazife.fldTitle, '') AS fldNezamVazifeTitle, Com.tblEmployee.fldName + ' ' + Com.tblEmployee.fldFamily AS fldNameEmployee, Com.tblEmployee.fldName, Com.tblEmployee.fldFamily, " +
                         " Com.tblEmployee_Detail.fldFatherName, Com.tblEmployee_Detail.fldJensiyat, Com.tblChartOrgan.fldTitle AS fldTitleChartOrgan, Com.fn_stringDecode(Com.tblOrganization.fldName) AS fldNameOrgan, " +
                         " Com.tblCity.fldName AS fldNameMahalTavalod, tblCity_1.fldName AS fldNameMahlSodoor, Com.tblEmployee.fldCodemeli, Com.tblEmployee_Detail.fldFileId, " +
                         " CASE WHEN fldJensiyat = 0 THEN N'زن' ELSE N'مرد' END AS fldNameJensiyat, Com.tblOrganizationalPosts.fldTitle AS NamePostOran, Com.tblReshteTahsili.fldTitle AS fldReshteTahsiliTitle, " +
                         " Com.tblEmployee.fldFamily + '_' + Com.tblEmployee.fldName + '(' + Com.tblEmployee_Detail.fldFatherName + ')' AS fldName_FamilyEmployee, ISNULL(Com.tblChartOrgan.fldOrganId, 0) AS fldOrganId, " +
                         " ISNULL" +
                         "    ((SELECT        TOP (1) fldId" +
                         "        FROM            Com.tblPersonalStatus" +
                          "       WHERE        (fldPrsPersonalInfoId = Prs.Prs_tblPersonalInfo.fldId)" +
                         "        ORDER BY fldId DESC), 0) AS fldIdStatus, ISNULL" +
                           "  ((SELECT        TOP (1) fldNoeEstekhdamId" +
                           "      FROM            Prs.tblHistoryNoeEstekhdam" +
                            "     WHERE        (fldPrsPersonalInfoId = Prs.Prs_tblPersonalInfo.fldId)" +
                            "     ORDER BY fldId DESC), 0) AS fldNoeEstekhdam," +
                            " (SELECT        TOP (1) Com.tblStatus.fldTitle" +
                            "   FROM            Com.tblPersonalStatus AS tblPersonalStatus_1 INNER JOIN" +
                             "                            Com.tblStatus ON tblPersonalStatus_1.fldStatusId = Com.tblStatus.fldId" +
                             "  WHERE        (tblPersonalStatus_1.fldPrsPersonalInfoId = Prs.Prs_tblPersonalInfo.fldId)" +
                             "  ORDER BY tblPersonalStatus_1.fldId DESC) AS fldTitleStatus, Com.tblEmployee_Detail.fldTarikhSodoor, Com.tblEmployee_Detail.fldTaaholId, Com.tblEmployee.fldStatus, " +
                        " ISNULL((SELECT     TOP (1)  Com.tblAnvaEstekhdam.fldTitle "+
                        " FROM         Prs.tblHistoryNoeEstekhdam INNER JOIN "+
                      "    Com.tblAnvaEstekhdam ON Prs.tblHistoryNoeEstekhdam.fldNoeEstekhdamId = Com.tblAnvaEstekhdam.fldId "+
                            "  WHERE     (fldPrsPersonalInfoId = Prs.Prs_tblPersonalInfo.fldId) "+
                            "  ORDER BY tblHistoryNoeEstekhdam.fldId DESC), '')AS TitleNoeEstekhdam " +
            " FROM            Prs.Prs_tblPersonalInfo INNER JOIN" +
                        " Com.tblEmployee ON Prs.Prs_tblPersonalInfo.fldEmployeeId = Com.tblEmployee.fldId INNER JOIN" +
                        " Com.tblEmployee_Detail ON Com.tblEmployee.fldId = Com.tblEmployee_Detail.fldEmployeeId INNER JOIN" +
                        " Com.tblStatusTaahol ON Com.tblEmployee_Detail.fldTaaholId = Com.tblStatusTaahol.fldId INNER JOIN" +
                        " Com.tblMadrakTahsili ON Com.tblEmployee_Detail.fldMadrakId = Com.tblMadrakTahsili.fldId INNER JOIN" +
                        " Com.tblReshteTahsili ON Com.tblEmployee_Detail.fldReshteId = Com.tblReshteTahsili.fldId INNER JOIN" +
                        " Com.tblCity ON Com.tblEmployee_Detail.fldMahalTavalodId = Com.tblCity.fldId INNER JOIN" +
                        " Com.tblCity AS tblCity_1 ON Com.tblEmployee_Detail.fldMahalSodoorId = tblCity_1.fldId INNER JOIN" +
                        " Prs.tblVaziyatEsargari ON Prs.Prs_tblPersonalInfo.fldEsargariId = Prs.tblVaziyatEsargari.fldId INNER JOIN" +
                        " Com.tblOrganizationalPosts ON Prs.Prs_tblPersonalInfo.fldOrganPostId = Com.tblOrganizationalPosts.fldId INNER JOIN" +
                        " Com.tblChartOrgan ON Com.tblOrganizationalPosts.fldChartOrganId = Com.tblChartOrgan.fldId INNER JOIN" +
                        " Com.tblOrganization ON Com.tblChartOrgan.fldOrganId = Com.tblOrganization.fldId LEFT OUTER JOIN" +
                        " Com.tblNezamVazife ON Com.tblEmployee_Detail.fldNezamVazifeId = Com.tblNezamVazife.fldId" +
                      " WHERE tblChartOrgan.fldOrganId = " + (Session["OrganId"]).ToString() + where(Status, Gender, Esargari, Madrak, NoeEstekhdam);

            NewFMS.FormulaService.WebServiceFormula service = new NewFMS.FormulaService.WebServiceFormula();
            var per = service.GetData(commandText).Tables[0];

            List<WCF_Personeli.OBJ_PersonalInfo> p = new List<WCF_Personeli.OBJ_PersonalInfo>();
            for (int i = 0; i < per.Rows.Count; i++)
            {
                WCF_Personeli.OBJ_PersonalInfo l = new WCF_Personeli.OBJ_PersonalInfo();
                l.fldId = (int)(per.Rows[i]["fldId"]);
                l.fldName = (string)(per.Rows[i]["fldName"]);
                l.fldFamily = (string)(per.Rows[i]["fldFamily"]);
                l.fldFatherName = (string)(per.Rows[i]["fldFatherName"]);
                l.fldCodemeli = (string)(per.Rows[i]["fldCodemeli"]);
                l.fldNameJensiyat = (string)(per.Rows[i]["fldNameJensiyat"]);
                l.fldSh_Shenasname = (string)(per.Rows[i]["fldSh_Shenasname"]);
                l.fldTarikhTavalod = (string)(per.Rows[i]["fldTarikhTavalod"]);
                l.fldNameMahlSodoor = (string)(per.Rows[i]["fldNameMahlSodoor"]);
                l.fldAddress = (string)(per.Rows[i]["fldAddress"]);
                l.fldCodePosti = (string)(per.Rows[i]["fldCodePosti"]);
                l.fldVaziyatEsargariTitle = (string)(per.Rows[i]["fldVaziyatEsargariTitle"]);
                l.fldSh_Personali = (string)(per.Rows[i]["fldSh_Personali"]);
                l.fldReshteTahsiliTitle = (string)(per.Rows[i]["fldReshteTahsiliTitle"]);
                l.fldNezamVazifeTitle = (string)(per.Rows[i]["fldNezamVazifeTitle"]);
                l.fldNameOrgan = (string)(per.Rows[i]["fldNameOrgan"]);
                l.fldRasteShoghli = (string)(per.Rows[i]["fldRasteShoghli"]);
                l.fldReshteShoghli = (string)(per.Rows[i]["fldReshteShoghli"]);
                l.fldTarikhEstekhdam = (string)(per.Rows[i]["fldTarikhEstekhdam"]);
                l.fldTabaghe = (string)(per.Rows[i]["fldTabaghe"]);
                l.fldMeliyatName = (string)(per.Rows[i]["fldMeliyatName"]);
                l.fldSh_MojavezEstekhdam = (string)(per.Rows[i]["fldSh_MojavezEstekhdam"]);
                l.fldTarikhMajavezEstekhdam = (string)(per.Rows[i]["fldTarikhMajavezEstekhdam"]);
                l.NamePostOran = (string)(per.Rows[i]["NamePostOran"]);
                l.fldTarikhSodoor = (string)(per.Rows[i]["fldTarikhSodoor"]);
                l.fldNameMahalTavalod = (string)(per.Rows[i]["fldNameMahalTavalod"]);
                l.fldMadrakTahsiliTitle = (string)(per.Rows[i]["fldMadrakTahsiliTitle"]);
                l.fldSharhEsargari = (string)(per.Rows[i]["fldSharhEsargari"]);
                l.TitleNoeEstekhdam = (string)(per.Rows[i]["TitleNoeEstekhdam"]);
                l.fldTitleStatus = (string)(per.Rows[i]["fldTitleStatus"]);
                var prs = servic.GetPersoneliInfoWithFilter("fldCodemeli", l.fldCodemeli, Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).FirstOrDefault();
                var Sanavat = servic.GetSavabegheSanavateKHedmatWithFilter("fldPersonalId", prs.fldId.ToString(), Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList();
                int CountTarikh = 0;
                foreach (var item in Sanavat)
                {
                    CountTarikh += MyLib.Shamsi.DiffOfShamsiDate(item.fldAzTarikh, item.fldTaTarikh);
                }

                CountTarikh += MyLib.Shamsi.DiffOfShamsiDate(prs.fldTarikhEstekhdam, MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now));

                var DayMahSal = Common_Payservic.GetDiffDayMahSalWithFilter(CountTarikh, IP, out ErrC_P).FirstOrDefault();
                var SanavatRasmi = DayMahSal.CountString;
                l.fldDesc = SanavatRasmi;
                p.Add(l);
            }
            var Personal = p.ToList();
            /////////////////////////////////////
            Workbook wb = new Workbook();
            Worksheet sheet = wb.Worksheets[0];

            foreach (var item in StatusCheck)
            {
               // var Personal = servic.GetPersoneliInfoWithFilter("", "",Convert.ToInt32(Session["OrganId"]), 0, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).ToList();
                switch (item)
                {
                    case "Name":
                        Check = "نام";
                        Cell cell = sheet.Cells[alpha[index] + "1"];
                        cell.PutValue(Check);
                        int i = 0;
                        foreach (var _item in Personal)
                        {
                            Name = _item.fldName;
                            Cell Cell = sheet.Cells[alpha[index] + (i + 2)];
                            Cell.PutValue(Name);
                            i++;
                        }
                        index++;
                        break;
                    case "Family":
                        Check = "نام خانوادگی";
                        Cell cell1 = sheet.Cells[alpha[index] + "1"];
                        cell1.PutValue(Check);
                        int j = 0;
                        foreach (var _item in Personal)
                        {
                            FamilyName = _item.fldFamily;
                            Cell Cell = sheet.Cells[alpha[index] + (j + 2)];
                            Cell.PutValue(FamilyName);
                            j++;
                        }
                        index++;
                        break;
                    case "Father":
                        Check = "نام پدر";
                        Cell cell2 = sheet.Cells[alpha[index] + "1"];
                        cell2.PutValue(Check);
                        int k = 0;
                        foreach (var _item in Personal)
                        {
                            FatherName = _item.fldFatherName;
                            Cell Cell = sheet.Cells[alpha[index] + (k + 2)];
                            Cell.PutValue(FatherName);
                            k++;
                        }
                        index++;
                        break;
                    case "MeliCode":
                        Check = "کدملی";
                        Cell cell23 = sheet.Cells[alpha[index] + "1"];
                        cell23.PutValue(Check);
                        int k2 = 0;
                        foreach (var _item in Personal)
                        {
                            Codemeli = _item.fldCodemeli;
                            Cell Cell = sheet.Cells[alpha[index] + (k2 + 2)];
                            Cell.PutValue(Codemeli);
                            k2++;
                        }
                        index++;
                        break;
                    case "Jensiyat":
                        Check = "جنسیت";
                        Cell cell3 = sheet.Cells[alpha[index] + "1"];
                        cell3.PutValue(Check);
                        int q = 0;
                        foreach (var _item in Personal)
                        {
                            GenderChar = _item.fldNameJensiyat;
                            Cell Cell = sheet.Cells[alpha[index] + (q + 2)];
                            Cell.PutValue(GenderChar);
                            q++;
                        }
                        index++;
                        break;
                    case "ShomareShenasname":
                        Check = "شماره شناسنامه";
                        Cell cell4 = sheet.Cells[alpha[index] + "1"];
                        cell4.PutValue(Check);
                        int w = 0;
                        foreach (var _item in Personal)
                        {
                            ShomareShenasname = _item.fldSh_Shenasname;
                            Cell Cell = sheet.Cells[alpha[index] + (w + 2)];
                            Cell.PutValue(ShomareShenasname);
                            w++;
                        }
                        index++;
                        break;
                    case "TarikhTavalod":
                        Check = "تاریخ تولد";
                        Cell cell5 = sheet.Cells[alpha[index] + "1"];
                        cell5.PutValue(Check);
                        int z = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhTavalod = _item.fldTarikhTavalod;
                            Cell Cell = sheet.Cells[alpha[index] + (z + 2)];
                            Cell.PutValue(TarikhTavalod);
                            z++;
                        }
                        index++;
                        break;
                    case "City":
                        Check = "شهر محل صدور";
                        Cell cell6 = sheet.Cells[alpha[index] + "1"];
                        cell6.PutValue(Check);
                        int x = 0;
                        foreach (var _item in Personal)
                        {
                            CityName = _item.fldNameMahlSodoor;
                            Cell Cell = sheet.Cells[alpha[index] + (x + 2)];
                            Cell.PutValue(CityName);
                            x++;
                        }
                        index++;
                        break;
                    case "Addres":
                        Check = "آدرس";
                        Cell cell7 = sheet.Cells[alpha[index] + "1"];
                        cell7.PutValue(Check);
                        int y = 0;
                        foreach (var _item in Personal)
                        {
                            Address = _item.fldAddress;
                            Cell Cell = sheet.Cells[alpha[index] + (y + 2)];
                            Cell.PutValue(Address);
                            y++;
                        }
                        index++;
                        break;
                    case "CodePosti":
                        Check = "کد پستی";
                        Cell cell8 = sheet.Cells[alpha[index] + "1"];
                        cell8.PutValue(Check);
                        int a = 0;
                        foreach (var _item in Personal)
                        {
                            CodePosti = _item.fldCodePosti;
                            Cell Cell = sheet.Cells[alpha[index] + (a + 2)];
                            Cell.PutValue(CodePosti);
                            a++;
                        }
                        index++;
                        break;
                    case "StatusEsargari":
                        Check = "وضعیت ایثارگری";
                        Cell cell9 = sheet.Cells[alpha[index] + "1"];
                        cell9.PutValue(Check);
                        int b = 0;
                        foreach (var _item in Personal)
                        {
                            StatusEsargari = _item.fldVaziyatEsargariTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (b + 2)];
                            Cell.PutValue(StatusEsargari);
                            b++;
                        }
                        index++;
                        break;
                   
                    case "ShomarePersoneli":
                        Check = "شماره پرسنلی";
                        Cell cell11 = sheet.Cells[alpha[index] + "1"];
                        cell11.PutValue(Check);
                        int r = 0;
                        foreach (var _item in Personal)
                        {
                            ShomarePersoneli = _item.fldSh_Personali;
                            Cell Cell = sheet.Cells[alpha[index] + (r + 2)];
                            Cell.PutValue(ShomarePersoneli);
                            r++;
                        }
                        index++;
                        break;
                    case "ReshteTahsili":
                        Check = "رشته تحصیلی";
                        Cell cell12 = sheet.Cells[alpha[index] + "1"];
                        cell12.PutValue(Check);
                        int u = 0;
                        foreach (var _item in Personal)
                        {
                            ReshteTahsili = _item.fldReshteTahsiliTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (u + 2)];
                            Cell.PutValue(ReshteTahsili);
                            u++;
                        }
                        index++;
                        break;
                    case "StatusNezamVazife":
                        Check = "وضعیت نظام وظیفه";
                        Cell cell13 = sheet.Cells[alpha[index] + "1"];
                        cell13.PutValue(Check);
                        int d = 0;
                        foreach (var _item in Personal)
                        {
                            StatusNezamVazife = _item.fldNezamVazifeTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (d + 2)];
                            Cell.PutValue(StatusNezamVazife);
                            d++;
                        }
                        index++;
                        break;
                    case "OrganizationalPosts":
                        Check = "پست سازمانی";
                        Cell cell14 = sheet.Cells[alpha[index] + "1"];
                        cell14.PutValue(Check);
                        int f = 0;
                        foreach (var _item in Personal)
                        {
                            Post = _item.NamePostOran;
                            Cell Cell = sheet.Cells[alpha[index] + (f + 2)];
                            Cell.PutValue(Post);
                            f++;
                        }
                        index++;
                        break;
                    case "RasteShoghli":
                        Check = "رسته شغلی";
                        Cell cell15 = sheet.Cells[alpha[index] + "1"];
                        cell15.PutValue(Check);
                        int g = 0;
                        foreach (var _item in Personal)
                        {
                            RasteShoghli = _item.fldRasteShoghli;
                            Cell Cell = sheet.Cells[alpha[index] + (g + 2)];
                            Cell.PutValue(RasteShoghli);
                            g++;
                        }
                        index++;
                        break;
                    case "ReshteShoghli":
                        Check = "رشته شغلی";
                        Cell cell16 = sheet.Cells[alpha[index] + "1"];
                        cell16.PutValue(Check);
                        int h = 0;
                        foreach (var _item in Personal)
                        {
                            ReshteShoghli = _item.fldReshteShoghli;
                            Cell Cell = sheet.Cells[alpha[index] + (h + 2)];
                            Cell.PutValue(ReshteShoghli);
                            h++;
                        }
                        index++;
                        break;
                    case "TarikhEstekhdam":
                        Check = "تاریخ استخدام";
                        Cell cell17 = sheet.Cells[alpha[index] + "1"];
                        cell17.PutValue(Check);
                        int l = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhEstekhdam = _item.fldTarikhEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (l + 2)];
                            Cell.PutValue(TarikhEstekhdam);
                            l++;
                        }
                        index++;
                        break;
                    
                    case "Tabaghe":
                        Check = "طبقه";
                        Cell cell19 = sheet.Cells[alpha[index] + "1"];
                        cell19.PutValue(Check);
                        int v = 0;
                        foreach (var _item in Personal)
                        {
                            Tabaghe = _item.fldTabaghe;
                            Cell Cell = sheet.Cells[alpha[index] + (v + 2)];
                            Cell.PutValue(Tabaghe);
                            v++;
                        }
                        index++;
                        break;
                    case "Meliyat":
                        Check = "ملیت";
                        Cell cell20 = sheet.Cells[alpha[index] + "1"];
                        cell20.PutValue(Check);
                        int n = 0;
                        foreach (var _item in Personal)
                        {
                            MeliyatChar = _item.fldMeliyatName;
                            Cell Cell = sheet.Cells[alpha[index] + (n + 2)];
                            Cell.PutValue(MeliyatChar);
                            n++;
                        }
                        index++;
                        break;
                    case "ShMojavezEstekhdam":
                        Check = "شماره مجوز استخدام";
                        Cell cell21 = sheet.Cells[alpha[index] + "1"];
                        cell21.PutValue(Check);
                        int t = 0;
                        foreach (var _item in Personal)
                        {
                            ShMojavezEstekhdam = _item.fldSh_MojavezEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (t + 2)];
                            Cell.PutValue(ShMojavezEstekhdam);
                            t++;
                        }
                        index++;
                        break;
                    case "TMojavezEstekhdam":
                        Check = "تاریخ مجوز استخدام";
                        Cell cell22 = sheet.Cells[alpha[index] + "1"];
                        cell22.PutValue(Check);
                        int m = 0;
                        foreach (var _item in Personal)
                        {
                            TMojavezEstekhdam = _item.fldTarikhMajavezEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (m + 2)];
                            Cell.PutValue(TMojavezEstekhdam);
                            m++;
                        }
                        index++;
                        break;
                    case "TarikhSodoor":
                        Check = "تاریخ صدور";
                        Cell cell24 = sheet.Cells[alpha[index] + "1"];
                        cell24.PutValue(Check);
                        int m1 = 0;
                        foreach (var _item in Personal)
                        {
                            TarikhSodoor = _item.fldTarikhSodoor;
                            Cell Cell = sheet.Cells[alpha[index] + (m1 + 2)];
                            Cell.PutValue(TarikhSodoor);
                            m1++;
                        }
                        index++;
                        break;
                    case "CityTavalod":
                        Check = "شهر محل تولد";
                        Cell cell25 = sheet.Cells[alpha[index] + "1"];
                        cell25.PutValue(Check);
                        int m2 = 0;
                        foreach (var _item in Personal)
                        {
                            CityTavalod = _item.fldNameMahalTavalod;
                            Cell Cell = sheet.Cells[alpha[index] + (m2 + 2)];
                            Cell.PutValue(CityTavalod);
                            m2++;
                        }
                        index++;
                        break;
                    case "MadrakTitle":
                        Check = "مقطع تحصیلی";
                        Cell cell26 = sheet.Cells[alpha[index] + "1"];
                        cell26.PutValue(Check);
                        int m3 = 0;
                        foreach (var _item in Personal)
                        {
                            MadrakTitle = _item.fldMadrakTahsiliTitle;
                            Cell Cell = sheet.Cells[alpha[index] + (m3 + 2)];
                            Cell.PutValue(MadrakTitle);
                            m3++;
                        }
                        index++;
                        break;
                    case "SharhEsargari":
                        Check = "شرح ایثارگری";
                        Cell cell27 = sheet.Cells[alpha[index] + "1"];
                        cell27.PutValue(Check);
                        int m4 = 0;
                        foreach (var _item in Personal)
                        {
                            SharhEsargari = _item.fldSharhEsargari;
                            Cell Cell = sheet.Cells[alpha[index] + (m4 + 2)];
                            Cell.PutValue(SharhEsargari);
                            m4++;
                        }
                        index++;
                        break;
                    case "NoeEstekhdamTitle":
                        Check = "نوع استخدام";
                        Cell cell28 = sheet.Cells[alpha[index] + "1"];
                        cell28.PutValue(Check);
                        int m5 = 0;
                        foreach (var _item in Personal)
                        {
                            NoeEstekhdamTitle = _item.TitleNoeEstekhdam;
                            Cell Cell = sheet.Cells[alpha[index] + (m5 + 2)];
                            Cell.PutValue(NoeEstekhdamTitle);
                            m5++;
                        }
                        index++;
                        break;
                    case "StatusTitle":
                        Check = "وضعیت";
                        Cell cell29 = sheet.Cells[alpha[index] + "1"];
                        cell29.PutValue(Check);
                        int m6 = 0;
                        foreach (var _item in Personal)
                        {
                            StatusTitle = _item.fldTitleStatus;
                            Cell Cell = sheet.Cells[alpha[index] + (m6 + 2)];
                            Cell.PutValue(StatusTitle);
                            m6++;
                        }
                        index++;
                        break;
                    case "Sanavat":
                        Check = "سنوات خدمت";
                        Cell cell30 = sheet.Cells[alpha[index] + "1"];
                        cell30.PutValue(Check);
                        int m7 = 0;
                        foreach (var _item in Personal)
                        {
                            SanavatKh = _item.fldDesc;
                            Cell Cell = sheet.Cells[alpha[index] + (m7 + 2)];
                            Cell.PutValue(SanavatKh);
                            m7++;
                        }
                        index++;
                        break;

                    /*          */
                }
            }
            MemoryStream stream = new MemoryStream();
            wb.Save(stream, SaveFormat.Excel97To2003);
            return File(stream.ToArray(), "xls", "Personal.xls");
        }
        public ActionResult SabagheJebheWin(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.PrsPersonalInfoId = id;
            return PartialView;
        }
        public ActionResult GetSabegheJebhe()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSavabeghJebhe_ItemsWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldTitle });
            return this.Store(q);
        }
        public ActionResult ReadSabagheJebhe(StoreRequestParameters parameters, string PersonalId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Personeli.OBJ_SavabeghJebhe_Personal> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Personeli.OBJ_SavabeghJebhe_Personal> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldAzTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldAzTarikh";
                            break;
                        case "fldTaTarikh":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTaTarikh";
                            break;
                        case "fldTitle":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldTitle";
                            break;
                        case "fldDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDesc";
                            break;

                    }
                    if (data != null)
                        data1 = servic.GetSavabeghJebhe_PersonalWithFilter(field, searchtext, PersonalId, 100, IP, out Err).ToList();
                    else
                        data = servic.GetSavabeghJebhe_PersonalWithFilter(field, searchtext, PersonalId, 100, IP, out Err).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetSavabeghJebhe_PersonalWithFilter("fldPrsPersonalId", PersonalId, "", 100, IP, out Err).ToList();
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

            List<WCF_Personeli.OBJ_SavabeghJebhe_Personal> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
        public ActionResult SabagheJebheDetail(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetSavabeghJebhe_PersonalDetail(Id, IP, out Err);
     
            return Json(new
            {
                fldId = q.fldId,
                fldAzTarikh = q.fldAzTarikh,
                fldItemId = q.fldItemId.ToString(),
                fldPersonalId = q.fldPrsPersonalId,
                fldTaTarikh = q.fldTaTarikh,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveSabagheJebhe(WCF_Personeli.OBJ_SavabeghJebhe_Personal SavabeghJebhe)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (SavabeghJebhe.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertSavabeghJebhe_Personal(SavabeghJebhe.fldItemId, SavabeghJebhe.fldPrsPersonalId, SavabeghJebhe.fldAzTarikh, SavabeghJebhe.fldTaTarikh, SavabeghJebhe.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateSavabeghJebhe_Personal(SavabeghJebhe.fldId, SavabeghJebhe.fldItemId, SavabeghJebhe.fldPrsPersonalId, SavabeghJebhe.fldAzTarikh, SavabeghJebhe.fldTaTarikh, SavabeghJebhe.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Err = Er
            }, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult DeleteSabagheJebhe(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteSavabeghJebhe_Personal(Convert.ToByte(id), Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
        public ActionResult EmzaPersonalInfo(int id)
        {//باز شدن پنجره
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.fldCommitionId = id;
            var SignFileId = 0;
            var q = servic.GetPersonalSignWithFilter("fldCommitionId", id.ToString(), 0, IP, out Err).FirstOrDefault();
            if(q!=null)
                SignFileId=q.fldFileId;
            PartialView.ViewBag.SignFileId = SignFileId;
            return PartialView;
        }
        public ActionResult UploadEmza()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = ""; string MsgTitle = ""; int Er = 0;
            try
            {
                if (Session["savePathEmza"] != null)
                {
                    System.IO.File.Delete(Session["savePathEmza"].ToString());
                    Session.Remove("FileName");
                    Session.Remove("savePathEmza");
                }

                var IsImage = FileInfoExtensions.IsImage(Request.Files[0]);
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (IsImage == true)
                {
                    if (Request.Files[0].ContentLength <= 10485760)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        var Name = Guid.NewGuid();
                        string savePath = Server.MapPath(@"~\Uploaded\" + Name + extension);
                        file.SaveAs(savePath);
                        Session["savePathEmza"] = savePath;
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
        public ActionResult DeleteSessionFile()
        {
            string Msg = "";

            if (Session["savePathEmza"] != null)
            {
                string physicalPath = System.IO.Path.Combine(Session["savePathEmza"].ToString());
                Session.Remove("savePathEmza");
                Session.Remove("FileName");
                System.IO.File.Delete(physicalPath);
                Msg = "حذف فایل با موفقیت انجام شد";
            }
            return Json(new
            {
                Msg = Msg
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult SaveEmza(int fldCommitionId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0; byte[] Image = null; string Pasvand = "";
            try
            {

            
                    //ذخیره
                if (Session["savePathEmza"] != null)
                    {
                        MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePathEmza"].ToString()));
                        Image = stream.ToArray();
                        Pasvand = Path.GetExtension(Session["savePathEmza"].ToString());
                    }
                else
                {
                    return Json(new
                    {
                        Msg = "لطفا فایل امضا را بارگذاری نمایید.",
                        MsgTitle = "خطا",
                        Err = 1
                    }, JsonRequestBehavior.AllowGet);
                }

                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertPersonalSign(fldCommitionId, Image, Pasvand, "", Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
               
              
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
                if (Session["savePathEmza"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathEmza"].ToString());
                    Session.Remove("savePathEmza");
                    System.IO.File.Delete(physicalPath);
                }
            }
            catch (Exception x)
            {
                if (x.InnerException != null)
                    Msg = x.InnerException.Message;
                else
                    Msg = x.Message;

                if (Session["savePathEmza"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePathEmza"].ToString());
                    Session.Remove("savePathEmza");
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
    }
}
