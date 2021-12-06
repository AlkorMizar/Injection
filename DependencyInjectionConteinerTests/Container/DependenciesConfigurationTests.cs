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
    public class DependenciesConfigurationTests
    {
        [TestMethod()]
        public void GetImplementationsTest()
        {

            var conf = new DependenciesConfiguration();
            conf.Register<IService, ServiceImpl1>();
            conf.Register<IRepository, RepositoryImpl>();
            conf.Register<IRepository2, RepositoryImpl2>();
            var res = conf.GetImplementations(typeof(IService));
            Assert.IsTrue(res != null);
            var resArr = res.ToArray();
            Assert.IsTrue(resArr.Length == 1 && resArr[0].TImplementation == typeof(ServiceImpl1)
                          && resArr[0].LCycle == LifeCycle.PER_DEPENDENCY
                          && resArr[0].Name == null);
        }

        [TestMethod()]
        public void RegisterTestOneWithDefault()
        {
            var conf = new DependenciesConfiguration();
            conf.Register<IService, ServiceImpl1>();
            var res = conf.GetImplementations(typeof(IService));
            Assert.IsTrue(res!=null);
            var resArr = res.ToArray();
            Assert.IsTrue(resArr.Length == 1 && resArr[0].TImplementation == typeof(ServiceImpl1)
                          && resArr[0].LCycle==LifeCycle.PER_DEPENDENCY
                          && resArr[0].Name==null);
        }

        [TestMethod()]
        public void RegisterTestOneWithSpecifiedLifeTime()
        {
            var conf = new DependenciesConfiguration();
            conf.Register<IService, ServiceImpl2>(LifeCycle.SINGLETONE);
            var res = conf.GetImplementations(typeof(IService));
            Assert.IsTrue(res != null);
            var resArr = res.ToArray();
            Assert.IsTrue(resArr.Length == 1 && resArr[0].TImplementation == typeof(ServiceImpl2)
                          && resArr[0].LCycle == LifeCycle.SINGLETONE
                          && resArr[0].Name == null);
        }

        [TestMethod()]
        public void RegisterTestOneWithSpecifiedLiftimeName()
        {
            Object[] names = { 1, "Garry", LifeCycle.SINGLETONE,null};
            foreach (var name in names)
            {
                var conf = new DependenciesConfiguration();
                conf.Register<IService, ServiceImpl2>(LifeCycle.PER_DEPENDENCY, name);
                var res = conf.GetImplementations(typeof(IService));
                Assert.IsTrue(res != null);
                var resArr = res.ToArray();
                Assert.IsTrue(resArr.Length == 1 && resArr[0].TImplementation == typeof(ServiceImpl2)
                              && resArr[0].LCycle == LifeCycle.PER_DEPENDENCY
                              && resArr[0].NameCheck(name));
            }
            
        }

        [TestMethod()]
        public void RegisterTestSeveralImpl()
        {
            var conf = new DependenciesConfiguration();
            conf.Register<IService, ServiceImpl1>();
            conf.Register<IService, ServiceImpl2>(LifeCycle.PER_DEPENDENCY, "Garry");
            conf.Register<IService, ServiceImpl3>(LifeCycle.SINGLETONE);
            var res = conf.GetImplementations(typeof(IService));
            Assert.IsTrue(res != null);
            var resArr = res.ToArray();
            Assert.IsTrue(resArr.Length == 3 && resArr[0].TImplementation == typeof(ServiceImpl1)
                          && resArr[1].TImplementation == typeof(ServiceImpl2)
                          && resArr[2].TImplementation == typeof(ServiceImpl3));
        }

        [TestMethod()]
        public void RegisterTestWrong()
        {
            var conf = new DependenciesConfiguration();
            Type[][] input = new Type[][] {
                new Type[]{typeof(IService),typeof(int)},
                new Type[]{typeof(int),typeof(int)},
                new Type[]{typeof(int),typeof(IService) },
                new Type[]{typeof(IService),typeof(RepositoryImpl)},//not assignable
                new Type[]{typeof(IService<>),typeof(ServiceImpl1)}
            };
            for (int i = 0; i < input.Length; i++)
            {
                try
                {
                    conf.Register(input[i][0], input[i][1]);
                    Assert.Fail();
                }
                catch (Exception e)
                {
                }
            }
            
        }
    }
}