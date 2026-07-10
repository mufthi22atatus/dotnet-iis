using System;
using System.Collections.Generic;
using TaskManager.Data;
using TaskManager.Data.Repositories;
using TaskManager.Services;

namespace TaskManager
{
    /// <summary>
    /// Tiny manual service locator. Keeps the sample dep-light (no Autofac/Unity) but
    /// still gives controllers and background jobs a single place to resolve services.
    /// </summary>
    public static class DependencyConfig
    {
        private static readonly Dictionary<Type, Func<object>> _factories = new Dictionary<Type, Func<object>>();

        public static void Configure()
        {
            // Per-call DbContext factory — controllers/services own their lifetime via `using`.
            Func<AppDbContext> dbFactory = () => new AppDbContext();
            Register<Func<AppDbContext>>(() => dbFactory);

            Register<IPasswordHasher>(() => new PasswordHasher());
            Register<ICacheService>(() => new CacheService());
            Register<IExternalApiClient>(() => new ExternalApiClient());
            Register<IFileStorageService>(() => new FileStorageService());

            Register<IUserRepository>(() => new UserRepository(Resolve<Func<AppDbContext>>()));
            Register<ITaskRepository>(() => new TaskRepository(Resolve<Func<AppDbContext>>()));

            Register<IAuditService>(() => new AuditService(Resolve<Func<AppDbContext>>()));
            Register<INotificationService>(() => new NotificationService());
            Register<IAuthService>(() => new AuthService(
                Resolve<IUserRepository>(),
                Resolve<IPasswordHasher>(),
                Resolve<IAuditService>()));
            Register<ITaskService>(() => new TaskService(
                Resolve<ITaskRepository>(),
                Resolve<IUserRepository>(),
                Resolve<INotificationService>(),
                Resolve<IAuditService>(),
                Resolve<ICacheService>()));

            // Register SqlQueryService and workflow services
            Register<SqlQueryService>(() => new SqlQueryService());
            Register<ITaskWorkflowService>(() => new TaskWorkflowService(
                Resolve<Func<AppDbContext>>(),
                Resolve<SqlQueryService>()));
        }

        public static void Register<T>(Func<object> factory) => _factories[typeof(T)] = factory;

        public static T Resolve<T>()
        {
            if (!_factories.TryGetValue(typeof(T), out var f))
                throw new InvalidOperationException($"No registration for {typeof(T).FullName}");
            return (T)f();
        }
    }
}
