
namespace Optimus.Core.Domain.Aggregates.Ticket
{
    public class Driver : Entity
    {
        public string CPF { get; private set; }
        public string Name { get; private set; }

        public static Driver Create(string cpf, string name = "Não informado")
        {
            return new Driver
            {
                CPF = cpf,
                Name = name
            };
        }
    }
}
