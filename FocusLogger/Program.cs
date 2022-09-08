using System;
using System.Runtime.InteropServices;

namespace FocusLogger
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


        static void Main(string[] args)
        {
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, 0);

            new FocusDetection();
        }
    }
}
