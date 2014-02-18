using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Script.Serialization;
using AttributeRouting.Web.Http;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using MiniTrello.Api.Controllers;
using MiniTrello.Api.Controllers.Helpers;
using MiniTrello.Api.CustomExceptions;
using MiniTrello.Api.Models;
using MiniTrello.Api.Other;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;


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
        public HttpResponseMessage Register([FromBody] AccountRegisterModel model)
        {
            string errorMessage = new ValidationHelper().Validate(model);
            if (errorMessage.Length > 0) throw new BadRequestException(errorMessage);
            Account account = _mappingEngine.Map<AccountRegisterModel,Account>(model);
            Account accountCreated = _writeOnlyRepository.Create(account);
            if (accountCreated != null) return new HttpResponseMessage(HttpStatusCode.OK);
            throw new BadRequestException("There has been an error while trying to add this user");
        }

        [GET("boards/{token}")]
        public GetBoardsModel GetBoards(string token)
        {
            Session session = Security.VerifiySession(token, _readOnlyRepository);
            Security.IsTokenExpired(session);
            Account myAccount = Security.GetAccountFromSession(session, _readOnlyRepository);
            List<Board> myBoardsList = myAccount.Boards.ToList();
            List<string> myBoardNamesList = myBoardsList.Select(board => (board.Title) + " " + (board.Id)).ToList();
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            GetBoardsModel myModel= new GetBoardsModel{ Boards = oSerializer.Serialize(myBoardNamesList) };
            return myModel;
        }

        [POST("restorepassword")]
        public HttpResponseMessage RestorePassword(PasswordRestoreModel model)
        {
            RegexUtilities.IsValidEmail(model.Email);
            String token = Security.CreateRestoreToken(model.Email);
            Account myAccount = FindCorrespondingAccount(model.Email);
            Session newSession = new Session
            {
                SessionAccount = _readOnlyRepository.First<Account>(account => account.Email == model.Email),
                Token = token,
                DateStarted = DateTime.UtcNow,
                Duration = 15,
            };
            Session sessionCreated = _writeOnlyRepository.Create(newSession);
            //send token to email.
            var returnValue = new HttpResponseMessage
            {
                ReasonPhrase = "Instructions on password reset have been sent to your Email"
            };
            return returnValue;
        }


        [AcceptVerbs(new[] { "PUT" })]
        [PUT("profile/edit/{token}")]
        public HttpResponseMessage EditProfile([FromBody] AccountEditModel model, string token)
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
            return new HttpResponseMessage(HttpStatusCode.OK);
        }



        private Account FindCorrespondingAccount(AccountLoginModel model)
        {
            var account =  _readOnlyRepository.First<Account>(
                account1 => account1.Email == model.Email && account1.Password == model.Password);
            if (account == null) throw new BadRequestException("Incorrect username or password");
            return account;
        }
        private Account FindCorrespondingAccount(string email)
        {
            var account = _readOnlyRepository.First<Account>(
                account1 => account1.Email == email);
            if (account == null) throw new BadRequestException("This account was not found on this server");
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

    public class PasswordRestoreModel
    {
        public string Email { set; get; }
    }

    public class GetBoardsModel
    {
        public string Boards { set; get; }
    }

    public class AccountEditModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
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