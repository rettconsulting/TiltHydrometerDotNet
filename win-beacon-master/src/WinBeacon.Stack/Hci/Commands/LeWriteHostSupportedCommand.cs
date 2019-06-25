﻿/*
 * Copyright 2015-2016 Huysentruit Wouter
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using WinBeacon.Stack.Hci.Opcodes;
using WinBeacon.Stack.Hci.Parameters;

namespace WinBeacon.Stack.Hci.Commands
{
    internal class LeWriteHostSupportedCommand : Command
    {
        public LeWriteHostSupportedCommand(bool supportedHost, bool simultaneousHost)
            : base(OpcodeGroup.ControllerBaseband, (int)ControllerBasebandOpcode.LeWriteHostSupported)
        {
            Parameters.Add(new BoolCommandParameter(supportedHost));
            Parameters.Add(new BoolCommandParameter(simultaneousHost));
        }
    }
}
