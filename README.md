# real-orders

## Overview

The Real-Time Order Tracking System is a modular, distributed system designed to efficiently process, track, and manage customer orders. It leverages modern microservice architecture, actor-based concurrency, and real-time messaging systems to handle order lifecycle from creation to fulfillment.

## Current Implementation

As of now, two key services have been implemented:

1. **OrderAPI:** RESTful API for managing orders and communicating with external systems.
2. **OrderProcessingService:** Actor-based service for handling the order lifecycle, inventory validation, and API updates.

## Project Structure

1. **OrderAPI**

A web service built using ASP.NET Core that exposes endpoints for order management and communicates with other services through Kafka.

### Key Features:

- CRUD operations for an Order.
- Manages order status updates.
- Produces Kafka events for downstream services (e.g., processing, inventory, notifications).
- Provides a centralized interface for external clients.

### Directory Overview

```shell
src/OrderAPI
├── Controllers           // API controllers for handling HTTP requests
├── Models                // Entity models (Order, Product, Customer, etc.)
├── Services              // Business logic for processing API requests
├── DTOs                  // Data Transfer Objects for structured input/output
├── Repositories          // Data access interfaces and implementations
├── Producers             // Kafka producers for publishing order events
├── Configurations        // Configuration files for API, Kafka, etc.
├── Program.cs            // Entry point for the API
├── Startup.cs            // Middleware and service registration
```

2. **OrderProcessingService**

A microservice built with Akka.NET to manage the order lifecycle and validate inventory in real-time. It interacts with the OrderAPI for status updates.

### Key Features:

- Implements Akka.NET actors to manage order workflows.
- Processes Kafka events related to new orders.
- Communicates with the InventoryService to ensure product availability. (Not yet done)

### Directory Overview

```shell
src/OrderProcessingService
├── Actors                // Akka.NET actors for order lifecycle states
│   ├── OrderActor.cs     // Handles individual order state and API updates
│   ├── InventoryActor.cs // Manages inventory validation
├── Services              // Kafka and Redis integration services
├── DTOs                  // Data Transfer Objects for actor communications
├── Consumers             // Kafka consumers for processing order events
├── Configurations        // Configuration files for Akka.NET, Kafka, Redis
├── Program.cs            // Entry point for the service
├── Startup.cs            // Middleware and service registration
```

### Workflow

1. Order Event Received:

- Processes Kafka messages containing new order details.
- Spawns an OrderActor to handle the order lifecycle.

2. OrderActor:

- Updates order status in the OrderAPI.
- Sends the order details to the InventoryActor for validation.

3. InventoryActor:

- Simulates inventory checks (integration with a future InventoryService).
- Updates the OrderAPI with the status InventoryChecked.

## Project set up.

1. **Clone the repository**

```shell
git clone git@github.com:cedricahenkorah/real-orders.git
```

2. **Navigate to the project directory**

```shell
cd real-orders
```

3. **Build the project**

```shell
dotnet build
```

4. **Run the OrderAPI**

```shell
cd src/OrderAPI
dotnet run
```

5. **Run the OrderProcessingService**

```shell
cd src/OrderProcessingService
dotnet run
```

## Future Enhancements

- Implement InventoryService for real inventory checks and caching.
- Add NotificationService for real-time updates to customers.
- Build the RealTimeTrackingDashboard for monitoring orders.
- Extend tests in tests/ for end-to-end coverage.
