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

using System;
using NUnit.Framework;
using WinBeacon.Stack.Hci;
using WinBeacon.Stack.Hci.Commands;
using WinBeacon.Stack.Hci.Events;

namespace WinBeacon.Tests
{
    [TestFixture]
    public class CommandTests
    {
        [Test]
        public void Command_SimpleToByteArray()
        {
            var command = new Command(OpcodeGroup.LeController, 0x1234);
            Assert.AreEqual(command.ToByteArray(), new byte[] { 0x34, 0x32, 0x00 });
        }

        [Test]
        public void Command_WithParamToByteArray()
        {
            var parameter = new TestParameter { Data = new byte[] { 0x98, 0x76, 0x54, 0x32, 0x10 } };
            var command = new Command(OpcodeGroup.InformationalParameters, 0x12345);
            command.Parameters.Add(parameter);
            Assert.AreEqual(command.ToByteArray(), new byte[] { 0x45, 0x33, 0x05, 0x98, 0x76, 0x54, 0x32, 0x10 });
        }

        [Test]
        public void Command_OnCommandComplete()
        {
            Command callbackCommand = null;
            var command = new Command(OpcodeGroup.LeController, 0x1234);
            command.CommandCompleteCallback = (cmd, evt) => callbackCommand = cmd;
            Assert.IsNull(callbackCommand, "Callback called too early");
            command.OnCommandComplete(new CommandCompleteEvent());
            Assert.AreEqual(command, callbackCommand, "Command from callback differs");
        }

        [Test]
        public void Command_GenericOnCommandComplete()
        {
            Command callbackCommand = null;
            var command = new GenericTestCommand();
            command.CommandCompleteCallback = (cmd, result) =>
                {
                    callbackCommand = cmd;
                    Assert.AreEqual(result, 0x4687, "Result incorrect");
                };
            command.OnCommandComplete(new CommandCompleteEvent());
            Assert.AreEqual(command, callbackCommand, "Command from callback differs");
        }

        [Test]
        public void Command_LeSetAdvertisingDataCommand()
        {
            var command = new LeSetAdvertisingDataCommand(new byte[] { 0x12, 0x57, 0x78, 0x32 });
            Assert.AreEqual(new byte[] { 0x08, 0x20, 0x05, 0x04, 0x12, 0x57, 0x78, 0x32 }, command.ToByteArray());
        }

        [Test]
        public void Command_LeSetAdvertisingEnableCommand()
        {
            var command = new LeSetAdvertisingEnableCommand(false);
            Assert.AreEqual(new byte[] { 0x0A, 0x20, 0x01, 0x00 }, command.ToByteArray());
            command = new LeSetAdvertisingEnableCommand(true);
            Assert.AreEqual(new byte[] { 0x0A, 0x20, 0x01, 0x01 }, command.ToByteArray());
        }

        [Test]
        public void Command_LeSetAdvertisingParametersCommand()
        {
            Assert.That(() => new LeSetAdvertisingParametersCommand(20, 20, AdvertisingType.ConnectableUndirected, OwnAddressType.Public, PeerAddressType.Public, new byte[6], AdvertisingChannelMap.UseAllChannels, AdvertisingFilterPolicy.ConnectAllScanAll), Throws.Nothing);
            Assert.That(() => new LeSetAdvertisingParametersCommand(19, 20, AdvertisingType.ConnectableUndirected, OwnAddressType.Public, PeerAddressType.Public, new byte[6], AdvertisingChannelMap.UseAllChannels, AdvertisingFilterPolicy.ConnectAllScanAll), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LeSetAdvertisingParametersCommand(20, 19, AdvertisingType.ConnectableUndirected, OwnAddressType.Public, PeerAddressType.Public, new byte[6], AdvertisingChannelMap.UseAllChannels, AdvertisingFilterPolicy.ConnectAllScanAll), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LeSetAdvertisingParametersCommand(10240, 10240, AdvertisingType.ConnectableUndirected, OwnAddressType.Public, PeerAddressType.Public, new byte[6], AdvertisingChannelMap.UseAllChannels, AdvertisingFilterPolicy.ConnectAllScanAll), Throws.Nothing);
            Assert.That(() => new LeSetAdvertisingParametersCommand(10241, 10240, AdvertisingType.ConnectableUndirected, OwnAddressType.Public, PeerAddressType.Public, new byte[6], AdvertisingChannelMap.UseAllChannels, AdvertisingFilterPolicy.ConnectAllScanAll), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new LeSetAdvertisingParametersCommand(10240, 10241, AdvertisingType.ConnectableUndirected, OwnAddressType.Public, PeerAddressType.Public, new byte[6], AdvertisingChannelMap.UseAllChannels, AdvertisingFilterPolicy.ConnectAllScanAll), Throws.InstanceOf<ArgumentOutOfRangeException>());
            var command = new LeSetAdvertisingParametersCommand(200, 400, AdvertisingType.ConnectableUndirected, OwnAddressType.Public, PeerAddressType.Public, new byte[6], AdvertisingChannelMap.UseAllChannels, AdvertisingFilterPolicy.ConnectAllScanAll);
            Assert.That(command.ToByteArray(), Is.EqualTo(new byte[] { 0x06, 0x20, 0x0F, 0x40, 0x01, 0x80, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x00 })); 
        }

