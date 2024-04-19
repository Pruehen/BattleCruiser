using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : SceneSingleton<MainSceneManager>
{
    private void Awake()
    {
        Debug.Log($"{Instance.name} 로컬 인스턴싱 완료");
        SkyboxChanger.Instance.ChangeSkybox();
    }

    public void SetStageNum(int num)
    {
        GameManager.Instance.SetStageNum(num);
    }
    public void GameExit()
    {
        Debug.Log("게임을 종료합니다.");
        JsonDataManager.Instance.DataSave();
        Application.Quit();
    }
}
