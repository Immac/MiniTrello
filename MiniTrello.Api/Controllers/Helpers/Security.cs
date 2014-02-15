using System;
using System.Globalization;
using MiniTrello.Api.Models;
using MiniTrello.Domain.Entities;

namespace MiniTrello.Api.Controllers.Helpers
{
    public class Security
    {
        public static string CreateToken(Account account)
        {
            var token = account.FirstName + ";" 
                        + account.Id + ";"
                        + DateTime.Now.ToString(CultureInfo.InvariantCulture) + ";"
                        + (DateTime.Now + TimeSpan.FromHours(2));
            return token;
        }

        public static TimeSpan GetTokenLifeSpan(AccountLoginModel model)
        {
            return TimeSpan.FromHours(model.SessionDuration) > TimeSpan.FromHours(2)
                    ? TimeSpan.FromHours(model.SessionDuration)
                    : TimeSpan.FromHours(2);
        }
    }
}