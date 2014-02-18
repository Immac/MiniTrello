using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AutoMapper;
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
            string errorMessage = new RegisterValidator().Validate(model);
            if (errorMessage.Length > 0) throw new BadRequestException(errorMessage);
            Account account = _mappingEngine.Map<AccountRegisterModel,Account>(model);
            Account accountCreated = _writeOnlyRepository.Create(account);
            if (accountCreated != null) return new HttpResponseMessage(HttpStatusCode.OK);
            throw new BadRequestException("There has been an error while trying to add this user");
        }


        

        private Account FindCorrespondingAccount(AccountLoginModel model)
        {
            var account =  _readOnlyRepository.First<Account>(
                account1 => account1.Email == model.Email && account1.Password == model.Password);
            if (account == null) throw new BadRequestException("Incorrect username or password");
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
}



public class RegisterValidator : IRegisterValidator<AccountRegisterModel>
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
}