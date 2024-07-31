using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;
using Revit.DependencyInjection.Interfaces;
using Revit.DependencyInjection.Resolvers;

namespace Revit.DependencyInjection.Commands.GreetRevit.DI
{
    public class GreetMessageCommand : RevitDICommandBase<GreetMessageCommand>, IRevitDICommand
    {
        private readonly IRevitDependencyResolver<UIApplication> _uiAppResolver;
        private readonly GreetMessageDependency _greetMessageDependency;

        public static bool ExecuteWithinScope => true;

        public GreetMessageCommand(IRevitDependencyResolver<UIApplication> uiAppResolver, GreetMessageDependency greetMessageDependency)
        {
            _uiAppResolver = uiAppResolver;
            _greetMessageDependency = greetMessageDependency;
        }

        public static IServiceCollection RegisterCommandDependencies(IServiceCollection services)
        {
            // Register additional dependencies here.
            // IRevitDependencyResolver<UIApplication> is included already as a base dependency.
            services.AddScoped<GreetMessageDependency>();
            return services;
        }

        public Result Execute()
        {
            // The UI application can be accessed at runtime via the IRevitDependencyResolver<UIApplication>
            var uiApplication = _uiAppResolver.GetDependency();

            TaskDialog.Show("DI Greet Message", $"{_greetMessageDependency.Message}, " +
                $"\n\nActive Revit Document Title: {uiApplication.ActiveUIDocument.Document.Title}");

            return Result.Succeeded;
        }
    }
}
