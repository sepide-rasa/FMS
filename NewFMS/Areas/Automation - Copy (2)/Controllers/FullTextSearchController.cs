using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Ext.Net;
using NewFMS.Models;
using System.Data.SqlClient;
using System.Data;
using NewFMS.DataSet;
using System.Configuration;

namespace NewFMS.Areas.Automation.Controllers
{
    public class FullTextSearchController : Controller
    {
        //
        // GET: /Automation/FullTextSearch/

        WCF_Automation.AutomationService servic = new WCF_Automation.AutomationService();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        WCF_Automation.ClsError Err = new WCF_Automation.ClsError();
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

        public ActionResult Search(string Text)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Models.AutomationEntities m = new Models.AutomationEntities();
            try
            {


                //string sqlString = "SELECT    tblLetter.fldID, tblLetter.fldYear, tblLetter.fldOrderId, tblLetter.fldSubject, tblLetter.fldLetterNumber, " +
                //     "fldLetterDate,fldCreatedDate, tblLetter.fldKeywords, tblLetter.fldLetterTypeID ,c.fldid as fldCommisionId, " +
                //     "tblAssignment.fldid as fldAssignmentId,tblLetter.fldDesc " +
                //     "FROM Auto.tblAssignment INNER JOIN Auto.tblContentFile  " +
                //     "INNER JOIN Auto.tblLetter ON tblContentFile.fldLetterID = tblLetter.fldID ON tblAssignment.fldLetterID = tblLetter.fldID  " +
                //     "INNER JOIN Auto.tblInternalAssignmentReceiver ON tblAssignment.fldID = tblInternalAssignmentReceiver.fldAssignmentID   " +

                //    "cross apply (SELECT top 1 c.fldid FROM Auto.tblCommision as c    " +
                //    "inner join com.tblAshkhas as a on a.fldId=c.fldAshkhasID  " +
                //    "inner join Com.tblUser as u on a.fldHaghighiId=u.fldEmployId " +
                //     "where c.fldID=tblInternalAssignmentReceiver.fldReceiverComisionID  and u.fldID=   1   )c  " + 
                //" where  freetext(fldLetterText,N" + "'" + Text + "'" + ")   " +
                //  "order by fldCreatedDate desc" ;

              string sqlString =   "select tblLetter.fldID, tblLetter.fldYear, tblLetter.fldOrderId, " +
             " tblLetter.fldSubject, tblLetter.fldLetterNumber, fldLetterDate,fldCreatedDate,  " +
             " tblLetter.fldKeywords, tblLetter.fldLetterTypeID ,isnull(c.fldid,0) as fldCommisionId,  " +
             " isnull(tblAssignment.fldid,0) as fldAssignmentId,tblLetter.fldDesc  " +
              " from auto.tblletter  " +
            " INNER JOIN Auto.tblContentFile  ON tblContentFile.fldLetterID = tblLetter.fldID " +
            " left join auto.tblAssignment on tblAssignment.fldLetterID = tblLetter.fldID  " +
            " left join  Auto.tblInternalAssignmentReceiver  " +
              " ON tblAssignment.fldID = tblInternalAssignmentReceiver.fldAssignmentID  " +
               " outer apply (SELECT top 1 c.fldid FROM Auto.tblCommision as c  " +
             "  inner join com.tblAshkhas as a on a.fldId=c.fldAshkhasID  " +
             "  inner join Com.tblUser as u on a.fldHaghighiId=u.fldEmployId " +
              "  where c.fldID=tblInternalAssignmentReceiver.fldReceiverComisionID and u.fldID= " + (Session["UserId"]) .ToString()+ " )c  " +
              "  where freetext(fldLetterText,N" + "'" + Text + "'" + ") order by fldCreatedDate desc ";

                List<Models.Letter> Inf = new List<Models.Letter>();
                string ConnectionString = ConfigurationManager.ConnectionStrings["RasaNewFMSConnectionString"].ConnectionString;
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                SqlCommand com = new SqlCommand(sqlString, con);
                com.CommandTimeout = 200000;
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    Inf.Add(new Models.Letter
                    {
                        fldID = Convert.ToInt32(dr["fldID"]),
                        fldYear = Convert.ToInt32(dr["fldYear"]),
                        fldOrderId = Convert.ToInt64(dr["fldOrderId"]),
                        fldSubject = dr["fldSubject"].ToString(),
                        fldDesc = dr["fldDesc"].ToString(),
                        fldLetterNumber = dr["fldLetterNumber"].ToString(),
                        fldLetterDate = dr["fldLetterDate"].ToString(),
                        fldCreatedDate = dr["fldCreatedDate"].ToString(),
                        fldLetterTypeID = Convert.ToInt32(dr["fldLetterTypeID"]),
                        fldKeywords = dr["fldKeywords"].ToString(),
                        fldComisionID = Convert.ToInt32(dr["fldCommisionId"]),
                        fldAssignmentId = Convert.ToInt32(dr["fldAssignmentId"]),
                    });
                }
                con.Close();
                return new JsonResult()
                {
                    Data = new
                    {
                        Er = 0,
                        Letter = Inf
                    },
                    MaxJsonLength = Int32.MaxValue,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception x)
            {
                System.Data.Entity.Core.Objects.ObjectParameter ErrorId = new System.Data.Entity.Core.Objects.ObjectParameter("fldID", typeof(int));
                string InnerException = "";
                if (x.InnerException != null)
                    InnerException = x.InnerException.Message;
                else
                    InnerException = x.Message;
                m.spr_tblErrorInsert(ErrorId, Session["Username"].ToString(), InnerException, DateTime.Now, IP, Convert.ToInt32(Session["UserId"]), "");
                return Json(new
                {
                    MsgTitle = "خطا",
                    Msg = "خطایی با شماره: " + ErrorId.Value + " رخ داده است لطفا با پشتیبانی تماس گرفته و کد خطا را اعلام فرمایید.",
                    Er = 1
                });
            }
            
          
        }
    }
}
