
namespace AutoMais.Core.Domain.Aggregates.Account
{
    /// <summary>
    /// This entity represents an application where the User will have access
    /// </summary>
    public class Application : Entity
    {
        /// <summary>
        /// The unique of the application
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The application Name
        /// </summary>
        public string Name { get; private set; }

        public Application(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    /// <summary>
    /// The role assined to one specific application
    /// </summary>
    public class UserApplicationRole : Entity
    {
        public Application Application { get; private set; }
        public string RoleName { get; private set; }

        public UserApplicationRole(int applicationId, string applicationName, string roleName)
        {
            Application = new Application(applicationId, applicationName);
            RoleName = roleName;
        }

        public void ChangeRole(string roleName)
        {
            RoleName = roleName;
        }
    }
}
