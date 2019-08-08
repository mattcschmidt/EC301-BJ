using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace DVH_Report.ViewModels
{
    public class DVHViewModel
    {
        private PlanSetup _planSetup;

        public PlotModel MyPlotModel { get; private set; }
        public DVHViewModel(PlanSetup planSetup)
        {
            _planSetup = planSetup;
            MyPlotModel = new PlotModel
            {
                Title = $"DVH for {_planSetup.Id}",
                LegendTitle = "Structure Id"
            };
            DrawDVH();
        }

        private void DrawDVH()
        {
            MyPlotModel.Series.Clear();
            foreach(Structure s in _planSetup.StructureSet.Structures)
            {
                if(s.DicomType != "MARKER" && s.DicomType != "SUPPORT" && s.HasSegment)
                {
                    //get the DVH
                    DVHData dvh = _planSetup.GetDVHCumulativeData(s,
                        VMS.TPS.Common.Model.Types.DoseValuePresentation.Absolute,
                        VMS.TPS.Common.Model.Types.VolumePresentation.Relative,
                        1);
                    //draw dvh to MyPlotModel
                    LineSeries series = new LineSeries { Title = $"{s.Id}" };
                    foreach(var datapoint in dvh.CurveData)
                    {
                        series.Points.Add(new DataPoint(datapoint.DoseValue.Dose,
                            datapoint.Volume));
                    }
                    MyPlotModel.Series.Add(series);
                }
            }
        }
    }
}
