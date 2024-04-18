using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultWdw : MonoBehaviour
{
    public GameObject gameEndText;
    public GameObject winText;
    public GameObject defeatText;
    public GameObject rewardWdw;
    public RectTransform rewardItemTrf;
    public GameObject mainBtn;

    public bool isWin = false;
    int itemCount = 0;

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
        yield return new WaitForSecondsRealtime(1);
        gameEndText.SetActive(true);

        yield return new WaitForSecondsRealtime(1);
        if (isWin)
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

    public void AddRewardItem(string weaponKey, Color rarityColor)
    {
        GameObject item = new GameObject();
        item.transform.SetParent(rewardItemTrf);
        item.transform.localPosition = Vector3.zero;
        item.transform.position += new Vector3((itemCount % 8) * 120, (itemCount / 8) * -120);

        int spriteIndex = JsonDataManager.Instance.saveData.weaponDataDictionary[weaponKey].sptiteIndex;
        Instantiate(PrefabManager.Instance.weapons_Image[spriteIndex], item.transform.position, Quaternion.identity, item.transform);
        Instantiate(PrefabManager.Instance.rarityCurver, item.transform.position, Quaternion.identity, item.transform).GetComponent<Image>().color = rarityColor;

        itemCount++;
    }
}
