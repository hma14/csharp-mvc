using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Libs
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource obj, string mappingName = null) where TDestination : class;

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination, string mappingName = null);

        TDestination MapWithError<TSource, TDestination>(TSource source, string errorMessage, string mappingName = null)
            where TDestination : class, IErrorDetails;

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        TDestination Map<TDestination>(params object[] sources) where TDestination : class;
    }
}
