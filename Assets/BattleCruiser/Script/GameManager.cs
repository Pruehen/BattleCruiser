using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GlobalSingleton<GameManager>
{
    public int selectedStage { get; private set; }

    private void Awake()
    {
        Debug.Log($"{Instance.name} 인스턴싱 완료");
    }
    public void SetStageNum(int num)
    {
        selectedStage = num;
        Debug.Log($"{selectedStage} 스테이지로 설정");
    }
}
