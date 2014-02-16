using System;
using System.Globalization;
using System.Security.Cryptography;
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
            return session.DateStarted.AddMinutes(session.Duration) < DateTime.UtcNow;
        }
    }
}