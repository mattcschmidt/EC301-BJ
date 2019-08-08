using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using PlanReport.Startup;
using Autofac;
using PlanReport.Views;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
// [assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
    public class Script
    {
        public Script()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context, System.Windows.Window window, ScriptEnvironment environment)
        {
            // TODO : Add here the code that is called when the script is launched from Eclipse.
            var bs = new Bootstrapper();
            var container = bs.Bootstrap(context.PlanSetup,
                context.CurrentUser,
                context.PlansInScope);
            var mainView = container.Resolve<MainView>();
            window.Content = mainView;
            window.Height = 1050;
            window.Width = 810;
            OxyPlot.Wpf.BarSeries fake_class = new OxyPlot.Wpf.BarSeries();
        }
    }
}
