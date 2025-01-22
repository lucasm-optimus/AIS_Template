using AutoMais.Core.Domain.Aggregates.Account.Commands;
using AutoMais.Core.Domain.Aggregates.Account.Events;

namespace AutoMais.Core.Domain.Aggregates.Account
{
    /// <summary>
    /// This aggregate root represents all methods to manipulare an Account
    /// </summary>
    public class AccountAgg : AggRoot
    {
        public string Id { get; private set; }

        public string FullName { get; private set; }

        public string Firstname { get; private set; }

        public string LastName { get; private set; }

        public string JobTitle { get; private set; }

        public string Department { get; private set; }

        public string Division { get; private set; }

        public string Email { get; private set; }

        public DateTime BlockingDate { get; private set; }

        public IEnumerable<UserApplicationRole> Roles { get; private set; }

        internal AccountAgg(CreateAccountCommand command)
        {
            FullName = command.FullName;
            Firstname = command.Firstname;
            LastName = command.LastName;
            JobTitle = command.JobTitle;
            Department = command.Department;
            Division = command.Division;
            Email = command.Email;
            BlockingDate = command.BlockingDate;
            Roles = new List<UserApplicationRole>();

            command.Roles?.Select(appRole =>
            {
                AssignApplicationRole(appRole.ToDomain());
                return appRole;
            }).ToList();
        }

        internal AccountAgg(TransferAccountCommand command)
        {
            Id = command.Id;
            FullName = command.FullName;
            Firstname = command.Firstname;
            LastName = command.LastName;
            JobTitle = command.JobTitle;
            Department = command.Department;
            Division = command.Division;
            Email = command.Email;
            BlockingDate = command.BlockingDate;
            Roles = new List<UserApplicationRole>();

            command.Roles?.Select(appRole =>
            {
                AssignApplicationRole(appRole.ToDomain());
                return appRole;
            }).ToList();
        }
        internal AccountAgg(BlockAccountCommand command)
        {
            Id = command.Id;
            Email = command.Email;
            BlockingDate = command.BlockingDate;
            Roles = new List<UserApplicationRole>();
        }

        /// <summary>
        /// Indicates if the account block should be scheduled
        /// </summary>
        /// <returns></returns>
        public bool BlockingNeedsTobeScheduled()
        {
            return BlockingDate.Year > 2000;
        }

        /// <summary>
        /// Adds an Application Role to this Account
        /// </summary>
        /// <param name="appRole">One application and it's role name</param>
        /// <returns>A result with an event that represents the completion of the operation</returns>
        public Result<ApplicationRoleAssigned> AssignApplicationRole(UserApplicationRole appRole)
        {
            if (Roles.FirstOrDefault(x => x.Application.Id == appRole.Application.Id && x.RoleName == appRole.RoleName) == null)
            {
                var roles = new List<UserApplicationRole>(Roles);
                roles.Add(appRole);
                Roles = roles;
                return new ApplicationRoleAssigned(appRole);
            }

            return Result.Fail($"User already assigned to application {appRole.Application.Name} and role {appRole.RoleName}");
        }

        /// <summary>
        /// Revoke the account role in the specified Application
        /// </summary>
        /// <param name="appRole">One application and it's role name</param>
        /// <returns>A result with an event that represents the completion of the operation</returns>
        public Result<ApplicationRoleRevoked> RevokeApplicationRole(UserApplicationRole appRole)
        {
            var roles = new List<UserApplicationRole>(Roles);
            var existingRole = roles.FirstOrDefault(x => x.Application.Id == appRole.Application.Id && x.RoleName == appRole.RoleName);

            if (existingRole == null)
            {
                return Result.Fail($"User already does not have permission in application {appRole.Application.Name} with role {appRole.RoleName}");
            }
            else
            {
                roles.Remove(existingRole);
                Roles = roles;
                return Result.Ok(new ApplicationRoleRevoked());
            }
        }


        /// <summary>
        /// Block this account
        /// </summary>
        /// <returns>A result with an event that represents the completion of the operation</returns>
        public Result<IDomainEvent> Block()
        {
            //Validate if the account can be blocked
            if (BlockingDate <= DateTime.UtcNow)
                return new AccountBlockedEvent(this.Id, this.Email);
            else
                return ScheduleBlock();
        }

        /// <summary>
        /// Schedule a block for this account
        /// </summary>
        /// <returns>A result with an event that represents the completion of the operation</returns>
        private Result<IDomainEvent> ScheduleBlock()
        {
            if (BlockingDate < DateTime.UtcNow)
                return Result.Fail("This account cannot be scheduled to be blocked because the date is in the past");
            else
                return new AccountBlockScheduled(this.Id, this.Email, this.BlockingDate);
        }

        public Result<AccountCreatedEvent> Create()
        {
            //Validate if the account can be transfered
            return new AccountCreatedEvent(this);
        }

        public Result<AccountTransferedEvent> Transfer()
        {
            //Validate if the account can be transfered
            return new AccountTransferedEvent(this);
        }

        public void ChangeId(string? id)
        {
            if (!string.IsNullOrEmpty(id))
                Id = id;
        }
    }
}
