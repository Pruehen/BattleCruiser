using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultWdw : MonoBehaviour
{
    public GameObject gameEndText;
    public GameObject winText;
    public GameObject defeatText;
    public GameObject rewardWdw;
    public GameObject mainBtn;

    public bool isWin = false;

    private void OnEnable()
    {
        gameEndText.SetActive(false);
        winText.SetActive(false);
        defeatText.SetActive(false);
        rewardWdw.SetActive(false);
        mainBtn.SetActive(false);

        StartCoroutine(ResultWdwInit());
    }

    IEnumerator ResultWdwInit()
    {
        JsonDataManager.Instance.saveData.userData.customWeaponDatas.Add(new CustomWeaponData("Weapon_002", 1, 1, 0, Random.Range(0.9f, 1.1f)));
        JsonDataManager.Instance.DataSave();

        yield return new WaitForSecondsRealtime(1);
        gameEndText.SetActive(true);

        yield return new WaitForSecondsRealtime(1);
        if(isWin)
        {
            winText.SetActive(true);
        }
        else
        {
            defeatText.SetActive(true);
        }

        yield return new WaitForSecondsRealtime(1);
        rewardWdw.SetActive(true);

        yield return new WaitForSecondsRealtime(1);
        mainBtn.SetActive(true);
    }
}
