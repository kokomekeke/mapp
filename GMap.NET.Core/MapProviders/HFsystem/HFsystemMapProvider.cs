using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using GMap.NET.Entity;
using GMap.NET.Internals;
using GMap.NET.MapProviders.HFsystem;
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
            // Az INI-t az exe mappájából olvassuk, nem az aktuális munkakönyvtárból
            LoadParameters(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HFsystemMap.ini"));
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

        #region szerver paraméterek (HFsystemMap.ini)

        /// <summary>
        ///     http vagy https. INI kulcs: Protocol
        /// </summary>
        public string Protocol { get; set; } = "http";

        /// <summary>
        ///     A csempeszerver IP címe vagy gépneve. INI kulcs: ServerIp
        /// </summary>
        public string ServerIp { get; set; } = "localhost";

        /// <summary>
        ///     A csempeszerver portja; 0 = nincs megadva (http: 80, https: 443). INI kulcs: ServerPort
        /// </summary>
        public int ServerPort { get; set; } = 80;

        /// <summary>
        ///     A csempék útvonala a szerveren, a {z}/{x}/{y} előtt (pl. data/v3). INI kulcs: TilePath
        /// </summary>
        public string TilePath { get; set; } = "data/v3";

        /// <summary>
        ///     A csempék formátuma (fájlkiterjesztés): pbf / mvt = vektoros, a program rajzolja ki;
        ///     png / jpg = kész kép a szervertől. INI kulcs: TileFormat
        /// </summary>
        public string TileFormat { get; set; } = "pbf";

        /// <summary>
        ///     IP[:port] egyben (régi INI kulcs: ServerUrl)
        /// </summary>
        public string ServerUrl
        {
            get { return ServerPort > 0 ? ServerIp + ":" + ServerPort : ServerIp; }
            set
            {
                string v = (value ?? string.Empty).Trim();
                int colon = v.LastIndexOf(':');
                if (colon > 0 && int.TryParse(v.Substring(colon + 1), out int port))
                {
                    ServerIp = v.Substring(0, colon);
                    ServerPort = port;
                }
                else
                {
                    ServerIp = v;
                    ServerPort = 0;
                }
            }
        }

        /// <summary>
        ///     A ténylegesen használt URL minta: {0}=IP[:port], {1}=zoom, {2}=x, {3}=y.
        ///     Ha az INI-ben UrlFormat meg van adva, az felülírja a Protocol/TilePath/TileFormat alapján összeállítottat.
        /// </summary>
        public string EffectiveUrlFormat
        {
            get
            {
                if (!string.IsNullOrEmpty(UrlFormat))
                    return UrlFormat;

                string path = (TilePath ?? string.Empty).Trim().Trim('/');
                string ext = (TileFormat ?? "pbf").Trim().TrimStart('.');
                return Protocol + "://{0}" + (path.Length > 0 ? "/" + path : string.Empty) + "/{1}/{2}/{3}." + ext;
            }
        }

        #endregion

        #region címkeresés (geocoder) - csak erre a providerre vonatkozik

        /// <summary>
        ///     Címkeresés: HTTP GET erre a címre, {0}=keresett szöveg. Nominatim-kompatibilis JSON választ vár
        ///     ([{"lat":"..","lon":".."}, ...]). INI kulcs: GeocoderUrlFormat
        /// </summary>
        public string HFGeocoderUrlFormat { get; set; } = "http://localhost:537/search?q={0}&format=json";

        /// <summary>
        ///     INI kulcs: GeocoderDetailedUrlFormat
        /// </summary>
        public string HFGeocoderDetailedUrlFormat { get; set; } =
            "http://localhost:537/search?street={0}&city={1}&county={2}&state={3}&country={4}&postalcode={5}&format=json";

        /// <summary>
        ///     INI kulcs: ReverseGeocoderUrlFormat
        /// </summary>
        public string HFReverseGeocoderUrlFormat { get; set; } =
            "http://localhost:537/reverse?format=json&lat={0}&lon={1}&zoom=18&addressdetails=1";

        protected override string GeocoderUrl => HFGeocoderUrlFormat;
        protected override string GeocoderDetailedUrl => HFGeocoderDetailedUrlFormat;
        protected override string ReverseGeocoderUrl => HFReverseGeocoderUrlFormat;

        /// <summary>
        ///     A keresett szöveg az URL útvonalába kerül (pl. /settlement/{0}), ezért szabványos
        ///     URL-kódolás kell: "Szent Endre" → "Szent%20Endre", "Győr" → "Gy%C5%91r".
        /// </summary>
        protected override string EncodeGeocoderKeywords(string keywords)
        {
            // a SgxMapPanel a '#'-et előre %23-ra cseréli; visszaalakítjuk, hogy ne kódolódjon kétszer
            return Uri.EscapeDataString(keywords.Replace("%23", "#").Trim());
        }

        #endregion

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

        /// <summary>
        ///     A vektoros csempék legnagyobb zoomszintje a szerveren (OpenMapTiles: 14).
        ///     Efölött a szülőcsempét töltjük le és nagyítva rajzoljuk ki. INI kulcs: MaxVectorZoom
        /// </summary>
        public int MaxVectorZoom { get; set; } = 14;

        /// <summary>
        ///     true, ha az URL vektoros (.pbf / .mvt) csempére mutat
        /// </summary>
        public bool IsVectorTileUrl
        {
            get
            {
                string u = EffectiveUrlFormat;
                int q = u.IndexOf('?');
                if (q >= 0) u = u.Substring(0, q);
                return u.EndsWith(".pbf", StringComparison.OrdinalIgnoreCase) ||
                       u.EndsWith(".mvt", StringComparison.OrdinalIgnoreCase);
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            if (IsVectorTileUrl)
            {
                return GetVectorTileImage(pos, zoom);
            }

            string url = MakeTileImageUrl(pos, zoom, string.Empty);

            return GetTileImageUsingHttp(url);
        }

        /// <summary>
        ///     .pbf letöltése, kirajzolása PNG-be, majd PureImage-dzsé alakítása.
        ///     A PNG bájtok kerülnek a GMap gyorsítótárába is.
        /// </summary>
        PureImage GetVectorTileImage(GPoint pos, int zoom)
        {
            int overzoom = Math.Max(0, zoom - MaxVectorZoom);
            var src = new GPoint(pos.X >> overzoom, pos.Y >> overzoom);
            long subX = pos.X - (src.X << overzoom);
            long subY = pos.Y - (src.Y << overzoom);

            string url = MakeTileImageUrl(src, zoom - overzoom, string.Empty);
            byte[] pbf = GetTileDataUsingHttp(url);

            byte[] png = VectorTileRenderer.RenderToPng(pbf, zoom, overzoom, subX, subY);

            // publiclyVisible: true kell, mert a GMaps a gyorsítótárazáshoz Data.GetBuffer()-t hív,
            // ami a sima new MemoryStream(byte[]) esetén UnauthorizedAccessException-t dob
            var data = new MemoryStream(png, 0, png.Length, false, true);
            var ret = TileImageProxy.FromStream(data);
            if (ret != null)
            {
                data.Position = 0;
                ret.Data = data;
            }
            else
            {
                data.Dispose();
            }

            return ret;
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
            return string.Format(EffectiveUrlFormat, ServerUrl, zoom, pos.X, pos.Y);
        }

        /// <summary>
        ///     Opcionális teljes URL minta; ha üres, a Protocol/ServerIp/ServerPort/TilePath/TileFormat alapján áll össze
        /// </summary>
        static public string UrlFormat
        {
            get;
            protected set;
        } = null;

        //static readonly string UrlFormat = "https://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png";
        
        //static public string UrlFormat
        //{
            //get;
            //protected set;
        //} = "https://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png";

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
                            case "ServerUrl":
                                if (!string.IsNullOrEmpty(ls[1].Trim()))
                                    ServerUrl = ls[1].Trim();
                                break;
                            case "Protocol":
                                if (!string.IsNullOrEmpty(ls[1].Trim()))
                                    Protocol = ls[1].Trim().ToLowerInvariant();
                                break;
                            case "ServerIp":
                                if (!string.IsNullOrEmpty(ls[1].Trim()))
                                    ServerIp = ls[1].Trim();
                                break;
                            case "ServerPort":
                                if (string.IsNullOrEmpty(ls[1].Trim()))
                                    ServerPort = 0;
                                else if (int.TryParse(ls[1].Trim(), out int port) && port >= 0 && port <= 65535)
                                    ServerPort = port;
                                break;
                            case "TilePath":
                                TilePath = ls[1].Trim();
                                break;
                            case "TileFormat":
                                if (!string.IsNullOrEmpty(ls[1].Trim()))
                                    TileFormat = ls[1].Trim();
                                break;
                            case "MaxVectorZoom":
                                if (int.TryParse(ls[1].Trim(), out int mvz) && mvz >= 0)
                                    MaxVectorZoom = mvz;
                                break;
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
                                    HFReverseGeocoderUrlFormat = ls[1].Trim();
                                break;
                            case "GeocoderUrlFormat":
                                if (!string.IsNullOrEmpty(ls[1].Trim()))
                                    HFGeocoderUrlFormat = ls[1].Trim();
                                break;
                            case "GeocoderDetailedUrlFormat":
                                if (!string.IsNullOrEmpty(ls[1].Trim()))
                                    HFGeocoderDetailedUrlFormat = ls[1].Trim();
                                break;
                        } /* switch */
                    }
                } /* while */
            } /* using */
        }/* LoadParameters */

    } /* class HFsystemMapProvider */

} /* namespace GMap.NET.MapProviders */
