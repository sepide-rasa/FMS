using System.Web.Mvc;

namespace NewFMS.Areas.Archive
{
    public class ArchiveAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Archive";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Archive_default",
                "Archive/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
