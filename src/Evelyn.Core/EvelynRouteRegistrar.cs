namespace Evelyn.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using CQRSlite.Routing;

    public class EvelynRouteRegistrar : RouteRegistrar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvelynRouteRegistrar"/> class.
        /// </summary>
        /// <param name="serviceLocator">Service locator that can resolve all handlers</param>
        public EvelynRouteRegistrar(IServiceProvider serviceLocator)
            : base(serviceLocator)
        {
        }

        protected IServiceProvider ServiceLocator => typeof(RouteRegistrar).GetField("_serviceLocator", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as IServiceProvider;

        public void RegisterHandlers(params Type[] handlerTypes)
        {
            var registrar = (IHandlerRegistrar)ServiceLocator.GetService(typeof(IHandlerRegistrar));

            var executorTypes = handlerTypes
                .Select(t => new { Type = t, Interfaces = ResolveMessageHandlerInterface(t) })
                .Where(e => e.Interfaces != null && e.Interfaces.Any() && !e.Type.GetTypeInfo().IsAbstract);

            foreach (var executorType in executorTypes)
            {
                foreach (var @interface in executorType.Interfaces)
                {
                    InvokeHandler(@interface, registrar, executorType.Type);
                }
            }
        }

        protected IEnumerable<Type> ResolveMessageHandlerInterface(Type type)
        {
            return (IEnumerable<Type>)typeof(RouteRegistrar)
                .GetMethod("ResolveMessageHandlerInterface", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { type });
        }

        protected void InvokeHandler(Type @interface, IHandlerRegistrar registrar, Type executorType)
        {
            typeof(RouteRegistrar)
                .GetMethod("InvokeHandler", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(this, new object[] { @interface, registrar, executorType });
        }
    }
}