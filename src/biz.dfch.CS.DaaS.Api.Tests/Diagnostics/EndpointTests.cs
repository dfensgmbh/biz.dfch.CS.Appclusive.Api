using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.DaaS.Api.Diagnostics;

namespace biz.dfch.CS.DaaS.Api.Tests.Diagnostics
{
    [TestClass]
    public class EndpointTests
    {
        private String _uriPrefix = ConfigurationManager.AppSettings["Service.Reference.URI.Prefix"];

        [TestMethod]
        public void GetEndpointsReturnsEndpoints()
        {
            var uri = new Uri(_uriPrefix + "Diagnostics");
            biz.dfch.CS.DaaS.Api.Diagnostics.Diagnostics svc = new biz.dfch.CS.DaaS.Api.Diagnostics.Diagnostics(uri);
            svc.Credentials = System.Net.CredentialCache.DefaultCredentials;
            var result = svc.Endpoints.AddQueryOption("$top", 1).Execute();

            Assert.IsNotNull(result);
        }
    }
}
