using AutoMais.Core.Domain.Aggregates.Account.Events;
using FluentValidation;

namespace AutoMais.Core.Domain.Aggregates.Account.Commands
{
    /// <summary>
    /// Represents a feature to create a new Account and returns an Event
    /// </summary>
    public class CreateAccountCommand : ChangeAccountCommand<AccountCreatedEvent>
    {
        public AccountAgg ToDomain()
        {
            return new AccountAgg(this);
        }

        public CreateAccountCommand(AccountChangeRequestedCommand command)
        {
            FullName = command.FullName;
            Firstname = command.Firstname;
            LastName = command.LastName;
            JobTitle = command.JobTitle;
            Department = command.Department;
            Division = command.Division;
            Email = command.Email;
            Roles = command.ApplicationRoles ?? new List<UserApplicationRoleDTO>();
        }
    }

    /// <summary>
    /// Represents a feature to create a new Account and returns an Event
    /// </summary>
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.FullName).NotEmpty();
        }
    }
}
