using Autofac;
using AutoMapper;
using MiniTrello.Data;
using MiTrello.Domain.Services;

namespace MiniTrello.Infrastructure
{
    public class ConfigureDependencies : IBootstrapperTask
    {
        readonly ContainerBuilder _containerBuilder;

        public ConfigureDependencies(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        #region IBootstrapperTask Members

        public void Run()
        {
            _containerBuilder.Register(c => Mapper.Engine).As<IMappingEngine>();
            _containerBuilder.RegisterType<ReadOnlyRepository>().As<IReadOnlyRepository>();
            _containerBuilder.RegisterType<WriteOnlyRepository>().As<IWriteOnlyRepository>();
        }

        #endregion
    }
}