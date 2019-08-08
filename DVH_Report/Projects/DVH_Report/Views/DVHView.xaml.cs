using DVH_Report.ViewModels;
using System.Windows.Controls;
using VMS.TPS.Common.Model.API;

namespace DVH_Report.Views
{
    /// <summary>
    /// Interaction logic for DVHView.xaml
    /// </summary>
    public partial class DVHView : UserControl
    {
        public DVHView(PlanSetup planSetup)
        {
            DVHViewModel dVHViewModel = new DVHViewModel(planSetup);
            this.DataContext = dVHViewModel;
            InitializeComponent();
        }
    }
}
