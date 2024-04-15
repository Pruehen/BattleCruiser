using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RarityColor
{
    public static string commonCode = "#00FF00";
    public static string rareCode = "#007FFF";
    public static string epicCode = "#7F00FF";
    public static string legendaryCode = "#FFCC00";

    public static Color common = ConvertHexToColor(commonCode);
    public static Color rare = ConvertHexToColor(rareCode);
    public static Color epic = ConvertHexToColor(epicCode);
    public static Color legendary = ConvertHexToColor(legendaryCode);

    static Color ConvertHexToColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}