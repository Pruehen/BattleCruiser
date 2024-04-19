using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class GameUI : SceneSingleton<GameUI> 
{
    public TextMeshProUGUI kineticDmgText;
    public TextMeshProUGUI chemicalDmgText;

    public TextMeshProUGUI kineticDmgText_Enemy;
    public TextMeshProUGUI chemicalDmgText_Enemy;

    public GameObject resultWdw;
    public GameObject pauseWdw;
    bool isPause;

    private void Start()
    {
        DisableDmgText();
        isPause = false;
    }

    public void SetDmgText(float kntDmg, float expDmg)
    {
        if (kntDmg != 0)
        {
            kineticDmgText.gameObject.SetActive(true);
            kineticDmgText.text = DmgToString(kntDmg);
        }
        if(expDmg != 0)
        {
            chemicalDmgText.gameObject.SetActive(true);
            chemicalDmgText.text = DmgToString(expDmg);
        }
    }
    public void SetEnemyDmgText(float kntDmg, float expDmg)
    {
        if (kntDmg != 0)
        {
            kineticDmgText_Enemy.gameObject.SetActive(true);
            kineticDmgText_Enemy.text = DmgToString(kntDmg);
        }
        if (expDmg != 0)
        {
            chemicalDmgText_Enemy.gameObject.SetActive(true);
            chemicalDmgText_Enemy.text = DmgToString(expDmg);
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

        kineticDmgText_Enemy.gameObject.SetActive(false);
        chemicalDmgText_Enemy.gameObject.SetActive(false);
    }    

    public void OnResultWdw(bool isWin)
    {
        resultWdw.SetActive(true);
        resultWdw.gameObject.GetComponent<ResultWdw>().isWin = isWin;
    }

    public void PauseToggle()
    {
        isPause = !isPause;
        pauseWdw.SetActive(isPause);

        if(isPause)
        {
            BattleSceneManager.Instance.SetTimeScale(0);
        }
        else
        {
            BattleSceneManager.Instance.SetTimeScale(1);
        }
    }
}
