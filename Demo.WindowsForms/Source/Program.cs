using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Demo.WindowsForms.Properties;
using GMap.NET;
using GMap.NET.MapProviders;

namespace Demo.WindowsForms
{
    static class Program
    {
        public static Properties.Settings settings = new Properties.Settings();


        // map
        public static GMapProvider mapProvider = GMapProviders.BingHybridMap;           // initial map
        public static double initialLatitude = 47.4979;                                 // initial center position
        public static double initialLongitude = 19.0402;
        public static double initialZoom = 10;                                          // initial zoom of map
        public static string settlement = "Hungary, Budapest";                          // initial center settlement
        public static string pingaddress = "google.com";                                // ip address to check conenction
        public static AccessMode accesmode = AccessMode.ServerOnly;                     // from where load map

        static void LoadSettings()
        {
            settings.Reload();

            initialLatitude  = settings.Latitude;
            initialLongitude = settings.Longitude;
            initialZoom      = settings.Zoom;
            settlement       = settings.Settlement;
            pingaddress      = settings.Ping;

            if (GMapProviders.List.Count == 0)
            {
                if (settings.MapProvider == GMapProviders.BingMap.ToString())
                    Program.mapProvider = GMapProviders.BingMap;
                else if (settings.MapProvider == GMapProviders.BingHybridMap.ToString())
                    Program.mapProvider = GMapProviders.BingHybridMap;
                else if (settings.MapProvider == GMapProviders.BingSatelliteMap.ToString())
                    Program.mapProvider = GMapProviders.BingSatelliteMap;
                else if (settings.MapProvider == GMapProviders.GoogleMap.ToString())
                    Program.mapProvider = GMapProviders.GoogleMap;
                else if (settings.MapProvider == GMapProviders.GoogleHybridMap.ToString())
                    Program.mapProvider = GMapProviders.GoogleHybridMap;
                else if (settings.MapProvider == GMapProviders.GoogleSatelliteMap.ToString())
                    Program.mapProvider = GMapProviders.GoogleSatelliteMap;
                else if (settings.MapProvider == GMapProviders.GoogleTerrainMap.ToString())
                    Program.mapProvider = GMapProviders.GoogleTerrainMap;
                else if (settings.MapProvider == GMapProviders.OpenStreetMap.ToString())
                    Program.mapProvider = GMapProviders.OpenStreetMap;
                else if (settings.MapProvider == GMapProviders.HFsystemMap.ToString())
                    Program.mapProvider = GMapProviders.HFsystemMap;
            }
            else 
            { 
                foreach(var m in GMapProviders.List)
                {
                    if (m.Name == settings.MapProvider)
                    {
                        Program.mapProvider = m;
                        break;
                    }    
                }
            }


            foreach (AccessMode a in Enum.GetValues(typeof(AccessMode)))
            {
                if (a.ToString() == settings.AccessMode)
                {
                    accesmode = a;
                    break;
                }
            }

        } /* LoadSettings */


        static void SaveSettings()
        {
            settings.MapProvider = mapProvider.ToString();
            settings.Latitude = initialLatitude;
            settings.Longitude = initialLongitude;
            settings.Zoom = initialZoom;
            settings.Settlement = settlement;
            settings.AccessMode = accesmode.ToString();

            settings.Save();
        } /* SaveSettings */

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoadSettings();
            Application.Run(new MainForm());
            SaveSettings();
        }
    }



    public class Dummy
    {
    }

    class IpInfo
    {
        public string Ip;
        public int Port;
        public TcpState State;
        public string ProcessName;

        public string CountryName;
        public string RegionName;
        public string City;
        public double Latitude;
        public double Longitude;
        public DateTime CacheTime;

        public DateTime StatusTime;
        public bool TracePoint;
    }

    struct IpStatus
    {
        public string CountryName
        {
            get;
            set;
        }

        public int ConnectionsCount
        {
            get;
            set;
        }
    }

    class DescendingComparer : IComparer<IpStatus>
    {
        public bool SortOnlyCountryName = false;

        public int Compare(IpStatus x, IpStatus y)
        {
            int r = 0;

            if (!SortOnlyCountryName)
            {
                r = y.ConnectionsCount.CompareTo(x.ConnectionsCount);
            }

            if (r == 0)
            {
                return x.CountryName.CompareTo(y.CountryName);
            }

            return r;
        }
    }

    class TraceRoute
    {
        static readonly string Data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        static readonly byte[] DataBuffer;
        static readonly int timeout = 8888;

        static TraceRoute()
        {
            DataBuffer = Encoding.ASCII.GetBytes(Data);
        }

        public static List<PingReply> GetTraceRoute(string hostNameOrAddress)
        {
            var ret = GetTraceRoute(hostNameOrAddress, 1);

            return ret;
        }

        private static List<PingReply> GetTraceRoute(string hostNameOrAddress, int ttl)
        {
            var result = new List<PingReply>();

            using (var pinger = new Ping())
            {
                var pingerOptions = new PingOptions(ttl, true);

                var reply = pinger.Send(hostNameOrAddress, timeout, DataBuffer, pingerOptions);

                //Debug.WriteLine("GetTraceRoute[" + hostNameOrAddress + "]: " + reply.RoundtripTime + "ms " + reply.Address + " -> " + reply.Status);

                if (reply.Status == IPStatus.Success)
                {
                    result.Add(reply);
                }
                else if (reply.Status == IPStatus.TtlExpired)
                {
                    // add the currently returned address
                    result.Add(reply);

                    // recurse to get the next address...
                    result.AddRange(GetTraceRoute(hostNameOrAddress, ttl + 1));
                }
                else
                {
                    Debug.WriteLine("GetTraceRoute: " + hostNameOrAddress + " - " + reply.Status);
                }
            }

            return result;
        }
    }

