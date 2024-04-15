using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonDataManager : GlobalSingleton<JsonDataManager>
{
    private void Awake()
    {
        Debug.Log($"{Instance.name} 인스턴싱 완료");
        DataLoad();        
        DataSave();
    }
    void NewDataInsert()
    {        
        saveData.shipDataDictionary.Add("Ship_001", new ShipData("글라디우스급 중초계함", 280000, 2800, 48, 30, 60, 3, new List<string>()));
        saveData.shipDataDictionary["Ship_001"].weaponDatas.Add("Weapon_001");
        saveData.shipDataDictionary["Ship_001"].weaponDatas.Add("Weapon_001");
        saveData.shipDataDictionary["Ship_001"].weaponDatas.Add("Weapon_001");
        saveData.shipDataDictionary["Ship_001"].weaponDatas.Add("Weapon_001");

        saveData.shipDataDictionary.Add("Ship_002", new ShipData("글라디우스급 중초계함", 360000, 3600, 50, 36, 60, 3, new List<string>()));
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_002");
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_002");
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_002");
        saveData.shipDataDictionary["Ship_002"].weaponDatas.Add("Weapon_002");

        saveData.weaponDataDictionary.Add("Weapon_001", new WeaponData("30mm 기관포", 100, 6, 10, 30, 1, 1, 180, 3, 20, 0.05f));
        saveData.weaponDataDictionary.Add("Weapon_002", new WeaponData("76mm 속사포", 130, 3, 15, 76, 1, 1, 90, 3, 3, 0.5f));

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

        try
        {
            var data = JsonConvert.DeserializeObject<SaveData>(fileData);
            saveData = data;
            if(saveData == null)
            {
                saveData = new SaveData();
                NewDataInsert();
                Debug.Log("새 저장 데이터 생성");
            }

            Debug.Log("데이터 불러오기 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"데이터 불러오기 실패 : {e.Message}");
            saveData = new SaveData();
            throw;
        }
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