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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Api.Core
{
    public partial class Core : global::System.Data.Services.Client.DataServiceContext
    {
        public static Version GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            return assemblyName.Version;
        }

        public void AttachIfNeeded(object entity)
        {
            Contract.Requires(null != entity);

            var entitySetName = string.Concat(entity.GetType().Name, "s");

            AttachIfNeededPrivate(entitySetName, entity);
            return;
        }

        public void AttachIfNeeded(string entitySetName, object entity)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));

            AttachIfNeededPrivate(entitySetName, entity);
            return;
        }

        private void AttachIfNeededPrivate(string entitySetName, object entity)
        {
            try
            {
                this.AttachTo(entitySetName, entity);
            }
            catch (InvalidOperationException ex)
            {
                if (!ex.Message.Equals("The context is already tracking the entity."))
                {
                    throw;
                }
            }
        }
    }
}
