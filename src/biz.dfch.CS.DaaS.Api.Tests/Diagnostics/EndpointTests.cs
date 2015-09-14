using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.DaaS.Api.Diagnostics;

namespace biz.dfch.CS.DaaS.Api.Tests.Diagnostics
{
    [TestClass]
    public class EndpointTests
    {
        [TestMethod]
        public void GetEndpointsReturnsEndpoints()
        {
            var uri = new Uri("http://daas/api/Diagnostics");
            biz.dfch.CS.DaaS.Api.Diagnostics.Diagnostics svc = new biz.dfch.CS.DaaS.Api.Diagnostics.Diagnostics(uri);
            svc.Credentials = System.Net.CredentialCache.DefaultCredentials;
            var result = svc.Endpoints.AddQueryOption("$top", 1).Execute();

            Assert.IsNotNull(result);
            
        }
    }
}
