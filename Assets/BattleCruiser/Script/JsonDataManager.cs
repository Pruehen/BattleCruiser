using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class JsonDataManager : GlobalSingleton<JsonDataManager>
{
    private void Awake()
    {
        Debug.Log($"{Instance.name} �ν��Ͻ� �Ϸ�");
        DataLoad();
        TestInsert(false);

        DataSave();
    }
    void TestInsert(bool isTest)
    {
        if (!isTest)
            return;

        saveData.stageList = new List<StageData>();
        saveData.stageList.Add(new StageData(new List<string>()));
        saveData.stageList[0].stageShipDataList.Add("Ship_001");
        saveData.stageList[0].stageShipDataList.Add("Ship_001");
    }
    
    string saveDataFileName = "/JsonData/SaveData.json";    
    string saveFolderPath = "/JsonData/";

    public class SaveData
    {
        public Dictionary<string, ShipData> shipDataDictionary;
        public List<StageData> stageList;

        public SaveData(Dictionary<string, ShipData> shipDataDictionary, List<StageData> stageList)
        {
            this.shipDataDictionary = shipDataDictionary;
            this.stageList = stageList;
        }
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