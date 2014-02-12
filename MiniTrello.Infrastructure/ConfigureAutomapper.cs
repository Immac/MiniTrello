using AutoMapper;
using MiTrello.Domain.Entities;

namespace MiniTrello.Infrastructure
{
    public class ConfigureAutomapper : IBootstrapperTask
    {
        #region IBootstrapperTask Members

        public void Run()
        {
            //automappings go here
            //Ex: Mapper.CreateMap<SomeType, SomeOtherType>().ReverseMap();
          //  Mapper.CreateMap<Account, AccountLoginModel>().ReverseMap();
          
        }

        #endregion
    }
}