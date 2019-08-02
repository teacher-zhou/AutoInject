using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// The extension of <see cref="IServiceCollection"/> for auto services.
    /// </summary>
    public static class AutoServiceExtensions
    {
        /// <summary>
        /// These types should be ignored for injection.
        /// </summary>
        readonly static Type[] _internalServiceType = new[] { typeof(ISingleton), typeof(ITransient), typeof(IScoped) };

        /// <summary>
        /// Add the service to scan the current app domain in bin directory with all assembly files and inject the implementation with service automatically into <see cref="IServiceCollection"/> .
        /// </summary>
        /// <param name="services">The instance of <see cref="IServiceCollection"/>.</param>
        /// <returns>The instance of <see cref="IServiceCollection"/> .</returns>
        public static IServiceCollection AddAutoService(this IServiceCollection services)
        {
            return services.AddAutoService("*");
        }

        /// <summary>
        /// Add the service to scan the current app domain in bin directory with search pattern for assembly files and inject the implementation with service automatically into <see cref="IServiceCollection"/> .
        /// </summary>
        /// <param name="services">The instance of <see cref="IServiceCollection"/>.</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files in path. This parameter
        /// can contain a combination of valid literal path and wildcard (* and ?) characters,
        /// but it doesn't support regular expressions.
        /// </param>
        /// <returns>The instance of <see cref="IServiceCollection"/> .</returns>
        public static IServiceCollection AddAutoService(this IServiceCollection services, string searchPattern)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var assemblies = Directory.GetFiles(baseDirectory, $"{searchPattern}.dll").Select(m => Assembly.LoadFrom(m));
            return services.AddAutoService(assemblies.ToArray());
        }

        /// <summary>
        /// Add the service to scan the specify assemblies and inject the implementation with service automatically into <see cref="IServiceCollection"/> .
        /// </summary>
        /// <param name="service">The instance of <see cref="IServiceCollection"/>.</param>
        /// <param name="assemblies">The assemblies to scan.</param>
        /// <returns>The instance of <see cref="IServiceCollection"/> .</returns>
        public static IServiceCollection AddAutoService(this IServiceCollection services,Assembly[] assemblies)
        {
            if (assemblies is null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            var allTypes = assemblies.SelectMany(m => m.GetTypes());
            return services.AddAutoService(allTypes.ToArray());
        }

        /// <summary>
        /// Add the service to scan the specify types and inject the implementation with service automatically into <see cref="IServiceCollection"/> .
        /// </summary>
        /// <param name="service">The instance of <see cref="IServiceCollection"/>.</param>
        /// <param name="types">The types to scan.</param>
        /// <returns>The instance of <see cref="IServiceCollection"/> .</returns>
        /// <exception cref="ArgumentNullException"><paramref name="types"/> is null.</exception>
        public static IServiceCollection AddAutoService(this IServiceCollection services, Type[] types)
        {
            if (types is null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            //获取所有的实例类型
            foreach (var instanceType in types
                .Where(m => m.IsClass && !m.IsAbstract 
                    && m.GetInterfaces().Any(i => _internalServiceType.Contains(i))
                    )
                )
            {
                var lifetime = ServiceLifetime.Transient;


                if (typeof(ITransient).GetTypeInfo().IsAssignableFrom(instanceType))
                {
                    lifetime = ServiceLifetime.Transient;
                }

                if (typeof(IScoped).GetTypeInfo().IsAssignableFrom(instanceType))
                {
                    lifetime = ServiceLifetime.Scoped;
                }

                if (typeof(ISingleton).GetTypeInfo().IsAssignableFrom(instanceType))
                {
                    lifetime = ServiceLifetime.Singleton;
                }

                //find out the services to be register with current instance class.
                var servicesTypeToBeRegister = instanceType.GetInterfaces()
                    .Where(type => typeof(IDependencyService) != type)
                    .Where(type => !_internalServiceType.Contains(type)) 
                    .Where(type => !FilterIgnoreServiceAttribute(instanceType, type))
                    ;

                if (!servicesTypeToBeRegister.Any()) //inject self to be service
                {
                    services.AddServiceDescriptor(lifetime, instanceType);
                }
                else
                {
                    //inject all interfaces to be service
                    foreach (var serviceType in servicesTypeToBeRegister)
                    {
                        services.AddServiceDescriptor(lifetime, instanceType, serviceType);
                    }
                }
            }

            //select and filter witch has 'IgnoreServiceAttribute' attribute.
            bool FilterIgnoreServiceAttribute(Type instanceType, Type serviceType)
            {
                var ignoreServiceAttributes = new List<IgnoreServiceAttribute>();
                ignoreServiceAttributes.AddRange(serviceType.GetCustomAttributes<IgnoreServiceAttribute>());
                ignoreServiceAttributes.AddRange(instanceType.GetCustomAttributes<IgnoreServiceAttribute>());

                foreach (var ignoreServiceAttribute in ignoreServiceAttributes)
                {
                    return ignoreServiceAttribute.ServiceType == serviceType;
                }
                return false;
            }

            return services;
        }

        /// <summary>
        /// Add the implementation type and service type into the <see cref="ServiceDescriptor"/> container with specify service lifetime.
        /// <para>
        /// The method can resolve automatically the type witch inject generic type or not.
        /// </para>
        /// </summary>
        /// <param name="service">The instance of <see cref="IServiceCollection"/>.</param>
        /// <param name="lifetime">The service lifetime to be injected.</param>
        /// <param name="implementationType">The implementation type to be injected.</param>
        /// <param name="serviceType">The service type to be injected.</param>
        public static void AddServiceDescriptor(this IServiceCollection service, ServiceLifetime lifetime, Type implementationType, Type serviceType = null)
        {
            if (serviceType == null)
            {
                serviceType = implementationType;//self inject for instance.
            }

            implementationType = ResolveGenericType(implementationType);
            serviceType = ResolveGenericType(serviceType);

            var serviceDescriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);

            service.Add(serviceDescriptor);
        }

        /// <summary>
        /// Resolve the generic type for type of instance.
        /// </summary>
        /// <param name="instanceType">The instance type to be resolve.</param>
        /// <returns>If the instance type not a generic type, return the instance type; or return the generic type.</returns>
        static Type ResolveGenericType(Type instanceType)
        {
            if (!instanceType.IsGenericType)
            {
                return instanceType;
            }

            if (instanceType.IsGenericTypeDefinition)
            {
                var args = instanceType.GetGenericArguments();
                return instanceType.MakeGenericType(args);
            }

            return instanceType.GetGenericTypeDefinition();
        }
    }


}
