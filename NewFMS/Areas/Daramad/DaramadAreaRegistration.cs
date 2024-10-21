using System.Web.Mvc;

namespace NewFMS.Areas.Daramad
{
    public class DaramadAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Daramad";
            }
        }
         
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Daramad_default",
                "Daramad/{controller}/{action}/{id}",
                new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
