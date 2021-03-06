﻿/**
 * Copyright 2015 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

﻿using System;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Appclusive.Api.Tests.Diagnostics
{
    [TestClass]
    public class EndpointTest
    {
        private static string _uriPrefix;
        private static Uri _uri;
        private static readonly Guid TENANT_GUID_SYSTEM = new Guid("11111111-1111-1111-1111-111111111111");

        static EndpointTest()
        {
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
            var endpointName = "BaseUri";
            var endpointCreatorId = 1;
            long endpointId = 1;
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            var result = svc.Endpoints.Where(e => e.Id == endpointId).First();

            Assert.IsNotNull(result);
            Assert.AreEqual(endpointId, result.Id);
            Assert.AreEqual(new Guid("11111111-1111-1111-1111-111111111111"), result.Tid);
            Assert.AreEqual(endpointName, result.Name);
            Assert.AreEqual(endpointName, result.Description);
            Assert.AreEqual(endpointCreatorId, result.CreatedById);
            Assert.AreEqual(endpointCreatorId, result.ModifiedById);
            Assert.AreEqual((new Version(0, 0, 0, 0)).ToString(), result.Version);
        }

        [TestMethod]
        public void GetEndpointCoreSucceeds()
        {
            var endpointName = "Core";
            var endpointCreatorId = 1;
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            var result = svc.Endpoints.Where(e => e.Name == endpointName).First();

            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Id);
            Assert.AreEqual(TENANT_GUID_SYSTEM, result.Tid);
            Assert.AreEqual(endpointName, result.Name);
            Assert.AreEqual(endpointCreatorId, result.CreatedById);
            Assert.AreEqual(endpointCreatorId, result.ModifiedById);
            Assert.AreNotEqual((new Version(0, 0, 0, 0)).ToString(), result.Version);
        }

        [TestMethod]
        public void GetEndpointDiagnosticsSucceeds()
        {
            var endpointName = "Diagnostics";
            var endpointCreatorId = 1;
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            var result = svc.Endpoints.Where(e => e.Name == endpointName).First();

            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Id);
            Assert.AreEqual(TENANT_GUID_SYSTEM, result.Tid);
            Assert.AreEqual(endpointName, result.Name);
            Assert.AreEqual(endpointCreatorId, result.CreatedById);
            Assert.AreEqual(endpointCreatorId, result.ModifiedById);
            Assert.AreNotEqual((new Version(0, 0, 0, 0)).ToString(), result.Version);
        }

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
                // ... as we cannot catch a ContractException with ExpectedException
            }
        }
    }
}
