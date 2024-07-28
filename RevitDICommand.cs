using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;
using Revit.DependencyInjection.Interfaces;
using System;

namespace Revit.DependencyInjection
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RevitDICommand<T> : IExternalCommand where T : class, IRevitDICommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Create a new scope for the command, resolve it's dependencies and execute it.
            var provider = RevitDIApplication.GetServiceProvider() ?? 
                throw new InvalidOperationException("Service provider is not initialized.");

            T? command;
            if (T.ExecuteWithinScope)
            {
                // The command can be run within a service scope, so create a scope and resolve the command within it.
                using var scope = provider.CreateScope();
                command = scope.ServiceProvider.GetRequiredService<T>();
            }
            else
            {
                // Otherwise, resolve the command directly from the root provider.
                command = provider.GetRequiredService<T>();
            }

            return command.Execute();
        }
    }
}
