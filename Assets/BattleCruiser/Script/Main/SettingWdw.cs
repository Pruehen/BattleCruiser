using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingWdw : MonoBehaviour
{
    public Slider bgm;
    public Slider sfx;
    public Slider radarRange;
    public Slider wheelSens;
    public Slider camSpeed;
    public Slider camRange;

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
    }
    public void UpdataData()
    {
        Setting setting = new Setting(bgm.value, sfx.value, radarRange.value, wheelSens.value, camSpeed.value, camRange.value);
        JsonDataManager.Instance.saveData.userData.setting = setting;
        JsonDataManager.Instance.DataSave();
        GameManager.Instance.Setting = setting;
    }
}
