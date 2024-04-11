using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : SceneSingleton<GameUI> 
{
    public TextMeshProUGUI kineticDmgText;
    public TextMeshProUGUI chemicalDmgText;

    private void Start()
    {
        DisableDmgText();
    }

    public void SetKineticDmgText(float dmg)
    {
        if (dmg != 0)
        {
            kineticDmgText.gameObject.SetActive(true);
            kineticDmgText.text = DmgToString(dmg);
        }
    }
    public void SetChemicalDmgText(float dmg)
    {
        if (dmg != 0)
        {
            chemicalDmgText.gameObject.SetActive(true);
            chemicalDmgText.text = DmgToString(dmg);
        }
    }
    string DmgToString(float dmg)
    {
        string prefix;

        if(dmg > 1000000000)
        {
            prefix = "T";
            dmg *= 0.000000001f;
        }
        else if (dmg > 1000000)
        {
            prefix = "M";
            dmg *= 0.000001f;
        }
        else if(dmg > 1000)
        {
            prefix = "K";
            dmg *= 0.001f;
        }
        else
        {
            prefix = "";
        }

        return string.Format($"{dmg:N2}{prefix}");
    }
    public void DisableDmgText()
    {
        kineticDmgText.gameObject.SetActive(false);
        chemicalDmgText.gameObject.SetActive(false);
    }    
}
