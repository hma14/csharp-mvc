using System.Web.Http;
using AutoMapper;

namespace Omnae.WebApi.App_Start.Mapping
{
    class AutoMapperConfig 
    {
        public static IMapper CreateMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.ConstructServicesUsing((type => GlobalConfiguration.Configuration.DependencyResolver.GetService(type)));

                cfg.AddProfile(new AutoMapperProfile());
            });
            
            return Mapper.Instance;
        }
    }
}