using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
namespace NewFMS.Areas.Accounting.Controllers
{
    public class CodingController : Controller
    {
        //
        // GET: /Accounting/Coding/

        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Common.CommonService servic_com = new WCF_Common.CommonService(); 
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        WCF_Common.ClsError Err_com = new WCF_Common.ClsError();
        WCF_Accounting.ClsError Err = new WCF_Accounting.ClsError();
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

        public ActionResult Coding(int HeaderId, short Year, byte Status, int rowIdx)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            if (Status == 1)
            {
                result.ViewBag.HeaderId = HeaderId.ToString();
            }
            else
            {
                var q = servic.GetCoding_HeaderWithFilter("fldYear", Year.ToString(), Convert.ToInt32(Session["OrganId"]), 1, IP, out Err).FirstOrDefault();
                result.ViewBag.HeaderId = q.fldId.ToString();
            }
            result.ViewBag.Year = Year.ToString();
            result.ViewBag.Status = Status;
            result.ViewBag.rowIdx = rowIdx;
            return result;
        }

        public ActionResult SelectTemplate(int HeaderId, short Year)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.HeaderId = HeaderId.ToString();
            result.ViewBag.Year = Year.ToString();
            return result;
        }
        public ActionResult SetCaseType(int NodeId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.NodeId = NodeId.ToString();
            string CaseSelected = "";
            var q=servic.GetCodingDetail_CaseTypeWithFilter("fldCodingDetailId", NodeId.ToString(), 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                CaseSelected = CaseSelected + item.fldCaseTypeId + ";";
            }
            result.ViewBag.CaseSelected =CaseSelected;
            return result;
        }
        public ActionResult ReadCaseType()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Accounting.OBJ_CaseType> data = null;
            data = servic.GetCaseTypeWithFilter("","",0, IP, out Err).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetSal()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetFiscalYearWithFilter("", "", Convert.ToInt32(Session["OrganId"]), 0, IP, out Err).ToList().Select(c => new { fldId = c.fldId, fldYear = c.fldYear });
            return this.Store(q);
        }
        public ActionResult GetAccountingType()
        {
            var q = servic.GetAccountingTypeWithFilter("AccountingLevel", "", 0, IP, out Err).ToList().Select(l => new { fldName = l.fldName, fldId = l.fldId });
            return this.Store(q);
        }
        public ActionResult GetTypeHesab()
        {
            var q = servic.GetTypeHesabWithFilter("", "", 0, IP, out Err).ToList().Select(l => new { fldName = l.fldName, fldId = l.fldId });
            return this.Store(q);
        }
        public ActionResult GetTemplateCoding(int TemplateNameId, string PCode)
        {
            var q = servic.GetTemplateCodingWithFilter("PCod",PCode, TemplateNameId.ToString(),"",0, 0, IP, out Err).ToList().Select(l => new { fldTitle = l.fldName, 
                fldId = l.fldId, fldMahiyatId = l.fldMahiyatId ,fldTypeHesabId=l.fldTypeHesabId,fldMahiyat_GardeshId=l.fldMahiyat_GardeshId});
            return this.Store(q);
        }
        public ActionResult LoadTreeTemplateCoding_DaramadCode(string nod, string TemplateId,int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var q = servic.GetTemplateCodingWithFilter("PCod_Daramd", nod, TemplateId,"",HeaderId, 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();                
                var LevelName = servic.GetLevelsAccountingTypeDetail(item.fldLevelsAccountTypId, IP, out Err).fldName;
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text = item.fldCode + " - " + item.fldName;
                asyncNode.Expanded = true;
                switch (LevelName)/*سطوح انواع حسابداری*/
                {
                    case "گروه":
                        asyncNode.IconCls = "GroupIco";
                        break;
                    case "کل":
                        asyncNode.IconCls = "KolIco";
                        break;
                    case "معین":
                        asyncNode.IconCls = "MoeinIco";
                        break;
                    default:
                        asyncNode.IconCls = "TafsiliIco";
                        break;
                }
                asyncNode.AttributesObject = new
                {
                    fldCode = item.fldCode,
                    fldItemId = item.fldItemId,
                    fldName = nod == "0" ? item.fldName : item.fldName + "&nbsp;<input id='"+CodeDecode.stringcode(item.fldId.ToString())+"' class='TmpCode' type='text' value='"+item.fldDaramadCode+"' style='width:" + "70px;" + "'></input>",
                    fldMahiyatId = item.fldMahiyatId,
                    fldMahiyat_GardeshId=item.fldMahiyat_GardeshId,
                    fldLevelsAccountTypId = item.fldLevelsAccountTypId,
                    fldName_LevelsAccountingType = item.fldName_LevelsAccountingType,
                    fldPCod = item.fldPCod,
                    fldDesc = item.fldDesc,
                    fldTypeHesabId = item.fldTypeHesabId.ToString()
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
        }

        public ActionResult GetTemplate(short Year, int AccountingTypeId)
        {
            var q = servic.CheckAccountingLevel(AccountingTypeId, Convert.ToInt32(Session["OrganId"]), Year, IP, out Err).ToList().Select(l => new { fldName = l.fldName, fldId = l.fldId });
            return this.Store(q);
        }

        public ActionResult SaveHeader(WCF_Accounting.OBJ_Coding_Header Coding_Header)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Coding_Header.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertCoding_Header(Coding_Header.fldYear,Coding_Header.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateCoding_Header(Coding_Header.fldId, Coding_Header.fldYear, Coding_Header.fldDesc,Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;

            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteCoding_Header(Id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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

        public ActionResult DeleteNode(int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {

                MsgTitle = "حذف موفق";
                Msg = servic.DeleteCoding_Details(Id, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetCoding_HeaderDetail(Id,Convert.ToInt32(Session["OrganId"]), IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldYear = q.fldYear,
                fldDesc = q.fldDesc
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFirstParent(string Code,int HeaderID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCoding_DetailsWithFilter("FirstParent", Code, HeaderID.ToString(),"",1, IP, out Err).FirstOrDefault();
            return Json(new
            {
                fldTypeHesabId = q.fldTypeHesabId.ToString(),
                fldMahiyatId = q.fldMahiyatId.ToString(),
                fldMahiyat_GardeshId=q.fldMahiyat_GardeshId.ToString()
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CopyTemplate(int HeaderId, int TemplateNameId, int AccountTypeId, List<WCF_Accounting.OBJ_Coding_Details> details)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                var NewDetails=details.Select(l => new Models.Coding_DaramadCode {
                    fldId=Convert.ToInt32(CodeDecode.stringDecode(l.fldTitle)),
                    fldDaramadCode=l.fldDaramadCode
                });
                System.Data.DataTable dt = new System.Data.DataTable { TableName = "acc.tblCoding_Details" };
                using (var reader = FastMember.ObjectReader.Create(NewDetails))
                {
                    dt.Load(reader);
                }
                var q = servic.GetCoding_DetailsWithFilter("fldHeaderCodId", HeaderId.ToString(), "","", 0, IP, out Err).ToList();
                if (Err.ErrorType)
                {
                    Msg = Err.ErrorMsg;
                    MsgTitle = "خطا";
                    Er = 1;
                }
                else
                {
                    if (q.Count == 0)
                    {
                        MsgTitle = "عملیات موفق";
                        Msg = servic.CopyFromTemplateCodingToCoding(HeaderId, TemplateNameId, dt,Convert.ToInt32(Session["OrganId"]), IP, Session["Username"].ToString(), Session["Password"].ToString(), out Err);
                        if (Err.ErrorType)
                        {
                            MsgTitle = "خطا";
                            Msg = Err.ErrorMsg;
                            Er = 1;
                        }
                    }
                    else
                    {
                        Msg = "برای سال انتخاب شده قبلا اطلاعات ثبت شده است.";
                        MsgTitle = "خطا";
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        } 
        
        public ActionResult GetMahiyat()
        {
            var q = servic.GetMahiyatWithFilter("", "", 0, IP, out Err).ToList().Select(l => new { fldTitle = l.fldTitle, fldId = l.fldId });
            return this.Store(q);
        }
        public ActionResult LoadTreeCoding(string nod, int HeaderId, string FilterValue)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account", new { area = "" });
            try
            {

            
            NodeCollection nodes = new Ext.Net.NodeCollection();
            var q = servic.GetCoding_DetailsWithFilter("fldPID", nod,HeaderId.ToString(),"%" + FilterValue + "%", 0, IP, out Err).ToList();
            foreach (var item in q)
            {
                Node asyncNode = new Node();
                var LevelName = servic.GetAccountingLevelDetail(item.fldAccountLevelId,Convert.ToInt32(Session["OrganId"]), IP, out Err).fldName;
                var q1 = servic.GetCoding_DetailsWithFilter("fldPID", item.fldId.ToString(), HeaderId.ToString(), "", 0, IP, out Err).ToList();
                switch (LevelName)
                {
                    case "گروه":
                        asyncNode.IconCls = "GroupIco";
                        break;
                    case "کل":
                        asyncNode.IconCls = "KolIco";
                        break;
                    case "معین":
                        asyncNode.IconCls = "MoeinIco";
                        break;
                    default:
                        asyncNode.IconCls = "TafsiliIco";
                        break;
                }
                asyncNode.NodeID = item.fldId.ToString();
                asyncNode.Text = item.fldCode + " - " + item.fldTitle;
                asyncNode.Expanded = false;
                asyncNode.Cls = q1.Count.ToString();
                asyncNode.AttributesObject = new
                {
                    fldCode = item.fldCode,
                    fldTempCodingId = item.fldTempCodingId.ToString(),
                    fldTitle = item.fldTitle,
                    fldMahiyatId = item.fldMahiyatId.ToString(),
                    fldMahiyat_GardeshId = item.fldMahiyat_GardeshId.ToString(),
                    fldTempNameId=item.fldTempNameId.ToString(),
                    fldAccountingTypeId=item.fldAccountingTypeId.ToString(),
                    fldAccountLevelId = item.fldAccountLevelId,
                    fldName_AccountingLevel = item.fldName_AccountingLevel,
                    fldPCod = item.fldPCod,
                    fldDesc = item.fldDesc,
                    fldTypeHesabId=item.fldTypeHesabId.ToString(),
                    fldDaramadCode=item.fldDaramadCode,
                    fldItemIdParent=item.fldItemIdParent,
                    lastNode=item.lastNode
                };
                nodes.Add(asyncNode);
            }
            return this.Store(nodes);
            }
            catch (Exception x)
            {

                return null;
            }
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult SaveCoding_Details(WCF_Accounting.OBJ_Coding_Details Coding_Details, int PID)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Coding_Details.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertCoding_Details(Coding_Details.fldHeaderCodId, PID, Coding_Details.fldPCod, Coding_Details.fldTempCodingId, Coding_Details.fldTitle, Coding_Details.fldCode,
                        Coding_Details.fldDaramadCode, Coding_Details.fldAccountLevelId, Coding_Details.fldMahiyatId, Coding_Details.fldMahiyat_GardeshId,Coding_Details.fldTypeHesabId, Coding_Details.fldDesc, Session["Username"].ToString(),
                        Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateCoding_Details(Coding_Details.fldId, Coding_Details.fldHeaderCodId, Coding_Details.fldTempCodingId, Coding_Details.fldTitle, Coding_Details.fldCode,Coding_Details.fldDaramadCode,
                    Coding_Details.fldAccountLevelId, Coding_Details.fldMahiyatId, Coding_Details.fldMahiyat_GardeshId, Coding_Details.fldTypeHesabId, Coding_Details.fldDesc, Session["Username"].ToString(), Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });

            List<WCF_Accounting.OBJ_Coding_Header> data = null;
            data = servic.GetCoding_HeaderWithFilter("","",Convert.ToInt32(Session["OrganId"]),0, IP, out Err).ToList();

            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<WCF_Accounting.OBJ_Coding_Header> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------
            return this.Store(rangeData, data.Count);
        }

        public ActionResult CheckExistPCode(string Pcode, int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetCoding_DetailsWithFilter("fldCode", Pcode, HeaderId.ToString(),"", 0, IP, out Err).ToList();
            return Json(new
            {
                Valid = q.Count
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckValidateCode(string Code, int HeaderId, string Pcode, int Id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var Valid = servic.CheckValidCode_CodingDetail(HeaderId, Code, Pcode, Id, IP, out Err);
            return Json(new
            {
                Valid = Valid
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDefaultCode(string Pcode, int HeaderId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var q = servic.GetDefaultCode_Coding(HeaderId, Pcode, IP, out Err).FirstOrDefault();
            var parent = servic.GetCoding_DetailsWithFilter("fldCode", Pcode, HeaderId.ToString(), "", 0, IP, out Err).FirstOrDefault();
            return Json(new
            {
                parentId=parent.fldId,
                parentItemId=parent.fldItemIdParent,
                DefaultCode = q.fldCode,
                fldAccountLevelId = q.fldAccountLevelId,
                LevelName = q.fldName_AccountingLevel
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveCaseTypes(int NodeId, string cases)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                servic.DeleteCodingDetail_CaseType(NodeId, Session["Username"].ToString(),
                        Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err);

                var selected = cases.Split(';');
                foreach (var item in selected)
                {
                   MsgTitle = "ذخیره موفق";
                    if(item!="")
                    Msg = servic.InsertCodingDetail_CaseType(NodeId,Convert.ToInt32(item),Session["Username"].ToString(),
                        Session["Password"].ToString(), Convert.ToInt32(Session["OrganId"]), IP, out Err) ;
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
                Er = Er
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
