﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsDesktop;

namespace zVirtualDesktop
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            if (args.Length > 1)
            {
                if (args[0] == "intptr")
                {
                    Window win = new Window((IntPtr)Convert.ToInt32(args[1]));
                    win.MoveToDesktop(Convert.ToInt32(args[2]));
                    win.GoToDesktop();
                }

                if (args[0] == "title")
                {
                    Window.FindWindowMatch(args[1]);
                    if ((int)Window.FoundWindowHandle != 0)
                    {
                        Window win = new Window(Window.FoundWindowHandle);
                        win.MoveToDesktop(Convert.ToInt32(args[2]));
                        win.GoToDesktop();
                    }
                }

                Console.WriteLine("Found Handle: {0}", Window.FoundWindowHandle);

                return;
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //CheckVersion();

                //Add Excluded windows
                ExcludedWindowCaptions.Add("ASUS_Check");
                ExcludedWindowCaptions.Add("NVIDIA GeForce Overlay");
                ExcludedWindowCaptions.Add("zVirtualDesktop Settings");

                //Run the main form
                Application.Run(MainForm = new frmMain());
            }

        }

        public static frmMain MainForm;
        public const string version = "1.0.20";

        public static IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly();
        public static List<string> WallpaperStyles = new List<string>();
        public static List<string> PinnedApps = new List<string>();
        public static List<Window> windows = new List<Window>();
        public static List<HotkeyItem> hotkeys = new List<HotkeyItem>();
        public static VirtualDesktop[] Desktops = VirtualDesktop.GetDesktops();
        public static List<string> ExcludedWindowCaptions = new List<string>();
        

        public static string IconTheme = "White Box";

        //stats to log
        public static uint PinCount = 0;
        public static uint MoveCount = 0;
        public static uint NavigateCount = 0;

        public static void AddWindowToList(Window win)
        {
            IEnumerable<Window> window = from Window w in Program.windows
                                         where w.Handle == win.Handle
                                         select w;
            if (window.Count() < 1)
            {
                windows.Add(win);
            }
        }

        public static bool IsExcludedWindow(string caption)
        {
            foreach(string s in ExcludedWindowCaptions)
            {
                if(caption == s)
                {
                    return true;
                }
            }

            return false;
        }

        public static void CheckVersion()
        {
            return;
            //try
            //{
            //    MainForm.timerCheckVersion.Enabled = false;
            //}
            //catch { }
            
            //string latestversion = GetCurrentVersion();
            //if (latestversion != "" && latestversion != version)
            //{
            //    DialogResult result = MessageBox.Show("zVirtualDesktop " + latestversion + " is available.\r\n" +
            //        "You are currently running version "+ version + "\r\n" + 
            //        "Would you like to download it now?", "zVirtualDesktop", MessageBoxButtons.YesNo);
            //    if (result == DialogResult.Yes)
            //    {
            //        System.Diagnostics.Process.Start("https://github.com/mzomparelli/zVirtualDesktop/blob/master/zVirtualDesktop/bin/Release/zVirtualDesktop.zip?raw=true");
            //        Environment.Exit(0);
            //    }
            //}
            //try
            //{
            //    MainForm.timerCheckVersion.Enabled = true;
            //}
            //catch { }
        }

        public static string GetCurrentVersion()
        {
            try
            {
                WSHttpBinding b = new WSHttpBinding(SecurityMode.Transport);
                b.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                b.Name = "WSHttpBinding_IService";
                EndpointAddress address = new EndpointAddress("https://zomp.co/z/ZompWebService.svc");

                using (Zomp.ServiceClient z = new Zomp.ServiceClient(b, address))
                {
                    return z.zVD_CurrentVersion();
                }


            }
            catch (Exception ex)
            {
                return "";
            }


        }

    }
}
