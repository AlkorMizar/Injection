using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionConteiner.Container
{
    public class DependencyProvider
    {
        ConcurrentDictionary<Type, Object> singletones;
        private DependenciesConfiguration configuration;
        public DependencyProvider(DependenciesConfiguration configuration) {
            this.configuration = configuration;
            singletones = new ConcurrentDictionary<Type, object>();
        }

        public T Resolve<T>(Object name=null)
            where T:class
        {
            return (T)Resolve(typeof(T),name);
        }


        public IEnumerable<T> ResolveAll<T>()
            where T : class
        {
            return (IEnumerable<T>)ResolveAll(typeof(T));
        }

        private IEnumerable<object> ResolveAll(Type _interface)
        {
            var impls = configuration.GetImplementations(_interface);
            if (impls!=null)
            {
                var collection = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(_interface));

                foreach (var impl in impls)
                {
                    collection.Add(createImplementation(impl, _interface));
                }

                return (IEnumerable<object>)collection;
            }

            return null;
        }

        private Object Resolve(Type _interface,Object name=null)
        {
            var impls = configuration.GetImplementations(_interface);
            if (impls != null)
            {
                var impl = from _impl in impls
                           where _impl.NameCheck(name)
                           select _impl;
                return createImplementation(impl.First(), _interface);

            }
            else 
            {
                if (_interface.IsGenericType && _interface.GenericTypeArguments.Length!=0)
                {
                    var @int = _interface.GetGenericTypeDefinition();
                    impls = configuration.GetImplementations(@int);
                    if (impls != null)
                    {
                        var impl = from _impl in impls
                                   where _impl.NameCheck(name)
                                   select _impl;
                        return createImplementation(impl.First(), _interface);

                    }
                }

            }
            return null;
        }

        private object createImplementation(ImplContext impl,Type _interface) {
            if (impl.LCycle == LifeCycle.SINGLETONE)
            {
                if (!singletones.ContainsKey(impl.TImplementation))
                {
                    while (!singletones.TryAdd(impl.TImplementation, CreateInstance(impl.TImplementation,_interface))) {}
                    
                }
                return singletones.GetValueOrDefault(impl.TImplementation);
            }
            else
            {
                return CreateInstance(impl.TImplementation,_interface);

            }
        }

        private object CreateInstance(Type implementation, Type _interface)
        {
            if (   _interface.IsGenericType
                && _interface.GenericTypeArguments.Length != 0
                && implementation.GenericTypeArguments.Length == 0) {

                implementation = implementation.MakeGenericType(_interface.GenericTypeArguments);
            }
            var constructors = ChooseConstructors(implementation).ToList();
            if (constructors.Count == 0) throw new ArgumentException($"{implementation} has no injectable constructor");
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var arguments = ProvideParameters(parameters);
                    return constructor.Invoke(arguments.ToArray());
                }
                catch (Exception e) { };
                
            }

            throw new Exception($"Can't create instance of {implementation}");
        }

        private IEnumerable<object> ProvideParameters(IEnumerable<ParameterInfo> parameters)
        {
            List<Object> arguments = new List<object>();
            foreach (var param in parameters)
            {
                arguments.Add(Resolve(param.ParameterType));
            }
            return arguments;
        }

        private IEnumerable<ConstructorInfo> ChooseConstructors(Type type)
        {
            return type.GetConstructors()
                .Where(HasConstructedParameters);
        }

        private bool HasConstructedParameters(ConstructorInfo constructor)
        {
            return constructor.GetParameters()
                .All(IsParameterConstructable);
        }

        private bool IsParameterConstructable(ParameterInfo parameter)
        {
            var parameterType = parameter.GetType();
            return parameterType.IsClass;
        }
       
    }
}

