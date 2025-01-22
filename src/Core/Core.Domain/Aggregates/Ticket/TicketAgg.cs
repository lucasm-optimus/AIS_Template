using Optimus.Core.Domain.Aggregates.Product;
using Optimus.Core.Domain.Aggregates.Pump;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;
using Optimus.Core.Domain.Aggregates.Ticket.Events;
using Optimus.Core.Domain.Aggregates.Vehicle;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;

namespace Optimus.Core.Domain.Aggregates.Ticket;

public class TicketAgg : AggRoot
{
    public string Id { get; internal set; }
    public string Code { get; internal set; }
    public string Description { get; internal set; }
    public TicketStatusEnum Status { get { return GetState(); } internal set { } }
    public Attendant Attendant { get; internal set; }
    public Driver Driver { get; internal set; }
    public Vehicle Vehicle { get; internal set; }
    public Payment? Payment { get; internal set; }

    /// <summary>
    /// Date when the ticket was created
    /// </summary>
    public DateTime CreatedDate { get; internal set; }

    /// <summary>
    /// Date when the ticket was last updated
    /// </summary>
    public DateTime LastUpdatedDate { get; internal set; }

    /// <summary>
    /// Date when the ticket was abandoned by the system
    /// </summary>
    public DateTime? AbandonedDate { get; internal set; }

    /// <summary>
    /// Date when the ticket was sent to payment by the attendant
    /// </summary>
    public DateTime? SendToPaymentDate { get; internal set; }

    /// <summary>
    /// Date when the ticket was paid
    /// </summary>
    public DateTime? PaidDate { get; internal set; }

    /// <summary>
    /// Date when the ticket was reopened by the cashier
    /// </summary>
    public DateTime? ReopenedDate { get; internal set; }

    /// <summary>
    /// Date when the ticket was completely finished
    /// </summary>
    public DateTime? FinishedDate { get; private set; }

    public IEnumerable<Product> Products { get; internal set; } = new List<Product>();
    public IEnumerable<Supply> Supplies { get; internal set; } = new List<Supply>();

    public decimal ProductsTotalQuantity
    {
        get
        {
            return Products.Sum(p => p.Quantity);
        }
        private set
        {

        }
    }

    public decimal ProductsTotalPrice
    {
        get
        {
            return Products.Sum(p => p.Total);
        }
        private set
        {

        }
    }

    public decimal FuelTotalVolume
    {
        get
        {
            return Supplies.Sum(p => p.Quantity);
        }
        private set
        {

        }
    }

    public decimal FuelTotalPrice
    {
        get
        {
            return Supplies.Sum(p => p.Cost);
        }
        private set
        {

        }
    }

    public decimal TotalCost
    {
        get
        {
            return FuelTotalPrice + ProductsTotalPrice;
        }
        private set
        {

        }
    }


    /// <summary>
    /// Enable create a new Ticket only inside this namespace
    /// </summary>
    private TicketAgg()
    {

    }

    /// <summary>
    /// Creates a new Aggregate with all business rules to be validated
    /// </summary>
    /// <param name="plate"></param>
    private TicketAgg(Vehicle vehicle, Attendant attendant)
    {
        Id = Guid.NewGuid().ToString();
        Code = RandomNumberGenerator.GetString(
            choices: "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
            length: 6
        );
        Description = $"Ticket for {vehicle.Plate}";
        CreatedDate = DateTime.Now;
        LastUpdatedDate = DateTime.Now;
        Attendant = attendant;
        Status = TicketStatusEnum.Opened;
        Vehicle = vehicle;
    }


    /// <summary>
    /// Creates a new Aggregate with all business rules to be validated
    /// </summary>
    /// <param name="command"></param>
    private TicketAgg(CreateTicketForAttendantCommand command, Attendant attendant)
    {
        Id = Guid.NewGuid().ToString();
        Code = RandomNumberGenerator.GetString(
            choices: "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
            length: 6
        );
        Description = $"Ticket for Card {command.CardId}";
        CreatedDate = DateTime.Now;
        LastUpdatedDate = DateTime.Now;
        Attendant = attendant;
        Status = TicketStatusEnum.Opened;

        Supplies = new List<Supply>() {
            Supply.Create(command.Pump, command.Nozzle)
        };
    }

    public static Result<TicketAgg> Create(CreateTicketForAttendantCommand command, Attendant attendant)
    {
        var result = Result.Ok();
        //Add additional validations and return failures in Result if needed
        var agg = new TicketAgg(command, attendant);

        return result.ToResult(agg);
    }

    public static Result<TicketCreated> Create(Vehicle vehicle, Attendant attendant)
    {
        var result = Result.Ok();
        var agg = new TicketAgg(vehicle, attendant);

        return result.ToResult(agg.Created());
    }

