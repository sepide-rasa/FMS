
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using System.IO;

namespace NewFMS.Areas.Automation.Controllers
{
    public class LetterCharkheController : Controller
    {
        //
        // GET: /Automation/LetterCharkhe/

        WCF_Common.CommonService servic_Common = new WCF_Common.CommonService();
        WCF_Common.ClsError ErrCommon = new WCF_Common.ClsError();
        string IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
        public ActionResult Index(string LetterId,string MessageId)
        {//باز شدن تب
            if (Session["Username"] == null)
                return RedirectToAction("Logon", "Account", new { area = "" });
            var result = new Ext.Net.MVC.PartialViewResult();
            result.ViewBag.LetterId = LetterId;
            result.ViewBag.MessageId = MessageId;
            return result;
        }
        public ActionResult Help()
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            return PartialView;
        }
        public ActionResult NodeLoad(string nod, string LetterId, string MessageId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            NodeCollection nodes = new Ext.Net.NodeCollection();
            Models.AutomationEntities m = new Models.AutomationEntities();
            if (LetterId != "" && LetterId != "0")
            {
                if (nod == "0")
                {
                    var q = m.spr_Charkhe(Convert.ToInt32(LetterId), null).ToList();

                    foreach (var item in q)
                    {
                        Node asyncNode = new Node();
                        asyncNode.Text = item.SenderName;
                        asyncNode.NodeID = item.id.ToString();
                        asyncNode.Icon = Ext.Net.Icon.EmailTransfer;
                        asyncNode.AttributesObject = new
                        {
                            id = item.id,
                            text = item.SenderName,
                            fldTypeAss = item.fldTypeAss,
                            fldNameStatus = item.fldNameStatus,
                            fldLetterReadDate = item.fldLetterReadDate,
                            fldTarikhErja = item.fldTarikhErja,
                            fldDesc = item.fldDesc
                        };

                        var child = m.spr_Charkhe(Convert.ToInt32(LetterId), item.id).ToList();
                        foreach (var ch in child)
                        {
                            Node childNode = new Node();
                            childNode.Text = ch.SenderName;
                            childNode.NodeID = ch.id.ToString();
                            childNode.Icon = Ext.Net.Icon.EmailTransfer;
                            childNode.AttributesObject = new
                            {
                                id = ch.id,
                                text = ch.SenderName,
                                fldTypeAss = ch.fldTypeAss,
                                fldNameStatus = ch.fldNameStatus,
                                fldLetterReadDate = ch.fldLetterReadDate,
                                fldTarikhErja = ch.fldTarikhErja,
                                fldDesc = ch.fldDesc
                            };
                            asyncNode.Children.Add(childNode);
                        }

                        nodes.Add(asyncNode);
                    }

                }
                else
                {
                    var child = m.spr_Charkhe(Convert.ToInt32(LetterId), Convert.ToInt32(nod)).ToList();

                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.SenderName;
                        childNode.NodeID = ch.id.ToString();
                        childNode.Icon = Ext.Net.Icon.EmailTransfer;
                        childNode.AttributesObject = new
                        {
                            id = ch.id,
                            text = ch.SenderName,
                            fldTypeAss = ch.fldTypeAss,
                            fldNameStatus = ch.fldNameStatus,
                            fldLetterReadDate = ch.fldLetterReadDate,
                            fldTarikhErja = ch.fldTarikhErja,
                            fldDesc = ch.fldDesc
                        };
                        nodes.Add(childNode);

                    }

                }
            }
            else
            {
                if (nod == "0")
                {
                    var q = m.spr_CharkheMessage(Convert.ToInt32(MessageId), null).ToList();

                    foreach (var item in q)
                    {
                        Node asyncNode = new Node();
                        asyncNode.Text = item.SenderName;
                        asyncNode.NodeID = item.id.ToString();
                        asyncNode.Icon = Ext.Net.Icon.EmailTransfer;
                        asyncNode.AttributesObject = new
                        {
                            id = item.id,
                            text = item.SenderName,
                            fldTypeAss = item.fldTypeAss,
                            fldNameStatus = item.fldNameStatus,
                            fldLetterReadDate = item.fldLetterReadDate,
                            fldTarikhErja = item.fldTarikhErja,
                            fldDesc = item.fldDesc
                        };
                        var child = m.spr_CharkheMessage(Convert.ToInt32(MessageId), item.id).ToList();
                        foreach (var ch in child)
                        {
                            Node childNode = new Node();
                            childNode.Text = ch.SenderName;
                            childNode.NodeID = ch.id.ToString();
                            childNode.Icon = Ext.Net.Icon.EmailTransfer;
                            childNode.AttributesObject = new
                            {
                                id = ch.id,
                                text = ch.SenderName,
                                fldTypeAss = ch.fldTypeAss,
                                fldNameStatus = ch.fldNameStatus,
                                fldLetterReadDate = ch.fldLetterReadDate,
                                fldTarikhErja = ch.fldTarikhErja,
                                fldDesc = ch.fldDesc
                            };
                            asyncNode.Children.Add(childNode);
                        }

                        nodes.Add(asyncNode);
                    }

                }
                else
                {
                    var child = m.spr_CharkheMessage(Convert.ToInt32(MessageId), Convert.ToInt32(nod)).ToList();

                    foreach (var ch in child)
                    {
                        Node childNode = new Node();
                        childNode.Text = ch.SenderName;
                        childNode.NodeID = ch.id.ToString();
                        childNode.Icon = Ext.Net.Icon.EmailTransfer;
                        childNode.AttributesObject = new
                        {
                            id = ch.id,
                            text = ch.SenderName,
                            fldTypeAss = ch.fldTypeAss,
                            fldNameStatus = ch.fldNameStatus,
                            fldLetterReadDate = ch.fldLetterReadDate,
                            fldTarikhErja = ch.fldTarikhErja,
                            fldDesc = ch.fldDesc
                        };
                        nodes.Add(childNode);

                    }

                }
            }
            
