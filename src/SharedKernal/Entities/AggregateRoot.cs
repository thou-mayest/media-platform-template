using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernal.Messaging;

namespace SharedKernal.Entities
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        protected AggregateRoot(Guid id) : base(id) { }
        protected AggregateRoot() { }

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
            _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
