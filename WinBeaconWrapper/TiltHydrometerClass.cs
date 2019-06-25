using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinBeacon;

namespace TiltHydrometerClass
{
    public class TiltHydrometerHubEventArgs : EventArgs
    {
        public BeaconEventArgs RawEvent { set; get; }
        public string Uuid { set; get; }
        public int Temp { set; get; }
        public double SG { set; get; }
        public int Power { set; get; }
    }

    public class TiltHydrometerHub : IDisposable
    {
        private BeaconHub _hub;

        public delegate void TiltHydrometerHubEventHandler(object sender, TiltHydrometerHubEventArgs args);
        public event TiltHydrometerHubEventHandler TiltHydrometerHubEvent;

        public TiltHydrometerHub(int VID, int PID)
        {
            _hub = new BeaconHub(VID, PID);
            _hub.BeaconDetected += (sender, e) => { BeaconDetected(e); };
        }

        private void BeaconDetected(BeaconEventArgs e)
        {
            var uuid = e.Beacon.Uuid;
            var temp = e.Beacon.Major;
            var sg = e.Beacon.Minor / 1000.0;
            var power = e.Beacon.Rssi;

            // make sure it is a tilt hydrometer device
            if (uuid.Contains("a495bb50"))
            {
                // fire event
                TiltHydrometerHubEventHandler mce = TiltHydrometerHubEvent;
                var ev = new TiltHydrometerHubEventArgs { RawEvent = e, Uuid = uuid, Power = power, SG = sg, Temp = temp };
                mce?.Invoke(this, ev);
            }
        }

        public void Shutdown()
        {
            this.Dispose();
        }

        ~TiltHydrometerHub()
        {
            // Finalizer calls Dispose(false)  
            Dispose(false);
        }

        // Dispose() calls Dispose(true)  
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)  
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources  
                if (_hub != null)
                {
                    _hub.Dispose();
                    _hub = null;
                }
            }
            // free native resources if there are any.  
            //if (_hub != IntPtr.Zero)
            //{
            //    Marshal.FreeHGlobal(nativeResource);
            //    nativeResource = IntPtr.Zero;
            //}
        }
    }
}
