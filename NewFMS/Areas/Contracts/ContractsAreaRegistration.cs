using System.Web.Mvc;

namespace NewFMS.Areas.Contracts
{
    public class ContractsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Contracts";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Contracts_default",
                "Contracts/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
