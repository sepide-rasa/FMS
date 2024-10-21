using System.Web.Mvc;

namespace NewFMS.Areas.Deceased
{
    public class DeceasedAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Deceased";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Deceased_default",
                "Deceased/{controller}/{action}/{id}",
                new {action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
