﻿/**
 * Copyright 2016 d-fens GmbH
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
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Api.Fab
{
    public partial class Fab : global::System.Data.Services.Client.DataServiceContext, IDataServiceClientHelper, IOdataActionHelper
    {
        #region Request Headers

        private const string AUTHORIZATION_HEADER_NAME = "Authorization";
        private const string AUTHORIZATION_BEARER_SCHEME = "Bearer {0}";
        private const string AUTHORIZATION_BASIC_SCHEME = "Basic {0}";
        private const string DEFAULT_TENANT_HEADER_NAME = "Biz-Dfch-Tenant-Id";
        public const string AuthorisationBaererUserName = "[AuthorisationBaererUser]";

        private string tenantHeaderName;
        public string TenantHeaderName
        {
            get
            {
                if (string.IsNullOrEmpty(this.tenantHeaderName))
                {
                    this.tenantHeaderName = Fab.DEFAULT_TENANT_HEADER_NAME;
                }
                return this.tenantHeaderName;
            }
            set
            {
                this.tenantHeaderName = value;
            }
        }

        private string tenantID;
        public string TenantID
        {
            get
            {
                return this.tenantID;
            }
            set
            {
                if (value != tenantID)
                {
                    this.tenantID = value;
                    this.RegisterSendingRequestEvent();
                }
            }
        }

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
                    base.Credentials = value;
                    this.RegisterSendingRequestEvent();
                }
            }
        }

        private bool SetBasicAuthenticationHeader()
        {
            bool setHeader = false;
            if (this.Credentials is NetworkCredential)
            {
                NetworkCredential networkCredentials = (NetworkCredential)this.Credentials;
                if ((!string.IsNullOrEmpty(networkCredentials.UserName)) && (!string.IsNullOrEmpty(networkCredentials.Password)))
                {
                    setHeader = Fab.AuthorisationBaererUserName != networkCredentials.UserName;
                }
            }
            return setHeader;
        }

        private bool SetBearerAuthenticationHeader()
        {
            bool setHeader = false;
            if (this.Credentials is NetworkCredential)
            {
                NetworkCredential networkCredentials = (NetworkCredential)this.Credentials;
                if ((!string.IsNullOrEmpty(networkCredentials.UserName)) && (!string.IsNullOrEmpty(networkCredentials.Password)))
                {
                    setHeader = Fab.AuthorisationBaererUserName == networkCredentials.UserName;
                }
            }
            return setHeader;
        }

        private bool SetTenantHeader()
        {
            return !string.IsNullOrEmpty(this.TenantID);
        }

        private void RegisterSendingRequestEvent()
        {

            if ((this.SetBearerAuthenticationHeader()) || (this.SetBasicAuthenticationHeader()) || (this.SetTenantHeader()))
            {
                this.SendingRequest2 += Fab_SendingRequest2;
            }
            else
            {
                this.SendingRequest2 -= Fab_SendingRequest2;
            }
        }

        void Fab_SendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            if (this.SetBearerAuthenticationHeader())
            {
                NetworkCredential networkCredentials = (NetworkCredential)this.Credentials;
                e.RequestMessage.SetHeader(Fab.AUTHORIZATION_HEADER_NAME, string.Format(Fab.AUTHORIZATION_BEARER_SCHEME, networkCredentials.Password));
            }

            if (this.SetBasicAuthenticationHeader())
            {
                NetworkCredential networkCredentials = (NetworkCredential)this.Credentials;
                string basicAuthString = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", networkCredentials.UserName, networkCredentials.Password)));
                e.RequestMessage.SetHeader(Fab.AUTHORIZATION_HEADER_NAME, string.Format(Fab.AUTHORIZATION_BASIC_SCHEME, basicAuthString));
            }

            if (this.SetTenantHeader())
            {
                e.RequestMessage.SetHeader(this.TenantHeaderName, this.TenantID);
            }
        }

        #endregion Request Headers

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

            return this.Execute<T>(uriAction, METHOD_NAME, false, bodyParameters).ToList();
        }

        public IEnumerable<T> InvokeEntitySetActionWithListResult<T>(object entity, string actionName, object inputParameters)
        {
            var entitySetName = string.Concat(entity.GetType().Name, "s");

            var result = InvokeEntitySetActionWithListResult<T>(entitySetName, actionName, inputParameters).ToList();
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
            string entityUrl = Fab.GetEntityUrl(entitySetName, id);
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
            string entityUrl = Fab.GetEntityUrl(entitySetName, id);
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
            string entityUrl = Fab.GetEntityUrl(entitySetName, id);
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

            return this.Execute<T>(uriAction, METHOD_NAME, false, bodyParameters).ToList();
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