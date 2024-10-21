using System.Web.Mvc;

namespace NewFMS.Areas.Comon
{
    public class ComonAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Comon";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Comon_default",
                "Comon/{controller}/{action}/{id}",
                new {controller="Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
