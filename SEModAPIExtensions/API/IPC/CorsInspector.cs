using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;

namespace SEModAPIExtensions.API.IPC
{
	/// <summary>
	/// Class used to set the CORS (Cross Origin Ressource Sharing) headers in the response for the web WCF(Windows Communication Foundation) service
	/// </summary>
	public class CorsInspector : IDispatchMessageInspector
	{
		Dictionary<string, string> requiredHeaders;

		#region "Constructor & Initialisers"
		public CorsInspector(Dictionary<string, string> headers)
		{
			requiredHeaders = headers ?? new Dictionary<string, string>();
		}
		#endregion

		#region "Methods"

		/// <summary>
		/// Method called by the WCF service after reception of a request
		/// </summary>
		/// <param name="request"></param>
		/// <param name="channel"></param>
		/// <param name="instanceContext"></param>
		/// <returns>Can be used for a callback</returns>
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			return null;
		}

		/// <summary>
		/// Method called by the WCF service just before sending a response
		/// </summary>
		/// <param name="reply"></param>
		/// <param name="correlationState"></param>
		public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
		{
			var httpHeader = reply.Properties["httpResponse"] as HttpResponseMessageProperty;
			foreach (var item in requiredHeaders)
			{
				httpHeader.Headers.Add(item.Key, item.Value);
			}
		}
		#endregion
	}

	/// <summary>
	/// Class used as a command for the WCF service
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class EnableCorsBehavior : Attribute, IServiceBehavior
	{
		#region "Methods"
		/// <summary>
		/// Method called by the WCF service
		/// </summary>
		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{

		}

		/// <summary>
		/// Method called by the WCF service
		/// </summary>
		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			var requiredHeaders = new Dictionary<string, string>();

			requiredHeaders.Add("Access-Control-Allow-Origin", "*");
			requiredHeaders.Add("Access-Control-Request-Method", "POST,GET,PUT,DELETE,OPTIONS");
			requiredHeaders.Add("Access-Control-Allow-Headers", "X-Requested-With,Content-Type");

			foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
			{
				foreach (EndpointDispatcher ed in cd.Endpoints)
				{
					ed.DispatchRuntime.MessageInspectors.Add(new CorsInspector(requiredHeaders));
				}
			}
		}

		/// <summary>
		/// Method called by the WCF service
		/// </summary>
		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{

		}
		#endregion
	}
}
