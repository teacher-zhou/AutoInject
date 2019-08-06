# AutoInject
The extension of Microsoft.Extensions.DependencyInjection to inject all services automatically for implementations.

# Where to get?
The package deploy on [Nuget](http://www.nuget.org) and the code can view on [Github](https://github.com/Michael-Pro/AutoInject). You can install from `Package Management Console` with command as below:
> Install-Package Micpro.AutoInject 

# Release Note for v1.0.0
- [New]`AddAutoInject` extensions method of `IServiceCollection` object.
- [New]Resolve the service automatically while the instance implement from `ITransient`,`IScoped` or `ISingleton`.


# Why to use?
In DI of `.NET Core`, user always inject the services in `ConfigureServices` manually, but sometimes the would get the message `Unable to resolve xxxxx from service` while running because forgot to inject to the container.

And if should have a lot of codes to add the service to the container like this:
```csharp
services.AddTransient<xxx>();
services.AddTransient<xxx>();
services.AddScoped<xxx>();
services.AddSingletone<xxx>();
services.AddScoped<xxx>();
services.AddTransient<xxx>();
services.AddScoped<xxx>();
services.AddSingletone<xxx>();
services.AddScoped<xxx>();
services.AddTransient<xxx>();
services.AddSingletone<xxx>();
//.....
```
once you forget some service to inject, you will get the exception thrown *Unable to resolve xxxxxx from service*.

So I consider about the injection could be automatically.

# How to use?
Just implement `ITransient`,`IScoped` or `ISingleton` interface for your injection service, such as your class or interface that the service type you wanna be.
```csharp
class MyService : ITransient { } //the service of class
```
or
```csharp
interface IMyService : IScoped { } // the service of interface

class MyService : IMyService { }
```

The lifetime should be the similar name of `ITransient` for transient, `IScoped` for scoped and `ISingleton` for singleton.

and they have the same called in `ConfigureServices` like:
```csharp
class MyService : ITransient
//same as
services.AddTransient<MyService>();
```
or
```csharp
interface IMyService : IScoped { }

class MyService : IMyService { }

//same as
services.AddScoped<IMyService, MyService>();
```
Just call `AddAutoInject` method in `ConfigureServices` in `Startup` class.

```csharp
//scan all assembly
services.AddAutoInject();

//scan specify assemblies
services.AddAutoInject(new []{ Assembly.Load("XXXX.AAA"), Assembly.Load("XXXX.BBB") })

//scan specify search pattern of assemblies
services.AddAutoInject("XXX.YY.*");

//scan specify types
services.AddAutoInject(new []{ Type.LoadType("AAA.BB"), Type.LoadType("CC.DD") });
```

and you can get your service from constructor or injection container.

```csharp
public MyController(IMyService service)//get the injected service
{
    
}
```

# Support Generic Type
You will inject generic type manully like:
```csharp
service.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```
AutoInject also support this kind of expression without write any codes :

```
public interface IRepository<T> : IScoped

public interface IRepository<TSource, TResult> : ITransient
```

it will resolve automatically and inject the correct generic arguments for generic type

```
public MyController(IRepository<Entity> repository, IRepository<int, List<int>> repository2) //still get the generic type service
{
    
}
```