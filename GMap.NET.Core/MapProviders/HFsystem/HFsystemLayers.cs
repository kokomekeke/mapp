using System;
using System.Collections.Generic;

namespace GMap.NET.MapProviders.HFsystem
{
    /// <summary>
    ///     A HFsystemMap vektoros térképének be-/kikapcsolható rétegei.
    ///     Egy réteg az OpenMapTiles séma egy vagy több rétegét fedi le.
    /// </summary>
    [Flags]
    public enum HFsystemLayers
    {
        None = 0,

        // --- területek ---
        /// <summary>water (tavak, folyók felülete), waterway (patakok, folyók vonala)</summary>
        Water = 1 << 0,
        /// <summary>landcover (erdő, rét, homok...), park</summary>
        Vegetation = 1 << 1,
        /// <summary>landuse (lakóterület, ipari, temető, sportpálya...)</summary>
        Landuse = 1 << 2,
        /// <summary>building</summary>
        Buildings = 1 << 3,

        // --- vonalak ---
        /// <summary>transportation: autópálya ... kiszolgáló út</summary>
        Roads = 1 << 4,
        /// <summary>transportation: földút, gyalogút</summary>
        Paths = 1 << 5,
        /// <summary>transportation: vasút, villamos, metró</summary>
        Railways = 1 << 6,
        /// <summary>boundary: ország- és megyehatár</summary>
        Boundaries = 1 << 7,

        // --- feliratok ---
        /// <summary>place: ország, város, falu, városrész nevek</summary>
        PlaceNames = 1 << 8,
        /// <summary>transportation_name: utcanevek</summary>
        StreetNames = 1 << 9,
        /// <summary>water_name: tavak, folyók nevei</summary>
        WaterNames = 1 << 10,
        /// <summary>housenumber: házszámok (17-es zoomtól)</summary>
        HouseNumbers = 1 << 11,

        All = Water | Vegetation | Landuse | Buildings | Roads | Paths | Railways | Boundaries |
              PlaceNames | StreetNames | WaterNames | HouseNumbers
    }

    public static class HFsystemLayerInfo
    {
        /// <summary>
        ///     A rétegek a menüben megjelenő sorrendben, magyar névvel és csoporttal.
        ///     Az INI kulcs: "Layer." + a réteg neve, pl. Layer.StreetNames=0
        /// </summary>
        public static readonly IList<(HFsystemLayers Layer, string Title, string Group)> Items =
            new List<(HFsystemLayers, string, string)>
            {
                (HFsystemLayers.Water, "Vizek", "Területek"),
                (HFsystemLayers.Vegetation, "Növényzet (erdő, rét, park)", "Területek"),
                (HFsystemLayers.Landuse, "Területhasználat (lakó, ipari...)", "Területek"),
                (HFsystemLayers.Buildings, "Épületek", "Területek"),

                (HFsystemLayers.Roads, "Utak, utcák", "Vonalak"),
                (HFsystemLayers.Paths, "Földutak, gyalogutak", "Vonalak"),
                (HFsystemLayers.Railways, "Vasút, villamos", "Vonalak"),
                (HFsystemLayers.Boundaries, "Közigazgatási határok", "Vonalak"),

                (HFsystemLayers.PlaceNames, "Településnevek", "Feliratok"),
                (HFsystemLayers.StreetNames, "Utcanevek", "Feliratok"),
                (HFsystemLayers.WaterNames, "Vizek nevei", "Feliratok"),
                (HFsystemLayers.HouseNumbers, "Házszámok", "Feliratok"),
            };
    }
}
