using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MahApps.Metro.Controls.Dialogs;

namespace BeanTraderClient.DependencyInjection
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IDialogCoordinator>().Instance(DialogCoordinator.Instance));
        }
    }
}
