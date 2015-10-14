using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Esynctraining.Core.Business.Models;
using Esynctraining.Core.Providers.Mailer;

namespace EdugameCloud.Core
{
    public sealed class MailWindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ITemplateProvider>().ImplementedBy<TemplateProvider>().LifeStyle.Transient);
            container.Register(Component.For<IAttachmentsProvider>().ImplementedBy<AttachmentsProvider>().LifeStyle.Transient);

            container.Register(Component.For<MailModel>().ImplementedBy<MailModel>().LifeStyle.Transient);
        }

    }

}
