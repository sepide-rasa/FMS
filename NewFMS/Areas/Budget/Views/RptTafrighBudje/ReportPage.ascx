

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Register Assembly="FastReport.Web" Namespace="FastReport.Web" TagPrefix="cc1" %>

<%{
      

      string Path = ViewBag.Path;
      WebReport3.Report.Load(Path);

      WebReport3.ID = ViewBag.RId;


      WebReport3.Report.SetParameterValue("aztarikh", ViewBag.AzTarikh);
      WebReport3.Report.SetParameterValue("tatarikh", ViewBag.TaTarikh);
      WebReport3.Report.SetParameterValue("Tarikh", ViewBag.Tarikh);
      WebReport3.Report.SetParameterValue("salmaliID", Convert.ToInt16(ViewBag.salMali));
      WebReport3.Report.SetParameterValue("sal", Convert.ToInt32(ViewBag.sal));
      WebReport3.Report.SetParameterValue("OrganId", Convert.ToInt32(ViewBag.OrganId));
      WebReport3.Report.SetParameterValue("organid", Convert.ToInt16(ViewBag.OrganId));
      WebReport3.Report.SetParameterValue("azsanad", Convert.ToInt32(ViewBag.AzSanad));
      WebReport3.Report.SetParameterValue("tasanad", Convert.ToInt32(ViewBag.TaSanad));
      WebReport3.Report.SetParameterValue("Psefr", Convert.ToBoolean(ViewBag.PrintZero));
      WebReport3.Report.SetParameterValue("sanadtype", Convert.ToInt32(ViewBag.sanadtype));
      WebReport3.Report.SetParameterValue("UserId", Convert.ToInt32(ViewBag.UserId));
      WebReport3.Report.SetParameterValue("digits", Convert.ToInt32(ViewBag.digits));
      WebReport3.Report.SetParameterValue("ReportId", 53);
      WebReport3.Report.SetParameterValue("fldModule_OrganId", 7);
      WebReport3.Report.SetParameterValue("connectionstr", ViewBag.connectionstr);
      
     
  
  } %>
<div class="center-form">
<form id="Form1" runat="server" dir="ltr">
    <cc1:WebReport ID="WebReport3" runat="server"  Width="100%" Height="100%"></cc1:WebReport>

</form> </div>  

<style>
    .center-form{

    justify-content: center;
    display: flex;
    margin: 40px 50px;
    border: 3px solid #050f1c4a;
    padding: 30px;
    background-color: rgb(10 8 20 / 27%);

}
</style>

