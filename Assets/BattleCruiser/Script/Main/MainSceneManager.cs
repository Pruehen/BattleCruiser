using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : SceneSingleton<MainSceneManager>
{
    private void Awake()
    {
        Debug.Log($"{Instance.name} ���� �ν��Ͻ� �Ϸ�");
        SkyboxChanger.Instance.ChangeSkybox();
    }

    public void SetStageNum(int num)
    {
        GameManager.Instance.SetStageNum(num);
    }
    public void GameExit()
    {
        Debug.Log("������ �����մϴ�.");
        JsonDataManager.Instance.DataSave();
        Application.Quit();
    }
}
