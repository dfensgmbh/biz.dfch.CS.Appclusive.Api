﻿using System;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Appclusive.Api.Diagnostics;

namespace biz.dfch.CS.Appclusive.Api.Tests.Diagnostics
{
    [TestClass]
    public class EndpointTests
    {
        private static string _uriPrefix;
        private static Uri _uri;

        static EndpointTests()
        {
            // DFTODO - comply with naming convention
            _uriPrefix = ConfigurationManager.AppSettings["Service.Reference.URI.Prefix"];
            _uri = new Uri(_uriPrefix + "Diagnostics");
        }

        [TestMethod]
        public void GetEndpointsSucceeds()
        {
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            var result = svc.Endpoints.AddQueryOption("$top", 1).Execute();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetEndpoint1BaseUriSucceeds()
        {
            long endpointId = 1;
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            var result = svc.Endpoints.Where(e => e.Id == endpointId).First();

            Assert.IsNotNull(result);
            Assert.AreEqual(endpointId, result.Id);
            Assert.AreEqual(Guid.Empty.ToString(), result.Tid);
            Assert.AreEqual("BaseUri", result.Name);
            Assert.AreEqual("BaseUri", result.Description);
            Assert.AreEqual("SYSTEM", result.CreatedBy);
            Assert.AreEqual("SYSTEM", result.ModifiedBy);
            Assert.AreEqual((new Version(0, 0, 0, 0)).ToString(), result.Version);
        }

        // DFTODO - move [ExpectContractException] into System.Utilities module
        [TestMethod]
        public void GetEndpoint0Fails()
        {
            long endpointId = 0;
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            try
            {
                var result = svc.Endpoints.Where(e => e.Id == endpointId).FirstOrDefault();
                Assert.Fail("Test should have failed before.");
            }
            catch
            {
                // Intentionally left blank
            }
        }
    }
}
