using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Threading;

namespace WcfServiceFaker
{
    /// <summary>
    /// Creates a WCF service 
    /// Note that this is not thread safe, so unit tests that use this cannot be run in parallel
    /// </summary>
    public class WcfServiceFaker
    {

        private static readonly Dictionary<Type, ServiceHostBase> OpenHosts = new Dictionary<Type, ServiceHostBase>();

        private static void AddNewEndpoint(string bindingName, string configName, string address, ClientSection clientSection)
        {
            var element2 = new ChannelEndpointElement(new EndpointAddress(address), configName)
                               {
                                   Binding = bindingName
                               };
            ChannelEndpointElement element = element2;
            clientSection.Endpoints.Add(element);
        }

        private static void CreateClientStubEndpointInConfig(string bindingName, string configName, string address)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSectionGroup sectionGroup = configuration.GetSectionGroup("system.serviceModel");
            if (sectionGroup != null)
            {
                foreach (ConfigurationSection section in sectionGroup.Sections)
                {
                    var clientSection = section as ClientSection;
                    if (clientSection != null)
                    {
                        RemoveEndPointIfExists(configName, clientSection);
                        AddNewEndpoint(bindingName, configName, address, clientSection);
                    }
                }
            }
            configuration.Save();
            ConfigurationManager.RefreshSection("system.serviceModel/client");
        }

        private static void HostStubService<TServiceContract>(TServiceContract serviceImplementation, Binding binding, Type contractType, string address)
        {
            var waitForHost = new AutoResetEvent(false);
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 var host = new ServiceHost(serviceImplementation, new Uri[0]);
                                                 host.AddServiceEndpoint(contractType, binding, address);
                                                 host.Description.Behaviors.Find<ServiceBehaviorAttribute>().IncludeExceptionDetailInFaults = true;
                                                 host.Description.Behaviors.Find<ServiceBehaviorAttribute>().InstanceContextMode = InstanceContextMode.Single;
                                                 host.Open();
                                                 OpenHosts.Add(typeof(TServiceContract), host);
                                                 waitForHost.Set();
                                             });
            if (!waitForHost.WaitOne(0x4e20))
            {
                throw new ServiceActivationException("Could not open FakeWcf service for type:" + typeof(TServiceContract).FullName);
            }
        }

        //public static void InitForRhinoMock()
        //{
        //    AttributesToAvoidReplicating.Add(typeof(ServiceContractAttribute));
        //}

        private static void RemoveEndPointIfExists(string configName, ClientSection clientSection)
        {
            ChannelEndpointElement element = null;
            foreach (ChannelEndpointElement element2 in clientSection.Endpoints)
            {
                if (element2.Contract == configName)
                {
                    element = element2;
                }
            }
            if (element != null)
            {
                clientSection.Endpoints.Remove(element);
            }
        }

        private static void RemoveOldServiceInstanceIfExists<TServiceContract>()
        {
            if (OpenHosts.ContainsKey(typeof(TServiceContract)))
            {
                var base2 = OpenHosts[typeof(TServiceContract)];
                var waiter = new AutoResetEvent(false);
                base2.Closed += (s, e) => waiter.Set();
                base2.Close();
                ((IDisposable) base2).Dispose();
                if (!waiter.WaitOne(0x1388))
                {
                    throw new ServiceActivationException("Could not close FakeWcf service for type:" + typeof(TServiceContract).FullName);
                }
                OpenHosts.Remove(typeof(TServiceContract));
            }
        }

        private static void StubWcfClientProxy<TServiceContract>(TServiceContract stubServiceInstance, out IProxyConfig proxyConfig) where TServiceContract : class
        {

            TServiceContract proxyServiceInstance = (TServiceContract)ServerSideDynamicProxy.NewInstance(stubServiceInstance, out proxyConfig);

            var binding = new NetNamedPipeBinding();
            var attribute = typeof(TServiceContract).GetCustomAttributes(typeof(ServiceContractAttribute), false)[0] as ServiceContractAttribute;
            string configurationName = attribute.ConfigurationName;
            string address = string.Format("net.pipe://localhost/{0}", configurationName);
            if (!OpenHosts.ContainsKey(typeof(TServiceContract)))
            {
                CreateClientStubEndpointInConfig("netNamedPipeBinding", configurationName, address);
            }

            RemoveOldServiceInstanceIfExists<TServiceContract>();
            HostStubService<TServiceContract>(proxyServiceInstance, binding, typeof(TServiceContract), address);
        }

        private static IProxyConfig _currentServiceConfig;
        private static readonly Dictionary<Type,object > ServiceCache = new Dictionary<Type, object>();

        /// <summary>
        /// Note that this is not thread safe, so unit tests that use this cannot be run in parallel
        /// </summary>
        public static void SetWcfService<TServiceContract>(TServiceContract stubServiceInstance) where TServiceContract : class
        {
            if (!ServiceCache.ContainsKey(typeof(TServiceContract)))
            {
                StubWcfClientProxy<TServiceContract>(stubServiceInstance, out _currentServiceConfig);
                ServiceCache[typeof (TServiceContract)] = stubServiceInstance;
            }
            _currentServiceConfig.ServiceImplementation = stubServiceInstance;
        }
    }
}
