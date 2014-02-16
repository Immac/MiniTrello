using System;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using MiniTrello.Api.CustomExceptions;
using MiniTrello.Api.Models;
using MiniTrello.Domain.Entities;

namespace MiniTrello.Api.Controllers.Helpers
{
    public static class Security
    {
        public static string CreateToken(Account account)
        {
            SimpleAES myAES = new SimpleAES();
            int random = RandomHelper.Instance.Next();
            long tokenSeed = account.Id.GetHashCode() + random;
            string token = myAES.EncryptToString(tokenSeed.ToString(CultureInfo.InvariantCulture));
            return token;
        }

        public static long GetTokenLifeSpan(AccountLoginModel model)
        {
            return model.SessionDuration > 30 ? model.SessionDuration : 30;
        }

        public static bool IsTokenExpired(Session session)
        {
            bool isTokenExpired = session.DateStarted.AddMinutes(session.Duration) < DateTime.UtcNow;
            if(isTokenExpired)
                throw new BadRequestException("Your session has expired, please log in again");
            return false;
        }
    }
}