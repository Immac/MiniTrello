namespace MiniTrello.Api.Controllers.Helpers
{
    public class ErrorStrings
    {
        public const string NotEnoughPriviledges = "You do not posses enough priviledges to perform this action";
        public const string SessionHasExpired = "The session you are trying to reach has expired.";
        public const string SessionDoesNotExistOnThisServer = "The session you are trying to reach does not exist on this server.";
        public const string AccountDoesNotExist = "The account you are tring to reach does not exist on this server.";
        public const string LaneDoesNotExist = "The lane you are tring to reach does not exist on this server.";
        public const string BoardDoesNotExist = "The board you are tring to reach does not exist on this server.";
        public const string CardDoesNotExist = "The card you are tring to reach does not exist on this server.";
    }
}