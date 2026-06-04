# ESAPI 18.x Update Notes

These notes summarize practical upgrade and review points for Eclipse Scripting API work in an ESAPI 18.x-era environment. They are not a replacement for the official Varian/Eclipse documentation installed at your clinic.

## Version And Build Assumptions

- Verify every project against the installed Eclipse and ESAPI version.
- ESAPI 18.0 uses .NET Framework 4.8.
- ESAPI 18.0 assembly version is `1.0.600`.
- Build binary plug-ins and stand-alone executables as x64.
- Binary plug-ins should use the `.esapi.dll` extension so Eclipse recognizes them.
- Stand-alone executables older than ESAPI 15.5 may need rebuilding for ESAPI 18.0.

## Supported Runtime Modes

### Single-File Plug-Ins

- Loaded from Eclipse and compiled on demand.
- Start from `ScriptContext`.
- Work on the current Eclipse patient/plan context.
- Must null-check `context.Patient`, `context.Course`, `context.PlanSetup`, `context.StructureSet`, `context.Image`, and dose objects before use.

### Binary Plug-Ins

- Compiled .NET assemblies.
- Better for larger GUI applications, shared libraries, testing, versioning, and deployment control.
- Keep `AssemblyVersion` aligned with script approval and release notes.

### Stand-Alone Executables

- Create the ESAPI root object with `Application.CreateApplication()`.
- Entry point must be `[STAThread]`.
- Dispose `Application` deterministically.
- Keep all ESAPI calls on the STA thread that created `Application`.
- Open only one patient object model at a time.
- Always call `Application.ClosePatient()` before opening another patient.
- Never use ESAPI object references after closing the patient.
- If parallel or expensive processing is needed, copy primitive data out of ESAPI first, then process detached DTOs.

Minimal pattern:

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
                // ESAPI access stays on this thread.
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

### Approval Extension Plug-Ins

- Approval extensions are binary plug-ins that run during the Eclipse/BrachyVision Plan Approval Wizard.
- They are invoked when promoting plans to reviewed or approved states.
- `PrePromote(...)` should return information, warnings, or blocking validation errors.
- `PostPromote(...)` is for post-approval logging or follow-up, not for blocking approval.
- Plan collections can contain multiple plans, for example in plan-sum workflows.
- Blocking validation errors should be rare and backed by local clinical policy and commissioning.
- Approval extensions can be write-enabled, but should not make dosimetrically relevant changes during approval.

Typical shape:

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

## Write-Enabled Automation

Write-enabled ESAPI is a clinical governance boundary.

- Add `[assembly: ESAPIScript(IsWriteable = true)]` only for scripts that intentionally modify patient/model/plan data.
- Call `Patient.BeginModifications()` before changing patient data.
- In stand-alone executables, call `Application.SaveModifications()` only at an explicit persistence point.
- In plug-ins, Eclipse controls save/discard after the script exits.
- Use development/research systems for development and testing.
- Keep approval status, assembly version, script release notes, and validation evidence aligned.

Review traps:

- Hidden `BeginModifications()` in constructors or helper methods.
- `SaveModifications()` inside utility code.
- Broad patient database scans that may modify the wrong patient.
- Hard-coded structure names without a protocol guard.
- Catching exceptions and continuing after partial changes.

## DVH And Plan-Check Rules

- Treat `PlanSetup` and `PlanSum` as separate paths even though both derive from `PlanningItem`.
- Check `PlanningItemDose`, dose, structure set, and structure availability before DVH calls.
- `DoseValue` carries units. Do not compare dose thresholds to raw numbers without selecting Gy, cGy, or percent.
- Explicitly state whether volume thresholds are absolute cm3 or relative percent.
- Missing required data should become a visible finding, not a null-reference crash.
- Use stable check IDs such as `PC001` for auditability and tests.

Minimal result shape:

```csharp
public sealed class CheckResult
{
    public string Code { get; set; }
    public string Severity { get; set; } // Info, Warning, Error
    public string Message { get; set; }
    public string Observed { get; set; }
    public string Expected { get; set; }
}
```

## Upgrade Review Checklist

- Confirm target Eclipse/ESAPI assembly version.
- Rebuild against installed ESAPI assemblies when moving major versions.
- Re-check Script Wizard templates for plug-in and approval extension signatures.
- Confirm target .NET Framework, platform target, and assembly extension.
- Re-run script approval for changed versions where required.
- Re-test null context handling, dose units, plan-sum paths, DICOM export behavior, and write-enabled code paths.
- Confirm stand-alone patient open/close behavior and single-thread ESAPI access.
- Update clinical documentation, validation reports, and retirement of old script versions.

## Public References

- Varian ESAPI documentation hub: <https://docs.developer.varian.com/articles/index.html>
- Varian ESAPI object model article: <https://docs.developer.varian.com/articles/17.0/05_Eclipse_Scripting_API_Object_Model.html>
- Varian ESAPI online API help: <https://docs.developer.varian.com/api/index.html>
- Varian API Book: <https://varianapis.github.io/VarianApiBook.pdf>
- Gateway Scripts approval extension first look: <https://www.gatewayscripts.com/post/script-approval-extensions-v18-scripting-first-look>
- Gateway approval extension example: <https://github.com/Gateway-Scripts/ApprovalChecks_ApprExt>
