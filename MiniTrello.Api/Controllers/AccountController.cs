using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AutoMapper;
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
        public AuthenticationModel Login([FromBody] AccountLoginModel model)
        {
            var account =
                _readOnlyRepository.First<Account>(
                    account1 => account1.Email == model.Email && account1.Password == model.Password);
            if (account != null)
            {
                string token = Security.CreateToken(account);
                var newSession = new Session
                {
                    Token = token,
                    DateStarted = DateTime.Now,
                    Duration = TimeSpan.FromHours(18)
                };
                var sessionCreated = _writeOnlyRepository.Create(newSession);
                if(sessionCreated != null) return new AuthenticationModel {Token = token};
            }
            
            throw new BadRequestException(
                "Incorrect Username or Password!");
        }

        [POST("register")]
        public HttpResponseMessage Register([FromBody] AccountRegisterModel model)
        {
            var errorMessage = new RegisterValidator().Validate(model);
            if (errorMessage.Length > 0) throw new BadRequestException(errorMessage);
            Account account = _mappingEngine.Map<AccountRegisterModel,Account>(model);
            Account accountCreated = _writeOnlyRepository.Create(account);
            if (accountCreated != null) return new HttpResponseMessage(HttpStatusCode.OK);
            throw new BadRequestException("There has been an error while trying to add this user");
        }

        [POST("/boards/create/{token}")]
        public HttpResponseMessage CreateBoard([FromBody]BoardCreateModel model,string token)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }

    public class Security
    {
        public static string CreateToken(Account account)
        {
            var token = account.FirstName + ";" 
                + account.Id + ";"
                    + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            return token;
        }
    }

    public class BoardCreateModel
    {
        public string Name { set; get; } 
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