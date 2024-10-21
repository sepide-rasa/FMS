using System.Web.Mvc;

namespace NewFMS.Areas.Automation
{
    public class AutomationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Automation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Automation_default",
                "Automation/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
