using Optimus.Core.Application.Ticket.Adapters;
using Optimus.Core.Domain.Aggregates.Ticket;
using Optimus.Core.Domain.Aggregates.Ticket.Commands;

namespace Optimus.States.Mongo.Repositories.Ticket;

public class TicketRepository : MongoRepositoryBase<TicketAgg>, ITicketState
{
    public TicketRepository(IMongoDatabase database) : base(database, "Tickets")
    {

    }

    public Task<Result<TicketAgg>> GetOpenedTicket(FinishSupply command)
    {
        return GetOpenedTicket(command.PumpNumber, command.NozzleNumber);
    }

    public async Task<Result<TicketAgg>> GetOpenedTicket(AuthorizeRefuelingForTicketCommand command)
    {
        var result = await db.Find(x =>
            x.Id == command.TicketId.ToString() &&
            (x.Status == TicketStatusEnum.Opened || x.Status == TicketStatusEnum.InProgress))
            .FirstOrDefaultAsync();

        if (result != null)
            return Result.Ok(result);

        return Result.Fail<TicketAgg>($"No Ticket opened or in progress for Pump {command.PumpNumber} and Nozzle {command.NozzleNumber}");
    }

    public Task<Result<TicketAgg>> GetOpenedTicket(AddFuelToTicketCommand command)
    {
        return GetOpenedTicket(command.CardId, command.PumpNumber, command.NozzleNumber);
    }

    public async Task<Result<TicketAgg>> GetOpenedTicket(string cardId, int pumpNumber, int nozzleNumber)
    {
        var result = await db.Find(x =>
            x.Attendant.CardId == cardId &&
            (x.Status == TicketStatusEnum.Opened || x.Status == TicketStatusEnum.InProgress) && 
            x.Supplies.Any(s => s.Pump.Number == pumpNumber && s.Pump.Nozzle.Number == nozzleNumber))
            .FirstOrDefaultAsync();

        if (result != null)
            return Result.Ok(result);

        return Result.Fail<TicketAgg>($"No Ticket opened or in progress for Pump {pumpNumber} and Nozzle {nozzleNumber}");
    }

    public Task<Result<TicketAgg>> GetOpenedTicket(UpdateFuelToTicketCommand command)
    {
        return GetOpenedTicket(command.PumpNumber, command.NozzleNumber);
    }

    public async Task<Result<TicketAgg>> GetOpenedTicket(int pumpNumber, int nozzleNumber)
    {
        var result = await db.Find(x =>
            (x.Status == TicketStatusEnum.Opened || x.Status == TicketStatusEnum.InProgress) &&
            x.Supplies.Any(s => s.Pump.Number == pumpNumber && s.Pump.Nozzle.Number == nozzleNumber))
            .FirstOrDefaultAsync();

        if (result != null)
            return Result.Ok(result);

        return Result.Fail<TicketAgg>($"No Ticket opened or in progress for Pump {pumpNumber} and Nozzle {nozzleNumber}");
    }

    //public async Task<Result<TicketAgg>> AddAsync(TicketAgg ticket)
    //{
    //    var result = await base.SaveAsync(TicketModel.FromDomain(ticket));

    //    if (result.IsSuccess)
    //    {
    //        return Result.Ok(ticket);
    //    }
    //    else
    //        return Result.Fail<TicketAgg>("Insert failed").WithErrors(result.Errors);
    //}

    //public Result<TicketAgg> GetTicket(string id)
    //{
    //    var model = db.Find(p => p.ID == id).FirstOrDefault();

    //    return TicketModel.ToDomain(model);
    //}

    //public Result<TicketAgg> GetTicket(string id, string attendantId)
    //{
    //    throw new NotImplementedException();
    //}

    //public Result<IEnumerable<TicketAgg>> GetTickets(QueryManyBase queryMany)
    //{
    //    throw new NotImplementedException();
    //}

    //#region Write
    //public bool Add(Core.Domain.Aggregates.Ticket.TicketAgg ticket)
    //{

    //    var state = new ConnectionMongo(stateContext, Dp);
    //    var _client = ToState(client);
    //    state.Client.InsertOne(_client);
    //    return true;
    //}
    //public bool Delete(Guid clientID)
    //{
    //    var result = Dp.Pipeline(ExecuteResult: (stateContext) =>
    //    {
    //        var state = new ConnectionMongo(stateContext, Dp);
    //        state.Client.DeleteOne(p => p.ID == clientID);
    //        return true;
    //    });
    //    if (result is null)
    //        return false;
    //    return result;
    //}
    //public bool Update(Domain.Aggregates.Client.Client client)
    //{
    //    var result = Dp.Pipeline(ExecuteResult: (stateContext) =>
    //    {
    //        var state = new ConnectionMongo(stateContext, Dp);
    //        var _client = ToState(client);
    //        _client._Id = state.Client.Find(p => p.ID == client.ID).FirstOrDefault()._Id;
    //        state.Client.ReplaceOne(p => p.ID == client.ID, _client);
    //        return true;
    //    });
    //    if (result is null)
    //        return false;
    //    return result;
    //}

