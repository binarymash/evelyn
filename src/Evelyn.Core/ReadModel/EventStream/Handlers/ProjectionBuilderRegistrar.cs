namespace Evelyn.Core.ReadModel.EventStream.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.Projections;

    public class ProjectionBuilderRegistrar : IProjectionBuilderRegistrar
    {
        private readonly IServiceProvider _serviceLocator;
        private readonly Dictionary<Type, ProjectionBuildersByEventType> _projectionBuildersByEventType = new Dictionary<Type, ProjectionBuildersByEventType>();

        public ProjectionBuilderRegistrar(IServiceProvider serviceLocator)
        {
            _serviceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
        }

        public void Register(Type eventStreamHandlerType, IEnumerable<Type> projectionBuilderTypes)
        {
            var projectionBuilderDefinitions = projectionBuilderTypes
                .Select(t =>
                new
                {
                    ProjectionBuilderType = t,
                    Interfaces = ResolveProjectionBuilderInterfaces(t)
                })
                .Where(e =>
                    e.Interfaces != null &&
                    e.Interfaces.Any() &&
                    !e.ProjectionBuilderType.GetTypeInfo().IsAbstract);

            foreach (var projectionBuilderDefinition in projectionBuilderDefinitions)
            {
                foreach (var eventHandlerType in projectionBuilderDefinition.Interfaces)
                {
                    InvokeRegistrar(eventStreamHandlerType, projectionBuilderDefinition.ProjectionBuilderType, eventHandlerType);
                }
            }
        }

        public ProjectionBuildersByEventType Get(Type eventStreamHandlerType)
        {
            return _projectionBuildersByEventType[eventStreamHandlerType] ?? ProjectionBuildersByEventType.Null;
        }

        private static IEnumerable<Type> ResolveProjectionBuilderInterfaces(Type projectionBuilderType)
        {
            return projectionBuilderType
                .GetInterfaces()
                .Where(i =>
                    i.GetTypeInfo().IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IBuildProjectionsFrom<>));
        }

        private static string GetImplementationNameOfInterfaceMethod(Type implementation, Type eventHandlerType, string methodName, params Type[] argtypes)
        {
#if NET452 || NETSTANDARD2_0
            var map = implementation.GetInterfaceMap(eventHandlerType);
            var method = map.InterfaceMethods.Single(x =>
                x.Name == methodName && x.GetParameters().Select(p => p.ParameterType).SequenceEqual(argtypes));
            var index = Array.IndexOf(map.InterfaceMethods, method);
            return map.TargetMethods[index].Name;
#else
            return methodName;
#endif
        }

        private void InvokeRegistrar(Type eventStreamHandlerType, Type projectionBuilderType, Type eventHandlerType)
        {
            var eventType = eventHandlerType.GetGenericArguments()[0];

            var registerExecutorMethod = this
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(mi => mi.Name == "Register")
                .Where(mi => mi.IsGenericMethod)
                .Where(mi => mi.GetGenericArguments().Length == 1)
                .Single(mi => mi.GetParameters().Length == 2)
                .MakeGenericMethod(eventType);

            var eventHandlerMethodName = GetImplementationNameOfInterfaceMethod(projectionBuilderType, eventHandlerType, "Handle", eventType, typeof(CancellationToken));

            Func<object, CancellationToken, Task> eventHandlerMethodInvocation = (@event, token) =>
            {
                var projectionBuilder = _serviceLocator.GetService(projectionBuilderType) ?? throw new Exception(projectionBuilderType.Name);
                return (Task)projectionBuilder.Invoke(eventHandlerMethodName, @event, token) ?? throw new Exception(projectionBuilderType.Name);
            };

            registerExecutorMethod.Invoke(this, new object[] { eventStreamHandlerType, eventHandlerMethodInvocation });
        }

        private void Register<TEvent>(Type eventStreamHandlerType, Func<TEvent, CancellationToken, Task> eventHandlerMethodInvocation)
            where TEvent : class, IEvent
        {
            if (!_projectionBuildersByEventType.TryGetValue(eventStreamHandlerType, out var projectionBuildersForEventStreamHandler))
            {
                projectionBuildersForEventStreamHandler = new ProjectionBuildersByEventType();
                _projectionBuildersByEventType.Add(eventStreamHandlerType, projectionBuildersForEventStreamHandler);
            }

            if (!projectionBuildersForEventStreamHandler.TryGetValue(typeof(TEvent), out var projectionBuildersForEvent))
            {
                projectionBuildersForEvent = new List<Func<IEvent, CancellationToken, Task>>();
                projectionBuildersForEventStreamHandler.Add(typeof(TEvent), projectionBuildersForEvent);
            }

            projectionBuildersForEvent.Add((@event, stoppingToken) => eventHandlerMethodInvocation((TEvent)@event, stoppingToken));
        }
    }
}
