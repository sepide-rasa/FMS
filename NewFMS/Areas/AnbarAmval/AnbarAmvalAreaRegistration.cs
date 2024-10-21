using System.Web.Mvc;

namespace NewFMS.Areas.AnbarAmval
{
    public class AnbarAmvalAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AnbarAmval";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AnbarAmval_default",
                "AnbarAmval/{controller}/{action}/{id}",
                 new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
