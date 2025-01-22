using Optimus.Core.Domain.Aggregates.Ticket;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;

namespace Optimus.Core.Application.Ticket.Adapters
{
    public interface ITicketState : IState<TicketAgg>
    {
        /// <summary>
        /// Get the Opened or In Progress ticket to the specified Pump and Nozzle
        /// </summary>
        /// <param name="pumpNumber">The Pump unique number</param>
        /// <param name="nozzleNumber">The Pump Nozzle unique number</param>
        /// <returns>Returns the ticket aggregate</returns>
        Task<Result<TicketAgg>> GetOpenedTicket(int pumpNumber, int nozzleNumber);

        /// <summary>
        /// Get the Opened or In Progress ticket to the specified Pump and Nozzle
        /// </summary>
        /// <param name="command">The FinishRefuelingCommand to search ticket</param>
        /// <returns>Returns the ticket aggregate</returns>
        Task<Result<TicketAgg>> GetOpenedTicket(FinishSupply command);

        /// <summary>
        /// Get the Opened or In Progress ticket to the specified Pump and Nozzle
        /// </summary>
        /// <param name="command">The FinishRefuelingCommand to search ticket</param>
        /// <returns>Returns the ticket aggregate</returns>
        Task<Result<TicketAgg>> GetOpenedTicket(AuthorizeRefuelingForTicketCommand command);

        /// <summary>
        /// Get the Opened or In Progress ticket to the specified Pump and Nozzle
        /// </summary>
        /// <param name="command">The AddFuelToTicketCommand to search ticket</param>
        /// <returns>Returns the ticket aggregate</returns>
        Task<Result<TicketAgg>> GetOpenedTicket(AddFuelToTicketCommand command);


        /// <summary>
        /// Get the Opened or In Progress ticket to the specified Car, Pump and Nozzle
        /// </summary>
        /// <param name="cardId">The attendant cardId</param>
        /// <param name="pumpNumber">The Pump unique number</param>
        /// <param name="nozzleNumber">The Pump Nozzle unique number</param>
        /// <returns>Returns the ticket aggregate</returns>
        Task<Result<TicketAgg>> GetOpenedTicket(string cardId, int pumpNumber, int nozzleNumber);

        /// <summary>
        /// Get the Opened or In Progress ticket to the specified Car, Pump and Nozzle
        /// </summary>
        /// <param name="cardId">The attendant cardId</param>
        /// <param name="pumpNumber">The Pump unique number</param>
        /// <param name="nozzleNumber">The Pump Nozzle unique number</param>
        /// <returns>Returns the ticket aggregate</returns>
        Task<Result<TicketAgg>> GetOpenedTicket(UpdateFuelToTicketCommand command);
    }
}
