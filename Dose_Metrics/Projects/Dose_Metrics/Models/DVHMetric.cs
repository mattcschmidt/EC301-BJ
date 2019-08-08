using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dose_Metrics.Models
{
    public class DVHMetric
    {
        public string StructureId { get; set; }
        public string DoseMetric { get; set; }
        public string OutputValue { get; set; }
    }
}
