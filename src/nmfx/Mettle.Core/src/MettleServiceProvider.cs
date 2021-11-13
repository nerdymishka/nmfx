using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mettle.Abstractions;
using Mettle.Sdk;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mettle
{
    /// <summary>
    /// The default <see cref="IServiceProvider" /> implementation that ships
    /// with Mettle.
    /// </summary>
    public class MettleServiceProvider : IServiceProvider, IDisposable
    {
        private readonly ConcurrentDictionary<Type, Func<IServiceProvider, object?>> factories = new();

        private readonly ScopedLifetime scopedLifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="MettleServiceProvider"/> class.
        /// </summary>
        public MettleServiceProvider()
        {
            this.scopedLifetime = new ScopedLifetime();
            this.factories.TryAdd(typeof(IAssert), _ => AssertImpl.Instance);
            this.factories.TryAdd(typeof(ScopedLifetime), _ => this.scopedLifetime);
            this.factories.TryAdd(typeof(IServiceProviderLifetimeFactory), _ => new SimpleServiceProviderLifetimeFactory(this));
            this.factories.TryAdd(typeof(IServiceProviderLifetime), _ => new SimpleServiceProviderLifetime(this));
            this.AddScoped(typeof(ITestOutputHelperContainer), _ => new TestOutputHelperContainer());
            this.AddScoped(typeof(ITestOutputHelper), _ => new TestOutputHelper());
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        /// <summary>
        /// Gets an instance of the specified service type.
        /// </summary>
        /// <param name="serviceType">The clr type that should be injected.</param>
        /// <returns>
        /// The instance of the service type; otherwise, <see langword="null" />.
        /// </returns>
        public object? GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (this.factories.TryGetValue(serviceType, out var factory))
                return factory(this);

            if (!serviceType.IsValueType)
            {
                var ctors = serviceType.GetConstructors();
                if (ctors.Length > 1)
                    return null;

                var ctor = ctors[0];
                var parameters = ctor.GetParameters();

                if (parameters.Length == 0)
                {
                    this.factories.TryAdd(serviceType, (_) => Activator.CreateInstance(serviceType));
                    return Activator.CreateInstance(serviceType);
                }

                this.factories.TryAdd(serviceType, (s) =>
                {
                    var args = new List<object>();
                    foreach (var p in parameters)
                    {
                        args.Add(s.GetService(p.ParameterType));
                    }

                    return Activator.CreateInstance(serviceType, args.ToArray());
                });

                var args = new List<object?>();
                foreach (var p in parameters)
                {
                    args.Add(this.GetService(p.ParameterType));
                }

                return Activator.CreateInstance(serviceType, args.ToArray());
            }

            return Activator.CreateInstance(serviceType);
        }

        /// <summary>
        /// Adds a single instance of the specified type.
        /// </summary>
        /// <param name="type">The clr type that should be injected.</param>
        /// <param name="instance">The live object instance.</param>
        public void AddSingleton(Type type, object instance)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            this.scopedLifetime.SetState(type, instance);
            this.factories.TryAdd(type, _ => instance);
        }

        /// <summary>
        /// Adds a single instance of the specified type.
        /// </summary>
        /// <param name="type">The clr type that should be injected.</param>
        /// <param name="activator">The factory method used to create the instance.</param>
        public void AddSingleton(Type type, Func<IServiceProvider, object> activator)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            this.AddScoped(type, activator);
        }

        /// <summary>
        /// Adds a service type that will be created once per lifetime scope
        /// when <see cref="GetService(Type)" /> is called.
        /// </summary>
        /// <param name="type">The clr type that should be injected.</param>
        /// <param name="activator">The factory method used to create the instance.</param>
        public void AddScoped(Type type, Func<IServiceProvider, object?> activator)
        {
            this.factories.TryAdd(type, s =>
            {
                var sl = s.GetService(typeof(ScopedLifetime));
                if (sl == null)
                    return null;

                var scope = (ScopedLifetime)sl;
                if (scope.ContainsKey(type))
                    return scope.GetState(type);

                var r = activator(s);
                scope.SetState(type, r);
                return r;
            });
        }

        /// <summary>
        /// Adds a service type that will be created each time <see cref="GetService(Type)" />
        /// is called.
        /// </summary>
        /// <param name="type">The clr type that should be injected.</param>
        public void AddTransient(Type type)
        {
            this.factories.TryAdd(type, _ => Activator.CreateInstance(type));
        }

        /// <summary>
        /// Adds a service type that will be created each time <see cref="GetService(Type)" />
        /// is called.
        /// </summary>
        /// <param name="type">The clr type that should be injected.</param>
        /// <param name="activator">The factory method used to create the instance.</param>
        public void AddTransient(Type type, Func<IServiceProvider, object> activator)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (activator == null)
                throw new ArgumentNullException(nameof(activator));

            this.factories.TryAdd(type, activator);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.scopedLifetime == null)
                    return;

                foreach (var disposable in this.scopedLifetime.GetDisposables())
                    disposable.Dispose();

                this.scopedLifetime?.Clear();
                this.factories?.Clear();
            }
        }

        public class ScopedLifetime
        {
            private readonly ConcurrentDictionary<Type, object?> state = new();

            public bool ContainsKey(Type type)
            {
                return this.state.ContainsKey(type);
            }

            public void SetState(Type type, object? instance)
            {
                this.state[type] = instance;
            }

            public object? GetState(Type type)
            {
                this.state.TryGetValue(type, out var instance);
                return instance;
            }

            public void Clear()
            {
                this.state.Clear();
            }

            public IEnumerable<IDisposable> GetDisposables()
            {
                var list = new List<IDisposable>();
                foreach (var kv in this.state)
                {
                    if (kv.Value is IDisposable disposable)
                        list.Add(disposable);
                }

                return list;
            }
        }

        private class SimpleServiceProviderLifetimeFactory : IServiceProviderLifetimeFactory
        {
            private readonly MettleServiceProvider provider;

            public SimpleServiceProviderLifetimeFactory(MettleServiceProvider provider)
            {
                this.provider = provider;
            }

            public IServiceProviderLifetime CreateLifetime()
            {
                return new SimpleServiceProviderLifetime(this.provider);
            }
        }

        // this is a private class, so fully implementing IDispose is overkill.
        private class SimpleServiceProviderLifetime : IServiceProviderLifetime
        {
            private readonly MettleServiceProvider provider;

            public SimpleServiceProviderLifetime(MettleServiceProvider provider)
            {
                var provider2 = new MettleServiceProvider();
                foreach (var kv in provider.factories)
                {
                    if (kv.Key == typeof(ScopedLifetime))
                        continue;

                    if (provider2.factories.ContainsKey(kv.Key))
                        continue;

                    provider2.factories.TryAdd(kv.Key, kv.Value);
                }

                this.provider = provider2;
            }

            public IServiceProvider ServiceProvider => this.provider;

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                this.provider.Dispose();
            }
        }
    }
}