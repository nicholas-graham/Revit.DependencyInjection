using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Reflection;

namespace Revit.DependencyInjection.Extensions
{
    public static class UIControlledApplicationExtensions
    {
        /// <summary>
        /// Gets <see cref="Autodesk.Revit.UI.UIApplication"/> using the <see cref="UIControlledApplication"/>
        /// </summary>
        public static UIApplication GetUIApplication(this UIControlledApplication application)
        {
            var type = typeof(UIControlledApplication);

            var uiAppProperty = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(e => e.FieldType == typeof(UIApplication));

            return uiAppProperty?.GetValue(application) as UIApplication ??
                throw new NullReferenceException($"{nameof(UIApplication)} could not be resolved."); ;
        }

        /// <summary>
        /// Gets <see cref="UIControlledApplication"/> using the <see cref="Autodesk.Revit.UI.UIApplication"/>
        /// </summary>
        public static UIControlledApplication GetUIControlledApplication(this UIApplication application)
        {
            var type = typeof(UIControlledApplication);

            var constructor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { application.GetType() }, null);

            return constructor?.Invoke(new object[] { application }) as UIControlledApplication ??
                throw new NullReferenceException($"{nameof(UIControlledApplication)} could not be resolved."); ;
        }
    }
}
