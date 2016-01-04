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
using System.Net;

namespace biz.dfch.CS.Appclusive.Api.Core
{
    public partial class Core : global::System.Data.Services.Client.DataServiceContext, IDataServiceClientHelper, IOdataActionHelper
    {
        private const string AUTHORIZATION_HEADER_NAME = "Authorization";
        private const string AUTHORIZATION_BEARER_SCHEME = "Bearer {0}";
        public const string AuthorisationBaererUserName = "[AuthorisationBaererUser]";

        public new ICredentials Credentials
        {
            get
            {
                return base.Credentials;
            }
            set
            {
                if (base.Credentials != value)
                {
                    if (value is NetworkCredential)
                    {
                        NetworkCredential networkCredentials = (NetworkCredential)value;
                        if (Core.AuthorisationBaererUserName == networkCredentials.UserName)
                        {
                            this.SendingRequest2 += Core_SendingRequest2;
                        }
                        else
                        {
                            this.SendingRequest2 -= Core_SendingRequest2;
                        }
                    }
                    else
                    {
                        this.SendingRequest2 -= Core_SendingRequest2;
                    }
                    base.Credentials = value;
                }
            }
        }

        void Core_SendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            NetworkCredential networkCredentials = (NetworkCredential)this.Credentials;
            e.RequestMessage.SetHeader(Core.AUTHORIZATION_HEADER_NAME, string.Format(Core.AUTHORIZATION_BEARER_SCHEME, networkCredentials.Password));
        }

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

        public bool HasPendingEntityChanges()
        {
            var hasChanges = this.Entities.Any(e => e.State != EntityStates.Unchanged);
            return hasChanges;
        }

        public bool HasPendingLinkChanges()
        {
            var hasChanges = this.Links.Any(e => e.State != EntityStates.Unchanged);
            return hasChanges;
        }

        public bool HasPendingChanges()
        {
            return HasPendingEntityChanges() || HasPendingLinkChanges();
        }

        public void RevertEntityState(object entity)
        {
            var entityDescriptor = this.GetEntityDescriptor(entity);
            if (EntityStates.Added == entityDescriptor.State || EntityStates.Deleted == entityDescriptor.State)
            {
                this.ChangeState(entity, EntityStates.Detached);
            }
            else
            {
                this.ChangeState(entity, EntityStates.Unchanged);
            }
        }

