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
        Debug.Log($"{Instance.name} 인스턴싱 완료");
        DataLoad();
        
        //List<WeaponData> weaponDatas = new List<WeaponData>();
        //weaponDatas.Add(new WeaponData(0, 100, 3, 30, 200, 1, 1, 180, 0.2f));
        //weaponDatas.Add(new WeaponData(0, 100, 3, 30, 200, 1, 1, 180, 0.2f));
        //weaponDatas.Add(new WeaponData(0, 100, 3, 30, 200, 1, 1, 180, 0.2f));
        //weaponDatas.Add(new WeaponData(0, 100, 3, 30, 200, 1, 1, 180, 0.2f));

        //saveData.shipDataDictionary.Add("Ship_002", new ShipData("테스트 초계함 2", 1000000, 10000, 50, 10, 20, 3, weaponDatas));

        DataSave();
    }
    
    string saveDataFileName = "/JsonData/SaveData.json";    
    string saveFolderPath = "/JsonData/";

    public class SaveData
    {
        public Dictionary<string, ShipData> shipDataDictionary = new Dictionary<string, ShipData>();
    }
    public SaveData saveData;

    public void DataSave()//Dictionary 데이터를 json으로 저장하는 함수
    {
        string folderPath = Application.dataPath + saveFolderPath;
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var data = JsonConvert.SerializeObject(saveData, Formatting.Indented);

        File.WriteAllText(Application.dataPath + saveDataFileName, data);

        Debug.Log("데이터 저장 완료");
    }
    public void DataLoad()//json을 Dictionary 데이터로 변환하는 함수
    {
        if (!File.Exists(Application.dataPath + saveDataFileName))
            return;

        var fileData = File.ReadAllText(Application.dataPath + saveDataFileName);
        var data = JsonConvert.DeserializeObject<SaveData>(fileData);
        saveData = data;

        Debug.Log("데이터 불러오기 완료");
    }

    //public void SetData(string id, ShipData targetValue)//특정 키의 값을 저장할 때 사용
    //{
    //    // 존재 여부 체크
    //    if (shipDataDictionary.ContainsKey(id))//키가 존재함
    //    {
    //        shipDataDictionary[id] = targetValue;
    //        DataSave();
    //    }
    //    else//키가 없음. 만들어야 함
    //    {
    //        Debug.LogWarning("키의 값이 없습니다. 저장 데이터를 생성합니다.");
    //        shipDataDictionary.Add(id, targetValue);
    //        DataSave();
    //    }        
    //}

    //public ShipData GetData(string id)//특정 키의 값을 불러올 때 사용
    //{
    //    if (shipDataDictionary.ContainsKey(id))//키가 존재함
    //    {
    //        return shipDataDictionary[id];
    //    }
    //    else//없는 키임
    //    {
    //        Debug.LogError("키가 존재하지 않습니다. 데이터 불러오기 실패");
    //        return null;
    //    }
    //}
}