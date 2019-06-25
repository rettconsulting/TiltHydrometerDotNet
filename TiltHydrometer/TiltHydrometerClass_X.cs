using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiltHydrometerClass;

namespace TiltHydrometer_X
{
    public partial class TiltHydrometerClass_X : Form
    {
        TiltHydrometerHub THhub;

        public TiltHydrometerClass_X()
        {
            InitializeComponent();
        }

        private void TiltHydrometerClass_X_Load(object sender, EventArgs e)
        {
            // start up beacon listener
            //THhub = new TiltHydrometerHub(0x0A5C, 0x21E8); // 
            THhub = new TiltHydrometerHub(0x2458, 0x0001);

            // register callback event
            THhub.TiltHydrometerHubEvent += (se, ev) => { TiltHydrometerAdvertisement(ev); };
        }

        private void TiltHydrometerAdvertisement(TiltHydrometerHubEventArgs e)
        {
            // update gui textbox with new status
            this.UIThread(() => tbLog.AppendText(e.Uuid + ": Temp = " + e.Temp + "°C, SG = " + e.SG + " Power = " + e.Power + " dB" + "\r\n"));
        }

        private void TiltHydrometerClass_X_FormClosing(object sender, FormClosingEventArgs e)
        {
            // shutdown beacon listener
            THhub.Shutdown();
        }
    }

    public static class ControlExtensions
    {
        // this is a method which you can use to invoke calls on gui objects from external threads
        public static void UIThread(this Control @this, Action code)
        {
            if (@this.InvokeRequired)
            {
                @this.BeginInvoke(code);
            }
            else
            {
                code.Invoke();
            }
        }
    }
}
