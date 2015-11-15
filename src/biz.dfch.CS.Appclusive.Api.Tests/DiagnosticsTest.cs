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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Appclusive.Api.Diagnostics;
using Telerik.JustMock;
using System.Configuration;
using System.Collections;

namespace biz.dfch.CS.Appclusive.Api.Tests
{
    [TestClass]
    public class DiagnosticsTest
    {
        private static string _uriPrefix;
        private static Uri _uri;

        static DiagnosticsTest()
        {
            _uriPrefix = ConfigurationManager.AppSettings["Service.Reference.URI.Prefix"];
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
    }
}
