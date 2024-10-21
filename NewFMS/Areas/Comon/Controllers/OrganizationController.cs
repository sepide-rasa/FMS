using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.Comon.Controllers
{
    public class OrganizationController : Controller
    {
        //
        // GET: /Comon/Organization/

        WCF_Common.CommonService servic = new WCF_Common.CommonService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.ClsError Err = new WCF_Common.ClsError();

        /// <summary>
        /// فرم اصلی مربوط به سازمان
        /// </summary>
         public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
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

            this.GetCmp<TabPanel>(containerId).SetLastTabAsActive();
            return result;
        }
        /// <summary>
        /// تمامی اطلاعات جدول شهرها در لیست بازشو در فرم سازمان نمایش داده میشود.
        /// </summary>
        public ActionResult GetCity()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCityWithFilter("", "", 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldName = c.fldName });
            return this.Store(q);
        }
        public ActionResult ShowPicUpload(string dc)
        {//برگرداندن عکس 
            byte[] file = null;
            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
            file = stream.ToArray();
            var image = Convert.ToBase64String(file);
            return Json(new { image = image });
        }
        /// <summary>
        /// این متد برای آپلود لوگو سازمان در فرم استفاده میشود
        /// </summary>
        /// 
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
                var extension = Path.GetExtension(Request.Files[0].FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                {
                    if (Request.Files[0].ContentLength <= 102400)
                    {
                        HttpPostedFileBase file = Request.Files[0];
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
                            Message = "حجم فایل انتخابی می بایست کمتر از 1 مگابایت باشد."
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
        public FileContentResult Download(int FileId)
        {
            var q = servic.GetFileWithFilter("fldId", FileId.ToString(), 1,IP, out Err).FirstOrDefault();

            if (q != null)
            {
                MemoryStream st = new MemoryStream(q.fldImage);
                return File(st.ToArray(), MimeType.Get(q.fldPasvand), "DownloadFile" + q.fldPasvand);
            }
            return null;
        }
        /// <summary>
        ///.این متد برای نمایش عکس استفاده میشود
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
        //    var q = servic.GetOrganizationWithFilter("fldId", UserId.ToString(), 1, Session["Username"].ToString(), (Session["Password"].ToString()), out Err).FirstOrDefault();
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
        //            string e1 = "jpg";
        //            // qq.fldName.Substring(qq.fldName.IndexOf('.') + 1);


        //            MemoryStream st = new MemoryStream(qq.fldImage);
        //            return File(st.ToArray(), MimeType.Get(e1), "fileUpload." + e1);
        //        }

        //    }
        //    return null;
        //}
        /// <summary>
        /// از این متد در زمانی که ساختار درختی در فرم داشته باشیم استفاده میشود و برای بارگذاری درخت ونمایش آن در فرم استفاده میشود.
        /// </summary>
        /// <param name="nod">کد جدول سازمان ها</param>
        public ActionResult NodeLoad(string nod)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            if (nod == "0")
            {
                var q = servic.GetOrganizationWithFilter("", "", 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();

                foreach (var item in q)
                {
                    Node asyncNode = new Node();
                    asyncNode.Text = item.fldName;
                    asyncNode.NodeID = item.fldId.ToString();

                    var child = servic.GetOrganizationWithFilter("fldPId", item.fldId.ToString(), 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();
                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.fldName;
                        childNode.NodeID = ch.fldId.ToString();
                        childNode.Icon = Ext.Net.Icon.Building;
                        asyncNode.Children.Add(childNode);
                    }

                    nodes.Add(asyncNode);
                }

            }
            else
            {
                var child = servic.GetOrganizationWithFilter("fldPId", nod, 0, Session["Username"].ToString(), (Session["Password"].ToString()),IP, out Err).ToList();

                foreach (var ch in child)
                {
                    Node childNode = new Node();
                    childNode.Text = ch.fldName;
                    childNode.NodeID = ch.fldId.ToString();
                    childNode.Icon = Ext.Net.Icon.Building;
                    nodes.Add(childNode);

                }

            }
            return this.Direct(nodes);
        }
        public ActionResult DeleteFile()
        {
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                Session.Remove("savePath");
                Session.Remove("FileName");
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
        ///. از این متد برای  ذخیره اطلاعات جدید یا اطلاعات ویرایش شده استفاده می شود 
        /// </summary>
        /// <remarks><para> این قسمت به دو قسمت تقسیم میشودUpdate کد</para><para> .میکند Update اگر لوگو سازمان نیاز به ویرایش داشته باشد آنرا هم</para>
        ///              <para>پر میشود NULL در غیراینصورت در فیلد مربوط به عکس با مقدار</para> </remarks>
        ///     <example>     
        ///       <code>
        ///       if(Organization.Id==0)
        ///          {
        ///              //Insert
        ///          }
        ///       else
        ///          {
        ///              //Update
        ///            
        ///           }
        ///      </code>
        ///     </example>
        /// <param name="Organization">. که شامل تمامی فیلد های جدول سازمان به همراه مقادیر آنها میشود object</param>
        ///<returns> . موفقیت آمیز باشد پیغام ذخیره موفق یا ویرایش موفق را نمایش میدهد Update و Insert در صورتی که</returns>

        public ActionResult Save(WCF_Common.OBJ_Organization Organization, int IsDelOrg, int AshkhaseHoghoghiDetailId, string Desc)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";
            try
            {
                byte[] report_file = null; string FileName = ""; string Pasvand = "";
                if (Session["savePath"] != null && IsDelOrg==0)
                {
                    MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(Session["savePath"].ToString()));
                    report_file = stream.ToArray();
                    FileName = Session["FileName"].ToString();
                    Pasvand= Path.GetExtension(Session["savePath"].ToString());
                    var lenght = stream.Length;
                    if (lenght > 102400)
                    {
                        Msg = "حجم عکس انتخاب شده نباید بیشتر از 100 کیلو بایت  باشد.";
                        MsgTitle = "اخطار";

                        return Json(new
                        {
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
                    Pasvand=".jpg";
                }
                if (Organization.fldDesc == null)
                    Organization.fldDesc = "";
                if (AshkhaseHoghoghiDetailId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertAshkhaseHoghoghi_Detail(Organization.fldAshkhaseHoghoghiId, Organization.fldCodEghtesadi, Organization.fldAddress, Organization.fldCodePosti,
                        Organization.fldTelphon, Desc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                }
                else
                {
                    //ویرایش

                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateAshkhaseHoghoghi_Detail(AshkhaseHoghoghiDetailId, Organization.fldAshkhaseHoghoghiId, Organization.fldCodEghtesadi, Organization.fldAddress, Organization.fldCodePosti,
                    Organization.fldTelphon, Desc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);


                }
                if (Organization.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertOrganization(CodeDecode.stringcode(Organization.fldName), Organization.fldPId, report_file, Pasvand, Organization.fldCityId,/* Organization.fldCodePosti,
                       Organization.fldTelphon,Organization.fldAddress,*/Organization.fldCodAnformatic,Organization.fldCodKhedmat,Organization.fldAshkhaseHoghoghiId,
                        /*Organization.fldShenaseMelli,Organization.fldCodEghtesadi,Organization.fldShomareSabt,*/Organization.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                   
                }
                else
                {
                    if (Session["savePath"] == null && IsDelOrg==0)
                    {
                        //ویرایش
                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdateOrganization(Organization.fldId, CodeDecode.stringcode(Organization.fldName), Organization.fldPId, null,null, Organization.fldCityId,/* Organization.fldCodePosti,
                             Organization.fldTelphon, Organization.fldAddress,*/ Organization.fldFileId, Organization.fldCodAnformatic, Organization.fldCodKhedmat, Organization.fldAshkhaseHoghoghiId,
                            /* Organization.fldShenaseMelli,Organization.fldCodEghtesadi,Organization.fldShomareSabt, */Organization.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                    else
                    {
                        MsgTitle = "ویرایش موفق";
                        Msg = servic.UpdateOrganization(Organization.fldId, CodeDecode.stringcode(Organization.fldName), Organization.fldPId, report_file, Pasvand, Organization.fldCityId,/* Organization.fldCodePosti,
                             Organization.fldTelphon, Organization.fldAddress,*/ Organization.fldFileId, Organization.fldCodAnformatic, Organization.fldCodKhedmat, Organization.fldAshkhaseHoghoghiId,
                            /* Organization.fldShenaseMelli,Organization.fldCodEghtesadi,Organization.fldShomareSabt, */ Organization.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                    }
                }
                if (Err.ErrorType)
                {
                    MsgTitle = "خطا";
                    Msg = Err.ErrorMsg;
                }
                if (Session["savePath"] != null)
                {
                    string physicalPath = System.IO.Path.Combine(Session["savePath"].ToString());
                    Session.Remove("savePath");
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
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// .اطلاعات مربوط به یک سطر از جدول سازمان را براساس کد داده شده پاک میکند
        /// </summary>
        /// <param name="id">کد جدول سازمان</param>
        /// <returns>. درصورتیکه حذف با موفقیت انجام شود پیغام حذف موفق نمایش داده میشود</returns>
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = "";

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteOrganization(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                if (Err.ErrorType)
                {
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
            }
            return Json(new
            {
                Msg = Msg,
                MsgTitle = MsgTitle
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///. از این متد برای قسمت ویرایش فرم استفاده میشود.زمانی که فرم در حالت ویرایش باشد اطلاعات مربوط به آن قسمت را نشان میدهد
        /// </summary>
        /// <param name="Id">کد مربوط به سطر در حال ویرایش</param>
        /// <returns>اطلاعات مربوط به یک سطر از جدول سازمان ها را بر میگرداند</returns>
        public ActionResult Details(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetOrganizationDetail(Id, Session["Username"].ToString(), (Session["Password"].ToString()), IP, out Err);
            var pic = servic.GetFileWithFilter("fldId", q.fldFileId.ToString(), 1,IP, out Err).FirstOrDefault();
            var Img = "";
            if (pic.fldImage!=null)
             Img = Convert.ToBase64String(pic.fldImage);
            var p = servic.GetAshkhaseHoghoghi_DetailDetail(q.fldAshkhaseHoghoghiId, IP, out Err);
            var AshkhaseHoghoghiDetailId = 0;
            var Desc = "";
            if (p != null)
            {
                AshkhaseHoghoghiDetailId = p.fldId;
                Desc = p.fldDesc;
            }
            return Json(new
            {
                fldId = q.fldId,
                fldName =q.fldName,
                fldAddress = q.fldAddress,
                fldCityId = q.fldCityId.ToString(),
                fldCityName = q.fldCityName,
                fldCodePosti = q.fldCodePosti,
                fldFileId = q.fldFileId,
                fldPId = q.fldPId,
                fldTelphon = q.fldTelphon,
                fldDesc = q.fldDesc,
                fldCodAnformatic=q.fldCodAnformatic,
                fldCodEghtesadi=q.fldCodEghtesadi,
                fldCodKhedmat=q.fldCodKhedmat.ToString(),
                fldShenaseMelli=q.fldShenaseMelli,
                fldShomareSabt=q.fldShomareSabt,
                fldImage = Img,
                fldAshkhaseHoghoghiId = q.fldAshkhaseHoghoghiId,
                FldNameAshkhaseHoghoghi = q.FldNameAshkhaseHoghoghi,
                AshkhaseHoghoghiDetailId=AshkhaseHoghoghiDetailId,
                Desc=Desc
            }, JsonRequestBehavior.AllowGet);
        }


    }
}
