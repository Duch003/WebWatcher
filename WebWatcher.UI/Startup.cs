using Autofac;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using WebWatcher.UI.Interfaces;
using WebWatcher.UI.Readers;
using WebWatcher.UI.Services;
using WebWatcher.UI.Validators;
using WebWatcher.UI.ViewModels;
using WebWatcher.UI.Views;
using Xceed.Wpf.Toolkit;
using IContainer = Autofac.IContainer;

namespace WebWatcher.UI
{
    class Startup : BootstrapperBase
    {
        private static IContainer container;
        public static T Resolve<T>() => container.Resolve<T>();
        public Startup()
        {
            Initialize();
            RegisterTypes();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<RootViewModel>();
        }

        private void RegisterTypes()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                //  must be a type that ends with ViewModel
                .Where(type => type.Name.EndsWith("ViewModel"))
                //  must be in a namespace ending with ViewModels
                .Where(type => !( string.IsNullOrWhiteSpace(type.Namespace) ) && ( type.Namespace.EndsWith("ViewModels") || type.Namespace.EndsWith("SubWindows") || type.Namespace.EndsWith("Tabs") || type.Namespace.EndsWith("UserControls") ))
                //  must implement INotifyPropertyChanged (deriving from PropertyChangedBase will statisfy this)
                .Where(type => type.GetInterface(typeof(INotifyPropertyChanged).Name) != null)
                //  registered as self
                .AsSelf()
                //  always create a new one
                .InstancePerDependency();

            //  register views
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                //  must be a type that ends with View
                .Where(type => type.Name.EndsWith("View"))
                //  must be in a namespace that ends in Views
                .Where(type => !( string.IsNullOrWhiteSpace(type.Namespace) ) && ( type.Namespace.EndsWith("Views") || type.Namespace.EndsWith("SubWindows") || type.Namespace.EndsWith("Tabs") || type.Namespace.EndsWith("UserControls") ))
                //  registered as self
                .AsSelf()
                //  always create a new one
                .InstancePerDependency();

            //  register the single window manager for this container
            builder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();
            //  register the single event aggregator for this container
            builder.Register<IEventAggregator>(c => new EventAggregator()).InstancePerLifetimeScope();

            //IO
            builder.RegisterType<WebReader>().As<IWebReader>();

            //Validators
            builder.RegisterType<UrlValidator>().As<IUrlValidator>();

            //Services
            builder.RegisterType<WebService>().As<IWebService>();
            builder.RegisterType<ConsoleLogService>().As<IConsoleLogService>();

            //ViewModels
            builder.RegisterType<UCLogPanelViewModel>().As<IUCLogPanelViewModel>();
            builder.RegisterType<UCMessageBoxViewModel>().As<IUCMessageBoxViewModel>();
            builder.RegisterType<UCControlPanelViewModel>().As<IUCControlPanelViewModel>();

            container = builder.Build();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (container.IsRegistered(serviceType))
                    return container.Resolve(serviceType);
            }
            else
            {
                if (container.IsRegisteredWithKey(key, serviceType))
                {
                    return container.ResolveKeyed(key, serviceType);
                }
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? serviceType.Name));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.Resolve(typeof(IEnumerable<>).MakeGenericType(serviceType)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            container.InjectProperties(instance);
        }

        private void OpenWindow<T>()
        {
            var manager = container.Resolve<IWindowManager>();
            manager.ShowWindow(container.Resolve<T>());
        }
    }
}