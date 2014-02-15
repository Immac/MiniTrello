using System;
using System.Security.Cryptography.X509Certificates;

namespace MiniTrello.Api.Models
{
    public class AccountLoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public long SessionDuration { get; set; }
    }
}