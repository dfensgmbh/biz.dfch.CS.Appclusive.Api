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
using System.Diagnostics.Contracts;

namespace biz.dfch.CS.Appclusive.Api.Diagnostics
{
    public partial class Diagnostics : DataServiceContextBase
    {
        public SemverCompatibilityFlags GetSemverCompatibility()
        {
            var assemblyVersion = GetVersion();

            var result = GetSemverCompatibility(assemblyVersion);
            return result;
        }

        public SemverCompatibilityFlags GetSemverCompatibility(Version value)
        {
            Contract.Requires(null != value);
            Contract.Ensures(null != Contract.Result<SemverCompatibilityFlags>());

            var serverVersion = new DiagnosticsEndpointCommunication().GetBaseUriVersion(_Endpoints);
            
            var result = SemverCompatibilityFlags.Compatible;

            if(value < serverVersion)
            {
                result |= SemverCompatibilityFlags.ServerIsNewer;
            }

            if(value.Major < serverVersion.Major)
            {
                result |= SemverCompatibilityFlags.BreakingChanges;
            }

            if(value.Major < serverVersion.Major || value.Minor < serverVersion.Minor)
            {
                result |= SemverCompatibilityFlags.AdditionalFeatures;
            }

            if(value.Major < serverVersion.Major || value.Minor < serverVersion.Minor || value.Build < serverVersion.Build)
            {
                result |= SemverCompatibilityFlags.Patched;
            }

            return result;
        }
    }
}
