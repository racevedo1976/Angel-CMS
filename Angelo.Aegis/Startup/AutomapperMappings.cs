using System;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace Angelo.Aegis
{
    public static class AutoMapperMappings
    {
        public static IServiceCollection AddAutoMapperMappings(this IServiceCollection services)
        {

            Mapper.Initialize(config => {
                config.CreateMissingTypeMaps = true;
            });

            return services;
        }
 
    }
}