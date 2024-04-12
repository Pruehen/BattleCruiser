using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GlobalSingleton<GameManager>
{
    int selectedStage;

    private void Awake()
    {
        Debug.Log($"{Instance.name} 인스턴싱 완료");
    }
    public void SetStageNum(int num)
    {
        selectedStage = num;
    }
    public void StageInit()
    {
        Transform spawnTrf = GameObject.Find("Vehicles").transform;
        Debug.Log(selectedStage);
    }


    float totalKineticDmg = 0;
    float totalChemicalDmg = 0;

    float dmgViewTime = 10;
    float dmgViewDelay = 0;

    bool isDmgView = false;

    private void Update()
    {        
        if(isDmgView)
        {
            dmgViewDelay += Time.deltaTime;
            if(dmgViewDelay >= dmgViewTime)
            {
                isDmgView = false;
                GameUI.Instance.DisableDmgText();

                totalKineticDmg = 0;
                totalChemicalDmg = 0;
            }
        }        
    }

    public void KineticDmgUp(float dmg)
    {        
        totalKineticDmg += dmg;
        dmgViewDelay = 0;
        isDmgView = true;        
        GameUI.Instance.SetKineticDmgText(totalKineticDmg);
    }
    public void ChemicalDmgUp(float dmg)
    {
        totalChemicalDmg += dmg;
        dmgViewDelay = 0;
        isDmgView = true;
        GameUI.Instance.SetChemicalDmgText(totalChemicalDmg);
    }
}
