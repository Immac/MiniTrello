using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AutoMapper;
using MiniTrello.Api.Controllers.Helpers;
using MiniTrello.Api.Models;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;

namespace MiniTrello.Api.Controllers
{
    public class OrganizationController : ApiController
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IMappingEngine _mappingEngine;

        public OrganizationController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository,
            IMappingEngine mappingEngine)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _mappingEngine = mappingEngine;
        }

        [POST("organizations/create/{token}")]
        public HttpResponseMessage CreateOrganization([FromBody]OrganizationCreateModel model, string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Organization organization = _mappingEngine.Map<OrganizationCreateModel,Organization>(model);
            
            Account myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            Organization newOrganization = _writeOnlyRepository.Create(organization);
            myAccount.AddOrganization(newOrganization);
            _writeOnlyRepository.Update(myAccount);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

    }

    public class OrganizationCreateModel
    {
        public string Name { set; get; }
        public string Description { set; get; }
    }
}