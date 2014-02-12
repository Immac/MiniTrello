using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcklenAvenue.Data;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using MiniTrello.Domain.Entities;

namespace MiniTrello.Data
{
    public class MappingScheme : IDatabaseMappingScheme<MappingConfiguration>
    {
        public Action<MappingConfiguration> Mappings
        {
            get
            {
                AutoPersistenceModel autoPersistenceModel = AutoMap.Assemblies(typeof(IEntity).Assembly)
                                                                   .Where(t => typeof(IEntity).IsAssignableFrom(t))
                    // .UseOverridesFromAssemblyOf<AccountOverride>()
                                                                   .Conventions.Add(DefaultCascade.All());

                return x => x.AutoMappings.Add(autoPersistenceModel);
            }
        }
    }
}
