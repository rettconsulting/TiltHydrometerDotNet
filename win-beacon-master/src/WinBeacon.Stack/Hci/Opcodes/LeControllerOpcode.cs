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

namespace WinBeacon.Stack.Hci.Opcodes
{
    internal enum LeControllerOpcode : ushort
    {
        SetEventMask = 0x0001,
        SetAdvertisingParameters = 0x0006, // http://stackoverflow.com/questions/21124993/is-there-a-way-to-increase-ble-advertisement-frequency-in-bluez
        SetAdvertisingData = 0x0008,
        SetAdvertisingEnable = 0x000A,
        SetScanParameters = 0x000B,
        SetScanEnable = 0x000C,
    }
}
