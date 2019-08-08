using Autofac;
using Dose_Metrics.ViewModels;
using DVH_Report.ViewModels;
using Example_Plan.ViewModels;
using PlanReport.ViewModels;
using PlanReport.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace PlanReport.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap(PlanSetup planSetup, User user, IEnumerable<PlanSetup> planSetups)
        {
            var builder = new ContainerBuilder();
            //view
            builder.RegisterType<MainView>().AsSelf();
            //viewmodels
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<PlanViewModel>().AsSelf();
            builder.RegisterType<DoseMetricViewModel>().AsSelf();
            builder.RegisterType<DVHViewModel>().AsSelf();
            //esapi stuff.
            builder.RegisterInstance<PlanSetup>(planSetup);
            builder.RegisterInstance(user);
            builder.RegisterInstance(planSetups);

            return builder.Build();
        }
    }
}
