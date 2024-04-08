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
        DataListAdd();
        LoadData();
    }

    Dictionary<string, int> saveDataDic = new Dictionary<string, int>();
    string saveDataFileName = "/JsonData/SaveData.txt";
    string userDataFileName = "/JsonData/UserData.txt";
    string saveFolderPath = "/JsonData/";

    public void SaveData()//Dictionary �����͸� json���� �����ϴ� �Լ�
    {
        string folderPath = Application.dataPath + saveFolderPath;
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var data = JsonConvert.SerializeObject(saveDataDic, Formatting.Indented);

        File.WriteAllText(Application.dataPath + saveDataFileName, data);

        Debug.Log("������ ���� �Ϸ�");
    }
    public void LoadData()//json�� Dictionary �����ͷ� ��ȯ�ϴ� �Լ�
    {
        if (!File.Exists(Application.dataPath + saveDataFileName))
            return;

        var fileData = File.ReadAllText(Application.dataPath + saveDataFileName);
        var data = JsonConvert.DeserializeObject<Dictionary<string, int>>(fileData);
        saveDataDic = data;

        Debug.Log("������ �ҷ����� �Ϸ�");
    }

    public void SetData(string id, float targetValue)//Ư�� Ű�� ���� ������ �� ���
    {
        // ���� ���� üũ
        if (saveDataDic.ContainsKey(id))//Ű�� ������
        {
            saveDataDic[id] = (int)targetValue;
            SaveData();
        }
        else if (GetTryUpgradeList(id))//Ű�� ������ ������ ��
        {
            Debug.LogWarning("Ű�� ���� �����ϴ�. ���� �����͸� �����մϴ�.");
            saveDataDic.Add(id, (int)targetValue);
            SaveData();
        }
        else//���� Ű��
        {
            Debug.LogError("Ű�� �������� �ʽ��ϴ�. ������ ���� ����");
        }
    }

    public float GetData(string id)//Ư�� Ű�� ���� �ҷ��� �� ���
    {
        if (saveDataDic.ContainsKey(id))//Ű�� ������
        {
            return saveDataDic[id];
        }
        else if (GetTryUpgradeList(id))//Ű�� ������ ���� �� ����
        {
            Debug.LogWarning("Ű�� ���� �����ϴ�. ���� �����͸� �����մϴ�.");
            saveDataDic.Add(id, 0);
            SaveData();
            return saveDataDic[id];
        }
        else//���� Ű��
        {
            Debug.LogError("Ű�� �������� �ʽ��ϴ�. ������ �ҷ����� ����");
            return 0;
        }
    }

    void DataListAdd()//json �����ͷ� ������ Ű���� ������� �Լ�.
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

    List<string> DataList = new List<string>();//�Է¹��� Ű���� ����ص� �Ǵ� Ű������ Ȯ���ϴ� �뵵�� ����Ʈ

    bool GetTryUpgradeList(string findString)
    {
        return DataList.Contains(findString);
    }
}