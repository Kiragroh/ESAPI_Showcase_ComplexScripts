---
name: esapi-scripting
description: Use when working on Varian Eclipse Scripting API (ESAPI) C# code, including plug-in scripts, binary .esapi.dll plug-ins, stand-alone ESAPI executables, ESAPI 18+ approval extension plug-ins, DVH/plan-check/reporting scripts, write-enabled automation, and clinical-safety review.
---

# ESAPI Scripting

Use this skill to orient an AI assistant before writing, reviewing, or refactoring Eclipse Scripting API code.

## First Steps

1. Identify the runtime mode: single-file plug-in, binary plug-in, stand-alone executable, approval extension, or utility library.
2. Identify the target Eclipse/ESAPI version and installed ESAPI assembly version.
3. Decide whether the script is read-only or write-enabled.
4. Check whether the current context can be sparse: patient, course, plan, structure set, image, and dose can be null.
5. Require explicit clinical validation language. Code review and unit tests are not clinical commissioning.

## Core Runtime Rules

- Plug-ins start from `ScriptContext` and the current Eclipse context.
- Stand-alone executables start from `Application.CreateApplication()`.
- Stand-alone ESAPI access must stay on the single STA thread that created `Application`.
- Stand-alone scripts should open one patient object model at a time and call `Application.ClosePatient()` before opening another.
- Never use `Patient`, `PlanSetup`, `StructureSet`, `Structure`, `Dose`, `Beam`, or other ESAPI objects after the patient has been closed.
- Copy primitive data out before using worker threads, `Task`, async continuations, background workers, or PLINQ.

## ESAPI 18.x Notes

- ESAPI 18.0 projects target .NET Framework 4.8.
- ESAPI 18.0 assembly version is `1.0.600`.
- Build binary plug-ins and stand-alone executables as x64.
- Approval extensions are binary plug-ins that run during plan approval.
- Check installed Script Wizard templates when targeting a different installed ESAPI version.

## Plug-In Pattern

```csharp
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

public class Script
{
    public void Execute(ScriptContext context)
    {
        Patient patient = context.Patient;
        PlanSetup plan = context.PlanSetup;
        StructureSet structureSet = context.StructureSet;

        if (patient == null || plan == null || structureSet == null)
        {
            // Return a user-visible message instead of failing later.
            return;
        }
    }
}
```

## Stand-Alone Pattern

```csharp
using System;
using VMS.TPS.Common.Model.API;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        using (Application app = Application.CreateApplication())
        {
            Patient patient = null;
            try
            {
                patient = app.OpenPatientById(args[0]);
                // Read or modify one active patient object model.
            }
            finally
            {
                if (patient != null)
                {
                    app.ClosePatient();
                }
            }
        }
    }
}
```

## Approval Extension Pattern

Approval extensions should mostly report readiness findings. Use blocking errors only for validated institutional policy.

```csharp
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VMS.TPS
{
    public class ApprovalExtension
    {
        public PlanValidationResult PrePromote(
            PlanSetupApprovalStatus newStatus,
            IEnumerable<PlanSetup> plans)
        {
            var result = new PlanValidationResult();
            return result;
        }

        public void PostPromote(
            PlanSetupApprovalStatus newStatus,
            IEnumerable<PlanSetup> plans,
            PlanValidationResult results)
        {
        }
    }
}
```

## DVH And Plan Checks

- Check plan dose, structure set, structures, and empty structures before DVH calls.
- Decide dose presentation explicitly: Gy, cGy, or percent.
- Decide volume presentation explicitly: cm3 or percent.
- Do not compare `DoseValue` to raw numbers without a conversion policy.
- Keep plan-sum reporting separate from plan-editing logic.
- Return missing required data as findings, not crashes.
- Use stable check IDs such as `PC001`.

## Write-Enabled Automation

Do not add write behavior unless the user explicitly asked for mutation.

Required markers and calls:

```csharp
[assembly: ESAPIScript(IsWriteable = true)]
context.Patient.BeginModifications();
```

For stand-alone persistence:

```csharp
app.SaveModifications(); // persist
app.ClosePatient();      // close/discard when not saving
```

Review for hidden mutation: `BeginModifications`, `SaveModifications`, `AddStructure`, `AddCourse`, `AddExternalPlanSetup`, `SetPrescription`, `CalculateDose`, `Optimize`, contour changes, and reference-point changes.

## Review Checklist

- Null-check current context and expected objects.
- Confirm dose and volume units.
- Avoid hard-coded structure IDs unless they are explicitly local protocol data.
- Avoid `First(...)` without an empty-result guard.
- Keep UI/report rendering separate from calculation logic.
- Confirm stand-alone thread and patient lifetime rules.
- Confirm write-enabled approval, versioning, and save/discard behavior.
- Confirm approval extension behavior for multiple `PlanSetup` inputs.
- State validation limitations when ESAPI assemblies or Eclipse runtime are unavailable.

## Public References

- Varian ESAPI documentation hub: <https://docs.developer.varian.com/articles/index.html>
- Varian ESAPI online API help: <https://docs.developer.varian.com/api/index.html>
- Varian API Book: <https://varianapis.github.io/VarianApiBook.pdf>
- Gateway approval extension example: <https://github.com/Gateway-Scripts/ApprovalChecks_ApprExt>
