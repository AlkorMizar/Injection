using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjectionConteiner.Container
{
    public class DependenciesConfiguration
    {
        // dictionaty of TDependency and their IEnumerable of TImplementation
        Dictionary<Type, HashSet<ImplContext>> configuration;
        public DependenciesConfiguration() {
            configuration = new Dictionary<Type, HashSet<ImplContext>>();
        }

        public IEnumerable<ImplContext> GetImplementations(Type dependency) {
            return configuration.GetValueOrDefault(dependency);
        }

        public void Register<TDependency, TImplementation>(LifeCycle lc=LifeCycle.PER_DEPENDENCY,Object name=null) 
            where TDependency : class
            where TImplementation : class, TDependency
        {
            Register(typeof(TDependency),typeof(TImplementation),lc,name);
        }

        public void Register(Type tDependeny, Type tImplementation, LifeCycle lc=LifeCycle.PER_DEPENDENCY,Object name=null)
        {
            if (tDependeny == null)
            {
                throw new ArgumentNullException(nameof(tDependeny));
            }
            if (tImplementation == null)
            {
                throw new ArgumentNullException(nameof(tImplementation));
            }
            if (!tImplementation.IsClass) 
            {
                throw new ArgumentException($"{tImplementation} must be a reference type");
            }
            if (tImplementation.IsAbstract || tImplementation.IsInterface)
            {
                throw new ArgumentException($"{tImplementation} must be constructable");
            }

            if (!tDependeny.IsAssignableFrom(tImplementation)) {
                if (!tDependeny.IsGenericTypeDefinition || !tDependeny.IsGenericTypeDefinition) {
                    if (!IsAssignableFromGeneric(tImplementation, tDependeny)) {
                        throw new ArgumentException($"{tImplementation} must be non abstract and must subtype of {tDependeny}");
                    }
                }

            }
                

            if (!configuration.ContainsKey(tDependeny))
            {
                var newL = new HashSet<ImplContext>();
                configuration.Add(tDependeny, newL);
            }
            configuration.GetValueOrDefault(tDependeny).Add(new ImplContext(tImplementation,lc,name));

        }


        private IEnumerable<Type> GetBaseTypes(Type type)
        {
            for (var baseType = type; baseType != null; baseType = baseType.BaseType)
                yield return baseType;

            var interfaceTypes =
                from Type interfaceType in type.GetInterfaces()
                select interfaceType;

            foreach (var interfaceType in interfaceTypes)
                yield return interfaceType;
        }

        private Type GetTypeDefinition(Type type) =>
            type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        public bool IsAssignableFromGeneric(Type implType, Type interfaceType)
        {
            var baseTypes = GetBaseTypes(GetTypeDefinition(implType));
            return baseTypes
                .Select(GetTypeDefinition)
                .Contains(GetTypeDefinition(interfaceType));
        }
    }

}
