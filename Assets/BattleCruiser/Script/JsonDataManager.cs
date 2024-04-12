using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using static JsonDataManager;

public class JsonDataManager : GlobalSingleton<JsonDataManager>
{
    private void Awake()
    {
        Debug.Log($"{Instance.name} �ν��Ͻ� �Ϸ�");
        DataLoad();
        TestInsert(true);

        DataSave();
    }
    void TestInsert(bool isTest)
    {
        if (!isTest)
            return;


        /*
        saveData.shipDataDictionary.Add("Ship_002", new ShipData("�۶��콺�� ���ʰ���", 360000, 3600, 50, 12, 20, 3, new List<string>()));
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_001");
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_001");
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_001");
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_001");

        saveData.weaponDataDictionary.Add("Weapon_001", new WeaponData("30mm �����", 0, 100, 3, 10, 30, 1, 1, 180, 0.1f));

        saveData.stageList.Add(new StageData(new List<string>()));
        saveData.stageList[2].stageShipDataList.Add("Ship_001");
        saveData.stageList[2].stageShipDataList.Add("Ship_001");
        saveData.stageList[2].stageShipDataList.Add("Ship_001");
        saveData.stageList[2].stageShipDataList.Add("Ship_001");
        */
    }
    
    string saveDataFileName = "/JsonData/SaveData.json";    
    string saveFolderPath = "/JsonData/";

    public class SaveData
    {
        public Dictionary<string, ShipData> shipDataDictionary = new Dictionary<string, ShipData>();
        public Dictionary<string, WeaponData> weaponDataDictionary = new Dictionary<string, WeaponData>();
        public List<StageData> stageList = new List<StageData>();
    }
    public SaveData saveData;

    public void DataSave()//Dictionary �����͸� json���� �����ϴ� �Լ�
    {
        string folderPath = Application.dataPath + saveFolderPath;
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var data = JsonConvert.SerializeObject(saveData, Formatting.Indented);

        File.WriteAllText(Application.dataPath + saveDataFileName, data);

        Debug.Log("������ ���� �Ϸ�");
    }
    public void DataLoad()//json�� Dictionary �����ͷ� ��ȯ�ϴ� �Լ�
    {
        if (!File.Exists(Application.dataPath + saveDataFileName))
            return;

        var fileData = File.ReadAllText(Application.dataPath + saveDataFileName);
        var data = JsonConvert.DeserializeObject<SaveData>(fileData);
        saveData = data;

        Debug.Log("������ �ҷ����� �Ϸ�");
    }

    //public void SetData(string id, ShipData targetValue)//Ư�� Ű�� ���� ������ �� ���
    //{
    //    // ���� ���� üũ
    //    if (shipDataDictionary.ContainsKey(id))//Ű�� ������
    //    {
    //        shipDataDictionary[id] = targetValue;
    //        DataSave();
    //    }
    //    else//Ű�� ����. ������ ��
    //    {
    //        Debug.LogWarning("Ű�� ���� �����ϴ�. ���� �����͸� �����մϴ�.");
    //        shipDataDictionary.Add(id, targetValue);
    //        DataSave();
    //    }        
    //}

    //public ShipData GetData(string id)//Ư�� Ű�� ���� �ҷ��� �� ���
    //{
    //    if (shipDataDictionary.ContainsKey(id))//Ű�� ������
    //    {
    //        return shipDataDictionary[id];
    //    }
    //    else//���� Ű��
    //    {
    //        Debug.LogError("Ű�� �������� �ʽ��ϴ�. ������ �ҷ����� ����");
    //        return null;
    //    }
    //}
}