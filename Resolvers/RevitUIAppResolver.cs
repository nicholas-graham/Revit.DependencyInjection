using Autodesk.Revit.UI;
using Revit.DependencyInjection.Extensions;
using System;

namespace Revit.DependencyInjection.Resolvers
{
    /// <summary>
    /// Simple resolver to get the active <see cref="Autodesk.Revit.UI.UIApplication"/> at runtime.
    /// </summary>
    public class RevitUIAppResolver : IRevitDependencyResolver<UIApplication>
    {
        private readonly UIControlledApplication _uiCapp;

        public RevitUIAppResolver(UIControlledApplication uiCapp)
        {
            _uiCapp = uiCapp;
        }

        /// <summary>
        /// Resolves the active <see cref="Autodesk.Revit.UI.UIApplication"/> at runtime.
        /// </summary>
        /// <returns>The <see cref="Autodesk.Revit.UI.UIApplication"/> which may be used to further resolve Revit API objects.</returns>
        /// <exception cref="NullReferenceException">Thrown if the UIApplication could not be resolved.</exception>
        public UIApplication GetDependency()
        {
            return _uiCapp.GetUIApplication();
        }
    }
}
