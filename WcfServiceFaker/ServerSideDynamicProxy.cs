using System;
using System.Reflection;

namespace WcfServiceFaker
{
    public interface IProxyConfig
    {
        Object ServiceImplementation { get; set; }
    }

    /// <summary>
    /// Test proxy invocation handler which is used to check a methods security
    /// before invoking the method
    /// </summary>
    public class ServerSideDynamicProxy : IProxyInvocationHandler, IProxyConfig
    {
        public Object ServiceImplementation { get; set; }

        ///<summary>
        /// Class constructor
        ///</summary>
        ///<param name="implementationObject">Instance of object to be proxied</param>
        private ServerSideDynamicProxy(Object implementationObject)
        {
            this.ServiceImplementation = implementationObject;
        }

        #region IProxyInvocationHandler Members

        ///<summary>
        /// IProxyInvocationHandler method that gets called from within the proxy
        /// instance. 
        ///</summary>
        ///<param name="proxy">Instance of proxy</param>
        ///<param name="method">Method instance 
        public Object Invoke(Object proxy, MethodInfo method, Object[] parameters)
        {

            return method.Invoke(ServiceImplementation, parameters);
        }

        #endregion

        ///<summary>
        /// Factory method to create a new proxy instance.
        ///</summary>
        public static Object NewInstance(Object obj, out IProxyConfig proxyConfig)
        {
            var proxy = new ServerSideDynamicProxy(obj);
            proxyConfig = proxy;
            return DynamicProxyFactory.GetInstance().Create(
                proxy, obj.GetType());
        }
    }
}