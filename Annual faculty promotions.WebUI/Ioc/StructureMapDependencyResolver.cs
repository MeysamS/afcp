using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StructureMap;
using Microsoft.AspNet.SignalR;

namespace Annual_faculty_promotions.WebUI.Ioc
{
    public class StructureMapDependencyResolver:DefaultDependencyResolver
    {
        private readonly IContainer _container;
        public StructureMapDependencyResolver(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            _container = container;
        }
        public override object GetService(Type serviceType)
        {
            if (serviceType == null)
                return null;

            var service = _container.TryGetInstance(serviceType) ?? base.GetService(serviceType);
            if (service != null) return service;

            return (!serviceType.IsAbstract && !serviceType.IsInterface && serviceType.IsClass)
                ? _container.GetInstance(serviceType)
                : _container.TryGetInstance(serviceType);
        }
        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.GetAllInstances(serviceType).Cast<object>().Concat(base.GetServices(serviceType));
        }
    }
}