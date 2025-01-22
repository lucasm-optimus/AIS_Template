
using AutoMais.Core.Domain.Aggregates.Account.Events;

namespace AutoMais.Core.Domain.Aggregates.Account.Commands
{

    /// <summary>
    /// This entity represents an application where the User will have access
    /// </summary>
    public class ApplicationDTO
    {
        /// <summary>
        /// The unique of the application
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The application Name
        /// </summary>
        public string Name { get; set; }

        public ApplicationDTO()
        {

        }
    }

    /// <summary>
    /// The role assined to one specific application
    /// </summary>
    public class UserApplicationRoleDTO
    {
        public Application Application { get; set; }
        public string RoleName { get; set; }

        public UserApplicationRoleDTO()
        {
        }

        internal UserApplicationRole ToDomain()
        {
            return new UserApplicationRole(this.Application.Id, this.Application.Name, this.RoleName);
        }
    }


    /// <summary>
    /// Represents change request to an Account
    /// </summary>
    public class AccountChangeRequestedCommand : MediatR.IRequest<Result<string>>
    {
        public string Id { get; set; }

        public ChangeType Operation { get; set; }

        public string FullName { get; set; }

        public string Firstname { get; set; }

        public string LastName { get; set; }

        public string JobTitle { get; set; }

        public string Department { get; set; }

        public string Division { get; set; }

        public string Email { get; set; }

        public DateTime BlockingDate { get; set; }

        public IEnumerable<UserApplicationRoleDTO> ApplicationRoles { get; set; }

        /// <summary>
        /// Change the behavior of the command to be a Block command
        /// </summary>
        public void MakeItBlock(string id)
        {
            Operation = ChangeType.Block;
        }

        /// <summary>
        /// Change the behavior of the command to be a Create command
        /// </summary>
        public void MakeItCreate()
        {
            Operation = ChangeType.Create;
        }

        /// <summary>
        /// Change the behavior of the command to be a Transfer command
        /// </summary>
        public void MakeItTransfer(string id)
        {
            Operation = ChangeType.Transfer;
        }


        public AccountChangeRequestedEvent ToEvent()
        {
            return new AccountChangeRequestedEvent()
            {
                Id = Id,
                Operation = Operation,
                FullName = FullName,
                Firstname = Firstname,
                LastName = LastName,
                JobTitle = JobTitle,
                Department = Department,
                Division = Division,
                Email = Email,
                BlockingDate = BlockingDate,
                ApplicationRoles = ApplicationRoles
            };
        }
    }
}
