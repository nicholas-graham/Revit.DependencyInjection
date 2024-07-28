using Autodesk.Revit.UI;
using System;

namespace Revit.DependencyInjection.Interfaces
{
    public interface IRevitDIExternalApplication : IExternalApplication
    {
        /// <summary>
        /// Returns the <see cref="IServiceProvider"/> for the Revit DI application.
        /// </summary>
        public static abstract IServiceProvider GetServiceProvider();
    }
}
