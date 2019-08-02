using AutoService.Test.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using static Xunit.Assert;

namespace AutoService.Test
{
    public class Startup : IStartup
    {
        public void Configure(IApplicationBuilder app)
        {
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAutoService();
            return services.BuildServiceProvider();
        }
    }

    public class AspNetCoreAutoServiceTest
    {
        IWebHost _host;
        public AspNetCoreAutoServiceTest()
        {
            _host = WebHost.CreateDefaultBuilder<Startup>(null)
                .Build();
        }

        [Fact]
        public void TestSelfService_NotNull()
        {
            var transient = _host.Services.GetService<SelfServiceForTransient>();
            var scope = _host.Services.GetService<SelfServiceForScoped>();
            var singleton = _host.Services.GetService<SelfServiceForSingleton>();

            NotNull(transient);
            NotNull(scope);
            NotNull(singleton);
        }

        [Fact]
        public void TestSelfService_WithCorrectLifetime()
        {
            var transient1 = _host.Services.GetService<SelfServiceForTransient>();
            var scope1 = _host.Services.GetService<SelfServiceForScoped>();
            var singleton1 = _host.Services.GetService<SelfServiceForSingleton>();


            var transient2 = _host.Services.GetService<SelfServiceForTransient>();
            var scope2 = _host.Services.GetService<SelfServiceForScoped>();
            var singleton2 = _host.Services.GetService<SelfServiceForSingleton>();

            NotEqual(transient1.Id, transient2.Id);
            Equal(scope1.Id, scope2.Id);
            Equal(singleton1.Id, singleton2.Id);
        }
    }
}
