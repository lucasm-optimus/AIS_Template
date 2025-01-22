//using Optimus.Core.Domain.Aggregates.Account.Commands;
//using Optimus.Core.Domain.Aggregates.Account.Events;

//namespace Optimus.Core.Application.Account.Handlers
//{
//    public class BlockAccountCommandHandler : IRequestHandler<BlockAccountCommand, Result<IAccountBlockedEvent>>
//    {
//        private readonly ILogger<BlockAccountCommandHandler> _Logger;
//        private readonly IMediator _Mediator;
//        private readonly IUserManagement _userManagementService;

//        public BlockAccountCommandHandler(ILogger<BlockAccountCommandHandler> logger, IUserManagement userManagement, IMediator mediator)
//        {
//            _Logger = logger;
//            _Mediator = mediator;
//            _userManagementService = userManagement;
//        }

//        public async Task<Result<IAccountBlockedEvent>> Handle(BlockAccountCommand command, CancellationToken cancellationToken)
//        {
//            var domain = command.ToDomain();
//            var domainEvent = domain.Block();

//            if (domainEvent?.Value is AccountBlockScheduled)
//            {
//                await _Mediator.Publish(domainEvent.Value, cancellationToken);
//                return Result.Ok<IAccountBlockedEvent>((IAccountBlockedEvent)domainEvent.Value);

//            } else
//            {
//                var accountBlocked = await _userManagementService.BlockUser(command, cancellationToken);
//                // If the result is success, we can proceed sending the data to the infrastructure
//                if (accountBlocked.IsSuccess)
//                {
//                    // To finish the command, we publish the received result
//                    var blockedEvent = new AccountBlockedEvent(command.Id, command.Email);
//                    await _Mediator.Publish(blockedEvent, cancellationToken);

//                    return Result.Ok<IAccountBlockedEvent>(blockedEvent);
//                }
//                else
//                    return Result.Fail<IAccountBlockedEvent>("Error blocking or scheduling block to account").WithErrors(accountBlocked.Errors);
//            }
//        }

//    }
//}
