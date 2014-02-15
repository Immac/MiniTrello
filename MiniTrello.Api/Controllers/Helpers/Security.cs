using System;
using System.Globalization;
using MiniTrello.Api.Models;
using MiniTrello.Domain.Entities;

namespace MiniTrello.Api.Controllers.Helpers
{
    public static class Security
    {
        public static string CreateToken(Account account)
        {
            var token = account.Id.ToString(CultureInfo.InvariantCulture).GetHashCode().ToString(CultureInfo.InvariantCulture);
            return token;
        }

        public static long GetTokenLifeSpan(AccountLoginModel model)
        {
            return model.SessionDuration > 30 ? model.SessionDuration : 30;
        }

        public static bool IsTokenExpired(Session session)
        {
            return session.DateStarted.AddMinutes(session.Duration) > DateTime.UtcNow;
        }
    }
}