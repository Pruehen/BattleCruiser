using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameIntroManager : MonoBehaviour
{
    public GameObject[] scripts;
    // Start is called before the first frame update
    void Start()
    {
        int delayTime = 1;
        foreach (GameObject script in scripts)
        {
            script.SetActive(false);           
            StartCoroutine(scriptsActivate(delayTime, script));
            delayTime += 2;
        }
    }

    IEnumerator scriptsActivate(float delayTime, GameObject item)
    {
        yield return new WaitForSeconds(delayTime);
        item.SetActive(true);
    }
}