    //#endregion Write

    //#region Read
    //public Domain.Aggregates.Client.Client Get(Guid clientID)
    //{
    //    return Dp.Pipeline(ExecuteResult: (stateContext) =>
    //    {
    //        var state = new ConnectionMongo(stateContext, Dp);
    //        var client = state.Client.Find(p => p.ID == clientID).FirstOrDefault();
    //        var _client = ToDomain(client);
    //        return _client;
    //    });
    //}
    //public List<Domain.Aggregates.Client.Client> GetAll(int? limit, int? offset, string ordering, string sort, string filter)
    //{
    //    return Dp.Pipeline(ExecuteResult: (stateContext) =>
    //    {
    //        var state = new ConnectionMongo(stateContext, Dp);
    //        List<Model.Client> client = null;
    //        if (sort?.ToLower() == "desc")
    //        {
    //            var result = state.Client.Find(GetFilter(filter)).SortByDescending(GetOrdering(ordering));
    //            if (limit != null && offset != null)
    //                client = result.Skip((offset - 1) * limit).Limit(limit).ToList();
    //            else
    //                client = result.ToList();
    //        }
    //        else if (sort?.ToLower() == "asc")
    //        {
    //            var result = state.Client.Find(GetFilter(filter)).SortBy(GetOrdering(ordering));
    //            if (limit != null && offset != null)
    //                client = result.Skip((offset - 1) * limit).Limit(limit).ToList();
    //            else
    //                client = result.ToList();
    //        }
    //        else
    //        {
    //            var result = state.Client.Find(GetFilter(filter));
    //            if (limit != null && offset != null)
    //                client = result.Skip((offset - 1) * limit).Limit(limit).ToList();
    //            else
    //                client = result.ToList();
    //        }
    //        var _client = ToDomain(client);
    //        return _client;
    //    });
    //}
    //private Expression<Func<Model.Client, object>> GetOrdering(string field)
    //{
    //    Expression<Func<Model.Client, object>> exp = p => p.ID;
    //    if (!string.IsNullOrWhiteSpace(field))
    //    {
    //        if (field.ToLower() == "name")
    //            exp = p => p.Name;
    //        else if (field.ToLower() == "cnpj")
    //            exp = p => p.CNPJ;
    //        else if (field.ToLower() == "supportemail")
    //            exp = p => p.SupportEmail;
    //        else if (field.ToLower() == "isactive")
    //            exp = p => p.IsActive;
    //        else if (field.ToLower() == "activationdate")
    //            exp = p => p.ActivationDate;
    //        else if (field.ToLower() == "deactivationdate")
    //            exp = p => p.DeactivationDate;
    //        else if (field.ToLower() == "subscriptionid")
    //            exp = p => p.SubscriptionId;
    //        else
    //            exp = p => p.ID;
    //    }
    //    return exp;
    //}
    //private FilterDefinition<Model.Client> GetFilter(string filter)
    //{
    //    var builder = Builders<Model.Client>.Filter;
    //    FilterDefinition<Model.Client> exp;
    //    string Name = string.Empty;
    //    string CNPJ = string.Empty;
    //    string SupportEmail = string.Empty;
    //    bool? IsActive = null;
    //    DateTime? ActivationDate = null;
    //    DateTime? DeactivationDate = null;
    //    Guid? SubscriptionId = null;
    //    if (!string.IsNullOrWhiteSpace(filter))
    //    {
    //        var conditions = filter.Split(",");
    //        if (conditions.Count() >= 1)
    //        {
    //            foreach (var condition in conditions)
    //            {
    //                var slice = condition?.Split("=");
    //                if (slice.Length > 1)
    //                {
    //                    var field = slice[0];
    //                    var value = slice[1];
    //                    if (field.ToLower() == "name")
    //                        Name = value;
    //                    else if (field.ToLower() == "cnpj")
    //                        CNPJ = value;
    //                    else if (field.ToLower() == "supportemail")
    //                        SupportEmail = value;
    //                    else if (field.ToLower() == "isactive")
    //                        IsActive = Convert.ToBoolean(value);
    //                    else if (field.ToLower() == "activationdate")
    //                        ActivationDate = Convert.ToDateTime(value);
    //                    else if (field.ToLower() == "deactivationdate")
    //                        DeactivationDate = Convert.ToDateTime(value);
    //                    else if (field.ToLower() == "subscriptionid")
    //                        SubscriptionId = new Guid(value);
    //                }
    //            }
    //        }
    //    }
    //    var bfilter = builder.Empty;
    //    if (!string.IsNullOrWhiteSpace(Name))
    //    {
    //        var NameFilter = builder.Eq(x => x.Name, Name);
    //        bfilter &= NameFilter;
    //    }
    //    if (!string.IsNullOrWhiteSpace(CNPJ))
    //    {
    //        var CNPJFilter = builder.Eq(x => x.CNPJ, CNPJ);
    //        bfilter &= CNPJFilter;
    //    }
    //    if (!string.IsNullOrWhiteSpace(SupportEmail))
    //    {
    //        var SupportEmailFilter = builder.Eq(x => x.SupportEmail, SupportEmail);
    //        bfilter &= SupportEmailFilter;
    //    }
    //    if (IsActive != null)
    //    {
    //        var IsActiveFilter = builder.Eq(x => x.IsActive, IsActive);
    //        bfilter &= IsActiveFilter;
    //    }
    //    if (ActivationDate != null)
    //    {
    //        var ActivationDateFilter = builder.Eq(x => x.ActivationDate, ActivationDate);
    //        bfilter &= ActivationDateFilter;
    //    }
    //    if (DeactivationDate != null)
    //    {
    //        var DeactivationDateFilter = builder.Eq(x => x.DeactivationDate, DeactivationDate);
    //        bfilter &= DeactivationDateFilter;
    //    }
    //    if (SubscriptionId != null)
    //    {
    //        var SubscriptionIdFilter = builder.Eq(x => x.SubscriptionId, SubscriptionId);
    //        bfilter &= SubscriptionIdFilter;
    //    }
    //    exp = bfilter;
    //    return exp;
    //}
    //public bool Exists(Guid clientID)
    //{
    //    var result = Dp.Pipeline(ExecuteResult: (stateContext) =>
    //    {
    //        var state = new ConnectionMongo(stateContext, Dp);
    //        var client = state.Client.Find(x => x.ID == clientID).Project<Model.Client>("{ ID: 1 }").FirstOrDefault();
    //        return clientID == client?.ID;
    //    });
    //    if (result is null)
    //        return false;
    //    return result;
    //}
    //public long Total(string filter)
    //{
    //    return Dp.Pipeline(ExecuteResult: (stateContext) =>
    //    {
    //        var state = new ConnectionMongo(stateContext, Dp);
    //        var total = state.Client.Find(GetFilter(filter)).CountDocuments();
    //        return total;
    //    });
    //}

