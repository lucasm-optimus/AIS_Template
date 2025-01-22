//using Optimus.Core.Domain.Aggregates.Account.Commands;
//using Optimus.Core.Domain.Aggregates.Account.Events;

//namespace Optimus.Core.Application.Account.Handlers
//{
//    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result<AccountCreatedEvent>>
//    {
//        private readonly ILogger<CreateAccountCommandHandler> _Logger;
//        private readonly IMediator _Mediator;
//        private readonly IUserManagement _userManagement;
//        private readonly ISharepointService _sharepointService;

//        public CreateAccountCommandHandler(ILogger<CreateAccountCommandHandler> logger, IUserManagement userManagement, IMediator mediator, ISharepointService sharepointService)
//        {
//            _Logger = logger;
//            _Mediator = mediator;
//            _userManagement = userManagement;
//            _sharepointService = sharepointService;
//        }

//        public async Task<Result<AccountCreatedEvent>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
//        {
//            // Translate the command into a domain where the rules will be evaluated
//            var domain = request.ToDomain();

//            // Ask the domain to perform one specific action
//            var canBeCreated = domain.Create();

//            // If the result is success, we can proceed sending the data to the infrastructure
//            if (canBeCreated.IsSuccess)
//            {
//                // Send the domain to the infrastructure handle
//                var userCreation = await _userManagement.CreateUser(canBeCreated.Value.Account, cancellationToken);

//                if (userCreation.IsSuccess)
//                {
//                    foreach (var role in domain.Roles)
//                    {
//                        role.ChangeRole(await _sharepointService.GetRole(role.Application.Name, domain.Department, domain.Division, domain.JobTitle));
//                    }                   


//                    // To finish the command, we publish the received result
//                    await _Mediator.Publish(canBeCreated.Value, cancellationToken);

//                    return Result.Ok(canBeCreated.Value);
//                }
//                return Result.Fail<AccountCreatedEvent>("Error creating account in Active Directory").WithErrors(userCreation.Errors);
//            }

//            // If cannot be created, the result is Failed and already contains the validation rules
//            return Result.Fail<AccountCreatedEvent>("Account creation failed")
//                .WithErrors(canBeCreated.Errors);
//        }

//    }
//}
