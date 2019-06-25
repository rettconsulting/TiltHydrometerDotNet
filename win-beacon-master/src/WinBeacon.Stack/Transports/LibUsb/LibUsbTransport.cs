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
using System.Collections.Generic;
using System.Linq;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using WinBeacon.Stack.Hci;
using WinBeacon.Stack.Transport;

namespace WinBeacon.Stack.Transports.LibUsb
{
    internal class LibUsbTransport : ITransport
    {
        private static readonly TimeSpan writeTimeout = TimeSpan.FromSeconds(5);

        private ILibUsbDevice usbDevice;
        private Dictionary<UsbBluetoothEndpointType, UsbEndpointBase> endpoints = new Dictionary<UsbBluetoothEndpointType, UsbEndpointBase>();

        internal LibUsbTransport(ILibUsbDevice usbDevice)
        {
            if (usbDevice == null)
                throw new ArgumentNullException("usbDevice");
            this.usbDevice = usbDevice;
        }

        ~LibUsbTransport()
        {
            Dispose();
        }

        #region ITransport implementation

        public void Dispose()
        {
            Close();
        }

        public void Open()
        {
            usbDevice.Open();
            OpenEndpoints();
        }

        public void Close()
        {
            CloseEndpoints();
            usbDevice.Close();
        }

        public void Send(byte[] data, DataType dataType)
        {
            if (usbDevice == null)
                throw new WinBeaconException("Transport not open");
            if (data == null)
                throw new ArgumentNullException("data");
            switch (dataType)
            {
                case DataType.Command:
                    SendCommand(data);
                    break;
                case DataType.Acl:
                    SendAcl(data);
                    break;
                default:
                    throw new ArgumentException("Invalid dataType");
            }
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        private void OnDataReceived(byte[] data, DataType dataType)
        {
            if (DataReceived != null)
                DataReceived(this, new DataReceivedEventArgs(data, dataType));
        }

        #endregion

        public int Vid
        {
            get { return usbDevice.Vid; }
        }

        public int Pid
        {
            get { return usbDevice.Pid; }
        }

        private Dictionary<UsbBluetoothEndpointType, byte> GetBluetoothEndpointIds()
        {
            var endpointIds = new Dictionary<UsbBluetoothEndpointType, byte>
                {
                    { UsbBluetoothEndpointType.Commands, 0x00 },
                    { UsbBluetoothEndpointType.Events, 0x81 },
                    { UsbBluetoothEndpointType.AclDataOut, 0x02 },
                    { UsbBluetoothEndpointType.AclDataIn, 0x82 }
                };
            foreach (var info in usbDevice.EnumerateBluetoothEndpointInfo())
                endpointIds[info.Type] = info.Id;
            return endpointIds;
        }

        internal virtual void OpenEndpoints()
        {
            var endpointIds = GetBluetoothEndpointIds();
            endpoints[UsbBluetoothEndpointType.Commands] = usbDevice.OpenEndpointWriter((WriteEndpointID)endpointIds[UsbBluetoothEndpointType.Commands]);
            endpoints[UsbBluetoothEndpointType.Events] = usbDevice.OpenEndpointReader((ReadEndpointID)endpointIds[UsbBluetoothEndpointType.Events]);
            endpoints[UsbBluetoothEndpointType.AclDataOut] = usbDevice.OpenEndpointWriter((WriteEndpointID)endpointIds[UsbBluetoothEndpointType.AclDataOut]);
            endpoints[UsbBluetoothEndpointType.AclDataIn] = usbDevice.OpenEndpointReader((ReadEndpointID)endpointIds[UsbBluetoothEndpointType.AclDataIn]);
            var failedEndpointTypes = from x in endpoints where x.Value == null select x.Key;
            if (failedEndpointTypes.Count() > 0)
                throw new WinBeaconException("Failed to open endpoint(s): {0}",  string.Join(" ", failedEndpointTypes));
            endpoints[UsbBluetoothEndpointType.Events].SubscribeForDataReceived(data => OnDataReceived(data, DataType.Command));
            endpoints[UsbBluetoothEndpointType.AclDataIn].SubscribeForDataReceived(data => OnDataReceived(data, DataType.Acl));
        }

        internal virtual void CloseEndpoints()
        {
            var openEndpoints = from x in endpoints where x.Value != null select x.Value;
            foreach (var endpoint in openEndpoints)
            {
                if (endpoint is UsbEndpointReader)
                    (endpoint as UsbEndpointReader).DataReceivedEnabled = false;
                endpoint.Dispose();
            }
            endpoints.Clear();
        }

        internal virtual void SendCommand(byte[] data)
        {
            var setupPacket = new UsbSetupPacket((byte)UsbRequestType.TypeClass | (byte)UsbRequestRecipient.RecipInterface, 0, 0, 0, (short)data.Length);
            int lengthTransferred;
            if (!usbDevice.ControlTransfer(ref setupPacket, data, data.Length, out lengthTransferred))
                throw new WinBeaconException("USB ControlTransfer failed");
            if (lengthTransferred != data.Length)
                throw new WinBeaconException("USB ControlTransfer didn't send all bytes. Sent {0} out of {1} bytes.", lengthTransferred, data.Length);
        }

        internal virtual void SendAcl(byte[] data)
        {
            var endpoint = endpoints[UsbBluetoothEndpointType.AclDataOut] as UsbEndpointWriter;
            int transferLength;
            var errorCode = endpoint.Write(data, (int)writeTimeout.TotalMilliseconds, out transferLength);
            if (errorCode != ErrorCode.Ok)
                throw new WinBeaconException("USB write operation failed with error code: {0}", errorCode);
            if (transferLength != data.Length)
                throw new WinBeaconException("USB write operation didn't send all bytes. Sent {0} out of {1} bytes.", transferLength, data.Length);
        }
    }
}
