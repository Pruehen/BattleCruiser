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
        DataListAdd();
        LoadData();
    }

    Dictionary<string, int> saveDataDic = new Dictionary<string, int>();
    string saveDataFileName = "/JsonData/SaveData.txt";
    string userDataFileName = "/JsonData/UserData.txt";
    string saveFolderPath = "/JsonData/";

    public void SaveData()//Dictionary 데이터를 json으로 저장하는 함수
    {
        string folderPath = Application.dataPath + saveFolderPath;
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var data = JsonConvert.SerializeObject(saveDataDic, Formatting.Indented);

        File.WriteAllText(Application.dataPath + saveDataFileName, data);

        Debug.Log("데이터 저장 완료");
    }
    public void LoadData()//json을 Dictionary 데이터로 변환하는 함수
    {
        if (!File.Exists(Application.dataPath + saveDataFileName))
            return;

        var fileData = File.ReadAllText(Application.dataPath + saveDataFileName);
        var data = JsonConvert.DeserializeObject<Dictionary<string, int>>(fileData);
        saveDataDic = data;

        Debug.Log("데이터 불러오기 완료");
    }

    public void SetData(string id, float targetValue)//특정 키의 값을 저장할 때 사용
    {
        // 존재 여부 체크
        if (saveDataDic.ContainsKey(id))//키가 존재함
        {
            saveDataDic[id] = (int)targetValue;
            SaveData();
        }
        else if (GetTryUpgradeList(id))//키가 없으나 만들어야 함
        {
            Debug.LogWarning("키의 값이 없습니다. 저장 데이터를 생성합니다.");
            saveDataDic.Add(id, (int)targetValue);
            SaveData();
        }
        else//없는 키임
        {
            Debug.LogError("키가 존재하지 않습니다. 데이터 저장 실패");
        }
    }

    public float GetData(string id)//특정 키의 값을 불러올 때 사용
    {
        if (saveDataDic.ContainsKey(id))//키가 존재함
        {
            return saveDataDic[id];
        }
        else if (GetTryUpgradeList(id))//키가 없으나 만들 수 있음
        {
            Debug.LogWarning("키의 값이 없습니다. 저장 데이터를 생성합니다.");
            saveDataDic.Add(id, 0);
            SaveData();
            return saveDataDic[id];
        }
        else//없는 키임
        {
            Debug.LogError("키가 존재하지 않습니다. 데이터 불러오기 실패");
            return 0;
        }
    }

    void DataListAdd()//json 데이터로 저장할 키값을 적어놓는 함수.
    {
        DataList.Add("BioCatalog");
        DataList.Add("LastClearStage");

        DataList.Add("Research_NeutrophilCreate");
        DataList.Add("Research_MacrophageCreate");
        DataList.Add("Research_DendriticCellCreate");
        DataList.Add("Research_HelperTCellCreate");
        DataList.Add("Research_KillerTCellCreate");
        DataList.Add("Research_BCellCreate");

        DataList.Add("Research_ToxicNeutrophils");
        DataList.Add("Research_CytokineRelease");
        DataList.Add("Research_EnhancedCytokineRelease");
        DataList.Add("Research_FusingPhagocytosis");
        DataList.Add("Research_ProfessionalAntigenPresentation");
        DataList.Add("Research_GameSpeedMax2");
        DataList.Add("Research_GameSpeedMax2.5");
        DataList.Add("Research_GameSpeedMax3");

        DataList.Add("Enhance_Ap");
        DataList.Add("Enhance_AttackDelay");
        DataList.Add("Enhance_Hp");
        DataList.Add("Enhance_CreateTime");
        DataList.Add("Enhance_BioDataFactor");
        DataList.Add("Enhance_BioCatalogFactor");
    }

    List<string> DataList = new List<string>();//입력받은 키값이 사용해도 되는 키값인지 확인하는 용도의 리스트

    bool GetTryUpgradeList(string findString)
    {
        return DataList.Contains(findString);
    }
}