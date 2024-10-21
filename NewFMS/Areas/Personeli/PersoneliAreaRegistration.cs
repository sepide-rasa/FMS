using System.Web.Mvc;

namespace NewFMS.Areas.Personeli
{
    public class PersoneliAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Personeli";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Personeli_default",
                "Personeli/{controller}/{action}/{id}",
                new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
