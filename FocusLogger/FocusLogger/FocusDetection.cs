using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FocusLogger
{
    class FocusDetection
    {
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);



        const uint EVENT_SYSTEM_FOREGROUND = 3;
        const uint WINEVENT_OUTOFCONTEXT = 0;


        static WinEventDelegate procDelegate = new WinEventDelegate(WinEventProc);

        public FocusDetection()
        {
            IntPtr hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
                    procDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);

            File.Create("log.txt");


            // Application.Run();

            UnhookWinEvent(hhook);
        }

        static void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Console.WriteLine("Foreground changed to {0:x8}", hwnd.ToInt32());
            //Console.WriteLine("ObjectID changed to {0:x8}", idObject);
            //Console.WriteLine("ChildID changed to {0:x8}", idChild);
            GetForegroundProcessName();

        }
        static void GetForegroundProcessName()
        {
            IntPtr hwnd = GetForegroundWindow();

            if (hwnd == null)
                return;

            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.Id == pid)
                {
                    Console.WriteLine("Pid is: {0}", pid);
                    Console.WriteLine("Process name is {0}", p.ProcessName);
                    File.AppendAllText("log.txt", p.ProcessName + Environment.NewLine);

                    return;
                }
            }

            Console.WriteLine("null");
        }

    }
}
