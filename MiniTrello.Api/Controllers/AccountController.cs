using System;
using System.Linq;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AutoMapper;
using MiniTrello.Api.Controllers.Helpers;
using MiniTrello.Api.CustomExceptions;
using MiniTrello.Domain.DataObjects;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;
using RestSharp;


namespace MiniTrello.Api.Controllers
{
    public class AccountController : ApiController
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IMappingEngine _mappingEngine;

        public AccountController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository,
            IMappingEngine mappingEngine)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _mappingEngine = mappingEngine;
        }

        [POST("login")]
        public AccountAuthenticationModel Login([FromBody] AccountLoginModel model)
        {
            var account = FindCorrespondingAccount(model);
            if (account != null)
            {
                var retrievedSession = RetrieveSession(account);
                if (retrievedSession == null)
                {
                    var token = Security.CreateToken(account, _readOnlyRepository);
                    var sessionDuration = Security.GetTokenLifeSpan(model);
                    var newSession = new Session
                    {
                        SessionAccount = account,
                        Token = token,
                        DateStarted = DateTime.UtcNow,
                        Duration = sessionDuration
                    };
                    var sessionCreated = _writeOnlyRepository.Create(newSession);
                    if (sessionCreated != null) return new AccountAuthenticationModel {AccountName = sessionCreated.SessionAccount.FirstName, Token = token };
                }
                if (retrievedSession != null) return new AccountAuthenticationModel {AccountName = retrievedSession.SessionAccount.FirstName, Token = retrievedSession.Token };
            }
            return new AccountAuthenticationModel
            {
                ErrorCode = 1,
                ErrorMessage = "Invalid username or password",
                Token = "",
                AccountName = ""
            };
        }

       

        [POST("register")]
        public RegisterConfirmationModel Register([FromBody] AccountRegisterModel model)
        {
            var errorMessage = new ValidationHelper().Validate(model);
            if (errorMessage.Length > 0)
                return new RegisterConfirmationModel
                {
                    ErrorCode = 1,
                    ErrorMessage = errorMessage,
                    Message = ""
                };

            var account = _mappingEngine.Map<AccountRegisterModel, Account>(model);
            account.Password = Security.EncryptPassword(account.Password);
            var accountCreated = _writeOnlyRepository.Create(account);
            if (accountCreated == null) 
                return new RegisterConfirmationModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "The Email you are trying to use is already registered.",
                    Message = ""
                };

            SendAccountCreatedMail(accountCreated);
            return new RegisterConfirmationModel
            {
                Message = "Account confirmation message will be sent to " + model.Email
            };
        }

        

        [GET("boards/{token}")]
        public GetBoardsModel GetBoards(string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new GetBoardsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "The session you are trying to reach does not exist on this server."
                };
            }

            if(Security.IsTokenExpired(session))
            {
                return new GetBoardsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "The session you are trying to reach has expired."
                };
            }
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            var boardsList = accountFromSession.Boards.ToList();
            var getBoardsModel = new GetBoardsModel();
            foreach (var board in boardsList)
            {
                var boardModel = _mappingEngine.Map<Board,BoardModel>(board);
                getBoardsModel.AddBoard(boardModel);
            }
            return getBoardsModel;
        }

        [POST("restorepassword")]
        public RestorePasswordModel RestorePassword(PasswordRestoreInputModel inputModel)
        {
            if (RegexUtilities.IsValidEmail(inputModel.Email))
            {
                return new RestorePasswordModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "You have entered an invalid Email."
                };
            }
            var token = Security.CreateRestoreToken(inputModel.Email);
            var correspondingAccount = FindCorrespondingAccount(inputModel.Email);
            if (correspondingAccount == null)
            {
                return new RestorePasswordModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "The account you are trying to reach does not exist on this server."
                };
            }
            var temporarySession = new Session
            {
                IsRestoreSession = true,
                SessionAccount = _readOnlyRepository.First<Account>(account => account.Email == inputModel.Email),
                Token = token,
                DateStarted = DateTime.UtcNow,
                Duration = 15,
            };
            var sessionCreated = _writeOnlyRepository.Create(temporarySession);
            if (sessionCreated == null)
            {
                return new RestorePasswordModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "There was an error while trying to recover your account, please try again later or contact the server administrator"
                };
            }
            SendPasswordRestoreMail(inputModel.Email, token);
            var restorePasswordModel = new RestorePasswordModel
            {
                Message = "Instructions on password reset have been sent to your Email"
            };
            return restorePasswordModel;
        }


        [AcceptVerbs(new[] { "PUT" })]
        [PUT("profile/edit/{token}")]
        public EditedProfileModel EditProfile([FromBody] AccountEditModel accountEditModel, string token)
        {
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                return new EditedProfileModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "The session you are trying to reach does not exist on this server."
                };
            }
            if (Security.IsTokenExpired(session))
            {
                return new EditedProfileModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "The session you are trying to reach has expired."
                };
            }
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            if (accountFromSession == null)
            {
                return new EditedProfileModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "The account you are trying to reach is not available, or does not exist."
                };
            }
            var errorMessage = ValidationHelper.ValidateEditModel(accountEditModel);
            if (errorMessage.Length > 0)
            {
                return new EditedProfileModel
                {
                    ErrorCode = 1,
                    ErrorMessage = errorMessage
                };
            }
            accountFromSession.FirstName = accountEditModel.FirstName;
            accountFromSession.LastName = accountEditModel.LastName;
            accountFromSession.Password = accountEditModel.Password;
            _writeOnlyRepository.Update(accountFromSession);
            return _mappingEngine.Map<Account, EditedProfileModel>(accountFromSession);
        }


        


        private Account FindCorrespondingAccount(AccountLoginModel model)
        {
            var myAES = new SimpleAES();
            var account = _readOnlyRepository.First<Account>(
                account1 => account1.Email == model.Email);
            if (account == null) return null;
            var accountPassword = myAES.DecryptString(account.Password);
            return accountPassword != model.Password ? null : account;
        }
        private Account FindCorrespondingAccount(string email)
        {
            var account = _readOnlyRepository.First<Account>(
                account1 => account1.Email == email);            
            return account;
        }

        private Session RetrieveSession(Account account)
        {
            var session = _readOnlyRepository.Query<Session>(x => x.SessionAccount.Email == account.Email).OrderByDescending(x => x.DateStarted).FirstOrDefault();
            if (session == null) return null;
                return Security.IsTokenExpired(session) ? null : session;
        }
        private static void SendAccountCreatedMail(Account accountCreated)
        {
            var client = new RestClient
            {
                BaseUrl = "https://api.mailgun.net/v2",
                Authenticator = new HttpBasicAuthenticator("api", "key-5sbcxpwm9avrbeds-35y2i5hmda4y8k1")
            };
            var request = new RestRequest();
            request.AddParameter("domain", "sandbox37840.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "MiniTrello MC <postmaster@sandbox37840.mailgun.org>");
            request.AddParameter("to", accountCreated.FirstName + "<" + accountCreated.Email + ">");
            request.AddParameter("subject", "Welcome");
            request.AddParameter("text", "Welcome to MiniTrello MC!");
            request.Method = Method.POST;
            client.Execute(request);
        }
        private static void SendPasswordRestoreMail(string email,string token)
        {
            var client = new RestClient
            {
                BaseUrl = "https://api.mailgun.net/v2",
                Authenticator = new HttpBasicAuthenticator("api", "key-5sbcxpwm9avrbeds-35y2i5hmda4y8k1")
            };
            var request = new RestRequest();
            request.AddParameter("domain", "sandbox37840.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "MiniTrello MC <postmaster@sandbox37840.mailgun.org>");
            request.AddParameter("to", email);
            request.AddParameter("subject","Password restore");
            request.AddParameter("text", "To restore your password, please visit http://mcminitrelloapi.apphb.com/boards/" + token);
            request.Method = Method.POST;
            client.Execute(request);
        }
    }
}