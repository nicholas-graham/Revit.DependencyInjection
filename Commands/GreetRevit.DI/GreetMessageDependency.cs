namespace Revit.DependencyInjection.Commands.GreetRevit.DI
{
    public class GreetMessageDependency
    {
        public string Message { get; } = $"Message from {nameof(GreetMessageDependency)}: Hello from Revit DI!";
    }
}
