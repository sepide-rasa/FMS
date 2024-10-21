using System.Web.Mvc;

namespace NewFMS.Areas.Khazanedari
{
    public class KhazanedariAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Khazanedari";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Khazanedari_default",
                "Khazanedari/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}