using Optimus.Core.Application.Attendant.State;
using Optimus.Core.Application.Pump.State;
using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;
using Optimus.Core.Domain.Aggregates.Ticket.Events;

namespace Optimus.Core.Application.Ticket.Commands
{
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class AddFuelToTicketCommandValidator : AbstractValidator<AddFuelToTicketCommand>
    {
        public AddFuelToTicketCommandValidator(IPumpState pumpState, IAttendantState attendantState)
        {
            RuleFor(command => command.CardId)
                .NotEmpty()
                .MustAsync(async (instance, propValue, c) =>
                {
                    instance.Attendant = await attendantState.Get(x => x.CardId == propValue);
                    return instance.Attendant != null;
                });

            RuleFor(command => command.PumpNumber)
                .NotEmpty()
                .MustAsync(async (instance, propValue, cancellationToken) =>
                {
                    instance.Pump = await pumpState.Get(x => x.Number == propValue);
                    return instance.Pump != null;
                }).DependentRules(() =>
                {
                    RuleFor(command => command.NozzleNumber)
                        .NotEmpty()
                        .Must((instance, property, token) =>
                        {
                            instance.Nozzle = instance?.Pump?.Nozzles?.FirstOrDefault(x => x.Number == property);
                            return instance?.Nozzle != null;
                        });
                });

            RuleFor(command => command.Quantity)
                .NotEmpty();
        }
    }
    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class AuthorizeRefuelingForTicketCommandValidator : AbstractValidator<AuthorizeRefuelingForTicketCommand>
    {
        public AuthorizeRefuelingForTicketCommandValidator(IPumpState pumpState, ITicketState ticketState)
        {
            RuleFor(command => command.TicketId)
                .NotEmpty()
                .MustAsync(async (instance, propValue, c) =>
                {
                    var result = await ticketState.GetOpenedTicket(instance);
                    if (result.IsSuccess)
                        instance.Ticket = result.Value;

                    return instance.Ticket != null;
                })
                .WithMessage("Ticket not found");

            RuleFor(command => command.PumpNumber)
                .NotEmpty()
                .MustAsync(async (instance, propValue, cancellationToken) =>
                {
                    instance.Pump = await pumpState.Get(x => x.Number == propValue);
                    return instance.Pump != null;
                }).DependentRules(() =>
                {
                    RuleFor(command => command.NozzleNumber)
                        .NotEmpty()
                        .Must((instance, property, token) =>
                        {
                            instance.Nozzle = instance?.Pump?.Nozzles?.FirstOrDefault(x => x.Number == property);
                            return instance?.Nozzle != null;
                        });
                });
        }
    }

    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class FinishRefuelingCommandValidator : AbstractValidator<FinishSupply>
    {
        public FinishRefuelingCommandValidator(IPumpState pumpState)
        {
            RuleFor(command => command.PumpNumber)
                .NotEmpty()
                .MustAsync(async (instance, propValue, cancellationToken) =>
                {
                    instance.Pump = await pumpState.Get(x => x.Number == propValue);
                    return instance.Pump != null;
                }).DependentRules(() =>
                {
                    RuleFor(command => command.NozzleNumber)
                        .NotEmpty()
                        .Must((instance, property, token) =>
                        {
                            instance.Nozzle = instance?.Pump?.Nozzles?.FirstOrDefault(x => x.Number == property);
                            return instance?.Nozzle != null;
                        });
                });
        }
    }

    /// <summary>
    /// The command validator contains the rules to ensure the object is valid
    /// </summary>
    public sealed class UpdateFuelToTicketCommandValidator : AbstractValidator<UpdateFuelToTicketCommand>
    {
        public UpdateFuelToTicketCommandValidator(IPumpState pumpState)
        {
            RuleFor(command => command.Quantity);

            RuleFor(command => command.Cost);
        }
    }

