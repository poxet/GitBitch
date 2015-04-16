using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ClownCrew.GitBitch.Client.Agents;
using ClownCrew.GitBitch.Client.Business;
using ClownCrew.GitBitch.Client.Commands.Application;
using ClownCrew.GitBitch.Client.Commands.Git;
using ClownCrew.GitBitch.Client.Commands.Windows;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Repositories;

namespace ClownCrew.GitBitch.Client
{
    internal class CompositeRoot
    {
        private static CompositeRoot _instance;
        private readonly WindsorContainer _container;

        public static CompositeRoot Instance
        {
            get { return _instance ?? (_instance = new CompositeRoot()); }
        }

        //TODO: Remove direct access to theese as much as possible
        public ITalkAgent TalkAgent { get { return _container.Resolve<ITalkAgent>(); } }
        public IQuestionAgent QuestionAgent { get { return _container.Resolve<IQuestionAgent>(); } }
        public ICommandAgent CommandAgent { get { return _container.Resolve<ICommandAgent>(); } }
        public ISettingAgent SettingAgent { get { return _container.Resolve<ISettingAgent>(); } }
        public IEventHub EventHub { get { return _container.Resolve<IEventHub>(); } }
        public INotifyer Notifyer { get { return _container.Resolve<INotifyer>(); } }

        public T Resolve<T>() { return _container.Resolve<T>(); }

        private CompositeRoot()
        {
            _container = new WindsorContainer();

            //TODO: Register all Agents by name convension
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<TalkAgent>()).WithService.DefaultInterfaces().LifestyleSingleton());
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<GitRepoAgent>()).WithService.DefaultInterfaces().LifestyleSingleton());
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<SettingAgent>()).WithService.DefaultInterfaces().LifestyleSingleton());

            //TODO: Register all Busines classes by name convension
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<RepositoryBusines>()).WithService.DefaultInterfaces().LifestyleSingleton());
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<GitBusiness>()).WithService.DefaultInterfaces().LifestyleSingleton());

            //TODO: Register all Repositories by name convension
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<DataRepository>()).WithService.DefaultInterfaces().LifestyleSingleton());
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<RegistryRepository>()).WithService.DefaultInterfaces().LifestyleSingleton());

            //TODO: Register all commands by name convension
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<HelpCommand>()).WithService.DefaultInterfaces().LifestyleSingleton());
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<LockMachineCommand>()).WithService.DefaultInterfaces().LifestyleSingleton());

            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<EventHub>()).WithService.DefaultInterfaces().LifestyleSingleton());
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<Notifyer>()).WithService.DefaultInterfaces().LifestyleSingleton());
        }

        ~CompositeRoot()
        {
            _container.Dispose();
        }
    }
}