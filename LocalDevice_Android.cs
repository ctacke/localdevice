#if ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;

namespace System
{
    public static partial class LocalDevice
    {
        private class AndroidInterface : ILocalInterface
        {
            public void SetHostName(string name)
            {
                throw new NotSupportedException();
            }

            public void SetLocalTime(DateTime time)
            {
                throw new NotSupportedException();
            }

            public void SetSystemTime(DateTime time)
            {
                throw new NotSupportedException();
            }

            public string GetHostName()
            {
                throw new NotSupportedException();
            }

#region ILocalInterface Members


            public Net.NetworkInformation.PhysicalAddress GetMACAddress()
            {
                throw new NotImplementedException();
            }

            #endregion

            public bool Is64BitOperatingSystem()
            {
                throw new NotImplementedException();
            }

            public void Restart()
            {
                throw new NotImplementedException();
            }

            public ulong GetTotalMemory()
            {
                throw new NotImplementedException();
            }

            public ulong GetFreeMemory()
            {
                throw new NotImplementedException();
            }

            public decimal GetCpuLoad()
            {
                var proc = Java.Lang.Runtime.GetRuntime().Exec("top -n 1");
                string dataLine;
                using (var reader = new StreamReader(proc.OutputStream))
                {
                    do
                    {
                        dataLine = reader.ReadLine();
                    } while (dataLine.IsNullOrWhiteSpace());
                }
                var tokens = dataLine
                    .Replace("User", string.Empty)
                    .Replace("System", string.Empty)
                    .Replace("IRW", string.Empty)
                    .Replace("IRQ", string.Empty)
                    .Replace("%", string.Empty)
                    .Replace("  ", " ")
                    .Split(' ');

                // TODO: validate this - right now I'm just guessing, not testing
                return Convert.ToDecimal(tokens[0]);
            }
        }
    }
}
#endif
