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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using biz.dfch.CS.Appclusive.Api.Core;
using System.Configuration;

namespace biz.dfch.CS.Appclusive.Api.Tests
{
    [TestClass]
    public class CoreTest
    {
        [TestMethod]
        public void AttachingDetachedEntitySucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(new Uri("http://localhost"));
            var entity = new Node();
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
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(new Uri("http://localhost"));
            var entity = new Node();
            var entitySetName = "InvalidEntitySetName";
            Mock.Arrange(() => svc.AttachTo(Arg.AnyString, Arg.AnyObject)).OccursOnce();

            // Act
            svc.AttachIfNeeded(entitySetName, entity);

            // Assert
            Mock.Assert(svc);
        }

        private static string _uriPrefix;
        private static Uri _uri;
        private static long nodeId = 1218;

        static CoreTest()
        {
            _uriPrefix = ConfigurationManager.AppSettings["Service.Reference.URI.Prefix"];
            _uri = new Uri(_uriPrefix + "Core");
        }

        [TestMethod]
        public void InvokeCoreNodeTemplateWithExecuteSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            var _uriAction = new Uri(_uriPrefix + "Core/Nodes/Template");

            // Act
            var result = svc.Execute<Node>(_uriAction, "POST", true).Single();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeTemplateWithGenericHelperAndEntitySetNameSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            var _uriAction = new Uri(_uriPrefix + "Core/Nodes/Template");

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult<Node>("Nodes", "Template", null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeTemplateWithGenericHelperAndEntitySucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult<Node>(new Node(), "Template", null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeTemplateWithNonGenericHelperAndEntitySetNameAndObjectSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult("Nodes", "Template", new Node(), null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeTemplateWithNonGenericHelperAndEntityAndObjectSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult(new Node(), "Template", new Node(), null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeTemplateWithNonGenericHelperAndEntitySetNameAndTypeSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult("Nodes", "Template", typeof(Node), null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeTemplateWithNonGenericHelperAndEntityAndTypeSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntitySetActionWithSingleResult(new Node(), "Template", typeof(Node), null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeStatusWithGenericHelperAndEntitySetNameSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithSingleResult<Job>("Nodes", CoreTest.nodeId, "Status", null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeStatusWithGenericHelperAndEntitySucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithSingleResult<Job>(new Node() { Id = CoreTest.nodeId }, "Status", null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeStatusWithNonGenericHelperAndEntitySetNameAndObjectSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithSingleResult("Nodes", CoreTest.nodeId, "Status", new Job(), null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeStatusWithNonGenericHelperAndEntityAndObjectSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithSingleResult(new Node() { Id = CoreTest.nodeId }, "Status", new Job(), null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeStatusWithNonGenericHelperAndEntitySetNameAndTypeSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithSingleResult("Nodes", CoreTest.nodeId, "Status", typeof(Job), null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeStatusWithNonGenericHelperAndEntityAndTypeSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithSingleResult(new Node() { Id = CoreTest.nodeId }, "Status", typeof(Job), null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeAvailableActionsWithGenericHelperAndEntitySetNameSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithListResult<string>("Nodes", CoreTest.nodeId, "AvailableActions", null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeAvailableActionsWithGenericHelperAndEntitySucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithListResult<string>(new Node() { Id = CoreTest.nodeId }, "AvailableActions", null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeAvailableActionsWithNonGenericHelperAndEntitySetNameAndObjectSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithListResult("Nodes", CoreTest.nodeId, "AvailableActions", "", null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeAvailableActionsWithNonGenericHelperAndEntityAndObjectSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithListResult(new Node() { Id = CoreTest.nodeId }, "AvailableActions", "", null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeAvailableActionsWithNonGenericHelperAndEntitySetNameAndTypeSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithListResult("Nodes", CoreTest.nodeId, "AvailableActions", typeof(string), null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void InvokeCoreNodeAvailableActionsWithNonGenericHelperAndEntityAndTypeSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            svc.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            // Act
            var result = svc.InvokeEntityActionWithListResult(new Node() { Id = CoreTest.nodeId }, "AvailableActions", typeof(string), null);

            // Assert
            Assert.IsNotNull(result);
        }

        private const string FIELD3_VALUE = "field3";

        public class BodyOperationTestClass
        {
            public string Param1 { get; set; }
            public int Param2;
            public string Field3 = FIELD3_VALUE;
        }

        [TestMethod]
        public void GetBodyOperationParametersFromObjectSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);

            var param1Value = "some arbitrary value";
            var param2Value = 42;
            var input = new BodyOperationTestClass()
            {
                Param1 = param1Value
                ,
                Param2 = param2Value
            };

            // Act
            var result = svc.GetBodyOperationParametersFromObject(input);

            // Assert
            Assert.AreEqual(3, result.Count());
            var p1 = result.Where(e => e.Name == "Param1").Single();
            Assert.AreEqual(param1Value, p1.Value);
            var p2 = result.Where(e => e.Name == "Param2").Single();
            Assert.AreEqual(param2Value, p2.Value);
            var p3 = result.Where(e => e.Name == "Field3").Single();
            Assert.AreEqual(FIELD3_VALUE, p3.Value);
        }

        [TestMethod]
        public void GetBodyOperationParametersFromHashtableSucceeds()
        {
            // Arrange
            var param1Value = "some arbitrary value";
            var param2Value = 42;

            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);

            var input = new Hashtable();
            input.Add("Param1", param1Value);
            input.Add("Param2", param2Value);
            input.Add("Field3", FIELD3_VALUE);

            // Act
            var result = svc.GetBodyOperationParametersFromHashtable(input);

            // Assert
            Assert.AreEqual(3, result.Count());
            var p1 = result.Where(e => e.Name == "Param1").Single();
            Assert.AreEqual(param1Value, p1.Value);
            var p2 = result.Where(e => e.Name == "Param2").Single();
            Assert.AreEqual(param2Value, p2.Value);
            var p3 = result.Where(e => e.Name == "Field3").Single();
            Assert.AreEqual(FIELD3_VALUE, p3.Value);
        }

        [TestMethod]
        public void GetBodyOperationParametersFromDictionarySucceeds()
        {
            // Arrange
            var param1Value = "some arbitrary value";
            var param2Value = 42;

            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);

            var input = new Dictionary<string, object>();
            input.Add("Param1", param1Value);
            input.Add("Param2", param2Value);
            input.Add("Field3", FIELD3_VALUE);

            // Act
            var result = svc.GetBodyOperationParametersFromDictionary(input);

            // Assert
            Assert.AreEqual(3, result.Count());
            var p1 = result.Where(e => e.Name == "Param1").Single();
            Assert.AreEqual(param1Value, p1.Value);
            var p2 = result.Where(e => e.Name == "Param2").Single();
            Assert.AreEqual(param2Value, p2.Value);
            var p3 = result.Where(e => e.Name == "Field3").Single();
            Assert.AreEqual(FIELD3_VALUE, p3.Value);
        }

        [TestMethod]
        public void HasPendingChangesWithNoChangesReturnsFalse()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);

            // Act
            var result = svc.HasPendingChanges();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasPendingChangesWithChangesReturnsTrue()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            var node = new Node();
            svc.AddToNodes(node);

            // Act
            var result = svc.HasPendingChanges();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RevertEntityStateWithAddedStateSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            var node = new Node();
            svc.AddToNodes(node);

            // Act
            svc.RevertEntityState(node);

            // Assert
            Assert.IsFalse(svc.HasPendingChanges());
        }

        [TestMethod]
        public void RevertEntityStateWithModifiedStateSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            var node = svc.Nodes.First();
            node.Description = "arbitrary-changed-description-setting-the-entity-state-to-modified";

            // Act
            svc.RevertEntityState(node);

            // Assert
            Assert.IsFalse(svc.HasPendingChanges());
        }

        [TestMethod]
        public void RevertEntityStateWithDeletedStateSucceeds()
        {
            // Arrange
            var svc = new biz.dfch.CS.Appclusive.Api.Core.Core(_uri);
            var entity = svc.Nodes.First();
            svc.DeleteObject(entity);

            // Act
            svc.RevertEntityState(entity);

            // Assert
            Assert.IsFalse(svc.HasPendingChanges());
        }
    }
}
