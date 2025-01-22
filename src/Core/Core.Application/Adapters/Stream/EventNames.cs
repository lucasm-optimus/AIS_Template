namespace Optimus.Core.Application.Adapters.Stream
{
    /// <summary>
    /// Define the names of the Topics used in this solution
    /// </summary>
    public static class TOPICS
    {
        public const string ADUserCreated = "ADUserCreated";
        public const string ADUserBlockScheduled = "ADUserBlockScheduled";
        public const string ADUserBlocked = "ADUserBlocked";
        public const string ADUserTransfered = "ADUserTransfered";
        public const string AccountChangeRequested = "PowerAppsAccountChangeRequested";
    }

    public static class SUBSCRIPTIONS
    {
        public const string ExecuteAccountChange = "ExecuteAccountChange";
        public const string ExecuteAccountBlock = "BlockAccount";

    }
}
