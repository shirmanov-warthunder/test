using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Castle.Windsor;

namespace Test.Web.Infrastructure.Plumbing
{
    public class WindsorDependencyResolver : IDependencyResolver, IDisposable
    {
        private IWindsorContainer container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return this.container.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return (IEnumerable<object>)this.container.ResolveAll(serviceType);
            }
            catch
            {
                return new List<object>();
            }
        }

        public void Dispose()
        {
            this.container.Dispose();
        }
    }
}