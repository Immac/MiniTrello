using System;
using System.Globalization;
using System.Linq;
using MiniTrello.Api.CustomExceptions;
using MiniTrello.Domain.DataObjects;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;

namespace MiniTrello.Api.Controllers.Helpers
{
    public static class Security
    {
        public static string CreateToken(Account account,IReadOnlyRepository readOnlyRepository)
        {   
            var myAES = new SimpleAES();
            int random = RandomHelper.Instance.Next();
            long tokenSeed = account.Id.GetHashCode() + random;
            string token = myAES.EncryptToString(tokenSeed.ToString(CultureInfo.InvariantCulture));
            return token;
        }

        public static long GetTokenLifeSpan(AccountLoginModel model)
        {
            return model.SessionDuration > 15 ? model.SessionDuration : 15;
        }

        public static bool IsTokenExpired(Session session)
        {
            return  session.DateStarted.AddMinutes(session.Duration) < DateTime.UtcNow;
        }
        public static Session VerifiySession(string token,IReadOnlyRepository readOnlyRepository)
        {
            var session = readOnlyRepository.First<Session>(session1 => session1.Token == token);
            return session;
        }
        public static bool IsThisAccountAdminOfThisBoard(Board board, Account account)
        {
            return account.Email == board.OwnerAccount.Email || board.AdministratorAccounts.Any(adminAccount => adminAccount.Email == account.Email);
        }

        public static string EncryptPassword(string password)
        {
            var myAES = new SimpleAES();
            return myAES.EncryptToString(password);
        }

        public static bool IsThisAccountMemberOfThisBoard(Board board, Account account)
        {
            return board.MemberAccounts.Any(memberAccount => memberAccount.Email == account.Email) || IsThisAccountAdminOfThisBoard(board, account);
        }

        public static Account GetAccountFromSession(Session session, IReadOnlyRepository readOnlyRepository)
        {
            //Support for archived accs
            var account = readOnlyRepository.First<Account>(account1 => account1.Email == session.SessionAccount.Email);
            if (account != null) return account;
            throw new BadRequestException("The account you are trying to reach does not exist in this server");
        }

        public static string CreateRestoreToken(string email)
        {
            SimpleAES myAES = new SimpleAES();
            int random = RandomHelper.Instance.Next();
            long tokenSeed = email.GetHashCode() + random;
            string token = myAES.EncryptToString(tokenSeed.ToString(CultureInfo.InvariantCulture));
            return token;
        }
    }
}