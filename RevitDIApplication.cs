using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;
using Revit.DependencyInjection.Commands.GreetRevit.DI;
using Revit.DependencyInjection.Interfaces;
using System;

namespace Revit.DependencyInjection
{
    public class RevitDIApplication : IRevitDIExternalApplication
    {
        private static IServiceProvider? _serviceProvider;
        private static UIControlledApplication? _uiCapp;
        public Result OnStartup(UIControlledApplication application)
        {
            _uiCapp = application;

            AddPushButtons(application);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public static IServiceProvider GetServiceProvider()
        {
            if (_uiCapp == null)
                throw new InvalidOperationException($"{nameof(UIControlledApplication)} is not initialized.");

            if (_serviceProvider == null)
            {
                // First command we run, build the service provider.
                var revitServiceCollection = new RevitDIServiceCollection(_uiCapp);

                // Add commands to the service collection here so their dependencies get added before we build the provider.
                revitServiceCollection.AddCommand<GreetMessageCommand>();

                // Store the service provider for subsequent commands.
                _serviceProvider = revitServiceCollection.BuildServiceProvider();
            }

            // If we've already run a command, return the built provider.
            return _serviceProvider;
        }

        private void AddPushButtons(UIControlledApplication application)
        {
            // Create panel and button for this sample.
            var panel = application.CreateRibbonPanel("Revit DI");

            var buttonData = new PushButtonData(
                "GreetRevitDI",
                "Greet Revit!",
                typeof(RevitDICommand<GreetMessageCommand>).Assembly.Location,
                typeof(RevitDICommand<GreetMessageCommand>).FullName);

            panel.AddItem(buttonData);
        }
    }
}
