//using Optimus.Core.Domain.Aggregates.Account.Commands;
//using Optimus.Core.Domain.Aggregates.Account.Events;

//namespace Optimus.Core.Application.Account.Handlers
//{
//    public class TransferAccountCommandHandler : IRequestHandler<TransferAccountCommand, Result<AccountTransferedEvent>>
//    {
//        private readonly ILogger<TransferAccountCommandHandler> _Logger;
//        private readonly IMediator _Mediator;
//        private readonly IUserManagement _userManagement;

//        public TransferAccountCommandHandler(ILogger<TransferAccountCommandHandler> logger, IUserManagement userManagement, IMediator mediator)
//        {
//            _Logger = logger;
//            _Mediator = mediator;
//            _userManagement = userManagement;
//        }

//        public async Task<Result<AccountTransferedEvent>> Handle(TransferAccountCommand request, CancellationToken cancellationToken)
//        {
//            // Translate the command into a domain where the rules will be evaluated
//            var domain = request.ToDomain();

//            // Ask the domain to perform one specific action
//            var canBeTransfered = domain.Transfer();

//            // If the result is success, we can proceed sending the data to the infrastructure
//            if (canBeTransfered.IsSuccess)
//            {
//                // To finish the command, we publish the received result
//                await _Mediator.Publish(canBeTransfered.Value, cancellationToken);

//                return Result.Fail<AccountTransferedEvent>("Error transfering account").WithErrors(canBeTransfered.Errors);
//            }

//            // If cannot be created, the result is Failed and already contains the validation rules
//            return canBeTransfered;
//        }

//    }
//}
