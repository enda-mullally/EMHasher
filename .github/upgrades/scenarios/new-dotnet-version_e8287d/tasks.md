# EM.Hasher .NET Upgrade Tasks

## Overview

Upgrade all projects in the solution `src\EM.Hasher.sln` simultaneously in a single atomic operation. The work includes verifying prerequisites, performing the atomic project+package upgrade, running the test suite, and a single final commit.

**Progress**: 0/4 tasks complete (0%) ![0%](https://progress-bar.xyz/0)

---

## Tasks

### [▶] TASK-001: Verify prerequisites
**References**: Plan §Implementation Timeline, Plan §Migration Strategy

- [▶] (1) Verify required .NET SDK version is installed per Plan §Prerequisites and `global.json` (if present)
- [ ] (2) Runtime/SDK version meets minimum requirements (**Verify**)
- [ ] (3) Verify required toolchain and CLI versions (e.g., `dotnet` SDK, `nuget` tooling) per Plan §Prerequisites
- [ ] (4) Configuration files (`Directory.Build.props`, `Directory.Packages.props`, `global.json`) are compatible with the target framework per Plan §Migration Strategy (**Verify**)

### [ ] TASK-002: Atomic framework and package upgrade with compilation fixes
**References**: Plan §Migration Strategy, Plan §Project-by-Project Plans, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [ ] (1) Update TargetFramework/TargetFrameworks in all project files listed in Plan §Project-by-Project Plans (all projects in `src\EM.Hasher.sln`)
- [ ] (2) Update all NuGet package references per Plan §Package Update Reference (apply versions and replacements specified there)
- [ ] (3) Restore dependencies for the solution (`dotnet restore`) per Plan §Migration Strategy
- [ ] (4) Build the solution and fix all compilation errors caused by framework/package upgrades per Plan §Breaking Changes Catalog
- [ ] (5) Solution builds with 0 errors (**Verify**)

### [ ] TASK-003: Run full test suite and validate upgrade
**References**: Plan §Testing & Validation Strategy, Plan §Breaking Changes Catalog

- [ ] (1) Run all test projects defined in Plan §Testing & Validation Strategy (execute `dotnet test` for test projects listed)
- [ ] (2) Fix any test failures referencing Plan §Breaking Changes Catalog for common causes
- [ ] (3) Re-run tests after fixes
- [ ] (4) All tests pass with 0 failures (**Verify**)

### [ ] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [ ] (1) Commit all changes with message: "TASK-004: Complete atomic upgrade and test validation"
