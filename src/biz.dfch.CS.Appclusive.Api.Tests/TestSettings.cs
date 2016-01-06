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
 
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Api.Tests
{
    public class TestSettings
    {
        public Dictionary<string, string> settings;

        public TestSettings()
        {
            this.settings = new Dictionary<string, string>();
        }

        public string GetValue(string key)
        {
            return settings[key];
        }

        public static TestSettings Load()
        {
            TestSettings settings = new TestSettings();
            string settingsText = File.ReadAllText(@"../../TestSettings.txt");
            foreach(string settingString in settingsText.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] settingsPart = settingString.Split(new string[]{"="}, StringSplitOptions.RemoveEmptyEntries);
                settings.settings.Add(settingsPart[0], settingsPart[1]);
            }
            return settings;
        }
    }
}
