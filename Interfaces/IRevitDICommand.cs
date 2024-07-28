using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;

namespace Revit.DependencyInjection.Interfaces
{
    /// <summary>
    /// Base DI command interface for Revit DI commands.
    /// </summary>
    public interface IRevitDICommand
    {
        /// <summary>
        /// Can be set to True if the command does not exit the <see cref="Execute()"/> method until the run is fully complete.
        /// Otherwise, the command may need access to it's services after the Execute method has completed. Defaults to false.
        /// </summary>
        public static abstract bool ExecuteWithinScope { get; }

        /// <summary>
        /// Body of the command to execute. Functions similar to <see cref="IExternalCommand.Execute(ExternalCommandData, ref string, Autodesk.Revit.DB.ElementSet)"/>.
        /// </summary>
        public Result Execute();

        /// <summary>
        /// Register any dependencies required for the <see cref="IRevitDICommand"/> on the <see cref="IServiceCollection"/>.
        /// </summary>
        public static abstract IServiceCollection RegisterCommandDependencies(IServiceCollection services);

    }
}
