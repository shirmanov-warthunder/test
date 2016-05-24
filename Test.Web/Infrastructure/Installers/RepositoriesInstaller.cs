using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Test.BL;
using Test.BL.Providers;
using Test.Core.Interfaces;
using Test.Data;

namespace Test.Web.Infrastructure.Installers
{
    public class RepositoriesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For(typeof(IDataContext)).ImplementedBy<DataContext>().LifeStyle.PerWebRequest,
                Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)).LifeStyle.PerWebRequest,
                Component.For<ITestProvider>().ImplementedBy<TestProvider>().LifeStyle.PerWebRequest);
            //Component.For<IRepository<User>>().ImplementedBy<Repository<User>>().LifeStyle.PerWebRequest
        }
    }
}