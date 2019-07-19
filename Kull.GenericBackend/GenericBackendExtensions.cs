﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kull.GenericBackend
{
    /// <summary>
    /// Extension method for Swashbuckle
    /// </summary>
    public static class SwashbuckleExtensions
    {
        public static void AddGenericBackend(this Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
        {
            options.DocumentFilter<SwaggerGeneration.DatabaseOperations>();
        }
    }

    /// <summary>
    /// Extension methods for MVC/Services
    /// </summary>
    public static class GenericBackendExtensions
    {
        public static void AddGenericBackend(this IServiceCollection services,
            GenericSP.SPMiddlewareOptions options = null,
            SwaggerGeneration.SwaggerFromSPOptions swaggerFromSPOptions=null )
        {
            services.AddRouting();
            services.AddSingleton<Model.SqlHelper>();
            services.AddSingleton<Model.SPParametersProvider>();
            services.AddSingleton<Model.NamingMappingHandler>();
            services.AddSingleton<GenericSP.SystemParameters>();
            services.AddSingleton<GenericSP.MiddlewareRegistration>();

            services.AddTransient<GenericSP.IGenericSPSerializer, GenericSP.GenericSPJsonSerializer>();
            services.AddTransient<GenericSP.IGenericSPMiddleware, GenericSP.GenericSPMiddleware>();

            var opts = options ??
                    new GenericSP.SPMiddlewareOptions();
            services.AddSingleton(opts);
            services.AddSingleton(swaggerFromSPOptions ?? new SwaggerGeneration.SwaggerFromSPOptions());
        }


        public static void UseGenericBackend(
            this IApplicationBuilder applicationBuilder,
            IRouteBuilder routeBuilder
            )
        {
            var service = applicationBuilder.ApplicationServices.GetService<GenericSP.MiddlewareRegistration>();
            var opts = applicationBuilder.ApplicationServices.GetService<GenericSP.SPMiddlewareOptions>();

            service.RegisterMiddleware(opts, routeBuilder);

        }
    }
}