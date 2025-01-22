using AutoMais.Core.Domain.Aggregates.Account.Events;

namespace AutoMais.Core.Domain.Aggregates.Account.Commands
{
    /// <summary>
    /// Represents a feature to transfer an account and returns an Event
    /// </summary>
    public class TransferAccountCommand : ChangeAccountCommand<AccountTransferedEvent>
    {
        public AccountAgg ToDomain()
        {
            return new AccountAgg(this);
        }

        public TransferAccountCommand(AccountChangeRequestedCommand command)
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
}
