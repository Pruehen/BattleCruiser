using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonDataManager : GlobalSingleton<JsonDataManager>
{
    private void Awake()
    {
        Debug.Log($"{Instance.name} �ν��Ͻ� �Ϸ�");
        DataLoad();        
        DataSave();
    }
    void NewDataInsert()
    {        
        saveData.shipDataDictionary.Add("Ship_001", new ShipData("�۶��콺�� ���ʰ���", 280000, 2800, 48, 30, 60, 3, new List<string>()));
        saveData.shipDataDictionary["Ship_001"].weaponDatas.Add("Weapon_001");
        saveData.shipDataDictionary["Ship_001"].weaponDatas.Add("Weapon_001");
        saveData.shipDataDictionary["Ship_001"].weaponDatas.Add("Weapon_001");
        saveData.shipDataDictionary["Ship_001"].weaponDatas.Add("Weapon_001");

        saveData.shipDataDictionary.Add("Ship_002", new ShipData("�۶��콺�� ���ʰ���", 360000, 3600, 50, 36, 60, 3, new List<string>()));
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_002");
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_002");
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_002");
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_002");

        saveData.weaponDataDictionary.Add("Weapon_001", new WeaponData("30mm �����", 100, 6, 10, 30, 1, 1, 180, 3, 20, 0.05f));
        saveData.weaponDataDictionary.Add("Weapon_002", new WeaponData("76mm �ӻ���", 130, 3, 15, 76, 1, 1, 90, 3, 3, 0.5f));

        saveData.stageList.Add(new StageData(new List<string>()));
        saveData.stageList[0].stageShipDataList.Add("Ship_001");
        saveData.stageList[0].stageShipDataList.Add("Ship_001");
        saveData.stageList.Add(new StageData(new List<string>()));
        saveData.stageList[1].stageShipDataList.Add("Ship_001");
        saveData.stageList[1].stageShipDataList.Add("Ship_001");
        saveData.stageList[1].stageShipDataList.Add("Ship_001");
        saveData.stageList[1].stageShipDataList.Add("Ship_001");
    }

    string saveDataFileName = "/JsonData/SaveData.json";    
    string saveFolderPath = "/JsonData/";

    public class SaveData
    {
        public Dictionary<string, ShipData> shipDataDictionary = new Dictionary<string, ShipData>();
        public Dictionary<string, WeaponData> weaponDataDictionary = new Dictionary<string, WeaponData>();
        public List<StageData> stageList = new List<StageData>();
        public UserData userData = new UserData();
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

        try
        {
            var data = JsonConvert.DeserializeObject<SaveData>(fileData);
            saveData = data;
            if(saveData == null)
            {
                saveData = new SaveData();
                NewDataInsert();
                Debug.Log("�� ���� ������ ����");
            }

            Debug.Log("������ �ҷ����� �Ϸ�");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"������ �ҷ����� ���� : {e.Message}");
            saveData = new SaveData();
            throw;
        }
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