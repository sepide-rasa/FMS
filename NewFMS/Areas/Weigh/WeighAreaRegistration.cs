using System.Web.Mvc;

namespace NewFMS.Areas.Weigh
{
    public class WeighAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Weigh";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Weigh_default",
                "Weigh/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
