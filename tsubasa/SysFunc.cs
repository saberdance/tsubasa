using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace tsubasa
{
    public class PerformanceInfo
    {
        public float Cpu = 0;
        public float Ram = 0;
    }
    /// <summary>
    /// 常用系统函数
    /// </summary>
    public static class SysFunc
    {
        public static string GetMacAddr()
        { 
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in interfaces)
                {
                    return BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes()).Replace("-",":");
                }
            }
            catch (Exception)
            {
                return "";
            }
            return "";
        }
        public static string GetUUID()
        {
            try
            {
                return UtilFunc.MD5String(GetMacAddr().ToLower());
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static void RestartComputer()
        {
#if _WINDOWS
            WindowsControl.DoExitWindows(WindowsControl.ExitWindows.Force | WindowsControl.ExitWindows.Reboot);
#endif
        }
         public static void ShutdownComputer()
        {
#if _WINDOWS
            WindowsControl.DoExitWindows(WindowsControl.ExitWindows.Force | WindowsControl.ExitWindows.ShutDown);
#endif
        }
        public static IPEndPoint GetPort(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    return endPoint;
                }
            }

            return null;
        }
        public static bool PortInBind(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipProperties.GetActiveTcpConnections();
            foreach (TcpConnectionInformation tcpInfo in tcpConnInfoArray)
            {
                if (tcpInfo.LocalEndPoint.Port == port&&tcpInfo.State==TcpState.Established)
                {
                    Logger.Log($"Find Port: {port.ToString()} Bind");   
                    return true;
                }
            }
            return false;
        }
        public static long GetDiskFreeSpace(string diskName="c")
        {
            System.IO.DriveInfo disk = new System.IO.DriveInfo(diskName);
            return disk.AvailableFreeSpace;
        }

        public static string GetIpAddr(bool isRequire192 = true)
        {

            var IpAddrs = NetworkInterface.GetAllNetworkInterfaces()
            .Select(p => p.GetIPProperties())
            .SelectMany(p => p.UnicastAddresses)
            .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address));

            return IpAddrs.Where(o => o.Address.ToString().Contains("192")).FirstOrDefault().Address.ToString();
            
        }
    }
#if _WINDOWS
    /// <summary>
    /// 显示器控制
    /// </summary>
    public static class MonitorControl
    {
        public enum MonitorPowerType
        {
            // 打开显示器
            POWER_ON = -1,
            // 进入省电状态
            POWER_SAVE = 1,
            // 关闭显示器
            POWER_OFF = 2
        }
        private const int WM_SYSCOMMAND = 0x112;
        private const int SC_SCREENSAVE = 0xF140;
        private const int SC_MONITORPOWER = 0xF170;
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
  
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        /// <summary>
        /// 显示器操作，打开、关闭、省电状态
        /// </summary>
        /// <param name="power">指示对显示器用何种操作</param>
        public static void MonitorPower(MonitorPowerType power)
        {
            SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, (int)power);
        }
        /// <summary>
        /// 开启屏幕保护程序
        /// </summary>
        public static void ScreenSave()
        {
            SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_SCREENSAVE, 0);
        }
    }

    /// <summary>
    /// Windows运行控制
    /// </summary>
    public static class WindowsControl
    {
        //关闭计算机

        private const int SE_PRIVILEGE_ENABLED = 0x00000002;
        private const int TOKEN_QUERY = 0x00000008;
        private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        [Flags]
        public enum ExitWindows : uint
        {
            LogOff = 0x00, //注销
            ShutDown = 0x01, //关机
            Reboot = 0x02, //重启
            Force = 0x04,
            PowerOff = 0x08,
            ForceIfHung = 0x10
        }

        [Flags]
        private enum ShutdownReason : uint
        {
            MajorApplication = 0x00040000,
            MajorHardware = 0x00010000,
            MajorLegacyApi = 0x00070000,
            MajorOperatingSystem = 0x00020000,
            MajorOther = 0x00000000,
            MajorPower = 0x00060000,
            MajorSoftware = 0x00030000,
            MajorSystem = 0x00050000,
            MinorBlueScreen = 0x0000000F,
            MinorCordUnplugged = 0x0000000b,
            MinorDisk = 0x00000007,
            MinorEnvironment = 0x0000000c,
            MinorHardwareDriver = 0x0000000d,
            MinorHotfix = 0x00000011,
            MinorHung = 0x00000005,
            MinorInstallation = 0x00000002,
            MinorMaintenance = 0x00000001,
            MinorMMC = 0x00000019,
            MinorNetworkConnectivity = 0x00000014,
            MinorNetworkCard = 0x00000009,
            MinorOther = 0x00000000,
            MinorOtherDriver = 0x0000000e,
            MinorPowerSupply = 0x0000000a,
            MinorProcessor = 0x00000008,
            MinorReconfig = 0x00000004,
            MinorSecurity = 0x00000013,
            MinorSecurityFix = 0x00000012,
            MinorSecurityFixUninstall = 0x00000018,
            MinorServicePack = 0x00000010,
            MinorServicePackUninstall = 0x00000016,
            MinorTermSrv = 0x00000020,
            MinorUnstable = 0x00000006,
            MinorUpgrade = 0x00000003,
            MinorWMI = 0x00000015,
            FlagUserDefined = 0x40000000,
            FlagPlanned = 0x80000000
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("user32.dll")]
        private static extern bool ExitWindowsEx(ExitWindows uFlags, ShutdownReason dwReason);

        /// <summary>
        /// 关机、重启、注销windows
        /// </summary>
        /// <param name="flag"></param>
        public static void DoExitWindows(ExitWindows flag)
        {
            TokPriv1Luid tp;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;
            LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
            AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            ExitWindowsEx(flag, ShutdownReason.MajorOther);
        }
    }
#endif
}

