# Revit.DependencyInjection
![Revit 2025](https://img.shields.io/badge/Revit-2025+-blue.svg)
![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio-2022-blue)
![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)

## 💡 About
The main objective of this project is to demonstrate how Microsoft's dependency injection [**libraries**](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) [**Microsoft.Extensions.DependencyInjection**](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) and [**Microsoft.Extensions.DependencyInjection.Abstractions**](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions) *could* be used within an Autodesk Revit addin. 

This sample demonstrates a `.Net8` implementation, although some simple modifications (removal of static interface methods and properties) will allow it to work with Revit versions 2024 and earlier.

The goal was to simply extend Autodesk's [**IExternalCommand**](https://www.revitapidocs.com/2017/ad99887e-db50-bf8f-e4e6-2fb86082b5fb.htm) to allow a basic service collection to be used in a pattern familiar to Revit API developers.

This is a very basic implementation lacking tests and significant architecture and is meant as a POC only. 

## 🔎 Why?
Without dependency injection, Revit [**IExternalApplication**](https://www.revitapidocs.com/2024/196c8712-71de-03e8-b30d-a9625bd626d2.htm) objects often become nightmarish, riddled with static dependencies and cross references, making long term maintenance and testing extremely difficult.

Establishing a dependency injection based Revit application using well supported Microsoft libraries would allow a modern patterns to also be used in Revit plugins. It is also much more testable, allowing Dependency Inversion and loose coupling. 

Revit ships with package version `2.0.0` in version 2024 and earlier, and version `7.0.0` in 2025, so these versions can be used in addins with minimal .dll hell risk inside Revit's single AppDomain.

## ⚡️ Quick Start
First, clone this repo.

Add references to `RevitAPI.dll` and `RevitAPIUI.dll` for your chosen Revit version.

Build the .sln. 

Navigate to your chosen Revit version `Addins` folder, for 2025 typically: `C:\ProgramData\Autodesk\Revit\Addins\2025`

Copy the output from the `bin` folder to this folder and include the `.addin` file.

Start Revit! 

You should see the DI application loaded under `Add-Ins -> Revit DI panel -> Greet Revit!` Click the button to be presented with a simple task dialog demonstrating accessing [**UIApplication**](https://www.revitapidocs.com/2017/51ca80e2-3e5f-7dd2-9d95-f210950c72ae.htm) at runtime as well as a `GreetMessageDependency` sample message!

## ✨ Useage
The application must inherit from `IRevitDIExternalApplication`, which defines an abstract method to return a [IServiceProvider](https://learn.microsoft.com/en-us/dotnet/api/system.iserviceprovider?view=net-8.0) for this app. It can be stored on the application or built and run on demand depending on the app requirements.
```csharp
public interface IRevitDIExternalApplication : IExternalApplication
{
    public static abstract IServiceProvider GetServiceProvider();
}
```
New commands inherit from both `RevitDICommand<T>` and the interface `IRevitDICommand`
```csharp
public class GreetMessageCommand : RevitDICommand<GreetMessageCommand>, IRevitDICommand
```
`ExecuteWithinScope` can set to `true` which allows the `RevitDICommand<T>` to use a scope to execute the command, more efficiently freeing up resources after the `Execute()` method returns. This defaults to `false` and will need to be false for any non-modal commands that require continued access to the service collection after `Execute()` returns.
```csharp
public static bool ExecuteWithinScope => true;
```
`RevitDICommand<T>` provides our base implementation of [IExternalCommand.Execute()](https://www.revitapidocs.com/2017/ad99887e-db50-bf8f-e4e6-2fb86082b5fb.htm). This is where the command is resolved with it's dependencies within a scope or from the root container, and `Execute()` is called.
```csharp
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
```
If we need more command dependencies, we can register them inside `RegisterCommandDependencies`. This is where we can build our command specific dependency tree.
```csharp
public static IServiceCollection RegisterCommandDependencies(IServiceCollection services)
{
    // Register dependencies here
    services.AddScoped<GreetMessageDependency>();
    return services;
}
```
Constructor injection can be used to resolve the `IRevitDependencyResolver<UIApplication>` which allows access to [UIApplication](https://www.revitapidocs.com/2017/51ca80e2-3e5f-7dd2-9d95-f210950c72ae.htm) at runtime, along with any other dependencies we registered inside `RegisterCommandDependencies`.
```csharp
private readonly IRevitDependencyResolver<UIApplication> _uiAppResolver;
private readonly GreetMessageDependency _greetMessageDependency;

public GreetMessageCommand(IRevitDependencyResolver<UIApplication> uiAppResolver, GreetMessageDependency greetMessageDependency)
{
    _uiAppResolver = uiAppResolver;
    _greetMessageDependency = greetMessageDependency;
}
```
Similar to the [IExternalCommand.Execute()](https://www.revitapidocs.com/2017/ad99887e-db50-bf8f-e4e6-2fb86082b5fb.htm) method, `Execute()` can be used to run Revit API code while also accessing constructor dependencies.
```csharp
public Result Execute()
{
    // The UI application can be accessed at runtime via the IRevitUIAppResolver
    var uiApplication = _uiAppResolver.GetDependency();

    TaskDialog.Show("DI Greet Message", $"{_greetMessageDependency.Message}, " +
        $"\n\nActive Revit Document Title: {uiApplication.ActiveUIDocument.Document.Title}");

    return Result.Succeeded;
}
```
Commands may be added to the service collection inside the `GetServiceProvider` method as we need to register all dependencies before building the provider.
```csharp
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
```
`RevitDIServiceCollection` provides a base set of dependencies (in this sample, only the dependencies required to access the [UIApplication](https://www.revitapidocs.com/2017/51ca80e2-3e5f-7dd2-9d95-f210950c72ae.htm) at runtime, `RevitUIAppResolver`). In larger apps this could be a suite of dependencies for a set of tools.

Push buttons may be added similar to regular commands:
```csharp
var buttonData = new PushButtonData(
    "GreetRevitDI",
    "Greet Revit!",
    typeof(RevitDICommand<GreetMessageCommand>).Assembly.Location,
    typeof(RevitDICommand<GreetMessageCommand>).FullName);

panel.AddItem(buttonData);
```
## 🫡 Acknowledgements
Some insparation taken from [**ricaun.Revit.DI**](https://github.com/ricaun-io/ricaun.Revit.DI) with an attempt to use Microsoft's libraries instead with a simplified pattern and syntax more familiar to .Net Core developers.

## ⚠️ License
This project is covered under the MIT license. 