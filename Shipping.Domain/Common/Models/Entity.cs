namespace Shipping.Domain.Common.Models;

public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; protected set; }

    protected Entity(TId id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity &&
               entity.GetType() == GetType() &&
               EqualityComparer<TId>.Default.Equals(Id, entity.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();
}