            return this.Store(nodes);
        }
        public ActionResult PrintCharkhe(string LetterId, string MessageId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Logon", "Account");
            Ext.Net.MVC.PartialViewResult PartialView = new Ext.Net.MVC.PartialViewResult();
            PartialView.ViewBag.LetterId = LetterId;
            PartialView.ViewBag.MessageId = MessageId;
            return PartialView;
        }

        public ActionResult GeneratePDFRptCharkhe(string LetterId, string MessageId)
        {
            if (Session["Username"] == null)
                return RedirectToAction("logon", "Account", new { area = "" });
            try
            {
                DataSet.DataSet1 dt = new DataSet.DataSet1();
                DataSet.DataSet_Com dt_Com = new DataSet.DataSet_Com();

                dt.EnforceConstraints = false;
                dt_Com.EnforceConstraints = false;
                DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter LogoReport = new DataSet.DataSet_ComTableAdapters.spr_LogoReportWithUserIdTableAdapter();
                DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter GetDate = new DataSet.DataSet_ComTableAdapters.spr_GetDateTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptCharkheTableAdapter Charkhe = new DataSet.DataSet1TableAdapters.spr_RptCharkheTableAdapter();
                DataSet.DataSet1TableAdapters.spr_RptCharkheMessageTableAdapter CharkheMessage = new DataSet.DataSet1TableAdapters.spr_RptCharkheMessageTableAdapter();
                var User = servic_Common.GetUserWithFilter("CheckPass", Session["Username"].ToString(), Session["Password"].ToString(), 1, IP, out ErrCommon).FirstOrDefault();
               

                LogoReport.Fill(dt_Com.spr_LogoReportWithUserId, Convert.ToInt32(Session["OrganId"]));
                GetDate.Fill(dt_Com.spr_GetDate);
                

                FastReport.Report Report = new FastReport.Report();
                Report.RegisterData(dt_Com, "dataSetAutomation");
                Report.RegisterData(dt, "dataSetAutomation");

                if (LetterId != null)
                {
                    Charkhe.Fill(dt.spr_RptCharkhe, Convert.ToInt32(LetterId));
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Automation\RptCharkhe.frx");
                }

                if (MessageId != null)
                {
                    CharkheMessage.Fill(dt.spr_RptCharkheMessage, Convert.ToInt32(MessageId));
                    Report.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Reports\Automation\RptCharkheMessage.frx");
                }

                Report.SetParameterValue("UserName", User.fldNameEmployee);
                FastReport.Export.Pdf.PDFExport pdf = new FastReport.Export.Pdf.PDFExport();
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
