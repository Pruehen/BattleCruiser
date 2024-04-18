using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RarityColor
{
    public static string tech0_Code = "#d1d1d1";
    public static string tech1_Code = "#d9ca27";
    public static string tech2_Code = "#89de2f";
    public static string tech3_Code = "#1aba5d";
    public static string tech4_Code = "#1ab2ba";
    public static string tech5_Code = "#864ce6";
    public static string tech6_Code = "#5e0080";
    public static string tech7_Code = "#e34c00";

    public static Color tech0 = ConvertHexToColor(tech0_Code);
    public static Color tech1 = ConvertHexToColor(tech1_Code);
    public static Color tech2 = ConvertHexToColor(tech2_Code);
    public static Color tech3 = ConvertHexToColor(tech3_Code);
    public static Color tech4 = ConvertHexToColor(tech4_Code);
    public static Color tech5 = ConvertHexToColor(tech5_Code);
    public static Color tech6 = ConvertHexToColor(tech6_Code);
    public static Color tech7 = ConvertHexToColor(tech7_Code);

    static Color ConvertHexToColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}