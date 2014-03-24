using System.Linq;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AutoMapper;
using MiniTrello.Api.Controllers.Helpers;
using MiniTrello.Domain.DataObjects;
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
        public GetOrganizationsModel CreateOrganization([FromBody]OrganizationCreateModel model, string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
                return new GetOrganizationsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionDoesNotExistOnThisServer
                };
            if (Security.IsTokenExpired(session))
                return new GetOrganizationsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.SessionHasExpired
                };
            var organization = _mappingEngine.Map<OrganizationCreateModel,Organization>(model);
            
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (accountFromSession == null)
                return new GetOrganizationsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = ErrorStrings.AccountDoesNotExist
                };
            var newOrganization = _writeOnlyRepository.Create(organization);
            accountFromSession.AddOrganization(newOrganization);
            _writeOnlyRepository.Update(accountFromSession);

            var getOrganizations = accountFromSession.Organizations.ToList();
            var getOrganizationsModel = new GetOrganizationsModel();
            foreach (var orga in getOrganizations)
            {
                var organizationNameDescriptionModel =
                    _mappingEngine.Map<Organization, OrganizationNameDescriptionModel>(orga);
                if (!orga.IsArchived)
                    getOrganizationsModel.AddNameDescription(organizationNameDescriptionModel);
            }
            return getOrganizationsModel;
        }

    }

    public class OrganizationCreateModel
    {
        public string Name { set; get; }
        public string Description { set; get; }
    }
}