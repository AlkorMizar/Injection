using DependencyInjectionConteiner.Container;
using System;
using System.Collections.Generic;

namespace DependencyInjectionConteiner
{
    class Program
    {
        static void Main(string[] args)
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<IService, ServiceImpl>();
            dependencies.Register<IRepository, RepositoryImpl>();
            dependencies.Register<IRepository2, RepositoryImpl2>();
            dependencies.Register(typeof(IService<>),typeof(ServiceImpl<>));
            dependencies.Register<IService<IRepository>, ServiceImpl<IRepository>>();

            var provider = new DependencyProvider(dependencies);

            var service1 = provider.Resolve<IService>();
            var r = provider.Resolve<IService<IRepository2>>();

        }
    }

    interface IService { }
    class ServiceImpl : IService
    {
        public ServiceImpl(IRepository repository) // ServiceImpl зависит от IRepository
        {
        }
    }

    interface IRepository { }

    interface IRepository2: IRepository { }

    class RepositoryImpl : IRepository
    {
        public RepositoryImpl() { } // может иметь свои зависимости, опустим для простоты
    }
    class RepositoryImpl2 : IRepository2
    {
        public RepositoryImpl2() { } // может иметь свои зависимости, опустим для простоты
    }

    interface IService<TRepository>
    {}

    class ServiceImpl<TRepository> : IService<TRepository>
    {
        public ServiceImpl(TRepository repository)
        {
        }
    }

}

