using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AutoMapper;
using MiniTrello.Api.Controllers.Helpers;
using MiniTrello.Api.CustomExceptions;
using MiniTrello.Api.Other;
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
                    if (sessionCreated != null) return new AccountAuthenticationModel { Token = token };
                }
                if (retrievedSession != null) return new AccountAuthenticationModel { Token = retrievedSession.Token };
            }
            return new AccountAuthenticationModel
            {
                ErrorCode = 1,
                ErrorMessage = "Invalid username or password",
                Token = ""
            };
        }

       

        [POST("register")]
        public RegisterConfirmationModel Register([FromBody] AccountRegisterModel model)
        {
            var errorMessage = new ValidationHelper().Validate(model);
            if (errorMessage.Length > 0) //ERROR OCURRED
                return new RegisterConfirmationModel
                {
                    ErrorCode = 1,
                    ErrorMessage = errorMessage,
                    Message = ""
                };

            var account = _mappingEngine.Map<AccountRegisterModel, Account>(model);
            account.Password = encryptPassword(account.Password);
            var accountCreated = _writeOnlyRepository.Create(account);
            if (accountCreated == null) //ERROR OCURRED
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
            GetBoardsModel getBoardsModel;
            var session = Security.VerifiySession(token, _readOnlyRepository);
            if (session == null)
            {
                getBoardsModel = new GetBoardsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "The session you are trying to reach does not exist on this server."
                };
                return getBoardsModel;
            }

            if(Security.IsTokenExpired(session))
            {
                getBoardsModel = new GetBoardsModel
                {
                    ErrorCode = 1,
                    ErrorMessage = "The session you are trying to reach has expired."
                };
                return getBoardsModel;
            }
            var accountFromSession = Security.GetAccountFromSession(session, _readOnlyRepository);
            var boardsList = accountFromSession.Boards.ToList();
            getBoardsModel = new GetBoardsModel();
            foreach (var board in boardsList)
            {
                var boardModel = _mappingEngine.Map<Board,BoardModel>(board);
                getBoardsModel.AddBoard(boardModel);
            }
            return getBoardsModel;
        }

        [POST("restorepassword")]
        public RestorePasswordModel RestorePassword(PasswordRestoreModel model)
        {
            RegexUtilities.IsValidEmail(model.Email);
            String token = Security.CreateRestoreToken(model.Email);
            Account myAccount = FindCorrespondingAccount(model.Email);
            if (myAccount == null) throw new BadRequestException("This account was not found on this server");
            Session newSession = new Session
            {
                SessionAccount = _readOnlyRepository.First<Account>(account => account.Email == model.Email),
                Token = token,
                DateStarted = DateTime.UtcNow,
                Duration = 15,
            };
            Session sessionCreated = _writeOnlyRepository.Create(newSession);
            //send token to email.
            var returnValue = new RestorePasswordModel
            {
                Message = "Instructions on password reset have been sent to your Email"
            };
            return returnValue;
        }


        [System.Web.Http.AcceptVerbs(new[] { "PUT" })]
        [PUT("profile/edit/{token}")]
        public EditedProfileModel EditProfile([FromBody] AccountEditModel model, string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            string errorMessage = ValidationHelper.ValidateEditModel(model);
            if(errorMessage.Length > 0) throw new BadRequestException(errorMessage);
            myAccount.FirstName = model.FirstName;
            myAccount.LastName = model.LastName;
            myAccount.Password = model.Password;
            _writeOnlyRepository.Update(myAccount);
            EditedProfileModel myEditedProfileModel = _mappingEngine.Map<Account, EditedProfileModel>(myAccount);
            return myEditedProfileModel;
        }


        private string encryptPassword(string password)
        {
            var myAES = new SimpleAES();
            return myAES.EncryptToString(password);
        }


        private Account FindCorrespondingAccount(AccountLoginModel model)
        {
            var myAES = new SimpleAES();
            var account = _readOnlyRepository.First<Account>(
                account1 => account1.Email == model.Email);
            if (account == null) return null;
            var accountPassword = myAES.DecryptString(account.Password);
            if (accountPassword != model.Password) return null;
            return account;
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
            try
            {
                Security.IsTokenExpired(session);
            }
            catch (BadRequestException exception)
            {
                if (exception.Response.ReasonPhrase == "Your session has expired, please log in again")
                    return null;
            }
            return session;
        }
        private void SendAccountCreatedMail(Account accountCreated)
        {
            RestClient client = new RestClient
            {
                BaseUrl = "https://api.mailgun.net/v2",
                Authenticator = new HttpBasicAuthenticator("api", "key-5sbcxpwm9avrbeds-35y2i5hmda4y8k1")
            };
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandbox37840.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "MiniTrello MC <postmaster@sandbox37840.mailgun.org>");
            request.AddParameter("to", accountCreated.FirstName + "<" + accountCreated.Email + ">");
            request.AddParameter("subject", "Welcome");
            request.AddParameter("text", "Welcome to MiniTrello MC!");
            request.Method = Method.POST;
            client.Execute(request);
        }
    }

    public class EditedProfileModel
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Message { get; set; }
    }

    public class RestorePasswordModel
    {
        public string Message { get; set; }
    }

    public class PasswordRestoreModel
    {
        public string Email { set; get; }
    }

    public class GetBoardsModel
    {
        private readonly List<BoardModel> _boards = new List<BoardModel>();
        public int ErrorCode { set; get; }
        public string ErrorMessage { set; get; }
        public IEnumerable<BoardModel> Boards
        {
            get { return _boards; }
        }

        public void AddBoard(BoardModel board)
        {
            _boards.Add(board);
        }
    }
}



public class ValidationHelper : IRegisterValidator<AccountRegisterModel>
{
    public string Validate(AccountRegisterModel model)
    {
        if (model.Password != model.ConfirmPassword)
        {
            return "The password confirmation and password fields do not match";
        }

        if (model.Password.Count() < 8)
        {
            return "Your password must contain 8 or more characters";
        }
        if (!RegexUtilities.IsValidEmail(model.Email))
        {
            return "The email you entered is not valid, please enter a valid email.";
        }
        return "";
    }

    public static string ValidateEditModel(AccountEditModel model)
    {
        if (model.FirstName == "")
        {
            return "You must provide a first name.";
        }
        if (model.LastName == "")
        {
            return "You must provide a last name.";
        }
        if (model.Password != model.ConfirmPassword)
        {
            return "The password confirmation and password fields do not match";
        }

        if (model.Password.Count() < 8)
        {
            return "Your password must contain 8 or more characters";
        }
        return "";
    }
}