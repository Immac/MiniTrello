using System.Linq;
using MiniTrello.Domain.DataObjects;
using MiniTrello.Domain.Services;

namespace MiniTrello.Api.Controllers.Helpers
{
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
}