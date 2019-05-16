using Newtonsoft.Json; //https://www.newtonsoft.com/json
using PInvoke; //https://github.com/AArnott/pinvoke
using static PInvoke.Kernel32; 
using static PInvoke.User32;
using System.Collections.Generic;
using System.Collections; 
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System;
using CommandLine; //https://github.com/commandlineparser/commandline
using System.Linq;


namespace MyLayoutManager
{

    class Program
    {
        private static Dictionary<IntPtr, App> openWindowHandleMap = new Dictionary<IntPtr, App>(); 
        

        static bool WindowCallBack(IntPtr handle, IntPtr lparameter)
        {
            bool isVisible = User32.IsWindowVisible(handle); 
            
            if(isVisible)
            {
                App openWindow = new App();                           
                
                openWindow.ShortWindowTitle = GetWindowTitle(handle); 
                openWindow.WindowPlacement = User32.GetWindowPlacement(handle); 

                openWindowHandleMap.Add(handle, openWindow);       
            }   

            return true; //needed, enum stops iterating through windows otherwise
        }

        private static string GetWindowTitle(IntPtr handle)
        {
            int lengthOfTitle = User32.GetWindowTextLength(handle); 
            char[] charArry = new char[lengthOfTitle];                 
            User32.GetWindowText(handle, charArry, lengthOfTitle);
            string windowText = new string(charArry); 
            windowText = windowText.Trim('\0'); 
            return windowText;
        }

        private static void AddProcessInformationToMap()
        {
            foreach (Process proc in Process.GetProcesses())
            {                   
                if(openWindowHandleMap.ContainsKey(proc.MainWindowHandle))
                {
                    openWindowHandleMap[proc.MainWindowHandle].ProcessName = proc.ProcessName; 
                    openWindowHandleMap[proc.MainWindowHandle].FileName = proc.MainModule.FileName; 
                    openWindowHandleMap[proc.MainWindowHandle].WindowTitle = proc.MainWindowTitle;                         
                }
            } 
        }

        static void SaveWindowLayout(string layoutName, bool interactive)
        {
            User32.WNDENUMPROC callback = new WNDENUMPROC(WindowCallBack); 
            User32.EnumWindows(callback,new IntPtr(0)); 

            AddProcessInformationToMap(); 
            
            List<App> forConfigFile = new List<App>(openWindowHandleMap.Values.Where(app => !String.IsNullOrEmpty(app.FileName)));        
            
            List<App> saveTheseWindows = new List<App>(); 
            if(interactive)
            {
                foreach(App app in forConfigFile)
                {
                    string prompt = "Do you want to save the window with \nfilename = {0} and \nwindow title = {1}? Y for yes or anything else for no.";
                    Console.WriteLine(String.Format(prompt, app.FileName, app.ShortWindowTitle)); 
                    string input = Console.ReadLine(); 
                    
                    if(input.ToLower().StartsWith("y"))
                    {
                        saveTheseWindows.Add(app); 
                    }
                }
            }
            else
                saveTheseWindows = forConfigFile; 

            File.WriteAllText(layoutName, JsonConvert.SerializeObject(saveTheseWindows));
        }


        static void LoadWinowLayout(string layoutName)
        {
            List<App> loadTheseApps = JsonConvert.DeserializeObject<List<App>>(File.ReadAllText(layoutName));

            foreach(App app in loadTheseApps)
            {
                Process appProcess = new Process(); 
                appProcess.StartInfo.FileName = app.FileName; 
                appProcess.StartInfo.Arguments = app.LaunchArgs; 
                appProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal; 
                appProcess.Start(); 
                if(!appProcess.HasExited) appProcess.WaitForInputIdle(); 
                System.Threading.Thread.Sleep(100);                 
                
                //can't get the mainwindowhandle from p, for whatever reason
                Process thisAppProcess = Process.GetProcessesByName(app.ProcessName).First(); 
                User32.SetWindowPlacement(thisAppProcess.MainWindowHandle, app.WindowPlacement);              
            }          
        }

        static void Main(string[] args)
        {
            /*                  
            note: dotnet publish --self-contained                              
            -add support for multiple monitors (https://docs.microsoft.com/en-us/windows/desktop/gdi/positioning-objects-on-multiple-display-monitors)
            */
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                if(o.Interactive) //only applies to savelayout
                    SaveWindowLayout(o.LayoutName, o.Interactive);
                else
                {
                    if(o.SaveLayout)     
                        SaveWindowLayout(o.LayoutName, interactive: false); 
                    if(o.LoadLayout)
                        LoadWinowLayout(o.LayoutName); 
                }
            });
        }
    }
}
