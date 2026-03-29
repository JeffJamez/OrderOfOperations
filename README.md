# OrderOfOperations - Event Sourcing Architecture Demo

A comprehensive demonstration of event sourcing, CQRS (Command Query Responsibility Segregation), and event-driven architecture in an ecommerce context, leveraging EventStoreDB as the event-native database.

## Overview

This project showcases enterprise-grade patterns for building scalable ecommerce systems using:

- **Event Sourcing**: Storing application state as a sequence of events
- **CQRS**: Separating read and write operations for optimal performance
- **EventStoreDB**: The event-native database designed for event sourcing
- **Event-Driven Architecture**: Decoupled services communicating via events

## Architecture

The application demonstrates multiple microservices across different value streams:

| Value Stream | Module      | Language  | Event Store Library | Data Stores            |
| ------------ | ----------- | --------- | ------------------- | ---------------------- |
| Retail       | Cart        | C# / .NET | Eventuous           | MongoDB                |
| Catalog      | Products    | C# / .NET | Eventuous           | MongoDB, Elasticsearch |
| Catalog      | Prices      | C# / .NET | Eventuous           | PostgreSQL             |
| Supply Chain | Fulfillment | C# / .NET | Eventuous           | PostgreSQL             |

## Tech Stack

### Core Technologies

- **Runtime**: .NET 8.0
- **Language**: C#
- **Event Store**: EventStoreDB
- **CQRS Framework**: Eventuous
- **Messaging**: RabbitMQ, Kafka (planned)

### Data Stores

- **Event Store**: EventStoreDB
- **Read Models**: MongoDB, PostgreSQL, Elasticsearch, SQL Server

### Additional Tools

- **Docker**: Container orchestration
- **JetBrains Rider**: Development IDE

## Key Concepts Demonstrated

### Event Sourcing

Instead of storing current state, all changes are stored as an immutable sequence of events:

```
AccountCreated → MoneyDeposited → MoneyWithdrawn → AccountClosed
```

This provides:

- Complete audit trail
- Temporal queries (state at any point in time)
- Event replay for debugging
- Scalable write model

### CQRS Pattern

Separate models for reading and writing:

```
Commands (Write) → Command Handler → Aggregate → Event Store
                                    ↓
Queries (Read) ← Query Handler ← Read Model (Projection)
```

### Event-Driven Communication

Services communicate through events, enabling:

- Loose coupling
- Independent scalability
- Polyglot persistence

## Project Structure

```
src/
├── Cart/                    # Shopping cart module
│   ├── Commands/            # Write operations
│   ├── Queries/            # Read operations
│   ├── Events/              # Domain events
│   └── Projections/         # Read models
├── Products/                # Product catalog module
│   ├── Commands/
│   ├── Queries/
│   ├── Events/
│   └── Projections/
└── Prices/                  # Pricing module
    ├── Commands/
    ├── Queries/
    ├── Events/
    └── Projections/
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Docker & Docker Compose

### Build

```bash
# Build the solution
dotnet build
```

### Start Infrastructure

```bash
# Start databases and services
docker-compose up -d
```

### Run Services

```bash
# Navigate to module directory
cd src/Cart

# Run the API
dotnet run
```

## Implementation Highlights

### Aggregate Design

Domain aggregates handle business logic and enforce invariants:

```csharp
public class Cart : Aggregate<CartState, CartState.Builder>
{
    public void AddItem(ProductId productId, int quantity)
    {
        if (State.Items.Any(i => i.ProductId == productId))
            throw new DomainException("Item already in cart");

        Apply(new ItemAdded(productId, quantity));
    }
}
```

### Projections

Read models are built from event streams:

```csharp
public class CartProjection : Projection<CartView>
{
    public CartProjection()
    {
        On<ItemAdded>((view, evt) => view.AddItem(evt.ProductId, evt.Quantity));
        On<ItemRemoved>((view, evt) => view.RemoveItem(evt.ProductId));
    }
}
```

### Event Store Integration

Using Eventuous for seamless EventStoreDB integration:

```csharp
[Aggregate("carts")]
public class Cart : Aggregate<CartState>
{
    public void AddItem(AddItem cmd)
    {
        EnsureNotCancelled();
        EnsureDoesNotHaveItem(cmd.ProductId);

        Apply(new ItemAdded(cmd.CartId, cmd.ProductId, cmd.Quantity));
    }
}
```

## Benefits of This Architecture

1. **Scalability**: Write and read models scale independently
2. **Performance**: Optimized read models for specific queries
3. **Auditability**: Complete event history for compliance
4. **Flexibility**: Multiple read models from single event stream
5. **Resilience**: Event replay for recovery

## Resources

- Event Sourcing Guide
- CQRS Pattern
- Eventuous Documentation

## License

MIT License - see [LICENSE](LICENSE) for details.