#if !MONO

    #region Managed IP Helper API

    public struct TcpTable : IEnumerable<TcpRow>
    {
        #region Private Fields

        private IEnumerable<TcpRow> tcpRows;

        #endregion

        #region Constructors

        public TcpTable(IEnumerable<TcpRow> tcpRows)
        {
            this.tcpRows = tcpRows;
        }

        #endregion

        #region Public Properties

        public IEnumerable<TcpRow> Rows
        {
            get
            {
                return tcpRows;
            }
        }

        #endregion

        #region IEnumerable<TcpRow> Members

        public IEnumerator<TcpRow> GetEnumerator()
        {
            return tcpRows.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tcpRows.GetEnumerator();
        }

        #endregion
    }

    public struct TcpRow
    {
        #region Private Fields

        #endregion

        #region Constructors

        public TcpRow(IpHelper.TcpRow tcpRow)
        {
            State = tcpRow.state;
            ProcessId = tcpRow.owningPid;

            int localPort = (tcpRow.localPort1 << 8) + tcpRow.localPort2 + (tcpRow.localPort3 << 24) +
                            (tcpRow.localPort4 << 16);
            long localAddress = tcpRow.localAddr;
            LocalEndPoint = new IPEndPoint(localAddress, localPort);

            int remotePort = (tcpRow.remotePort1 << 8) + tcpRow.remotePort2 + (tcpRow.remotePort3 << 24) +
                             (tcpRow.remotePort4 << 16);
            long remoteAddress = tcpRow.remoteAddr;
            RemoteEndPoint = new IPEndPoint(remoteAddress, remotePort);
        }

        #endregion

        #region Public Properties

        public IPEndPoint LocalEndPoint { get; }

        public IPEndPoint RemoteEndPoint { get; }

        public TcpState State { get; }

        public int ProcessId { get; }

        #endregion
    }

    public static class ManagedIpHelper
    {
        public static readonly List<TcpRow> TcpRows = new List<TcpRow>();

        #region Public Methods

        public static void UpdateExtendedTcpTable(bool sorted)
        {
            TcpRows.Clear();

            var tcpTable = IntPtr.Zero;
            int tcpTableLength = 0;

            if (IpHelper.GetExtendedTcpTable(tcpTable,
                    ref tcpTableLength,
                    sorted,
                    IpHelper.AfInet,
                    IpHelper.TcpTableType.OwnerPidConnections,
                    0) != 0)
            {
                try
                {
                    tcpTable = Marshal.AllocHGlobal(tcpTableLength);
                    if (IpHelper.GetExtendedTcpTable(tcpTable,
                            ref tcpTableLength,
                            true,
                            IpHelper.AfInet,
                            IpHelper.TcpTableType.OwnerPidConnections,
                            0) == 0)
                    {
                        var table =
                            (IpHelper.TcpTable)Marshal.PtrToStructure(tcpTable, typeof(IpHelper.TcpTable));

                        var rowPtr = (IntPtr)((long)tcpTable + Marshal.SizeOf(table.Length));
                        for (int i = 0; i < table.Length; ++i)
                        {
                            TcpRows.Add(
                                new TcpRow((IpHelper.TcpRow)Marshal.PtrToStructure(rowPtr, typeof(IpHelper.TcpRow))));
                            rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(typeof(IpHelper.TcpRow)));
                        }
                    }
                }
                finally
                {
                    if (tcpTable != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(tcpTable);
                    }
                }
            }
        }

        #endregion
    }

    #endregion

    #region P/Invoke IP Helper API

    /// <summary>
    ///     <see cref="http://msdn2.microsoft.com/en-us/library/aa366073.aspx" />
    /// </summary>
    public static class IpHelper
    {
        #region Public Fields

        public const string DllName = "iphlpapi.dll";
        public const int AfInet = 2;

        #endregion

        #region Public Methods

        /// <summary>
        ///     <see cref="http://msdn2.microsoft.com/en-us/library/aa365928.aspx" />
        /// </summary>
        [DllImport(DllName, SetLastError = true)]
        public static extern uint GetExtendedTcpTable(IntPtr tcpTable, ref int tcpTableLength, bool sort, int ipVersion,
            TcpTableType tcpTableType, int reserved);

        #endregion

        #region Public Enums

        /// <summary>
        ///     <see cref="http://msdn2.microsoft.com/en-us/library/aa366386.aspx" />
        /// </summary>
        public enum TcpTableType
        {
            BasicListener,
            BasicConnections,
            BasicAll,
            OwnerPidListener,
            OwnerPidConnections,
            OwnerPidAll,
            OwnerModuleListener,
            OwnerModuleConnections,
            OwnerModuleAll,
        }

        #endregion

        #region Public Structs

        /// <summary>
        ///     <see cref="http://msdn2.microsoft.com/en-us/library/aa366921.aspx" />
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TcpTable
        {
            public uint Length;
            public TcpRow row;
        }

        /// <summary>
        ///     <see cref="http://msdn2.microsoft.com/en-us/library/aa366913.aspx" />
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TcpRow
        {
            public TcpState state;
            public uint localAddr;
            public byte localPort1;
            public byte localPort2;
            public byte localPort3;
            public byte localPort4;
            public uint remoteAddr;
            public byte remotePort1;
            public byte remotePort2;
            public byte remotePort3;
            public byte remotePort4;
            public int owningPid;
        }

        #endregion
    }

    #endregion

#endif
}
