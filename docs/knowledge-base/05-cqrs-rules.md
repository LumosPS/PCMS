# 05 — CQRS Rules

## Design Decision

PCMS uses manual CQRS.

Do not use MediatR unless the architecture decision is explicitly changed.

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

```csharp
public interface ICommand { }

public interface ICommand<TResult> { }

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken ct);
}

public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken ct);
}
```

```csharp
public interface IQuery<TResult> { }

public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct);
}
```

```csharp
public interface ICommandDispatcher
{
    Task<Result> DispatchAsync<TCommand>(TCommand command, CancellationToken ct)
        where TCommand : ICommand;

    Task<Result<TResult>> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken ct)
        where TCommand : ICommand<TResult>;
}

public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken ct)
        where TQuery : IQuery<TResult>;
}
```

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
    var result = await _commands.DispatchAsync<CreateAppointmentCommand, CreateAppointmentResult>(
        command,
        ct
    );

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