        [Test]
        public void Command_LeSetScanEnableCommand()
        {
            var command = new LeSetScanEnableCommand(false, false);
            Assert.AreEqual(new byte[] { 0x0C, 0x20, 0x02, 0x00, 0x00 }, command.ToByteArray());
            command = new LeSetScanEnableCommand(false, true);
            Assert.AreEqual(new byte[] { 0x0C, 0x20, 0x02, 0x00, 0x01 }, command.ToByteArray());
            command = new LeSetScanEnableCommand(true, false);
            Assert.AreEqual(new byte[] { 0x0C, 0x20, 0x02, 0x01, 0x00 }, command.ToByteArray());
            command = new LeSetScanEnableCommand(true, true);
            Assert.AreEqual(new byte[] { 0x0C, 0x20, 0x02, 0x01, 0x01 }, command.ToByteArray());
        }

        [Test]
        public void Command_LeSetScanParametersCommand()
        {
            var command = new LeSetScanParametersCommand(false, 0x1234, 0x2345, false, false);
            Assert.AreEqual(new byte[] { 0x0B, 0x20, 0x07, 0x00, 0x20, 0x1D, 0x6F, 0x38, 0x00, 0x00 }, command.ToByteArray());
            command = new LeSetScanParametersCommand(false, 0x2345, 0x1234, false, true);
            Assert.AreEqual(new byte[] { 0x0B, 0x20, 0x07, 0x00, 0x6F, 0x38, 0x20, 0x1D, 0x00, 0x01 }, command.ToByteArray());
            command = new LeSetScanParametersCommand(false, 0x1234, 0x2345, true, false);
            Assert.AreEqual(new byte[] { 0x0B, 0x20, 0x07, 0x00, 0x20, 0x1D, 0x6F, 0x38, 0x01, 0x00 }, command.ToByteArray());
            command = new LeSetScanParametersCommand(false, 0x2345, 0x1234, true, true);
            Assert.AreEqual(new byte[] { 0x0B, 0x20, 0x07, 0x00, 0x6F, 0x38, 0x20, 0x1D, 0x01, 0x01 }, command.ToByteArray());
            command = new LeSetScanParametersCommand(true, 0x1234, 0x2345, false, false);
            Assert.AreEqual(new byte[] { 0x0B, 0x20, 0x07, 0x01, 0x20, 0x1D, 0x6F, 0x38, 0x00, 0x00 }, command.ToByteArray());
            command = new LeSetScanParametersCommand(true, 0x2345, 0x1234, false, true);
            Assert.AreEqual(new byte[] { 0x0B, 0x20, 0x07, 0x01, 0x6F, 0x38, 0x20, 0x1D, 0x00, 0x01 }, command.ToByteArray());
            command = new LeSetScanParametersCommand(true, 0x1234, 0x2345, true, false);
            Assert.AreEqual(new byte[] { 0x0B, 0x20, 0x07, 0x01, 0x20, 0x1D, 0x6F, 0x38, 0x01, 0x00 }, command.ToByteArray());
            command = new LeSetScanParametersCommand(true, 0x2345, 0x1234, true, true);
            Assert.AreEqual(new byte[] { 0x0B, 0x20, 0x07, 0x01, 0x6F, 0x38, 0x20, 0x1D, 0x01, 0x01 }, command.ToByteArray());
        }

        [Test]
        public void Command_LeWriteHostSupportedCommand()
        {
            var command = new LeWriteHostSupportedCommand(false, false);
            Assert.AreEqual(new byte[] { 0x6D, 0x0C, 0x02, 0x00, 0x00 }, command.ToByteArray());
            command = new LeWriteHostSupportedCommand(false, true);
            Assert.AreEqual(new byte[] { 0x6D, 0x0C, 0x02, 0x00, 0x01 }, command.ToByteArray());
            command = new LeWriteHostSupportedCommand(true, false);
            Assert.AreEqual(new byte[] { 0x6D, 0x0C, 0x02, 0x01, 0x00 }, command.ToByteArray());
            command = new LeWriteHostSupportedCommand(true, true);
            Assert.AreEqual(new byte[] { 0x6D, 0x0C, 0x02, 0x01, 0x01 }, command.ToByteArray());
        }

        [Test]
        public void Command_ResetCommand()
        {
            var command = new ResetCommand();
            Assert.AreEqual(new byte[] { 0x03, 0x0C, 0x00 }, command.ToByteArray());
        }

        [Test]
        public void Command_ReadBdAddrCommand()
        {
            var command = new ReadBdAddrCommand();
            Assert.AreEqual(new byte[] { 0x09, 0x10, 0x00 }, command.ToByteArray());
        }

        [Test]
        public void Command_ReadLocalVersionCommand()
        {
            var command = new ReadLocalVersionCommand();
            Assert.AreEqual(new byte[] { 0x01, 0x10, 0x00 }, command.ToByteArray());
        }

        [Test]
        public void Command_LeReadHostSupportedCommand()
        {
            var command = new LeReadHostSupportedCommand();
            Assert.AreEqual(new byte[] { 0x6C, 0x0C, 0x00 }, command.ToByteArray());
        }

        [Test]
        public void Command_LeSetEventMaskCommand()
        {
            var command = new LeSetEventMaskCommand(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 });
            Assert.AreEqual(new byte[] { 0x01, 0x20, 0x08, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 }, command.ToByteArray());
        }

        [Test]
        public void Command_SetEventMaskCommand()
        {
            var command = new SetEventMaskCommand(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 });
            Assert.AreEqual(new byte[] { 0x01, 0x0C, 0x08, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 }, command.ToByteArray());
        }

        #region Helpers

        private class TestParameter : ICommandParameter
        {
            public byte[] Data { get; set; }
            public byte[] ToByteArray() { return Data; }
        }

        private class GenericTestCommand : Command<ushort>
        {
            public GenericTestCommand()
                : base(OpcodeGroup.LinkControl, 0x7854)
            {
            }
            internal override ushort ParseCommandResult(CommandCompleteEvent e)
            {
                return 0x4687;
            }
        }

        #endregion
    }
}