    public TicketCreated Created()
    {
        return TicketCreated.Create(this);
    }

    


    #region Product Management

    public Result<TicketProductsChanged> AddProduct(AddProductToTicketCommand command, ProductAgg productAgg)
    {
        var result = Result.Ok();

        if (productAgg == null)
        {
            result.WithError("Product cannot be null or invalid");
            return result;
        }

        var productList = Products.ToList();

        var existing = productList.FirstOrDefault(x => x.Id == command.ProductId);
        var productQuantity = (existing?.Quantity ?? 0) + command.Quantity;

        if (productAgg.MaxItemsPerCart > 0 && productQuantity > productAgg.MaxItemsPerCart)
            result.WithValidationError("Quantity", $"Ticket exceeds maximum quantity of {productAgg.MaxItemsPerCart} of this product");

        if (existing != null)
            existing.ChangeQuantity(existing.Quantity + command.Quantity);
        else
            productList.Add(Product.Create(command, productAgg));

        Products = productList;

        this.LastUpdatedDate = DateTime.Now;

        return result.ToResult(TicketProductsChanged.Create(this));
    }

    public Result<TicketUpdated> ChangeProduct(ProductAgg product, decimal quantity)
    {
        var list = this.Products.ToList();
        var index = list.FindIndex(x => x.Id == product.Id);
        if (index < 0)
            return Result.Fail<TicketUpdated>("Product not found").WithValidationError("ProductId", "Ticket does not contain this product");

        list[index].ChangeQuantity(quantity);

        this.Products = list;
        this.LastUpdatedDate = DateTime.Now;

        return this.Updated();
    }

    public Result<TicketUpdated> RemoveProduct(RemoveProductFromTicketCommand command)
    {
        var list = this.Products.ToList();
        var index = list.FindIndex(x => x.Id == command.ProductId);
        if (index < 0)
            return Result.Fail<TicketUpdated>("Product not found").WithValidationError("ProductId", "Ticket does not contain this product");

        list.RemoveAt(index);
        this.Products = list;
        this.LastUpdatedDate = DateTime.Now;

        return this.Updated();
    }

    #endregion

    public Result<TicketUpdated> ChangeDriver(ChangeTicketDriverCommand command)
    {
        this.Driver = Driver.Create(command.CPF);
        this.LastUpdatedDate = DateTime.Now;

        return this.Updated();
    }

    public Result<TicketUpdated> ChangeVehicle(VehicleAgg vehicle)
    {
        var lastKms = this.Vehicle?.Kilometers ?? 0;

        this.Vehicle = Vehicle.Create(vehicle, lastKms);
        this.LastUpdatedDate = DateTime.Now;

        return this.Updated();
    }

    public Result<TicketUpdated> ChangePayment(Payment payment)
    {
        this.Payment = payment;
        this.LastUpdatedDate = DateTime.Now;

        return this.Updated();
    }

    public Result<TicketUpdated> ChangeKilometers(int kms)
    {
        if (this.Vehicle is null)
            this.Vehicle = Vehicle.Create(kms);

        this.Vehicle?.ChangeKilometers(kms);
        this.LastUpdatedDate = DateTime.Now;

        return this.Updated();
    }


    #region State Management

    private TicketStatusEnum GetState()
    {
        if (this.FinishedDate is not null)
            return TicketStatusEnum.Finished;

        if (this.AbandonedDate is not null)
            return TicketStatusEnum.Abandoned;

        if (this.PaidDate is not null)
            return TicketStatusEnum.Paid;

        if (this.SendToPaymentDate is not null || this.ReopenedDate is not null)
            return TicketStatusEnum.WaitingPayment;

        if (AnyRefuelingInProgress())
            return TicketStatusEnum.InProgress;

        if (AllRefuelingFinished())
            return TicketStatusEnum.Finished;

        return TicketStatusEnum.Opened;
    }

    private bool AnyRefuelingInProgress()
    {
        return this.Supplies.Any(x => x.Status == SupplyStatus.InProgress);
    }

    private bool AllRefuelingFinished()
    {
        return this.Supplies.Count() != 0 && this.Supplies.All(x => x.Status == SupplyStatus.Finished);
    }

    /// <summary>
    /// Should make this ticket in progress starting the fueling process
    /// </summary>
    /// <returns></returns>
    public Result<TicketUpdated> StartFueling()
    {
        var canChange = ValidateChange();
        if (canChange.IsFailed)
            return canChange;

        //TODO: Call AddOrUpdateRefuelling

        return this.Updated();
    }

