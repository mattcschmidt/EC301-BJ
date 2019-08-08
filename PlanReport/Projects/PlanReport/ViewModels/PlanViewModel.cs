using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace Example_Plan.ViewModels
{
    public class PlanViewModel
    {
        private string planDetails;
        private PlanSetup _plan;
        private User _user;
        private IEnumerable<PlanSetup> _plans;

        public string PlanDetails
        {
            get { return planDetails; }
            set { planDetails = value; }
        }
        public PlanViewModel(PlanSetup plan, User user, IEnumerable<PlanSetup> plans)
        {
            _plan = plan;
            _user = user;
            _plans = plans;
            PlanDetails = SetText();
        }

        private string SetText()
        {
            // Retrieve the count of plans displayed in Scope Window
            int scopePlanCount = _plans.Count();
            if (scopePlanCount == 0)
            {
                MessageBox.Show("Scope Window does not contain any plans.");
                return "";
            }

            // Retrieve names for different types of plans
            List<string> externalPlanIds = new List<string>();
            List<string> brachyPlanIds = new List<string>();
            List<string> protonPlanIds = new List<string>();
            foreach (var ps in _plans)
            {
                if (ps is BrachyPlanSetup)
                {
                    brachyPlanIds.Add(ps.Id);
                }
                else if (ps is IonPlanSetup)
                {
                    protonPlanIds.Add(ps.Id);
                }
                else
                {
                    externalPlanIds.Add(ps.Id);
                }
            }

            // Construct output message
            string message = string.Format("Hello {0}, the number of plans in Scope Window is {1}.",
              _user.Name,
              scopePlanCount);
            if (externalPlanIds.Count > 0)
            {
                message += string.Format("\nPlan(s) {0} are external beam plans.", string.Join(", ", externalPlanIds));
            }
            if (brachyPlanIds.Count > 0)
            {
                message += string.Format("\nPlan(s) {0} are brachytherapy plans.", string.Join(", ", brachyPlanIds));
            }
            if (protonPlanIds.Count > 0)
            {
                message += string.Format("\nPlan(s) {0} are proton plans.", string.Join(", ", protonPlanIds));
            }

            // Display additional information. Use the active plan if available.
            PlanSetup plan = _plan != null ? _plan : _plans.ElementAt(0);
            message += string.Format("\n\nAdditional details for plan {0}:", plan.Id);

            // Access the structure set of the plan
            if (plan.StructureSet != null)
            {
                Image image = plan.StructureSet.Image;
                var structures = plan.StructureSet.Structures;
                message += string.Format("\n* Image ID: {0}", image.Id);
                message += string.Format("\n* Size of the Structure Set associated with the plan: {0}.", structures.Count());
                foreach (Structure s in structures)
                {
                    message += String.Format("\n\t-{0}: {1:F2}cc", s.Id, s.Volume);
                }
            }
            message += string.Format("\n* Number of Fractions: {0}.", plan.NumberOfFractions);
            message += String.Format("\n* Dose Per Fraction: {0}", plan.DosePerFraction);
            // Handle brachytherapy plans separately from external beam plans
            if (plan is BrachyPlanSetup)
            {
                BrachyPlanSetup brachyPlan = (BrachyPlanSetup)plan;
                var catheters = brachyPlan.Catheters;
                var seedCollections = brachyPlan.SeedCollections;
                message += string.Format("\n* Number of Catheters: {0}.", catheters.Count());
                message += string.Format("\n* Number of Seed Collections: {0}.", seedCollections.Count());
            }
            else
            {
                var beams = plan.Beams;
                message += string.Format("\n* Number of Beams: {0}.", beams.Count());
                foreach (Beam b in beams.OrderBy(x => x.BeamNumber))
                {
                    message += String.Format("\n\t-{0}: {1}MU", b.Id, b.Meterset.Value);
                }
            }
            if (plan is IonPlanSetup)
            {
                IonPlanSetup ionPlan = plan as IonPlanSetup;
                IonBeam beam = ionPlan.IonBeams.FirstOrDefault();
                if (beam != null)
                {
                    message += string.Format("\n* Number of Lateral Spreaders in first beam: {0}.", beam.LateralSpreadingDevices.Count());
                    message += string.Format("\n* Number of Range Modulators in first beam: {0}.", beam.RangeModulators.Count());
                    message += string.Format("\n* Number of Range Shifters in first beam: {0}.", beam.RangeShifters.Count());
                }
            }

            // MessageBox.Show(message);
            return message;
        }
    }
}
