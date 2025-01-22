using System.Text.Json.Serialization;

namespace Optimus.Core.Domain.Aggregates.Ticket
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TicketStatusEnum
    {
        /// <summary>
        /// When the ticket is recently created
        /// </summary>
        Opened = 1,

        /// <summary>
        /// The ticket contains a supply in progress
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// All Supplies are finished
        /// </summary>
        Finished = 3,

        /// <summary>
        /// The ticket is waiting for payment
        /// </summary>
        WaitingPayment = 4,

        /// <summary>
        /// The ticket is already paid by cashier
        /// </summary>
        Paid = 5,

        /// <summary>
        /// The ticket was abandoned by system
        /// </summary>
        Abandoned = 6
    }
}
