/**
 * Copyright 2015 Marc Rufer, d-fens GmbH
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
using System.Collections.Generic;
﻿using System.Configuration;
﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.DaaS.Api.Core;
using System.Data.Services.Client;

namespace biz.dfch.CS.DaaS.Api.Tests.Core
{
    [TestClass]
    public class OrderTest
    {
        private String _uriPrefix = ConfigurationManager.AppSettings["Service.Reference.URI.Prefix"];

        [TestMethod]
        public void CreateOrderCreatesOrderItemsJobsAndApproval()
        {
            var uri = new Uri(_uriPrefix + "Core");
            biz.dfch.CS.DaaS.Api.Core.Core svc = new biz.dfch.CS.DaaS.Api.Core.Core(uri);
            svc.Credentials = System.Net.CredentialCache.DefaultCredentials;

            var catalogueItem = new CatalogueItem();
            catalogueItem.Created = DateTimeOffset.Now;
            catalogueItem.CreatedBy = "User";
            catalogueItem.Modified = DateTimeOffset.Now;
            catalogueItem.ModifiedBy = "User";
            catalogueItem.Name = "CatalogueItem";
            catalogueItem.Tid = "Tid";
            catalogueItem.Version = "1.0";
            catalogueItem.Collection = "";

            svc.AddToCatalogueItems(catalogueItem);
            var catalogueItems = svc.CatalogueItems.Execute();
            Assert.AreEqual(1, catalogueItems.Count());

            var order = new Order();
            order.Created = DateTimeOffset.Now;
            order.CreatedBy = "User";
            order.Modified = DateTimeOffset.Now;
            order.ModifiedBy = "User";
            order.Description = "Description";
            order.Name = "MyOrder";
            order.OrderItems = new DataServiceCollection<OrderItem>();
            order.Parameters = "[{\"Quantity\":1,\"CatalogueItemId\":1}]";
            order.Status = "Created";
            order.Tid = "Tid";

            svc.AddToOrders(order);

            var orderResult = svc.Orders.Execute();
            Assert.AreEqual(1, orderResult.Count());
            var orderEntity = orderResult.FirstOrDefault();
            Assert.AreEqual("Created", orderEntity.Status);
            Assert.AreNotEqual("User", orderEntity.CreatedBy);

            var orderItemsResult = svc.OrderItems.Execute();
            Assert.AreEqual(1, orderItemsResult.Count());
            var orderItemEntity = orderItemsResult.FirstOrDefault();
            Assert.IsFalse(String.IsNullOrWhiteSpace(orderItemEntity.Parameters));

            var jobResult = svc.Jobs.Execute();
            Assert.AreEqual(2, jobResult);
            var jobResultEntity = jobResult.FirstOrDefault();

            var approvalResult = svc.Approvals;
            Assert.AreEqual(1, approvalResult.Count());
            var approval = approvalResult.FirstOrDefault();
        }

        [TestMethod]
        public void ApproveApprovalChangesStateOfJobsApprovalAndOrder()
        {
            // DFTODO Approve/Decline (-> Approval itself, Jobs and Order get adjusted)
        }

        [TestMethod]
        public void DeclineApprovalChangesStateOfJobsApprovalAndOrder()
        {
            // DFTODO Approve/Decline (-> Approval itself, Jobs and Order get adjusted)
        }
    }
}
