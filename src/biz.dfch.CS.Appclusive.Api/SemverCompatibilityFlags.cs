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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Api
{
    public enum SemverCompatibilityFlags : int
    {
        Compatible = 0x0000
        ,
        ServerIsNewer = 0x8
        ,
        Patched = ServerIsNewer | 0x1
        ,
        AdditionalFeatures = ServerIsNewer | 0x2
        ,
        BreakingChanges = ServerIsNewer | 0x4
    }
}