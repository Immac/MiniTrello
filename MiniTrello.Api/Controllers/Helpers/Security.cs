using System;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using MiniTrello.Api.CustomExceptions;
using MiniTrello.Api.Models;
using MiniTrello.Data;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;

namespace MiniTrello.Api.Controllers.Helpers
{
    public static class Security
    {
        public static string CreateToken(Account account,IReadOnlyRepository readOnlyRepository)
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
        public static Session VerifiySession(string token,IReadOnlyRepository readOnlyRepository)
        {
            Session session = readOnlyRepository.First<Session>(session1 => session1.Token == token);
            if (session == null)
                throw new BadRequestException("Session you are trying to reach does not exist in this server");
            return session;
        }
        public static void IsThisAccountAdminOfThisBoard(Board board, Account account)
        {
            if(account.Email == board.OwnerAccount.Email) //Owner of the board is always admin to it
                return;
            if (board.AdministratorAccounts.Any(adminAccount => adminAccount.Email == account.Email))
                return;
            throw new BadRequestException("You do not posses Administrative priviledges on this board, you are not a True Administrator");
        }

        public static void IsThisAccountMemberOfThisBoard(Board board, Account account)
        {
            if (board.MemberAccounts.Any(memberAccount => memberAccount.Email == account.Email))
                return;
            try
            {
                IsThisAccountAdminOfThisBoard(board, account);
            }
            catch (BadRequestException)
            {
                throw new BadRequestException("You are not a member of this board.");
            }
            
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