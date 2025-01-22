using Optimus.Core.Domain.Aggregates.Pump.Commands;
using Optimus.Core.Domain.Aggregates.Pump.Events;

namespace Optimus.Core.Domain.Aggregates.Pump;

public class Nozzle : Entity
{
    public int Number { get; internal set; }
    public string Description { get; internal set; }
    public string Color { get; internal set; }
    public Fuel Fuel { get; internal set; }


    public Result<FuelChanged> ChangeFuel(ChangeNozzleFuelCommand command)
    {
        Fuel = Fuel.Create(command);

        return Result.Ok(new FuelChanged(Fuel));
    }


    public static Nozzle Create(CreateNozzleCommand command)
    {
        return new Nozzle
        {
            Number = command.Number,
            Description = command.Description,
            Color = command.Color
        };
    }
}


public record Fuel
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Color { get; private set; }
    public decimal Price { get; private set; }


    public static Fuel Create(ChangeNozzleFuelCommand command)
    {
        return new Fuel
        {
            Id = command.Id,
            Name = command.Name,
            Color = command.Color,
            Price = command.Price
        };
    }
}