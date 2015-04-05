using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ClownCrew.GitBitch.Client.Agents;
using ClownCrew.GitBitch.Client.Interfaces;

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

        public ITalkAgent TalkAgent { get { return _container.Resolve<ITalkAgent>(); } }
        public IQuestionAgent QuestionAgent { get { return _container.Resolve<IQuestionAgent>(); } }
        public ICommandAgent CommandAgent { get { return _container.Resolve<ICommandAgent>(); } }
        public IGitRepoAgent GitRepoAgent { get { return _container.Resolve<IGitRepoAgent>(); } }
        public IRepositoryBusines RepositoryBusines { get { return _container.Resolve<IRepositoryBusines>(); } }
        public ISettingAgent SettingAgent { get { return _container.Resolve<ISettingAgent>(); } }

        private CompositeRoot()
        {
            _container = new WindsorContainer();
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<TalkAgent>()).WithService.DefaultInterfaces().LifestyleSingleton());
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<GitRepoAgent>()).WithService.DefaultInterfaces().LifestyleSingleton());
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<DataRepository>()).WithService.DefaultInterfaces().LifestyleSingleton());
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<RepositoryBusines>()).WithService.DefaultInterfaces().LifestyleSingleton());
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<SettingAgent>()).WithService.DefaultInterfaces().LifestyleSingleton());
        }

        ~CompositeRoot()
        {
            _container.Dispose();
        }
    }
}