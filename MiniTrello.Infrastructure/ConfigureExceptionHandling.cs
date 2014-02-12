using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;
using Autofac;
using log4net.Config;

namespace MiniTrello.Infrastructure
{
    public class ConfigureExceptionHandling : IBootstrapperTask
    {
        readonly ContainerBuilder _containerBuilder;
        readonly List<ExceptionFilterAttribute> _exceptionFilters = new List<ExceptionFilterAttribute>();

        public ConfigureExceptionHandling(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
        }

        #region IBootstrapperTask Members

        public ConfigureExceptionHandling WithWebApiExceptionFilter(ExceptionFilterAttribute filter)
        {
            _exceptionFilters.Add(filter);
            return this;
        }

        public void Run()
        {
            XmlConfigurator.Configure();
            _exceptionFilters.Add(new LoggingExceptionFilter());
            _exceptionFilters.ForEach(x => GlobalConfiguration.Configuration.Filters.Add(x));
        }

        #endregion
    }
}