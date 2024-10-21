using System.Web.Mvc;

namespace NewFMS.Areas.PayRoll
{
    public class PayRollAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PayRoll";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PayRoll_default",
                "PayRoll/{controller}/{action}/{id}",
                new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
