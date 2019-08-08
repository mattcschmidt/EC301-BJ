using Dose_Metrics.ViewModels;
using DVH_Report.ViewModels;
using Example_Plan.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanReport.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel(PlanViewModel planViewModel,
            DoseMetricViewModel doseMetricViewModel,
            DVHViewModel dVHViewModel)
        {
            PlanViewModel = planViewModel;
            DoseMetricViewModel = doseMetricViewModel;
            DVHViewModel = dVHViewModel;
        }

        public PlanViewModel PlanViewModel { get; }
        public DoseMetricViewModel DoseMetricViewModel { get; }
        public DVHViewModel DVHViewModel { get; }
    }
}
