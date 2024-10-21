using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using NewFMS.Models;
using System.Threading;

namespace NewFMS.Controllers
{
    public class HomeController : Controller
    {
       WCF_Common.CommonService servic = new WCF_Common.CommonService();
        //WCF_Report.ReportService ReportService = new WCF_Report.ReportService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        public JsonResult LongRunningProcess()
        {
            //THIS COULD BE SOME LIST OF DATA
            int itemsCount = 100;

            for (int i = 0; i <= itemsCount; i++)
            {
                //SIMULATING SOME TASK
                Thread.Sleep(500);

                //CALLING A FUNCTION THAT CALCULATES PERCENTAGE AND SENDS THE DATA TO THE CLIENT
                Functions.SendProgress("محاسبه شده: ", i, itemsCount);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
           
            //Session["Username"] = "admin";
            //Session["Password"] = "3781462CF954B7DAE340C624A8590B58FFF0B7E8";
            var time=servic.GetGeneralSettingDetail(2,1, IP, out Err);
           
            DateTime d1, d2;
            d2 = MyLib.Shamsi.Shamsi2miladiDateTime(time.fldValue);
            d1 = DateTime.Now;
            var s = d1.ToString();
            //d2 = Convert.ToDateTime("2022-11-11");/*20 ابان*/
          //  d2 = Convert.ToDateTime("2023-02-09");/*20 بهمن*/
            //d2 = Convert.ToDateTime("2033/03/05"); /*شاهرود دامغان*/
            if (DateTime.Compare(d1, d2) > 0)
                return null;   
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            string OrganIDD = "0";
            string OrganName = "";
            if (Session["OrganId"] != null)
            {
                OrganIDD = Session["OrganId"].ToString();
                var organ = servic.GetOrganizationDetail(Convert.ToInt32(OrganIDD), Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
                OrganName = organ.fldName;
            }
            else
            {
                var q = servic.GetOrganizationWithFilter("UserId", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err).ToList();
                if (q.Count == 1)
                {
                    OrganIDD = q[0].fldId.ToString();
                    OrganName = q[0].fldName;
                    Session["OrganId"] = OrganIDD;
                }
            }
                NewFMS.Models.CurrentDate.Year = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(0, 4);
                NewFMS.Models.CurrentDate.Month = MyLib.Shamsi.Miladi2ShamsiString(DateTime.Now).Substring(5, 2);
                NewFMS.Models.CurrentDate.nobatPardakht = 1;
                NewFMS.Models.CurrentDate.CostCenter = "";
                ViewBag.OrganName = OrganName;// Session["OrganName"].ToString();    
                ViewBag.OrganIDD = OrganIDD;
                ViewBag.time = DateTime.Now.TimeOfDay.Hours.ToString().PadLeft(2, '0') + ":" + DateTime.Now.TimeOfDay.Minutes.ToString().PadLeft(2, '0') + ":" + DateTime.Now.TimeOfDay.Seconds.ToString().PadLeft(2, '0');
                return View();
            
        }
        public ActionResult WinSalMali()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            var q = servic.GetDate(IP, out Err);
            string year = q.fldTarikh.Substring(0, 4);
            PartialView.ViewBag.Year = year;
           
            
            Session["Saal"] = year;
            
            
            
            return PartialView;
        }
        public ActionResult Home()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });

            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult GetSal()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<NewFMS.Models.CurrentDate> sh = new List<NewFMS.Models.CurrentDate>();
            var q = servic.GetDate(IP, out Err);
            int fldsal = Convert.ToInt32(q.fldTarikh.Substring(0, 4)) - 7;
            for (int i = fldsal; i < fldsal + 16; i++)
            {
                Models.CurrentDate CboSal = new Models.CurrentDate();

                CboSal.fldYear = i;
                sh.Add(CboSal);
            }
            return Json(sh, JsonRequestBehavior.AllowGet);
        }
        public FileContentResult DownloadChrome()
        {
            string savePath = Server.MapPath(@"~\Uploaded\Chrome\Google.Chrome.zip");
            MemoryStream st = new MemoryStream(System.IO.File.ReadAllBytes(savePath));
            return File(st.ToArray(), MimeType.Get(".zip"), "Google.Chrome.zip");
        }
        public ActionResult ChangePassword()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.userNamee = Session["Username"].ToString();
            return PartialView;
        }

        public ActionResult SaveChangePass(string fldPass, string fldNewPass, string fldUserName)
        {
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                Msg=servic.ChangePass(servic.HashPass(fldPass), fldUserName, servic.HashPass(fldNewPass), out Err);
                MsgTitle = "ذخیره موفق";
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                    Er = 1;
                }
                else
                {
                    Session["Password"] = servic.HashPass(fldNewPass);
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

        public ActionResult Logout()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });

            var date = servic.GetDate(IP, out Err).fldDateTime;
            servic.InsertInputInfo(date, IP, "", false, "", Session["Username"].ToString(), Session["Password"].ToString(), IP, out Err);
            UserLoginCount.RemoveOnlineUser(Session["UserId"].ToString());
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            return RedirectToAction("Logon", "Account", new { area = "" });
        }


        public ActionResult Setting()
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult NewSetting(int id)
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.Id = id;
            return PartialView;
        }
        //public ActionResult SaveSetting(WCF_Common.OBJ_Setting setting)
        //{
        //    string Msg = "", MsgTitle = "";
        //    try
        //    {

        //        byte[] report_file = null; string FileName = "";
        //        if (Session["savePath"] != null)
        //        {
        //            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
        //            report_file = stream.ToArray();
        //            FileName = Session["FileName"].ToString();
        //            var lenght = stream.Length;
        //            if (lenght > 102400)
        //            {
        //                Msg = "حجم عکس انتخاب شده بیشتر از 100 کیلو بایت نباید باشد.";
        //                MsgTitle = "اخطار";

        //                return Json(new
        //                {
        //                    Msg = Msg,
        //                    MsgTitle = MsgTitle
        //                }, JsonRequestBehavior.AllowGet);
        //            }

        //        }
        //        else
        //        {
        //            var Image = Server.MapPath("~/Content/Blank.jpg");
        //            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Image.ToString()));
        //            report_file = stream.ToArray();
        //            FileName = "";
        //        }

        //        if (setting.fldId == 0)
        //        {//ذخیره رکورد جدید

        //            MsgTitle = "ذخیره موفق";
        //            Msg = servic.InsertSetting(setting.fldTitle, setting.fldLogo, setting.fldDesc, Session["Username"].ToString(), servic.HashPass(Session["Password"].ToString()), IP, out Err);


        //        }
        //        else
        //        {//ویرایش رکورد 
        //            var q = servic.GetSettingDetail(setting.fldId, Session["Username"].ToString(), servic.HashPass(Session["Password"].ToString()), IP, out Err);
        //            if (Session["savePath"] == null)
        //            {
        //                MsgTitle = "ویرایش موفق";
        //                Msg = servic.UpdateSetting(setting.fldId, setting.fldTitle, setting.fldLogo, setting.fldDesc, Session["Username"].ToString(), servic.HashPass(Session["Password"].ToString()), IP, out Err);
        //            }
        //            else
        //            {
        //                MsgTitle = "ویرایش موفق";
        //                Msg = servic.UpdateSetting(setting.fldId, setting.fldTitle, setting.fldLogo, setting.fldDesc, Session["Username"].ToString(), servic.HashPass(Session["Password"].ToString()), IP, out Err);
        //            }

        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        if (x.InnerException != null)
        //            Msg = x.InnerException.Message;
        //        else
        //            Msg = x.Message;

        //        MsgTitle = "خطا";
        //    }
        //    return Json(new
        //    {
        //        Msg = Msg,
        //        MsgTitle = MsgTitle
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult DetailsSetting(int id)
        //{//نمایش اطلاعات جهت رویت کاربر
        //    try
        //    {

        //        var q = servic.GetSettingDetail(id, Session["Username"].ToString(), servic.HashPass(Session["Password"].ToString()), IP, out Err);

        //        var Id = 0; var Title = ""; var Desc = ""; byte[] Logo = new byte[1];
        //        if (q != null)
        //        {
        //            id = q.fldId;
        //            Title = q.fldTitle;
        //            Logo = q.fldLogo;
        //            Desc = q.fldDesc;
        //        }
        //        return Json(new
        //        {
        //            fldId = Id,
        //            fldTitle = Title,

        //            fldDesc = Desc

        //        }, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception x)
        //    {
        //        return Json(new { data = x.InnerException.Message });
        //    }
        //}
      
        /* public ActionResult NewChangePass()
        {//باز شدن پنجره
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult SaveChangePass(string fldPass, string fldNewPass, string fldConfirmPass)
        {
            string Msg = "", MsgTitle = "";
            try
            {
                Models.UniThesisDBEntities p = new Models.UniThesisDBEntities();

                var q = p.sp_tblUserSelect("fldId", Session["UserId"].ToString(), "", Convert.ToByte(Session["UserType"].ToString()), 1).FirstOrDefault();

                var pass = fldPass.GetHashCode().ToString();
                if (q.fldPassword == pass)
                {
                    if (fldNewPass == fldConfirmPass)
                    {
                        p.sp_UserPassUpdate(q.fldId.ToString(), fldNewPass.GetHashCode().ToString());
                        Msg = "ذخیره با موفقیت انجام شد.";
                        MsgTitle = "ذخیره موفق";

                    }
                    else
                    {
                        Msg = "رمز جدید با تکرار آن یکسان نیست.";
                        MsgTitle = "اخطار";
                    }
                }
                else
                {
                    Msg = "رمز فعلی وارد شده معتبر نیست.";
                    MsgTitle = "اخطار";
                }
            }
            catch (Exception x)
            {
                return Json(new { data = x.InnerException.Message, state = 1 });
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Upload()
        {
            if (Session["savePath"] != null)
            {
                string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                Session.Remove("savePath");
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
                    Icon = MessageBox.Icon.INFO,
                    Title = "خطا",
                    Message = "فایل غیرمجاز"
                });
                DirectResult result = new DirectResult();
                result.IsUpload = true;
                return result;
            }
        }*/

      
       /* public ActionResult Read(StoreRequestParameters parameters)
        {
            Models.UniThesisDBEntities p = new Models.UniThesisDBEntities();
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<Models.sp_tblSettingSelect> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<Models.sp_tblSettingSelect> data1 = null;
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
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldTitle";
                            break;

                    }
                    if (data != null)
                        data1 = p.sp_tblSettingSelect(field, searchtext, 100).ToList();
                    else
                        data = p.sp_tblSettingSelect(field, searchtext, 100).ToList();
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = p.sp_tblSettingSelect("", "", 100).ToList();
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

            List<Models.sp_tblSettingSelect> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }*/

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [DirectMethod]
        [OutputCache(Duration = 86400, VaryByParam = "theme")]
        public DirectResult GetTheme(string theme)
        {
            //string t = GestMezziUTY.uty.OnlyLetters(theme);


            Theme temp = (Theme)Enum.Parse(typeof(Theme), theme);

            this.Session["Ext.Net.Theme"] = temp;
            X.ResourceManager.Theme = temp;
            //return this.Direct(MvcResourceManager.GetThemeUrl(temp));
            return this.Direct();
        }
        public ActionResult ShowPic()
        {//برگرداندن عکس 
            if (Session["Username"] == null)
            {
                return RedirectToAction("Logon", "Account", new { area = "" });
            }
            var user=servic.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out Err).FirstOrDefault();
            var Employee = servic.GetEmployeeDetail(user.fldEmployeeId, IP, out Err);
            if (Employee != null)
            {
                var Employee_Detail = servic.GetEmployee_DetailWithFilter("fldEmployeeId", Employee.fldId.ToString(), 1, IP, out Err).FirstOrDefault();
                if (Employee_Detail.fldFileId != null)
                {
                    var file = servic.GetFileWithFilter("fldId", Employee_Detail.fldFileId.ToString(), 1, IP, out Err).FirstOrDefault().fldImage;
                    return File((byte[])file, "jpg");
                }
            }
            return null;
        }

        public ActionResult GetDate()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var q = servic.GetDate(IP, out Err);
            var time = q.fldDateTime;
            return Json(new
            {
                datetime = time.Hour.ToString().PadLeft(2, '0') + ":" +
                            time.Minute.ToString().PadLeft(2, '0') + ":" +
                            time.Second.ToString().PadLeft(2, '0')
            }, JsonRequestBehavior.AllowGet);
        }

      /*  public JsonResult online()
        {
            int UserLoginCount = UniThesis.Models.UserLoginCount.userObj.Count();
            return Json(new
            {
                UserLoginCount = UserLoginCount
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TaeedProposal()
        {
            Models.UniThesisDBEntities p = new Models.UniThesisDBEntities();
            bool TaeedProposal = p.sp_TaeedNahaeeProposal("TaeedProposal", Convert.ToInt32(Session["ProposalId"]), 0).FirstOrDefault().fldType;
            return Json(new
            {
                Taiid = TaeedProposal
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }*/
        public ActionResult ScanLetWin2()
        {
            return View();

        }
        [HttpPost]
        public ActionResult SaveToFile()
        {
            try
            {
                String strImageName;
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                HttpPostedFile uploadfile = files["RemoteFile"];
                strImageName = uploadfile.FileName;

                uploadfile.SaveAs(Server.MapPath("/") + "\\UploadedImages\\" + strImageName);

            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
