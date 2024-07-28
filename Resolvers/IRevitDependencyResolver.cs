namespace Revit.DependencyInjection.Resolvers
{
    /// <summary>
    /// Generic resolver interface that may be used to resolve Revit API objects at runtime.
    /// </summary>
    /// <typeparam name="T">The type of Revit API object to abstract resolution of.</typeparam>
    public interface IRevitDependencyResolver<T> where T : class
    {
        T GetDependency();
    }
}