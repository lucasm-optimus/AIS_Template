
namespace AutoMais.Core.Domain.Aggregates.Account.Commands
{
    /// <summary>
    /// Abstract class used to any account change
    /// </summary>
    /// <typeparam name="T">The event of the operation completion</typeparam>
    public abstract class ChangeAccountCommand<T> : MediatR.IRequest<Result<T>>
    {
        public ChangeType Operation { get; set; }

        public string Id { get; set; }

        public string FullName { get; set; }

        public string Firstname { get; set; }

        public string LastName { get; set; }

        public string JobTitle { get; set; }

        public string Department { get; set; }

        public string Division { get; set; }

        public string Email { get; set; }

        public DateTime BlockingDate { get; set; }

        public IEnumerable<UserApplicationRoleDTO> Roles { get; set; }

    }
}
