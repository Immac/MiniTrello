using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using AttributeRouting.Web.Http;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using MiniTrello.Api.Controllers;
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
            Account account = FindCorrespondingAccount(model);
            Session retrievedSession = RetrieveSession(account);
            if (retrievedSession == null)
            {
                string token = Security.CreateToken(account, _readOnlyRepository);
                long sessionDuration = Security.GetTokenLifeSpan(model);
                Session newSession = new Session
                {
                    SessionAccount = account,
                    Token = token,
                    DateStarted = DateTime.UtcNow,
                    Duration = sessionDuration
                };
                Session sessionCreated = _writeOnlyRepository.Create(newSession);
                if (sessionCreated != null) return new AccountAuthenticationModel { Token = token };
            }
            if (retrievedSession != null) return new AccountAuthenticationModel { Token = retrievedSession.Token };
            throw new BadRequestException("Incorrect Username or Password!");
        }

       

        [POST("register")]
        public RegisterConfirmationModel Register([FromBody] AccountRegisterModel model)
        {
            string errorMessage = new ValidationHelper().Validate(model);
            if (errorMessage.Length > 0) throw new BadRequestException(errorMessage);
            Account account = _mappingEngine.Map<AccountRegisterModel,Account>(model);
            account.Password = encryptPassword(account.Password);            
            
            Account accountCreated = _writeOnlyRepository.Create(account);
            if (accountCreated != null)
            {
                SendMail(accountCreated);
                return new RegisterConfirmationModel()
                {
                    Message = "Account confirmation message will be sent to " + model.Email
                };}
            throw new BadRequestException("There has been an error while trying to add this user");
        }

        private string encryptPassword(string password)
        {
            SimpleAES myAES = new SimpleAES();
            return myAES.EncryptToString(password);
        }

        private void SendMail(Account accountCreated)
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

        [GET("boards/{token}")]
        public GetBoardsModel GetBoards(string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            List<Board> myBoardsList = myAccount.Boards.ToList();
            GetBoardsModel myModel = new GetBoardsModel();
            foreach (var board in myBoardsList)
            {
                BoardModel myBoardModel = _mappingEngine.Map<Board,BoardModel>(board);
                myModel.AddBoard(myBoardModel);
            }
            return myModel;
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



        private Account FindCorrespondingAccount(AccountLoginModel model)
        {
            SimpleAES myAES = new SimpleAES();
            var account = _readOnlyRepository.First<Account>(
                account1 => account1.Email == model.Email);
            if (account == null) throw new BadRequestException("Incorrect username or password");
            var accountPassword = myAES.DecryptString(account.Password);
            if (accountPassword != model.Password) throw new BadRequestException("Incorrect username or password");
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