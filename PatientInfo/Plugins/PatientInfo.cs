using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

[assembly: AssemblyVersion("1.0.0.1")]

namespace VMS.TPS
{
    public class Script
    {
        public Script()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context /*, System.Windows.Window window, ScriptEnvironment environment*/)
        {
            // TODO : Add here the code that is called when the script is launched from Eclipse.
            string name = context.Patient.Name;
            string dob = String.IsNullOrEmpty(context.Patient.DateOfBirth.ToString()) ?
                "No DOB" :
                context.Patient.DateOfBirth.ToString();
            PlanSetup plan = context.PlanSetup;
            string currentPlan = plan.Id;
            string maxDose = plan.Dose.DoseMax3D.ToString();
            string output = String.Format("Patient: {0}\nDOB: {1}\nPlan: {2}\nMax Dose: {3}",
                name, dob, currentPlan, maxDose);
            MessageBox.Show(output);
        }
    }
}
