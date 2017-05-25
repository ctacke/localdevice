﻿#if WindowsCE
using OpenNETCF.Net.NetworkInformation;
#else
    using System.Net.NetworkInformation;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace System
{
    public static partial class LocalDevice
    {
        private static string _rootPath;
        private static string _executingAssemblyFullPath;
        private static bool? m_mono;

        static LocalDevice()
        {
#if WINDOWS_PHONE
            m_local = new WinPhoneInterface();
#elif WindowsCE
            m_local = new WinCEInterface();
#elif ANDROID
            m_local = new AndroidInterface();
#elif DESKTOP
            m_local = new WindowsDesktopInterface();
#elif MONO
            m_local = new MonoInterface();
#endif
        }

#if !WINDOWS_PHONE
        public static IPAddress CurrentIPAddress
        {
            get
            {
                return (from a in Dns.GetHostEntry(Dns.GetHostName()).AddressList
                        where a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                        select a).FirstOrDefault();
            }
        }
#endif
        public static bool IsRunningUnderMono
        {
            get
            {
                if (!m_mono.HasValue)
                {
                    m_mono = (Type.GetType("Mono.Runtime") != null);
                }

                return m_mono.Value;
            }
        }

        public static bool IsUNIX
        {
            get { return Environment.OSVersion.Platform == PlatformID.Unix; }
        }

        public static bool Is64BitOS
        {
            get { return m_local.Is64BitOperatingSystem(); }
        }

        public static DateTime LocalTime
        {
            get { return DateTime.Now; }
            set { m_local.SetLocalTime(value); }
        }

        public static DateTime SystemTime
        {
            get { return DateTime.Now.ToUniversalTime(); }
            set { m_local.SetSystemTime(value); }
        }

        public static string HostName
        {
            get { return m_local.GetHostName(); }
            set { m_local.SetHostName(value); }
        }

        public static PhysicalAddress MACAddress
        {
            get { return m_local.GetMACAddress(); }
        }

        public static string GetPlatformPath(string s)
        {
            var path = s.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

            // not sure this is valid for all cases, we'll need to do heavy testing
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(RootPath, path);
            }

            return path;

        }

        public static string ExecutingAssemblyFullPath
        {
            get
            {

                if (string.IsNullOrEmpty(_executingAssemblyFullPath))
                {
                    Assembly getPathFrom = null;

#if !WindowsCE
                    getPathFrom = Assembly.GetEntryAssembly();
#endif

                    if (getPathFrom == null)
                    {
                        getPathFrom = Assembly.GetExecutingAssembly();
                    }
                    var uri = new Uri(getPathFrom.GetName().CodeBase);
                    _executingAssemblyFullPath = GetPlatformPath(uri.LocalPath);
                }
                return _executingAssemblyFullPath;
            }

            set
            {
                _executingAssemblyFullPath = value;
            }
        }

        public static string ApplicationDataPath
        {
            get
            {
                // TODO: handle Phone, Android platforms
#if ANDROID
                return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
#else

                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.WinCE:
                        return RootPath;
                    case PlatformID.Unix:
                        return "/usr/SF";
                    default:
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        path = Path.Combine(path, "SolutionFamily");
                        path = Path.Combine(path, "Solution Engine");
                        return path;
                }
#endif
            }
        }

        public static string RootPath
        {
            get
            {

                if (string.IsNullOrEmpty(_rootPath))
                {
#if ANDROID
                    _rootPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
#else

                    Assembly getPathFrom = null;

#if !WindowsCE
                    getPathFrom = Assembly.GetEntryAssembly();
#endif

                    if (getPathFrom == null)
                    {
                        getPathFrom = Assembly.GetExecutingAssembly();
                    }
                    var uri = new Uri(getPathFrom.GetName().CodeBase);
                    _rootPath = Path.GetDirectoryName(uri.LocalPath);
#endif
                }
                return _rootPath;
            }
            private set
            {
                _rootPath = value;
            }
        }

        public static void Restart()
        {
            m_local.Restart();
        }

        public static decimal TotalMemoryMB
        {
            get { return m_local.GetTotalMemory() / (decimal)0x100000; }
        }

        public static decimal FreeMemoryMB
        {
            get { return m_local.GetFreeMemory() / (decimal)0x100000; }
        }

        public static decimal ProcessorLoad
        {
            get { return m_local.GetCpuLoad(); }
        }

        public static IPAddress GetPublicIPAddress()
        {
            try
            {
                var ip = new WebClient().DownloadString("http://checkip.amazonaws.com/").Trim();
                return IPAddress.Parse(ip);
            }
            catch { }
        
            try
            {
                var ip = new WebClient().DownloadString("http://icanhazip.com").Trim();
                return IPAddress.Parse(ip);
            }
            catch { }

            try
            {
                var ip = new WebClient().DownloadString("http://bot.whatismyipaddress.com").Trim();
                return IPAddress.Parse(ip);
            }
            catch { }

            try
            {
                var ip = new WebClient().DownloadString("http://ipinfo.io/ip").Trim();
                return IPAddress.Parse(ip);
            }
            catch { }

            throw new Exception("Unable to determine public IP Address.  Are you connected to the internet?");
        }
    }
}
