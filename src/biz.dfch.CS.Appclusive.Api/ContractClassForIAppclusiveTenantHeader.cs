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

using System.Diagnostics.Contracts;

namespace biz.dfch.CS.Appclusive.Api
{
    [ContractClassFor(typeof(IAppclusiveTenantHeader))]
    internal abstract class ContractClassForIAppclusiveTenantHeader : IAppclusiveTenantHeader
    {
        public void AttachIfNeeded(object entity)
        {
            Contract.Requires(null != entity);
        }

        public void AttachIfNeeded(string entitySetName, object entity)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(null != entity);
        }

        public bool HasPendingEntityChanges()
        {
            return default(bool);
        }

        public bool HasPendingLinkChanges()
        {
            return default(bool);
        }

        public bool HasPendingChanges()
        {
            return default(bool);
        }

        public void RevertEntityState(object entity)
        {
            Contract.Requires(null != entity);
        }
    }
}