using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : SceneSingleton<MainSceneManager>
{
    private void Awake()
    {
        Debug.Log($"{Instance.name} 로컬 인스턴싱 완료");
    }

    public void SetStageNum(int num)
    {
        GameManager.Instance.SetStageNum(num);
    }
}
