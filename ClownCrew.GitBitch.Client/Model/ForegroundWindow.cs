using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClownCrew.GitBitch.Client.Model
{
    internal class ForegroundWindow : IWin32Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        private static ForegroundWindow _obj;

        public static ForegroundWindow CurrentWindow
        {
            get { return _obj ?? (_obj = new ForegroundWindow()); }
        }

        public IntPtr Handle
        {
            get { return GetForegroundWindow(); }
        }
    }
}