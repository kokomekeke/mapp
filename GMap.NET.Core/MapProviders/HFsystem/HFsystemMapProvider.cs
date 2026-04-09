using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using GMap.NET.Entity;
using GMap.NET.Internals;
using GMap.NET.Projections;
using Newtonsoft.Json;

namespace GMap.NET.MapProviders
{
    /// <summary>
    ///     OpenStreetMap provider - http://www.openstreetmap.org/
    /// </summary>
    public class HFsystemMapProvider : OpenStreetMapProviderBase
    {
        public static readonly HFsystemMapProvider Instance;

        HFsystemMapProvider()
        {
            LoadParameters("HFsystemMap.ini");
        }

        static HFsystemMapProvider()
        {
            Instance = new HFsystemMapProvider();
        }

        #region GMapProvider Members

        public override Guid Id
        {
            get;
        } = new Guid("5649E1B3-20E0-435D-9A25-0CD8B82DB2F1");


        private string _name = "HFsystemMap";

        public override string Name
        {
            get { return _name; }
        }// = "HFsystemMap";

        public string YoursClientName { get; set; }

        GMapProvider[] _overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (_overlays == null)
                {
                    _overlays = new GMapProvider[] { this };
                }

                return _overlays;
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom, string.Empty);

            return GetTileImageUsingHttp(url);
        }

        protected override void InitializeWebRequest(WebRequest request)
        {
            base.InitializeWebRequest(request);

            if (!string.IsNullOrEmpty(YoursClientName))
            {
                request.Headers.Add("X-Yours-client", YoursClientName);
            }
        }

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            char letter = ServerLetters[GetServerNum(pos, 3)];
            return string.Format(UrlFormat, letter, zoom, pos.X, pos.Y);
            //return string.Format(UrlFormat, zoom, pos.X, pos.Y);
        }

        //static readonly string UrlFormat = "https://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png";
        static public string UrlFormat
        {
            get;
            protected set;
        } = "https://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png";

        public void LoadParameters(string inifilename)
        {
            if (string.IsNullOrEmpty(inifilename))
            {
                return;
            }

            if (!File.Exists(inifilename))
            {
                return;
            }

            using (var reader = new StreamReader(inifilename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    if (line.StartsWith("#", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    var ls = line.Split(new char[] {'='},2);
                    if (ls.Length > 1)
                    {
                        switch (ls[0].Trim())
                        {
                            case "YoursClientName":
                                YoursClientName = ls[1].Trim();
                                break;
                            case "Name":
                                _name = ls[1].Trim();
                                break;
                            case "Copyright":
                                Copyright = ls[1].Trim();
                                break;
                            case "UrlFormat":
                                if (!string.IsNullOrEmpty(ls[1].Trim()))
                                    UrlFormat = ls[1].Trim();
                                break;
                            case "RefererUrl":
                                if (!string.IsNullOrEmpty(ls[1].Trim()))
                                RefererUrl = ls[1].Trim();
                            break;
                            case "ReverseGeocoderUrlFormat":
                                if (!string.IsNullOrEmpty(ls[1].Trim()))
                                    ReverseGeocoderUrlFormat = ls[1].Trim();
                                break;
                            case "GeocoderUrlFormat":
                                if (!string.IsNullOrEmpty(ls[1].Trim()))
                                    GeocoderUrlFormat = ls[1].Trim();
                                break;
                            case "GeocoderDetailedUrlFormat":
                                if (!string.IsNullOrEmpty(ls[1].Trim()))
                                    GeocoderDetailedUrlFormat = ls[1].Trim();
                                break;
                        } /* switch */
                    }
                } /* while */
            } /* using */
        }/* LoadParameters */

    } /* class HFsystemMapProvider */

} /* namespace GMap.NET.MapProviders */
