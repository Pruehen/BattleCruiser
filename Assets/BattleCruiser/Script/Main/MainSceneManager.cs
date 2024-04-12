using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : SceneSingleton<MainSceneManager>
{
    private void Awake()
    {
        Debug.Log($"{Instance.name} ���� �ν��Ͻ� �Ϸ�");
    }

    public void SetStageNum(int num)
    {
        GameManager.Instance.SetStageNum(num);
    }
}
