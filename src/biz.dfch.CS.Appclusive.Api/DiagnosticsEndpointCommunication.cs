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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Api.Diagnostics
{
    public class DiagnosticsEndpointCommunication
    {
        public Version GetBaseUriVersion(System.Data.Services.Client.DataServiceQuery<biz.dfch.CS.Appclusive.Core.OdataServices.Diagnostics.Endpoint> endpoint)
        {
            Contract.Requires(null != endpoint);
            Contract.Ensures(null != Contract.Result<Version>());

            var baseUri = endpoint.FirstOrDefault(e => e.Id == 1);
            Contract.Assert(null != baseUri, "Retrieving version information FAILED.");
            
            var result = new Version(baseUri.Version);
            return result;
        }
    }
}
