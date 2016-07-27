/**
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Data.Services.Common;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Api
{
    public class DataServiceContextBase : 
        DataServiceContext,
        IDataServiceClientHelper, 
        IOdataActionHelper,
        IAppclusiveTenantHeader
    {

        #region Constructors from DataServiceContext
        
        // Summary:
        //     Initializes a new instance of the System.Data.Services.Client.DataServiceContext
        //     class.
        //
        // Remarks:
        //     It is expected that the BaseUri or ResolveEntitySet properties will be set
        //     before using the newly created context.
        public DataServiceContextBase()
        {
            // N/A
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Data.Services.Client.DataServiceContext
        //     class with the specified serviceRoot.
        //
        // Parameters:
        //   serviceRoot:
        //     An absolute URI that identifies the root of a data service.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     When the serviceRoot is null.
        //
        //   System.ArgumentException:
        //     If the serviceRoot is not an absolute URI -or-If the serviceRoot is a well
        //     formed URI without a query or query fragment.
        //
        // Remarks:
        //     The library expects the Uri to point to the root of a data service, but does
        //     not issue a request to validate it does indeed identify the root of a service.
        //      If the Uri does not identify the root of the service, the behavior of the
        //     client library is undefined. A Uri provided with a trailing slash is equivalent
        //     to one without such a trailing character.  With Silverlight, the serviceRoot
        //     can be a relative Uri that will be combined with System.Windows.Browser.HtmlPage.Document.DocumentUri.

        public DataServiceContextBase(Uri serviceRoot)
            : base(serviceRoot)
        {
            // N/A
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Data.Services.Client.DataServiceContext
        //     class with the specified serviceRoot and targeting the specific maxProtocolVersion.
        //
        // Parameters:
        //   serviceRoot:
        //     An absolute URI that identifies the root of a data service.
        //
        //   maxProtocolVersion:
        //     A System.Data.Services.Common.DataServiceProtocolVersion value that is the
        //     maximum protocol version that the client understands.
        //
        // Remarks:
        //     The library expects the Uri to point to the root of a data service, but does
        //     not issue a request to validate it does indeed identify the root of a service.
        //      If the Uri does not identify the root of the service, the behavior of the
        //     client library is undefined. A Uri provided with a trailing slash is equivalent
        //     to one without such a trailing character.  With Silverlight, the serviceRoot
        //     can be a relative Uri that will be combined with System.Windows.Browser.HtmlPage.Document.DocumentUri.
        public DataServiceContextBase(Uri serviceRoot, DataServiceProtocolVersion maxProtocolVersion)
            : base(serviceRoot, maxProtocolVersion)
        {
            // N/A
        }
        
        #endregion

        public static Version GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();
            return assemblyName.Version;
        }
        
        #region IDataServiceClientHelper

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
                AttachTo(entitySetName, entity);
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
            var hasChanges = Entities.Any(e => e.State != EntityStates.Unchanged);
            return hasChanges;
        }

        public bool HasPendingLinkChanges()
        {
            var hasChanges = Links.Any(e => e.State != EntityStates.Unchanged);
            return hasChanges;
        }

        public bool HasPendingChanges()
        {
            return HasPendingEntityChanges() || HasPendingLinkChanges();
        }

        public void RevertEntityState(object entity)
        {
            var entityDescriptor = GetEntityDescriptor(entity);
            if (EntityStates.Added == entityDescriptor.State || EntityStates.Deleted == entityDescriptor.State)
            {
                ChangeState(entity, EntityStates.Detached);
            }
            else
            {
                ChangeState(entity, EntityStates.Unchanged);
            }
        }

        #endregion

        #region IOdataActionHelper

        public void InvokeEntitySetActionWithVoidResult(object entity, string actionName, object inputParameters)
        {
            var entitySetName = string.Concat(entity.GetType().Name, "s");
            InvokeEntitySetActionWithVoidResult(entitySetName, actionName, inputParameters);
        }

        public void InvokeEntitySetActionWithVoidResult(string entitySetName, string actionName, object inputParameters)
        {
            var methodName = "POST";
            var uriAction = new Uri(string.Concat(BaseUri.AbsoluteUri.TrimEnd('/'), "/", entitySetName, "/", actionName));

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
            var result = Execute(uriAction, methodName, bodyParameters);
        }

        public object InvokeEntitySetActionWithSingleResult(string entitySetName, string actionName, Type type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntitySetActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithSingleResult(object entity, string actionName, Type type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntitySetActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithSingleResult(string entitySetName, string actionName, object type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntitySetActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithSingleResult(object entity, string actionName, object type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntitySetActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public T InvokeEntitySetActionWithSingleResult<T>(string entitySetName, string actionName, object inputParameters)
        {
            const string METHOD_NAME = "POST";
            var uriAction = new Uri(string.Concat(BaseUri.AbsoluteUri.TrimEnd('/'), "/", entitySetName, "/", actionName));

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

            var result = Execute<T>(uriAction, METHOD_NAME, true, bodyParameters).Single();
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
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntitySetActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithListResult(object entity, string actionName, Type type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntitySetActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithListResult(string entitySetName, string actionName, object type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntitySetActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, actionName, inputParameters });
            return result;
        }

        public object InvokeEntitySetActionWithListResult(object entity, string actionName, object type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntitySetActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public IEnumerable<T> InvokeEntitySetActionWithListResult<T>(string entitySetName, string actionName, object inputParameters)
        {
            const string METHOD_NAME = "POST";
            var uriAction = new Uri(string.Concat(BaseUri.AbsoluteUri.TrimEnd('/'), "/", entitySetName, "/", actionName));

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

            return Execute<T>(uriAction, METHOD_NAME, false, bodyParameters).ToList();
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
            InvokeEntityActionWithVoidResult(entitySetName, (object)id, actionName, inputParameters);
        }

        public void InvokeEntityActionWithVoidResult(string entitySetName, object id, string actionName, object inputParameters)
        {
            var methodName = "POST";
            var entityUrl = GetEntityUrl(entitySetName, id);
            var uriAction = new Uri(string.Concat(BaseUri.AbsoluteUri.TrimEnd('/'), "/", entityUrl, "/", actionName));

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
            var result = Execute(uriAction, methodName, bodyParameters);
        }

        public object InvokeEntityActionWithSingleResult(string entitySetName, long id, string actionName, Type type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntityActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, id, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithSingleResult(object entity, string actionName, Type type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntityActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithSingleResult(string entitySetName, long id, string actionName, object type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntityActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, id, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithSingleResult(object entity, string actionName, object type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntityActionWithSingleResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public T InvokeEntityActionWithSingleResult<T>(string entitySetName, long id, string actionName, object inputParameters)
        {
            return InvokeEntityActionWithSingleResult<T>(entitySetName, (object)id, actionName, inputParameters);
        }

        public T InvokeEntityActionWithSingleResult<T>(string entitySetName, object id, string actionName, object inputParameters)
        {
            const string METHOD_NAME = "POST";
            var entityUrl = GetEntityUrl(entitySetName, id);
            var uriAction = new Uri(string.Concat(BaseUri.AbsoluteUri.TrimEnd('/'), "/", entityUrl, "/", actionName));

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

            var result = Execute<T>(uriAction, METHOD_NAME, true, bodyParameters).Single();
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
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntityActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, id, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithListResult(object entity, string actionName, Type type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntityActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type);
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithListResult(string entitySetName, long id, string actionName, object type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntityActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entitySetName").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entitySetName, id, actionName, inputParameters });
            return result;
        }

        public object InvokeEntityActionWithListResult(object entity, string actionName, object type, object inputParameters)
        {
            var mi = GetCallerType().GetMethods().Where(m => m.Name == "InvokeEntityActionWithListResult" && m.IsGenericMethod && m.GetParameters()[0].Name == "entity").First();
            Contract.Assert(null != mi, "No generic method type found.");
            var genericMethod = mi.MakeGenericMethod(type.GetType());
            Contract.Assert(null != genericMethod, "Cannot create generic method.");

            var result = genericMethod.Invoke(this, new object[] { entity, actionName, inputParameters });
            return result;
        }

        public IEnumerable<T> InvokeEntityActionWithListResult<T>(string entitySetName, long id, string actionName, object inputParameters)
        {
            return InvokeEntityActionWithListResult<T>(entitySetName, (object)id, actionName, inputParameters);
        }

        public IEnumerable<T> InvokeEntityActionWithListResult<T>(string entitySetName, object id, string actionName, object inputParameters)
        {
            const string METHOD_NAME = "POST";
            var entityUrl = GetEntityUrl(entitySetName, id);
            var uriAction = new Uri(string.Concat(BaseUri.AbsoluteUri.TrimEnd('/'), "/", entityUrl, "/", actionName));

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

            return Execute<T>(uriAction, METHOD_NAME, false, bodyParameters).ToList();
        }

        public IEnumerable<T> InvokeEntityActionWithListResult<T>(object entity, string actionName, object inputParameters)
        {
            var entitySetName = string.Concat(entity.GetType().Name, "s");

            dynamic dynamicEntity = entity;
            var id = dynamicEntity.Id;

            var result = InvokeEntityActionWithListResult<T>(entitySetName, id, actionName, inputParameters);
            return result;
        }

        private string GetEntityUrl(string entitySetName, object id)
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

        private Type GetCallerType()
        {
            // this is an internal method that will retrieve the caller type of the caller 
            var frame = new StackFrame(2);
            var method = frame.GetMethod();

            var type = method.DeclaringType;
            return type;
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
        
        #endregion
   
        #region IAppclusiveTenantHeader

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
                if (string.IsNullOrEmpty(tenantHeaderName))
                {
                    tenantHeaderName = DEFAULT_TENANT_HEADER_NAME;
                }
                return tenantHeaderName;
            }
            set
            {
                tenantHeaderName = value;
            }
        }

        private string tenantID;
        public string TenantID
        {
            get
            {
                return tenantID;
            }
            set
            {
                if (value != tenantID)
                {
                    tenantID = value;
                    RegisterSendingRequestEvent();
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
                    RegisterSendingRequestEvent();
                }
            }
        }

        private bool SetBasicAuthenticationHeader()
        {
            var setHeader = false;
            if (Credentials is NetworkCredential)
            {
                var networkCredentials = (NetworkCredential)Credentials;
                if (!string.IsNullOrEmpty(networkCredentials.UserName) && !string.IsNullOrEmpty(networkCredentials.Password))
                {
                    setHeader = AuthorisationBaererUserName != networkCredentials.UserName;
                }
            }
            return setHeader;
        }

        private bool SetBearerAuthenticationHeader()
        {
            var setHeader = false;
            if (Credentials is NetworkCredential)
            {
                var networkCredentials = (NetworkCredential)Credentials;
                if (!string.IsNullOrEmpty(networkCredentials.UserName) && !string.IsNullOrEmpty(networkCredentials.Password))
                {
                    setHeader = AuthorisationBaererUserName == networkCredentials.UserName;
                }
            }
            return setHeader;
        }

        private bool SetTenantHeader()
        {
            return !string.IsNullOrEmpty(TenantID);
        }

        private void RegisterSendingRequestEvent()
        {

            if (SetBearerAuthenticationHeader() || SetBasicAuthenticationHeader() || SetTenantHeader())
            {
                SendingRequest2 += DataServiceContext_SendingRequest2;
            }
            else
            {
                SendingRequest2 -= DataServiceContext_SendingRequest2;
            }
        }

        public void DataServiceContext_SendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            if (SetBearerAuthenticationHeader())
            {
                var networkCredentials = (NetworkCredential)Credentials;
                e.RequestMessage.SetHeader(AUTHORIZATION_HEADER_NAME, string.Format(AUTHORIZATION_BEARER_SCHEME, networkCredentials.Password));
            }

            if (SetBasicAuthenticationHeader())
            {
                var networkCredentials = (NetworkCredential)Credentials;
                var basicAuthString = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", networkCredentials.UserName, networkCredentials.Password)));
                e.RequestMessage.SetHeader(AUTHORIZATION_HEADER_NAME, string.Format(AUTHORIZATION_BASIC_SCHEME, basicAuthString));
            }

            if (SetTenantHeader())
            {
                e.RequestMessage.SetHeader(TenantHeaderName, TenantID);
            }
        }

        #endregion
    }
}
