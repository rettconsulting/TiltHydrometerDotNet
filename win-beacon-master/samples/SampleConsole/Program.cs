using System;
using WinBeacon;

namespace SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var hub = new BeaconHub(0x0A5C, 0x21E8))
            {
                hub.BeaconDetected += (sender, e) => { Bleep(e); };
                Console.ReadKey();
            }
        }

        static void Bleep(BeaconEventArgs e)
        {
            var addr = e.Beacon.Uuid;
            var temp = e.Beacon.Major;
            var sg = e.Beacon.Minor / 1000.0;
            var power = e.Beacon.Rssi;

            // make sure it is a tilt hydrometer device
            if (addr.Contains("a495bb50"))
            {
                Console.WriteLine(addr + ": " + temp + "°C SG = " + sg + " Power = " + power + " dB");
            }
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            return BitConverter.ToString(ba); //.Replace("-", "");
        }
    }
}