    /// <summary>
    /// Should mark this ticket as Waiting Payment
    /// </summary>
    /// <returns></returns>
    public Result<TicketUpdated> RequestPayment()
    {
        var canChange = ValidateChange();
        if (canChange.IsFailed)
            return canChange;

        if (AnyRefuelingInProgress())
            return Result.Fail<TicketUpdated>("Refueling In Progress. Cannot request payment yet.");

        if (this.Status == TicketStatusEnum.WaitingPayment)
            return Result.Fail<TicketUpdated>("Ticket is already waiting payment");

        this.SendToPaymentDate = DateTime.Now;
        return this.Updated();
    }

    /// <summary>
    /// Should mark this ticket as Waiting Payment
    /// </summary>
    /// <returns></returns>
    public Result<TicketUpdated> ReopenPayment()
    {
        var canChange = ValidateChange();
        if (canChange.IsFailed)
            return canChange;

        if (this.SendToPaymentDate?.AddMinutes(5) < DateTime.Now)
            return Result.Fail<TicketUpdated>("Ticket cannot be reopened after 5 minutes");

        if (Status != TicketStatusEnum.Paid)
            return Result.Fail<TicketUpdated>("Ticket is not paid");

        this.PaidDate = null;
        this.ReopenedDate = DateTime.Now;
        return this.Updated();
    }

    /// <summary>
    /// Should mark this ticket as Paid
    /// </summary>
    /// <returns></returns>
    public Result<TicketUpdated> Pay()
    {
        var canChange = ValidateChange();
        if (canChange.IsFailed)
            return canChange;

        if (Status != TicketStatusEnum.WaitingPayment)
            return Result.Fail<TicketUpdated>("Ticket is not waiting payment");

        this.PaidDate = DateTime.Now;
        return this.Updated();
    }
    /// <summary>
    /// Should make this ticket abandoned
    /// </summary>
    /// <returns></returns>
    public Result<TicketUpdated> Abandon()
    {
        var canChange = ValidateChange();
        if (canChange.IsFailed)
            return canChange;

        this.AbandonedDate = DateTime.Now;
        return this.Updated();
    }

    public Result<TicketUpdated> Finish()
    {
        var canChange = ValidateChange();
        if (canChange.IsFailed)
            return canChange;

        this.FinishedDate = DateTime.Now;
        return this.Updated();
    }

    private Result<TicketUpdated> ValidateChange()
    {
        if (this.FinishedDate is not null)
            return Result.Fail<TicketUpdated>("Ticket is already finished");

        if (this.AbandonedDate is not null)
            return Result.Fail<TicketUpdated>("Ticket is abandoned");

        return Result.Ok<TicketUpdated>(null);
    }

    private Result<TicketUpdated> Updated()
    {
        this.LastUpdatedDate = DateTime.Now;
        return TicketUpdated.Create(this);
    }


    #endregion


    #region Refueling Management

    public Result<TicketUpdated> AddOrUpdateSupply(AuthorizeRefuelingForTicketCommand command)
    {
        return UpdateSupply(command.PumpNumber, command.NozzleNumber, 0, 0, command.Pump, command.Nozzle);
    }

    public Result<TicketUpdated> AddOrUpdateSupply(AddFuelToTicketCommand command)
    {
        return UpdateSupply(command.PumpNumber, command.NozzleNumber, command.Quantity, command.Cost, command.Pump, command.Nozzle);
    }

    public Result<TicketUpdated> AddOrUpdateSupply(UpdateFuelToTicketCommand command)
    {
        return UpdateSupply(command.PumpNumber, command.NozzleNumber, command.Quantity, command.Cost, command.Pump, command.Nozzle);
    }

    public Result<TicketUpdated> FinishSupply(FinishSupply command)
    {
        var list = this.Supplies.ToList();
        var index = list.FindIndex(x => x.Pump.Number == command.PumpNumber && x.Pump.Nozzle.Number == command.NozzleNumber);
        if (index >= 0)
        {
            list[index].Finish();
        }
        else
        {
            return Result.Fail<TicketUpdated>("Supply not found").WithValidationError("PumpNumber", "Supply not found");
        }

        Supplies = list;
        this.LastUpdatedDate = DateTime.Now;

        return this.Updated();
    }

    private Result<TicketUpdated> UpdateSupply(int pumpNumber, int nozzleNumber, decimal quantity, decimal cost, PumpAgg pump, Nozzle nozzle)
    {
        var list = this.Supplies.ToList();
        var index = list.FindIndex(x => x.Pump.Number == pumpNumber && x.Pump.Nozzle.Number == nozzleNumber);
        if (index >= 0)
        {
            list[index].ChangeQuantityAndCost(quantity, cost);
        }
        else
        {
            list.Add(Supply.Create(pump, nozzle, quantity, cost));
        }

        Supplies = list;
        this.LastUpdatedDate = DateTime.Now;

        return this.Updated();
    }

    #endregion

}
