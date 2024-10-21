using System.Web.Mvc;

namespace NewFMS.Areas.Accounting
{
    public class AccountingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Accounting";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Accounting_default",
                "Accounting/{controller}/{action}/{id}",
                new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
