using Autofac;

namespace MiniTrello.Infrastructure
{
    public class ConfigureCache : IBootstrapperTask
    {
        readonly ContainerBuilder _containerBuilder;

        public ConfigureCache(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        public void Run()
        {
            _containerBuilder.RegisterType<AspNetCacheProvider>().As<ICacheProvider>();
        }
    }
}