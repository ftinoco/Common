using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCommon.AutoMapper;
using NetCoreCommon.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetCoreCommon.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection DiscoverAndRegisterMappingProfiles<TProfileID>(
            this IServiceCollection services, Assembly assembly, TProfileID defaultProfile, bool usingUniqueProfileAsDefault = true)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly), "You must specify a valid assembly to perform mapping profile search and registration");

            Dictionary<TProfileID, MapperConfigurationExpression> dictionary1 = new();
            foreach (Type type in assembly.DefinedTypes.Where(x => x.IsClass && x.IsSubclassOf(typeof(AutoMapperProfile<TProfileID>))).ToList())
            {
                AutoMapperProfile<TProfileID> instance = Activator.CreateInstance(type) as AutoMapperProfile<TProfileID>;
                if (dictionary1.ContainsKey(instance.ProfileID))
                    instance.ConfigureProfile(dictionary1[instance.ProfileID]);
                else
                {
                    MapperConfigurationExpression configurationExpression = new MapperConfigurationExpression();
                    instance.ConfigureProfile(configurationExpression);
                    dictionary1.Add(instance.ProfileID, configurationExpression);
                }
            }
            Dictionary<TProfileID, IMapper> dictionary2 = new();

            foreach (KeyValuePair<TProfileID, MapperConfigurationExpression> keyValuePair in dictionary1)
                dictionary2.Add(keyValuePair.Key, new MapperConfiguration(keyValuePair.Value).CreateMapper());

            DefaultAutoMapperManager<TProfileID> autoMapperManager = new(dictionary2, defaultProfile, usingUniqueProfileAsDefault);
            services.AddSingleton((IAutoMapperManager<TProfileID>)autoMapperManager);
            return services;
        }

        public static IServiceCollection DiscoverAndRegisterRepositories(this IServiceCollection services,
            Assembly assembly, Type repositoryInterface, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly), "You must specify an assembly to examine");

            if (!repositoryInterface.IsGenericType || !repositoryInterface.IsInterface)
                throw new ArgumentException($"The data type {repositoryInterface.Name} must be a generic interface");

            foreach (TypeInfo typeInfo in assembly.DefinedTypes.Where(x => !x.IsInterface && !x.IsAbstract && !x.IsNested).ToList())
            {
                List<Type> list = typeInfo.ImplementedInterfaces.Where(x => !x.IsGenericType).ToList();
                int num = list.SelectMany(x => x.GetInterfaces()).Where(new Func<Type, bool>(TypeValidator)).Count();

                if (num > 1)
                    throw new AmbiguousMatchException($"The data type \"{ typeInfo.Name }\" implements more than one interface of the type \"{ repositoryInterface.Name}\". The discovery and registration process automatically adds only the types that implement an interface at a time of type \"{ repositoryInterface.Name}\". Please specify manually in the service configuration the interface and its implementation to take into account for the injection of dependencies.");

                if (num == 1)
                {
                    foreach (Type serviceType in list)
                    {
                        if (((IEnumerable<Type>)serviceType.GetInterfaces()).Where(new Func<Type, bool>(TypeValidator)).Any())
                        {
                            services.Add(new ServiceDescriptor(serviceType, typeInfo, lifetime));
                            break;
                        }
                    }
                }
            }
            return services;

            bool TypeValidator(Type type) => type.IsGenericType ? type.GetGenericTypeDefinition().FullName == repositoryInterface.FullName : type.FullName == repositoryInterface.FullName;
        }
    }
}