    public class UpdateFuelToTicketCommandHandler(ITicketState ticketState, IPumpState pumpState, IMediator mediator) : 
        ICommandHandler<FinishSupply, TicketUpdated>,
        ICommandHandler<UpdateFuelToTicketCommand, TicketUpdated>,
        ICommandHandler<AddFuelToTicketCommand, TicketUpdated>,
        ICommandHandler<AuthorizeRefuelingForTicketCommand, TicketUpdated>
    {
        public async Task<Result<TicketUpdated>> Handle(AddFuelToTicketCommand command, CancellationToken cancellationToken)
        {
            var openedTicket = await ticketState.GetOpenedTicket(command);

            if (openedTicket == null)
            {
                var ticketCreated = await mediator.Send(new CreateTicketForAttendantCommand(command.CardId, command.PumpNumber, command.NozzleNumber));

                if (ticketCreated.IsSuccess)
                    openedTicket = Result.Ok(ticketCreated.Value.Ticket);
                else
                    return Result.Fail<TicketUpdated>("Failure creating ticket").WithErrors(ticketCreated.Errors);
            }

            var result = Result.Ok();

            if (openedTicket != null)
            {
                var updated = openedTicket.Value.AddOrUpdateSupply(command);

                if (updated.IsSuccess)
                {
                    var saved = await ticketState.Update(openedTicket.Value.Id, updated.Value.Ticket);
                    if (saved.IsSuccess)
                    {
                        await mediator.Publish(updated.Value);
                        return updated.Value;
                    }
                    else
                        result.WithErrors(saved.Errors);
                }

                result.WithErrors(updated.Errors);
            }

            return result;
        }

        public async Task<Result<TicketUpdated>> Handle(UpdateFuelToTicketCommand command, CancellationToken cancellationToken)
        {
            command.Pump = await pumpState.Get(x => x.Number == command.PumpNumber && x.Nozzles.Any(n => n.Number == command.NozzleNumber));
            command.Nozzle = command.Pump?.Nozzles?.FirstOrDefault(x => x.Number == command.NozzleNumber);

            var openedTicket = await ticketState.GetOpenedTicket(command);

            if (openedTicket.IsFailed)
            {
                var ticketCreated = await mediator.Send(new CreateTicketForAttendantCommand("00000000", command.PumpNumber, command.NozzleNumber));

                if (ticketCreated.IsSuccess)
                    openedTicket = Result.Ok(ticketCreated.Value.Ticket);
                else
                    return Result.Fail<TicketUpdated>("Failure creating ticket").WithErrors(ticketCreated.Errors);
            }

            var result = Result.Ok();

            var updated = openedTicket.Value.AddOrUpdateSupply(command);

            if (updated.IsSuccess)
            {
                var saved = await ticketState.Update(openedTicket.Value.Id, updated.Value.Ticket);
                if (saved.IsSuccess)
                {
                    await mediator.Publish(updated.Value);
                    return updated.Value;
                }
                else
                    result.WithErrors(saved.Errors);
            }

            result.WithErrors(updated.Errors);

            return result;
        }

        public async Task<Result<TicketUpdated>> Handle(FinishSupply command, CancellationToken cancellationToken)
        {
            var openedTicket = await ticketState.GetOpenedTicket(command);

            if (openedTicket.IsFailed)
            {
                return Result.Fail<TicketUpdated>("No ticket Opened or In Progress found");
            }

            var result = Result.Ok();

            var updated = openedTicket.Value.FinishSupply(command);

            if (updated.IsSuccess)
            {
                var saved = await ticketState.Update(openedTicket.Value.Id, updated.Value.Ticket);
                if (saved.IsSuccess)
                {
                    await mediator.Publish(updated.Value);
                    return updated.Value;
                }
                else
                    result.WithErrors(saved.Errors);
            }

            result.WithErrors(updated.Errors);

            return result;
        }

        public async Task<Result<TicketUpdated>> Handle(AuthorizeRefuelingForTicketCommand command, CancellationToken cancellationToken)
        {
            if (command.Ticket == null)
            {
                var ticketCreated = await mediator.Send(new CreateTicketForAttendantCommand(command.Ticket.Attendant.CardId, command.PumpNumber, command.NozzleNumber));

                if (ticketCreated.IsSuccess)
                    command.Ticket = ticketCreated.Value.Ticket;
                else
                    return Result.Fail<TicketUpdated>("Failure creating ticket").WithErrors(ticketCreated.Errors);
            }

            var result = Result.Ok();

            if (command.Ticket != null)
            {
                var updated = command.Ticket.AddOrUpdateSupply(command);

                if (updated.IsSuccess)
                {
                    var saved = await ticketState.Update(command.Ticket.Id, updated.Value.Ticket);
                    if (saved.IsSuccess)
                    {
                        await mediator.Publish(updated.Value);
                        return updated.Value;
                    }
                    else
                        result.WithErrors(saved.Errors);
                }

                result.WithErrors(updated.Errors);
            }

            return result;
        }
    }
}
