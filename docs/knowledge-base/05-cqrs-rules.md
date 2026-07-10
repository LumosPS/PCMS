# 05 — CQRS Rules

## Design Decision

PCMS uses CQRS with MediatR.

Use MediatR 12.x only. It is the last Apache 2.0 licensed line; MediatR 13+ is commercially licensed. Do not upgrade past 12.x and do not switch dispatch mechanisms unless the architecture decision is explicitly changed.

CQRS separates write workflows from read queries and keeps clinical, financial, and tenant-sensitive operations explicit.

---

## Command Rules

A command changes system state.

Examples:

- `CreatePatientCommand`
- `UpdateDoctorCommand`
- `CreateAppointmentCommand`
- `CancelAppointmentCommand`
- `CheckInAppointmentCommand`
- `IssueInvoiceCommand`
- `AddInvoicePaymentCommand`

Commands must not return full entity data.

Allowed command return types:

- `Result`
- `Result<Guid>`
- `Result<TSmallCommandResult>`

Examples:

```csharp
public sealed record CreateAppointmentResult(Guid AppointmentId);
public sealed record IssueInvoiceResult(Guid InvoiceId, string InvoiceNumber);
```

Commands must not return EF Core entities or domain entities.

---

## Query Rules

A query reads data.

Examples:

- `GetPatientsQuery`
- `GetPatientByIdQuery`
- `GetAppointmentsQuery`
- `GetInvoiceByIdQuery`

Queries return DTOs or paged results.

Queries must not change system state.

Queries must not return domain entities directly.

---

## Core Interfaces

Commands and queries stay explicit. Define thin markers over MediatR's `IRequest` so the command/query distinction is visible in the type system:

```csharp
public interface ICommand : IRequest<Result> { }

public interface ICommand<TResult> : IRequest<Result<TResult>> { }

public interface ICommandHandler<TCommand>
    : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{ }

public interface ICommandHandler<TCommand, TResult>
    : IRequestHandler<TCommand, Result<TResult>>
    where TCommand : ICommand<TResult>
{ }
```

```csharp
public interface IQuery<TResult> : IRequest<TResult> { }

public interface IQueryHandler<TQuery, TResult>
    : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{ }
```

Handlers implement `ICommandHandler`/`IQueryHandler`, never raw `IRequestHandler` directly.

Dispatch through `ISender` only:

- Controllers inject `ISender` and call `Send`.
- Do not inject `IMediator` or `IPublisher`.
- Do not use MediatR notifications (`INotification`) or streaming unless the architecture decision is explicitly extended.

---

## Registration and Pipeline Behaviors

Register once in the API layer:

```csharp
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));
```

Pipeline behaviors are allowed only for cross-cutting concerns, kept minimal:

- `ValidationBehavior` — runs FluentValidation validators before the handler; short-circuits with a failed `Result`.
- `LoggingBehavior` — optional, no clinical or financial payload data in logs.

Do not put tenant resolution, authorization decisions, or business rules in behaviors. Tenant context comes from middleware; authorization is enforced in handlers.

---

## Naming Convention

| File Type | Pattern | Example |
|---|---|---|
| Command | `[Action][Entity]Command.cs` | `CreateAppointmentCommand.cs` |
| Command Handler | `[Action][Entity]Handler.cs` | `CreateAppointmentHandler.cs` |
| Query | `Get[Entity/Entities]Query.cs` | `GetAppointmentsQuery.cs` |
| Query Handler | `Get[Entity/Entities]Handler.cs` | `GetAppointmentsHandler.cs` |
| DTO | `[Entity]Dto.cs` | `AppointmentDto.cs` |
| Result | `[Action][Entity]Result.cs` | `CreateAppointmentResult.cs` |
| Validator | `[Action][Entity]Validator.cs` | `CreateAppointmentValidator.cs` |

---

## Command Handler Responsibilities

A command handler should:

1. Validate input.
2. Verify tenant context.
3. Verify authorization.
4. Load required entities.
5. Enforce business rules.
6. Modify domain state through domain methods.
7. Save using Unit of Work.
8. Create audit logs for sensitive actions.
9. Return a safe `Result`.

Good:

```csharp
appointment.Cancel(userId, reason, utcNow);
invoice.AddPayment(payment, userId, utcNow);
```

Bad:

```csharp
appointment.Status = AppointmentStatus.Cancelled;
invoice.PaidAmount += command.Amount;
```

---

## Query Handler Responsibilities

A query handler should:

1. Verify tenant context.
2. Verify authorization.
3. Build the read query.
4. Apply tenant-safe filtering through `TenantDbContext`.
5. Apply filters, sorting, and pagination.
6. Project to DTO.
7. Return DTO or `PagedResult<T>`.

List queries must be paginated unless explicitly approved.

---

## Tenant and Authorization Rules

Commands and queries must not accept `TenantId`, database name, or connection string from normal request bodies.

Tenant context must come from server-side tenant resolution.

Handlers must use the resolved tenant context and authenticated staff context.

Authorization must be enforced on the backend.

Critical permission checks include:

- Patient access
- Appointment management
- Visit record creation/editing
- Invoice creation/payment/cancellation
- Staff management
- Role changes
- Reports access

Authorization failures are critical safety bugs.

---

## Transaction Rules

Commands that modify multiple records or create audit logs must use a transaction.

Examples:

- Create invoice with line items
- Add payment and update invoice totals
- Cancel appointment and write audit log
- Create visit record and update appointment status
- Create staff user and assign roles

A command must not partially complete if one required step fails.

---

## Audit Rules

Sensitive commands must create audit logs.

Examples:

- Create/update/archive patient
- Create/update/cancel/check-in appointment
- Create/update visit record
- Issue/cancel invoice
- Add invoice payment
- Create/deactivate staff user
- Change roles or permissions

Audit metadata must not store unnecessary clinical text.

---

## Validation Rules

Validation should happen before state changes.

Use validators for input shape and basic rules:

- Required fields
- Phone format
- Date format
- Positive amount

Use domain/application logic for business invariants:

- Appointment status transitions
- Doctor appointment overlap
- Paid invoice cannot be edited
- Cancelled appointment cannot become active

---

## Controller Pattern

Controllers must be thin.

```csharp
[HttpPost]
public async Task<IActionResult> Create(
    CreateAppointmentCommand command,
    CancellationToken ct)
{
    var result = await _sender.Send(command, ct);

    return result.IsSuccess
        ? CreatedAtAction(nameof(GetById), new { id = result.Value.AppointmentId }, result.Value)
        : result.ToActionResult();
}
```

---

## Forbidden in Controllers

Do not put these in controllers:

- Business logic
- EF Core queries
- Tenant resolution
- Manual permission logic beyond attributes/policies
- Complex validation
- Direct `DbContext` injection
- Invoice calculations
- Appointment status transition logic
- Clinical record logic

---

## Non-Negotiable CQRS Rules

- Commands change state.
- Queries read state.
- Queries must not change state.
- Commands must not return full entities.
- Handlers must enforce tenant safety.
- Handlers must enforce authorization.
- Sensitive commands must be audit logged.
- Financial and clinical state changes must go through explicit commands.
- Controllers must stay thin.
