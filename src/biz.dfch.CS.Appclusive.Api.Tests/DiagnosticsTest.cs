/**
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
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;
using System.Configuration;
using System.Collections;
using System.Net;
using biz.dfch.CS.Appclusive.Core.OdataServices.Diagnostics;
using biz.dfch.CS.Appclusive.Api.Diagnostics;

namespace biz.dfch.CS.Appclusive.Api.Tests
{
    [TestClass]
    public class DiagnosticsTest
    {
        private static string _uriPrefix;
        private static Uri _uri;

        static DiagnosticsTest()
        {
            _uriPrefix =  ConfigurationManager.AppSettings["Service.Reference.URI.Prefix"];
            _uri = new Uri(_uriPrefix + "Diagnostics");
        }

        [TestMethod]
        public void AttachingDetachedEntitySucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(new Uri("http://localhost"));
            var entity = new Endpoint();
            Mock.Arrange(() => svc.AttachTo(Arg.AnyString, Arg.AnyObject)).OccursOnce();

            // Act
            svc.AttachIfNeeded(entity);

            // Assert
            Mock.Assert(svc);
        }

        [TestMethod]
        public void AttachingEntityToInvalidEntitySetThrowsException()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(new Uri("http://localhost"));
            var entity = new Endpoint();
            var entitySetName = "InvalidEntitySetName";
            Mock.Arrange(() => svc.AttachTo(Arg.AnyString, Arg.AnyObject)).OccursOnce();

            // Act
            svc.AttachIfNeeded(entitySetName, entity);

            // Assert
            Mock.Assert(svc);
        }

        [TestMethod]
        public void InvokeDiagnosticsPingSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            svc.InvokeEntitySetActionWithVoidResult("Endpoints", "Ping", null);

            // Assert
            // Nothing to assert. No exception is all we expect.
        }

        [TestMethod]
        public void InvokeDiagnosticsAuthenticatedPingSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);

            // Act
            svc.InvokeEntitySetActionWithVoidResult("Endpoints", "AuthenticatedPing", null);

            // Assert
            // Nothing to assert. No exception is all we expect.
        }

        [TestMethod]
        public void InvokeDiagnosticsEchoWithGenericHelperSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            var content = "arbitraryContent";

            var input = new Hashtable();
            input.Add("Content", content);

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult<string>(new Endpoint(), "Echo", input);

            // Assert
            Assert.AreEqual(content, result);
        }

        [TestMethod]
        public void InvokeDiagnosticsTimeWithGenericHelperAndEntitySetNameSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            var provider = CultureInfo.InvariantCulture;
            var format = "yyyy-MM-ddTHH:mm:ss.fffzzz";

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult<string>("Endpoints", "Time", null);

            // Assert
            var expectedDateTimeOffset = DateTimeOffset.ParseExact(result, format, provider);
            Assert.IsNotNull(expectedDateTimeOffset);
        }

        [TestMethod]
        public void InvokeDiagnosticsTimeWithGenericHelperAndEntitySucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            
            var provider = CultureInfo.InvariantCulture;
            var format = "yyyy-MM-ddTHH:mm:ss.fffzzz";

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult<string>(new Endpoint(), "Time", null);

            // Assert
            var expectedDateTimeOffset = DateTimeOffset.ParseExact(result, format, provider);
            Assert.IsNotNull(expectedDateTimeOffset);
        }

        [TestMethod]
        public void InvokeDiagnosticsTimeWithNonGenericHelperAndEntitySetNameAndObjectSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            var provider = CultureInfo.InvariantCulture;
            var format = "yyyy-MM-ddTHH:mm:ss.fffzzz";

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult("Endpoints", "Time", "", null);

            // Assert
            var expectedDateTimeOffset = DateTimeOffset.ParseExact(result.ToString(), format, provider);
            Assert.IsNotNull(expectedDateTimeOffset);
        }

        [TestMethod]
        public void InvokeDiagnosticsTimeWithNonGenericHelperAndEntityAndObjectSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            var provider = CultureInfo.InvariantCulture;
            var format = "yyyy-MM-ddTHH:mm:ss.fffzzz";

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult(new Endpoint(), "Time", "", null);

            // Assert
            var expectedDateTimeOffset = DateTimeOffset.ParseExact(result.ToString(), format, provider);
            Assert.IsNotNull(expectedDateTimeOffset);
        }

        [TestMethod]
        public void InvokeDiagnosticsTimeWithNonGenericHelperAndEntitySetNameAndTypeSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            var provider = CultureInfo.InvariantCulture;
            var format = "yyyy-MM-ddTHH:mm:ss.fffzzz";

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult("Endpoints", "Time", typeof(string), null);

            // Assert
            var expectedDateTimeOffset = DateTimeOffset.ParseExact(result.ToString(), format, provider);
            Assert.IsNotNull(expectedDateTimeOffset);
        }

        [TestMethod]
        public void InvokeDiagnosticsTimeWithNonGenericHelperAndEntityAndTypeSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            var provider = CultureInfo.InvariantCulture;
            var format = "yyyy-MM-ddTHH:mm:ss.fffzzz";

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult(new Endpoint(), "Time", typeof(string), null);

            // Assert
            var expectedDateTimeOffset = DateTimeOffset.ParseExact(result.ToString(), format, provider);
            Assert.IsNotNull(expectedDateTimeOffset);
        }

        [TestMethod]
        public void SameVersionReturnCompatibleVersion()
        {
            // Arrange
            var clientVersion = new Version(2, 2, 2, 2);
            var serverVersion = new Version(2, 2, 2, 2);

            var communication = Mock.Create<DiagnosticsEndpointCommunication>();
            Mock.Arrange(() => communication
                    .GetBaseUriVersion(Arg.IsAny<System.Data.Services.Client.DataServiceQuery<biz.dfch.CS.Appclusive.Core.OdataServices.Diagnostics.Endpoint>>()))
                .IgnoreInstance()
                .Returns(serverVersion)
                .MustBeCalled();

            var sut = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            
            // Act
            var result = sut.GetSemverCompatibility(clientVersion);

            // Assert
            Mock.Assert(communication);
            Assert.AreEqual(SemverCompatibilityFlags.Compatible, result);
        }

        [TestMethod]
        public void LastFieldIsIgnored()
        {
            // Arrange
            var clientVersion = new Version(2, 2, 2, 42);
            var serverVersion = new Version(2, 2, 2, 5);

            var communication = Mock.Create<DiagnosticsEndpointCommunication>();
            Mock.Arrange(() => communication
                    .GetBaseUriVersion(Arg.IsAny<System.Data.Services.Client.DataServiceQuery<biz.dfch.CS.Appclusive.Core.OdataServices.Diagnostics.Endpoint>>()))
                .IgnoreInstance()
                .Returns(serverVersion)
                .MustBeCalled();

            var sut = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            
            // Act
            var result = sut.GetSemverCompatibility(clientVersion);

            // Assert
            Mock.Assert(communication);
            Assert.AreEqual(SemverCompatibilityFlags.Compatible, result);
        }

        [TestMethod]
        public void ServerIsNewerMajorMinorPatch()
        {
            // Arrange
            var clientVersion = new Version(1, 1, 1, 1);
            var serverVersion = new Version(3, 3, 3, 2);

            var communication = Mock.Create<DiagnosticsEndpointCommunication>();
            Mock.Arrange(() => communication
                    .GetBaseUriVersion(Arg.IsAny<System.Data.Services.Client.DataServiceQuery<biz.dfch.CS.Appclusive.Core.OdataServices.Diagnostics.Endpoint>>()))
                .IgnoreInstance()
                .Returns(serverVersion)
                .MustBeCalled();

            var sut = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            
            // Act
            var result = sut.GetSemverCompatibility(clientVersion);

            // Assert
            Mock.Assert(communication);
            Assert.IsTrue(SemverCompatibilityFlags.ServerIsNewer < result);
            Assert.AreEqual(
                SemverCompatibilityFlags.BreakingChanges | SemverCompatibilityFlags.AdditionalFeatures | SemverCompatibilityFlags.Patched
                , result);
        }

        [TestMethod]
        public void ServerIsNewerMajorClientIsNewerMinor()
        {
            // Arrange
            var clientVersion = new Version(1, 3, 2, 2);
            var serverVersion = new Version(3, 1, 2, 2);

            var communication = Mock.Create<DiagnosticsEndpointCommunication>();
            Mock.Arrange(() => communication
                    .GetBaseUriVersion(Arg.IsAny<System.Data.Services.Client.DataServiceQuery<biz.dfch.CS.Appclusive.Core.OdataServices.Diagnostics.Endpoint>>()))
                .IgnoreInstance()
                .Returns(serverVersion)
                .MustBeCalled();

            var sut = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            
            // Act
            var result = sut.GetSemverCompatibility(clientVersion);

            // Assert
            Mock.Assert(communication);
            Assert.IsTrue(SemverCompatibilityFlags.ServerIsNewer < result);
            Assert.IsTrue(0 != (SemverCompatibilityFlags.BreakingChanges & result) ? true : false);
        }

        [TestMethod]
        public void ClientIsNewerMajorClientIsNewerMinor()
        {
            // Arrange
            var clientVersion = new Version(3, 3, 2, 2);
            var serverVersion = new Version(1, 1, 2, 2);

            var communication = Mock.Create<DiagnosticsEndpointCommunication>();
            Mock.Arrange(() => communication
                    .GetBaseUriVersion(Arg.IsAny<System.Data.Services.Client.DataServiceQuery<biz.dfch.CS.Appclusive.Core.OdataServices.Diagnostics.Endpoint>>()))
                .IgnoreInstance()
                .Returns(serverVersion)
                .MustBeCalled();

            var sut = new biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics(_uri);
            
            // Act
            var result = sut.GetSemverCompatibility(clientVersion);

            // Assert
            Mock.Assert(communication);
            Assert.IsFalse(SemverCompatibilityFlags.ServerIsNewer < result);
            Assert.IsFalse(0 != (SemverCompatibilityFlags.BreakingChanges & result) ? true : false);
            Assert.AreEqual(SemverCompatibilityFlags.Compatible, result);
        }
    }
}
