using System.Collections.Generic;

namespace Model_Validation
{
    public class ScanData
    {
        public double FieldSize { get; set; }
        public double Depth { get; set; }
        public string Direction { get; set; }
        public List<ScanDataPoint> DataPoints { get; set; }
        public ScanData()
        {
            DataPoints = new List<ScanDataPoint>();
        }
    }
}
