using Machine.Specifications;
using MiniTrello.Domain.DataObjects;


namespace MiniTrello.ApiWrapper.Specs
{
    [Subject(typeof(AccountAuthenticationModel),"Test if you can log in")]
    public class when_user_wants_to_login
    {
        
        

        Establish context = () => { _accountLoginModel = new AccountLoginModel(); };

        Because of = () => { _result = MiniTrelloSdk.Login(_accountLoginModel); };

        It should_return_the_expected_token = () =>
        {
               _resu
        };
        static AccountLoginModel _accountLoginModel;
        private static AccountAuthenticationModel _result;
    }
}