using AutoMapper;
using Omnae.App_Start.Mapping;

namespace Omnae.App_Start
{
    class AutoMapperConfig 
    {
        public static IMapper CreateMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            
            return Mapper.Instance;
        }
    }
}