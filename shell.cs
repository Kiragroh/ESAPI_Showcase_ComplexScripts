using System.Reflection;
using System.Windows;
using VMS.TPS.Common.Model.API;

// Increment AssemblyVersion for every released/approved clinical build.
[assembly: AssemblyVersion("2.1.0.0")]

// Keep this template read-only by default.
// Set IsWriteable = true only for scripts that intentionally modify patient,
// structure, image, plan, dose, or reference-point data and have gone through
// the local script approval and clinical validation workflow.
[assembly: ESAPIScript(IsWriteable = false)]

namespace VMS.TPS
{
    public class Script
    {
        public void Execute(ScriptContext context)
        {
            if (context == null)
            {
                MessageBox.Show("No ESAPI script context is available.", "ESAPI Script");
                return;
            }

            if (context.Patient == null)
            {
                MessageBox.Show("Open a patient in Eclipse before running this script.", "ESAPI Script");
                return;
            }

            // Add read-only workflow code here.
            // For write-enabled scripts, call context.Patient.BeginModifications()
            // only after explicit user intent and local approval requirements are met.
        }
    }
}
