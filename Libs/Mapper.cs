using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Libs
{
    public class Mapper : IMapper
    {
        private readonly Dictionary<string, Action<global::AutoMapper.IMappingOperationOptions>> _mappingOptions = new Dictionary<string, Action<IMappingOperationOptions>>();

        public Mapper()
        {

        }
        public TDestination Map<TSource, TDestination>(TSource obj, string mappingName) where TDestination : class
        {
            if (mappingName == null)
                return global::AutoMapper.Mapper.Map<TSource, TDestination>(obj);

            var options = _mappingOptions[mappingName];
            return global::AutoMapper.Mapper.Map<TSource, TDestination>(obj, options);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return global::AutoMapper.Mapper.Map(source, destination);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, string mappingName)
        {
            if (mappingName == null)
                return global::AutoMapper.Mapper.Map(source, destination);

            var options = _mappingOptions[mappingName];
            return global::AutoMapper.Mapper.Map(source, destination, options);
        }

        public TDestination Map<TDestination>(params object[] sources) where TDestination : class
        {
            if (sources == null || sources.Length == 0)
            {
                return default(TDestination);
            }
            var destinationType = typeof(TDestination);
            var result = global::AutoMapper.Mapper.Map(sources[0], sources[0].GetType(), destinationType) as TDestination;
            for (var i = 1; i < sources.Length; i++)
            {
                if (sources[i] == null)
                {
                    continue;
                }
                global::AutoMapper.Mapper.Map(sources[i], result, sources[i].GetType(), destinationType);
            }
            return result;
        }

        public TDestination MapWithError<TSource, TDestination>(TSource source, string errorMessage, string mappingName)
            where TDestination : class, IErrorDetails
        {
            var result = Map<TSource, TDestination>(source, mappingName);
            result.HasError = true;
            result.ErrorMessage = errorMessage;
            return result;
        }
    }
}
