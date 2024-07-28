using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;
using Revit.DependencyInjection.Extensions;
using Revit.DependencyInjection.Interfaces;

namespace Revit.DependencyInjection
{
    /// <summary>
    /// Extension of <see cref="ServiceCollection"/> that adds Revit-specific dependencies."
    /// </summary>
    public class RevitDIServiceCollection : ServiceCollection
    {
        public RevitDIServiceCollection(UIControlledApplication uiCapp)
        {
            this.AddRevitResolvers(uiCapp);
        }
        public RevitDIServiceCollection(UIApplication uiApp)
        {
            var uiCapp = uiApp.GetUIControlledApplication();
            this.AddRevitResolvers(uiCapp);
        }

        /// <summary>
        /// Adds a Revit DI command to the service collection and registers it's dependencies.
        /// </summary>
        /// <typeparam name="T">The type of command to add</typeparam>
        public IServiceCollection AddCommand<T>() where T : class, IRevitDICommand
        {
            this.AddScoped<T>();
            T.RegisterCommandDependencies(this);
            return this;
        }
    }
}
