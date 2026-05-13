# 11 — Hard Constraints

These rules must never be violated.

If a future requirement conflicts with these constraints, escalate and update the Knowledge Base. Do not silently break them.

| # | Constraint | Reason |
|---|---|---|
| 1 | No lazy loading in EF Core | Prevent N+1 queries and hidden performance issues |
| 2 | No hard deletes on clinical, financial, or staff records through normal workflows | Preserve audit trail and recovery options |
| 3 | No business logic in controllers | Maintain Clean Architecture |
| 4 | No cross-tenant database access in MVP | Tenant isolation is the core SaaS guarantee |
| 5 | No `float` / `double` for money | Avoid financial rounding errors |
| 6 | No hardcoded VAT rate | VAT must be configurable per clinic |
| 7 | No stack traces or internal errors in production responses | Security and privacy |
| 8 | Store persisted timestamps in UTC | Avoid timezone bugs |
| 9 | Fluent API only; no EF data annotations on domain entities | Keep domain entities clean |
| 10 | Staff JWT must include `tenantId` and `subdomain` | Required for tenant-aware authorization |
| 11 | All list endpoints must be paginated | Prevent performance and memory issues |
| 12 | Tenant connection resolution must happen server-side only | Prevent tenant spoofing and data leaks |
| 13 | Platform Database must not store tenant clinical or financial data | Preserve tenant isolation and privacy |
| 14 | Patient login is not part of MVP | Patient is a record, not an authenticated user |
| 15 | Paid or Cancelled invoices cannot be edited directly | Preserve financial integrity |
| 16 | Backend authorization is mandatory | Frontend permission checks are UX only |
| 17 | Tenant ID, database name, and connection string must not be trusted from frontend input | Prevent tenant spoofing |
| 18 | Do not expose domain entities directly from API endpoints | Prevent data leaks and tight coupling |
| 19 | Do not store clinical data or tenant secrets in browser storage | Protect patient privacy and tenant security |
| 20 | Sensitive actions must be audit logged | Support accountability and incident investigation |

## Short LLM Instruction

Use this short instruction in every coding prompt:

```txt
Follow the PCMS hard constraints strictly. If my request conflicts with a hard constraint, tell me before generating code.