using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInjectionConteiner.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInjectionConteinerTests;

namespace DependencyInjectionConteiner.Container.Tests
{
    [TestClass()]
    public class DependencyProviderTests
    {


        [TestMethod()]
        public void ResolveTestPlainInjection()
        {
            var conf = new DependenciesConfiguration();
            conf.Register<IService, ServiceImpl1>();
            conf.Register<IService, ServiceImpl2>();
            conf.Register<IService, ServiceImpl3>();
            conf.Register<IRepository, RepositoryImpl>();
            conf.Register<IRepository, RepositoryImpl2>();
            var provider = new DependencyProvider(conf);
            var result = provider.Resolve<IService>();

            Assert.IsTrue(result is ServiceImpl1);
        }

        [TestMethod()]
        public void ResolveTestSingletone()
        {
            var conf = new DependenciesConfiguration();
            conf.Register<IService, ServiceImpl1>(LifeCycle.SINGLETONE);
            conf.Register<IService, ServiceImpl2>(LifeCycle.SINGLETONE);
            conf.Register<IService, ServiceImpl3>(LifeCycle.SINGLETONE);
            conf.Register<IRepository, RepositoryImpl>();
            conf.Register<IRepository, RepositoryImpl2>();
            var provider = new DependencyProvider(conf);
            var result1 = provider.Resolve<IService>();
            var result2 = provider.Resolve<IService>();

            Assert.AreEqual(result1,result2);
        }

        [TestMethod()]
        public void ResolveTestPerInj()
        {
            var conf = new DependenciesConfiguration();
            conf.Register<IService, ServiceImpl1>(LifeCycle.PER_DEPENDENCY);
            conf.Register<IService, ServiceImpl2>(LifeCycle.PER_DEPENDENCY);
            conf.Register<IService, ServiceImpl3>(LifeCycle.PER_DEPENDENCY);
            conf.Register<IRepository, RepositoryImpl>();
            conf.Register<IRepository, RepositoryImpl2>();
            var provider = new DependencyProvider(conf);
            var result1 = provider.Resolve<IService>();
            var result2 = provider.Resolve<IService>();

            Assert.AreNotEqual(result1, result2);
        }

        [TestMethod()]
        public void ResolveTestNamedInjection()
        {
            var conf = new DependenciesConfiguration();
            conf.Register<IService, ServiceImpl1>(LifeCycle.SINGLETONE,1);
            conf.Register<IService, ServiceImpl2>(LifeCycle.PER_DEPENDENCY, 2);
            conf.Register<IService, ServiceImpl3>(LifeCycle.PER_DEPENDENCY,3);
            conf.Register<IRepository, RepositoryImpl>();
            conf.Register<IRepository, RepositoryImpl2>();
            var provider = new DependencyProvider(conf);
            var result = provider.Resolve<IService>(3);

            Assert.IsTrue(result is ServiceImpl3);
        }

        [TestMethod()]
        public void ResolveTestGeneric()
        {
            var conf = new DependenciesConfiguration();
            conf.Register<IService, ServiceImpl1>();
            conf.Register<IRepository, RepositoryImpl>();
            conf.Register<IRepository, RepositoryImpl2>();
            conf.Register<IRepository2, RepositoryImpl2>();
            conf.Register<IService<IRepository>, GenericServiceImpl<IRepository>>();
            var provider = new DependencyProvider(conf);
            var result = provider.Resolve<IService<IRepository>>();
            Assert.IsTrue(result is GenericServiceImpl<IRepository>);
            Assert.IsTrue(result.Repository is RepositoryImpl);
        }

        [TestMethod()]
        public void ResolveTestGenericAndNotGeneric()
        {
            var conf = new DependenciesConfiguration();
            conf.Register<IService, ServiceImpl1>();
            conf.Register<IRepository, RepositoryImpl>();
            conf.Register<IRepository, RepositoryImpl2>();
            conf.Register<IRepository2, RepositoryImpl2>();
            conf.Register<IService<IService>, NotGenericService>();
            var provider = new DependencyProvider(conf);
            var result = provider.Resolve<IService<IService>>();
            Assert.IsTrue(result is GenericServiceImpl<IService>);
            Assert.IsTrue(result.Repository is ServiceImpl1);
        }

        [TestMethod()]
        public void ResolveTestOpenGeneric()
        {
            var conf = new DependenciesConfiguration();
            conf.Register<IService, ServiceImpl1>();
            conf.Register<IRepository, RepositoryImpl>();
            conf.Register<IRepository, RepositoryImpl2>();
            conf.Register<IRepository2, RepositoryImpl2>();
            conf.Register(typeof(IService<>), typeof(GenericServiceImpl<>));
            var provider = new DependencyProvider(conf);
            var result = provider.Resolve<IService<IRepository>>();

            Assert.IsTrue(result is GenericServiceImpl<RepositoryImpl2>);
        }


        [TestMethod()] 
        public void ResolveAllTest()
        {
            Assert.Fail();
        }
    }
}