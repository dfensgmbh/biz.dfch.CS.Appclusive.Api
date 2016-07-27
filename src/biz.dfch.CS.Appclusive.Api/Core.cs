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
    public partial class Core : DataServiceContextBase
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
                    this.tenantHeaderName = Core.DEFAULT_TENANT_HEADER_NAME;
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
                    setHeader = Core.AuthorisationBaererUserName != networkCredentials.UserName;
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
                    setHeader = Core.AuthorisationBaererUserName == networkCredentials.UserName;
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
                this.SendingRequest2 += Core_SendingRequest2;
            }
            else
            {
                this.SendingRequest2 -= Core_SendingRequest2;
            }
        }

        void Core_SendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            if (this.SetBearerAuthenticationHeader())
            {
                NetworkCredential networkCredentials = (NetworkCredential)this.Credentials;
                e.RequestMessage.SetHeader(Core.AUTHORIZATION_HEADER_NAME, string.Format(Core.AUTHORIZATION_BEARER_SCHEME, networkCredentials.Password));
            }

            if (this.SetBasicAuthenticationHeader())
            {
                NetworkCredential networkCredentials = (NetworkCredential)this.Credentials;
                string basicAuthString = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", networkCredentials.UserName, networkCredentials.Password)));
                e.RequestMessage.SetHeader(Core.AUTHORIZATION_HEADER_NAME, string.Format(Core.AUTHORIZATION_BASIC_SCHEME, basicAuthString));
            }

            if (this.SetTenantHeader())
            {
                e.RequestMessage.SetHeader(this.TenantHeaderName, this.TenantID);
            }
        }

        #endregion Request Headers
    }
}
