using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace Model_Validation
{
    public class MainViewModel : BindableBase
    {
        private string patientId;
        public string PatientId
        {
            get { return patientId; }
            set { SetProperty(ref patientId, value); }
        }
        public DelegateCommand OpenPatientCommand { get; private set; }
        public ObservableCollection<string> Courses { get; private set; }
        private string selectedCourse;
        public string SelectedCourse
        {
            get { return selectedCourse; }
            set
            {
                SetProperty(ref selectedCourse, value);
                AddPlanSetups();
            }
        }

        public ObservableCollection<string> PlanSetups { get; private set; }
        public ObservableCollection<ScanData> ScanDataCollection { get; private set; }
        public DelegateCommand ExportDataCommand { get; private set; }
        private string fieldSizes;

        public string FieldSizes
        {
            get { return fieldSizes; }
            set { SetProperty(ref fieldSizes,value); }
        }
        public DelegateCommand CalculateBeamsCommand { get; private set; }

        private VMS.TPS.Common.Model.API.Application app;
        private ESAPIMethods esapi;
        private Patient patient;
        private Course course;
        private PlanSetup plan;
        public MainViewModel(VMS.TPS.Common.Model.API.Application app)
        {
            this.app = app;
            esapi = new ESAPIMethods();
            Courses = new ObservableCollection<string>();
            PlanSetups = new ObservableCollection<string>();
            ScanDataCollection = new ObservableCollection<ScanData>();

            FieldSizes = "3;7;17";
            OpenPatientCommand = new DelegateCommand(OnOpenPatient);
            ExportDataCommand = new DelegateCommand(OnExportData, CanExportData);
            CalculateBeamsCommand = new DelegateCommand(OnCalculateBeams);
        }

        private void OnCalculateBeams()
        {
            esapi.OnCalculateBeams(patient, FieldSizes, app);
            AddCourses();
            SelectedCourse = Courses.Last();
            SelectedPlan = PlanSetups.Last(); ;
        }

        private void AddCourses()
        {
            foreach (Course c in patient.Courses)
            {
                Courses.Add(c.Id);
            }
        }

        private void OnExportData()
        {
            foreach(var scan_type in ScanDataCollection.GroupBy(x => x.Direction))
            {
                if(scan_type.Key == "PDD")
                {
                    //export pdd
                    ExportFiles(scan_type.ToArray(),true);
                }
                else
                {
                    //group by field size
                    foreach(var prof_fs in scan_type.ToArray().GroupBy(x => x.FieldSize))
                    {
                        ExportFiles(prof_fs.ToArray(),false);
                    }
                }
            }
        }

        private void ExportFiles(ScanData[] scanData, bool isPDD)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filename = $"{scanData.First().Direction}_" +
                $"{(isPDD ? "All FS" : "FS_" + scanData.First().FieldSize.ToString())}.csv";
            using(StreamWriter sw = new StreamWriter(Path.Combine(path, filename)))
            {
                sw.WriteLine($"Pos," +
                    $"{(isPDD ? String.Join(",", scanData.Select(x => x.FieldSize)) : String.Join(",", scanData.Select(x => x.Depth)))}");
                for(int i =0; i < scanData.First().DataPoints.Count(); i++)
                {
                    sw.WriteLine($"{scanData.First().DataPoints[i].Position:F2}," +
                        $"{String.Join(",", scanData.Select(x => x.DataPoints[i].Dose)):F2}");
                }
                sw.Flush();
            }
        }

        private bool CanExportData()
        {
            return !String.IsNullOrEmpty(SelectedPlan) && ScanDataCollection.Count() != 0;
        }

        private void OnOpenPatient()
        {
            Courses.Clear();
            if (!String.IsNullOrEmpty(PatientId))
            {
                app.ClosePatient();
                patient = app.OpenPatientById(PatientId);
                if(patient == null)
                {
                    MessageBox.Show("Patient not found");
                }
                else
                {
                    foreach(Course c in patient.Courses)
                    {
                        Courses.Add(c.Id);
                    }
                }
            }
        }
        private string selectedPlan;
        public string SelectedPlan
        {
            get { return selectedPlan; }
            set
            {
                SetProperty(ref selectedPlan, value);
                CalculateDoseProfiles();
                ExportDataCommand.RaiseCanExecuteChanged();
            }
        }
        private void AddPlanSetups()
        {
            PlanSetups.Clear();
            if (!String.IsNullOrEmpty(SelectedCourse))
            {
                course = patient.Courses.SingleOrDefault(x => x.Id == SelectedCourse);
                if (course != null)
                {
                    foreach(PlanSetup ps in course.PlanSetups)
                    {
                        PlanSetups.Add(ps.Id);
                    }
                }
            }
        }
        private void CalculateDoseProfiles()
        {
            ScanDataCollection.Clear();
            if (!String.IsNullOrEmpty(SelectedPlan))
            {
                plan = course.PlanSetups.SingleOrDefault(x => x.Id == SelectedPlan);
                if (plan != null)
                {
                    double[] depths = new double[] { 15, 50, 100, 200, 300 };//mm
                    foreach(Beam b in plan.Beams)
                    {
                        //get 5 profiles and 1 pdd.
                        foreach(double d in depths)
                        {
                            ScanDataCollection.Add(GetProfileAtDepth(b, d));
                        }
                        ScanDataCollection.Add(GetPDD(b));
                    }
                } 
            }
        }

        private ScanData GetPDD(Beam b)
        {
            double FS = GetFieldSize(b);
            ScanData scan = new ScanData
            {
                FieldSize = FS,
                Direction = "PDD"
            };
            VVector start = new VVector();
            start.x = start.z = 0;
            start.y = -200;
            VVector end = new VVector();
            end.x = end.z = 0;
            end.y = 100;
            double[] size = new double[301];
            DoseProfile dp = b.Dose.GetDoseProfile(start,
                end,
                size);
            foreach(ProfilePoint p in dp)
            {
                scan.DataPoints.Add(new ScanDataPoint
                {
                    Position = p.Position.y + 200,
                    Dose = p.Value
                });
            }
            return scan;
        }

        private double GetFieldSize(Beam b)
        {
            double x1 = b.ControlPoints.First().JawPositions.X1;
            double x2 = b.ControlPoints.First().JawPositions.X2;
            return x2 - x1;
        }

        private ScanData GetProfileAtDepth(Beam b, double d)
        {
            ScanData scan = new ScanData
            {
                FieldSize = GetFieldSize(b),
                Depth = d,
                Direction = "Profile (x)"
            };
            VVector start = new VVector();
            start.z = 0;
            start.y = d - 200;//shift for depth of dicom origin.
            start.x = -(scan.FieldSize / 2) * 1.5;
            VVector end = new VVector();
            end.z = start.z;
            end.y = start.y;
            end.x = scan.FieldSize / 2 * 1.5;
            double[] size = new double[Convert.ToInt32(end.x - start.x)];
            DoseProfile dp = b.Dose.GetDoseProfile(start, end, size);
            foreach(ProfilePoint p in dp)
            {
                scan.DataPoints.Add(new ScanDataPoint
                {
                    Position = p.Position.x,
                    Dose = p.Value
                });
            }
            return scan;
        }
    }
}
