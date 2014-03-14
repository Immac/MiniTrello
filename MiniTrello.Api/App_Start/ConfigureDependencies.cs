using Autofac;
using AutoMapper;
using MiniTrello.Api.Controllers.Helpers;
using MiniTrello.Data;
using MiniTrello.Domain.DataObjects;
using MiniTrello.Domain.Services;
using MiniTrello.Infrastructure;

namespace MiniTrello.Api
{
    public class ConfigureDependencies : IBootstrapperTask
    {
        readonly ContainerBuilder _containerBuilder;

        public ConfigureDependencies(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        public void Run()
        {
            _containerBuilder.Register(c => Mapper.Engine).As<IMappingEngine>();
            _containerBuilder.RegisterType<ReadOnlyRepository>().As<IReadOnlyRepository>();
            _containerBuilder.RegisterType<WriteOnlyRepository>().As<IWriteOnlyRepository>();
            _containerBuilder.RegisterType<ValidationHelper>().As<IRegisterValidator<AccountRegisterModel>>();
        }
    }
}