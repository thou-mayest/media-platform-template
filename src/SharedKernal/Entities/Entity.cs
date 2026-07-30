namespace SharedKernal.Entities
{
    public abstract class Entity : BaseEntity
    {
        public Guid Id { get; protected init; }

        protected Entity(Guid id)
        {
            Id = id;
        }

        protected Entity() { } // for ORM materialization

        public override bool Equals(object? obj)
        {
            if (obj is not Entity other) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            return Id == other.Id;
        }

        public override int GetHashCode() => HashCode.Combine(GetType(), Id);

        public static bool operator ==(Entity? a, Entity? b) => a?.Equals(b) ?? b is null;
        public static bool operator !=(Entity? a, Entity? b) => !(a == b);
    }
}