/*=============================================================================
*  Program: Main entry point for SgxMapDemo application.
*
* (c) Kótai 2026.
*
* ver 1.00                                                         31-Aug-2026.
=============================================================================*/
using GMap.NET;
using GMap.NET.MapProviders;

namespace SgxMapDemo
{
    public static class Program
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

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            GMapProvider.UserAgent = "SgxMapDemo/1.0.26.831 (Windows; SAGAX Communications Ltd.)";
            LoadSettings();
            Application.Run(new MainForm());
            SaveSettings();
        }

        static void LoadSettings()
        {
            settings.Reload();

            initialLatitude = settings.Latitude;
            initialLongitude = settings.Longitude;
            initialZoom = settings.Zoom;
            settlement = settings.Settlement;
            pingaddress = settings.Ping;

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
                foreach (var m in GMapProviders.List)
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

    }
}
