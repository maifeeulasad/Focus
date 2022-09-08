using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;

namespace FocusLogger
{
    class FocusDetection
    {
        delegate void
            WinEventDelegate
            (
                IntPtr hWinEventHook,
                uint eventType,
                IntPtr hwnd,
                int idObject,
                int idChild,
                uint dwEventThread,
                uint dwmsEventTime
            )
            ;

        [DllImport("user32.dll")]
        static extern IntPtr
        SetWinEventHook(
            uint eventMin,
            uint eventMax,
            IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            uint idProcess,
            uint idThread,
            uint dwFlags
        );

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern Int32
        GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        const uint EVENT_SYSTEM_FOREGROUND = 3;

        const uint WINEVENT_OUTOFCONTEXT = 0;

        IntPtr hhook;

        static WinEventDelegate 
            procDelegate = new WinEventDelegate(WinEventProc);

        #if DEBUG
            static string pathProgramFiles = "D:/";
        #else
            static string pathProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        #endif
        static string pathDirectory = Path.Combine( pathProgramFiles, "focus");
        static string todaysFilePath;

        static XmlDocument documentLogging = new XmlDocument();


        void CreateLoggingDirectory()
        {
            if (!Directory.Exists(pathDirectory))
            {
                Directory.CreateDirectory(pathDirectory);
            }

        }

        string CreateLoggingFile()
        {
            string dateToday = DateTime.Now.ToString("yyyy-dd-MM");
            string todaysFile = Path.Combine(pathDirectory , dateToday + ".xml");
            if (!File.Exists(todaysFile))
            {
                using (StreamWriter sw = File.CreateText(todaysFile))
                {
                    sw.WriteLine("<?xml version=\"1.0\" ?>");
                    sw.WriteLine("<log>");
                    sw.WriteLine("");
                    sw.WriteLine("</log>");
                }
            }

            return todaysFile;
        }


        public FocusDetection()
        {
            CreateLoggingDirectory();
            todaysFilePath = CreateLoggingFile();

            hhook =
                SetWinEventHook(EVENT_SYSTEM_FOREGROUND,
                EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero,
                procDelegate,
                0,
                0,
                WINEVENT_OUTOFCONTEXT);

            Application.Run();
        }

        ~FocusDetection()
        {
            UnhookWinEvent (hhook);
        }

        static void WinEventProc(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime
        )
        {
            GetForegroundProcessName();
        }

        static void GetForegroundProcessName()
        {
            IntPtr hwnd = GetForegroundWindow();

            if (hwnd == null) return;

            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            documentLogging.Load(todaysFilePath);

            foreach (System.Diagnostics.Process
                p
                in
                System.Diagnostics.Process.GetProcesses()
            )
            {
                if (p.Id == pid)
                {

                    XmlElement record = documentLogging.CreateElement("record");

                    XmlElement processNameElement = documentLogging.CreateElement("process");
                    processNameElement.InnerText = p.ProcessName;
                    XmlElement pidElement = documentLogging.CreateElement("pid");
                    pidElement.InnerText = pid.ToString();
                    XmlElement timestampElement = documentLogging.CreateElement("timestamp");
                    timestampElement.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");

                    record.AppendChild(processNameElement);
                    record.AppendChild(pidElement);
                    record.AppendChild(timestampElement);

                    documentLogging.DocumentElement.AppendChild(record);
                    documentLogging.Save(todaysFilePath);

                    return;
                }
            }


            XmlElement emptyRecord = documentLogging.CreateElement("empty");

            emptyRecord.InnerText = "Process doesn't exist";

            documentLogging.DocumentElement.AppendChild(emptyRecord);
            documentLogging.Save(todaysFilePath);
        }
    }
}
