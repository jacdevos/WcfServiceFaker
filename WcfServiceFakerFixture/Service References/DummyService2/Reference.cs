﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.237
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WcfServiceFakerFixture.DummyService2 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DummyService2.IDummyService")]
    public interface IDummyService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDummyService/DoWork", ReplyAction="http://tempuri.org/IDummyService/DoWorkResponse")]
        string DoWork(string input);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDummyServiceChannel : IDummyService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DummyServiceClient : System.ServiceModel.ClientBase<IDummyService>, IDummyService {
        
        public DummyServiceClient() {
        }
        
        public DummyServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DummyServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DummyServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DummyServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string DoWork(string input) {
            return base.Channel.DoWork(input);
        }
    }
}
