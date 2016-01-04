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

        public T InvokeEntitySetActionWithSingleResult<T>(string entitySetName, string actionName, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));

            return default(T);
        }

        public T InvokeEntitySetActionWithSingleResult<T>(object entity, string actionName, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));

            return default(T);
        }

        public object InvokeEntitySetActionWithSingleResult(string entitySetName, string actionName, object type, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntitySetActionWithSingleResult(object entity, string actionName, object type, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntitySetActionWithSingleResult(string entitySetName, string actionName, Type type, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntitySetActionWithSingleResult(object entity, string actionName, Type type, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntitySetActionWithListResult(string entitySetName, string actionName, Type type, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntitySetActionWithListResult(object entity, string actionName, Type type, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntitySetActionWithListResult(string entitySetName, string actionName, object type, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntitySetActionWithListResult(object entity, string actionName, object type, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public IEnumerable<T> InvokeEntitySetActionWithListResult<T>(string entitySetName, string actionName, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));

            return default(IEnumerable<T>);
        }

        public IEnumerable<T> InvokeEntitySetActionWithListResult<T>(object entity, string actionName, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));

            return default(IEnumerable<T>);
        }

        public void InvokeEntityInstanceActionWithVoidResult(object entity, string actionName)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
        }

        public void InvokeEntityInstanceActionWithVoidResult(string entitySetName, long id, string actionName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(id > 0);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
        }

        public void InvokeEntityActionWithVoidResult(object entity, string actionName, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
        }

        public void InvokeEntityActionWithVoidResult(string entitySetName, object id, string actionName, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(null != id);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
        }

        public void InvokeEntityActionWithVoidResult(string entitySetName, long id, string actionName, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(id > 0);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
        }

        public T InvokeEntityActionWithSingleResult<T>(string entitySetName, object id, string actionName, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(null != id);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));

            return default(T);
        }

        public T InvokeEntityActionWithSingleResult<T>(string entitySetName, long id, string actionName, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(id > 0);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));

            return default(T);
        }

        public T InvokeEntityActionWithSingleResult<T>(object entity, string actionName, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));

            return default(T);
        }

        public object InvokeEntityActionWithSingleResult(string entitySetName, long id, string actionName, object type, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(id > 0);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntityActionWithSingleResult(object entity, string actionName, object type, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntityActionWithSingleResult(string entitySetName, long id, string actionName, Type type, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(id > 0);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntityActionWithSingleResult(object entity, string actionName, Type type, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntityActionWithListResult(string entitySetName, long id, string actionName, Type type, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(id > 0);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntityActionWithListResult(object entity, string actionName, Type type, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntityActionWithListResult(string entitySetName, long id, string actionName, object type, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(id > 0);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public object InvokeEntityActionWithListResult(object entity, string actionName, object type, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));
            Contract.Requires(null != type);

            return default(object);
        }

        public IEnumerable<T> InvokeEntityActionWithListResult<T>(string entitySetName, long id, string actionName, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(id > 0);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));

            return default(IEnumerable<T>);
        }

        public IEnumerable<T> InvokeEntityActionWithListResult<T>(string entitySetName, object id, string actionName, object inputParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(entitySetName));
            Contract.Requires(null!= id);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));

            return default(IEnumerable<T>);
        }

        public IEnumerable<T> InvokeEntityActionWithListResult<T>(object entity, string actionName, object inputParameters)
        {
            Contract.Requires(null != entity);
            Contract.Requires(!string.IsNullOrWhiteSpace(actionName));

            return default(IEnumerable<T>);
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
