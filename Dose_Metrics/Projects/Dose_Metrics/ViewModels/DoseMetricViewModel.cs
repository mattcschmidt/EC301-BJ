using Dose_Metrics.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace Dose_Metrics.ViewModels
{
    public class DoseMetricViewModel:BindableBase
    {
        public List<string> Structures { get; set; }
        private string selectedStructure;

        public string SelectedStructure
        {
            get { return selectedStructure; }
            set
            {
                SetProperty(ref selectedStructure, value);
                AddMetric.RaiseCanExecuteChanged();
            }
        }
        public List<string> Metrics { get; set; }

        private PlanSetup _planSetup;
        private string selectedMetric;

        public string SelectedMetric
        {
            get { return selectedMetric; }
            set
            {
                SetProperty(ref selectedMetric, value);
                AddMetric.RaiseCanExecuteChanged();
            }
        }
        public DelegateCommand AddMetric { get; private set; }
        public ObservableCollection<DVHMetric> DQPs { get; private set; }
        private string customMetric;

        public string CustomMetric
        {
            get { return customMetric; }
            set { SetProperty(ref customMetric, value); }
        }

        public DoseMetricViewModel(PlanSetup planSetup)
        {
            _planSetup = planSetup;
            Structures = new List<string>(
                planSetup.StructureSet.Structures.Select(x => x.Id));
            Metrics = new List<string>(Enum.GetNames(typeof(MetricType)).Cast<string>());
            DQPs = new ObservableCollection<DVHMetric>();
            AddMetric = new DelegateCommand(OnAddMetric, CanAddMetric);
        }

        private void OnAddMetric()
        {
            DVHMetric dvh_temp = new DVHMetric
            {
                StructureId = SelectedStructure,
                DoseMetric = SelectedMetric,
                OutputValue = "N/A"
            };
            Structure s = _planSetup.StructureSet.Structures.FirstOrDefault(x => x.Id == SelectedStructure);
            DVHData dvh = _planSetup.GetDVHCumulativeData(
                s,
                VMS.TPS.Common.Model.Types.DoseValuePresentation.Absolute,
                VMS.TPS.Common.Model.Types.VolumePresentation.Relative,
                1);
            if (dvh != null)
            {
                switch (Metrics.IndexOf(SelectedMetric))
                {
                    case (int)MetricType.Max:
                        dvh_temp.OutputValue = dvh.MaxDose.ToString();
                        break;
                    case (int)MetricType.Min:
                        dvh_temp.OutputValue = dvh.MinDose.ToString();
                        break;
                    case (int)MetricType.Mean:
                        dvh_temp.OutputValue= dvh.MeanDose.ToString();
                        break;
                    case (int)MetricType.Volume:
                        dvh_temp.OutputValue = s.Volume.ToString("F2")+"cc";
                        break;
                    case (int)MetricType.VolA_at_DoseA:
                        DoseValue dv = new DoseValue(Convert.ToDouble(CustomMetric),
                            DoseValue.DoseUnit.cGy);
                        double volume = _planSetup.GetVolumeAtDose(s, 
                            dv, 
                            VolumePresentation.AbsoluteCm3);
                        dvh_temp.OutputValue = volume.ToString("F2") + "cc";
                        dvh_temp.DoseMetric = $"Volume at {dv}"; .

                        break;
                }
            }
            DQPs.Add(dvh_temp);
        }

        private bool CanAddMetric()
        {
            return SelectedStructure != null && SelectedMetric != null;
        }
    }
}
