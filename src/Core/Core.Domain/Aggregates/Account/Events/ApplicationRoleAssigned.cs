
namespace AutoMais.Core.Domain.Aggregates.Account.Events
{
    public class ApplicationRoleAssigned : MediatR.INotification
    {
        public ApplicationRoleAssigned(UserApplicationRole userApplicationRole)
        {

        }
    }
}
