using AutoInject.Test.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using static Xunit.Assert;

namespace AutoInject.Test
{
    public class Startup : IStartup
    {
        public void Configure(IApplicationBuilder app)
        {
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAutoInject();
            return services.BuildServiceProvider();
        }
    }

    public class AspNetCoreAutoInjectTest
    {
        IWebHost _host;
        public AspNetCoreAutoInjectTest()
        {
            _host = WebHost.CreateDefaultBuilder<Startup>(null)
                .Build();
        }

        #region Test class without any interfaces will be injected by it-self
        [Fact(DisplayName ="Self-service inject sucessfully")]
        public void TestSelfService_NotNull()
        {
            var transient = _host.Services.GetService<SelfServiceForTransient>();
            var scope = _host.Services.GetService<SelfServiceForScoped>();
            var singleton = _host.Services.GetService<SelfServiceForSingleton>();

            NotNull(transient);
            NotNull(scope);
            NotNull(singleton);
        }

        [Fact(DisplayName ="Self-service inject with correct lifetime.")]
        public void TestSelfService_WithCorrectLifetime()
        {
            var transient1 = _host.Services.GetService<SelfServiceForTransient>();
            var singleton1 = _host.Services.GetService<SelfServiceForSingleton>();


            var transient2 = _host.Services.GetService<SelfServiceForTransient>();
            var singleton2 = _host.Services.GetService<SelfServiceForSingleton>();

            NotEqual(transient1.Id, transient2.Id);
            Equal(singleton1.Id, singleton2.Id);

            using (var scope=_host.Services.CreateScope())
            {
                var scope1= scope.ServiceProvider.GetService<SelfServiceForScoped>();
                var scope2 = scope.ServiceProvider.GetService<SelfServiceForScoped>();

                Equal(scope1.Id, scope2.Id);
            }
        }
        #endregion

        #region Test the instance injected by an interface for service
        [Fact(DisplayName ="The interface inject for service.")]
        public void Test_InjectWithInterface()
        {
            var serviceTransient = _host.Services.GetService<ISingleServiceWithTransient>();
            NotNull(serviceTransient);
            IsType<SingleServiceWithTransient>(serviceTransient);


            var serviceSingleton = _host.Services.GetService<ISingleServiceWithSingleton>();
            NotNull(serviceSingleton);
            IsType<SingleServiceWithSingleton>(serviceSingleton);


            var serviceScoped = _host.Services.GetService<ISingleServiceWithScoped>();
            NotNull(serviceScoped);
            IsType<SingleServiceWithScoped>(serviceScoped);
        }

        [Fact(DisplayName ="The interface inject for service with correct lifetime")]
        public void Test_InjectWithInterface_WithCorrectLifetime()
        {
            var transient1 = _host.Services.GetService<ISingleServiceWithTransient>();
            var singleton1 = _host.Services.GetService<ISingleServiceWithSingleton>();


            var transient2 = _host.Services.GetService<ISingleServiceWithTransient>();
            var singleton2 = _host.Services.GetService<ISingleServiceWithSingleton>();

            NotEqual(transient1.Id, transient2.Id);
            Equal(singleton1.Id, singleton2.Id);

            using (var scope = _host.Services.CreateScope())
            {
                var scope1 = scope.ServiceProvider.GetService<ISingleServiceWithScoped>();
                var scope2 = scope.ServiceProvider.GetService<ISingleServiceWithScoped>();

                Equal(scope1.Id, scope2.Id);
            }
        }
        #endregion

        #region inject multiple services
        [Fact(DisplayName ="One or more services inject for service")]
        public void Test_MultipleServices_InOneImplementation()
        {
            var service1 = _host.Services.GetService<IMultiService1>();
            var service2 = _host.Services.GetService<IMultiService2>();

            NotNull(service1);
            NotNull(service2);

            IsType<MultiService>(service1);
            IsType<MultiService>(service2);
        }
        #endregion

        #region more lifetime
        [Fact(DisplayName ="3 lifetime injections with same service should be singleton")]
        public void Test_3MoreLifetimeService_ShouldBeSingleton()
        {
            var service1 = _host.Services.GetService<TransientScopeSingletonService>();
            var service2 = _host.Services.GetService<TransientScopeSingletonService>();
            var service3 = _host.Services.GetService<TransientScopeSingletonService>();
            var service4 = _host.Services.GetService<TransientScopeSingletonService>();


            Equal(service1.Id, service2.Id);
            Equal(service1.Id, service3.Id);
            Equal(service1.Id, service4.Id);
            Equal(service2.Id, service3.Id);
            Equal(service2.Id, service4.Id);
            Equal(service3.Id, service4.Id);
        }

        [Fact(DisplayName ="scope/transient injections with save service should be scope")]
        public void Test_MoreLifetimeService_ShouldBeScope()
        {
            using (var scope=_host.Services.CreateScope())
            {
                var scope1 = scope.ServiceProvider.GetService<ScopeTransient>();
                var scope2 = scope.ServiceProvider.GetService<ScopeTransient>();
                var scope3 = scope.ServiceProvider.GetService<ScopeTransient>();
                var scope4 = scope.ServiceProvider.GetService<ScopeTransient>();


                Equal(scope1.Id, scope2.Id);
                Equal(scope1.Id, scope3.Id);
                Equal(scope1.Id, scope4.Id);
                Equal(scope2.Id, scope3.Id);
                Equal(scope2.Id, scope4.Id);
                Equal(scope3.Id, scope4.Id);
            }
        }

        [Fact(DisplayName = "singleton/transient injections with save service should be singleton")]
        public void Test_MoreLifetimeService_ShouldBeSingleton()
        {
            var service1 = _host.Services.GetService<SingletonTransient>();
            var service2 = _host.Services.GetService<SingletonTransient>();
            var service3 = _host.Services.GetService<SingletonTransient>();
            var service4 = _host.Services.GetService<SingletonTransient>();


            Equal(service1.Id, service2.Id);
            Equal(service1.Id, service3.Id);
            Equal(service1.Id, service4.Id);
            Equal(service2.Id, service3.Id);
            Equal(service2.Id, service4.Id);
            Equal(service3.Id, service4.Id);
        }
        #endregion
    }
}
