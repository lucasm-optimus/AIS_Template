using Optimus.Core.Domain.Aggregates.Pump.Commands;
using Optimus.Core.Domain.Aggregates.Pump.Events;

namespace Optimus.Core.Domain.Aggregates.Pump;

public class PumpAgg : AggRoot
{
    public Guid Id { get; internal set; }
    public int Number { get; internal set; }
    public string Description { get; internal set; }
    public IEnumerable<Nozzle> Nozzles { get; internal set; } = new List<Nozzle>();



    public Result<NozzleCreated> AddNozzle(CreateNozzleCommand command)
    {
        var result = Result.Ok();

        var nozzles = Nozzles.ToList();

        if (nozzles.Any(x => x.Number == command.Number))
            return result.WithValidationError("Nozzle", $"Nozzle No.{command.Number} already exists");

        var nozzle = Nozzle.Create(command);
        nozzles.Add(nozzle);

        Nozzles = nozzles;

        return Result.Ok(new NozzleCreated(nozzle));
    }

    public Result<NozzleRemoved> RemoveNozzle(int nozzleNumber)
    {
        if (Nozzles.Any(x => x.Number == nozzleNumber))
        {
            var nozzles = Nozzles.ToList();
            nozzles.Remove(nozzles.First(x => x.Number == nozzleNumber));
            Nozzles = nozzles;
            return Result.Ok(new NozzleRemoved());
        }
        return Result.Fail("Nozzle not found");
    }


    public Result<PumpCreated>? Created()
    {
        return Result.Ok(new PumpCreated(this));
    }


    public static Result<PumpAgg> Create(CreateNewPumpCommand command)
    {
        var result = Result.Ok();

        var pump = new PumpAgg
        {
            Id = Guid.NewGuid(),
            Number = command.Number,
            Description = command.Description
        };

        return result.ToResult(pump);
    }

    public void ChangeNozzle(Nozzle nozzle)
    {
        var list = this.Nozzles.ToList();
        var index = list.FindIndex(x => x.Number == nozzle.Number);

        list[index] = nozzle;

        this.Nozzles = list;
    }
}