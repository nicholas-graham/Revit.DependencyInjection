using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;
using Revit.DependencyInjection.Resolvers;

namespace Revit.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds any base Revit dependency resolvers to the service collection. At present, only the <see cref="RevitUIAppResolver"/> is added.
        /// </summary>
        public static IServiceCollection AddRevitResolvers(this IServiceCollection services, UIControlledApplication uiCapp)
        {
            services.AddSingleton(uiCapp);
            services.AddSingleton<IRevitDependencyResolver<UIApplication>, RevitUIAppResolver>();
            // Any other resolvers can be added here to help de-couple API dependencies.
            return services;
        }
    }
}
