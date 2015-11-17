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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Services.Client;

namespace biz.dfch.CS.Appclusive.Api.Core
{
    public partial class Core : global::System.Data.Services.Client.DataServiceContext, IDataServiceClientHelper, IOdataActionHelper
    {
        public static Version GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            return assemblyName.Version;
        }

        public void AttachIfNeeded(object entity)
        {
            var entitySetName = string.Concat(entity.GetType().Name, "s");

            AttachIfNeededPrivate(entitySetName, entity);
            return;
        }

        public void AttachIfNeeded(string entitySetName, object entity)
        {
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

        public void InvokeEntitySetActionWithVoidResult(object entity, string actionName, object inputParameters)
        {
            InvokeEntitySetActionWithVoidResult(string.Concat(entity.GetType().Name, "s"), actionName, inputParameters);
        }

        public void InvokeEntitySetActionWithVoidResult(string entitySetName, string actionName, object inputParameters)
        {
            var methodName = "POST";
            var uriAction = new Uri(string.Concat(this.BaseUri.AbsoluteUri.TrimEnd('/'), "/", entitySetName, "/", actionName));

            BodyOperationParameter[] bodyParameters;
            if (inputParameters is Hashtable)
            {
                bodyParameters = GetBodyOperationParametersFromHashtable(inputParameters as Hashtable);
            }
            else if (inputParameters is Dictionary<string, object>)
            {
                bodyParameters = GetBodyOperationParametersFromDictionary(inputParameters as Dictionary<string, object>);
            }
            else
            {
                bodyParameters = GetBodyOperationParametersFromObject(inputParameters);
            }
            var result = this.Execute(uriAction, methodName, bodyParameters);
        }

        public object InvokeEntitySetActionWithSingleResult(object entity, string actionName, Type type, object inputParameters)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public object InvokeEntitySetActionWithSingleResult(object entity, string actionName, object type, object inputParameters)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public T InvokeEntitySetActionWithSingleResult<T>(object entity, string actionName, object inputParameters)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public BodyOperationParameter[] GetBodyOperationParametersFromObject(object input)
        {
            var operationParameters = new List<BodyOperationParameter>();
            if(null == input)
            {
                return operationParameters.ToArray();
            }

            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            var properties = input.GetType().GetProperties(bindingFlags);
            foreach (var property in properties)
            {
                var operationParameter = new BodyOperationParameter(property.Name, property.GetValue(input));
                operationParameters.Add(operationParameter);
            }
            var fields = input.GetType().GetFields(bindingFlags);
            foreach(var field in fields)
            {
                var operationParameter = new BodyOperationParameter(field.Name, field.GetValue(input));
                operationParameters.Add(operationParameter);
            }
            return operationParameters.ToArray();
        }

        public BodyOperationParameter[] GetBodyOperationParametersFromHashtable(Hashtable input)
        {
            var operationParameters = new List<BodyOperationParameter>();
            if (null == input)
            {
                return operationParameters.ToArray();
            }

            foreach (DictionaryEntry entry in input)
            {
                var operationParameter = new BodyOperationParameter(entry.Key.ToString(), entry.Value);
                operationParameters.Add(operationParameter);
            }
            return operationParameters.ToArray();
        }

        public BodyOperationParameter[] GetBodyOperationParametersFromDictionary(Dictionary<string, object> input)
        {
            var operationParameters = new List<BodyOperationParameter>();
            if (null == input)
            {
                return operationParameters.ToArray();
            }

            foreach (var entry in input)
            {
                var operationParameter = new BodyOperationParameter(entry.Key, entry.Value);
                operationParameters.Add(operationParameter);
            }
            return operationParameters.ToArray();
        }
    }
}
