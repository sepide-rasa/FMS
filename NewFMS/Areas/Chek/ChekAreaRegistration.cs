using System.Web.Mvc;

namespace NewFMS.Areas.Chek
{
    public class ChekAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Chek";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Chek_default",
                "Chek/{controller}/{action}/{id}",
                 new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
