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

        private CompositeRoot()
        {
            _container = new WindsorContainer();
            //container.Install(FromAssembly.This());
            _container.Register(Classes.FromThisAssembly().Where(Component.IsInSameNamespaceAs<TalkAgent>()).WithService.DefaultInterfaces().LifestyleSingleton()); 
        }

        ~CompositeRoot()
        {
            _container.Dispose();
        }
    }
}