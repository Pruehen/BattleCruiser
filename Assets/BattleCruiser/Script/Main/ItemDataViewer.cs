using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemDataViewer : MonoBehaviour
{
    public TextMeshProUGUI dataText;

    public void SetText(string[] datas)
    {
        string data = "";

        foreach (string s in datas)
        {
            data += $"{s}\n";
        }
        dataText.text = data;
    }
}
