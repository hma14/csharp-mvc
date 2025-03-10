using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Omnae.WebApi.SlapperAutoMapper;
using Slapper;

namespace Omnae.WebApi.App_Start
{
    public static class SlapperAutoMapperConfig
    {
        public static void RegisterConverter()
        {
            Slapper.AutoMapper.Logging.TraceLogger.MinimumLogLevel = Slapper.AutoMapper.Logging.LogLevel.Debug;

            Slapper.AutoMapper.Configuration.IdentifierConventions.Add((type => "id"));
            Slapper.AutoMapper.Configuration.IdentifierConventions.Add((type => "_Id"));
            Slapper.AutoMapper.Configuration.IdentifierConventions.Add((type => "_id"));
            Slapper.AutoMapper.Configuration.IdentifierConventions.Add((type => type.Name + "Id"));
            Slapper.AutoMapper.Configuration.IdentifierConventions.Add((type => type.Name + "_Id"));
            Slapper.AutoMapper.Configuration.IdentifierConventions.Add((type => type.Name + "_id"));

            Slapper.AutoMapper.Configuration.TypeConverters.Add(new StringToNullableInt());
            Slapper.AutoMapper.Configuration.TypeConverters.Add(new StringToNullableDecimal());
        }
    }
}