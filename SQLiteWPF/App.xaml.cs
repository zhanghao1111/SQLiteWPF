using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SQLiteWPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Mutex mutex = new Mutex(true, "SQLiteWPF", out bool isNewInstance);
            if (isNewInstance != true)
            {
                // MessageBox.Show("程序已启动");
                IntPtr intPtr = FindWindowW(null, "SQLiteWPF");
                if (intPtr != IntPtr.Zero)
                {
                    SetForegroundWindow(intPtr);
                }
                Shutdown();
            }
        }
        [DllImport("User32", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowW(String lpClassName,String lpWindowName);
        [DllImport("User32", CharSet = CharSet.Unicode)]
        static extern Boolean SetForegroundWindow(IntPtr hWnd);
    }
}
