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
using System.Data.Services.Client;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Api
{
    [ContractClassFor(typeof(IOdataActionHelper))]
    internal abstract class ContractClassForIOdataActionHelper : IOdataActionHelper
    {
        public void InvokeEntitySetActionWithVoidResult(object entity, string actionName)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
        }

        public void InvokeEntitySetActionWithVoidResult(string entitySetName, string actionName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
        }

        public void InvokeEntitySetActionWithVoidResult(object entity, string actionName, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
        }

        public void InvokeEntitySetActionWithVoidResult(string entitySetName, string actionName, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
        }

        public BodyOperationParameter[] GetBodyOperationParametersFromObject(object input)
        {
            return default(BodyOperationParameter[]);
        }

        public BodyOperationParameter[] GetBodyOperationParametersFromHashtable(Hashtable input)
        {
            return default(BodyOperationParameter[]);
        }

        public BodyOperationParameter[] GetBodyOperationParametersFromDictionary(Dictionary<string, object> input)
        {
            return default(BodyOperationParameter[]);
        }
    }
}