        public void InvokeEntitySetActionWithVoidResult(object entity, string actionName, object inputParameters)
        {
            var entitySetName = string.Concat(entity.GetType().Name, "s");
            InvokeEntitySetActionWithVoidResult(entitySetName, actionName, inputParameters);
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

        public object InvokeEntitySetActionWithSingleResult(string entitySetName, string actionName, Type type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntitySetActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithSingleResult(object entity, string actionName, Type type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntitySetActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithSingleResult(string entitySetName, string actionName, object type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntitySetActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithSingleResult(object entity, string actionName, object type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntitySetActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public T InvokeEntitySetActionWithSingleResult<T>(string entitySetName, string actionName, object inputParameters)
        {
            const string METHOD_NAME = "POST";
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

            var result = this.Execute<T>(uriAction, METHOD_NAME, true, bodyParameters).Single();
            return result;
        }

        public T InvokeEntitySetActionWithSingleResult<T>(object entity, string actionName, object inputParameters)
        {
            var entitySetName = string.Concat(entity.GetType().Name, "s");

            var result = InvokeEntitySetActionWithSingleResult<T>(entitySetName, actionName, inputParameters);
            return result;
        }

        public object InvokeEntitySetActionWithListResult(string entitySetName, string actionName, Type type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntitySetActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithListResult(object entity, string actionName, Type type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntitySetActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithListResult(string entitySetName, string actionName, object type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntitySetActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithListResult(object entity, string actionName, object type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntitySetActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public IEnumerable<T> InvokeEntitySetActionWithListResult<T>(string entitySetName, string actionName, object inputParameters)
        {
            const string METHOD_NAME = "POST";
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

            return this.Execute<T>(uriAction, METHOD_NAME, true, bodyParameters).ToList();
        }

        public IEnumerable<T> InvokeEntitySetActionWithListResult<T>(object entity, string actionName, object inputParameters)
        {
            var entitySetName = string.Concat(entity.GetType().Name, "s");

            var result = InvokeEntitySetActionWithListResult<T>(entitySetName, actionName, inputParameters);
            return result;
        }

        public void InvokeEntityActionWithVoidResult(object entity, string actionName, object inputParameters)
        {
            var entitySetName = string.Concat(entity.GetType().Name, "s");
            dynamic dynamicEntity = entity;
            var id = dynamicEntity.Id;

            InvokeEntityActionWithVoidResult(entitySetName, id, actionName, inputParameters);
        }

        public void InvokeEntityActionWithVoidResult(string entitySetName, long id, string actionName, object inputParameters)
        {
            this.InvokeEntityActionWithVoidResult(entitySetName, (object)id, actionName, inputParameters);
        }

        public void InvokeEntityActionWithVoidResult(string entitySetName, object id, string actionName, object inputParameters)
        {
            var methodName = "POST";
            string entityUrl = Core.GetEntityUrl(entitySetName, id);
            var uriAction = new Uri(string.Concat(this.BaseUri.AbsoluteUri.TrimEnd('/'), "/", entityUrl, "/", actionName));

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

        public object InvokeEntityActionWithSingleResult(string entitySetName, long id, string actionName, Type type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntityActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, id, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithSingleResult(object entity, string actionName, Type type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntityActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithSingleResult(string entitySetName, long id, string actionName, object type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntityActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, id, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithSingleResult(object entity, string actionName, object type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntityActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public T InvokeEntityActionWithSingleResult<T>(string entitySetName, long id, string actionName, object inputParameters)
        {
            return this.InvokeEntityActionWithSingleResult<T>(entitySetName, (object)id, actionName, inputParameters);
        }

        public T InvokeEntityActionWithSingleResult<T>(string entitySetName, object id, string actionName, object inputParameters)
        {
            const string METHOD_NAME = "POST";
            string entityUrl = Core.GetEntityUrl(entitySetName, id);
            var uriAction = new Uri(string.Concat(this.BaseUri.AbsoluteUri.TrimEnd('/'), "/", entityUrl, "/", actionName));

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

            var result = this.Execute<T>(uriAction, METHOD_NAME, true, bodyParameters).Single();
            return result;
        }

        public T InvokeEntityActionWithSingleResult<T>(object entity, string actionName, object inputParameters)
        {
            var entitySetName = string.Concat(entity.GetType().Name, "s");

            dynamic dynamicEntity = entity;
            var id = dynamicEntity.Id;

            var result = InvokeEntityActionWithSingleResult<T>(entitySetName, id, actionName, inputParameters);
            return result;
        }

        public object InvokeEntityActionWithListResult(string entitySetName, long id, string actionName, Type type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntityActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, id, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithListResult(object entity, string actionName, Type type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntityActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithListResult(string entitySetName, long id, string actionName, object type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntityActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, id, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithListResult(object entity, string actionName, object type, object inputParameters)
        {
            var mi = this.GetType().GetMethods().Where(m => (m.Name == "InvokeEntityActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity")).First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public IEnumerable<T> InvokeEntityActionWithListResult<T>(string entitySetName, long id, string actionName, object inputParameters)
        {
            return this.InvokeEntityActionWithListResult<T>(entitySetName, (object)id, actionName, inputParameters);
        }

        public IEnumerable<T> InvokeEntityActionWithListResult<T>(string entitySetName, object id, string actionName, object inputParameters)
        {
            const string METHOD_NAME = "POST";
            string entityUrl = Core.GetEntityUrl(entitySetName, id);
            var uriAction = new Uri(string.Concat(this.BaseUri.AbsoluteUri.TrimEnd('/'), "/", entityUrl, "/", actionName));

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

            return this.Execute<T>(uriAction, METHOD_NAME, true, bodyParameters);
        }

        public IEnumerable<T> InvokeEntityActionWithListResult<T>(object entity, string actionName, object inputParameters)
        {
            var entitySetName = string.Concat(entity.GetType().Name, "s");

            dynamic dynamicEntity = entity;
            var id = dynamicEntity.Id;

            var result = InvokeEntityActionWithListResult<T>(entitySetName, id, actionName, inputParameters);
            return result;
        }

        private static string GetEntityUrl(string entitySetName, object id)
        {
            string entityUrl = null;
            if (id is long)
            {
                entityUrl = string.Format("{0}({1}L)", entitySetName, id);
            }
            else if (id is Guid)
            {
                entityUrl = string.Format("{0}(guid'{1}')", entitySetName, id);
            }
            else
            {
                throw new Exception(string.Format("Id type '{0}' not supported", id.GetType()));
            }
            return entityUrl;
        }

        public BodyOperationParameter[] GetBodyOperationParametersFromObject(object input)
        {
            var operationParameters = new List<BodyOperationParameter>();
            if (null == input)
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
            foreach (var field in fields)
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
