using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;

namespace NewFMS.Areas.Accounting.Controllers
{
    public class DescriptionDocumentController : Controller
    {
        //
        // GET: /Accounting/DescriptionDocument/
        WCF_Accounting.AccountingService servic = new WCF_Accounting.AccountingService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
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

        public ActionResult IndexWin(byte State)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.State = State;
            return result;
        }
        public ActionResult Check(string Value)
        {
            var aloow = false; var gg = false;
            int count = Value.Where(c => c == ']').Count();
            int count1 = Value.Where(c => c == '[').Count();
            if (count == 0 && count1 == 0)
            {
                return Json(new
                {
                    Msg = ""
                }, JsonRequestBehavior.AllowGet);
            }
            if (count != count1)
            {
                return Json(new
                {
                    Msg = "فرمت پارامتر وارد شده نادرست است"
                }, JsonRequestBehavior.AllowGet);
            }
            var list = Value.ToCharArray();
            for (int i = 0; i < list.Length;i++)
            {
                if (list[i].ToString() == "[")
                        aloow = true;
                if (aloow == true)
                {
                    for (int j = i + 1; j < list.Length;j++)
                    {

                        if (list[j].ToString() == "[")
                        {
                            gg = true;
                            break;
                        }
                        else if (list[j].ToString() == "]")
                        {
                            aloow = false;
                            gg = false;
                            i = j;
                            break;
                        }
                        
                    }
                }
                if (gg == true)
                    break;
                
            }
            /*var mm=Value.IndexOf('[');
           var pp= Value.Remove(0, mm);
            var m = pp.StartsWith("[");*/
            
            if (gg == true)
            {
                return Json(new
                {
                    Msg = "فرمت پارامتر وارد شده نادرست است"
                }, JsonRequestBehavior.AllowGet);
            }
           // var q = Value.EndsWith("]");
            return Json(new
            {
                Msg=""
                //,q = q
            }, JsonRequestBehavior.AllowGet);

        }        
        public ActionResult Save(WCF_Accounting.OBJ_DocumentDesc Document)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; var Er = 0;
            try
            {
                if (Document.fldId == 0)
                {
                    //ذخیره
                    MsgTitle = "ذخیره موفق";
                    Msg = servic.InsertDocumentDesc(Document.fldName,Document.fldDocDesc,Convert.ToBoolean(Document.fldFlag), Document.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
                }
                else
                {
                    //ویرایش
                    MsgTitle = "ویرایش موفق";
                    Msg = servic.UpdateDocumentDesc(Document.fldId, Document.fldName, Document.fldDocDesc,Convert.ToBoolean(Document.fldFlag), Document.fldDesc, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);

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
        public ActionResult Delete(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            string Msg = "", MsgTitle = ""; int Er = 0;
            try
            {
                //حذف
                MsgTitle = "حذف موفق";
                Msg = servic.DeleteDocumentDesc(id, Session["Username"].ToString(), (Session["Password"].ToString()), Convert.ToInt32(Session["OrganId"]), IP, out Err);
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
            var q = servic.GetDocumentDescDetail(Id, IP, out Err);
            return Json(new
            {
                fldId = q.fldId,
                fldName = q.fldName,
                fldDocDesc = q.fldDocDesc,
                fldDesc = q.fldDesc,
                fldFlag=Convert.ToByte(q.fldFlag).ToString()
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            var filterHeaders = new FilterHeaderConditions(this.Request.Params["filterheader"]);

            List<WCF_Accounting.OBJ_DocumentDesc> data = null;
            if (filterHeaders.Conditions.Count > 0)
            {
                string field = "";
                string searchtext = "";
                List<WCF_Accounting.OBJ_DocumentDesc> data1 = null;
                foreach (var item in filterHeaders.Conditions)
                {
                    var ConditionValue = (Newtonsoft.Json.Linq.JValue)item.ValueProperty.Value;

                    switch (item.FilterProperty.Name)
                    {
                        case "fldId":
                            searchtext = ConditionValue.Value.ToString();
                            field = "fldId";
                            break;
                        case "fldName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldName";
                            break;
                        case "fldDocDesc":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldDocDesc";
                            break;
                        case "fldFlagName":
                            searchtext = "%" + ConditionValue.Value.ToString() + "%";
                            field = "fldFlagName";
                            break;
                    }
                    if (data != null)
                    {
                        data1 = servic.GetDocumentDescWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    }
                    else
                    {
                        data = servic.GetDocumentDescWithFilter(field, searchtext, 100, IP, out Err).ToList();
                    }
                }
                if (data != null && data1 != null)
                    data.Intersect(data1);
            }
            else
            {
                data = servic.GetDocumentDescWithFilter("", "", 100, IP, out Err).ToList();
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

            List<WCF_Accounting.OBJ_DocumentDesc> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return this.Store(rangeData, data.Count);
        }
    }
}
