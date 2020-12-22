using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace tsubasa
{
#if _WINDOWS
    public class WindowEnum
    {
        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        public class WindowInfo
        {
            public IntPtr hWnd = IntPtr.Zero;
            //public int ProcID;
            //public string ProcName;
            public string szWindowName;
            public string szClassName;
            public RECT Rect;
            public bool isShow;
        }
        public enum ShowState 
        {
            Hide=0,
            Show=1,
            Maximum=2,
            Minimum=3,
        }
        //用来遍历所有窗口 
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);

        //获取窗口Text 
        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        //获取窗口类名 
        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        //获取窗口进程ID
        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int pid);

        //控制窗口显示隐藏
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //判断窗口是否隐藏
        [DllImport("user32.dll", EntryPoint = "IsWindowVisible")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        //获取窗口大小
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRECT); 


        public WindowInfo[] GetAllDesktopWindows()
        {
            List<WindowInfo> wndList = new List<WindowInfo>();

            //enum all desktop windows 
            EnumWindows(delegate(IntPtr hWnd, int lParam)
            {
                WindowInfo wnd = new WindowInfo();
                StringBuilder sb = new StringBuilder(256);
                //get hwnd 
                wnd.hWnd = hWnd;
                //get window name  
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();

                //get window class 
                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();

                //取窗口大小
                RECT rect = new RECT();
                GetWindowRect(hWnd, ref rect);
                wnd.Rect = rect;

                //取窗口是否可见
                wnd.isShow = IsWindowVisible(hWnd);
                //add it into list 
                wndList.Add(wnd);
                return true;
            }, 0);

            return wndList.ToArray();
        }
        public bool ShowWnd(IntPtr hWnd, ShowState showState)
        {
            return ShowWindow(hWnd,Convert.ToInt32(showState));
        }
        public bool ShowWnd(string wndName, ShowState showState)
        {
            WindowInfo[] wnds = GetAllDesktopWindows();
            bool Succ = false;
            foreach (WindowEnum.WindowInfo wndinfo in wnds)
            {
                if (wndinfo.szWindowName == wndName)
                {
                    Succ=ShowWindow(wndinfo.hWnd, Convert.ToInt32(showState));
                }
            }
            return Succ;
        }

        public bool ShowWndByKeyword(string szToFind, ShowState showState)
        {
            WindowInfo[] wnds = GetAllDesktopWindows();
            bool Succ = false;
            foreach (WindowEnum.WindowInfo wndinfo in wnds)
            {
                if(szToFind.Contains(wndinfo.szWindowName) == true)
                {
                    Succ = ShowWindow(wndinfo.hWnd, Convert.ToInt32(showState));
                }
            }
            return Succ;
        }
        public bool ShowWndByKeyword(string[] wndNames, ShowState showState)
        {
            WindowInfo[] wnds = GetAllDesktopWindows();
            bool Succ = false;
            foreach (WindowEnum.WindowInfo wndinfo in wnds)
            {
                if (UtilFunc.FindPattenInArray(wndNames, wndinfo.szWindowName).Length > 0)
                {
                    Succ = ShowWindow(wndinfo.hWnd, Convert.ToInt32(showState));
                } 
            }
            return Succ;
        }
    }
#else
     /// <summary>
    /// 窗体枚举无法在非Windows操作系统下使用
    /// </summary>
     public class WindowEnum
    {
    }
#endif
}
