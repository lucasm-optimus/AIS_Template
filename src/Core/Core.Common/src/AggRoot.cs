namespace Tilray.Integrations.Core.Common;

/// <summary>
/// Represents an Aggregate Root
/// </summary>
public abstract class AggRoot;

/// <summary>
/// Represents an Entity
/// </summary>
public abstract class Entity;

public interface IDomainEvent : MediatR.INotification;

public interface IDomainEventHandler<T> : MediatR.INotificationHandler<T> where T : IDomainEvent;

public interface ICommand : MediatR.IRequest<Result>;

public interface ICommand<R> : MediatR.IRequest<Result<R>> where R : IDomainEvent;

public interface ICommandHandler<T> : MediatR.IRequestHandler<T, Result> where T : ICommand;

public interface ICommandHandler<T, R> : MediatR.IRequestHandler<T, Result<R>> where T : ICommand<R> where R : IDomainEvent;

public interface IQueryHandler<T, R> : MediatR.IRequestHandler<T, Result<R>> where T : IQuery<R> where R : class;

public interface IQueryManyHandler<T, R> : MediatR.IRequestHandler<T, Result<IEnumerable<R>>> where T : IQueryMany<R> where R : class;

public interface IQuery<T> : MediatR.IRequest<Result<T>>;

public interface IQueryMany<R> : MediatR.IRequest<Result<IEnumerable<R>>>
{
    public int Page { get; set; }
    public int ItemsPerPage { get; set; }
    public bool MustSort { get; set; }
    public bool MultiSort { get; set; }

    public IEnumerable<string> SortBy { get; set; }
    public IEnumerable<bool> SortDesc { get; set; }
    public IEnumerable<string> GroupBy { get; set; }
    public IEnumerable<string> GroupDesc { get; set; }
    public IEnumerable<QueryHeader> Headers { get; set; }
}

public abstract record QueryManyBase<T> : IQueryMany<T>
{
    public int Page { get; set; }
    public int ItemsPerPage { get; set; }
    public bool MustSort { get; set; }
    public bool MultiSort { get; set; }

    public IEnumerable<string> SortBy { get; set; }
    public IEnumerable<bool> SortDesc { get; set; }
    public IEnumerable<string> GroupBy { get; set; }
    public IEnumerable<string> GroupDesc { get; set; }
    public IEnumerable<QueryHeader> Headers { get; set; }

}

public class QueryHeader
{
    //Display text
    public string Text { get; set; }
    //Data value field, same as ViewModel
    public string Value { get; set; }
    //Text to be filtered
    public string Filter { get; set; }
}
