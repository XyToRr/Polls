# Copilot instructions — Polls repository

Quick commands

- Restore dependencies: `dotnet restore` (run from repo root)
- Build solution: `dotnet build Polls.sln -c Debug`
- Build release: `dotnet build Polls.sln -c Release`
- Run API locally: `dotnet run --project Polls\Polls.Api` (uses the minimal-hosting Program.cs)
- Run with specific URLs: `dotnet run --project Polls\Polls.Api --urls "https://localhost:5001;http://localhost:5000"`
- Tests: `dotnet test` (runs all test projects if any). Run a single test:
  - `dotnet test <testProject>.csproj --filter "FullyQualifiedName=Namespace.Class.Method"`
  - or `dotnet test <testProject>.csproj --filter "DisplayName~PartialName"`
- Format / lint: `dotnet format Polls.sln` (requires dotnet-format available; use `dotnet tool restore` if using a local tool manifest)

High-level architecture

- Solution: `Polls.sln` (root). Projects:
  - `Polls` / `Polls.Api`: ASP.NET Core Web API (TargetFramework net8.0). Entry point: `Polls\Program.cs` (minimal hosting model). Controllers live under `Polls\Controllers` and use attribute routing (e.g., `[Route("[controller]")]`). Swagger is enabled in development.
  - `Polls.DataAccess`: .NET class library for data access (net8.0).
  - `Polls.Database`: SQL Server Database project (`.sqlproj`) for schema / dacpac management (SSDT).

Key conventions

- Naming: Projects use the `Polls.*` prefix (Polls.Api, Polls.DataAccess, Polls.Database).
- Minimal hosting: all app configuration and DI registrations occur in `Program.cs` (no separate Startup class).
- Controllers: placed in `Polls.Controllers` namespace and use conventional `[controller]` route tokens.
- Target framework: net8.0 with `Nullable` and `ImplicitUsings` enabled in project files.
- Database work: schema and deployment handled via `Polls.Database` (SSDT). If using EF migrations, they live in the DataAccess or a tests/deploy project — none are present by default.

Repository-specific notes for Copilot

- Look at `Program.cs` to see which services are registered; most runtime wiring happens there.
- Prefer editing DataAccess when adding data-layer logic; update `Polls.Database` for schema changes.
- No existing test projects detected; suggest adding a `Polls.Tests` project for unit tests if needed.

AI integrations

- No CLAUDE.md, .cursorrules, AGENTS.md, or other assistant-config files were found.

Questions

- Would you like me to configure any MCP servers (e.g., Playwright or other test runners) for this project?

Summary

Created a Copilot instructions file with build/test/lint commands, high-level architecture, and repository conventions. Tell me if you want adjustments or additional coverage (CI, deployment, testing patterns, or examples).