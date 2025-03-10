using System;

namespace Omnae.WebApi.SlapperAutoMapper
{
    public class StringToNullableInt : Slapper.AutoMapper.Configuration.ITypeConverter
    {
        private readonly Slapper.AutoMapper.Configuration.ITypeConverter _typeConverter = new Slapper.AutoMapper.Configuration.ValueTypeConverter();

        public object Convert(object value, Type type)
        {
            var srtValue = (value as string);
            return string.IsNullOrWhiteSpace(srtValue) ? null : _typeConverter.Convert(value, type);
        }

        public bool CanConvert(object value, Type type)
        {
            var b = value is string && type == typeof(int?);
            return b;
        }

        public int Order => 80;
    }

    public class StringToNullableDecimal : Slapper.AutoMapper.Configuration.ITypeConverter
    {
        private readonly Slapper.AutoMapper.Configuration.ITypeConverter _typeConverter = new Slapper.AutoMapper.Configuration.ValueTypeConverter();

        public object Convert(object value, Type type)
        {
            var srtValue = (value as string);
            return string.IsNullOrWhiteSpace(srtValue) ? null : _typeConverter.Convert(value, type);
        }

        public bool CanConvert(object value, Type type)
        {
            var b = value is string && type == typeof(decimal?);
            return b;
        }

        public int Order => 80;
    }

}