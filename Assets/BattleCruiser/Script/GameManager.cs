using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GlobalSingleton<GameManager>
{
    public int selectedStage { get; private set; }

    private void Awake()
    {
        Debug.Log($"{Instance.name} �ν��Ͻ� �Ϸ�");
    }
    public void SetStageNum(int num)
    {
        selectedStage = num;
        Debug.Log($"{selectedStage} ���������� ����");
    }
}
