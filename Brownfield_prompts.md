# Brownfield Prompts

## Evaluation

```
Analyze the entire monolithic ASP.NET web application to:
- Identify all distinct data domains (bounded contexts or business areas) used in the application.
- Map each domain to its associated files and resources (models, controllers, services, repositories, data access code, views, etc.).
- Assess the structure and boundaries of each domain and determine if they are properly encapsulated or overly coupled.
- Evaluate the current domain organization in the context of clean architecture, DDD (Domain-Driven Design) principles, and separation of concerns.

Output a comparison summary of the current state vs. ideal state (modular, loosely coupled domain structure).
Instructions:
Please perform the following steps using static analysis of the source code:

1. Enumerate All Data Domains:
Traverse the entire codebase to detect business-specific entities, models, and related logic. Group them into distinct business domains or bounded contexts, such as:
- User Management
- Billing or Payments
- Orders or Transactions
- Product or Catalog
- Reporting
- Notifications or Communication
- Security / Authentication
- Logging / Auditing

For each domain, provide:
- Domain name
- Brief description
- Directory structure where it is located

2. Map Files and Resources per Domain:
For each domain, list all relevant components and their file paths:
- Models / Entities
- DTOs / ViewModels
- Controllers / API Endpoints
- Services / Business Logic
- Repositories / Data Access Layer
- Configuration Files (e.g., connection strings, DI bindings)
- Views / Razor Pages (if applicable)
- Unit or Integration Tests
- Any third-party library usage directly associated with the domain

3. Analyze Current Domain Design:
Check for cross-domain references that might indicate tight coupling or leaky abstractions.
Highlight any:
- Shared model usage across unrelated domains
- Business logic in controllers instead of services
- Over-reliance on static or global state
- Domain logic embedded within views or UI components
- Identify potential violations of SOLID, Separation of Concerns, or DRY principles.

4. Assess for Modularization Readiness:
- Which domains are sufficiently decoupled and can be extracted into modules or microservices?
- Which areas require refactoring before modularization?
- Suggest a potential modular architecture structure (project layout or layering strategy) that the current monolith could evolve into.

5. Generate a Summary Report:
Provide a final report including:
- List of all domains and associated files
- Assessment of domain boundaries and cohesion
- Issues found with current domain organization
- Suggestions for improving modularity
- Comparison: Current State vs. Ideal DDD Structure

Additional Context:
- The application is written in ASP.NET (likely MVC or Web Forms or Web API).
- The project may include Entity Framework, ADO.NET, or other ORM/data access strategies.
- Assume a monolithic structure with limited separation between layers/domains.
- We are preparing for future migration to a modular or service-oriented architecture, so clarity in domain boundaries is critical.

```