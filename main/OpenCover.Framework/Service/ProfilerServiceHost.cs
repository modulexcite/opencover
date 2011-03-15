﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace OpenCover.Framework.Service
{
    /// <summary>
    /// Manage a WCF service on a supplied port - also allows extraction of metadata
    /// </summary>
    public class ProfilerServiceHost
    {
        private ProfilerCommunicationServiceHost _serviceHost;
        public void Open(int port)
        {
            var baseAddress = new Uri(string.Format("net.tcp://localhost:{0}/OpenCover.Profiler.Host", port));
            _serviceHost = new ProfilerCommunicationServiceHost(typeof(ProfilerCommunication), baseAddress);
            
            var smb = _serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>() ?? new ServiceMetadataBehavior();

            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy12;
            _serviceHost.Description.Behaviors.Add(smb);

            _serviceHost.AddServiceEndpoint(
                ServiceMetadataBehavior.MexContractName,
                MetadataExchangeBindings.CreateMexTcpBinding(),
                "mex");

            var binding = new NetTcpBinding()
            {
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                Security = { Mode = SecurityMode.None }
            };

            _serviceHost.AddServiceEndpoint(
                typeof(IProfilerCommunication), binding, baseAddress);

            _serviceHost.Open();
        }

        public void Close()
        {
            _serviceHost.Close();
        }
    }
}