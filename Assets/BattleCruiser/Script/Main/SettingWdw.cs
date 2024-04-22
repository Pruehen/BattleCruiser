using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SettingWdw : MonoBehaviour
{
    public Slider bgm;
    public Slider sfx;
    public Slider radarRange;
    public Slider wheelSens;
    public Slider camSpeed;
    public Slider camRange;
    int difficulty = 0;

    public TextMeshProUGUI difficultyText;

    public void LoadData()
    {
        Setting setting = JsonDataManager.Instance.saveData.userData.setting;
        GameManager.Instance.Setting = setting;
        Debug.Log("데이터 로드");
        bgm.value = setting.bgm;
        sfx.value = setting.sfx;
        radarRange.value = setting.radarRange;
        wheelSens.value = setting.wheelSens;
        camSpeed.value = setting.camSpeed;
        camRange.value = setting.camRange;
        difficulty = setting.difficulty;
        SetDifficultyText(difficulty);
    }
    public void UpdataData()
    {
        Setting setting = new Setting(bgm.value, sfx.value, radarRange.value, wheelSens.value, camSpeed.value, camRange.value, difficulty);
        JsonDataManager.Instance.saveData.userData.setting = setting;
        JsonDataManager.Instance.DataSave();
        GameManager.Instance.Setting = setting;
    }

    public void DifficultyChange(int value)
    {
        difficulty += value;
        difficulty = Mathf.Clamp(difficulty, 0, 3);

        SetDifficultyText(difficulty);
    }

    void SetDifficultyText(int difficulty)
    {
        switch (difficulty)
        {
            case 0:
                difficultyText.text = "<color=green>EASY</color>";
                break;
            case 1:
                difficultyText.text = "<color=yellow>NORMAL</color>";
                break;
            case 2:
                difficultyText.text = "<color=red>HARD</color>";
                break;
            case 3:
                difficultyText.text = "<color=#FF00FF>INSANE</color>";
                break;
            default:
                break;
        }
    }
}