    //#endregion Read

    //#region mappers
    //public static DevPrime.State.Repositories.Client.Model.Client ToState(Domain.Aggregates.Client.Client client)
    //{
    //    if (client is null)
    //        return new DevPrime.State.Repositories.Client.Model.Client();
    //    DevPrime.State.Repositories.Client.Model.Client _client = new DevPrime.State.Repositories.Client.Model.Client();
    //    _client.ID = client.ID;
    //    _client.Name = client.Name;
    //    _client.CNPJ = client.CNPJ;
    //    _client.SupportEmail = client.SupportEmail;
    //    _client.IsActive = client.IsActive;
    //    _client.ActivationDate = client.ActivationDate;
    //    _client.DeactivationDate = client.DeactivationDate;
    //    _client.SubscriptionId = client.SubscriptionId;
    //    return _client;
    //}
    //public static Domain.Aggregates.Client.Client ToDomain(DevPrime.State.Repositories.Client.Model.Client client)
    //{
    //    if (client is null)
    //        return new Domain.Aggregates.Client.Client()
    //        {
    //            IsNew = true
    //        };
    //    Domain.Aggregates.Client.Client _client = new Domain.Aggregates.Client.Client(client.ID, client.Name, client.CNPJ, client.SupportEmail, client.IsActive, client.ActivationDate, client.DeactivationDate, client.SubscriptionId);
    //    return _client;
    //}
    //public static List<Domain.Aggregates.Client.Client> ToDomain(IList<DevPrime.State.Repositories.Client.Model.Client> clientList)
    //{
    //    List<Domain.Aggregates.Client.Client> _clientList = new List<Domain.Aggregates.Client.Client>();
    //    if (clientList != null)
    //    {
    //        foreach (var client in clientList)
    //        {
    //            Domain.Aggregates.Client.Client _client = new Domain.Aggregates.Client.Client(client.ID, client.Name, client.CNPJ, client.SupportEmail, client.IsActive, client.ActivationDate, client.DeactivationDate, client.SubscriptionId);
    //            _clientList.Add(_client);
    //        }
    //    }
    //    return _clientList;
    //}

    //#endregion mappers
}
