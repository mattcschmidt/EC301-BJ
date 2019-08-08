using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace Model_Validation
{
    public class ESAPIMethods
    {
        public void OnCalculateBeams(Patient patient, string FieldSizes, Application app)
        {
            if (patient != null)
            {
                patient.BeginModifications();
                Course course_temp = patient.AddCourse();
                ExternalPlanSetup plan_temp = course_temp.
                    AddExternalPlanSetup(patient.StructureSets.FirstOrDefault());
                ExternalBeamMachineParameters exBeamParams = new ExternalBeamMachineParameters(
                    "HESN10",
                    "6X",
                    600,
                    "STATIC",
                    null);
                foreach (string fs in FieldSizes.Split(';'))
                {
                    double fsd = Convert.ToDouble(fs);
                    plan_temp.AddStaticBeam(exBeamParams,
                        new VRect<double>(-fsd / 2 * 10, -fsd / 2 * 10, fsd / 2 * 10, fsd / 2 * 10),
                        0,
                        0,
                        0,
                        new VVector(0, -200, 0));
                }
                plan_temp.SetPrescription(1, new DoseValue(100, DoseValue.DoseUnit.cGy), 1);
                plan_temp.CalculateDose();

                app.SaveModifications();
                
            }
        }
    }
}
