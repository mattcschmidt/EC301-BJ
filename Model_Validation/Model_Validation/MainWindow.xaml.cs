using System.Windows;
using VMS.TPS.Common.Model.API;

[assembly: ESAPIScript(IsWriteable = true)]
namespace Model_Validation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VMS.TPS.Common.Model.API.Application _app;
        public MainWindow()
        {
            _app = VMS.TPS.Common.Model.API.Application.CreateApplication();
            //version 13.X
            //_app = VMS.TPS.Common.Model.API.Application.CreateApplication(null,null);
            MainViewModel mainViewModel = new MainViewModel(_app);
            this.DataContext = mainViewModel;
            //MessageBox.Show(_app.CurrentUser.Id);
            this.Closing += MainWindow_Closing;
            InitializeComponent();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _app.ClosePatient();
            _app.Dispose();
        }
    }
}
