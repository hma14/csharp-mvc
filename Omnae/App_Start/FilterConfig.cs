using System.Web.Mvc;

namespace Omnae
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandler.AiHandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
#if !DEBUG
            filters.Add(new RequireHttpsAttribute());
#endif
        }
    }
}
