using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class JsonDataManager : MonoBehaviour
{
    static JsonDataManager instance;
    public static JsonDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (JsonDataManager)FindObjectOfType(typeof(JsonDataManager));
                DontDestroyOnLoad(instance.gameObject);

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    singletonObject.name = "JsonDataManager";
                    instance = singletonObject.AddComponent<JsonDataManager>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        Debug.Log($"{Instance.name} �ν��Ͻ� �Ϸ�");
        DataLoad();
        
        //List<WeaponData> weaponDatas = new List<WeaponData>();
        //weaponDatas.Add(new WeaponData(0, 100, 3, 30, 200, 1, 1, 180, 0.2f));
        //weaponDatas.Add(new WeaponData(0, 100, 3, 30, 200, 1, 1, 180, 0.2f));
        //weaponDatas.Add(new WeaponData(0, 100, 3, 30, 200, 1, 1, 180, 0.2f));
        //weaponDatas.Add(new WeaponData(0, 100, 3, 30, 200, 1, 1, 180, 0.2f));

        //saveData.shipDataDictionary.Add("Ship_002", new ShipData("�׽�Ʈ �ʰ��� 2", 1000000, 10000, 50, 10, 20, 3, weaponDatas));

        DataSave();
    }
    
    string saveDataFileName = "/JsonData/SaveData.json";    
    string saveFolderPath = "/JsonData/";

    public class SaveData
    {
        public Dictionary<string, ShipData> shipDataDictionary = new Dictionary<string, ShipData>();
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