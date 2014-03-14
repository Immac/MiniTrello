using System;
using System.Security.Principal;
using AutoMapper;
using MiniTrello.Api.Controllers;
using MiniTrello.Domain.DataObjects;
using MiniTrello.Domain.Entities;
using MiniTrello.Infrastructure;

namespace MiniTrello.Api
{
    public class ConfigureAutomapper : IBootstrapperTask
    {
        public void Run()
        {
            Mapper.CreateMap<Account, AccountLoginModel>().ReverseMap();
            Mapper.CreateMap<Account, AccountRegisterModel>().ReverseMap();
            Mapper.CreateMap<Board, BoardChangeTitleModel>().ReverseMap();
            Mapper.CreateMap<Board, BoardCreateModel>().ReverseMap();
            Mapper.CreateMap<LaneCreateModel, Lane>().ReverseMap();
            Mapper.CreateMap<CardCreateModel, Card>().ReverseMap();
            Mapper.CreateMap<OrganizationCreateModel, Organization>().ReverseMap();
            Mapper.CreateMap<Board, BoardModel>().ReverseMap();
            Mapper.CreateMap<Board, BoardLogModel>().ReverseMap();
            Mapper.CreateMap<Board, BoardTitleModel>().ReverseMap();
            Mapper.CreateMap<EditedProfileModel, Account>().ReverseMap();
            Mapper.CreateMap<Account, AccountModel>().ReverseMap();
            Mapper.CreateMap<Lane,LaneModel>().ReverseMap();
            Mapper.CreateMap<Card, CardModel>().ReverseMap();
            Mapper.CreateMap<Organization, OrganizationModel>().ReverseMap();
            Mapper.CreateMap<Organization, OrganizationNameModel>().ReverseMap();
            
            //Mapper.CreateMap<DemographicsEntity, DemographicsModel>().ReverseMap();
            //Mapper.CreateMap<IReportEntity, IReportModel>()
            //    .Include<DemographicsEntity, DemographicsModel>();
        }
    }

}