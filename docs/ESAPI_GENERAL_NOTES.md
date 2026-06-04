# General ESAPI Notes

These notes are shared across the ESAPI example repositories. They are intended as orientation for developers and reviewers, not as a substitute for the official Varian/Eclipse documentation installed at your clinic.

## Version And Build Checks

- Verify the project against the Eclipse and ESAPI version installed at your institution.
- Older examples may mention Eclipse 15.x or 16.x because they were originally created in that era.
- For ESAPI 18.0, projects target .NET Framework 4.8 and the ESAPI assembly version is `1.0.600`.
- Build binary plug-ins and stand-alone executables as x64.
- Obtain `VMS.TPS.Common.Model.API.dll` and `VMS.TPS.Common.Model.Types.dll` from your local Eclipse/ARIA installation; they are not redistributed in these repositories.
- Rebuild and reapprove scripts when upgrading Eclipse/ESAPI versions if local governance requires it.

## Runtime Modes

- Single-file plug-ins run inside Eclipse and start from `ScriptContext`.
- Binary plug-ins are compiled assemblies, usually deployed as `.esapi.dll`.
- Stand-alone executables create the ESAPI root object with `Application.CreateApplication()` and can open database patients.
- ESAPI 18+ approval extensions are binary plug-ins that run during the plan approval workflow.

## Clinical Context And Null Safety

- `context.Patient`, `context.Course`, `context.PlanSetup`, `context.StructureSet`, `context.Image`, dose, beams, and structures can be null depending on where the script is launched.
- Missing structures, empty contours, missing dose, or missing prescriptions should become clear user-visible findings, not null-reference crashes.
- Structure IDs and plan names are local workflow data; validate aliases and protocol-specific names explicitly.

## Dose, DVH, And Geometry

- `DoseValue` carries units. Decide explicitly whether comparisons use Gy, cGy, or percent.
- Decide explicitly whether volume thresholds use absolute cm3 or relative percent.
- Keep `PlanSetup` and `PlanSum` paths separate when behavior differs.
- ESAPI positions are DICOM coordinates and distances are generally in millimeters.

## Stand-Alone Thread And Patient Lifetime

- Mark stand-alone entry points with `[STAThread]`.
- Keep all live ESAPI object access on the STA thread that created `Application`.
- Do not pass live ESAPI objects into `Task`, thread-pool work, async continuations, background workers, or PLINQ.
- Open only one patient object model at a time.
- Always call `Application.ClosePatient()` before opening another patient.
- After `ClosePatient()`, discard all ESAPI object references from that patient. Copy primitive values out first if later processing is needed.

## Write-Enabled Scripts

- Add `[assembly: ESAPIScript(IsWriteable = true)]` only when the script intentionally modifies patient/model/plan data.
- Call `Patient.BeginModifications()` before changing patient data.
- Use `Application.SaveModifications()` in stand-alone executables only at an explicit persistence point.
- Develop, test, approve, and commission write-enabled scripts under the local clinical quality management process.
- Code review and unit tests are not clinical validation.

## References

- Varian ESAPI documentation hub: https://docs.developer.varian.com/articles/index.html
- Varian ESAPI online API help: https://docs.developer.varian.com/api/index.html
- Varian API Book: https://varianapis.github.io/VarianApiBook.pdf
- Gateway Scripts approval extension first look: https://www.gatewayscripts.com/post/script-approval-extensions-v18-scripting-first-look
- Gateway approval extension example: https://github.com/Gateway-Scripts/ApprovalChecks_ApprExt
