using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionConteinerTests
{

    interface IService { }
    class ServiceImpl1 : IService
    {
        public ServiceImpl1(IRepository repository) // ServiceImpl зависит от IRepository
        {
        }
    }

    class ServiceImpl2 : IService
    {
        public ServiceImpl2(IRepository repository) // ServiceImpl зависит от IRepository
        {
        }
    }

    class ServiceImpl3 : IService
    {
        public ServiceImpl3(IRepository repository) // ServiceImpl зависит от IRepository
        {
        }
    }

    interface IRepository { }

    interface IRepository2 : IRepository { }

    class RepositoryImpl : IRepository
    {
        public RepositoryImpl() { } // может иметь свои зависимости, опустим для простоты
    }
    class RepositoryImpl2 : IRepository2
    {
        public RepositoryImpl2() { } // может иметь свои зависимости, опустим для простоты
    }

    interface IService<TRepository>
    {
        public TRepository Repository { get; set; }
    }

    class NotGenericService : IService<IService> {
        public NotGenericService(IService repository) {
            Repository = repository;
        }

        public IService Repository { get; set; }
    }
    class GenericServiceImpl<TRepository> : IService<TRepository>
    {
        
        public GenericServiceImpl(TRepository repository)
        {
            Repository = repository;
        }

        public TRepository Repository { get; set; }
    }
}
